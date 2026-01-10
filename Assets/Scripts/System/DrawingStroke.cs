using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class DrawingStroke : MonoBehaviour
{
    private LineRenderer line;
    private EdgeCollider2D edge;
    private List<Vector2> colliderPoints = new List<Vector2>();

    public float minPointDistance = 0.05f; // Minimum distance between points
    public float lineThickness = 0.05f;       // Optional: make collider thicker

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        edge = GetComponent<EdgeCollider2D>();

        ResetStoke();
    }

    // Add a point to the line and update collider
    public void AddPoint(Vector3 worldPos)
    {
        if (line.positionCount > 0)
        {
            Vector3 last = line.GetPosition(line.positionCount - 1);
            if (Vector3.Distance(last, worldPos) < minPointDistance)
                return;
        }

        // LineRenderer
        line.positionCount++;
        line.SetPosition(line.positionCount - 1, worldPos);

        // EdgeCollider2D (LOCAL SPACE!)
        colliderPoints.Add(transform.InverseTransformPoint(worldPos));
        edge.SetPoints(colliderPoints);
    }

    // Check if the mouse/touch hits this stroke
    public bool HitTest(Vector2 worldPos)
    {
        // Raycast against collider
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        Debug.Log(hit.collider);
        if (hit.collider == edge)
            return true;

        return false;
    }

    public void SetStrokeColor(Color color)
    {
        line.startColor = color;
        line.endColor = color;
    }

    public void ResetStoke()
    {
        line.startWidth = line.endWidth = lineThickness;
        line.positionCount = 0;
        edge.points = new Vector2[0];
        edge.edgeRadius = lineThickness / 2.0f;
    }
}
