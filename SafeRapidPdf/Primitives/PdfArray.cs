using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfArray : PdfObject
	{
		public PdfArray(Lexical.ILexer lexer)
		{
			IsContainer = true;
			lexer.Expects("[");
			var list = new List<PdfObject>();
			String token;
			while ((token = lexer.PeekToken()) != "]")
			{
				PdfObject value = PdfObject.Parse(lexer);
				list.Add(value);
			}
			lexer.Expects("]");
			Items = list.AsReadOnly();
		}

		public ReadOnlyCollection<PdfObject> Items { get; private set; }

		public override string ToString()
		{
			return String.Format("[{0}]", String.Join(" ", Items.Select(x => x.ToString())));
		}
	}
}
