#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Arch.MicrochipPIC.Common
{

    /// <summary>
    /// This class provides support for the PIC registers table implementations.
    /// </summary>
    public class PICRegisters : PICRegistersBuilder, IPICRegisterSymTable
    {

        /// <summary>
        /// A key based on bit-field's position and register's address.
        /// </summary>
        private class BitFieldPosAndAddrKey : IEquatable<BitFieldPosAndAddrKey>, IComparable<BitFieldPosAndAddrKey>, IComparable
        {
            public readonly PICDataAddress RegAddr;
            public readonly ulong BitPos;

            /// <summary>
            /// Instantiates a new bit-field address with given holding register address and bit-field position.
            /// </summary>
            /// <param name="regAddr">The register PIC data memory address.</param>
            /// <param name="bitPos">The bit position of the bit-field.</param>
            public BitFieldPosAndAddrKey(PICDataAddress regAddr, ulong bitPos)
            {
                RegAddr = regAddr;
                BitPos = bitPos;
            }

            public int CompareTo(BitFieldPosAndAddrKey? other)
            {
                if (other is null)
                    return 1;
                if (ReferenceEquals(this, other))
                    return 0;
                int res = RegAddr.CompareTo(other.RegAddr);
                if ((res = RegAddr.CompareTo(other.RegAddr)) != 0)
                    return res;
                return BitPos.CompareTo(other.BitPos);
            }

            public int CompareTo(object? obj) => CompareTo(obj as BitFieldPosAndAddrKey);

            public bool Equals(BitFieldPosAndAddrKey? other) => CompareTo(other) == 0;

            public override bool Equals(object? obj) => Equals(obj as BitFieldPosAndAddrKey);

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

        private static readonly object symTabLock = new object(); // lock to allow concurrent access.
        private static PICRegisters? regs = null;

        private static SortedList<string, PICRegisterStorage> registersByName
            = new SortedList<string, PICRegisterStorage>();
        private static SortedList<string, PICRegisterBitFieldStorage> bitFieldsByName
            = new SortedList<string, PICRegisterBitFieldStorage>();
        private static SortedList<PICRegisterSizedUniqueAddress, PICRegisterStorage> registersByAddressAndWidth
            = new SortedList<PICRegisterSizedUniqueAddress, PICRegisterStorage>();
        private static SortedList<ushort, PICRegisterStorage> alwayAccessibleRegisters
            = new SortedList<ushort, PICRegisterStorage>();
        private static HashSet<PICRegisterStorage> invalidDestRegisters
            = new HashSet<PICRegisterStorage>();
        private static Dictionary<PICRegisterStorage, (FSRIndexedMode iop, PICRegisterStorage fsr)> indirectParentRegisters
            = new Dictionary<PICRegisterStorage, (FSRIndexedMode, PICRegisterStorage)>();
        private static List<UserRegisterValue> registersAtPOR
            = new List<UserRegisterValue>();

        public static Identifier GlobalStack = new Identifier("Stack", PrimitiveType.Ptr32, MemoryStorage.Instance);
        public static Identifier GlobalData = new Identifier("Data", PrimitiveType.Byte, MemoryStorage.Instance);
        public static Identifier GlobalCode = new Identifier("Code", PrimitiveType.Ptr32, MemoryStorage.Instance);


        /// <summary>
        /// Instantiates a new empty registers pool.
        /// </summary>
        protected PICRegisters() : base()
        {
        }

#nullable disable

        public static PICRegisterStorage PCL { get; protected set; }
        public static PICRegisterStorage STATUS { get; protected set; }
        public static PICRegisterStorage WREG { get; protected set; }
        public static PICRegisterStorage PCLATH { get; protected set; }
        public static PICRegisterStorage STKPTR { get; protected set; }
        public static PICRegisterStorage BSR { get; protected set; }
        /// <summary> Carry bit in STATUS register. </summary>
        public static PICRegisterBitFieldStorage C { get; protected set; }

        /// <summary> Digit-Carry bit in STATUS register. </summary>
        public static PICRegisterBitFieldStorage DC { get; protected set; }

        /// <summary> Zero bit in STATUS register. </summary>
        public static PICRegisterBitFieldStorage Z { get; protected set; }

        /// <summary> Power-Down bit in STATUS or PCON register. </summary>
        public static PICRegisterBitFieldStorage PD { get; protected set; }

        /// <summary> Timed-Out bit in STATUS or PCON register. </summary>
        public static PICRegisterBitFieldStorage TO { get; protected set; }
#nullable  enable


        /// <summary>
        /// Loads the PIC registers into the registers symbol table.
        /// </summary>
        /// <param name="pic">The PIC definition as provided by Microchip.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pic"/> is null.</exception>
        public static void LoadRegisters(IPICDescriptor pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            regs = new PICRegisters();
            regs.Reset();
            regs.LoadRegistersInTable(regs, pic);
            HWStackDepth = pic.HWStackDepth;
        }

        /// <summary>
        /// Sets core registers common to all 8-bit PIC.
        /// </summary>
        protected virtual void SetCoreRegisters()
        {
            STATUS = GetRegister("STATUS");
            C = GetBitField("C");
            DC = GetBitField("DC");
            Z = GetBitField("Z");
            if (!TryGetBitField("nPD", out var pd))
            {
                TryGetBitField("PD", out pd);
            }
            PD = pd;
            if (!TryGetBitField("nTO", out var to))
            {
                TryGetBitField("TO", out to);
            }
            TO = to;
            PCL = GetRegister("PCL");
            PCLATH = GetRegister("PCLATH");
            WREG = GetRegister("WREG");
            STKPTR = GetRegister("STKPTR");
        }

        /// <summary>
        /// Set the registers values at Power-On Reset time.
        /// </summary>
        protected virtual void SetRegistersValuesAtPOR()
        {
            AddRegisterAtPOR(GetRegisterResetValue(STATUS));
            AddRegisterAtPOR(GetRegisterResetValue(PCL));
            AddRegisterAtPOR(GetRegisterResetValue(PCLATH));
            AddRegisterAtPOR(GetRegisterResetValue(WREG));
            AddRegisterAtPOR(GetRegisterResetValue(STKPTR));
        }

        /// <summary>
        /// Adds a register's value in the Power-On-Reset list.
        /// </summary>
        /// <param name="registerValue">The register value.</param>
        protected void AddRegisterAtPOR(UserRegisterValue registerValue)
        {
            if ((registerValue is not null) && (!registersAtPOR.Contains(registerValue)))
                registersAtPOR.Add(registerValue);
        }

        /// <summary>
        /// Adds a PIC register to the registers symbol table. Returns null if no addition done.
        /// </summary>
        /// <param name="reg">The register.</param>
        /// <returns>
        /// A <seealso cref="PICRegisterStorage"/> or null if tentative of duplication.
        /// </returns>
        bool IPICRegisterSymTable.AddRegister(PICRegisterStorage reg)
            => AddRegister(reg);

        /// <summary>
        /// Adds a register's named bit field. Returns null if no addition done.
        /// </summary>
        /// <param name="field">The bit field.</param>
        /// <returns>
        /// A <seealso cref="PICRegisterBitFieldStorage"/> or null if tentative of duplication.
        /// </returns>
        bool IPICRegisterSymTable.AddRegisterBitField(PICRegisterBitFieldStorage field)
            => AddRegisterBitField(field);


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
        public static PICRegisterStorage[] GetRegisters => registersByName.Values.ToArray();

        /// <summary>
        /// Attempts to get a PIC register from its name.
        /// </summary>
        /// <param name="regName">The name of the register as a string.</param>
        /// <param name="reg">[out] The PIC register if it exists.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regName"/> is null or empty string.</exception>
        public static bool TryGetRegister(string regName, [MaybeNullWhen(false)] out PICRegisterStorage reg)
        {
            if (string.IsNullOrWhiteSpace(regName))
                throw new ArgumentNullException(nameof(regName));

            reg = default!;
            lock (symTabLock)
                return registersByName.TryGetValue(regName, out reg);
        }

        /// <summary>
        /// Attempts to get a register by its absolute data memory address and bit-width.
        /// </summary>
        /// <param name="absDataAddr">The absolute data memory address of the register.</param>
        /// <param name="reg">[out] The PIC register if it exists.</param>
        /// <param name="bitWidth">(Optional) The bit width of the register. If ommitted, look for the
        ///                        narrowest register.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="absDataAddr"/> is null.</exception>
        public static bool TryGetRegister(PICDataAddress absDataAddr, out PICRegisterStorage reg, int bitWidth = 0)
        {
            if (absDataAddr is null)
                throw new ArgumentNullException(nameof(absDataAddr));

            var key = new PICRegisterSizedUniqueAddress(absDataAddr, (bitWidth <= 0 ? 0 : bitWidth));
            lock (symTabLock)
            {
                reg = registersByAddressAndWidth.FirstOrDefault(r => r.Key.Equals(key)).Value;
                return (reg is not null);
            }
        }

        /// <summary>
        /// Attempts to get a register by its 12/14 bit absolute address and bit-width.
        /// </summary>
        /// <param name="uDataAddr">The absolute address of the register as a 12/14 bit address.</param>
        /// <param name="bitWidth">(Optional) The bit width of the register. If ommitted, look for the
        ///                        narrowest register.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public static bool TryGetRegister(ushort uDataAddr, out PICRegisterStorage reg, int bitWidth = 0)
            => TryGetRegister(PICDataAddress.Ptr(uDataAddr), out reg, bitWidth);

        /// <summary>
        /// Gets a PIC register from its name.
        /// </summary>
        /// <param name="regName">The name of the register as a string.</param>
        /// <returns>
        /// The PIC register instance, or null if no such register exists.
        /// </returns>
        public static PICRegisterStorage? GetRegister(string regName)
        {
            if (TryGetRegister(regName, out var reg))
                return reg;
            return null;
        }

        /// <summary>
        /// Gets a register by its data memory address/bit-width.
        /// </summary>
        /// <param name="regDataAddr">The data memory address of the register.</param>
        /// <param name="bitWidth">(Optional) The bit width of the register. If ommitted, look for the
        ///                        narrowest register.</param>
        /// <returns>
        /// The PIC register instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if this register with given width does not exist.</exception>
        public static PICRegisterStorage GetRegister(PICDataAddress regDataAddr, int bitWidth = 0)
        {
            if (TryGetRegister(regDataAddr, out var reg, bitWidth))
                return reg;
            throw new InvalidOperationException($"No special register at address 0x{regDataAddr:X} / {bitWidth} bit-wide.");
        }

        /// <summary>
        /// Gets a register by its absolute address/bit-width.
        /// </summary>
        /// <param name="regAbsAddr">The absolute address of the register.</param>
        /// <param name="bitWidth">(Optional) The bit width of the register. If ommitted, look for the
        ///                        narrowest register.</param>
        /// <returns>
        /// The PIC register instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if this register with given width does not exist.</exception>
        public static PICRegisterStorage GetRegister(ushort regAbsAddr, int bitWidth = 0)
            => GetRegister(PICDataAddress.Ptr(regAbsAddr), bitWidth);

        /// <summary>
        /// Gets a standard (core) register by its index.
        /// </summary>
        /// <param name="i">Zero-based index of the register.</param>
        /// <returns>
        /// The register or <seealso cref="RegisterStorage.None"/>.
        /// </returns>
        public static PICRegisterStorage? PeekRegisterByIdx(int i)
        {
            lock (symTabLock)
            {
                var entry = registersByAddressAndWidth.Where(p => p.Value.Number == i).Select(e => e.Value).FirstOrDefault();
                return entry;
            }
        }

        /// <summary>
        /// Attempts to get PIC register's bit field from its name.
        /// </summary>
        /// <param name="fieldName">Name of the bit field.</param>
        /// <param name="field">[out] The bit field if it exists, otherwise null.</param>
        /// <param name="defltFld">(Optional) The default bit-field value if not found.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public static bool TryGetBitField(string fieldName, [MaybeNullWhen(false)] out PICRegisterBitFieldStorage field)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName));
            field = default!;
            lock (symTabLock)
            {
                return bitFieldsByName.TryGetValue(fieldName, out field);
            }
        }

        /// <summary>
        /// Gets a register bit-field by its name.
        /// </summary>
        /// <param name="name">The name of the bit-field as a string.</param>
        /// <returns>
        /// The bit-field instance or null.
        /// </returns>
        public static PICRegisterBitFieldStorage GetBitField(string fieldName)
        {
            if (TryGetBitField(fieldName, out var fld))
                return fld!;
            throw new InvalidOperationException($"Missing definition of bit field '{fieldName}' in symbol table.");
        }

        /// <summary>
        /// Gets a register bit field by its parent register and bit position/width. If
        /// <paramref name="bitWidth"/> is 0, then the widest bit-field is retrieved.
        /// </summary>
        /// <param name="parentReg">The parent register.</param>
        /// <param name="field">[out] The bit field if it exists, otherwise null.</param>
        /// <param name="bitPos">The bit position of the bit-field.</param>
        /// <param name="bitWidth">(Optional) The bit-field width.</param>
        /// <returns>
        /// True if bit-field found, false otherwise.
        /// </returns>
        public static bool TryGetBitField(PICRegisterStorage parentReg, [MaybeNullWhen(false)] out PICRegisterBitFieldStorage field, byte bitPos, byte bitWidth = 0)
        {
            field = null!;
            if (parentReg is null)
                return false;

            var fldkey = new PICRegisterBitFieldSortKey(bitPos, bitWidth);

            lock (symTabLock)
            {
                if (bitPos > parentReg.Traits.BitWidth || (bitPos + bitWidth) > parentReg.Traits.BitWidth)
                    return false;
                if (parentReg.BitFields!.Count <= 0)
                    return false;
                field = parentReg.BitFields.FirstOrDefault(f => f.Key.Equals(fldkey)).Value;
                return field is not null;
            }
        }

        /// <summary>
        /// Gets a register bit-field by its parent register address and bit position/width or null. If
        /// <paramref name="bitwidth"/> is 0, then the widest bit-field is retrieved.
        /// </summary>
        /// <param name="regAddress">The parent register address.</param>
        /// <param name="field">[out] The bit field if it exists, otherwise null.</param>
        /// <param name="bitPos">The bit position of the bit-field.</param>
        /// <param name="bitWidth">(Optional) The bit-field width.</param>
        /// <returns>
        /// True if bit-field found, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        public static bool TryGetBitField(PICDataAddress regAddress, [MaybeNullWhen(false)] out PICRegisterBitFieldStorage field, byte bitPos, byte bitWidth = 0)
        {
            field = null!;
            if (regAddress is null)
                throw new ArgumentNullException(nameof(regAddress));

            lock (symTabLock)
            {
                if (!TryGetRegister(regAddress, out var reg))
                    return false;
                return TryGetBitField(reg, out field, bitPos, bitWidth);
            }
        }

        /// <summary>
        /// Gets a register bit-field by its parent register name and bit position/width or null. If
        /// <paramref name="bitWidth"/> is 0, then the widest bit field is retrieved.
        /// </summary>
        /// <param name="regName">The parent register name.</param>
        /// <param name="field">[out] The bit field if it exists, otherwise null.</param>
        /// <param name="bitPos">The bit position of the bit-field.</param>
        /// <param name="bitWidth">(Optional) The bit field width.</param>
        /// <returns>
        /// True if bit-field found, false otherwise.
        /// </returns>
        public static bool TryGetBitField(string regName, [MaybeNullWhen(false)] out PICRegisterBitFieldStorage field, byte bitPos, byte bitWidth = 0)
        {
            field = null!;
            if (TryGetRegister(regName, out var reg))
                return TryGetBitField(reg!, out field, bitPos, bitWidth);
            return false;
        }

        /// <summary>
        /// Gets the maximum number of PIC18 registers.
        /// </summary>
        public static int Max => registersByName.Count;

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
        public static RegisterStorage? GetSubregister(RegisterStorage? reg, int offset, int width)
        {
            if (reg is null)
                throw new ArgumentNullException(nameof(reg));
            if (offset == 0 && reg.BitSize == (ulong)width)
                return reg;
            if (reg is PICRegisterStorage preg)
            {
                if (preg.HasAttachedRegs)
                    return preg.AttachedRegs!.Find(r => r.BitAddress == (ulong)offset && r.Traits.BitWidth == width);
            }
            return null;
        }

        /// <summary>
        /// Gets widest sub-register.
        /// </summary>
        /// <param name="reg">The parent register.</param>
        /// <param name="regs">The regs.</param>
        /// <returns>
        /// The widest sub-register.
        /// </returns>
        public static RegisterStorage? GetWidestSubregister(RegisterStorage reg, HashSet<RegisterStorage> regs)
        {
            if (regs is null || regs.Count <= 0)
                return reg;
            ulong mask = regs.Where(b => b is not null && b.OverlapsWith(reg)).Aggregate(0ul, (a, r) => a | r.BitMask);
            if ((mask & reg.BitMask) == reg.BitMask)
                return reg;
            RegisterStorage? rMax = null;
            //$TODO: Setup sub-registers (FSR, TBLPTR, ...) see what can be done with Joined Registers from XML
            /*
            Dictionary<uint, RegisterStorage> subregs;
            if (PICRegisters.SubRegisters.TryGetValue(reg, out subregs))
            {
                foreach (var subreg in subregs.Values)
                {
                    if ((subreg.BitMask & mask) == subreg.BitMask &&
                        (rMax is null || subreg.BitSize > rMax.BitSize))
                    {
                        rMax = subreg;
                    }
                }
            }
            */
            return rMax;
        }

        /// <summary>
        /// Gets the parent register of a PIC register
        /// </summary>
        /// <param name="subReg">The child PIC register.</param>
        /// <returns>
        /// The parent register or null if orphan.
        /// </returns>
        public static RegisterStorage? GetParentRegister(RegisterStorage subReg)
        {
            if (subReg is PICRegisterStorage preg)
                return preg.ParentRegister;
            return null;
        }

        /// <summary>
        /// Query if data memory absolute address corresponds to one of the move forbidden destination registers (per PIC data sheet)..
        /// </summary>
        /// <param name="dstAddr">The data memory absolute address.</param>
        /// <returns>
        /// True if forbidden, false allowed.
        /// </returns>
        public static bool NotAllowedDest(ushort dstAddr)
        {
            if (TryGetRegister(dstAddr, out var fsr))
                return invalidDestRegisters.Contains(fsr);
            return false;
        }

        /// <summary>
        /// Query if '<paramref name="sfr"/>' register is an indirect register (INDFx, PLUSWx, POSTINCx,... ) and get the associated FSR register.
        /// Returns the indirect addressing mode if applicable, else return None if <paramref name="sfr"/> is not an indirect register.
        /// </summary>
        /// <param name="sfr">The register used in instruction's operand.</param>
        /// <param name="parentFSR">[out] The actual FSR index register if <paramref name="sfr"/> is an indirect register or <see cref="PICRegisterStorage.None"/>.</param>
        /// <returns>
        /// The indirect addressing mode, or None.
        /// </returns>
        public static FSRIndexedMode IndirectOpMode(PICRegisterStorage sfr, out PICRegisterStorage parentFSR)
        {
            parentFSR = PICRegisterStorage.None;
            if (sfr is null)
                return FSRIndexedMode.None;
            if (indirectParentRegisters.TryGetValue(sfr, out (FSRIndexedMode indMode, PICRegisterStorage fsr) ent))
            {
                parentFSR = ent.fsr;
                return ent.indMode;
            }
            return FSRIndexedMode.None;
        }

        /// <summary>
        /// Query if given PIC register is always accessible (from any data memory bank).
        /// </summary>
        /// <param name="reg">The PIC register of interest.</param>
        /// <returns>
        /// True if always accessible from any data memory bank, false if not.
        /// </returns>
        public static bool IsAlwaysAccessible(PICRegisterStorage reg)
            => alwayAccessibleRegisters.ContainsValue(reg);

        /// <summary>
        /// Attempts to get an always-accessible (any bank) register given a PIC data memory address.
        /// </summary>
        /// <param name="regAddr">The PIC data memory address of the target PIC register.</param>
        /// <param name="reg">[out] The register, if any.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="regAddr"/> is null.</exception>
        public static bool TryGetAlwaysAccessibleRegister(PICBankedAddress regAddr, [MaybeNullWhen(false)] out PICRegisterStorage reg)
        {
            if (regAddr is null)
                throw new ArgumentNullException(nameof(regAddr));
            return TryGetAlwaysAccessibleRegister(regAddr.BankOffset.ToUInt16(), out reg);
        }

        /// <summary>
        /// Attempts to get an always-accessible (any bank) register given a memory address.
        /// </summary>
        /// <param name="regAbsAddr">The absolute address of the register.</param>
        /// <param name="reg">[out] The register, if any.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        public static bool TryGetAlwaysAccessibleRegister(ushort regOffset, [MaybeNullWhen(false)] out PICRegisterStorage reg)
            => alwayAccessibleRegisters.TryGetValue(regOffset, out reg);


        /// <summary>
        /// Gets the list of registers' values at Power-On-Reset.
        /// </summary>
        public static List<UserRegisterValue> GetPORRegistersList() => registersAtPOR;

        #endregion


        protected static bool AddRegister(PICRegisterStorage reg)
        {
            if (reg is null)
                return false;

            PICRegisterSizedUniqueAddress addr =
                (reg.Traits.IsMemoryMapped
                    ? new PICRegisterSizedUniqueAddress(reg.Traits.Address, reg.Traits.BitWidth)
                    : new PICRegisterSizedUniqueAddress(reg.Traits.NMMRID, reg.Traits.BitWidth)
                );

            lock (symTabLock)
            {
                if (registersByName.ContainsKey(reg.Name))
                    return false;      // Do not duplicate name
                if (registersByAddressAndWidth.ContainsKey(addr))
                    return false;   // Do not duplicate register with same address and bit width
                registersByName[reg.Name] = reg;
                registersByAddressAndWidth[addr] = reg;
            }
            return true;
        }

        protected static bool AddRegisterBitField(PICRegisterBitFieldStorage field)
        {
            lock (symTabLock)
            {
                if (registersByName.ContainsKey(field.Name))
                    return false;      // Do not duplicate bit-field name
                bitFieldsByName[field.Name] = field;
            }
            return true;
        }

        protected static void AddForbiddenDests(bool clean, params PICRegisterStorage[] regs)
        {
            if (clean)
            {
                invalidDestRegisters.Clear();
            }
            if (regs is not null && regs.Count() > 0)
            {
                foreach (var r in regs)
                {
                    invalidDestRegisters.Add(r);
                }
            }
        }

        protected static void AddIndirectParents(bool clean, params (PICRegisterStorage child, (FSRIndexedMode op, PICRegisterStorage reg) parent)[] pairs)
        {
            if (clean)
            {
                indirectParentRegisters.Clear();
            }
            if (pairs is not null && pairs.Count() > 0)
            {
                foreach (var (child, parent) in pairs)
                    indirectParentRegisters.Add(child, parent);
            }
        }

        protected static void AddAlwaysAccessibleRegisters(bool clean, params PICRegisterStorage[] regs)
        {
            if (clean)
            {
                alwayAccessibleRegisters.Clear();
            }
            if (regs is not null && regs.Count() > 0)
            {
                foreach (var reg in regs)
                {
                    alwayAccessibleRegisters.Add((ushort)reg.Traits.Address.Offset, reg);
                }
            }
        }

        protected static UserRegisterValue GetRegisterResetValue(PICRegisterStorage reg)
            => new UserRegisterValue(reg, Constant.Create(reg.DataType, new PICRegisterContent(reg.Traits).ResetValue));

        private void Reset()
        {
            lock (symTabLock)
            {
                registersByName.Clear();
                bitFieldsByName.Clear();
                registersByAddressAndWidth.Clear();
                alwayAccessibleRegisters.Clear();
                invalidDestRegisters.Clear();
                indirectParentRegisters.Clear();
                registersAtPOR.Clear();
            }
        }


    }

}
