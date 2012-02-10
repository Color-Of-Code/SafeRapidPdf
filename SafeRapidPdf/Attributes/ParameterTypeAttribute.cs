using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Attributes
{
	public class ParameterTypeAttribute : Attribute 
	{
		public ParameterTypeAttribute(Boolean required, Boolean inheritable = false,
			String version = "", Boolean obsolete = false)
		{
			Required = required;
			Inheritable = inheritable;
			Version = version;
			Obsolete = obsolete;
		}

		/// <summary>
		/// Required or Optional
		/// </summary>
		public Boolean Required { get; private set; }

		/// <summary>
		/// Inheritable attribute
		/// </summary>
		public Boolean Inheritable { get; private set; }

		/// <summary>
		/// PDF version from which this parameter is allowed
		/// </summary>
		public String Version { get; private set; }

		/// <summary>
		/// Was this parameter obsoleted?
		/// </summary>
		public Boolean Obsolete { get; private set; }
	}
}
