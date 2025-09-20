using UnityEngine;

public class ObstacleAudioController : MonoBehaviour
{
    [Header("Sons de Obstáculos")]
    public AudioClip placeObstacleSfx;
    public AudioClip rotateObstacleSfx;

    private ObstaclePlacer placer;

    private void Start()
    {
        placer = FindObjectOfType<ObstaclePlacer>();
        if (placer != null)
        {
            placer.OnObstaclePlaced += HandleObstaclePlaced;
            placer.OnObstacleRotated += HandleObstacleRotated;
        }
    }

    private void OnDestroy()
    {
        if (placer != null)
        {
            placer.OnObstaclePlaced -= HandleObstaclePlaced;
            placer.OnObstacleRotated -= HandleObstacleRotated;
        }
    }

    private void HandleObstaclePlaced()
    {
        if (placeObstacleSfx != null)
            AudioManager.Instance?.PlaySFX(placeObstacleSfx);
    }

    private void HandleObstacleRotated()
    {
        if (rotateObstacleSfx != null)
            AudioManager.Instance?.PlaySFX(rotateObstacleSfx);
    }
}
