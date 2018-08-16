using System;
using System.Collections.Generic;
using System.Text;

using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Objects
{
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
        {
            get
            {
                if (!IsContainer)
                    return null;
                throw new NotImplementedException();
            }
        }

        public static PdfObject ParseAny(ILexer lexer)
        {
            return ParseAny(lexer, string.Empty);
        }

        public static PdfObject ParseAny(ILexer lexer, string endToken)
        {
            string token = lexer.ReadToken();
            if (token == null)
                return null;

            PdfObject obj = null;
            switch (token)
            {
                // null object
                case "null":
                    obj = PdfNull.Null;
                    break;

                case "true":
                case "false":
                    obj = PdfBoolean.Parse(token);
                    break;

                case "/":
                    obj = PdfName.Parse(lexer);
                    break;

                case "%":
                    obj = PdfComment.Parse(lexer);
                    break;

                case "<":
                    obj = PdfHexadecimalString.Parse(lexer);
                    break;

                case "(":
                    obj = PdfLiteralString.Parse(lexer);
                    break;

                case "xref":
                    obj = PdfXRef.Parse(lexer);
                    break;

                case "trailer":
                    obj = PdfTrailer.Parse(lexer);
                    break;

                case "<<":
                    obj = PdfDictionary.Parse(lexer);

                    // check for stream and combine put dictionary into stream object
                    token = lexer.PeekToken1();
                    if (token == "stream")
                    {
                        obj = PdfStream.Parse(obj as PdfDictionary, lexer);
                    }

                    break;

                case "[":
                    obj = PdfArray.Parse(lexer);
                    break;

                case "startxref":
                    obj = PdfStartXRef.Parse(lexer);
                    break;

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
                                obj = PdfIndirectObject.Parse(lexer, num);
                                break;

                            case "R":
                                PdfIndirectReference ir = PdfIndirectReference.Parse(lexer, num);
                                ir.Resolver = lexer.IndirectReferenceResolver;
                                obj = ir;
                                break;
                            default:
                                // ignore;
                                obj = num;
                                break;
                        }
                    }
                    else
                    {
                        obj = num;
                    }
                    break;
            }

            if (obj == null)
            {
                throw new ParsingException("Could not read object");
            }

            return obj;
        }

        public static PdfObject ParseAny(PdfStream stream)
        {
            var decodedBytes = stream.Decode();
            var s = Encoding.UTF8.GetString(decodedBytes);

            // contents are not always pdf objects...
            // var s = new MemoryStream(decodedBytes);
            // var parser = new LexicalParser(s, true);
            // return PdfObject.ParseAny(parser);
            return null;
        }
    }
}