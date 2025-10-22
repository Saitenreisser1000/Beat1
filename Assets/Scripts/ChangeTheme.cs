using UnityEngine;

public class ChangeTheme : MonoBehaviour
{
    [SerializeField] private SkinManager skinManager;
    [SerializeField] private BackgroundManager backgroundManager;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private MusicManager musicManager;    
    private Vector2 lastDPad = Vector2.zero;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
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
            ChangeThemeTo("metal");
        }
        // Reggae (↖ oder Taste 5)
        else if (Input.GetKeyDown(KeyCode.Alpha5) ||
            (currentDPad.x < -0.5f && currentDPad.y > 0.5f &&
             !(lastDPad.x < -0.5f && lastDPad.y > 0.5f)))
        {
            ChangeThemeTo("reggae");
        }
        // Beat (↑ oder Taste 1)
        else if (Input.GetKeyDown(KeyCode.Alpha1) ||
                 (currentDPad.y > 0.5f && !(lastDPad.y > 0.5f)))
        {
            ChangeThemeTo("metal");
        }
        // Tracht (← oder Taste 3)
        else if (Input.GetKeyDown(KeyCode.Alpha3) ||
                 (currentDPad.x < -0.5f && !(lastDPad.x < -0.5f)))
        {
            ChangeThemeTo("tracht");
        }
        // Punk (→ oder Taste 2)
        else if (Input.GetKeyDown(KeyCode.Alpha2) ||
                 (currentDPad.x > 0.5f && !(lastDPad.x > 0.5f)))
        {
            ChangeThemeTo("punk");
        }
        // Klassik (↓ oder Taste 4)
        else if (Input.GetKeyDown(KeyCode.Alpha4) ||
                 (currentDPad.y < -0.5f && !(lastDPad.y < -0.5f)))
        {
            ChangeThemeTo("klassik");
        }
        lastDPad = currentDPad; // DPad-Zustand für nächsten Frame merken
    }
    
    public void ChangeThemeTo(string theme)
    {
        ChangeSkinTo(theme);
        ChangeBackgroundTo(theme);
        ChangeTilesTo(theme);
        ChangeMusicTo(theme);
    }

    public void ChangeSkinTo(string skin)
    {
        skinManager.SetSkin(skin);
        Debug.Log($"[ChangeTheme] ApplySkin aufgerufen mit '{skin}'");
    }
    public void ChangeBackgroundTo(string theme)
    {
        backgroundManager.ChangeBackgroundTo(theme);
        Debug.Log($"[ChangeTheme] ApplyTheme aufgerufen mit '{theme}'");
    }
    public void ChangeTilesTo(string tileSet)
    {
        tileManager.ChangeTilesTo(tileSet);
        //Debug.Log($"[ChangeTheme] ApplyTiles aufgerufen mit '{tileSet}'");
    }
    public void ChangeMusicTo(string music)
    {
        musicManager.PlayScratchAndChangeMusic(music);
    }
}


