using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.File
{
	public abstract class PdfObject : IPdfObject
	{
		protected PdfObject(PdfObjectType type)
		{
			ObjectType = type;
		}

		public PdfObjectType ObjectType
		{
			get; private set;
		}

		public static PdfObject ParseAny(Lexical.ILexer lexer)
		{
			String token = lexer.PeekToken();
			if (token == null) return null;

			File.PdfObject obj = null;
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
					PdfIndirectReference ir = PdfIndirectReference.Parse(lexer);
					ir.Resolver = lexer.IndirectReferenceResolver;
					obj = ir;
					break;
				case "<<":
					obj = PdfDictionary.Parse(lexer);
					// check for stream and combine put dictionary into stream object
					token = lexer.PeekToken();
					if (token == "stream")
						obj = PdfStream.Parse(obj as PdfDictionary, lexer);
					break;
				case "[":
					obj = PdfArray.Parse(lexer);
					break;
				case "<":
					obj = PdfHexadecimalString.Parse(lexer);
					break;
				case "(":
					obj = PdfLiteralString.Parse(lexer);
					break;
				case "/":
					obj = PdfName.Parse(lexer);
					break;
				case "xref":
					obj = PdfXRef.Parse(lexer);
					break;
				case "trailer":
					obj = PdfTrailer.Parse(lexer);
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
				throw new Exception("Parse error, could not read object");
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
