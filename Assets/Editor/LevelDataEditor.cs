using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelData level = (LevelData)target;

        // Draw all default fields except blockedCells
        DrawPropertiesExcluding(serializedObject, "blockedCells");

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Blocked Cells", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Click cells to toggle blocked/unblocked",
            EditorStyles.miniLabel);
        EditorGUILayout.Space(5);

        // Make sure array is always 81
        if (level.blockedCells == null || level.blockedCells.Length != 81)
            level.blockedCells = new bool[81];

        // Draw 9x9 grid top to bottom (row 8 at top, row 0 at bottom)
        // so it matches Unity's world space orientation
        for (int row = 8; row >= 0; row--)
        {
            EditorGUILayout.BeginHorizontal();

            // Row label
            EditorGUILayout.LabelField(row.ToString(),
                GUILayout.Width(15));

            for (int col = 0; col < 9; col++)
            {
                int index = row * 9 + col;
                bool isBlocked = level.blockedCells[index];

                // Blocked = dark gray button, unblocked = light button
                GUI.backgroundColor = isBlocked
                    ? new Color(0.2f, 0.2f, 0.2f)
                    : new Color(0.6f, 0.8f, 1f);

                if (GUILayout.Button(isBlocked ? "X" : "",
                    GUILayout.Width(28), GUILayout.Height(28)))
                {
                    Undo.RecordObject(level, "Toggle Blocked Cell");
                    level.blockedCells[index] = !isBlocked;
                    EditorUtility.SetDirty(level);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        // Reset background color
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);

        // Column labels
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(15));
        for (int col = 0; col < 9; col++)
            EditorGUILayout.LabelField(col.ToString(),
                GUILayout.Width(28));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Clear and Fill buttons for convenience
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear All"))
        {
            Undo.RecordObject(level, "Clear All Blocked Cells");
            level.blockedCells = new bool[81];
            EditorUtility.SetDirty(level);
        }
        if (GUILayout.Button("Fill All"))
        {
            Undo.RecordObject(level, "Fill All Blocked Cells");
            for (int i = 0; i < 81; i++)
                level.blockedCells[i] = true;
            EditorUtility.SetDirty(level);
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}