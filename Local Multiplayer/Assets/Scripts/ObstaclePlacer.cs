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
        if (selectedObstacle.prefab == null) return; // checa prefab

        Instantiate(selectedObstacle.prefab, ghostObstacle.transform.position, ghostObstacle.transform.rotation);

        ClearPlacement();
        turnManager.EndTurn();
    }
    private void RotateGhost(float angle)
    {
        ghostObstacle.transform.Rotate(0f, 0f, angle);
    }

    private bool IsPlacementValid(Vector3 position)
    {
        if (activePlacementArea == null || !activePlacementArea.OverlapPoint(position))
            return false;

        // Checa áreas de start/goal
        if (activePlacementArea == placementAreaPlayer1)
        {
            if (startAreaPlayer1.OverlapPoint(position) || goalAreaPlayer1.OverlapPoint(position)) return false;
        }
        else if (activePlacementArea == placementAreaPlayer2)
        {
            if (startAreaPlayer2.OverlapPoint(position) || goalAreaPlayer2.OverlapPoint(position)) return false;
        }

        if (ghostObstacle.TryGetComponent(out BoxCollider2D ghostCollider))
        {
            Vector2 size = ghostCollider.size;
            Vector2 scale = ghostObstacle.transform.lossyScale;
            Vector2 scaledSize = new(size.x * scale.x, size.y * scale.y);

            // Obstáculo
            if (Physics2D.OverlapBox(position, scaledSize, ghostObstacle.transform.eulerAngles.z, obstacleLayer))
                return false;

            // Terreno (via tag)
            foreach (var hit in Physics2D.OverlapBoxAll(position, scaledSize, ghostObstacle.transform.eulerAngles.z))
                if (hit.CompareTag("Terrain")) return false;
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
            Vector2 scaledSize = new Vector2(size.x * scale.x, size.y * scale.y);

            Gizmos.color = isValidPlacement ? Color.green : Color.red;
            Gizmos.matrix = Matrix4x4.TRS(
                ghostObstacle.transform.position,
                Quaternion.Euler(0, 0, ghostObstacle.transform.eulerAngles.z),
                Vector3.one
            );
            Gizmos.DrawWireCube(Vector3.zero, scaledSize);
        }
    }
}
