using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("UI Painéis")]
    public GameObject pauseMenuUI;
    public OptionsMenu optionsMenu; // referência ao OptionsMenu

    private void Update()
    {
        // Tecla ESC abre/fecha o pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    // ================================
    // CONTROLE DE PAUSA
    // ================================
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    // ================================
    // BOTÕES
    // ================================
    public void OnResumePressed()
    {
        Resume();
    }

    public void OnOptionsPressed()
    {
        optionsMenu.Open(OptionsOrigin.PauseMenu);
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f; // reset para não voltar pausado
        SceneManager.LoadScene("Menu"); // nome da cena do menu principal
    }
}
