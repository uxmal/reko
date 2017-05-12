#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core;
using System.Collections.Generic;
using Reko.Core.Types;
using Expression = Reko.Core.Expressions.Expression;
using Identifier = Reko.Core.Expressions.Identifier;
using IrConstant = Reko.Core.Expressions.Constant;
using IrDomain = Reko.Core.Types.Domain;

namespace Reko.ImageLoaders.LLVM
{
    public class ProgramBuilder
    {
        private PrimitiveType framePointerSize;
        private Program program;

        public ProgramBuilder(PrimitiveType framePointerSize)
        {
            this.framePointerSize = framePointerSize;
            this.program = new Program();
            this.Functions = new Dictionary<FunctionDefinition, ProcedureBuilder>();
            this.Globals = new Dictionary<string, Identifier>();
        }

        public Dictionary<FunctionDefinition, ProcedureBuilder> Functions { get; private set; }
        public Dictionary<string, Identifier> Globals { get; private set; }
        public Dictionary<LocalId, DataType> Types { get; private set;}

        public Program BuildProgram(Module module)
        {
            var address = Address.Create(framePointerSize, 0x1000);
            foreach (var entry in module.Entries)
            {
                address = RegisterEntry(entry, address);
            }

            foreach (var entry in module.Entries)
            {
                TranslateEntry(entry);
            }
            return program;
        }

        public Address RegisterEntry(ModuleEntry entry, Address addr)
        {
            var global = entry as GlobalDefinition;
            if (global != null)
            {
                var glob = RegisterGlobal(global);
                program.GlobalFields.Fields.Add((int)addr.ToLinear(), glob.DataType, glob.Name);
                return addr + 1;
            }
            var fn = entry as FunctionDefinition;
            if (fn != null)
            {
                var proc = RegisterFunction(fn);
                program.Procedures.Add(addr, proc);
                return addr + 1;
            }
            var tydec = entry as TypeDefinition;
            if (fn != null)
            {
                RegisterTypeDefinition(tydec);
                return addr;
            }
            var decl = entry as Declaration;
            if (decl != null)
            {
                RegisterDeclaration(decl);
                return addr;
            }
            throw new NotImplementedException(entry.GetType().Name);
        }

        public Identifier RegisterGlobal(GlobalDefinition global)
        {
            var dt = TranslateType(global.Type);
            var id = new Identifier(global.Name, dt, new MemoryStorage());
            this.Globals.Add(global.Name, id);
            return id;
        }

        public void RegisterDeclaration(Declaration decl)
        {
            var dt = TranslateType(decl.Type);
            var id = new Identifier(decl.Name, dt, new MemoryStorage());
            this.Globals.Add(id.Name, id);
        }
        
        public Procedure RegisterFunction(FunctionDefinition fn)
        {
            var proc = new Procedure(fn.FunctionName, new Frame(framePointerSize));
            var builder = new ProcedureBuilder(proc);
            Functions.Add(fn, builder);
            return proc;
        }

        public void RegisterTypeDefinition(TypeDefinition tydef)
        {
            Types.Add(tydef.Name, tydef.Type.Accept(new TypeTranslator(framePointerSize.Size)));
        }

        public void TranslateEntry(ModuleEntry entry)
        {
            if (entry is GlobalDefinition)
                return;
            var fn = entry as FunctionDefinition;
            if (fn != null)
            {
                TranslateFunction(fn);
                return;
            }
            throw new NotImplementedException(string.Format("TranslateEntry({0})", entry.GetType()));
        }


        public Reko.Core.Types.FunctionType TranslateSignature(
            LLVMType retType, 
            List<LLVMParameter> parameters, 
            ProcedureBuilder m)
        {
            var rt = TranslateType(retType);
            var sigRet = m.Procedure.Frame.CreateTemporary(rt);
            var sigParameters = new List<Identifier>();
            foreach (var param in parameters)
            {
                if (param.name == "...")
                {
                    throw new NotImplementedException("Varargs");
                }
                else
                {
                    var prefix = "arg";
                    var pt = TranslateType(param.Type);
                    var id = m.CreateLocalId(prefix, pt);
                    sigParameters.Add(id);
                }
            }
            return new FunctionType(sigRet, sigParameters.ToArray());
        }

        private DataType TranslateType(LLVMType type)
        {
            var xlat = new TypeTranslator(this.framePointerSize.Size);
            return type.Accept(xlat);
        }

        public void TranslateFunction(FunctionDefinition fn)
        {
            var m = Functions[fn];
            var proc = m.Procedure;
            var sig = TranslateSignature(fn.ResultType, fn.Parameters, m);
            proc.Signature = sig;

            m.EnsureBlock(null);
            var pb = new ProcedureBuilder(proc);
            foreach (var instr in fn.Instructions)
            {
                TranslateInstruction(instr, m);
            }
        }

        public void TranslateInstruction(Instruction instr, ProcedureBuilder m)
        {
            var ret = instr as RetInstr;
            if (ret != null)
            {
                if (ret.Value == null)
                {
                    m.Return();
                }
                else
                {
                    var e = MakeValueExpression(ret.Value, m, TranslateType(ret.Type));
                    m.Return(e);
                }
                return;
            }
            var bin = instr as Binary;
            if (bin != null)
            {
                var type = TranslateType(bin.Type);
                var left = MakeValueExpression(bin.Left, m,   type);
                var right = MakeValueExpression(bin.Right, m, type);
                var dst = m.CreateLocalId("loc", type);
                Func<Expression,Expression,Expression> fn;
                switch (bin.Operator)
                {
                default:
                    throw new NotImplementedException(string.Format("TranslateInstruction({0})", bin.Operator));
                case TokenType.add: fn = m.IAdd; break;
                }
                m.Assign(dst, fn(left, right));
                return;
            }
            var load = instr as Load;
            if (load != null)
            {
                TranslateLoad(load, m);
                return;
            }
            var alloca = instr as Alloca;
            if (alloca != null)
            {
                TranslateAlloca(alloca, m);
                return;
            }
            var conv = instr as Conversion;
            if (conv != null)
            {
                TranslateConversion(conv, m);
                return;
            }
            var br = instr as BrInstr;
            if (br != null)
            {
                TranslateBr(br, m);
                return;
            }
            var cmp = instr as CmpInstruction;
            if (cmp != null)
            {
                TranslateCmp(cmp, m);
            }
            throw new NotImplementedException(string.Format("TranslateInstruction({0})", instr.GetType().Name));
        }

        private void TranslateConversion(Conversion conv, ProcedureBuilder m)
        {
            var dstType = TranslateType(conv.TypeTo);
            var srcType = TranslateType(conv.TypeFrom);
            var src =  MakeValueExpression(conv.Value, m, srcType);
            var dst = m.CreateLocalId("loc", dstType);
            Expression e;
            switch (conv.Operator)
            {
            default:
                throw new NotImplementedException(string.Format("TranslateConversion({0})", conv.Operator));
            case TokenType.sext:
                dstType = PrimitiveType.Create(IrDomain.SignedInt, dstType.Size);
                e = m.Cast(dstType, src);
                break;
            }
            m.Assign(dst, e);
        }

        private void TranslateCmp(CmpInstruction cmp, ProcedureBuilder m)
        {
            var srcType = TranslateType(cmp.Type);
            var op1 = MakeValueExpression(cmp.Op1, m, srcType);
            var op2 = MakeValueExpression(cmp.Op2, m, srcType);
            var dst = m.CreateLocalId("loc", PrimitiveType.Bool);
            Func<Expression,Expression,Expression> fn;
            if (cmp.Operator == TokenType.icmp)
            {
                switch (cmp.ConditionCode)
                {
                default:
                    throw new NotImplementedException(string.Format("TranslateCmp({0})", cmp.ConditionCode));
                }
            }
            else
            {
                switch (cmp.ConditionCode)
                {
                default:
                    throw new NotImplementedException(string.Format("TranslateCmp({0})", cmp.ConditionCode));
                }
            }
            m.Assign(dst, fn(op1, op2));
        }

        private void TranslateLoad(Load load, ProcedureBuilder m)
        {
            var dstType = TranslateType(load.DstType);
            var srcType = TranslateType(load.SrcType);
            var ea = MakeValueExpression(load.Src, m, srcType);
            var dst = m.CreateLocalId("loc", srcType);
            m.Assign(dst, m.Load(dst.DataType, ea));
        }

        private void TranslateBr(BrInstr br, ProcedureBuilder m)
        {
            if (br.Cond == null)
            {
                m.Goto(br.IfTrue.Name);
                return;

            }
            throw new NotImplementedException(string.Format("TranslateBr({0})", br));

        }

        private void TranslateAlloca(Alloca alloca, ProcedureBuilder m)
        {
            var type = TranslateType(alloca.Type);
            int count = 1;
            if (alloca.ElementCount != null)
            {
                throw new NotImplementedException();
            }
            var stk = m.AllocateStackVariable(type, count);
            var dst = m.CreateLocalId("loc", type);
            m.Assign(dst, m.AddrOf(stk));
        }

        private Expression MakeValueExpression(Value value, ProcedureBuilder m, DataType dt)
        {
            var c = value as Constant;
            if (c != null)
            {
                return IrConstant.Create(dt, Convert.ToInt64(c.Value));
            }
            var local = value as LocalId;
            if (local != null)
            {
                return m.GetLocalId(local.Name);
            }
            var global = value as GlobalId;
            if (global != null)
            {
                return Globals[global.Name];
            }
            throw new NotImplementedException(string.Format("MakeValueExpression: {0}", value.ToString() ?? "(null)"));
        }

    }
}