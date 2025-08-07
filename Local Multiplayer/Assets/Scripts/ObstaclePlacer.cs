using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    [Header("Detecção de Sobreposição")]
    public LayerMask obstacleLayer; // Defina uma Layer para seus obstáculos

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

            // Snap para o grid
            mousePos.x = Mathf.Round(mousePos.x / gridSize) * gridSize;
            mousePos.y = Mathf.Round(mousePos.y / gridSize) * gridSize;

            ghostObstacle.transform.position = mousePos;

            // Rotação 90° por tecla R
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateGhost(90f);
            }

            // Rotação 90° por scroll do mouse (opcional)
            float scroll = Input.mouseScrollDelta.y;
            if (scroll > 0f)
            {
                RotateGhost(90f);
            }
            else if (scroll < 0f)
            {
                RotateGhost(-90f);
            }

            // Checa se posição é válida
            isValidPlacement = IsPlacementValid(mousePos);

            // Atualiza cor do ghost
            UpdateGhostColor();

            // Clique para confirmar
            if (Input.GetMouseButtonDown(0) && isValidPlacement)
            {
                PlaceObstacle();
            }
        }
    }

    public void StartPlayerTurn(int player, GameObject inventoryUI)
    {
        int currentPlayer = player;

        // Define área de colocação
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

        // Pega botões do inventário atual
        inventoryButtons = inventoryUI.GetComponentsInChildren<Button>();

        // Gera obstáculos aleatórios
        currentOptions.Clear();
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            int rand = Random.Range(0, allObstacles.Length);
            currentOptions.Add(allObstacles[rand]);

            Image btnImg = inventoryButtons[i].GetComponent<Image>();
            btnImg.sprite = allObstacles[rand].icon;
            btnImg.color = Color.white;

            int index = i; // evita bug de closure
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
    }

    public void SelectObstacle(int index)
    {
        if (index >= 0 && index < currentOptions.Count)
        {
            selectedObstacle = currentOptions[index];

            // Fecha inventário do jogador da vez
            inventoryButtons[0].transform.parent.gameObject.SetActive(false);

            // Cria ghost do obstáculo fora da tela
            ghostObstacle = Instantiate(selectedObstacle.prefab);
            Collider2D col = ghostObstacle.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            ghostRenderer = ghostObstacle.GetComponent<SpriteRenderer>();
            ghostOriginalColor = ghostRenderer.color;
            ghostRenderer.color = new Color(ghostOriginalColor.r, ghostOriginalColor.g, ghostOriginalColor.b, 0.5f);

            ghostRenderer.sortingLayerName = "UI";
            ghostRenderer.sortingOrder = 10;

            // Começa fora da tela para não aparecer atrás do botão
            ghostObstacle.transform.position = new Vector3(9999, 9999, 0);

            isPlacing = true;
        }
    }

    private bool IsPlacementValid(Vector3 position)
    {
        // 1. Checa se está dentro da área ativa
        if (activePlacementArea == null || !activePlacementArea.OverlapPoint(position))
            return false;

        // 2. Checa sobreposição com outros obstáculos
        if (ghostObstacle.TryGetComponent(out BoxCollider2D ghostCollider))
        {
            Vector2 size = ghostCollider.size;
            Vector2 scale = ghostObstacle.transform.lossyScale;
            Vector2 scaledSize = new Vector2(size.x * scale.x, size.y * scale.y);

            Collider2D hit = Physics2D.OverlapBox(
                position,       // posição do ghost
                scaledSize,     // tamanho ajustado pela escala
                ghostObstacle.transform.eulerAngles.z, // rotação em Z
                obstacleLayer
            );

            if (hit != null) return false;
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
