using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject painelTutorial;
    [SerializeField] private GameObject painelMainMenu;
    [SerializeField] private GameObject painelControles;
    [SerializeField] private GameObject painelCreditos;

    public void Comecar()
    {
        SceneManager.LoadScene(nomeDoLevelDeJogo);
    }

    public void AbrirTutorial()
    {
        painelMainMenu.SetActive(false);
        painelTutorial.SetActive(true);
    }
    public void FecharTutorial()
    {
        painelTutorial.SetActive(false);
        painelMainMenu.SetActive(true);
    }

    public void AbrirConfig()
    {
        painelMainMenu.SetActive(false);
        painelControles.SetActive(true);
    }

    public void FecharConfig()
    {
        painelControles.SetActive(false);
        painelMainMenu.SetActive(true);
    }

    public void AbrirCreditos()
    {
        painelMainMenu.SetActive(false);
        painelCreditos.SetActive(true);
    }

    public void FecharCreditos()
    {
        painelCreditos.SetActive(false);
        painelMainMenu.SetActive(true);
    }

    public void SairJogo()
    {
        Debug.Log("Sair do Jogo");
        Application.Quit();
    }
}