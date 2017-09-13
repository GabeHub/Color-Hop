using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

#if UNITY_ANDROID
using Firebase;
using Firebase.Analytics;
#endif

public class GameOver : MonoBehaviour
{

	public Text txtBestScore, txtGems, txtLastScore, txtVideoAds;
	public Text[] alpha;
	int adreword = 0, oldGems = 0;


	void OnEnable ()
	{
		#if UNITY_ANDROID
		//Firebase
		FirebaseAnalytics.SetCurrentScreen ("GameOver_Screen", this.name);
		#endif
		GameData.playerSession = PlayerPrefs.GetInt (" playerSession", 0);
		GameData.playerSession++;
		PlayerPrefs.SetInt ("PlayerSession", GameData.playerSession);
		PlayerPrefs.Save ();
		UpdateScore ();
	}

	public void UpdateScore ()
	{
		txtBestScore.text = Util.GetBestScore ().ToString ();
		txtGems.text = Util.GetDiamond ().ToString ();
		oldGems = Util.GetDiamond ();
		txtLastScore.text = Util.GetLastScore ().ToString ();
		char[] charAlpha = GameData.EndlessWord.ToCharArray ();
		char[] filledCharAlpha = Util.getFilledEndlessWord ().ToCharArray ();
//		Debug.Log ("Current GameMode : " + GameData.CurrentGameMode.ToString ());

		GameData.FilledLevelWord = "";

		if (GameData.CurrentGameMode == 0) {
			for (int i = 0; i < alpha.Length; i++) {
				alpha [i].text = "";
			}

			for (int i = 0; i < charAlpha.Length; i++) {
//				Debug.Log (charAlpha [i].ToString ());
				alpha [i].text = charAlpha [i].ToString ();

				alpha [i].color = Color.white;
			}

			for (int i = 0; i < filledCharAlpha.Length; i++) {
				alpha [i].color = Color.black;			
			}

		}
	}

	public void ShowAds ()
	{
		#if UNITY_ANDROID
		//Firebase
		FirebaseAnalytics.SetCurrentScreen ("VideoAds_Button", this.name);
		#endif
		if (Advertisement.IsReady ("rewardedVideo")) {
			Debug.Log ("Ads");
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show ("rewardedVideo", options);
		}
	}


	private void HandleShowResult (ShowResult result)
	{
		switch (result) {
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			adreword = 2;
			StartCoroutine ("UpdateGems");
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

	IEnumerator UpdateGems ()
	{

		yield return new WaitForSeconds (1f);

		for (int i = 1; i <= adreword; i++) {
			txtGems.text = (oldGems + i).ToString ();
			yield return new WaitForSeconds (0.5f);
		}
		oldGems = int.Parse (txtGems.text);

		yield return new WaitForSeconds (0.1f);

		Util.SetDiamond (oldGems);
		StopCoroutine ("UpdateGems");

		GameObject.Find ("_GameOverPanel").GetComponent<GameOver> ().UpdateScore ();
	}
}
