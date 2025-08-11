using UnityEngine;

public class MiddleLineDeathZone : MonoBehaviour
{
    public int playerId; // 1 ou 2
    public bool isUpperLimit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null && player.playerId == playerId && !player.isInvincible)
        {
            player.LoseLife();
        }
    }
}
