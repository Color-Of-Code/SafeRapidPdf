using System;

using SafeRapidPdf.Objects;
using SafeRapidPdf.Parsing;
using SafeRapidPdf.UnitTests.Util;

using Xunit;

namespace SafeRapidPdf.UnitTests.File
{
    public class PdfFileTests
    {
        [Fact]
        public void Parsing_TinyFile()
        {
            var r = PdfFile.Parse(@"%PDF-
trailer<</Root<</Pages<<>>>>>>
%%EOF".ToStream());
            Assert.True(r.Items.Count == 3);
        }

        [Fact]
        public void Parsing_TinyFile_Without_EOF_YieldsException()
        {
            var exception = Assert.Throws<ParsingException>(() => { PdfFile.Parse(@"%PDF-
trailer<</Root<</Pages<<>>>>>>".ToStream()); });
            Assert.Equal("End of file reached without EOF marker", exception.Message);
        }

        [Fact]
        public void Parsing_Non_Pdf_Yields_Exception()
        {
            Assert.Throws<UnexpectedTokenException>(() =>
            {
                PdfFile.Parse("Not a PDF".ToStream());
            });
        }
    }
}
