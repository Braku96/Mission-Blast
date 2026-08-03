using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public GridManager gridManager;
    public PieceSpawner pieceSpawner;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    private bool isGameOver = false;
    private bool isLevelComplete = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void CheckGameOver()
    {
        // Don't check if game already ended
        if (isGameOver || isLevelComplete) return;

        for (int slot = 0; slot < 3; slot++)
        {
            PieceData piece = pieceSpawner.currentPieces[slot];
            if (piece == null) continue;

            for (int row = 0; row < gridManager.gridSize; row++)
                for (int col = 0; col < gridManager.gridSize; col++)
                    if (gridManager.CanPlace(piece, new Vector2Int(col, row)))
                        return;
        }

        TriggerGameOver();
    }

    public void TriggerGameOver()
    {
        if (isGameOver || isLevelComplete) return;
        isGameOver = true;

        // Stop timer
        if (TimerManager.Instance != null)
            TimerManager.Instance.StopTimer();

        // Disable input
        InputHandler input = FindObjectOfType<InputHandler>();
        if (input != null) input.enabled = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameOver();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "SCORE\n" + ScoreManager.Instance.GetScore();
    }

    public void SetLevelComplete()
    {
        isLevelComplete = true;

        // Stop timer
        if (TimerManager.Instance != null)
            TimerManager.Instance.StopTimer();
    }

    public void RestartGame()
    {
        isGameOver = false;
        isLevelComplete = false;
        ScoreManager.Instance.ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        isGameOver = false;
        isLevelComplete = false;
        ScoreManager.Instance.ResetScore();
        NavigationManager.Instance.GoToMainMenu();
    }
}