using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core
{
    /// <summary>
    /// The provenance of a basic block or procedure is evidence of how it arrived to be deduced.
    /// </summary>
    public class Provenance
    {
        public ProvenanceType ProvenanceType;
        public string Comment;
    }

    public enum ProvenanceType
    {
        None,
        ImageEntrypoint,     // reached here because image file "said so".
        UserInput,          // reached here because users input "said so".
        Scanning,           // reached here as part of the scanning process.
        Heuristic,          // reached here a a guess.
    }

    public class Provenance<T> : Provenance
    {
        public T ReachedFrom;
    }
}
