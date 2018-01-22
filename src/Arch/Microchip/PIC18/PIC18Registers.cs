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

using Microchip.Crownking;
using Reko.Arch.Microchip.Common;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Reko.Arch.Microchip.PIC18
{

    /// <summary>
    /// This class implements the PIC18 registers pool.
    /// </summary>
    public class PIC18Registers : PICRegistersBuilder, IPICRegisterSymTable
    {

        #region Helper classes

        /// <summary>
        /// A register address which can be an actual data memory address or a Non-Memory-Mapped Register ID.
        /// </summary>
        private sealed class RegAddress : IEquatable<RegAddress>
        {
            /// <summary>
            /// The register address if memory-mapped.
            /// </summary>
            public readonly Address Addr;

            /// <summary>
            /// The register ID is non-memory-mapped.
            /// </summary>
            public readonly string NMMRID;

            #region Constructors

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="regAddr">The register address.</param>
            public RegAddress(Address regAddr)
            {
                Addr = regAddr;
                NMMRID = String.Empty;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="regAddr">The register absolute 16-bit address.</param>
            public RegAddress(ushort regAddr)
            {
                Addr = Address.Ptr16(regAddr);
                NMMRID = String.Empty;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="nmmrID">The register ID if non-memory-mapped.</param>
            public RegAddress(string nmmrID)
            {
                Addr = null;
                NMMRID = nmmrID;
            }

            #endregion

            public bool Equals(RegAddress other)
            {
                if (ReferenceEquals(this, other)) return true;
                if (ReferenceEquals(other, null)) return false;
                if (!String.IsNullOrEmpty(NMMRID)) return (NMMRID == other.NMMRID);
                return Addr == other.Addr;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as RegAddress);
            }

            public override int GetHashCode()
            {
                return (Addr?.GetHashCode() ?? 0 * 17) ^ NMMRID.GetHashCode(); ;
            }

            public override string ToString()
            {
                if (Addr != null) return $"{Addr}";
                return $"NMMRID({NMMRID})";
            }

        }

        /// <summary>
        /// A bit field address composed of the containing register's address and bit field position.
        /// </summary>
        private sealed class BitFieldAddr : IEquatable<BitFieldAddr>
        {
            public readonly Address RegAddr;
            public readonly ulong BitPos;

            public BitFieldAddr(Address regaddr, ulong bitpos)
            {
                RegAddr = regaddr;
                BitPos = bitpos;
            }

            public bool Equals(BitFieldAddr other)
            {
                if (other == null) return false;
                return RegAddr.Equals(other.RegAddr) && BitPos.Equals(other.BitPos);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as BitFieldAddr);
            }

            public override int GetHashCode()
            {
                return (RegAddr.GetHashCode() * 17) ^ BitPos.GetHashCode(); ;
            }

            public override string ToString()
            {
                return $"{RegAddr}.bit{BitPos}";
            }
        }

        /// <summary>
        /// List of bit fields. Permits to get bit fields with different widths at same register's position.
        /// </summary>
        private sealed class BitFieldList : SortedList<uint, PICBitFieldStorage>
        {
        }

        #endregion

        #region Locals

        private Dictionary<string, PICRegisterStorage> UniqueRegNames = new Dictionary<string, PICRegisterStorage>();
        private Dictionary<string, PICBitFieldStorage> UniqueFieldNames = new Dictionary<string, PICBitFieldStorage>();
        private Dictionary<RegAddress, RegisterStorage> RegsByAddr = new Dictionary<RegAddress, RegisterStorage>();
        private Dictionary<BitFieldAddr, BitFieldList> RegsBitFields = new Dictionary<BitFieldAddr, BitFieldList>();

        private static PIC18Registers _registers;
        private static object _symtabLock = new object(); // to allow concurrent access.

        #endregion

        #region PIC18 standard (core) registers and bit fields

        /// <summary>STATUS register. </summary>
        public static PICRegisterStorage STATUS { get; private set; }

        /// <summary>Carry bit in STATUS register. </summary>
        public static PICBitFieldStorage C { get; private set; }

        /// <summary>Digit-Carry bit in STATUS register.. </summary>
        public static PICBitFieldStorage DC { get; private set; }

        /// <summary>Zero bit in STATUS register.. </summary>
        public static PICBitFieldStorage Z { get; private set; }

        /// <summary>Overflow bit in STATUS register.. </summary>
        public static PICBitFieldStorage OV { get; private set; }

        /// <summary>Negative bit in STATUS register.. </summary>
        public static PICBitFieldStorage N { get; private set; }

        /// <summary>Power-Down bit in STATUS or PCON register.. </summary>
        public static PICBitFieldStorage PD { get; private set; }

        /// <summary>Timed-Out bit in STATUS or PCON register.. </summary>
        public static PICBitFieldStorage TO { get; private set; }

        /// <summary>FSR2L special function register. </summary>
        public static PICRegisterStorage FSR2L { get; private set; }

        /// <summary>FSR2H special function register. </summary>
        public static PICRegisterStorage FSR2H { get; private set; }

        /// <summary>PLUSW2 special function register. </summary>
        public static PICRegisterStorage PLUSW2 { get; private set; }

        /// <summary>PREINC2 special function register. </summary>
        public static PICRegisterStorage PREINC2 { get; private set; }

        /// <summary>POSTDEC2 special function register. </summary>
        public static PICRegisterStorage POSTDEC2 { get; private set; }

        /// <summary>POSTINC2 special function register. </summary>
        public static PICRegisterStorage POSTINC2 { get; private set; }

        /// <summary>INDF2 special function register. </summary>
        public static PICRegisterStorage INDF2 { get; private set; }

        /// <summary>BSR special function register. </summary>
        public static PICRegisterStorage BSR { get; private set; }

        /// <summary>FSR1L special function register. </summary>
        public static PICRegisterStorage FSR1L { get; private set; }

        /// <summary>FSR1H special function register. </summary>
        public static PICRegisterStorage FSR1H { get; private set; }

        /// <summary>PLUSW1 special function register. </summary>
        public static PICRegisterStorage PLUSW1 { get; private set; }

        /// <summary>PREINC1 special function register. </summary>
        public static PICRegisterStorage PREINC1 { get; private set; }

        /// <summary>POSTDEC1 special function register. </summary>
        public static PICRegisterStorage POSTDEC1 { get; private set; }

        /// <summary>POSTINC1 special function register. </summary>
        public static PICRegisterStorage POSTINC1 { get; private set; }

        /// <summary>INDF1 special function register. </summary>
        public static PICRegisterStorage INDF1 { get; private set; }

        /// <summary>WREG special function register. </summary>
        public static PICRegisterStorage WREG { get; private set; }

        /// <summary>FSR0L special function register. </summary>
        public static PICRegisterStorage FSR0L { get; private set; }

        /// <summary>FSR0H special function register. </summary>
        public static PICRegisterStorage FSR0H { get; private set; }

        /// <summary>PLUSW0 special function register. </summary>
        public static PICRegisterStorage PLUSW0 { get; private set; }

        /// <summary>PREINC0 special function register. </summary>
        public static PICRegisterStorage PREINC0 { get; private set; }

        /// <summary>POSTDEC0 special function register. </summary>
        public static PICRegisterStorage POSTDEC0 { get; private set; }

        /// <summary>POSTINC0 special function register. </summary>
        public static PICRegisterStorage POSTINC0 { get; private set; }

        /// <summary>INDF0 special function register. </summary>
        public static PICRegisterStorage INDF0 { get; private set; }

        /// <summary>PRODL special function register. </summary>
        public static PICRegisterStorage PRODL { get; private set; }

        /// <summary>PRODH special function register. </summary>
        public static PICRegisterStorage PRODH { get; private set; }

        /// <summary>TABLAT special function register. </summary>
        public static PICRegisterStorage TABLAT { get; private set; }

        /// <summary>TBLPTRL special function register. </summary>
        public static PICRegisterStorage TBLPTRL { get; private set; }

        /// <summary>TBLPTRH special function register. </summary>
        public static PICRegisterStorage TBLPTRH { get; private set; }

        /// <summary>TBLPTRU special function register. </summary>
        public static PICRegisterStorage TBLPTRU { get; private set; }

        /// <summary>PCL special function register. </summary>
        public static PICRegisterStorage PCL { get; private set; }

        /// <summary>PCLH special function register. </summary>
        public static PICRegisterStorage PCLATH { get; private set; }

        /// <summary>PCLU special function register. </summary>
        public static PICRegisterStorage PCLATU { get; private set; }

        /// <summary>STKPTR special function register. </summary>
        public static PICRegisterStorage STKPTR { get; private set; }

        /// <summary>TOSL special function register. </summary>
        public static RegisterStorage TOSL { get; private set; }

        /// <summary>TOSH special function register. </summary>
        public static PICRegisterStorage TOSH { get; private set; }

        /// <summary>TOSU special function register. </summary>
        public static PICRegisterStorage TOSU { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pic">The PIC18 definition.</param>
        private PIC18Registers(PIC pic) : base(pic)
        {
            lock (_symtabLock)
            {
                UniqueRegNames.Clear();
                UniqueFieldNames.Clear();
                RegsByAddr.Clear();
                RegsBitFields.Clear();
            }
        }

        #endregion

        #region Helpers

        private PICRegisterStorage _getRegister(string name)
        {
            PICRegisterStorage reg;
            lock (_symtabLock)
            {
                if (!UniqueRegNames.TryGetValue(name, out reg))
                    throw new InvalidOperationException($"Missing definition of register '{name}' in symbol table.");
            }
            return reg;
        }

        private PICRegisterStorage _peekRegister(string name)
        {
            PICRegisterStorage reg;
            lock (_symtabLock)
            {
                UniqueRegNames.TryGetValue(name, out reg);
            }
            return reg;
        }

        private PICRegisterStorage _peekRegister(RegAddress aAddr)
        {
            RegisterStorage reg;
            lock (_symtabLock)
            {
                RegsByAddr.TryGetValue(aAddr, out reg);
            }
            return reg as PICRegisterStorage;
        }

        private RegisterStorage _peekRegister(int number)
        {
            lock (_symtabLock)
            {
                var reg = RegsByAddr.Where(e => e.Value.Number == number)
                    .Select(e => (KeyValuePair<RegAddress, RegisterStorage>?)e)
                    .FirstOrDefault()?.Value;
                return reg ?? RegisterStorage.None;
            }
        }

        private RegisterStorage _peekCoreRegister(int i)
        {
            lock (_symtabLock)
            {
                var entry = RegsByAddr.Where(p => p.Value.Number == i).Select(e => e.Value).FirstOrDefault();
                return entry ?? RegisterStorage.None;
            }
        }

        private PICBitFieldStorage _getBitField(string name)
        {
            PICBitFieldStorage fld;
            lock (_symtabLock)
            {
                if (!UniqueFieldNames.TryGetValue(name, out fld))
                    throw new InvalidOperationException($"Missing definition of bit field '{name}' in symbol table.");
            }
            return fld;
        }

        private PICBitFieldStorage _peekBitField(string name)
        {
            PICBitFieldStorage fld;
            lock (_symtabLock)
            {
                UniqueFieldNames.TryGetValue(name, out fld);
            }
            return fld;
        }

        private FlagGroupStorage _peekBitField(Address regAddress, uint bitPos, uint bitWidth = 0)
        {
            lock (_symtabLock)
            {
                BitFieldList flist;
                if (RegsBitFields.TryGetValue(new BitFieldAddr(regAddress, bitPos), out flist))
                {
                    if (bitWidth == 0)
                        return flist.LastOrDefault().Value;
                    return flist.FirstOrDefault(f => f.Value.BitWidth == bitWidth).Value;
                }
            }
            return null;

        }

        /// <summary>
        /// This method sets the standard "core" registers as retrieved from the registers symbol table populated by the PIC definition.
        /// </summary>
        /// <remarks>
        /// This permits to still get a direct reference to standard registers and keeps having the capability to get a dynamic definition
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        private void _setCoreRegisters()
        {

            STATUS = _getRegister("STATUS");
            C = _getBitField("C");
            DC = _getBitField("DC");
            Z = _getBitField("Z");
            OV = _getBitField("OV");
            N = _getBitField("Z");

            PD = _peekBitField("PD");
            if (PD == null)
                PD = _peekBitField("nPD");
            TO = _peekBitField("TO");
            if (TO == null)
                TO = _peekBitField("nTO");

            FSR2L = _getRegister("FSR2L");
            FSR2H = _getRegister("FSR2H");
            PLUSW2 = _getRegister("PLUSW2");
            PREINC2 = _getRegister("PREINC2");
            POSTDEC2 = _getRegister("POSTDEC2");
            POSTINC2 = _getRegister("POSTINC2");
            INDF2 = _getRegister("INDF2");
            BSR = _getRegister("BSR");
            FSR1L = _getRegister("FSR1L");
            FSR1H = _getRegister("FSR1H");
            PLUSW1 = _getRegister("PLUSW1");
            PREINC1 = _getRegister("PREINC1");
            POSTDEC1 = _getRegister("POSTDEC1");
            POSTINC1 = _getRegister("POSTINC1");
            INDF1 = _getRegister("INDF1");
            WREG = _getRegister("WREG");
            FSR0L = _getRegister("FSR0L");
            FSR0H = _getRegister("FSR0H");
            PLUSW0 = _getRegister("PLUSW0");
            PREINC0 = _getRegister("PREINC0");
            POSTDEC0 = _getRegister("POSTDEC0");
            POSTINC0 = _getRegister("POSTINC0");
            INDF0 = _getRegister("INDF0");
            PRODL = _getRegister("PRODL");
            PRODH = _getRegister("PRODH");
            TABLAT = _getRegister("TABLAT");
            TBLPTRL = _getRegister("TBLPTRL");
            TBLPTRH = _getRegister("TBLPTRH");
            TBLPTRU = _getRegister("TBLPTRU");
            PCL = _getRegister("PCL");
            PCLATH = _getRegister("PCLATH");
            PCLATU = _getRegister("PCLATU");
            STKPTR = _getRegister("STKPTR");
            TOSL = _getRegister("TOSL");
            TOSH = _getRegister("TOSH");
            TOSU = _getRegister("TOSU");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="PIC18Registers"/> instance.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <returns>
        /// A <see cref="PICRegistersBuilder"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pic"/> is null.</exception>
        public static PIC18Registers Create(PIC pic)
        {
            if (pic == null) throw new ArgumentNullException(nameof(pic));
            _registers = new PIC18Registers(pic);
            return _registers;
        }

        /// <summary>
        /// Loads the PIC18 registers into the registers symbol table.
        /// </summary>
        public void LoadRegisters()
        {
            base.LoadRegisters(this);
            _setCoreRegisters();
        }

        #endregion

        #region IRegisterSymTable interface

        /// <summary>
        /// Adds a PIC register. Returns null if no addition done.
        /// </summary>
        /// <param name="reg">The register.</param>
        /// <returns>
        /// A <seealso cref="RegisterStorage"/> or null if tentative of duplication.
        /// </returns>
        public RegisterStorage AddRegister(PICRegisterStorage reg)
        {
            RegAddress addr = (reg.IsNMMR ? new RegAddress(reg.SFRDef.NMMRID) : new RegAddress(reg.Address));

            lock (_symtabLock)
            {
                if (UniqueRegNames.ContainsKey(reg.Name)) return null;      // Do not duplicate name
                if (RegsByAddr.ContainsKey(addr)) return null;   // Do not duplicate register with same address
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
            lock (_symtabLock)
            {
                if (UniqueRegNames.ContainsKey(field.Name)) return null;      // Do not duplicate name
                BitFieldAddr key = new BitFieldAddr(reg.Address, field.BitPos);
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
        /// Gets all the defined PIC registers.
        /// </summary>
        /// <value>
        /// An array of PIC registers.
        /// </value>
        public static RegisterStorage[] GetRegisters => _registers?.RegsByAddr.Values.ToArray();

        /// <summary>
        /// Gets a register by its name or <seealso cref="RegisterStorage.None"/>.
        /// </summary>
        /// <param name="name">The name as a string.</param>
        /// <returns>
        /// The register or null.
        /// </returns>
        public static RegisterStorage GetRegister(string name)
        {
            PICRegisterStorage reg = _registers?._peekRegister(name);
            if (reg == null)
                return RegisterStorage.None;
            return reg;
        }

        /// <summary>
        /// Gets a register by its address or <seealso cref="RegisterStorage.None"/>.
        /// </summary>
        /// <param name="address">The data memory address.</param>
        /// <returns>
        /// The register or null.
        /// </returns>
        public static RegisterStorage GetRegister(Address address)
        {
            RegAddress addr = new RegAddress(address);
            RegisterStorage reg = _registers?._peekRegister(addr);
            if (reg == null)
                return RegisterStorage.None;
            return reg;
        }

        /// <summary>
        /// Gets a register by its absolute address or <seealso cref="RegisterStorage.None"/>.
        /// </summary>
        /// <param name="uAddr">The absolute address of the register.</param>
        /// <returns>
        /// The register or <seealso cref="RegisterStorage.None"/>.
        /// </returns>
        public static RegisterStorage GetRegister(ushort uAddr)
        {
            var regaddr = new RegAddress(uAddr);
            RegisterStorage reg = _registers?._peekRegister(regaddr);
            if (reg == null)
                return RegisterStorage.None;
            return reg;
        }

        /// <summary>
        /// Gets a register by its index number or <seealso cref="RegisterStorage.None"/>.
        /// </summary>
        /// <param name="number">Index number of the register.</param>
        /// <returns>
        /// The register or <seealso cref="RegisterStorage.None"/>.
        /// </returns>
        public static RegisterStorage GetRegister(int number)
        {
            return _registers?._peekRegister(number) ?? RegisterStorage.None;
        }

        /// <summary>
        /// Gets a standard (core) register by its index or <seealso cref="RegisterStorage.None"/>.
        /// </summary>
        /// <param name="i">Zero-based index of the register.</param>
        /// <returns>
        /// The register or <seealso cref="RegisterStorage.None"/>.
        /// </returns>
        public static RegisterStorage GetCoreRegister(int i)
        {
            return _registers?._peekCoreRegister(i) ?? RegisterStorage.None;
        }

        /// <summary>
        /// Gets a register bit field by its name or null.
        /// </summary>
        /// <param name="name">The name as a string.</param>
        /// <returns>
        /// The bit field or null.
        /// </returns>
        public static FlagGroupStorage GetBitField(string name)
        {
            return _registers?._peekBitField(name);
        }

        /// <summary>
        /// Gets a register bit field by its register address and bit position/width or null.
        /// If <paramref name="bitwidth"/> is 0, then the widest bit field is retrieved.
        /// </summary>
        /// <param name="regAddress">The parent register address.</param>
        /// <param name="bitPos">The bit position.</param>
        /// <param name="bitWidth">(Optional) The bit field width.</param>
        /// <returns>
        /// The bit field or null.
        /// </returns>
        public static FlagGroupStorage GetBitField(Address regAddress, uint bitPos, uint bitWidth = 0)
        {
            return _registers?._peekBitField(regAddress, bitPos, bitWidth);
        }

        /// <summary>
        /// Gets a register bit field by its register and bit position/width or null.
        /// If <paramref name="bitWidth"/> is 0, then the widest bit field is retrieved.
        /// </summary>
        /// <param name="reg">The parent register.</param>
        /// <param name="bitPos">The bit position.</param>
        /// <param name="bitWidth">(Optional) The bit field width.</param>
        /// <returns>
        /// The bit field or null.
        /// </returns>
        public static FlagGroupStorage GetBitField(PICRegisterStorage reg, uint bitPos, uint bitWidth = 0)
        {
            if (reg == null) return null;
            return GetBitField(reg.Address, bitPos, bitWidth);
        }

        /// <summary>
        /// Gets a register bit field by its register name and bit position/width or null.
        /// If <paramref name="bitWidth"/> is 0, then the widest bit field is retrieved.
        /// </summary>
        /// <param name="name">The parent register name.</param>
        /// <param name="bitPos">The bit position.</param>
        /// <param name="bitWidth">(Optional) The bit field width.</param>
        /// <returns>
        /// The bit field or null.
        /// </returns>
        public static FlagGroupStorage GetBitField(string name, uint bitPos, uint bitWidth = 0)
        {
            return GetBitField(GetRegister(name) as PICRegisterStorage, bitPos, bitWidth);
        }

        /// <summary>
        /// Gets the maximum number of registers.
        /// </summary>
        public static int Max => _registers?.RegsByAddr.Count ?? 0;

        #endregion

    }

}
