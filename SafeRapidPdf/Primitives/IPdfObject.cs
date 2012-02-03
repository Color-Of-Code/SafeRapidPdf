using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public interface IPdfObject
	{
		String Text { get; }

		ReadOnlyCollection<IPdfObject> Items { get; }
	}
}
