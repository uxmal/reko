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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Arch.X86;
using Reko.Assemblers.x86;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Scanning
{
    class ScannerTestPrograms
    {
        //$TODO: translate to RtlTraces
        /// <summary>
        /// Simple program consisting of a single procedure.
        /// </summary>
        /// <returns></returns>
        public Program Simple()
        {
            var b = new ProgramBuilder();
            b.Add("bob", m =>
            {
                m.Label("Zlon");
                m.Return();
            });
            return b.BuildProgram();
        }

        /// <summary>
        /// This program has a cross procedural jump
        /// that should result in a new procedure, at Real_entry,since the jumped-to code 
        /// is not a simple linear block but a branch.
        /// </summary>
        /// <returns></returns>
        public static Program CrossJump()
        {
            var b = new ProgramBuilder();
            b.Add("bob", m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Label("bob_1");
                m.Assign(r1, 0);
                // Fall through should be promoted to call/return pair.

                m.Label("Real_entry"); // Cross jump target: should become a new function entry point.
                m.MStore(r2, r1);
                m.BranchIf(r2, "Real_entry");
                m.Return();
            });

            b.Add("ext", m =>
            {
                var r1 = m.Reg32("r1", 1);
                m.Label("ext_1");
                m.Assign(r1, 4);
                m.Goto("Real_entry");
            });
            return b.BuildProgram();
        }

        /// <summary>
        /// This program has a cross procedural jump
        /// that should result in a cloned procedure, since the jumped-to code is a simple linear block
        /// followed by a return statement.
        /// </summary>
        /// <returns></returns>
        private Program CrossJumpLinearReturn()
        {
            var b = new ProgramBuilder();
            b.Add("bob", m =>
            {
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.Label("bob_1");
                m.Assign(r1, 0);
                m.Label("Real_entry");
                m.MStore(r2, r1);
                m.Return();
            });

            b.Add("ext", m =>
            {
                var r1 = m.Reg32("r1", 1);
                m.Label("ext_1");
                m.Assign(r1, 4);
                m.Goto("Real_entry");
            });
            return b.BuildProgram();
        }
    }
}
