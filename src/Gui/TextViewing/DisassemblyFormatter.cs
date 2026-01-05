#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Gui.TextViewing
{
    /// <summary>
    /// Used to render TextSpans for use in the disassembly viewer.
    /// </summary>
    public class DisassemblyFormatter : MachineInstructionRenderer 
    {
        private readonly TextSpanFactory factory;
        private readonly Program program;
        private readonly IProcessorArchitecture arch;
        private readonly MachineInstruction instr;
        private StringBuilder sb = new StringBuilder();
        private List<ITextSpan> line;
        private List<string> annotations;
        private string mnemonicStyle;
        private Address addrInstr;

        public DisassemblyFormatter(
            TextSpanFactory factory,
            Program program, 
            IProcessorArchitecture arch, 
            MachineInstruction instr, 
            List<ITextSpan> line)
        {
            this.factory = factory;
            this.program = program;
            this.arch = arch;
            this.instr = instr;
            this.line = line;
            this.addrInstr = default!;
            this.annotations = new List<string>();
            this.mnemonicStyle = Gui.Services.UiStyles.DisassemblerOpcode;
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

        public void WriteMnemonic(string sMnemonic)
        {
            TerminateSpan();
            line.Add(factory.CreateInstructionTextSpan(instr, sMnemonic + " ", this.mnemonicStyle));
            TerminateSpan();
        }

        public void WriteAddress(string formattedAddress, Address addr)
        {
            TerminateSpan();
            ITextSpan span;
            if (program.Procedures.TryGetValue(addr, out Procedure proc))
            {
                span = factory.CreateProcedureTextSpan(proc, addr);
            }
            else if (program.ImageSymbols.TryGetValue(addr, out var symbol))
            {
                span = factory.CreateAddressTextSpan(addr, symbol.Name ?? formattedAddress);
            }
            else
            {
                span = factory.CreateAddressTextSpan(addr, formattedAddress);
            }
            line.Add(span);
        }

        public void WriteAddress(string formattedAddres, ulong uAddr)
        {
            WriteAddress(formattedAddres, Address.Ptr64(uAddr));
        }

        public void Tab()
        {
            TerminateSpan();
            this.mnemonicStyle = Gui.Services.UiStyles.DisassemblerOpcodeColor;
        }

        private void TerminateSpan()
        {
            if (sb.Length == 0)
                return;

            var span = factory.CreateTextSpan(sb.ToString(), Gui.Services.UiStyles.Disassembler);
            line.Add(span);
            sb = new StringBuilder();
        }

        public void WriteChar(char c)
        {
            sb.Append(c);
        }

        public void WriteUInt32(uint n)
        {
            sb.Append(n);
        }

        public void WriteString(string? s)
        {
            sb.Append(s);
        }

        public void WriteFormat(string fmt, params object[] parms)
        {
            sb.AppendFormat(fmt, parms);
        }

        public void AddAnnotation(string? annotation)
        {
            if (annotation is not null)
            {
                this.annotations.Add(annotation);
            }
        }

        public void NewLine()
        {
            if (annotations.Count > 0)
            {
                int padding = 60 - sb.Length;
                if (padding > 0)
                    sb.Append(' ', padding);
                sb.AppendFormat("; {0}", string.Join(", ", annotations));
            }
            TerminateSpan();
        }
    }
}
