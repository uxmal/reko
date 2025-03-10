#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Hll.C;
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
using Reko.Core.Loading;
using Reko.Core.Machine;
using Reko.Core.Memory;

namespace Reko.Environments.Windows
{
    // https://msdn.microsoft.com/en-us/library/ms881468.aspx
    public class Win32MipsPlatform : Platform
    {
        private readonly HashSet<RegisterStorage> implicitRegs;

        public Win32MipsPlatform(IServiceProvider services, IProcessorArchitecture arch) : 
            base(services, arch, "winMips")
        {
            var gp = arch.GetRegister("r28")!;
            var sp = arch.GetRegister("sp")!;
            implicitRegs = new HashSet<RegisterStorage>
            {
                gp, sp
            };
            this.StructureMemberAlignment = 8;
            this.TrashedRegisters = CreateTrashedRegisters();
        }

        public override string DefaultCallingConvention => "";

        public override CParser CreateCParser(TextReader rdr, ParserState? state)
        {
            state ??= new ParserState();
            var lexer = new CLexer(rdr, CLexer.MsvcKeywords);
            var parser = new CParser(state, lexer);
            return parser;
        }

        public override bool IsImplicitArgumentRegister(RegisterStorage reg)
        {
            return implicitRegs.Contains(reg);
        }

        private HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            return new HashSet<RegisterStorage>
            {
                Architecture.GetRegister("r2")!,
                Architecture.GetRegister("r3")!,
                Architecture.GetRegister("r4")!,
                Architecture.GetRegister("r5")!,
                Architecture.GetRegister("r6")!,
                Architecture.GetRegister("r7")!,
                Architecture.GetRegister("r8")!,
                Architecture.GetRegister("r9")!,
                Architecture.GetRegister("r10")!,
                Architecture.GetRegister("r11")!,
                Architecture.GetRegister("r12")!,
                Architecture.GetRegister("r13")!,
                Architecture.GetRegister("r14")!,
                Architecture.GetRegister("r15")!,

                Architecture.GetRegister("r24")!,
                Architecture.GetRegister("r25")!,
            };

        }

        public override ICallingConvention GetCallingConvention(string? ccName)
        {
            return new MipsCallingConvention(this.Architecture);
        }

        public override ImageSymbol? FindMainProcedure(Program program, Address addrStart)
        {
            Services.RequireService<IEventListener>().Warn(new NullCodeLocation(program.Name),
                "Win32 MIPS main procedure finder not supported.");
            return null;
        }

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
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
        public override Trampoline? GetTrampolineDestination(Address addrInstr, List<RtlInstructionCluster> instrs, IRewriterHost host)
        {
            if (instrs.Count < 3)
                return null;

            var matches = new ExpressionMatch[3];
            for (int i = 0; i < 3; ++i)
            {
                matches[i] = trampPattern[i].Match(instrs[instrs.Count - 3 + i].Instructions[0]);
                if (!matches[i].Success)
                    return null;
            }
            if (matches[0].CapturedExpression("r0d") != matches[1].CapturedExpression("r1s"))
                return null;
            if (matches[1].CapturedExpression("r1d") != matches[2].CapturedExpression("r2s"))
                return null;
            var hi = (Constant)matches[0].CapturedExpression("hi")!;
            var lo = (Constant)matches[1].CapturedExpression("lo")!;
            var c = Operator.IAdd.ApplyConstants(hi.DataType, hi, lo);
            var a = MakeAddressFromConstant(c, false);
            if (a is null)
                return null;
            var addrTarget = a.Value;
            ProcedureBase? proc = host.GetImportedProcedure(this.Architecture, addrTarget, addrInstr);
            if (proc is null)
                proc = host.GetInterceptedCall(this.Architecture, addrTarget);
            if (proc is null)
                return null;
            return new Trampoline(instrs[^3].Address, proc);
        }

        public override ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> rtls, IRewriterHost host)
        {
            var instrs = rtls
                .Take(3)
                .ToArray();
            var addrFrom = addrInstr;
            if (instrs.Length < 3)
                return null;

            var matches = new ExpressionMatch[3];
            for (int i = 0; i < 3; ++i)
            {
                var m = trampPattern[i].Match(instrs[i]);
                if (!m.Success)
                    return null;
                matches[i] = m;
            }
            if (matches[0].CapturedExpression("r0d") != matches[1].CapturedExpression("r1s"))
                return null;
            if (matches[1].CapturedExpression("r1d") != matches[2].CapturedExpression("r2s"))
                return null;
            var hi = (Constant)matches[0].CapturedExpression("hi")!;
            var lo = (Constant)matches[1].CapturedExpression("lo")!;
            var c = Operator.IAdd.ApplyConstants(hi.DataType, hi, lo);
            var a = MakeAddressFromConstant(c, false);
            if (a is null)
                return null;
            var addrTarget = a.Value;
            ProcedureBase? proc = host.GetImportedProcedure(this.Architecture, addrTarget, addrFrom);
            if (proc is not null)
                return proc;
            return host.GetInterceptedCall(this.Architecture, addrTarget);
        }

        public override int GetBitSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
            case CBasicType.Bool: return 8;
            case CBasicType.Char: return 8;
            case CBasicType.WChar_t: return 16;
            case CBasicType.Short: return 16;
            case CBasicType.Int: return 32;
            case CBasicType.Long: return 32;
            case CBasicType.LongLong: return 64;
            case CBasicType.Float: return 32;
            case CBasicType.Double: return 64;
            case CBasicType.LongDouble: return 64;
            case CBasicType.Int64: return 64;
            default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
            }
        }

        public override ExternalProcedure? LookupProcedureByOrdinal(string? moduleName, int ordinal)
        {
            if (moduleName is null)
                return null;
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            if (!metadata.Modules.TryGetValue(moduleName.ToUpper(), out ModuleDescriptor? mod))
                return null;
            if (mod.ServicesByOrdinal.TryGetValue(ordinal, out SystemService? svc))
            {
                return new ExternalProcedure(svc.Name!, svc.Signature!);
            }
            else
                return null;
        }

        public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
        {
            if (moduleName is null)
                return null;
            var metadata = EnsureTypeLibraries(PlatformIdentifier);
            if (!metadata.Modules.TryGetValue(moduleName.ToUpper(), out ModuleDescriptor? mod))
                return null;
            if (mod.ServicesByName.TryGetValue(moduleName, out SystemService? svc))
            {
                return new ExternalProcedure(svc.Name!, svc.Signature!);
            }
            else
                return null;
        }
    }
}
