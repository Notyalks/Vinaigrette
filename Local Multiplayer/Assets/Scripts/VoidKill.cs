using UnityEngine;

public class VoidKill : MonoBehaviour
{
    public int playerId; // 1 ou 2

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && player.playerId == playerId && !player.invincible)
        {
            player.LoseLife();
        }
    }
}
