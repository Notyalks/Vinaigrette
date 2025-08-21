using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameState { Build, Play, End }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentState = GameState.Build;
    public event System.Action<GameState> OnGameStateChanged;

    [Header("Turnos de Construção")]
    public int buildRounds = 1;
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

    [Header("Tela de Vitória")]
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
        OnGameStateChanged?.Invoke(currentState);
        currentBuildTurn = 0;
        constructionGrid.SetActive(true);
        turnManager.StartTurn(1);

        // Esconde jogadores no modo construção
        player1.gameObject.SetActive(false);
        player2.gameObject.SetActive(false);
        player1.isActive = false;
        player2.isActive = false;

        player1.SetLivesUIVisible(false);
        player2.SetLivesUIVisible(false);
    }

    public void OnTurnEnded(int previousPlayer)
    {
        currentBuildTurn++;
        int maxTurns = buildRounds * 2;

        if (currentBuildTurn >= maxTurns)
            ShowPreparePanel();
        else
            turnManager.ShowTurnTransition(previousPlayer == 1 ? 2 : 1);
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
        OnGameStateChanged?.Invoke(currentState);
        preparePanel.SetActive(false);

        constructionGrid.SetActive(false);
        turnManager.gameObject.SetActive(false);

        // Ativa jogadores
        player1.gameObject.SetActive(true);
        player2.gameObject.SetActive(true);

        player1.ReactivatePlayer();
        player2.ReactivatePlayer();

        player1.SetLivesUIVisible(true);
        player2.SetLivesUIVisible(true);
    }

    public void EndGame(int winner)
    {
        currentState = GameState.End;
        OnGameStateChanged?.Invoke(currentState);

        constructionGrid.SetActive(false);
        player1.isActive = false;
        player2.isActive = false;

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
        yield return new WaitForSeconds(2f);
        RestartRound();
    }

    public void RestartRound()
    {
        currentState = GameState.Build;
        OnGameStateChanged?.Invoke(currentState);
        currentBuildTurn = 0;

        player1.ResetPlayer();
        player2.ResetPlayer();

        ScoreManager.Instance.ResetRoundState();

        constructionGrid.SetActive(true);
        turnManager.gameObject.SetActive(true);
        turnManager.StartTurn(1);
    }

    public Vector3 GetRandomStartPosition(int playerId)
    {
        Collider2D area = (playerId == 1) ? startAreaP1 : startAreaP2;
        Bounds bounds = area.bounds;

        float playerHeight = (playerId == 1 ? player1 : player2)
            .GetComponent<Collider2D>().bounds.extents.y;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float offsetY = 0.05f;
        float y = bounds.min.y + playerHeight + offsetY;

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
