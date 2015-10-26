#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Environments.Win32
{
    // https://msdn.microsoft.com/en-us/library/ms881468.aspx
    public class Win32MipsPlatform : Win32Platform
    {
        public Win32MipsPlatform(IServiceProvider services, IProcessorArchitecture arch) : 
            base(services, arch)
        {
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override Address AdjustProcedureAddress(Address addr)
        {
            return addr;
        }

        public override BitSet CreateImplicitArgumentRegisters()
        {
            var bitset = base.Architecture.CreateRegisterBitset();
            var gp = Architecture.GetRegister("r28");
            var sp = Architecture.GetRegister("sp");
            bitset[gp.Number] = true;
            bitset[sp.Number] = true;
            return bitset;
        }

        public override ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention)
        {
            return new MipsProcedureSerializer(Architecture, typeLoader, defaultConvention);
        }

        private readonly RtlInstructionMatcher[] trampPattern = new RtlInstructionMatcher[] {
            new RtlInstructionMatcher(
                new RtlAssignment(ExpressionMatcher.AnyId("r0d"), ExpressionMatcher.AnyConstant("hi"))),
            new RtlInstructionMatcher(
                new RtlAssignment(ExpressionMatcher.AnyId("r1d"), new MemoryAccess(
                    new BinaryExpression(
                        Operator.IAdd,
                        ExpressionMatcher.AnyDataType(null),
                        ExpressionMatcher.AnyId("r1s"),
                        ExpressionMatcher.AnyConstant("lo")),
                    PrimitiveType.Word32))),
            new RtlInstructionMatcher(
                new RtlGoto(ExpressionMatcher.AnyId("r2s"), RtlClass.Delay|RtlClass.Transfer))
        };

        public override ProcedureBase GetTrampolineDestination(ImageReader imageReader, IRewriterHost host)
        {
            var rtls = Architecture.CreateRewriter(
                imageReader,
                Architecture.CreateProcessorState(),
                Architecture.CreateFrame(),
                host)
                .Take(3)
                .ToArray();
            if (rtls.Length < 3)
                return null;
            var instrs = rtls
                .SelectMany(rtl => rtl.Instructions)
                .ToArray();

            for (int i = 0; i < 3; ++i)
            {
                if (!trampPattern[i].Match(instrs[i]))
                    return null;
            }
            if (trampPattern[0].CapturedExpressions("r0d") != trampPattern[1].CapturedExpressions("r1s"))
                return null;
            if (trampPattern[1].CapturedExpressions("r1d") != trampPattern[2].CapturedExpressions("r2s"))
                return null;
            var hi = (Constant)trampPattern[0].CapturedExpressions("hi");
            var lo = (Constant)trampPattern[1].CapturedExpressions("lo");
            var c = Operator.IAdd.ApplyConstants(hi, lo);
            var addrTarget= MakeAddressFromConstant(c);
            ProcedureBase proc = host.GetImportedProcedure(addrTarget, rtls[2].Address);
            if (proc != null)
                return proc;
            return host.GetInterceptedCall(addrTarget);

        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
