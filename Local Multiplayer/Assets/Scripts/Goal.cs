using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null && player.isActive)
        {
            // Marca o jogador como tendo terminado
            player.ReachGoal();

            // Atualiza pontuação
            ScoreManager.Instance.PlayerReachedGoal(player.playerId, player.lives);
        }
    }
}
