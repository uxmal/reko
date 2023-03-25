using Microsoft.VisualBasic;
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
        private readonly Procedure proc;
        private readonly List<Identifier> valueStack;
        private readonly List<ControlEntry> controlStack;
        private readonly List<Identifier> locals;
        private readonly ExpressionEmitter m;
        private Block block;
        private WasmInstruction instr;

        public WasmProcedureBuilder(
            FunctionDefinition func, 
            WasmArchitecture arch, 
            WasmFile wasmFile,
            Dictionary<int, ProcedureBase> mpFunidxToProc)
        {
            this.func = func;
            this.arch = arch;
            this.wasmFile = wasmFile;
            this.mpFunidxToProc = mpFunidxToProc;
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
            Identifier Loc(LocalVariable lv, int i)
            {
                return proc.Frame.CreateTemporary($"loc{i}", lv.DataType);
            }

            var parameters = proc.Signature.Parameters;
            if (parameters is not null)
            {
                locals.AddRange(parameters);
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
                case Mnemonic.call: RewriteCall(); break;
                case Mnemonic.end: RewriteEnd(rdr); break;
                case Mnemonic.get_local:
                    local = this.locals[OpAsInt(0)];
                    id = PushValue(local.DataType);
                    Assign(id, local);
                    break;
                case Mnemonic.i32_add: RewriteBinary(m.IAdd, PrimitiveType.Word32); break;
                case Mnemonic.i32_and: RewriteBinary(m.And, PrimitiveType.Word32); break;
                case Mnemonic.i32_mul: RewriteBinary(m.IMul, PrimitiveType.Word32); break;
                case Mnemonic.i32_sub: RewriteBinary(m.ISub, PrimitiveType.Word32); break;
                case Mnemonic.i32_const:
                case Mnemonic.f32_const: RewriteConst(); break;
                case Mnemonic.i64_eq: RewriteCmp(m.Eq); break;
                case Mnemonic.i64_extend_u_i32: RewriteExtend(PrimitiveType.Word32, PrimitiveType.UInt64);break;
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
            if (block.Statements.Count != 0)
            {
                var blockOld = block;
                block = MakeBlock(instr.Address);
                proc.ControlGraph.AddEdge(blockOld, block);
            }
            PushControl(Mnemonic.block, Array.Empty<Identifier>(), null, false);
        }

        private void RewriteEnd(WasmImageReader rdr)
        {
            if (rdr.Offset == this.func.End)
            {
                if (controlStack.Count != 0)
                    throw new BadImageFormatException("Control stack unbalanced at end of function.");
                Return();
            }
            else
            {
                if (controlStack.Count == 0)
                    throw new BadImageFormatException("Control stack unbalanced at end of function.");
                var ctrl = PopControl();
            }
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

        private void RewriteExtend(DataType dtFrom, DataType dtTo)
        {
            var arg1 = PopValue();
            var e = m.Convert(arg1, dtFrom, dtTo);
            var id = PushValue(PrimitiveType.Bool);
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
            block.Statements.Add(instr.Address, ret);
        }

        private int OpAsInt(int v)
        {
            return ((ImmediateOperand) instr.Operands[v]).Value.ToInt32();
        }

        private void Assign(Identifier id, Expression value)
        {
            var ass = new Assignment(id, value);
            block.Statements.Add(instr.Address, ass);
        }

        private void SideEffect(Expression e)
        {
            var s = new SideEffect(e);
            block.Statements.Add(instr.Address, s);
        }

        private ControlEntry PopControl()
        {
            var iTop = this.controlStack.Count - 1;
            var ctrl = this.controlStack[iTop];
            this.controlStack.RemoveAt(iTop);
            return ctrl;
        }

        private void PushControl(Mnemonic mnemonic, Identifier[] inputs, Identifier? output, bool isUnreachable)
        {
            this.controlStack.Add(new ControlEntry(mnemonic, inputs, output, valueStack.Count, isUnreachable));
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
                throw new AddressCorrelatedException(instr.Address, "Exhausted value stack.");
            var id = this.valueStack[iTop];
            this.valueStack.RemoveAt(iTop);
            return id;
        }

        private Expression[] PopValues(int cValues)
        {
            int iTop = this.valueStack.Count - cValues;
            if (iTop < 0)
                throw new AddressCorrelatedException(instr.Address, "Exhausted value stack.");
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
            public ControlEntry(Mnemonic mnemonic, Identifier[] inputs, Identifier? output, int blockDepth, bool isUnreachable)
            {
                this.Mnemonic = mnemonic;
                this.Inputs = inputs;
                this.Output = output;
                this.BlockDepth = blockDepth;
                this.IsUnreachable = isUnreachable;
            }

            public Mnemonic Mnemonic { get; }
            public Identifier[] Inputs { get; }
            public Identifier? Output { get; }
            public int BlockDepth { get; }
            public bool IsUnreachable { get; }
        }

    }
}
