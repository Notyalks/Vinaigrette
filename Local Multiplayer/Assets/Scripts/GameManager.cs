using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum GameState { Build, Play, End }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton simples
    public GameState currentState = GameState.Build;

    [Header("Turnos de Construção")]
    public int buildRounds = 1; // quantos turnos cada jogador tem antes de começar gameplay
    private int currentBuildTurn = 0;

    [Header("Transição para gameplay")]
    public GameObject preparePanel;
    public CanvasGroup prepareCanvasGroup;
    public float prepareDuration = 3f;
    public float fadeDuration = 1f;

    [Header("Referências")]
    public TurnManager turnManager;
    public PlayerController player1;
    public PlayerController player2;
    public Collider2D startAreaP1;
    public Collider2D startAreaP2;

    [Header("Elementos Visuais")]
    public GameObject constructionGrid;

    public GameObject winScreen;
    public Text winText;

    private bool waitingToRestart = false;
    private Vector3 player1StartPosition;
    private Vector3 player2StartPosition;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player1StartPosition = player1.transform.position;
        player2StartPosition = player2.transform.position;
        StartBuildPhase();
    }

    void Update()
    {
        if (waitingToRestart && Input.anyKeyDown)
        {
            waitingToRestart = false;
            winScreen.SetActive(false);
            RestartRound();
        }
    }

    public void StartBuildPhase()
    {
        currentState = GameState.Build;
        currentBuildTurn = 0;
        constructionGrid.SetActive(true);
        turnManager.StartTurn(1);
    }

    public void OnTurnEnded(int previousPlayer)
    {
        currentBuildTurn++;

        int maxTurns = buildRounds * 2;

        if (currentBuildTurn >= maxTurns)
        {
            ShowPreparePanel();
        }
        else
        {
            int nextPlayer = previousPlayer == 1 ? 2 : 1;
            turnManager.ShowTurnTransition(nextPlayer);
        }
    }

    void ShowPreparePanel()
    {
        constructionGrid.SetActive(false);

        preparePanel.SetActive(true);
        prepareCanvasGroup.alpha = 1f;
        StartCoroutine(FadeOutPrepareAndStartGameplay());
    }

    public void StartPlayPhase()
    {
        currentState = GameState.Play;
        preparePanel.SetActive(false);

        constructionGrid.SetActive(false);

        turnManager.gameObject.SetActive(false);
        player1.EnableControl(true);
        player2.EnableControl(true);
    }

    public void EndGame(int winner)
    {
        currentState = GameState.End;

        constructionGrid.SetActive(false);
        player1.EnableControl(false);
        player2.EnableControl(false);

        winScreen.SetActive(true);
        winText.text = "Jogador " + winner + " venceu!";

        waitingToRestart = true;
    }

    public bool IsLastBuildTurn()
    {
        int maxTurns = buildRounds * 2;
        return currentBuildTurn + 1 >= maxTurns;
    }

    IEnumerator FadeOutPrepareAndStartGameplay()
    {
        yield return new WaitForSeconds(prepareDuration);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            prepareCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        preparePanel.SetActive(false);
        StartPlayPhase();
    }

    public void EndRound()
    {
        Debug.Log("Rodada terminou!");
        StartCoroutine(RestartRoundAfterDelay());
    }

    private IEnumerator RestartRoundAfterDelay()
    {
        yield return new WaitForSeconds(2f); // tempo antes de reiniciar
        RestartRound();
    }

    public void RestartRound()
    {
        currentState = GameState.Build;
        currentBuildTurn = 0;

        player1.transform.position = player1StartPosition;
        player2.transform.position = player2StartPosition;
        player1.lives = player1.maxLives;
        player2.lives = player2.maxLives;
        player1.UpdateLivesUI();
        player2.UpdateLivesUI();

        ScoreManager.Instance.ResetRoundState();

        constructionGrid.SetActive(true);
        turnManager.gameObject.SetActive(true);
        turnManager.StartTurn(1);
    }

    public Vector3 GetRandomStartPosition(int playerId)
    {
        Collider2D area = (playerId == 1) ? startAreaP1 : startAreaP2;
        Bounds bounds = area.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(x, y, 0f);
    }

    public void PlayerReachedGoal(int playerId, int livesRemaining)
    {
        ScoreManager.Instance.PlayerReachedGoal(playerId, livesRemaining);
    }

    public void PlayerDied(int playerId)
    {
        ScoreManager.Instance.PlayerDied(playerId);
    }

}
