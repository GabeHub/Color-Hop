using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using GoogleMobileAds.Api;

#if UNITY_ANDROID
using Firebase;
using Firebase.Analytics;
#endif

public class UIManager : MonoBehaviour
{


	public GameObject UICanvas, MainScreen, BallScreen, IAPBannerScreen, GiftScreen, WheelOfFourtneScreen, LevelUpScreen, GameOverScreen, LevelScreen, GamePlayScreen;
	public GameObject ads;
	public Material BallMaterial = null;
	public Image CurrentBallImage = null;
	public GameObject[] Balls;

	//MAIN SCREEN
	public Text txtMainBest, txtMainBest2, txtMainGems;

	//BALL SCREEN
	public Text txtBallGems;

	//	Levels Up Screen and LevelScreen
	public Text txtFinishedLevel, txtNextLevel, txtLevelScreenCurrentLevel;
	public List<GameObject> NewLevelList;

	//Gift Screen
	public Text txtPlus, txtGiftGems, txtGemsInfo;
	public GameObject[] gifts;
	public GameObject giftImage;


	public Camera mainCamera;
	public GameObject GameBG;
	public GameObject DayNight, GameMode, GameAreaScreen, UiScreen;
	public Button btnPlay;

	public Dictionary<int,string> kProductIDLevels;
	public DataService ds;

	//Game Levels
	public GameObject LevelContainer, LevelPrefab;

	//SettingScreen
	public Sprite VolumeOff, VolumeOn;
	public Image ImgVolume;
	public Text txtSession;

	//Instruction Screen
	public GameObject InstructionScreen, InGems, InTime, InWord, InTiles, InPoints;
	public Button InBtnPlay;
	public Text TxtInstruction;

	public bool isColorTarget = false;
	public Color mycolor;




	void Start ()
	{
        ds = new DataService ("LevelsDatabase.db");
		ShowMainScreen ();
		GameData.CurrentGameMode = 1;
		BallMaterial.mainTexture = Resources.Load ("BallsImages/ball", typeof(Texture)) as Texture;
		callGameMode ();
	}


	public void UIController (bool isUIActive = true, bool isMainScreenActive = false, bool isBallScreenActive = false, bool isIAPBannerScreenActive = false,
	                          bool isGiftScreenActive = false, bool isWheelOfFourtneScreenActive = false, bool isLevelUpScreenActive = false, bool isGameOverScreenActive = false, bool isLevelScreenActive = false, bool isGamePlayScreenActive = false)
	{
		txtGiftGems.text = Util.GetDiamond ().ToString ();
		txtMainBest2.text = Util.GetBestScore ().ToString ();
		txtMainGems.text = Util.GetDiamond ().ToString ();
		txtBallGems.text = Util.GetDiamond ().ToString ();


		if (isUIActive)
			mainCamera.gameObject.SetActive (true);
		
		UICanvas.SetActive (isUIActive);
		MainScreen.SetActive (isMainScreenActive);
		BallScreen.SetActive (isBallScreenActive);
		if (isBallScreenActive)
			BallLockedButton ();
		IAPBannerScreen.SetActive (isIAPBannerScreenActive);
		GiftScreen.SetActive (isGiftScreenActive);
		if (isGiftScreenActive) {
			ActiveGift ();
		}
		WheelOfFourtneScreen.SetActive (isWheelOfFourtneScreenActive);

		
		LevelUpScreen.SetActive (isLevelUpScreenActive);



		GameOverScreen.SetActive (isGameOverScreenActive);


		LevelScreen.SetActive (isLevelScreenActive);
		GamePlayScreen.SetActive (isGamePlayScreenActive);
	}

	public void callDayNight ()
	{
		//FireBase 
		#if UNITY_ANDROID
		FirebaseAnalytics.SetCurrentScreen ("Day_Night_Button", this.name);
		#endif
		Debug.Log ("IS DAY :" + GameData.isDay.ToString ());
		if (GameData.isDay) {
			GameData.isDay = false;
			//Day
			DayNight.GetComponentsInChildren<Image> () [1].sprite = Resources.Load ("sun", typeof(Sprite)) as Sprite;
			mainCamera.backgroundColor = Color.black;

			//Main Screen
			txtMainBest.color = Color.white;
			txtMainBest2.color = Color.white;
			txtMainGems.color = Color.white;
			//Ball Screen
			txtBallGems.color = new Color32 (0, 191, 255, 255);
			//Levels Screen
//			imgLevelsContaimer.color = new Color32 (7, 64, 103, 255);
			//GiftScreen
			txtPlus.color = Color.white;
			txtGiftGems.color = Color.yellow;
			txtGemsInfo.color = Color.white;

			MainScreen.GetComponent<Image> ().color = Color.black;
			BallScreen.GetComponent<Image> ().color = Color.black;
			IAPBannerScreen.GetComponentsInChildren<Image> () [1].color = Color.black;
			GiftScreen.GetComponentsInChildren<Image> () [1].color = Color.black;
			WheelOfFourtneScreen.GetComponentsInChildren<Image> () [1].color = Color.black;
			LevelUpScreen.GetComponentsInChildren<Image> () [1].color = Color.black;
			LevelScreen.GetComponent<Image> ().color = Color.black;

		} else {
			GameData.isDay = true;

			//Night
			DayNight.GetComponentsInChildren<Image> () [1].sprite = Resources.Load ("moon", typeof(Sprite)) as Sprite;
			//Main Screen
			txtMainBest.color = new Color32 (0, 131, 91, 255);
			txtMainBest2.color = new Color32 (0, 131, 91, 255);
			txtMainGems.color = new Color32 (0, 131, 91, 255);
			//Ball Screen
			txtBallGems.color = new Color32 (0, 191, 255, 255);
			//Levels Screen
//			imgLevelsContaimer.color = Color.white;
			//GiftScreen
			txtPlus.color = Color.black;
			txtGiftGems.color = new Color32 (0, 89, 26, 255);

			txtGemsInfo.color = new Color32 (116, 98, 159, 255);

			mainCamera.backgroundColor = Color.white;
			MainScreen.GetComponent<Image> ().color = Color.white;
			BallScreen.GetComponent<Image> ().color = Color.white;
			IAPBannerScreen.GetComponentsInChildren<Image> () [1].color = Color.white;
			GiftScreen.GetComponentsInChildren<Image> () [1].color = Color.white;
			WheelOfFourtneScreen.GetComponentsInChildren<Image> () [1].color = Color.white;
			LevelUpScreen.GetComponentsInChildren<Image> () [1].color = Color.white;
			LevelScreen.GetComponent<Image> ().color = Color.white;
		}
	}

	public void callGameMode ()
	{
		#if UNITY_ANDROID
		//FireBase 
		FirebaseAnalytics.SetCurrentScreen ("GameMode_Button", this.name);
		#endif
		if (GameData.CurrentGameMode == 0) {
			//Endless
			GameMode.GetComponentsInChildren<Text> () [0].enabled = false;
			GameMode.GetComponentsInChildren<Text> () [1].enabled = true;
			GameMode.GetComponent<Image> ().sprite = Resources.Load ("Levels", typeof(Sprite)) as Sprite;

			//SetButton Property for Play Button
			btnPlay.onClick.RemoveAllListeners ();
			btnPlay.onClick.AddListener (OpenLevelList);
			btnPlay.GetComponentInChildren<Text> ().text = "Career & Levels";

			GameData.CurrentGameMode = 1;
		} else {
			//Carrier Levels`
			GameMode.GetComponentsInChildren<Text> () [0].enabled = true;
			GameMode.GetComponentsInChildren<Text> () [1].enabled = false;
			GameMode.GetComponent<Image> ().sprite = Resources.Load ("Endless", typeof(Sprite)) as Sprite;

			//SetButton Property for Play Button
			btnPlay.onClick.RemoveAllListeners ();
			btnPlay.onClick.AddListener (PlayEndless);
			btnPlay.GetComponentInChildren<Text> ().text = "Play";
		
			GameData.CurrentGameMode = 0;
		}

		Debug.Log ("GameData.CurrentGameMode : " +	GameData.CurrentGameMode.ToString ());
	}


	public void GetLevelsFromDB ()
	{
		#if UNITY_ANDROID
		//FireBase 
		FirebaseAnalytics.SetCurrentScreen ("Levels_Screen", this.name);
		#endif
		GameData.Levels = new List<Levels_Master> ();
		IEnumerable<Levels_Master> data = ds.GetLevel ();
		GameData.Levels = data.ToList ();
		kProductIDLevels = new Dictionary<int,string> ();

		int i = 0;


		if (LevelContainer.transform.childCount < GameData.Levels.Count) {
			NewLevelList = new List<GameObject> ();
		}
			
		foreach (var level in GameData.Levels) {
			if (LevelContainer.transform.childCount < GameData.Levels.Count) {
				GameObject NewLevel = Instantiate (LevelPrefab)as GameObject;
				NewLevel.name = level.Level_Id.ToString ();
				NewLevelList.Add (NewLevel);
				NewLevel.transform.SetParent (LevelContainer.transform);
				NewLevel.GetComponent<RectTransform> ().localScale = Vector3.one;
				Button LevelButton = NewLevel.GetComponentsInChildren<Button> () [0];
				Button LockButton = NewLevel.GetComponentsInChildren<Button> () [1];
				Text[] TxtLevelNo = NewLevel.transform.GetComponentsInChildren<Text> ();
				TxtLevelNo [0].text = level.Level_Id.ToString ();
				TxtLevelNo [2].text = level.Level_Id.ToString ();
				setbutton (LevelButton, level.Level_Id);
				if (level.Is_Locked) {
					LockButton.gameObject.SetActive (true);
					setLockbutton (LockButton, level.Level_Id);
				} else {
					LockButton.gameObject.SetActive (false);
				}
			} else {

				Debug.Log (NewLevelList [level.Level_Id - 1].name);
				Button[] LevelButton = NewLevelList [level.Level_Id - 1].GetComponentsInChildren<Button> ();

				if (level.Is_Locked) {
					LevelButton [1].gameObject.SetActive (true);
					setLockbutton (LevelButton [1], level.Level_Id);
				} else {
					if (LevelButton.Length > 1)
						LevelButton [1].gameObject.SetActive (false);
				}
			}

		} 

		GetComponent<IAPController> ().StartIAPInit ();
	}

	void setbutton (Button btn, int level)
	{
		btn.onClick.AddListener (() => TargetInstruction (level));
	}

	void TargetInstruction (int level)
	{
		#if UNITY_ANDROID
		//FireBase 
		FirebaseAnalytics.SetCurrentScreen ("Levels_Target_Screen", this.name);
		#endif
		InstructionScreen.SetActive (true);
		InGems.SetActive (false);
		InPoints.SetActive (false);
		InWord.SetActive (false);
		InTime.SetActive (false);
		InTiles.SetActive (false);

		GameData.CurrentLevel = level;
		Levels_Master LevelTargets = ds.GetSingleLevel ();
		string msg = "";
		if (LevelTargets.Point > 0) {
			Debug.Log ("Point:" + LevelTargets.Point.ToString ());
			InPoints.SetActive (true);
			InPoints.GetComponentsInChildren<Text> () [1].text = LevelTargets.Point.ToString ();
			msg = "Collect " + LevelTargets.Point.ToString () + " points and clear level";
		}

		if (LevelTargets.Gems > 0) {
			Debug.Log ("Gems:" + LevelTargets.Gems.ToString ());

			InGems.SetActive (true);
			InGems.GetComponentInChildren<Text> ().text = LevelTargets.Gems.ToString ();
			msg = "Collect " + LevelTargets.Gems.ToString () + " gems and clear level";
		}

		if (LevelTargets.Word != "Null") {
			Debug.Log ("Word:" + LevelTargets.Word.ToString ());
			InWord.SetActive (true);
			InWord.GetComponentInChildren<Text> ().text = LevelTargets.Word;
			msg = "Collect " + LevelTargets.Word.ToString () + " alphabets and clear level";
		}

		if (LevelTargets.Time > 0.1) {
			Debug.Log ("Time:" + LevelTargets.Gems.ToString ());
			InTime.SetActive (true);
			TimeSpan timeSpan1 = TimeSpan.FromMinutes (Double.Parse (LevelTargets.Time.ToString ()));
			InTime.GetComponentInChildren<Text> ().text = string.Format ("{0:D2}:{1:D2}:{2:D2}", timeSpan1.Hours, timeSpan1.Minutes, timeSpan1.Seconds);
			msg = "Stay in game till " + InTime.GetComponentInChildren<Text> ().text + " time and clear level";
		}

		if (LevelTargets.No_Of_ColoredTiles > 0) {
			isColorTarget = true;
			Debug.Log ("Tiles:" + LevelTargets.No_Of_ColoredTiles.ToString ());
			InTiles.SetActive (true);
			mycolor = new Color (float.Parse (LevelTargets.Color_Code.Split (',') [0]), float.Parse (LevelTargets.Color_Code.Split (',') [1]), float.Parse (LevelTargets.Color_Code.Split (',') [2]), float.Parse (LevelTargets.Color_Code.Split (',') [3]));
			InTiles.GetComponentsInChildren<Image> () [1].color = mycolor;
			InTiles.GetComponentInChildren<Text> ().text = LevelTargets.No_Of_ColoredTiles.ToString ();
			msg = "Collect " + LevelTargets.No_Of_ColoredTiles.ToString () + " tiles with above color and clear level";
		} else {
			isColorTarget = false;
		}

		TxtInstruction.text = msg;
		InBtnPlay.onClick.RemoveAllListeners ();
		InBtnPlay.GetComponentsInChildren<Text> () [1].text = level.ToString ();
		InBtnPlay.onClick.AddListener (() => PlayLevel (level));
	}

	void setLockbutton (Button btn, int level)
	{
		kProductIDLevels.Add (level, "com.colorhop.level" + level.ToString ());
		btn.onClick.AddListener (() => GetComponent<IAPController> ().buyLevels (level.ToString ()));
	}


	void PlayLevel (int LevelNo)
	{
		InstructionScreen.SetActive (false);
		Debug.Log ("Level No:" + LevelNo);
		GameData.CurrentLevel = LevelNo;
		PlayEndless ();
	}

	public void PlayEndless ()
	{
		#if UNITY_ANDROID
		if (GameData.CurrentGameMode == 1) {
			//FireBase 
			FirebaseAnalytics.SetCurrentScreen ("Level_Gameplay_Screen", this.name);
		} else {
			//FireBase 
			FirebaseAnalytics.SetCurrentScreen ("Endless_Gameplay_Screen", this.name);
		}
		#endif

		Debug.Log ("GameData.CurrentGameMode : " +	GameData.CurrentGameMode.ToString ());
		UiScreen.SetActive (false);
		mainCamera.gameObject.SetActive (false);
		if (GameData.isDay)
			GameBG.GetComponent<Image> ().sprite = Resources.Load ("Day", typeof(Sprite)) as Sprite;
		else
			GameBG.GetComponent<Image> ().sprite = Resources.Load ("Night", typeof(Sprite)) as Sprite;
		
		GameAreaScreen.SetActive (true);

	}

	void OpenLevelList ()
	{
		UIController (true, false, false, false, false, false, false, false, true);
		GetLevelsFromDB ();
	}

	public void ShowMainScreen ()
	{
		#if UNITY_ANDROID
		//FireBase 
		FirebaseAnalytics.SetCurrentScreen ("Home_Screen", this.name);
		#endif
		UIController (true, true);
		SetBanner ();
	}

	public void GetGift (Text txtGift)
	{
		#if UNITY_ANDROID
		//FireBase 
		FirebaseAnalytics.SetCurrentScreen ("Gift_Screen", this.name);
		#endif
		int gems = Util.GetDiamond () + int.Parse (txtGift.text);
		Util.SetDiamond (gems);
		giftImage.GetComponentInChildren<Text> ().text = gems.ToString ();
	}

	public void ActiveGift ()
	{
		giftImage.GetComponentInChildren<Button> ().gameObject.GetComponent<Image> ().enabled = true;

		for (int i = 0; i < gifts.Length; i++) {
			gifts [i].SetActive (true);
			gifts [i].GetComponentInChildren<Text> ().text = UnityEngine.Random.Range (1, 5).ToString ();
			gifts [i].GetComponentInChildren<Button> ().gameObject.GetComponent<Image> ().enabled = true;
		}
	}

	public void ShowAchivementScreen ()
	{
		#if UNITY_ANDROID
		//FireBase 
		FirebaseAnalytics.SetCurrentScreen ("Achivement_Button", this.name);
		#endif
		LeaderboardManager.ShowAchivements ();	
	}

	public void ShowLeaderboardScreen ()
	{		
		#if UNITY_ANDROID
		//FireBase 
		FirebaseAnalytics.SetCurrentScreen ("Leaderboard_Button", this.name);
		#endif

		LeaderboardManager.ShowLeaderboard ();
	}

	public void BallLockedButton ()
	{
		#if UNITY_ANDROID
		//FireBase 
		FirebaseAnalytics.SetCurrentScreen ("Balls_Screen", this.name);
		#endif
		txtBallGems.text = Util.GetDiamond ().ToString ();

		for (int i = 0; i < Balls.Length; i++) {
			Button[] ballsButtons = Balls [i].GetComponentsInChildren<Button> ();
			Button BtnBall = ballsButtons [0];
			Button BtnBallLock = null;
			string ballname = BtnBall.GetComponent<Image> ().sprite.name;

			int gems = 0;
			if (ballsButtons.Length > 1) {
				BtnBallLock = ballsButtons [1];
				gems = int.Parse (BtnBallLock.GetComponentInChildren<Text> ().text);
			}

				
			if (PlayerPrefs.GetInt (ballname) == 0)
				setBall (ballname, gems, BtnBallLock, true);
			else {
				if (ballsButtons.Length > 1)
					BtnBallLock.gameObject.SetActive (false);
			}
			setBall (ballname, gems, BtnBall, false);
		}
	}

	void setBall (string BallName, int gems, Button balls, bool islocked)
	{
		balls.onClick.RemoveAllListeners ();
		balls.onClick.AddListener (() => UnlockedBall (BallName, gems, balls, islocked));
	}

	void UnlockedBall (string BallName, int gems, Button balls, bool isLocked)
	{

		Debug.Log ("islocked:" + isLocked);

		if (isLocked) {
			int availableGems = Util.GetDiamond ();
			if (availableGems > gems) {
				availableGems -= gems;
				PlayerPrefs.SetInt ("_DIAMOND", availableGems);	
				Util.SetDiamond (availableGems);
				txtBallGems.text = Util.GetDiamond ().ToString ();
				balls.gameObject.SetActive (false);
				BallMaterial.mainTexture = Resources.Load ("BallsImages/" + BallName, typeof(Texture)) as Texture;
				CurrentBallImage.sprite = Resources.Load ("BallsImages/" + BallName, typeof(Sprite)) as Sprite;
				PlayerPrefs.SetInt (BallName, 1);
			} else {
				balls.gameObject.SetActive (true);
			}
		} else {
			BallMaterial.mainTexture = Resources.Load ("BallsImages/" + BallName, typeof(Texture)) as Texture;
			CurrentBallImage.sprite = Resources.Load ("BallsImages/" + BallName, typeof(Sprite)) as Sprite;
		}
	}

	public void ShowSettings ()
	{
		#if UNITY_ANDROID
		//FireBase 
		FirebaseAnalytics.SetCurrentScreen ("Setting_Screen", this.name);
		#endif
		txtSession.text = Util.GetPlayerSession ().ToString ();
		if (Util.GetVolume () == 0) {
			ImgVolume.sprite = VolumeOff;
		} else {
			ImgVolume.sprite = VolumeOn;
		}
	}

	public void SetVolume ()
	{
		if (Util.GetVolume () == 0) {
			PlayerPrefs.SetInt ("Volume", 1);
			ImgVolume.sprite = VolumeOn;
		} else {
			PlayerPrefs.SetInt ("Volume", 0);
			ImgVolume.sprite = VolumeOff;
		}
	}

	public void SetBanner ()
	{
		if (MainScreen.activeInHierarchy) {
			ads.GetComponent<UnityAdsInitializer> ().ShowInterstitial ();
		} else if (WheelOfFourtneScreen.activeInHierarchy) {
			ads.GetComponent<UnityAdsInitializer> ().ShowBanner ();
		} else if (LevelUpScreen.activeInHierarchy) {
			ads.GetComponent<UnityAdsInitializer> ().ShowBanner ();
		} else if (GameOverScreen.activeInHierarchy) {
			ads.GetComponent<UnityAdsInitializer> ().ShowBanner ();
		} else {
//			ads.GetComponent<UnityAdsInitializer> ().removeBanner ();
		}
	}


}
