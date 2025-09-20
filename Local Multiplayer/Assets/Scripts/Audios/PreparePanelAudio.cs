using UnityEngine;

public class PreparePanelAudio : MonoBehaviour
{
    [Header("Áudio da contagem")]
    public AudioClip countdownBeep;
    [Header("Áudio de largada")]
    public AudioClip startHorn;

    // Chamado por Animation Event no "3", "2" e "1"
    public void PlayCountdownBeep()
    {
        if (countdownBeep != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(countdownBeep);
    }

    // Chamado por Animation Event no "GO!"
    public void PlayStartHorn()
    {
        if (startHorn != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(startHorn);
    }
}
