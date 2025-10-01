using UnityEngine;

public class ParallaxSkinManager : MonoBehaviour
{
    [Header("BEAT – Sprites")]
    public Sprite beat_Back;
    public Sprite beat_Mid;
    public Sprite beat_Front;

    [Header("PUNK – Sprites")]
    public Sprite punk_Back;
    public Sprite punk_Mid;
    public Sprite punk_Front;

    [Header("TRACHT – Sprites")]
    public Sprite tracht_Back;
    public Sprite tracht_Mid;
    public Sprite tracht_Front;

    [Header("KLASSIK – Sprites")]
    public Sprite klassik_Back;
    public Sprite klassik_Mid;
    public Sprite klassik_Front;

    [Header("REGGAE – Sprites")]
    public Sprite reggae_Back;
    public Sprite reggae_Mid;
    public Sprite reggae_Front;

    [Header("METAL – Sprites")]
    public Sprite metal_Back;
    public Sprite metal_Mid;
    public Sprite metal_Front;

    [Header("Ziel-SpriteRenderers")]
    public SpriteRenderer backRenderer;
    public SpriteRenderer midRenderer;
    public SpriteRenderer frontRenderer;

    public void ApplySkin(string skin)
    {
        switch (skin)
        {
            case "Beat":
                backRenderer.sprite = beat_Back;
                midRenderer.sprite = beat_Mid;
                frontRenderer.sprite = beat_Front;
                break;
            case "Punk":
                backRenderer.sprite = punk_Back;
                midRenderer.sprite = punk_Mid;
                frontRenderer.sprite = punk_Front;
                break;
            case "Tracht":
                backRenderer.sprite = tracht_Back;
                midRenderer.sprite = tracht_Mid;
                frontRenderer.sprite = tracht_Front;
                break;
            case "Klassik":
                backRenderer.sprite = klassik_Back;
                midRenderer.sprite = klassik_Mid;
                frontRenderer.sprite = klassik_Front;
                break;
            case "Reggae":
                backRenderer.sprite = reggae_Back;
                midRenderer.sprite = reggae_Mid;
                frontRenderer.sprite = reggae_Front;
                break;
            case "Metal":
                backRenderer.sprite = metal_Back;
                midRenderer.sprite = metal_Mid;
                frontRenderer.sprite = metal_Front;
                break;
        }
    }
}
