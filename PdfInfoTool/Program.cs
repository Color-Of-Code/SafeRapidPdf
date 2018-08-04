using System;
using SafeRapidPdf;
using SafeRapidPdf.File;

namespace PdfInfoTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var file = PdfFile.Parse(args[0]);
                Console.WriteLine("PDF Version: {0}", file.Version);
                foreach (var item in file.Items)
                {
                    var type = item.ObjectType;
                    if (item.ObjectType == PdfObjectType.IndirectObject)
                    {
                        var iobj = item as PdfIndirectObject;
                        type = iobj.Object.ObjectType;
                    }
                    Console.WriteLine(" - {0}: {1}", item, type);
                }
            }
            else
                Console.WriteLine("Usage: PdfInfoTool <path to pdf>");
        }
    }
}
