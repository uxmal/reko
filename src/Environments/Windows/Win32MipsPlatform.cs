#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.CLanguage;
using Reko.Core.Code;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Environments.Windows
{
    // https://msdn.microsoft.com/en-us/library/ms881468.aspx
    public class Win32MipsPlatform : Platform
    {
        public Win32MipsPlatform(IServiceProvider services, IProcessorArchitecture arch) : 
            base(services, arch, "winMips")
        {
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters()
        {
            var gp = Architecture.GetRegister("r28");
            var sp = Architecture.GetRegister("sp");
            return new HashSet<RegisterStorage>
            {
                gp, sp
            };
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>
            {
                Architecture.GetRegister("r2"),
                Architecture.GetRegister("r3"),
                Architecture.GetRegister("r4"),
                Architecture.GetRegister("r5"),
                Architecture.GetRegister("r6"),
                Architecture.GetRegister("r7"),
                Architecture.GetRegister("r8"),
                Architecture.GetRegister("r9"),
                Architecture.GetRegister("r10"),
                Architecture.GetRegister("r11"),
                Architecture.GetRegister("r12"),
                Architecture.GetRegister("r13"),
                Architecture.GetRegister("r14"),
                Architecture.GetRegister("r15"),

                Architecture.GetRegister("r24"),
                Architecture.GetRegister("r25"),
            };

        }

        public override CallingConvention GetCallingConvention(string ccName)
        {
            return new MipsCallingConvention(this.Architecture);
        }

        public override ImageSymbol FindMainProcedure(Program program, Address addrStart)
        {
            Services.RequireService<DecompilerEventListener>().Warn(new NullCodeLocation(program.Name),
                "Win32 MIPS main procedure finder not supported.");
            return null;
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException("INT services are not supported by " + this.GetType().Name);
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
                new RtlGoto(ExpressionMatcher.AnyId("r2s"), InstrClass.Delay|InstrClass.Transfer))
        };

        /// <summary>
        /// The sequence 
        ///     lui rX,hiword
        ///     lw  rY,[rX + loword]
        ///     jr  rY
        /// is treated as a trampoline.
        /// </summary>
        /// <param name="insts"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public override ProcedureBase GetTrampolineDestination(IEnumerable<RtlInstructionCluster> rtls, IRewriterHost host)
        {
            var instrs = rtls.SelectMany(r => r.Instructions)
                .Take(3)
                .ToArray();
            var addrFrom = rtls.ElementAt(2).Address;
            if (instrs.Length < 3)
                return null;

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
            var addrTarget= MakeAddressFromConstant(c, false);
            ProcedureBase proc = host.GetImportedProcedure(this.Architecture, addrTarget, addrFrom);
            if (proc != null)
                return proc;
            return host.GetInterceptedCall(this.Architecture, addrTarget);
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 1;
            case CBasicType.Char: return 1;
            case CBasicType.Short: return 2;
            case CBasicType.Int: return 4;
            case CBasicType.Long: return 4;
            case CBasicType.LongLong: return 8;
            case CBasicType.Float: return 4;
            case CBasicType.Double: return 8;
            case CBasicType.LongDouble: return 8;
            case CBasicType.Int64: return 8;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ExternalProcedure LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            EnsureTypeLibraries(PlatformIdentifier);
            ModuleDescriptor mod;
            if (!Metadata.Modules.TryGetValue(moduleName.ToUpper(), out mod))
                return null;
            SystemService svc;
            if (mod.ServicesByOrdinal.TryGetValue(ordinal, out svc))
            {
                return new ExternalProcedure(svc.Name, svc.Signature);
            }
            else
                return null;
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            EnsureTypeLibraries(PlatformIdentifier);
            ModuleDescriptor mod;
            if (!Metadata.Modules.TryGetValue(moduleName.ToUpper(), out mod))
                return null;
            SystemService svc;
            if (mod.ServicesByName.TryGetValue(moduleName, out svc))
            {
                return new ExternalProcedure(svc.Name, svc.Signature);
            }
            else
                throw new NotImplementedException();
        }
    }
}
