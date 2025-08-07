using UnityEngine;
using UnityEngine.UI;

public enum GameState { Build, Play, End }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton simples
    public GameState currentState = GameState.Build;

    [Header("Turnos de Construção")]
    public int buildRounds = 1; // quantos turnos cada jogador tem antes de começar gameplay
    private int currentBuildTurn = 0;

    [Header("Referências")]
    public TurnManager turnManager;
    public PlayerController player1;
    public PlayerController player2;

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
        turnManager.gameObject.SetActive(true);
        turnManager.StartTurn(1); // começa com jogador 1
    }

    public void OnTurnEnded()
    {
        // Conta cada vez que qualquer jogador termina um turno
        currentBuildTurn++;

        // Cada "round" de construção = 2 turnos (P1 + P2)
        int maxTurns = buildRounds * 2;

        if (currentBuildTurn >= maxTurns)
        {
            StartPlayPhase();
        }
    }

    public void StartPlayPhase()
    {
        currentState = GameState.Play;
        turnManager.gameObject.SetActive(false); // desativa sistema de turnos
        player1.EnableControl(true);
        player2.EnableControl(true);
    }

    public void EndGame(int winner)
    {
        currentState = GameState.End;

        player1.EnableControl(false);
        player2.EnableControl(false);

        Debug.Log("Jogador " + winner + " venceu!");

        // Ativa tela de vitória
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            winText.text = "Jogador " + winner + " venceu!";
        }
    }
}
