using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Loading;

/// <summary>
/// Represents an address relocation.
/// </summary>
public interface IRelocation
{
    /// <summary>
    /// Offset at which the relocation is to take place.
    /// </summary>
    /// <remarks>
    /// In relocatable files, this offset may be relative
    /// to a section start. Otherwise, the offset is
    /// relative to the base of the executable.
    /// </remarks>
    ulong Offset { get; }

    /// <summary>
    /// Optional added to apply when relocating.
    /// </summary>
    long? Addend { get; }

    /// <summary>
    /// Optional symbol whose value may or may not
    /// be used when computing the relocation.
    /// </summary>
    IBinarySymbol? Symbol { get; }

    /// <summary>
    /// File format-specific information about this
    /// relocation.
    /// </summary>
    ulong Info { get; }
}

