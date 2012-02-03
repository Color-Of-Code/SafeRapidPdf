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

		public static PdfObject Parse(Lexical.ILexer lexer)
		{
			String token = lexer.PeekToken();
			if (token == null) return null;

			Primitives.PdfObject obj = null;
			switch (token)
			{
				// null object
				case "null":
					obj = new PdfNull(lexer);
					break;
				case "%":
					obj = new PdfComment(lexer);
					break;
				case "true":
				case "false":
					obj = new PdfBoolean(lexer);
					break;
				case "obj":
					obj = new PdfIndirectObject(lexer);
					break;
				case "R":
					obj = new PdfIndirectReference(lexer, lexer.IndirectReferenceResolver);
					break;
				case "<<":
					obj = new PdfDictionary(lexer);
					// check for stream and combine put dictionary into stream object
					token = lexer.PeekToken();
					if (token == "stream")
						obj = new PdfStream(obj as PdfDictionary, lexer);
					break;
				case "[":
					obj = new PdfArray(lexer);
					break;
				case "<":
					obj = new PdfHexadecimalString(lexer);
					break;
				case "(":
					obj = new PdfLiteralString(lexer);
					break;
				case "/":
					obj = new PdfName(lexer);
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
					obj = new PdfStartXRef(lexer);
					break;

				default:
					// must be an integer or double value
					obj = new PdfNumeric(lexer);
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
