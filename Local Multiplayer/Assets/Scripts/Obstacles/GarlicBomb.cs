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

        // toca a animação da bomba
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Explode");
            // espera a duração da animação antes de destruir
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        }

        // Efeito de partículas extra (se tiver)
        if (explosionEffect != null)
            Instantiate(explosionEffect, spriteCenter, Quaternion.identity);

        // Destrói obstáculos na área
        Collider2D[] hits = Physics2D.OverlapBoxAll(spriteCenter, areaSize, 0f, obstacleMask);

        foreach (var hit in hits)
        {
            Debug.Log($"[GarlicBomb] Destruindo obstáculo: {hit.name}");

            DestroyEffect effect = hit.GetComponent<DestroyEffect>();
            if (effect != null) effect.PlayEffect();

            SpriteRenderer sr = hit.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            Collider2D col = hit.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            Destroy(hit.gameObject, destroyDelay);
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
