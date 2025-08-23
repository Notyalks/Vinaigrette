using UnityEngine;
using System.Collections;

/// Classe base para obstáculos que disparam ações em intervalos durante a fase de gameplay.
public abstract class TimedObstacleBase : ActiveObstacleBase
{
    [Header("Configuração do Tempo")]
    public float interval = 2f;     // intervalo entre ativações
    public float startDelay = 0f;   // atraso inicial antes de começar

    private Coroutine routine;

    protected override void Activate()
    {
        base.Activate();
        routine = StartCoroutine(RunRoutine());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (routine != null) StopCoroutine(routine);
    }

    /// Define a ação que o obstáculo faz a cada intervalo.
    protected abstract void OnInterval();

    private IEnumerator RunRoutine()
    {
        if (startDelay > 0) yield return new WaitForSeconds(startDelay);

        while (isActive)
        {
            OnInterval();
            yield return new WaitForSeconds(interval);
        }
    }

    // Esse tipo de obstáculo normalmente não usa Update
    protected override void ActiveUpdate() { }
}
