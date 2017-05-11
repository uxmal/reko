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

namespace Reko.ImageLoaders.LLVM
{
    public class ProgramBuilder
    {
        private PrimitiveType framePointerSize;
        private Block block;

        public ProgramBuilder(PrimitiveType framePointerSize)
        {
            this.framePointerSize = framePointerSize;
            this.Functions = new Dictionary<FunctionDefinition, ProcedureBuilder>();
        }

        public Dictionary<FunctionDefinition, ProcedureBuilder> Functions { get; private set; }
        public Dictionary<LocalId, DataType> Types { get; private set;}
    

        public Program MakeProgram()
        {
            throw new NotImplementedException();
        }

        public void RegisterEntry(ModuleEntry entry)
        {
            var fn = entry as FunctionDefinition;
            if (fn != null)
            {
                RegisterFunction(fn);
                return;
            }
            var tydec = entry as TypeDefinition;
            if (fn != null)
            {
                RegisterTypeDefinition(tydec);
                return;
            }
            throw new NotImplementedException();
        }

        public void TranslateEntry(ModuleEntry entry)
        {
            throw new NotImplementedException();
        }

        public void RegisterFunction(FunctionDefinition fn)
        {
            var proc = new Procedure(fn.FunctionName, new Frame(framePointerSize));
            var builder = new ProcedureBuilder(proc);
            Functions.Add(fn, builder);
        }

        public void RegisterTypeDefinition(TypeDefinition tydef)
        {
            Types.Add(tydef.Name, tydef.Type.Accept(new TypeTranslator(framePointerSize.Size)));
        }

        public Reko.Core.Types.FunctionType TranslateSignature(
            LLVMType retType, 
            List<LLVMArgument> parameters, 
            ProcedureBuilder state)
        {
            var rt = TranslateType(retType);
            var sigRet = state.Procedure.Frame.CreateTemporary(rt);
            var sigParameters = new List<Identifier>();
            foreach (var param in parameters)
            {
                if (param.name == "...")
                {
                    throw new NotImplementedException("Varargs");
                }
                else
                {
                    var pt = TranslateType(param.Type);
                    var name = "%" + state.NextTemp();
                    var id = state.Procedure.Frame.CreateTemporary(name, pt);
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

            var labels = new Dictionary<string, Block>();
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
                    var e = MakeValueExpression(ret.Value, ret.Type);
                    m.Return(e);
                }
            }
        }

        private Expression MakeValueExpression(Value value, LLVMType type)
        {
            var c = value as Constant;
            if (c != null)
            {
                var dt = type.Accept(new TypeTranslator(framePointerSize.Size));
                return IrConstant.Create(dt, Convert.ToInt64(c.Value));
            }
            throw new NotImplementedException();
        }

    }
}