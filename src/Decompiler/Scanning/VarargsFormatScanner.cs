#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Core.CLanguage;
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
        private Program program;
        private IProcessorArchitecture arch;
        private Frame frame;
        private ExpressionSimplifier eval;
        private IServiceProvider services;
        private FunctionType expandedSig;

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
            ProcedureCharacteristics chr)
        {
            if (callee is ProcedureConstant pc)
                pc.Procedure.Signature = this.expandedSig;
            var ab = arch.CreateFrameApplicationBuilder(frame, site, callee);
            return ab.CreateInstruction(this.expandedSig, chr);
        }

        public bool TryScan(Address addrInstr, Expression callee, FunctionType sig, ProcedureCharacteristics chr)
        {
            if (
                sig == null || !sig.IsVarargs() ||
                chr == null || !VarargsParserSet(chr)
            )
            {
                this.expandedSig = null;    //$out parameter
                return false;
            }
            var format = ReadVarargsFormat(addrInstr, callee, sig);
            if (format == null)
                return false;
            var argTypes = ParseVarargsFormat(addrInstr, chr, format);
            this.expandedSig = ReplaceVarargs(program.Platform, sig, argTypes);
            return true;
        }

        private Expression GetValue(Expression op)
        {
            return op.Accept<Expression>(eval);
        }

        private string ReadVarargsFormat(Address addrInstr, Expression callee, FunctionType sig)
        {
            var formatIndex = sig.Parameters.Length - 2;
            if (formatIndex < 0)
                throw new ApplicationException(
                    string.Format("Varargs: should be at least 2 parameters"));
            var formatParam = sig.Parameters[formatIndex];
            // $TODO: Issue #471: what about non-x86 architectures, like Sparc or PowerPC,
            // there can be varargs functions where the first N parameters are
            // passed in registers and the remaining are passed on the stack.
            var stackStorage = formatParam.Storage as StackStorage;
            if (stackStorage == null)
            {
                var reg = GetValue(formatParam) as Address;
                if (reg != null)
                {
                    return ReadCString(reg);
                }
                WarnUnableToDetermineFormatString(addrInstr, callee);
                return null;
            }
            var stackAccess = arch.CreateStackAccess(
                frame,
                stackStorage.StackOffset,
                stackStorage.DataType);
            var c = GetValue(stackAccess) as Constant;
            if (c == null || !c.IsValid)
            {
                WarnUnableToDetermineFormatString(addrInstr, callee);
                return null;
            }
            return ReadCString(c);
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

        private string ReadCString(Constant cAddr)
        {
            var addr = program.Platform.MakeAddressFromConstant(cAddr, false);
            return ReadCString(addr);
        }

        private string ReadCString(Address addr)
        {
            if (!program.SegmentMap.IsValidAddress(addr))
                throw new ApplicationException(
                    string.Format("Varargs: invalid address: {0}", addr));
            var rdr = program.CreateImageReader(program.Architecture, addr);
            var c = rdr.ReadCString(PrimitiveType.Char, program.TextEncoding);
            return c.ToString();
        }

        private bool VarargsParserSet(ProcedureCharacteristics chr)
        {
            return !string.IsNullOrEmpty(chr.VarargsParserClass);
        }

        private IEnumerable<DataType> ParseVarargsFormat(
            Address addrInstr,
            ProcedureCharacteristics chr,
            string format)
        {
            var type = Type.GetType(chr.VarargsParserClass);
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
                services);
            varargsParser.Parse();
            return varargsParser.ArgumentTypes;
        }

        public static FunctionType ReplaceVarargs(
            IPlatform platform,
            FunctionType sig,
            IEnumerable<DataType> argumentTypes)
        {
            var fixedArgs = sig.Parameters.TakeWhile(p => p.Name != "...").ToList();
            var cc = platform.GetCallingConvention(""); //$REVIEW: default CC tends to be __cdecl.
            var allTypes = fixedArgs
                .Select(p => p.DataType)
                .Concat(argumentTypes)
                .ToList();
            var ccr = new CallingConventionEmitter();
            cc.Generate(
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
