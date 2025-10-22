using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways] // Damit es auch im Editor funktioniert
public class TileSkinManager : MonoBehaviour
{
    // === Referenzen & Tile-Listen ===
    [Header("Parallax-Hintergrund")]
    [SerializeField] private BackgroundManager backgroundManager;

    [Header("Ziel-Tilemaps (z. B. Boden + Deko)")]
    public List<Tilemap> tilemaps;

    // Tile-Listen für verschiedene Skins
    [HideInInspector] public List<TileBase> beatTiles;
    [HideInInspector] public List<TileBase> punkTiles;
    [HideInInspector] public List<TileBase> trachtTiles;
    [HideInInspector] public List<TileBase> klassikTiles;
    [HideInInspector] public List<TileBase> reggaeTiles;
    [HideInInspector] public List<TileBase> metalTiles;

    // Mapping: Skin-Name -> Tile-Liste
    private Dictionary<string, List<TileBase>> styleTiles;

    // === Initialisierung ===
    private void Awake()
    {
        BuildStyleDictionary(); // Dictionary beim Start aufbauen
    }

    private void OnEnable()
    {
        BuildStyleDictionary(); // Auch beim Aktivieren im Editor
    }

    /// <summary>
    /// Baut das Mapping von Skin-Namen zu Tile-Listen auf.
    /// </summary>
    private void BuildStyleDictionary()
    {
        styleTiles = new Dictionary<string, List<TileBase>>
        {
            ["Beat"] = beatTiles,
            ["Punk"] = punkTiles,
            ["Tracht"] = trachtTiles,
            ["Klassik"] = klassikTiles,
            ["Reggae"] = reggaeTiles,
            ["Metal"] = metalTiles
        };
    }

    // === Skin-Wechsel: Tiles und Parallax anpassen ===
    /// <summary>
    /// Wendet den gewünschten Skin auf alle Tilemaps und den Parallax-Hintergrund an.
    /// </summary>
    /// <param name="targetStyle">Skin-Name (z.B. "Beat")</param>
    public void ApplySkin(string targetStyle)
    {
        // Prüfe, ob Dictionary existiert
        if (styleTiles == null || styleTiles.Count == 0)
            BuildStyleDictionary();

        // Prüfe, ob Skin existiert
        if (!styleTiles.ContainsKey(targetStyle))
        {
            Debug.LogWarning($"Stil '{targetStyle}' nicht gefunden.");
            return;
        }

        List<TileBase> targetList = styleTiles[targetStyle];
        List<TileBase> referenceList = FindCurrentReferenceList();

        if (referenceList == null)
        {
            Debug.LogWarning("Kein passender Startstil gefunden.");
            return;
        }

        // Tiles auf allen Tilemaps ersetzen
        foreach (Tilemap map in tilemaps)
        {
            BoundsInt bounds = map.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    TileBase current = map.GetTile(pos);
                    if (current == null) continue;

                    int index = referenceList.IndexOf(current);
                    if (index >= 0 && index < targetList.Count)
                    {
                        TileBase newTile = targetList[index];
                        if (newTile != null)
                            map.SetTile(pos, newTile);
                    }
                }
            }
        }

    // Parallax-Hintergrund anpassen
    backgroundManager?.ChangeBackgroundTo(targetStyle);

    // Im Editor: Szene als geändert markieren
#if UNITY_EDITOR
    if (!Application.isPlaying)
    {
        EditorUtility.SetDirty(this);
        foreach (var map in tilemaps)
        EditorUtility.SetDirty(map);
    }
#endif

        Debug.Log($"Skin gewechselt auf: {targetStyle}");
    }

    // === Hilfsfunktion: Finde die aktuelle Referenz-Tileliste ===
    /// <summary>
    /// Sucht die Tile-Liste, die aktuell auf den Tilemaps verwendet wird.
    /// </summary>
    /// <returns>Referenz-Tileliste oder null</returns>
    private List<TileBase> FindCurrentReferenceList()
    {
        foreach (var pair in styleTiles)
        {
            var list = pair.Value;
            if (list == null) continue;

            foreach (Tilemap map in tilemaps)
            {
                BoundsInt bounds = map.cellBounds;
                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    for (int y = bounds.yMin; y < bounds.yMax; y++)
                    {
                        TileBase tile = map.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null && list.Contains(tile))
                            return list;
                    }
                }
            }
        }
        return null;
    }

    // === Editor-Utilities: Nur im Editor verfügbar ===
#if UNITY_EDITOR
    /// <summary>
    /// Tiles automatisch aus Asset-Ordnern laden und Listen befüllen.
    /// </summary>
    [ContextMenu("Tiles automatisch aus Ordnern laden")]
    public void AutoFillTileLists()
    {
        BuildStyleDictionary();

    beatTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Beat");
    punkTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Punk");
    trachtTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Tracht");
    klassikTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Klassik");
    reggaeTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Reggae");
    metalTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Metal");

        Debug.Log("Tile-Listen automatisch befüllt.");
        ApplySkin("Beat"); // Setze Standard-Skin

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
    /// <summary>
    /// Lädt alle Tiles aus einem Asset-Ordner.
    /// </summary>
    private List<TileBase> LoadTilesFromPath(string folderPath)
    {
    #if UNITY_EDITOR
        string[] guids = AssetDatabase.FindAssets("t:TileBase", new[] { folderPath });
        List<TileBase> tiles = new List<TileBase>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TileBase tile = AssetDatabase.LoadAssetAtPath<TileBase>(path);
            if (tile != null)
                tiles.Add(tile);
        }
        tiles.Sort((a, b) => a.name.CompareTo(b.name));
        return tiles;
    #else
        // Pfad für Resources.LoadAll: ohne "Assets/Resources/", nur ab "Tiles/..."
        string resourcesPath = folderPath.Replace("Assets/Resources/", "");
        TileBase[] loadedTiles = Resources.LoadAll<TileBase>(resourcesPath);
        List<TileBase> tiles = new List<TileBase>(loadedTiles);
        tiles.Sort((a, b) => a.name.CompareTo(b.name));
        return tiles;
    #endif
    }

    /// <summary>
    /// Gibt den im Editor gewählten Startskin zurück, oder "Beat" als Fallback.
    /// </summary>
    /// <returns>Skin-Name (z.B. "Beat")</returns>
    public string GetStartSkin()
    {
#if UNITY_EDITOR
        string skin = UnityEditor.EditorPrefs.GetString("ThemeSwitcher_LastSkin", "beat");
        if (string.IsNullOrEmpty(skin))
            return "Beat";
        // Großschreibung wie im Dictionary
        return char.ToUpper(skin[0]) + skin.Substring(1);
#else
        return "Beat";
#endif
    }
}
