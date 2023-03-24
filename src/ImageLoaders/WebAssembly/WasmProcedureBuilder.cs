using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.block = proc.AddBlock(proc.EntryAddress, NamingPolicy.Instance.BlockName(proc.EntryAddress));
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
            var m = new ExpressionEmitter();
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                Identifier id;
                Identifier local;
                switch (instr.Mnemonic)
                {
                case Mnemonic.end:
                    if (rdr.Offset == this.func.End)
                    {
                        Return();
                    }
                    else
                    {
                        //$TODO: Pop controlstack?
                        throw new NotImplementedException();
                    }
                    break;
                case Mnemonic.get_local:
                    local = this.locals[OpAsInt(0)];
                    id = PushValue(local.DataType);
                    Assign(id, local);
                    break;
                case Mnemonic.i32_add: RewriteBinary(m.IAdd); break;
                case Mnemonic.i32_sub: RewriteBinary(m.ISub); break;
                case Mnemonic.i32_const:
                    var imm = (ImmediateOperand) instr.Operands[0];
                    id = PushValue(imm.Width);
                    Assign(id, imm.Value);
                    break;
                default:
                    EmitUnitTest(dasm, rdr);
                    Console.WriteLine($"Unhandled mnemonic {instr.Mnemonic}.");
                    return;
                }
            }
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

        private void RewriteBinary(Func<Expression,Expression,Expression> fn)
        {
            var arg2 = PopValue();
            var arg1 = PopValue();
            var e = fn(arg1, arg2);
            var id = PushValue(e.DataType);
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

        private Identifier PushValue(DataType dt)
        {
            var id  = proc.Frame.CreateTemporary(dt);
            this.valueStack.Add(id);
            return id;
        }

        private Identifier PopValue()
        {
            if (valueStack.Count == 0)
                throw new AddressCorrelatedException(instr.Address, "Exhaused value stack.");
            int iTop = this.valueStack.Count - 1;
            var id = this.valueStack[iTop];
            this.valueStack.RemoveAt(iTop);
            return id;
        }

        private void EmitUnitTest(IEnumerator<WasmInstruction> dasm, EndianImageReader rdr)
        {
             arch.Services.GetService<ITestGenerationService>()?.ReportMissingRewriter("WasmRw", dasm.Current, dasm.Current.Mnemonic.ToString(), rdr, "");
        }

        private class ControlEntry
        {
        }

    }
}
