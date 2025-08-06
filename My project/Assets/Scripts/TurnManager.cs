using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public GameObject panelPlayer1;
    public GameObject panelPlayer2;
    public ObstaclePlacer placer;

    private int currentPlayer = 1;

    void Start()
    {
        UpdatePanels();
        placer.StartPlayerTurn(); // Começa com jogador 1
    }

    public void NextTurn()
    {
        // Fecha inventário do jogador anterior
        placer.EndPlayerTurn();

        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        UpdatePanels();

        // Abre inventário para novo jogador
        placer.StartPlayerTurn();
    }

    void UpdatePanels()
    {
        panelPlayer1.SetActive(currentPlayer == 1);
        panelPlayer2.SetActive(currentPlayer == 2);
    }

    public int GetCurrentPlayer()
    {
        return currentPlayer;
    }
}
