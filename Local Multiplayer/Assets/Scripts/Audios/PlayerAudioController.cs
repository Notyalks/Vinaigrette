using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(PlayerMovementBase))]
public class PlayerAudioController : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerMovementBase movementBase;

    [Header("Sons do Player")]
    public AudioClip jumpClip;
    public AudioClip dashClip;
    [Tooltip("Clipes de passos que serão escolhidos aleatoriamente pelo Animation Event")]
    public AudioClip[] footstepClips;
    public AudioClip deathClip;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        movementBase = GetComponent<PlayerMovementBase>();
    }

    private void OnEnable()
    {
        if (movementBase != null)
        {
            movementBase.OnJump += HandleJump;
            movementBase.OnDash += HandleDash;
        }

        if (playerController != null)
        {
            playerController.OnDeath += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (movementBase != null)
        {
            movementBase.OnJump -= HandleJump;
            movementBase.OnDash -= HandleDash;
        }

        if (playerController != null)
        {
            playerController.OnDeath -= HandleDeath;
        }
    }

    private void HandleJump()
    {
        if (jumpClip != null)
            AudioManager.Instance.PlaySFX(jumpClip);
    }

    private void HandleDash()
    {
        if (dashClip != null)
            AudioManager.Instance.PlaySFX(dashClip);
    }

    private void HandleDeath()
    {
        if (deathClip != null)
            AudioManager.Instance.PlaySFX(deathClip);
    }

    //  Chamado por Animation Event
    public void PlayFootstep()
    {
        if (footstepClips != null && footstepClips.Length > 0)
        {
            int index = Random.Range(0, footstepClips.Length);
            var chosenClip = footstepClips[index];
            if (chosenClip != null)
            {
                // leve variação de pitch para ficar natural
                AudioManager.Instance.PlaySFXRandomPitch(chosenClip, 1f, 0.1f);
            }
        }
    }
}
