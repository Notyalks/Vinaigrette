using UnityEngine;

public class DestroySFX : MonoBehaviour
{
    [Header("Som ao destruir")]
    public AudioClip destroySfx;

    [Tooltip("Se verdadeiro, o objeto será destruído junto com o efeito.")]
    public bool destroyObject = true;

    // Chame essa função quando o obstáculo for destruído
    public void PlayDestroySfx()
    {
        if (destroySfx != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(destroySfx);

        if (destroyObject)
            Destroy(gameObject);
    }
}
