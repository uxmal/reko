#region License
/* 
 * Copyright (C) 1999-2023 Pavel Tomin.
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
using Reko.Core;
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Core.Services;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Scanning
{
    /// <summary>
    /// Try to read varargs format, then to parse it, get arguments types and
    /// build application instruction.
    /// </summary>
    public class VarargsFormatScanner
    {
        private readonly IReadOnlyProgram program;
        private readonly IProcessorArchitecture arch;
        private readonly EvaluationContext ctx;
        private readonly ExpressionSimplifier eval;
        private readonly IServiceProvider services;
        private StringConstant? formatString;

        public VarargsFormatScanner(
            IReadOnlyProgram program,
            IProcessorArchitecture arch,
            EvaluationContext ctx,
            IServiceProvider services)
        {
            this.program = program;
            this.arch = arch;
            this.services = services;
            this.ctx = ctx;
            this.eval = new ExpressionSimplifier(
                program.SegmentMap,
                ctx,
                services.RequireService<DecompilerEventListener>());
        }

        public Instruction BuildInstruction(
            Expression callee,
            FunctionType originalSig,
            FunctionType expandedSig,
            ProcedureCharacteristics? chr,
            ApplicationBuilder ab)
        {
            if (callee is ProcedureConstant pc)
            {
                callee = new ProcedureConstant(pc.DataType, expandedSig, pc.Procedure);
            }
            int iFormatArg = originalSig.Parameters!.Length - 1;
            var instr = ab.CreateInstruction(callee, expandedSig, chr);
            ReplaceFormatArgumentWithFormatString(instr, iFormatArg);
            return instr;
        }

        private void ReplaceFormatArgumentWithFormatString(
            Instruction instr,
            int iFormatArg)
        {
            Expression? eFnCall = instr switch
            {
                Assignment ass => ass.Src,
                SideEffect side => side.Expression,
                _ => null,
            };
            if (eFnCall is not Application fnCall)
                return;
            var formatArg = fnCall.Arguments[iFormatArg];
            if (formatArg is Identifier idFormat)
            {
                this.ctx.RemoveIdentifierUse(idFormat);
                fnCall.Arguments[iFormatArg] = this.formatString!;
            }
        }

        public bool TryScan(
            Address addrInstr,
            Expression callee,
            FunctionType sig,
            ProcedureCharacteristics? chr,
            ApplicationBuilder ab,
            [MaybeNullWhen(false)] out VarargsResult result)
        {
            if (sig is null || !sig.IsVariadic ||
                chr is null || !VarargsParserSet(chr))
            {
                result = null;
                return false;
            }
            var addrFormatString = ReadVarargsFormat(addrInstr, callee, sig, ab);
            if (addrFormatString is null)
            {
                result = null;
                return false;
            }
            this.formatString = ReadCString(PrimitiveType.Char, addrFormatString);
            if (formatString is null)
            {
                result = null;
                return false;
            }
            var argTypes = ParseVarargsFormat(chr.VarargsParserClass!, addrInstr, chr);
            var extendedSig = ReplaceVarargs(program.Platform, sig, argTypes);
            result = new VarargsResult(extendedSig, addrFormatString, formatString);
            return true;
        }

        private Expression GetValue(Expression op)
        {
            var (e, _) = op.Accept(eval);
            return e;
        }

        private Address? ReadVarargsFormat(Address addrInstr, Expression callee, FunctionType sig, ApplicationBuilder ab)
        {
            var formatIndex = sig.Parameters!.Length - 1;
            if (formatIndex < 0)
                throw new ApplicationException("Expected variadic function to take at least one parameter.");
            var formatParam = sig.Parameters[formatIndex];
            if (formatParam.Storage is StackStorage stackStorage)
            {
                var stackAccess = ab.BindInStackArg(stackStorage, 0);
                if (stackAccess is { } && GetValue(stackAccess) is Constant c && c.IsValid)
                {
                    return program.Platform.MakeAddressFromConstant(c, false)!;
                }
            }
            else
            {
                var reg = ab.BindInArg(formatParam.Storage)!;
                switch (GetValue(reg))
                {
                case Address addrFormat:
                    return addrFormat;
                case Constant c:
                    if (c.IsValid)
                    {
                        return program.Platform.MakeAddressFromConstant(c, false)!;
                    }
                    break;
                }
            }
            WarnUnableToDetermineFormatString(addrInstr, callee);
            return null;
        }

        private void WarnUnableToDetermineFormatString(Address addrInstr, Expression callee)
        {
            var listener = services.RequireService<DecompilerEventListener>();
            listener.Warn(
                //$TODO: get address of call instruction
                listener.CreateAddressNavigator(this.program, addrInstr),
                "Unable to determine format string for call to '{0}'.",
                callee);
        }

        private StringConstant? ReadCString(PrimitiveType charType, Address addr)
        {
            if (!program.SegmentMap.IsValidAddress(addr))
            {
                return null;
            }
            var rdr = program.CreateImageReader(arch, addr);
            //$BUG: what about wsprintf? Be sure to use an appropriate 
            // Unicode encoding
            var c = rdr.ReadCString(charType, program.TextEncoding);
            // Because the string is going to be used as a parameter to a function,
            // we "decay" it to a pointer to a character.
            c.DataType = new Pointer(charType, addr.DataType.BitSize);
            return c;
        }

        private static bool VarargsParserSet(ProcedureCharacteristics chr)
        {
            return !string.IsNullOrEmpty(chr.VarargsParserClass);
        }

        private IEnumerable<DataType> ParseVarargsFormat(
            string varargsParserTypename,
            Address addrInstr,
            ProcedureCharacteristics chr)
        {
            var svc = services.RequireService<IPluginLoaderService>();
            var type = svc.GetType(varargsParserTypename);
            if (type is null)
                throw new TypeLoadException($"Unable to load {chr.VarargsParserClass} varargs parser.");
            var varargsParser = (IVarargsFormatParser)Activator.CreateInstance(
                type,
                program,
                addrInstr,
                formatString!.ToString(),
                services)!;
            varargsParser.Parse();
            return varargsParser.ArgumentTypes;
        }

        public static FunctionType ReplaceVarargs(
            IPlatform platform,
            FunctionType sig,
            IEnumerable<DataType> argumentTypes)
        {
            var fixedArgs = sig.Parameters!.TakeWhile(p => p.Name != "...").ToList();
            //$REVIEW: default CC tends to be __cdecl.
            var cc = platform.GetCallingConvention("");
            //$BUG: what if platform returns null or throws?
            var allTypes = fixedArgs
                .Select(p => p.DataType)
                .Concat(argumentTypes)
                .ToList();
            var ccr = new CallingConventionEmitter();
            cc!.Generate(
                ccr,
                sig.ReturnAddressOnStack,
                sig.ReturnValue.DataType,
                null, //$TODO: what to do about implicit this?
                allTypes);
            var varArgs = argumentTypes.Zip(
                ccr.Parameters.Skip(fixedArgs.Count),
                (t, s) => new Identifier("", t, s));
            return sig.ReplaceParameters(fixedArgs.Concat(varArgs).ToArray());
        }
    }
}
