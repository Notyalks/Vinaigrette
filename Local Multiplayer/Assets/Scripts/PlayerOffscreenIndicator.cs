using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PlayerOffscreenIndicator : MonoBehaviour
{
    [Header("Referências")]
    public Transform target;                 // Player transform
    public PlayerController targetController;// PlayerController do target
    public Camera cam;                       // Main Camera
    public Canvas rootCanvas;                // Canvas raiz
    public RectTransform indicatorRect;      // Este próprio RectTransform
    public RectTransform canvasRect;         // RectTransform do Canvas

    [Header("Config")]
    public bool isPlayer2 = false;           // Se for o indicador do Player 2
    public bool stickToTopEdgeForP2 = true;  // P2 no topo, P1 embaixo
    public float edgePadding = 24f;          // Margem das bordas

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
        if (target == null || cam == null || rootCanvas == null) return;

        bool show;
        if (isPlayer2)
        {
            // Para o P2, aparece quando ele está "atrás do caminho do P1"
            show = (targetController != null && targetController.IsBehindPlayer1);
        }
        else
        {
            // Para o P1, aparece se sair da tela verticalmente
            var vp = cam.WorldToViewportPoint(target.position);
            show = vp.y < 0f || vp.y > 1f;
        }

        indicatorRect.gameObject.SetActive(show);
        if (!show) return;

        // Pega X do mundo e converte para tela
        Vector3 screen = cam.WorldToScreenPoint(target.position);

        // Clampa X dentro da tela
        float x = Mathf.Clamp(screen.x, edgePadding, Screen.width - edgePadding);

        // Y na borda: P2 no topo (ou embaixo se quiser), P1 embaixo
        float y = isPlayer2 && stickToTopEdgeForP2 ? Screen.height - edgePadding : edgePadding;

        Vector2 screenClamped = new Vector2(x, y);

        // Converte Screen UI (funciona para Overlay e para Screen Space - Camera)
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
