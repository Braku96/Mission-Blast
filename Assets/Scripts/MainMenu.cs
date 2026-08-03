using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI bestScoreText;

    void Start()
    {
        // Load and display best score
        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (bestScoreText != null)
            bestScoreText.text = "BEST: " + best;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}