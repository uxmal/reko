#region License
/* 
 * Copyright (C) 1999-2017 Pavel Tomin.
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
        private FunctionType sig;

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
                ctx,
                services.RequireService<DecompilerEventListener>());
            this.services = services;
        }

        public Instruction BuildInstruction(
            Expression callee, 
            CallSite site,
            ProcedureCharacteristics chr)
        {
            var pc = callee as ProcedureConstant;
            if (pc != null)
                pc.Procedure.Signature = this.sig;
            var ab = arch.CreateFrameApplicationBuilder(frame, site, callee);
            return ab.CreateInstruction(this.sig, chr);
        }

        public bool TryScan(Address addrInstr, FunctionType sig, ProcedureCharacteristics chr)
        {
            if (
                sig == null || !sig.IsVarargs() ||
                chr == null || !VarargsParserSet(chr)
            )
            {
                this.sig = null;    //$out parameter
                return false;
            }
            var format = ReadVarargsFormat(sig);
            var argTypes = ParseVarargsFormat(addrInstr, chr, format);
            this.sig = ReplaceVarargs(sig, argTypes);
            return true;
        }

        private Expression GetValue(Expression op)
        {
            return op.Accept<Expression>(eval);
        }

        private string ReadVarargsFormat(FunctionType sig)
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
                throw new NotSupportedException(
                    string.Format(
                        "The {0} parameter of {1} wasn't a stack access.",
                         formatParam.Name,
                         sig.Name));
            var stackAccess = arch.CreateStackAccess(
                frame,
                stackStorage.StackOffset,
                stackStorage.DataType);
            var c = GetValue(stackAccess) as Constant;
            if (c == null || !c.IsValid)
                throw new ApplicationException(
                    string.Format("Varargs: invalid format constant"));
            return ReadCString(c);
        }

        private string ReadCString(Constant cAddr)
        {
            var addr = program.Platform.MakeAddressFromConstant(cAddr);
            if (!program.SegmentMap.IsValidAddress(addr))
                throw new ApplicationException(
                    string.Format("Varargs: invalid address: {0}", addr));
            var rdr = program.CreateImageReader(addr);
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

        private FunctionType ReplaceVarargs(
            FunctionType sig,
            IEnumerable<DataType> argumentTypes)
        {
            var varargs = sig.Parameters.Last();
            var varargsStorage = varargs.Storage as StackStorage;
            if (varargsStorage == null)
                throw new NotSupportedException(
                    string.Format(
                        "The {0} parameter of {1} wasn't a stack access.",
                        varargs.Name,
                        sig.Name));
            var stackOffset = varargsStorage.StackOffset;
            var args = argumentTypes.Select(dt =>
            {
                var stg = new StackArgumentStorage(stackOffset, dt);
                stackOffset += dt.Size;
                return new Identifier(null, dt, stg);
            });

            return sig.ReplaceVarargs(args.ToArray());
        }
    }
}
