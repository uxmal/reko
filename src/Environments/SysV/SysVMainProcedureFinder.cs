#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Linq;

namespace Reko.Environments.SysV;

public class SysVMainProcedureFinder
{
    private readonly Program program;
    private readonly IEventListener listener;

    public SysVMainProcedureFinder(Program program, IEventListener listener)
    {
        this.program = program;
        this.listener = listener;
    }

    public ImageSymbol? Find(Address addrStart)
    {
        if (!program.TryCreateImageReader(addrStart, out var rdr))
            return null;
        var arch = program.Architecture;
        var state = arch.CreateProcessorState();
        var host = new Host(program, listener);
        var rw = arch.CreateRewriter(rdr, state, new StorageBinder(), host);
        foreach (var cluster in rw)
        {
            foreach (var instr in cluster.Instructions)
            {
                switch (instr)
                {
                case RtlAssignment ass:
                    var src = Evaluate(ass.Src, state);
                    if (src is Constant cAss &&
                        ass.Dst is Identifier idDst && idDst.Storage is RegisterStorage reg)
                    {
                        state.SetRegister(reg, cAss);
                    }
                    break;
                case RtlCall call:
                    if (call.Target is not Address addrCallTarget)
                        break;
                    if (!program.TryCreateImageReader(addrCallTarget, out var rdr2))
                        return null;
                    var rw2 = arch.CreateRewriter(rdr2, state.Clone(), new StorageBinder(), host);
                    var stub = ReadStubInstructions(rw2);
                    var target = program.Platform.GetTrampolineDestination(cluster.Address, stub, host);
                    if (target is null)
                        return null;
                    if (target.Procedure.Name != "__libc_start_main")
                        return null;

                    return ImageSymbol.Procedure(program.Architecture,

                        target.StubAddress);
                default:
                    throw new NotImplementedException($"Not implemented: {instr.GetType().Name}");
                }
            }
        }



//00012C1C[0] = E3A0B000 mov fp,#0
//00012C20[1] = E3A0E000 mov lr,#0
//00012C24[2] = E49D1004 pop r1
//00012C28[3] = E1A0200D mov r2, sp
//00012C2C[4] = E52D2004 push r2
//00012C30[5] = E52D0004 push r0
//00012C34[6] = E59FC010 ldr ip,[00012C4C]; [pc,#&10]
//00012C38[7] = E52DC004 push ip
//00012C3C[8] = E59F000C ldr r0,[00012C50]; [pc,#&C]
//00012C40[9] = E59F300C ldr r3,[00012C54]; [pc,#&C]
//00012C44[10] = EBFFFC64 bl $00011DDC
//00012C48[11] = EBFFFD24 bl $000120E0

        return null;
    }

    private List<RtlInstructionCluster> ReadStubInstructions(IEnumerable<RtlInstructionCluster> clusters)
    {
        List<RtlInstructionCluster> result = [];
        foreach (var cluster in clusters)
        {
            result.Add(cluster);
            if (cluster.Instructions.Length > 0 &&
                cluster.Instructions[^1] is RtlGoto)
            {
                return result;
            }
            if (result.Count > 10)
                break;
        }
        return [];
    }

    private Constant? Evaluate(Expression expr, ProcessorState state)
    {
        switch (expr)
        {
        case Constant c:
            return c;
        case Identifier id:
            if (id.Storage is RegisterStorage reg)
                return state.GetRegister(reg);
            break;
        case MemoryAccess mem:
            return state.GetValue(mem, program.Memory) as Constant;
        }
        return null;
    }
    
    private bool Emulate(RtlInstructionCluster cluster)
    {
        throw new NotImplementedException();
        /*
        var instr = cluster.Instructions[0];
        if (instr.Class. == "call" && instr.Operands.Count == 1)
        {
            var op = instr.Operands[0];
            if (op is AddressOperand addrOp)
            {
                var addr = addrOp.Address;
                if (program.TryGetSymbol(addr, out var sym) && sym is ImageSymbol imgSym)
                {
                    return true;
                }
            }
        }*/
        return false;
    }

    private class Host : IRewriterHost, IDynamicLinker
    {
        private readonly Program program;
        private readonly IEventListener eventListener;

        public Host(Program program, IEventListener eventListener)
        {
            this.program = program;
            this.eventListener = eventListener;
        }

        public Constant? GlobalRegisterValue => throw new NotImplementedException();

        public void Error(Address address, string format, params object[] args)
        {
            var nav = eventListener.CreateAddressNavigator(program, address);
            eventListener.Error(nav, format, args);
        }

        public IProcessorArchitecture GetArchitecture(string archMoniker)
        {
            throw new NotImplementedException();
        }

        public Expression? GetImport(Address addrThunk, Address addrInstr)
        {
            throw new NotImplementedException();
        }

        public ExternalProcedure? GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstr)
        {
            if (!this.program.ImportReferences.TryGetValue(addrThunk, out var reference))
                return null;
            return reference.ResolveImportedProcedure(
                this,
                program.Platform,
                new ProgramAddress(this.program, addrThunk),
                eventListener);
        }

        public ExternalProcedure? GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
        {
            throw new NotImplementedException();
        }

        public bool TryRead(IProcessorArchitecture arch, Address addr, PrimitiveType dt, [MaybeNullWhen(false)] out Constant value)
        {
            throw new NotImplementedException();
        }

        public void Warn(Address address, string format, params object[] args)
        {
            var nav = eventListener.CreateAddressNavigator(program, address);
            eventListener.Warn(nav, format, args);
        }

        public Expression ResolveImport(string? module, string procName, IPlatform platform)
        {
            throw new NotImplementedException();
        }

        public Expression ResolveImport(string? module, int ordinal, IPlatform platform)
        {
            throw new NotImplementedException();
        }

        public ExternalProcedure? ResolveProcedure(string? module, string procName, IPlatform platform)
        {
            var x = program.EnvironmentMetadata;
            var xx = x.Lookup(procName);
            return null;
            throw new NotImplementedException();
        }

        public ExternalProcedure ResolveProcedure(string? module, int ordinal, IPlatform platform)
        {
            throw new NotImplementedException();
        }

        public Expression ResolveToImportedValue(Statement stm, Constant c)
        {
            throw new NotImplementedException();

        }
    }
}
