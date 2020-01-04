#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// Encapsulates architecture-dependent storage mechanisms for an identifier.
    /// </summary>
    public abstract class Storage
    {
        public Storage(string storageKind, DataType dataType)
        {
            this.Kind = storageKind;
            this.DataType = dataType;
        }

        public string Kind { get; private set; }

        /// <summary>
        /// The storage domain for this storage. 
        /// </summary>
        public StorageDomain Domain { get; set; }

        /// <summary>
        /// The starting bit position of this storage.
        /// </summary>
        public ulong BitAddress { get; set; }

        /// <summary>
        /// Size of this storage, in bits.
        /// </summary>
        public virtual ulong BitSize { get; set; }

        /// <summary>
        /// The size and domain of the storage.
        /// </summary>
        public DataType DataType { get; }

        /// <summary>
        /// The name of this storage.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Returns the bit offset of <paramref name="storage"/> within this 
        /// storage, or -1 if <paramref name="storage"/> is not a part of 
        /// this storage.
        /// </summary>
        /// <param name="storage"></param>
        /// <returns>The offset in bits, or -1.</returns>
        public abstract int OffsetOf(Storage storage);
        public abstract T Accept<T>(StorageVisitor<T> visitor);
        public abstract T Accept<C, T>(StorageVisitor<C, T> visitor, C context);

        /// <summary>
        /// Returns true if the Storage <paramref name="that"/> overlaps with this
        /// Storage.
        public abstract bool OverlapsWith(Storage that);

        /// <summary>
        /// A storage <code>a</code> is said to cover a storage 
        /// <code>b</code> if they alias a location, but after 
        /// assigning a with b, some of the original contents of 
        /// a still peek through.
        /// For instance, the x86 register 'eax' covers the 'ah' register,
        /// but the 'ah' register doesn't cover 'eax'.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public abstract bool Covers(Storage that);

        public virtual bool Exceeds(Storage that)
        {
            throw new NotImplementedException(string.Format("Exceeds not implemented for {0}.", that.GetType().Name));
        }

        public virtual BitRange GetBitRange()
        {
            return new BitRange(0, (int)BitSize);
        }

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

        public class ArrayComparer : IEqualityComparer<Storage[]>
        {
            public bool Equals(Storage[] x, Storage[] y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                if (x.Length != y.Length)
                    return false;
                for (int i = 0; i < x.Length; ++i)
                {
                    if (!x[i].Equals(y[i]))
                        return false;
                }
                return true;
            }

            public int GetHashCode(Storage[] obj)
            {
                var h = 0;
                for (int i = 0; i < obj.Length; ++i)
                {
                    h = h * 23 ^ obj[i].GetHashCode();
                }
                return h;
            }
        }

    }

    public enum StorageDomain
    {
        None = -1,
        Register = 0,   // Few architectures have 4096 registers (fingers xD)
        Memory = 4096, // Refers to a memory space
        FpuStack = 4098,
        Global = 8191,          // Global variable within a memory space
        SystemRegister = 8192,  // Space for system / control registers
        Stack = 32768,
        Temporary = 65536,
    }

    /// <summary>
    /// This class represents groups of bits stored in flag registers. 
    /// Typically, these are the Carry, Zero, Overflow etc flags that are set
    /// after ALU operations.
    /// </summary>
	public class FlagGroupStorage : Storage
    {
        public FlagGroupStorage(RegisterStorage freg, uint grfMask, string name, DataType dataType) : base("FlagGroup", dataType)
        {
            this.Domain = freg.Domain;
            this.FlagRegister = freg;
            this.FlagGroupBits = grfMask;
            this.Domain = freg.Domain;
            this.Name = name;
            this.BitSize = (uint)dataType.BitSize;
        }

        /// <summary>
        /// The architectural register in which the flag group bits are
        /// located.
        /// </summary>
        public RegisterStorage FlagRegister { get; private set; }

        /// <summary>
        /// Combined bit mask of the flag group bits.
        /// </summary>
        public uint FlagGroupBits { get; private set; }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitFlagGroupStorage(this);
        }

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitFlagGroupStorage(this, context);
        }

        public override bool Covers(Storage sThat)
        {
            var that = sThat as FlagGroupStorage;
            if (that == null || this.FlagRegister != that.FlagRegister)
                return false;
            return (this.FlagGroupBits | that.FlagGroupBits) == this.FlagGroupBits;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FlagGroupStorage that))
                return false;
            return this.FlagGroupBits == that.FlagGroupBits;
        }

        public override bool Exceeds(Storage sThat)
        {
            var that = sThat as FlagGroupStorage;
            if (that == null || this.FlagRegister != that.FlagRegister)
                return false;
            return (this.FlagGroupBits & ~that.FlagGroupBits) != 0;
        }

        public IEnumerable<uint> GetFlagBitMasks()
        {
            for (uint bitMask = 1; bitMask <= FlagGroupBits; bitMask <<= 1)
            {
                if ((FlagGroupBits & bitMask) != 0)
                {
                    yield return bitMask;
                }
            }
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ FlagGroupBits.GetHashCode();
        }

        public override int OffsetOf(Storage stgSub)
        {
            if (!(stgSub is FlagGroupStorage f))
                return -1;
            return ((f.FlagGroupBits & FlagGroupBits) != 0) ? 0 : -1;
        }

        public override bool OverlapsWith(Storage sThat)
        {
            var that = sThat as FlagGroupStorage;
            if (that == null || this.FlagRegister != that.FlagRegister)
                return false;
            return (this.FlagGroupBits & that.FlagGroupBits) != 0;
        }

        public override SerializedKind Serialize()
        {
            return new FlagGroup_v1(Name);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write(Name);
        }
    }

    /// <summary>
    /// Used to model locations in the x87 FPU stack.
    /// </summary>
    public class FpuStackStorage : Storage
    {
        public FpuStackStorage(int depth, DataType dataType) : base("FpuStack", dataType)
        {
            this.FpuStackOffset = depth;
            this.Domain = (StorageDomain)(StorageDomain.FpuStack + depth);
            if (FpuStackOffset >= 0)
            {
                Name = string.Format("FPU +{0}", FpuStackOffset);
            }
            else
            {
                Name = string.Format("FPU -{0}", -FpuStackOffset);
            }
        }

        public override ulong BitSize { get { return (ulong)DataType.BitSize; } }

        public int FpuStackOffset { get; private set; }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitFpuStackStorage(this);
        }

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitFpuStackStorage(this, context);
        }

        public override bool Covers(Storage other)
        {
            return other is FpuStackStorage that &&
                this.FpuStackOffset == that.FpuStackOffset;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FpuStackStorage that))
                return false;
            return this.FpuStackOffset == that.FpuStackOffset;
        }

        public override bool Exceeds(Storage that)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ FpuStackOffset.GetHashCode();
        }

        public override int OffsetOf(Storage stgSub)
        {
            return -1;
        }

        public override bool OverlapsWith(Storage sThat)
        {
            var that = sThat as FpuStackStorage;
            return that != null &&
                this.FpuStackOffset == that.FpuStackOffset;
        }

        public override void Write(TextWriter writer)
        {
            if (FpuStackOffset >= 0)
            {
                writer.Write("FPU +{0}", FpuStackOffset);
            }
            else
            {
                writer.Write("FPU -{0}", -FpuStackOffset);
            }
        }
    }

    /// <summary>
    /// Storage is some unspecified part of global memory.
    /// </summary>
    public class MemoryStorage : Storage
    {
        public MemoryStorage(string name, StorageDomain domain) : base(name, null)
        {
            this.Name = name;
            this.Domain = domain;
            this.BitAddress = 0;
            this.BitSize = 1;
        }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitMemoryStorage(this);
        }

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitMemoryStorage(this, context);
        }

        public override bool Covers(Storage that)
        {
            return true;
        }

        public override bool Exceeds(Storage that)
        {
            return false;
        }

        public override int OffsetOf(Storage stgSub)
        {
            return -1;
        }

        public override bool OverlapsWith(Storage that)
        {
            return that is MemoryStorage;
        }

        public override void Write(TextWriter writer)
        {
            writer.Write(Name);
        }

        public static MemoryStorage Instance { get; private set; }

        static MemoryStorage()
        {
            Instance = new MemoryStorage("Mem", StorageDomain.Memory);
        }
    }

    /// <summary>
    /// Storage for registers or other identifiers that are live-out of a procedure.
    /// </summary>
    public class OutArgumentStorage : Storage
    {
        public OutArgumentStorage(Identifier originalId) : base("out", originalId.DataType)
        {
            this.OriginalIdentifier = originalId;
        }

        public Identifier OriginalIdentifier { get; private set; }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitOutArgumentStorage(this);
        }

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitOutArgumentStorage(this, context);
        }

        public override bool Covers(Storage that)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is OutArgumentStorage that))
                return false;
            return this.OriginalIdentifier.Equals(that.OriginalIdentifier);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ OriginalIdentifier.GetHashCode();
        }

        public override int OffsetOf(Storage stgSub)
        {
            return -1;
        }

        public override bool OverlapsWith(Storage that)
        {
            throw new NotImplementedException();
        }

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
	public class RegisterStorage : Storage, IComparable<RegisterStorage>
    {
        public RegisterStorage(string regName, int number, uint bitAddress, PrimitiveType dataType) : base("Register", dataType)
        {
            this.Name = regName;
            this.Number = number;
            this.BitAddress = bitAddress;
            this.Domain = (StorageDomain)(number + (int)StorageDomain.Register);
            int bitSize = dataType.BitSize;
            if (bitSize == 64)
            {
                BitMask = ~0ul;
            }
            else
            {
                BitMask = ((1ul << bitSize) - 1) << (int)bitAddress;
            }
        }

        public static RegisterStorage Reg8(string name, int number)
        {
            return new RegisterStorage(name, number, 0, PrimitiveType.Byte);
        }

        public static RegisterStorage Reg16(string name, int number)
        {
            return new RegisterStorage(name, number, 0, PrimitiveType.Word16);
        }

        public static RegisterStorage Reg32(string name, int number)
        {
            return new RegisterStorage(name, number, 0, PrimitiveType.Word32);
        }

        public static RegisterStorage Reg64(string name, int number)
        {
            return new RegisterStorage(name, number, 0, PrimitiveType.Word64);
        }

        // Create a system register.
        public static RegisterStorage Sysreg(string name, int number, PrimitiveType size)
        {
            return new RegisterStorage(name, number + (int)StorageDomain.SystemRegister, 0, size);
        }

        public override ulong BitSize
        {
            get { return (ulong)DataType.BitSize; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Bitmask used to extract subregister values from a larger backing register.
        /// </summary>
        public ulong BitMask { get; private set; }

        /// <summary>
        /// The size and domain of the register.
        /// </summary>
        /// <remarks>
        /// General-purpose registers can use the Domain.Word </remarks>
        public new PrimitiveType DataType => (PrimitiveType) base.DataType;

        public int Number { get; private set; }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitRegisterStorage(this);
        }

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitRegisterStorage(this, context);
        }

        public override bool Covers(Storage sThat)
        {
            var that = sThat as RegisterStorage;
            if (that == null || that.Domain != this.Domain)
                return false;
            return (this.BitMask | that.BitMask) == this.BitMask;
        }

        public override bool Exceeds(Storage sThat)
        {
            var that = sThat as RegisterStorage;
            if (that == null || that.Domain != this.Domain)
                return false;
            return (this.BitMask & ~that.BitMask) != 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RegisterStorage that))
                return false;
            return this.Domain == that.Domain &&
                this.BitAddress == that.BitAddress &&
                this.BitSize == that.BitSize;
        }

        public override int GetHashCode()
        {
            return (int)Domain * 17 ^ BitAddress.GetHashCode() ^ BitSize.GetHashCode();
        }

        public override BitRange GetBitRange()
        {
            int bitOffset = (int)BitAddress;
            int bitSize = (int)BitSize;
            return new BitRange(bitOffset, bitOffset + bitSize);
        }

        /// <summary>
        /// Returns true if this register is strictly contained by reg2.
        /// </summary>
        /// <param name="reg1"></param>
        /// <param name="reg2"></param>
        /// <returns></returns>
        public bool IsSubRegisterOf(RegisterStorage reg2)
        {
            if (this.BitSize >= reg2.BitSize)
                return false;
            return this.OverlapsWith(reg2);
        }

        public override int OffsetOf(Storage stgSub)
        {
            if (!(stgSub is RegisterStorage that))
                return -1;
            if (!this.OverlapsWith(that))
                return -1;
            return (int)that.BitAddress;
        }

        public override bool OverlapsWith(Storage sThat)
        {
            var that = sThat as RegisterStorage;
            if (that == null || this.Domain != that.Domain)
                return false;
            var thisStart = this.BitAddress;
            var thisEnd = this.BitAddress + this.BitSize;
            var thatStart = that.BitAddress;
            var thatEnd = that.BitAddress + that.BitSize;
            return thisStart < thatEnd && thatStart < thisEnd;
        }

        /// <summary>
		/// Given a register, sets/resets the bits corresponding to the register
		/// and any other registers it aliases.
		/// </summary>
		/// <param name="iReg">Register to set</param>
		/// <param name="bits">BitSet to modify</param>
		/// <param name="f">truth value to set</param>
		public virtual void SetAliases(ISet<RegisterStorage> bitset, bool f)
        {
            if (f)
                bitset.Add(this);
            else
                bitset.Remove(this);
        }

        public override SerializedKind Serialize()
        {
            return new Register_v1(Name);
        }

        public virtual void SetRegisterStateValues(Expression value, bool isValid, Dictionary<Storage, Expression> ctx)
        {
            ctx[this] = value;
        }

        public int SubregisterOffset(RegisterStorage subReg)
        {
            if (subReg is RegisterStorage sub && Number == sub.Number)
                return 0;
            return -1;
        }

        public override void Write(TextWriter writer)
        {
            writer.Write(Name);
        }

        public static RegisterStorage None { get; } = 
            new RegisterStorage("None", -1, 0, PrimitiveType.Create(Types.Domain.Any, 0))
            {
                Domain = StorageDomain.None,
            };

        public Expression GetSlice(Expression value)
        {
            if (value is Constant c && c.IsValid && !c.IsReal)
            {
                var newValue = (c.ToUInt64() & this.BitMask) >> (int)this.BitAddress;
                return Constant.Create(this.DataType, newValue);
            }
            else
                return Constant.Invalid;
        }

        public int CompareTo(RegisterStorage that)
        {
            var d = this.Domain.CompareTo(that.Domain);
            if (d != 0)
                return d;
            return this.BitMask.CompareTo(that.BitMask);
        }
    }

    public class SequenceStorage : Storage
    {
        public SequenceStorage(params Storage[] elements) : this(
            PrimitiveType.CreateWord(elements.Sum(e => (int)e.BitSize)), 
            elements)
        {
        }

        public SequenceStorage(DataType dt, params Storage [] elements)
            : this(string.Join(":", elements.Select(e => e.Name)), dt, elements)
        {
        }

        public SequenceStorage(string name, DataType dt, params Storage [] elements)
            : base("Sequence", dt)
        {
            this.Elements = elements;
            this.BitSize = (ulong) dt.BitSize;
            this.Name = name;
        }

        public Storage[] Elements { get; }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitSequenceStorage(this);
        }

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitSequenceStorage(this, context);
        }

        public override bool Covers(Storage that)
        {
            if (this == that)
                return true;
            foreach (var e in Elements)
            {
                if (e.Domain == that.Domain)
                {
                    return e.Covers(that);
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SequenceStorage that))
                return false;
            if (this.Elements.Length != that.Elements.Length)
                return false;
            for (int i = 0; i < this.Elements.Length; ++i)
            { 
                if (!this.Elements[i].Equals(that.Elements[i]))
                    return false;
            }
            return true;
        }

        public override bool Exceeds(Storage that)
        {
            if (this == that)
                return true;
            for (int i = 0; i < this.Elements.Length; ++i)
            {
                var e = this.Elements[i];
                if (e.Domain == that.Domain)
                    return e.Exceeds(that);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^
                Elements.Length.GetHashCode() * 9 ^
                Elements[0].GetHashCode() * 17 ^
                Elements[1].GetHashCode() * 33;
        }

        public override int OffsetOf(Storage stgSub)
        {
            int offPrev = 0;
            for (int i = Elements.Length - 1; i >= 0; --i)
            {
                var e = Elements[i];
                int off = e.OffsetOf(stgSub);
                if (off != -1)
                    return off + offPrev;
                offPrev += (int)e.BitSize;
            }
            return -1;
        }

        public override bool OverlapsWith(Storage that)
        {
            if (this == that)
                return true;
            foreach (var stg in Elements)
            {
                if (stg.Domain == that.Domain)
                {
                    return stg.OverlapsWith(that);
                }
            }
            return false;
        }

        public override SerializedKind Serialize()
        {
            return new SerializedSequence(this);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write("Sequence {0}", Name);
        }
    }

    public abstract class StackStorage : Storage
    {
        public StackStorage(string kind, int cbOffset, DataType dt)
            : base(kind, dt)
        {
            this.StackOffset = cbOffset;
            this.BitSize = (uint)dt.BitSize;
            this.Domain = StorageDomain.Stack + cbOffset;
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

        public override bool OverlapsWith(Storage other)
        {
            if (!(other is StackStorage that))
                return false;
            var thisStart = this.StackOffset * DataType.BitsPerByte;
            var thisEnd = thisStart + (int)this.BitSize;
            var thatStart = that.StackOffset * DataType.BitsPerByte;
            var thatEnd = thatStart + (int)that.BitSize;
            return thisStart < thatEnd && thatStart < thisEnd;
        }

        public override bool Covers(Storage other)
        {
            if (!(other is StackStorage that))
                return false;
            var thisStart = this.StackOffset * DataType.BitsPerByte;
            var thisEnd = thisStart + (int)this.BitSize;
            var thatStart = that.StackOffset * DataType.BitsPerByte;
            var thatEnd = thatStart + (int)that.BitSize;
            return thisStart <= thatStart && thatEnd <= thisEnd;
        }
    }

    public class StackArgumentStorage : StackStorage
    {
        public StackArgumentStorage(int cbOffset, DataType dataType) : base("Stack", cbOffset, dataType)
        {
        }

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
            if (!(obj is StackArgumentStorage that))
                return false;
            return this.StackOffset == that.StackOffset;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ StackOffset;
        }

        public override int OffsetOf(Storage stgSub)
        {
            if (!(stgSub is StackArgumentStorage that))
                return -1;
            if (that.StackOffset >= this.StackOffset && that.StackOffset + that.DataType.Size <= StackOffset + DataType.Size)
                return (that.StackOffset - this.StackOffset) * DataType.BitsPerByte;
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
            : base("Local", cbOffset, dataType)
        {
        }

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
            if (!(obj is StackLocalStorage that))
                return false;
            return this.StackOffset == that.StackOffset;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ StackOffset;
        }

        public override int OffsetOf(Storage stgSub)
        {
            if (!(stgSub is StackLocalStorage that))
                return -1;
            if (that.StackOffset >= this.StackOffset && that.StackOffset + that.DataType.Size <= this.StackOffset + DataType.Size)
                return (that.StackOffset - this.StackOffset) * DataType.BitsPerByte;
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
	public class TemporaryStorage : Storage
    {
        protected TemporaryStorage(string name, StorageDomain domain, DataType dt)
            : base("Temporary", dt)
        {
            Domain = domain;
            Name = name;
            BitSize = (uint)dt.BitSize;
        }

        public TemporaryStorage(string name, int number, DataType dt)
            : this(name, StorageDomain.Temporary + number, dt)
        {
        }

        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitTemporaryStorage(this);
        }

        public override T Accept<C, T>(StorageVisitor<C, T> visitor, C context)
        {
            return visitor.VisitTemporaryStorage(this, context);
        }

        public override bool Covers(Storage that)
        {
            return ReferenceEquals(this, that);
        }

        public override bool Exceeds(Storage that)
        {
            return false;
        }

        public override int OffsetOf(Storage stgSub)
        {
            return -1;
        }

        public override bool OverlapsWith(Storage that)
        {
            return ReferenceEquals(this, that);
        }

        public override void Write(TextWriter writer)
        {
            writer.Write(Name);
        }
    }

    public class GlobalStorage : TemporaryStorage
    {
        public GlobalStorage(string name, DataType dt)
            : base(name, StorageDomain.Global, dt)
        {
        }
    }
}
