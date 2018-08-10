using System;
using CommandLine;
using SafeRapidPdf;
using SafeRapidPdf.File;

namespace PdfInfoTool
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<DumpOptions, ShowOptions>(args)
              .MapResult(
                (DumpOptions opts) => RunDumpAndReturnExitCode(opts),
                (ShowOptions opts) => RunShowAndReturnExitCode(opts),
                _ => 1);
        }

        private static int RunShowAndReturnExitCode(ShowOptions opts)
        {
            var file = PdfFile.Parse(opts.FileName);
            Console.WriteLine("PDF Version: {0}", file.Version);

            if (opts.What == "xref")
            {
                // this is the information coming from the interpreted xref
                var xref = file.XRef;
                foreach (var section in xref.Items)
                {
                    foreach (var entry in section.Items)
                    {
                        var xrefEntry = entry as PdfXRefEntry;
                        var obj = xrefEntry.ObjectNumber.ToString("D5");
                        Console.WriteLine($"{obj}: {entry}");
                    }
                }
            }
            // the information coming from the objects itselves
            /*
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
            */
            return 0;
        }

        private static int RunDumpAndReturnExitCode(DumpOptions opts)
        {
            throw new NotImplementedException();
        }
    }
}
