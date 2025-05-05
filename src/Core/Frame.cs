#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// Contains the non-global locations accessed by the code inside a procedure. 
    /// </summary>
    /// <remarks>
    /// Variables accessed by the procedure can live in many places: registers, stack, and temporaries.
    /// <para>
    /// The frame layout is particularly interesting. On the Intel x86 architecture in real mode we have the following:
    /// </para>
    /// <code>
    ///   Layout             offset value
    /// +-----------------+
    /// | arg3            |   6
    /// +-----------------+
    /// | arg2            |   4
    /// +-----------------+
    /// | arg1            |   2
    /// +-----------------+
    /// | return address  |	  0
    /// +-----------------+
    /// | frame pointer   |   -2
    /// +-----------------+
    /// | local1          |   -4
    /// +-----------------+
    /// | local2          |   -6
    /// 
    /// etc.
    /// </code>
    /// <para>Note that variables that are stack arguments use offsets based on the state of stack
    /// _after_ the return address was pushed. The return address, if passed on the stack, is always
    /// considered to be at offset 0.</para>
    /// <para>In addition, support has to be provided for architectures that have separate FPU stacks.</para>
    /// </remarks>
    public class Frame : IStorageBinder
	{
        private readonly IProcessorArchitecture arch;
        private readonly NamingPolicy namingPolicy;

        /// <summary>
        /// Creates a Frame instance for maintaining the local variables and arguments.
        /// </summary>
        /// <param name="arch">Processor architecture used in this frame.</param>
        /// <param name="framePointerSize">The size of the frame pointer must match the size of the 
        /// stack register, if any, or at the least the size of a pointer.</param>
        /// <param name="codeAddressSize">The size of the return address. Usually it is the same
        /// as a pointer size, but on x86 this can differ from the frame pointer size.</param>
		public Frame(
            IProcessorArchitecture arch,
            PrimitiveType framePointerSize,
            PrimitiveType codeAddressSize)
		{
            if (framePointerSize is null)
                throw new ArgumentNullException(nameof(framePointerSize));
            this.arch = arch;
            this.Identifiers = [];
            this.namingPolicy = NamingPolicy.Instance;

			// There is always a "variable" for the global memory and the frame
			// pointer.

            this.Memory = new Identifier("Mem0", new UnknownType(), MemoryStorage.Instance);
            this.Identifiers.Add(Memory);
			this.FramePointer = CreateTemporary("fp", framePointerSize);
            this.Continuation = CreateTemporary("%continuation", codeAddressSize);
		}

        /// <summary>
        /// Creates a Frame instance for maintaining the local variables and arguments whose
        /// frame pointer size is same as a general purpose pointer size.
        /// </summary>
        /// <param name="arch">Processor architecture used in this frame.</param>
        /// <param name="pointerSize">The size of a pointer.
        /// </param>
        public Frame(IProcessorArchitecture arch, PrimitiveType pointerSize) : this(arch, pointerSize, pointerSize)
        {
        }

        /// <summary>
        /// True if frame escapes, and is used by a callee.
        /// </summary>
        public bool Escapes { get; set; }

        /// <summary>
        /// Frame offset from the frame of the caller.
        /// </summary>
        public int FrameOffset { get; set; }

        /// <summary>
        /// A virtual register that is considered to coincide with the value
        /// of the stack pointer at the entry to a procedure.
        /// </summary>
        public Identifier FramePointer { get; }

        /// <summary>
        /// A symbolic value for the continuation (return address) of this
        /// procedure.
        /// </summary>
        public Identifier Continuation { get; }

        /// <summary>
        /// The list of identifiers of this frame.
        /// </summary>
        public List<Identifier> Identifiers { get; }

        /// <summary>
        /// The memory storage for this frame.
        /// </summary>
        public Identifier Memory { get; private set; }

        /// <summary>
        /// Amount of bytes that the calling function pushed on the stack to store 
        /// the return address. Some architectures pass the return address in a register,
        /// which implies that in those architectures the return address size should be zero.
        /// </summary>
        public int? ReturnAddressSize { get; set; }

        /// <summary>
        /// Creates a new identifier for a sequence of storages.
        /// </summary>
        /// <param name="dt">The data type for this identifier.</param>
        /// <param name="elements">The storages constituting the sequemce,
        /// in big-endian order: the most significant sub-storage appears first.</param>
        /// <returns></returns>
        public Identifier CreateSequence(DataType dt, params Storage [] elements)
        {
            var name = string.Join("_", elements.Select(e => e.Name));
            var id = new Identifier(name, dt, new SequenceStorage(dt, elements));
            Identifiers.Add(id);
            return id;
        }

        /// <summary>
        /// Creates a new identifier for a sequence of storages with a given name.
        /// </summary>
        /// <param name="dt">The data type for this identifier.</param>
        /// <param name="name">The name to use for the resulting identifier.</param>
        /// <param name="elements">The storages constituting the sequemce,
        /// in big-endian order: the most significant sub-storage appears first.</param>
        /// <returns></returns>
        public Identifier CreateSequence(DataType dt, string name, params Storage[] elements)
        {
            var id = new Identifier(name, dt, new SequenceStorage(dt, elements));
            Identifiers.Add(id);
            return id;
        }

        /// <summary>
        /// Given a <see cref="Storage"/> ensures there is an identifier backed by that
        /// storage.
        /// </summary>
        /// <param name="storage">The given <see cref="Storage"/> instance.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Identifier EnsureIdentifier(Storage storage)
        {
            switch (storage)
            {
            case RegisterStorage reg:
                return EnsureRegister(reg);
            case FlagGroupStorage grf:
                return EnsureFlagGroup(grf);
            case SequenceStorage seq:
                return EnsureSequence(seq);
            case FpuStackStorage fp:
                return EnsureFpuStackVariable(fp.FpuStackOffset, fp.DataType);
            case StackStorage st:
                return EnsureStackVariable(st.StackOffset, st.DataType);
            case TemporaryStorage tmp:
                return CreateTemporary(tmp.Name, tmp.DataType);
            }
            throw new NotImplementedException(string.Format(
                "Unsupported storage {0}.",
                storage is not null ? storage.ToString() : "(null)"));
        }

		/// <summary>
		/// Creates a temporary variable whose storage and name is guaranteed not to collide with any other variable.
		/// </summary>
        /// <param name="dt">Datatype of the identifier.</param>
        /// <returns>A new identifier.</returns>
		public Identifier CreateTemporary(DataType dt)
		{
            string name = "v" + Identifiers.Count;
			var id = new Identifier(name, dt,
                new TemporaryStorage(name, Identifiers.Count, dt));
			Identifiers.Add(id);
			return id;
		}

        /// <summary>
        /// Creates an identifier with a given name whose storage is guaranteed not to collide with any other variable.
        /// </summary>
        /// <param name="name">Name to give the identifier.</param>
        /// <param name="dt">Datatype of the identifier.</param>
        /// <returns>A new identifier.</returns>
		public Identifier CreateTemporary(string name, DataType dt)
		{
            var id = new Identifier(name, dt, 
                new TemporaryStorage(name, Identifiers.Count, dt));
			Identifiers.Add(id);
			return id;
		}

        /// <summary>
        /// Given a <see cref="FlagGroupStorage"/>, ensures that there is a 
        /// corresponding identifier in this frame.
        /// </summary>
        /// <param name="grf">A <see cref="FlagGroupStorage"/> instance.</param>    
        /// <returns>A previously existing identifier with the same 
        /// flag group storage, or a newly created one.
        /// </returns>
        public Identifier EnsureFlagGroup(FlagGroupStorage grf)
        {
            if (grf.FlagGroupBits == 0)
                throw new ArgumentException("Argument must have non-zero flag group bits.", nameof(grf));
            var id = FindFlagGroup(grf.FlagRegister, grf.FlagGroupBits);
            if (id is null)
            {
                id = Identifier.Create(grf);
                Identifiers.Add(id);
            }
            return id;
        }

        /// <summary>
        /// Ensures the existence of a floating-point stack storage in this frame.
        /// </summary>
        /// <param name="depth">The </param>
        /// <param name="type"></param>
        /// <returns></returns>
		public Identifier EnsureFpuStackVariable(int depth, DataType type)
		{
			Identifier? id = FindFpuStackVariable(depth);
			if (id is null)
			{
				string name = string.Format("{0}{1}", (depth < 0 ? "rLoc" : "rArg"), Math.Abs(depth));
				id = new Identifier(name, type, new FpuStackStorage(depth, type));
				Identifiers.Add(id);
			}
			return id;
		}

		/// <summary>
		/// Ensures a register accessed in this function. 
		/// </summary>
		/// <param name="reg">Register storage backing the identifier.</param>
		/// <returns>The resulting identifier.</returns>
		public Identifier EnsureRegister(RegisterStorage reg)
		{
			Identifier? id = FindRegister(reg);
			if (id is null)
			{
                id = Identifier.Create(reg);
				Identifiers.Add(id);
			}
			return id;
		}

        /// <summary>
        /// Ensures an out argument identifier for the given original identifier in this frame.
        /// </summary>
        /// <param name="idOrig">Original identifier.</param>
        /// <param name="outArgumentPointer">Data type to use for pointers.</param>
		/// <returns>The resulting identifier.</returns>
		public Identifier EnsureOutArgument(Identifier idOrig, DataType outArgumentPointer)
		{
			Identifier? idOut = FindOutArgument(idOrig);
			if (idOut is null)
			{
				idOut = new Identifier(idOrig.Name + "Out", outArgumentPointer, new OutArgumentStorage(idOrig));
				Identifiers.Add(idOut);
			}
			return idOut;
		}

        /// <inheritdoc />
        public Identifier EnsureSequence(SequenceStorage sequence)
        {
            var idSeq = FindSequence(sequence.Elements);
            if (idSeq is null)
            {
                idSeq = new Identifier(sequence.Name, sequence.DataType, sequence);
                Identifiers.Add(idSeq);
            }
            return idSeq;
        }

        /// <inheritdoc />
		public Identifier EnsureSequence(DataType dt, params Storage [] elements)
        {
			Identifier? idSeq = FindSequence(elements);
			if (idSeq is null)
			{
				idSeq = CreateSequence(dt, elements);
			}
			return idSeq;
		}

        /// <summary>
        /// Makes sure that there is a local variable at the given offset.
        /// </summary>
        /// <param name="cbOffset">Stack offset.</param>
        /// <param name="type">Local variable.</param>
        /// <returns>Identifier of a stack local.</returns>
        public Identifier EnsureStackLocal(int cbOffset, DataType type)
		{
			return EnsureStackLocal(cbOffset, type, null);
		}

        /// <summary>
        /// Makes sure that there is a local variable at the given offset.
        /// </summary>
        /// <param name="cbOffset">Stack offset.</param>
        /// <param name="type">Local variable.</param>
        /// <param name="name">Name of the variable, or null to generate it
        /// automatically.</param>
        /// <returns>Identifier of a stack local.</returns>
		public Identifier EnsureStackLocal(int cbOffset, DataType type, string? name)
		{
			Identifier? id = FindStackLocal(cbOffset, type);
			if (id is null)
			{
				id = new Identifier(namingPolicy.StackLocalName(type, cbOffset, name), type, new StackStorage(cbOffset, type));
				Identifiers.Add(id);
			}
			return id;
		}

        /// <summary>
        /// Makes sure that there is a stack argument variable at the given offset.
        /// </summary>
        /// <param name="cbOffset">Stack offset.</param>
        /// <param name="type">Local variable.</param>
        /// <returns>Identifier of a stack argument.</returns>
		public Identifier EnsureStackArgument(int cbOffset, DataType type)
		{
			return EnsureStackArgument(cbOffset, type, null);
		}

        /// <summary>
        /// Makes sure that there is a stack argument variable at the given offset.
        /// </summary>
        /// <param name="cbOffset">Stack offset.</param>
        /// <param name="type">Local variable.</param>
        /// <param name="argName">Name of the variable, or null to generate it
        /// automatically.</param>
        /// <returns>Identifier of a stack argument.</returns>
		public Identifier EnsureStackArgument(int cbOffset, DataType type, string? argName)
		{
			Identifier? id = FindStackArgument(cbOffset, type.Size);
			if (id is null)
			{
				id = new Identifier(
					namingPolicy.StackArgumentName(type, cbOffset, argName), 
					type, 
					new StackStorage(cbOffset, type));
				Identifiers.Add(id);
			}			
			return id;
		}

        /// <summary>
        /// Ensures that there is a stack variable at the given offset.
        /// </summary>
        /// <param name="byteOffset">Offset from frame pointer, in storage units.</param>
        /// <param name="type">Data type of the variable.</param>
        /// <param name="name">The name of the variable, or null to
        /// automatically generate name.</param>
        /// <returns>Identifier of a stack variable.</returns>
        public Identifier EnsureStackVariable(int byteOffset, DataType type, string? name = null)
        {
            return arch.IsStackArgumentOffset(byteOffset)
                ? EnsureStackArgument(byteOffset, type, name)
                : EnsureStackLocal(byteOffset, type, name);
        }

        /// <summary>
        /// Finds an identifier in this frame that is backed by the given
        /// sequence of storages.
        /// </summary>
        /// <param name="elements">Sequence of storages.</param>
        /// <returns>The corresponding <see cref="Identifier"/>, or 
        /// null if no sequence was found.
        /// </returns>
        public Identifier? FindSequence(Storage[] elements)
        {
            foreach (Identifier id in Identifiers)
            {
                if (id.Storage is SequenceStorage seq &&
                    seq.Elements.Length == elements.Length)
                {
                    var allSame = true;
                    for (int i = 0; allSame && i < seq.Elements.Length; ++i)
                    {
                        allSame &= seq.Elements[i].Equals(elements[i]);
                    }
                    if (allSame)
                        return id;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds an identifier in this frame that is backed by the given
        /// register <paramref name="reg"/>.
        /// </summary>
        /// <param name="reg">Backing register.</param>
        /// <param name="grfMask">Bits of the flag group.</param>
        /// <returns>A matching identifier if one is found; otherwise null.
        /// </returns>
		public Identifier? FindFlagGroup(RegisterStorage reg, uint grfMask)
		{
			foreach (Identifier id in Identifiers)
			{
                if (id.Storage is FlagGroupStorage flags &&
                    flags.FlagRegister == reg &&
                    flags.FlagGroupBits == grfMask)
                {
                    return id;
                }
            }
			return null;
		}

        /// <summary>
        /// Finds the identifier for an FPU stack variable at the given offset.
        /// </summary>
        /// <param name="offset">Offset of the stack variable.</param>
        /// <returns>The corrsesponding identifier, or null if none
        /// was found.
        /// </returns>
		public Identifier? FindFpuStackVariable(int offset)
		{
			foreach (Identifier id in Identifiers)
			{
                if (id.Storage is FpuStackStorage fst && fst.FpuStackOffset == offset)
                    return id;
            }
			return null;
		}

        /// <summary>
        /// Finds the identifier for an output variable .
        /// </summary>
        /// <param name="idOrig">The original identifier.</param>
        /// <returns>The corrsesponding identifier, or null if none
        /// was found.
        /// </returns>
		public Identifier? FindOutArgument(Identifier idOrig)
		{
			foreach (Identifier id in Identifiers)
			{
                if (id.Storage is OutArgumentStorage s && s.OriginalIdentifier == idOrig)
                {
                    return id;
                }
            }
			return null;
		}

        /// <summary>
        /// Finds the identifier for the given register.
        /// </summary>
        /// <param name="reg">Register to find.</param>
        /// <returns>The corresponding identifier, or null if none
        /// was found.
        /// </returns>
		public Identifier? FindRegister(RegisterStorage reg)
		{
			foreach (Identifier id in Identifiers)
			{
                if (id.Storage is RegisterStorage s && s == reg)
                    return id;
            }
			return null;
		}

        /// <summary>
        /// Finds the identifier for a stack argument at the given offset.
        /// </summary>
        /// <param name="offset">Offset of the stack argument.</param>
        /// <param name="size">Size of the variable in storage units.</param>
        /// <returns>The corresponding identifier, or null if none
        /// was found.
        /// </returns>
		public Identifier? FindStackArgument(int offset, int size)
		{
			foreach (Identifier id in Identifiers)
			{
                if (id.Storage is StackStorage s && s.StackOffset == offset && id.DataType.Size == size)
                {
                    return id;
                }
            }
			return null;
		}

        /// <summary>
        /// Finds the identifier for a stack local variable at the given offset.
        /// </summary>
        /// <param name="offset">Offset of the stack variable.</param>
        /// <param name="dt">Size of the variable.</param>
        /// <returns>The corresponding identifier, or null if none
        /// was found.
        /// </returns>
		public Identifier? FindStackLocal(int offset, DataType dt)
		{
			foreach (Identifier id in Identifiers)
			{
                if (id.Storage is StackStorage loc &&
                    loc.StackOffset == offset &&
                    (id.DataType.Size == dt.Size))
                {
                    return id;
                }
            }
			return null;
		}

        /// <summary>
        /// Writes a textual representation of the identifiers of this frame
        /// to the provided <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="text">A <see cref="TextWriter"/> instance to which 
        /// the textual representation is written.
        /// </param>
		public void Write(TextWriter text)
		{
			foreach (Identifier id in Identifiers)
			{
				text.Write("// ");
				text.Write(id.Name);
				text.Write(":");
				id.Storage.Write(text);
				text.WriteLine();
			}
			if (Escapes)
				text.WriteLine("// Frame escapes");
			text.WriteLine("// return address size: {0}", ReturnAddressSize.HasValue
                ? ReturnAddressSize.Value.ToString()
                : "?");
		}
    }
}