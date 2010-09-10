#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Mocks
{
    public class LowLevelStatementStream : CodeEmitter2
    {
        private Frame frame;
        private List<Statement> stms;
        private Block block;
        private IProcessorArchitecture arch;

        public LowLevelStatementStream()
        {
            this.frame = new Frame(PrimitiveType.Word32);
            this.block = new Block(null, "test");
            this.arch = new ArchitectureMock();
        }

        public override Statement Emit(Instruction instr)
        {
            block.Statements.Add(instr);
            return block.Statements.Last;
        }

        public Block Block
        {
            get { return block; }
        }

        public override Frame Frame
        {
            get { return frame; }
        }

        public override Identifier Register(int i)
        {
            return Frame.EnsureRegister(arch.GetRegister(i));
        }


        public IEnumerator<RewrittenInstruction> GetRewrittenInstructions()
        {
            uint linaddr = 0x100000;
            foreach (Statement stm in block.Statements)
            {
                var i = new RewrittenInstruction();
                i.Instruction = stm.Instruction;
                i.Address = new Address(linaddr++);
                i.Length = 1;
                yield return i;
            }
        }

    }
}
