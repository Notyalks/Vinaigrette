using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public int maxLives = 3;
    public int lives = 3;

    private Rigidbody2D rb;
    private bool isGrounded;

    [Header("UI")]
    public Text livesText;

    public int playerId;

    public bool hasFinished = false;
    public bool isActive = true; // Jogador está "vivo"

    public float respawnInvincibleTime = 2f; // Tempo de invencibilidade
    private bool invincible = false;
    private SpriteRenderer spriteRenderer;

    public bool isInvincible = false;
    public float invincibleTime = 2f;
    public bool IsInvincible() => isInvincible;

    [HideInInspector] public string originalSortingLayer;
    [HideInInspector] public int originalSortingOrder;

    public bool IsBehindPlayer1 { get; private set; } = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer.sortingOrder;
        UpdateLivesUI();
    }

    void Update()
    {
        // 🔹 Impede qualquer movimento fora do jogo ou se já terminou
        if (!isActive || hasFinished || GameManager.Instance.currentState != GameState.Play)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float move = 0f;

        // Movimento do Player 1
        if (gameObject.name == "Player1")
        {
            if (Input.GetKey(KeyCode.A)) move = -1;
            if (Input.GetKey(KeyCode.D)) move = 1;

            if (Input.GetKeyDown(KeyCode.W) && isGrounded)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        // Movimento do Player 2
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow)) move = -1;
            if (Input.GetKey(KeyCode.RightArrow)) move = 1;

            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.contacts[0].normal.y > 0.5f)
            isGrounded = true;

        if (col.collider.CompareTag("Hazard") && !invincible)
        {
            LoseLife();
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.contacts[0].normal.y > 0.5f)
            isGrounded = false;
    }

    public void LoseLife()
    {
        lives--;
        UpdateLivesUI();

        if (lives <= 0)
        {
            isActive = false;
            ScoreManager.Instance.CheckRoundEnd();
        }
        else
        {
            // Respawn normal
            Vector3 respawnPos = GameManager.Instance.GetRandomStartPosition(playerId);
            transform.position = respawnPos;
            rb.linearVelocity = Vector2.zero;

            ReactivatePlayer();
            StartCoroutine(RespawnInvincibility());
            StartCoroutine(SetInvincible(invincibleTime));
        }
    }

    public void ReactivatePlayer()
    {
        isActive = true;
        hasFinished = false;
        rb.simulated = true;
        GetComponent<Collider2D>().enabled = true;

        if (playerId == 1)
            RestoreOriginalSorting();

        // Ao respawn, ninguém fica “atrás” do outro por padrão
        SetBehindPlayer1(false);
    }
    public void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Vidas: " + lives;
    }

    public void BeginGoalEntry(Vector2 goalCenter)
    {
        isActive = false; // desliga controles
        hasFinished = true;

        StartCoroutine(GoalEntryMovement(goalCenter));
    }

    private IEnumerator GoalEntryMovement(Vector2 goalCenter)
    {
        float entryDuration = 0.5f; // tempo puxando para o centro
        float timer = 0f;

        while (timer < entryDuration)
        {
            // Só ajusta no X, mantém Y original
            float targetX = goalCenter.x;
            float dirX = targetX - transform.position.x;

            rb.AddForce(new Vector2(dirX * 30f, 0f), ForceMode2D.Force);

            timer += Time.deltaTime;
            yield return null;
        }

        // Para o movimento final
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private IEnumerator RespawnInvincibility()
    {
        isActive = false;
        invincible = true;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < respawnInvincibleTime)
        {
            // Alterna visibilidade (efeito de piscar)
            visible = !visible;
            spriteRenderer.enabled = visible;

            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        // Garante que o sprite fique visível no final
        spriteRenderer.enabled = true;
        invincible = false;
        isActive = true;
    }
    public IEnumerator SetInvincible(float time)
    {
        isInvincible = true;
        // Aqui pode ativar animação de piscar
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }
    public void RestoreOriginalSorting()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && !string.IsNullOrEmpty(originalSortingLayer))
        {
            sr.sortingLayerName = originalSortingLayer;
            sr.sortingOrder = originalSortingOrder;
        }
    }
    public void SetBehindPlayer1(bool state)
    {
        IsBehindPlayer1 = state;
        // Debug opcional
        // Debug.Log($"P{playerId} IsBehindPlayer1 = {state}");
    }
}
