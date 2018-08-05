using System;
using SafeRapidPdf.File;
using SafeRapidPdf.UnitTests.Util;
using Xunit;

namespace SafeRapidPdf.UnitTests.File
{
    public class PdfStreamTests
    {
        [Fact]
        public void Decode_FlateDecode_PngPredictor_Up()
        {
            //Contents:
            // 711 0 obj
            // <</DecodeParms
            //  <</Columns 4/Predictor 12>>
            //  /Filter /FlateDecode
            //  /ID[<8826D7FB49D6D5A039A65154F309C2AB><99F0493FB972254F87B930756174CF99>]
            //  /Index[703 14]
            //  /Info 702 0 R
            //  /Length 58
            //  /Prev 6238004
            //  /Root 704 0 R
            //  /Size 717
            //  /Type /XRef
            //  /W[1 2 1]>>
            // stream ...
            //endstream
            //endobj
            var base64XrefStream =
@"NzExIDAgb2JqDTw8L0RlY29kZVBhcm1zPDwvQ29sdW1ucyA0L1ByZWRpY3RvciAxMj4+L0ZpbHRl
ci9GbGF0ZURlY29kZS9JRFs8ODgyNkQ3RkI0OUQ2RDVBMDM5QTY1MTU0RjMwOUMyQUI+PDk5RjA0
OTNGQjk3MjI1NEY4N0I5MzA3NTYxNzRDRjk5Pl0vSW5kZXhbNzAzIDE0XS9JbmZvIDcwMiAwIFIv
TGVuZ3RoIDU4L1ByZXYgNjIzODAwNC9Sb290IDcwNCAwIFIvU2l6ZSA3MTcvVHlwZS9YUmVmL1db
MSAyIDFdPj5zdHJlYW0NCmjeYmJkEGBgYmDuBRIMoUCCcSOIUAQRS4EEVyuQYNkDJN6cYmBiZPID
qWNgRCL+/xf6CxBgAO9WCPMNCmVuZHN0cmVhbQ1lbmRvYmo=";
            var xrefStream = PdfObject.ParseAny(base64XrefStream.Base64ToLexer()) as PdfIndirectObject;
            PdfStream pdfStream = xrefStream.Object as PdfStream;
            var data = pdfStream.Decode();

            String hex = data.ToHexString();
            // known good result:
            Assert.Equal("0100100001039d000103f2000104a3000105c400010669000110ee000114aa00010074000202c2000202c2010202c2020202c2030101d400", hex);
        }
    }
}