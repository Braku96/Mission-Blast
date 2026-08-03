using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelCard : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI levelNumberText;
    public TextMeshProUGUI scoreThresholdsText;
    public TextMeshProUGUI playerBestScoreText;
    public GameObject[] stars; // 3 star objects
    public GameObject lockIcon;
    public GameObject playButton;
    public Image cardBackground;
    public Image levelImage;  // Add this line here

    [Header("Colors")]
    public Color unlockedColor = new Color(0.08f, 0.08f, 0.18f);
    public Color lockedColor = new Color(0.05f, 0.05f, 0.1f);

    private int levelIndex;
    private bool isUnlocked;

    public void Setup(LevelData data, int index, bool unlocked, int earnedStars)
    {
        levelIndex = index;
        isUnlocked = unlocked;

        if (levelNumberText != null)
            levelNumberText.text = $"LEVEL {data.levelNumber}";
        if (levelNameText != null)
            levelNameText.text = data.levelName;

        // Set stars
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].SetActive(i < earnedStars);
        }

        // Lock/unlock
        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);
        if (playButton != null)
            playButton.SetActive(unlocked);
        if (cardBackground != null)
            cardBackground.color = unlocked ? unlockedColor : lockedColor;
        // Show score thresholds
        if (scoreThresholdsText != null)
        {
            if (unlocked)
                scoreThresholdsText.text =
                    $"<voffset=0.35em><sprite=0></voffset> {data.targetScoreOneStar}\n" +
                    $"<voffset=0.35em><sprite=0><sprite=0></voffset> {data.targetScoreTwoStar}\n" +
                    $"<voffset=0.35em><sprite=0><sprite=0><sprite=0></voffset> {data.targetScoreThreeStar}";
            else
                scoreThresholdsText.text = "";
        }

        // Show player's best score for this level
        if (playerBestScoreText != null)
        {
            int best = PlayerPrefs.GetInt($"Level_{index}_BestScore", 0);
            if (unlocked && best > 0)
                playerBestScoreText.text = "BEST: " + best;
            else if (unlocked)
                playerBestScoreText.text = "BEST: -";
            else
                playerBestScoreText.text = "";
        }
        if (levelImage != null && data.levelCardImage != null)
            levelImage.sprite = data.levelCardImage;
    }

    public void PlayLevel()
    {
        if (!isUnlocked) return;
        LevelManager.selectedLevelIndex = levelIndex;
        SceneManager.LoadScene("SampleScene");
    }
}