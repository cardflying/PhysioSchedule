using System;
using UnityEngine;
using UnityEngine.UI;

public class Texture2DDrawer : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage;
    [SerializeField]
    private Button summitButton;
    [SerializeField]
    private Button clearButton;

    private int textureWidth = 1024;
    private int textureHeight = 1024;
    private Color drawColor = Color.black;
    private int brushSize = 8;
    private bool eraseMode = false;

    private Texture2D drawTexture;
    private RectTransform rectTransform;

    private bool _enableSign = false;
    private bool hasLastPoint = false;
    private Vector2Int lastPixelPos;
    private float minDrawDistance = 1f;

    private Action<Texture2D> signatureCallback;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        drawTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Bilinear;

        ClearTexture();
        rawImage.texture = drawTexture;
    }

    public void EnableSign(Action<Texture2D> signCallback)
    {
        signatureCallback = signCallback;

        transform.parent.gameObject.SetActive(true);
        _enableSign = true;
        summitButton.onClick.AddListener(SendTexture);
        clearButton.onClick.AddListener(ClearTexture);
    }

    public void DisableSign()
    {
        signatureCallback = null;

        transform.parent.gameObject.SetActive(false);
        _enableSign = false;
        summitButton.onClick.RemoveAllListeners();
        clearButton.onClick.RemoveAllListeners();
    }

    void Update()
    {
        if (!_enableSign)
            return;

        if (Input.GetMouseButtonDown(0))
            hasLastPoint = false;

        if (Input.GetMouseButton(0))
            DrawAtMouse();
    }

    void DrawAtMouse()
    {
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                Input.mousePosition,
                null,
                out localPoint))
            return;

        Rect rect = rectTransform.rect;

        float x = (localPoint.x - rect.x) / rect.width;
        float y = (localPoint.y - rect.y) / rect.height;

        int px = Mathf.RoundToInt(x * textureWidth);
        int py = Mathf.RoundToInt(y * textureHeight);

        Vector2Int currentPixel = new Vector2Int(px, py);

        if (!hasLastPoint)
        {
            DrawCircle(px, py);
            lastPixelPos = currentPixel;
            hasLastPoint = true;
            drawTexture.Apply();
            return;
        }

        float distance = Vector2Int.Distance(lastPixelPos, currentPixel);

        if (distance < minDrawDistance)
            return;

        int steps = Mathf.CeilToInt(distance / minDrawDistance);

        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            int ix = Mathf.RoundToInt(Mathf.Lerp(lastPixelPos.x, currentPixel.x, t));
            int iy = Mathf.RoundToInt(Mathf.Lerp(lastPixelPos.y, currentPixel.y, t));

            DrawCircle(ix, iy);
        }

        lastPixelPos = currentPixel;
        drawTexture.Apply();
    }

    void DrawCircle(int cx, int cy)
    {
        Color color = eraseMode ? Color.clear : drawColor;

        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                if (x * x + y * y > brushSize * brushSize)
                    continue;

                int px = cx + x;
                int py = cy + y;

                if (px < 0 || px >= textureWidth || py < 0 || py >= textureHeight)
                    continue;

                drawTexture.SetPixel(px, py, color);
            }
        }
    }

    public void ClearTexture()
    {
        Color[] clear = new Color[textureWidth * textureHeight];
        for (int i = 0; i < clear.Length; i++)
            clear[i] = Color.clear;

        drawTexture.SetPixels(clear);
        drawTexture.Apply();
    }

    public void SendTexture()
    {
        if (signatureCallback != null)
        {
            signatureCallback(drawTexture);
        }

        DisableSign();
    }
}
