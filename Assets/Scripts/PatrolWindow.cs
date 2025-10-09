using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PatrolWindow : EditorWindow
{
    private List<GameObject> patrolObjects = new List<GameObject>();
    private Vector2 scrollPos;

    [MenuItem("Window/Patrol Manager")]
    public static void ShowWindow()
    {
        GetWindow<PatrolWindow>("Patrol Manager");
    }

    void OnGUI()
    {
        GUILayout.Label("Patrouillen Objekte", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < patrolObjects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            patrolObjects[i] = (GameObject)EditorGUILayout.ObjectField(patrolObjects[i], typeof(GameObject), true);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                patrolObjects.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Objekt hinzufügen"))
        {
            patrolObjects.Add(null);
        }
    }
}
