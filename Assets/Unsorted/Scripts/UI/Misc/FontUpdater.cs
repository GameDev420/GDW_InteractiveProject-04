using UnityEngine;
using TMPro;

public class FontUpdater : MonoBehaviour
{
    [SerializeField] FontSelector.FontType fontType;
    [SerializeField] FontSelector fontSelector;
    [SerializeField] TMP_Text text;

    private void OnValidate() => UpdateFont();

    private void Start() => UpdateFont();

    void UpdateFont()
    {
        if (fontSelector != null && (text != null || TryGetComponent(out text)) && text.font != fontSelector.GetFont(fontType)) text.font = fontSelector.GetFont(fontType);
    }
}
