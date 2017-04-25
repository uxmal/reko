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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    public abstract class ScannerBase
    {
        public ScannerBase(Program program)
        {
            this.Program = program;
        }

        public Program Program { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        protected Procedure EnsureProcedure(Address addr, string procedureName)
        {
            Procedure proc;
            if (Program.Procedures.TryGetValue(addr, out proc))
                return proc;

            ImageSymbol sym = null;
            proc = Procedure.Create(procedureName, addr, Program.Architecture.CreateFrame());
            if (procedureName == null && Program.ImageSymbols.TryGetValue(addr, out sym))
            {
                procedureName = sym.Name;
                if (sym.Signature != null)
                {
                    var sser = Program.CreateProcedureSerializer();
                    proc.Signature = sser.Deserialize(sym.Signature, proc.Frame);
                }
            }
            if (procedureName != null)
            {
                var exp = Program.Platform.SignatureFromName(procedureName);
                if (exp != null)
                {
                    proc.Name = exp.Name;
                    proc.Signature = exp.Signature;
                    proc.EnclosingType = exp.EnclosingType;
                }
                else
                {
                    proc.Name = procedureName;
                }
            }
            Program.Procedures.Add(addr, proc);
            Program.CallGraph.AddProcedure(proc);
            return proc;
        }
    }
}
