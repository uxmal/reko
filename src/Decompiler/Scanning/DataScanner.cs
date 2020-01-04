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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Core.Services;

namespace Reko.Scanning
{
    public class DataScanner : ScannerBase, IScannerQueue
    {
        private DecompilerEventListener listener;
        private Queue<WorkItem> queue;
        private ScanResults sr;
        private Dictionary<Address, ImageSymbol> procedures;

        public DataScanner(Program program, ScanResults sr, DecompilerEventListener listener)
            :base(program, listener)
        {
            this.sr = sr;
            this.listener = listener;
            this.queue = new Queue<WorkItem>();
            this.procedures = new Dictionary<Address, ImageSymbol>();
        }

        public void ProcessQueue()
        {
            while (queue.Count > 0)
            {
                if (listener.IsCanceled())
                    return;
                var wi = queue.Dequeue();
                wi.Process();
            }
        }

        public void EnqueueImageSymbol(ImageSymbol sym, bool isEntryPoint)
        {
            throw new NotImplementedException();
        }

        public void EnqueueProcedure(IProcessorArchitecture arch, Address addr)
        {
            throw new NotImplementedException();
        }

        public void EnqueueUserGlobalData(Address addr, DataType dt, string name)
        {
            if (Program.SegmentMap.IsValidAddress(addr))
            {
                var wi = new GlobalDataWorkItem(this, Program, addr, dt, name);
                queue.Enqueue(wi);
            }
        }

        public void EnqueueUserProcedure(IProcessorArchitecture arch, Address addr, FunctionType sig, string name)
        {
            if (procedures.ContainsKey(addr))
                return;
            if (IsNoDecompiledProcedure(addr))
                return;
            //$BUG: this needs to be fixed. If in an ARM binary, we scan a code 
            // address that has an odd address, we need to make it Thumb.
            procedures.Add(addr, ImageSymbol.Procedure(arch, addr, name, sig));
            sr.KnownProcedures.Add(addr);
            var proc = Program.EnsureProcedure(arch, addr, name);
            proc.Signature = sig;
        }
    }
}
