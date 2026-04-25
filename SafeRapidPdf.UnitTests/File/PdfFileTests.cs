using SafeRapidPdf.Objects;
using SafeRapidPdf.Parsing;
using SafeRapidPdf.UnitTests.Util;

using Xunit;

namespace SafeRapidPdf.UnitTests.File;

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
        Assert.Equal(3, r.Items.Count);
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
                _ = PdfFile.Parse(pdf.ToStream());
            });
        Assert.Equal("End of file reached without EOF marker", exception.Message);
    }

    [Theory]
    [InlineData(
        """
        Not a PDF
        """
    )]
    public void Parsing_Non_Pdf_Yields_Exception(string pdf)
    {
        _ = Assert.Throws<UnexpectedTokenException>(() =>
          {
              _ = PdfFile.Parse(pdf.ToStream());
          });
    }
}
