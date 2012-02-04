using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfArray : PdfObject
	{
		private PdfArray(ReadOnlyCollection<IPdfObject> items)
		{
			IsContainer = true;
			_items = items;
		}

		public static PdfArray Parse(Lexical.ILexer lexer)
		{
			lexer.Expects("[");
			var list = new List<IPdfObject>();
			String token;
			while ((token = lexer.PeekToken()) != "]")
			{
				PdfObject value = PdfObject.ParseAny(lexer);
				list.Add(value);
			}
			lexer.Expects("]");
			return new PdfArray(list.AsReadOnly());
		}

		private ReadOnlyCollection<IPdfObject> _items;
		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				return _items;
			}
		}

		public override string ToString()
		{
			return "[...]";
		}
	}
}
