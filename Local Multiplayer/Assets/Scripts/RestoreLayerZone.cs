using UnityEngine;

public class RestoreLayerZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        // Restaura a layer original
        player.RestoreOriginalSorting();

        // Se for o Player 2, desliga o indicador
        if (player.playerId == 2)
        {
            player.SetBehindPlayer1(false);
        }
    }
}
