﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf 
{
	public enum PdfObjectType 
	{
		// low level objects
		Null,
		Boolean,
		Comment,
		Numeric,
		HexadecimalString,
		LiteralString,
		Name,
		Data,

		IndirectObject,
		IndirectReference,

		Array,
		Dictionary,
		KeyValuePair,

		Stream,

		XRef,
		XRefSection,
		XRefEntry,

		Trailer,

		StartXRef,

		// high level objects
		File,
		Document,
		Catalog,
		PageTree,
		Page
	}
}
