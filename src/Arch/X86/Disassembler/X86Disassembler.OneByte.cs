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
using Reko.Core.Machine;

namespace Reko.Arch.X86
{
    using Decoder = Decoder<X86Disassembler, Mnemonic, X86Instruction>;

    public partial class X86Disassembler
    {
        public partial class InstructionSet 
        {
            private void CreateOnebyteDecoders(
                Decoder[] d,
                bool isRex2,
                Decoder[] decoders0F)
            {
				// 00
				d[0x00] = Instr(Mnemonic.add, InstrClass.Linear|InstrClass.Zero, Eb,Gb);
				d[0x01] = Instr(Mnemonic.add, Ev,Gv);
				d[0x02] = Instr(Mnemonic.add, Gb,Eb);
				d[0x03] = Instr(Mnemonic.add, Gv,Ev);
				d[0x04] = Instr(Mnemonic.add, AL,Ib);
				d[0x05] = Instr(Mnemonic.add, rAX,Iz);
				d[0x06] = Amd64Instr(
                    Instr(Mnemonic.push, s0),
                    s_invalid);
				d[0x07] = Amd64Instr(
                    Instr(Mnemonic.pop, s0),
                    s_invalid);

				d[0x08] = Instr(Mnemonic.or, Eb,Gb);
				d[0x09] = Instr(Mnemonic.or, Ev,Gv);
				d[0x0A] = Instr(Mnemonic.or, Gb,Eb);
				d[0x0B] = Instr(Mnemonic.or, Gv,Ev);
				d[0x0C] = Instr(Mnemonic.or, AL,Ib);
				d[0x0D] = Instr(Mnemonic.or, rAX,Iz);
				d[0x0E] = Amd64Instr(
                    Instr(Mnemonic.push, s1),
                    s_invalid);
				d[0x0F] = isRex2 ? s_invalid : instr286(new AdditionalByteDecoder(decoders0F));

				// 10
				d[0x10] = Instr(Mnemonic.adc, Eb,Gb);
				d[0x11] = Instr(Mnemonic.adc, Ev,Gv);
				d[0x12] = Instr(Mnemonic.adc, Gb,Eb);
				d[0x13] = Instr(Mnemonic.adc, Gv,Ev);
				d[0x14] = Instr(Mnemonic.adc, AL,Ib);
				d[0x15] = Instr(Mnemonic.adc, rAX,Iz);
				d[0x16] = Amd64Instr(
                    Instr(Mnemonic.push, s2),
                    s_invalid);
				d[0x17] = Amd64Instr(
                    Instr(Mnemonic.pop, s2),
                    s_invalid);

				d[0x18] = Instr(Mnemonic.sbb, Eb,Gb);
				d[0x19] = Instr(Mnemonic.sbb, Ev,Gv);
				d[0x1A] = Instr(Mnemonic.sbb, Gb,Eb);
				d[0x1B] = Instr(Mnemonic.sbb, Gv,Ev);
				d[0x1C] = Instr(Mnemonic.sbb, AL,Ib);
				d[0x1D] = Instr(Mnemonic.sbb, rAX,Iz);
				d[0x1E] = Amd64Instr(
                    Instr(Mnemonic.push, s3),
                    s_invalid);
				d[0x1F] = Amd64Instr(
                    Instr(Mnemonic.pop, s3),
                    s_invalid);

				// 20
				d[0x20] = Instr(Mnemonic.and, Eb,Gb); 
				d[0x21] = Instr(Mnemonic.and, Ev,Gv);
				d[0x22] = Instr(Mnemonic.and, Gb,Eb);
				d[0x23] = Instr(Mnemonic.and, Gv,Ev);
				d[0x24] = Instr(Mnemonic.and, AL,Ib);
				d[0x25] = Instr(Mnemonic.and, rAX,Iz);
				d[0x26] = isRex2 ? s_invalid : new SegmentOverrideDecoder(0);
				d[0x27] = Amd64Instr(
                    Instr(Mnemonic.daa),
                    s_invalid);

				d[0x28] = Instr(Mnemonic.sub, Eb,Gb);
				d[0x29] = Instr(Mnemonic.sub, Ev,Gv);
				d[0x2A] = Instr(Mnemonic.sub, Gb,Eb);
				d[0x2B] = Instr(Mnemonic.sub, Gv,Ev);
				d[0x2C] = Instr(Mnemonic.sub, AL,Ib);
				d[0x2D] = Instr(Mnemonic.sub, rAX,Iz);
                d[0x2E] = isRex2 ? s_invalid : new SegmentOverrideDecoder(1);
				d[0x2F] = Amd64Instr(
                    Instr(Mnemonic.das),
                    s_invalid);

				// 30
				d[0x30] = Instr(Mnemonic.xor, Eb,Gb);
				d[0x31] = Instr(Mnemonic.xor, Ev,Gv);
				d[0x32] = Instr(Mnemonic.xor, Gb,Eb);
				d[0x33] = Instr(Mnemonic.xor, Gv,Ev);
				d[0x34] = Instr(Mnemonic.xor, AL,Ib);
				d[0x35] = Instr(Mnemonic.xor, rAX,Iz);
                d[0x36] = isRex2 ? s_invalid : new SegmentOverrideDecoder(2);
				d[0x37] = Amd64Instr(
                    Instr(Mnemonic.aaa),
                    s_invalid);

				d[0x38] = Instr(Mnemonic.cmp, Eb,Gb);
				d[0x39] = Instr(Mnemonic.cmp, Ev,Gv);
				d[0x3A] = Instr(Mnemonic.cmp, Gb,Eb);
				d[0x3B] = Instr(Mnemonic.cmp, Gv,Ev);
				d[0x3C] = Instr(Mnemonic.cmp, AL,Ib);
				d[0x3D] = Instr(Mnemonic.cmp, rAX,Iz);
                d[0x3E] = isRex2 ? s_invalid : new SegmentOverrideDecoder(3);
				d[0x3F] = Amd64Instr(
                    Instr(Mnemonic.aas),
                    s_invalid);

				// 40
				d[0x40] = isRex2 ? s_invalid : RexInstr(Mnemonic.inc, rv);
				d[0x41] = isRex2 ? s_invalid : RexInstr(Mnemonic.inc, rv);
				d[0x42] = isRex2 ? s_invalid : RexInstr(Mnemonic.inc, rv);
				d[0x43] = isRex2 ? s_invalid : RexInstr(Mnemonic.inc, rv);
				d[0x44] = isRex2 ? s_invalid : RexInstr(Mnemonic.inc, rv);
				d[0x45] = isRex2 ? s_invalid : RexInstr(Mnemonic.inc, rv);
				d[0x46] = isRex2 ? s_invalid : RexInstr(Mnemonic.inc, rv);
				d[0x47] = isRex2 ? s_invalid : RexInstr(Mnemonic.inc, rv);

				d[0x48] = isRex2 ? s_invalid : RexInstr(Mnemonic.dec, rv);
				d[0x49] = isRex2 ? s_invalid : RexInstr(Mnemonic.dec, rv);
				d[0x4A] = isRex2 ? s_invalid : RexInstr(Mnemonic.dec, rv);
				d[0x4B] = isRex2 ? s_invalid : RexInstr(Mnemonic.dec, rv);
				d[0x4C] = isRex2 ? s_invalid : RexInstr(Mnemonic.dec, rv);
				d[0x4D] = isRex2 ? s_invalid : RexInstr(Mnemonic.dec, rv);
				d[0x4E] = isRex2 ? s_invalid : RexInstr(Mnemonic.dec, rv);
				d[0x4F] = isRex2 ? s_invalid : RexInstr(Mnemonic.dec, rv);

				// 50
                d[0x50] = Instr(Mnemonic.push, rV);
                d[0x51] = Instr(Mnemonic.push, rV);
                d[0x52] = Instr(Mnemonic.push, rV);
                d[0x53] = Instr(Mnemonic.push, rV);
                d[0x54] = Instr(Mnemonic.push, rV);
                d[0x55] = Instr(Mnemonic.push, rV);
                d[0x56] = Instr(Mnemonic.push, rV);
                d[0x57] = Instr(Mnemonic.push, rV);

                d[0x58] = Instr(Mnemonic.pop, rV);
                d[0x59] = Instr(Mnemonic.pop, rV);
                d[0x5A] = Instr(Mnemonic.pop, rV);
                d[0x5B] = Instr(Mnemonic.pop, rV);
                d[0x5C] = Instr(Mnemonic.pop, rV);
                d[0x5D] = Instr(Mnemonic.pop, rV);
                d[0x5E] = Instr(Mnemonic.pop, rV);
                d[0x5F] = Instr(Mnemonic.pop, rV);

				// 60
				d[0x60] = Amd64Instr(
                    Instr186(Mnemonic.pusha),
                    s_invalid);
                d[0x61] = Amd64Instr(
                    Instr186(Mnemonic.popa),
                    s_invalid);
                d[0x62] = isRex2 ? s_invalid : Amd64Instr(
                    Instr186(Mnemonic.bound, Gv,Mv),
                    new EvexDecoder(this.s_decoders0F, s_decoders0F38, s_decoders0F3A));
                d[0x63] = Amd64Instr(
    				Instr286(Mnemonic.arpl, Ew,rw),
    				Instr(Mnemonic.movsxd, Gv,Ed));
				d[0x64] = isRex2 ? s_invalid : Instr386(new SegmentOverrideDecoder(4));
				d[0x65] = isRex2 ? s_invalid : Instr386(new SegmentOverrideDecoder(5));
				d[0x66] = isRex2 ? s_invalid : Instr386(new ChangeDataWidth(this.rootDecoders));
				d[0x67] = isRex2 ? s_invalid : Instr386(new ChangeAddressWidth(this.rootDecoders));

				d[0x68] = new PrefixedDecoder(
                    dec: Instr186(Mnemonic.push, Iz),
                    dec66: Instr186(Mnemonic.pushw, Iw));
				d[0x69] = Instr186(Mnemonic.imul, Gv,Ev,Iz);
				d[0x6A] = new PrefixedDecoder(
                    dec: Instr186(Mnemonic.push, Ib),
                    dec66:Instr186(Mnemonic.pushw, Ib));
				d[0x6B] = Instr186(Mnemonic.imul, Gv,Ev,Ib);
				d[0x6C] = Instr186(Mnemonic.insb, InstrClass.Linear | InstrClass.Privileged, Yb, DX);
				d[0x6D] = Instr186(Mnemonic.ins, InstrClass.Linear | InstrClass.Privileged, Yv, DX);
				d[0x6E] = Instr186(Mnemonic.outsb, InstrClass.Linear | InstrClass.Privileged, DX, Xb);
				d[0x6F] = Instr186(Mnemonic.outs, InstrClass.Linear | InstrClass.Privileged, DX, Xv);

				// 70
				d[0x70] = isRex2 ? s_invalid : Instr(Mnemonic.jo, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x71] = isRex2 ? s_invalid : Instr(Mnemonic.jno, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x72] = isRex2 ? s_invalid : Instr(Mnemonic.jc, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x73] = isRex2 ? s_invalid : Instr(Mnemonic.jnc, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x74] = isRex2 ? s_invalid : Instr(Mnemonic.jz, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x75] = isRex2 ? s_invalid : Instr(Mnemonic.jnz, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x76] = isRex2 ? s_invalid : Instr(Mnemonic.jbe, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x77] = isRex2 ? s_invalid : Instr(Mnemonic.ja, InstrClass.Transfer|InstrClass.Conditional, Jb);

				d[0x78] = isRex2 ? s_invalid : Instr(Mnemonic.js, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x79] = isRex2 ? s_invalid : Instr(Mnemonic.jns, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x7A] = isRex2 ? s_invalid : Instr(Mnemonic.jpe, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x7B] = isRex2 ? s_invalid : Instr(Mnemonic.jpo, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x7C] = isRex2 ? s_invalid : Instr(Mnemonic.jl, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x7D] = isRex2 ? s_invalid : Instr(Mnemonic.jge, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x7E] = isRex2 ? s_invalid : Instr(Mnemonic.jle, InstrClass.Transfer|InstrClass.Conditional, Jb);
				d[0x7F] = isRex2 ? s_invalid : Instr(Mnemonic.jg, InstrClass.Transfer|InstrClass.Conditional, Jb);

				// 80
				d[0x80] = new GroupDecoder(Grp1, Eb,Ib);
				d[0x81] = new GroupDecoder(Grp1, Ev,Iz);
				d[0x82] = Amd64Instr(
                    new GroupDecoder(Grp1, Eb,Ib),
                    s_invalid);
				d[0x83] = new GroupDecoder(Grp1, Ev,Ib);
				d[0x84] = Instr(Mnemonic.test, Eb,Gb);
				d[0x85] = Instr(Mnemonic.test, Ev,Gv);
				d[0x86] = Instr(Mnemonic.xchg, Eb,Gb);
				d[0x87] = Instr(Mnemonic.xchg, Ev,Gv);

				d[0x88] = Instr(Mnemonic.mov, Eb,Gb);
				d[0x89] = Instr(Mnemonic.mov, Ev,Gv);
				d[0x8A] = Instr(Mnemonic.mov, Gb,Eb);
				d[0x8B] = Instr(Mnemonic.mov, Gv,Ev);
				d[0x8C] = Instr(Mnemonic.mov, Ewv,Sw);
				d[0x8D] = Instr(Mnemonic.lea, Gv,Mv);
				d[0x8E] = Instr(Mnemonic.mov, Sw,Ew);
				d[0x8F] = Amd64Instr(
                    Instr(Mnemonic.pop, Ev),
                    new GroupDecoder(Grp1A));

				// 90
				d[0x90] = new PrefixedDecoder(
                    dec:Instr(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding),
                    dec66:Instr386(Mnemonic.nop, InstrClass.Linear | InstrClass.Padding),
                    decF3:Instr386(Mnemonic.pause, InstrClass.Linear | InstrClass.Padding));
				d[0x91] = Instr(Mnemonic.xchg, rv,rAX);
				d[0x92] = Instr(Mnemonic.xchg, rv,rAX);
				d[0x93] = Instr(Mnemonic.xchg, rv,rAX);
				d[0x94] = Instr(Mnemonic.xchg, rv,rAX);
				d[0x95] = Instr(Mnemonic.xchg, rv,rAX);
				d[0x96] = Instr(Mnemonic.xchg, rv,rAX);
				d[0x97] = Instr(Mnemonic.xchg, rv,rAX);

				d[0x98] = DataWidthDependent(
                    bit16:Instr(Mnemonic.cbw),
                    bit32:Instr(Mnemonic.cwde),
                    bit64:Instr(Mnemonic.cdqe));
                d[0x99] = DataWidthDependent(
                    bit16:Instr(Mnemonic.cwd),
                    bit32:Instr(Mnemonic.cdq),
                    bit64:Instr(Mnemonic.cqo));
                d[0x9A] = Amd64Instr(
                    Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Ap),
                    s_invalid);
				d[0x9B] = Instr(Mnemonic.wait);
				d[0x9C] = Instr(Mnemonic.pushf);
                d[0x9D] = new PrefixedDecoder(
                    dec: Instr(Mnemonic.popf),
                    dec66: Instr386(Mnemonic.popfw));
				d[0x9E] = Instr(Mnemonic.sahf);
				d[0x9F] = Instr(Mnemonic.lahf);

				// A0
				d[0xA0] = isRex2 ? s_invalid : Instr(Mnemonic.mov, AL,Ob);
				d[0xA1] = isRex2 ? s_invalid : Instr(Mnemonic.mov, rAX,Ov); //$BUG: should be jmpabs
				d[0xA2] = isRex2 ? s_invalid : Instr(Mnemonic.mov, Ob,AL);
				d[0xA3] = isRex2 ? s_invalid : Instr(Mnemonic.mov, Ov,rAX);
				d[0xA4] = isRex2 ? s_invalid : Instr(Mnemonic.movsb, Yb, Xb);
				d[0xA5] = isRex2 ? s_invalid : Instr(Mnemonic.movs, Yv, Xv);
				d[0xA6] = isRex2 ? s_invalid : Instr(Mnemonic.cmpsb, Yb, Xb);
				d[0xA7] = isRex2 ? s_invalid : Instr(Mnemonic.cmps, Yv, Xv);

				d[0xA8] = isRex2 ? s_invalid : Instr(Mnemonic.test, AL,Ib);
				d[0xA9] = isRex2 ? s_invalid : Instr(Mnemonic.test, rAX,Iz);
				d[0xAA] = isRex2 ? s_invalid : Instr(Mnemonic.stosb, Yb, AL);
				d[0xAB] = isRex2 ? s_invalid : Instr(Mnemonic.stos, Yv, rAX);
				d[0xAC] = isRex2 ? s_invalid : Instr(Mnemonic.lodsb, AL, Xb);
				d[0xAD] = isRex2 ? s_invalid : Instr(Mnemonic.lods, rAX, Xv);
				d[0xAE] = isRex2 ? s_invalid : Instr(Mnemonic.scasb, AL, Yb);
				d[0xAF] = isRex2 ? s_invalid : Instr(Mnemonic.scas, rAX, Yv);

				// B0
				d[0xB0] = Instr(Mnemonic.mov, rb,Ib);
				d[0xB1] = Instr(Mnemonic.mov, rb,Ib);
				d[0xB2] = Instr(Mnemonic.mov, rb,Ib);
				d[0xB3] = Instr(Mnemonic.mov, rb,Ib);
				d[0xB4] = Instr(Mnemonic.mov, rb,Ib);
				d[0xB5] = Instr(Mnemonic.mov, rb,Ib);
				d[0xB6] = Instr(Mnemonic.mov, rb,Ib);
				d[0xB7] = Instr(Mnemonic.mov, rb,Ib);

				d[0xB8] = Instr(Mnemonic.mov, rv,Iv);
				d[0xB9] = Instr(Mnemonic.mov, rv,Iv);
				d[0xBA] = Instr(Mnemonic.mov, rv,Iv);
				d[0xBB] = Instr(Mnemonic.mov, rv,Iv);
				d[0xBC] = Instr(Mnemonic.mov, rv,Iv);
				d[0xBD] = Instr(Mnemonic.mov, rv,Iv);
				d[0xBE] = Instr(Mnemonic.mov, rv,Iv);
				d[0xBF] = Instr(Mnemonic.mov, rv,Iv);

				// C0
				d[0xC0] = Instr286(new GroupDecoder(Grp2, Eb,Ib));
				d[0xC1] = Instr286(new GroupDecoder(Grp2, Ev,Ib));
				d[0xC2] = Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return, Iw);
				d[0xC3] = Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return);
                d[0xC4] = isRex2 ? s_invalid : Amd64Instr(
                    new C0Decoder(
                        Instr(Mnemonic.les, Gv, Mp),
                        new VexDecoder3(decoders0F, s_decoders0F38, s_decoders0F3A)),
                    new VexDecoder3(decoders0F, s_decoders0F38, s_decoders0F3A));
                d[0xC5] = isRex2 ? s_invalid : Amd64Instr(
                    new C0Decoder(
                        Instr(Mnemonic.lds, Gv,Mp),
                        new VexDecoder2(decoders0F)),
                    new VexDecoder2(decoders0F));
                d[0xC6] = Amd64Instr(
                    Instr(Mnemonic.mov, Eb,Ib),
                    new GroupDecoder(Grp11b));
                d[0xC7] = Amd64Instr(
                    Instr(Mnemonic.mov, Ev,Iz),
                    new GroupDecoder(Grp11z));

                d[0xC8] = new PrefixedDecoder(
                    dec: Instr186(Mnemonic.enter, Iw, Ib),
                    dec66: Instr186(Mnemonic.enterw, Iw, Ib));
				d[0xC9] = Instr186(Mnemonic.leave);
				d[0xCA] = Instr(Mnemonic.retf, InstrClass.Transfer | InstrClass.Return, Iw);
				d[0xCB] = Instr(Mnemonic.retf, InstrClass.Transfer | InstrClass.Return);
				d[0xCC] = Instr(Mnemonic.@int, InstrClass.Linear|InstrClass.Padding, n3);
				d[0xCD] = new InterruptDecoder(Mnemonic.@int, Ib);
				d[0xCE] = Amd64Instr(
                    Instr(Mnemonic.into),
                    s_invalid);
				d[0xCF] = Instr(Mnemonic.iret, InstrClass.Transfer | InstrClass.Return);

				// D0
				d[0xD0] = new GroupDecoder(Grp2, Eb,n1);
				d[0xD1] = new GroupDecoder(Grp2, Ev,n1);
				d[0xD2] = new GroupDecoder(Grp2, Eb,c);
				d[0xD3] = new GroupDecoder(Grp2, Ev,c);
				d[0xD4] = Amd64Instr(
                    Instr(Mnemonic.aam, Ib),
                    s_invalid);
				d[0xD5] = isRex2 ? s_invalid : Amd64Instr(
                    Instr(Mnemonic.aad, Ib),
				    Rex2Prefix(s_invalid));
                d[0xD6] = s_invalid;
				d[0xD7] = Instr(Mnemonic.xlat, b);

				d[0xD8] = X87Instr();
				d[0xD9] = X87Instr();
				d[0xDA] = X87Instr();
				d[0xDB] = X87Instr();
				d[0xDC] = X87Instr();
				d[0xDD] = X87Instr();
				d[0xDE] = X87Instr();
				d[0xDF] = X87Instr();

				// E0
				d[0xE0] = isRex2 ? s_invalid : Instr(Mnemonic.loopne, InstrClass.ConditionalTransfer, Jb);
				d[0xE1] = isRex2 ? s_invalid : Instr(Mnemonic.loope, InstrClass.ConditionalTransfer, Jb);
				d[0xE2] = isRex2 ? s_invalid : Instr(Mnemonic.loop, InstrClass.ConditionalTransfer, Jb);
				d[0xE3] = isRex2 ? s_invalid : AddrWidthDependent(
                    bit16:Instr(Mnemonic.jcxz, InstrClass.ConditionalTransfer, Jb),
                    bit32:Instr(Mnemonic.jecxz, InstrClass.ConditionalTransfer, Jb),
                    bit64:Instr(Mnemonic.jrcxz, InstrClass.ConditionalTransfer, Jb));
                d[0xE4] = isRex2 ? s_invalid : Instr(Mnemonic.@in,  InstrClass.Linear|InstrClass.Privileged, AL,Ib);
				d[0xE5] = isRex2 ? s_invalid : Instr(Mnemonic.@in,  InstrClass.Linear|InstrClass.Privileged, eAX,Ib);
				d[0xE6] = isRex2 ? s_invalid : Instr(Mnemonic.@out, InstrClass.Linear|InstrClass.Privileged, Ib,AL);
				d[0xE7] = isRex2 ? s_invalid : Instr(Mnemonic.@out, InstrClass.Linear|InstrClass.Privileged, Ib,eAX);

				d[0xE8] = isRex2 ? s_invalid : Instr(Mnemonic.call, InstrClass.Transfer|InstrClass.Call, Jv);
				d[0xE9] = isRex2 ? s_invalid : Instr(Mnemonic.jmp, InstrClass.Transfer, Jv);
				d[0xEA] = isRex2 ? s_invalid : Amd64Instr(
                    Instr(Mnemonic.jmp, InstrClass.Transfer, Ap),
                    s_invalid);
				d[0xEB] = isRex2 ? s_invalid : Instr(Mnemonic.jmp, InstrClass.Transfer, Jb);
				d[0xEC] = isRex2 ? s_invalid : Instr(Mnemonic.@in, AL,DX);
				d[0xED] = isRex2 ? s_invalid : Instr(Mnemonic.@in, eAX,DX);
				d[0xEE] = isRex2 ? s_invalid : Instr(Mnemonic.@out, DX,AL);
				d[0xEF] = isRex2 ? s_invalid : Instr(Mnemonic.@out, DX,eAX);

				// F0
				d[0xF0] = isRex2 ? s_invalid : Instr(Mnemonic.@lock);
				d[0xF1] = Instr(Mnemonic.icebp, InstrClass.Invalid);
				d[0xF2] = isRex2 ? s_invalid : new F2PrefixDecoder(rootDecoders);
				d[0xF3] = isRex2 ? s_invalid : new F3PrefixDecoder(rootDecoders);
				d[0xF4] = Instr(Mnemonic.hlt, InstrClass.Terminates|InstrClass.Privileged);
				d[0xF5] = Instr(Mnemonic.cmc);
				d[0xF6] = new GroupDecoder(Grp3, Eb);
				d[0xF7] = new GroupDecoder(Grp3, Ev);

				d[0xF8] = Instr(Mnemonic.clc);
				d[0xF9] = Instr(Mnemonic.stc);
				d[0xFA] = Instr(Mnemonic.cli);
				d[0xFB] = Instr(Mnemonic.sti);
				d[0xFC] = Instr(Mnemonic.cld);
				d[0xFD] = Instr(Mnemonic.std);
				d[0xFE] = new GroupDecoder(Grp4);
                d[0xFF] = new GroupDecoder(Grp5);
            }
        }
    }
}
