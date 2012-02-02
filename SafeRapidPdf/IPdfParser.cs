using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf
{
	public interface IPdfParser
	{
		/// <summary>
		/// Parse the document
		/// </summary>
		/// <returns></returns>
		Pdf.Document Parse();

		/// <summary>
		/// Read next PDF object in the stream
		/// Is equivalent to ReadPdfObject(ReadToken());
		/// </summary>
		/// <returns></returns>
		Primitives.PdfObject ReadPdfObject();

		/// <summary>
		/// Read next token
		/// </summary>
		/// <returns></returns>
		String ReadToken();

		/// <summary>
		/// Read next PDF object in the stream
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		Primitives.PdfObject ReadPdfObject(String token);

		Primitives.PdfIndirectObject GetObject(int objNumber, int genNumber);

		byte[] ReadBytes(int length);

		char ReadChar();

		long Position { get; set; }
	}
}
