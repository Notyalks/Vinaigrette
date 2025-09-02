using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Painéis")]
    public GameObject preparePanel;
    public CanvasGroup prepareCanvasGroup;
    public GameObject constructionGrid;

    [Header("Configurações de Transição")]
    public float prepareDuration = 3f;
    public float fadeDuration = 1f;

    [Header("Tela de Vitória")]
    public GameObject winScreen;
    public Text winText;

    [Header("Turnos - Painéis de Ocultação")]
    public GameObject panelPlayer1;
    public GameObject panelPlayer2;

    [Header("Turnos - Inventários")]
    public GameObject inventoryPlayer1;
    public GameObject inventoryPlayer2;

    [Header("Turnos - Painéis de Transição")]
    public GameObject turnTransitionPanelP1;
    public Text turnTransitionTextP1;
    public GameObject turnTransitionPanelP2;
    public Text turnTransitionTextP2;

    [Header("Turnos - Timer UI")]
    public Text timerPlayer1;
    public Text timerPlayer2;

    [Header("Referências Extras")]
    public GameObject startIndicatorP1;
    public GameObject goalIndicatorP1;
    public GameObject startIndicatorP2;
    public GameObject goalIndicatorP2;

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
        preparePanel.SetActive(true);
        prepareCanvasGroup.alpha = 1f;

        // Esconde os indicadores de início/fim
        HideIndicators();

        // Ativa jogadores + UI de vidas antes do fade-out
        GameManager.Instance.player1.SetLivesUIVisible(true);
        GameManager.Instance.player2.SetLivesUIVisible(true);

        GameManager.Instance.player1.gameObject.SetActive(true);
        GameManager.Instance.player2.gameObject.SetActive(true);

        StartCoroutine(FadeOutPrepareAndStartGameplay());
    }

    private IEnumerator FadeOutPrepareAndStartGameplay()
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
        GameManager.Instance.StartPlayPhase(); // depois do fade, começa o jogo
    }

    // -----------------------------
    // CONSTRUCTION GRID
    // -----------------------------
    public void ShowConstructionGrid() =>
        constructionGrid.SetActive(true);

    public void HideConstructionGrid() =>
        constructionGrid.SetActive(false);

    // -----------------------------
    // WIN SCREEN
    // -----------------------------
    public void ShowWinScreen(int winner)
    {
        winScreen.SetActive(true);
        winText.text = "Jogador " + winner + " venceu!";
    }

    public void HideWinScreen() =>
        winScreen.SetActive(false);

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
        timerPlayer2.gameObject.SetActive(player == 2 && active);
    }

    public void UpdateTimerUI(int time)
    {
        timerPlayer1.text = time.ToString();
        timerPlayer2.text = time.ToString();
    }

    public void ShowTurnTransition(int nextPlayer)
    {
        // garante estado limpo antes de ativar
        turnTransitionPanelP1.SetActive(false);
        turnTransitionPanelP2.SetActive(false);

        if (nextPlayer == 1)
        {
            turnTransitionTextP1.text = "Vez do Jogador 1";
            turnTransitionPanelP1.SetActive(true);
        }
        else
        {
            turnTransitionTextP2.text = "Vez do Jogador 2";
            turnTransitionPanelP2.SetActive(true);
        }
    }


    public void HideTurnTransition(int player)
    {
        if (player == 1) turnTransitionPanelP1.SetActive(false);
        else turnTransitionPanelP2.SetActive(false);
    }

    // -----------------------------
    // INDICADORES
    // -----------------------------
    public void HideIndicators()
    {
        if (startIndicatorP1 != null) startIndicatorP1.SetActive(false);
        if (startIndicatorP2 != null) startIndicatorP2.SetActive(false);
        if (goalIndicatorP1 != null) goalIndicatorP1.SetActive(false);
        if (goalIndicatorP2 != null) goalIndicatorP2.SetActive(false);
    }
}
