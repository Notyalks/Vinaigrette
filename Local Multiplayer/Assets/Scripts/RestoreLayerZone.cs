using UnityEngine;

public class RestoreLayerZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null) return;

        if (player.playerId == 2)
        {
            player.RestoreOriginalSorting();
            player.SetBehindPlayer1(false);

            player.SetOffscreenIndicatorActive(false);
        }
    }
}
