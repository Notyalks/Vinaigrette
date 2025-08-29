using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    [Header("Efeito de Destruição")]
    public GameObject destroyEffectPrefab;

    private void OnDestroy()
    {
        if (destroyEffectPrefab != null && Application.isPlaying)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
