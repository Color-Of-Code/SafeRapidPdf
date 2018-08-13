namespace SafeRapidPdf.UnitTests.File
{
    using System;

    using SafeRapidPdf.Objects;
    using SafeRapidPdf.UnitTests.Util;

    using Xunit;

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
            var exception = Assert.Throws<Exception>(() => { PdfFile.Parse(@"%PDF-
trailer<</Root<</Pages<<>>>>>>".ToStream()); });
            Assert.Equal("End of file reached without EOF marker", exception.Message);
        }

        [Fact]
        public void Parsing_Non_Pdf_Yields_Exception()
        {
            Assert.Throws<Exception>(() =>
            {
                PdfFile.Parse("Not a PDF".ToStream());
            });
        }
    }
}