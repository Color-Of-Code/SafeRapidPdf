using System.IO;
using System.Linq;
using SafeRapidPdf.Objects;

using Xunit;

namespace SafeRapidPdf.UnitTests.File
{
    public class PdfDocumentTests
    {
        [Fact]
        public void CanAccessThroughLoad()
        {
            using var stream = System.IO.File.OpenRead(GetTestDataFilePath("3.pdf")); // "%PDF-1.5

            var pdf = PdfDocument.Load(stream);

            var pages = pdf.GetPages().ToArray();

            var page = Assert.Single(pages);

            Assert.Equal(10, page.Items.Count);

            Assert.Equal("ArtBox [0.0; 0.0; 1920.0; 1080.0]", page.ArtBox.ToString());
            Assert.Equal("BleedBox [0.0; 0.0; 1920.0; 1080.0]", page.BleedBox.ToString());
            Assert.Null(page.CropBox);
            Assert.Equal("MediaBox [0.0; 0.0; 1920.0; 1080.0]", page.MediaBox.ToString());
            Assert.Null(page.Rotate);
        }

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

            var mediaBox = pages[0].MediaBox.ToPixels();

            Assert.Equal(0, mediaBox.X);
            Assert.Equal(0, mediaBox.Y);
            Assert.Equal(816, mediaBox.Width);
            Assert.Equal(1056, mediaBox.Height);
        }

        private static string GetTestDataFilePath(string name)
        {
            var baseDirectory = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent;

            return Path.Combine(baseDirectory.FullName, "testdata", name);
        }
    }
}
