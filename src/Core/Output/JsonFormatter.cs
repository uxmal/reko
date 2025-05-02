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

using Reko.Core.Code;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.Core.Output
{
    /// <summary>
    /// Writes the contents of a <see cref="Program"/> in JSON format.
    /// </summary>
    public class JsonFormatter : InstructionVisitor
    {
        private readonly JsonWriter w;

        /// <summary>
        /// Constructs a JSON formatter.
        /// </summary>
        /// <param name="w">Output sink.</param>
        public JsonFormatter(TextWriter w)
        {
            this.w = new JsonWriter(w);
        }

        /// <summary>
        /// Flushes any pending characters to the output device.
        /// </summary>
        public void Flush()
        {
            w.Flush();
        }

        /// <summary>
        /// Writes a whole <see cref="Program"/> as JSON.
        /// </summary>
        /// <param name="program">Program to format.</param>
        public void WriteProgram(Program program)
        {
            w.WriteStartObject();
            w.WritePropertyName("version");
            w.Write("reko-IR-v1");
            w.WritePropertyName("procs");
            w.WriteStartArray();
            foreach (var proc in program.Procedures.Values)
            {
                 WriteProcedure(proc.EntryAddress, proc);
            }
            w.WriteEnd();
            w.WriteEnd();
        }

        /// <summary>
        /// Writes a <see cref="Procedure"/> as JSON.
        /// </summary>
        /// <param name="addr">Address to format.</param>
        /// <param name="proc"><see cref="Procedure"/>) to format.</param>
        public void WriteProcedure(Address addr, Procedure proc)
        {
            w.WriteStartObject();
            w.WritePropertyName("name");
            w.Write(proc.Name);
            w.WritePropertyName("addr");
            w.Write(addr.ToString());
            w.WritePropertyName("blocks");

            w.WriteStartArray();
            foreach (var block in proc.ControlGraph.Blocks)
            {
                if (block == proc.ExitBlock)
                    return;
            }
            w.WriteEnd();
            w.WriteEnd();
        }

        private void WriteBlock(Block b)
        {
            w.WriteStartObject();
            w.WritePropertyName("name");
            w.Write(b.DisplayName);
            w.WritePropertyName("stms");

            w.WriteStartArray();
            foreach (var stm in b.Statements)
            {
                stm.Instruction.Accept(this);
            }
            w.WriteEnd();
            w.WriteEnd();
        }

        #region InstructionVisitor methods

        /// <inheritdoc/>
        public void VisitAssignment(Assignment ass)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitBranch(Branch branch)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitCallInstruction(CallInstruction ci)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitComment(CodeComment comment)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitGotoInstruction(GotoInstruction g)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitReturnInstruction(ReturnInstruction ret)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitSideEffect(SideEffect side)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitStore(Store store)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void VisitUseInstruction(UseInstruction use)
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}
