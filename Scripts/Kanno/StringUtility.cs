using System.Text;

public static class StringUtility
{
    private static StringBuilder builder = new StringBuilder();

    /// <summary>
    /// カタカナとひらがなの文字コードの差分
    /// </summary>
    private static readonly int offset = beginKatakana - beginHiragana;
    private const char beginHiragana = 'ぁ';
    private const char endHiragana = 'ゖ';
    private const char beginKatakana = 'ァ';
    private const char endKatakana = 'ヶ';

    /// <summary>
    /// ひらがなをカタカナに変換
    /// </summary>
    public static string ToKatakana(this string text)
    {
        builder.Clear();
        foreach (char c in text)
        {
            builder.Append(
                beginHiragana <= c && c <= endHiragana ?
                    (char)(c + offset) :
                    c
            );
        }
        return builder.ToString();
    }

    /// <summary>
    /// カタカナをひらがなに変換
    /// </summary>
    public static string ToHiragana(this string text)
    {
        builder.Clear();
        foreach (char c in text)
        {
            builder.Append(
                beginKatakana <= c && c <= endKatakana ?
                    (char)(c - offset) :
                    c
            );
        }
        return builder.ToString();
    }
}