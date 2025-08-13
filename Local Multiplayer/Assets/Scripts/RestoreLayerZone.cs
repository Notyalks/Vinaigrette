using UnityEngine;

public class RestoreLayerZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        if (player.playerId == 2) // Só restaura para o Player 2
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null && !string.IsNullOrEmpty(player.originalSortingLayer))
            {
                sr.sortingLayerName = player.originalSortingLayer;
                sr.sortingOrder = player.originalSortingOrder;
            }

            player.RestoreOriginalSorting();
            player.SetBehindPlayer1(false);
        }
    }
}
