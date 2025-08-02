using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SymbolManager : MonoBehaviour
{
    [Header("Symbol Settings")]
    public string[] symbolNames = { "circle", "zigzag", "vshape", "square", "triangle" };

    [Header("UI References")]
    public TMP_Text targetSymbolText; // Text hiển thị (có thể bỏ nếu chỉ dùng ảnh)
    public Image targetSymbolImage; // Ảnh hiển thị symbol cần vẽ

    [Header("Symbol Images")]
    public Sprite circleSprite; // Ảnh cho circle
    public Sprite zigzagSprite; // Ảnh cho zigzag
    public Sprite vshapeSprite; // Ảnh cho vshape
    public Sprite squareSprite; // Ảnh cho square (hình vuông)
    public Sprite triangleSprite; // Ảnh cho triangle (hình tam giác)

    [HideInInspector] public string currentSymbol;

    void Start()
    {
        GenerateNewSymbol();
    }

    public void GenerateNewSymbol()
    {
        currentSymbol = symbolNames[Random.Range(0, symbolNames.Length)];

        // Cập nhật text (nếu có)
        if (targetSymbolText != null)
        {
            //targetSymbolText.text = $"Draw: {currentSymbol}";
            targetSymbolText.text = $"Vẽ:";
        }

        // Cập nhật ảnh (nếu có)
        if (targetSymbolImage != null)
        {
            Sprite symbolSprite = GetSpriteForSymbol(currentSymbol);
            if (symbolSprite != null)
            {
                targetSymbolImage.sprite = symbolSprite;
                targetSymbolImage.enabled = true;
            }
            else
            {
                // Nếu không có ảnh, ẩn Image và dùng text
                targetSymbolImage.enabled = false;
                if (targetSymbolText != null)
                {
                    //targetSymbolText.text = $"Draw: {currentSymbol}";
                    targetSymbolText.text = $"Vẽ:";
                }
            }
        }
    }

    // Lấy sprite tương ứng với symbol
    private Sprite GetSpriteForSymbol(string symbolName)
    {
        switch (symbolName.ToLower())
        {
            case "circle":
                return circleSprite;
            case "zigzag":
                return zigzagSprite;
            case "vshape":
                return vshapeSprite;
            case "square":
                return squareSprite;
            case "triangle":
                return triangleSprite;
            default:
                return null;
        }
    }

    // Method để set ảnh cho symbol cụ thể
    public void SetSymbolImage(string symbolName, Sprite sprite)
    {
        switch (symbolName.ToLower())
        {
            case "circle":
                circleSprite = sprite;
                break;
            case "zigzag":
                zigzagSprite = sprite;
                break;
            case "vshape":
                vshapeSprite = sprite;
                break;
            case "square":
                squareSprite = sprite;
                break;
            case "triangle":
                triangleSprite = sprite;
                break;
        }
    }
}
