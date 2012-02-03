using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf
{
	/// <summary>
	/// Interface common to all PDF objects high or low level.
	/// The presence of this interface eases the implementation
	/// of code crawling through all objects.
	/// </summary>
	public interface IPdfObject
	{
		/// <summary>
		/// Description of this object
		/// </summary>
		String Text { get; }

		/// <summary>
		/// Does this object have descendants
		/// </summary>
		bool IsContainer { get; }

		/// <summary>
		/// The children of this object
		/// </summary>
		ReadOnlyCollection<IPdfObject> Items { get; }
	}
}
