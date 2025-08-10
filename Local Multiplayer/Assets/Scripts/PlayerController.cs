using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

        if (col.collider.CompareTag("Hazard"))
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
            isActive = false; // sem controle
            ScoreManager.Instance.CheckRoundEnd();
        }
        else
        {
            // Respawn
            Vector3 respawnPos = GameManager.Instance.GetRandomStartPosition(playerId);
            transform.position = respawnPos;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void ReactivatePlayer()
    {
        isActive = true;
        hasFinished = false; // Volta a poder jogar
        rb.simulated = true;
        GetComponent<Collider2D>().enabled = true;
    }

    public void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Vidas: " + lives;
    }

    public void ReachGoal()
    {
        hasFinished = true;
        isActive = false;
        rb.linearVelocity = Vector2.zero; // Para imediatamente
    }
}
