using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    [Header("Efeito de Destruição")]
    public GameObject destroyEffectPrefab;

    [Header("Som de Destruição")]
    public AudioClip destroySFX;

    public void PlayEffect()
    {
        if (destroyEffectPrefab != null && Application.isPlaying)
        {
            GameObject effect = Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // tempo de vida do efeito
        }

        //  toca som se houver um clipe configurado
        if (destroySFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(destroySFX);
        }
    }
}
