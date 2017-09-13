using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
	public static List<Levels_Master> Levels;
	public static int CurrentGameMode = 0;
	public static int CurrentLevel = 0;
	public static bool isDay = true;
	public static string LevelWord = "";
	public static string FilledLevelWord = "";
	public static string EndlessWord = "";
	public static string FilledEndlessWord = "";
	public static string LeaderboardTopScoreID = "ColorHop_Leaderboard";
	public static string LeaderboardLevelsID = "ColorHop_Levels";
	public static string AchivementsAwards = "Milestone1";
	public static  int playerSession = 0;
	public static bool displayBannerAds = false;

}
