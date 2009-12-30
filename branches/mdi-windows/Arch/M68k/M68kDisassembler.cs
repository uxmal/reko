/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decompiler.Arch.M68k
{
    // M68k opcode map in http://www.freescale.com/files/archives/doc/ref_manual/M68000PRM.pdf

    public class M68kDisassembler : Disassembler
    {
        private ImageReader rdr;

        public M68kDisassembler(ImageReader rdr)
        {
            this.rdr = rdr;
        }

        public override Address Address
        {
            get { return rdr.Address; }
        }

        public override MachineInstruction DisassembleInstruction()
        {
            return Disassemble();
        }

        private Decoder FindDecoder(ushort opcode)
        {
            int l = 0;
            int h = oprecs.Count - 1;
            int m;
            while (l <= h)
            {
                m = l + (h - l) / 2;
                Opmask d = oprecs.Keys[m];
                int c = d.Compare(opcode);
                if (c == 0)
                    return oprecs.Values[m];
                else if (c < 0)
                    h = m - 1;
                else
                    l = m + 1;
            }
            return null;
        }

        public M68kInstruction Disassemble()
        {
            ushort opcode = rdr.ReadBeUint16();
            Decoder decoder = FindDecoder(opcode);
            if (decoder == null)
                throw new InvalidOperationException(string.Format("Unknown 680x0 opcode {0:X4}.", opcode));
            System.Diagnostics.Debug.WriteLine(string.Format("{0:X4}->{1} {2}", opcode, decoder.opcode, decoder.args));
            return decoder.Decode(opcode, rdr);
        }
    

        private class Opmask: IComparable<Opmask>
        {
            public readonly ushort opcode;
            public readonly ushort mask;

            public Opmask(ushort code, ushort mask)
            {
                this.opcode = code;
                this.mask = mask;
            }
 
            public int Compare(ushort value)
            {
                return (value & mask) - (this.opcode & mask);
            }

            public int CompareTo(Opmask other)
            {
                return (mask & opcode) - (other.mask & other.opcode);
            }
        }


        private static SortedList<Opmask, Decoder> oprecs;

        static M68kDisassembler()
        {
            oprecs = new SortedList<Opmask, Decoder>();
            oprecs.Add(new Opmask(0x0000, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0008, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0010, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0018, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0020, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0028, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x0030, 0xFF3C), new Decoder(Opcode.ori, "s6:Iv,e0"));
            oprecs.Add(new Opmask(0x003C, 0xFFFF), new Decoder(Opcode.ori, "sb:Ib,c"));
            oprecs.Add(new Opmask(0x007C, 0xFFFF), new Decoder(Opcode.ori, "sw:Iw,s"));
            oprecs.Add(new Opmask(0x0140, 0xF1C0), new Decoder(Opcode.bchg, "D9,E0"));
            oprecs.Add(new Opmask(0x2000, 0xF000), new Decoder(Opcode.movea, "sl:E0,A9"));
            oprecs.Add(new Opmask(0x3000, 0xF000), new Decoder(Opcode.move, "sw:E0,e6"));
            oprecs.Add(new Opmask(0x41C0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x43C0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x45C0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x47C0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x4BC0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x4DC0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x4FC0, 0xFFC0), new Decoder(Opcode.lea, "E0,A9"));
            oprecs.Add(new Opmask(0x48C0, 0xFFC0), new Decoder(Opcode.movem, "sl:Iw,E0"));
            oprecs.Add(new Opmask(0x5000, 0xF100), new Decoder(Opcode.addq, "s6:q9,E0"));
            oprecs.Add(new Opmask(0x6000, 0xFF00), new Decoder(Opcode.bra, "J"));
            oprecs.Add(new Opmask(0x7000, 0xF100), new Decoder(Opcode.moveq, "Q0,D9"));
            oprecs.Add(new Opmask(0xD2C1, 0xF0C0), new Decoder(Opcode.adda, "sw:E0,A9"));
            oprecs.Add(new Opmask(0xE108, 0xF138), new Decoder(Opcode.lsl, "s6:q9,D0"));

            foreach (KeyValuePair<Opmask, Decoder> item in oprecs)
            {
                Debug.WriteLine(string.Format("{0:X4},{1:x4}:{2},{3}", item.Key.opcode, item.Key.mask, item.Value.opcode, item.Value.args));
            }
        }
    }
}
