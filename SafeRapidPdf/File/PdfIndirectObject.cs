using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SafeRapidPdf.File
{
    public class PdfIndirectObject : PdfObject
	{
		private PdfIndirectObject(int objectNumber, int generationNumber, IPdfObject obj)
			: base(PdfObjectType.IndirectObject)
		{
			IsContainer = true;

			ObjectNumber = objectNumber;
			GenerationNumber = generationNumber;
			Object = obj;
		}

		public static PdfIndirectObject Parse(Lexical.ILexer lexer)
		{
			int objectNumber = int.Parse(lexer.ReadToken());
			return Parse(lexer, objectNumber);
		}

		public static PdfIndirectObject Parse(Lexical.ILexer lexer, int objectNumber)
		{
			int generationNumber = int.Parse(lexer.ReadToken());
			lexer.Expects("obj");
			PdfObject obj = PdfObject.ParseAny(lexer);
			lexer.Expects("endobj");
			return new PdfIndirectObject(objectNumber, generationNumber, obj);
		}

		public int ObjectNumber { get; private set; }

		public int GenerationNumber { get; private set; }

		public IPdfObject Object
		{
			get;
			private set;
		}

		public override string ToString()
		{
			return String.Format("{0} {1} obj", ObjectNumber, GenerationNumber);
		}

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.Add(Object);
				return list.AsReadOnly();
			}
		}
	}
}
