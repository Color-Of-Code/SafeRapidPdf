using System;
using Xunit;

using SafeRapidPdf.File;
using SafeRapidPdf.UnitTests.Util;

namespace SafeRapidPdf.UnitTests.File
{
    public class PdfXRefTests
    {
        [Fact]
        public void Parsing_Uncompressed_XRef()
        {
            var r = PdfXRef.Parse(@"0 6
0000000000 65535 f 
0000000016 00000 n 
0000000051 00000 n 
0000000109 00000 n 
0000000281 00000 n 
0000000385 00000 n 
".ToLexer());
            // 1 section
            Assert.Equal(1, r.Items.Count);
            var s = r.Items[0] as PdfXRefSection;
            // 6 entries
            Assert.Equal(6, s.Items.Count);
        }

        [Fact]
        public void Parsing_CompressedXRef()
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
            // this yields now a decoded buffer that needs to get decoded further using PNG algos
            //predictor W[1 2 1] (4 columns) -> decoded properly?
            // the order of flags is strange but the resulting offsets are valid offsets of objects in PDF!
            //02 01 00 10 00 -> 01 00 10 00 -> int 1 16 0
            //02 00 03 8d 00 -> 01 03 9d 00 -> int 1 925 0
            //02 00 00 55 00 -> 01 03 f2 00 -> int 1 1010 0
            //02 00 01 b1 00 -> 01 04 a3 00 -> int 1 1187 0
            //02 00 01 21 00 -> 01 05 c4 00 -> int 1 1475 0
            //02 00 01 a5 00 -> 01 06 69 00 -> int 1 1641 0
            //02 00 0a 85 00 -> 01 10 ee 00 -> int 1 4334 0
            //02 00 04 bc 00 -> 01 14 aa 00 -> int 1 5290 0
            //02 00 ec ca 00 -> 01 00 74 00 -> int 1 116 0
            //02 01 02 4e 00 -> 02 02 c2 00 -> int 2 706 0
            //02 00 00 00 01 -> 02 02 c2 01 -> int 2 706 1
            //02 00 00 00 01 -> 02 02 c2 02 -> int 2 706 2
            //02 00 00 00 01 -> 02 02 c2 03 -> int 2 706 3
            //02 ff ff 12 fd -> 01 01 d4 00 -> int 1 468 0 // is this the CRC?
            //                              -> int flag offset generation (why is this different from uncompressed variant...??)
            //                              -> looks like 1 means in use, 2 free here.

/* Right offsets for the objects:
00703: 0000000016 00000 n 
00704: 0000000925 00000 n 
00705: 0000001010 00000 n 
00706: 0000001187 00000 n 
00707: 0000001476 00000 n 
00708: 0000001641 00000 n 
00709: 0000004334 00000 n 
00710: 0000005290 00000 n 
00711: 0000000116 00000 n 
00712: 0000000706 00000 o 
00713: 0000000706 00001 o 
00714: 0000000706 00002 o 
00715: 0000000706 00003 o 
00716: 0000000468 00000 n
*/
            var xref = PdfXRef.Parse(xrefStream);
        }

    }
}