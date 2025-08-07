using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RuntimeGrid : MonoBehaviour
{
    public float gridSize = 1f;
    public int width = 20;
    public int height = 10;
    public Color lineColor = new Color(1f, 1f, 1f, 0.15f);
    public Vector2 origin = Vector2.zero;
    public Material lineMaterial;

    void Start()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        GameObject gridParent = new GameObject("GridLines");
        gridParent.transform.SetParent(transform);

        for (int x = 0; x <= width; x++)
        {
            CreateLine(
                new Vector3(origin.x + x * gridSize, origin.y, 0),
                new Vector3(origin.x + x * gridSize, origin.y + height * gridSize, 0),
                gridParent.transform
            );
        }

        for (int y = 0; y <= height; y++)
        {
            CreateLine(
                new Vector3(origin.x, origin.y + y * gridSize, 0),
                new Vector3(origin.x + width * gridSize, origin.y + y * gridSize, 0),
                gridParent.transform
            );
        }
    }

    void CreateLine(Vector3 start, Vector3 end, Transform parent)
    {
        GameObject lineObj = new GameObject("GridLine");
        lineObj.transform.SetParent(parent);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        lr.material = lineMaterial;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.sortingLayerName = "Foreground"; // ou "Default" se preferir
        lr.sortingOrder = -1; // atrás dos obstáculos
    }
}
