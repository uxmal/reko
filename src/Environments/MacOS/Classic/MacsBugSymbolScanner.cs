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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Lib;
using System.Text.RegularExpressions;

namespace Reko.Environments.MacOS.Classic
{
    /*
    MacsBug accepts and returns addresses as procedure names and offsets.
    MacsBug finds names by scanning relocatable heap blocks for valid
    procedure deﬁnitions. A procedure deﬁnition, in the simplest case, 
    consists of a return instruction followed by the procedure’s name. A
    procedure is deﬁned as follows:

    - LINK A6   ($4E56 nnnn)
    This instruction is optional; if it is missing, the start of the
    procedure is assumed to be immediately after the preceding procedure,
    or at the start of the heap block.
    - 
    Procedure code 
    -
    RTS         ($4E75)
    or
    JMP(A0)     ($4ED0)
    or
    RTD nnnn    ($4E74 nnnn)
    -
    Procedure name
    -
    Procedure constants

    The procedure name can be a ﬁxed length of 8 or 16 bytes, or of variable
    length. Valid characters for procedure names are a–z, A–Z, 0–9, underscore
    (_), percent (%), period (.), and space. The space character is allowed
    only to pad fixed-length names to the maximum length.
    
    With fixed-length format, the ﬁrst byte is in the range $20 through $7F.
    The high-order bit may or may not be set. The high-order bit of the second
    byte is set for 16-character names, clear for 8-character names. Fixed-
    length 16-character names are used in object Pascal to show class.method
    names instead of procedure names. The method name is contained in the
    first 8 bytes and the class name is in the second 8 bytes. MacsBug swaps
    the order and inserts the period before displaying the name.

    With variable-length format, the ﬁrst byte is in the range $80 to $9F. 
    Stripping the high-order bit produces a length in the range $00 through
    $1F. If the length is 0, the next byte contains the actual length, in the
    range $01 through $FF. Data after the name starts on a word boundary.
    Compilers can place a procedure’s constant data immediately after the
    procedure in memory. The first word after the name speciﬁes how many bytes
    of constant data are present. If there are no constants, a length of 0
    must be given.
    */
    public class MacsBugSymbolScanner
    {
        public const ushort LINK = 0x4E56;
        public const ushort RTS = 0x4E75;
        public const ushort JMP_A0 = 0x4ED0;
        public const ushort RTD = 0x4E74;

        private IProcessorArchitecture arch;
        private BeImageReader rdr;
        private Regex reValidVariableLengthProcedureName;

        public MacsBugSymbolScanner(IProcessorArchitecture arch, MemoryArea mem)
        {
            this.arch = arch;
            this.rdr = mem.CreateBeReader(0);
            this.reValidVariableLengthProcedureName = new Regex(
                "[a-zA-Z%_]([a-zA-Z0-9%_.])*");
        }

        // Pseudo-grammar (unfortunately it's not context-free.
        // code_segment ::= ( procedure )+
        //
        // procedure ::=
        //      link_procedure trailer?
        //      linkless_procedure trailer?
        //
        // link_procedure ::= 
        //      LINK (instr) * procedure_exit
        //
        // linkless_procedure ::=
        //      (instr) * procedure
        //
        // procedure_exit:
        //      RTS
        //      JMP (a0)
        //      RTD nnnn
        //
        //  trailer:
        //      80 by [id_char]{by} pad? code_data
        //      81-9F [id_char]{by} pad? code_data
        //
        //  code_data:
        //      len [by]{len} pad?
        public List<ImageSymbol> ScanForSymbols()
        {
            var symbols = new List<ImageSymbol>();
            ImageSymbol sym;
            while (TryScanProcedure(out sym))
            {
                if (sym.Type == SymbolType.Procedure)
                {
                    symbols.Add(sym);
                }
            }
            return symbols;
        }

        private bool TryScanProcedure(out ImageSymbol sym)
        {
            ushort us;
            var addrStart = rdr.Address;
            sym = null;
            while (rdr.TryReadBeUInt16(out us))
            {
                switch (us)
                {
                case RTS:
                case JMP_A0:
                    // Found what looks like a terminator.
                    break;
                case RTD:
                    // Read uint16 of bytes to pop.
                    if (!rdr.TryReadBeUInt16(out us))
                    {
                        return false;
                    }
                    // looks like a RTD xxx instruction.
                    break;
                default:
                    // Any other words are quietly eaten.
                    continue;
                }

                // Remember the end of the procedure.
                var position = rdr.Offset;

                // We think we saw the end of the procedure. Could there be a MacsBug symbol?
                string symbol;
                if (!TryReadMacsBugSymbol(out symbol))
                {
                    // Don't really want a symbol in this case.
                    // But there might be more procedures.
                    continue;
                }

                if (!SkipConstantData())
                {
                    // That wasn't valid constant data, but there might be more procedures.
                    // But there might be more procedures.
                    continue;
                }
                
                sym = ImageSymbol.Procedure(arch, addrStart, symbol);
                return true;
            }
            return false;
        }

        private bool TryReadMacsBugSymbol(out string symbol)
        {
            var offset = rdr.Offset;
            byte b;
            symbol = null;
            if (!rdr.TryReadByte(out b))
                return false;
            int symLength = 0;
            // May have read the length byte.
            if (b == 0x80)
            {
                // long symbol: next byte is the 8-bit length.
                if (!rdr.TryReadByte(out b) || b == 0)
                {
                    rdr.Offset = offset;
                    return false;
                }
                symLength = b;
            }
            else if (0x81 <= b && b < 0xA0)
            {
                symLength = b - 0x80;
            }
            else
            {
                rdr.Offset = offset;
                return false;
            }

            // Now try reading `symLength` valid 8-bit chars.
            var symOffset = rdr.Offset;
            var sb = new StringBuilder();
            for (int i = 0; i < symLength; ++i)
            {
                if (!rdr.TryReadByte(out b))
                {
                    rdr.Offset = offset;
                    return false;
                }
                sb.Append((char)b);
            }
            if ((rdr.Offset & 1) == 1)
                ++rdr.Offset;
            symbol = sb.ToString();
            if (!this.reValidVariableLengthProcedureName.IsMatch(symbol))
            {
                symbol = null;
                rdr.Offset = offset;
                return false;
            }
            return true;
        }

        private bool SkipConstantData()
        {
            var offset = rdr.Offset;
            ushort us;
            if (!rdr.TryReadBeUInt16(out us))
                return false;
            if ((us & 1) == 1)
                ++us;
            rdr.Offset += us;
            if (rdr.Offset > rdr.Bytes.Length)
            {
                rdr.Offset = offset;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
