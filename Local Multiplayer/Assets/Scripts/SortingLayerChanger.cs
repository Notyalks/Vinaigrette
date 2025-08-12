using UnityEngine;

public class SortingLayerChanger : MonoBehaviour
{
    public string newSortingLayer;
    public int newSortingOrder = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = newSortingLayer;
                sr.sortingOrder = newSortingOrder;
            }
        }
    }
}
