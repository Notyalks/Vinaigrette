using UnityEngine;
using System.Collections;

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
        // Só processa lógica durante a fase de construção
        if (GameManager.Instance.currentState != GameState.Build)
            return;

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

        GameObject currentInventory;
        Collider2D placementArea;

        if (GameManager.Instance.currentBuildMode == BuildMode.Adversary)
        {
            currentInventory = (currentPlayer == 1)
                ? UIManager.Instance.inventoryPlayer1
                : UIManager.Instance.inventoryPlayer2;

            placementArea = (currentPlayer == 1)
                ? placer.placementAreaPlayer2
                : placer.placementAreaPlayer1;

            // mostra painel normal
            UIManager.Instance.ShowTurnUI(currentPlayer, true);
        }
        else // --- modo próprio ---
        {
            if (currentPlayer == 1)
            {
                currentInventory = UIManager.Instance.inventoryPlayer2;
                placementArea = placer.placementAreaPlayer1;

                // ativa painel 2 (já que player1 está construindo no caminho dele)
                UIManager.Instance.ShowTurnUI(2, true);
            }
            else // currentPlayer == 2
            {
                currentInventory = UIManager.Instance.inventoryPlayer1;
                placementArea = placer.placementAreaPlayer2;

                // ativa painel 1 (já que player2 está construindo no caminho dele)
                UIManager.Instance.ShowTurnUI(1, true);
            }
        }

        inventoryManager.SetupInventory(currentInventory, placementArea);
    }

    public void EndTurn(float extraDelay = 0f)
    {
        StartCoroutine(EndTurnRoutine(extraDelay));
    }

    private IEnumerator EndTurnRoutine(float extraDelay)
    {
        if (extraDelay > 0f)
            yield return new WaitForSeconds(extraDelay);

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
