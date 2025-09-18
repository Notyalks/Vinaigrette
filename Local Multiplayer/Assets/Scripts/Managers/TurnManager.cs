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
    [SerializeField] private float inputDelay = 0.25f;

    // --- Controle do painel de modo ---
    private BuildMode lastShownBuildMode;
    private bool hasShownModeBefore = false;

    void Update()
    {
        // se saiu da fase de construção, garante reset
        if (PauseMenu.GameIsPaused || GameManager.Instance == null || GameManager.Instance.currentState != GameState.Build)
        {
            if (waitingForInput)
            {
                waitingForInput = false;
                pendingNextPlayer = -1;
            }
            return;
        }

        if (waitingForInput)
        {
            if (Time.unscaledTime >= allowInputAt && Input.anyKeyDown)
            {
                waitingForInput = false;
                UIManager.Instance.HideTurnTransition(pendingNextPlayer);

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
        int inventoryOwner; // novo

        if (GameManager.Instance.currentBuildMode == BuildMode.Adversary)
        {
            currentInventory = (currentPlayer == 1)
                ? UIManager.Instance.inventoryPlayer1
                : UIManager.Instance.inventoryPlayer2;

            placementArea = (currentPlayer == 1)
                ? placer.placementAreaPlayer2
                : placer.placementAreaPlayer1;

            inventoryOwner = currentPlayer; // usa o próprio
            UIManager.Instance.ShowTurnUI(currentPlayer, true);
        }
        else // modo próprio (self)
        {
            if (currentPlayer == 1)
            {
                currentInventory = UIManager.Instance.inventoryPlayer2;
                placementArea = placer.placementAreaPlayer1;

                inventoryOwner = 2; // invertido
                UIManager.Instance.ShowTurnUI(2, true);
            }
            else
            {
                currentInventory = UIManager.Instance.inventoryPlayer1;
                placementArea = placer.placementAreaPlayer2;

                inventoryOwner = 1; // invertido
                UIManager.Instance.ShowTurnUI(1, true);
            }
        }

        int placementOwner = (placementArea == placer.placementAreaPlayer1) ? 1 : 2;
        UIManager.Instance.ShowConstructionGridForOwner(placementOwner);

        // passa o id do jogador que define os obstáculos visíveis
        inventoryManager.SetupInventory(currentInventory, placementArea, inventoryOwner);
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
        // Se nunca mostrou o modo ainda, ou se o modo atual mudou, mostramos o painel de modo.
        BuildMode currentMode = GameManager.Instance.currentBuildMode;
        if (!hasShownModeBefore || currentMode != lastShownBuildMode)
        {
            hasShownModeBefore = true;
            lastShownBuildMode = currentMode;

            string msg = (currentMode == BuildMode.Adversary)
                ? "Dificulte o caminho do seu adversário!"
                : "Construa no seu próprio caminho!";

            UIManager.Instance.ShowModeInfo(msg, () =>
            {
                // callback: depois do dismiss, inicia a transição normal
                BeginTransition(nextPlayer);
            });
        }
        else
        {
            BeginTransition(nextPlayer);
        }
    }

    private void BeginTransition(int nextPlayer)
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

    public int GetCurrentPlayer() => currentPlayer;
}
