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
using Decompiler.Core.Lib;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Scanning
{
    public class Scanner2 : IScanner
    {
        private IProcessorArchitecture arch;
        private PriorityQueue<WorkItem2> queue;

        private const int PriorityEntryPoint = 5;
        private const int PriorityJumpTarget = 6;

        public Scanner2(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.Procedures = new SortedList<Address, Procedure>();
            this.queue = new PriorityQueue<WorkItem2>();
        }

        #region IScanner Members

        public Block AddBlock(Address addr)
        {
            throw new NotImplementedException();
        }

        public void EnqueueEntryPoint(EntryPoint ep)
        {
            queue.Enqueue(PriorityEntryPoint, new EntryPointWorkitem2());
        }

        public void EnqueueProcedure(WorkItem wiPrev, Procedure proc, Address addrProc)
        {
            throw new NotImplementedException();
        }

        public Block EnqueueJumpTarget(Address addrStart)
        {
            Block block = FindBlock(addrStart);
            if (block != null)
            {
                block = SplitBlock(block, addrStart);
            }
            else
            {
                block = AddBlock(addrStart);
                queue.Enqueue(PriorityJumpTarget, new BlockWorkitem2(this, this.arch, block, addrStart));
            }
            return block;
        }

        public Procedure EnqueueProcedure(WorkItem wiPrev, Address addr, string procedureName, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public void EnqueueUserProcedure(SerializedProcedure sp)
        {
            throw new NotImplementedException();
        }

        public Block FindBlock(Address address)
        {
            throw new NotImplementedException();
        }

        public Block SplitBlock(Block block, Address addr)
        {
            throw new NotImplementedException();
        }

        public void ProcessQueue()
        {
            while (queue.Count > 0)
            {
                var workitem = queue.Dequeue();
                workitem.Process();
            }
            var addr = new Address(0x12314);
            Procedures.Add(addr, Procedure.Create(addr, new Frame(arch.FramePointerType)));
        }

        #endregion

        public SortedList<Address, Procedure> Procedures { get; private set; }

        public abstract class WorkItem2
        {
            public abstract void Process();
        }

        private class EntryPointItem : WorkItem2
        {
            public override void Process()
            {
                throw new NotImplementedException();
            }
        }

    }
}
