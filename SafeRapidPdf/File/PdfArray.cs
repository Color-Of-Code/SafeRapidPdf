using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.File
{
	public class PdfArray: PdfObject
	{
		private PdfArray(List<IPdfObject> items)
			: base(PdfObjectType.Array)
		{
			IsContainer = true;
			_items = items;
		}

		public static PdfArray Parse(Lexical.ILexer lexer)
		{
			var list = new List<IPdfObject>();
			PdfObject value;
			while ((value = PdfObject.ParseAny(lexer, "]")) != null)
			{
				list.Add(value);
			}
			return new PdfArray(list);
		}

		private List<IPdfObject> _items;
		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				return _items.AsReadOnly();
			}
		}

		public override string ToString()
		{
			return "[...]";
		}
	}
}
