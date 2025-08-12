using UnityEngine;

public class MiddleLineTransition : MonoBehaviour
{
    public string layerForPlayer1 = "BehindPath2";
    public string layerForPlayer2 = "BehindPath1";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // Armazena original se ainda não tiver salvo
        if (string.IsNullOrEmpty(player.originalSortingLayer))
        {
            player.originalSortingLayer = sr.sortingLayerName;
            player.originalSortingOrder = sr.sortingOrder;
        }

        if (player.playerId == 1)
        {
            sr.sortingLayerName = layerForPlayer1;
            sr.sortingOrder = 0; // Ordem para ficar atrás do cenário do Player 2
        }
        else
        {
            sr.sortingLayerName = layerForPlayer2;
            sr.sortingOrder = 0; // Ordem para ficar atrás do cenário do Player 1
        }
    }
}
