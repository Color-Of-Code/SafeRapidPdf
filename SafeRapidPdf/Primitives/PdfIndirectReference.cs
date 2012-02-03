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
    public class PdfIndirectReference : PdfObject
    {
        public PdfIndirectReference(Lexical.ILexer lexer, IIndirectReferenceResolver resolver)
        {
			IsContainer = true;

			ObjectNumber = int.Parse(lexer.ReadToken());
			GenerationNumber = int.Parse(lexer.ReadToken());
			lexer.Expects("R");
			_resolver = resolver;
		}

		public int ObjectNumber { get; private set; }

        public int GenerationNumber { get; private set; }

		private IIndirectReferenceResolver _resolver;
		public PdfIndirectObject ReferencedObject
		{
			get
			{
				return _resolver.GetObject(ObjectNumber, GenerationNumber);
			}
		}

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.Add(ReferencedObject.PdfObject);
				return list.AsReadOnly();
			}
		}

		public override string ToString()
		{
			return String.Format("{0} {1} R", ObjectNumber, GenerationNumber);
		}
	}
}
