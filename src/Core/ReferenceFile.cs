using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core
{
    /// <summary>
    /// Represents a file that only used for the  metdata it contains.
    /// </summary>
    public class ReferenceFile : ProjectFile
    {
        public string OriginalFile { get; set; }

        public string MetadataType { get; set; }

        public TypeLibrary TypeLibrary { get; set; }
    }
}
