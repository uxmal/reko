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

using Reko.Arch.X86.Emulator;
using Reko.Arch.X86.Rewriter;
using Reko.Core;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Arch.X86
{
    using Decoder = Decoder<X86Disassembler, Mnemonic, X86Instruction>;

    public abstract class ProcessorMode
    {
        public static readonly ProcessorMode Real = new RealMode();
        public static readonly ProcessorMode ProtectedSegmented = new SegmentedMode();
        public static readonly ProcessorMode Protected32 = new FlatMode32();
        public static readonly ProcessorMode Protected64 = new FlatMode64();

        protected RegisterStorage[] controlRegs;
        protected RegisterStorage[] debugRegs;

        protected ProcessorMode(PrimitiveType wordSize, PrimitiveType framePointerType, PrimitiveType pointerType, bool isProtected)
        {
            this.WordWidth = wordSize;
            this.FramePointerType = framePointerType;
            this.PointerType = pointerType;
            this.IsProtected = isProtected; 
            this.controlRegs = Enumerable.Range(0, 9)
                .Select(n => new RegisterStorage($"cr{n}", Registers.ControlRegisterMin, 0, PrimitiveType.Word32))
                .ToArray();
            this.debugRegs = Enumerable.Range(0, 8)
               .Select(n => new RegisterStorage($"dr{n}", Registers.DebugRegisterMin, 0, PrimitiveType.Word32))
               .ToArray();
        }

        public virtual Address? MakeAddressFromSegOffset(X86State state, RegisterStorage seg, uint offset)
        {
            return state.AddressFromSegOffset(seg, offset);
        }

        public bool IsProtected { get; }

        public PrimitiveType FramePointerType { get; private set ; }

        public PrimitiveType PointerType { get; private set; }

        public PrimitiveType WordWidth { get; private set; }

        public virtual RegisterStorage StackRegister
        {
            get { return Registers.sp; }
        }

        public abstract X86Disassembler CreateDisassembler(IServiceProvider services, Decoder[] rootDecoders, EndianImageReader rdr, Dictionary<string, object> options);

        public abstract IProcessorEmulator CreateEmulator(IntelArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator);

        public abstract IEnumerable<Address> CreateInstructionScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags);

        public abstract OperandRewriter CreateOperandRewriter(IntelArchitecture arch, ExpressionEmitter m, IStorageBinder binder, IRewriterHost host, X86State state);

        public abstract Decoder[] CreateRootDecoders(Dictionary<string, object> options);

        public abstract Address? CreateSegmentedAddress(ushort seg, uint offset);

        public virtual Expression CreateStackAccess(IStorageBinder binder, int offset, DataType dataType)
        {
            var sp = binder.EnsureRegister(Registers.sp);
            var ss = binder.EnsureRegister(Registers.ss);
            var spOffset = MemoryAccess.CreateEffectiveAddress(sp, offset);
            var ea = new SegmentedPointer(this.PointerType, ss, spOffset);
            return new MemoryAccess(MemoryStorage.GlobalMemory, ea, dataType);
        }

        public RegisterStorage? GetControlRegister(int n)
        {
            if (0 <= n && n < controlRegs.Length)
                return controlRegs[n];
            else
                return null;
        }

        public RegisterStorage? GetDebugRegister(int n)
        {
            if (0 <= n && n < debugRegs.Length)
                return debugRegs[n];
            else
                return null;
        }

        public virtual List<RtlInstruction>? InlineCall(IServiceProvider services, X86Disassembler dasm, Address addrCallee, Address addrContinuation, IStorageBinder binder)
        {
            return null;
        }

        public abstract Address MakeAddressFromConstant(Constant c);

        public abstract bool TryReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState? state, out Address addr);

        protected bool TryReadSegmentedCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState? state, out Address addr)
        {
            if (byteSize == PrimitiveType.Word16.Size)
            {
                if (state is not null && rdr.TryReadLeUInt16(out ushort uOffset))
                {
                    addr = CreateSegmentedAddress(state.GetRegister(Registers.cs).ToUInt16(), uOffset)!.Value;
                    return true;
                }
            }
            else
            {
                if (rdr.TryReadLeUInt16(out var off) && rdr.TryReadLeUInt16(out var seg))
                {
                    addr = CreateSegmentedAddress(seg, off)!.Value;
                    return true;
                }
            }
            addr = default!;
            return false;
        }

        public abstract bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr);

        public bool TryParseSegmentedAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            if (txtAddress is not null)
            {
                int c = txtAddress.IndexOf(':');
                if (c > 0)
                {
                    try
                    {
                        addr = CreateSegmentedAddress(
                            Convert.ToUInt16(txtAddress[0..c], 16),
                            Convert.ToUInt32(txtAddress[(c + 1)..], 16))!.Value;
                        return true;
                    }
                    catch { }
                }
                if (txtAddress.Length > 0)
                {
                    bool isAllZeros = true;
                    for (int i = 0; isAllZeros && i < txtAddress.Length; ++i)
                    {
                        isAllZeros &= txtAddress[i] == '0';
                    }
                    if (isAllZeros)
                    {
                        addr = CreateSegmentedAddress(0, 0)!.Value;
                        return true;
                    }
                }
            }
            addr = default!;
            return false;
        }
    }

    internal class RealMode : ProcessorMode
    {
        public RealMode()
            : base(PrimitiveType.Word16, PrimitiveType.Offset16, PrimitiveType.SegPtr32, false)
        {
        }

        public override Decoder[] CreateRootDecoders(Dictionary<string, object> options)
        {
            var isa = X86Disassembler.InstructionSet.Create(false, false, options);
            return isa.CreateRootDecoders();
        }

        public override IEnumerable<Address> CreateInstructionScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            return new X86RealModePointerScanner(rdr, knownLinAddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
        }

        public override X86Disassembler CreateDisassembler(IServiceProvider services, Decoder[] rootDecoders, EndianImageReader rdr, Dictionary<string, object> options)
        {
            var dasm = new X86Disassembler(services, rootDecoders, this, rdr, PrimitiveType.Word16, PrimitiveType.Word16);
            if (!options.ContainsKey("Emulate8087") || (string) options["Emulate8087"] == "true")
            {
                dasm.Emulate8087 = true;
            }
            return dasm;
        }

        public override IProcessorEmulator CreateEmulator(IntelArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            return new X86RealModeEmulator(arch, segmentMap, envEmulator);
        }

        public override OperandRewriter CreateOperandRewriter(IntelArchitecture arch, ExpressionEmitter m, IStorageBinder binder, IRewriterHost host, X86State state)
        {
            return new OperandRewriter16(arch, m, binder, host, state);
        }

        public override Address? CreateSegmentedAddress(ushort seg, uint offset)
        {
            return Address.SegPtr(seg, offset);
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            var uAddr = c.ToUInt32();
            var off = (ushort) uAddr;
            var seg = (ushort) (uAddr >> 16);
            return Address.SegPtr(seg, off);
        }

        public override bool TryReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState? state, out Address addr)
        {
            return TryReadSegmentedCodeAddress(byteSize, rdr, state, out addr);
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return TryParseSegmentedAddress(txtAddress, out addr);
        }
    }

    internal class SegmentedMode : ProcessorMode
    {
        public SegmentedMode()
            : base(PrimitiveType.Word16, PrimitiveType.Offset16, PrimitiveType.SegPtr32, true)
        {
        }

        public override X86Disassembler CreateDisassembler(IServiceProvider services, Decoder[] rootDecoders, EndianImageReader rdr, Dictionary<string,object> options)
        {
            return new X86Disassembler(services, rootDecoders, this, rdr, PrimitiveType.Word16, PrimitiveType.Word16);
        }

        public override IProcessorEmulator CreateEmulator(IntelArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreateInstructionScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            return new X86RealModePointerScanner(rdr, knownLinAddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
        }

        public override OperandRewriter CreateOperandRewriter(IntelArchitecture arch, ExpressionEmitter m, IStorageBinder binder, IRewriterHost host, X86State state)
        {
            return new OperandRewriter16(arch, m, binder, host, state);
        }

        public override Decoder[] CreateRootDecoders(Dictionary<string, object> options)
        {
            var isa = X86Disassembler.InstructionSet.Create(false, false, options);
            return isa.CreateRootDecoders();
        }

        public override Address? CreateSegmentedAddress(ushort seg, uint offset)
        {
            return Address.ProtectedSegPtr(seg, offset);
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            throw new NotSupportedException("Must pass segment:Offset to make a segmented address.");
        }

        public override bool TryReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState? state, out Address addr)
        {
            return TryReadSegmentedCodeAddress(byteSize, rdr, state, out addr);
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return TryParseSegmentedAddress(txtAddress, out addr);
        }
    }

    internal class FlatMode32 : ProcessorMode
    {
        internal FlatMode32()
            : base(PrimitiveType.Word32, PrimitiveType.Ptr32, PrimitiveType.Ptr32, true)
        {
        }

        public override RegisterStorage StackRegister
        {
            get { return Registers.esp; }
        }

        public override List<RtlInstruction>? InlineCall(
            IServiceProvider services,
            X86Disassembler dasm,
            Address addrCallee,
            Address addrContinuation,
            IStorageBinder binder)
        {
            var instrs = dasm.Take(2).ToArray();
            if (instrs.Length < 2)
                return null;
            // Detect the pattern
            //   mov <reg>,[esp+0]
            //   ret
            // which is used by i386 ELF binaries to capture
            // the value in the EIP register.

            if (instrs[0].Mnemonic == Mnemonic.mov && 
                instrs[1].Mnemonic == Mnemonic.ret)
            {
                if (instrs[0].Operands[1] is not MemoryOperand mop)
                    return null;
                if (mop.Base != StackRegister)
                    return null;
                if (mop.Offset is not null && mop.Offset.IsValid && !mop.Offset.IsIntegerZero)
                    return null;
                if (mop.Index is not null && mop.Index != RegisterStorage.None)
                    return null;

                if (instrs[1].Operands.Length > 0)
                    return null; 
                var reg = binder.EnsureRegister((RegisterStorage)instrs[0].Operands[0]);
                var rtls = new List<RtlInstruction>();
                var m = new RtlEmitter(rtls);
                m.Assign(reg, addrContinuation);
                return rtls;
            }
            return null;
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr32((uint)c.ToUInt64());
        }

        public override Address? MakeAddressFromSegOffset(X86State state, RegisterStorage seg, uint offset)
        {
            return Address.Ptr32(offset);
        }

        public override IEnumerable<Address> CreateInstructionScanner(
            SegmentMap map,
            EndianImageReader rdr,
            IEnumerable<Address> knownAddresses,
            PointerScannerFlags flags)
        {
            var knownLinaddresses = knownAddresses.Select(a => a.ToUInt32()).ToHashSet();
            return new X86PointerScanner32(rdr, knownLinaddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
        }

        public override X86Disassembler CreateDisassembler(IServiceProvider services, Decoder[] rootDecoders, EndianImageReader rdr, Dictionary<string, object> options)
        {
            return new X86Disassembler(services, rootDecoders, this, rdr, PrimitiveType.Word32, PrimitiveType.Word32);
        }

        public override IProcessorEmulator CreateEmulator(IntelArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            return new X86Protected32Emulator(arch, segmentMap, envEmulator);
        }

        public override OperandRewriter CreateOperandRewriter(IntelArchitecture arch, ExpressionEmitter m, IStorageBinder binder, IRewriterHost host, X86State state)
        {
            return new OperandRewriter32(arch, m, binder, host, state);
        }

        public override Decoder[] CreateRootDecoders(Dictionary<string, object> options)
        {
            var isa = X86Disassembler.InstructionSet.Create(false, false, options);
            return isa.CreateRootDecoders();
        }

        public override Address? CreateSegmentedAddress(ushort seg, uint offset)
        {
            return Address.ProtectedSegPtr(seg, offset);
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int offset, DataType dataType)
        {
            var esp = binder.EnsureRegister(Registers.esp);
            return MemoryAccess.Create(esp, offset, dataType);
        }

        public override bool TryReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState? state, out Address addr)
        {
            if (rdr.TryReadLeUInt32(out uint uAddr))
            {
                addr = Address.Ptr32(uAddr);
                return true;
            }
            else
            {
                addr = default!;
                return false;
            }
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse32(txtAddress, out addr);
        }
    }

    internal class FlatMode64 : ProcessorMode
    {
        internal FlatMode64()
            : base(PrimitiveType.Word64, PrimitiveType.Ptr64, PrimitiveType.Ptr64, true)
        {
            this.controlRegs = Enumerable.Range(0, 9)
                .Select(n => new RegisterStorage($"cr{n}", Registers.ControlRegisterMin, 0, PrimitiveType.Word64))
                .ToArray();
            this.debugRegs = Enumerable.Range(0, 8)
               .Select(n => new RegisterStorage($"dr{n}", Registers.DebugRegisterMin, 0, PrimitiveType.Word64))
               .ToArray();
        }

        public override RegisterStorage StackRegister
        {
            get { return Registers.rsp; }
        }

        public override Address MakeAddressFromConstant(Constant c)
        {
            return Address.Ptr64(c.ToUInt64());
        }

        public override Address? MakeAddressFromSegOffset(X86State state, RegisterStorage seg, uint offset)
        {
            return Address.Ptr64(offset);
        }

        public override X86Disassembler CreateDisassembler(IServiceProvider services, Decoder[] rootDecoders, EndianImageReader rdr, Dictionary<string, object> options)
        {
            return new X86Disassembler(services, rootDecoders, this, rdr, PrimitiveType.Word32, PrimitiveType.Word64);
        }

        public override IProcessorEmulator CreateEmulator(IntelArchitecture arch, SegmentMap segmentMap, IPlatformEmulator envEmulator)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Address> CreateInstructionScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            var knownLinAddresses = knownAddresses.Select(a => (ulong)a.ToLinear()).ToHashSet();
            return new X86PointerScanner64(rdr, knownLinAddresses, flags).Select(li => map.MapLinearAddressToAddress(li));
        }

        public override OperandRewriter CreateOperandRewriter(IntelArchitecture arch, ExpressionEmitter m, IStorageBinder binder, IRewriterHost host, X86State state)
        {
            return new OperandRewriter64(arch, m, binder, host, state);
        }

        public override Decoder[] CreateRootDecoders(Dictionary<string, object> options)
        {
            var isa = X86Disassembler.InstructionSet.Create(true, true, options);
            return isa.CreateRootDecoders();
        }

        public override Address? CreateSegmentedAddress(ushort seg, uint offset)
        {
            throw new NotSupportedException("Segmented addresses are not supported in 64-bit protected mode.");
        }

        public override Expression CreateStackAccess(IStorageBinder binder, int offset, DataType dataType)
        {
            var rsp = binder.EnsureRegister(Registers.rsp);
            return MemoryAccess.Create(rsp, offset, dataType);
        }

        public override bool TryReadCodeAddress(int byteSize, EndianImageReader rdr, ProcessorState? state, out Address addr)
        {
            if (rdr.TryReadLeUInt64(out ulong uAddr))
            {
                addr = Address.Ptr64(uAddr);
                return true;
            }
            else
            {
                addr = default;
                return false;
            }
        }

        public override bool TryParseAddress(string? txtAddress, [MaybeNullWhen(false)] out Address addr)
        {
            return Address.TryParse64(txtAddress, out addr);
        }
    }
}