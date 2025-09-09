using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Configuração")]
    public ObstacleOption[] allObstacles;
    public ObstaclePlacer obstaclePlacer;

    private Button[] inventoryButtons;
    private List<ObstacleOption> currentOptions = new();

    private int roundCounter = 0; // conta rodadas
    private bool forceBombNext = false;

    public void SetupInventory(GameObject inventoryUI, Collider2D placementArea)
    {
        inventoryButtons = inventoryUI.GetComponentsInChildren<Button>();

        currentOptions.Clear();
        roundCounter++;

        // --- gera opções ---
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            int rand = Random.Range(0, allObstacles.Length);
            currentOptions.Add(allObstacles[rand]);

            Image btnImg = inventoryButtons[i].GetComponent<Image>();
            btnImg.sprite = allObstacles[rand].icon;
            btnImg.preserveAspect = true; // mantém a proporção original
            btnImg.color = Color.white;

            int index = i;
            inventoryButtons[i].onClick.RemoveAllListeners();
            inventoryButtons[i].onClick.AddListener(() =>
            {
                obstaclePlacer.BeginPlacement(currentOptions[index], placementArea);
                inventoryUI.SetActive(false);
            });
        }

        // --- a cada 2 rodadas força bomba ---
        if (forceBombNext)
        {
            EnsureBombInOptions(placementArea, inventoryUI);
            forceBombNext = false;
        }
    }

    private void EnsureBombInOptions(Collider2D placementArea, GameObject inventoryUI)
    {
        bool hasBomb = currentOptions.Exists(opt => opt.isBomb);

        if (!hasBomb)
        {
            int randomIndex = Random.Range(0, currentOptions.Count);
            ObstacleOption bomb = System.Array.Find(allObstacles, o => o.isBomb);

            if (bomb.prefab != null)
            {
                currentOptions[randomIndex] = bomb;

                // Atualiza botão
                Image btnImg = inventoryButtons[randomIndex].GetComponent<Image>();
                btnImg.sprite = bomb.icon;
                btnImg.preserveAspect = true; // mantém a proporção também na bomba
                btnImg.color = Color.white;

                int index = randomIndex;
                inventoryButtons[randomIndex].onClick.RemoveAllListeners();
                inventoryButtons[randomIndex].onClick.AddListener(() =>
                {
                    obstaclePlacer.BeginPlacement(currentOptions[index], placementArea);
                    inventoryUI.SetActive(false);
                });
            }
        }
    }

    public void ForceBombNextSetup()
    {
        forceBombNext = true;
    }

    public void ClearInventory()
    {
        if (inventoryButtons == null) return;

        foreach (var btn in inventoryButtons)
        {
            btn.onClick.RemoveAllListeners();
            var img = btn.GetComponent<Image>();
            img.sprite = null;
            img.color = Color.clear;
            img.preserveAspect = true; // garante que continue correto no próximo setup
        }
        currentOptions.Clear();
    }
}
