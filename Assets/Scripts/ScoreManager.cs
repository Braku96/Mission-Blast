using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;

    private int score = 0;
    private int best = 0;

    void Awake()
    {
        Instance = this;
        best = PlayerPrefs.GetInt("BestScore", 0);
    }

    public void AddScore(int points)
    {
        score += points;
        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt("BestScore", best);
        }

        // Save per-level best score
        if (LevelManager.Instance != null)
        {
            int levelIndex = LevelManager.selectedLevelIndex;
            int levelBest = PlayerPrefs.GetInt($"Level_{levelIndex}_BestScore", 0);
            if (score > levelBest)
                PlayerPrefs.SetInt($"Level_{levelIndex}_BestScore", score);
        }

        UpdateUI();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = score.ToString();
        if (bestText != null) bestText.text = best.ToString();
    }
    public int GetScore()
    {
        return score;
    }
}