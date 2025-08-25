using UnityEngine;

public class SaltShakerObstacle : TimedObstacleBase
{
    [Header("Disparo")]
    public Transform firePoint;
    public SaltProjectile projectilePrefab;
    public float projectileSpeed = 6f;

    [Tooltip("Se true, o projétil trava no eixo dominante (X ou Y).")]
    public bool snapToAxis = true;

    [Tooltip("Inverte a direção calculada (ex.: virar para esquerda em vez de direita).")]
    public bool invertDirection = false;

    protected override void OnInterval()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Instancia com a rotação do firePoint (apenas para orientação visual inicial)
        var proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Configura velocidade (pode ajustar direto no prefab também)
        proj.speed = projectileSpeed;

        // Calcula direção em MUNDO a partir do firePoint
        Vector2 rawDir = firePoint.right; // "frente" do firePoint
        if (invertDirection) rawDir = -rawDir;

        // Lança com snap de eixo (decide +X/-X ou +Y/-Y)
        proj.Launch(rawDir, snapToAxis);
    }
}
