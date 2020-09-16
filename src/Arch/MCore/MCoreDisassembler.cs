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
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.Arch.MCore
{
    public class MCoreDisassembler : Reko.Core.Machine.DisassemblerBase<MCoreInstruction, Mnemonic>
    {
        private readonly MCoreArchitecture arch;
        private readonly EndianImageReader rdr;

        public MCoreDisassembler(MCoreArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
        }

        public override MCoreInstruction CreateInvalidInstruction()
        {
            throw new System.NotImplementedException();
        }

        public override MCoreInstruction? DisassembleInstruction()
        {
            throw new System.NotImplementedException();
        }

        public override MCoreInstruction NotYetImplemented(uint wInstr, string message)
        {
            throw new System.NotImplementedException();
        }

        /*
        // CK-610 C-Sky
        // https://github.com/c-sky/tools/raw/master/gx6605s/
         Legend:
        rrrr - RX field
        ssss - RY field
        zzzz - RZ field
        ffff - Rfirst field
        ccccc - control register specifier
        iii..i - one of several immediate fields
        ddddddddddd - branch displacement
        bbbb - loopt displacement
        uu- accelerator unit
        ee..e - execution code
        nnn - register count
        p - update option
        xx..x - undefined fields
        0000000000000000 bkpt
        0000000000000001 sync
        0000000000000010 rte
        0000000000000011 rfi
        0000000000000100 stop
        0000000000000101 wait
        0000000000000110 doze
        0000000000000111
        00000000000010ii trap #ii
        00000000000011xx
        000000000001rrrr mvc
        000000000011rrrr mvcv
        000000000100rrrr ldq
        000000000101rrrr stq
        000000000110rrrr ldm
        000000000111rrrr stm
        000000001000rrrr dect
        000000001001rrrr decf
        000000001010rrrr inct
        000000001011rrrr incf
        000000001100rrrr jmp
        000000001101rrrr jsr
        000000001110rrrr ff1
        000000001111rrrr brev
        000000010000rrrr xtrb3
        000000010001rrrr xtrb2
        000000010010rrrr xtrb1
        000000010011rrrr xtrb0
        000000010100rrrr zextb
        000000010101rrrr sextb
        000000010110rrrr zexth
        M•CORE INSTRUCTIONS MOTOROLA
        REFERENCE MANUAL 3-7
        000000010111rrrr sexth
        000000011000rrrr declt
        000000011001rrrr tstnbz
        000000011010rrrr decgt
        000000011011rrrr decne
        000000011100rrrr clrt
        000000011101rrrr clrf
        000000011110rrrr abs
        000000011111rrrr not
        00000010ssssrrrr movt
        00000011ssssrrrr mult
        00000100ssssbbbb loopt
        00000101ssssrrrr subu
        00000110ssssrrrr addc
        00000111ssssrrrr subc
        00001000ssssrrrr
        00001001ssssrrrr
        00001010ssssrrrr movf
        00001011ssssrrrr lsr
        00001100ssssrrrr cmphs
        00001101ssssrrrr cmplt
        00001110ssssrrrr tst
        00001111ssssrrrr cmpne
        0001000cccccrrrr mfcr
        00010010ssssrrrr mov
        00010011ssssrrrr bgenr
        00010100ssssrrrr rsub
        00010101ssssrrrr ixw
        00010110ssssrrrr and
        00010111ssssrrrr xor
        0001100cccccrrrr mtcr
        00011010ssssrrrr asr
        00011011ssssrrrr lsl
        00011100ssssrrrr addu
        00011101ssssrrrr ixh
        00011110ssssrrrr or
        00011111ssssrrrr andn
        0010000iiiiirrrr addi
        0010001iiiiirrrr cmplti
        0010010iiiiirrrr subi
        0010011iiiiirrrr
        0010100iiiiirrrr rsubi
        0010101iiiiirrrr cmpnei
        001011000000rrrr bmaski #32 (set)
        001011000001rrrr divu
        00101100001xrrrr
        0010110001xxrrrr
        001011001iiirrrr bmaski
        00101101iiiirrrr bmaski
        0010111iiiiirrrr andi
        0011000iiiiirrrr bclri
        001100100000rrrr
        001100100001rrrr divs
        00101100001xrrrr
        00101100010xrrrr
        001011000110rrrr
        001100100111rrrr bgeni
        001100101iiirrrr bgeni
        00110011iiiirrrr bgeni
        0011010iiiiirrrr bseti
        0011011iiiiirrrr btsti
        001110000000rrrr xsr
        0011100iiiiirrrr rotli
        001110100000rrrr asrc
        0011101iiiiirrrr asri
        001111000000rrrr lslc
        0011110iiiiirrrr lsli
        001111100000rrrr lsrc
        0011111iiiiirrrr lsri
        0100uu00eeeeeeee h_exec
        0100uu010nnneeee h_ret
        0100uu011nnneeee h_call
        0100uu100piirrrr h_ld
        0100uu101piirrrr h_st
        0100uu110piirrrr h_ld.h
        0100uu111piirrrr h_st.h
        0101xxxxxxxxxxxx
        01100iiiiiiirrrr movi
        01101xxxxxxxxxxx
        0111zzzzdddddddd lrw
        01110000dddddddd jmpi
        01111111dddddddd jsri
        1000zzzziiiirrrr ld
        1001zzzziiiirrrr st
        1010zzzziiiirrrr ld.b
        1011zzzziiiirrrr st.b
        1100zzzziiiirrrr ld.h
        1101zzzziiiirrrr st.h
        11100ddddddddddd bt
        11101ddddddddddd bf
        11110ddddddddddd br
        11111ddddddddddd bsr
        */
    }
}