namespace SafeRapidPdf.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ParameterTypeAttribute(
    bool required,
    bool inheritable = false,
    string version = "",
    bool obsolete = false) : Attribute
{
    /// <summary>
    /// Required or Optional
    /// </summary>
    public bool Required { get; } = required;

    /// <summary>
    /// Inheritable attribute
    /// </summary>
    public bool Inheritable { get; } = inheritable;

    /// <summary>
    /// PDF version from which this parameter is allowed
    /// </summary>
    public string Version { get; } = version;

    /// <summary>
    /// Was this parameter obsoleted?
    /// </summary>
    public bool Obsolete { get; } = obsolete;
}
