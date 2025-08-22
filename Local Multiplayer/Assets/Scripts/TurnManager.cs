using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [Header("Managers")]
    public ObstaclePlacer placer;
    public InventoryManager inventoryManager;

    private bool waitingForInput = false;

    [Header("Timer")]
    public float turnDuration = 10f;
    private float currentTimer;
    private bool turnActive = false;

    private int currentPlayer = 1;
    private int pendingNextPlayer = -1;

    void Update()
    {
        if (waitingForInput)
        {
            if (Input.anyKeyDown)
            {
                waitingForInput = false;
                UIManager.Instance.HideTurnTransition();

                // Agora sempre inicia o turno depois da transição
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

        // Prepara inventário
        inventoryManager.SetupInventory(currentInventory, placementArea);
    }

    public void EndTurn()
    {
        turnActive = false;

        placer.EndPlacement();
        inventoryManager.ClearInventory();

        // Esconde HUD do jogador atual
        UIManager.Instance.ShowTurnUI(currentPlayer, false);

        // Notifica o GameManager
        GameManager.Instance.OnTurnEnded(currentPlayer);
    }

    public void ShowTurnTransition(int nextPlayer)
    {
        waitingForInput = true;
        pendingNextPlayer = nextPlayer;
        UIManager.Instance.ShowTurnTransition(nextPlayer);
    }

    // -----------------------------
    // TIMER
    // -----------------------------
    void UpdateTimerUI()
    {
        int time = Mathf.CeilToInt(currentTimer);
        UIManager.Instance.UpdateTimerUI(time);
    }

    // -----------------------------
    // RESET DE ESTADO
    // -----------------------------
    public void ResetTurnState()
    {
        waitingForInput = false;
        turnActive = false;
        pendingNextPlayer = -1;
        UIManager.Instance.HideTurnTransition();
    }

    // -----------------------------
    // GETTERS
    // -----------------------------
    public int GetCurrentPlayer() => currentPlayer;
}
