using UnityEngine;

public enum GameState { Build, Play }
public class GameManager : MonoBehaviour
{
    public TurnManager turnManager;
    public GameState state = GameState.Build;
    private int buildTurns = 0;

    void Update()
    {
        if (state == GameState.Build && Input.GetKeyDown(KeyCode.Return))
        {
            turnManager.NextTurn();
            buildTurns++;

            if (buildTurns >= 2) // Ambos jogaram
            {
                state = GameState.Play;
                turnManager.panelPlayer1.SetActive(false);
                turnManager.panelPlayer2.SetActive(false);
            }
        }
    }
}
