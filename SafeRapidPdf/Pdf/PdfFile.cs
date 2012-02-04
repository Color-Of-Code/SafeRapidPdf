using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

using SafeRapidPdf.Primitives;

namespace SafeRapidPdf.Pdf
{
	public class PdfFile : IPdfObject, IIndirectReferenceResolver
	{
		private PdfFile(ReadOnlyCollection<IPdfObject> objects)
		{
			Items = objects;

			// build up the fast object lookup dictionary
			_indirectObjects = new Dictionary<String, PdfIndirectObject>();
			foreach (var obj in Items.OfType<PdfIndirectObject>())
				InsertObject(obj);
		}

		public static Pdf.PdfFile Parse(String pdfFilePath)
		{
			using (Stream reader = File.Open(pdfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var lexer = new Lexical.LexicalParser(reader);

				List<IPdfObject> objects = new List<IPdfObject>();

				// check that this stuff is really looking like a PDF
				PdfComment comment = PdfComment.Parse(lexer);
				if (comment == null || !comment.Text.StartsWith("%PDF-"))
					throw new Exception("PDF header missing");
				objects.Add(comment);

				bool lastObjectWasOEF = false;
				while (true)
				{
					var obj = PdfObject.ParseAny(lexer);

					if (obj == null)
					{
						if (lastObjectWasOEF)
							break;
						else
							throw new Exception("End of file reached without EOF marker");
					}

					objects.Add(obj);

					lastObjectWasOEF = false;
					if (obj is PdfComment)
					{
						PdfComment cmt = obj as PdfComment;
						if (cmt.IsEOF)
						{
							// a linearized or updated document might contain several EOF markers
							lastObjectWasOEF = true;
						}
					}
				}
				return new Pdf.PdfFile(objects.AsReadOnly()); ;
			}
		}

		public String Version
		{
			get
			{
				return Items.First().ToString();
			}
		}

		public ReadOnlyCollection<IPdfObject> Items { get; private set; }

		public string Text
		{
			get { return "File"; }
		}

		public bool IsContainer
		{
			get { return true; }
		}

		private IDictionary<String, PdfIndirectObject> _indirectObjects;

		private void InsertObject(PdfIndirectObject obj)
		{
			if (obj == null)
				throw new Exception("This object must be an indirect object");
			String key = PdfXRef.BuildKey(obj.ObjectNumber, obj.GenerationNumber);
			_indirectObjects[key] = obj;
		}

		public PdfIndirectObject GetObject(int objectNumber, int generationNumber)
		{
			String key = PdfXRef.BuildKey(objectNumber, generationNumber);
			return _indirectObjects[key];
		}
	}
}
