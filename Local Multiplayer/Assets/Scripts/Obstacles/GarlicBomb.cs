using UnityEngine;
using System.Collections.Generic;

public class GarlicBomb : MonoBehaviour
{
    [Header("Configuração da Bomba")]
    public Vector2 areaSize = new Vector2(1f, 1f); // área da bomba
    public LayerMask obstacleMask;                 // layer de obstáculos
    public Color highlightColor = Color.red;       // cor antes de destruir
    public float destroyDelay = 0.1f;              // tempo antes de destruir

    [Header("Efeitos")]
    public GameObject explosionEffect;
    public float effectDuration = 0.5f; // tempo para mostrar efeitos antes de trocar turno

    private bool placed = false;

    void Update()
    {
        if (placed) return;

        // Só para visual feedback antes de colocar
        Vector3 spriteCenter = GetComponent<SpriteRenderer>().bounds.center;
        Collider2D[] hits = Physics2D.OverlapBoxAll(spriteCenter, areaSize, 0f, obstacleMask);

        foreach (var hit in hits)
        {
            SpriteRenderer sr = hit.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = highlightColor;
        }
    }

    /// <summary>
    /// Chamado pelo ObstaclePlacer quando a bomba é confirmada no cenário
    /// </summary>
    public void Place()
    {
        if (placed) return;
        placed = true;

        Vector3 spriteCenter = GetComponent<SpriteRenderer>().bounds.center;
        Collider2D[] hits = Physics2D.OverlapBoxAll(spriteCenter, areaSize, 0f, obstacleMask);

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                Debug.Log($"[GarlicBomb] Destruindo obstáculo: {hit.name}");
                Destroy(hit.gameObject, destroyDelay);
            }
        }
        else
        {
            Debug.Log("[GarlicBomb] Nenhum obstáculo atingido.");
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, spriteCenter, Quaternion.identity);
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
