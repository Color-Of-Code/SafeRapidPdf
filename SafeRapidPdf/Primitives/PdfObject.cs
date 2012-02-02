using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfObject
	{
		public object Object
		{ get; protected set; }

		public bool IsNull
		{
			get { return (Object == null); }
		}

		public bool IsContainer { get; protected set; }

		public static readonly PdfObject Null = new PdfObject();
	}
}
