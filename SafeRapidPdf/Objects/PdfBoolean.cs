using System;

namespace SafeRapidPdf.Objects
{
    public sealed class PdfBoolean : PdfObject
    {
        public static readonly PdfBoolean True = new PdfBoolean(true);
        public static readonly PdfBoolean False = new PdfBoolean(false);

        private PdfBoolean(bool value)
            : base(PdfObjectType.Boolean)
        {
            Value = value;
        }

        public bool Value { get; }

        public static PdfBoolean Parse(string token)
        {
            switch (token)
            {
                case "true":
                    return True;
                case "false":
                    return False;
                default:
                    throw new Exception("Parser error: invalid boolean value");
            }

        }

        public override string ToString() => Value ? "true" : "false";
    }
}
