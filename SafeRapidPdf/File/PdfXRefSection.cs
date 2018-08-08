﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SafeRapidPdf.File
{
    public sealed class PdfXRefSection : PdfObject
	{
		private PdfXRefSection(int firstId, int size, List<IPdfObject> entries)
			: base(PdfObjectType.XRefSection)
		{
			IsContainer = true;

			FirstId = firstId;
			Size = size;
			_entries = entries;
		}

		public static PdfXRefSection Parse(PdfStream pdfStream)
		{
			var dictionary = pdfStream.StreamDictionary;
			// W[1 2 1] (4 columns) 
			// W[1 3 1] (5 columns, larger indexes)
			var w = dictionary["W"] as PdfArray;
			var index = dictionary["Index"] as PdfArray;
			var firstId = (int)(index.Items[0] as PdfNumeric).Value;
			var size = (int)(index.Items[1] as PdfNumeric).Value;
			int items = w.Items.Count;
			// for xref this shall always be 3
			if (items != 3)
				throw new Exception("The W[] parameter must contain 3 columns for an XRef");
			int[] sizes = new int[w.Items.Count];
			int bytesPerEntry = 0;
			for (int i = 0; i < items; i++)
			{
				sizes[i] = (int)(w.Items[i] as PdfNumeric).Value;
				bytesPerEntry += sizes[i];
			}
			var decodedXRef = pdfStream.Decode();
			// Use W[...] to build up the xref
			int rowCount = decodedXRef.Length / bytesPerEntry;
			if (size != rowCount)
				throw new Exception("The number of refs inside the Index value must match the actual refs count present in the stream");
			var entries = new List<IPdfObject>(rowCount);
			for (int row = 0; row < rowCount; row++)
			{
				var entry = PdfXRefEntry.Parse(firstId + row, decodedXRef, sizes, row, bytesPerEntry);
				entries.Add(entry);
			}
			return new PdfXRefSection(firstId, size, entries);
		}

		public static PdfXRefSection Parse(Lexical.ILexer lexer)
		{
			int firstId = int.Parse(lexer.ReadToken());
			int size = int.Parse(lexer.ReadToken());
			var entries = new List<IPdfObject>(size);
			for (int i = 0; i < size; i++)
			{
				var entry = PdfXRefEntry.Parse(firstId + i, lexer);
				// first entry must be free and have a gen 65535
				// = head of the linked list of free objects
				if (i == 0)
				{
					if (entry.GenerationNumber != 65535)
						throw new Exception("The first xref entry must have generation number 65535");
					if (entry.InUse)
						throw new Exception("The first xref entry must be free");
				}
				entries.Add(entry);
			}
			return new PdfXRefSection(firstId, size, entries);
		}

		public int FirstId { get; }

		public int Size { get; }

		private List<IPdfObject> _entries;
		public override ReadOnlyCollection<IPdfObject> Items 
		{
            get => _entries.AsReadOnly();
        }

		public override string ToString()
		{
            return $"{FirstId} {Size}";
		}
	}
}