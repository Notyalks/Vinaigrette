using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum OptionsOrigin
{
    MainMenu,
    PauseMenu
}

public class OptionsMenu : MonoBehaviour
{
    [Header("UI Referências")]
    public Slider musicSlider;
    public Slider vfxSlider;
    public TextMeshProUGUI resolutionText;
    public GameObject mainMenuPanel;  // usado só no menu principal
    public GameObject pauseMenuPanel; // usado só no gameplay

    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;
    private OptionsOrigin origin;

    private void Start()
    {
        // --- Carrega resoluções ---
        resolutions = Screen.resolutions;

        // Pega resolução atual
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        ApplyResolution(); // já aplica inicial
        UpdateResolutionText();

        // --- Carrega PlayerPrefs ---
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        vfxSlider.value = PlayerPrefs.GetFloat("VFXVolume", 1f);

        // Adiciona listeners
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        vfxSlider.onValueChanged.AddListener(SetVFXVolume);
    }

    // ================================
    // RESOLUÇÃO
    // ================================
    public void NextResolution()
    {
        currentResolutionIndex = (currentResolutionIndex + 1) % resolutions.Length;
        ApplyResolution();
    }

    public void PreviousResolution()
    {
        currentResolutionIndex = (currentResolutionIndex - 1 + resolutions.Length) % resolutions.Length;
        ApplyResolution();
    }

    private void UpdateResolutionText()
    {
        Resolution res = resolutions[currentResolutionIndex];
        resolutionText.text = res.width + " x " + res.height;
    }

    private void ApplyResolution()
    {
        Resolution res = resolutions[currentResolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        UpdateResolutionText();
    }

    // ================================
    // ÁUDIO
    // ================================
    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        // futuramente: conectar no AudioMixer
    }

    public void SetVFXVolume(float value)
    {
        PlayerPrefs.SetFloat("VFXVolume", value);
        PlayerPrefs.Save();
        // futuramente: conectar no AudioMixer
    }

    // ================================
    // ORIGEM E BOTÃO VOLTAR
    // ================================
    public void Open(OptionsOrigin from)
    {
        origin = from;
        gameObject.SetActive(true);
    }

    public void OnBackPressed()
    {
        gameObject.SetActive(false);

        if (origin == OptionsOrigin.MainMenu && mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
        else if (origin == OptionsOrigin.PauseMenu && pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    }
}
