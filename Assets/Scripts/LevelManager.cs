using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("All Levels")]
    public LevelData[] levels;

    // Currently selected level
    public static int selectedLevelIndex = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetUnlockedLevelCount()
    {
        int count = 1; // Level 1 always unlocked
        for (int i = 0; i < levels.Length - 1; i++)
        {
            if (GetStars(i) > 0)
                count = i + 2; // Unlock next level
        }
        return Mathf.Min(count, levels.Length);
    }

    public int GetStars(int levelIndex)
    {
        return PlayerPrefs.GetInt($"Level_{levelIndex}_Stars", 0);
    }

    public void SetStars(int levelIndex, int stars)
    {
        int current = GetStars(levelIndex);
        if (stars > current)
            PlayerPrefs.SetInt($"Level_{levelIndex}_Stars", stars);
    }

    public LevelData GetCurrentLevel()
    {
        return levels[selectedLevelIndex];
    }
}