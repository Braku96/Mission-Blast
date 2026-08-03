using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelCompleteManager : MonoBehaviour
{
    public static LevelCompleteManager Instance;

    [Header("Panel")]
    public GameObject levelCompletePanel;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelNameText;
    public GameObject[] stars; // 3 star objects

    void Awake()
    {
        Instance = this;
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }

    public void ShowLevelComplete(int score, LevelData level)
    {
        // Calculate earned stars
        int earned = 0;
        if (score >= level.targetScoreThreeStar) earned = 3;
        else if (score >= level.targetScoreTwoStar) earned = 2;
        else if (score >= level.targetScoreOneStar) earned = 1;

        // Save stars — only if better than previous
        int levelIndex = LevelManager.selectedLevelIndex;
        LevelManager.Instance.SetStars(levelIndex, earned);

        // Unlock next level implicitly via SetStars saving progress
        // Show panel
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        // Populate UI
        if (scoreText != null)
            scoreText.text = "SCORE\n" + score;

        if (levelNameText != null)
            levelNameText.text = level.levelName;

        // Set stars
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].SetActive(i < earned);
        }

        // Play sound
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLevelComplete();

        // Stop timer just in case
        if (TimerManager.Instance != null)
            TimerManager.Instance.StopTimer();

        // Tell GameManager level is complete so CheckGameOver won't fire
        if (GameManager.Instance != null)
            GameManager.Instance.SetLevelComplete();

        // Disable input
        InputHandler input = FindObjectOfType<InputHandler>();
        if (input != null) input.enabled = false;

        StartCoroutine(AnimateStars(earned));
    }

    IEnumerator AnimateStars(int earned)
    {
        // Hide all stars first
        foreach (var star in stars)
            if (star != null) star.SetActive(false);

        yield return new WaitForSeconds(0.3f);

        // Reveal stars one by one with a pop
        for (int i = 0; i < earned; i++)
        {
            if (stars[i] != null)
            {
                stars[i].SetActive(true);
                yield return StartCoroutine(PopStar(stars[i]));
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    IEnumerator PopStar(GameObject star)
    {
        star.transform.localScale = Vector3.zero;
        float t = 0f;
        // Scale up beyond normal
        while (t < 1f)
        {
            t += Time.deltaTime / 0.15f;
            star.transform.localScale = Vector3.Lerp(
                Vector3.zero, Vector3.one * 1.4f, t);
            yield return null;
        }
        t = 0f;
        // Settle back to normal
        while (t < 1f)
        {
            t += Time.deltaTime / 0.1f;
            star.transform.localScale = Vector3.Lerp(
                Vector3.one * 1.4f, Vector3.one, t);
            yield return null;
        }
        star.transform.localScale = Vector3.one;
    }

    public void NextLevel()
    {
        int next = LevelManager.selectedLevelIndex + 1;
        if (next < LevelManager.Instance.levels.Length)
        {
            LevelManager.selectedLevelIndex = next;
            ScoreManager.Instance.ResetScore();
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            // No more levels — go to main menu
            NavigationManager.Instance.GoToMainMenu();
        }
    }

    public void ReplayLevel()
    {
        ScoreManager.Instance.ResetScore();
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToMainMenu()
    {
        ScoreManager.Instance.ResetScore();
        NavigationManager.Instance.GoToMainMenu();
    }
}