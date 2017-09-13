using SQLite4Unity3d;
using UnityEngine;

#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;

public class DataService
{

	private SQLiteConnection _connection;
	public string dbPath = "";

	public DataService (string DatabaseName)
	{


#if UNITY_EDITOR
		dbPath = string.Format (@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
		_connection = new SQLiteConnection (dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
		Debug.Log ("Final PATH: " + dbPath);     

	}

	public void CreateDB ()
	{
		_connection.DropTable<Levels_Master> ();
		_connection.CreateTable<Levels_Master> ();
	}

	public void SetLevel (Levels_Master level)
	{
		_connection.Insert (level);
	}

	public void UpdateLevel (Levels_Master level, int levelno)
	{
		string com = "update Levels_Master set Gems =" + level.Gems + ", Word='" + level.Word + "', Time=" + level.Time + " ,Point=" + level.Point + " ,Color_Code=" + level.Color_Code + " ,No_Of_ColoredTiles=" + level.No_Of_ColoredTiles + " WHERE Level_Id =" + levelno;
		SQLiteCommand cmd = new SQLiteCommand (_connection);
		cmd.CommandText = com;
		cmd.ExecuteNonQuery ();
	}

	public void ChangeLevelStatus (int levelno, int status)
	{
		Debug.Log ("ChangeLevelStatus");
		string com = "update Levels_Master set Is_Locked =" + status + " WHERE Level_Id =" + levelno;
		SQLiteCommand cmd = new SQLiteCommand (_connection);
		cmd.CommandText = com;
		cmd.ExecuteNonQuery ();
	}

	public int GetLastUpdateLevel ()
	{
		string com = "Select Level_Id from Levels_Master where Is_Locked=" + true + " ORDER BY _id DESC LIMIT 1";
		SQLiteCommand cmd = new SQLiteCommand (_connection);
		cmd.CommandText = com;
		return cmd.ExecuteNonQuery ();
	}

	
	public IEnumerable<Levels_Master> GetLevel ()
	{
		return _connection.Table<Levels_Master> ();
	}

	public Levels_Master GetSingleLevel ()
	{

		return _connection.Table<Levels_Master> ().Where (x => x.Level_Id == GameData.CurrentLevel).FirstOrDefault ();
	}

	public Levels_Master CreateLevel (Levels_Master level)
	{
		_connection.Insert (level);
		return level;
	}

	public void ABC ()
	{
		Debug.Log ("AA");
	}
}
