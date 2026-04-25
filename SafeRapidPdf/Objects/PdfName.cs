using System.Text.RegularExpressions;

namespace SafeRapidPdf.Objects;

public sealed partial class PdfName : PdfObject
{
    private PdfName(string name)
        : base(PdfObjectType.Name)
    {
        RawName = name;
    }

    private string RawName { get; }

    [GeneratedRegex(@"#(\d\d)")]
    private static partial Regex HexEncodedCharRegex();

    public string Name
        // process the # encoded chars
        => HexEncodedCharRegex().Replace(RawName, x =>
            {
                byte val = Convert.ToByte(x.Groups[1].Value, 16);
                return ((char)val).ToString();
            });

    internal static PdfName Parse(Parsing.Lexer lexer)
    {
        string name = lexer.ReadToken();
        return new PdfName(name);
    }

    public override string ToString()
    {
        return Name;
    }
}
