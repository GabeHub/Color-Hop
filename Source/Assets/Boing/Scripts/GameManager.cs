
/***********************************************************************************************************
 * Produced by App Advisory - http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/


using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Advertisements;

//myFix
using DG.Tweening;

#if AADOTWEEN
using DG.Tweening;
#endif




public class GameManager : MonoBehaviour
{
	public int numberOfPlayToShowInterstitial = 5;

	public string VerySimpleAdsURL = "http://u3d.as/oWD";

	public GameObject objGameArea;

	#region canvas

	public Text EndlesspointText, LevelPointText;
	public Text diamondText;
	public GameObject tutoAtStart;
	public bool isPause = false;
	public bool isGetChance = false;

	public GameObject LifeChance;

	#endregion

	#region player

	public Transform playerParent;
	public Transform playerSphere;
	public Transform shadow;

	#endregion

	#region customizable variables

	public List<Color> colors;
	public float distanceZ = 5;
	public float cutOffTime = 0.01f;
	public int numberOfPlatformToSpawnedAtStart = 6;
	public List<GameObject> PlatformsList = new List<GameObject> ();
	public int changeColorEveryXPoints = 15;
	public float speedBounceInSeconds = 0.4f;
	public float swipeSensibility = 7f;
	public float Second;

	#endregion


	#region public variables

	public int point = 0;
	public int diamond = 0;
	public int alphabetsCount = 0;
	public String[] WordSpell;


	#endregion

	#region references

	public GameObject platformPrefab, platformDoublePrefab;
	public int[] LevelDesignFormat;
	public Material plateformMaterial, BallMaterial, platformSingleMaterial, platformTargetMaterial1, platformTargetMaterial2;
	public UIManager UIM;

	#endregion

	#region sounds

	AudioSource audioSource;
	public AudioClip soundJump;

	void PlaySoundJump ()
	{
		audioSource.PlayOneShot (soundJump);
	}

	public AudioClip soundFall;

	void PlaySoundFall ()
	{
		audioSource.PlayOneShot (soundFall);
	}

	public AudioClip soundDiamond;

	void PlaySoundDiamond ()
	{
		audioSource.PlayOneShot (soundDiamond);
	}

	#endregion

	public bool gameIsStarted = false;

	public bool isGameOver = false;
	public  bool isWin;
	public Text txtCounter;

	int spawnPlatformCount = 0, nextTilesCounter = 0, nameCounter = 0, totalDouble = 0, totalSingle = 0;

	float posZTarget;

	public Camera camGame;


	void OnEnable ()
	{

		playerParent.parent.gameObject.SetActive (true);
		isPause = false;
		LifeChance.SetActive (false);
		audioSource = GetComponent<AudioSource> ();
		audioSource.volume = Util.GetVolume ();

		Time.fixedDeltaTime = 1f / 60f;
		Time.maximumDeltaTime = 5f / 60f;

		GameObject go = null; 
		go = GameObject.Find ("[DOTween]");

		if (go == null) {
            //myFix
            DOTween.Init ();

			#if AADOTWEEN
			DOTween.Init ();
			#endif
		}
		gameIsStarted = false;
		playerParent.transform.position = new Vector3 (0, 0, 0);
		playerParent.GetComponentInChildren<MeshRenderer> ().transform.position = new Vector3 (0, -0.4f, 0);

		ResetGame ();

	}


	public void levelLoad ()
	{

		Debug.Log ("GameData.CurrentGameMode:" + GameData.CurrentGameMode.ToString ());

		if (GameData.CurrentGameMode == 1) {
			alphabetsCount = 0;
			GetComponent<CheckLevels> ().GetLevelDtata ();
		} else {
			alphabetsCount = Util.getFilledEndlessWord ().ToCharArray ().Length;
            GameData.EndlessWord = WordSpell [UnityEngine.Random.Range(0, WordSpell.Length)];
			Util.setEndlessWord (GameData.EndlessWord);
		}
		StartCoroutine (StartGamePlay ());

		tutoAtStart.SetActive (true);
	}

	IEnumerator StartGamePlay ()
	{

		for (int i = 0; i < numberOfPlatformToSpawnedAtStart; i++) {
			SpawnPlatform ();
		}

		// Set Color
		Invoke ("setColor", 0.1f);

		while (true) {
			if (Input.GetMouseButton (0)) {
				Debug.Log ("Input 1:" + gameIsStarted);
				if (!gameIsStarted)
					StartTheGame ();

				break;
			}

			yield return 0;
		}
	}

	void StartTheGame ()
	{
		if (!gameIsStarted) {
			PlayerMove ();
			Debug.Log ("gameIsStarted");
			gameIsStarted = true;
			if (GetComponent<CheckLevels> ().TimeTarget.activeInHierarchy) {
				InvokeRepeating ("getSec", 0, 1);
			}
		}

		tutoAtStart.SetActive (false);
	}

	public void getSec ()
	{
		if (!isWin) {
			Second++;
			TimeSpan timeSpan1 = TimeSpan.FromSeconds (Double.Parse (Second.ToString ()));
			GetComponent<CheckLevels> ().timeText = string.Format ("{0:D2}:{1:D2}:{2:D2}", timeSpan1.Hours, timeSpan1.Minutes, timeSpan1.Seconds);
			if (GameObject.FindGameObjectWithTag ("W_Time")) {
				GameObject.FindGameObjectWithTag ("W_Time").GetComponent<Text> ().text = GetComponent<CheckLevels> ().timeText;
			}
		}
	}

	public void StopGetSec ()
	{
		CancelInvoke ("getSec");
	}

	void OnUpdate ()
	{
		if (isPause)
			return;
				
		if (isGameOver)
			return;

		if (Input.GetMouseButton (0) && !isWin) {
			if (!gameIsStarted)
				StartTheGame ();

			var playerPos = playerParent.position;
			playerSphere.position = new Vector3 (GetPositionTouchX (), playerSphere.position.y, playerSphere.position.z);

            //myFix
            playerSphere.DOMoveX (GetPositionTouchX (), 0.00001f);

			#if AADOTWEEN
//			playerSphere.DOMoveX (GetPositionTouchX (), 0.00001f);
			#endif

		}

	}

	float GetPositionTouchX ()
	{

		float x = 0;

		if (Application.isMobilePlatform) {
			if (Input.touchCount > 0) {

				Touch touch = Input.GetTouch (0);

				x = touch.position.x / Screen.width - 0.5f;
			}
		} else {
			x = Input.mousePosition.x / Screen.width - 0.5f;
		}

		if (x < -0.5f)
			x = -0.5f;

		if (x > 0.5f)
			x = 0.5f;

		return swipeSensibility * x;
	}

	void UpdateCamPosZ ()
	{
		if (!isWin && !isGameOver) {
			var pCam = camGame.transform.position;
			pCam.z = playerParent.position.z - 8.63f;
			camGame.transform.position = pCam;

//			pCam = camGame.transform.position;
//			pCam.z = playerParent.position.z - 8.63f;
//			camGame.transform.position = pCam;
		}
	}

    //myFix
    Ease playerAnimEase = Ease.OutCubic;

	#if AADOTWEEN
	//Ease playerAnimEase = Ease.OutCubic;
	Ease playerAnimEase = Ease.OutCubic;

	#endif


	void PlayerMove ()
	{


		CheckIfPlayerIsGrounded ();

		if (isGameOver)
			return;


		speedBounceInSeconds -= cutOffTime;

		if (speedBounceInSeconds < 0.25f)
			speedBounceInSeconds = 0.25f;

		AnimShadow ();

        //myFix
        playerSphere.DOMoveY (3f, speedBounceInSeconds).SetLoops (2, LoopType.Yoyo).SetEase (playerAnimEase);

		#if AADOTWEEN
		playerSphere.DOMoveY (3f, speedBounceInSeconds).SetLoops (2, LoopType.Yoyo).SetEase (playerAnimEase);
		#endif

		playerParent.position = new Vector3 (playerParent.position.x, playerParent.position.y, posZTarget); 

		posZTarget += 7.5f;

        //myFix
        playerParent.DOMoveZ (posZTarget, speedBounceInSeconds * 2)
			.SetEase (Ease.Linear)
			.OnUpdate (() => {
			OnUpdate ();
			UpdateCamPosZ ();
		})
			.OnComplete (() => {
			//PlaySoundJump ();
			PlayerMove ();
		});

		#if AADOTWEEN
		playerParent.DOMoveZ (posZTarget, speedBounceInSeconds * 2f)
			.SetEase (Ease.Linear)
			.OnUpdate (() => {
			OnUpdate ();
			UpdateCamPosZ ();
		})
			.OnComplete (() => {
			PlaySoundJump ();
			PlayerMove ();
		});
		#endif
	}

	public void SpawnPlatform ()
	{
				
		if (isGameOver)
			return;
		GameObject go = null;

		if (LevelDesignFormat [spawnPlatformCount] == 1) {
			go = Instantiate (platformDoublePrefab) as GameObject;
			Platform[] t = go.transform.GetComponentsInChildren<Platform> ();
			go.tag = "PlatformDouble";
			PlatformsList.Add (go.gameObject);
			SpawnPlatform (t [0].gameObject.transform, t [1].gameObject.transform, true);

		} else {
			go = Instantiate (platformPrefab) as GameObject;
			Transform t1 = go.transform;
			go.tag = "PlatformSingle";
			PlatformsList.Add (go.gameObject);

			SpawnPlatform (t1, null, false);
		}

		go.transform.SetParent (playerParent.transform.parent.gameObject.transform);


	}


	void setColor ()
	{
		nextTilesCounter++;
		GameObject NextTiles = null;

		if (nextTilesCounter == numberOfPlatformToSpawnedAtStart)
			nextTilesCounter = 0;

//		Debug.Log ("nextTilesCounter : " + nextTilesCounter.ToString ());
		
		if (PlatformsList [nextTilesCounter].tag == "PlatformSingle") {
			NextTiles = PlatformsList [nextTilesCounter].GetComponentInChildren<Rigidbody> ().gameObject;
			BallMaterial.color = NextTiles.GetComponent<MeshRenderer> ().material.color;
		} else {
			NextTiles = PlatformsList [nextTilesCounter].GetComponentsInChildren<Rigidbody> () [UnityEngine.Random.Range (0, 2)].gameObject;
			BallMaterial.color = NextTiles.GetComponent<MeshRenderer> ().material.color;
		}

		CancelInvoke ("setColor");
	}

	const float posXmax = 2.5f;
	//-2.5f;

	public void SpawnPlatform (Transform t = null, Transform t2 = null, bool isDouble = false)
	{
		if (isPause)
			return;

		if (isGameOver)
			return;
		if (isDouble)
			Debug.Log ("Suffle isDouble: " + t.transform.parent.name.ToString ());
		else
			Debug.Log ("Suffle : " + t.transform.name.ToString ());


		int colorIndex = 0;
//		if (point < 10) {
//			colorIndex = UnityEngine.Random.Range (0, 5);
//		} else if (point < 20) {	
//			colorIndex = UnityEngine.Random.Range (5, 10);
//		} else if (point < 30) {
//			colorIndex = UnityEngine.Random.Range (10, 15);
//		} else if (point < 40) {
//			colorIndex = UnityEngine.Random.Range (15, 19);
//		} else {

		Color gameColor;
		if (UIM.isColorTarget) {
			if (point % 5 == 0) {
				gameColor = UIM.mycolor;
			} else {
				colorIndex = UnityEngine.Random.Range (0, colors.Count);
				gameColor = colors [colorIndex];
			}
		} else {
			colorIndex = UnityEngine.Random.Range (0, colors.Count);
			gameColor = colors [colorIndex];
		}
		//}

		Color predefineTilesColor = colors [colorIndex];

		float posX = Util.GetRandomNumber (-posXmax, posXmax);

		if (Util.GetRandomNumber (0, 100f) < 70f) {
			if (Util.GetRandomNumber (0, 100f) < 50f) {
				posX = Util.GetRandomNumber (-posXmax, -1f);
			} else {
				posX = Util.GetRandomNumber (1f, posXmax);
			}
		}

		if (isDouble) {
			t.parent.transform.position = new Vector3 (0, 0, spawnPlatformCount * 7.5f);
			t.transform.localPosition = new Vector3 (posX, t.transform.position.y, 0);


			if (posX <= 0 && posX > -0.09f) {
				posX = -posX + 2f;
			} else if (posX <= 0 && posX > -1f) {
				posX = -posX + 1.5f;
			} else if (posX > 0 && posX < 0.09f) {
				posX = -posX - 2.3f;
			} else if (posX > 0 && posX < 1f) {
				posX = -posX - 1.5f;
			} else {
				posX = -posX;
			}

			t2.transform.localPosition = new Vector3 (posX, t.transform.position.y, 0);

			if (colorIndex > 0) {
				platformTargetMaterial1.color = colors [colorIndex];
                platformTargetMaterial2.color = colors [colorIndex - 1];
			} else {
				platformTargetMaterial1.color = colors [colorIndex];
				platformTargetMaterial2.color = colors [colorIndex + 1];
			}
		
			t.GetComponentInChildren<Rigidbody> ().gameObject.GetComponent<MeshRenderer> ().material = platformTargetMaterial1; 
			t2.GetComponentInChildren<Rigidbody> ().gameObject.GetComponent<MeshRenderer> ().material = platformTargetMaterial2; 

			t.parent.gameObject.name = spawnPlatformCount.ToString ();
			spawnPlatformCount++;

		} else {
			if (spawnPlatformCount == 0)
				t.transform.position = new Vector3 (0, 0f, spawnPlatformCount * 7.5f);
			else
				t.transform.position = new Vector3 (posX, 0f, spawnPlatformCount * 7.5f);

			GameObject NextTiles = t.GetComponentInChildren<Rigidbody> ().gameObject;
			platformSingleMaterial.color = predefineTilesColor;
			NextTiles.GetComponent<MeshRenderer> ().material = platformSingleMaterial; 

			t.gameObject.name = spawnPlatformCount.ToString ();
			spawnPlatformCount++;

		}


	}

	void AnimShadow ()
	{
		if (isPause)
			return;

		if (isGameOver)
			return;

		float shadowLocalScale = 0.08f;

        //myFix
        shadow.DOScale (0 * shadowLocalScale, speedBounceInSeconds)
			.SetEase (Ease.OutExpo)
			.OnComplete (() => {
			shadow.DOScale (shadowLocalScale, speedBounceInSeconds)
					.SetEase (Ease.InExpo);
		});

		#if AADOTWEEN
		shadow.DOScale (0 * shadowLocalScale, speedBounceInSeconds)
			.SetEase (Ease.OutExpo)
			.OnComplete (() => {
			shadow.DOScale (shadowLocalScale, speedBounceInSeconds)
					.SetEase (Ease.InExpo);
		});
		#endif
	}

	void CheckIfPlayerIsGrounded ()
	{

		if (isPause)
			return;

		if (isGameOver)
			return;

		if (posZTarget > 4) {
			RaycastHit hit;

			Vector3 down = playerSphere.TransformDirection (Vector3.down);

			if (Physics.Raycast (playerSphere.position, down, out hit)) {
				Platform platform = hit.transform.parent.GetComponent<Platform> ();


				if (platform != null) {
					
					if (hit.transform.gameObject.GetComponent<MeshRenderer> ().material.color == BallMaterial.color) {

                        PlaySoundJump();

                        platform.OnPlayerBounce ();

						Add1Point ();

						// Set Color
						Invoke ("setColor", 0.1f);

						if (GameData.CurrentGameMode == 1) {
					
							GetComponent<CheckLevels> ().CheckHitForLevel (hit.transform.gameObject);
						}

					} else {
                        StartCoroutine ("TakeGameOverChance");
					}
					
				} else {
                    StartCoroutine ("TakeGameOverChance");
				}
			} else {
                StartCoroutine ("TakeGameOverChance");
			}
		}
	}


	IEnumerator TakeGameOverChance ()
    {
        PlaySoundFall();
        isGetChance = false;
		playerParent.parent.gameObject.SetActive (false);
		LifeChance.SetActive (true);
		isPause = true;
		if (PlayerPrefs.GetInt ("GAMEOVER_COUNT", 0) == 3) {
			PlayerPrefs.DeleteKey ("GAMEOVER_COUNT");
			//Watch Add		
			if (Advertisement.IsReady ("rewardedVideo")) {
				Debug.Log ("Ads");
				var options = new ShowOptions { resultCallback = HandleShowResult };
				Advertisement.Show ("rewardedVideo", options);
			}
			GameOver ();
		
		} else {
			txtCounter.text = "5";
			yield return new WaitForSeconds (1);
			txtCounter.text = "4";
			yield return new WaitForSeconds (1);
			txtCounter.text = "3";
			yield return new WaitForSeconds (1);
			txtCounter.text = "2";
			yield return new WaitForSeconds (1);
			txtCounter.text = "1";
			yield return new WaitForSeconds (1);
			GameOver ();
		}
	}

	public void takeChance ()
	{
		StopCoroutine ("TakeGameOverChance");
		if (Advertisement.IsReady ("rewardedVideo")) {
			Debug.Log ("Ads");
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show ("rewardedVideo", options);
		}
	}

	public void avoidChance ()
	{
		int count = PlayerPrefs.GetInt ("GAMEOVER_COUNT", 0);
		count++;
		PlayerPrefs.SetInt ("GAMEOVER_COUNT", count);
		PlayerPrefs.Save ();
		isPause = false;
		GameOver ();
	}


	private void HandleShowResult (ShowResult result)
	{
		switch (result) {
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			LifeChance.SetActive (false);
			if (PlayerPrefs.GetInt ("GAMEOVER_COUNT", 0) == 3) {
				PlayerPrefs.DeleteKey ("GAMEOVER_COUNT");
				isGetChance = false;
			} else {
				isGetChance = true;
			}
			isPause = false;
			PlayerPrefs.DeleteKey ("GAMEOVER_COUNT");
			GameOver ();
			// YOUR CODE TO REWARD THE GAMER
			// Give coins etc.
			break;
		case ShowResult.Skipped:
			Debug.Log ("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError ("The ad failed to be shown.");
			break;
		}
	}

	void Add1Point ()
	{
		if (isPause)
			return;

		if (isGameOver)
			return;

		point++;

		if (GameData.CurrentGameMode == 0) {
			EndlesspointText.text = point.ToString ();
		} else if (GameData.CurrentGameMode == 1 && GetComponent<CheckLevels> ().TargetWithPoint.activeInHierarchy) {
			LevelPointText.text = point.ToString ();
		}
	}

	public void Add1Diamond ()
	{
		if (isPause)
			return;

		if (isGameOver)
			return;

		diamond++;
		diamondText.text = diamond.ToString ();
		PlaySoundDiamond ();
	}

	public void Add1Alphabet (string text)
	{

		if (isPause)
			return;

		if (isGameOver)
			return;



		if (GameData.CurrentGameMode == 0) {
			
			if (!GameData.FilledEndlessWord.Contains (text)) {
				GameData.FilledEndlessWord += text;
				alphabetsCount++;
			}
	
		} else {

			if (!GameData.FilledLevelWord.Contains (text)) {
				GameData.FilledLevelWord += text;	
				alphabetsCount++;
			}
			GetComponent<CheckLevels> ().CheckForWord ();


		}

		Debug.Log ("Char :  " + text + "  FilledLevelWord :" + GameData.FilledLevelWord);



		PlaySoundDiamond ();
	}



	void GameOver ()
	{

		if (isGameOver)
			return;

		if (!isWin) {

			isGameOver = true;

			//PlaySoundFall ();


			Util.SetDiamond (diamond);

			Util.SetLastScore (point);

			Util.setFilledEndlessWord (GameData.FilledLevelWord);
            //CancelInvoke ("StopGetSec");
            StopGetSec();

            //myFix
            DOTween.KillAll (false);
			playerParent.DOKill (false);
			playerSphere.DOKill (false);
			shadow.DOKill (false);

			#if AADOTWEEN
			DOTween.KillAll (false);
			playerParent.DOKill (false);
			playerSphere.DOKill (false);
			shadow.DOKill (false);
			#endif

			shadow.gameObject.SetActive (false);

            //myFix
            playerSphere.DOLocalMoveY (-10, 1).OnComplete (() => {
				
				if (!isGetChance) {
					UIM.UIController (true);

					UIM.GameOverScreen.SetActive (true);
					UIM.SetBanner ();
				} else {
					OnEnable ();
				}
			});

			#if AADOTWEEN
			playerSphere.DOLocalMoveY (-10, 1).OnComplete (() => {
				
				if (!isGetChance) {
					UIM.UIController (true);

					UIM.GameOverScreen.SetActive (true);
					UIM.SetBanner ();
				} else {
					OnEnable ();
				}
			});
			#endif


		}
	}

	void ResetGame ()
	{

		if (PlatformsList.Count > 0) {
			for (int i = 0; i < PlatformsList.Count; i++)
				DestroyImmediate (PlatformsList [i].gameObject);
		}
			
		PlatformsList = new List<GameObject> ();


		spawnPlatformCount = 0;
		nextTilesCounter = 0;
		posZTarget = 0;
		isGameOver = false;
		gameIsStarted = false;
		if (!isGetChance) {
			point = 0;
			diamond = 0;
			EndlesspointText.text = "0";
            LevelPointText.text = "0";
            diamondText.text = "0";
			speedBounceInSeconds = 0.4f;
		} 
		isWin = false;
		//cutOffTime = 0.001f;
		Second = 0.0f;
		nameCounter = 0;
//		UIM.SetBanner ();
		levelLoad ();
		isPause = false;
		isGameOver = false;

	}




}
