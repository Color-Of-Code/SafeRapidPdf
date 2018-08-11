
namespace PdfInfoTool
{
    using System;
    using SafeRapidPdf.File;

    internal static partial class Command
    {
        internal static int RunShowAndReturnExitCode(ShowOptions opts)
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
                        var o = xrefEntry.ObjectNumber;
                        var obj = o.ToString("D5");
                        if (opts.Verbose)
                        {
                            var type = "In-Stream";
                            if (xrefEntry.EntryType != 'o')
                            {
                                var g = xrefEntry.GenerationNumber;
                                try
                                {
                                    var refObject = file.GetObject(o, g);
                                    type = refObject.Object.ObjectType.ToString();
                                }
                                catch
                                {
                                    type = $"Not found {o} {g}";
                                }
                            }

                            Console.WriteLine($"{obj}: {entry} - {type}");
                        }
                        else
                        {
                            Console.WriteLine($"{obj}: {entry}");
                        }
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
    }
}
