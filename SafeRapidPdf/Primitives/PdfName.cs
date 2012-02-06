using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SafeRapidPdf.Primitives
{
	public class PdfName : PdfObject
	{
		private PdfName(String name)
			: base(PdfObjectType.Name)
		{
			Name = name;
		}

		public static PdfName Parse(Lexical.ILexer lexer)
		{
			lexer.Expects("/");
			String name = lexer.ReadToken();
			// process the # encoded chars
			name = Regex.Replace(name, @"#(\d\d)", x => { 
					byte val = Convert.ToByte(x.Groups[1].Value, 16);
					return ((char)val).ToString();
				});
			return new PdfName(name);
		}

		public string Name { get; private set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
