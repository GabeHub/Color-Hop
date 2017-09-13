using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CheckLevels : MonoBehaviour
{

	public int Points, Gems, ColoredTilesCount = 0;
	float NoOfColoredTiles = 0.0f, Times = 0.0f;
	[SerializeField] private float _duration = 1f;
	public string ColorCode;
	public Text[] Alphabets;
	public GameObject EndlessTarget, LevelTarget, GemsTarget, TimeTarget, ColorTarget, WordTarget, PointTarget, TargetWithPoint;



	int ScoreCounter = 0;
	Image TargetColorImage;
	//	string timeText = "", s;
	public string Word;
	Color mycolor = new Color ();

	UIManager UiM;


	void OnEnable ()
	{
		if (GameData.CurrentGameMode == 0) {
			EndlessTarget.SetActive (true);
			LevelTarget.SetActive (false);
		} else {
			EndlessTarget.SetActive (false);
			LevelTarget.SetActive (true);
		}

	}

	void GameBoardController (bool isTargetWithPoint = false, bool isGemsTarget = false, bool isTimeTarget = false, bool isColorTarget = false, bool isWordTarget = false, bool isPointTarget = false)
	{

		Debug.Log (isTargetWithPoint.ToString () + "  " + isColorTarget.ToString ());

		GemsTarget.SetActive (isGemsTarget);
		TimeTarget.SetActive (isTimeTarget);
		ColorTarget.SetActive (isColorTarget);
		WordTarget.SetActive (isWordTarget);
		PointTarget.SetActive (isPointTarget);
		TargetWithPoint.SetActive (isTargetWithPoint);

	}

	public string timeText = "";
	string timeText1 = "";

	void Update ()
	{

	}

	public void GetLevelDtata ()
	{
		UiM = GameObject.Find ("UIManager").GetComponent<UIManager> ();


		if (GameData.CurrentGameMode == 0) {
			EndlessTarget.SetActive (true);
			LevelTarget.SetActive (false);
		} else {
			EndlessTarget.SetActive (false);
			LevelTarget.SetActive (true);
		}

		Debug.Log ("Current Level:" + GameData.CurrentLevel.ToString ());
		Levels_Master LevelTargets = UiM.ds.GetSingleLevel ();

		Points = LevelTargets.Point;
		Gems = LevelTargets.Gems;
		Times = LevelTargets.Time;
		NoOfColoredTiles = LevelTargets.No_Of_ColoredTiles;
		ColorCode = LevelTargets.Color_Code;
		Word = LevelTargets.Word;
		Debug.Log ("Point : " + LevelTargets.Point.ToString () + " Gems : " + LevelTargets.Gems.ToString () + " _duration : " + _duration.ToString () + " NoOfColoredTiles : " + NoOfColoredTiles.ToString () + " Word: " + LevelTargets.Word);


		if (Times > 0.1f) {
			Debug.Log ("Times");
			GameBoardController (true, false, true);
			TimeSpan timeSpan1 = TimeSpan.FromMinutes (Double.Parse (Times.ToString ()));
			timeText1 = string.Format ("{0:D2}:{1:D2}:{2:D2}", timeSpan1.Hours, timeSpan1.Minutes, timeSpan1.Seconds);
//			timeText = string.Format ("{1:D2}:{2:D2}", timeSpan.Minutes, timeSpan.Seconds);
			GameObject.FindGameObjectWithTag ("W_Time").GetComponent<Text> ().text = "00:00";
			GameObject.FindGameObjectWithTag ("T_Time").GetComponent<Text> ().text = "/" + timeText1.ToString ();
		}

		if (Points > 0) {
			Debug.Log ("Points");
			GameBoardController (false, false, false, false, false, true);
			GameObject.FindGameObjectWithTag ("W_Point").GetComponent<Text> ().text = "00";
			GameObject.FindGameObjectWithTag ("TargetPoint").GetComponent<Text> ().text = Points.ToString ();
		}

		if (Gems > 0) {
			Debug.Log ("Gems");
			GameBoardController (true, true);
			GameObject.FindGameObjectWithTag ("W_Gems").GetComponent<Text> ().text = "00";
			GameObject.FindGameObjectWithTag ("T_Gems").GetComponent<Text> ().text = "/" + Gems.ToString ();
		}

		if (NoOfColoredTiles > 0) {
			Debug.Log ("NoOfColoredTiles");
			GameBoardController (true, false, false, true);
			mycolor = new Color (float.Parse (ColorCode.Split (',') [0]), float.Parse (ColorCode.Split (',') [1]), float.Parse (ColorCode.Split (',') [2]), float.Parse (ColorCode.Split (',') [3]));
			GameObject.FindGameObjectWithTag ("T_Color").GetComponent<Image> ().color = mycolor;
			GameObject.FindGameObjectWithTag ("W_ColoredTiles").GetComponent<Text> ().text = "00";
			GameObject.FindGameObjectWithTag ("T_ColoredTiles").GetComponent<Text> ().text = "/" + NoOfColoredTiles.ToString ();
		}

		if (Word != "Null") {
			Debug.Log ("Word");
			GameBoardController (true, false, false, false, true);
			GameData.LevelWord = Word;
			char[] alpha = Word.ToCharArray ();
			Alphabets = WordTarget.GetComponentsInChildren<Text> ();

			if (Word.ToCharArray ().Length < 7) {
				for (int i = 0; i < alpha.Length; i++) {
					Alphabets [i].text = alpha [i].ToString ();
					Alphabets [i].color = Color.white;
				}
			}
		}

	}

	public void CheckHitForLevel (GameObject HitGameObject)
	{
		//ScoreCounter++;

		if (Points > 0) {
			if (Points == GetComponent<GameManager> ().point) {
				GameObject.FindGameObjectWithTag ("W_Point").GetComponent<Text> ().text = Points.ToString ();
				GetComponent<GameManager> ().isWin = true;
				Debug.Log ("Win");
				StartCoroutine ("GameLevelWin"); 
			} else {
                //GameObject.FindGameObjectWithTag ("W_Point").GetComponent<Text> ().text = ScoreCounter.ToString ();
                GameObject.FindGameObjectWithTag("W_Point").GetComponent<Text>().text = GetComponent<GameManager>().point.ToString();
            }
		}

		if (Gems > 0) {
			if (Gems == GetComponent<GameManager> ().diamond) {
				GameObject.FindGameObjectWithTag ("W_Gems").GetComponent<Text> ().text = Gems.ToString ();
				Debug.Log ("Win");
				GetComponent<GameManager> ().isWin = true;
				StartCoroutine ("GameLevelWin");
			} else {
				GameObject.FindGameObjectWithTag ("W_Gems").GetComponent<Text> ().text = GetComponent<GameManager> ().diamond.ToString ();
			}
		}

		if (Times > 0) {
			if (timeText1 == timeText) {
				GetComponent<GameManager> ().isWin = true;
				Debug.Log ("Win");
				StartCoroutine ("GameLevelWin");
			}
		}

		if (NoOfColoredTiles > 0f) {

			double r1 = Math.Round (float.Parse (HitGameObject.GetComponent<MeshRenderer> ().material.color.r.ToString ()) - 0.005, 2);
			double g1 = Math.Round (float.Parse (HitGameObject.GetComponent<MeshRenderer> ().material.color.g.ToString ()) - 0.005, 2);
			double b1 = Math.Round (float.Parse (HitGameObject.GetComponent<MeshRenderer> ().material.color.b.ToString ()) - 0.005, 2);
			double a1 = Math.Round (float.Parse (HitGameObject.GetComponent<MeshRenderer> ().material.color.a.ToString ()) - 0.005, 2);
			double r2 = Math.Round (float.Parse (mycolor.r.ToString ()) - 0.006, 2); 
			double g2 = Math.Round (float.Parse (mycolor.g.ToString ()) - 0.006, 2); 
			double b2 = Math.Round (float.Parse (mycolor.b.ToString ()) - 0.006, 2); 
			double a2 = Math.Round (float.Parse (mycolor.a.ToString ()) - 0.006, 2); 

			Debug.Log (r1.ToString () + " " + r2.ToString () + " " + g1.ToString () + " " + g2.ToString () + " " + b2.ToString () + " " + b2.ToString () + " " + a1.ToString () + " " + a2.ToString ());
			if (r1 == r2 && g1 == g2 && b1 == b2) {

				ColoredTilesCount++;
				if (NoOfColoredTiles == ColoredTilesCount) {
					Debug.Log ("Win");
					GetComponent<GameManager> ().isWin = true;
					GameObject.FindGameObjectWithTag ("W_ColoredTiles").GetComponent<Text> ().text = NoOfColoredTiles.ToString ();
					StartCoroutine ("GameLevelWin");
					ColoredTilesCount = 0;
				} else {
					GameObject.FindGameObjectWithTag ("W_ColoredTiles").GetComponent<Text> ().text = ColoredTilesCount.ToString ();
				}
			}
		}
	}


	public void CheckForWord ()
	{
		if (Word != "Null") {
			Debug.Log ("Inside");
			int WordLength = GameData.FilledLevelWord.ToCharArray ().Length; 
			Debug.Log (GameData.FilledLevelWord + " ==" + GameData.LevelWord);
			if (GameData.FilledLevelWord == GameData.LevelWord) {
				Debug.Log ("Win");
				GetComponent<GameManager> ().isWin = true;
				Alphabets [WordLength - 1].color = Color.black;
				GameData.FilledLevelWord = GameData.LevelWord;
				StartCoroutine ("GameLevelWin");
			} else {
				Alphabets [WordLength - 1].color = Color.black;
			}
		}
	}


	IEnumerator GameLevelWin ()
	{
		GameData.playerSession = PlayerPrefs.GetInt (" GameData.playerSession", 0);
		GameData.playerSession++;
		PlayerPrefs.SetInt ("PlayerSession", GameData.playerSession);
		PlayerPrefs.Save ();

		GetComponent<GameManager> ().StopGetSec ();
		
		yield return new WaitForSeconds (2);
		Util.SetDiamond (Util.GetDiamond () + 2);
		UiM.txtFinishedLevel.text = GameData.CurrentLevel.ToString (); 
		#if UNITY_IOS
		LeaderboardManager.ReportScore (GameData.CurrentLevel, GameData.LeaderboardLevelsID);
		#else
		LeaderboardManager.ReportScore (GameData.CurrentLevel, LeaderboardManagerAndroid.leaderboard_colorhop_levels);
		#endif
		GameData.CurrentLevel = GameData.CurrentLevel + 1;
		UiM.txtNextLevel.text = GameData.CurrentLevel.ToString ();
		UiM.txtLevelScreenCurrentLevel.text = GameData.CurrentLevel.ToString ();
		UiM.ds.ChangeLevelStatus (GameData.CurrentLevel, 0);
	
		UiM.UIController (true, false, false, false, true, false, true);
		UiM.SetBanner ();
	}
}
