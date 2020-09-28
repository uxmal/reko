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

namespace Reko.Core.Output
{
    /// <summary>
    /// Renders an intermediate code procedure.
    /// </summary>
    public class ProcedureFormatter
    {
        private CodeFormatter formatter;
        private BlockDecorator decorator;
        private Procedure proc;

        public ProcedureFormatter(Procedure procedure, CodeFormatter formatter)
        {
            this.proc = procedure;
            this.decorator = new BlockDecorator();
            this.formatter = formatter;
        }

        public ProcedureFormatter(Procedure procedure, BlockDecorator decorator, CodeFormatter formatter)
        {
            this.proc = procedure;
            this.decorator = decorator;
            this.formatter = formatter;
        }

        public void WriteProcedureBlocks()
        {
            var blocks = proc.SortBlocksByName().ToArray();
            for (var i = 0; i < blocks.Length; ++i)
            {
                var block = blocks[i];
                if (block == null)
                    continue;
                Comment(block, decorator.BeforeBlock);
                WriteBlock(block, formatter);
                if (block != proc.ExitBlock)
                {
                    var succ = block.Succ;
                    if (succ.Count == 1)
                    {
                        var ret = block.Statements.Count > 0 && (block.Statements.Last.Instruction is ReturnInstruction);
                        if (!ret && (i == blocks.Length - 1 || succ[0] != blocks[i + 1]))
                        {
                            WriteGoto(succ[0] != null ? succ[0].Name : "(null)");
                        }
                    }
                    else if (succ.Count == 2 && block.Statements.Count > 0)
                    {
                        var br = block.Statements.Last.Instruction is Branch;
                        if (br && (i == blocks.Length - 1 || succ[0] != blocks[i + 1]))
                        {
                            WriteGoto(succ[0].Name);
                        }
                    }
                }
                Comment(block, decorator.AfterBlock);
            }
        }

        private void Comment(Block block, Action<Block, List<string>> generator)
        {
            var lines = new List<string>();
            generator(block, lines);
            foreach (var line in lines)
            {
                formatter.InnerFormatter.Indent();
                formatter.InnerFormatter.WriteComment("// " + line);
                formatter.InnerFormatter.WriteLine();
            }
        }

        public void WriteBlock(Block block, CodeFormatter writer)
        {
            if (!string.IsNullOrEmpty(block.Name))
            {
                writer.InnerFormatter.Write(block.Name);
                writer.InnerFormatter.Write(":");
                writer.InnerFormatter.WriteLine();
            }
            foreach (var stm in block.Statements)
                stm.Instruction.Accept(writer);
        }

        private void WriteGoto(string label)
        {
            formatter.InnerFormatter.Indent();
            formatter.InnerFormatter.WriteKeyword("goto");
            formatter.InnerFormatter.Write(" ");
            formatter.InnerFormatter.Write(label);
            formatter.InnerFormatter.WriteLine();
        }
    }
}
