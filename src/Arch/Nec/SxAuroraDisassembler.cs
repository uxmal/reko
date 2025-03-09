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
using System;

namespace Reko.Arch.Nec;

public class SxAuroraDisassembler : DisassemblerBase<SxAuroraInstruction, Mnemonic>
{
    private static readonly Decoder<SxAuroraDisassembler, Mnemonic, SxAuroraInstruction> rootDecoder;

    public override SxAuroraInstruction CreateInvalidInstruction()
    {
        throw new NotImplementedException();
    }

    public override SxAuroraInstruction? DisassembleInstruction()
    {
        throw new NotImplementedException();
    }

    public override SxAuroraInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        throw new NotImplementedException();
    }

    public override SxAuroraInstruction NotYetImplemented(string message)
    {
        throw new NotImplementedException();
    }

    private static bool CF(uint uInstr, SxAuroraDisassembler dasm) => throw new NotImplementedException();
    private static bool RM(uint uInstr, SxAuroraDisassembler dasm) => throw new NotImplementedException();
    private static bool RR(uint uInstr, SxAuroraDisassembler dasm) => throw new NotImplementedException();
    private static bool RRM(uint uInstr, SxAuroraDisassembler dasm) => throw new NotImplementedException();
    private static bool RV(uint uInstr, SxAuroraDisassembler dasm) => throw new NotImplementedException();
    private static bool RVM(uint uInstr, SxAuroraDisassembler dasm) => throw new NotImplementedException();
    private static bool RW(uint uInstr, SxAuroraDisassembler dasm) => throw new NotImplementedException();

    private static Decoder<SxAuroraDisassembler, Mnemonic, SxAuroraInstruction> Instr(Mnemonic mnemonic, params Mutator<SxAuroraDisassembler>[] mutators)
    {
        throw new NotImplementedException();
    }

    private static Decoder<SxAuroraDisassembler, Mnemonic, SxAuroraInstruction> Instr(Mnemonic mnemonic, InstrClass iclass,  params Mutator<SxAuroraDisassembler>[] mutators)
    {
        throw new NotImplementedException();
    }


    static SxAuroraDisassembler()
    {
        // 10.1. List of SX-Aurora TSUBASA Instructions 
        rootDecoder = Sparse(56, 8, "SX-Aurora", Instr(Mnemonic.Invalid, InstrClass.Invalid),
           (0x01, Instr(Mnemonic.lds, RM)),    //  8.2.2. Load S
(0x02, Instr(Mnemonic.ldu, RM)),    //  8.2.3. Load S Upper
(0x03, Instr(Mnemonic.ldl, RM)),    //  8.2.4. Load S Lower
(0x04, Instr(Mnemonic.ld2b, RM)),    //  8.2.5. Load 2B
(0x05, Instr(Mnemonic.ld1b, RM)),    //  8.2.6. Load 1B
(0x06, Instr(Mnemonic.lea, RM)),    //  8.2.1. Load Effective Address
(0x08, Instr(Mnemonic.bsic, RM)),    //  8.8.5. Branch and Save IC
(0x09, Instr(Mnemonic.dlds, RM)),    //  8.2.12. Dismissable Load S
(0x0A, Instr(Mnemonic.dldu, RM)),    //  8.2.13. Dismissable Load Upper
(0x0B, Instr(Mnemonic.dldl, RM)),    //  8.2.14. Dismissable Load Lower
(0x0C, Instr(Mnemonic.pfch, RM)),    //  8.2.15. Pre Fetch
(0x0F, Instr(Mnemonic.cvd, RW)),    //  8.7.16. Convert to Double-format
(0x11, Instr(Mnemonic.sts, RM)),    //  8.2.7. Store S
(0x12, Instr(Mnemonic.stu, RM)),    //  8.2.8. Store S Upper
(0x13, Instr(Mnemonic.stl, RM)),    //  8.2.9. Store S Lower
(0x14, Instr(Mnemonic.st2b, RM)),    //  8.2.10. Store 2B
(0x15, Instr(Mnemonic.st1b, RM)),    //  8.2.11. Store 1B
(0x18, Instr(Mnemonic.bcr, CF)),    //  8.8.4. Branch on Condition Relative
(0x19, Instr(Mnemonic.bc, CF)),    //  8.8.1. Branch
(0x1B, Instr(Mnemonic.bcs, CF)),    //  8.8.2. Branch on Condition Single
(0x1C, Instr(Mnemonic.bcf, CF)),    //  8.8.3. Branch on Condition Floating Point
(0x1F, Instr(Mnemonic.cvs, RW)),    //  8.7.15. Convert to Single-format
(0x20, Instr(Mnemonic.fence, RR)),    //  8.3.1. Fence
(0x21, Instr(Mnemonic.lhm, RRM)),    //  8.20.1. Load Host Memory
(0x22, Instr(Mnemonic.smir, RR)),    //  8.19.6. Save Miscellaneous Register
(0x28, Instr(Mnemonic.sic, RR)),    //  8.19.1. Save Instruction Counter
(0x29, Instr(Mnemonic.sfr, RR)),    //  8.19.5. Save Flag Register
(0x2A, Instr(Mnemonic.spm, RR)),    //  8.19.3. Save Program Mode Flags
(0x2B, Instr(Mnemonic.bswp, RR)),    //  8.5.10. Byte Swap
(0x2D, Instr(Mnemonic.cvq, RW)),    //  8.7.17. Convert to Quadruple-format
(0x2E, Instr(Mnemonic.smvl, RR)),    //  8.18.3. Save Maximum Vector Length
(0x2F, Instr(Mnemonic.svl, RR)),    //  8.18.2. Save VL
(0x30, Instr(Mnemonic.svob, RR)),    //  8.3.2. Set Vector Out-of-order memory access Boundary
(0x31, Instr(Mnemonic.shm, RRM)),    //  8.20.2. Store Host Memory
(0x38, Instr(Mnemonic.pcnt, RR)),    //  8.5.8. Population Count
(0x39, Instr(Mnemonic.brv, RR)),    //  8.5.9. Bit Reverse
(0x3A, Instr(Mnemonic.lpm, RR)),    //  8.19.2. Load Program Mode Flags
(0x3B, Instr(Mnemonic.cmov, RR)),    //  8.5.11. Conditional Move
(0x3E, Instr(Mnemonic.fcm, RR)),    //  8.7.6. Floating Compare and Select Maximum/Minimum
(0x3F, Instr(Mnemonic.monc, RR)),    //  8.19.8. Monitor Call
(0x40, Instr(Mnemonic.lcr, RR)),    //  8.19.9. Load Communication Register
(0x41, Instr(Mnemonic.tscr, RR)),    //  8.19.11. Test & Set Communication Register
(0x42, Instr(Mnemonic.ts1am, RRM)),    //  8.2.16. Test and Set 1 AM
(0x43, Instr(Mnemonic.ts2am, RRM)),    //  8.2.17. Test and Set 2 AM
(0x44, Instr(Mnemonic.and, RR)),    //  8.5.1. AND
(0x45, Instr(Mnemonic.or, RR)),    //  8.5.2. OR
(0x46, Instr(Mnemonic.xor, RR)),    //  8.5.3. Exclusive OR
(0x47, Instr(Mnemonic.eqv, RR)),    //  8.5.4. Equivalence
(0x48, Instr(Mnemonic.add, RR)),    //  8.4.1. Add
(0x49, Instr(Mnemonic.mpy, RR)),    //  8.4.7. Multiply
(0x4A, Instr(Mnemonic.ads, RR)),    //  8.4.2. Add Single
(0x4B, Instr(Mnemonic.mps, RR)),    //  8.4.8. Multiply Single
(0x4C, Instr(Mnemonic.fad, RR)),    //  8.7.1. Floating Add
(0x4D, Instr(Mnemonic.fmp, RR)),    //  8.7.3. Floating Multiply
(0x4E, Instr(Mnemonic.fix, RR)),    //  8.7.11. Convert to Fixed Point
(0x4F, Instr(Mnemonic.fixx, RR)),    //  8.7.12. Convert to Fixed Point
(0x50, Instr(Mnemonic.scr, RR)),    //  8.19.10. Store Communication Register
(0x51, Instr(Mnemonic.fidcr, RR)),    //  8.19.12. Fetch & Increment/Decrement CR
(0x52, Instr(Mnemonic.ts3am, RRM)),    //  8.2.18. Test and Set 3 AM
(0x53, Instr(Mnemonic.atmam, RRM)),    //  8.2.19. Atomic AM
(0x54, Instr(Mnemonic.nnd, RR)),    //  8.5.5. Negate AND
(0x55, Instr(Mnemonic.cmp, RR)),    //  8.4.14. Compare
(0x56, Instr(Mnemonic.mrg, RR)),    //  8.5.6. Merge
(0x57, Instr(Mnemonic.slax, RR)),    //  8.6.6. Shift Left Arithmetic
(0x58, Instr(Mnemonic.sub, RR)),    //  8.4.4. Subtract
(0x59, Instr(Mnemonic.adx, RR)),    //  8.4.3. Add
(0x5A, Instr(Mnemonic.sbs, RR)),    //  8.4.5. Subtract Single
(0x5B, Instr(Mnemonic.sbx, RR)),    //  8.4.6. Subtract
(0x5C, Instr(Mnemonic.fsb, RR)),    //  8.7.2. Floating Subtract
(0x5D, Instr(Mnemonic.fdv, RR)),    //  8.7.4. Floating Divide
(0x5E, Instr(Mnemonic.flt, RR)),    //  8.7.13. Convert to Floating Point
(0x5F, Instr(Mnemonic.fltx, RR)),    //  8.7.14. Convert to Floating Point
(0x62, Instr(Mnemonic.cas, RRM)),    //  8.2.20. Compare and Swap
(0x64, Instr(Mnemonic.sld, RR)),    //  8.6.2. Shift Left Double
(0x65, Instr(Mnemonic.sll, RR)),    //  8.6.1. Shift Left Logical
(0x66, Instr(Mnemonic.sla, RR)),    //  8.6.5. Shift Left Arithmetic
(0x67, Instr(Mnemonic.ldz, RR)),    //  8.5.7. Leading Zero Count
(0x68, Instr(Mnemonic.cmx, RR)),    //  8.4.18. Compare and Select Maximum/Minimum
(0x69, Instr(Mnemonic.lfr, RR)),    //  8.19.4. Load Flag Register
(0x6A, Instr(Mnemonic.cpx, RR)),    //  8.4.16. Compare
(0x6B, Instr(Mnemonic.mpd, RR)),    //  8.4.10. Multiply
(0x6C, Instr(Mnemonic.faq, RW)),    //  8.7.7. Floating Add Quadruple
(0x6D, Instr(Mnemonic.fmq, RW)),    //  8.7.9. Floating Multiply Quadruple
(0x6E, Instr(Mnemonic.mpx, RR)),    //  8.4.9. Multiply
(0x6F, Instr(Mnemonic.div, RR)),    //  8.4.11. Divide
(0x74, Instr(Mnemonic.srd, RR)),    //  8.6.4. Shift Right Double
(0x75, Instr(Mnemonic.srl, RR)),    //  8.6.3. Shift Right Logical
(0x76, Instr(Mnemonic.sra, RR)),    //  8.6.7. Shift Right Arithmetic
(0x77, Instr(Mnemonic.srax, RR)),    //  8.6.8. Shift Right Arithmetic
(0x78, Instr(Mnemonic.cms, RR)),    //  8.4.17. Compare and Select Maximum/Minimum Single
(0x79, Instr(Mnemonic.nop, RR)),    //  8.19.7. No Operation
(0x7A, Instr(Mnemonic.cps, RR)),    //  8.4.15. Compare Single
(0x7B, Instr(Mnemonic.dvs, RR)),    //  8.4.12. Divide Single
(0x7C, Instr(Mnemonic.fsq, RW)),    //  8.7.8. Floating Subtract Quadruple
(0x7D, Instr(Mnemonic.fcq, RW)),    //  8.7.10. Floating Compare Quadruple
(0x7E, Instr(Mnemonic.fcp, RR)),    //  8.7.5. Floating Compare
(0x7F, Instr(Mnemonic.dvx, RR)),    //  8.4.13. Divide
(0x80, Instr(Mnemonic.pfchv, RVM)),    //  8.9.19. Pre FetCH Vector
(0x81, Instr(Mnemonic.vld, RVM)),    //  8.9.1. Vector Load
(0x82, Instr(Mnemonic.vldu, RVM)),    //  8.9.2. Vector Load Upper
(0x83, Instr(Mnemonic.vldl, RVM)),    //  8.9.3. Vector Load Lower
(0x84, Instr(Mnemonic.andm, RV)),    //  8.17.4. AND VM
(0x85, Instr(Mnemonic.orm, RV)),    //  8.17.5. OR VM
(0x86, Instr(Mnemonic.xorm, RV)),    //  8.17.6. Exclusive OR VM
(0x87, Instr(Mnemonic.eqvm, RV)),    //  8.17.7. Equivalence VM
(0x88, Instr(Mnemonic.vrand, RV)),    //  8.14.7. Vector Reduction AND
(0x89, Instr(Mnemonic.vrxor, RV)),    //  8.14.9. Vector Reduction Exclusive OR
(0x8A, Instr(Mnemonic.vcms, RV)),    //  8.10.17. Vector Compare and Select Maximum/Minimum Single
(0x8B, Instr(Mnemonic.vadx, RV)),    //  8.10.3. Vector Add
(0x8C, Instr(Mnemonic.vbrd, RV)),    //  8.9.24. Vector Broadcast
(0x8D, Instr(Mnemonic.vcp, RV)),    //  8.16.3. Vector Compress
(0x8E, Instr(Mnemonic.lsv, RR)),    //  8.9.20. Load S to V
(0x8F, Instr(Mnemonic.vcvd, RV)),    //  8.13.19. Vector Convert to Double-format
(0x91, Instr(Mnemonic.vst, RVM)),    //  8.9.7. Vector Store
(0x92, Instr(Mnemonic.vstu, RVM)),    //  8.9.8. Vector Store Upper
(0x93, Instr(Mnemonic.vstl, RVM)),    //  8.9.9. Vector Store Lower
(0x94, Instr(Mnemonic.nndm, RV)),    //  8.17.8. Negate AND VM
(0x95, Instr(Mnemonic.negm, RV)),    //  8.17.9. Negate VM
(0x98, Instr(Mnemonic.vror, RV)),    //  8.14.8. Vector Reduction OR
(0x99, Instr(Mnemonic.vseq, RV)),    //  8.11.8. Vector Sequential Number
(0x9A, Instr(Mnemonic.vcmx, RV)),    //  8.10.18. Vector Compare and Select Maximum/Minimum
(0x9B, Instr(Mnemonic.vsbx, RV)),    //  8.10.6. Vector Subtract
(0x9C, Instr(Mnemonic.vmv, RV)),    //  8.9.25. Vector Move
(0x9D, Instr(Mnemonic.vex, RV)),    //  8.16.4. Vector Expand
(0x9E, Instr(Mnemonic.lvs, RR)),    //  8.9.21. Load V to S
(0x9F, Instr(Mnemonic.vcvs, RV)),    //  8.13.18. Vector Convert to Single-format
(0xa1, Instr(Mnemonic.vgt, RVM)),    //  8.9.13. Vector Gather
(0xA2, Instr(Mnemonic.vgtu, RVM)),    //  8.9.14. Vector Gather Upper
(0xA3, Instr(Mnemonic.vgtl, RVM)),    //  8.9.15. Vector Gather Lower
(0xA4, Instr(Mnemonic.pcvm, RV)),    //  8.17.10. Population Count of VM
(0xA5, Instr(Mnemonic.lzvm, RV)),    //  8.17.11. Leading Zero of VM
(0xA6, Instr(Mnemonic.tovm, RV)),    //  8.17.12. Trailing One of VM
(0xa7, Instr(Mnemonic.svm, RR)),    //  8.9.23. Save VM
(0xA8, Instr(Mnemonic.vfixx, RV)),    //  8.13.15. Vector Convert to Fixed Point
(0xAA, Instr(Mnemonic.vsumx, RV)),    //  8.14.2. Vector Sum
(0xAB, Instr(Mnemonic.vmaxx, RV)),    //  8.14.5. Vector Maximum/Minimum
(0xAC, Instr(Mnemonic.vpcnt, RV)),    //  8.11.6. Vector Population Count
(0xAD, Instr(Mnemonic.vfmax, RV)),    //  8.14.6. Vector Floating Maximum/Minimum
(0xAF, Instr(Mnemonic.lvix, RR)),    //  8.18.4. Load Vector Data Index
(0xb1, Instr(Mnemonic.vsc, RVM)),    //  8.9.16. Vector Scatter
(0xB2, Instr(Mnemonic.vscu, RVM)),    //  8.9.17. Vector Scatter Upper
(0xB3, Instr(Mnemonic.vscl, RVM)),    //  8.9.18. Vector Scatter Lower
(0xB4, Instr(Mnemonic.vfmk, RV)),    //  8.17.1. Vector Form Mask
(0xB5, Instr(Mnemonic.vfms, RV)),    //  8.17.2. Vector Form Mask Single
(0xB6, Instr(Mnemonic.vfmf, RV)),    //  8.17.3. Vector Form Mask Floating Point
(0xb7, Instr(Mnemonic.lvm, RR)),    //  8.9.22. Load VM
(0xB8, Instr(Mnemonic.vfltx, RV)),    //  8.13.17. Vector Convert to Floating Point
(0xB9, Instr(Mnemonic.vcmp, RV)),    //  8.10.14. Vector Compare
(0xBA, Instr(Mnemonic.vcpx, RV)),    //  8.10.16. Vector Compare
(0xBB, Instr(Mnemonic.vmaxs, RV)),    //  8.14.4. Vector Maximum/Minimum Single
(0xBC, Instr(Mnemonic.vshf, RV)),    //  8.16.2. Vector Shuffle
(0xBD, Instr(Mnemonic.vfcm, RV)),    //  8.13.7. Vector Floating Compare and Select Maximum/Minimum
(0xbF, Instr(Mnemonic.lvl, RR)),    //  8.18.1. Load VL
(0xC1, Instr(Mnemonic.vld2d, RVM)),    //  8.9.4. Vector Load 2D 
(0xC2, Instr(Mnemonic.vldu2d, RVM)),    //  8.9.5. Vector Load Upper 2D 
(0xC3, Instr(Mnemonic.vldl2d, RVM)),    //  8.9.6. Vector Load Lower 2D
(0xC4, Instr(Mnemonic.vand, RV)),    //  8.11.1. Vector And
(0xc5, Instr(Mnemonic.vor, RV)),    //  8.11.2. Vector OR
(0xC6, Instr(Mnemonic.vxor, RV)),    //  8.11.3. Vector Exclusive OR
(0xC7, Instr(Mnemonic.veqv, RV)),    //  8.11.4. Vector Equivalence
(0xC8, Instr(Mnemonic.vadd, RV)),    //  8.10.1. Vector Add
(0xC9, Instr(Mnemonic.vmpy, RV)),    //  8.10.7. Vector Multiply
(0xCA, Instr(Mnemonic.vads, RV)),    //  8.10.2. Vector Add Single
(0xCB, Instr(Mnemonic.vmps, RV)),    //  8.10.8. Vector Multiply Single
(0xCC, Instr(Mnemonic.vfad, RV)),    //  8.13.1. Vector Floating Add
(0xCD, Instr(Mnemonic.vfmp, RV)),    //  8.13.3. Vector Floating Multiply
(0xCE, Instr(Mnemonic.vfia, RV)),    //  8.15.1. Vector Floating Iteration Add
(0xCF, Instr(Mnemonic.vfim, RV)),    //  8.15.3. Vector Floating Iteration Multiply
(0xD1, Instr(Mnemonic.vst2d, RVM)),    //  8.9.10. Vector Store 2D 
(0xD2, Instr(Mnemonic.vstu2d, RVM)),    //  8.9.11. Vector Store Upper 2D 
(0xD3, Instr(Mnemonic.vstl2d, RVM)),    //  8.9.12. Vector Store Lower 2D
(0xD4, Instr(Mnemonic.vslax, RV)),    //  8.12.6. Vector Shift Left Arithmetic
(0xD5, Instr(Mnemonic.vsrax, RV)),    //  8.12.8. Vector Shift Right Arithmetic
(0xD6, Instr(Mnemonic.vmrg, RV)),    //  8.16.1. Vector Merge
(0xD7, Instr(Mnemonic.vsfa, RV)),    //  8.12.9. Vector Shift Left and Add
(0xD8, Instr(Mnemonic.vsub, RV)),    //  8.10.4. Vector Subtract
(0xD9, Instr(Mnemonic.vmpd, RV)),    //  8.10.10. Vector Multiply
(0xDA, Instr(Mnemonic.vsbs, RV)),    //  8.10.5. Vector Subtract Single
(0xDB, Instr(Mnemonic.vmpx, RV)),    //  8.10.9. Vector Multiply
(0xDC, Instr(Mnemonic.vfsb, RV)),    //  8.13.2. Vector Floating Subtract
(0xDD, Instr(Mnemonic.vfdv, RV)),    //  8.13.4. Vector Floating Divide
(0xDE, Instr(Mnemonic.vfis, RV)),    //  8.15.2. Vector Floating Iteration Subtract
(0xE1, Instr(Mnemonic.vrcp, RV)),    //  8.13.12. Vector floating Reciprocal
(0xE2, Instr(Mnemonic.vfmad, RV)),    //  8.13.8. Vector Floating Fused Multiply Add
(0xE3, Instr(Mnemonic.vfnmad, RV)),    //  8.13.10. Vector Floating Fused Negative Multiply Add
(0xE4, Instr(Mnemonic.vsld, RV)),    //  8.12.2. Vector Shift Left Double
(0xE5, Instr(Mnemonic.vsll, RV)),    //  8.12.1. Vector Shift Left Logical
(0xE6, Instr(Mnemonic.vsla, RV)),    //  8.12.5. Vector Shift Left Arithmetic
(0xE7, Instr(Mnemonic.vldz, RV)),    //  8.11.5. Vector Leading Zero Count
(0xE8, Instr(Mnemonic.vfix, RV)),    //  8.13.14. Vector Convert to Fixed Point
(0xE9, Instr(Mnemonic.vdiv, RV)),    //  8.10.11. Vector Divide
(0xEA, Instr(Mnemonic.vsums, RV)),    //  8.14.1. Vector Sum Single
(0xEB, Instr(Mnemonic.vdvs, RV)),    //  8.10.12. Vector Divide Single
(0xEC, Instr(Mnemonic.vfsum, RV)),    //  8.14.3. Vector Floating Sum
(0xED, Instr(Mnemonic.vfsqrt, RV)),    //  8.13.5. Vector floating Square Root
(0xEE, Instr(Mnemonic.vfiam, RV)),    //  8.15.4. Vector Floating Iteration Add and Multiply
(0xEF, Instr(Mnemonic.vfima, RV)),    //  8.15.6. Vector Floating Iteration Multiply and Add
(0xF1, Instr(Mnemonic.vrsqrt, RV)),    //  8.13.13. Vector floating Reciprocal Square Root
(0xF2, Instr(Mnemonic.vfmsb, RV)),    //  8.13.9. Vector Floating Fused Multiply Subtract
(0xF3, Instr(Mnemonic.vfnmsb, RV)),    //  8.13.11. Vector Floating Fused Negative Multiply Subtract
(0xF4, Instr(Mnemonic.vsrd, RV)),    //  8.12.4. Vector Shift Right Double
(0xF5, Instr(Mnemonic.vsrl, RV)),    //  8.12.3. Vector Shift Right Logical
(0xF6, Instr(Mnemonic.vsra, RV)),    //  8.12.7. Vector Shift Right Arithmetic
(0xF7, Instr(Mnemonic.vbrv, RV)),    //  8.11.7. Vector Bit Reverse
(0xF8, Instr(Mnemonic.vflt, RV)),    //  8.13.16. Vector Convert to Floating Point
(0xFA, Instr(Mnemonic.vcps, RV)),    //  8.10.15. Vector Compare Single
(0xFB, Instr(Mnemonic.vdvx, RV)),    //  8.10.13. Vector Divide
(0xFC, Instr(Mnemonic.vfcp, RV)),    //  8.13.6. Vector Floating Compare
(0xFE, Instr(Mnemonic.vfism, RV)),    //  8.15.5. Vector Floating Iteration Subtract and Multiply
(0xFF, Instr(Mnemonic.vfims, RV)));    //  8.15.7. Vector Floating Iteration Multiply and Subtract
}
}