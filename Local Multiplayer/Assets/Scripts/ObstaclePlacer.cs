using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ObstaclePlacer : MonoBehaviour
{
    [Header("Referências")]
    public Camera cam;
    public TurnManager turnManager;
    public GameObject inventoryPanel;
    public Button[] inventoryButtons;

    [Header("Obstáculos")]
    public ObstacleOption[] allObstacles;
    private ObstacleOption selectedObstacle;
    private GameObject ghostObstacle;
    private SpriteRenderer ghostRenderer;
    private Color ghostOriginalColor;
    private List<ObstacleOption> currentOptions = new List<ObstacleOption>();

    private bool isPlacing = false;
    private bool isValidPlacement = false;

    [Header("Áreas de Colocação")]
    public Collider2D placementAreaPlayer1;
    public Collider2D placementAreaPlayer2;
    private Collider2D activePlacementArea;

    [Header("Áreas de Início / Goal")]
    public Collider2D startAreaPlayer1;
    public Collider2D goalAreaPlayer1;
    public Collider2D startAreaPlayer2;
    public Collider2D goalAreaPlayer2;

    [Header("Indicadores Visuais")]
    public GameObject startIndicatorP1;
    public GameObject goalIndicatorP1;
    public GameObject startIndicatorP2;
    public GameObject goalIndicatorP2;

    [Header("Textos de Indicador")]
    public TMP_Text startTextP1;
    public TMP_Text goalTextP1;
    public TMP_Text startTextP2;
    public TMP_Text goalTextP2;

    [Header("Detecção de Sobreposição")]
    public LayerMask obstacleLayer;
    public LayerMask terrainLayer;

    public float gridSize = 1f;

    void Start()
    {
        inventoryPanel.SetActive(false);
        placementAreaPlayer1.enabled = false;
        placementAreaPlayer2.enabled = false;
    }

    void Update()
    {
        if (isPlacing && ghostObstacle != null)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            mousePos.x = Mathf.Round(mousePos.x / gridSize) * gridSize;
            mousePos.y = Mathf.Round(mousePos.y / gridSize) * gridSize;

            ghostObstacle.transform.position = mousePos;

            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateGhost(90f);
            }

            float scroll = Input.mouseScrollDelta.y;
            if (scroll > 0f)
            {
                RotateGhost(90f);
            }
            else if (scroll < 0f)
            {
                RotateGhost(-90f);
            }

            isValidPlacement = IsPlacementValid(mousePos);

            UpdateGhostColor();

            if (Input.GetMouseButtonDown(0) && isValidPlacement)
            {
                PlaceObstacle();
            }
        }
    }

    public void StartPlayerTurn(int player, GameObject inventoryUI)
    {
        int currentPlayer = player;

        if (currentPlayer == 1)
        {
            activePlacementArea = placementAreaPlayer2;
            placementAreaPlayer2.enabled = true;
            placementAreaPlayer1.enabled = false;
        }
        else
        {
            activePlacementArea = placementAreaPlayer1;
            placementAreaPlayer1.enabled = true;
            placementAreaPlayer2.enabled = false;
        }

        if (player == 1)
        {
            startIndicatorP2.SetActive(true);
            goalIndicatorP2.SetActive(true);
            startTextP2.gameObject.SetActive(true);
            goalTextP2.gameObject.SetActive(true);

            startIndicatorP1.SetActive(false);
            goalIndicatorP1.SetActive(false);
            startTextP1.gameObject.SetActive(false);
            goalTextP1.gameObject.SetActive(false);
        }
        else
        {
            startIndicatorP1.SetActive(true);
            goalIndicatorP1.SetActive(true);
            startTextP1.gameObject.SetActive(true);
            goalTextP1.gameObject.SetActive(true);

            startIndicatorP2.SetActive(false);
            goalIndicatorP2.SetActive(false);
            startTextP2.gameObject.SetActive(false);
            goalTextP2.gameObject.SetActive(false);
        }

        inventoryButtons = inventoryUI.GetComponentsInChildren<Button>();

        currentOptions.Clear();
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            int rand = Random.Range(0, allObstacles.Length);
            currentOptions.Add(allObstacles[rand]);

            Image btnImg = inventoryButtons[i].GetComponent<Image>();
            btnImg.sprite = allObstacles[rand].icon;
            btnImg.color = Color.white;

            int index = i;
            inventoryButtons[i].onClick.RemoveAllListeners();
            inventoryButtons[i].onClick.AddListener(() => SelectObstacle(index));
        }
    }

    public void EndPlayerTurn()
    {
        ClearPlacement();
        placementAreaPlayer1.enabled = false;
        placementAreaPlayer2.enabled = false;
        activePlacementArea = null;

        startIndicatorP1.SetActive(false);
        goalIndicatorP1.SetActive(false);
        startIndicatorP2.SetActive(false);
        goalIndicatorP2.SetActive(false);

        startTextP1.gameObject.SetActive(false);
        goalTextP1.gameObject.SetActive(false);
        startTextP2.gameObject.SetActive(false);
        goalTextP2.gameObject.SetActive(false);
    }

    public void SelectObstacle(int index)
    {
        if (index >= 0 && index < currentOptions.Count)
        {
            selectedObstacle = currentOptions[index];

            inventoryButtons[0].transform.parent.gameObject.SetActive(false);

            ghostObstacle = Instantiate(selectedObstacle.prefab);
            Collider2D col = ghostObstacle.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            ghostRenderer = ghostObstacle.GetComponent<SpriteRenderer>();
            ghostOriginalColor = ghostRenderer.color;
            ghostRenderer.color = new Color(ghostOriginalColor.r, ghostOriginalColor.g, ghostOriginalColor.b, 0.5f);

            ghostRenderer.sortingLayerName = "UI";
            ghostRenderer.sortingOrder = 10;

            ghostObstacle.transform.position = new Vector3(9999, 9999, 0);

            isPlacing = true;
        }
    }

    private bool IsPlacementValid(Vector3 position)
    {
        if (activePlacementArea == null || !activePlacementArea.OverlapPoint(position))
            return false;

        bool insideStartOrGoal = false;
        if (activePlacementArea == placementAreaPlayer1)
        {
            if (startAreaPlayer1.OverlapPoint(position)) insideStartOrGoal = true;
            if (goalAreaPlayer1.OverlapPoint(position)) insideStartOrGoal = true;
        }
        else if (activePlacementArea == placementAreaPlayer2)
        {
            if (startAreaPlayer2.OverlapPoint(position)) insideStartOrGoal = true;
            if (goalAreaPlayer2.OverlapPoint(position)) insideStartOrGoal = true;
        }

        if (insideStartOrGoal) return false;

        if (ghostObstacle.TryGetComponent(out BoxCollider2D ghostCollider))
        {
            Vector2 size = ghostCollider.size;
            Vector2 scale = ghostObstacle.transform.lossyScale;
            Vector2 scaledSize = new Vector2(size.x * scale.x, size.y * scale.y);

            // Checa se bate em outro obstáculo
            Collider2D hitObstacle = Physics2D.OverlapBox(
                position,
                scaledSize,
                ghostObstacle.transform.eulerAngles.z,
                obstacleLayer
            );
            if (hitObstacle != null) return false;

            // Checa se bate no terreno
            Collider2D hitTerrain = Physics2D.OverlapBox(
                position,
                scaledSize,
                ghostObstacle.transform.eulerAngles.z,
                terrainLayer
            );
            if (hitTerrain != null) return false;
        }

        return true;
    }

    private void RotateGhost(float angle)
    {
        ghostObstacle.transform.Rotate(0f, 0f, angle);
    }

    private void UpdateGhostColor()
    {
        if (ghostRenderer != null)
        {
            if (isValidPlacement)
            {
                ghostRenderer.color = new Color(ghostOriginalColor.r, ghostOriginalColor.g, ghostOriginalColor.b, 0.5f);
            }
            else
            {
                ghostRenderer.color = new Color(1f, 0f, 0f, 0.5f);
            }
        }
    }

    private void PlaceObstacle()
    {
        if (selectedObstacle.prefab == null) return;

        Instantiate(selectedObstacle.prefab, ghostObstacle.transform.position, ghostObstacle.transform.rotation);

        ClearPlacement();

        // Fim do turno
        turnManager.EndTurn();
    }

    private void ClearPlacement()
    {
        if (ghostObstacle != null) Destroy(ghostObstacle);
        selectedObstacle.prefab = null;
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
