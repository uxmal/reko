#region License
/* Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Memory;
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
        private readonly FunctionDefinition func;
        private readonly WasmArchitecture arch;
        private readonly WasmFile wasmFile;
        private readonly Dictionary<int, ProcedureBase> mpFunidxToProc;
        private readonly Dictionary<int, Address> mpGlobidxToAddr;
        private readonly Procedure proc;
        private readonly List<Identifier> valueStack;
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
            this.valueStack = new List<Identifier>();
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
            GenerateControlFlow();
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


        private void GenerateControlFlow()
        {
        }

        private void GenerateInstructions()
        {
            var mem = new ByteMemoryArea(proc.EntryAddress - func.Start, func.ByteCode);
            var rdr = new WasmImageReader(mem);
            rdr.Offset = func.Start;
            var dasm = new WasmDisassembler(arch, rdr).GetEnumerator();
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                Identifier id;
                Identifier local;
                switch (instr.Mnemonic)
                {
                case Mnemonic.block: RewriteBlock(); break;
                case Mnemonic.br: RewriteBr(); break;
                case Mnemonic.br_if: RewriteBrIf(); break;
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.@else: RewriteElse(); break;
                case Mnemonic.end: 
                    if (RewriteEnd(rdr))
                        return;
                    break;
                case Mnemonic.loop: RewriteLoop(); break;
                case Mnemonic.f32_add: RewriteBinary(m.FAdd, PrimitiveType.Real32); break;
                case Mnemonic.f32_div: RewriteBinary(m.FDiv, PrimitiveType.Real32); break;
                case Mnemonic.f32_mul: RewriteBinary(m.FMul, PrimitiveType.Real32); break;
                case Mnemonic.f32_sub: RewriteBinary(m.FSub, PrimitiveType.Real32); break;
                case Mnemonic.f64_add: RewriteBinary(m.FAdd, PrimitiveType.Real64); break;
                case Mnemonic.f64_div: RewriteBinary(m.FDiv, PrimitiveType.Real64); break;
                case Mnemonic.f64_mul: RewriteBinary(m.FMul, PrimitiveType.Real64); break;
                case Mnemonic.f64_sub: RewriteBinary(m.FSub, PrimitiveType.Real64); break;
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
                case Mnemonic.i32_add: RewriteBinary(m.IAdd, PrimitiveType.Word32); break;
                case Mnemonic.i32_and: RewriteBinary(m.And, PrimitiveType.Word32); break;
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
                case Mnemonic.i32_mul: RewriteBinary(m.IMul, PrimitiveType.Word32); break;
                case Mnemonic.i32_sub: RewriteBinary(m.ISub, PrimitiveType.Word32); break;
                case Mnemonic.i32_or: RewriteBinary(m.Or, PrimitiveType.Word32); break;
                case Mnemonic.i32_rem_s: RewriteBinary(m.SMod, PrimitiveType.Word32); break;
                case Mnemonic.i32_rem_u: RewriteBinary(m.UMod, PrimitiveType.Word32); break;
                case Mnemonic.i32_shl: RewriteBinary(m.Shl, PrimitiveType.Word32); break;
                case Mnemonic.i32_shr_s: RewriteBinary(m.Sar, PrimitiveType.Word32); break;
                case Mnemonic.i32_shr_u: RewriteBinary(m.Shr, PrimitiveType.Word32); break;
                case Mnemonic.i32_xor: RewriteBinary(m.Xor, PrimitiveType.Word32); break;
                case Mnemonic.i32_const:
                case Mnemonic.i64_const:
                case Mnemonic.f32_const:
                case Mnemonic.f64_const: RewriteConst(); break;
                case Mnemonic.i64_add: RewriteBinary(m.IAdd, PrimitiveType.Word64); break;
                case Mnemonic.i64_and: RewriteBinary(m.And, PrimitiveType.Word64); break;
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
                case Mnemonic.i64_extend_u_i32: RewriteExtend(PrimitiveType.Word32, PrimitiveType.UInt64);break;
                case Mnemonic.i64_mul: RewriteBinary(m.IMul, PrimitiveType.Word64); break;
                case Mnemonic.i64_sub: RewriteBinary(m.ISub, PrimitiveType.Word64); break;
                case Mnemonic.i64_or: RewriteBinary(m.Or, PrimitiveType.Word64); break;
                case Mnemonic.i64_rem_s: RewriteBinary(m.SMod, PrimitiveType.Word64); break;
                case Mnemonic.i64_rem_u: RewriteBinary(m.UMod, PrimitiveType.Word64); break;
                case Mnemonic.i64_shl: RewriteBinary(m.Shl, PrimitiveType.Word64); break;
                case Mnemonic.i64_shr_s: RewriteBinary(m.Sar, PrimitiveType.Word64); break;
                case Mnemonic.i64_shr_u: RewriteBinary(m.Shr, PrimitiveType.Word64); break;
                case Mnemonic.i64_xor: RewriteBinary(m.Xor, PrimitiveType.Word64); break;
                case Mnemonic.@if: RewriteIf(); break;
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
                    Console.WriteLine($"Unhandled mnemonic {instr.Mnemonic}.");
                    return;
                }
            }
        }

        private Block MakeBlock(Address addr)
        {
            return proc.AddBlock(addr, NamingPolicy.Instance.BlockName(addr));
        }

        private void RewriteBinary(Func<Expression,Expression,Expression> fn, DataType dt)
        {
            var arg2 = PopValue();
            var arg1 = PopValue();
            var e = fn(arg1, arg2);
            var id = PushValue(dt);
            Assign(id, e);
        }

        private void RewriteBlock()
        {
            Debug.Assert(this.block is not null);
            if (block.Statements.Count != 0)
            {
                var blockOld = block;
                block = MakeBlock(instr.Address);
                proc.ControlGraph.AddEdge(blockOld, block);
            }
            PushControl(Mnemonic.block, Array.Empty<Identifier>(), null, false);
        }

        private void RewriteBr()
        {
            Debug.Assert(this.block is not null);
            var cLevelsUp = OpAsInt(0);
            int iLabel = this.controlStack.Count - cLevelsUp - 1;
            if (iLabel < 0)
                throw new BadImageFormatException("Control stack unbalanced at br instruction.");
            var ctrl = this.controlStack[iLabel];
            proc.ControlGraph.AddEdge(this.block, ctrl.Block);
            // The current block is undefined until the next 'end' instruction.
            this.block = null;
        }

        private void RewriteBrIf()
        {
            Debug.Assert(this.block is not null);
            var cLevelsUp = OpAsInt(0);
            int iLabel = this.controlStack.Count - cLevelsUp - 1;
            if (iLabel < 0)
                throw new BadImageFormatException("Control stack unbalanced at br instruction.");
            var ctrl = this.controlStack[iLabel];

            var followBlock = MakeBlock(instr.Address + instr.Length);
            var predicate = PopValue();
            block.Statements.Add(instr.Address, new Branch(predicate, ctrl.Block));
            proc.ControlGraph.AddEdge(this.block, followBlock);
            proc.ControlGraph.AddEdge(this.block, ctrl.Block);
            this.block = followBlock;
        }

        private void RewriteLoop()
        {
            //$TODO: if loop has args
            PushControl(Mnemonic.loop, Array.Empty<Identifier>(), null, false);
        }

        private void RewriteElse()
        {
            if (controlStack.Count == 0)
                throw new BadImageFormatException("Control stack unbalanced at else instruction.");
            var blockThen = block;
            var ctrl = PopControl();
            if (ctrl.Mnemonic != Mnemonic.@if)
                throw new BadImageFormatException("'else' was not preceded by 'if'.");
            BackpatchIf(ctrl);
            ctrl = PushControl(Mnemonic.@else, Array.Empty<Identifier>(), null, false);
            ctrl.ThenBlock = blockThen;
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
            switch (ctrl.Mnemonic)
            {
            case Mnemonic.@if:
                BackpatchIf(ctrl);
                break;
            case Mnemonic.@else:
                Debug.Assert(ctrl.ThenBlock is not null, "Should have been set by the 'else' handler");
                var followBlock = MakeBlock(instr.Address);
                if (this.block is not null)
                    proc.ControlGraph.AddEdge(this.block, followBlock);
                proc.ControlGraph.AddEdge(ctrl.ThenBlock, followBlock);
                this.block = followBlock;
                break;
            case Mnemonic.block:
                followBlock = MakeBlock(instr.Address);
                if (this.block is not null)
                    proc.ControlGraph.AddEdge(block, followBlock);
                this.block = followBlock;
                break;
            case Mnemonic.loop:
                // Assumes a `br` or `br_if` is the last instruction in the loop.
                break;
            default:
                throw new NotImplementedException($"Don't know how to end {ctrl.Mnemonic}.");
            }
            return false;
        }

        private void BackpatchIf(ControlEntry ctrl)
        {
            Debug.Assert(this.block is not null);
            var placeholder = ((Assignment) ctrl.Block.Statements[^1].Instruction).Dst;

            var followBlock = MakeBlock(instr.Address);
            ctrl.Block.Statements[^1].Instruction = new Branch(m.Not(placeholder), followBlock);
            ctrl.Block.Succ[1].Pred.RemoveAt(0);
            ctrl.Block.Succ[1] = followBlock;
            followBlock.Pred.Add(ctrl.Block);
            if (this.instr.Mnemonic != Mnemonic.@else)
            {
                block.Succ.Add(followBlock);
            }
            this.block = followBlock;
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
                var id = PushValue(sig.ReturnValue.DataType);
                Assign(id, application);
            }
        }

        private void RewriteConst()
        {
            var imm = (ImmediateOperand) instr.Operands[0];
            var id = PushValue(imm.Width);
            Assign(id, imm.Value);
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

        private void RewriteExtend(DataType dtFrom, DataType dtTo)
        {
            var arg1 = PopValue();
            var e = m.Convert(arg1, dtFrom, dtTo);
            var id = PushValue(PrimitiveType.Bool);
            Assign(id, e);
        }

        private void RewriteIf()
        {
            if (instr.Operands.Length > 0)
                Console.WriteLine($"Unhandled mnemonic if with argument.");

            Debug.Assert(this.block is not null);
            var predicate = PopValue();
            // Can't create the branch instruction yet; the
            // following 'else' or 'end' instruction is responsible for this.
            Assign(predicate, predicate);
            PushControl(Mnemonic.@if, Array.Empty<Identifier>(), null, false);

            var thenBlock = this.MakeBlock(instr.Address + instr.Length);
            proc.ControlGraph.AddEdge(block, thenBlock);        // Will be replaced when 'else' or 'end' is found.
            proc.ControlGraph.AddEdge(block, thenBlock);
            this.block = thenBlock;
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

        private int OpAsInt(int v)
        {
            return ((ImmediateOperand) instr.Operands[v]).Value.ToInt32();
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

        private ControlEntry PopControl()
        {
            var iTop = this.controlStack.Count - 1;
            var ctrl = this.controlStack[iTop];
            this.controlStack.RemoveAt(iTop);
            return ctrl;
        }

        private ControlEntry PushControl(Mnemonic mnemonic, Identifier[] inputs, Identifier? output, bool isUnreachable)
        {
            Debug.Assert(this.block is not null);
            var e = new ControlEntry(mnemonic, block, inputs, output, valueStack.Count, isUnreachable);
            this.controlStack.Add(e);
            return e;
        }

        private Identifier PeekValue()
        {
            int iTop = this.valueStack.Count - 1;
            if (iTop < 0)
                throw new AddressCorrelatedException(instr.Address, "Exhausted value stack.");
            return this.valueStack[iTop];
        }

        private Identifier PushValue(DataType dt)
        {
            var id  = proc.Frame.CreateTemporary(dt);
            this.valueStack.Add(id);
            return id;
        }

        private Identifier PopValue()
        {
            int iTop = this.valueStack.Count - 1;
            if (iTop < 0)
                throw new AddressCorrelatedException(instr.Address, $"Exhausted value stack when rewriting procedure {proc.Name}.");
            var id = this.valueStack[iTop];
            this.valueStack.RemoveAt(iTop);
            return id;
        }

        private Expression[] PopValues(int cValues)
        {
            int iTop = this.valueStack.Count - cValues;
            if (iTop < 0)
                throw new AddressCorrelatedException(instr.Address, $"Exhausted value stack when rewriting procedure {proc.Name}.");
            var exps = new Expression[cValues];
            for (int i = 0; i < cValues; i++)
            {
                exps[i] = this.valueStack[iTop + i];
            }
            this.valueStack.RemoveRange(iTop, cValues);
            return exps;
        }

        private void EmitUnitTest(IEnumerator<WasmInstruction> dasm, EndianImageReader rdr)
        {
             arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("WasmRw", dasm.Current, dasm.Current.Mnemonic.ToString(), rdr, "");
        }

        private class ControlEntry
        {
            public ControlEntry(Mnemonic mnemonic, Block block, Identifier[] inputs, Identifier? output, int blockDepth, bool isUnreachable)
            {
                this.Mnemonic = mnemonic;
                this.Block = block;
                this.Inputs = inputs;
                this.Output = output;
                this.BlockDepth = blockDepth;
                this.IsUnreachable = isUnreachable;
            }

            public Mnemonic Mnemonic { get; }
            public Block Block { get; }
            public Block? ThenBlock { get; set; } // Only present if an 'else' instruction is encountered.
            public Identifier[] Inputs { get; }
            public Identifier? Output { get; }
            public int BlockDepth { get; }
            public bool IsUnreachable { get; }
        }

    }
}
