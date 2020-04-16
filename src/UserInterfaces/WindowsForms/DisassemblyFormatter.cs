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

using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;
using System;
using System.Text;
using Reko.UserInterfaces.WindowsForms.Controls;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Used to render TextSpans for use in the disassembly viewer.
    /// </summary>
    public class DisassemblyFormatter : MachineInstructionWriter 
    {
        private readonly Program program;
        private readonly IProcessorArchitecture arch;
        private readonly MachineInstruction instr;
        private StringBuilder sb = new StringBuilder();
        private List<TextSpan> line;
        private List<string> annotations;

        public DisassemblyFormatter(Program program, IProcessorArchitecture arch, MachineInstruction instr, List<TextSpan> line)
        {
            this.program = program;
            this.arch = arch;
            this.instr = instr;
            this.line = line;
            this.Platform = program.Platform;
            this.annotations = new List<string>();
        }

        public IPlatform Platform { get; private set; }
        public Address Address { get; set; }

        public void WriteMnemonic(string sMnemonic)
        {
            TerminateSpan();
            line.Add(new DisassemblyTextModel.InstructionTextSpan(instr, sMnemonic + " ", Gui.UiStyles.DisassemblerOpcode));
        }

        public void WriteAddress(string formattedAddress, Address addr)
        {
            TerminateSpan();
            TextSpan span;
            if (program.Procedures.TryGetValue(addr, out Procedure proc))
            {
                span = new DisassemblyTextModel.ProcedureTextSpan(proc, addr);
            }
            else
            {
                span = new DisassemblyTextModel.AddressTextSpan(addr, formattedAddress);
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
        }

        private void TerminateSpan()
        {
            if (sb.Length == 0)
                return;
            var span = new DasmTextSpan
            {
                Text = sb.ToString(),
                Tag = null,
                Style = Gui.UiStyles.Disassembler,
            };
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

        public void WriteString(string s)
        {
            sb.Append(s);
        }

        public void WriteFormat(string fmt, params object[] parms)
        {
            sb.AppendFormat(fmt, parms);
        }

        public void AddAnnotation(string annotation)
        {
            this.annotations.Add(annotation);
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

        public class DasmTextSpan : TextSpan
        {
            public override string GetText()
            {
                return Text;
            }

            public string Text { get; set; }
        }
    }
}
