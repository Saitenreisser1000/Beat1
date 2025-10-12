using UnityEngine;

public class ChangeTheme : MonoBehaviour
{
    [Header("Skins")]
    [SerializeField] private AnimatorOverrideController punkController;
    [SerializeField] private AnimatorOverrideController trachtController;
    [SerializeField] private AnimatorOverrideController klassikController;
    [SerializeField] private AnimatorOverrideController reggaeController;
    [SerializeField] private AnimatorOverrideController metalController;
    [SerializeField] private TileSkinManager tileSkinManager;
    [SerializeField] private GameObject player;
    //[SerializeField] private Animator anim;
    


    private AnimatorOverrideController originalController;
    private Animator anim;
    private string currentSkin = "beat";
    private Vector2 lastDPad = Vector2.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = player.GetComponent<Animator>(); //TODO Animator von Player Movement holen
        originalController = new AnimatorOverrideController(anim.runtimeAnimatorController);

    }

    void Update()
    {
        HandleSkinChange();
    }


   // === Skinwechsel mit Priorisierung und Anti-Doppeltrigger ===
    private void HandleSkinChange()
    {
        Vector2 currentDPad = new Vector2(
            Input.GetAxisRaw("DPadHorizontal"),
            Input.GetAxisRaw("DPadVertical")
        );

        // Metal (↗ oder Taste 6)
        if (Input.GetKeyDown(KeyCode.Alpha6) ||
                 (currentDPad.x > 0.5f && currentDPad.y > 0.5f &&
                  !(lastDPad.x > 0.5f && lastDPad.y > 0.5f)))
        {
            TryChangeSkin("metal", metalController, new Color(0.3f, 0.3f, 0.3f));
        }
        // Reggae (↖ oder Taste 5)
        else if (Input.GetKeyDown(KeyCode.Alpha5) ||
            (currentDPad.x < -0.5f && currentDPad.y > 0.5f &&
             !(lastDPad.x < -0.5f && lastDPad.y > 0.5f)))
        {
            TryChangeSkin("reggae", reggaeController, new Color(0.8f, 0.7f, 0.4f));
        }
        // Beat (↑ oder Taste 1)
        else if (Input.GetKeyDown(KeyCode.Alpha1) ||
                 (currentDPad.y > 0.5f && !(lastDPad.y > 0.5f)))
        {
            TryChangeSkin("beat", originalController, new Color(0.5f, 0.5f, 0.5f));
        }
        // Tracht (← oder Taste 3)
        else if (Input.GetKeyDown(KeyCode.Alpha3) ||
                 (currentDPad.x < -0.5f && !(lastDPad.x < -0.5f)))
        {
            TryChangeSkin("tracht", trachtController, new Color(0.55f, 0.8f, 0.5f));
        }
        // Punk (→ oder Taste 2)
        else if (Input.GetKeyDown(KeyCode.Alpha2) ||
                 (currentDPad.x > 0.5f && !(lastDPad.x > 0.5f)))
        {
            TryChangeSkin("punk", punkController, new Color(0.8f, 0.3f, 0.3f));
        }
        // Klassik (↓ oder Taste 4)
        else if (Input.GetKeyDown(KeyCode.Alpha4) ||
                 (currentDPad.y < -0.5f && !(lastDPad.y < -0.5f)))
        {
            TryChangeSkin("klassik", klassikController, new Color(0.75f, 0.55f, 0.85f));
        }
      
        lastDPad = currentDPad; // DPad-Zustand für nächsten Frame merken
    }

    private void TryChangeSkin(string skinName, AnimatorOverrideController controller, Color backgroundColor)
    {
        if (currentSkin == skinName) return;

        ChangeSkin(controller, backgroundColor);
        FindFirstObjectByType<MusicManager>().PlayScratchAndChangeMusic(skinName);
        tileSkinManager.ApplySkin(char.ToUpper(skinName[0]) + skinName.Substring(1));
        currentSkin = skinName;
    }

    private void ChangeSkin(AnimatorOverrideController newController, Color backgroundColor)
    {
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
    }


    // Skinwechsel im Editormode erzwingen
#if UNITY_EDITOR
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
    tileSkinManager.ApplySkin(char.ToUpper(envName[0]) + envName.Substring(1));
}
#endif


}


