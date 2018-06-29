using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

//using ComponentAce.Compression.Libs.zlib;

namespace SafeRapidPdf.File
{
    public sealed class PdfStream : PdfObject
	{
		private PdfStream(PdfDictionary dictionary, PdfData data)
			: base(PdfObjectType.Stream)
		{
			IsContainer = true;
			StreamDictionary = dictionary;
			Data = data;

			//IPdfObject filter = StreamDictionary["Filter"];
			//if (filter.Text == "FlateDecode")
			//{
			//    ZInputStream zin = new ZInputStream(new MemoryStream(Data));
			//    //zin.
			//    int r;
			//    IList<char> output = new List<char>(data.Length*10);
			//    while ((r = zin.Read()) != -1)
			//    {
			//        output.Add((char)r);
			//    }
			//    char[] decompressed = output.ToArray();
			//    String test = new String(decompressed);
			//    zin.Close();
			//}
			//else if (filter.Text == "DCTDecode")
			//{
			//    // JPEG image
			//}
			//else
			//    throw new NotImplementedException("Implement Filter");
		}

		public static PdfStream Parse(PdfDictionary dictionary, Lexical.ILexer lexer)
		{
			lexer.Expects("stream");
			char eol = lexer.ReadChar();
			if (eol == '\r')
				eol = lexer.ReadChar();
			if (eol != '\n')
				throw new Exception(@"Parser error: stream needs to be followed by either \r\n or \n alone");

			if (dictionary == null)
				throw new Exception("Parser error: stream needs a dictionary");

			IPdfObject lengthObject = dictionary["Length"];
			if (lengthObject == null)
				throw new Exception("Parser error: stream dictionary requires a Length entry");

			int length = 0;
			if (lengthObject is PdfIndirectReference)
			{
				PdfIndirectReference reference = lengthObject as PdfIndirectReference;
				
				PdfIndirectObject lenobj = lexer.IndirectReferenceResolver
					.GetObject(reference.ObjectNumber, reference.GenerationNumber);

				PdfNumeric len = lenobj.Object as PdfNumeric;
				length = int.Parse(len.ToString());
			}
			else
			{
				length = int.Parse(lengthObject.ToString());
			}

			PdfData data = PdfData.Parse(lexer, length);
			lexer.Expects("endstream");

			return new PdfStream(dictionary, data);
		}

		public PdfDictionary StreamDictionary { get; private set; }

		public PdfData Data { get; private set; }

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.AddRange(StreamDictionary.Items);
				list.Add(Data);
				return list.AsReadOnly();
			}
		}

		public override string ToString()
		{
			return "stream";
		}
	}
}