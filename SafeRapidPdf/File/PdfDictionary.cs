using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.File
{
	/// <summary>
	/// A PDF Dictionary type, a collection of named objects
	/// </summary>
	public class PdfDictionary : PdfObject
    {
        private PdfDictionary(IList<PdfKeyValuePair> dictionary)
			: base(PdfObjectType.Dictionary)
		{
			IsContainer = true;
			_dictionary = dictionary;
		}

		public void ExpectsType(String name)
		{
			PdfName type = this["Type"] as PdfName;
			if (type.Name != name)
				throw new Exception(String.Format("Expected {0}, but got {1}", name, type.Name));
		}

        public static PdfDictionary Parse(Lexical.ILexer lexer)
        {
			lexer.Expects("<<");
			var dictionary = new List<PdfKeyValuePair>();
			String token;
			while ((token = lexer.PeekToken()) != ">>")
			{
				PdfName name = PdfName.Parse(lexer);
				PdfObject value = PdfObject.ParseAny(lexer);

				dictionary.Add(new PdfKeyValuePair(name, value));
			}
			lexer.Expects(">>");
			return new PdfDictionary(dictionary);
        }

		public IPdfObject this[string name]
		{
			get
			{
				return _dictionary.First(x => x.Key.Text == name).Value;
			}
		}

		private IList<PdfKeyValuePair> _dictionary;

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				return _dictionary.ToList().ConvertAll(x => x as IPdfObject).AsReadOnly();
			}
		}

		public override string ToString()
		{
			return "<<...>>";
		}
	}
}
