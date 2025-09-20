using UnityEngine;

public class SpoonObstacle : ActiveObstacleBase
{
    [Header("Configurações de Rotação")]
    public float rotationSpeed = 30f; // Velocidade da rotação em graus por segundo
    public bool clockwise = true;     // Se a rotação será no sentido horário ou anti-horário

    private SpriteRenderer sr;

    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
    }

    protected override void ActiveUpdate()
    {
        float direction = clockwise ? -1f : 1f;

        // centro do sprite em coordenadas locais
        Vector3 localCenter = sr.bounds.center - transform.position;

        // converte para mundo
        Vector3 pivotWorld = transform.position + localCenter;

        // gira em torno do centro real do sprite
        transform.RotateAround(pivotWorld, Vector3.forward, rotationSpeed * direction * Time.deltaTime);
    }

    protected override void ResetObstacle()
    {
        base.ResetObstacle();
    }
}
