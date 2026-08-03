using UnityEngine;
using TMPro;

public class LevelInfoDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI levelNumberText;

    void Start()
    {
        if (LevelManager.Instance == null) return;

        LevelData level = LevelManager.Instance.GetCurrentLevel();
        if (level == null) return;

        if (levelNameText != null)
            levelNameText.text = level.levelName;

        if (levelNumberText != null)
            levelNumberText.text = "LvL " + level.levelNumber;
    }
}