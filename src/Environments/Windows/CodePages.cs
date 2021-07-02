using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Reko.Environments.Windows
{
    // https://en.wikipedia.org/wiki/Windows_code_page
    public static class CodePages
    {
        // Provides a translation from Windows code page numbers to
        // .NET encoding names.
        public static IDictionary<int, string> ToEncodings = new ReadOnlyDictionary<int, string>(
            new Dictionary<int, string>
            {
                { 28591, "ISO-8859-1"  },
                { 1252, "ISO-8859-1" },     // Almost true; some code points are different
                { 65001, "UTF-8" }
            });
    }
}
