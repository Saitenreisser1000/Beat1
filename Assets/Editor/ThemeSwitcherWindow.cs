using UnityEditor;
using UnityEngine;

public class ThemeSwitcherWindow : EditorWindow
{
    private const string StartWithBeatKey = "ThemeSwitcher_StartWithBeat";

    private ChangeTheme themeScript;
    private GameObject gameManagerObj;
    private string[] skinOptions = { "beat", "punk", "tracht", "klassik", "reggae", "metal" };
    private bool startWithBeatTheme = true;
    private string lastSkin = "beat";

    [MenuItem("Window/Theme Switcher")]
    public static void ShowWindow()
    {
        var window = GetWindow<ThemeSwitcherWindow>("Theme Switcher");
        window.minSize = new Vector2(260, 120);
    }

    private void OnEnable()
    {
        startWithBeatTheme = EditorPrefs.GetBool(StartWithBeatKey, true);
        lastSkin = EditorPrefs.GetString("ThemeSwitcher_LastSkin", "beat");
        // Playmode-Callback registrieren
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode && themeScript != null)
        {
            if (startWithBeatTheme)
            {
                themeScript.ForceChangeSkin("beat");
                Debug.Log("[SkinSwitcher] Playmode: Starte immer mit Beat-Theme.");
            }
            else
            {
                themeScript.ForceChangeSkin(lastSkin);
                Debug.Log($"[SkinSwitcher] Playmode: Starte mit letztem Theme '{lastSkin}'.");
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("🎨 Theme Switcher", EditorStyles.boldLabel);
        GUILayout.Space(4);

        // Versuche das ChangeTheme-Script vom GameObject _GameManager zu holen
        if (themeScript == null)
        {
            gameManagerObj = GameObject.Find("_GameManager");
            if (gameManagerObj != null)
            {
                themeScript = gameManagerObj.GetComponent<ChangeTheme>();
            }
        }

        GUILayout.Space(10);

        // Checkbox für Startverhalten
        startWithBeatTheme = EditorGUILayout.Toggle("Immer mit Beat-Theme starten", startWithBeatTheme);
        EditorPrefs.SetBool(StartWithBeatKey, startWithBeatTheme);

        GUILayout.Space(10);

        // Buttons für jeden Skin
        GUILayout.Label("Skin auswählen:", EditorStyles.label);
        GUILayout.BeginHorizontal();
        foreach (var skin in skinOptions)
        {
            if (GUILayout.Button(skin, GUILayout.Height(32)))
            {
                themeScript.ForceChangeSkin(skin);
                lastSkin = skin;
                EditorPrefs.SetString("ThemeSwitcher_LastSkin", lastSkin);
                Debug.Log($"[SkinSwitcher] {skin} wurde aktiviert.");
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (themeScript == null)
        {
            EditorGUILayout.HelpBox("Das GameObject '_GameManager' mit ChangeTheme-Script wurde nicht gefunden!", MessageType.Error);
            return;
        }

        GUILayout.Space(6);

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("⚠️ Funktioniert auch im Edit-Modus!", MessageType.None);
    }
}
