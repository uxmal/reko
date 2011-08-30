#region License
/* 
 * Copyright (C) 1999-2011 John Källén.
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
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Decompiler.Scanning
{
    /// <summary>
    /// Builds a jump or call table by backtracking from the call site to find a comparison with a constant.
    /// This is (usually!) the upper bound of the size of the table.
    /// </summary>
    public class VectorBuilder : IBackWalkHost
    {
        private IProcessorArchitecture arch;
        private IScanner scanner;
        private ProgramImage image;
        private ImageMap imageMap;
        private int cbTable;
        private Backwalker bw;
        private DirectedGraphImpl<object> jumpGraph;        //$TODO:

        [Obsolete]
        public VectorBuilder(Program prog, ImageMap imageMap, DirectedGraphImpl<object> jumpGraph)
        {
            this.arch = prog.Architecture;
            this.imageMap = imageMap;
            this.jumpGraph = jumpGraph;
        }

        public VectorBuilder(IProcessorArchitecture arch, ProgramImage image, DirectedGraphImpl<object> jumpGraph)
        {
            this.arch = arch;
            this.image = image;
            this.imageMap = image.Map;
            this.jumpGraph = jumpGraph;
        }

        public VectorBuilder(IProcessorArchitecture arch, IScanner scanner, DirectedGraphImpl<object> jumpGraph)
        {
            this.arch = arch;
            this.scanner = scanner;
            this.jumpGraph = jumpGraph;
        }

        public List<Address> Build(Address addrTable, Address addrFrom, ProcessorState state)
        {
            bw = new Backwalker(this, null, null);
            if (bw == null)
                return null;
            List<BackwalkOperation> operations = bw.BackWalk(null);
            if (operations == null)
                return PostError("Unable to determine limit", addrFrom, addrTable);
            return BuildAux(bw, addrFrom, state);
        }

        public List<Address> BuildAux(Backwalker bw, Address addrFrom, ProcessorState state)
        {
            int limit = 0;
            int[] permutation = null;
            foreach (BackwalkOperation op in bw.Operations)
            {
                BackwalkError err = op as BackwalkError;
                if (err != null)
                {
                    return PostError(err.ErrorMessage, addrFrom, bw.VectorAddress);
                }
                var deref = op as BackwalkDereference;
                if (deref != null)
                {
                    permutation = BuildMapping(deref, limit);
                }
                limit = op.Apply(limit);
            }
            if (limit == 0)
                return PostError("Unable to determine limit", addrFrom, bw.VectorAddress);

            return BuildTable(bw.VectorAddress, limit, permutation, bw.Stride, state);
        }

        private int[] BuildMapping(BackwalkDereference deref, int limit)
        {
            int[] map = new int[limit];
            var rdr = scanner.Image.CreateReader(new Address((uint)deref.TableOffset));
            for (int i = 0; i < limit; ++i)
            {
                map[i] = rdr.ReadByte();
            }
            return map;
        }

        private List<Address> BuildTable(Address addrTable, int limit, int[] permutation, int stride, ProcessorState state)
        {
            List<Address> vector = new List<Address>();
            if (permutation != null)
            {
                int cbEntry = stride;
                int iMax = 0;
                for (int i = 0; i < permutation.Length; ++i)
                {
                    if (permutation[i] > iMax)
                        iMax = permutation[i];
                    var entryAddr = (uint) (addrTable-scanner.Image.BaseAddress) + (uint)(permutation[i] * cbEntry);
                    vector.Add(new Address(scanner.Image.ReadLeUInt32(entryAddr)));      //$BUG: will fail on 64-bit arch.
                }
            }
            else
            {
                ImageReader rdr = scanner.CreateReader(addrTable);
                int cItems = limit / (int)stride;
                for (int i = 0; i < cItems; ++i)
                {
                    vector.Add(arch.ReadCodeAddress(stride, rdr, state));
                }
                cbTable = limit;
            }
            return vector;

        }

        public Block GetSinglePredecessor(Block block)
        {
            return block.Procedure.ControlGraph.Predecessors(block).FirstOrDefault();
        }

        public AddressRange GetSinglePredecessorAddressRange(Address addr)
        {
            ImageMapBlock block = null;
            foreach (Address addrPred in this.jumpGraph.Predecessors(addr))
            {
                if (block != null)
                    return null;
                ImageMapItem item;
                if (!imageMap.TryFindItem(addrPred, out item))
                    return null;
                block = item as ImageMapBlock;
            }
            if (block == null)
                return null;
            else
                return new AddressRange(block.Address, block.Address + block.Size);
        }

        public Address GetBlockStartAddress(Address addr)
        {
            ImageMapItem item;
            if (!imageMap.TryFindItem(addr, out item))
                return null;
            ImageMapBlock block = item as ImageMapBlock;
            if (block == null)
                return null;
            return block.Address;
        }

        public MachineRegister IndexRegister
        {
            get { return bw != null ? bw.Index: MachineRegister.None; }
        }

        private List<Address> PostError(string err, Address addrInstr, Address addrTable)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("Instruction at {0}, table at {1}: {2}", addrInstr, addrTable, err));
            return new List<Address>();
        }

        public int TableByteSize
        {
            get { return cbTable; }
        }
    }
}
