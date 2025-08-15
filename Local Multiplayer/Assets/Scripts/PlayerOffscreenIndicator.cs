using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PlayerOffscreenIndicator : MonoBehaviour
{
    [Header("Referências")]
    public Transform target;
    public PlayerController targetController;
    public Camera cam;
    public Canvas rootCanvas;
    public RectTransform indicatorRect;
    public RectTransform canvasRect;

    [Header("Config")]
    public bool isPlayer1 = false;         // Se é o indicador do Player 1
    public float edgePadding = 24f;        // Margem para não colar na borda
    public float customYPosition = 200f;   // Para Player 2: posição Y livre

    void Reset()
    {
        indicatorRect = GetComponent<RectTransform>();
    }

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
        if (canvasRect == null) canvasRect = rootCanvas.GetComponent<RectTransform>();
        if (indicatorRect == null) indicatorRect = GetComponent<RectTransform>();
        if (targetController == null && target != null) targetController = target.GetComponent<PlayerController>();

        indicatorRect.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (target == null || cam == null || rootCanvas == null || targetController == null) return;

        if (!targetController.isActive || targetController.lives <= 0)
        {
            if (indicatorRect.gameObject.activeSelf)
                indicatorRect.gameObject.SetActive(false);
            return;
        }

        bool show;
        if (isPlayer1)
        {
            // Player 1: aparece se sair da visão vertical
            var vp = cam.WorldToViewportPoint(target.position);
            show = vp.y < 0f || vp.y > 1f;
        }
        else
        {
            // Player 2: aparece quando está atrás do Player 1
            show = targetController.IsBehindPlayer1;
        }

        indicatorRect.gameObject.SetActive(show);
        if (!show) return;

        // Converte posição do jogador para tela
        Vector3 screen = cam.WorldToScreenPoint(target.position);

        // Mantém X clamped
        float x = Mathf.Clamp(screen.x, edgePadding, Screen.width - edgePadding);

        float y;
        if (isPlayer1)
        {
            // Player 1 no topo
            y = Screen.height - edgePadding;
        }
        else
        {
            // Player 2 com Y ajustável
            y = customYPosition;
        }

        Vector2 screenClamped = new Vector2(x, y);

        // Converte para coordenadas de UI
        Vector2 anchored;
        if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenClamped, null, out anchored);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenClamped, cam, out anchored);
        }

        indicatorRect.anchoredPosition = anchored;
    }
}
