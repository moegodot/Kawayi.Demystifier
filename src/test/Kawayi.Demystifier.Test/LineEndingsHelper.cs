using System.Text.RegularExpressions;

namespace Kawayi.Demystifier.Test;

internal static class LineEndingsHelper
{
    private static readonly Regex ReplaceLineEndings = new Regex(" in [^\n]+");

    public static string RemoveLineEndings(string original)
    {
        return ReplaceLineEndings.Replace(original.Replace("\r\n", "\n").Replace("\r", "\n"), "");
    }
}
