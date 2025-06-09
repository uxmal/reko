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
        public PrimitiveType DoubleWordSignedInteger { get; private set; }
        public PrimitiveType DoubleWordWidth { get; private set; }
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
            var dasm = new RiscVDisassembler(this, decoders!, imageReader);
            return new LongConstantFuser(dasm);
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

        public static bool Is64Bit(Dictionary<string, object> options)
        {
            return
                !options.TryGetValue(ProcessorOption.WordSize, out var oWordSize) ||
                int.TryParse(oWordSize.ToString(), out var wordSize) &&
                wordSize == 64;
        }

        public override void LoadUserOptions(Dictionary<string, object>? options)
        {
            if (options is null)
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

        public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, object> SaveUserOptions()
        {
            return Options;
        }

        private void SetOptionDependentProperties()
        {
            if (Is64Bit(this.Options))
            {
                this.PointerType = PrimitiveType.Ptr64;
                this.WordWidth = PrimitiveType.Word64;
                this.FramePointerType = PrimitiveType.Ptr64;
                this.NaturalSignedInteger = PrimitiveType.Int64;
                this.DoubleWordWidth = PrimitiveType.Word128;
                this.DoubleWordSignedInteger = PrimitiveType.Int128;
            }
            else
            {
                this.PointerType = PrimitiveType.Ptr32;
                this.WordWidth = PrimitiveType.Word32;
                this.FramePointerType = PrimitiveType.Ptr32;
                this.NaturalSignedInteger = PrimitiveType.Int32;
                this.DoubleWordWidth = PrimitiveType.Word64;
                this.DoubleWordSignedInteger = PrimitiveType.Int64;

            }

            this.GpRegs = regnames
                .Select((n, i) => new RegisterStorage(
                    n,
                    i,
                    0,
                    WordWidth))
                .ToArray();

            if (Options.TryGetValue("FloatAbi", out object? oFloatAbi) &&
                ProcessorOption.TryParseNumericOption(oFloatAbi, out int floatAbi))
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
            // Unprivileged Floating-Point CSRs
            DefineCsr(0x001, "fflags"); // URW Floating - Point Accrued Exceptions.
            DefineCsr(0x002, "frm"); // URW Floating - Point Dynamic Rounding Mode.
            DefineCsr(0x003, "fcsr"); // URW Floating - Point Control and Status Register(frm + fflags).

            // User Counter / Timers
            // Unprivileged Counter/Timers
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
            // Supervisor Trap Setup

            DefineCsr(0x100, "sstatus"); // SRW Supervisor status register.
            DefineCsr(0x102, "sedeleg"); // SRW Supervisor exception delegation register.
            DefineCsr(0x103, "sideleg"); // SRW Supervisor interrupt delegation register.
            DefineCsr(0x104, "sie"); // SRW Supervisor interrupt - enable register.
            DefineCsr(0x105, "stvec"); // SRW Supervisor trap handler base address.
            DefineCsr(0x106, "scounteren"); // SRW Supervisor counter enable.
            DefineCsr(0x121, "stimecmp"); // SRW Wall-clock timer compare value.

            // Supervisor Counter/Timers
            DefineCsr(0xD00, "scycle"); // SRO Supervisor cycle counter.
            // DefineCsr(0xD01, "stime"); // SRO Supervisor wall-clock time.
            DefineCsr(0xD02, "sinstret"); // SRO Supervisor instructions-retired counter.
            DefineCsr(0xD80, "scycleh"); // SRO Upper 32 bits of scycle, RV32I only.
            // DefineCsr(0xD81, "stimeh"); // SRO Upper 32 bits of stime, RV32I only.
            DefineCsr(0xD82, "sinstreth"); // SRO Upper 32 bits of sinstret, RV32I only.

            // Supervisor Timer
            DefineCsr(0xD01, "stime"); // SRO Supervisor wall-clock time register.
            DefineCsr(0xD81, "stimeh"); // SRO Upper 32 bits of stime, RV32I only.

            // Supervisor Configuration
            DefineCsr(0x10A, "senvcfg"); // SRW Supervisor environment configuration register.

            // Supervisor Trap Handling
            DefineCsr(0x140, "sscratch"); // SRW Scratch register for supervisor trap handlers.
            DefineCsr(0x141, "sepc"); // SRW Supervisor exception program counter.
            DefineCsr(0x142, "scause"); // SRW Supervisor trap cause.
            DefineCsr(0x143, "stval"); // SRW Supervisor bad address or instruction.
            DefineCsr(0x144, "sip"); // SRW Supervisor interrupt pending.

            // Supervisor Protection and Translation
            // DefineCsr(0x180, "sptbr"); // SRW Page-table base register.
            DefineCsr(0x180, "satp"); // SRW Supervisor address translation and protection.
            DefineCsr(0x181, "sasid"); // SRW Address-space ID.

            // Supervisor Read/Write Shadow of User Read-Only registers
            DefineCsr(0x900, "cyclew"); // SRW Cycle counter for RDCYCLE instruction.
            DefineCsr(0x901, "timew"); // SRW Timer for RDTIME instruction.
            DefineCsr(0x902, "instretw"); // SRW Instructions-retired counter for RDINSTRET instruction.
            DefineCsr(0x980, "cyclehw"); // SRW Upper 32 bits of cycle, RV32I only.
            DefineCsr(0x981, "timehw"); // SRW Upper 32 bits of time, RV32I only.
            DefineCsr(0x982, "instrethw"); // SRW Upper 32 bits of instret, RV32I only.

            // Hypervisor Trap Setup
            // DefineCsr(0x200, "hstatus"); // HRW Hypervisor status register.
            // DefineCsr(0x202, "hedeleg"); // HRW Hypervisor exception delegation register.
            DefineCsr(0x203, "hideleg"); // HRW Hypervisor interrupt delegation register.
            // DefineCsr(0x204, "hie"); // HRW Hypervisor interrupt-enable register.
            // DefineCsr(0x205, "htvec"); // HRW Hypervisor trap handler base address.

            // Hypervisor Trap Setup
            // DefineCsr(0x200, "hstatus"); // HRW Hypervisor status register.
            DefineCsr(0x201, "htvec"); // HRW Hypervisor trap handler base address.
            DefineCsr(0x202, "htdeleg"); // HRW Hypervisor trap delegation register.
            DefineCsr(0x221, "htimecmp"); // HRW Hypervisor wall-clock timer compare value.

            // Debug / Trace Registers
            DefineCsr(0x5A8, "scontext"); // SRW Supervisor - mode context register.

            // Hypervisor level registers
            // Hypervisor Trap Setup
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

            // Hypervisor Configuration
            DefineCsr(0x60A, "henvcfg"); // HRW Hypervisor environment configuration register.
            DefineCsr(0x61A, "henvcfgh"); // HRM Upper 32 bits of henvcfg, RV32 only.

            // Hypervisor Protection and Translation
            DefineCsr(0x680, "hgatp"); // HRW Hypervisor guest address translation and protection.

            // Debug/Trace Registers
            DefineCsr(0x6A8, "hcontext"); // HRW Hypervisor-mode context register.

            // Hypervisor Counter/Timer Virtualization Registers
            DefineCsr(0x605, "htimedelta"); // HRW Delta for VS/VU-mode timer.
            DefineCsr(0x615, "htimedeltah"); // HRW Upper 32 bits of htimedelta, RV32 only.

            // Hypervisor Trap Handling
            // DefineCsr(0x240, "hscratch"); // HRW Scratch register for hypervisor trap handlers.
            // DefineCsr(0x241, "hepc"); // HRW Hypervisor exception program counter.
            // DefineCsr(0x242, "hcause"); // HRW Hypervisor trap cause.
            // DefineCsr(0x243, "hbadaddr"); // HRW Hypervisor bad address.

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

            // Hypervisor Counter/Timers
            DefineCsr(0xE00, "hcycle"); // HRO Hypervisor cycle counter.
            DefineCsr(0xE01, "htime"); // HRO Hypervisor wall-clock time.
            DefineCsr(0xE02, "hinstret"); // HRO Hypervisor instructions-retired counter.
            DefineCsr(0xE80, "hcycleh"); // HRO Upper 32 bits of hcycle, RV32I only.
            DefineCsr(0xE81, "htimeh"); // HRO Upper 32 bits of htime, RV32I only.
            DefineCsr(0xE82, "hinstreth"); // HRO Upper 32 bits of hinstret, RV32I only.

            // Hypervisor Read/Write Shadow of Supervisor Read-Only Registers
            DefineCsr(0xA01, "stimew"); // HRW Supervisor wall-clock timer.
            DefineCsr(0xA81, "stimehw"); // HRW Upper 32 bits of supervisor wall-clock timer, RV32I only.

            // Machine Information Registers
            DefineCsr(0xF00, "mcpuid"); // MRO CPU description.
            DefineCsr(0xF01, "mimpid"); // MRO Vendor ID and version number.
            DefineCsr(0xF10, "mhartid"); // MRO Hardware thread ID.

            // Machine level registers
            // Machine Information Registers
            // DefineCsr(0xF10, "misa"); // MRO ISA and extensions supported.
            DefineCsr(0xF11, "mvendorid"); // MRO Vendor ID.
            DefineCsr(0xF12, "marchid"); // MRO Architecture ID.
            DefineCsr(0xF13, "mimpid"); // MRO Implementation ID.
            DefineCsr(0xF14, "mhartid"); // MRO Hardware thread ID.
            DefineCsr(0xF15, "mconfigptr"); // MRO Pointer to configuration data structure.

            // Machine Trap Setup
            // Machine Information Registers
            // DefineCsr(0x300, "mstatus"); // MRW Machine status register.
            // DefineCsr(0x301, "mtvec"); // MRW Machine trap-handler base address.
            // DefineCsr(0x302, "mtdeleg"); // MRW Machine trap delegation register.
            // DefineCsr(0x304, "mie"); // MRW Machine interrupt-enable register.
            DefineCsr(0x321, "mtimecmp"); // MRW Machine wall-clock timer compare value.

            DefineCsr(0x300, "mstatus"); // MRW Machine status register.
            DefineCsr(0x301, "misa");//  MRW ISA and extensions.
            DefineCsr(0x302, "medeleg"); // MRW Machine exception delegation register.
            DefineCsr(0x303, "mideleg"); // MRW Machine interrupt delegation register.
            DefineCsr(0x304, "mie"); // MRW Machine interrupt-enable register.
            DefineCsr(0x305, "mtvec"); // MRW Machine trap-handler base address.
            DefineCsr(0x306, "mcounteren"); // MRW Machine counter enable.
            DefineCsr(0x310, "mstatush"); // MRW Additional machine status register, RV32 only.

            // Machine Timers and Counters
            // DefineCsr(0x701, "mtime"); // MRW Machine wall-clock time.
            // DefineCsr(0x741, "mtimeh"); // MRW Upper 32 bits of mtime, RV32I only.

            // Machine Trap Handling
            DefineCsr(0x340, "mscratch"); // MRW Scratch register for machine trap handlers.
            DefineCsr(0x341, "mepc"); // MRW Machine exception program counter.
            DefineCsr(0x342, "mcause"); // MRW Machine trap cause.
            DefineCsr(0x343, "mtval"); // MRW Machine bad address or instruction.
            //$ TODO: OLD NAME? DefineCsr(0x343, "mbadaddr"); // MRW Machine bad address or instruction.
            DefineCsr(0x344, "mip"); // MRW Machine interrupt pending.
            DefineCsr(0x34A, "mtinst"); // MRW Machine trap instruction (transformed).
            DefineCsr(0x34B, "mtval2"); // MRW Machine bad guest physical address.

            // Machine Protection and Translation
            DefineCsr(0x380, "mbase"); // MRW Base register.
            DefineCsr(0x381, "mbound"); // MRW Bound register.
            DefineCsr(0x382, "mibase"); // MRW Instruction base register.
            DefineCsr(0x383, "mibound"); // MRW Instruction bound register.
            DefineCsr(0x384, "mdbase"); // MRW Data base register.
            DefineCsr(0x385, "mdbound"); // MRW Data bound register.

            // Machine Timers and Counters
            // DefineCsr(0xF00, "mcycle"); // MRO Machine cycle counter.
            // DefineCsr(0xF01, "mtime"); // MRO Machine wall-clock time.
            DefineCsr(0xF02, "minstret"); // MRO Machine instructions-retired counter.
            DefineCsr(0xF80, "mcycleh"); // MRO Upper 32 bits of mcycle, RV32I only.
            DefineCsr(0xF81, "mtimeh"); // MRO Upper 32 bits of mtime, RV32I only.
            DefineCsr(0xF82, "minstreth"); // MRO Upper 32 bits of minstret, RV32I only.

            // Machine Counter Setup
            // DefineCsr(0x310, "mucounteren"); // MRW User-mode counter enable.
            DefineCsr(0x311, "mscounteren"); // MRW Supervisor-mode counter enable.
            DefineCsr(0x312, "mhcounteren"); // MRW Hypervisor-mode counter enable.

            // Table 2.2: Currently allocated RISC-V user-level CSR addresses.
            // Table 2.3: Currently allocated RISC-V supervisor-level CSR addresses.
            // Table 2.4: Currently allocated RISC-V hypervisor-level CSR addresses.
            // Table 2.5: Currently allocated RISC-V machine-level CSR addresses.
            // Table 2.6: Currently allocated RISC-V machine-level CSR addresses.

            // Machine Counter-Delta Registers
            DefineCsr(0x700, "mucycle"); // MRW delta cycle counter delta.
            DefineCsr(0x701, "mutime"); // MRW delta time counter delta.
            DefineCsr(0x702, "muinstret"); // MRW delta instret counter delta.
            DefineCsr(0x704, "mscycle"); // MRW delta scycle counter delta.
            DefineCsr(0x705, "mstime"); // MRW delta stime counter delta.
            DefineCsr(0x706, "msinstret"); // MRW delta sinstret counter delta.
            DefineCsr(0x708, "mhcycle"); // MRW delta hcycle counter delta.
            DefineCsr(0x709, "mhtime"); // MRW delta htime counter delta.
            DefineCsr(0x70A, "mhinstret"); // MRW delta hinstret counter delta.
            DefineCsr(0x780, "mucycle"); // MRW deltah Upper 32 bits of cycle counter delta, RV32I only.
            DefineCsr(0x781, "mutime"); // MRW deltah Upper 32 bits of time counter delta, RV32I only.
            DefineCsr(0x782, "muinstret"); // MRW deltah Upper 32 bits of instret counter delta, RV32I only.
            DefineCsr(0x784, "mscycle"); // MRW deltah Upper 32 bits of scycle counter delta, RV32I only.
            DefineCsr(0x785, "mstime"); // MRW deltah Upper 32 bits of stime counter delta, RV32I only.
            DefineCsr(0x786, "msinstret"); // MRW deltah Upper 32 bits of sinstret counter delta, RV32I only.
            DefineCsr(0x788, "mhcycle"); // MRW deltah Upper 32 bits of hcycle counter delta, RV32I only.
            DefineCsr(0x789, "mhtime"); // MRW deltah Upper 32 bits of htime counter delta, RV32I only.
            DefineCsr(0x78A, "mhinstret"); // MRW deltah Upper 32 bits of hinstret counter delta, RV32I only.

            // Machine Read-Write Shadow of Hypervisor Read-Only Registers
            DefineCsr(0xB01, "htimew"); // MRW Hypervisor wall-clock timer.
            DefineCsr(0xB81, "htimehw"); // MRW Upper 32 bits of hypervisor wall-clock timer, RV32I only.

            // Machine Host-Target Interface (Non-Standard Berkeley Extension)
            // DefineCsr(0x780, "mtohost"); // MRW Output register to host.
            // DefineCsr(0x781, "mfromhost"); // MRW Input register from host.

            // Machine Configuration
            DefineCsr(0x30A, "menvcfg"); // MRW Machine environment configuration register.
            DefineCsr(0x31A, "menvcfgh"); // MRW Upper 32 bits of menvcfg, RV32 only.
            DefineCsr(0x747, "mseccfg"); // MRW Machine security configuration register.
            DefineCsr(0x757, "mseccfgh"); // MRW Upper 32 bits of mseccfg, RV32 only.

            // Machine Memory Protection
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

            // Machine Non-Maskable Interrupt Handling
            DefineCsr(0x740, "mnscratch"); // MRW Resumable NMI scratch register.
            DefineCsr(0x741, "mnepc"); // MRW Resumable NMI program counter.
            DefineCsr(0x742, "mncause"); // MRW Resumable NMI cause.
            DefineCsr(0x744, "mnstatus"); // MRW Resumable NMI status.

            // Machine Counter/Timers
            DefineCsr(0xB00, "mcycle"); // MRW Machine cycle counter.
            DefineCsr(0xB02, "minstret"); // MRW Machine instructions-retired counter.
            DefineCsr(0xB03, "mhpmcounter3"); // MRW Machine performance-monitoring counter.
            DefineCsr(0xB04, "mhpmcounter4"); // MRW Machine performance-monitoring counter.
            DefineCsr(0xB05, "mhpmcounter5");
            DefineCsr(0xB06, "mhpmcounter6");
            DefineCsr(0xB07, "mhpmcounter7");
            DefineCsr(0xB08, "mhpmcounter8");
            DefineCsr(0xB09, "mhpmcounter9");
            DefineCsr(0xB0A, "mhpmcounter10");
            DefineCsr(0xB0B, "mhpmcounter11");
            DefineCsr(0xB0C, "mhpmcounter12");
            DefineCsr(0xB0D, "mhpmcounter13");
            DefineCsr(0xB0E, "mhpmcounter14");
            DefineCsr(0xB0F, "mhpmcounter15");
            DefineCsr(0xB10, "mhpmcounter16");
            DefineCsr(0xB11, "mhpmcounter17");
            DefineCsr(0xB12, "mhpmcounter18");
            DefineCsr(0xB13, "mhpmcounter19");
            DefineCsr(0xB14, "mhpmcounter20");
            DefineCsr(0xB15, "mhpmcounter21");
            DefineCsr(0xB16, "mhpmcounter22");
            DefineCsr(0xB17, "mhpmcounter23");
            DefineCsr(0xB18, "mhpmcounter24");
            DefineCsr(0xB19, "mhpmcounter25");
            DefineCsr(0xB1A, "mhpmcounter26");
            DefineCsr(0xB1B, "mhpmcounter27");
            DefineCsr(0xB1C, "mhpmcounter28");
            DefineCsr(0xB1D, "mhpmcounter29");
            DefineCsr(0xB1E, "mhpmcounter30");
            DefineCsr(0xB1F, "mhpmcounter31"); // MRW Machine performance-monitoring counter.
            DefineCsr(0xB80, "mcycleh"); // MRW Upper 32 bits of mcycle, RV32 only.
            DefineCsr(0xB82, "minstreth"); // MRW Upper 32 bits of minstret, RV32 only.
            DefineCsr(0xB83, "mhpmcounter3h"); // MRW Upper 32 bits of mhpmcounter3, RV32 only.
            DefineCsr(0xB84, "mhpmcounter4h"); // MRW Upper 32 bits of mhpmcounter4, RV32 only.
            DefineCsr(0xB85, "mhpmcounter5h");
            DefineCsr(0xB86, "mhpmcounter6h");
            DefineCsr(0xB87, "mhpmcounter7h");
            DefineCsr(0xB88, "mhpmcounter8h");
            DefineCsr(0xB89, "mhpmcounter9h");
            DefineCsr(0xB8A, "mhpmcounter10h");
            DefineCsr(0xB8B, "mhpmcounter11h");
            DefineCsr(0xB8C, "mhpmcounter12h");
            DefineCsr(0xB8D, "mhpmcounter13h");
            DefineCsr(0xB8E, "mhpmcounter14h");
            DefineCsr(0xB8F, "mhpmcounter15h");
            DefineCsr(0xB90, "mhpmcounter16h");
            DefineCsr(0xB91, "mhpmcounter17h");
            DefineCsr(0xB92, "mhpmcounter18h");
            DefineCsr(0xB93, "mhpmcounter19h");
            DefineCsr(0xB94, "mhpmcounter20h");
            DefineCsr(0xB95, "mhpmcounter21h");
            DefineCsr(0xB96, "mhpmcounter22h");
            DefineCsr(0xB97, "mhpmcounter23h");
            DefineCsr(0xB98, "mhpmcounter24h");
            DefineCsr(0xB99, "mhpmcounter25h");
            DefineCsr(0xB9A, "mhpmcounter26h");
            DefineCsr(0xB9B, "mhpmcounter27h");
            DefineCsr(0xB9C, "mhpmcounter28h");
            DefineCsr(0xB9D, "mhpmcounter29h");
            DefineCsr(0xB9E, "mhpmcounter30h");
            DefineCsr(0xB9F, "mhpmcounter31h"); // MRW Upper 32 bits of mhpmcounter31, RV32 only.

            // Machine Counter Setup
            DefineCsr(0x320, "mcountinhibit"); // MRW Machine counter-inhibit register.
            DefineCsr(0x323, "mhpmevent3"); // MRW Machine performance-monitoring event selector.
            DefineCsr(0x324, "mhpmevent4"); // MRW Machine performance-monitoring event selector.
            DefineCsr(0x325, "mhpmevent5");
            DefineCsr(0x326, "mhpmevent6");
            DefineCsr(0x327, "mhpmevent7");
            DefineCsr(0x328, "mhpmevent8");
            DefineCsr(0x329, "mhpmevent9");
            DefineCsr(0x32A, "mhpmevent10");
            DefineCsr(0x32B, "mhpmevent11");
            DefineCsr(0x32C, "mhpmevent12");
            DefineCsr(0x32D, "mhpmevent13");
            DefineCsr(0x32E, "mhpmevent14");
            DefineCsr(0x32F, "mhpmevent15");
            DefineCsr(0x330, "mhpmevent16");
            DefineCsr(0x331, "mhpmevent17");
            DefineCsr(0x332, "mhpmevent18");
            DefineCsr(0x333, "mhpmevent19");
            DefineCsr(0x334, "mhpmevent20");
            DefineCsr(0x335, "mhpmevent21");
            DefineCsr(0x336, "mhpmevent22");
            DefineCsr(0x337, "mhpmevent23");
            DefineCsr(0x338, "mhpmevent24");
            DefineCsr(0x339, "mhpmevent25");
            DefineCsr(0x33A, "mhpmevent26");
            DefineCsr(0x33B, "mhpmevent27");
            DefineCsr(0x33C, "mhpmevent28");
            DefineCsr(0x33D, "mhpmevent29");
            DefineCsr(0x33E, "mhpmevent30");
            DefineCsr(0x33F, "mhpmevent31"); // MRW Machine performance-monitoring event selector.

            // Debug/Trace Registers (shared with Debug Mode)
            DefineCsr(0x7A0, "tselect"); // MRW Debug/Trace trigger register select.
            DefineCsr(0x7A1, "tdata1"); // MRW First Debug/Trace trigger data register.
            DefineCsr(0x7A2, "tdata2"); // MRW Second Debug/Trace trigger data register.
            DefineCsr(0x7A3, "tdata3"); // MRW Third Debug/Trace trigger data register.
            DefineCsr(0x7A8, "mcontext"); // MRW Machine-mode context register.

            // Debug Mode Registers
            DefineCsr(0x7B0, "dcsr"); // DRW Debug control and status register.
            DefineCsr(0x7B1, "dpc"); // DRW Debug program counter.
            DefineCsr(0x7B2, "dscratch0"); // DRW Debug scratch register 0.
            DefineCsr(0x7B3, "dscratch1"); // DRW Debug scratch register 1.
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
