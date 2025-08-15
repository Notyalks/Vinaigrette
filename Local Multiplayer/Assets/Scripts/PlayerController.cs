using System.Collections;
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
    public GameObject livesUI; // Container com os corações
    private Image[] heartImages;

    [Header("Indicador Fora da Tela")]
    public GameObject offscreenIndicator; // arrastar no Inspector

    public bool IsBehindPlayer1 { get; private set; }

    public int playerId;
    public bool hasFinished = false;
    public bool isActive = true;

    public float respawnInvincibleTime = 2f;
    private bool invincible = false;
    private SpriteRenderer spriteRenderer;

    public bool isInvincible = false;
    public float invincibleTime = 2f;
    public bool IsInvincible() => isInvincible;

    [HideInInspector] public string originalSortingLayer;
    [HideInInspector] public int originalSortingOrder;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer.sortingOrder;

        if (livesUI != null)
            heartImages = livesUI.GetComponentsInChildren<Image>();

        UpdateLivesUI();
    }

    void Update()
    {
        if (!isActive || hasFinished || GameManager.Instance.currentState != GameState.Play)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float move = 0f;

        if (playerId == 1)
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
        if (col.contacts.Length > 0 && col.contacts[0].normal.y > 0.5f)
            isGrounded = false;
    }

    public void LoseLife()
    {
        lives--;
        UpdateLivesUI();

        if (playerId == 2)
        {
            SetBehindPlayer1(false);
            RestoreOriginalSorting();
        }

        if (lives <= 0)
        {
            isActive = false;
            ScoreManager.Instance.CheckRoundEnd();
        }
        else
        {
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
        {
            RestoreOriginalSorting();
        }
        else if (playerId == 2)
        {
            SetBehindPlayer1(false);
            RestoreOriginalSorting();
        }
    }


    public void UpdateLivesUI()
    {
        if (heartImages == null) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < lives;
        }
    }

    public void BeginGoalEntry(Vector2 goalCenter)
    {
        isActive = false;
        hasFinished = true;
        StartCoroutine(GoalEntryMovement(goalCenter));
    }

    private IEnumerator GoalEntryMovement(Vector2 goalCenter)
    {
        float entryDuration = 0.5f;
        float timer = 0f;

        while (timer < entryDuration)
        {
            float targetX = goalCenter.x;
            float dirX = targetX - transform.position.x;
            rb.AddForce(new Vector2(dirX * 30f, 0f), ForceMode2D.Force);

            timer += Time.deltaTime;
            yield return null;
        }

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
            visible = !visible;
            spriteRenderer.enabled = visible;

            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        spriteRenderer.enabled = true;
        invincible = false;
        isActive = true;
    }

    public IEnumerator SetInvincible(float time)
    {
        isInvincible = true;
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
    }

    public void SetOffscreenIndicatorActive(bool active)
    {
        if (!isActive && active) return;
        if (offscreenIndicator != null)
            offscreenIndicator.SetActive(active);
    }
}

