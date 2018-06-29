using System;

namespace SafeRapidPdf.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
	public sealed class ParameterTypeAttribute : Attribute 
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
		public Boolean Required { get; }

		/// <summary>
		/// Inheritable attribute
		/// </summary>
		public Boolean Inheritable { get; }

		/// <summary>
		/// PDF version from which this parameter is allowed
		/// </summary>
		public String Version { get; }

		/// <summary>
		/// Was this parameter obsoleted?
		/// </summary>
		public Boolean Obsolete { get; }
	}
}
