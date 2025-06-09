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

using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Collections;
using System.Collections.Generic;

namespace Reko.Scanning
{
    /// <summary>
    /// Injects user-defined comments in code areas as CodeComment instructions.
    /// </summary>
    /// <remarks>
    /// This class is responsible for the flow of comments 
    /// from (Program.User.Annotation) ===> (Procedure). It can only 
    /// happen when scanning. The flow (Procedure) ===> (Program.User.Annotation)
    /// is carried out by the GUI. The command line client has no way of 
    /// adding annotations after loading the Reko project file.
    /// </remarks>
    public class CommentInjector
    {
        private SortedList<Address, Annotation> annotations;

        public CommentInjector(IEnumerable<Annotation> annotations)
        {
            this.annotations = annotations.ToSortedList(a => a.Address);
        }

        /// <summary>
        /// Creates comment statements in the locations specified by user.
        /// </summary>
        /// <param name="proc"></param>
        public void InjectComments(Procedure proc)
        {
            foreach (var block in proc.ControlGraph.Blocks)
            {
                if (block == proc.EntryBlock || block == proc.ExitBlock)
                    continue;

                Statement? stmPrev = null;
                for (int i = 0; i < block.Statements.Count; ++i)
                {
                    var stm = block.Statements[i];
                    if (stmPrev is null || stm.Address != stmPrev.Address)
                    {
                        if (this.annotations.TryGetValue(stm.Address, out var ann))
                        {
                            block.Statements.Insert(i, stm.Address, new CodeComment(ann.Text));
                            ++i;
                        }
                    }
                    stmPrev = stm;
                }
            }
        }
    }
}