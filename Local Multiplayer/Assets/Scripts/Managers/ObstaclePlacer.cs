using UnityEngine;

public class ObstaclePlacer : MonoBehaviour
{
    [Header("Referências")]
    public Camera cam;
    public TurnManager turnManager;
    public float gridSize = 1f;
    public LayerMask obstacleLayer;

    [Header("Áreas de Colocação")]
    public Collider2D placementAreaPlayer1;
    public Collider2D placementAreaPlayer2;
    public Collider2D startAreaPlayer1;
    public Collider2D goalAreaPlayer1;
    public Collider2D startAreaPlayer2;
    public Collider2D goalAreaPlayer2;

    private Collider2D activePlacementArea;

    private ObstacleOption selectedObstacle;
    private GameObject ghostObstacle;
    private SpriteRenderer ghostRenderer;
    private Color ghostOriginalColor;
    private bool isPlacing = false;
    private bool isValidPlacement = false;

    void Update()
    {
        if (PauseMenu.GameIsPaused) return; // bloqueia colocação de obstáculos

        if (!isPlacing || ghostObstacle == null) return;

        // Seguir mouse no grid
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        mousePos.x = Mathf.Round(mousePos.x / gridSize) * gridSize;
        mousePos.y = Mathf.Round(mousePos.y / gridSize) * gridSize;
        ghostObstacle.transform.position = mousePos;

        // Rotação
        if (Input.GetKeyDown(KeyCode.R)) RotateGhost(90f);
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0) RotateGhost(scroll > 0 ? 90f : -90f);

        // Validar posição
        isValidPlacement = IsPlacementValid(mousePos);
        UpdateGhostColor();

        // Colocar obstáculo
        if (Input.GetMouseButtonDown(0) && isValidPlacement)
            PlaceObstacle();
    }


    public void BeginPlacement(ObstacleOption option, Collider2D placementArea)
    {
        ClearPlacement();

        selectedObstacle = option;
        activePlacementArea = placementArea;

        ghostObstacle = Instantiate(option.prefab);
        if (ghostObstacle.TryGetComponent(out Collider2D col)) col.enabled = false;

        ghostRenderer = ghostObstacle.GetComponent<SpriteRenderer>();
        ghostOriginalColor = ghostRenderer.color;
        ghostRenderer.color = new Color(ghostOriginalColor.r, ghostOriginalColor.g, ghostOriginalColor.b, 0.5f);
        ghostRenderer.sortingLayerName = "UI";
        ghostRenderer.sortingOrder = 10;

        ghostObstacle.transform.position = new Vector3(9999, 9999, 0);
        isPlacing = true;
    }

    public void EndPlacement()
    {
        ClearPlacement();
        activePlacementArea = null;
    }

    private void PlaceObstacle()
    {
        if (selectedObstacle.prefab == null) return;

        GameObject placed = Instantiate(selectedObstacle.prefab, ghostObstacle.transform.position, ghostObstacle.transform.rotation);

        if (placed.TryGetComponent<GarlicBomb>(out var bomb))
        {
            bomb.OnBombResolved += () =>
            {
                turnManager.EndTurn(bomb.destroyDelay); // espera antes de encerrar
            };
            bomb.Place();
        }
        else
        {
            turnManager.EndTurn();
        }

        ClearPlacement();
    }

    private void RotateGhost(float angle)
    {
        ghostObstacle.transform.Rotate(0f, 0f, angle);
    }

    // agora recebe a posição do pivot (transform.position) mas usa o centro real do colisor
    private bool IsPlacementValid(Vector3 pivotPosition)
    {
        if (activePlacementArea == null) return false;

        // Determinar o centro em mundo do colisor (se houver)
        Vector2 checkCenter = new Vector2(pivotPosition.x, pivotPosition.y);
        float angle = 0f;
        Vector2 scaledSize = Vector2.zero;
        BoxCollider2D ghostBox = null;

        if (ghostObstacle.TryGetComponent(out BoxCollider2D box))
        {
            ghostBox = box;
            // centro do colisor em mundo (leva em conta offset, rotação e escala)
            Vector3 worldCenter3 = ghostObstacle.transform.TransformPoint(ghostBox.offset);
            checkCenter = new Vector2(worldCenter3.x, worldCenter3.y);

            // tamanho do colisor em unidades já com escala aplicada
            Vector2 size = ghostBox.size;
            Vector2 scale = ghostObstacle.transform.lossyScale;
            scaledSize = new Vector2(size.x * Mathf.Abs(scale.x), size.y * Mathf.Abs(scale.y));

            angle = ghostObstacle.transform.eulerAngles.z;
        }

        // Verifica se o centro do colisor está dentro da área de colocação
        if (!activePlacementArea.OverlapPoint(checkCenter))
            return false;

        // Checa áreas de start/goal (usando o centro do colisor)
        if (activePlacementArea == placementAreaPlayer1)
        {
            if (startAreaPlayer1.OverlapPoint(checkCenter) || goalAreaPlayer1.OverlapPoint(checkCenter)) return false;
        }
        else if (activePlacementArea == placementAreaPlayer2)
        {
            if (startAreaPlayer2.OverlapPoint(checkCenter) || goalAreaPlayer2.OverlapPoint(checkCenter)) return false;
        }

        // Se tivermos um BoxCollider2D, usamos OverlapBox com o centro correto
        if (ghostBox != null)
        {
            // checa colisão com outros obstáculos (usando obstacleLayer)
            if (Physics2D.OverlapBox(checkCenter, scaledSize, angle, obstacleLayer))
                return false;

            // checa colisão com terreno via tag (sem filtro de layer aqui — alternativamente você pode ter uma layer para terrain)
            Collider2D[] hits = Physics2D.OverlapBoxAll(checkCenter, scaledSize, angle);
            foreach (var hit in hits)
            {
                if (hit == null) continue;
                if (hit.CompareTag("Terrain")) return false;
            }
        }

        return true;
    }

    private void UpdateGhostColor()
    {
        if (ghostRenderer != null)
        {
            ghostRenderer.color = isValidPlacement ?
                new Color(ghostOriginalColor.r, ghostOriginalColor.g, ghostOriginalColor.b, 0.5f) :
                new Color(1f, 0f, 0f, 0.5f);
        }
    }

    private void ClearPlacement()
    {
        if (ghostObstacle != null) Destroy(ghostObstacle);
        ghostObstacle = null;
        ghostRenderer = null;

        selectedObstacle = default;
        isPlacing = false;
    }

    void OnDrawGizmosSelected()
    {
        if (ghostObstacle != null && ghostObstacle.TryGetComponent(out BoxCollider2D ghostCollider))
        {
            Vector2 size = ghostCollider.size;
            Vector2 scale = ghostObstacle.transform.lossyScale;
            Vector2 scaledSize = new Vector2(size.x * Mathf.Abs(scale.x), size.y * Mathf.Abs(scale.y));

            // Centro real do colisor em mundo
            Vector3 centerWorld = ghostObstacle.transform.TransformPoint(ghostCollider.offset);

            Gizmos.color = isValidPlacement ? Color.green : Color.red;
            // usamos o centro real e a rotação do transform
            Gizmos.matrix = Matrix4x4.TRS(centerWorld, ghostObstacle.transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, scaledSize);
        }
    }
}
