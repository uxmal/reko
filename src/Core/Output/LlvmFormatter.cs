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
using System.IO;
using System.Linq;

namespace Reko.Core.Output
{
    /// <summary>
    /// Formats an instruction in LLVM IR format.
    /// </summary>
    public class LlvmFormatter : InstructionVisitor
    {
        private readonly TextWriter w;

        /// <summary>
        /// Constructs an instance of <see cref="LlvmFormatter"/>.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        public LlvmFormatter(TextWriter writer)
        {
            this.w = writer;
        }

        /// <summary>
        /// Writes a whole program.
        /// </summary>
        /// <param name="program">Program to write.</param>
        public void Write(Program program)
        {
            //WriteTriple();

            foreach (var proc in program.Procedures.Values)
            {
                WriteProcedure(proc);
            }
        }

        /// <summary>
        /// Writes a procedure in LLVM format.
        /// </summary>
        /// <param name="proc"><see cref="Procedure"/> to write.</param>
        public void WriteProcedure(Procedure proc)
        {
            // write attrs
            w.Write("define ");
            if (proc.Signature.ParametersValid)
            {
                //WriteTypeRef(proc.Signature.ReturnValue);
                w.Write(proc.Name);
            }
            else
            {
                w.Write("void");
            }
            w.WriteLine(" @{0}() {1}", proc.Name, "{");

            WriteProcedureBody(proc);
            w.WriteLine("}");
        }

        private void WriteProcedureBody(Procedure proc)
        {
            var blocks = proc.SortBlocksByName().ToArray();
            for (int i = 0; i < blocks.Length; ++i)
            {
                if (blocks[i] == proc.ExitBlock)
                    continue;
                WriteBlock(blocks[i], i, blocks);
            }
        }

        private void WriteBlock(Block block, int i, Block[] blocks)
        {
            w.WriteLine("{0}:", block.DisplayName);
            foreach (var stm in block.Statements)
            {
                w.Write("    ");
                stm.Instruction.Accept(this);
            }
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

        void InstructionVisitor.VisitGotoInstruction(GotoInstruction gotoInstruction)
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
            w.Write("ret ");
            if (ret.Expression is null)
            {
                w.WriteLine("void");
            }
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
