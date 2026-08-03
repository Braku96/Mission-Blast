using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "BlockBlast/Level")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber;
    public string levelName;
    public Sprite levelCardImage;

    [Header("Goals")]
    public int targetScoreOneStar;
    public int targetScoreTwoStar;
    public int targetScoreThreeStar;
    public float timeLimit = 120f; // seconds

    [Header("Grid")]
    public bool[] blockedCells = new bool[81]; // 9x9 grid
}