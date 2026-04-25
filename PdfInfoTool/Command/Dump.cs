using System;
using System.Globalization;
using System.IO;
using System.Text;
using SafeRapidPdf;
using SafeRapidPdf.Objects;
using SafeRapidPdf.Parsing;

namespace PdfInfoTool;

internal static partial class Command
{
    internal static int RunDumpAndReturnExitCode(DumpOptions opts)
    {
        if (opts.Hex && opts.Binary)
        {
            Console.Error.WriteLine("--hex and --binary are mutually exclusive.");
            return 1;
        }

        try
        {
            var file = PdfFile.Parse(opts.FileName);

            if (opts.Binary)
            {
                DumpBinary(file);
                return 0;
            }

            if (!opts.Quiet)
            {
                Console.WriteLine("PDF Version: {0}", file.Version);
            }

            if (opts.Hex)
            {
                DumpHex(file);
            }
            else
            {
                WriteObject(file, depth: 0, includeType: opts.Verbose);

                if (!opts.Quiet)
                {
                    Console.WriteLine("Top-level objects: {0}", file.Items.Count);
                }
            }

            return 0;
        }
        catch (Exception ex) when (
            ex is ParsingException or IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    /// <summary>
    /// Recursively print all stream data as hex strings, one stream per line,
    /// prefixed with the owning indirect object number.
    /// </summary>
    private static void DumpHex(IPdfObject obj, string prefix = "")
    {
        if (obj is PdfIndirectObject indirect)
        {
            prefix = indirect.ObjectNumber.ToString(CultureInfo.InvariantCulture);
        }

        if (obj is PdfStream stream)
        {
            byte[] decoded = stream.Decode();
            Console.WriteLine("{0}: {1}", prefix, ToHexString(decoded));
            return;
        }

        if (obj.IsContainer && obj.Items is not null)
        {
            foreach (var item in obj.Items)
            {
                DumpHex(item, prefix);
            }
        }
    }

    /// <summary>
    /// Write the raw decoded bytes of every stream in the file to stdout.
    /// Intended for piping to external tools.
    /// </summary>
    private static void DumpBinary(IPdfObject obj)
    {
        if (obj is PdfStream stream)
        {
            byte[] decoded = stream.Decode();
            using var stdout = new BinaryWriter(Console.OpenStandardOutput(), Encoding.UTF8, leaveOpen: true);
            stdout.Write(decoded);
            return;
        }

        if (obj.IsContainer && obj.Items is not null)
        {
            foreach (var item in obj.Items)
            {
                DumpBinary(item);
            }
        }
    }

    private static string ToHexString(byte[] data)
    {
        var sb = new StringBuilder(data.Length * 2);
        foreach (byte b in data)
        {
            _ = sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
        }
        return sb.ToString();
    }

    private static void WriteObject(IPdfObject obj, int depth, bool includeType)
    {
        var indent = new string(' ', depth * 2);
        if (includeType)
        {
            Console.WriteLine($"{indent}{obj.ObjectType}: {obj.Text}");
        }
        else
        {
            Console.WriteLine($"{indent}{obj.Text}");
        }

        if (!obj.IsContainer || obj.Items is null)
        {
            return;
        }

        foreach (var item in obj.Items)
        {
            WriteObject(item, depth + 1, includeType);
        }
    }
}
