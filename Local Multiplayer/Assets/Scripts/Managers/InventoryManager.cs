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

    /// <summary>
    /// Prepara os botões com opções aleatórias para o jogador atual
    /// </summary>
    public void SetupInventory(GameObject inventoryUI, Collider2D placementArea)
    {
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
            inventoryButtons[i].onClick.AddListener(() =>
            {
                obstaclePlacer.BeginPlacement(currentOptions[index], placementArea);
                inventoryUI.SetActive(false); // Esconde inventário ao escolher
            });
        }
    }

    /// <summary>
    /// Reseta os botões no fim do turno
    /// </summary>
    public void ClearInventory()
    {
        if (inventoryButtons == null) return;

        foreach (var btn in inventoryButtons)
        {
            btn.onClick.RemoveAllListeners();
            btn.GetComponent<Image>().sprite = null;
            btn.GetComponent<Image>().color = Color.clear;
        }
        currentOptions.Clear();
    }
}
