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
    private bool canControl = false;

    [Header("UI")]
    public Text livesText; // Use TMP_Text se estiver usando TextMeshPro

    public int playerId;

    [HideInInspector] public bool isActive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateLivesUI();
    }

    void Update()
    {
        if (!canControl) return;

        float move = 0f;

        if (gameObject.name == "Player1")
        {
            if (Input.GetKey(KeyCode.A)) move = -1;
            if (Input.GetKey(KeyCode.D)) move = 1;

            if (Input.GetKeyDown(KeyCode.W) && isGrounded)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftArrow)) move = -1;
            if (Input.GetKey(KeyCode.RightArrow)) move = 1;

            if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
    }

    public void EnableControl(bool enable)
    {
        canControl = enable;
        rb.linearVelocity = Vector2.zero;
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
    ///void OnTriggerEnter2D(Collider2D other)
    // {
    //    if (other.CompareTag("Goal"))
    //    {
    //        GameManager.Instance.EndGame(gameObject.name == "Player1" ? 1 : 2);
    //    }
    // }
    public void LoseLife()
    {
        lives--;
        UpdateLivesUI();

        if (lives <= 0)
        {
            // Jogador fica inativo, sem acabar o jogo
            isActive = false;
            rb.velocity = Vector2.zero;
            rb.simulated = false; // Desativa física
            GetComponent<Collider2D>().enabled = false; // Evita colisões

            GameManager.Instance.CheckRoundEnd();
        }
        else
        {
            // Respawn normal
            Vector3 respawnPos = GameManager.Instance.GetRandomStartPosition(playerId);
            transform.position = respawnPos;
            rb.velocity = Vector2.zero;
        }
    }
    public void ReactivatePlayer()
    {
        isActive = true;
        rb.simulated = true;
        GetComponent<Collider2D>().enabled = true;
    }

    public void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = "Vidas: " + lives;
    }
}
