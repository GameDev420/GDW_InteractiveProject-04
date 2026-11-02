using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "NewFontSelector", menuName = "Font Selector")]
public class FontSelector : ScriptableObject
{
    public enum FontType { Primary, Secondary, Tertiary }
    public TMP_FontAsset PrimaryFont, SecondaryFont, TertiaryFont;

    public TMP_FontAsset GetFont(FontType type)
    {
        switch (type)
        {
            case FontType.Primary:
                return PrimaryFont;
            case FontType.Secondary:
                return SecondaryFont;
            case FontType.Tertiary:
                return TertiaryFont;

            default:
                return TertiaryFont != null ? TertiaryFont : (SecondaryFont != null ? SecondaryFont : PrimaryFont);
        }
    }
}
