using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null && player.isActive)
        {
            Vector2 goalCenter = GetComponent<Collider2D>().bounds.center;

            player.BeginGoalEntry(goalCenter);
            ScoreManager.Instance.PlayerReachedGoal(player.playerId, player.lives);
        }
    }
}
