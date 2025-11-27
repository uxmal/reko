using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Reko.Core.Output;

/// <summary>
/// This class generates C# code to instantiate identifiers in a 
/// SSA procedure code mock.
/// </summary>

public class SsaMockIdentifierWriter 
    : IMockIdentifierWriter,
    StorageVisitor<int, SsaMockIdentifierWriter.Context>
{
    private readonly TextWriter writer;

    /// <summary>
    /// Context object used when visiting storage types.
    /// </summary>
    public struct Context
    {
        /// <summary>
        /// The name of the variable used to store the identifier's storage.
        /// </summary>
        public Identifier Identifier;

        /// <summary>
        /// The name of the identifier to generate.
        /// </summary>
        public string StorageName;

        /// <summary>
        /// <see cref="MockGenerator"/> instance used to render type references.
        /// </summary>
        public MockGenerator MockGenerator;
    }

    /// <summary>
    /// Constructs a <see cref="MockIdentifierWriter"/> instance.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    public SsaMockIdentifierWriter(TextWriter writer)
    {
        this.writer = writer;
    }

    /// <inheritdoc/>
    public void WriteContinuation()
    {
        writer.Write("m.Continuation()");
    }

    /// <inheritdoc/>
    public void WriteFramePointer()
    {
        writer.Write("m.FramePointer()");
    }

    /// <inheritdoc/>
    public void WriteIdentifier(Identifier id, string storageName, MockGenerator mockGenerator)
    {
        id.Storage.Accept(this, new Context
        {
            Identifier = id, 
            StorageName = storageName,
            MockGenerator = mockGenerator
        });
    }

    /// <inheritdoc/>
    public int VisitFlagGroupStorage(FlagGroupStorage grf, Context context)
    {
        writer.Write($"m.Flags(\"{context.Identifier.Name}\", {context.StorageName})");
        return 0;
    }

    /// <inheritdoc/>
    public int VisitFpuStackStorage(FpuStackStorage fpu, Context context)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public int VisitMemoryStorage(MemoryStorage mem, Context context)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public int VisitRegisterStorage(RegisterStorage reg, Context context)
    {
        var id = context.Identifier;
        writer.Write($"m.Reg(\"{id.Name}\", {context.StorageName})");
        return 0;
    }

    /// <inheritdoc/>
    public int VisitSequenceStorage(SequenceStorage seq, Context context)
    {
        var id = context.Identifier;
        writer.Write($"m.SeqId(\"{id.Name}\", ");
        id.DataType.Accept(context.MockGenerator);
        writer.Write($", {context.StorageName})");
        return 0;
    }

    /// <inheritdoc/>
    public int VisitStackStorage(StackStorage stack, Context context)
    {
        var id = context.Identifier;
        writer.Write($"m.Stack(\"{id.Name}\", {stack.StackOffset}, ");
        id.DataType.Accept(context.MockGenerator);
        writer.Write(")");
        return 0;
    }

    /// <inheritdoc/>
    public int VisitTemporaryStorage(TemporaryStorage temp, Context context)
    {
        var id = context.Identifier;
        writer.Write("m.Temp(");
        id.DataType.Accept(context.MockGenerator);
        writer.Write(", \"{id.Name}\")");
        return 0;
    }
}
