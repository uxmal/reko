using Reko.Core;
using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Loading;

/// <summary>
/// A simple implementation of <see cref="IBinaryImage"/>.
/// </summary>
public class DefaultBinaryImage : IBinaryImage
{
    /// <inheritdoc/>
    public IBinaryHeader Header => throw new NotImplementedException();

    /// <inheritdoc/>
    public EndianServices Endianness => throw new NotImplementedException();

    /// <inheritdoc/>
    public IReadOnlyList<IBinarySection> Sections => throw new NotImplementedException();

    /// <inheritdoc/>
    public IReadOnlyList<IBinarySegment> Segments => throw new NotImplementedException();

    /// <inheritdoc/>
    public IBinaryDebugInfo? DebugInfo => throw new NotImplementedException();

    /// <inheritdoc/>
    public IReadOnlyList<IBinarySymbol> Symbols => throw new NotImplementedException();

    /// <inheritdoc/>
    public IReadOnlyDictionary<int, IBinarySymbol> DynamicSymbols => throw new NotImplementedException();

    /// <inheritdoc/>
    public IReadOnlyDictionary<int, IReadOnlyList<IRelocation>> Relocations => throw new NotImplementedException();

    /// <inheritdoc/>
    public IReadOnlyList<IRelocation> DynamicRelocations => throw new NotImplementedException();

    /// <inheritdoc/>
    public ImageLocation Location => throw new NotImplementedException();

    /// <inheritdoc/>
    public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IBinaryDumper CreateImageDumper()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Program Load(Address? addrLoad = null)
    {
        throw new NotImplementedException();
    }
}
