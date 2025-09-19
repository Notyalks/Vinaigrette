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

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        ApplyResolution();
        UpdateResolutionText();

        // --- Carrega PlayerPrefs e aplica volumes ---
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedVFX = PlayerPrefs.GetFloat("VFXVolume", 1f);

        musicSlider.value = savedMusic;
        vfxSlider.value = savedVFX;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(savedMusic);
            AudioManager.Instance.SetSFXVolume(savedVFX);
        }

        // --- Listeners ---
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetVFXVolume(float value)
    {
        PlayerPrefs.SetFloat("VFXVolume", value);
        PlayerPrefs.Save();

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
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
