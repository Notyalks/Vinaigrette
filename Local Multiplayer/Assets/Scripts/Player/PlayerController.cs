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
    public GameObject livesUI;       // Container com os corações (cheios)
    public GameObject livesUIBG;     // Container com o fundo dos corações
    private Image[] heartImages;

    [Header("Indicador Fora da Tela")]
    public GameObject offscreenIndicator; // arrastar no Inspector

    public bool IsBehindPlayer1 { get; private set; }

    public int playerId;
    public bool hasFinished = false;
    public bool isActive = true;

    [Header("Invencibilidade")]
    public float respawnInvincibleTime = 2f;
    public float invincibleTime = 2f;
    public bool invincible = false;
    private SpriteRenderer spriteRenderer;

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

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Hazard") && !invincible)
        {
            LoseLife();
        }
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
            SetOffscreenIndicatorActive(false);
            ScoreManager.Instance.CheckRoundEnd();
        }
        else
        {
            Vector3 respawnPos = GameManager.Instance.GetRandomStartPosition(playerId);
            transform.position = respawnPos;
            rb.linearVelocity = Vector2.zero;

            ReactivatePlayer();
            StartCoroutine(RespawnInvincibility());
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

        StartCoroutine(SetTemporaryInvincible(invincibleTime));
    }

    public void Deactivate()
    {
        isActive = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void ResetPlayer()
    {
        lives = maxLives;
        UpdateLivesUI();

        Vector3 respawnPos = GameManager.Instance.GetRandomStartPosition(playerId);
        transform.position = respawnPos;

        rb.linearVelocity = Vector2.zero;
        ReactivatePlayer();
    }

    public void SetLivesUIVisible(bool visible)
    {
        if (livesUI != null)
            livesUI.SetActive(visible);

        if (livesUIBG != null)
            livesUIBG.SetActive(visible);
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

    public IEnumerator SetTemporaryInvincible(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
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
    public void HideForBuildPhase()
    {
        gameObject.SetActive(false);
        isActive = false;
        SetLivesUIVisible(false);
    }

    public void ShowForPlayPhase()
    {
        gameObject.SetActive(true);
        ReactivatePlayer();
        SetLivesUIVisible(true);
    }

}
