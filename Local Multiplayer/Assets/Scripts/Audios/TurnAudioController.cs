using UnityEngine;

public class TurnAudioController : MonoBehaviour
{
    [Header("Efeitos Sonoros")]
    public AudioClip confirmSfx;
    public AudioClip lastSecondsTickSfx;

    private TurnManager turnManager;

    private void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.OnTransitionConfirmed += HandleTransitionConfirmed;
            turnManager.OnLastSecondsTick += HandleLastSecondsTick;
        }
    }

    private void OnDestroy()
    {
        if (turnManager != null)
        {
            turnManager.OnTransitionConfirmed -= HandleTransitionConfirmed;
            turnManager.OnLastSecondsTick -= HandleLastSecondsTick;
        }
    }

    private void HandleTransitionConfirmed()
    {
        if (confirmSfx != null)
            AudioManager.Instance?.PlaySFX(confirmSfx);
    }

    private void HandleLastSecondsTick(int secondsLeft)
    {
        if (lastSecondsTickSfx != null)
            AudioManager.Instance?.PlaySFX(lastSecondsTickSfx);

        // muda a cor do timer para vermelho
        UIManager.Instance.SetTimerColor(Color.red);
    }
}
