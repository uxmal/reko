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

using Reko.Core.Loading;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;

namespace Reko.Core.Output
{
    /// <summary>
    /// This class is a <see cref="MachineInstructionRenderer"/> implementation
    /// that renders its output to a supplied <see cref="Formatter"/>.
    /// </summary>
    public class FormatterInstructionWriter : MachineInstructionRenderer
    {
        private readonly Formatter formatter;
        private readonly IDictionary<Address, Procedure> procedures;
        private readonly IDictionary<Address, ImageSymbol> symbols;
        private readonly bool separateWithTab;
        private int chars;
        private readonly List<string> annotations;
        private Address addrInstr;

        /// <summary>
        /// Creates a <see cref="FormatterInstructionWriter"/>.
        /// </summary>
        /// <param name="formatter">A <see cref="Formatter"/> to which the 
        /// machine rendering is sent.</param>
        /// <param name="separateWithTab">If true, separate the mnemonic and the
        /// first operand with a tab, otherwise use a single space.</param>
        public FormatterInstructionWriter(
            Formatter formatter,
            IDictionary<Address, Procedure> procedures,
            IDictionary<Address, ImageSymbol> symbols,
            bool separateWithTab)
        {
            this.formatter = formatter;
            this.procedures = procedures;
            this.symbols = symbols;
            this.separateWithTab = separateWithTab;
            this.annotations = new List<string>();
            this.addrInstr = Address.Ptr32(0);
        }

        public Address Address => addrInstr;

        public void BeginInstruction(Address addr)
        {
            this.addrInstr = addr;
        }

        public void EndInstruction()
        {
        }

        public void BeginOperand()
        {
        }

        public void EndOperand()
        {
        }

        public void Tab()
        {
            ++chars;
            formatter.Write(separateWithTab ? "\t" : " ");
        }

        public void WriteString(string? s)
        {
            if (s is not null)
            {
                chars += s.Length;
                formatter.Write(s);
            }
        }

        public void WriteUInt32(uint n)
        {
            var nn = n.ToString();
            chars += nn.Length;
            formatter.Write(nn);
        }

        public void WriteChar(char c)
        {
            ++chars;
            formatter.Write(c);
        }

        public void WriteFormat(string fmt, params object[] parms)
        {
            var s = string.Format(fmt, parms);
            chars += s.Length;
            formatter.Write(s);
        }

        public void WriteAddress(string formattedAddress, ulong uAddr)
        {
            chars += formattedAddress.Length;
            formatter.WriteHyperlink(formattedAddress, uAddr);
        }

        public void WriteAddress(string formattedAddress, Address addr)
        {
            if (procedures.TryGetValue(addr, out Procedure? proc))
            {
                formattedAddress = proc.Name;
            }
            else if (symbols.TryGetValue(addr, out var symbol))
            {
                formattedAddress = symbol.Name ?? formattedAddress;
            }

            chars += formattedAddress.Length;
            formatter.WriteHyperlink(formattedAddress, addr);
        }

        public void WriteMnemonic(string sMnemonic)
        {
            chars += sMnemonic.Length;
            formatter.Write(sMnemonic);
        }

        public void WriteLine()
        {
            if (annotations.Count > 0)
            {
                var pad = 60 - chars;
                if (pad > 0)
                {
                    formatter.WriteSpaces(pad);
                    chars += pad;
                }
                WriteString("; ");
                WriteString(string.Join(", ", annotations));
                annotations.Clear();
            }
            chars = 0;
            formatter.WriteLine();
        }

        public void AddAnnotation(string? annotation)
        {
            if (!string.IsNullOrEmpty(annotation))
            {
                annotations.Add(annotation);
            }
        }
    }
}
