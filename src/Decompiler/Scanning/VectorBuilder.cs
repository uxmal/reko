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
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.Scanning
{
    /// <summary>
    /// Builds a jump or call table by backtracking from the call site to find a comparison with a constant.
    /// This is (usually!) the upper bound of the size of the table.
    /// </summary>
    public class VectorBuilder : IBackWalkHost<Block,Instruction>
    {
        private IServiceProvider services;
        private Program program;
        private Backwalker<Block,Instruction> bw;
        private DirectedGraphImpl<object> jumpGraph;        //$TODO:

        public VectorBuilder(IServiceProvider services, Program program, DirectedGraphImpl<object> jumpGraph)
        {
            this.services = services;
            this.program = program;
            this.jumpGraph = jumpGraph;
        }

        public IProcessorArchitecture Architecture => program.Architecture;
        public SegmentMap SegmentMap => program.SegmentMap;
        public int TableByteSize { get; private set; }
        public Program Program => program;

        public Tuple<Expression,Expression> AsAssignment(Instruction instr)
        {
            var ass = instr as Assignment;
            if (ass == null)
                return null;
            return Tuple.Create((Expression)ass.Dst, ass.Src);
        }

        public Expression AsBranch(Instruction instr)
        {
            var bra = instr as Branch;
            if (bra == null)
                return null;
            return bra.Condition;
        }

        public List<Address> Build(Address addrTable, Address addrFrom, ProcessorState state)
        {
            bw = new Backwalker<Block,Instruction>(this, null, null);
            if (bw == null)
                return null;
            List<BackwalkOperation> operations = bw.BackWalk(null);
            if (operations == null)
                return PostError("Unable to determine limit", addrFrom, addrTable);
            return BuildAux(bw, addrFrom, state);
        }

        public List<Address> BuildAux(Backwalker<Block, Instruction> bw, Address addrFrom, ProcessorState state)
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

            return BuildTable(
                bw.VectorAddress, 
                limit, 
                permutation,
                (bw.Stride == 1 || bw.Stride == 0) && bw.JumpSize > 1 
                    ? bw.JumpSize 
                    : bw.Stride,
                state);
        }

        private int[] BuildMapping(BackwalkDereference deref, int limit)
        {
            int[] map = new int[limit];
            var addrTableStart = Address.Ptr32((uint)deref.TableOffset); //$BUG: breaks on 64- and 16-bit platforms.
            if (!program.SegmentMap.IsValidAddress(addrTableStart))
                return new int[0];      //$DEBUG: look into this case.
            var rdr = program.CreateImageReader(program.Architecture, addrTableStart);
            for (int i = 0; i < limit; ++i)
            {
                map[i] = rdr.ReadByte();
            }
            return map;
        }

        /// <summary>
        /// Builds a list of addresses that will be used as a jump or call vector.
        /// </summary>
        /// <param name="addrTable">The address at which the table starts.</param>
        /// <param name="limit">The number of bytes that comprise the table</param>
        /// <param name="permutation">If not null, a permutation of the items in the table</param>
        /// <param name="stride">The size of the individual addresses in the table.</param>
        /// <param name="state">Current processor state.</param>
        /// <returns>The target addresses reached by the vector</returns>
        public List<Address> BuildTable(
            Address addrTable,
            int limit, 
            int[] permutation,
            int stride,
            ProcessorState state)
        {
            List<Address> vector = new List<Address>();

            var arch = program.Architecture;
            if (permutation != null)
            {
                int cbEntry = stride;
                int iMax = 0;
                for (int i = 0; i < permutation.Length; ++i)
                {
                    if (permutation[i] > iMax)
                        iMax = permutation[i];
                    var entryAddr = addrTable + (uint)(permutation[i] * cbEntry);
                    var addr = arch.ReadCodeAddress(0, program.CreateImageReader(arch, entryAddr), state);
                    if (addr != null)
                    { 
                        vector.Add(addr);
                    }
                }
            }
            else
            {
                EndianImageReader rdr = program.CreateImageReader(arch, addrTable);
                int cItems = limit / stride;
                var segmentMap = program.SegmentMap;
                for (int i = 0; i < cItems; ++i)
                {
                    var entryAddr = program.Architecture.ReadCodeAddress(stride, rdr, state);
                    if (entryAddr == null || !segmentMap.IsValidAddress(entryAddr))
                    {
                        if (services != null)
                        {
                            var diagSvc = services.RequireService<DecompilerEventListener>();
                            diagSvc.Warn(
                                diagSvc.CreateAddressNavigator(program, addrTable),
                                "The call or jump table has invalid addresses; stopping.");
                        }
                        break;
                    }
                    vector.Add(entryAddr);
                }
                TableByteSize = limit;
            }
            return vector;
        }

        public RegisterStorage GetSubregister(RegisterStorage reg, int offset, int width)
        {
            throw new NotImplementedException();
        }

        public Block GetSinglePredecessor(Block block)
        {
            return block.Procedure.ControlGraph.Predecessors(block).FirstOrDefault();
        }

        public List<Block> GetPredecessors(Block block)
        {
            return block.Pred.ToList();
        }

        public AddressRange GetSinglePredecessorAddressRange(Address addr)
        {
            ImageMapBlock block = null;
            foreach (Address addrPred in this.jumpGraph.Predecessors(addr))
            {
                if (block != null)
                    return null;
                ImageMapItem item;
                if (!program.ImageMap.TryFindItem(addrPred, out item))
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
            if (!program.ImageMap.TryFindItem(addr, out item))
                return null;
            ImageMapBlock block = item as ImageMapBlock;
            if (block == null)
                return null;
            return block.Address;
        }

        public Address MakeAddressFromConstant(Constant c)
        {
            return program.Platform.MakeAddressFromConstant(c, true);
        }

        public Address MakeSegmentedAddress(Constant seg, Constant off)
        {
            return program.Architecture.MakeSegmentedAddress(seg, off);
        }

        public bool IsFallthrough(Instruction instr, Block block)
        {
            var bra = instr as Branch;
            if (bra == null)
                return false;
            return bra.Target != block;
        }

        public RegisterStorage IndexRegister
        {
            get { return bw != null ? bw.Index: RegisterStorage.None; }
        }

        public bool IsStackRegister(Storage stg)
        {
            return stg == program.Architecture.StackRegister;
        }

        private List<Address> PostError(string err, Address addrInstr, Address addrTable)
        {
            Debug.WriteLine(string.Format("Instruction at {0}, table at {1}: {2}", addrInstr, addrTable, err));
            return new List<Address>();
        }

        public bool IsValidAddress(Address addr)
        {
            return program.SegmentMap.IsValidAddress(addr);
        }

        public IEnumerable<Instruction> GetBlockInstructions(Block block)
        {
            return block.Statements.Select(s => s.Instruction);
        }

        public int BlockInstructionCount(Block block)
        {
            return block.Statements.Count;
        }
    }
}
