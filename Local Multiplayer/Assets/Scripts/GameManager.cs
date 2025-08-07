using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    [Header("Elementos Visuais")]
    public GameObject constructionGrid;

    public GameObject winScreen;
    public Text winText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartBuildPhase();
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

        player1.EnableControl(false);
        player2.EnableControl(false);

        constructionGrid.SetActive(false);

        // Ativa tela de vitória
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            winText.text = "Jogador " + winner + " venceu!";
        }
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
}
