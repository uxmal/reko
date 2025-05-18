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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// A <see cref="Storage"/> is a value type that encapsulates 
    /// architecture-dependent storage locations for <see cref="Identifier"/>s.
    /// </summary>
    public abstract class Storage
    {
        /// <summary>
        /// Initializes an instance of <see cref="Storage"/>.
        /// </summary>
        /// <param name="domain"><see cref="StorageDomain"/> of this storage.</param>
        /// <param name="name">The name of the storage.</param>
        /// <param name="dataType">The size of the storage.</param>
        public Storage(StorageDomain domain, string name, DataType dataType)
        {
            this.Domain = domain;
            this.DataType = dataType;
            this.Name = name;
        }

        /// <summary>
        /// Textual representation of the kind of this storage.
        /// </summary>
        public abstract string Kind { get; }

        /// <summary>
        /// The storage domain for this storage. 
        /// </summary>
        public StorageDomain Domain { get; }

        /// <summary>
        /// The starting bit position of this storage.
        /// </summary>
        public ulong BitAddress { get; set; }

        /// <summary>
        /// Size of this storage, in bits.
        /// </summary>
        public virtual ulong BitSize { get; set; }

        /// <summary>
        /// The data type of the storage.
        /// </summary>
        public DataType DataType { get; }

        /// <summary>
        /// The name of this storage.
        /// </summary>
        public string Name { get; }


        /// <summary>
        /// Accepts a visitor and calls the appropriate method on it.
        /// </summary>
        /// <typeparam name="T">Return type from the visitor.</typeparam>
        /// <param name="visitor">Visitor to accept.</param>
        /// <returns>Value returned from visitor.</returns>
        public abstract T Accept<T>(StorageVisitor<T> visitor);

        /// <summary>
        /// Accept a context-sensitive <see cref="StorageVisitor{T}"/>.
        /// </summary>
        /// <typeparam name="T">Return value from the visitor.</typeparam>
        /// <typeparam name="C">Contextual information.</typeparam>
        /// <param name="visitor">A <see cref="StorageVisitor{T, C}"/>.</param>
        /// <param name="context">Any contextual data the visitor requires in 
        /// addition to the <see cref="Storage"/> itself.
        /// </param>
        /// <returns>Values from the visitor.
        /// </returns>
        public abstract T Accept<T, C>(StorageVisitor<T, C> visitor, C context);

        /// <summary>
        /// Returns the bit offset of <paramref name="storage"/> within this 
        /// storage, or -1 if <paramref name="storage"/> is not a part of 
        /// this storage.
        /// </summary>
        /// <param name="storage"></param>
        /// <returns>The offset in bits, or -1.</returns>
        public abstract int OffsetOf(Storage storage);

        /// <summary>
        /// Returns true if the Storage <paramref name="that"/> overlaps with this
        /// Storage.
        /// </summary>
        /// <param name="that">Storage to overlap with.</param>
        public abstract bool OverlapsWith(Storage that);

        /// <summary>
        /// A storage <code>a</code> is said to cover a storage 
        /// <code>b</code> if they alias a location, but after 
        /// assigning a with b, some of the original contents of 
        /// a still peek through.
        /// For instance, the x86 register 'eax' covers the 'ah' register,
        /// but the 'ah' register doesn't cover 'eax'.
        /// </summary>
        public abstract bool Covers(Storage that);

        /// <summary>
        /// A storage <c>a</c> is said to exceed a storage <c>b</c> if they alias a location,
        /// but after assigning <c>a</c> with <c>b</c>, some of the original contents of <c>a</c>
        /// aren't overwritten by <c>b</c>.
        /// </summary>
        /// <param name="that"></param>
        /// <returns>True </returns>
        public virtual bool Exceeds(Storage that)
        {
            throw new NotImplementedException($"Exceeds() not implemented for {this.GetType().Name}.");
        }

        /// <summary>
        /// The bit range covered by this storage.
        /// </summary>
        /// <returns></returns>
        public virtual BitRange GetBitRange()
        {
            return new BitRange(0, (int)BitSize);
        }

        /// <summary>
        /// Serializes this storage to a <see cref="SerializedStorage"/>.
        /// </summary>
        /// <returns>A serialized value.</returns>
        public virtual SerializedStorage Serialize()
        {
            throw new NotImplementedException(this.GetType().Name + ".Serialize() not implemented.");
        }

        /// <summary>
        /// Returns a string representation of this storage.
        /// </summary>
        public override string ToString()
        {
            var w = new StringWriter();
            Write(w);
            return w.ToString();
        }

        /// <summary>
        /// Writes a string representation of this storage to <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        public abstract void Write(TextWriter writer);

        /// <summary>
        /// Compares two arrays of storages for equality.
        /// </summary>
        public class ArrayComparer : IEqualityComparer<Storage[]>
        {
            /// <summary>
            /// Compares two arrays of storages for equality.
            /// </summary>
            public bool Equals(Storage[]? x, Storage[]? y)
            {
                if (x is null && y is null)
                    return true;
                if (x is null || y is null)
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

            /// <summary>
            /// Returns a hash code for the array of storages.
            /// </summary>
            /// <param name="obj">Array of storages to compute the hash for.</param>
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

    /// <summary>
    /// This class represents groups of bits stored in flag registers. 
    /// Typically, these are the Carry, Zero, Overflow etc flags that are set
    /// after ALU operations.
    /// </summary>
	public class FlagGroupStorage : Storage, MachineOperand
    {
        /// <summary>
        /// Constructs a flag group storage.
        /// </summary>
        /// <param name="freg">The backing status register.</param>
        /// <param name="grfMask">The individual bits constituting the flag storage.</param>
        /// <param name="name">The name of the flag group.</param>
        public FlagGroupStorage(RegisterStorage freg, uint grfMask, string name)
            : base(freg.Domain, name, freg.DataType)
        {
            this.FlagRegister = freg;
            this.FlagGroupBits = grfMask;
            this.BitSize = (uint) freg.DataType.BitSize;
        }

        /// <inheritdoc/>
        public override string Kind => "FlagGroup";

        DataType MachineOperand.DataType
        {
            get => this.DataType;
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// The architectural register in which the flag group bits are
        /// located.
        /// </summary>
        public RegisterStorage FlagRegister { get; }

        /// <summary>
        /// Combined bit mask of the flag group bits.
        /// </summary>
        public uint FlagGroupBits { get; }

        /// <inheritdoc/>
        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitFlagGroupStorage(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(StorageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitFlagGroupStorage(this, context);
        }

        /// <inheritdoc/>
        public override bool Covers(Storage sThat)
        {
            if (sThat is not FlagGroupStorage that || this.FlagRegister != that.FlagRegister)
                return false;
            return (this.FlagGroupBits | that.FlagGroupBits) == this.FlagGroupBits;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not FlagGroupStorage that)
                return false;
            return this.FlagGroupBits == that.FlagGroupBits;
        }

        /// <inheritdoc/>
        public override bool Exceeds(Storage sThat)
        {
            if (sThat is not FlagGroupStorage that || this.FlagRegister != that.FlagRegister)
                return false;
            return (this.FlagGroupBits & ~that.FlagGroupBits) != 0;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ FlagGroupBits.GetHashCode();
        }

        /// <inheritdoc/>
        public override int OffsetOf(Storage stgSub)
        {
            if (stgSub is not FlagGroupStorage f)
                return -1;
            return ((f.FlagGroupBits & FlagGroupBits) != 0) ? 0 : -1;
        }

        /// <inheritdoc/>
        public override bool OverlapsWith(Storage sThat)
        {
            if (sThat is not FlagGroupStorage that || this.FlagRegister != that.FlagRegister)
                return false;
            return (this.FlagGroupBits & that.FlagGroupBits) != 0;
        }

        string MachineOperand.ToString(MachineInstructionRendererOptions options)
        {
            var sr = new StringRenderer();
            Render(sr, options);
            return sr.ToString();
        }

        /// <summary>
        /// Renders the flag group storage as a string to the <see cref="MachineInstructionRenderer"/>.
        /// </summary>
        /// <param name="renderer">Output sink.</param>
        /// <param name="options">Options controlling the rendering.</param>
        private void Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.BeginOperand();
            renderer.WriteString(this.Name);
            renderer.EndOperand();
        }

        void MachineOperand.Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options) =>
            Render(renderer, options);
        
        /// <inheritdoc/>
        public override SerializedStorage Serialize()
        {
            return new FlagGroup_v1(Name);
        }

        /// <inheritdoc/>
        public override void Write(TextWriter writer)
        {
            writer.Write(Name);
        }

        /// <summary>
        /// Returns a string that represents this <see cref="FlagGroupStorage"/>.
        /// </summary>
        /// <param name="options">An instance of <see cref="MachineInstructionRendererOptions"/>
        /// that controls the rendering of the flag group.
        /// </param>
        /// <returns></returns>
        public string ToString(MachineInstructionRendererOptions options) => Name;
    }

    /// <summary>
    /// Used to model locations in the x87 FPU stack.
    /// </summary>
    public class FpuStackStorage : Storage
    {
        /// <summary>
        /// Constructs an instance of <see cref="FpuStackStorage"/>.
        /// </summary>
        /// <param name="depth">The depth within the FPU stack (0..7 inclusive).</param>
        /// <param name="dataType">Data type of the value.</param>
        public FpuStackStorage(int depth, DataType dataType) 
            : base(
                  StorageDomain.FpuStack + depth,
                  MakeName(depth),
                  dataType)
        {
            this.FpuStackOffset = depth;
            MakeName(depth);
        }

        /// <inheritdoc/>
        public override string Kind => "FpuStack";

        private static string MakeName(int depth)
        {
            if (depth >= 0)
            {
                return string.Format("FPU +{0}", depth);
            }
            else
            {
                return string.Format("FPU -{0}", -depth);
            }
        }

        /// <inheritdoc/>
        public override ulong BitSize => (ulong)DataType.BitSize;

        /// <summary>
        /// The offset of this FPU stack location from the top of the FPU stack.
        /// </summary>
        public int FpuStackOffset { get; }

        /// <inheritdoc/>
        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitFpuStackStorage(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(StorageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitFpuStackStorage(this, context);
        }

        /// <inheritdoc/>
        public override bool Covers(Storage other)
        {
            return other is FpuStackStorage that &&
                this.FpuStackOffset == that.FpuStackOffset;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not FpuStackStorage that)
                return false;
            return this.FpuStackOffset == that.FpuStackOffset;
        }

        /// <inheritdoc/>
        public override bool Exceeds(Storage that)
        {
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^ FpuStackOffset.GetHashCode();
        }

        /// <inheritdoc/>
        public override int OffsetOf(Storage stgSub)
        {
            if (stgSub is not FpuStackStorage that)
                return -1;
            if (that.FpuStackOffset != this.FpuStackOffset)
                return -1;
            return 0;
        }

        /// <inheritdoc/>
        public override bool OverlapsWith(Storage sThat)
        {
            return sThat is FpuStackStorage that &&
                this.FpuStackOffset == that.FpuStackOffset;
        }

        /// <inheritdoc/>
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
    /// This class identifies an address space within a program.
    /// </summary>
    /// <remarks>
    /// Instances of <see cref="MemoryAccess"/> need to
    /// indicate what address space is being used to perform the memory access.
    /// On von Neumann architectures, where all of memory is treated equal,
    /// there is only need for the <see cref="GlobalMemory"/>. On
    /// Harvard architectures, where there may be two or more separate address
    /// spaces (e.g. one for instructions and one for data), the corresponding 
    /// <see cref="IProcessorArchitecture"/> implementation must define an 
    /// appropriate MemoryIdentifier for each separate address space. The 
    /// IProcessorarchitecture must then ensure that when RtlInstructions for
    /// memory accesses are generated, they refer to the correct address space.
    /// <para>
    /// Later, SSA analysis will break apart memory access
    /// after each store operation, giving rise to new address space identifiers
    /// MEM1, MEM2 etc. If ambitious, memory alias analysis can be done. In this
    /// case, we will have several MEMx variables before SSA, each MEMx variable
    /// will be an alias class. 
    /// </para>
    /// </remarks>
    public class MemoryStorage : Storage
    {
        /// <summary>
        /// Default instance of memory storage.
        /// </summary>
        public static MemoryStorage Instance { get; } = new MemoryStorage("Mem", StorageDomain.Memory);

        /// <summary>
        /// Identifier using the default memory storage.
        /// </summary>
        public static Identifier GlobalMemory { get; } = new Identifier("Mem0", new UnknownType(), MemoryStorage.Instance);

        /// <summary>
        /// Constructs an instance of <see cref="MemoryStorage"/>.
        /// </summary>
        /// <param name="name">The name of this memory storage.</param>
        /// <param name="domain">Its storage domain.</param>
        public MemoryStorage(string name, StorageDomain domain) : base(domain, name, null!)
        {
            this.BitAddress = 0;
            this.BitSize = 1;
        }

        /// <inheritdoc/>
        public override string Kind => Name;

        /// <inheritdoc/>
        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitMemoryStorage(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(StorageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitMemoryStorage(this, context);
        }

        /// <inheritdoc/>
        public override bool Covers(Storage that)
        {
            return true;
        }

        /// <inheritdoc/>
        public override bool Exceeds(Storage that)
        {
            return false;
        }

        /// <inheritdoc/>
        public override int OffsetOf(Storage stgSub)
        {
            return -1;
        }

        /// <inheritdoc/>
        public override bool OverlapsWith(Storage that)
        {
            return that is MemoryStorage;
        }

        /// <inheritdoc/>
        public override void Write(TextWriter writer)
        {
            writer.Write(Name);
        }
    }

    /// <summary>
    /// Represents a machine register.
    /// </summary>
    /// <remarks>
    /// Registers commonly alias each other (e.g. the X86 registers 'eax' and 'ax').
    /// To achive aliasing, registers are given the same <see cref="StorageDomain"/>,
    /// but differing bit addresses and/or data types.
    /// </remarks>
	public class RegisterStorage : Storage, MachineOperand, IComparable<RegisterStorage>
    {
        /// <summary>
        /// Constructs a register storage.
        /// </summary>
        /// <param name="regName">The name of the register.</param>
        /// <param name="number">Unique number of the register. These do not 
        /// necessarily correspond to the architecture's encoding of the register,
        /// as encodings may overlap.</param>
        /// <param name="bitAddress">Position of the least significant bit in the backking storage.
        /// This allows us to express registers like the X86 <c>ah</c> register as being
        /// in the same storage domain as the <c>rax</c> register, but at a different bit offset
        /// and data size.</param>
        /// <param name="dataType">The size of this register.</param>
        public RegisterStorage(string regName, int number, uint bitAddress, PrimitiveType dataType)
            : base(StorageDomain.Register + number, regName, dataType)
        {
            this.Number = number;
            this.BitAddress = bitAddress;
            int bitSize = dataType.BitSize;
            if (bitSize == 64)
            {
                BitMask = ~0ul;
            }
            else
            {
                BitMask = ((1ul << bitSize) - 1) << (int) bitAddress;
            }
        }

        /// <inheritdoc/>
        public override string Kind => "Register";

        /// <summary>
        /// True if this register is a system register.
        /// </summary>
        public bool IsSystemRegister
        {
            get
            {
                return
                    StorageDomain.SystemRegister <= this.Domain &&
                    this.Domain < StorageDomain.MaxSystemRegister;
            }
        }

        /// <summary>
        /// Factory method to create an 8-bit register.
        /// </summary>
        /// <param name="name">Name of the register.</param>
        /// <param name="number">Identifier for the storage domain of the register.</param>
        /// <param name="bitOffset">Bit offset within the storage domain.</param>
        /// <returns>A new register storage.
        /// </returns>
        public static RegisterStorage Reg8(string name, int number, uint bitOffset = 0)
        {
            return new RegisterStorage(name, number, bitOffset, PrimitiveType.Byte);
        }

        /// <summary>
        /// Factory method to create an 16-bit register.
        /// </summary>
        /// <param name="name">Name of the register.</param>
        /// <param name="number">Identifier for the storage domain of the register.</param>
        /// <param name="bitOffset">Bit offset within the storage domain.</param>
        /// <returns>A new register storage.
        /// </returns>
        public static RegisterStorage Reg16(string name, int number, uint bitOffset = 0)
        {
            return new RegisterStorage(name, number, bitOffset, PrimitiveType.Word16);
        }

        /// <summary>
        /// Factory method to create an 32-bit register.
        /// </summary>
        /// <param name="name">Name of the register.</param>
        /// <param name="number">Identifier for the storage domain of the register.</param>
        /// <param name="bitOffset">Bit offset within the storage domain.</param>
        /// <returns>A new register storage.
        /// </returns>
        public static RegisterStorage Reg32(string name, int number, uint bitOffset = 0)
        {
            return new RegisterStorage(name, number, bitOffset, PrimitiveType.Word32);
        }

        /// <summary>
        /// Factory method to create an 64-bit register.
        /// </summary>
        /// <param name="name">Name of the register.</param>
        /// <param name="number">Identifier for the storage domain of the register.</param>
        /// <param name="bitOffset">Bit offset within the storage domain.</param>
        /// <returns>A new register storage.
        /// </returns>
        public static RegisterStorage Reg64(string name, int number, uint bitOffset = 0)
        {
            return new RegisterStorage(name, number, bitOffset, PrimitiveType.Word64);
        }

        /// <summary>
        /// Creates a system register named <paramref name="name"/>, whose number is 
        /// <paramref name="number"/> and whose size is <paramref name="size"/>.
        /// </summary>
        public static RegisterStorage Sysreg(string name, int number, PrimitiveType size)
        {
            return new RegisterStorage(name, number + (int) StorageDomain.SystemRegister, 0, size);
        }

        /// <summary>
        /// Creates a pseudo register named <paramref name="name"/>, whose number is <paramref name="number"/>
        /// and whose size is <paramref name="size"/>.
        /// </summary>
        public static RegisterStorage PseudoReg(string name, int number, PrimitiveType size)
        {
            return new RegisterStorage(name, number + (int) StorageDomain.PseudoRegister, 0, size);
        }

        /// <summary>
        /// Size of the register, in bits.
        /// </summary>
        public override ulong BitSize
        {
            get { return (ulong) DataType.BitSize; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Bitmask used to extract subregister values from a larger backing register.
        /// </summary>
        public ulong BitMask { get; }

        DataType MachineOperand.DataType
        {
            get => base.DataType;
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Unique register number.
        /// </summary>
        public int Number { get; }

        /// <inheritdoc/>
        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitRegisterStorage(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(StorageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitRegisterStorage(this, context);
        }

        /// <inheritdoc/>
        public override bool Covers(Storage other)
        {
            if (other is not RegisterStorage that || that.Domain != this.Domain)
                return false;
            return (this.BitMask | that.BitMask) == this.BitMask;
        }

        /// <summary>
        /// Returns true if the given <paramref name="range"/> is covered by
        /// this register.
        /// </summary>
        /// <param name="range">Bitrange to test.</param>
        /// <returns>True if this register covers the given bit range.</returns>
        public bool Covers(BitRange range)
        {
            return this.BitAddress <= (ulong)range.Msb &&
                this.BitAddress + BitSize >= (ulong)range.Msb;
        }

        /// <inheritdoc/>
        public override bool Exceeds(Storage sThat)
        {
            if (sThat is not RegisterStorage that || that.Domain != this.Domain)
                return false;
            return (this.BitMask & ~that.BitMask) != 0;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is RegisterStorage that &&
                this.Domain == that.Domain &&
                this.BitAddress == that.BitAddress &&
                this.BitSize == that.BitSize;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (int) Domain * 17 ^ BitAddress.GetHashCode() ^ BitSize.GetHashCode();
        }

        /// <inheritdoc/>
        public override BitRange GetBitRange()
        {
            int bitOffset = (int) BitAddress;
            int bitSize = (int) BitSize;
            return new BitRange(bitOffset, bitOffset + bitSize);
        }

        /// <summary>
        /// Returns true if this register is strictly contained by <paramref name="reg"/>.
        /// </summary>
        /// <param name="reg">Another register to compare with.</param>
        /// <returns>True if this register is strictly contained by 
        /// <paramref name="reg"/>.</returns>
        public bool IsSubRegisterOf(RegisterStorage reg)
        {
            if (this.BitSize >= reg.BitSize)
                return false;
            return this.OverlapsWith(reg);
        }

        /// <inheritdoc/>
        public override int OffsetOf(Storage stgSub)
        {
            if (stgSub is not RegisterStorage that)
                return -1;
            if (!this.Covers(that))
                return -1;
            return (int) (that.BitAddress - this.BitAddress);
        }

        /// <inheritdoc/>
        public override bool OverlapsWith(Storage sThat)
        {
            if (sThat is not RegisterStorage that || this.Domain != that.Domain)
                return false;
            var thisStart = this.BitAddress;
            var thisEnd = this.BitAddress + this.BitSize;
            var thatStart = that.BitAddress;
            var thatEnd = that.BitAddress + that.BitSize;
            return thisStart < thatEnd && thatStart < thisEnd;
        }

        /// <inheritdoc/>
        public override SerializedStorage Serialize()
        {
            return new Register_v1(Name);
        }
        /// <inheritdoc/>
        /// <inheritdoc/>
        public override void Write(TextWriter writer)
        {
            writer.Write(Name);
        }

        /// <summary>
        /// Dummy value to represent the lack of a register.
        /// </summary>
        //$REVIEW: consider simply using null here. With the stronger
        // C# checking for nullness, a 'None' value is no longer needed.
        public static RegisterStorage None { get; } =
            new RegisterStorage("None", -1, 0, PrimitiveType.Create(Types.Domain.Any, 0))
            {
            };


        /// <summary>
        /// Given an expression that is a valid integer bit vector, slice that expression to the bits covered by this register.
        /// </summary>
        /// <param name="value">Expression to be sliced.</param>
        //$REVIEW: only used in unit tests.
        public Expression GetSlice(Expression value)
        {
            if (value is Constant c && c.IsValid && !c.IsReal)
            {
                var newValue = (c.ToUInt64() & this.BitMask) >> (int) this.BitAddress;
                return Constant.Create(this.DataType, newValue);
            }
            else
                return InvalidConstant.Create(this.DataType);
        }

        /// <summary>
        /// Compare this register with another register.
        /// </summary>
        /// <param name="that">Other register.</param>
        public int CompareTo(RegisterStorage? that)
        {
            if (that is null)
                return 1;
            var d = this.Domain.CompareTo(that.Domain);
            if (d != 0)
                return d;
            return this.BitMask.CompareTo(that.BitMask);
        }

        /// <inheritdoc/>
        public DataType Width
        {
            get => this.DataType;
            set => throw new NotSupportedException();
        }

        string MachineOperand.ToString(MachineInstructionRendererOptions options)
        {
            var sr = new StringRenderer();
            Render(sr, options);
            return sr.ToString();
        }

        private void Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.BeginOperand();
            renderer.WriteString(this.Name);
            renderer.EndOperand();
        }

        void MachineOperand.Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options) =>
            Render(renderer, options);
    }

    /// <summary>
    /// Represents a value that is stored sequentially as an 
    /// ordered tuple of sub-storages (like the <c>dx:ax</c> idiom on x86).
    /// Sub-storages are not restricted to being <see cref="RegisterStorage"/>s.
    /// </summary>
    public class SequenceStorage : Storage, MachineOperand
    {
        /// <summary>
        /// Constructs a sequence storage based on indivudual sub-storages.
        /// </summary>
        /// <param name="elements">The substorages to combine, arranged in big-endian order;
        /// that is, the first element in the array is the most significant one.
        /// </param>
        public SequenceStorage(params Storage[] elements) : this(
            PrimitiveType.CreateWord(elements.Sum(e => (int)e.BitSize)), 
            elements)
        {
        }

        /// <summary>
        /// Constructs a sequence storage based on indivudual sub-storages.
        /// </summary>
        /// <param name="dt">Data type of the sequence storage.</param>
        /// <param name="elements">The substorages to combine, arranged in big-endian order;
        /// that is, the first element in the array is the most significant one.
        /// </param>
        public SequenceStorage(DataType dt, params Storage [] elements)
            : this(string.Join(":", elements.Select(e => e.Name)), dt, elements)
        {
        }

        /// <summary>
        /// Constructs a sequence storage based on indivudual sub-storages.
        /// </summary>
        /// <param name="name">Name of the sequence storage.</param>
        /// <param name="dt">Data type of the sequence storage.</param>
        /// <param name="elements">The substorages to combine, arranged in big-endian order;
        /// that is, the first element in the array is the most significant one.
        /// </param>
        public SequenceStorage(string name, DataType dt, params Storage [] elements)
            : base(StorageDomain.None, name, dt)
        {
            this.Elements = elements;
            this.BitSize = (ulong) dt.BitSize;
        }

        /// <summary>
        /// The sub-storages that make up this sequence. The sub-storages are ordered
        /// in big-endian order; the most significant sub-storage is first in the
        /// array.
        /// </summary>
        public Storage[] Elements { get; }

        /// <inheritdoc/>
        public override string Kind => "Sequence";

        DataType MachineOperand.DataType
        {
            get => this.DataType;
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitSequenceStorage(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(StorageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitSequenceStorage(this, context);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not SequenceStorage that)
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GetType().GetHashCode() ^
                Elements.Length.GetHashCode() * 9 ^
                Elements[0].GetHashCode() * 17 ^
                Elements[1].GetHashCode() * 33;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.BeginOperand();
            renderer.WriteString(this.Name);
            renderer.EndOperand();
        }

        /// <inheritdoc/>
        public override SerializedStorage Serialize()
        {
            return new SerializedSequence(this);
        }

        string MachineOperand.ToString(MachineInstructionRendererOptions options)
        {
            var sr = new StringRenderer();
            Render(sr, options);
            return sr.ToString();
        }

        /// <inheritdoc/>
        public override void Write(TextWriter writer)
        {
            writer.Write("Sequence {0}", Name);
        }
    }

    /// <summary>
    /// Represents a value that has been stored at a known location
    /// in the call frame of a procedre.
    /// </summary>
    public class StackStorage : Storage
    {
        /// <summary>
        /// Constructs an instance of <see cref="StackStorage"/>.
        /// </summary>
        /// <param name="offset">offset from the stack frame start.</param>
        /// <param name="dt">Data type of this storage.</param>
        public StackStorage(int offset, DataType dt)
            : base(StorageDomain.Stack + offset, "stack", dt)
        {
            this.StackOffset = offset;
            this.BitSize = (uint)dt.BitSize;
        }

        /// <inheritdoc/>
        public override string Kind => "Stack";

        /// <summary>
        /// Offset from stack pointer as it was when the procedure was entered.
        /// </summary>
        /// <remarks>
        /// If the architecture stores the return address on the stack, the
        /// return address will be at offset 0 and any stack arguments will
        /// have offsets > 0. If the architecture passes the return address in
        /// a register or a separate return stack, there may be stack
        /// arguments with offset 0. Depending on which direction the stack
        /// grows, there may be negative stack offsets for parameters,
        /// although most populargeneral purpose processors (x86, PPC, m68K)
        /// grow their stacks down toward lower memory addresses.
        /// </remarks>
        public int StackOffset { get; }

        /// <inheritdoc/>
        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitStackStorage(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(StorageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitStackStorage(this, context);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not StackStorage that)
                return false;
            return this.StackOffset == that.StackOffset;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return StackOffset.GetHashCode();
        }

        /// <inheritdoc/>
        public override int OffsetOf(Storage stgSub)
        {
            if (stgSub is not StackStorage that)
                return -1;
            if (that.StackOffset >= this.StackOffset &&
                that.StackOffset + that.DataType.Size <= this.StackOffset + DataType.Size)
                return (that.StackOffset - this.StackOffset) * DataType.BitsPerByte;
            return -1;
        }

        /// <inheritdoc/>
        public override bool OverlapsWith(Storage other)
        {
            if (other is not StackStorage that)
                return false;
            var thisStart = this.StackOffset * DataType.BitsPerByte;
            var thisEnd = thisStart + (int)this.BitSize;
            var thatStart = that.StackOffset * DataType.BitsPerByte;
            var thatEnd = thatStart + (int)that.BitSize;
            return thisStart < thatEnd && thatStart < thisEnd;
        }

        /// <inheritdoc/>
        public override bool Covers(Storage other)
        {
            if (other is not StackStorage that)
                return false;
            var thisStart = this.StackOffset * DataType.BitsPerByte;
            var thisEnd = thisStart + (int)this.BitSize;
            var thatStart = that.StackOffset * DataType.BitsPerByte;
            var thatEnd = thatStart + (int)that.BitSize;
            return thisStart <= thatStart && thatEnd <= thisEnd;
        }

        /// <inheritdoc/>
        public override SerializedStorage Serialize()
        {
            return new StackVariable_v1();
        }

        /// <inheritdoc/>
        public override void Write(TextWriter writer)
        {
            var sign = StackOffset >= 0 ? "+" : "-";
            writer.Write("{0} {1}{2:X4}", Kind, sign, Math.Abs(StackOffset));
        }
    }

    /// <summary>
    /// Temporary storage is used for expressing intermediate results that become exposed
    /// when complex machine instructions are broken down into simpler RTL sequences.
    /// These values are not stored in architectural registers, but may represent 
    /// internal registers of the CPU.
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
        /// <summary>
        /// Constructs an instance of <see cref="TemporaryStorage"/>.
        /// </summary>
        /// <param name="name">The name of the temporary storage.</param>
        /// <param name="domain">Storage domain to use for the temporary variable.</param>
        /// <param name="dt">Size of the temporary variable.</param>
        protected TemporaryStorage(string name, StorageDomain domain, DataType dt)
            : base(domain, name, dt)
        {
            BitSize = (uint)dt.BitSize;
        }

        /// <inheritdoc/>
        public override string Kind => "Temporary";

        /// <summary>
        /// Constructs an instance of <see cref="TemporaryStorage"/>.
        /// </summary>
        /// <param name="name">The name of the temporary storage.</param>
        /// <param name="number">Storage domain to use for the temporary variable.</param>
        /// <param name="dt">Size of the temporary variable.</param>
        public TemporaryStorage(string name, int number, DataType dt)
            : this(name, StorageDomain.Temporary + number, dt)
        {
        }

        /// <inheritdoc/>
        public override T Accept<T>(StorageVisitor<T> visitor)
        {
            return visitor.VisitTemporaryStorage(this);
        }

        /// <inheritdoc/>
        public override T Accept<T, C>(StorageVisitor<T, C> visitor, C context)
        {
            return visitor.VisitTemporaryStorage(this, context);
        }

        /// <inheritdoc/>
        public override bool Covers(Storage that)
        {
            return ReferenceEquals(this, that);
        }

        /// <inheritdoc/>
        public override bool Exceeds(Storage that)
        {
            return false;
        }

        /// <inheritdoc/>
        public override int OffsetOf(Storage stgSub)
        {
            if (ReferenceEquals(this, stgSub))
                return 0;
            else
                return -1;
        }

        /// <inheritdoc/>
        public override bool OverlapsWith(Storage that)
        {
            return ReferenceEquals(this, that);
        }

        /// <inheritdoc/>
        public override void Write(TextWriter writer)
        {
            writer.Write(Name);
        }
    }

    /// <summary>
    /// Global storage is used to represent global variables.
    /// </summary>
    public class GlobalStorage : TemporaryStorage
    {
        /// <summary>
        /// Constructs an instance of <see cref="GlobalStorage"/>.
        /// </summary>
        /// <param name="name">Name of the global.</param>
        /// <param name="dt">Data type of the global.</param>
        public GlobalStorage(string name, DataType dt)
            : base(name, StorageDomain.Global, dt)
        {
        }
    }
}
