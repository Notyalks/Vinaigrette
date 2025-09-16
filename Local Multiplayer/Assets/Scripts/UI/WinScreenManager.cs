using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreenManager : MonoBehaviour
{
    [Header("Referências")]
    public GameObject player1WinImage;
    public GameObject player2WinImage;

    [Header("Botões")]
    public Button exitButton;
    public Button restartButton;

    private void Start()
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(ReturnToMenu);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        HideAll();
    }

    private void HideAll()
    {
        if (player1WinImage != null) player1WinImage.SetActive(false);
        if (player2WinImage != null) player2WinImage.SetActive(false);
    }

    /// <summary>
    /// Chamado via Animation Event.
    /// </summary>
    public void ShowWinner()
    {
        HideAll();

        int winner = GameManager.Instance.GetLastWinner();
        if (winner == 1 && player1WinImage != null)
            player1WinImage.SetActive(true);
        else if (winner == 2 && player2WinImage != null)
            player2WinImage.SetActive(true);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void RestartGame()
    {
        // Recarrega a cena atual
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
}
