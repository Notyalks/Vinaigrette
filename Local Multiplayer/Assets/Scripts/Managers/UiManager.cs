using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Painéis")]
    public GameObject preparePanel;
    public CanvasGroup prepareCanvasGroup;
    public GameObject constructionGrid;

    [Header("Tela de Vitória")]
    public GameObject winScreen;

    [Header("Turnos - Painéis de Ocultação")]
    public GameObject panelPlayer1;
    public GameObject panelPlayer2;

    [Header("Turnos - Inventários")]
    public GameObject inventoryPlayer1;
    public GameObject inventoryPlayer2;

    [Header("Turnos - Painéis de Transição")]
    public GameObject turnTransitionPanelP1;
    public GameObject turnTransitionPanelP2;

    [Header("Turnos - Timer UI")]
    public TextMeshProUGUI timerPlayer1;
    public GameObject imageTimerPlayer1;
    public TextMeshProUGUI timerPlayer2;
    public GameObject imageTimerPlayer2;

    [Header("Indicadores visuais")]
    public GameObject startIndicatorP1;
    public GameObject goalIndicatorP1;
    public GameObject startIndicatorP2;
    public GameObject goalIndicatorP2;

    [Header("Indicadores de Texto")]
    public TextMeshProUGUI startTextP1;
    public TextMeshProUGUI goalTextP1;
    public TextMeshProUGUI startTextP2;
    public TextMeshProUGUI goalTextP2;

    [Header("Painel de Modo (Adversário/Próprio)")]
    public GameObject modeInfoPanel;
    public TextMeshProUGUI modeInfoText;
    [Tooltip("Tempo mínimo (segundos) que o painel do modo fica visível antes de aceitar input para fechar.")]
    public float modeInfoMinDisplay = 0.12f;

    public PreparePanelController preparePanelController;

    // coroutine handle para evitar múltiplas chamadas sobrepostas
    private Coroutine modeInfoCoroutine;

    void Awake()
    {
        Instance = this;
    }

    // -----------------------------
    // PREPARE PANEL
    // -----------------------------
    public void ShowPreparePanel()
    {
        constructionGrid.SetActive(false);
        preparePanelController.Show();

        HideIndicators();

        GameManager.Instance.player1.SetLivesUIVisible(true);
        GameManager.Instance.player2.SetLivesUIVisible(true);

        GameManager.Instance.player1.gameObject.SetActive(true);
        GameManager.Instance.player2.gameObject.SetActive(true);
    }

    // -----------------------------
    // CONSTRUCTION GRID
    // -----------------------------
    public void ShowConstructionGrid()
    {
        constructionGrid.SetActive(true);
        HideIndicators();
    }
    public void ShowConstructionGridForOwner(int owner)
    {
        ShowConstructionGrid();

        if (owner == 1)
        {
            if (startIndicatorP1) startIndicatorP1.SetActive(true);
            if (goalIndicatorP1) goalIndicatorP1.SetActive(true);
            if (startTextP1) startTextP1.gameObject.SetActive(true);
            if (goalTextP1) goalTextP1.gameObject.SetActive(true);
        }
        else // owner == 2
        {
            if (startIndicatorP2) startIndicatorP2.SetActive(true);
            if (goalIndicatorP2) goalIndicatorP2.SetActive(true);
            if (startTextP2) startTextP2.gameObject.SetActive(true);
            if (goalTextP2) goalTextP2.gameObject.SetActive(true);
        }
    }

    public void HideConstructionGrid()
    {
        constructionGrid.SetActive(false);
        HideIndicators();
    }

    // -----------------------------
    // WIN SCREEN
    // -----------------------------
    public void ShowWinScreen(int winner)
    {
        if (winScreen != null)
            winScreen.SetActive(true);
        // Texto não é mais usado, WinScreenManager cuida da imagem
    }

    public void HideWinScreen()
    {
        if (winScreen != null)
            winScreen.SetActive(false);
    }

    // -----------------------------
    // TURNOS
    // -----------------------------
    public void ShowTurnUI(int player, bool active)
    {
        panelPlayer1.SetActive(player == 1 && active);
        panelPlayer2.SetActive(player == 2 && active);

        inventoryPlayer1.SetActive(player == 1 && active);
        inventoryPlayer2.SetActive(player == 2 && active);

        timerPlayer1.gameObject.SetActive(player == 1 && active);
        imageTimerPlayer1.SetActive(player == 1 && active);
        timerPlayer2.gameObject.SetActive(player == 2 && active);
        imageTimerPlayer2.SetActive(player == 2 && active);
    }

    public void UpdateTimerUI(int time)
    {
        timerPlayer1.text = time.ToString();
        timerPlayer2.text = time.ToString();
    }

    // -----------------------------
    // TURN TRANSITION (sem texto "Vez do jogador X")
    // -----------------------------
    public void ShowTurnTransition(int nextPlayer)
    {
        // garante estado limpo antes de ativar
        turnTransitionPanelP1.SetActive(false);
        turnTransitionPanelP2.SetActive(false);

        if (nextPlayer == 1)
            turnTransitionPanelP1.SetActive(true);
        else
            turnTransitionPanelP2.SetActive(true);
    }

    public void HideTurnTransition(int player)
    {
        if (player == 1) turnTransitionPanelP1.SetActive(false);
        else turnTransitionPanelP2.SetActive(false);
    }

    // -----------------------------
    // Painel de Modo (sobrecargas)
    // -----------------------------
    public void ShowModeInfo(BuildMode mode, Action onDismiss = null)
    {
        string msg = (mode == BuildMode.Adversary)
            ? "Dificulte o caminho do seu adversário!"
            : "Construa no seu próprio caminho!";
        ShowModeInfo(msg, onDismiss);
    }

    public void ShowModeInfo(string message, Action onDismiss = null)
    {
        if (modeInfoPanel == null || modeInfoText == null)
        {
            onDismiss?.Invoke();
            return;
        }

        // atualiza texto e ativa painel
        modeInfoText.text = message;
        modeInfoPanel.SetActive(true);

        // garante que não existam corrotinas duplicadas
        if (modeInfoCoroutine != null)
        {
            StopCoroutine(modeInfoCoroutine);
            modeInfoCoroutine = null;
        }

        modeInfoCoroutine = StartCoroutine(ModeInfoRoutine(onDismiss));
    }

    private IEnumerator ModeInfoRoutine(Action onDismiss)
    {
        // pequena espera para evitar fechar instantaneamente por um input que ocorreu um quadro antes
        if (modeInfoMinDisplay > 0f)
            yield return new WaitForSecondsRealtime(modeInfoMinDisplay);

        // espera por qualquer tecla
        while (!Input.anyKeyDown)
            yield return null;

        // fecha e chama callback
        modeInfoPanel.SetActive(false);
        modeInfoCoroutine = null;

        onDismiss?.Invoke();
    }

    public void HideModeInfo()
    {
        if (modeInfoCoroutine != null)
        {
            StopCoroutine(modeInfoCoroutine);
            modeInfoCoroutine = null;
        }
        if (modeInfoPanel != null)
            modeInfoPanel.SetActive(false);
    }

    // -----------------------------
    // INDICADORES
    // -----------------------------
    public void HideIndicators()
    {
        if (startIndicatorP1) startIndicatorP1.SetActive(false);
        if (goalIndicatorP1) goalIndicatorP1.SetActive(false);
        if (startIndicatorP2) startIndicatorP2.SetActive(false);
        if (goalIndicatorP2) goalIndicatorP2.SetActive(false);

        if (startTextP1) startTextP1.gameObject.SetActive(false);
        if (goalTextP1) goalTextP1.gameObject.SetActive(false);
        if (startTextP2) startTextP2.gameObject.SetActive(false);
        if (goalTextP2) goalTextP2.gameObject.SetActive(false);
    }
}
