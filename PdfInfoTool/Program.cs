namespace PdfInfoTool
{
    using System;
    using CommandLine;
    using SafeRapidPdf;
    using SafeRapidPdf.Objects;

    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<DumpOptions, ShowOptions>(args)
              .MapResult(
                (DumpOptions opts) => Command.RunDumpAndReturnExitCode(opts),
                (ShowOptions opts) => Command.RunShowAndReturnExitCode(opts),
                _ => 1);
        }
    }
}
