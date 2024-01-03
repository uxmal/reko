#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reko.Arch.RiscV
{
    using Decoder = Decoder<RiscVDisassembler, Mnemonic, RiscVInstruction>;

    public class RiscVArchitecture : ProcessorArchitecture
    {
        private static readonly string [] regnames = {
            "zero", "ra", "sp", "gp", "tp", "t0", "t1", "t2",
            "s0",   "s1", "a0", "a1", "a2", "a3", "a4", "a5",
            "a6",   "a7", "s2", "s3", "s4", "s5", "s6", "s7",
            "s8",   "s9", "s10","s11","t3", "t4", "t5", "t6"
        };

        private static readonly string[] fpuregnames = {
          "ft0", "ft1", "ft2",  "ft3",  "ft4", "ft5", "ft6",  "ft7",
          "fs0", "fs1", "fa0",  "fa1",  "fa2", "fa3", "fa4",  "fa5",
          "fa6", "fa7", "fs2",  "fs3",  "fs4", "fs5", "fs6",  "fs7",
          "fs8", "fs9", "fs10", "fs11", "ft8", "ft9", "ft10", "ft11"
        };

        private static readonly string[] predsuccNames =
        {
            "", "w", "r", "rw",     "o", "ow", "or", "orw",
            "i", "iw", "ir", "irw", "io", "iow", "ior", "iorw"
        };

        private static readonly string?[] roundingModes =
        {
            "rne", "rtz", "rdn", "rup", "rmm", null, null, "dyn"
        };


        private Decoder[]? decoders;

#nullable disable
        public RiscVArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
            : base(services, archId, options, null, null)
        {
            this.Endianness = EndianServices.Little;
            this.InstructionBitSize = 16;
            Csrs = new Dictionary<uint, RegisterStorage>();
            SetOptionDependentProperties();
        }
#nullable enable

        public Dictionary<uint, RegisterStorage> Csrs { get; }
        public RegisterStorage[] GpRegs { get; private set; }
        public RegisterStorage[] FpRegs { get; private set; }
        public RegisterStorage LinkRegister { get; private set; }
        public PrimitiveType NaturalSignedInteger { get; private set; }
        public RegisterStorage[] PredSuccRegs { get; private set; }
        public RegisterStorage?[] RoundingModes { get; private set; }

        /// <summary>
        /// The size of the return address (in bytes) if pushed on stack.
        /// </summary>
        /// <remarks>
        /// Return address is not pushed directly on a stack in memory on
        /// RISC-V.
        /// </remarks>
        public override int ReturnAddressOnStack => 0;

        public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        {
            return new RiscVDisassembler(this, decoders!, imageReader);
        }

        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
        {
            return new RiscVInstructionComparer(norm);
        }

        public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        {
            throw new NotImplementedException();
        }

        public override ProcessorState CreateProcessorState()
        {
            return new RiscVState(this);
        }

        public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            return new RiscVRewriter(this, decoders!, rdr, state, binder, host);
        }

        public override FlagGroupStorage GetFlagGroup(string name)
        {
            throw new NotImplementedException();
        }

        public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
        {
            throw new NotImplementedException();
        }

        public override SortedList<string, int> GetMnemonicNames()
        {
            throw new NotImplementedException();
        }

        public override int? GetMnemonicNumber(string name)
        {
            throw new NotImplementedException();
        }

        public RegisterStorage? GetRegister(int i)
        {
            if (regsByDomain is null)
                return null;
            return regsByDomain.TryGetValue((StorageDomain) i, out var reg)
                ? reg
                : null;
        }

        public override RegisterStorage[] GetRegisters()
        {
            return GpRegs;
        }

        public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        {
            return "";
        }

        private bool Is64Bit()
        {
            return
                !Options.TryGetValue("WordSize", out var oWordSize) ||
                int.TryParse(oWordSize.ToString(), out var wordSize) &&
                wordSize == 64;
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            if (options == null)
                return;
            foreach (var option in options)
            {
                this.Options[option.Key] = option.Value;
            }
            SetOptionDependentProperties();
        }

        public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        {
            if (this.WordWidth.BitSize == 32)
            {
                var uAddr = c.ToUInt32();
                if (codeAlign)
                    uAddr &= ~1u;
                return Address.Ptr32(uAddr);
            }
            else
            {
                var uAddr = c.ToUInt64();
                return Address.Ptr64(uAddr);
            }
        }

        public override Address ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, object> SaveUserOptions()
        {
            return Options;
        }

        private void SetOptionDependentProperties()
        {
            if (Is64Bit())
            {
                this.PointerType = PrimitiveType.Ptr64;
                this.WordWidth = PrimitiveType.Word64;
                this.FramePointerType = PrimitiveType.Ptr64;
                this.NaturalSignedInteger = PrimitiveType.Int64;
            }
            else
            {
                this.PointerType = PrimitiveType.Ptr32;
                this.WordWidth = PrimitiveType.Word32;
                this.FramePointerType = PrimitiveType.Ptr32;
                this.NaturalSignedInteger = PrimitiveType.Int32;
            }

            this.GpRegs = regnames
                .Select((n, i) => new RegisterStorage(
                    n,
                    i,
                    0,
                    WordWidth))
                .ToArray();

            if (Options.TryGetValue("FloatAbi", out object? oFloatAbi) &&
                oFloatAbi is int floatAbi)
            {
                this.FpRegs = CreateFpRegs(floatAbi);
            }
            else
            {
                this.FpRegs = Array.Empty<RegisterStorage>();
            }


            this.regsByDomain = 
                GpRegs
                .Concat(FpRegs)
                .ToDictionary(r => r.Domain);
            this.regsByName = regsByDomain.Values.ToDictionary(r => r.Name);
            this.LinkRegister = regsByDomain[(StorageDomain) 1];        // ra
            this.StackRegister = regsByDomain[(StorageDomain) 2];       // sp

            DefineCsrs();
            DefinePseudoRegisters();

            var isa = RiscVDisassembler.InstructionSet.Create(Options);
            this.decoders = isa.CreateRootDecoders();
        }

        private void DefineCsrs()
        {
            Csrs.Clear();

            // User level registers.
            DefineCsr(0x000, "ustatus"); // URW User status register.
            DefineCsr(0x004, "uie"); // URW User interrupt-enable register.
            DefineCsr(0x005, "utvec"); // URW User trap handler base address.
                                       //User Trap Handling
            DefineCsr(0x040, "uscratch"); // URW Scratch register for user trap handlers.
            DefineCsr(0x041, "uepc"); // URW User exception program counter.
            DefineCsr(0x042, "ucause"); // URW User trap cause.
            DefineCsr(0x043, "utval"); // URW User bad address or instruction.
            DefineCsr(0x044, "uip"); // URW User interrupt pending.
                                     // User Floating - Point CSRs
            DefineCsr(0x001, "fflags"); // URW Floating - Point Accrued Exceptions.
            DefineCsr(0x002, "frm"); // URW Floating - Point Dynamic Rounding Mode.
            DefineCsr(0x003, "fcsr"); // URW Floating - Point Control and Status Register(frm + fflags).
                                      // User Counter / Timers
            DefineCsr(0xC00, "cycle"); // URO Cycle counter for RDCYCLE instruction.
            DefineCsr(0xC01, "time"); // URO Timer for RDTIME instruction.
            DefineCsr(0xC02, "instret"); // URO Instructions - retired counter for RDINSTRET instruction.
            DefineCsr(0xC03, "hpmcounter3"); // URO Performance - monitoring counter.

            DefineCsr(0xC04, "hpmcounter4"); // URO Performance - monitoring counter.
            DefineCsr(0xC05, "hpmcounter5");
            DefineCsr(0xC06, "hpmcounter6");
            DefineCsr(0xC07, "hpmcounter7");

            DefineCsr(0xC08, "hpmcounter8");
            DefineCsr(0xC09, "hpmcounter9");
            DefineCsr(0xC0A, "hpmcounter10");
            DefineCsr(0xC0B, "hpmcounter11");

            DefineCsr(0xC0C, "hpmcounter12");
            DefineCsr(0xC0D, "hpmcounter13");
            DefineCsr(0xC0E, "hpmcounter14");
            DefineCsr(0xC0F, "hpmcounter15");

            DefineCsr(0xC10, "hpmcounter16");
            DefineCsr(0xC11, "hpmcounter17");
            DefineCsr(0xC12, "hpmcounter18");
            DefineCsr(0xC13, "hpmcounter19");

            DefineCsr(0xC14, "hpmcounter20");
            DefineCsr(0xC15, "hpmcounter21");
            DefineCsr(0xC16, "hpmcounter22");
            DefineCsr(0xC17, "hpmcounter23");

            DefineCsr(0xC18, "hpmcounter24");
            DefineCsr(0xC19, "hpmcounter25");
            DefineCsr(0xC1A, "hpmcounter26");
            DefineCsr(0xC1B, "hpmcounter27");

            DefineCsr(0xC1C, "hpmcounter28");
            DefineCsr(0xC1D, "hpmcounter29");
            DefineCsr(0xC1E, "hpmcounter30");
            DefineCsr(0xC1F, "hpmcounter31"); // URO Performance - monitoring counter.
            DefineCsr(0xC80, "cycleh"); // URO Upper 32 bits of cycle, RV32 only.
            DefineCsr(0xC81, "timeh"); // URO Upper 32 bits of time, RV32 only.
            DefineCsr(0xC82, "instreth"); // URO Upper 32 bits of instret, RV32 only.
            DefineCsr(0xC83, "hpmcounter3h"); // URO Upper 32 bits of hpmcounter3, RV32 only.
            DefineCsr(0xC84, "hpmcounter4h"); // URO Upper 32 bits of hpmcounter4, RV32 only.
            DefineCsr(0xC85, "hpmcounter5h");
            DefineCsr(0xC86, "hpmcounter6h");
            DefineCsr(0xC87, "hpmcounter7h");

            DefineCsr(0xC88, "hpmcounter8h");
            DefineCsr(0xC89, "hpmcounter9h");
            DefineCsr(0xC8A, "hpmcounter10h");
            DefineCsr(0xC8B, "hpmcounter11h");

            DefineCsr(0xC8C, "hpmcounter12h");
            DefineCsr(0xC8D, "hpmcounter13h");
            DefineCsr(0xC8E, "hpmcounter14h");
            DefineCsr(0xC8F, "hpmcounter15h");

            DefineCsr(0xC90, "hpmcounter16h");
            DefineCsr(0xC91, "hpmcounter17h");
            DefineCsr(0xC92, "hpmcounter18h");
            DefineCsr(0xC93, "hpmcounter19h");

            DefineCsr(0xC94, "hpmcounter20h");
            DefineCsr(0xC95, "hpmcounter21h");
            DefineCsr(0xC96, "hpmcounter22h");
            DefineCsr(0xC97, "hpmcounter23h");

            DefineCsr(0xC98, "hpmcounter24h");
            DefineCsr(0xC99, "hpmcounter25h");
            DefineCsr(0xC9A, "hpmcounter26h");
            DefineCsr(0xC9B, "hpmcounter27h");

            DefineCsr(0xC9C, "hpmcounter28h");
            DefineCsr(0xC9D, "hpmcounter29h");
            DefineCsr(0xC9E, "hpmcounter30h");
            DefineCsr(0xC9F, "hpmcounter31h");

            // Supervisor level registers

            DefineCsr(0x100, "sstatus"); // SRW Supervisor status register.
            DefineCsr(0x102, "sedeleg"); // SRW Supervisor exception delegation register.
            DefineCsr(0x103, "sideleg"); // SRW Supervisor interrupt delegation register.
            DefineCsr(0x104, "sie"); // SRW Supervisor interrupt - enable register.
            DefineCsr(0x105, "stvec"); // SRW Supervisor trap handler base address.
            DefineCsr(0x106, "scounteren"); // SRW Supervisor counter enable.
                                            // Supervisor Trap Handling
            DefineCsr(0x140, "sscratch"); // SRW Scratch register for supervisor trap handlers.
            DefineCsr(0x141, "sepc"); // SRW Supervisor exception program counter.
            DefineCsr(0x142, "scause"); // SRW Supervisor trap cause.
            DefineCsr(0x143, "stval"); // SRW Supervisor bad address or instruction.
            DefineCsr(0x144, "sip"); // SRW Supervisor interrupt pending.
                                     // Supervisor Protection and Translation
            DefineCsr(0x180, "satp"); // SRW Supervisor address translation and protection.
                                      // Debug / Trace Registers
            DefineCsr(0x5A8, "scontext"); // SRW Supervisor - mode context register.

            // Hypervisor level registers
            DefineCsr(0x600, "hstatus"); // HRW Hypervisor status register.
            DefineCsr(0x602, "hedeleg"); // HRW Hypervisor exception delegation register.
            DefineCsr(0x603, "hideleg"); // HRW Hypervisor interrupt delegation register.
            DefineCsr(0x604, "hie"); // HRW Hypervisor interrupt - enable register.
            DefineCsr(0x606, "hcounteren"); // HRW Hypervisor counter enable.
            DefineCsr(0x607, "hgeie"); // HRW Hypervisor guest external interrupt - enable register.
                                       // Hypervisor Trap Handling
            DefineCsr(0x643, "htval"); // HRW Hypervisor bad guest physical address.
            DefineCsr(0x644, "hip"); // HRW Hypervisor interrupt pending.
            DefineCsr(0x645, "hvip"); // HRW Hypervisor virtual interrupt pending.
            DefineCsr(0x64A, "htinst"); // HRW Hypervisor trap instruction (transformed).
            DefineCsr(0xE12, "hgeip"); // HRO Hypervisor guest external interrupt pending.
                                       // Hypervisor Protection and Translation
            DefineCsr(0x680, "hgatp"); // HRW Hypervisor guest address translation and protection.
                                       // Debug/Trace Registers
            DefineCsr(0x6A8, "hcontext"); // HRW Hypervisor-mode context register.
                                          // Hypervisor Counter/Timer Virtualization Registers
            DefineCsr(0x605, "htimedelta"); // HRW Delta for VS/VU-mode timer.
            DefineCsr(0x615, "htimedeltah"); // HRW Upper 32 bits of htimedelta, RV32 only.
                                             // Virtual Supervisor Registers
            DefineCsr(0x200, "vsstatus"); // HRW Virtual supervisor status register.
            DefineCsr(0x204, "vsie"); // HRW Virtual supervisor interrupt-enable register.
            DefineCsr(0x205, "vstvec"); // HRW Virtual supervisor trap handler base address.
            DefineCsr(0x240, "vsscratch"); // HRW Virtual supervisor scratch register.
            DefineCsr(0x241, "vsepc"); // HRW Virtual supervisor exception program counter.
            DefineCsr(0x242, "vscause"); // HRW Virtual supervisor trap cause.
            DefineCsr(0x243, "vstval"); // HRW Virtual supervisor bad address or instruction.
            DefineCsr(0x244, "vsip"); // HRW Virtual supervisor interrupt pending.
            DefineCsr(0x280, "vsatp"); // HRW Virtual supervisor address translation and protection

            // Machine level registers

            DefineCsr(0xF11, "mvendorid"); // MRO Vendor ID.
            DefineCsr(0xF12, "marchid"); // MRO Architecture ID.
            DefineCsr(0xF13, "mimpid"); // MRO Implementation ID.
            DefineCsr(0xF14, "mhartid"); // MRO Hardware thread ID.
                                         //Machine Trap Setup
            DefineCsr(0x300, "mstatus"); // MRW Machine status register.
            DefineCsr(0x301, "misa");//  MRW misa ISA and extensions.
            DefineCsr(0x302, "medeleg"); // MRW Machine exception delegation register.
            DefineCsr(0x303, "mideleg"); // MRW Machine interrupt delegation register.
            DefineCsr(0x304, "mie"); // MRW Machine interrupt-enable register.
            DefineCsr(0x305, "mtvec"); // MRW Machine trap-handler base address.
            DefineCsr(0x306, "mcounteren"); // MRW Machine counter enable.
            DefineCsr(0x310, "mstatush"); // MRW Additional machine status register, RV32 only.
                                          //Machine Trap Handling
            DefineCsr(0x340, "mscratch"); // MRW Scratch register for machine trap handlers.
            DefineCsr(0x341, "mepc"); // MRW Machine exception program counter.
            DefineCsr(0x342, "mcause"); // MRW Machine trap cause.
            DefineCsr(0x343, "mtval"); // MRW Machine bad address or instruction.
            DefineCsr(0x344, "mip"); // MRW Machine interrupt pending.
            DefineCsr(0x34A, "mtinst"); // MRW Machine trap instruction (transformed).
            DefineCsr(0x34B, "mtval2"); // MRW Machine bad guest physical address.
                                        //Machine Memory Protection
            DefineCsr(0x3A0, "pmpcfg0"); // MRW Physical memory protection configuration.
            DefineCsr(0x3A1, "pmpcfg1"); // MRW Physical memory protection configuration, RV32 only.
            DefineCsr(0x3A2, "pmpcfg2"); // MRW Physical memory protection configuration.
            DefineCsr(0x3A3, "pmpcfg3"); // MRW Physical memory protection configuration, RV32 only.

            DefineCsr(0x3A4, "pmpcfg4");
            DefineCsr(0x3A5, "pmpcfg5");
            DefineCsr(0x3A6, "pmpcfg6");
            DefineCsr(0x3A7, "pmpcfg7");
            DefineCsr(0x3A8, "pmpcfg8");
            DefineCsr(0x3A9, "pmpcfg9");
            DefineCsr(0x3AA, "pmpcfg10");
            DefineCsr(0x3AB, "pmpcfg11");
            DefineCsr(0x3AC, "pmpcfg12");
            DefineCsr(0x3AD, "pmpcfg13");
            DefineCsr(0x3AE, "pmpcfg14"); // MRW Physical memory protection configuration.
            DefineCsr(0x3AF, "pmpcfg15"); // MRW Physical memory protection configuration, RV32 only.

            DefineCsr(0x3B0, "pmpaddr0"); // MRW Physical memory protection address register.
            DefineCsr(0x3B1, "pmpaddr1"); // MRW Physical memory protection address register.
            DefineCsr(0x3B2, "pmpaddr2");
            DefineCsr(0x3B3, "pmpaddr3");

            DefineCsr(0x3B4, "pmpaddr4");
            DefineCsr(0x3B5, "pmpaddr5");
            DefineCsr(0x3B6, "pmpaddr6");
            DefineCsr(0x3B7, "pmpaddr7");
            DefineCsr(0x3B8, "pmpaddr8");
            DefineCsr(0x3B9, "pmpaddr9");
            DefineCsr(0x3BA, "pmpaddr10");
            DefineCsr(0x3BB, "pmpaddr11");
            DefineCsr(0x3BC, "pmpaddr12");
            DefineCsr(0x3BD, "pmpaddr13");
            DefineCsr(0x3BE, "pmpaddr14");
            DefineCsr(0x3BF, "pmpaddr15");
            DefineCsr(0x3C0, "pmpaddr16");
            DefineCsr(0x3C1, "pmpaddr17");
            DefineCsr(0x3C2, "pmpaddr18");
            DefineCsr(0x3C3, "pmpaddr19");
            DefineCsr(0x3C4, "pmpaddr20");
            DefineCsr(0x3C5, "pmpaddr21");
            DefineCsr(0x3C6, "pmpaddr22");
            DefineCsr(0x3C7, "pmpaddr23");
            DefineCsr(0x3C8, "pmpaddr24");
            DefineCsr(0x3C9, "pmpaddr25");
            DefineCsr(0x3CA, "pmpaddr26");
            DefineCsr(0x3CB, "pmpaddr27");
            DefineCsr(0x3CC, "pmpaddr28");
            DefineCsr(0x3CD, "pmpaddr29");
            DefineCsr(0x3CE, "pmpaddr30");
            DefineCsr(0x3CF, "pmpaddr31");

            DefineCsr(0x3D0, "pmpaddr32");
            DefineCsr(0x3D1, "pmpaddr33");
            DefineCsr(0x3D2, "pmpaddr34");
            DefineCsr(0x3D3, "pmpaddr35");
            DefineCsr(0x3D4, "pmpaddr36");
            DefineCsr(0x3D5, "pmpaddr37");
            DefineCsr(0x3D6, "pmpaddr38");
            DefineCsr(0x3D7, "pmpaddr39");
            DefineCsr(0x3D8, "pmpaddr40");
            DefineCsr(0x3D9, "pmpaddr41");
            DefineCsr(0x3DA, "pmpaddr42");
            DefineCsr(0x3DB, "pmpaddr43");
            DefineCsr(0x3DC, "pmpaddr44");
            DefineCsr(0x3DD, "pmpaddr45");
            DefineCsr(0x3DE, "pmpaddr46");
            DefineCsr(0x3DF, "pmpaddr47");
            DefineCsr(0x3E0, "pmpaddr48");
            DefineCsr(0x3E1, "pmpaddr49");
            DefineCsr(0x3E2, "pmpaddr50");
            DefineCsr(0x3E3, "pmpaddr51");
            DefineCsr(0x3E4, "pmpaddr52");
            DefineCsr(0x3E5, "pmpaddr53");
            DefineCsr(0x3E6, "pmpaddr54");
            DefineCsr(0x3E7, "pmpaddr55");
            DefineCsr(0x3E8, "pmpaddr56");
            DefineCsr(0x3E9, "pmpaddr57");
            DefineCsr(0x3EA, "pmpaddr58");
            DefineCsr(0x3EB, "pmpaddr59");
            DefineCsr(0x3EC, "pmpaddr60");
            DefineCsr(0x3ED, "pmpaddr61");
            DefineCsr(0x3EE, "pmpaddr62");
            DefineCsr(0x3EF, "pmpaddr63"); // MRW Physical memory protection address register.
        }

        private void DefineCsr(int number, string name)
        {
            var reg = new RegisterStorage(name, number + (int)StorageDomain.SystemRegister, 0, WordWidth);
            Csrs.Add((uint)number, reg);
        }

        private void DefinePseudoRegisters()
        {
            var factory = new StorageFactory(StorageDomain.PseudoRegister);

            this.PredSuccRegs = predsuccNames
                .Select(n => factory.Reg(n, PrimitiveType.Byte))
                 .ToArray();
            this.RoundingModes = roundingModes
                .Select(n => n is null ? null : factory.Reg(n, PrimitiveType.Byte))
                .ToArray();
        }
        private RegisterStorage[] CreateFpRegs(int floatAbi)
        {
            PrimitiveType fpType;
            switch (floatAbi)
            {
            case 32: fpType = PrimitiveType.Word32; break;
            case 64: fpType = PrimitiveType.Word64; break;
            case 128: fpType = PrimitiveType.Word128; break;
            default: fpType = WordWidth; break;
            }

            return fpuregnames
                .Select((n, i) => new RegisterStorage(
                    n,
                    i + 32,
                    0,
                    fpType))
                .ToArray();
        }

        public override bool TryGetRegister(string name, [MaybeNullWhen(false)] out RegisterStorage reg)
        {
            if (regsByName is null)
            {
                reg = null!;
                return false;
            }
            return regsByName.TryGetValue(name, out reg);
        }

        public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        {
            if (this.WordWidth.BitSize == 32)
                return Address.TryParse32(txtAddr, out addr);
            else
                return Address.TryParse64(txtAddr, out addr);
        }
    }
}
