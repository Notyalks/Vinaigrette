using UnityEngine;
using UnityEngine.Tilemaps;

public class MiddleLineDeathZonesGenerator : MonoBehaviour
{
    public Tilemap tilemap; // Referência ao Tilemap do cenário
    public float middleY = 0f; // Altura da linha do meio
    public float triggerThickness = 0.2f; // Espessura do trigger
    public LayerMask playerLayer; // Camada dos jogadores

    private void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap não definido!");
            return;
        }

        Bounds bounds = tilemap.localBounds;
        float minX = bounds.min.x;
        float maxX = bounds.max.x;

        CriarZonaMorte(minX, maxX, middleY, false, 1); // Player 1 - zona inferior
        CriarZonaMorte(minX, maxX, middleY, true, 2);  // Player 2 - zona superior
    }

    private void CriarZonaMorte(float minX, float maxX, float y, bool isUpperLimit, int playerId)
    {
        GameObject zone = new GameObject($"DeathZone_Player{playerId}");
        zone.transform.parent = transform;
        zone.transform.position = new Vector3((minX + maxX) / 2f, y, 0f);

        BoxCollider2D col = zone.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(maxX - minX, triggerThickness);

        MiddleLineDeathZone script = zone.AddComponent<MiddleLineDeathZone>();
        script.playerId = playerId;
        script.isUpperLimit = isUpperLimit;
    }
}
