using UnityEngine;

public abstract class ActiveObstacleBase : MonoBehaviour
{
    protected bool isActive = false;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;

    protected virtual void Awake()
    { }
    protected virtual void Start()
    {
        // Salva estado inicial
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;

        // Escuta eventos
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    protected virtual void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Play)
        {
            Activate();
        }
        else if (newState == GameState.Build)
        {
            ResetObstacle();
        }
        else if (newState == GameState.End)
        {
            isActive = false; // apenas pausa
        }
    }

    protected virtual void Update()
    {
        if (isActive)
            ActiveUpdate();
    }

    // -----------------------------
    // Métodos para herança
    // -----------------------------
    protected abstract void ActiveUpdate();

    protected virtual void Activate()
    {
        isActive = true;
    }

    protected virtual void ResetObstacle()
    {
        isActive = false;

        // Volta para o estado inicial
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;
    }
}
