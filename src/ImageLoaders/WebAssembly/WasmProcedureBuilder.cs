#region License
/* Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Reko.ImageLoaders.WebAssembly
{
    public class WasmProcedureBuilder
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(WasmProcedureBuilder), nameof(WasmProcedureBuilder))
        {
            Level = TraceLevel.Warning
        };

        private readonly FunctionDefinition func;
        private readonly WasmArchitecture arch;
        private readonly WasmFile wasmFile;
        private readonly Dictionary<int, ProcedureBase> mpFunidxToProc;
        private readonly Dictionary<int, Address> mpGlobidxToAddr;
        private readonly Procedure proc;
        private readonly List<DataType> valueTypeStack;
        private readonly Dictionary<DataType, Dictionary<int, Identifier>> stackIds;
        private readonly List<ControlEntry> controlStack;
        private readonly List<Identifier> locals;
        private readonly ExpressionEmitter m;
        private Block? block;
        private WasmInstruction instr;

        public WasmProcedureBuilder(
            FunctionDefinition func, 
            WasmArchitecture arch, 
            WasmFile wasmFile,
            Dictionary<int, ProcedureBase> mpFunidxToProc,
            Dictionary<int, Address> mpGlobidxToAddr)
        {
            this.func = func;
            this.arch = arch;
            this.wasmFile = wasmFile;
            this.mpFunidxToProc = mpFunidxToProc;
            this.mpGlobidxToAddr = mpGlobidxToAddr;
            this.proc = (Procedure) mpFunidxToProc[func.FunctionIndex];
            this.proc.Signature = wasmFile.TypeSection!.Types[(int)func.TypeIndex];
            this.valueTypeStack = new List<DataType>();
            this.stackIds = new Dictionary<DataType, Dictionary<int, Identifier>>();
            this.controlStack = new List<ControlEntry>();
            this.locals = new List<Identifier>();
            this.m = new ExpressionEmitter();
            this.block = MakeBlock(proc.EntryAddress);
            proc.ControlGraph.AddEdge(proc.EntryBlock, block);
            this.instr = default!;
        }

        public Procedure GenerateProcedure()
        {
            GenerateLocals();
            GenerateInstructions();
            return proc;
        }

        private void GenerateLocals()
        {
            int cParams = 0;

            Identifier Loc(LocalVariable lv, int i)
            {
                return proc.Frame.CreateTemporary($"loc{i + cParams}", lv.DataType);
            }

            var parameters = proc.Signature.Parameters;
            if (parameters is not null)
            {
                locals.AddRange(parameters);
                cParams = parameters.Length;
            }
            locals.AddRange(func.Locals.Select(Loc));
        }

        private void GenerateInstructions()
        {
            var mem = new ByteMemoryArea(proc.EntryAddress - func.Start, func.ByteCode);
            var rdr = new WasmImageReader(mem);
            rdr.Offset = func.Start;
            var dasm = new WasmDisassembler(arch, rdr).GetEnumerator();
            trace.Inform("== Build WASM procedure {0}", proc.Name);
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                Identifier id;
                Identifier local;
                trace.Verbose("    {0,3} {1}{2}",
                    this.valueTypeStack.Count,
                    new string(' ', this.controlStack.Count),
                    instr);
                switch (instr.Mnemonic)
                {
                case Mnemonic.block: RewriteBlock(); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.br_if: RewriteBrIf(); break;
                case Mnemonic.br_table: RewriteBrTable(); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.call_indirect: RewriteIndirectCall(); break;
                case Mnemonic.@else: RewriteElse(); break;
                case Mnemonic.end: 
                    if (RewriteEnd(rdr))
                        return;
                    break;
                case Mnemonic.@if: RewriteIf(); break;
                case Mnemonic.loop: RewriteLoop(); break;
                case Mnemonic.nop: break;
                case Mnemonic.@return: RewriteReturn(); break;

                case Mnemonic.drop: RewriteDrop(); break;
                case Mnemonic.f32_add: RewriteBinary(Operator.FAdd, PrimitiveType.Real32); break;
                case Mnemonic.f32_abs: RewriteUnary(FpOps.FAbs32, PrimitiveType.Real32); break;
                case Mnemonic.f32_convert_s_i32: RewriteConversion(PrimitiveType.Int32, PrimitiveType.Real32); break;
                case Mnemonic.f32_convert_u_i32: RewriteConversion(PrimitiveType.UInt32, PrimitiveType.Real32); break;
                case Mnemonic.f32_demote_f64: RewriteConversion(PrimitiveType.Real64, PrimitiveType.Real32); break;
                case Mnemonic.f32_reinterpret_i32: RewriteIntrinsic(CommonOps.ReinterpretCast.MakeInstance(PrimitiveType.Real32)); break;
                case Mnemonic.f32_div: RewriteBinary(Operator.FDiv, PrimitiveType.Real32); break;
                case Mnemonic.f32_eq: RewriteCmp(m.FEq); break;
                case Mnemonic.f32_ne: RewriteCmp(m.FNe); break;
                case Mnemonic.f32_ge: RewriteCmp(m.FGe); break;
                case Mnemonic.f32_gt: RewriteCmp(m.FGt); break;
                case Mnemonic.f32_le: RewriteCmp(m.FLe); break;
                case Mnemonic.f32_lt: RewriteCmp(m.FLt); break;
                case Mnemonic.f32_load: RewriteLoad(PrimitiveType.Real32); break;
                case Mnemonic.f32_mul: RewriteBinary(Operator.FMul, PrimitiveType.Real32); break;
                case Mnemonic.f32_neg: RewriteUnary(Operator.FNeg, PrimitiveType.Real32); break;
                case Mnemonic.f32_sqrt: RewriteIntrinsic(FpOps.sqrtf); break;
                case Mnemonic.f32_store: RewriteStore(PrimitiveType.Real32); break;
                case Mnemonic.f32_sub: RewriteBinary(Operator.FSub, PrimitiveType.Real32); break;
                case Mnemonic.f64_abs: RewriteUnary(FpOps.FAbs64, PrimitiveType.Real64); break;
                case Mnemonic.f64_add: RewriteBinary(Operator.FAdd, PrimitiveType.Real64); break;
                case Mnemonic.f64_convert_s_i32: RewriteConversion(PrimitiveType.Int32, PrimitiveType.Real64); break;
                case Mnemonic.f64_convert_s_i64: RewriteConversion(PrimitiveType.Int64, PrimitiveType.Real64); break;
                case Mnemonic.f64_convert_u_i32: RewriteConversion(PrimitiveType.UInt32, PrimitiveType.Real64); break;
                case Mnemonic.f64_convert_u_i64: RewriteConversion(PrimitiveType.UInt64, PrimitiveType.Real64); break;
                case Mnemonic.f64_div: RewriteBinary(Operator.FDiv, PrimitiveType.Real64); break;
                case Mnemonic.f64_eq: RewriteCmp(m.FEq); break;
                case Mnemonic.f64_ne: RewriteCmp(m.FNe); break;
                case Mnemonic.f64_ge: RewriteCmp(m.FGe); break;
                case Mnemonic.f64_gt: RewriteCmp(m.FGt); break;
                case Mnemonic.f64_le: RewriteCmp(m.FLe); break;
                case Mnemonic.f64_lt: RewriteCmp(m.FLt); break;
                case Mnemonic.f64_load: RewriteLoad(PrimitiveType.Real64); break;
                case Mnemonic.f64_mul: RewriteBinary(Operator.FMul, PrimitiveType.Real64); break;
                case Mnemonic.f64_neg: RewriteUnary(Operator.FNeg, PrimitiveType.Real64); break;
                case Mnemonic.f64_promote_f32: RewriteConversion(PrimitiveType.Real32, PrimitiveType.Real64); break;
                case Mnemonic.f64_reinterpret_i64: RewriteIntrinsic(CommonOps.ReinterpretCast.MakeInstance(PrimitiveType.Real64)); break;
                case Mnemonic.f64_sqrt: RewriteIntrinsic(FpOps.sqrt); break;
                case Mnemonic.f64_store: RewriteStore(PrimitiveType.Real64); break;
                case Mnemonic.f64_sub: RewriteBinary(Operator.FSub, PrimitiveType.Real64); break;
                case Mnemonic.get_global:
                    var idxGlob = OpAsInt(0);
                    var global = wasmFile.GlobalIndex[idxGlob];
                    id = PushValue(global.Type.Item1);
                    Assign(id, m.Mem(id.DataType, this.mpGlobidxToAddr[idxGlob]));
                    break;
                case Mnemonic.get_local:
                    local = this.locals[OpAsInt(0)];
                    id = PushValue(local.DataType);
                    Assign(id, local);
                    break;
                case Mnemonic.i32_add: RewriteBinary(Operator.IAdd, PrimitiveType.Word32); break;
                case Mnemonic.i32_and: RewriteBinary(Operator.And, PrimitiveType.Word32); break;
                case Mnemonic.i32_div_s: RewriteBinary(Operator.SDiv, PrimitiveType.Int32); break;
                case Mnemonic.i32_div_u: RewriteBinary(Operator.UDiv, PrimitiveType.UInt32); break;
                case Mnemonic.i32_eq: RewriteCmp(m.Eq); break;
                case Mnemonic.i32_eqz: RewriteCmp0(m.Eq, PrimitiveType.Word32); break;
                case Mnemonic.i32_ge_s: RewriteCmp(m.Ge); break;
                case Mnemonic.i32_ge_u: RewriteCmp(m.Uge); break;
                case Mnemonic.i32_gt_s: RewriteCmp(m.Gt); break;
                case Mnemonic.i32_gt_u: RewriteCmp(m.Ugt); break;
                case Mnemonic.i32_le_s: RewriteCmp(m.Le); break;
                case Mnemonic.i32_le_u: RewriteCmp(m.Ule); break;
                case Mnemonic.i32_lt_s: RewriteCmp(m.Lt); break;
                case Mnemonic.i32_lt_u: RewriteCmp(m.Ult); break;
                case Mnemonic.i32_ne: RewriteCmp(m.Ne); break;
                case Mnemonic.i32_load: RewriteLoad(PrimitiveType.Word32); break;
                case Mnemonic.i32_load8_s: RewriteLoadWiden(PrimitiveType.SByte, PrimitiveType.Word32); break;
                case Mnemonic.i32_load8_u: RewriteLoadWiden(PrimitiveType.Byte, PrimitiveType.Word32); break;
                case Mnemonic.i32_load16_s: RewriteLoadWiden(PrimitiveType.Int16, PrimitiveType.Word32); break;
                case Mnemonic.i32_load16_u: RewriteLoadWiden(PrimitiveType.UInt16, PrimitiveType.Word32); break;
                case Mnemonic.i32_mul: RewriteBinary(Operator.IMul, PrimitiveType.Word32); break;
                case Mnemonic.i32_sub: RewriteBinary(Operator.ISub, PrimitiveType.Word32); break;
                case Mnemonic.i32_or: RewriteBinary(Operator.Or, PrimitiveType.Word32); break;
                case Mnemonic.i32_rem_s: RewriteBinary(Operator.SMod, PrimitiveType.Word32); break;
                case Mnemonic.i32_rem_u: RewriteBinary(Operator.UMod, PrimitiveType.Word32); break;
                case Mnemonic.i32_shl: RewriteBinary(Operator.Shl, PrimitiveType.Word32); break;
                case Mnemonic.i32_shr_s: RewriteBinary(Operator.Sar, PrimitiveType.Word32); break;
                case Mnemonic.i32_shr_u: RewriteBinary(Operator.Shr, PrimitiveType.Word32); break;
                case Mnemonic.i32_store: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.i32_store8: RewriteStoreNarrow(PrimitiveType.Byte); break;
                case Mnemonic.i32_store16: RewriteStoreNarrow(PrimitiveType.Word16); break;
                case Mnemonic.i32_wrap_i64: RewriteWrap(PrimitiveType.Word64, PrimitiveType.Word32); break;
                case Mnemonic.i32_xor: RewriteBinary(Operator.Xor, PrimitiveType.Word32); break;
                case Mnemonic.i32_const:
                case Mnemonic.i64_const:
                case Mnemonic.f32_const:
                case Mnemonic.f64_const: RewriteConst(); break;
                case Mnemonic.i64_add: RewriteBinary(Operator.IAdd, PrimitiveType.Word64); break;
                case Mnemonic.i64_and: RewriteBinary(Operator.And, PrimitiveType.Word64); break;
                case Mnemonic.i64_div_s: RewriteBinary(Operator.SDiv, PrimitiveType.Int64); break;
                case Mnemonic.i64_div_u: RewriteBinary(Operator.UDiv, PrimitiveType.UInt64); break;
                case Mnemonic.i64_eq: RewriteCmp(m.Eq); break;
                case Mnemonic.i64_eqz: RewriteCmp0(m.Eq, PrimitiveType.Word64); break;
                case Mnemonic.i64_ge_s: RewriteCmp(m.Ge); break;
                case Mnemonic.i64_ge_u: RewriteCmp(m.Uge); break;
                case Mnemonic.i64_gt_s: RewriteCmp(m.Gt); break;
                case Mnemonic.i64_gt_u: RewriteCmp(m.Ugt); break;
                case Mnemonic.i64_le_s: RewriteCmp(m.Le); break;
                case Mnemonic.i64_le_u: RewriteCmp(m.Ule); break;
                case Mnemonic.i64_lt_s: RewriteCmp(m.Lt); break;
                case Mnemonic.i64_lt_u: RewriteCmp(m.Ult); break;
                case Mnemonic.i64_ne: RewriteCmp(m.Ne); break;
                case Mnemonic.i64_extend_s_i32: RewriteConversion(PrimitiveType.Word32, PrimitiveType.Int64);break;
                case Mnemonic.i64_extend_u_i32: RewriteConversion(PrimitiveType.Word32, PrimitiveType.UInt64);break;
                case Mnemonic.i64_load: RewriteLoad(PrimitiveType.Word64); break;
                case Mnemonic.i64_load8_s: RewriteLoadWiden(PrimitiveType.SByte, PrimitiveType.Word64); break;
                case Mnemonic.i64_load8_u: RewriteLoadWiden(PrimitiveType.Byte, PrimitiveType.Word64); break;
                case Mnemonic.i64_load16_s: RewriteLoadWiden(PrimitiveType.Int16, PrimitiveType.Word64); break;
                case Mnemonic.i64_load16_u: RewriteLoadWiden(PrimitiveType.UInt16, PrimitiveType.Word64); break;
                case Mnemonic.i64_load32_s: RewriteLoadWiden(PrimitiveType.Int32, PrimitiveType.Word64); break;
                case Mnemonic.i64_load32_u: RewriteLoadWiden(PrimitiveType.Word32, PrimitiveType.Word64); break;
                case Mnemonic.i64_mul: RewriteBinary(Operator.IMul, PrimitiveType.Word64); break;
                case Mnemonic.i64_sub: RewriteBinary(Operator.ISub, PrimitiveType.Word64); break;
                case Mnemonic.i64_or: RewriteBinary(Operator.Or, PrimitiveType.Word64); break;
                case Mnemonic.i64_reinterpret_f64: RewriteIntrinsic(CommonOps.ReinterpretCast.MakeInstance(PrimitiveType.UInt64)); break;
                case Mnemonic.i64_rem_s: RewriteBinary(Operator.SMod, PrimitiveType.Word64); break;
                case Mnemonic.i64_rem_u: RewriteBinary(Operator.UMod, PrimitiveType.Word64); break;
                case Mnemonic.i64_shl: RewriteBinary(Operator.Shl, PrimitiveType.Word64); break;
                case Mnemonic.i64_shr_s: RewriteBinary(Operator.Sar, PrimitiveType.Word64); break;
                case Mnemonic.i64_shr_u: RewriteBinary(Operator.Shr, PrimitiveType.Word64); break;
                case Mnemonic.i64_store: RewriteStore(PrimitiveType.Word64); break;
                case Mnemonic.i64_store8: RewriteStoreNarrow(PrimitiveType.Byte); break;
                case Mnemonic.i64_store16: RewriteStoreNarrow(PrimitiveType.Word16); break;
                case Mnemonic.i64_store32: RewriteStoreNarrow(PrimitiveType.Word32); break;
                case Mnemonic.i64_xor: RewriteBinary(Operator.Xor, PrimitiveType.Word64); break;

                case Mnemonic.select: RewriteSelect(); break;
                case Mnemonic.set_global:
                    idxGlob = OpAsInt(0);
                    global = wasmFile.GlobalIndex[idxGlob];
                    id = PopValue();
                    Store(m.Mem(id.DataType, this.mpGlobidxToAddr[idxGlob]), id);
                    break;
                case Mnemonic.set_local:
                    local = this.locals[OpAsInt(0)];
                    id = PopValue();
                    Assign(local, id);
                    break;
                case Mnemonic.tee_local:
                    local = this.locals[OpAsInt(0)];
                    id = PeekValue();
                    Assign(local, id);
                    break;
                default:
                    EmitUnitTest(dasm, rdr);
                    Console.WriteLine($"Unhandled Wasm mnemonic {instr.Mnemonic}.");
                    return;
                }
            }
        }

        private void RewriteBinary(BinaryOperator fn, DataType dt)
        {
            var arg2 = PopValue();
            var arg1 = PopValue();
            var e = m.Bin(fn, arg1, arg2);
            var id = PushValue(dt);
            Assign(id, e);
        }

        private void RewriteBlock()
        {
            Debug.Assert(this.block is not null);
            DataType? output = DecodeBlockDataType();
            if (block.Statements.Count != 0)
            {
                var blockOld = block;
                block = MakeBlock(instr.Address);
                proc.ControlGraph.AddEdge(blockOld, block);
            }
            PushControl(Mnemonic.block, Array.Empty<Identifier>(), output, false);
        }

        private void RewriteBr()
        {
            if (this.block is null)
            {
                // We see this in some WASM binaries; sequences of consecutive
                // br statements. But all of the br's after the first one must
                // be unreachable, so why are they even there?
                Console.WriteLine($"*** fn index {this.func.FunctionIndex} {instr.Address} {instr}: unreachable break");
                return;
            }
            var cLevelsUp = OpAsInt(0);
            EmitBranchReference(this.block, cLevelsUp, null);
            // The current block is undefined until the next 'end' instruction.
            this.block = null;
        }

        private void RewriteBrTable()
        {
            Debug.Assert(this.block is not null);
            var brTable = (BranchTableOperand) instr.Operands[0];
            var swTargets = new Block[brTable.Targets.Length];
            for (uint i = 0; i <brTable.Targets.Length; ++i)
            {
                var cLevelsUp = (int) brTable.Targets[i];
                var blockTarget = EmitBranchReference(this.block, cLevelsUp, m.UInt32(i));
                swTargets[i] = blockTarget!;
            }
            var value = PopValue();
            block.Statements.Add(instr.Address, new SwitchInstruction(value, swTargets));
            // The current block is undefined until the next 'end' instruction.
            this.block = null;
        }


        private Block? EmitBranchReference(Block block, int cLevelsUp, Expression? placeHolder)
        {
            int iLabel = this.controlStack.Count - cLevelsUp - 1;
            if (iLabel < 0)
                throw new BadImageFormatException("Control stack unbalanced at br instruction.");
            var ctrl = this.controlStack[iLabel];
            if (ctrl.Mnemonic == Mnemonic.loop)
            {
                // branches to loop continuation can always be done.
                proc.ControlGraph.AddEdge(block, ctrl.Block);
                return ctrl.Block;
            }
            else
            {
                ctrl.IncompleteBranches.Add((block, placeHolder));
                return null;
            }
        }

        private void RewriteBrIf()
        {
            Debug.Assert(this.block is not null);
            var cLevelsUp = OpAsInt(0);
            int iLabel = this.controlStack.Count - cLevelsUp - 1;
            if (iLabel < 0)
                throw new BadImageFormatException("Control stack unbalanced at br instruction.");
            var ctrl = this.controlStack[iLabel];

            var followBlock = MakeFollowBlock();
            var predicate = PopValue();
            if (ctrl.Mnemonic == Mnemonic.loop)
            {
                // Branches to loops are "continue"s.
                block.Statements.Add(instr.Address, new Branch(predicate, ctrl.Block));
                proc.ControlGraph.AddEdge(this.block, followBlock);
                proc.ControlGraph.AddEdge(this.block, ctrl.Block);
            }
            else
            {
                // Emit a placeholder instruction
                Assign(predicate, predicate);
                proc.ControlGraph.AddEdge(this.block, followBlock);
                ctrl.IncompleteBranches.Add((this.block, predicate));
            }
            this.block = followBlock;
        }

        private void RewriteLoop()
        {
            if (instr.Operands.Length > 0)
            {
                //$TODO: if loop has args
                Console.WriteLine("WASM loop instruction with operands not handled yet.");
            }
            var followBlock = MakeFollowBlock();
            Debug.Assert(this.block is not null);
            proc.ControlGraph.AddEdge(this.block, followBlock);
            this.block = followBlock;
            PushControl(Mnemonic.loop, Array.Empty<Identifier>(), null, false);
        }

        private void RewriteReturn()
        {
            //$TODO: what if the block we're leaving has variables on stack.
            Debug.Assert(this.block is not null);
            Return();
            this.block = null;
        }

        private void RewriteElse()
        {
            if (controlStack.Count == 0)
                throw new BadImageFormatException("Control stack unbalanced at else instruction.");
            var ctrl = PopControl();
            if (ctrl.Mnemonic != Mnemonic.@if)
                throw new BadImageFormatException("'else' was not preceded by 'if'.");
            this.valueTypeStack.Clear();
            this.valueTypeStack.AddRange(ctrl.TypeStack);
            var followBlock = MakeFollowBlock();
            BackpatchIf(ctrl.Block, followBlock);
            if (this.block is not null)
            {
                ctrl = PushControl(Mnemonic.@else, ctrl.Inputs, ctrl.Output, false);
                ctrl.IncompleteBranches.Add((this.block, null));
                this.block = followBlock;
            }
            else
            {
                this.block = followBlock;
                ctrl = PushControl(Mnemonic.@else, ctrl.Inputs, ctrl.Output, false);
            }
        }

        private bool RewriteEnd(WasmImageReader rdr)
        {
            if (rdr.Offset == this.func.End)
            {
                if (controlStack.Count != 0)
                    throw new BadImageFormatException("Control stack unbalanced at end of function.");
                if (block is not null)
                {
                    Return();
                }
                return true;
            }
            if (controlStack.Count == 0)
                throw new BadImageFormatException("Control stack unbalanced at end of function.");
            var ctrl = PopControl();
            if (ctrl.Output is not null)
                this.PushValue(ctrl.Output);
            switch (ctrl.Mnemonic)
            {
            case Mnemonic.@if:
                var followBlock = MakeFollowBlock();
                BackpatchIf(ctrl.Block, followBlock);
                if (this.instr.Mnemonic != Mnemonic.@else &&
                    this.block is not null)
                {
                    this.proc.ControlGraph.AddEdge(this.block, followBlock);
                }
                CompleteBranches(ctrl, followBlock);
                this.block = followBlock;
                break;
            case Mnemonic.@else:
                followBlock = MakeBlock(instr.Address);
                if (this.block is not null)
                    proc.ControlGraph.AddEdge(this.block, followBlock);
                CompleteBranches(ctrl, followBlock);
                this.block = followBlock;
                break;
            case Mnemonic.block:
                followBlock = MakeBlock(instr.Address);
                CompleteBranches(ctrl, followBlock);
                if (this.block is not null)
                    proc.ControlGraph.AddEdge(block, followBlock);
                this.block = followBlock;
                break;
            case Mnemonic.loop:
                // Assumes a `br` or `br_if` is the last instruction in the loop.
                CompleteBranches(ctrl, ctrl.Block);
                if (this.block is not null && this.block.Statements.Count > 0)
                {
                    followBlock = MakeFollowBlock();
                    proc.ControlGraph.AddEdge(this.block, followBlock);
                    this.block = followBlock;
                }
                break;
            default:
                throw new NotImplementedException($"Don't know how to end {ctrl.Mnemonic}.");
            }
            return false;
        }

        private void CompleteBranches(ControlEntry ctrl, Block followBlock)
        {
            foreach (var (block, predicate) in ctrl.IncompleteBranches)
            {
                if (predicate is not null)
                {
                    var lastStm = block.Statements[^1];
                    if (lastStm.Instruction is SwitchInstruction sw)
                    {
                        var c = (Constant) predicate;
                        sw.Targets[c.ToUInt32()] = followBlock;
                    }
                    else
                    {
                        lastStm.Instruction = new Branch(predicate, followBlock);
                    }
                }
                proc.ControlGraph.AddEdge(block, followBlock);
            }
        }

        private void BackpatchIf(Block ifBlock, Block followBlock)
        {
            Debug.Assert(ifBlock.Succ.Count == 1, "There should be one out edge created in ReriteIf");
            var placeholder = ((Assignment) ifBlock.Statements[^1].Instruction).Dst;
            ifBlock.Statements[^1].Instruction = new Branch(m.Not(placeholder), followBlock);

            // Add the second edge, the 'if' part already added the first.
            proc.ControlGraph.AddEdge(ifBlock, followBlock);
        }

        private void RewriteCall()
        {
            var idxFunc = OpAsInt(0);
            var callee = this.wasmFile.FunctionIndex[idxFunc];
            var procCallee = mpFunidxToProc[idxFunc];
            var sig = this.wasmFile.TypeSection!.Types[(int)callee.TypeIndex];
            var args = PopValues(sig.Parameters!.Length);
            var pc = new ProcedureConstant(PrimitiveType.Ptr32, procCallee);
            var application = m.Fn(pc, args);
            if (sig.HasVoidReturn)
            {
                SideEffect(application);
            }
            else
            {
                var id = PushValue(sig.Outputs[0].DataType);
                Assign(id, application);
            }
        }

        private void RewriteIndirectCall()
        {
            //$TODO: this is not correct yet.
            var idxTable = OpAsInt(0);
            var idxFunc = OpAsInt(1);
        }

        private void RewriteConst()
        {
            var imm = (Constant) instr.Operands[0];
            var id = PushValue(imm.DataType);
            Assign(id, imm);
        }

        private void RewriteCmp(Func<Expression, Expression, Expression> fn)
        {
            var arg2 = PopValue();
            var arg1 = PopValue();
            var e = fn(arg1, arg2);
            var id = PushValue(PrimitiveType.Bool);
            Assign(id, e);
        }

        private void RewriteCmp0(Func<Expression, Expression, Expression> fn, DataType dt)
        {
            var arg1 = PopValue();
            var arg2 = Constant.Zero(dt);
            var e = fn(arg1, arg2);
            var id = PushValue(PrimitiveType.Bool);
            Assign(id, e);
        }

        private void RewriteDrop()
        {
            PopValue();
        }

        private void RewriteConversion(DataType dtFrom, DataType dtTo)
        {
            var arg1 = PopValue();
            var e = m.Convert(arg1, dtFrom, dtTo);
            var id = PushValue(dtTo);
            Assign(id, e);
        }

        private void RewriteIf()
        {
            DataType? output = DecodeBlockDataType();
            Debug.Assert(this.block is not null);
            var predicate = PopValue();
            // Can't create the branch instruction yet; the
            // following 'else' or 'end' instruction is responsible for this.
            // We leave a dummy instruction at the end of the block, and
            // let the 'else' or 'end' change it to a 'Branch'.
            Assign(predicate, predicate);
            PushControl(Mnemonic.@if, Array.Empty<Identifier>(), output, false);

            var thenBlock = this.MakeFollowBlock();
            // We only create the 'then' edge. The 'end' or 'else' block is 
            // responsible for inserting the 'else' edge.
            proc.ControlGraph.AddEdge(block, thenBlock);
            this.block = thenBlock;
        }

        private void RewriteIntrinsic(IntrinsicProcedure intrinsic)
        {
            var parameters = intrinsic.Signature.Parameters;
            Debug.Assert(parameters is not null);
            var args = PopValues(parameters.Length);
            var ret = PushValue(intrinsic.ReturnType);
            Assign(ret, m.Fn(intrinsic, args));
        }

        private void RewriteLoad(PrimitiveType dt)
        {
            MemoryAccess mm = MakeMemoryAccess(dt);
            var val = PushValue(dt);
            Assign(val, mm);
        }

        private void RewriteLoadWiden(PrimitiveType dtFrom, PrimitiveType dtWiden)
        {
            var tmp = proc.Frame.CreateTemporary(dtFrom);
            MemoryAccess mm = MakeMemoryAccess(dtFrom);
            Assign(tmp, mm);
            var val = PushValue(dtWiden);
            Assign(val, m.Convert(tmp, dtFrom, dtWiden));
        }

        private void RewriteSelect()
        {
            var predicate = PopValue();
            var val2 = PopValue();
            var val1 = PopValue();
            var result = PushValue(val1.DataType);
            Assign(result, m.Conditional(val1.DataType, predicate, val1, val2));
        }

        private void RewriteStore(PrimitiveType dt)
        {
            Expression val = PopValue();
            var mem = MakeMemoryAccess(dt);
            Store(mem, val);
        }

        private void RewriteStoreNarrow(PrimitiveType dtNarrow)
        {
            Expression val = PopValue();
            var narrowed = proc.Frame.CreateTemporary(dtNarrow);
            Assign(narrowed, m.Slice(val, dtNarrow));
            var mem = MakeMemoryAccess(dtNarrow);
            Store(mem, narrowed);
        }

        private void RewriteUnary(UnaryOperator fn, DataType dt)
        {
            var arg = PopValue();
            var e = m.Unary(fn, arg);
            var id = PushValue(dt);
            Assign(id, e);
        }

        private void RewriteUnary(IntrinsicProcedure fn, DataType dt)
        {
            var arg = PopValue();
            var e = m.Fn(fn, arg);
            var id = PushValue(dt);
            Assign(id, e);
        }

        private void RewriteWrap(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var arg = PopValue();
            var e = m.Slice(arg, dtFrom);
            var id = PushValue(dtTo);
            Assign(id, e);
        }

        private void Return()
        {
            Debug.Assert(wasmFile.TypeSection is not null);
            var fnType = wasmFile.TypeSection.Types[(int) func.TypeIndex];
            ReturnInstruction ret;
            if (fnType.HasVoidReturn)
            {
                ret = new ReturnInstruction();
            }
            else
            {
                var value = PopValue();
                ret = new ReturnInstruction(value);
            }
            Debug.Assert(this.block is not null);
            block.Statements.Add(instr.Address, ret);
            proc.ControlGraph.AddEdge(block, proc.ExitBlock);
        }

        /// <summary>
        /// Interpret the <paramref name="iOp"/>'th operand as a 
        /// signed integer.
        /// </summary>
        /// <param name="iOp">Index of the current instruction's 
        /// operands.</param>
        /// <returns>The operand value as a signed integer.
        /// </returns>
        private int OpAsInt(int iOp)
        {
            return ((Constant) instr.Operands[iOp]).ToInt32();
        }

        private void Assign(Identifier id, Expression value)
        {
            Debug.Assert(this.block is not null);
            var ass = new Assignment(id, value);
            block.Statements.Add(instr.Address, ass);
        }

        private void SideEffect(Expression e)
        {
            Debug.Assert(this.block is not null);
            var s = new SideEffect(e);
            block.Statements.Add(instr.Address, s);
        }

        private void Store(Expression dst, Expression value)
        {
            Debug.Assert(this.block is not null);
            var store = new Store(dst, value);
            block.Statements.Add(instr.Address, store);
        }

        private DataType? DecodeBlockDataType()
        {
            if (instr.Operands.Length == 0)
                return null;
            var v = (Constant)instr.Operands[0];
            if (v.DataType.Size == 1)
            {
                switch (v.ToInt32())
                {
                case 127: return PrimitiveType.Word32;
                case 126: return PrimitiveType.Word64;
                case 125: return PrimitiveType.Real32;
                case 124: return PrimitiveType.Real64;
                default: throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private Block MakeBlock(Address addr)
        {
            return proc.AddBlock(addr, NamingPolicy.Instance.BlockName(addr));
        }

        private Block MakeFollowBlock()
        {
            var addrFollow = instr.Address + instr.Length;
            return MakeBlock(addrFollow);
        }

        private MemoryAccess MakeMemoryAccess(PrimitiveType dt)
        {
            Expression ea = PopValue();
            var mem = (MemoryOperand) instr.Operands[0];
            if (mem.Offset != 0)
            {
                ea = m.IAdd(ea, m.UInt32(mem.Offset));
            }
            return m.Mem(dt, ea);
        }

        private ControlEntry PopControl()
        {
            var iTop = this.controlStack.Count - 1;
            var ctrl = this.controlStack[iTop];
            this.controlStack.RemoveAt(iTop);
            this.valueTypeStack.Clear();
            this.valueTypeStack.AddRange(ctrl.TypeStack);
            return ctrl;
        }

        private ControlEntry PushControl(Mnemonic mnemonic, Identifier[] inputs, DataType? output, bool isUnreachable)
        {
            Debug.Assert(this.block is not null);
            var e = new ControlEntry(mnemonic, block, inputs, output, valueTypeStack, isUnreachable);
            this.controlStack.Add(e);
            return e;
        }

        private Identifier PeekValue()
        {
            int iTop = this.valueTypeStack.Count - 1;
            if (iTop < 0)
                throw new AddressCorrelatedException(instr.Address, "Exhausted value stack.");
            return this.stackIds[valueTypeStack[iTop]][iTop];
        }

        private Identifier PushValue(DataType dt)
        {
            int iTop = this.valueTypeStack.Count;
            this.valueTypeStack.Add(dt);
            if (!this.stackIds.TryGetValue(dt, out var ids))
            {
                ids = new Dictionary<int, Identifier>();
                this.stackIds.Add(dt, ids);
            }
            if (!ids.TryGetValue(iTop, out var id))
            {
                id = proc.Frame.CreateTemporary(dt);
                ids.Add(iTop, id);
            }
            return id;
        }

        private Identifier PopValue()
        {
            int iTop = this.valueTypeStack.Count - 1;
            if (iTop < 0)
                throw new AddressCorrelatedException(instr.Address, $"Exhausted value stack when rewriting procedure {proc.Name}.");
            var dt = this.valueTypeStack[iTop];
            var id = this.stackIds[dt][iTop];
            this.valueTypeStack.RemoveAt(iTop);
            return id;
        }

        private Expression[] PopValues(int cValues)
        {
            int iTop = this.valueTypeStack.Count - cValues;
            if (iTop < 0)
                throw new AddressCorrelatedException(instr.Address, $"Exhausted value stack when rewriting procedure {proc.Name}.");
            var exps = new Expression[cValues];
            for (int i = 0; i < cValues; i++)
            {
                var dt = this.valueTypeStack[iTop + i];
                exps[i] = this.stackIds[dt][iTop + i];
            }
            this.valueTypeStack.RemoveRange(iTop, cValues);
            return exps;
        }

        private void EmitUnitTest(IEnumerator<WasmInstruction> dasm, EndianImageReader rdr)
        {
             arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("WasmRw", dasm.Current, dasm.Current.Mnemonic.ToString(), rdr, "");
        }

        private class ControlEntry
        {
            public ControlEntry(
                Mnemonic mnemonic,
                Block block, 
                Identifier[] inputs,
                DataType? output,
                List<DataType> valueStack, 
                bool isUnreachable)
            {
                this.Mnemonic = mnemonic;
                this.Block = block;
                this.Inputs = inputs;
                this.Output = output;
                this.IsUnreachable = isUnreachable;
                this.IncompleteBranches = new();
                this.TypeStack = new List<DataType>(valueStack);
            }

            public Mnemonic Mnemonic { get; }
            public Block Block { get; }
            public Identifier[] Inputs { get; }
            public DataType? Output { get; }
            public int BlockDepth { get; }
            public bool IsUnreachable { get; }
            public List<(Block, Expression? predicate)> IncompleteBranches { get; }
            public List<DataType> TypeStack { get; }
        }

    }
}
