using UnityEngine;

public class MenuMusicPlayer : MonoBehaviour
{
    [Header("Configuração")]
    public AudioClip menuMusic;

    void Start()
    {
        if (menuMusic != null)
        {
            // Toca música do menu em loop
            AudioManager.Instance?.PlayMusic(menuMusic, crossfade: 0.5f);
        }
    }
}
