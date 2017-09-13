using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;


public class LevelGenerator : MonoBehaviour
{

	//Navigation Buttons
	public Button btnFirst, btnLast, btnLeft, btnRight, btnAddLevel, btnSubmit;

	//Edit Mode and other toggles
	public Toggle tglEditMode, tglGems, tglWord, tglTime, tglPoint, tglColor;

	//DropDown
	public Image dropdColor;
	public List<Color> colors;
	public GameObject ColorContainer, ColorGrids;

	//Input Fields
	public InputField inGems, inWord, inTime, inPoint, inColorTiles;

	//Level Lable Text
	public Text lblLevel;

	List<Levels_Master> levels;

	private int LevelNo = 0;
	DataService ds = null;

	void Start ()
	{
		ds = new DataService ("LevelsDatabase.db");

		StartSync ();
		LoadLevelData ();
		DisplayLevel ();
	}

	public void CheckToggleState ()
	{
		if (tglEditMode.isOn) {
			btnSubmit.gameObject.SetActive (true);
			inGems.readOnly = false;
			inWord.readOnly = false;
			inPoint.readOnly = false;
			inTime.readOnly = false;
			inColorTiles.readOnly = false;
			dropdColor.GetComponent<Button> ().interactable = true;

		} else {
			btnSubmit.gameObject.SetActive (false);
			inGems.readOnly = true;
			inWord.readOnly = true;
			inPoint.readOnly = true;
			inTime.readOnly = true;
			inColorTiles.readOnly = true;
			dropdColor.GetComponent<Button> ().interactable = true;
		}
		if (tglGems.isOn) {
			inGems.gameObject.SetActive (true);
		} else {
			inGems.text = "";
			inGems.gameObject.SetActive (false);
		}
		if (tglWord.isOn) {
			inWord.gameObject.SetActive (true);
		} else {
			inWord.text = "";
			inWord.gameObject.SetActive (false);
		}
		if (tglTime.isOn) {
			inTime.gameObject.SetActive (true);
		} else {
			inTime.text = "";
			inTime.gameObject.SetActive (false);
		}
		if (tglPoint.isOn) {
			inPoint.gameObject.SetActive (true);
		} else {
			inPoint.text = "";
			inPoint.gameObject.SetActive (false);
		}
		if (tglColor.isOn) {
			dropdColor.gameObject.SetActive (true);
			inColorTiles.gameObject.SetActive (true);
		} else {
			inColorTiles.text = "";
			dropdColor.color = Color.white; 
			dropdColor.gameObject.SetActive (false);
			inColorTiles.gameObject.SetActive (false);
		}
	}

	public void LoadColor ()
	{
		ColorContainer.SetActive (true);
		Button[] colorimage = ColorGrids.GetComponentsInChildren<Button> ();

		for (int i = 0; i < colorimage.Length; i++) {
			colorimage [i].gameObject.GetComponent<Image> ().color = colors [i];
			colorset (colorimage [i], colors [i]);
		}
	}

	void colorset (Button btn, Color color)
	{
		btn.onClick.RemoveAllListeners ();
		btn.onClick.AddListener (() => SetColor (color));
	}


	public void SetColor (Color rgb)
	{
		dropdColor.color = rgb;
		ColorContainer.SetActive (false);

	}

	private void LoadLevelData ()
	{
		
		IEnumerable<Levels_Master> data = ds.GetLevel ();
		levels = data.ToList ();

		Debug.Log (levels [0].Word);
//		foreach (var level in levels) {
//			
//			Debug.Log (level.Gems.ToString () + "," + level.Word + "," + level.Time.ToString () + "," + level.Point.ToString () + "," + level.Color_Code.ToString () + "," + level.No_Of_ColoredTiles.ToString ());
//
//		}	
	}


	public void CallLeft ()
	{
		if (LevelNo < 1) {
			LevelNo = 0;
			btnLeft.gameObject.SetActive (false);
		} else {
			btnLeft.gameObject.SetActive (true);
			LevelNo--;
		}
		btnRight.gameObject.SetActive (true);
		DisplayLevel ();
	}

	public void CallRight ()
	{

		if (LevelNo > levels.Count - 2) {
			LevelNo = levels.Count - 1;
			btnRight.gameObject.SetActive (false);
		} else {
			btnRight.gameObject.SetActive (true);
			LevelNo++;
		}
		btnLeft.gameObject.SetActive (true);

		DisplayLevel ();
		
	}

	public void CallFirst ()
	{
		LevelNo = 0;
		DisplayLevel ();
	}

	public void CallLast ()
	{
		LevelNo = levels.Count - 1;
		DisplayLevel ();
	}

	 


	void DisplayLevel ()
	{
		if (levels.Count > 0) {
			
			tglGems.isOn = false;
			tglWord.isOn = false;
			tglTime.isOn = false;
			tglPoint.isOn = false;
			tglColor.isOn = false;

			lblLevel.text = levels [LevelNo].Level_Id.ToString ();
			if (levels [LevelNo].Gems != 0) {
				tglGems.isOn = true;
				inGems.text = levels [LevelNo].Gems.ToString ();
			}
			if (levels [LevelNo].Word != "Null") {
				tglWord.isOn = true;
				inWord.text = levels [LevelNo].Word.ToString ();
			}
			if (levels [LevelNo].Time != 0) {
				tglTime.isOn = true;
				inTime.text = levels [LevelNo].Time.ToString ();
			}
			if (levels [LevelNo].Point != 0) {
				tglPoint.isOn = true;
				inPoint.text = levels [LevelNo].Point.ToString ();
			}
			if (levels [LevelNo].No_Of_ColoredTiles != 0) {
				tglColor.isOn = true;
				inColorTiles.text = levels [LevelNo].No_Of_ColoredTiles.ToString ();
				string[] rgb = levels [LevelNo].Color_Code.Split (',');
				dropdColor.color = new Color (float.Parse (rgb [0]), float.Parse (rgb [1]), float.Parse (rgb [2]), float.Parse (rgb [3]));
			}
		}
	}

	public void NewLevel ()
	{
		lblLevel.text = (levels.Count + 1).ToString ();
		tglGems.isOn = false;
		tglWord.isOn = false;
		tglTime.isOn = false;
		tglPoint.isOn = false;
		tglColor.isOn = false;

	}

	public void AddNewLevel ()
	{
		Levels_Master level = new Levels_Master ();
		if (tglGems.isOn) {
			level.Gems = int.Parse (inGems.text);
		} else {
			level.Gems = 0;
		}
		if (tglWord.isOn) {
			level.Word = inWord.gameObject.GetComponentsInChildren<Text> () [1].text;
		} else {
			level.Word = "Null";
		}
		if (tglTime.isOn) {
			level.Time = float.Parse (inTime.text);
		} else {
			level.Point = 0;
		}
		if (tglPoint.isOn) {
			level.Point = int.Parse (inPoint.text);
		} else {
			level.Point = 0;
		}
		if (tglColor.isOn) {
			level.Color_Code = dropdColor.color.r.ToString () + "," + dropdColor.color.g.ToString () + "," + dropdColor.color.b.ToString () + "," + dropdColor.color.a.ToString ();
			Debug.Log (level.Color_Code);
			level.No_Of_ColoredTiles = int.Parse (inColorTiles.text);
		} else {
			level.Color_Code = "Null";
			level.No_Of_ColoredTiles = 0;
		}

		level.Is_Locked = true;

		if (tglEditMode.isOn && int.Parse (lblLevel.text) <= levels.Count && levels.Count != 0) {
			Debug.Log ("Old");
			ds.UpdateLevel (level, int.Parse (lblLevel.text));

		} else {
			Debug.Log ("New");
			ds.SetLevel (level);
		}

		LoadLevelData ();

	}


	private void StartSync ()
	{
		if (!File.Exists (ds.dbPath)) {
			Debug.Log ("Exist");
			ds.CreateDB ();
		}

	}
}
