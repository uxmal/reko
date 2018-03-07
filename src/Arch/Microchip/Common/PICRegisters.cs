#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Libraries.Microchip;
using Reko.Arch.Microchip.Common;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// This class provides support for the PIC registers pool implementations.
    /// </summary>
    public abstract class PICRegisters : PICRegistersBuilder, IPICRegisterSymTable
    {

        #region Helper classes

        protected class RegSizedAddress : IEquatable<RegSizedAddress>
        {

            #region Properties/Fields

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

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="regAddr">The register address.</param>
            /// <param name="width">The bit width of the register.</param>
            public RegSizedAddress(PICDataAddress regAddr, int width = -1)
            {
                Addr = regAddr;
                BitWidth = width;
                NMMRID = String.Empty;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="regAddr">The register absolute 16-bit address.</param>
            /// <param name="width">The bit width of the register.</param>
            public RegSizedAddress(ushort regAddr, int width = -1)
            {
                Addr = PICDataAddress.Ptr(regAddr);
                BitWidth = width;
                NMMRID = String.Empty;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="nmmrID">The register ID if non-memory-mapped.</param>
            /// <param name="width">The bit width of the register.</param>
            public RegSizedAddress(string nmmrID, int width = -1)
            {
                Addr = null;
                NMMRID = nmmrID;
                BitWidth = width;
            }

            #endregion

            #region IEquatable implementation

            public bool Equals(RegSizedAddress other)
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
                return Equals(obj as RegSizedAddress);
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
        protected class BitFieldAddr : IEquatable<BitFieldAddr>
        {
            public readonly PICDataAddress RegAddr;
            public readonly ulong BitPos;

            public BitFieldAddr(PICDataAddress regaddr, ulong bitpos)
            {
                RegAddr = regaddr;
                BitPos = bitpos;
            }

            public bool Equals(BitFieldAddr other)
            {
                if (other is null)
                    return false;
                return RegAddr.Equals(other.RegAddr) && BitPos.Equals(other.BitPos);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as BitFieldAddr);
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
        protected class BitFieldList : SortedList<uint, PICBitFieldStorage>
        {
        }

        #endregion

        #region Locals

        private static object symTabLock = new object(); // lock to allow concurrent access.
        protected static PICRegisters registers;

        protected static Dictionary<string, PICRegisterStorage> UniqueRegNames
            = new Dictionary<string, PICRegisterStorage>();
        protected static Dictionary<string, PICBitFieldStorage> UniqueFieldNames
            = new Dictionary<string, PICBitFieldStorage>();
        protected static Dictionary<RegSizedAddress, RegisterStorage> RegsByAddr
            = new Dictionary<RegSizedAddress, RegisterStorage>();
        protected static Dictionary<BitFieldAddr, BitFieldList> RegsBitFields
            = new Dictionary<BitFieldAddr, BitFieldList>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pic">The PIC definition descriptor.</param>
        protected PICRegisters(PIC pic) : base(pic)
        {
            lock (symTabLock)
            {
                UniqueRegNames.Clear();
                UniqueFieldNames.Clear();
                RegsByAddr.Clear();
                RegsBitFields.Clear();
            }
        }

        #endregion

        #region Helpers

        protected PICRegisterStorage GetRegisterStorageByName(string name)
        {
            lock (symTabLock)
            {
                if (!UniqueRegNames.TryGetValue(name, out PICRegisterStorage reg))
                    throw new InvalidOperationException($"Missing definition for register '{name}' in symbol table.");
                return reg;
            }
        }

        protected PICRegisterStorage PeekRegisterStorageByName(string name)
        {
            lock (symTabLock)
            {
                UniqueRegNames.TryGetValue(name, out PICRegisterStorage reg);
                return reg;
            }
        }

        protected PICRegisterStorage PeekRegisterStorageBySizedAddr(RegSizedAddress aAddr)
        {
            lock (symTabLock)
            {
                if (RegsByAddr.TryGetValue(aAddr, out RegisterStorage reg))
                    return reg as PICRegisterStorage;
                return null;
            }
        }

        protected RegisterStorage PeekRegisterStorageByNum(int number)
        {
            lock (symTabLock)
            {
                var reg = RegsByAddr.Where(l => l.Value.Number == number)
                    .Select(e => (KeyValuePair<RegSizedAddress, RegisterStorage>?)e)
                    .FirstOrDefault()?.Value;
                return reg ?? RegisterStorage.None;
            }
        }

        protected RegisterStorage PeekCoreRegisterStorageByIdx(int i)
        {
            lock (symTabLock)
            {
                var entry = RegsByAddr.Where(p => p.Value.Number == i).Select(e => e.Value).FirstOrDefault();
                return entry ?? RegisterStorage.None;
            }
        }

        protected PICBitFieldStorage GetBitFieldStorageByName(string name)
        {
            lock (symTabLock)
            {
                if (!UniqueFieldNames.TryGetValue(name, out PICBitFieldStorage fld))
                    throw new InvalidOperationException($"Missing definition of bit field '{name}' in symbol table.");
                return fld;
            }
        }

        protected PICBitFieldStorage PeekBitFieldStorageByName(string name)
        {
            lock (symTabLock)
            {
                UniqueFieldNames.TryGetValue(name, out PICBitFieldStorage fld);
                return fld;
            }
        }

        protected FlagGroupStorage PeekBitFieldStorage(PICDataAddress regAddress, uint bitPos, uint bitWidth = 0)
        {
            lock (symTabLock)
            {
                if (RegsBitFields.TryGetValue(new BitFieldAddr(regAddress, bitPos), out BitFieldList flist))
                {
                    if (bitWidth == 0)
                        return flist.LastOrDefault().Value;
                    return flist.FirstOrDefault(f => f.Value.BitWidth == bitWidth).Value;
                }
            }
            return null;

        }

        /// <summary>
        /// This method sets each of the standard "core" registers of the PIC. It must be implemented by derived classes.
        /// They are retrieved from the registers symbol table which has been previously populated by loading the PIC definition.
        /// </summary>
        /// <remarks>
        /// This permits to still get a direct reference to standard registers and keeps having some flexibility on definitions.
        /// </remarks>
        protected abstract void SetCoreRegisters();

        #endregion

        #region Methods

        /// <summary>
        /// Loads the PIC18 registers into the registers symbol table.
        /// </summary>
        public void LoadRegisters()
        {
            base.LoadRegisters(this);
            SetCoreRegisters();
            HWStackDepth = PIC?.ArchDef.MemTraits.HWStackDepth ?? 0;
        }

        #endregion

        #region IPICRegisterSymTable interface

        /// <summary>
        /// Adds a PIC register to the registers symbol table. Returns null if no addition done.
        /// </summary>
        /// <param name="reg">The register.</param>
        /// <returns>
        /// A <seealso cref="RegisterStorage"/> or null if tentative of duplication.
        /// </returns>
        public RegisterStorage AddRegister(PICRegisterStorage reg)
        {
            RegSizedAddress addr =
                (reg.IsMemoryMapped
                    ? new RegSizedAddress(reg.Address, reg.BitWidth)
                    : new RegSizedAddress(reg.NMMRID, reg.BitWidth)
                );

            lock (symTabLock)
            {
                if (UniqueRegNames.ContainsKey(reg.Name))
                    return null;      // Do not duplicate name
                if (RegsByAddr.ContainsKey(addr))
                    return null;   // Do not duplicate register with same address and bit width
                UniqueRegNames[reg.Name] = reg;
                RegsByAddr[addr] = reg;
            }
            return reg;
        }

        /// <summary>
        /// Adds a register's named bit field. Returns null if no addition done.
        /// </summary>
        /// <param name="reg">The parent register.</param>
        /// <param name="field">The bit field.</param>
        /// <returns>
        /// A <seealso cref="FlagGroupStorage"/> or null if tentative of duplication.
        /// </returns>
        public FlagGroupStorage AddRegisterField(PICRegisterStorage reg, PICBitFieldStorage field)
        {
            lock (symTabLock)
            {
                if (UniqueRegNames.ContainsKey(field.Name))
                    return null;      // Do not duplicate name
                var key = new BitFieldAddr(reg.Address, field.BitPos);
                if (!RegsBitFields.ContainsKey(key))
                {
                    var newfieldlist = new BitFieldList
                    {
                        { field.BitWidth, field }
                    };
                    UniqueFieldNames[field.Name] = field;
                    RegsBitFields[key] = newfieldlist;
                    return field;
                }
                var fieldlist = RegsBitFields[key];
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

        #region Registers API

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
        public static RegisterStorage[] GetRegisters => RegsByAddr.Values.ToArray();

        /// <summary>
        /// Gets a register by its name or <seealso cref="RegisterStorage.None"/>.
        /// </summary>
        /// <param name="name">The name as a string.</param>
        /// <returns>
        /// The register or null.
        /// </returns>
        public static RegisterStorage GetRegisterByName(string name)
            => registers?.PeekRegisterStorageByName(name) ?? RegisterStorage.None;

        /// <summary>
        /// Gets a register by its address/bit-width.
        /// </summary>
        /// <param name="addr">The data memory address of the register.</param>
        /// <param name="bW">The bit width of the register.</param>
        /// <returns>
        /// The register or <seealso cref="RegisterStorage.None"/>.
        /// </returns>
        public static RegisterStorage GetRegisterBySizedAddr(PICDataAddress addr, int bW)
            => registers?.PeekRegisterStorageBySizedAddr(new RegSizedAddress(addr, bW)) ?? RegisterStorage.None;

        /// <summary>
        /// Gets a register by its absolute address/bit-width.
        /// </summary>
        /// <param name="uAddr">The absolute address of the register.</param>
        /// <param name="bW">The bit width of the register.</param>
        /// <returns>
        /// The register or <seealso cref="RegisterStorage.None"/>.
        /// </returns>
        public static RegisterStorage GetRegisterBySizedAddr(ushort uAddr, int bW)
            => registers?.PeekRegisterStorageBySizedAddr(new RegSizedAddress(uAddr, bW)) ?? RegisterStorage.None;

        /// <summary>
        /// Gets a register by its index number.
        /// </summary>
        /// <param name="number">Index number of the register.</param>
        /// <returns>
        /// The register or <seealso cref="RegisterStorage.None"/>.
        /// </returns>
        public static RegisterStorage GetRegisterByNum(int number)
            => registers?.PeekRegisterStorageByNum(number) ?? RegisterStorage.None;

        /// <summary>
        /// Gets a standard (core) register by its index.
        /// </summary>
        /// <param name="i">Zero-based index of the register.</param>
        /// <returns>
        /// The register or <seealso cref="RegisterStorage.None"/>.
        /// </returns>
        public static RegisterStorage GetCoreRegisterByIdx(int i)
            => registers?.PeekCoreRegisterStorageByIdx(i) ?? RegisterStorage.None;

        /// <summary>
        /// Gets a register bit-field by its name.
        /// </summary>
        /// <param name="name">The name of the bit-field as a string.</param>
        /// <returns>
        /// The bit-field instance or null.
        /// </returns>
        public static FlagGroupStorage GetBitFieldByName(string name)
            => registers?.PeekBitFieldStorageByName(name);

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
        public static FlagGroupStorage GetBitFieldByAddr(PICDataAddress regAddress, uint bitPos, uint bitWidth = 0)
            => registers?.PeekBitFieldStorage(regAddress, bitPos, bitWidth);

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
        public static FlagGroupStorage GetBitFieldByReg(PICRegisterStorage reg, uint bitPos, uint bitWidth = 0)
            => (reg is null ? null : GetBitFieldByAddr(reg.Address, bitPos, bitWidth));

        /// <summary>
        /// Gets a register bit-field by its parent register name and bit position/width or null.
        /// If <paramref name="bitWidth"/> is 0, then the widest bit field is retrieved.
        /// </summary>
        /// <param name="name">The parent register name.</param>
        /// <param name="bitPos">The bit position of the bit-field.</param>
        /// <param name="bitWidth">(Optional) The bit field width.</param>
        /// <returns>
        /// The bit-field instance or null.
        /// </returns>
        public static FlagGroupStorage GetBitFieldByName(string name, uint bitPos, uint bitWidth = 0)
            => GetBitFieldByReg(GetRegisterByName(name) as PICRegisterStorage, bitPos, bitWidth);

        /// <summary>
        /// Gets the maximum number of PIC18 registers.
        /// </summary>
        public static int Max => RegsByAddr.Count;

        #endregion

    }

}
