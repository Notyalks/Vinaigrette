using UnityEngine;

public class BuildPhaseIndicatorController : MonoBehaviour
{
    [Header("Indicadores de Construção")]
    public GameObject[] indicators; // Coloque aqui os objetos visuais (Start/Goal etc.)

    void Start()
    {
        // Garante que começa no estado certo
        UpdateIndicators(GameManager.Instance.currentState);

        // Se inscreve no evento de mudança de estado
        GameManager.Instance.OnGameStateChanged += UpdateIndicators;
    }

    void OnDestroy()
    {
        // Evita erro de referência nula ao destruir objeto
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= UpdateIndicators;
    }

    void UpdateIndicators(GameState newState)
    {
        bool show = (newState == GameState.Build);

        foreach (var ind in indicators)
        {
            if (ind != null)
                ind.SetActive(show);
        }
    }
}
