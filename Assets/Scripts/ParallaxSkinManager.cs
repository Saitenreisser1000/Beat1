using UnityEngine;

public class ParallaxSkinManager : MonoBehaviour
{
    [Header("Ziel-SpriteRenderers")]
    public SpriteRenderer backRenderer;
    public SpriteRenderer midRenderer;
    public SpriteRenderer frontRenderer;

    private const string RESOURCE_PATH = "Sprites/Levels";

    /// <summary>
    /// Lädt ein Sprite aus Resources anhand des angegebenen Namens.
    /// </summary>
    private Sprite LoadSprite(string name)
    {
        Sprite sprite = Resources.Load<Sprite>($"{RESOURCE_PATH}/{name}");
        if (sprite == null)
        {
            Debug.LogWarning($"Sprite '{name}' nicht gefunden in '{RESOURCE_PATH}'.");
        }
        return sprite;
    }

    /// <summary>
    /// Wechselt das Parallax-Hintergrund-Theme.
    /// </summary>
    public void ApplySkin(string skin)
    {
        string prefix = skin.ToLower(); // z. B. "beat"

        Sprite back  = LoadSprite($"{prefix}_Back");
        Sprite mid   = LoadSprite($"{prefix}_Mid");
        Sprite front = LoadSprite($"{prefix}_Front");

        backRenderer.sprite  = back;
        midRenderer.sprite   = mid;
        frontRenderer.sprite = front;

        Debug.Log($"Parallax-Skin auf '{skin}' gesetzt.");
    }
}
