namespace SafeRapidPdf.UnitTests.File
{
    using System.IO;
    using System.Linq;
    using SafeRapidPdf.Document;
    using SafeRapidPdf.Objects;

    using Xunit;

    public class PdfDocumentTests
    {
        [Fact]
        public void CanExtractPages()
        {
            var file = PdfFile.Parse(GetTestDataFilePath("1.pdf"));

            Assert.Equal("%PDF-1.3", file.Version.ToString());
            Assert.Equal(PdfObjectType.File, file.ObjectType);

            var pdf = new PdfDocument(file);

            var pages = pdf.GetPages().ToArray();

            Assert.Equal(3, pages.Length);

            Assert.Null(pages[0].ArtBox);
            Assert.Null(pages[0].BleedBox);
            Assert.Null(pages[0].CropBox);
            Assert.Equal("MediaBox [0; 0; 612; 792]", pages[0].MediaBox.ToString());
            Assert.Null(pages[0].Rotate);
        }

        private static string GetTestDataFilePath(string name)
        {
            var baseDirectory = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent;

            return Path.Combine(baseDirectory.FullName, "testdata", name);
        }
    }
}