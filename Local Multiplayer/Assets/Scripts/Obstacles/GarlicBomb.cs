using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class GarlicBomb : MonoBehaviour
{
    [Header("Configuração da Bomba")]
    public Vector2 areaSize = new Vector2(1f, 1f);
    public LayerMask obstacleMask;
    public Color highlightColor = Color.red;
    public float destroyDelay = 0.1f;

    public GameObject explosionEffect;

    public event Action OnBombResolved;

    private bool placed = false;

    // Guardar as cores originais
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();

    void Update()
    {
        if (placed) return;

        // Resetar cores antigas
        foreach (var kvp in originalColors)
        {
            if (kvp.Key != null) kvp.Key.color = kvp.Value;
        }
        originalColors.Clear();

        // Só para feedback antes de colocar
        Vector3 spriteCenter = GetComponent<SpriteRenderer>().bounds.center;
        Collider2D[] hits = Physics2D.OverlapBoxAll(spriteCenter, areaSize, 0f, obstacleMask);

        foreach (var hit in hits)
        {
            SpriteRenderer sr = hit.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (!originalColors.ContainsKey(sr))
                    originalColors[sr] = sr.color;

                sr.color = highlightColor;
            }
        }
    }

    public void Place()
    {
        if (placed) return;
        placed = true;

        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        Vector3 spriteCenter = GetComponent<SpriteRenderer>().bounds.center;

        // Efeito da bomba
        if (explosionEffect != null)
            Instantiate(explosionEffect, spriteCenter, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapBoxAll(spriteCenter, areaSize, 0f, obstacleMask);

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                Debug.Log($"[GarlicBomb] Destruindo obstáculo: {hit.name}");

                // Toca o efeito imediatamente
                DestroyEffect effect = hit.GetComponent<DestroyEffect>();
                if (effect != null)
                    effect.PlayEffect();

                // Some imediatamente (sprite e colisão)
                SpriteRenderer sr = hit.GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = false;

                Collider2D col = hit.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                // Destrói de fato depois
                Destroy(hit.gameObject, destroyDelay);
            }
        }
        else
        {
            Debug.Log("[GarlicBomb] Nenhum obstáculo atingido.");
        }

        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
        OnBombResolved?.Invoke();
    }

    void OnDestroy()
    {
        // Restaura as cores se a bomba for cancelada
        foreach (var kvp in originalColors)
        {
            if (kvp.Key != null) kvp.Key.color = kvp.Value;
        }
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
