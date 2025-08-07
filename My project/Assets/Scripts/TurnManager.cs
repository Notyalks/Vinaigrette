using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    [Header("Painéis de Ocultação")]
    public GameObject panelPlayer1;
    public GameObject panelPlayer2;

    [Header("Inventários")]
    public GameObject inventoryPlayer1;
    public GameObject inventoryPlayer2;

    [Header("Timer UI")]
    public Text timerPlayer1;
    public Text timerPlayer2;

    public ObstaclePlacer placer;

    [Header("Timer")]
    public float turnDuration = 10f;
    private float currentTimer;
    private bool turnActive = false;

    private int currentPlayer = 1;

    void Start()
    {
        StartTurn(1);
    }

    void Update()
    {
        if (!turnActive) return;

        currentTimer -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTimer <= 0f)
        {
            EndTurn();
        }
    }

    public void StartTurn(int player)
    {
        currentPlayer = player;
        currentTimer = turnDuration;
        turnActive = true;

        UpdatePanels();
        UpdateHUDVisibility();

        // Passa para o ObstaclePlacer qual inventário usar
        GameObject currentInventory = (currentPlayer == 1) ? inventoryPlayer1 : inventoryPlayer2;
        placer.StartPlayerTurn(currentPlayer, currentInventory);
    }

    public void EndTurn()
    {
        turnActive = false;

        // Desativa inventário e timer
        placer.EndPlayerTurn();
        UpdateHUDVisibility();

        // Desativa painel do jogador que acabou de jogar
        if (currentPlayer == 1)
            panelPlayer1.SetActive(false);
        else
            panelPlayer2.SetActive(false);

        GameManager.Instance.OnTurnEnded();

        if (GameManager.Instance.currentState == GameState.Build)
        {
            StartTurn(currentPlayer == 1 ? 2 : 1);
        }
    }


    void UpdatePanels()
    {
        panelPlayer1.SetActive(currentPlayer == 1);
        panelPlayer2.SetActive(currentPlayer == 2);
    }

    void UpdateHUDVisibility()
    {
        inventoryPlayer1.SetActive(currentPlayer == 1 && turnActive);
        inventoryPlayer2.SetActive(currentPlayer == 2 && turnActive);

        timerPlayer1.gameObject.SetActive(currentPlayer == 1 && turnActive);
        timerPlayer2.gameObject.SetActive(currentPlayer == 2 && turnActive);
    }

    void UpdateTimerUI()
    {
        int time = Mathf.CeilToInt(currentTimer);
        timerPlayer1.text = time.ToString();
        timerPlayer2.text = time.ToString();
    }

    public int GetCurrentPlayer() => currentPlayer;
}
