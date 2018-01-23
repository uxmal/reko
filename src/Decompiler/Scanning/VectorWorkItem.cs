#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Scanning
{
    public class VectorWorkItem : WorkItem
    {
        public ImageMapVectorTable Table;
        public ProcessorState State;
        public Address AddrFrom;			// address from which the jump is called.
        public PrimitiveType Stride;
        public ushort SegBase;

        private IScanner scanner;
        private Program program;
        private Procedure proc;
        private Dictionary<Address, VectorUse> vectorUses;
        private bool isCallTable;

        public VectorWorkItem(IScanner scanner, Program program, ImageMapVectorTable table, bool isCallTable, Procedure proc)
            : base(table.Address)
        {
            this.scanner = scanner;
            this.program = program;
            this.Table = table;
            this.proc = proc;
            this.isCallTable = isCallTable;
            this.vectorUses = new Dictionary<Address, VectorUse>();
        }

        public override void Process()
        {
            var builder = new VectorBuilder(scanner.Services, program, new DirectedGraphImpl<object>());
            var vector = builder.Build(Table.Address, AddrFrom, State);
            if (vector.Count == 0)
            {
                Address addrNext = Table.Address + Stride.Size;
                if (program.SegmentMap.IsValidAddress(addrNext))
                {
                    // Can't determine the size of the table, but surely it has one entry?
                   program.ImageMap.AddItem(addrNext, new ImageMapItem());
                }
                return;
            }

            Table.Addresses.AddRange(vector);
            for (int i = 0; i < vector.Count; ++i)
            {
                var st = State.Clone();
                if (isCallTable)
                {
                    scanner.ScanProcedure(vector[i], null, st);
                }
                else
                {
                    scanner.EnqueueJumpTarget(AddrFrom, vector[i], proc, st);
                }
            }
            vectorUses[AddrFrom] = new VectorUse(Table.Address, builder.IndexRegister);
            program.ImageMap.AddItem(Table.Address + builder.TableByteSize, new ImageMapItem());
        }
    }
}
