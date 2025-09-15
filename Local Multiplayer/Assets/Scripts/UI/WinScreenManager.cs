using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreenManager : MonoBehaviour
{
    [Header("Imagens de Vitória")]
    public GameObject player1WinImage;
    public GameObject player2WinImage;

    [Header("Botão")]
    public Button exitButton;

    private void Start()
    {
        if (player1WinImage != null) player1WinImage.SetActive(false);
        if (player2WinImage != null) player2WinImage.SetActive(false);

        if (exitButton != null)
            exitButton.onClick.AddListener(ReturnToMenu);
    }

    /// <summary>
    /// Chamado via Animation Event (sem parâmetros).
    /// </summary>
    public void ShowWinner()
    {
        int winner = GameManager.Instance.GetLastWinner();

        if (player1WinImage != null) player1WinImage.SetActive(winner == 1);
        if (player2WinImage != null) player2WinImage.SetActive(winner == 2);
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
