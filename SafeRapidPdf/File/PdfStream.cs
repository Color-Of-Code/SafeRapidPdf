using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ComponentAce.Compression.Libs.zlib;

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
		}

		public Byte[] Decode()
		{
			//TODO: multiple filter in order can be specified
			IPdfObject filter = StreamDictionary["Filter"];
			if (filter.Text == "FlateDecode")
			{
				PdfDictionary parameters = StreamDictionary["DecodeParms"] as PdfDictionary;
				var samplesPerRow = parameters["Columns"] as PdfNumeric;
				//12 = PNG prediction (on encoding, PNG Up on all rows)
				var predictor = parameters["Predictor"] as PdfNumeric;

			    var zin = new ZInputStream(new MemoryStream(Data.Data));
			    int r;
			    var output = new List<Byte>(2000);
			    while ((r = zin.Read()) != -1)
			    {
			        output.Add((Byte)r);
			    }
			    byte[] decompressed = output.ToArray();
				zin.Close();

				// oh well now we have to handle the predictors...

				return decompressed;
			}
			//else if (filter.Text == "DCTDecode")
			//{
			//    // JPEG image
			//}
			//else
			throw new NotImplementedException("Implement Filter: " + filter.Text);
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
			if (lengthObject is PdfIndirectReference reference)
			{				
				PdfIndirectObject lenobj = lexer.IndirectReferenceResolver
					.GetObject(reference.ObjectNumber, reference.GenerationNumber);

				PdfNumeric len = (PdfNumeric)lenobj.Object;
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

		public PdfDictionary StreamDictionary { get; }

		public PdfData Data { get; }

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