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
using Reko.Core.Expressions;
using Reko.Core.Graphs;
using Reko.Core.Lib;
using Reko.Core.Services;
using Reko.Services;
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
        private readonly IServiceProvider services;
        private readonly Program program;
        private readonly DirectedGraphImpl<object> jumpGraph;        //$TODO:
        private Backwalker<Block, Instruction>? bw;

        public VectorBuilder(IServiceProvider services, Program program, DirectedGraphImpl<object> jumpGraph)
        {
            this.services = services;
            this.program = program;
            this.jumpGraph = jumpGraph;
        }

        public IProcessorArchitecture Architecture => program.Architecture;
        public int TableByteSize { get; private set; }
        public Program Program => program;

        public (Expression?,Expression?) AsAssignment(Instruction instr)
        {
            if (instr is Assignment ass)
                return (ass.Dst, ass.Src);
            return (null,null);
        }

        public Expression? AsBranch(Instruction instr)
        {
            if (instr is Branch bra)
                return bra.Condition;
            return null;
        }

        public List<Address> Build(Address addrTable, Address addrFrom, ProcessorState state)
        {
            //$BUG: all these nulls!
            bw = new Backwalker<Block,Instruction>(this, null!, null!);
            List<BackwalkOperation>? operations = bw.BackWalk(null!);
            if (operations is null)
                return PostError("Unable to determine limit", addrFrom, addrTable);
            return BuildAux(bw, addrFrom, state);
        }

        public List<Address> BuildAux(Backwalker<Block, Instruction> bw, Address addrFrom, ProcessorState state)
        {
            int limit = 0;
            int[]? permutation = null;
            foreach (BackwalkOperation op in bw.Operations)
            {
                if (op is BackwalkError err)
                {
                    return PostError(err.ErrorMessage, addrFrom, bw.VectorAddress ?? default);
                }
                if (op is BackwalkDereference deref)
                {
                    permutation = BuildMapping(deref, limit);
                }
                limit = op.Apply(limit);
            }
            if (limit == 0)
                return PostError("Unable to determine limit", addrFrom, bw.VectorAddress ?? default);

            return BuildTable(
                bw.VectorAddress!.Value, 
                limit, 
                permutation!,
                (bw.Stride == 1 || bw.Stride == 0) && bw.JumpSize > 1 
                    ? bw.JumpSize 
                    : bw.Stride,
                state);
        }

        private int[] BuildMapping(BackwalkDereference deref, int limit)
        {
            int[] map = new int[limit];
            var addrTableStart = Address.Ptr32((uint)deref.TableOffset); //$BUG: breaks on 64- and 16-bit platforms.
            if (!program.TryCreateImageReader(program.Architecture, addrTableStart, out var rdr))
                return Array.Empty<int>();      //$DEBUG: look into this case.
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
            var vector = new List<Address>();

            var arch = program.Architecture;
            if (permutation is not null)
            {
                int cbEntry = stride;
                int iMax = 0;
                for (int i = 0; i < permutation.Length; ++i)
                {
                    if (permutation[i] > iMax)
                        iMax = permutation[i];
                    var entryAddr = addrTable + (uint) (permutation[i] * cbEntry);
                    if (program.TryCreateImageReader(arch, entryAddr, out var rdr))
                    {
                        var addr = arch.ReadCodeAddress(0, rdr, state);
                        if (addr is not null)
                        {
                            vector.Add(addr.Value);
                        }
                    }
                }
            }
            else
            {
                if (!program.TryCreateImageReader(arch, addrTable, out var rdr))
                    return vector;
                int cItems = limit / stride;
                var memory = program.Memory;
                for (int i = 0; i < cItems; ++i)
                {
                    var entryAddr = program.Architecture.ReadCodeAddress(stride, rdr, state);
                    if (entryAddr is null || !memory.IsValidAddress(entryAddr.Value))
                    {
                        if (services is not null)
                        {
                            var diagSvc = services.RequireService<IDecompilerEventListener>();
                            diagSvc.Warn(
                                diagSvc.CreateAddressNavigator(program, addrTable),
                                "The call or jump table has invalid addresses; stopping.");
                        }
                        break;
                    }
                    vector.Add(entryAddr.Value);
                }
                TableByteSize = limit;
            }
            return vector;
        }

        public RegisterStorage GetSubregister(RegisterStorage reg, BitRange range)
        {
            throw new NotImplementedException();
        }

        public Block? GetSinglePredecessor(Block block)
        {
            return block.Procedure.ControlGraph.Predecessors(block).FirstOrDefault();
        }

        public List<Block> GetPredecessors(Block block)
        {
            return block.Pred.ToList();
        }

        public Address? MakeAddressFromConstant(Constant c)
        {
            return program.Platform.MakeAddressFromConstant(c, true);
        }

        public Address MakeSegmentedAddress(Constant seg, Constant off)
        {
            return program.Architecture.MakeSegmentedAddress(seg, off);
        }

        public bool IsFallthrough(Instruction instr, Block block)
        {
            if (instr is Branch bra)
                return bra.Target != block;
            return false;
        }

        public RegisterStorage IndexRegister
        {
            get { return bw is not null ? bw.Index! : RegisterStorage.None; }
        }

        public bool IsStackRegister(Storage stg)
        {
            return stg == program.Architecture.StackRegister;
        }

        private static List<Address> PostError(string err, Address addrInstr, Address addrTable)
        {
            Debug.WriteLine($"Instruction at {addrInstr}, table at {addrTable}: {err}");
            return new List<Address>();
        }

        public bool IsValidAddress(Address addr)
        {
            return program.Memory.IsValidAddress(addr);
        }

        public IEnumerable<(Address, Instruction?)> GetBlockInstructions(Block block)
        {
            return block.Statements.Select(s => (s.Address, s.Instruction))!;
        }

        public int BlockInstructionCount(Block block)
        {
            return block.Statements.Count;
        }
    }
}
