#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.Core
{
	/// <summary>
	/// Encapsulates architecture-dependent storage mechanisms for an identifier.
	/// </summary>
	public abstract class Storage
	{
		public Storage(string storageKind)
		{
			this.Kind = storageKind;
		}

		public string Kind { get; private set; }
		public abstract int OffsetOf(Storage storage);

        public abstract T Accept<T>(StorageVisitor<T> visitor);
        public abstract T Accept<C, T>(StorageVisitor<C, T> visitor, C context);

		public virtual SerializedKind Serialize()
		{
			throw new NotImplementedException(this.GetType().Name + ".Serialize not implemented.");
		}

		public override string ToString()
		{
			StringWriter w = new StringWriter();
			Write(w);
			return w.ToString();
		}

		public abstract void Write(TextWriter writer);
    }

    /// <summary>
    /// This class represents groups of bits stored in flag registers. Typically, these are the
    /// Carry, Zero, Overflow etc flags that are set after ALU operations.
    /// </summary>
	public class FlagGroupStorage : Storage
	{
		public FlagGroupStorage(uint grfMask, string name, DataType dataType) : base("FlagGroup")
		{
			this.FlagGroupBits = grfMask;
			this.Name = name;
            this.DataType = dataType;
		}

        public uint FlagGroupBits { get; private set; }
        public string Name { get; private set; }
        public DataType DataType { get; private set; }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
			return visitor.VisitFlagGroupStorage(this);
		}

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitFlagGroupStorage(this, context);
        }

		public override bool Equals(object obj)
		{
			FlagGroupStorage fgs = obj as FlagGroupStorage;
			if (fgs == null)
				return false;
			return FlagGroupBits == fgs.FlagGroupBits;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ FlagGroupBits.GetHashCode();
		}

		public override int OffsetOf(Storage stgSub)
		{
			FlagGroupStorage f = stgSub as FlagGroupStorage;
			if (f == null)
				return -1;
			return ((f.FlagGroupBits & FlagGroupBits) != 0) ? 0 : -1;
		}

		public override SerializedKind Serialize()
		{
			return new FlagGroup_v1(Name);
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("Flags");
		}
	}

	public class FpuStackStorage : Storage
	{
		public FpuStackStorage(int depth, DataType dataType) : base("FpuStack")
		{
			this.FpuStackOffset = depth;
			this.DataType = dataType;
		}

        public DataType DataType { get; private set; }
        public int FpuStackOffset { get; private set; }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
			return visitor.VisitFpuStackStorage(this);
		}

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitFpuStackStorage(this, context);
        }

		public override bool Equals(object obj)
		{
			FpuStackStorage fss = obj as FpuStackStorage;
			if (fss == null)
				return false;
			return FpuStackOffset == fss.FpuStackOffset;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ FpuStackOffset.GetHashCode();
		}

		public override int OffsetOf(Storage stgSub)
		{
			return -1;
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("FPU stack");
		}
	}

	/// <summary>
	/// Storage is some unspecified part of global memory.
	/// </summary>
	public class MemoryStorage : Storage
	{
		public MemoryStorage() : base("Global")
		{
		}

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
			return visitor.VisitMemoryStorage(this);
		}

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitMemoryStorage(this, context);
        }

		public override int OffsetOf(Storage stgSub)
		{
			return -1;
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("Global memory");
		}
	}

	/// <summary>
	/// Storage for registers or other identifiers that are live-out of a procedure.
	/// </summary>
	public class OutArgumentStorage : Storage
	{
		public OutArgumentStorage(Identifier originalId) : base("out")
		{
			this.OriginalIdentifier = originalId;
		}

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
			return visitor.VisitOutArgumentStorage(this);
		}

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitOutArgumentStorage(this, context);
        }

		public override bool Equals(object obj)
		{
			OutArgumentStorage oas = obj as OutArgumentStorage;
			if (oas == null)
				return false;
			return oas.OriginalIdentifier.Equals(OriginalIdentifier);
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ OriginalIdentifier.GetHashCode();
		}

		public override int OffsetOf(Storage stgSub)
		{
			return -1;
		}

		public Identifier OriginalIdentifier { get; private set; }

        public override SerializedKind Serialize()
        {
            return OriginalIdentifier.Storage.Serialize();
        }

		public override void Write(TextWriter writer)
		{
			writer.Write("Out:");
			OriginalIdentifier.Storage.Write(writer);
		}
	}

    /// <summary>
    /// Used to represent a machine register.
    /// </summary>
	public class RegisterStorage : Storage
	{
		public RegisterStorage(string name, int number, PrimitiveType dt) : base("Register")
		{
			this.Name = name;
            this.Number = number;
            this.DataType = dt;
		}

        /// <summary>
        /// If this register is a subregister of a wider register, this property the bit offset within that wider register.
        /// </summary>
        /// <remarks>For instance, on i386 systems, AH would return 8 here, since it is located at that bit offset of EAX.</remarks>
        public virtual int AliasOffset { get { return 0; } }

        /// <summary>
        /// The name of the register.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The size and domain of the register.
        /// </summary>
        /// <remarks>
        /// General-purpose registers can use the Domain.Word </remarks>
        public PrimitiveType DataType { get; private set; }

        /// <summary>
        /// Returns true if this is an ALU register that supports operations like addition, address dereference and the like.
        /// </summary>
        public virtual bool IsAluRegister { get { return true; } }

        public int Number { get; private set; }

        public override T Accept<T>(StorageVisitor<T> visitor)
		{
			return visitor.VisitRegisterStorage(this);
		}

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitRegisterStorage(this, context);
        }

		public override bool Equals(object obj)
		{
			var rs = obj as RegisterStorage;
			if (rs == null)
				return false;
			return Number == rs.Number;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ Number;
		}

        public RegisterStorage GetPart(DataType width)
        {
            return GetSubregister(0, width.BitSize);
        }

        public virtual RegisterStorage GetSubregister(int offset, int bitSize)
        {
            if (offset == 0 && bitSize == DataType.BitSize)
                return this;
            else
                return null;
        }

        public virtual RegisterStorage GetWidestSubregister(BitSet bits)
        {
            return null;
        }

        /// <summary>
        /// Returns true if this register is strictly contained by reg2.
        /// </summary>
        /// <param name="reg1"></param>
        /// <param name="reg2"></param>
        /// <returns></returns>
        public virtual bool IsSubRegisterOf(RegisterStorage reg2)
        {
            return false;
        }

		public override int OffsetOf(Storage stgSub)
		{
			var regSub = stgSub as RegisterStorage;
			if (regSub == null)
				return -1;
			if (regSub == this)
				return 0;
			return regSub.IsSubRegisterOf(this)
				? regSub.AliasOffset
				: -1;
		}

        /// <summary>
		/// Given a register, sets/resets the bits corresponding to the register
		/// and any other registers it aliases.
		/// </summary>
		/// <param name="iReg">Register to set</param>
		/// <param name="bits">BitSet to modify</param>
		/// <param name="f">truth value to set</param>
		public virtual void SetAliases(BitSet bitset, bool f)
		{
			bitset[Number] = f;
		}

		public override SerializedKind Serialize()
		{
			return new Register_v1(Name);
		}

        public virtual void SetRegisterFileValues(ulong[] registerFile, ulong value, bool[] valid)
        {
            registerFile[Number] = value;
            valid[Number] = true;
        }

        public virtual void SetRegisterStateValues(Expression value, bool isValid, Dictionary<Storage, Expression> ctx)
        {
            ctx[this] = value;
        }

        public int SubregisterOffset(RegisterStorage subReg)
        {
            var sub = subReg as RegisterStorage;
            if (sub != null)
            {
                if (Number == sub.Number)
                    return 0;
            }
            return -1;
        }

		public override void Write(TextWriter writer)
		{
			writer.Write(Name);
		}

        public static RegisterStorage None { get { return none; } }

        private static RegisterStorage none = new RegisterStorage("None", -1, PrimitiveType.Create(Domain.Any, 0));

        public Expression GetSlice(Expression value)
        {
            var c = value as Constant;
            if (c != null && c.IsValid)
            {
                return GetSliceImpl(c);
            }
            else
                return Constant.Invalid;
        }

        protected virtual Expression GetSliceImpl(Constant c)
        {
            return c;
        }
    }

	public class SequenceStorage : Storage
	{
		public SequenceStorage(Identifier head, Identifier tail) 
			: base("Sequence")		
		{
			this.Head = head;
			this.Tail = tail;
		}

        public Identifier Head { get; private set; }
        public Identifier Tail { get; private set; }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
			return visitor.VisitSequenceStorage(this);
		}

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitSequenceStorage(this, context);
        }

		public override bool Equals(object obj)
		{
			SequenceStorage ss = obj as SequenceStorage;
			if (ss == null)
				return false;
			return Head.Equals(ss.Head) && Tail.Equals(ss.Tail);
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ Head.GetHashCode() ^ (3 * Tail.GetHashCode());
		}

		public override int OffsetOf(Storage stgSub)
		{
			int off = Tail.Storage.OffsetOf(stgSub);
			if (off != -1)
				return off;
			off = Head.Storage.OffsetOf(stgSub);
			if (off != -1)
				return off + Tail.DataType.BitSize;
			return -1;
		}

		public override SerializedKind Serialize()
		{
			return new SerializedSequence(this);
		}

		public override void Write(TextWriter writer)
		{
			writer.Write("Sequence {0}:{1}", Head.Name, Tail.Name);
		}
	}

    public abstract class StackStorage : Storage
    {
        public StackStorage(string kind)
            : base(kind)
        {
        }
    }

	public class StackArgumentStorage : StackStorage
	{
		public StackArgumentStorage(int cbOffset, DataType dataType) : base("Stack")
		{
			this.StackOffset = cbOffset;
			this.DataType = dataType;
		}

        /// <summary>
        /// Offset from stack pointer as it was when the procedure was entered.
        /// </summary>
        /// <remarks>
        /// If the architecture stores the return address on the stack, the return address will be at offset 0 and
        /// any stack arguments will have offsets > 0. If the architecture passes the return address in a
        /// register or a separate return stack, there may be stack arguments with offset 0. Depending on which
        /// direction the stack grows, there may be negative stack offsets for parameters, although most popular
        /// general purpose processors (x86, PPC, m68K) grown their stacks down toward lower memory addresses.
        /// </remarks>
        public int StackOffset { get; private set; }
        public DataType DataType { get; private set; }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
			return visitor.VisitStackArgumentStorage(this);
		}

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitStackArgumentStorage(this, context);
        }

		public override bool Equals(object obj)
		{
			StackArgumentStorage sas = obj as StackArgumentStorage;
			if (sas == null)
				return false;
			return StackOffset == sas.StackOffset;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() ^ StackOffset;
		}

		public override int OffsetOf(Storage stgSub)
		{
			StackArgumentStorage arg = stgSub as StackArgumentStorage;
			if (arg == null)
				return -1;
            if (arg.StackOffset >= StackOffset && arg.StackOffset + arg.DataType.Size <= StackOffset + DataType.Size)
                return (arg.StackOffset - StackOffset) * DataType.BitsPerByte;
			return -1;
		}

        public override SerializedKind Serialize()
        {
            return new StackVariable_v1();
        }

		public override void Write(TextWriter writer)
		{
            writer.Write("{0} +{1:X4}", Kind, StackOffset);
		}
	}

    public class StackLocalStorage : StackStorage
    {
        public StackLocalStorage(int cbOffset, DataType dataType)
            : base("Local")
        {
            this.StackOffset = cbOffset;
            this.DataType = dataType;
        }

        public DataType DataType { get; private set; }
        public int StackOffset { get; private set; }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitStackLocalStorage(this);
        }

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitStackLocalStorage(this, context);
        }

        public override bool Equals(object obj)
        {
            StackLocalStorage sas = obj as StackLocalStorage;
            if (sas == null)
                return false;
            return StackOffset == sas.StackOffset;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ StackOffset;
        }


        public override int OffsetOf(Storage stgSub)
        {
            StackLocalStorage local = stgSub as StackLocalStorage;
            if (local == null)
                return -1;
            if (local.StackOffset >= StackOffset && local.StackOffset + local.DataType.Size <= StackOffset + DataType.Size)
                return (local.StackOffset - StackOffset) * DataType.BitsPerByte;
            return -1;
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("{0} -{1:X4}", base.Kind, Math.Abs(StackOffset));
        }
    }

    /// <summary>
    /// Temporary storage is used for expressing intermediate results that become exposed
    /// when complex machine instructions are broken down into simpler RTL sequences.
    /// </summary>
    /// <remarks>
    /// An example is the x86 instruction <code>shr ds:[0x41],3</code> which is 
    /// rewritten into rtl as:
    /// <code>
    /// tmp = Mem0[0x0041] >> 3
    /// Mem[0x0041] = tmp
    /// SCZ = Cond(tmp)
    /// </code>
    /// </remarks>
	public class TemporaryStorage : RegisterStorage
	{
		public TemporaryStorage(string name, int number, PrimitiveType dt) : base(name, number, dt)
		{
		}

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
			return visitor.VisitTemporaryStorage(this);
		}

        public override RegisterStorage GetSubregister(int offset, int size)
        {
            return null;
        }

        public override bool IsSubRegisterOf(RegisterStorage reg2)
        {
            return false;
        }

		public override int OffsetOf(Storage stgSub)
		{
			return -1;
		}
	}
}

