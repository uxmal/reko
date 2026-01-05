#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core.Expressions;
using System.IO;

namespace Reko.Core.Output;

/// <summary>
/// This class generates C# code to instantiate identifiers in a 
/// procedure code mock.
/// </summary>
public class MockIdentifierWriter 
    : IMockIdentifierWriter
    , StorageVisitor<int, MockIdentifierWriter.Context>
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
        public string StorageName;

        /// <summary>
        /// The name of the identifier to generate.
        /// </summary>
        public Identifier Identifier;

        /// <summary>
        /// <see cref="MockGenerator"/> instance used to render type references.
        /// </summary>
        public MockGenerator MockGenerator;
    }

    /// <summary>
    /// Constructs a <see cref="MockIdentifierWriter"/> instance.
    /// </summary>
    /// <param name="writer">Output sink.</param>
    public MockIdentifierWriter(TextWriter writer)
    {
        this.writer = writer;
    }

    /// <inheritdoc/>
    public void WriteFramePointer()
    {
        writer.Write("m.Frame.FramePointer");
    }

    /// <inheritdoc/>
    public void WriteContinuation()
    {
        writer.Write("m.Frame.Continuation");
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
    public void CreateTemporary(TemporaryStorage tmp, Identifier id, MockGenerator mockGenerator, IndentingTextWriter writer)
    {
    }

    /// <inheritdoc/>
    public int VisitFlagGroupStorage(FlagGroupStorage grf, Context context)
    {
        writer.Write($"m.Frame.EnsureFlagGroup({context.StorageName})");
        return 0;
    }

    /// <inheritdoc/>
    public int VisitFpuStackStorage(FpuStackStorage fpu, Context context)
    {
        writer.Write($"m.Frame.EnsureFpuStackVariable({fpu.FpuStackOffset}");
        context.Identifier.DataType.Accept(context.MockGenerator);
        writer.Write(")");
        return 0;
    }

    /// <inheritdoc/>
    public int VisitMemoryStorage(MemoryStorage mem, Context context)
    {
        var id = context.Identifier;
        writer.WriteLine($"****** Storage {id.Storage.GetType().Name} for {id.Name} not implemented yet.");
        return 0;
    }

    /// <inheritdoc/>
    public int VisitRegisterStorage(RegisterStorage reg, Context context)
    {
        writer.Write($"m.Frame.EnsureRegister({context.StorageName})");
        return 0;
    }

    /// <inheritdoc/>
    public int VisitSequenceStorage(SequenceStorage seq, Context context)
    {
        writer.Write($"m.Frame.EnsureSequence(");
        context.Identifier.DataType.Accept(context.MockGenerator);
        writer.Write($", {context.StorageName})");
        return 0;
    }

    /// <inheritdoc/>
    public int VisitStackStorage(StackStorage stack, Context context)
    {
        writer.Write("m.Frame.EnsureStackVariable(");
        writer.Write($"{stack.StackOffset}, ");
        context.Identifier.DataType.Accept(context.MockGenerator);
        writer.Write($", \"{context.Identifier.Name}\")");
        return 0;
    }

    /// <inheritdoc/>
    public int VisitTemporaryStorage(TemporaryStorage tmp, Context context)
    {
        writer.Write("m.Frame.CreateTemporary(");
        writer.Write($"\"{tmp.Name}\", {tmp.Number}, ");
        context.Identifier.DataType.Accept(context.MockGenerator);
        writer.Write(")");
        return 0;
    }
}
