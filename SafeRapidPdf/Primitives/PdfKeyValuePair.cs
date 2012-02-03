using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfKeyValuePair : PdfObject
	{
		public PdfKeyValuePair(PdfName key, PdfObject value)
		{
			IsContainer = true;
			Key = key;
			Value = value;
		}

		public PdfName Key { get; private set; }
		public PdfObject Value { get; private set; }

		public override string ToString()
		{
			return Key.Text;
		}

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.Add(Value);
				return list.AsReadOnly();
			}
		}
	}
}
