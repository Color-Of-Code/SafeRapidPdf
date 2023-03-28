using CommandLine;
using PdfInfoTool;

return Parser.Default.ParseArguments<DumpOptions, ShowOptions>(args)
             .MapResult(
                (DumpOptions opts) => Command.RunDumpAndReturnExitCode(opts),
                (ShowOptions opts) => Command.RunShowAndReturnExitCode(opts),
                _ => 1);
