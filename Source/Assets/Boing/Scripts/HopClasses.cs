using SQLite4Unity3d;

public class Levels_Master
{
	[PrimaryKey, AutoIncrement]
	public int Level_Id { get; set; }

	public int Gems { get; set; }

	public string Word  { get; set; }

	public float Time { get; set; }

	public int Point { get; set; }

	public string Color_Code { get; set; }

	public float No_Of_ColoredTiles { get; set; }

	public bool Is_Locked { get; set; }

}



