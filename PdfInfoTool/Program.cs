using CommandLine;

namespace PdfInfoTool
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<DumpOptions, ShowOptions>(args)
              .MapResult(
                (DumpOptions opts) => Command.RunDumpAndReturnExitCode(opts),
                (ShowOptions opts) => Command.RunShowAndReturnExitCode(opts),
                _ => 1);
        }
    }
}
