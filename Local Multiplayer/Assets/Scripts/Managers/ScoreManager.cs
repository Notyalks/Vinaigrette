using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Pontuação")]
    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;
    public int winningScore = 1000;

    private bool player1Finished = false;
    private bool player2Finished = false;
    private bool player1ScoreFinalized = false;
    private bool player2ScoreFinalized = false;

    private int finishOrderCounter = 0;
    private int player1FinishOrder = 0;
    private int player2FinishOrder = 0;

    [Header("UI de Resultados")]
    public GameObject roundResultPanel;
    public Slider sliderPlayer1;
    public Slider sliderPlayer2;
    public TextMeshProUGUI textPercentPlayer1;
    public TextMeshProUGUI textPercentPlayer2;
    public Button continueButton;

    [Header("Sons de UI")]
    public AudioClip scoreFillSFX;


    public static event System.Action<int, int> OnPlayerReachedGoalGlobal;

    private void Awake()
    {
        Instance = this;

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinuePressed);
    }

    public void ResetRoundState()
    {
        player1Finished = false;
        player2Finished = false;
        player1ScoreFinalized = false;
        player2ScoreFinalized = false;
        finishOrderCounter = 0;
        player1FinishOrder = 0;
        player2FinishOrder = 0;
    }

    public void PlayerReachedGoal(int playerId, int livesRemaining)
    {
        if (playerId == 1 && player1Finished) return;
        if (playerId == 2 && player2Finished) return;

        OnPlayerReachedGoalGlobal?.Invoke(playerId, livesRemaining);

        finishOrderCounter++;
        if (playerId == 1)
        {
            player1Finished = true;
            player1FinishOrder = finishOrderCounter;
        }
        else
        {
            player2Finished = true;
            player2FinishOrder = finishOrderCounter;
        }

        int pontosPorVida = livesRemaining * 50;
        if (playerId == 1)
        {
            scorePlayer1 += pontosPorVida;
            Debug.Log($"P1 chegou com {livesRemaining} vidas -> +{pontosPorVida}");
        }
        else
        {
            scorePlayer2 += pontosPorVida;
            Debug.Log($"P2 chegou com {livesRemaining} vidas -> +{pontosPorVida}");
        }

        // Ambos chegaram
        if (player1Finished && player2Finished)
        {
            scorePlayer1 += 50;
            scorePlayer2 += 50;

            if (player1FinishOrder < player2FinishOrder)
                scorePlayer1 += 50;
            else if (player2FinishOrder < player1FinishOrder)
                scorePlayer2 += 50;

            player1ScoreFinalized = true;
            player2ScoreFinalized = true;

            ShowRoundResults();
            return;
        }

        // Um chegou, outro morreu
        if (playerId == 1 && !GameManager.Instance.player2.isActive)
        {
            scorePlayer1 += 100;
            player1ScoreFinalized = true;

            ShowRoundResults();
            return;
        }
        else if (playerId == 2 && !GameManager.Instance.player1.isActive)
        {
            scorePlayer2 += 100;
            player2ScoreFinalized = true;

            ShowRoundResults();
            return;
        }
    }

    public void PlayerDied(int playerId)
    {
        CheckRoundEnd();
    }

    public void CheckRoundEnd()
    {
        bool p1Ativo = GameManager.Instance.player1.isActive;
        bool p2Ativo = GameManager.Instance.player2.isActive;

        // Caso 1: Ambos morreram
        if (!p1Ativo && !p2Ativo)
        {
            ShowRoundResults();
            return;
        }

        // Caso 2: Ambos chegaram (já tratado em PlayerReachedGoal, mas reforçamos aqui)
        if (player1Finished && player2Finished)
        {
            ShowRoundResults();
            return;
        }

        // Caso 3: Um chegou e o outro morreu
        if (player1Finished && !p2Ativo && !player1ScoreFinalized)
        {
            scorePlayer1 += 100;
            player1ScoreFinalized = true;
            ShowRoundResults();
            return;
        }

        if (player2Finished && !p1Ativo && !player2ScoreFinalized)
        {
            scorePlayer2 += 100;
            player2ScoreFinalized = true;
            ShowRoundResults();
            return;
        }

        // Caso 4: Apenas um morreu rodada continua normalmente
        // (não chamamos ShowRoundResults)
    }

    private void ShowRoundResults()
    {
        roundResultPanel.SetActive(true);

        float p1Target = Mathf.Clamp01((float)scorePlayer1 / winningScore);
        float p2Target = Mathf.Clamp01((float)scorePlayer2 / winningScore);

        // Reseta valores antes da animação
        sliderPlayer1.value = 0f;
        sliderPlayer2.value = 0f;
        textPercentPlayer1.text = "0%";
        textPercentPlayer2.text = "0%";

        // Inicia corrotina de animação
        StartCoroutine(AnimateResults(sliderPlayer1, textPercentPlayer1, p1Target, scoreFillSFX));
        StartCoroutine(AnimateResults(sliderPlayer2, textPercentPlayer2, p2Target, scoreFillSFX));

    }

    private IEnumerator AnimateResults(Slider slider, TextMeshProUGUI text, float targetValue, AudioClip sfx = null)
    {
        float duration = 1.2f;
        float elapsed = 0f;

        AudioSource localSFX = null;

        // Cria um AudioSource local só para essa animação
        if (sfx != null && AudioManager.Instance != null)
        {
            localSFX = AudioManager.Instance.gameObject.AddComponent<AudioSource>();
            localSFX.clip = sfx;
            localSFX.loop = true;
            localSFX.volume = 1f;
            localSFX.outputAudioMixerGroup = AudioManager.Instance.sfxGroup;
            localSFX.Play();
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float value = Mathf.Lerp(0f, targetValue, t);
            slider.value = value;
            text.text = Mathf.RoundToInt(value * 100f) + "%";

            yield return null;
        }

        // Garante valor final exato
        slider.value = targetValue;
        text.text = Mathf.RoundToInt(targetValue * 100f) + "%";

        // Fade-out do som
        if (localSFX != null)
        {
            float fadeTime = 0.3f; // tempo do fade-out
            float startVolume = localSFX.volume;
            float t = 0f;

            while (t < fadeTime)
            {
                t += Time.deltaTime;
                localSFX.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
                yield return null;
            }

            localSFX.Stop();
            Destroy(localSFX);
        }
    }

    private AudioSource loopingSFX;


    private void OnContinuePressed()
    {
        roundResultPanel.SetActive(false);

        if (scorePlayer1 >= winningScore)
        {
            GameManager.Instance.EndGame(1);
        }
        else if (scorePlayer2 >= winningScore)
        {
            GameManager.Instance.EndGame(2);
        }
        else
        {
            GameManager.Instance.EndRound();
        }
    }
}
