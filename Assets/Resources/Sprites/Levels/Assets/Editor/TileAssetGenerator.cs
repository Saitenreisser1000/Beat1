using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class TileAssetGenerator : EditorWindow
{
    private Object spriteFolder;
    private string outputFolder = "Assets/Tiles/Generated";

    [MenuItem("Tools/Tile Asset Generator")]
    public static void ShowWindow()
    {
        GetWindow<TileAssetGenerator>("Tile Asset Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Sprites zu Tile-Assets", EditorStyles.boldLabel);
        spriteFolder = EditorGUILayout.ObjectField("Sprite-Ordner", spriteFolder, typeof(Object), false);
        outputFolder = EditorGUILayout.TextField("Zielordner", outputFolder);

        if (GUILayout.Button("Tile-Assets erzeugen"))
        {
            CreateTiles();
        }
    }

    void CreateTiles()
    {
        if (spriteFolder == null)
        {
            Debug.LogError("Bitte zuerst einen Ordner mit Sprites auswählen.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(spriteFolder);
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { path });

        if (!AssetDatabase.IsValidFolder(outputFolder))
            Directory.CreateDirectory(outputFolder);

        foreach (string guid in guids)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));
            if (sprite == null) continue;

            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;

            string tilePath = Path.Combine(outputFolder, $"{sprite.name}.asset");
            AssetDatabase.CreateAsset(tile, tilePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Fertig! {guids.Length} Tile-Assets wurden erzeugt und gespeichert in: {outputFolder}");
    }
}
