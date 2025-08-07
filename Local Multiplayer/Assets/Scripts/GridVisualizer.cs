using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public float gridSize = 1f;
    public int width = 20;
    public int height = 10;
    public Color gridColor = new Color(1f, 1f, 1f, 0.2f);

    public Vector2 origin = Vector2.zero;

    void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(origin.x + x * gridSize, origin.y, 0);
            Vector3 end = new Vector3(origin.x + x * gridSize, origin.y + height * gridSize, 0);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(origin.x, origin.y + y * gridSize, 0);
            Vector3 end = new Vector3(origin.x + width * gridSize, origin.y + y * gridSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}
