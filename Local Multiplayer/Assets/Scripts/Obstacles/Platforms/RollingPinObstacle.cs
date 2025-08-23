using UnityEngine;

public class RollingPinObstacle : ActiveObstacleBase
{
    [Header("Movimento")]
    public bool moveHorizontally = true; // true = no eixo local X | false = no eixo local Y
    public float moveDistance = 2f;      // Distância máxima do movimento
    public float moveSpeed = 2f;         // Velocidade do movimento

    private Vector3 startPos;   // Posição inicial
    private int direction = 1;  // 1 = frente, -1 = trás

    protected override void Start()
    {
        base.Start();
        startPos = transform.position; // salva posição inicial
    }

    protected override void ActiveUpdate()
    {
        // Define direção no espaço local
        Vector3 axis = moveHorizontally ? transform.right : transform.up;

        // Move no eixo local
        transform.position += axis * direction * moveSpeed * Time.deltaTime;

        // Calcula distância percorrida em relação à posição inicial
        float distanceFromStart = Vector3.Project(transform.position - startPos, axis).magnitude;

        // Se passou do limite, inverte direção
        if (distanceFromStart >= moveDistance)
        {
            // Corrige posição para não passar do limite
            transform.position = startPos + axis * (moveDistance * direction);

            // Inverte direção
            direction *= -1;
        }
    }

    protected override void ResetObstacle()
    {
        base.ResetObstacle();
        transform.position = startPos; // volta pra posição original
        direction = 1; // reseta direção
    }
}
