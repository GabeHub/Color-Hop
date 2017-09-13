
/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
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

//myFix
using DG.Tweening;

#if AADOTWEEN
using DG.Tweening;
#endif
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif



/// <summary>
/// Utility class.
/// </summary>
public static class Util
{
	static System.Random random = new System.Random ();

	public static double GetRandomNumber (double minimum, double maximum)
	{
		return random.NextDouble () * (maximum - minimum) + minimum;
	}

	public static float GetRandomNumber (float minimum, float maximum)
	{ 

//			float value = (float)random.NextDouble () * (maximum - minimum) + minimum;
//			Debug.Log(value.ToString());
//			return value;
		return (float)random.NextDouble () * (maximum - minimum) + minimum;
	}

	/// <summary>
	/// Real shuffle of List
	/// </summary>
	public static void Shuffle<T> (this IList<T> list)
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = random.Next (n + 1);  
			T value = list [k];  
			list [k] = list [n];  
			list [n] = value;  
		}  
	}

	public static void SetLastScore (int score)
	{
		PlayerPrefs.SetInt ("_LASTSCORE", score);

		PlayerPrefs.Save ();

		SetBestScore (score);

		PlayerPrefs.Save ();
	}

	public static void SetDiamond (int diamond)
	{
		if (PlayerPrefs.GetInt ("_DIAMOND") < diamond) {
			PlayerPrefs.SetInt ("_DIAMOND", diamond);
		}
		#if UNITY_IOS && !UNITY_EDITOR
		LeaderboardManager.ReportScore (Util.GetDiamond (), GameData.LeaderboardTopScoreID);
		#elif UNITY_ANDROID && !UNITY_EDITOR
		LeaderboardManager.ReportScore (Util.GetDiamond (), LeaderboardManagerAndroid.leaderboard_colorhop_topscore);
		#endif
		PlayerPrefs.Save ();
	}



	public static int GetDiamond ()
	{
		return PlayerPrefs.GetInt ("_DIAMOND", 0);
	}

	static void SetBestScore (int score)
	{
		int b = GetBestScore ();

		if (score > b) {
			PlayerPrefs.SetInt ("_BESTSCORE", score);
//			if (score % 30 == 0 && score > 0) {
//				#if UNITY_IOS
//				LeaderboardManager.ReportAchivements (score, GameData.AchivementsAwards);
//				#else
//				LeaderboardManager.ReportAchivements (Util.GetDiamond (), LeaderboardManagerAndroid.achievement_milestone_1);
//				#endif
//			}
		}
	}


	public static int GetBestScore ()
	{
		return PlayerPrefs.GetInt ("_BESTSCORE", 0);
	}

	public static int GetLastScore ()
	{
		return PlayerPrefs.GetInt ("_LASTSCORE", 0);
	}

	public static int GetPlayerSession ()
	{
		return PlayerPrefs.GetInt ("PlayerSession", 0);
	}

	public static int GetVolume ()
	{
		return PlayerPrefs.GetInt ("Volume", 1);
	}

	public static void setEndlessWord (string spell)
	{

		if (PlayerPrefs.GetString ("_EndlessWord", "") == PlayerPrefs.GetString ("_FilledEndlessWord", "")) {
			PlayerPrefs.SetString ("_EndlessWord", spell);
		} else {
			GameData.EndlessWord = PlayerPrefs.GetString ("_EndlessWord", "");
		}
		Debug.Log ("_EndlessWord :" + PlayerPrefs.GetString ("_EndlessWord"));

	}

	public static void setFilledEndlessWord (string spell)
	{
		if (PlayerPrefs.GetString ("_EndlessWord", "") != spell) {
			PlayerPrefs.SetString ("_FilledEndlessWord", spell);
		} else {
			SetDiamond (GetDiamond () + 20);
		}
	}

	public static string getFilledEndlessWord ()
	{
//		Debug.Log ("_EndlessWord :" + PlayerPrefs.GetString ("_EndlessWord"));

		return PlayerPrefs.GetString ("_EndlessWord");
	}

	public static string getEndlessWord ()
	{
		return PlayerPrefs.GetString ("_FilledEndlessWord");
	}

	/// <summary>
	/// Clean the memory and reload the scene
	/// </summary>
	public static void ReloadLevel ()
	{
		CleanMemory ();
	}

	/// <summary>
	/// Clean the memory
	/// </summary>
	public static void CleanMemory ()
	{
        //myFix
        DOTween.KillAll ();

		#if AADOTWEEN
		DOTween.KillAll ();
		#endif
		GC.Collect ();
		Application.targetFrameRate = 60;
	}

	public static void SetAlpha (this Text text, float alpha)
	{
		Color c = text.color;
		c.a = alpha;
		text.color = c;
	}

	public static void SetScaleX (this RectTransform rect, float scale)
	{
		var s = rect.localScale;
		s.x = scale;
		rect.localScale = s;
	}
}
