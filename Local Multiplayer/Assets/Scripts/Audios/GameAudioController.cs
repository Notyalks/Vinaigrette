using UnityEngine;

public class GameAudioController : MonoBehaviour
{
    [Header("Músicas")]
    public AudioClip gameplayMusic;

    [Header("Efeitos Sonoros")]
    public AudioClip victorySfx;
    public AudioClip goalSfx;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
        ScoreManager.OnPlayerReachedGoalGlobal += HandlePlayerReachedGoal;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
        ScoreManager.OnPlayerReachedGoalGlobal += HandlePlayerReachedGoal;
    }

    private void Start()
    {
        // ?? Música de gameplay no início
        if (gameplayMusic != null)
            AudioManager.Instance?.PlayMusic(gameplayMusic, 0.5f);
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.End && victorySfx != null)
        {
            AudioManager.Instance?.PlaySFX(victorySfx);
        }
    }

    private void HandlePlayerReachedGoal(int playerId, int livesRemaining)
    {
        if (goalSfx != null)
            AudioManager.Instance?.PlaySFX(goalSfx);
    }
}
