using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    [Header("Efeito de Destruição")]
    public GameObject destroyEffectPrefab;

    public void PlayEffect()
    {
        if (destroyEffectPrefab != null && Application.isPlaying)
        {
            GameObject effect = Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // tempo de vida do efeito
        }
    }
}
