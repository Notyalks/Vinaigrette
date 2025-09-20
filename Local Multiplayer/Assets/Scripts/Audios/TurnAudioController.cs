using UnityEngine;

public class TurnAudioController : MonoBehaviour
{
    [Header("Efeitos Sonoros")]
    public AudioClip confirmSfx;

    private TurnManager turnManager;

    private void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        if (turnManager != null)
            turnManager.OnTransitionConfirmed += HandleTransitionConfirmed;
    }

    private void OnDestroy()
    {
        if (turnManager != null)
            turnManager.OnTransitionConfirmed -= HandleTransitionConfirmed;
    }

    private void HandleTransitionConfirmed()
    {
        if (confirmSfx != null)
            AudioManager.Instance?.PlaySFX(confirmSfx);
    }
}
