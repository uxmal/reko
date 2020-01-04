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
    public class JsonFormatter : InstructionVisitor
    {
        private JsonWriter w;

        public JsonFormatter(TextWriter w)
        {
            this.w = new JsonWriter(w);
        }

        public void Flush()
        {
            w.Flush();
        }

        public void WriteProgram(Program program)
        {
            w.WriteStartObject();
            w.WritePropertyName("version");
            w.Write("reko-IR-v1");
            w.WritePropertyName("procs");
            w.WriteStartArray();
            foreach (var proc in program.Procedures)
            {
                 WriteProcedure(proc);
            }
            w.WriteEnd();
            w.WriteEnd();
        }

        public void WriteProcedure(KeyValuePair<Address,Procedure> de)
        {
            var proc = de.Value;
            w.WriteStartObject();
            w.WritePropertyName("name");
            w.Write(de.Value.Name);
            w.WritePropertyName("addr");
            w.Write(de.Key.ToString());
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
            w.Write(b.Name);
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

        public void VisitGotoInstruction(GotoInstruction g)
        {
            throw new NotImplementedException();
        }

        public void VisitPhiAssignment(PhiAssignment phi)
        {
            throw new NotImplementedException();
        }

        public void VisitReturnInstruction(ReturnInstruction ret)
        {
            throw new NotImplementedException();
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
