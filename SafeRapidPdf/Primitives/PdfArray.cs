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
			var list = new List<IPdfObject>();
			String token;
			while ((token = lexer.PeekToken()) != "]")
			{
				PdfObject value = PdfObject.Parse(lexer);
				list.Add(value);
			}
			lexer.Expects("]");
			_items = list.AsReadOnly();
		}

		private ReadOnlyCollection<IPdfObject> _items;
		public override ReadOnlyCollection<IPdfObject> Items { get { return _items; } }

		public override string ToString()
		{
			return "[...]";
		}
	}
}
