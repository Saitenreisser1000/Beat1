using UnityEngine;
using System.Collections.Generic; // Für List<>
using UnityEngine.Tilemaps; // Für Tilemap und TileBase

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways] // Damit es auch im Editor funktioniert
public class TileManager : MonoBehaviour
{

    [SerializeField] private BackgroundManager backgroundManager;

    public List<Tilemap> tilemaps;

    // Tile-Listen für verschiedene Skins
    public List<TileBase> beatTiles;
    public List<TileBase> punkTiles;
    public List<TileBase> trachtTiles;
    public List<TileBase> klassikTiles;
    public List<TileBase> reggaeTiles;
    public List<TileBase> metalTiles;
      // Mapping: Skin-Name -> Tile-Liste
    private Dictionary<string, List<TileBase>> styleTiles;

    [ContextMenu("Tiles automatisch aus Ordnern laden")]
    public void AutoFillTileLists()
    {
#if UNITY_EDITOR

        beatTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Beat");
        punkTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Punk");
        trachtTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Tracht");
        klassikTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Klassik");
        reggaeTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Reggae");
        metalTiles = LoadTilesFromPath("Assets/Resources/Tiles/LV-Metal");

        Debug.Log("Tile-Listen automatisch befüllt.");
        //ApplySkin("Beat"); // Setze Standard-Skin

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif

    private List<TileBase> LoadTilesFromPath(string folderPath)
    {
        // Pfad für Resources.LoadAll: ohne "Assets/Resources/", nur ab "Tiles/..."
        string resourcesPath = folderPath.Replace("Assets/Resources/", "");
        TileBase[] loadedTiles = Resources.LoadAll<TileBase>(resourcesPath);
        List<TileBase> tiles = new List<TileBase>(loadedTiles);
        tiles.Sort((a, b) => a.name.CompareTo(b.name));
        return tiles;
    }
    public List<TileBase> GetTilebaseByName(string style)
    {
        switch (style.ToLower())
        {
            case "beat": return beatTiles;
            case "punk": return punkTiles;
            case "tracht": return trachtTiles;
            case "klassik": return klassikTiles;
            case "reggae": return reggaeTiles;
            case "metal": return metalTiles;
            default:
                Debug.LogWarning($"⚠️ Unbekannter Stil: {style}");
                return null;
        }
    }


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

    public void ApplyTiles(string targetStyle)
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
            Debug.Log($"Breakpoint");
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
        
    }
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
    public void ChangeTilesTo(string tiles)
    {
        //Debug.Log($"[TileLoader] ChangeTilesTo aufgerufen mit '{tiles}'");
        ApplyTiles(tiles);
    }

    public string GetTileset()
    {
        Debug.Log($"[TileLoader] GetSkin aufgerufen");
        return "beat";
    }
}
