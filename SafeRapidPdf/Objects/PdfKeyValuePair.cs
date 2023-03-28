﻿using System.Collections.Generic;

namespace SafeRapidPdf.Objects;

/// <summary>
/// Object not described in the specification but eases use and
/// implementation in .NET
/// </summary>
public sealed class PdfKeyValuePair : PdfObject
{
    public PdfKeyValuePair(PdfName key, PdfObject value)
        : base(PdfObjectType.KeyValuePair)
    {
        IsContainer = true;
        Key = key;
        Value = value;
    }

    public PdfName Key { get; }

    public PdfObject Value { get; }

    public override IReadOnlyList<IPdfObject> Items => new[] { Value };

    public override string ToString()
    {
        return Key.Text;
    }
}
