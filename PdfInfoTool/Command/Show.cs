
namespace PdfInfoTool
{
    using System;
    using SafeRapidPdf;
    using SafeRapidPdf.Objects;

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
                            var type = "ref ObjStm";
                            if (xrefEntry.EntryType != 'o')
                            {
                                var g = xrefEntry.GenerationNumber;
                                try
                                {
                                    var refObject = file.GetObject(o, g);
                                    var objectType = refObject.Object.ObjectType;
                                    if (objectType == PdfObjectType.Stream)
                                    {
                                        var stream = refObject.Object as PdfStream;
                                        IPdfObject contentType;
                                        if (stream.StreamDictionary.TryGetValue("Type", out contentType))
                                        {
                                            if (contentType.Text == "XObject")
                                            {
                                                stream.StreamDictionary.TryGetValue("Subtype", out contentType);
                                                type = $"Stream(XObject: {contentType})";
                                            }
                                            else
                                            {
                                                type = $"Stream({contentType})";
                                            }
                                        }
                                        else
                                        {
                                            //var sObject = PdfObject.ParseAny(stream);
                                            type = $"Stream(?)";
                                        }
                                    }
                                    else if (objectType == PdfObjectType.Dictionary)
                                    {
                                        var dictionary = refObject.Object as PdfDictionary;
                                        IPdfObject contentType;
                                        if (dictionary.TryGetValue("Type", out contentType))
                                        {
                                            type = $"{contentType}";
                                        }
                                        else
                                        {
                                            if (dictionary.Items.Count > 0 &&
                                                dictionary.Items[0].Text == "Linearized")
                                                type = "Linearization Parameter";
                                            else
                                                type = $"Dictionary(?)";
                                        }
                                    }
                                    else
                                    {
                                        type = objectType.ToString();
                                    }
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
