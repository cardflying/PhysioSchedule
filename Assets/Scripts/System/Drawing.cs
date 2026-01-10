using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Drawing : MonoBehaviour
{
    [Header("Stroke")]
    [SerializeField]
    private DrawingStroke strokePrefab;

    [Header("UI Draw Area")]
    [SerializeField]
    private RectTransform drawImage;   // Assign Image RectTransform
    [SerializeField]
    private Canvas canvas;

    private DrawingStroke currentStroke;
    private Color currentColor;
    private bool drawing;
    private bool enableDraw;
    private Dictionary<Color,DrawingStroke> strokes = new();
    private List<DrawingStroke> strokesPool = new();

    private readonly HashSet<Color> usedColors = new();
    private const float MinValue = 0.25f;
    private const float MaxValue = 0.6f;

    public Action<Color> drawStrokeTrigger;

    public void Enable()
    {
        enableDraw = true;
    }
    public void Disable()
    {
        enableDraw = false;

        foreach (var strokeObj in strokes.Values)
        {
            strokeObj.gameObject.SetActive(false);
            strokesPool.Add(strokeObj);
        }
        usedColors.Clear();
    }

    void Update()
    {
        if (enableDraw == false) return;

        Vector2 screenPos = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            if (IsInsideImage(screenPos))
            {
                BeginLine(ScreenToWorld(screenPos));
                drawing = true;
            }
        }

        if (Input.GetMouseButton(0) && drawing)
        {
            if (IsInsideImage(screenPos))
                Draw(ScreenToWorld(screenPos));
            else
                EndLine();
        }

        if (Input.GetMouseButtonUp(0) && drawing)
        {
            EndLine();
        }
    }

    void BeginLine(Vector3 worldPos)
    {
        if (strokesPool.Count > 0)
        {
            currentStroke = strokesPool[0];
            currentStroke.gameObject.SetActive(true);
            strokesPool.RemoveAt(0);
        }
        else
        {
            currentStroke = Instantiate(strokePrefab, transform);
        }
        currentColor = GenerateColor();
        strokes.Add(currentColor,currentStroke);
        currentStroke.SetStrokeColor(currentColor);
        currentStroke.AddPoint(worldPos);
    }

    void Draw(Vector3 worldPos)
    {
        if (currentStroke != null)
            currentStroke.AddPoint(worldPos);
    }

    void EndLine()
    {
        currentStroke = null;
        drawing = false;
        
        if (drawStrokeTrigger != null)
            drawStrokeTrigger(currentColor);
    }

    // =========================
    // UI LOGIC
    // =========================

    bool IsInsideImage(Vector2 screenPos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            drawImage,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera
        );
    }

    Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            drawImage,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : canvas.worldCamera,
            out worldPos
        );

        worldPos.z = 0f;
        return worldPos;
    }

    /// <summary>
    /// Genearte Random Color
    /// </summary>
    /// <returns></returns>
    public Color GenerateColor()
    {
        Color color;

        do
        {
            color = Color.HSVToRGB(
                Random.value,              // Hue (random)
                Random.Range(0.6f, 1f),    // Saturation
                Random.Range(MinValue, MaxValue) // Value (dark)
            );
        }
        while (usedColors.Contains(color));

        usedColors.Add(color);
        return color;
    }

    public void RemoveStroke(Color color)
    {
        if (strokes.ContainsKey(color))
        {
            DrawingStroke selectStroke = strokes[color];
            selectStroke.gameObject.SetActive(false);
            selectStroke.ResetStoke();
            strokesPool.Add(selectStroke);
            strokes.Remove(color);
        }

        Debug.Log(usedColors.Contains(color));
    }
}
