using UnityEngine;

public class RollingPinObstacle : ActiveObstacleBase
{
    [Header("Movimento")]
    public bool moveHorizontally = true;
    public float moveDistance = 2f;
    public float moveSpeed = 2f;

    private Vector3 startPos;
    private int direction = 1;

    protected override void Start()
    {
        base.Start();
        startPos = transform.position;
    }

    protected override void ActiveUpdate()
    {
        Vector3 axis = moveHorizontally ? transform.right : transform.up;
        transform.position += axis * direction * moveSpeed * Time.deltaTime;

        float distanceFromStart = Vector3.Project(transform.position - startPos, axis).magnitude;

        if (distanceFromStart >= moveDistance)
        {
            transform.position = startPos + axis * (moveDistance * direction);
            direction *= -1;
        }
    }

    protected override void ResetObstacle()
    {
        base.ResetObstacle();
        transform.position = startPos;
        direction = 1;
    }

    // -----------------------------
    // Player "anda junto"
    // -----------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(transform, true); // entra sem problema
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Transform player = collision.transform;

            // salva a posição mundial antes de soltar
            Vector3 worldPos = player.position;
            Quaternion worldRot = player.rotation;

            player.SetParent(null, false);

            // restaura posição e rotação exata
            player.position = worldPos;
            player.rotation = worldRot;
        }
    }

}
