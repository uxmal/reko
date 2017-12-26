#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Absyn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Structure
{
    /// <summary>
    /// Removes reduntant 'return' statements in procedures that return
    /// void. These return statements will always be in tail position.
    /// </summary>
    public class TailReturnRemover
    {
        private Procedure proc;

        public TailReturnRemover(Procedure proc)
        {
            this.proc = proc;
        }

        public void Transform()
        {
            if (!proc.Signature.HasVoidReturn)
                return;
            var stmts = proc.Body;
            RemoveRedundantReturn(stmts);
        }

        private void RemoveRedundantReturn(List<AbsynStatement> stmts)
        {
            while (stmts.Count > 0)
            {
                int i = stmts.Count - 1;
                if (stmts[i] is AbsynReturn)
                {
                    Debug.Assert(((AbsynReturn)stmts.Last()).Value == null);
                    stmts.RemoveAt(i);
                }
                else
                {
                    return;
                }
            }
        }
    }
}
