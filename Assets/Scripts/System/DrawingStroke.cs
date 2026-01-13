using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawingStroke : MonoBehaviour
{
    private LineRenderer strokeLine;
    private Color strokeColor;

    public float minPointDistance = 0.05f; // Minimum distance between points
    public float lineThickness = 0.05f;       // Optional: make collider thicker

    void Awake()
    {
        strokeLine = GetComponent<LineRenderer>();

        ResetStoke();
    }

    // Add a point to the line and update collider
    public void AddPoint(Vector3 worldPos)
    {
        if (strokeLine.positionCount > 0)
        {
            Vector3 last = strokeLine.GetPosition(strokeLine.positionCount - 1);
            if (Vector3.Distance(last, worldPos) < minPointDistance)
                return;
        }

        // LineRenderer
        strokeLine.positionCount++;
        strokeLine.SetPosition(strokeLine.positionCount - 1, worldPos);
    }

    public void AddPoint(Vector3[] pointList)
    {
        strokeLine.positionCount = pointList.Length;
        strokeLine.SetPositions(pointList);
    }

    public StrokeData EndPoint()
    {
        StrokeData currentStrokeData = new StrokeData();
        currentStrokeData.color = strokeColor.ToHexString();
        currentStrokeData.thickness = lineThickness;

        for (int i = 0; i < strokeLine.positionCount; i++)
            currentStrokeData.linePoints.Add(new Vec3Data(strokeLine.GetPosition(i)));

        return currentStrokeData;
    }

    public void SetStrokeColor(Color color)
    {
        strokeColor = color;

        strokeLine.startColor = color;
        strokeLine.endColor = color;
    }

    public void ResetStoke()
    {
        strokeLine.startWidth = strokeLine.endWidth = lineThickness;
        strokeLine.positionCount = 0;
    }
}
