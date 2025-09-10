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

    private Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    protected override void OnInterval()
    {
        // Em vez de disparar já, tocamos a animação
        if (anim != null)
            anim.SetTrigger("Fire");
        else
            FireProjectile(); // fallback sem animação
    }

    // Chamado via Animation Event no fim da animação "PreFire"
    public void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        var proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        proj.speed = projectileSpeed;

        Vector2 rawDir = firePoint.right;
        if (invertDirection) rawDir = -rawDir;

        proj.Launch(rawDir, snapToAxis);
    }
}
