using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Pontuação")]
    public int scorePlayer1 = 0;
    public int scorePlayer2 = 0;
    public int winningScore = 1000;

    private bool player1Finished = false;
    private bool player2Finished = false;
    private bool player1ScoreFinalized = false;
    private bool player2ScoreFinalized = false;

    private int finishOrderCounter = 0;
    private int player1FinishOrder = 0;
    private int player2FinishOrder = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void ResetRoundState()
    {
        player1Finished = false;
        player2Finished = false;
        player1ScoreFinalized = false;
        player2ScoreFinalized = false;
        finishOrderCounter = 0;
        player1FinishOrder = 0;
        player2FinishOrder = 0;
    }

    public void PlayerReachedGoal(int playerId, int livesRemaining)
    {
        if (playerId == 1 && player1Finished) return;
        if (playerId == 2 && player2Finished) return;

        finishOrderCounter++;
        if (playerId == 1)
        {
            player1Finished = true;
            player1FinishOrder = finishOrderCounter;
        }
        else
        {
            player2Finished = true;
            player2FinishOrder = finishOrderCounter;
        }

        int pontosPorVida = livesRemaining * 50;
        if (playerId == 1)
        {
            scorePlayer1 += pontosPorVida;
            Debug.Log($"P1 chegou com {livesRemaining} vidas -> +{pontosPorVida}");
        }
        else
        {
            scorePlayer2 += pontosPorVida;
            Debug.Log($"P2 chegou com {livesRemaining} vidas -> +{pontosPorVida}");
        }

        // Ambos chegaram
        if (player1Finished && player2Finished)
        {
            scorePlayer1 += 50;
            scorePlayer2 += 50;

            if (player1FinishOrder < player2FinishOrder)
                scorePlayer1 += 50;
            else if (player2FinishOrder < player1FinishOrder)
                scorePlayer2 += 50;

            player1ScoreFinalized = true;
            player2ScoreFinalized = true;

            CheckForWinner();
            GameManager.Instance.EndRound();
            return;
        }

        // Um chegou, outro morreu
        if (playerId == 1 && !GameManager.Instance.player2.isActive)
        {
            scorePlayer1 += 100;
            player1ScoreFinalized = true;

            CheckForWinner();
            GameManager.Instance.EndRound();
            return;
        }
        else if (playerId == 2 && !GameManager.Instance.player1.isActive)
        {
            scorePlayer2 += 100;
            player2ScoreFinalized = true;

            CheckForWinner();
            GameManager.Instance.EndRound();
            return;
        }
    }

    public void PlayerDied(int playerId)
    {
        CheckRoundEnd();
    }

    public void CheckRoundEnd()
    {
        if (!GameManager.Instance.player1.isActive && !GameManager.Instance.player2.isActive)
        {
            CheckForWinner();
            GameManager.Instance.EndRound();
            return;
        }

        if (player1Finished && !GameManager.Instance.player2.isActive && !player1ScoreFinalized)
        {
            scorePlayer1 += 100;
            player1ScoreFinalized = true;

            CheckForWinner();
            GameManager.Instance.EndRound();
            return;
        }

        if (player2Finished && !GameManager.Instance.player1.isActive && !player2ScoreFinalized)
        {
            scorePlayer2 += 100;
            player2ScoreFinalized = true;

            CheckForWinner();
            GameManager.Instance.EndRound();
            return;
        }
    }

    private void CheckForWinner()
    {
        if (scorePlayer1 >= winningScore)
        {
            GameManager.Instance.EndGame(1);
        }
        else if (scorePlayer2 >= winningScore)
        {
            GameManager.Instance.EndGame(2);
        }
    }
}
