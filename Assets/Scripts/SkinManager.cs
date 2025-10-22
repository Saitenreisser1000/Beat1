using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [Header("Skins")]
    [SerializeField] private AnimatorOverrideController punkController;
    [SerializeField] private AnimatorOverrideController trachtController;
    [SerializeField] private AnimatorOverrideController klassikController;
    [SerializeField] private AnimatorOverrideController reggaeController;
    [SerializeField] private AnimatorOverrideController metalController;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private GameObject player;
    //[SerializeField] private Animator anim;
    


    private AnimatorOverrideController originalController;
    private Animator anim;
    private string currentSkin = "beat";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = player.GetComponent<Animator>(); //TODO Animator von Player Movement holen
        originalController = new AnimatorOverrideController(anim.runtimeAnimatorController);

    }

   // === Skinwechsel mit Priorisierung und Anti-Doppeltrigger ===
    public void SetSkin(string skinName)
    {
        switch (skinName)
        {
            case "metal":
                ChangeSkinTo("metal", metalController, new Color(0.3f, 0.3f, 0.3f));
                break;
            case "reggae":
                ChangeSkinTo("reggae", reggaeController, new Color(0.8f, 0.7f, 0.4f));
                break;
            case "beat":
                ChangeSkinTo("beat", originalController, new Color(0.5f, 0.5f, 0.5f));
                break;
            case "tracht":
                ChangeSkinTo("tracht", trachtController, new Color(0.55f, 0.8f, 0.5f));
                break;
            case "punk":
                ChangeSkinTo("punk", punkController, new Color(0.8f, 0.3f, 0.3f));
                break;
            case "klassik":
                ChangeSkinTo("klassik", klassikController, new Color(0.75f, 0.55f, 0.85f));
                break;
            default:
                Debug.LogWarning($"⚠️ Unbekannter Skinname: {skinName}");
                break;
        }
    }
    private void ChangeSkinTo(string skinName,AnimatorOverrideController newController, Color backgroundColor)
    {
        if (currentSkin == skinName) return;

        bool ducking = anim.GetBool("ducking");
        bool crawling = anim.GetBool("crawling");

        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = currentState.normalizedTime;

        anim.runtimeAnimatorController = newController;
        anim.Rebind();
        anim.SetBool("ducking", ducking);
        anim.SetBool("crawling", crawling);

        anim.Play(currentState.shortNameHash, 0, normalizedTime);
        Camera.main.backgroundColor = backgroundColor;
        currentSkin = skinName;
        Debug.Log($"✅ Skin zu '{skinName}' gewechselt.");
        tileManager.ApplyTiles(char.ToUpper(skinName[0]) + skinName.Substring(1));
    }


    // Skinwechsel im Editormode erzwingen
//#if UNITY_EDITOR
public void ForceChangeSkin(string skinName)
{
    string startSprite = "idle-left-01";
    string folderName;

    // Bestimme den richtigen Ordnernamen basierend auf dem Skin
    switch (skinName.ToLower())
    {
        case "beat": folderName = "Beat"; ForceChangeEnvironment("beat"); break;
        case "punk": folderName = "BeatPunk"; ForceChangeEnvironment("punk"); break;
        case "tracht": folderName = "BeatTracht"; ForceChangeEnvironment("tracht"); break;
        case "klassik": folderName = "BeatKlassik"; ForceChangeEnvironment("klassik"); break;
        case "reggae": folderName = "BeatReggae"; ForceChangeEnvironment("reggae"); break;
        case "metal": folderName = "BeatMetal"; ForceChangeEnvironment("metal"); break;
        default:
            Debug.LogWarning($"⚠️ Unbekannter Skinname: {skinName}");
            return;
    }

    // Dynamisch zusammengesetzter Pfad
    string spritePath = $"Sprites/{folderName}/{startSprite}";

    // Sprite laden
    Sprite newSprite = Resources.Load<Sprite>(spritePath);

    if (newSprite != null)
    {
        player.GetComponent<PlayerMovement>().ChangeSprite(newSprite);
        Debug.Log($"✅ Sprite '{spritePath}' erfolgreich geladen.");
    }
    else
    {
        Debug.LogWarning($"⚠️ Konnte Sprite unter Pfad '{spritePath}' nicht finden!");
    }
}
public void ForceChangeEnvironment(string envName)
{
    tileManager.ApplyTiles(char.ToUpper(envName[0]) + envName.Substring(1));
    Debug.Log($"✅ Environment zu '{envName}' gewechselt.");
}
//#endif


}



