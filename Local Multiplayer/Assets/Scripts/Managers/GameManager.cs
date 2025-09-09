using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameState { Build, Play, End }
public enum BuildMode { Adversary, Self }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentState = GameState.Build;
    public event System.Action<GameState> OnGameStateChanged;
    public event System.Action OnPlayPhaseStarted;

    [Header("Rodadas")]
    public int roundNumber = 1; // começa na rodada 1
    public BuildMode currentBuildMode = BuildMode.Adversary;

    [Header("Turnos de Construção")]
    public int buildRounds = 1;
    public int currentBuildTurn = 0;

    [Header("Referências")]
    public TurnManager turnManager;
    public PlayerController player1;
    public PlayerController player2;
    public Collider2D startAreaP1;
    public Collider2D startAreaP2;

    private bool waitingToRestart = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartBuildPhase();
    }

    void Update()
    {
        if (waitingToRestart && Input.anyKeyDown)
        {
            waitingToRestart = false;
            RestartRound();
        }
    }

    // -----------------------------
    // FASE DE CONSTRUÇÃO
    // -----------------------------
    public void StartBuildPhase()
    {
        currentState = GameState.Build;
        OnGameStateChanged?.Invoke(currentState);

        currentBuildTurn = 0;
        UIManager.Instance.ShowConstructionGrid();

        // Mostra transição antes do primeiro turno
        turnManager.ResetTurnState();
        turnManager.ShowTurnTransition(1);

        player1.HideForBuildPhase();
        player2.HideForBuildPhase();
    }


    public void OnTurnEnded(int previousPlayer)
    {
        currentBuildTurn++;
        int maxTurns = buildRounds * 2;

        if (currentBuildTurn >= maxTurns)
        {
            UIManager.Instance.ShowPreparePanel();
        }
        else
        {
            int nextPlayer = previousPlayer == 1 ? 2 : 1;
            turnManager.ShowTurnTransition(nextPlayer);
        }
    }

    // -----------------------------
    // FASE DE JOGO
    // -----------------------------
    public void StartPlayPhase()
    {
        currentState = GameState.Play;
        OnGameStateChanged?.Invoke(currentState);

        UIManager.Instance.HideConstructionGrid();

        turnManager.gameObject.SetActive(false);

        player1.ShowForPlayPhase();
        player2.ShowForPlayPhase();

        OnPlayPhaseStarted?.Invoke();
    }

    // -----------------------------
    // FIM DE JOGO
    // -----------------------------
    public void EndGame(int winner)
    {
        currentState = GameState.End;
        OnGameStateChanged?.Invoke(currentState);

        UIManager.Instance.HideConstructionGrid();

        player1.Deactivate();
        player2.Deactivate();

        UIManager.Instance.ShowWinScreen(winner);
        waitingToRestart = true;
    }

    // -----------------------------
    // RESTART DE RODADA
    // -----------------------------
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
        roundNumber++;
        currentState = GameState.Build;
        OnGameStateChanged?.Invoke(currentState);
        currentBuildTurn = 0;

        // Alterna entre modos
        if (roundNumber % 2 == 0)
            currentBuildMode = BuildMode.Adversary;
        else
            currentBuildMode = BuildMode.Self;

        // Notificar inventário que precisa forçar bomba
        if (currentBuildMode == BuildMode.Self)
        {
            InventoryManager[] inventories = FindObjectsOfType<InventoryManager>();
            foreach (var inv in inventories)
                inv.ForceBombNextSetup();
        }

        // Reset players
        player1.ResetPlayer();
        player2.ResetPlayer();

        // Esconde players na fase de construção
        player1.HideForBuildPhase();
        player2.HideForBuildPhase();

        ScoreManager.Instance.ResetRoundState();

        UIManager.Instance.ShowConstructionGrid();
        turnManager.gameObject.SetActive(true);

        // Mostra transição antes do Player 1
        turnManager.ResetTurnState();
        turnManager.ShowTurnTransition(1);
    }

    // -----------------------------
    // UTILS
    // -----------------------------
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

    public bool IsLastBuildTurn()
    {
        int maxTurns = buildRounds * 2;
        return currentBuildTurn + 1 >= maxTurns;
    }

    // Notificações para ScoreManager
    public void PlayerReachedGoal(int playerId, int livesRemaining) =>
        ScoreManager.Instance.PlayerReachedGoal(playerId, livesRemaining);

    public void PlayerDied(int playerId) =>
        ScoreManager.Instance.PlayerDied(playerId);
}
