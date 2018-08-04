using System;
using SafeRapidPdf.File;

namespace PdfInfoTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                var objects = PdfFile.Parse(args[0]);
            }
            else
                Console.WriteLine("Usage: PdfInfoTool <path to pdf>");
        }
    }
}
