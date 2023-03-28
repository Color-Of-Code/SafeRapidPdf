using System;
using System.Collections.Generic;
using System.Text;

using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Objects;

public abstract class PdfObject : IPdfObject
{
    protected PdfObject(PdfObjectType type)
    {
        ObjectType = type;
    }

    public PdfObjectType ObjectType { get; }

    public bool IsContainer { get; protected set; }

    public string Text => ToString();

    public virtual IReadOnlyList<IPdfObject> Items
        => !IsContainer
            ? null
            : throw new NotImplementedException();

    internal static PdfObject ParseAny(Lexer lexer)
    {
        return ParseAny(lexer, string.Empty);
    }

    internal static PdfObject ParseAny(Lexer lexer, string endToken)
    {
        string token = lexer.ReadToken();
        if (token is null)
            return null;

        switch (token)
        {
            case "null": return PdfNull.Null;  // null object

            case "true":
            case "false":
                return PdfBoolean.Parse(token);

            case "/": return PdfName.Parse(lexer);
            case "%": return PdfComment.Parse(lexer);
            case "<": return PdfHexadecimalString.Parse(lexer);
            case "(": return PdfLiteralString.Parse(lexer);

            case "xref":
                return PdfXRef.Parse(lexer);

            case "trailer":
                return PdfTrailer.Parse(lexer);

            case "<<":
                var dic = PdfDictionary.Parse(lexer);

                // check for stream and combine put dictionary into stream object
                token = lexer.PeekToken1();

                if (token == "stream")
                {
                    return PdfStream.Parse(dic, lexer);
                }

                return dic;

            case "[": return PdfArray.Parse(lexer);

            case "startxref":
                return PdfStartXRef.Parse(lexer);

            case ")":
            case ">":
            case ">>":
            case "]":
            case "}":
            case "stream":
            case "endstream":
            case "endobj":
                if (endToken == token)
                {
                    return null; // expected end
                }

                throw new ParsingException("Out of sync");

            default:
                // must be an integer or double value
                PdfNumeric num = PdfNumeric.Parse(token);
                if (num.IsInteger)
                {
                    string token2 = lexer.PeekToken2();
                    switch (token2)
                    {
                        case "obj":
                            return PdfIndirectObject.Parse(lexer, num.ToInt32());

                        case "R":
                            PdfIndirectReference ir = PdfIndirectReference.Parse(lexer, num.ToInt32());
                            ir.Resolver = lexer.IndirectReferenceResolver;

                            return ir;
                        default:
                            // ignore;
                            return num;
                    }
                }
                else
                {
                    return num;
                }
        }

        throw new ParsingException("Could not read object");
    }

    internal static PdfObject ParseAny(PdfStream stream)
    {
        byte[] decodedBytes = stream.Decode();
        _ = Encoding.UTF8.GetString(decodedBytes);

        // contents are not always pdf objects...
        // var s = new MemoryStream(decodedBytes);
        // var parser = new LexicalParser(s, true);
        // return PdfObject.ParseAny(parser);
        return null;
    }
}
