using UnityEngine;
using System.Collections.Generic;

public class GarlicBomb : MonoBehaviour
{
    [Header("Configuração da Bomba")]
    public Vector2 areaSize = new Vector2(1f, 1f); // área da bomba
    public LayerMask obstacleMask;                 // layer de obstáculos
    public Color highlightColor = Color.red;       // cor antes de destruir
    public float destroyDelay = 0.1f;              // tempo antes de destruir

    private List<GameObject> obstaclesInRange = new List<GameObject>();
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
    private bool placed = false;

    void Update()
    {
        if (placed) return;

        Vector3 spriteCenter = GetComponent<SpriteRenderer>().bounds.center;
        Collider2D[] hits = Physics2D.OverlapBoxAll(spriteCenter, areaSize, 0f, obstacleMask);

        // Reset cores antigas
        foreach (var kvp in originalColors)
        {
            if (kvp.Key != null)
                kvp.Key.color = kvp.Value;
        }
        obstaclesInRange.Clear();
        originalColors.Clear();

        // Marcar obstáculos encontrados
        foreach (var hit in hits)
        {
            SpriteRenderer sr = hit.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (!originalColors.ContainsKey(sr))
                    originalColors[sr] = sr.color;

                sr.color = highlightColor;
                obstaclesInRange.Add(hit.gameObject);
            }
        }
    }

    /// <summary>
    /// Chamado pelo ObstaclePlacer quando a bomba é confirmada no cenário
    /// </summary>
    public void Place()
    {
        if (placed) return;
        placed = true;

        if (obstaclesInRange.Count > 0)
        {
            foreach (var obj in obstaclesInRange)
            {
                if (obj != null)
                {
                    Debug.Log($"[GarlicBomb] Destruindo obstáculo: {obj.name}");
                    Destroy(obj, destroyDelay);
                }
            }
        }
        else
        {
            Debug.Log("[GarlicBomb] Nenhum obstáculo atingido.");
        }

        // A bomba sempre some
        Destroy(gameObject, destroyDelay);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 center = GetComponent<SpriteRenderer>() != null
            ? GetComponent<SpriteRenderer>().bounds.center
            : transform.position;

        Gizmos.DrawWireCube(center, areaSize);
    }
}
