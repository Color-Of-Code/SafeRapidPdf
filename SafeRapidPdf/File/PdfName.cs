using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SafeRapidPdf.File
{
	public class PdfName : PdfObject
	{
		private PdfName(String name)
			: base(PdfObjectType.Name)
		{
			_rawName = name;
		}

		public static PdfName Parse(Lexical.ILexer lexer)
		{
			String name = lexer.ReadToken();
			return new PdfName(name);
		}

		private String _rawName;

		public string Name
		{
			get
			{
				// process the # encoded chars
				return Regex.Replace(_rawName, @"#(\d\d)", x =>
				{
					byte val = Convert.ToByte(x.Groups[1].Value, 16);
					return ((char)val).ToString();
				});
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
