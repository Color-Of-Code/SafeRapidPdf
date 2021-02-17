using SafeRapidPdf.Parsing;

namespace SafeRapidPdf.Objects
{
    public sealed class PdfBoolean : PdfObject
    {
        public static readonly PdfBoolean True = new(true);
        public static readonly PdfBoolean False = new(false);

        private PdfBoolean(bool value)
            : base(PdfObjectType.Boolean)
        {
            Value = value;
        }

        public bool Value { get; }

        public static PdfBoolean Parse(string token)
        {
            return token switch
            {
                "true" => True,
                "false" => False,
                _ => throw new ParsingException($"Expected true or false. Was {token}."),
            };
        }

        public override string ToString()
        {
            return Value ? "true" : "false";
        }
    }
}
