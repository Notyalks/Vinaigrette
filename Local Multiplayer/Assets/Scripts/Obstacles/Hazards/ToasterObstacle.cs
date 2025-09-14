using UnityEngine;

public class ToasterObstacle : TimedObstacleBase
{
    [Header("Disparo")]
    public Transform firePoint;
    public BreadSliceProjectile breadPrefab;
    public float projectileSpeed = 5f;

    [Tooltip("Se true, o pão trava no eixo dominante (X ou Y).")]
    public bool snapToAxis = true;

    [Tooltip("Inverte a direção calculada.")]
    public bool invertDirection = false;

    private Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    protected override void OnInterval()
    {
        // Toca a animação (ex: "Pop")
        if (anim != null)
            anim.SetTrigger("Pop");
        else
            LaunchBread();
    }

    // Chamado via Animation Event
    public void LaunchBread()
    {
        if (breadPrefab == null || firePoint == null) return;

        var bread = Instantiate(breadPrefab, firePoint.position, firePoint.rotation);
        bread.speed = projectileSpeed;

        Vector2 rawDir = firePoint.up; // torradeira lança para cima por padrão
        if (invertDirection) rawDir = -rawDir;

        bread.Launch(rawDir, snapToAxis);
    }
}
