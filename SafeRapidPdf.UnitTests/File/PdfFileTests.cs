using System;
using SafeRapidPdf.Objects;
using SafeRapidPdf.Parsing;
using SafeRapidPdf.UnitTests.Util;

using Xunit;

namespace SafeRapidPdf.UnitTests.File
{
    public class PdfFileTests
    {
        [Theory]
        [InlineData(
            """
            %PDF-
            trailer<</Root<</Pages<<>>>>>>
            %%EOF
            """
        )]
        public void Parsing_TinyFile(string pdf)
        {
            var r = PdfFile.Parse(pdf.ToStream());
            Assert.True(r.Items.Count == 3);
        }

        [Theory]
        [InlineData(
            """
            %PDF-
            trailer<</Root<</Pages<<>>>>>>
            """
        )]
        public void Parsing_TinyFile_Without_EOF_YieldsException(string pdf)
        {
            var exception = Assert.Throws<ParsingException>(() =>
                {
                    PdfFile.Parse(pdf.ToStream());
                });
            Assert.Equal("End of file reached without EOF marker", exception.Message);
        }

        [Theory]
        [InlineData(
            """
            "Not a PDF"
            """
        )]
        public void Parsing_Non_Pdf_Yields_Exception(string pdf)
        {
            _ = Assert.Throws<UnexpectedTokenException>(() =>
              {
                  PdfFile.Parse(pdf.ToStream());
              });
        }
    }
}
