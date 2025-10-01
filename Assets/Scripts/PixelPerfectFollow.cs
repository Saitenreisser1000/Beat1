using UnityEngine;

public class PixelPerfectFollow : MonoBehaviour
{
    [Header("Referenz auf die Spielfigur")]
    public Transform target;

    [Header("Pixelgröße in der Welt")]
    public float pixelsPerUnit = 288f;

    [Header("Kamera-Offset in Welt-Einheiten")]
    public Vector2 offsetInUnits = new Vector2(0f, -5.5f); // z. B. 1,5 Kacheln tiefer

    [Header("Levelgrenzen (in Welt-Einheiten)")]
    public Vector2 minWorldBounds = new Vector2(-10f, -5f);
    public Vector2 maxWorldBounds = new Vector2(100f, 20f);

    void LateUpdate()
    {
        if (target == null) return;

        // Zielposition inkl. Offset
        Vector3 targetPos = target.position + (Vector3)offsetInUnits;

        // Auf das Pixelraster runden
        float snapX = Mathf.Round(targetPos.x * pixelsPerUnit) / pixelsPerUnit;
        float snapY = Mathf.Round(targetPos.y * pixelsPerUnit) / pixelsPerUnit;

        Vector3 clampedPos = new Vector3(snapX, snapY, transform.position.z);

        // Begrenzen auf den Sichtbereich
        clampedPos.x = Mathf.Clamp(clampedPos.x, minWorldBounds.x, maxWorldBounds.x);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minWorldBounds.y, maxWorldBounds.y);

        transform.position = clampedPos;
    }
}
