using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.File
{
	/// <summary>
	/// Object not described in the specification but eases use and
	/// implementation in .NET
	/// </summary>
	public class PdfKeyValuePair : PdfObject
	{
		public PdfKeyValuePair(PdfName key, PdfObject value)
			: base(PdfObjectType.KeyValuePair)
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
