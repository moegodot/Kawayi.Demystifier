using System.Text;

namespace Kawayi.Demystifier;

public sealed class StyledStringBuilder
{
    private readonly StringBuilder _builder = new StringBuilder();

    public StyledStringBuilder Append(string text)
    {
        _builder.Append(text);
        return this;
    }

    public StyledStringBuilder Append(Style style, string text)
    {
        var ansi = style.ToAnsiCode();
        _builder.Append(ansi);
        _builder.Append(text);
        if (!string.IsNullOrWhiteSpace(ansi))
        {
            _builder.Append(Style.ClearStyleAnsiCode);
        }
        return this;
    }

    public StyledStringBuilder AppendPath(Style pathStyle, Style fileStyle, string path, bool shortenPath)
    {
        if (shortenPath)
        {
            return this.Append(
                fileStyle,
                Path.GetFileName(path));
        }
        else
        {
            return this.Append(
                pathStyle,
                Path.GetDirectoryName(path) ?? string.Empty)
                .Append(Path.DirectorySeparatorChar.ToString())
                .Append(
                fileStyle,
                Path.GetFileName(path));
        }
    }

    public StyledStringBuilder AppendLine(string text)
    {
        _builder.Append(text);
        _builder.Append('\n');
        return this;
    }

    public StyledStringBuilder AppendLine()
    {
        _builder.Append('\n');
        return this;
    }

    public override string ToString()
    {
        return _builder.ToString();
    }
}
