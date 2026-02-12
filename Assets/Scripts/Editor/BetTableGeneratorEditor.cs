/*
 * Deterministic Roulette Game - Bet Table Generator Editor
 * 
 * Author Berkan Özgür
 * 
 * Custom inspector for BetTableGenerator.
 */

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BetTableGenerator))]
public class BetTableGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        BetTableGenerator generator = (BetTableGenerator)target;

        EditorGUILayout.LabelField("Generation Tools", EditorStyles.boldLabel);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Generate Bet Locations", GUILayout.Height(40)))
        {
            Undo.RecordObject(generator.transform, "Generate Bet Locations");
            generator.GenerateAllBetLocations();
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clear All Bet Locations", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Clear Bet Locations",
                "Are you sure you want to delete all bet locations?",
                "Yes", "Cancel"))
            {
                Undo.RecordObject(generator.transform, "Clear Bet Locations");
                generator.ClearAllBetLocations();
            }
        }

        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "1. Set up table dimensions and start position\n" +
            "2. Assign materials for visual distinction\n" +
            "3. Toggle bet types to generate\n" +
            "4. Click 'Generate Bet Locations'\n" +
            "5. Adjust parameters and regenerate if needed",
            MessageType.Info
        );
    }
}