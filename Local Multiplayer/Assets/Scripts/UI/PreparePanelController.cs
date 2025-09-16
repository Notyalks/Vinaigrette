using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class PreparePanelController : MonoBehaviour
{
    [Header("Configuração de Fade")]
    public float fadeDuration = 1f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Ativa e reseta o painel.
    /// Chamado pelo UIManager no início do prepare.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Chamado via Animation Event no fim da animação do Prepare.
    /// </summary>
    public void OnPrepareAnimationEnd()
    {
        StartCoroutine(FadeOutAndStartGame());
    }

    private IEnumerator FadeOutAndStartGame()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        gameObject.SetActive(false);
        GameManager.Instance.StartPlayPhase();
    }
}
