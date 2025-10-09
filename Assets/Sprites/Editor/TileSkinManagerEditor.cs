using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileSkinManager))]
public class TileSkinManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileSkinManager manager = (TileSkinManager)target;

        GUILayout.Space(10);
        GUILayout.Label("🎨 Skins wechseln", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Beat")) manager.ApplySkin("Beat");
        if (GUILayout.Button("Punk")) manager.ApplySkin("Punk");
        if (GUILayout.Button("Tracht")) manager.ApplySkin("Tracht");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Klassik")) manager.ApplySkin("Klassik");
        if (GUILayout.Button("Reggae")) manager.ApplySkin("Reggae");
        if (GUILayout.Button("Metal")) manager.ApplySkin("Metal");
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);
        if (GUILayout.Button("Tiles automatisch laden"))
        {
            manager.AutoFillTileLists();
        }
    }
}
