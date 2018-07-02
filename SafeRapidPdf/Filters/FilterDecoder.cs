using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace SafeRapidPdf.Filters 
{
	using SafeRapidPdf.File;

	public sealed class FilterDecoder
	{
		/// <summary>
		/// Decodes the input buffer and returns a decoded output buffer
		/// </summary>
		/// <param name="input"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static byte[] Decode(byte[] input, PdfDictionary options)
        {
            var filter = options["Filter"];
            switch (filter.Text)
            {
                case "FlateDecode":
                    return FlateDecode(input);
                default:
                    throw new NotImplementedException("Filter: " + filter);
            }
        }

        public static byte[] Decode(PdfStream stream)
        {
            return Decode(stream.Data.Data, stream.StreamDictionary);
        }

        private static byte[] FlateDecode(byte[] input)
        {
            var mi = new MemoryStream(input);
            //See: http://george.chiramattel.com/blog/2007/09/deflatestream-block-length-does-not-match.html
            mi.ReadByte();
            mi.ReadByte();
            var s = new DeflateStream(mi, CompressionMode.Decompress);
            var output = new Byte[64*1024];
            var mo = new MemoryStream(output);
            s.CopyTo(mo);
            s.Close();
            throw new NotImplementedException("Work in progress");
        }
    }
}
