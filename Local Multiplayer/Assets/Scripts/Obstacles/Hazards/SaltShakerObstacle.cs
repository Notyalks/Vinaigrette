using UnityEngine;

public class SaltShakerObstacle : TimedObstacleBase
{
    [Header("Configurações de Disparo")]
    public GameObject projectilePrefab;   // Prefab do projétil
    public Transform firePoint;           // Ponto de onde o projétil sai
    public float projectileSpeed = 5f;    // Velocidade do projétil
    public Vector2 fireDirection = Vector2.right; // Direção do disparo

    protected override void OnInterval()
    {
        FireProjectile();
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = fireDirection.normalized * projectileSpeed;
        }
    }

    protected override void ResetObstacle()
    {
        base.ResetObstacle();
        // Nada especial aqui, pois o TimedObstacleBase já reinicia a rotina ao ativar
    }
}
