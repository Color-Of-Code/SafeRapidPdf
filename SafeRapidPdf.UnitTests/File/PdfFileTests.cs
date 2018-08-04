using System;
using Xunit;

using SafeRapidPdf.File;
using SafeRapidPdf.UnitTests.Util;

namespace SafeRapidPdf.UnitTests.File
{
    public class PdfFileTests
    {
        [Fact]
        public void Parsing_TinyFile()
        {
            PdfFile.Parse("%PDF-trailer<</Root<</Pages<<>>>>>>".ToStream());
        }

        [Fact]
        public void Parsing_Non_Pdf_Yields_Exception()
        {
            Assert.Throws<Exception>(() => {
                PdfFile.Parse("Not a PDF".ToStream());
            });
        }
    }
}