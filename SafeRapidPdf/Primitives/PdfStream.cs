using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

//using ComponentAce.Compression.Libs.zlib;

namespace SafeRapidPdf.Primitives
{
	public class PdfStream : PdfObject
	{
		private PdfStream(PdfDictionary dictionary, byte[] data)
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
			lexer.SkipEol(); // position to begin of stream data

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

			var data = lexer.ReadBytes(length);
			lexer.Expects("endstream");

			return new PdfStream(dictionary, data);
		}

		public PdfDictionary StreamDictionary
		{
			get;
			private set;
		}

		public byte[] Data
		{
			get;
			private set;
		}

		public override ReadOnlyCollection<IPdfObject> Items
		{
			get
			{
				var list = new List<IPdfObject>();
				list.Add(StreamDictionary);
				return list.AsReadOnly();
			}
		}

		public override string ToString()
		{
			return "stream";
		}
	}
}
