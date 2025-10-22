using UnityEngine;

public class BackgroundManager : MonoBehaviour
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
    public void ChangeBackgroundTo(string skin)
    {
        string prefix = skin.ToLower(); // z. B. "beat"

        Sprite back = LoadSprite($"{prefix}_Back");
        Sprite mid = LoadSprite($"{prefix}_Mid");
        Sprite front = LoadSprite($"{prefix}_Front");

        backRenderer.sprite = back;
        midRenderer.sprite = mid;
        frontRenderer.sprite = front;

        Debug.Log($"Parallax-Skin auf '{skin}' gesetzt.");
    }
    
    public string GetBackground()
    {
        string skin = null;
        if (frontRenderer.sprite != null)
        {
            string spriteName = frontRenderer.sprite.name; // z. B. "beat_Front"
            Debug.Log($"Aktueller Parallax-Skin: '{skin}'");
            skin = spriteName.Split('_')[0]; // Extrahiere "beat"
        }
        else
        {
            Debug.Log("Kein Sprite im Front-Renderer gesetzt.");
        }
            return skin;
    }
}
