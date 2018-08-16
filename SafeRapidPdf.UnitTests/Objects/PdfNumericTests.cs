using SafeRapidPdf.Objects;
using Xunit;

namespace SafeRapidPdf.UnitTests.Objects
{
    public class PdfNumericTests
    {
        [Fact]
        public void CastTests()
        {
            Assert.Equal(1L,  PdfNumeric.Parse("1"));
            Assert.Equal(1.1d, PdfNumeric.Parse("1.1"));
            Assert.Equal(1.1m, PdfNumeric.Parse("1.1"));
        }

        [Fact]
        public void IsRealTests()
        {
            Assert.True(PdfNumeric.Parse("1.1").IsReal);
        }

        [Fact]
        public void IsIntergerTests()
        {
            Assert.True(PdfNumeric.Parse("1").IsInteger);
        }
    }
}
