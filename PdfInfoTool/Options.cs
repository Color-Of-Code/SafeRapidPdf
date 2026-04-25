using CommandLine;

namespace PdfInfoTool;


internal interface IOptions
{
    [Option('v', "verbose",
        HelpText = "Verbose message output.")]
    bool Verbose { get; set; }

    [Option('q', "quiet",
        HelpText = "Suppresses summary messages.")]
    bool Quiet { get; set; }

    [Value(0, MetaName = "input pdf file",
        HelpText = "Input pdf file to be processed.",
        Required = true)]
    string FileName { get; set; }
}

[Verb("dump", HelpText = "Dump an object out.")]
#pragma warning disable CA1812 // Instantiated by CommandLineParser via reflection
internal sealed class DumpOptions : IOptions
#pragma warning restore CA1812
{
    public bool Verbose { get; set; }
    public bool Quiet { get; set; }
    public string FileName { get; set; }

    [Option('x', "hex",
        HelpText = "Dump decoded stream data as a hex string (for use in unit tests).")]
    public bool Hex { get; set; }

    [Option('b', "binary",
        HelpText = "Write decoded stream data as raw bytes to stdout (for piping to other tools).")]
    public bool Binary { get; set; }
}

[Verb("show", HelpText = "Show object contents in a human readable way.")]
#pragma warning disable CA1812 // Instantiated by CommandLineParser via reflection
internal sealed class ShowOptions : IOptions
#pragma warning restore CA1812
{
    public bool Verbose { get; set; }
    public bool Quiet { get; set; }
    public string FileName { get; set; }

    [Value(1, MetaName = "Type of object to display",
        HelpText = "Type: xref.",
        Required = true)]
    public string What { get; set; }
}
