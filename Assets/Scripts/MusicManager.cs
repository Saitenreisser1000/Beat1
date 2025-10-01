using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip beatClip;
    public AudioClip punkClip;
    public AudioClip trachtClip;
    public AudioClip klassikClip;
    public AudioClip reggaeClip;
    public AudioClip metalClip;
    public AudioSource scratchSource;      // Neue Audioquelle
    public AudioClip scratchClip;          // Das Scratch-Geräusch


    void Start()
    {
        audioSource.clip = beatClip;
        audioSource.Play();
    }

    public void ChangeMusic(string skinName)
    {
        switch (skinName.ToLower())
        {
            case "beat":
                audioSource.clip = beatClip;
                break;
            case "punk":
                audioSource.clip = punkClip;
                break;
            case "tracht":
                audioSource.clip = trachtClip;
                break;
            case "klassik":
                audioSource.clip = klassikClip;
                break;
            case "reggae":
                audioSource.clip = reggaeClip;
                break;
            case "metal":
                audioSource.clip = metalClip;
                break;
            default:
                Debug.LogWarning("Unbekannter Skin: " + skinName);
                return;
        }
        Debug.Log("AudioClip gesetzt: " + (audioSource.clip != null ? audioSource.clip.name : "Kein Clip"));
        Debug.Log("Clip-Länge: " + (audioSource.clip != null ? audioSource.clip.length.ToString("F2") + " Sekunden" : "—"));
        Debug.Log("AudioSource istPlaying: " + audioSource.isPlaying);
        Debug.Log("Volume: " + audioSource.volume);

        audioSource.Play();
    }

    public void PlayScratchAndChangeMusic(string skinName)
    {
        if (scratchSource && scratchClip)
            scratchSource.PlayOneShot(scratchClip);

        ChangeMusic(skinName); // Musik gleichzeitig starten
    }

}
