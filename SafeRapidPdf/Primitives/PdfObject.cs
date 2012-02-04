using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public abstract class PdfObject : IPdfObject
	{
		public PdfObject()
		{
		}

		public static PdfObject ParseAny(Lexical.ILexer lexer)
		{
			String token = lexer.PeekToken();
			if (token == null) return null;

			Primitives.PdfObject obj = null;
			switch (token)
			{
				// null object
				case "null":
					obj = PdfNull.Parse(lexer);
					break;
				case "%":
					obj = PdfComment.Parse(lexer);
					break;
				case "true":
				case "false":
					obj = PdfBoolean.Parse(lexer);
					break;
				case "obj":
					obj = PdfIndirectObject.Parse(lexer);
					break;
				case "R":
					obj = PdfIndirectReference.Parse(lexer, lexer.IndirectReferenceResolver);
					break;
				case "<<":
					obj = PdfDictionary.Parse(lexer);
					// check for stream and combine put dictionary into stream object
					token = lexer.PeekToken();
					if (token == "stream")
						obj = new PdfStream(obj as PdfDictionary, lexer);
					break;
				case "[":
					obj = PdfArray.Parse(lexer);
					break;
				case "<":
					obj = new PdfHexadecimalString(lexer);
					break;
				case "(":
					obj = new PdfLiteralString(lexer);
					break;
				case "/":
					obj = PdfName.Parse(lexer);
					break;
				case "xref":
					obj = new PdfXRef(lexer);
					break;
				case "trailer":
					obj = new PdfTrailer(lexer);
					break;

				case ")":
				case ">":
				case ">>":
				case "]":
				case "}":
				case "stream":
				case "endstream":
				case "endobj":
					throw new Exception("Parser error: out of sync");

				case "startxref":
					obj = PdfStartXRef.Parse(lexer);
					break;

				default:
					// must be an integer or double value
					obj = PdfNumeric.Parse(lexer);
					break;
			}
			if (obj == null)
				throw new Exception("Parsing error, could not read object");
			return obj;
		}

		public bool IsContainer { get; protected set; }

		public String Text
		{
			get
			{
				return ToString();
			}
		}

		public virtual ReadOnlyCollection<IPdfObject> Items
		{
			get 
			{
				if (!IsContainer)
					return null;
				throw new NotImplementedException();
			}
		}
	}
}
