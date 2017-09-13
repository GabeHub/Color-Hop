using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_ANDROID
using GooglePlayGames;
#endif
using UnityEngine.SocialPlatforms;

public class LeaderboardManager : MonoBehaviour
{

	#region DEFAULT_UNITY_CALLBACKS

	void Awake ()
	{

		#if UNITY_ANDROID
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;

		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate ();

		#endif

		AuthenticateToGameCenter ();

	}

	#endregion

	#region BUTTON_CALLBACKS

	public static void  AuthenticateToGameCenter ()
	{
		Social.localUser.Authenticate ((bool success) => {
			if (success) {
				Debug.Log ("Login Sucess");
			} else {
				Debug.Log ("Login failed");
			}
		});
	}

	public static void ReportScore (long score, string leaderboardID)
	{
		//debugText=("Reporting score " + score + " on leaderboard " + leaderboardID);
		Social.ReportScore (score, leaderboardID, (bool success) => {
			if (success) {
				Debug.Log ("Update Score Success");
			} else {
				Debug.Log ("Update Score Fail");
			}
			Debug.Log (success ? "Reported score successfully" : "Failed to report score");
			Debug.Log ("New Score:" + score);  
		});
	}

	//	public static void ReportAchivements (int points, string achivementID)
	//	{
	//		Social.ReportProgress (achivementID, double.Parse (points.ToString ()), (bool success) => {
	//			if (success) {
	//				Debug.Log ("Reported Achivements successfully");
	//			} else {
	//				Debug.Log ("Failed to report Achivements");
	//			}
	//
	//			Debug.Log (success ? "Reported score successfully" : "Failed to report score");
	//			Debug.Log ("New Score:" + points);
	//		});
	//
	//	}

	public static void ShowLeaderboard ()
	{
		Social.ShowLeaderboardUI ();
	}


	public static void ShowAchivements ()
	{
		Social.ShowAchievementsUI ();
	}



	#endregion



}
