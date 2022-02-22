#region License
/* 
 * Copyright (C) 1999-2022 Pavel Tomin.
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
using Reko.Core.Hll.C;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.Core.Services;

namespace Reko.Scanning
{
    /// <summary>
    /// Try to read varargs format, then to parse it, get arguments types and
    /// build application instruction.
    /// </summary>
    public class VarargsFormatScanner
    {
        private readonly Program program;
        private readonly IProcessorArchitecture arch;
        private readonly Frame frame;
        private readonly ExpressionSimplifier eval;
        private readonly IServiceProvider services;
        private FunctionType? expandedSig;

        public VarargsFormatScanner(
            Program program,
            Frame frame,
            EvaluationContext ctx,
            IServiceProvider services)
        {
            this.program = program;
            this.arch = program.Architecture;
            this.frame = frame;
            this.eval = new ExpressionSimplifier(
                program.SegmentMap,
                ctx,
                services.RequireService<DecompilerEventListener>());
            this.services = services;
        }

        public Instruction BuildInstruction(
            Expression callee, 
            CallSite site,
            ProcedureCharacteristics? chr)
        {
            if (callee is ProcedureConstant pc)
                pc.Procedure.Signature = this.expandedSig!;
            var ab = arch.CreateFrameApplicationBuilder(frame, site, callee);
            return ab.CreateInstruction(this.expandedSig!, chr);
        }

        public bool TryScan(Address addrInstr, Expression callee, FunctionType sig, ProcedureCharacteristics? chr)
        {
            if (
                sig == null || !sig.IsVariadic ||
                chr == null || !VarargsParserSet(chr)
            )
            {
                this.expandedSig = null;    //$out parameter
                return false;
            }
            var format = ReadVarargsFormat(addrInstr, callee, sig);
            if (format == null)
                return false;
            var argTypes = ParseVarargsFormat(chr.VarargsParserClass!, addrInstr, chr, format);
            this.expandedSig = ReplaceVarargs(program.Platform, sig, argTypes);
            return true;
        }

        private Expression GetValue(Expression op)
        {
            var (e, _) = op.Accept(eval);
            return e;
        }

        private string? ReadVarargsFormat(Address addrInstr, Expression callee, FunctionType sig)
        {
            var formatIndex = sig.Parameters!.Length - 1;
            if (formatIndex < 0)
                throw new ApplicationException("Expected variadic function to take at least one parameter.");
            var formatParam = sig.Parameters[formatIndex];
            // $TODO: Issue #471: what about non-x86 architectures, like Sparc or PowerPC,
            // there can be varargs functions where the first N parameters are
            // passed in registers and the remaining are passed on the stack.
            if (formatParam.Storage is StackStorage stackStorage)
            {
                var stackAccess = arch.CreateStackAccess(
                    frame,
                    stackStorage.StackOffset,
                    stackStorage.DataType);
                if (GetValue(stackAccess) is Constant c && c.IsValid)
                {
                    var str = ReadCString(c);
                    if (str != null)
                        return str;
                }
            }
            else
            {
                var reg = GetValue(formatParam) as Address;
                if (reg != null)
                {
                    var str = ReadCString(reg);
                    if (str != null)
                        return str;
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

        private string? ReadCString(Constant cAddr)
        {
            var addr = program.Platform.MakeAddressFromConstant(cAddr, false)!;
            return ReadCString(addr);
        }

        private string? ReadCString(Address addr)
        {
            if (!program.SegmentMap.IsValidAddress(addr))
            {
                return null;
            }
            var rdr = program.CreateImageReader(program.Architecture, addr);
            var c = rdr.ReadCString(PrimitiveType.Char, program.TextEncoding);
            return c.ToString();
        }

        private bool VarargsParserSet(ProcedureCharacteristics chr)
        {
            return !string.IsNullOrEmpty(chr.VarargsParserClass);
        }

        private IEnumerable<DataType> ParseVarargsFormat(
            string varargsParserTypename,
            Address addrInstr,
            ProcedureCharacteristics chr,
            string format)
        {
            var svc = services.RequireService<IPluginLoaderService>();
            var type = svc.GetType(varargsParserTypename);
            if (type == null)
                throw new TypeLoadException(
                    string.Format(
                        "Unable to load {0} varargs parser.",
                        chr.VarargsParserClass));
            var varargsParser = (IVarargsFormatParser)Activator.CreateInstance(
                type,
                program,
                addrInstr,
                format,
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
            var cc = platform.GetCallingConvention(""); //$REVIEW: default CC tends to be __cdecl.
            //$BUG: what if platform returns null or throws?
            var allTypes = fixedArgs
                .Select(p => p.DataType)
                .Concat(argumentTypes)
                .ToList();
            var ccr = new CallingConventionEmitter();
            cc!.Generate(
                ccr,
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
