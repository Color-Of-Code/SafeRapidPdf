using System;
using Xunit;

using SafeRapidPdf.File;
using SafeRapidPdf.UnitTests.Util;

namespace SafeRapidPdf.UnitTests.File
{
    public class PdfXRefTests
    {
        [Fact]
        public void Parsing_Uncompressed_XRef()
        {
            var r = PdfXRef.Parse(@"0 6
0000000000 65535 f 
0000000016 00000 n 
0000000051 00000 n 
0000000109 00000 n 
0000000281 00000 n 
0000000385 00000 n 
".ToLexer());
            // 1 section
            Assert.Equal(1, r.Items.Count);
            var s = r.Items[0] as PdfXRefSection;
            // 6 entries
            Assert.Equal(6, s.Items.Count);
        }

        [Fact]
        public void Parsing_CompressedXRef()
        {
        }

    }
}