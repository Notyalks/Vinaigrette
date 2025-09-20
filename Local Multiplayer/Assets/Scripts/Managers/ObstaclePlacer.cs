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

    public event System.Action OnObstaclePlaced;
    public event System.Action OnObstacleRotated;

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

        // Desativa TODOS os colliders do ghost
        foreach (var col in ghostObstacle.GetComponents<Collider2D>())
            col.enabled = false;

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
                turnManager.EndTurn(bomb.destroyDelay);
            };
            bomb.Place();
        }
        else
        {
            turnManager.EndTurn();
        }

        OnObstaclePlaced?.Invoke();

        ClearPlacement();
    }

    private void RotateGhost(float angle)
    {
        ghostObstacle.transform.Rotate(0f, 0f, angle);

        OnObstacleRotated?.Invoke();
    }

    private bool IsPlacementValid(Vector3 pivotPosition)
    {
        if (activePlacementArea == null) return false;

        // Vamos checar TODOS os colliders do ghost
        Collider2D[] colliders = ghostObstacle.GetComponents<Collider2D>();
        if (colliders.Length == 0) return false;

        foreach (var col in colliders)
        {
            Vector2 checkCenter = ghostObstacle.transform.position;

            if (col is BoxCollider2D box)
            {
                Vector2 size = Vector2.Scale(box.size, ghostObstacle.transform.lossyScale);
                Vector2 center = ghostObstacle.transform.TransformPoint(box.offset);

                if (!activePlacementArea.OverlapPoint(center)) return false;
                if (Physics2D.OverlapBox(center, size, ghostObstacle.transform.eulerAngles.z, obstacleLayer)) return false;

                Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, ghostObstacle.transform.eulerAngles.z);
                foreach (var hit in hits)
                {
                    if (hit != null && hit.CompareTag("Terrain")) return false;
                }
            }
            else if (col is CircleCollider2D circle)
            {
                Vector2 center = ghostObstacle.transform.TransformPoint(circle.offset);
                float radius = circle.radius * Mathf.Max(ghostObstacle.transform.lossyScale.x, ghostObstacle.transform.lossyScale.y);

                if (!activePlacementArea.OverlapPoint(center)) return false;
                if (Physics2D.OverlapCircle(center, radius, obstacleLayer)) return false;
            }
            else if (col is PolygonCollider2D poly)
            {
                // 1) Colisão com obstáculos
                ContactFilter2D filter = new ContactFilter2D();
                filter.SetLayerMask(obstacleLayer);
                filter.useTriggers = false;

                Collider2D[] results = new Collider2D[5];
                int hits = Physics2D.OverlapCollider(poly, filter, results);
                if (hits > 0) return false;

                // 2) Verificar Start/Goal/Terrain usando todos os pontos do polígono
                Vector2[] localPoints = poly.points;
                foreach (var p in localPoints)
                {
                    Vector2 worldPoint = ghostObstacle.transform.TransformPoint(p + poly.offset);

                    // dentro da área de colocação?
                    if (!activePlacementArea.OverlapPoint(worldPoint)) return false;

                    // bloqueio por terreno
                    if (Physics2D.OverlapPoint(worldPoint, obstacleLayer) is Collider2D terrainHit && terrainHit.CompareTag("Terrain"))
                        return false;
                }
            }
        }

        // Checa áreas de start/goal usando o pivot (posição principal do objeto)
        if (activePlacementArea == placementAreaPlayer1)
        {
            if (startAreaPlayer1.OverlapPoint(pivotPosition) || goalAreaPlayer1.OverlapPoint(pivotPosition)) return false;
        }
        else if (activePlacementArea == placementAreaPlayer2)
        {
            if (startAreaPlayer2.OverlapPoint(pivotPosition) || goalAreaPlayer2.OverlapPoint(pivotPosition)) return false;
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
        if (ghostObstacle == null) return;

        Gizmos.color = isValidPlacement ? Color.green : Color.red;

        foreach (var col in ghostObstacle.GetComponents<Collider2D>())
        {
            if (col is BoxCollider2D box)
            {
                Vector2 size = Vector2.Scale(box.size, ghostObstacle.transform.lossyScale);
                Vector3 centerWorld = ghostObstacle.transform.TransformPoint(box.offset);

                Gizmos.matrix = Matrix4x4.TRS(centerWorld, ghostObstacle.transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, size);
            }
            else if (col is CircleCollider2D circle)
            {
                Vector3 centerWorld = ghostObstacle.transform.TransformPoint(circle.offset);
                float radius = circle.radius * Mathf.Max(ghostObstacle.transform.lossyScale.x, ghostObstacle.transform.lossyScale.y);

                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawWireSphere(centerWorld, radius);
            }
            else if (col is PolygonCollider2D poly)
            {
                Vector3[] worldPoints = new Vector3[poly.points.Length];
                for (int i = 0; i < poly.points.Length; i++)
                {
                    Vector2 local = poly.points[i] + poly.offset;
                    worldPoints[i] = ghostObstacle.transform.TransformPoint(local);
                }

                Gizmos.matrix = Matrix4x4.identity;
                for (int i = 0; i < worldPoints.Length; i++)
                {
                    Vector3 current = worldPoints[i];
                    Vector3 next = worldPoints[(i + 1) % worldPoints.Length];
                    Gizmos.DrawLine(current, next);
                }
            }
        }
    }
}
