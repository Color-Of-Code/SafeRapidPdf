namespace PdfInfoTool
{
    using CommandLine;

    interface IOptions
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
    class DumpOptions : IOptions
    {
        public bool Verbose { get; set; }
        public bool Quiet { get; set; }
        public string FileName { get; set; }
    }

    [Verb("show", HelpText = "Show object contents in a human readable way.")]
    class ShowOptions : IOptions
    {
        public bool Verbose { get; set; }
        public bool Quiet { get; set; }
        public string FileName { get; set; }

        [Value(1, MetaName = "Type of object to display",
            HelpText = "Type: xref.",
            Required = true)]
        public string What { get; set; }
    }
}