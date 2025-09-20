using UnityEngine;

public class AnimationSFX : MonoBehaviour
{
    [Header("Configuração")]
    public AudioClip[] clips;

    // Chamado por Animation Event (você pode passar o índice do clip)
    public void PlaySfxByIndex(int index)
    {
        if (AudioManager.Instance == null) return;

        if (index >= 0 && index < clips.Length)
        {
            AudioManager.Instance.PlaySFX(clips[index]);
        }
    }

    // Chamado por Animation Event passando um AudioClip específico
    public void PlaySfx(AudioClip clip)
    {
        if (clip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clip);
        }
    }
}
