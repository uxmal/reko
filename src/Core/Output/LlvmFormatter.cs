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

using Reko.Core.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Output
{
    public class LlvmFormatter : InstructionVisitor
    {
        private TextWriter w;

        public LlvmFormatter(TextWriter writer)
        {
            this.w = writer;
        }

        public void Write(Program program)
        {
            //WriteTriple();

            foreach (var proc in program.Procedures.Values)
            {
                WriteProcedure(proc);
            }
        }

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
            w.WriteLine("{0}:", block.Name);
            foreach (var stm in block.Statements)
            {
                w.Write("    ");
                stm.Instruction.Accept(this);
            }
        }

        #region InstructionVisitor methods
        public void VisitAssignment(Assignment ass)
        {
            throw new NotImplementedException();
        }

        public void VisitBranch(Branch branch)
        {
            throw new NotImplementedException();
        }

        public void VisitCallInstruction(CallInstruction ci)
        {
            throw new NotImplementedException();
        }

        public void VisitComment(CodeComment comment)
        {
            throw new NotImplementedException();
        }

        public void VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public void VisitDefInstruction(DefInstruction def)
        {
            throw new NotImplementedException();
        }

        public void VisitGotoInstruction(GotoInstruction gotoInstruction)
        {
            throw new NotImplementedException();
        }

        public void VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        public void VisitReturnInstruction(ReturnInstruction ret)
        {
            w.Write("ret ");
            if (ret.Expression == null)
            {
                w.WriteLine("void");
            }
        }

        public void VisitSideEffect(SideEffect side)
        {
            throw new NotImplementedException();
        }

        public void VisitStore(Store store)
        {
            throw new NotImplementedException();
        }

        public void VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        public void VisitUseInstruction(UseInstruction use)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
