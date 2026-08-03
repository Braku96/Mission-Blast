using UnityEngine;
using TMPro;
using System.Collections;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText; // Big centered text for 3,2,1,Start!

    private float timeRemaining;
    private bool timerRunning = false;
    private bool countdownDone = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Get time limit from current level
        LevelData currentLevel = LevelManager.Instance.GetCurrentLevel();
        timeRemaining = currentLevel != null ? currentLevel.timeLimit : 120f;

        // Disable input during countdown
        InputHandler input = FindObjectOfType<InputHandler>();
        if (input != null) input.enabled = false;

        UpdateTimerUI();
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        if (!timerRunning || !countdownDone) return;

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(timeRemaining, 0f);
        UpdateTimerUI();

        if (timeRemaining <= 0f)
        {
            timerRunning = false;
            OnTimeOut();
        }
    }

    IEnumerator StartCountdown()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        string[] steps = { "3", "2", "1", "Start!" };

        foreach (string step in steps)
        {
            if (countdownText != null)
            {
                countdownText.text = step;
                // Pop scale animation
                countdownText.transform.localScale = Vector3.one * 1.5f;
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime / 0.3f;
                    countdownText.transform.localScale =
                        Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t);
                    yield return null;
                }
            }
            yield return new WaitForSeconds(0.7f);
        }

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        // Re-enable input and start timer
        InputHandler input = FindObjectOfType<InputHandler>();
        if (input != null) input.enabled = true;

        countdownDone = true;
        timerRunning = true;
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int seconds = Mathf.CeilToInt(timeRemaining);
        int minutes = seconds / 60;
        int secs = seconds % 60;
        timerText.text = string.Format("{0}:{1:00}", minutes, secs);

        // Turn red when under 10 seconds
        timerText.color = timeRemaining <= 10f
            ? Color.red
            : Color.white;
    }

    void OnTimeOut()
    {
        // Disable input
        InputHandler input = FindObjectOfType<InputHandler>();
        if (input != null) input.enabled = false;

        // Check score against level thresholds
        LevelData level = LevelManager.Instance.GetCurrentLevel();
        int score = ScoreManager.Instance.GetScore();

        if (level != null && score >= level.targetScoreOneStar)
        {
            // Passed — trigger level complete
            LevelCompleteManager.Instance.ShowLevelComplete(score, level);
        }
        else
        {
            // Failed — trigger game over
            GameManager.Instance.TriggerGameOver();
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }
}