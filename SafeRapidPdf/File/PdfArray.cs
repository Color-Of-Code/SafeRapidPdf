using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.File
{
	public class PdfArray: PdfObject
	{
		private PdfArray(ReadOnlyCollection<IPdfObject> items)
			: base(PdfObjectType.Array)
		{
			IsContainer = true;
			_items = items;
		}

		public static PdfArray Parse(Lexical.ILexer lexer)
		{
			lexer.Expects("[");
			var list = new List<IPdfObject>();
			PdfObject value;
			while ((value = PdfObject.ParseAny(lexer, "]")) != null)
			{
				list.Add(value);
			}
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
