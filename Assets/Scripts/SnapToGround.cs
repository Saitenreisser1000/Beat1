using UnityEngine;

[ExecuteInEditMode] // auch im Editor aktiv
public class SnapToGround2D : MonoBehaviour
{
    [SerializeField] private bool snapToGround = true; // optional: Snap aktivieren/deaktivieren
    [Header("Where to Snap")]
    [SerializeField] private LayerMask groundLayer; // nur bestimmte Layer als Boden
    [SerializeField] private float rayLength = 100f; // maximale Höhe des Raycasts
    [SerializeField] private float offset = 0f; // optional: Abstandsversatz zum Boden

    private void Update()
    {
        // Nur im Editor ausführen, nicht im Playmode
        if (!Application.isPlaying && snapToGround)
        {
            SnapDown();
        }
    }

    private void SnapDown()
    {
        // von der aktuellen Position einen Ray nach unten werfen
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);

        if (hit.collider != null)
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + offset; // direkt auf den Boden setzen
            transform.position = pos;
        }
    }
}
