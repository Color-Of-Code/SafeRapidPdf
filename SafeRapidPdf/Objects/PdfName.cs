using System;
using System.Text.RegularExpressions;

namespace SafeRapidPdf.Objects;

public sealed class PdfName : PdfObject
{
    private readonly string _rawName;

    private PdfName(string name)
        : base(PdfObjectType.Name)
    {
        _rawName = name;
    }

    public string Name
        // process the # encoded chars
        => Regex.Replace(_rawName, @"#(\d\d)", x =>
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
