#region License
/* 
 * Copyright (C) 1999-2026 Pavel Tomin.
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
using Reko.Core.Analysis;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Evaluation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
        private readonly IEventListener listener;
        private StringConstant? formatString;

        /// <summary>
        /// Creates an instance of the <see cref="VarargsFormatScanner"/> class.
        /// </summary>
        /// <param name="program">Program being analyzed</param>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> to use.</param>
        /// <param name="ctx"><see cref="EvaluationContext"/> to use when evaluating 
        /// expressions.</param>
        /// <param name="services"><see cref="IServiceProvider"/> providing runtime services.</param>
        /// <param name="listener"><see cref="IEventListener"/> to which diagnostic messages
        /// are reported.</param>
        public VarargsFormatScanner(
            IReadOnlyProgram program,
            IProcessorArchitecture arch,
            EvaluationContext ctx,
            IServiceProvider services,
            IEventListener listener)
        {
            this.program = program;
            this.arch = arch;
            this.services = services;
            this.listener = listener;
            this.ctx = ctx;
            this.eval = new ExpressionSimplifier(
                program.Memory,
                ctx,
                listener);
        }

        /// <summary>
        /// Builds either a <see cref="Assignment"/> or a <see cref="SideEffect"/> instruction
        /// that invokes the variadic function with the given <paramref name="callee"/>.
        /// </summary>
        /// <param name="callee">Reference to a variadic function.</param>
        /// <param name="originalSig">Function signature before expansion of variadic extra argue</param>
        /// <param name="expandedSig">Function signature after expansion of variadic extra arguments.</param>
        /// <param name="chr">Optional procedure characteristics of variadic function.</param>
        /// <param name="ab"><see cref="ApplicationBuilder"/> used to build the variadic <see cref="Application"/> 
        /// expression.</param>
        /// <returns>Resulting instruction calling the <paramref name="callee"/>.</returns>
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
            ReplaceFormatArgumentWithFormatString(instr, iFormatArg, this.formatString!);
            return instr;
        }

        private void ReplaceFormatArgumentWithFormatString(
            Instruction instr,
            int iFormatArg,
            StringConstant formatString)
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
                //$REVIEW: this.ctx.RemoveIdentifierUse(idFormat);
                fnCall.Arguments[iFormatArg] = formatString;
            }
            else if (formatArg is Constant)
            {
                fnCall.Arguments[iFormatArg] = formatString;
            }
        }

        /// <summary>
        /// Determines whether there is a known parser for the variadic argument list
        /// format indicator.
        /// </summary>
        /// <param name="sig">Unexpanded function type of the variadic function.</param>
        /// <param name="chr"><see cref="ProcedureCharacteristics"/> of the variadic procedure.
        /// </param>
        /// <returns>True if there is a known parser for the format string/indicator.
        /// </returns>
        public static bool IsVariadicParserKnown(FunctionType? sig, ProcedureCharacteristics? chr)
        {
            return sig is not null &&
                   sig.IsVariadic &&
                   chr is not null && 
                   VarargsParserSet(chr);
        }

        /// <summary>
        /// Attempts to determine the format string of a variadic function
        /// </summary>
        /// <param name="addrInstr">Address of the call to a variadic function.</param>
        /// <param name="callee">The variadic function.</param>
        /// <param name="sig">The signature of the variadic function.</param>
        /// <param name="chr"><see cref="ProcedureCharacteristics"/> of the called function.</param>
        /// <param name="ab"><see cref="ApplicationBuilder"/> used to make the call to the variadic
        /// function.</param>
        /// <param name="result">The resulting <see cref="VarargsResult"/> if the varargs function's 
        /// format string was successfully parsed.
        /// </param>
        /// <returns>True if the varargs function could be resolved; otherwise false.
        /// </returns>
        public bool TryScan(
            Address addrInstr,
            Expression callee,
            FunctionType sig,
            ProcedureCharacteristics? chr,
            ApplicationBuilder ab,
            [MaybeNullWhen(false)] out VarargsResult result)
        {
            if (!IsVariadicParserKnown(sig, chr))
            {
                result = null;
                return false;
            }
            if (!TryReadFormatStringAddress(addrInstr, callee, sig, ab, out var addrFormatString, out var addrPtrFormatString))
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
            Debug.Assert(chr is not null);
            Debug.Assert(chr.VarargsParserClass is not null);
            var argTypes = ParseVarargsFormat(chr.VarargsParserClass, addrInstr, chr);
            var extendedSig = ReplaceVarargs(program.Platform, sig, argTypes);
            result = new VarargsResult(extendedSig, addrFormatString, formatString);
            return true;
        }

        private Expression GetValue(Expression op)
        {
            var (e, _) = op.Accept(eval);
            return e;
        }

        private bool TryReadFormatStringAddress(
            Address addrInstr,
            Expression callee,
            FunctionType sig,
            ApplicationBuilder ab,
            out Address addrFormatString,
            out Address addrPtrToFormatString)
        {
            addrFormatString = default;
            addrPtrToFormatString = default;
            var formatIndex = sig.Parameters!.Length - 1;
            if (formatIndex < 0)
                throw new ApplicationException("Expected variadic function to take at least one parameter.");
            var formatParam = sig.Parameters[formatIndex];
            if (formatParam.Storage is StackStorage stackStorage)
            {
                var stackAccess = ab.BindInStackArg(stackStorage, 0);
                if (stackAccess is null)
                    return false;
                
                switch (GetValue(stackAccess))
                {
                case Address addr:
                    addrFormatString = addr;
                    return true;
                case Constant c when c.IsValid:
                    addrFormatString = program.Platform.MakeAddressFromConstant(c, false)!.Value;
                    return true;
                case Identifier id:
                    var w = ctx.GetValue(id);
                    if (w is MemoryAccess mem && mem.DataType is PrimitiveType pt)
                    {
                        if (mem.EffectiveAddress is not Constant cea)
                            return false;
                        var ea = program.Platform.MakeAddressFromConstant(cea, false)!;
                        if (!arch.Endianness.TryRead(program.Memory, ea.Value, pt, out var dc))
                            return false;
                        addrFormatString = program.Platform.MakeAddressFromConstant(dc, false)!.Value;
                    }
                    w?.ToString();
                    return false;
                }
            }
            else
            {
                var reg = ab.BindInArg(formatParam.Storage)!;
                if (program.TryInterpretAsAddress(GetValue(reg), false, out addrFormatString))
                    return true;
            }
            // We may need to wait until the analysis stage provides the capability
            // to discover arguments passed on stack.
            return false;
        }

        private StringConstant? ReadCString(PrimitiveType charType, Address addr)
        {
            if (!program.TryCreateImageReader(arch, addr, out var rdr))
                return null;
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
            var pluginServce = services.RequireService<IPluginLoaderService>();
            var type = pluginServce.GetType(varargsParserTypename);
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

        /// <summary>
        /// Expands the variadic function signature by replacing the
        /// variadic argument list with the given <paramref name="argumentTypes"/>.
        /// </summary>
        /// <param name="platform"><see cref="IPlatform"/> </param>
        /// <param name="sig">"Unexpanded" arguments of the variadic funcition.</param>
        /// <param name="argumentTypes">Data types of the expanded variadic function.</param>
        /// <returns></returns>
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
            var ccr = new CallingConventionBuilder();
            cc!.Generate(
                ccr,
                sig.ReturnAddressOnStack,
                sig.Outputs[0].DataType,
                null, //$TODO: what to do about implicit this?
                allTypes);
            var varArgs = argumentTypes.Zip(
                ccr.Parameters.Skip(fixedArgs.Count),
                (t, s) => new Identifier("", t, s));
            return sig.ReplaceParameters(fixedArgs.Concat(varArgs).ToArray());
        }
    }
}
