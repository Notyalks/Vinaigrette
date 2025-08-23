using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("Managers")]
    public ObstaclePlacer placer;
    public InventoryManager inventoryManager;

    [Header("Timer")]
    public float turnDuration = 10f;
    private float currentTimer;
    private bool turnActive = false;

    private int currentPlayer = 1;
    private int pendingNextPlayer = -1;

    // --- Controle da transição ---
    private bool waitingForInput = false;
    private float allowInputAt = 0f;
    [SerializeField] private float inputDelay = 0.25f; // tempo mínimo antes de aceitar input

    void Update()
    {
        if (waitingForInput)
        {
            if (Time.unscaledTime >= allowInputAt && Input.anyKeyDown)
            {
                waitingForInput = false;
                UIManager.Instance.HideTurnTransition(pendingNextPlayer);

                // Inicia o próximo turno
                StartTurn(pendingNextPlayer);
            }
            return;
        }

        if (!turnActive) return;

        currentTimer -= Time.deltaTime;
        UIManager.Instance.UpdateTimerUI(Mathf.CeilToInt(currentTimer));

        if (currentTimer <= 0f)
        {
            EndTurn();
        }
    }

    // -----------------------------
    // TURNOS
    // -----------------------------
    public void StartTurn(int player)
    {
        currentPlayer = player;
        currentTimer = turnDuration;
        turnActive = true;

        // Atualiza UI do jogador atual
        UIManager.Instance.ShowTurnUI(currentPlayer, true);

        // Define inventário e área de colocação
        GameObject currentInventory = (currentPlayer == 1)
            ? UIManager.Instance.inventoryPlayer1
            : UIManager.Instance.inventoryPlayer2;

        Collider2D placementArea = (currentPlayer == 1)
            ? placer.placementAreaPlayer2
            : placer.placementAreaPlayer1;

        inventoryManager.SetupInventory(currentInventory, placementArea);
    }

    public void EndTurn()
    {
        turnActive = false;

        placer.EndPlacement();
        inventoryManager.ClearInventory();

        UIManager.Instance.ShowTurnUI(currentPlayer, false);

        GameManager.Instance.OnTurnEnded(currentPlayer);
    }

    // Chamado pelo GameManager
    public void ShowTurnTransition(int nextPlayer)
    {
        waitingForInput = true;
        pendingNextPlayer = nextPlayer;
        allowInputAt = Time.unscaledTime + inputDelay;

        UIManager.Instance.ShowTurnTransition(nextPlayer);
    }

    // -----------------------------
    // Reset de Estado
    // -----------------------------
    public void ResetTurnState()
    {
        waitingForInput = false;
        turnActive = false;
        pendingNextPlayer = -1;
    }

    // Getter
    public int GetCurrentPlayer() => currentPlayer;
}
