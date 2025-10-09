using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways] // Damit es auch im Editor funktioniert
public class TileSkinManager : MonoBehaviour
{
    [Header("Parallax-Hintergrund")]
    [SerializeField] private ParallaxSkinManager parallaxSkinManager;

    [Header("Ziel-Tilemaps (z. B. Boden + Deko)")]
    public List<Tilemap> tilemaps;

    [Header("Tiles im Beat-Stil (Startzustand)")]
    public List<TileBase> beatTiles;

    [Header("Tiles im Punk-Stil")]
    public List<TileBase> punkTiles;

    [Header("Tiles im Tracht-Stil")]
    public List<TileBase> trachtTiles;

    [Header("Tiles im Klassik-Stil")]
    public List<TileBase> klassikTiles;

    [Header("Tiles im Reggae-Stil")]
    public List<TileBase> reggaeTiles;

    [Header("Tiles im Metal-Stil")]
    public List<TileBase> metalTiles;

    private Dictionary<string, List<TileBase>> styleTiles;

    private void Awake()
    {
        BuildStyleDictionary();
    }

    private void OnEnable()
    {
        BuildStyleDictionary();
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

    // --------------------------------------------
    // SKIN-WECHSEL
    // --------------------------------------------
    public void ApplySkin(string targetStyle)
    {
        if (styleTiles == null || styleTiles.Count == 0)
            BuildStyleDictionary();

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

        parallaxSkinManager?.ApplySkin(targetStyle);

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

    [ContextMenu("Tiles automatisch aus Ordnern laden")]
    public void AutoFillTileLists()
    {
#if UNITY_EDITOR
        BuildStyleDictionary();

        beatTiles = LoadTilesFromPath("Assets/Tiles/LV-Beat");
        punkTiles = LoadTilesFromPath("Assets/Tiles/LV-Punk");
        trachtTiles = LoadTilesFromPath("Assets/Tiles/LV-Tracht");
        klassikTiles = LoadTilesFromPath("Assets/Tiles/LV-Klassik");
        reggaeTiles = LoadTilesFromPath("Assets/Tiles/LV-Reggae");
        metalTiles = LoadTilesFromPath("Assets/Tiles/LV-Metal");

        Debug.Log("Tile-Listen automatisch befüllt.");
        ApplySkin("Beat");

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }

#if UNITY_EDITOR
    private List<TileBase> LoadTilesFromPath(string folderPath)
    {
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
    }
#endif
}
