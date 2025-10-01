using UnityEngine;
using UnityEngine.Tilemaps;

public class TileScaler : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private float scaleFactor = 1.01f;

    void Start()
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(pos))
                {
                    Matrix4x4 scaleMatrix = Matrix4x4.TRS(
                        Vector3.zero,
                        Quaternion.identity,
                        new Vector3(scaleFactor, scaleFactor, 1f)
                    );

                    tilemap.SetTransformMatrix(pos, scaleMatrix);
                }
            }
        }
    }
}