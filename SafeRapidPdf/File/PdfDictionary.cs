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
	public class PdfDictionary: PdfObject
	{
		private PdfDictionary(IList<PdfKeyValuePair> dictionary)
			: base(PdfObjectType.Dictionary)
		{
			IsContainer = true;
			_dictionary = dictionary;
		}

		protected PdfDictionary(PdfDictionary dictionary, PdfObjectType type)
			: base(type)
		{
			IsContainer = true;
			_dictionary = dictionary._dictionary;
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
			PdfObject obj;
			while ((obj = PdfObject.ParseAny(lexer, ">>")) != null)
			{
				PdfName name = obj as PdfName;
				if (name == null)
					throw new Exception("Parser Error: the first item of a pair inside a dictionary must be a PDF name object");
				PdfObject value = PdfObject.ParseAny(lexer);

				dictionary.Add(new PdfKeyValuePair(name, value));
			}
			return new PdfDictionary(dictionary);
		}

		public IPdfObject this[string name]
		{
			get
			{
				return _dictionary.First(x => x.Key.Text == name).Value;
			}
		}

		/// <summary>
		/// Automatically dereference indirect references or returns the Pdf object
		/// after checking that it is of the expected type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public T Resolve<T>(String name) where T: class
		{
			IPdfObject value = this[name];
			if (value is PdfIndirectReference)
			{
				PdfIndirectReference reference = value as PdfIndirectReference;
				return reference.Dereference<T>();
			}
			if (value is T)
				return value as T;
			throw new Exception(String.Format("Value '{0}' not of the expected type {1}", typeof(T)));
		}

		private IList<PdfKeyValuePair> _dictionary;

		public IEnumerable<String> Keys
		{
			get
			{
				foreach (PdfKeyValuePair pair in _dictionary)
				{
					yield return pair.Key.Text;
				}
			}
		}

		public IEnumerable<IPdfObject> Values
		{
			get
			{
				foreach (PdfKeyValuePair pair in _dictionary)
				{
					yield return pair.Value;
				}
			}
		}

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				return _dictionary.ToList().ConvertAll(x => x as IPdfObject).AsReadOnly();
			}
		}

		public String Type
		{
			get
			{
				if (Keys.Contains("Type"))
				{
					PdfName type = this["Type"] as PdfName;
					return type.Name;
				}
				return null;
			}
		}

		public override string ToString()
		{
			String type = Type;
			if (type == null)
				return "<<...>>";
			return String.Format("<<...>> ({0})", type);
		}
	}
}
