﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Primitives
{
	public class PdfXRef : PdfObject
	{
		public PdfXRef(IFileStructureParser parser)
		{
			int firstid = int.Parse(parser.ReadToken());
			int size = int.Parse(parser.ReadToken());
			for (int i = 0; i < size; i++)
			{
				string offsetS = parser.ReadToken();
				if (offsetS.Length != 10)
					throw new Exception("Parser error: 10 digits expected for offset in xref");
				long offset = long.Parse(offsetS);

				string generationS = parser.ReadToken();
				if (generationS.Length != 5)
					throw new Exception("Parser error: 5 digits expected for generation in xref");
				int generation = int.Parse(generationS);

				string inuse = parser.ReadToken();
				if (inuse != "f" && inuse != "n")
					throw new Exception("Parser error: only 'f' and 'n' are valid flags in xref");
				if (inuse == "n")
				{
					String key = BuildKey(i + firstid, generation);
					_offsets.Add(key, offset);
				}
			}
		}

		public long GetOffset(int objectNumber, int generationNumber)
		{
			String key = BuildKey(objectNumber, generationNumber);
			return _offsets[key];
		}

		public static String BuildKey(int objectNumber, int generationNumber)
		{
			return String.Format("{0:0000000000}_{1:00000}", objectNumber, generationNumber);
		}

		private Dictionary<String, long> _offsets = new Dictionary<string, long>();
	}
}
