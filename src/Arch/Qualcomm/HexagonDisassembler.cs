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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Reko.Arch.Qualcomm
{
    public class HexagonDisassembler : DisassemblerBase<HexagonPacket, Mnemonic>
    {
        private static readonly Decoder iclassDecoder;
        private static readonly RegisterStorage[] subInstrRegs;

        private readonly HexagonArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly List<HexagonInstruction> instrs;
        private readonly List<MachineOperand> ops;
        private Address addrInstr;

        public HexagonDisassembler(HexagonArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.instrs = new List<HexagonInstruction>();
            this.ops = new List<MachineOperand>();
        }

        public override HexagonPacket DisassembleInstruction()
        {
            instrs.Clear();
            var addr = rdr.Address;
            for (; ;)
            {
                if (!rdr.TryReadLeUInt32(out uint uInstr))
                {
                    if (instrs.Count > 0)
                    {
                        // Packet should have been properly terminated.
                        return MakeInvalidPacket(addr);
                    }
                    else
                    {
                        return null;
                    }
                }
                var instr = DisassembleInstruction(uInstr);
                if (instr.InstructionClass == InstrClass.Invalid)
                {
                    return MakeInvalidPacket(addr);
                }
                instrs.Add(instr);
                if (ShouldTerminatePacket(instr))
                {
                    return MakePacket(addr);
                }
            }
        }

        private bool ShouldTerminatePacket(HexagonInstruction instr)
        {
            if (instr.ParseType == ParseType.Duplex)
                return true;
            throw new NotImplementedException();
        }

        private HexagonPacket MakePacket(Address addr)
        {
            var packet = new HexagonPacket(instrs.ToArray())
            {
                Address = addr,
                Length = (int) (this.rdr.Address - addr)
            };
            instrs.Clear();
            return packet;
        }


        private HexagonPacket MakeInvalidPacket(Address addr)
        {
            var packet = MakePacket(addr);
            packet.InstructionClass = InstrClass.Invalid;
            return packet;
        }

        public override HexagonPacket CreateInvalidInstruction()
        {
            return MakeInvalidPacket(this.addrInstr);   //$BUG: should be addrPacket.
        }

        private HexagonInstruction CreateInvalidInstruction(Address addr)
        {
            return new HexagonInstruction(addr, Mnemonic.Invalid, MachineInstruction.NoOperands)
            {
                InstructionClass = InstrClass.Invalid
            };
        }

        private HexagonInstruction DisassembleInstruction(uint uInstr)
        {
            this.addrInstr = rdr.Address;
            this.ops.Clear();
            var instr = iclassDecoder.Decode(uInstr, this);
            instr.Length = 4;
            return instr;
        }

        public override HexagonPacket NotYetImplemented(uint wInstr, string message)
        {
            var testGenSvc = arch.Services.GetService<ITestGenerationService>();
            testGenSvc?.ReportMissingDecoder("Hexagon_dasm", this.addrInstr, this.rdr, message);
            return CreateInvalidInstruction();
        }

        #region Mutators
        
        /// <summary>
        /// 4-bit register encoding in duplex sub-instruction.
        /// </summary>
        private static Mutator<HexagonDisassembler> r(int bitpos)
        {
            var regField = new Bitfield(bitpos, 4);
            return (u, d) =>
            {
                var regEnc = regField.Read(u);
                var reg = subInstrRegs[regEnc];
                d.ops.Add(new RegisterOperand(reg));
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> rd = r(0);

        /// <summary>
        /// memory access in duplex sub-instruction
        /// </summary>
        private static Mutator<HexagonDisassembler> m(PrimitiveType width, int offsetPos)
        {
            var regField = new Bitfield(4, 4);
            var offField = new Bitfield(offsetPos, 3);
            return (u, d) =>
            {
                var offset = offField.Read(u) * width.Size;
                var baseReg = regField.Read(u);
                var mem = new MemoryOperand(width)
                {
                    Base = subInstrRegs[baseReg],
                    Offset = (int) offset,
                };
                d.ops.Add(mem);
                return true;
            };
        }
        private static readonly Mutator<HexagonDisassembler> mw = m(PrimitiveType.Word32, 8);
        private static readonly Mutator<HexagonDisassembler> mub = m(PrimitiveType.Byte, 8);

        #endregion

        #region Decoders

        // We cannot reuse the standard decoders because of the awkward encoding of instructions/packets.
        public abstract class Decoder
        {
            public abstract HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm);
        }

        public class InstrDecoder : Decoder
        {
            private InstrClass iclass;
            private Mnemonic mnemonic;
            private Mutator<HexagonDisassembler>[] mutators;

            public InstrDecoder(InstrClass iclass, Mnemonic mnemonic, Mutator<HexagonDisassembler> [] mutators)
            {
                this.iclass = iclass;
                this.mnemonic = mnemonic;
                this.mutators = mutators;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                foreach (var m in mutators)
                {
                    if (!m(uInstr, dasm))
                        return dasm.CreateInvalidInstruction(dasm.addrInstr);
                }
                var instr = new HexagonInstruction(dasm.addrInstr, this.mnemonic, dasm.ops.ToArray())
                {
                    InstructionClass = this.iclass
                };
                return instr;
            }
        }

        public class DuplexDecoder : Decoder
        {
            private readonly Decoder slot0;
            private readonly Decoder slot1;

            public DuplexDecoder(Decoder slot0, Decoder slot1)
            {
                this.slot0 = slot0;
                this.slot1 = slot1;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                dasm.instrs.Add(slot1.Decode(uInstr >> 16, dasm));
                dasm.ops.Clear();
                return slot0.Decode(uInstr, dasm);
            }
        }


        public class MaskDecoder : Decoder
        {
            private readonly Bitfield field;
            private readonly string tag;
            private readonly Decoder[] subdecoders;

            public MaskDecoder(Bitfield field, string tag, Decoder[] subdecoders)
            {
                if (subdecoders.Length != (1 << field.Length))
                    throw new InvalidOperationException($"Expected {1 << field.Length} decoders but receuved {subdecoders.Length}.");
                this.field = field;
                this.tag = tag;
                this.subdecoders = subdecoders;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                var subfield = field.Read(uInstr);
                return subdecoders[subfield].Decode(uInstr, dasm);
            }
        }


        public class BitfieldDecoder : Decoder
        {
            private readonly Bitfield[] fields;
            private readonly string tag;
            private readonly Decoder[] subdecoders;

            public BitfieldDecoder(Bitfield[] fields, string tag, Decoder[] subdecoders)
            {
                this.fields = fields;
                this.tag = tag;
                this.subdecoders = subdecoders;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                var subfield = Core.Lib.Bitfield.ReadFields(fields, uInstr);
                return subdecoders[subfield].Decode(uInstr, dasm);
            }
        }

        public class NyiDecoder : Decoder
        {
            private readonly string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override HexagonInstruction Decode(uint uInstr, HexagonDisassembler dasm)
            {
                dasm.NotYetImplemented(uInstr, message);
                return new HexagonInstruction(dasm.addrInstr, Mnemonic.Invalid, MachineInstruction.NoOperands)
                {
                    InstructionClass = InstrClass.Invalid,
                    Length = 4,
                };
            }
        }

            private static Decoder Instr(Mnemonic mnemonic, params Mutator<HexagonDisassembler>[] mutators)
        {
            return new InstrDecoder(InstrClass.Linear, mnemonic, mutators);
        }

        private static Decoder Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<HexagonDisassembler>[] mutators)
        {
            return new InstrDecoder(iclass, mnemonic, mutators);
        }


        private static Decoder Duplex(Decoder slot0, Decoder slot1)
        {
            return new DuplexDecoder(slot0, slot1);
        }

        private static Decoder Mask(int bitpos, int bitlength, string tag, params Decoder[] decoders)
        {
            return new MaskDecoder(new Bitfield(bitpos, bitlength), tag, decoders);
        }


        private static Decoder Bitfield(Bitfield[] fields, string tag, params Decoder[] decoders)
        {
            return new BitfieldDecoder(fields, tag, decoders);
        }


        private static Decoder Nyi(string message)
        {
            return new NyiDecoder(message);
        }
        #endregion

        static HexagonDisassembler()
        {
            var invalid = Instr(Mnemonic.Invalid, InstrClass.Invalid);

            subInstrRegs = new RegisterStorage[16]
            {
                Registers.GpRegs[0],
                Registers.GpRegs[1 ],
                Registers.GpRegs[2 ],
                Registers.GpRegs[3 ],
                Registers.GpRegs[4 ],
                Registers.GpRegs[5 ],
                Registers.GpRegs[6 ],
                Registers.GpRegs[7 ],
                Registers.GpRegs[16],
                Registers.GpRegs[17],
                Registers.GpRegs[18],
                Registers.GpRegs[19],
                Registers.GpRegs[20],
                Registers.GpRegs[21],
                Registers.GpRegs[22],
                Registers.GpRegs[23],
            };

            /*  code&0xFFFF0000 == 0xA09D0000  - allocframe
             *       localsize = code&0x7FF
             *
             *  subinsn: code&0x1E00 == 0x1C00
             *       localsize = (code>>4)&0x1F
             *
             *  locate __save_rX_through_rY   type functions:
             *     memd (r30 + #0xFFFFFFD0) = r27:26
             *
             A  011uuuuuudddd   Rd = add(r29,#u6:2)              Add immediate to stack pointer
            L2  1110uuuuudddd   Rd = memw(r29+#u5:2)             Load word from stack
            L2  11110uuuuuddd   Rdd = memd(r29+#u5:3)            Load pair from stack
            S2  0100uuuuutttt   memw(r29+#u5:2) = Rt             Store word to stack
            S2  0101ssssssttt   memd(r29+#s6:3) = Rtt            Store pair to stack
            S2  1110uuuuu----   allocframe(#u5:3)                Allocate stack frame
        */

            /*

            L2
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,0,0,j,j,j,s,s,s,s,d,d,d,d,Rd = memh(Rs+#u3:1)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,0,1,j,j,j,s,s,s,s,d,d,d,d,Rd = memuh(Rs+#u3:1)

            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,0,j,j,j,s,s,s,s,d,d,d,d,Rd = memb(Rs+#u3:0)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,0,j,j,j,j,j,d,d,d,d,Rd = memw(r29+#u5:2)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,0,j,j,j,j,j,d,d,d,Rdd = memd(r29+#u5:3)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,0,0,-,-,-,0,-,-,deallocframe
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,0,1,-,-,-,0,-,-,dealloc_return
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,0,1,-,-,-,1,0,0,if (P0) dealloc_return
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,0,1,-,-,-,1,0,1,if (!P0) dealloc_return
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,0,1,-,-,-,1,1,0,if (P0.new) dealloc_return:nt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,0,1,-,-,-,1,1,1,if (!P0.new) dealloc_return:nt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,1,1,-,-,-,0,-,-,jumpr R31
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,1,1,-,-,-,1,0,0,if (P0) jumpr R31
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,1,1,-,-,-,1,0,1,if (!P0) jumpr R31
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,1,1,-,-,-,1,1,0,if (P0.new) jumpr:nt R31
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,1,1,1,1,1,1,1,1,-,-,-,1,1,1,if (!P0.new) jumpr:nt R31
            S1
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,0,0,j,j,j,j,s,s,s,s,t,t,t,t,memw(Rs+#u4:2) = Rt

            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,0,1,j,j,j,j,s,s,s,s,t,t,t,t,memb(Rs+#u4:0) = Rt
            S2
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1,0,0,j,j,j,s,s,s,s,t,t,t,t,memh(Rs+#u3:1) = Rt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1,0,1,0,0,j,j,j,j,j,t,t,t,t,memw(r29+#u5:2) = Rt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1,0,1,0,1,j,j,j,j,j,j,t,t,t,memd(r29+#s6:3) = Rtt
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1,1,0,0,0,I,s,s,s,s,j,j,j,j,memw(Rs+#u4:2) = #U1
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1,1,0,0,1,I,s,s,s,s,j,j,j,j,memb(Rs+#u4) = #U1
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,1,1,1,1,1,0,j,j,j,j,j,-,-,-,-,allocframe(#u5:3)

            A
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,0,0,j,j,j,j,j,j,j,x,x,x,x,"Rx = add(Rx,#s7)"

            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,0,0,0,0,s,s,s,s,d,d,d,d,Rd = Rs

            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,0,1,0,j,j,j,j,j,j,d,d,d,d,Rd = #u6
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,0,1,1,j,j,j,j,j,j,d,d,d,d,"Rd = add(r29,#u6:2)"

            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,1,0,1,-,-,0,-,-,d,d,d,d,Rd = #-1
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,1,0,1,-,-,1,1,0,d,d,d,d,if (P0) Rd = #0
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,1,0,1,-,-,1,1,1,d,d,d,d,if (!P0) Rd = #0
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,1,0,1,-,-,1,0,0,d,d,d,d,if (P0.new) Rd = #0
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,1,0,1,-,-,1,0,1,d,d,d,d,if (!P0.new) Rd = #0
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,1,0,0,0,s,s,s,s,x,x,x,x,"Rx = add(Rx,Rs)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,1,0,0,1,s,s,s,s,-,-,j,j,"P0 = cmp.eq(Rs,#u2)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,1,1,-,1,s,s,s,s,0,d,d,d,"Rdd = combine(#0,Rs)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,1,1,-,1,s,s,s,s,1,d,d,d,"Rdd = combine(Rs,#0)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,1,1,-,0,-,I,I,j,j,d,d,d,"Rdd = combine(#u2,#U2)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,0,0,0,1,s,s,s,s,d,d,d,d,"Rd = add(Rs,#1)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,0,0,1,1,s,s,s,s,d,d,d,d,"Rd = add(Rs,#-1)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,0,1,0,0,s,s,s,s,d,d,d,d,Rd = sxth(Rs)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,0,1,0,1,s,s,s,s,d,d,d,d,Rd = sxtb(Rs)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,0,1,1,0,s,s,s,s,d,d,d,d,Rd = zxth(Rs)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,0,0,1,0,s,s,s,s,d,d,d,d,"Rd = and(Rs,#1)"
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,1,0,0,1,0,1,1,1,s,s,s,s,d,d,d,d,"Rd = and(Rs,#255)"
            */

            /*             L1
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,0,0,j,j,j,j,s,s,s,s,d,d,d,d,Rd = memw(Rs+#u4:2)
            -,-,-,-,-,-,-,-,-,-,-,-,-,-,-,-,0,0,0,1,j,j,j,j,s,s,s,s,d,d,d,d,Rd = memub(Rs+#u4:0)
            */

            var L1 = Mask(13, 1, "  L1",
                    Instr(Mnemonic.ASSIGN, rd, mw),
                    Instr(Mnemonic.ASSIGN, rd, mub));
            var L2 = Nyi("L2");
            var S1 = Nyi("S1");
            var S2 = Nyi("S2");
            var A = Nyi("A");

var duplexDecoder = Bitfield(Bf((29, 3), (13, 1)), "  duplex",
    Duplex(L1, L1),
    Duplex(L2, L1),
    Duplex(L2, L2),
    Duplex(A,  A),
    Duplex(L1, A),
    Duplex(L2, A),
    Duplex(S1, A),
    Duplex(S2, A),
    Duplex(S1, L1),
    Duplex(S1, L2),
    Duplex(S1, S1),
    Duplex(S2, S1),
    Duplex(S2, L1),
    Duplex(S2, L2),
    Duplex(S2, S2),
    Duplex(invalid, invalid));

var simplexDecoder = Mask(28, 4, "  simplex",
    Nyi("Constant extender(Section 10.10)"),
    Nyi("J 2, 3"),
    Nyi("J 2, 3"),
    Nyi("LD ST 0, 1"),
    Nyi("LD ST (conditional or GP - relative) 0, 1"),
    Nyi("J 2, 3"),
    Nyi("CR 3"),
    Nyi("ALU32 0, 1, 2, 3"),
    Nyi("XTYPE 2, 3"),
    Nyi("LD 0, 1"),
    Nyi("ST 0"),
    Nyi("ALU32 0, 1, 2, 3"),
    Nyi("XTYPE 2, 3"),
    Nyi("XTYPE 2, 3"),
    Nyi("XTYPE 2, 3"),
    Nyi("ALU32 0, 1, 2, 3"));


iclassDecoder = Mask(14, 2, "HexagonInstruction",
    duplexDecoder,
    simplexDecoder,
    simplexDecoder,
    simplexDecoder);
}
}
}

 