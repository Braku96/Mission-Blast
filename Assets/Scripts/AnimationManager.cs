using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    void Awake()
    {
        Instance = this;
    }

    // Called when a piece is placed — pops each cell slightly
    public void PlayPlacePop(List<GameObject> cells)
    {
        foreach (var cell in cells)
        {
            if (cell != null)
                StartCoroutine(PopCell(cell));
        }
    }

    // Called when lines/zones are cleared — flashes cells white then fades
    public void PlayClearFlash(List<GameObject> cells, System.Action onComplete)
    {
        StartCoroutine(FlashAndClear(cells, onComplete));
    }

    // Called when score increases — floats a text upward
    public void PlayScorePopup(Vector3 worldPos, int points)
    {
        StartCoroutine(ScoreFloat(worldPos, points));
    }

    IEnumerator PopCell(GameObject cell)
    {
        if (cell == null) yield break;
        Vector3 original = cell.transform.localScale;
        Vector3 big = original * 1.3f;

        float t = 0f;
        // Scale up
        while (t < 1f)
        {
            t += Time.deltaTime / 0.07f;
            if (cell == null) yield break;
            cell.transform.localScale = Vector3.Lerp(original, big, t);
            yield return null;
        }
        t = 0f;
        // Scale back down
        while (t < 1f)
        {
            t += Time.deltaTime / 0.07f;
            if (cell == null) yield break;
            cell.transform.localScale = Vector3.Lerp(big, original, t);
            yield return null;
        }
        if (cell != null)
            cell.transform.localScale = original;
    }

    IEnumerator FlashAndClear(List<GameObject> cells, System.Action onComplete)
    {
        GridManager grid = FindObjectOfType<GridManager>();

        // Flash white — skip blocked cells
        foreach (var cell in cells)
        {
            if (cell == null) continue;
            if (grid != null && IsBlockedCell(grid, cell)) continue;
            cell.GetComponent<SpriteRenderer>().color = Color.white;
        }
        yield return new WaitForSeconds(0.08f);

        // Flash cyan — skip blocked cells
        foreach (var cell in cells)
        {
            if (cell == null) continue;
            if (grid != null && IsBlockedCell(grid, cell)) continue;
            cell.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.9f, 0.9f);
        }
        yield return new WaitForSeconds(0.08f);

        // Flash white again — skip blocked cells
        foreach (var cell in cells)
        {
            if (cell == null) continue;
            if (grid != null && IsBlockedCell(grid, cell)) continue;
            cell.GetComponent<SpriteRenderer>().color = Color.white;
        }
        yield return new WaitForSeconds(0.08f);

        onComplete?.Invoke();
    }

    bool IsBlockedCell(GridManager gridManager, GameObject cell)
    {
        // Parse cell name "Cell_row_col" to get coordinates
        string[] parts = cell.name.Split('_');
        if (parts.Length < 3) return false;
        if (int.TryParse(parts[1], out int row) &&
            int.TryParse(parts[2], out int col))
            return gridManager.IsBlocked(row, col);
        return false;
    }

    IEnumerator ScoreFloat(Vector3 worldPos, int points)
    {
        // Create a temporary text object
        GameObject go = new GameObject("ScorePopup");
        go.transform.position = worldPos;

        var text = go.AddComponent<TextMesh>();
        text.text = "+" + points;
        text.fontSize = 24;
        text.color = Color.yellow;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;

        float t = 0f;
        float duration = 0.8f;
        Vector3 startPos = worldPos;
        Vector3 endPos = worldPos + new Vector3(0, 2f, 0);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            go.transform.position = Vector3.Lerp(startPos, endPos, t);
            // Fade out
            text.color = new Color(1f, 1f, 0f, 1f - t);
            yield return null;
        }

        Destroy(go);
    }
}