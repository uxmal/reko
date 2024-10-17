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
    /// If the <see cref="Section"/> property is not null, 
    /// the offset is relative to the start of that section.
    /// Otherwise, the offset is relative to the base of the executable.
    /// </remarks>
    ulong Offset { get; }

    /// <summary>
    /// Optional added to apply when relocating.
    /// </summary>
    long Addend { get; }

    /// <summary>
    /// Optional section. If specified, the <see cref="Offset"/> is
    /// relative to the base address of the section.
    /// </summary>
    IBinarySection? Section { get; }
    uint Type { get; }
}

