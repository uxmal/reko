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
 * You should have received a copy of the GNU General Public License,
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Hll.C;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Environments.PlayStation.Ps2;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using Application = Reko.Core.Expressions.Application;

namespace Reko.Environments.PlayStation
{
    /// <summary>
    /// The operating environment of the Sony PlayStation 2.
    /// </summary>
    /// <remarks>
    /// PS2 games are statically linked MIPS-like EE binaries that request OS
    /// services by executing SYSCALL instructions whose code field selects
    /// a kernel service (threading, semaphores, GS video mode setup, ...).
    /// </remarks>
    public class Ps2Platform : Platform
    {
        private static readonly Lazy<IReadOnlyDictionary<int, SystemService>> sysServices =
            new(CreateSystemServices);

        public Ps2Platform(IServiceProvider services, IProcessorArchitecture arch)
            : base(services, arch, "ps2")
        {
            this.StructureMemberAlignment = 4;
            this.TrashedRegisters = new HashSet<RegisterStorage>
            {
                // Kernel calls follow the o32 convention: argument registers,
                // temporaries and return values are not preserved. The ra,
                // sp, gp and s registers are preserved by the kernel.
                Reg("v0"), Reg("v1"),
                Reg("a0"), Reg("a1"), Reg("a2"), Reg("a3"),
                Reg("t0"), Reg("t1"), Reg("t2"), Reg("t3"),
                Reg("t4"), Reg("t5"), Reg("t6"), Reg("t7"),
            };
        }

        private RegisterStorage Reg(string name) => Architecture.GetRegister(name)!;

        public override string DefaultCallingConvention => "ps2ee";


        /// <inheritdoc/>
        public override ICallingConvention? GetCallingConvention(string? ccName)
        {
            if (string.IsNullOrEmpty(ccName) || ccName == "ps2ee")
                return new Ps2EmotionEngineCallingConvention(Architecture);
            return null;
        }

        public override SystemService? FindService(int vector, ProcessorState? state, IMemory? memory)
        {
            // Some PS2 runtime libraries issue 'SYSCALL' with a zero code,
            // placing the desired service number in the v1 register; that
            // convention takes precedence over the embedded code.
            if (vector == 0 && state is not null)
            {
                var v1 = state.GetRegister(Architecture.GetRegister("v1")!);
                if (v1 is { IsValid: true })
                {
                    return sysServices.Value.TryGetValue(v1.ToInt32(), out var svc)
                        ? svc
                        : null;
                }
            }
            return sysServices.Value.TryGetValue(vector, out var svcDirect) ? svcDirect : null;
        }

        /// <summary>
        /// Recognizes the call pattern emitted when rewriting EE SYSCALL
        /// instructions: __syscall(code).
        /// </summary>
        public override SystemService? FindService(RtlInstruction rtl, ProcessorState? state, IMemory? memory)
        {
            if (rtl is not RtlSideEffect call ||
                call.Expression is not Application app ||
                app.Procedure is not ProcedureConstant procCst ||
                procCst.Procedure is not IntrinsicProcedure intrinsic ||
                !intrinsic.IsInstanceOf(CommonOps.Syscall_1) ||
                app.Arguments.Length != 1 ||
                app.Arguments[0] is not Constant code)
            {
                return null;
            }
            return FindService(code.ToInt32(), state, memory);
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
            default: throw new NotImplementedException($"C basic type {cb} is not supported.");
            }
        }

        public override ExternalProcedure LookupProcedureByName(string? moduleName, string procName)
        {
            // PS2 executables are statically linked; there are no imports.
            return null!;
        }

        public override Address? MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            var uAddr = c.ToUInt32();
            if (codeAlign)
                uAddr &= ~3u;
            return Address.Ptr32(uAddr);
        }

        public override bool TryParseAddress(string? sAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(sAddress, out addr);
        }

        ///////////////////////////////////////////////////////////////////
        // The EE kernel system services.

        private static IReadOnlyDictionary<int, SystemService> CreateSystemServices()
        {
            var sc = new ServiceContainer();
            var options = new Dictionary<string, object>
            {
                { ProcessorOption.InstructionSet, "ps2ee" },
                { ProcessorOption.Endianness, "le" },
                { ProcessorOption.WordSize, 32 }
            };
            var arch = new MipsLe32Architecture(sc, "mips-le-32", options);
            IProcessorArchitecture a = arch;

            DataType Int32 = PrimitiveType.Int32;
            DataType UInt32 = PrimitiveType.UInt32;
            var ptrVoid32 = new PointerType(PrimitiveType.Byte, 32);
            var ptrStr32 = new PointerType(PrimitiveType.Char, 32);
            var ptrPtrStr32 = new PointerType(ptrStr32, 32);
            var ee_thread_t = new StructureType("ee_thread_t", 0)
            {
                Fields =
                {
                    { 0, ptrVoid32, "func" }, // A pointer to the thread's entry function (void (*func)(void *arg)).
                    { 4, ptrVoid32, "stack" }, // A pointer to the memory allocated for the thread's stack.
                    { 8, Int32, "stacksize" },             // The size of the allocated stack in bytes
                    { 12, PrimitiveType.Int32, "gp_reg" },  // Global pointer
                    { 16, PrimitiveType.Int32, "initial_priority" }, // The starting priority level of the thread. Lower numerical values represent higher priorities.
                    { 20, PrimitiveType.Int32, "attr" },                // Thread attribute flags
                    { 24, PrimitiveType.Int32, "option" },          // Optional user data or settings (documented as obsolete/not working in standard kernels
                }
            };
            var ptrThread32 = new PointerType(ee_thread_t, 32);

            var ee_sema_t = new StructureType
            {
                Fields =
                {
                    { 0, PrimitiveType.Int32, "count" },            // Current value/count of the semaphore
                    { 4, PrimitiveType.Int32, "max_count" },        // Maximum allowed value of the semaphore
                    { 8, PrimitiveType.Int32, "init_count" },       // Initial value when the semaphore is created
                    { 12, PrimitiveType.Int32, "wait_threads" },    // Number of threads currently waiting on this semaphore
                    { 16, PrimitiveType.UInt32, "attr" },           // Semaphore attributes/flags
                    { 20, PrimitiveType.UInt32, "option" },         // Extra user-defined options or naming identifier
                }
            };
            var ptrSema32 = new PointerType(ee_sema_t, 32);

            var regRet = arch.GetRegister("v0")!;
            SystemService Svc(string name, DataType? ret, params DataType[] args)
            {
                var formals = new Identifier[args.Length];
                for (int i = 0; i < args.Length; ++i)
                {
                    var regName = i switch
                    {
                        0 => "a0",
                        1 => "a1",
                        2 => "a2",
                        3 => "a3",
                        _ => "t0",
                    };
                    formals[i] = new Identifier(
                        regName,
                        args[i],
                        arch.GetRegister(regName)!);
                }
                var retVal = ret is null ? null : new Identifier("", ret, regRet);
                return new SystemService
                {
                    Name = name,
                    SyscallInfo = new SyscallInfo(),
                    Signature = FunctionType.Create(retVal, formals),
                    Characteristics = new ProcedureCharacteristics(),
                };
            }

            var dict = new Dictionary<int, SystemService>
            {
                // --- Core & Initialization ---
                { 0x00, Svc("RFU000_FullReset", null) },
                { 0x01, Svc("ResetEE", null, UInt32) },
                { 0x02, Svc("SetGsCrt", null, Int32, Int32, Int32) },
                { 0x04, Svc("Exit", null) },
                { 0x05, Svc("ResumeIntrDispatch", null) },
                { 0x06, Svc("LoadExecPS2", Int32, ptrStr32, Int32, ptrPtrStr32) },
                { 0x07, Svc("ExecPS2", Int32, UInt32, UInt32, Int32, ptrPtrStr32) },
        
                // --- Interrupt Handlers & SBUS ---
                { 0x0A, Svc("AddSbusIntcHandler", Int32, Int32, ptrVoid32, Int32, ptrVoid32) },
                { 0x0B, Svc("RemoveSbusIntcHandler", Int32, Int32, Int32) },
                { 0x0C, Svc("Interrupt2Iop", null, Int32) },
                { 0x10, Svc("AddIntcHandler", Int32, Int32, ptrVoid32, Int32) },
                { 0x11, Svc("RemoveIntcHandler", Int32, Int32, Int32) },
                { 0x12, Svc("AddDmacHandler", Int32, Int32, ptrVoid32, Int32) },
                { 0x13, Svc("RemoveDmacHandler", Int32, Int32, Int32) },

                { 0x14, Svc("EnableIntc", Int32, Int32) },
                { 0x15, Svc("DisableIntc", Int32, Int32) },
                { 0x16, Svc("EnableDmac", Int32, Int32) },
                { 0x17, Svc("DisableDmac", Int32, Int32) },

                // --- Thread Management ---
                { 0x20, Svc("CreateThread", Int32, ptrThread32) },
                { 0x21, Svc("DeleteThread", Int32, Int32) },
                { 0x22, Svc("StartThread", Int32, Int32, ptrVoid32) },
                { 0x23, Svc("ExitThread", null) },
                { 0x24, Svc("ExitDeleteThread", null) },
                { 0x25, Svc("TerminateThread", Int32, Int32) },
                { 0x27, Svc("DisableDispatchThread", null) },
                { 0x28, Svc("EnableDispatchThread", null) },
                { 0x29, Svc("ChangeThreadPriority", Int32, Int32, Int32) },
                { 0x2B, Svc("RotateThreadReadyQueue", Int32, Int32) },
                { 0x2D, Svc("ReleaseWaitThread", Int32, Int32) },
                { 0x2F, Svc("GetThreadId", Int32) },
                { 0x30, Svc("ReferThreadStatus", Int32, Int32, ptrVoid32) },
                { 0x32, Svc("SleepThread", null) },
                { 0x33, Svc("WakeupThread", Int32, Int32) },
                { 0x35, Svc("CancelWakeupThread", Int32, Int32) },
                { 0x37, Svc("SuspendThread", Int32, Int32) },
                { 0x39, Svc("ResumeThread", Int32, Int32) },
                { 0x3B, Svc("JoinThread", Int32, Int32) },
                { 0x3C, Svc("SetupThread", null, Int32, ptrVoid32, Int32, ptrVoid32) },
        
                // --- Memory / Heap ---
                { 0x3D, Svc("SetupHeap", ptrVoid32, ptrVoid32, Int32) },
                { 0x3E, Svc("EndOfHeap", ptrVoid32) },

                // --- Semaphores ---
                { 0x40, Svc("CreateSema", Int32, ptrSema32) },
                { 0x41, Svc("DeleteSema", Int32, Int32) },
                { 0x42, Svc("SignalSema", Int32, Int32) },
                { 0x44, Svc("WaitSema", Int32, Int32) },
                { 0x45, Svc("PollSema", Int32, Int32) },
                { 0x47, Svc("ReferSemaStatus", Int32, Int32, ptrVoid32) },

                // --- OSD Config & System Info ---
                { 0x4A, Svc("SetOsdConfigParam", null, ptrVoid32) },
                { 0x4B, Svc("GetOsdConfigParam", null, ptrVoid32) },
                { 0x4C, Svc("GetGsHParam", ptrVoid32) },
                { 0x4D, Svc("GetGsVParam", ptrVoid32) },
                { 0x64, Svc("FlushCache", null, Int32) },
                { 0x75, Svc("print", null, ptrStr32) },
                { 0x7B, Svc("ExecOSD", Int32, Int32, ptrPtrStr32) },
                { 0x7E, Svc("MachineType", Int32) },
                { 0x7F, Svc("GetMemorySize", Int32) },

                // --- Negative (Interrupt-Safe i-functions) ---
                // Note: In an emulator interpreter, these usually bitmask out the sign bit or handle separately
                { unchecked((int)0xFFFF_FFDA), Svc("iEnableIntc", Int32, Int32) }, // -0x26 / -38
                { unchecked((int)0xFFFF_FFD9), Svc("iDisableIntc", Int32, Int32) },
                { unchecked((int)0xFFFF_FFBD), Svc("iSignalSema", Int32, Int32) } // -0x43 / -67
            };


            return dict;
        }
    }
}
