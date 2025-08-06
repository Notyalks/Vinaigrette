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
    public Collider2D placementAreaPlayer1; // Caminho do Player 1
    public Collider2D placementAreaPlayer2; // Caminho do Player 2
    private Collider2D activePlacementArea; // Área ativa no turno atual

    void Start()
    {
        inventoryPanel.SetActive(false);
        // Desativa áreas inicialmente
        placementAreaPlayer1.enabled = false;
        placementAreaPlayer2.enabled = false;
    }

    void Update()
    {
        if (isPlacing && ghostObstacle != null)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            ghostObstacle.transform.position = mousePos;

            // Checa se está dentro da área ativa
            isValidPlacement = activePlacementArea != null && activePlacementArea.OverlapPoint(mousePos);

            // Atualiza cor do ghost
            UpdateGhostColor();

            // Clique para confirmar
            if (Input.GetMouseButtonDown(0) && isValidPlacement)
            {
                PlaceObstacle();
            }
        }
    }

    public void StartPlayerTurn()
    {
        // Define área ativa com base em qual jogador está jogando
        int currentPlayer = turnManager.GetCurrentPlayer();
        if (currentPlayer == 1)
        {
            // Jogador 1 coloca obstáculos no caminho do jogador 2
            activePlacementArea = placementAreaPlayer2;
            placementAreaPlayer2.enabled = true;
            placementAreaPlayer1.enabled = false;
        }
        else
        {
            // Jogador 2 coloca obstáculos no caminho do jogador 1
            activePlacementArea = placementAreaPlayer1;
            placementAreaPlayer1.enabled = true;
            placementAreaPlayer2.enabled = false;
        }

        // Gera obstáculos aleatórios
        currentOptions.Clear();
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            int rand = Random.Range(0, allObstacles.Length);
            currentOptions.Add(allObstacles[rand]);

            Image btnImg = inventoryButtons[i].GetComponent<Image>();
            btnImg.sprite = allObstacles[rand].icon;
            btnImg.color = Color.white;
        }

        inventoryPanel.SetActive(true);
    }

    public void EndPlayerTurn()
    {
        inventoryPanel.SetActive(false);
        ClearPlacement();

        // Desativa áreas de colocação quando não é turno
        placementAreaPlayer1.enabled = false;
        placementAreaPlayer2.enabled = false;
        activePlacementArea = null;
    }

    public void SelectObstacle(int index)
    {
        if (index >= 0 && index < currentOptions.Count)
        {
            selectedObstacle = currentOptions[index];
            inventoryPanel.SetActive(false);

            // Cria ghost do obstáculo
            ghostObstacle = Instantiate(selectedObstacle.prefab);
            ghostObstacle.GetComponent<Collider2D>().enabled = false;

            ghostRenderer = ghostObstacle.GetComponent<SpriteRenderer>();
            ghostOriginalColor = ghostRenderer.color;

            // Adiciona apenas transparência
            ghostRenderer.color = new Color(ghostOriginalColor.r, ghostOriginalColor.g, ghostOriginalColor.b, 0.5f);

            isPlacing = true;
        }
    }

    private void UpdateGhostColor()
    {
        if (ghostRenderer != null)
        {
            if (isValidPlacement)
            {
                // Cor original + transparência
                ghostRenderer.color = new Color(ghostOriginalColor.r, ghostOriginalColor.g, ghostOriginalColor.b, 0.5f);
            }
            else
            {
                // Vermelho + transparência
                ghostRenderer.color = new Color(1f, 0f, 0f, 0.5f);
            }
        }
    }

    private void PlaceObstacle()
    {
        if (selectedObstacle.prefab == null) return;

        Instantiate(selectedObstacle.prefab, ghostObstacle.transform.position, Quaternion.identity);

        ClearPlacement();

        turnManager.NextTurn();
    }

    private void ClearPlacement()
    {
        if (ghostObstacle != null) Destroy(ghostObstacle);
        selectedObstacle.prefab = null;
        isPlacing = false;
    }
}
