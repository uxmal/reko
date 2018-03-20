#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Core;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// This class provides support for the PIC registers table implementations.
    /// </summary>
    public sealed class PICRegisters : PICRegistersBuilder, IPICRegisterSymTable
    {

        #region Helper classes

        /// <summary>
        /// A sized-register address key.
        /// </summary>
        private class RegSizedAddrKey : IEquatable<RegSizedAddrKey>
        {

            /// <summary>
            /// The register address if memory-mapped.
            /// </summary>
            public readonly PICDataAddress Addr;

            /// <summary>
            /// The register ID is non-memory-mapped.
            /// </summary>
            public readonly string NMMRID;

            /// <summary>
            /// The bit width of the register.
            /// </summary>
            public readonly int BitWidth;

            /// <summary>
            /// Instantiates a new sized-register address with given PIC data memory address.
            /// </summary>
            /// <param name="regAddr">The register PIC data memory address.</param>
            /// <param name="width">The bit width of the register.</param>
            public RegSizedAddrKey(PICDataAddress regAddr, int width = -1)
            {
                Addr = regAddr;
                BitWidth = width;
                NMMRID = String.Empty;
            }

            /// <summary>
            /// Instantiates a new sized-register address with given absolute address.
            /// </summary>
            /// <param name="regAddr">The register absolute 16-bit address.</param>
            /// <param name="width">The bit width of the register.</param>
            public RegSizedAddrKey(ushort regAddr, int width = -1)
                : this(PICDataAddress.Ptr(regAddr), width)
            {
            }

            /// <summary>
            /// Instantiates a new sized-register pseudo-address with given non-memory-mapped ID.
            /// </summary>
            /// <param name="nmmrID">The register ID if non-memory-mapped.</param>
            /// <param name="width">The bit width of the register.</param>
            public RegSizedAddrKey(string nmmrID, int width = -1)
            {
                Addr = null;
                NMMRID = nmmrID;
                BitWidth = width;
            }

            #region IEquatable implementation

            public bool Equals(RegSizedAddrKey other)
            {
                if (other is null)
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                if (Addr is null)
                {
                    if (NMMRID != other.NMMRID)
                        return false;
                }
                else
                {
                    if (Addr != other.Addr)
                        return false;
                }

                return (BitWidth == -1) || (other.BitWidth == -1) || (BitWidth == other.BitWidth);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as RegSizedAddrKey);
            }

            public override int GetHashCode()
            {
                return (Addr?.GetHashCode() ?? 0 * 17) ^ NMMRID.GetHashCode() ^ (BitWidth.GetHashCode() * 29);
            }

            #endregion

            public override string ToString()
            {
                if (Addr != null)
                    return $"{Addr}[{BitWidth}]";
                return $"NMMRID({NMMRID}[{BitWidth}])";
            }

        }

        /// <summary>
        /// A bit field address composed of the containing register's address and bit field position.
        /// </summary>
        private class BitFieldAddrKey : IEquatable<BitFieldAddrKey>
        {
            public readonly PICDataAddress RegAddr;
            public readonly ulong BitPos;

            /// <summary>
            /// Instantiates a new bit-field address with given holding register address and bit-field position.
            /// </summary>
            /// <param name="regaddr">The register PIC data memory address.</param>
            /// <param name="bitpos">The bit position of the bit-field.</param>
            public BitFieldAddrKey(PICDataAddress regaddr, ulong bitpos)
            {
                RegAddr = regaddr;
                BitPos = bitpos;
            }

            public bool Equals(BitFieldAddrKey other)
            {
                if (other is null)
                    return false;
                return RegAddr.Equals(other.RegAddr) && BitPos.Equals(other.BitPos);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as BitFieldAddrKey);
            }

            public override int GetHashCode()
            {
                return (RegAddr.GetHashCode() * 17) ^ BitPos.GetHashCode();
                ;
            }

            public override string ToString()
            {
                return $"{RegAddr}.bit{BitPos}";
            }
        }

        /// <summary>
        /// List of bit fields. Permits to get bit fields with different widths at same register's bit position.
        /// </summary>
        private class BitFieldList : SortedList<uint, PICBitFieldStorage>
        {
        }

        #endregion

        private static object symTabLock = new object(); // lock to allow concurrent access.

        private static Dictionary<string, PICRegisterStorage> UniqueRegNames
            = new Dictionary<string, PICRegisterStorage>();
        private static Dictionary<string, PICBitFieldStorage> UniqueFieldNames
            = new Dictionary<string, PICBitFieldStorage>();
        private static Dictionary<RegSizedAddrKey, PICRegisterStorage> UniqueRegsAddr
            = new Dictionary<RegSizedAddrKey, PICRegisterStorage>();
        private static Dictionary<BitFieldAddrKey, BitFieldList> UniqueBitFieldsAddr
            = new Dictionary<BitFieldAddrKey, BitFieldList>();
        private static Dictionary<RegisterStorage, Dictionary<uint, RegisterStorage>> JoineeRegisters
            = new Dictionary<RegisterStorage, Dictionary<uint, RegisterStorage>>();
        private static Dictionary<RegisterStorage, RegisterStorage> JoinedRegisters
            = new Dictionary<RegisterStorage, RegisterStorage>();

        /// <summary>
        /// Instantiates a new empty registers pool.
        /// </summary>
        private PICRegisters() : base()
        {
            Reset();
        }

        /// <summary>
        /// Loads the PIC registers into the registers symbol table.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pic"/> is null.</exception>
        public static void LoadRegisters(PIC pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            var regs = new PICRegisters();
            regs.LoadRegistersInTable(regs, pic);
            HWStackDepth = pic.ArchDef.MemTraits.HWStackDepth;
        }

        private void Reset()
        {
            lock (symTabLock)
            {
                UniqueRegNames.Clear();
                UniqueFieldNames.Clear();
                UniqueRegsAddr.Clear();
                UniqueBitFieldsAddr.Clear();
                JoineeRegisters.Clear();
                JoinedRegisters.Clear();
            }
        }


        #region IPICRegisterSymTable interface

        /// <summary>
        /// Adds a PIC register to the registers symbol table. Returns null if no addition done.
        /// </summary>
        /// <param name="reg">The register.</param>
        /// <returns>
        /// A <seealso cref="PICRegisterStorage"/> or null if tentative of duplication.
        /// </returns>
        PICRegisterStorage IPICRegisterSymTable.AddRegister(PICRegisterStorage reg)
        {
            if (reg is null)
                return null;

            RegSizedAddrKey addr =
                (reg.IsMemoryMapped
                    ? new RegSizedAddrKey(reg.Address, reg.BitWidth)
                    : new RegSizedAddrKey(reg.NMMRID, reg.BitWidth)
                );

            var subdic = new Dictionary<uint, RegisterStorage>();
            reg.SubRegs?.ToList().ForEach(r => subdic.Add(((uint)(r.BitWidth << 8) | (uint)r.BitAddress), r));

            lock (symTabLock)
            {
                if (UniqueRegNames.ContainsKey(reg.Name))
                    return null;      // Do not duplicate name
                if (UniqueRegsAddr.ContainsKey(addr))
                    return null;   // Do not duplicate register with same address and bit width
                UniqueRegNames[reg.Name] = reg;
                UniqueRegsAddr[addr] = reg;
                if (reg.SubRegs != null)
                {
                    JoineeRegisters.Add(reg, subdic);
                    reg.SubRegs?.ToList().ForEach(sr => JoinedRegisters.Add(sr, reg));
                }
            }
            return reg;
        }

        /// <summary>
        /// Adds a register's named bit field. Returns null if no addition done.
        /// </summary>
        /// <param name="reg">The parent register.</param>
        /// <param name="field">The bit field.</param>
        /// <returns>
        /// A <seealso cref="PICBitFieldStorage"/> or null if tentative of duplication.
        /// </returns>
        PICBitFieldStorage IPICRegisterSymTable.AddRegisterField(PICRegisterStorage reg, PICBitFieldStorage field)
        {
            lock (symTabLock)
            {
                if (UniqueRegNames.ContainsKey(field.Name))
                    return null;      // Do not duplicate name
                var key = new BitFieldAddrKey(reg.Address, field.BitPos);
                if (!UniqueBitFieldsAddr.ContainsKey(key))
                {
                    var newfieldlist = new BitFieldList
                    {
                        { field.BitWidth, field }
                    };
                    UniqueFieldNames[field.Name] = field;
                    UniqueBitFieldsAddr[key] = newfieldlist;
                    return field;
                }
                var fieldlist = UniqueBitFieldsAddr[key];
                if (!fieldlist.ContainsKey(field.BitWidth))
                {
                    UniqueFieldNames[field.Name] = field;
                    fieldlist.Add(field.BitWidth, field);
                    return field;
                }
            }
            return null;
        }

        #endregion

        #region PICRegisters API

        /// <summary>
        /// Gets the depth of the hardware stack.
        /// </summary>
        public static int HWStackDepth { get; private set; }

        /// <summary>
        /// Gets all the defined PIC registers.
        /// </summary>
        /// <value>
        /// An array of PIC registers.
        /// </value>
        public static PICRegisterStorage[] GetRegisters => UniqueRegsAddr.Values.ToArray();

        /// <summary>
        /// Gets a PIC register from its name.
        /// </summary>
        /// <param name="regName">The name of the register as a string.</param>
        /// <returns>
        /// The PIC register instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if this register does not exist.</exception>
        public static PICRegisterStorage GetRegister(string regName)
        {
            lock (symTabLock)
            {
                if (!UniqueRegNames.TryGetValue(regName, out PICRegisterStorage reg))
                    throw new ArgumentException("Unknown PIC register.", regName);
                return reg;
            }
        }

        /// <summary>
        /// Attempts to get a PIC register from its name.
        /// </summary>
        /// <param name="regName">The name of the register as a string.</param>
        /// <param name="reg">[out] The PIC register if it exists.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public static bool TryGetRegister(string regName, out PICRegisterStorage reg, PICRegisterStorage defltReg = null)
        {
            reg = defltReg;
            lock (symTabLock)
                return UniqueRegNames.TryGetValue(regName, out reg);
        }

        /// <summary>
        /// Gets a register by its data memory address/bit-width.
        /// </summary>
        /// <param name="regDataAddr">The data memory address of the register.</param>
        /// <param name="bitWidth">The bit width of the register.</param>
        /// <returns>
        /// The PIC register instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if this register with given width does not exist.</exception>
        public static PICRegisterStorage GetRegister(PICDataAddress regDataAddr, int bitWidth)
        {
            lock (symTabLock)
            {
                if (!UniqueRegsAddr.TryGetValue(new RegSizedAddrKey(regDataAddr, bitWidth), out var reg))
                    throw new InvalidOperationException($"No special register at address 0x{regDataAddr:X} / {bitWidth} bit-wide.");
                return reg;
            }

        }

        /// <summary>
        /// Attempts to get a register by its data memory address/bit-width.
        /// </summary>
        /// <param name="regDataAddr">The data memory address of the register.</param>
        /// <param name="bitWidth">The bit width of the register.</param>
        /// <param name="reg">[out] The PIC register if it exists.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public static bool TryGetRegister(PICDataAddress regDataAddr, int bitWidth, out PICRegisterStorage reg)
        {
            lock (symTabLock)
            {
                return UniqueRegsAddr.TryGetValue(new RegSizedAddrKey(regDataAddr, bitWidth), out reg);
            }

        }

        /// <summary>
        /// Gets a register by its absolute address/bit-width.
        /// </summary>
        /// <param name="regAbsAddr">The absolute address of the register.</param>
        /// <param name="bitWidth">The bit width of the register.</param>
        /// <returns>
        /// The PIC register instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if this register with given width does not exist.</exception>
        public static PICRegisterStorage GetRegister(ushort regAbsAddr, int bitWidth)
            => GetRegister(PICDataAddress.Ptr(regAbsAddr), bitWidth);

        /// <summary>
        /// Attempts to get a register by its absolute address/bit-width.
        /// </summary>
        /// <param name="regAbsAddr">The absolute address of the register.</param>
        /// <param name="bitWidth">The bit width of the register.</param>
        /// <returns>
        /// The PIC register instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if this register with given width does not exist.</exception>
        public static bool TryGetRegister(ushort regAbsAddr, int bitWidth, out PICRegisterStorage reg)
            => TryGetRegister(PICDataAddress.Ptr(regAbsAddr), bitWidth, out reg);

        /// <summary>
        /// Gets a register by its index number.
        /// </summary>
        /// <param name="number">Index number of the register.</param>
        /// <returns>
        /// The register instance or null.
        /// </returns>
        public static PICRegisterStorage PeekRegisterByNum(int number)
        {
            lock (symTabLock)
            {
                var reg = UniqueRegsAddr.Where(l => l.Value.Number == number)
                    .Select(e => (KeyValuePair<RegSizedAddrKey, PICRegisterStorage>?)e)
                    .FirstOrDefault()?.Value;
                return reg;
            }

        }

        /// <summary>
        /// Gets a standard (core) register by its index.
        /// </summary>
        /// <param name="i">Zero-based index of the register.</param>
        /// <returns>
        /// The register or <seealso cref="RegisterStorage.None"/>.
        /// </returns>
        public static PICRegisterStorage PeekRegisterByIdx(int i)
        {
            lock (symTabLock)
            {
                var entry = UniqueRegsAddr.Where(p => p.Value.Number == i).Select(e => e.Value).FirstOrDefault();
                return entry;
            }
        }

        /// <summary>
        /// Gets a register bit-field by its name.
        /// </summary>
        /// <param name="name">The name of the bit-field as a string.</param>
        /// <returns>
        /// The bit-field instance or null.
        /// </returns>
        public static PICBitFieldStorage GetBitField(string fieldName)
        {
            lock (symTabLock)
            {
                if (!UniqueFieldNames.TryGetValue(fieldName, out PICBitFieldStorage fld))
                    throw new InvalidOperationException($"Missing definition of bit field '{fieldName}' in symbol table.");
                return fld;
            }
        }

        /// <summary>
        /// Attempts to get PIC register's bit field from its name.
        /// </summary>
        /// <param name="fieldName">Name of the bit field.</param>
        /// <param name="field">[out] The bit field if it exists, otherwise null.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public static bool TryGetBitField(string fieldName, out PICBitFieldStorage field, PICBitFieldStorage defltFld = null)
        {
            field = defltFld;
            lock (symTabLock)
                return UniqueFieldNames.TryGetValue(fieldName, out field);
        }

        /// <summary>
        /// Gets a register bit-field by its parent register address and bit position/width or null.
        /// If <paramref name="bitwidth"/> is 0, then the widest bit-field is retrieved.
        /// </summary>
        /// <param name="regAddress">The parent register address.</param>
        /// <param name="bitPos">The bit position of the bit-field.</param>
        /// <param name="bitWidth">(Optional) The bit-field width.</param>
        /// <returns>
        /// The bit-field instance or null.
        /// </returns>
        public static PICBitFieldStorage PeekBitFieldFromRegister(PICDataAddress regAddress, uint bitPos, uint bitWidth = 0)
        {
            lock (symTabLock)
            {
                if (UniqueBitFieldsAddr.TryGetValue(new BitFieldAddrKey(regAddress, bitPos), out BitFieldList flist))
                {
                    if (bitWidth == 0)
                        return flist.LastOrDefault().Value;
                    return flist.FirstOrDefault(f => f.Value.BitWidth == bitWidth).Value;
                }
            }
            return null;

        }

        /// <summary>
        /// Gets a register bit field by its parent register and bit position/width.
        /// If <paramref name="bitWidth"/> is 0, then the widest bit-field is retrieved.
        /// </summary>
        /// <param name="reg">The parent register.</param>
        /// <param name="bitPos">The bit position of the bit-field.</param>
        /// <param name="bitWidth">(Optional) The bit-field width.</param>
        /// <returns>
        /// The bit-field instance or null.
        /// </returns>
        public static PICBitFieldStorage PeekBitFieldFromRegister(PICRegisterStorage reg, uint bitPos, uint bitWidth = 0)
            => (reg is null ? null : PeekBitFieldFromRegister(reg.Address, bitPos, bitWidth));

        /// <summary>
        /// Gets a register bit-field by its parent register name and bit position/width or null.
        /// If <paramref name="fldBitWidth"/> is 0, then the widest bit field is retrieved.
        /// </summary>
        /// <param name="regName">The parent register name.</param>
        /// <param name="fldBitPos">The bit position of the bit-field.</param>
        /// <param name="fldBitWidth">(Optional) The bit field width.</param>
        /// <returns>
        /// The bit-field instance or null.
        /// </returns>
        public static PICBitFieldStorage PeekBitFieldFromRegister(string regName, uint fldBitPos, uint fldBitWidth = 0)
            => PeekBitFieldFromRegister(GetRegister(regName), fldBitPos, fldBitWidth);

        /// <summary>
        /// Gets the maximum number of PIC18 registers.
        /// </summary>
        public static int Max => UniqueRegsAddr.Count;

        /// <summary>
        /// Get the proper sub-register of <paramref name="reg" /> that starts at offset
        /// <paramref name="offset" /> and is of size <paramref name="width"/>.
        /// </summary>
        /// <param name="reg">The parent register.</param>
        /// <param name="offset">The bit offset of the sub-register.</param>
        /// <param name="width">The bit width of the sub-register.</param>
        /// <returns>
        /// The sub-register.
        /// </returns>
        /// <remarks>
        /// Most architectures not have sub-registers, and will use this default implementation. This
        /// method is overridden for architectures like x86 and Z80, where sub-registers <code>(ah, al,
        /// etc)</code>
        /// do exist.
        /// </remarks>
        public static RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            if (offset == 0 && reg.BitSize == (ulong)width)
                return reg;
            if (!JoineeRegisters.TryGetValue(reg, out var dict))
                return null;
            if (!dict.TryGetValue((uint)(offset + (width << 8)), out var subReg))
                return null;
            return subReg;
        }

        /// <summary>
        /// Gets widest sub-register.
        /// </summary>
        /// <param name="reg">The parent register.</param>
        /// <param name="regs">The regs.</param>
        /// <returns>
        /// The widest sub-register.
        /// </returns>
        public static RegisterStorage GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> regs)
            => throw new NotImplementedException("PIC has no wider-register.");

        public static RegisterStorage GetParentRegister(RegisterStorage subreg)
        {
            if (subreg is null)
                return null;
            if (!JoinedRegisters.TryGetValue(subreg, out var par))
                return null;
            return par;
        }

        #endregion

    }

}
