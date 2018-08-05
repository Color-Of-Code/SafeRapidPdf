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
            //02 ff ff 12 fd -> 01 01 d4 00 -> int 1 468 0 

            // Meaning of types and fields within an xref stream
            // type  field
            // 0     0 = f
            //       2 -> object number of next free object
            //       3 -> generation number (if used again)
            // 1     1 = n (uncompressed)
            //       2 -> byte offset in file
            //       3 -> generation number
            // 2     1 = n (compressed)
            //       2 -> object number where the data is stored
            //       3 -> index of object in the stream

            // needed to resolve the values for refs encoded with 2
            var base64Object706 = @"NzA2IDAgb2JqDTw8L0ZpbHRlci9GbGF0ZURlY29kZS9GaXJzdCAzMC9MZW5ndGggMTkzL04gNC9U
eXBlL09ialN0bT4+c3RyZWFtDQpo3kSOwQ6CMBBEf2W/wG0BARPSRFAJBwKxHkwIh1qrUcES6EH/
3gIaTzO78zKZgDpAIKAuUOpZ9YC6rtUlUD+EKMJEN7rnnZBqPAZ/YgnsrQ8n3nrGcPsyKTfCjFTK
6dQwJ2WvJVemwnKzw6wVVxXXeCxOdyWNhbN2hMkMM1ZhliSxGNQZAhKO37pCrjrRC3PTT4wbIR+/
DRZZTYjtKUq4iGZQ1uRAka+/J8+BLIiDh3en/itRd3PO2EeAAQDI4UbhDQplbmRzdHJlYW0NZW5k
b2JqDQ==";

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
            // this contains the rest of the refs
            var base64Object559 = @"NTU5IDAgb2JqDTw8L0RlY29kZVBhcm1zPDwvQ29sdW1ucyA1L1ByZWRpY3RvciAxMj4+L0ZpbHRl
ci9GbGF0ZURlY29kZS9JRFs8ODgyNkQ3RkI0OUQ2RDVBMDM5QTY1MTU0RjMwOUMyQUI+PDk5RjA0
OTNGQjk3MjI1NEY4N0I5MzA3NTYxNzRDRjk5Pl0vSW5mbyA3MDIgMCBSL0xlbmd0aCAxNTQ2L1Jv
b3QgNzA0IDAgUi9TaXplIDcwMy9UeXBlL1hSZWYvV1sxIDMgMV0+PnN0cmVhbQ0KaN7sl2lsVVUQ
x+855769rxstLaGlaHFBlKUIQsoSQC2C0igmiGiCKShScQcEUYIYYqIsSuIWEUvEWEMrYixqgsao
RMVIsKjgRkoENSqicaks1s7vkDf3iyR85334ZzJn7sycWc+zgfyssX0PBzYITItgMBn6fuUUgfZK
OJXILBEcNBHOasG6BuiFgtVrBQuTgrGrkH9ZMN6/G832aiR/Vr6d0o1u51z434FefoOgW47MA/Lt
1HGC478QTvZu9H8sOPgRON7DQtXsLu7G2LYV3ZjoEBm7vl00tO2X05lFgj1f4Kvjajf5S85i4O6D
vgda0GSGCI7mFpkqoQ+tgb5QMDFcOFsnwRkMB4tH70V/P7ViLTpHdWM4eT38w3oa7tHTOmJljkW+
/R68BJyHJLby8CpvChrEt7AP+TL/RDQTPdsoXrnZZCpGJGcJpyODZCwn7/I7hG6pkNOD4o85ch2n
/6rO2FR8oCpsDYiMuzbiIVHK+1Q07MX/xGd8y00bJDImdRmcLYJbiE+GukosxtaLZLZVJOf72hsL
voLMeE7hBLWCG8nFQOyWzRBs3C2chUboO36icvZqNZo/BdNfCpZWYF3yaCo2Ct3rcsE+i5C8CCve
rr/1XdzxFrXuboMmAhn6YtXD0L7O3xbNi5+hbn12nHBW30CUkJzka3KA2orH0bw00qd86xbp3c2H
+IMVezP6z4DPfU29WNk3E/77cLaDdcJvvx7rQ4VOjsNb0WY6qTRTq9GONWtm7Xnw/d2HwemCfg3+
BuWHo/mW+eCyObuBeV2w4BxOv0FyBHypxvQMPDFDItbx0z7ILcS6vVHq3zQdFE7rpZEa9vIlyPuv
vB7fQb4f6W7zjnqV7STa+2R6rJPaMwd+JBpk2TJzQuZGsJVvm3ViGCTTElWzcppg9unclDuRxxFz
sEW9WT9tAq1w25fTp+AMB6tUv30W7AGO5JTpZ9FsVuq8sn6ytYF0sfkkMmf2RLJPr9mzBXtQXZYe
MeSigG9DIhMy5cJZRKOc+m9EvpSI7eL0D/w/BP8l4r8JbWM4pb9Cqtfu1zkWNEdOn9PbOerW0aGu
FT2+ur6iVqlw8yYcqSITevkj4BP0bE/B6Wyx/g8JVtBTldeI/AKfi2n4TzSqF0fsMjfcu/jjZ7L3
kCpy1I97VT0MqVW7DZpKcGAKb5NMAyf9FeTjefgk6CcGE6xkFTGXaWPSZfDxMByrU9RLhldDDwR9
rM4FL+CUenM+F9S/wwdHBl2bRslwd/OtZjxGHSZbtHfCUWSc3bEEnb1lwpgCNsgEpmj2JnQOUPkw
Bacr4gMc+znRZmK4v6EPQPeH5tRRpTGf91+hH41oZtcnplMDPnqbNBcn+pGOtu/BeQy6STsoLx9E
PhwUiaT3mQng2LmuHWQap86CXpCbLaZqM/Tv8JmEQaT7LK8gS2zDPuSduxTjTyGTOU02XaHufbuD
OJD9+Gadh3HeQkl2RKIADtGOUW+uVN9sjm+rlwkO+Fr8nLAb/p3w2W4VP5BTYu7okfgH0CuwgmSc
WW3JwhjeQs6/DPeBzIQEkUzhW8rXHpFMLWOP3IoV7tuD2ivCSskV3B2f3Ufs6OeJCe+6dD0a0BN/
HBleC2lmXRn7IuTVkfQvAZntpqEp9+oLSpgqxVRdAZsiNVtflfYtkCwn6OgEfsZ36+vRbyU/W7LM
OjdL95eftymftfl43lvfpSF9WnW+eFXJLu7HG6+KCVMOXcS0TDD/a5iN+cyHOHdMk4WQvZmR6jJn
NqjmuN9lZDDRoH104kX9W6QLmFQ19dpfJdw9w3wonaPxd3ji/uLbN6Dz0On/BSBve/EWIobJZt1Z
fpf57RaOj7wNmAn56E8elU09kV1mUrqdQ95XdqlG2N6e+99havl/kZUMmnk7tGKHkmXDG76BCjFM
v4Asj2RrD1urbw83V2mPCb+7mQ8B9VnAtDf0rOm+u2natZNTHwfZk6eCYSfIt7yrA24d4HMQi2Aa
xG7AGy+gHwOqNygG6eigNCJZru8rszzCz4voPFWfT+NpPDmadSc9Paa07fofmeP/CTAAUeNQwA0K
ZW5kc3RyZWFtDWVuZG9iag0=";

            var xref = PdfXRef.Parse(xrefStream);
        }

    }
}