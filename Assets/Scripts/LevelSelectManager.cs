using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectManager : MonoBehaviour
{
    [Header("References")]
    public LevelManager levelManager;
    public GameObject levelCardPrefab;
    public Transform cardContainer;

    [Header("Navigation")]
    public Button leftButton;
    public Button rightButton;

    [Header("Card Settings")]
    public float cardSpacing = 1080f;
    public float swipeSpeed = 5f;

    private int currentIndex = 0;
    private Vector3 targetPosition;
    private GameObject[] cards;

    void Start()
    {
        SpawnCards();

        // Jump straight to last selected level
        currentIndex = LevelManager.selectedLevelIndex;
        targetPosition = new Vector3(-currentIndex * cardSpacing, 0, 0);
        cardContainer.localPosition = targetPosition;

        UpdateButtons();
    }

    void SpawnCards()
    {
        int levelCount = levelManager.levels.Length;
        cards = new GameObject[levelCount];

        for (int i = 0; i < levelCount; i++)
        {
            GameObject card = Instantiate(levelCardPrefab, cardContainer);
            card.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(i * cardSpacing, -200f);

            LevelCard lc = card.GetComponent<LevelCard>();
            lc.Setup(levelManager.levels[i], i,
                     i < levelManager.GetUnlockedLevelCount(),
                     levelManager.GetStars(i));

            cards[i] = card;
        }

        targetPosition = cardContainer.localPosition;
    }

    void Update()
    {
        cardContainer.localPosition = Vector3.Lerp(
            cardContainer.localPosition, targetPosition,
            Time.deltaTime * swipeSpeed);
    }

    public void SwipeLeft()
    {
        if (currentIndex < levelManager.levels.Length - 1)
        {
            currentIndex++;
            targetPosition = new Vector3(-currentIndex * cardSpacing, 0, 0);
            UpdateButtons();
        }
    }

    public void SwipeRight()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            targetPosition = new Vector3(-currentIndex * cardSpacing, 0, 0);
            UpdateButtons();
        }
    }

    void UpdateButtons()
    {
        if (leftButton != null)
            leftButton.interactable = currentIndex > 0;
        if (rightButton != null)
            rightButton.interactable =
                currentIndex < levelManager.levels.Length - 1;
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}