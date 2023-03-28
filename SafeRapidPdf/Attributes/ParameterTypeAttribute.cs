using System;

namespace SafeRapidPdf.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ParameterTypeAttribute : Attribute
{
    public ParameterTypeAttribute(
        bool required,
        bool inheritable = false,
        string version = "",
        bool obsolete = false)
    {
        Required = required;
        Inheritable = inheritable;
        Version = version;
        Obsolete = obsolete;
    }

    /// <summary>
    /// Required or Optional
    /// </summary>
    public bool Required { get; }

    /// <summary>
    /// Inheritable attribute
    /// </summary>
    public bool Inheritable { get; }

    /// <summary>
    /// PDF version from which this parameter is allowed
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Was this parameter obsoleted?
    /// </summary>
    public bool Obsolete { get; }
}
