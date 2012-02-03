using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SafeRapidPdf.Primitives
{
	public class PdfName : PdfObject
	{
		public PdfName(Lexical.ILexer lexer)
		{
			lexer.Expects("/");
			String name = lexer.ReadToken();
			// process the # encoded chars
			Name = Regex.Replace(name, @"#(\d\d)", x => { 
					byte val = Convert.ToByte(x.Groups[1].Value, 16);
					return ((char)val).ToString();
				});
		}

		public string Name { get; private set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
