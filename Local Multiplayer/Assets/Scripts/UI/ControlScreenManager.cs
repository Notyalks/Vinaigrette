using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlScreenManager : MonoBehaviour
{
    [Header("Elementos")]
    public GameObject[] controlImages;        // cada imagem de controle
    public string[] controlDescriptions;      // descrição correspondente
    public TextMeshProUGUI descriptionText;   // onde mostrar a descrição

    [Header("Botões de Navegação")]
    public Button prevButton;
    public Button nextButton;

    private int currentIndex = 0;

    private void Start()
    {
        ShowPage(0);

        if (prevButton != null)
            prevButton.onClick.AddListener(ShowPrevious);

        if (nextButton != null)
            nextButton.onClick.AddListener(ShowNext);
    }

    private void ShowPage(int index)
    {
        if (index < 0 || index >= controlImages.Length) return;

        // Desativa todas as imagens
        for (int i = 0; i < controlImages.Length; i++)
            controlImages[i].SetActive(i == index);

        // Atualiza descrição
        if (descriptionText != null && index < controlDescriptions.Length)
            descriptionText.text = controlDescriptions[index];

        currentIndex = index;

        // Atualiza visibilidade dos botões
        if (prevButton != null)
            prevButton.gameObject.SetActive(currentIndex > 0);

        if (nextButton != null)
            nextButton.gameObject.SetActive(currentIndex < controlImages.Length - 1);
    }

    private void ShowPrevious()
    {
        ShowPage(currentIndex - 1);
    }

    private void ShowNext()
    {
        ShowPage(currentIndex + 1);
    }
}
