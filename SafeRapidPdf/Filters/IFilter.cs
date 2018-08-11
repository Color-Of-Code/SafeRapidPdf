using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Filters
{
    using SafeRapidPdf.File;

    public interface IFilter
    {
        /// <summary>
        /// Decodes the input buffer and returns a decoded output buffer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        byte[] Decode(byte[] input, PdfDictionary options);
    }
}
