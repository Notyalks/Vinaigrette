using UnityEngine;

public class MiddleLineTransition : MonoBehaviour
{
    [Header("Sorting layers para ficar atrás do outro caminho")]
    public string layerForPlayer1 = "BehindPath2";
    public string layerForPlayer2 = "BehindPath1";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        var sr = player.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // Salva layer original apenas uma vez
        if (string.IsNullOrEmpty(player.originalSortingLayer))
        {
            player.originalSortingLayer = sr.sortingLayerName;
            player.originalSortingOrder = sr.sortingOrder;
        }

        if (player.playerId == 1)
        {
            // Player 1 vai para trás do caminho do Player 2
            sr.sortingLayerName = layerForPlayer1;
            sr.sortingOrder = 0;
            // Indicador do P2 não precisa aqui; P1 não usa essa flag
            player.SetBehindPlayer1(false);
        }
        else // playerId == 2
        {
            // Player 2 vai para trás do caminho do Player 1
            sr.sortingLayerName = layerForPlayer2;
            sr.sortingOrder = 0;

            // Liga a flag para o indicador do P2 aparecer
            player.SetBehindPlayer1(true);
        }
    }
}
