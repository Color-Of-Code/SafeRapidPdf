using SafeRapidPdf.Objects;
using Xunit;

namespace SafeRapidPdf.UnitTests.Objects
{
    public class PdfNumericTests
    {
        [Fact]
        public void ToInt32Tests()
        {
            Assert.Equal(1,    PdfNumeric.Parse("1").ToInt32());
            Assert.Equal(1000, PdfNumeric.Parse("1000").ToInt32());
        }

        [Fact]
        public void CanCastImplictlyToDouble()
        {
            Assert.Equal(1.1d, PdfNumeric.Parse("1.1"));
            Assert.Equal(1000d, PdfNumeric.Parse("1000"));
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
