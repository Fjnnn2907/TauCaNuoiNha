using UnityEngine;
using TMPro;

public class SymbolManager : MonoBehaviour
{
    public string[] symbolNames = { "circle", "zigzag", "vshape" };
    public TMP_Text targetSymbolText;

    [HideInInspector] public string currentSymbol;

    void Start()
    {
        GenerateNewSymbol();
    }

    public void GenerateNewSymbol()
    {
        currentSymbol = symbolNames[Random.Range(0, symbolNames.Length)];
        targetSymbolText.text = $"Draw: {currentSymbol}";
    }
}
