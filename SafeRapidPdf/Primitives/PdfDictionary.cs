using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	/// <summary>
	/// Immutable type
	/// </summary>
	public class PdfDictionary : PdfObject
    {
        public PdfDictionary(IFileStructureParser parser)
        {
			IsContainer = true;
			Object = new Dictionary<string, PdfObject>();
			String token;
			while ((token = parser.ReadToken()) != ">>")
			{
				if (token != "/")
					throw new Exception("Parse error: Name excepted!");
				PdfName name = parser.ReadPdfObject(token) as PdfName;
				PdfObject value = parser.ReadPdfObject();
				Dictionary.Add(name.Text, value);
			}
        }

		public ReadOnlyCollection<string> Keys { get { return Dictionary.Keys.ToList().AsReadOnly(); } }

		public PdfObject this[string name]
		{
			get
			{
				return Dictionary[name];
			}
		}

        private IDictionary<string, PdfObject> Dictionary
        {
			get { return Object as IDictionary<string, PdfObject>; }
        }
    }
}
