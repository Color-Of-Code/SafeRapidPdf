using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfStream : PdfObject
	{
		public PdfStream(PdfDictionary dictionary, Lexical.ILexer lexer)
		{
			IsContainer = true;

			lexer.Expects("stream");
			lexer.SkipEol(); // position to begin of stream data

			if (dictionary == null)
				throw new Exception("Parser error: stream needs a dictionary");

			PdfObject lengthObject = dictionary["Length"];
			if (lengthObject == null)
				throw new Exception("Parser error: stream dictionary requires a Length entry");

			int length = 0;
			if (lengthObject is PdfIndirectReference)
			{
				PdfIndirectReference reference = lengthObject as PdfIndirectReference;
				PdfIndirectObject lenobj = lexer.GetObject(reference.ObjectNumber, reference.GenerationNumber);
				PdfNumeric len = lenobj.PdfObject as PdfNumeric;
				length = int.Parse(len.ToString());
			}
			else
			{
				length = int.Parse(lengthObject.ToString());
			}

			Data = lexer.ReadBytes(length);
			lexer.Expects("endstream");

			StreamDictionary = dictionary;
		}

		public PdfObject StreamDictionary
		{
			get;
			private set;
		}

		public byte[] Data
		{
			get;
			private set;
		}
	}
}
