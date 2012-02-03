﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

using SafeRapidPdf.Primitives;

namespace SafeRapidPdf.Pdf
{
	public class FileStructure
	{
		public FileStructure(ReadOnlyCollection<PdfObject> objects)
		{
			Objects = objects;
		}

		public static Pdf.FileStructure Parse(String pdfFilePath)
		{
			using (Stream reader = File.Open(pdfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				var lexer = new Lexical.LexicalParser(reader);

				List<PdfObject> objects = new List<PdfObject>();

				// check that this stuff is really looking like a PDF
				PdfComment comment = new PdfComment(lexer);
				if (comment == null || !comment.Text.StartsWith("%PDF-"))
					throw new Exception("PDF header missing");
				objects.Add(comment);

				bool lastObjectWasOEF = false;
				while (true)
				{
					var obj = PdfObject.Parse(lexer);

					if (obj == null)
					{
						if (lastObjectWasOEF)
							break;
						else
							throw new Exception("End of file reached without EOF marker");
					}

					if (obj is PdfIndirectObject)
						lexer.IndirectReferenceResolver.InsertObject(obj as PdfIndirectObject);

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
				return new Pdf.FileStructure(objects.AsReadOnly()); ;
			}
		}

		public String Version
		{
			get
			{
				return Objects.First().ToString();
			}
		}

		public ReadOnlyCollection<PdfObject> Objects { get; private set; }
	}
}
