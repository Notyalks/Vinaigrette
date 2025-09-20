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

    public void SetupInventory(GameObject inventoryUI, Collider2D placementArea, int playerId)
    {
        inventoryButtons = inventoryUI.GetComponentsInChildren<Button>();

        currentOptions.Clear();

        // pega rodada global do GameManager
        int currentRound = GameManager.Instance.roundNumber;

        // --- filtra obstáculos disponíveis ---
        List<ObstacleOption> filtered = new();
        foreach (var opt in allObstacles)
        {
            if ((playerId == 1 && opt.availableForPlayer1) ||
                (playerId == 2 && opt.availableForPlayer2))
            {
                filtered.Add(opt);
            }
        }

        if (filtered.Count == 0) filtered.AddRange(allObstacles);

        // --- gera opções ---
        for (int i = 0; i < inventoryButtons.Length; i++)
        {
            int rand = Random.Range(0, filtered.Count);
            currentOptions.Add(filtered[rand]);

            Image btnImg = inventoryButtons[i].GetComponent<Image>();
            btnImg.sprite = filtered[rand].icon;
            btnImg.preserveAspect = true;
            btnImg.color = Color.white;

            int index = i;
            inventoryButtons[i].onClick.RemoveAllListeners();
            inventoryButtons[i].onClick.AddListener(() =>
            {
                obstaclePlacer.BeginPlacement(currentOptions[index], placementArea);
                inventoryUI.SetActive(false);
            });
        }

        // --- só força bomba a partir da 2ª rodada global ---
        if (currentRound > 1 && forceBombNext)
        {
            EnsureBombInOptions(placementArea, inventoryUI, playerId);
            forceBombNext = false;
        }
        else if (currentRound == 1)
        {
            // remove bombas se caírem na 1ª rodada
            for (int i = 0; i < currentOptions.Count; i++)
            {
                if (currentOptions[i].isBomb)
                {
                    ObstacleOption replacement = filtered.Find(o => !o.isBomb);
                    if (replacement != null)
                    {
                        currentOptions[i] = replacement;

                        Image btnImg = inventoryButtons[i].GetComponent<Image>();
                        btnImg.sprite = replacement.icon;
                        btnImg.preserveAspect = true;
                        btnImg.color = Color.white;

                        int index = i;
                        inventoryButtons[i].onClick.RemoveAllListeners();
                        inventoryButtons[i].onClick.AddListener(() =>
                        {
                            obstaclePlacer.BeginPlacement(currentOptions[index], placementArea);
                            inventoryUI.SetActive(false);
                        });
                    }
                }
            }
        }
    }


    private void EnsureBombInOptions(Collider2D placementArea, GameObject inventoryUI, int playerId)
    {
        bool hasBomb = currentOptions.Exists(opt => opt.isBomb);

        if (!hasBomb)
        {
            int randomIndex = Random.Range(0, currentOptions.Count);

            ObstacleOption bomb = System.Array.Find(allObstacles, o =>
                o.isBomb &&
                ((playerId == 1 && o.availableForPlayer1) ||
                 (playerId == 2 && o.availableForPlayer2))
            );

            if (bomb != null && bomb.prefab != null)
            {
                currentOptions[randomIndex] = bomb;

                Image btnImg = inventoryButtons[randomIndex].GetComponent<Image>();
                btnImg.sprite = bomb.icon;
                btnImg.preserveAspect = true;
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
