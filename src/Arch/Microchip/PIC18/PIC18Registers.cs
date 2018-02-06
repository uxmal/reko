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
using Reko.Core.Types;
using Reko.Core.Expressions;
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

        private sealed class RegSizedAddress : IEquatable<RegSizedAddress>
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
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;
                if (Addr is null)
                {
                    if (NMMRID != other.NMMRID) return false;
                }
                else
                {
                    if (Addr != other.Addr) return false;
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
                if (Addr != null) return $"{Addr}[{BitWidth}]";
                return $"NMMRID({NMMRID}[{BitWidth}])";
            }

        }

        /// <summary>
        /// A bit field address composed of the containing register's address and bit field position.
        /// </summary>
        private sealed class BitFieldAddr : IEquatable<BitFieldAddr>
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
        /// List of bit fields. Permits to get bit fields with different widths at same register's bit position.
        /// </summary>
        private sealed class BitFieldList : SortedList<uint, PICBitFieldStorage>
        {
        }

        #endregion

        #region Locals

        private static object _symtabLock = new object(); // lock to allow concurrent access.
        private static PIC18Registers _registers;

        private static Dictionary<string, PICRegisterStorage> UniqueRegNames
            = new Dictionary<string, PICRegisterStorage>();
        private static Dictionary<string, PICBitFieldStorage> UniqueFieldNames
            = new Dictionary<string, PICBitFieldStorage>();
        private static Dictionary<RegSizedAddress, RegisterStorage> RegsByAddr
            = new Dictionary<RegSizedAddress, RegisterStorage>();
        private static Dictionary<BitFieldAddr, BitFieldList> RegsBitFields
            = new Dictionary<BitFieldAddr, BitFieldList>();

        #endregion

        #region PIC18 standard (core) registers and bit fields

        /// <summary>
        /// STATUS register.
        /// </summary>
        public static PICRegisterStorage STATUS { get; private set; }

        /// <summary>
        /// Carry bit in STATUS register.
        /// </summary>
        public static PICBitFieldStorage C { get; private set; }

        /// <summary>
        /// Digit-Carry bit in STATUS register..
        /// </summary>
        public static PICBitFieldStorage DC { get; private set; }

        /// <summary>
        /// Zero bit in STATUS register..
        /// </summary>
        public static PICBitFieldStorage Z { get; private set; }

        /// <summary>
        /// Overflow bit in STATUS register..
        /// </summary>
        public static PICBitFieldStorage OV { get; private set; }

        /// <summary>
        /// Negative bit in STATUS register..
        /// </summary>
        public static PICBitFieldStorage N { get; private set; }

        /// <summary>
        /// Power-Down bit in STATUS or PCON register..
        /// </summary>
        public static PICBitFieldStorage PD { get; private set; }

        /// <summary>
        /// Timed-Out bit in STATUS or PCON register..
        /// </summary>
        public static PICBitFieldStorage TO { get; private set; }

        /// <summary>
        /// FSR2L special function register.
        /// </summary>
        public static PICRegisterStorage FSR2L { get; private set; }

        /// <summary>
        /// FSR2H special function register.
        /// </summary>
        public static PICRegisterStorage FSR2H { get; private set; }

        /// <summary>
        /// PLUSW2 special function register.
        /// </summary>
        public static PICRegisterStorage PLUSW2 { get; private set; }

        /// <summary>
        /// PREINC2 special function register.
        /// </summary>
        public static PICRegisterStorage PREINC2 { get; private set; }

        /// <summary>
        /// POSTDEC2 special function register.
        /// </summary>
        public static PICRegisterStorage POSTDEC2 { get; private set; }

        /// <summary>
        /// POSTINC2 special function register.
        /// </summary>
        public static PICRegisterStorage POSTINC2 { get; private set; }

        /// <summary>
        /// INDF2 special function register.
        /// </summary>
        public static PICRegisterStorage INDF2 { get; private set; }

        /// <summary>
        /// BSR special function register.
        /// </summary>
        public static PICRegisterStorage BSR { get; private set; }

        /// <summary>
        /// FSR1L special function register.
        /// </summary>
        public static PICRegisterStorage FSR1L { get; private set; }

        /// <summary>
        /// FSR1H special function register.
        /// </summary>
        public static PICRegisterStorage FSR1H { get; private set; }

        /// <summary>
        /// PLUSW1 special function register.
        /// </summary>
        public static PICRegisterStorage PLUSW1 { get; private set; }

        /// <summary>
        /// PREINC1 special function register.
        /// </summary>
        public static PICRegisterStorage PREINC1 { get; private set; }

        /// <summary>
        /// POSTDEC1 special function register.
        /// </summary>
        public static PICRegisterStorage POSTDEC1 { get; private set; }

        /// <summary>
        /// POSTINC1 special function register.
        /// </summary>
        public static PICRegisterStorage POSTINC1 { get; private set; }

        /// <summary>
        /// INDF1 special function register.
        /// </summary>
        public static PICRegisterStorage INDF1 { get; private set; }

        /// <summary>
        /// WREG special function register.
        /// </summary>
        public static PICRegisterStorage WREG { get; private set; }

        /// <summary>
        /// FSR0L special function register.
        /// </summary>
        public static PICRegisterStorage FSR0L { get; private set; }

        /// <summary>
        /// FSR0H special function register.
        /// </summary>
        public static PICRegisterStorage FSR0H { get; private set; }

        /// <summary>
        /// PLUSW0 special function register.
        /// </summary>
        public static PICRegisterStorage PLUSW0 { get; private set; }

        /// <summary>
        /// PREINC0 special function register.
        /// </summary>
        public static PICRegisterStorage PREINC0 { get; private set; }

        /// <summary>
        /// POSTDEC0 special function register.
        /// </summary>
        public static PICRegisterStorage POSTDEC0 { get; private set; }

        /// <summary>
        /// POSTINC0 special function register.
        /// </summary>
        public static PICRegisterStorage POSTINC0 { get; private set; }

        /// <summary>
        /// INDF0 special function register.
        /// </summary>
        public static PICRegisterStorage INDF0 { get; private set; }

        /// <summary>
        /// PRODL special function register.
        /// </summary>
        public static PICRegisterStorage PRODL { get; private set; }

        /// <summary>
        /// PRODH special function register.
        /// </summary>
        public static PICRegisterStorage PRODH { get; private set; }

        /// <summary>
        /// TABLAT special function register.
        /// </summary>
        public static PICRegisterStorage TABLAT { get; private set; }

        /// <summary>
        /// TBLPTRL special function register.
        /// </summary>
        public static PICRegisterStorage TBLPTRL { get; private set; }

        /// <summary>
        /// TBLPTRH special function register.
        /// </summary>
        public static PICRegisterStorage TBLPTRH { get; private set; }

        /// <summary>
        /// TBLPTRU special function register.
        /// </summary>
        public static PICRegisterStorage TBLPTRU { get; private set; }

        /// <summary>
        /// PCL special function register.
        /// </summary>
        public static PICRegisterStorage PCL { get; private set; }

        /// <summary>
        /// PCLH special function register.
        /// </summary>
        public static PICRegisterStorage PCLATH { get; private set; }

        /// <summary>
        /// PCLU special function register.
        /// </summary>
        public static PICRegisterStorage PCLATU { get; private set; }

        /// <summary>
        /// STKPTR special function register.
        /// </summary>
        public static PICRegisterStorage STKPTR { get; private set; }

        /// <summary>
        /// TOSL special function register.
        /// </summary>
        public static PICRegisterStorage TOSL { get; private set; }

        /// <summary>
        /// TOSH special function register.
        /// </summary>
        public static PICRegisterStorage TOSH { get; private set; }

        ///<summary>
        /// TOSU special function register.
        ///</summary>
        public static PICRegisterStorage TOSU { get; private set; }

        #region Pseudo-registers

        /// <summary>
        /// PROD pseudo-register (alias to PRODH:PRODL).
        /// </summary>
        public static PICRegisterStorage PROD { get; private set; }

        /// <summary>
        /// FSR0 pseudo-register (alias to FSR0H:FSR0L).
        /// </summary>
        public static PICRegisterStorage FSR0 { get; private set; }

        /// <summary>
        /// FSR1 pseudo-register (alias to FSR1H:FSR1L).
        /// </summary>
        public static PICRegisterStorage FSR1 { get; private set; }

        /// <summary>
        /// FSR2 pseudo-register (alias to FSR2H:FSR2L).
        /// </summary>
        public static PICRegisterStorage FSR2 { get; private set; }

        /// <summary>
        /// TOS pseudo-register (alias to TOSU:TOSH:TOSL).
        /// </summary>
        public static PICRegisterStorage TOS { get; private set; }

        /// <summary>
        /// PC pseudo-register (alias to PCLATU:PCLATH:PCL).
        /// </summary>
        public static PICRegisterStorage PCLAT { get; private set; }

        /// <summary>
        /// TBLPTR pseudo-register (alias to TBLPTRL:TBLPTRH:TBLPTRL).
        /// </summary>
        public static PICRegisterStorage TBLPTR { get; private set; }

        #region Shadow registers for some PIC18

        public static PICRegisterStorage STATUS_CSHAD { get; private set; }
        public static PICRegisterStorage WREG_CSHAD { get; private set; }
        public static PICRegisterStorage BSR_CSHAD { get; private set; }

        #endregion

        #endregion

        public static MemoryIdentifier GlobalStack = new MemoryIdentifier("Stack", PrimitiveType.Ptr32);
        public static MemoryIdentifier GlobalData = new MemoryIdentifier("Data", PrimitiveType.Byte);
        public static MemoryIdentifier GlobalCode = new MemoryIdentifier("Code", PrimitiveType.Ptr32);

        /// <summary>
        /// Hardware Return Address Stack of the PIC.
        /// </summary>
        public static MemoryIdentifier HWStack { get; private set; }

        /// <summary>
        /// Gets the depth of the hardware stack.
        /// </summary>
        public static int HWStackDepth { get; private set; }

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

        private PICRegisterStorage _getRegisterByName(string name)
        {
            lock (_symtabLock)
            {
                if (!UniqueRegNames.TryGetValue(name, out PICRegisterStorage reg))
                    throw new InvalidOperationException($"Missing definition for register '{name}' in symbol table.");
                return reg;
            }
        }

        private PICRegisterStorage _peekRegisterByName(string name)
        {
            lock (_symtabLock)
            {
                UniqueRegNames.TryGetValue(name, out PICRegisterStorage reg);
                return reg;
            }
        }

        private PICRegisterStorage _peekRegisterBySizedAddr(RegSizedAddress aAddr)
        {
            lock (_symtabLock)
            {
                if (RegsByAddr.TryGetValue(aAddr, out RegisterStorage reg))
                {
                    return reg as PICRegisterStorage;
                }
                return null;
            }
        }

        private RegisterStorage _peekRegisterByNum(int number)
        {
            lock (_symtabLock)
            {
                var reg = RegsByAddr.Where(l => l.Value.Number == number)
                    .Select(e => (KeyValuePair<RegSizedAddress, RegisterStorage>?)e)
                    .FirstOrDefault()?.Value;
                return reg ?? RegisterStorage.None;
            }
        }

        private RegisterStorage _peekCoreRegisterByIdx(int i)
        {
            lock (_symtabLock)
            {
                var entry = RegsByAddr.Where(p => p.Value.Number == i).Select(e => e.Value).FirstOrDefault();
                return entry ?? RegisterStorage.None;
            }
        }

        private PICBitFieldStorage _getBitFieldByName(string name)
        {
            lock (_symtabLock)
            {
                if (!UniqueFieldNames.TryGetValue(name, out PICBitFieldStorage fld))
                    throw new InvalidOperationException($"Missing definition of bit field '{name}' in symbol table.");
                return fld;
            }
        }

        private PICBitFieldStorage _peekBitFieldByName(string name)
        {
            lock (_symtabLock)
            {
                UniqueFieldNames.TryGetValue(name, out PICBitFieldStorage fld);
                return fld;
            }
        }

        private FlagGroupStorage _peekBitField(PICDataAddress regAddress, uint bitPos, uint bitWidth = 0)
        {
            lock (_symtabLock)
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
        /// This method sets each of the standard "core" registers of the PIC18.
        /// They are retrieved from the registers symbol table which has been previously populated by loading the PIC definition.
        /// </summary>
        /// <remarks>
        /// This permits to still get a direct reference to standard registers and keeps having flexibility on definitions.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if a register cannot be found in the symbol table.</exception>
        private void _setCoreRegisters()
        {

            // *True* PIC registers

            STATUS = _getRegisterByName("STATUS");
            C = _getBitFieldByName("C");
            DC = _getBitFieldByName("DC");
            Z = _getBitFieldByName("Z");
            OV = _getBitFieldByName("OV");
            N = _getBitFieldByName("Z");

            PD = _peekBitFieldByName("PD");
            if (PD == null)
                PD = _peekBitFieldByName("nPD");
            TO = _peekBitFieldByName("TO");
            if (TO == null)
                TO = _peekBitFieldByName("nTO");

            FSR2L = _getRegisterByName("FSR2L");
            FSR2H = _getRegisterByName("FSR2H");
            PLUSW2 = _getRegisterByName("PLUSW2");
            PREINC2 = _getRegisterByName("PREINC2");
            POSTDEC2 = _getRegisterByName("POSTDEC2");
            POSTINC2 = _getRegisterByName("POSTINC2");
            INDF2 = _getRegisterByName("INDF2");
            BSR = _getRegisterByName("BSR");
            FSR1L = _getRegisterByName("FSR1L");
            FSR1H = _getRegisterByName("FSR1H");
            PLUSW1 = _getRegisterByName("PLUSW1");
            PREINC1 = _getRegisterByName("PREINC1");
            POSTDEC1 = _getRegisterByName("POSTDEC1");
            POSTINC1 = _getRegisterByName("POSTINC1");
            INDF1 = _getRegisterByName("INDF1");
            WREG = _getRegisterByName("WREG");
            FSR0L = _getRegisterByName("FSR0L");
            FSR0H = _getRegisterByName("FSR0H");
            PLUSW0 = _getRegisterByName("PLUSW0");
            PREINC0 = _getRegisterByName("PREINC0");
            POSTDEC0 = _getRegisterByName("POSTDEC0");
            POSTINC0 = _getRegisterByName("POSTINC0");
            INDF0 = _getRegisterByName("INDF0");
            PRODL = _getRegisterByName("PRODL");
            PRODH = _getRegisterByName("PRODH");
            TABLAT = _getRegisterByName("TABLAT");
            TBLPTRL = _getRegisterByName("TBLPTRL");
            TBLPTRH = _getRegisterByName("TBLPTRH");
            TBLPTRU = _getRegisterByName("TBLPTRU");
            PCL = _getRegisterByName("PCL");
            PCLATH = _getRegisterByName("PCLATH");
            PCLATU = _getRegisterByName("PCLATU");
            STKPTR = _getRegisterByName("STKPTR");
            TOSL = _getRegisterByName("TOSL");
            TOSH = _getRegisterByName("TOSH");
            TOSU = _getRegisterByName("TOSU");

            // *Pseudo* (joined) registers

            PROD = _getRegisterByName("PROD");
            FSR0 = _getRegisterByName("FSR0");
            FSR1 = _getRegisterByName("FSR1");
            FSR2 = _getRegisterByName("FSR2");
            TOS = _getRegisterByName("TOS");
            PCLAT = _getRegisterByName("PCLAT");
            TBLPTR = _getRegisterByName("TBLPTR");

            PRODH.BitAddress = 8;
            FSR0H.BitAddress = 8;
            FSR1H.BitAddress = 8;
            FSR2H.BitAddress = 8;
            TOSH.BitAddress = 8;
            TOSU.BitAddress = 16;
            PCLATH.BitAddress = 8;
            PCLATU.BitAddress = 16;
            TBLPTRH.BitAddress = 8;
            TBLPTRU.BitAddress = 16;

            STATUS_CSHAD = _peekRegisterByName("STATUS_CSHAD");
            WREG_CSHAD = _peekRegisterByName("WREG_CSHAD");
            BSR_CSHAD = _peekRegisterByName("BSR_CSHAD");
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

            lock (_symtabLock)
            {
                if (UniqueRegNames.ContainsKey(reg.Name)) return null;      // Do not duplicate name
                if (RegsByAddr.ContainsKey(addr)) return null;   // Do not duplicate register with same address and bit width
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
        public static RegisterStorage[] GetRegisters => RegsByAddr.Values.ToArray();

        /// <summary>
        /// Gets a register by its name or <seealso cref="RegisterStorage.None"/>.
        /// </summary>
        /// <param name="name">The name as a string.</param>
        /// <returns>
        /// The register or null.
        /// </returns>
        public static RegisterStorage GetRegisterByName(string name)
        {
            PICRegisterStorage reg = _registers?._peekRegisterByName(name);
            if (reg == null)
                return RegisterStorage.None;
            return reg;
        }

        /// <summary>
        /// Gets a register by its address and bit width or <seealso cref="RegisterStorage.None"/>.
        /// </summary>
        /// <param name="address">The data memory address with bit width.</param>
        /// <returns>
        /// The register or null.
        /// </returns>
        public static RegisterStorage GetRegisterBySizedAddr(PICDataAddress address, int bwidth)
        {
            var regaddr = new RegSizedAddress(address, bwidth);
            RegisterStorage reg = _registers?._peekRegisterBySizedAddr(regaddr);
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
        public static RegisterStorage GetRegisterBySizedAddr(ushort uAddr, int bwidth)
        {
            var regaddr = new RegSizedAddress(uAddr, bwidth);
            RegisterStorage reg = _registers?._peekRegisterBySizedAddr(regaddr);
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
        public static RegisterStorage GetRegisterByNum(int number)
        {
            return _registers?._peekRegisterByNum(number) ?? RegisterStorage.None;
        }

        /// <summary>
        /// Gets a standard (core) register by its index or <seealso cref="RegisterStorage.None"/>.
        /// </summary>
        /// <param name="i">Zero-based index of the register.</param>
        /// <returns>
        /// The register or <seealso cref="RegisterStorage.None"/>.
        /// </returns>
        public static RegisterStorage GetCoreRegisterByIdx(int i)
        {
            return _registers?._peekCoreRegisterByIdx(i) ?? RegisterStorage.None;
        }

        /// <summary>
        /// Gets a register bit field by its name or null.
        /// </summary>
        /// <param name="name">The name as a string.</param>
        /// <returns>
        /// The bit field or null.
        /// </returns>
        public static FlagGroupStorage GetBitFieldByName(string name)
        {
            return _registers?._peekBitFieldByName(name);
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
        public static FlagGroupStorage GetBitFieldByAddr(PICDataAddress regAddress, uint bitPos, uint bitWidth = 0)
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
        public static FlagGroupStorage GetBitFieldByReg(PICRegisterStorage reg, uint bitPos, uint bitWidth = 0)
        {
            if (reg == null) return null;
            return GetBitFieldByAddr(reg.Address, bitPos, bitWidth);
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
        public static FlagGroupStorage GetBitFieldByName(string name, uint bitPos, uint bitWidth = 0)
        {
            return GetBitFieldByReg(GetRegisterByName(name) as PICRegisterStorage, bitPos, bitWidth);
        }

        /// <summary>
        /// Gets the maximum number of registers.
        /// </summary>
        public static int Max => RegsByAddr.Count;

        #endregion

    }

}
