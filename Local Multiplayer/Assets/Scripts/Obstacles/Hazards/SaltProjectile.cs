using UnityEngine;

public class SaltProjectile : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 5f;         // Velocidade do projétil
    public float lifetime = 5f;      // Tempo até ser destruído automaticamente
    public int damage = 1;           // Dano (quantas vidas tirar por acerto)

    [Header("Efeitos")]
    public GameObject hitEffectPrefab; // Prefab opcional de partículas para impacto

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // O projétil anda sempre na direção "frente" do objeto (eixo X positivo do transform)
            rb.linearVelocity = transform.right * speed;
        }

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se acertou um jogador
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null && player.isActive && !player.invincible)
        {
            for (int i = 0; i < damage; i++)
                player.LoseLife();

            SpawnHitEffect();
            Destroy(gameObject);
            return;
        }

        // Se acertar obstáculo ou terreno, também destrói
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Terrain"))
        {
            SpawnHitEffect();
            Destroy(gameObject);
        }
    }

    private void SpawnHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
