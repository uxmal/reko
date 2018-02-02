#region License
/* 
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
using Reko.Core.Machine;
using Reko.Gui.Windows.Controls;
using System.Collections.Generic;
using System;
using System.Text;

namespace Reko.Gui.Windows
{
    /// <summary>
    /// Used to render TextSpans for use in the disassembly viewer.
    /// </summary>
    public class DisassemblyFormatter : MachineInstructionWriter 
    {
        private Program program;
        private MachineInstruction instr;
        private StringBuilder sb = new StringBuilder();
        private List<TextSpan> line;
        private List<string> annotations;

        public DisassemblyFormatter(Program program, MachineInstruction instr, List<TextSpan> line)
        {
            this.program = program;
            this.instr = instr;
            this.line = line;
            this.Platform = program.Platform;
            this.annotations = new List<string>();
        }

        public IPlatform Platform { get; private set; }
        public Address Address { get; set; }

        public void WriteOpcode(string opcode)
        {
            line.Add(new DisassemblyTextModel.InstructionTextSpan(instr, opcode + " ", UiStyles.DisassemblerOpcode));
        }

        public void WriteAddress(string formattedAddress, Address addr)
        {
            TerminateSpan();
            Procedure proc;
            TextSpan span;
            if (program.Procedures.TryGetValue(addr, out proc))
            {
                span = new DisassemblyTextModel.ProcedureTextSpan(proc, addr);
            }
            else
            {
                span = new DisassemblyTextModel.AddressTextSpan(addr, formattedAddress);
            }
            line.Add(span);
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
                Style = UiStyles.Disassembler,
            };
            line.Add(span);
            sb = new StringBuilder();
        }

        public void Write(char c)
        {
            sb.Append(c);
        }

        public void Write(uint n)
        {
            sb.Append(n);
        }

        public void Write(string s)
        {
            sb.Append(s);
        }

        public void Write(string fmt, params object[] parms)
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
