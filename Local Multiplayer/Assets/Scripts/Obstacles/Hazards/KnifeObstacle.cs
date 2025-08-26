using UnityEngine;

public class KnifeObstacle : ActiveObstacleBase
{
    [Header("Configurações de Rotação")]
    public float rotationSpeed = 60f; // Velocidade da rotação em graus por segundo
    public bool clockwise = true;     // Se a rotação será no sentido horário ou anti-horário

    private Vector3 pivotOffset;   // Offset para centralizar o pivot
    private Vector3 pivotWorld;    // Ponto do mundo usado como pivot de rotação

    protected override void Start()
    {
        base.Start();

        // recalculamos o centro do objeto
        Bounds bounds = GetComponent<SpriteRenderer>().bounds;
        pivotOffset = new Vector3(bounds.size.x / 2f, bounds.size.y / 2f, 0);

        // Converte para coordenada mundial
        pivotWorld = transform.position + pivotOffset;
    }

    protected override void ActiveUpdate()
    {
        float direction = clockwise ? -1f : 1f;

        // Rotaciona em torno do centro do sprite
        transform.RotateAround(pivotWorld, Vector3.forward, rotationSpeed * direction * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;

        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null && !player.invincible)
        {
            player.LoseLife();
        }
    }
}
