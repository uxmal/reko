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
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Services;
using System.Collections.Generic;

namespace Reko.Arch.Infineon.M8C;

public class M8CDisassembler : DisassemblerBase<M8CInstruction, Mnemonic>
{
    private static readonly Decoder<M8CDisassembler, Mnemonic, M8CInstruction> rootDecoder;

    private readonly M8CArchitecture arch;
    private readonly EndianImageReader rdr;
    private readonly List<MachineOperand> ops;
    private Address addr;

    public M8CDisassembler(M8CArchitecture arch, EndianImageReader rdr)
    {
        this.arch = arch;
        this.rdr = rdr;
        this.ops = [];
    }

    public override M8CInstruction CreateInvalidInstruction()
    {
        return new M8CInstruction { InstructionClass = InstrClass.Invalid, Mnemonic = Mnemonic.invalid };
    }

    public override M8CInstruction? DisassembleInstruction()
    {
        this.addr = rdr.Address;
        if (!rdr.TryReadByte(out var opcode))
            return null;
        var instr = rootDecoder.Decode(opcode, this);
        instr.Address = addr;
        instr.Length = (int) (rdr.Address - addr);
        ops.Clear();
        return instr;
    }

    public override M8CInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
    {
        return new M8CInstruction
        {
            InstructionClass = iclass,
            Mnemonic = mnemonic,
            Operands = ops.ToArray()
        };
    }

    public override M8CInstruction NotYetImplemented(string message)
    {
        var svc = arch.Services.GetService<ITestGenerationService>();
        svc?.ReportMissingDecoder("M8cDasm", this.addr, this.rdr, message);
        return new M8CInstruction {  InstructionClass = InstrClass.Invalid, Mnemonic = Mnemonic.invalid  };
    }

    private static bool A(uint uInstr, M8CDisassembler dasm)
    {
        dasm.ops.Add(Registers.A);
        return true;
    }

    private static bool X(uint uInstr, M8CDisassembler dasm)
    {
        dasm.ops.Add(Registers.X);
        return true;
    }

    private static bool SP(uint uInstr, M8CDisassembler dasm)
    {
        dasm.ops.Add(Registers.SP);
        return true;
    }

    private static bool F(uint uInstr, M8CDisassembler dasm)
    {
        dasm.ops.Add(Registers.F);
        return true;
    }

    private static bool Mem(uint uInstr, M8CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out var uAddr))
            return false;
        var mem = MemoryOperand.Direct(uAddr);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool MemX(uint uInstr, M8CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out var uAddr))
            return false;
        var mem = MemoryOperand.DirectIndexed(Registers.X, uAddr);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool regMem(uint uInstr, M8CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out var uAddr))
            return false;
        var mem = MemoryOperand.RegDirect(uAddr);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool regMemX(uint uInstr, M8CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out var uAddr))
            return false;
        var mem = MemoryOperand.RegDirectIndexed(Registers.X, uAddr);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool IndirectPostInc(uint uInstr, M8CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out var uAddr))
            return false;
        var mem = MemoryOperand.IndirectPostinc(uAddr);
        dasm.ops.Add(mem);
        return true;
    }

    private static bool Imm(uint uInstr, M8CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out var imm))
            return false;
        dasm.ops.Add(Constant.Byte(imm));
        return true;
    }


    private static bool Addr(uint uInstr, M8CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadBeUInt16(out var uAddr))
            return false;
        dasm.ops.Add(Address.Ptr16(uAddr));
        return true;
    }


    /// <summary>
    /// 12-bit displacement.
    /// </summary>
    private static bool D12(uint uInstr, M8CDisassembler dasm)
    {
        if (!dasm.rdr.TryReadByte(out var lo))
            return false;
        var offset = (Bits.SignExtend(uInstr, 4) << 8) | lo;
        var addr = dasm.addr + (offset + 1);
        dasm.ops.Add(addr);
        return true;
    }

    private static Decoder<M8CDisassembler, Mnemonic, M8CInstruction> Instr(Mnemonic mnemonic, InstrClass iclass, params Mutator<M8CDisassembler>[] mutators)
    {
        return new InstrDecoder<M8CDisassembler, Mnemonic, M8CInstruction>(iclass, mnemonic, mutators);
    }

    private static Decoder<M8CDisassembler, Mnemonic, M8CInstruction> Instr(Mnemonic mnemonic, params Mutator<M8CDisassembler>[] mutators)
    {
        return new InstrDecoder<M8CDisassembler, Mnemonic, M8CInstruction>(InstrClass.Linear, mnemonic, mutators);
    }


    static M8CDisassembler()
    {
        var jmp = Instr(Mnemonic.jmp, InstrClass.Transfer, D12);                // 8x 5 2 JMP
        var call = Instr(Mnemonic.call, InstrClass.ConditionalTransfer, D12);   // 9x 11 2 CALL
        var jz = Instr(Mnemonic.jz, InstrClass.ConditionalTransfer, D12);       // Ax 5 2 JZ
        var jnz = Instr(Mnemonic.jnz, InstrClass.ConditionalTransfer, D12);     // Bx 5 2 JNZ
        var jc = Instr(Mnemonic.jc, InstrClass.ConditionalTransfer, D12);       // Cx 5 2 JC
        var jnc = Instr(Mnemonic.jnc, InstrClass.ConditionalTransfer, D12);     // Dx 5 2 JNC
        var jacc = Instr(Mnemonic.jacc, InstrClass.Transfer, D12);              // Ex 7 2 JACC
        var index = Instr(Mnemonic.index, D12);                                 // Fx 13 2 INDEX Z

        rootDecoder = Mask(0, 8, "m8c",
            Instr(Mnemonic.ssc, InstrClass.Transfer|InstrClass.Call),   // 15 1 SSC                         
            Instr(Mnemonic.add, A, Imm),   // 4 2 ADD  A, expr C,   Z          
            Instr(Mnemonic.add, A, Mem),   // 6 2 ADD  A, [expr] C, Z          
            Instr(Mnemonic.add, A, MemX),   // 7 2 ADD  A, [X + expr] C, Z      
            Instr(Mnemonic.add, Mem, A ),   // 7 2 ADD [expr], A C, Z           
            Instr(Mnemonic.add, MemX, A ),   // 8 2 ADD [X + expr], A C, Z       
            Instr(Mnemonic.add, Mem, Imm),   // 9 3 ADD [expr], expr C, Z        
            Instr(Mnemonic.add, MemX, Imm),   // 10 3 ADD [X + expr], expr C, Z   

            Instr(Mnemonic.push, A),   // 4 1 PUSH  A                      
            Instr(Mnemonic.adc, A, Imm),   // 4 2 ADC  A, expr C, Z            
            Instr(Mnemonic.adc, A, Mem),   // 6 2 ADC  A, [expr] C, Z          
            Instr(Mnemonic.adc, A, MemX),   // 7 2 ADC  A, [X + expr] C, Z      
            Instr(Mnemonic.adc, Mem, A ),   // 7 2 ADC [expr], A C, Z           
            Instr(Mnemonic.adc, MemX, A ),  // 8 2 ADC [X + expr], A C, Z       
            Instr(Mnemonic.adc, Mem, Imm),  // 9 3 ADC [expr], expr C, Z        
            Instr(Mnemonic.adc, MemX, Imm),  // 10 3 ADC [X + expr], expr C, Z   

            Instr(Mnemonic.push, X),  // 4 1 PUSH  X                      
            Instr(Mnemonic.sub, A, Imm),  // 4 2 SUB  A, expr C, Z            
            Instr(Mnemonic.sub, A, Mem),  // 6 2 SUB  A, [expr] C, Z          
            Instr(Mnemonic.sub, A, MemX),  // 7 2 SUB  A, [X + expr] C, Z      
            Instr(Mnemonic.sub, Mem, A ),  // 7 2 SUB [expr], A C, Z           
            Instr(Mnemonic.sub, MemX, A ),  // 8 2 SUB [X + expr], A C, Z       
            Instr(Mnemonic.sub, Mem, Imm),  // 9 3 SUB [expr], expr C, Z        
            Instr(Mnemonic.sub, MemX, Imm),  // 10 3 SUB [X + expr], expr C, Z   

            Instr(Mnemonic.pop, A),  // 5 1 POP  A Z                     
            Instr(Mnemonic.sbb, A, Imm),  // 4 2 SBB  A, expr C, Z            
            Instr(Mnemonic.sbb, A, Mem),  // 6 2 SBB  A, [expr] C, Z          
            Instr(Mnemonic.sbb, A, MemX),  // 7 2 SBB  A, [X + expr] C, Z      
            Instr(Mnemonic.sbb, Mem, A ),  // 7 2 SBB [expr], A C, Z           
            Instr(Mnemonic.sbb, MemX, A ),  // 8 2 SBB [X + expr], A C, Z       
            Instr(Mnemonic.sbb, Mem, Imm),  // 9 3 SBB [expr], expr C, Z        
            Instr(Mnemonic.sbb, MemX, Imm),  // 10 3 SBB [X + expr], expr C, Z   

            Instr(Mnemonic.pop, X),  // 5 1 POP  X                       
            Instr(Mnemonic.and, A, Imm),  // 4 2 AND  A, expr Z               
            Instr(Mnemonic.and, A, Mem),  // 6 2 AND  A, [expr] Z             
            Instr(Mnemonic.and, A, MemX),  // 7 2 AND  A, [X + expr] Z         
            Instr(Mnemonic.and, Mem, A ),  // 7 2 AND [expr], A Z              
            Instr(Mnemonic.and, MemX, A ),  // 8 2 AND [X + expr], A Z          
            Instr(Mnemonic.and, Mem, Imm),  // 9 3 AND [expr], expr Z           
            Instr(Mnemonic.and, MemX, Imm),  // 10 3 AND [X + expr], expr Z      

            Instr(Mnemonic.romx),  // 11 1 ROMX  Z                     
            Instr(Mnemonic.or, A, Imm),  // 4 2 OR  A, expr Z                
            Instr(Mnemonic.or, A, Mem),  // 6 2 OR  A, [expr] Z              
            Instr(Mnemonic.or, A, MemX),  // 7 2 OR  A, [X + expr] Z          
            Instr(Mnemonic.or, Mem, A ),  // 7 2 OR [expr], A Z               
            Instr(Mnemonic.or, MemX, A ),  // 8 2 OR [X + expr], A Z           
            Instr(Mnemonic.or, Mem, Imm),  // 9 3 OR [expr], expr Z            
            Instr(Mnemonic.or, MemX, Imm),  // 10 3 OR [X + expr], expr Z       

            Instr(Mnemonic.halt, InstrClass.Terminates),  // 9 1 HALT                         
            Instr(Mnemonic.xor, A, Imm),  // 4 2 XOR  A, expr Z               
            Instr(Mnemonic.xor, A, Mem),  // 6 2 XOR  A, [expr] Z             
            Instr(Mnemonic.xor, A, MemX),  // 7 2 XOR  A, [X + expr] Z         
            Instr(Mnemonic.xor, Mem, A ),  // 7 2 XOR [expr], A Z              
            Instr(Mnemonic.xor, MemX, A ),  // 8 2 XOR [X + expr], A Z          
            Instr(Mnemonic.xor, Mem, Imm),  // 9 3 XOR [expr], expr Z           
            Instr(Mnemonic.xor, MemX, Imm),  // 10 3 XOR [X + expr], expr Z      

            Instr(Mnemonic.add, SP, Imm),  // 5 2 ADD  SP, expr                
            Instr(Mnemonic.cmp, A, Imm), // if (A = B) Z = 1 if (A < B) C =),  // 5 2 CMP  A, expr if (A = B) Z = 1 if (A < B) C =1
            Instr(Mnemonic.cmp, A, Mem),  // 7 2 CMP  A, [expr]               
            Instr(Mnemonic.cmp, A, MemX),  // 8 2 CMP  A, [X + expr]           
            Instr(Mnemonic.cmp, Mem, Imm),  // 8 3 CMP [expr], expr             
            Instr(Mnemonic.cmp, MemX, Imm),  // 9 3 CMP [X + expr], expr         
            Instr(Mnemonic.mvi, A, IndirectPostInc),  // 10 2 MVI  A, [[expr]++] Z        
            Instr(Mnemonic.mvi, IndirectPostInc, A),  // 10 2 MVI [[expr]++], A           

            Instr(Mnemonic.nop, InstrClass.Linear|InstrClass.Padding),  // 4 1 NOP                          
            Instr(Mnemonic.and, regMem, Imm),  // 9 3 AND  reg[expr], expr Z       
            Instr(Mnemonic.and, regMemX, Imm),  // 10 3 AND  reg[X + expr], expr Z  
            Instr(Mnemonic.or, regMem, Imm),  // 9 3 OR  reg[expr], expr Z        
            Instr(Mnemonic.or, regMemX, Imm),  // 10 3 OR  reg[X + expr], expr Z   
            Instr(Mnemonic.xor, regMem, Imm),  // 9 3 XOR  reg[expr], expr Z       
            Instr(Mnemonic.xor, regMemX, Imm),  // 10 3 XOR  reg[X + expr], expr Z  
            Instr(Mnemonic.tst, Mem, Imm),  // 8 3 TST [expr], expr Z           
            Instr(Mnemonic.tst, MemX, Imm),  // 9 3 TST [X + expr], expr Z       
            Instr(Mnemonic.tst, regMem, Imm),  // 9 3 TST  reg[expr], expr Z       
            Instr(Mnemonic.tst, regMemX, Imm),  // 10 3 TST  reg[X + expr], expr Z  
            Instr(Mnemonic.swap, A, X ),  // 5 1 SWAP  A, X Z                 
            Instr(Mnemonic.swap, A, Mem),  // 7 2 SWAP  A, [expr] Z            
            Instr(Mnemonic.swap, X, Mem),  // 7 2 SWAP  X, [expr]              
            Instr(Mnemonic.swap, A, SP ),  // 5 1 SWAP  A, SP Z                
            Instr(Mnemonic.mov, X, SP),  // 4 1 MOV  X, SP                   
            Instr(Mnemonic.mov, A, Imm),  // 4 2 MOV  A, expr Z               
            Instr(Mnemonic.mov, A, Mem),  // 5 2 MOV  A, [expr] Z             
            Instr(Mnemonic.mov, A, MemX),  // 6 2 MOV  A, [X + expr] Z         
            Instr(Mnemonic.mov, Mem, A),  // 5 2 MOV [expr], A                
            Instr(Mnemonic.mov, MemX, A),  // 6 2 MOV [X + expr], A            
            Instr(Mnemonic.mov, Mem, Imm),  // 8 3 MOV [expr], expr             
            Instr(Mnemonic.mov, MemX, Imm),  // 9 3 MOV [X + expr], expr         
            Instr(Mnemonic.mov, X, Imm),  // 4 2 MOV  X, expr                 
            Instr(Mnemonic.mov, X, Mem),  // 6 2 MOV  X, [expr]               
            Instr(Mnemonic.mov, X, MemX),  // 7 2 MOV  X, [X + expr]           
            Instr(Mnemonic.mov, Mem),  // 5 2 MOV [expr],X
            Instr(Mnemonic.mov, A, X),  // 4 1 MOV  A, XZ
            Instr(Mnemonic.mov, X, A),  // 4 1 MOV  X,A
            Instr(Mnemonic.mov, A, regMem),  // 6 2 MOV  A, reg[expr]Z
            Instr(Mnemonic.mov, A, regMemX),  // 7 2 MOV  A, reg[X + expr]Z
            Instr(Mnemonic.mov, Mem, Mem),  // 10 3 MOV [expr],[expr]
            Instr(Mnemonic.mov, regMem, A),  // 5 2 MOV  reg[expr],A
            Instr(Mnemonic.mov, regMemX, A),  // 6 2 MOV  reg[X + expr],A
            Instr(Mnemonic.mov, regMem, Imm),  // 8 3 MOV  reg[expr],expr
            Instr(Mnemonic.mov, regMemX, Imm),  // 9 3 MOV  reg[X + expr],expr
            Instr(Mnemonic.asl, A),  // 4 1 ASL  A C,Z
            Instr(Mnemonic.asl, Mem),  // 7 2 ASL [expr] C,Z
            Instr(Mnemonic.asl, MemX),  // 8 2 ASL [X + expr] C,Z
            Instr(Mnemonic.asr, A),  // 4 1 ASR  A C,Z

            Instr(Mnemonic.asr, Mem),  // 7 2 ASR [expr] C,Z
            Instr(Mnemonic.asr, MemX),  // 8 2 ASR [X + expr] C,Z
            Instr(Mnemonic.rlc, A),  // 4 1 RLC  A C,Z
            Instr(Mnemonic.rlc, Mem),  // 7 2 RLC [expr] C,Z
            Instr(Mnemonic.rlc, MemX),  // 8 2 RLC [X + expr] C,Z
            Instr(Mnemonic.rrc, A),  // 4 1 RRC  A C,Z
            Instr(Mnemonic.rrc, Mem),  // 7 2 RRC [expr] C,Z
            Instr(Mnemonic.rrc, MemX),  // 8 2 RRC [X + expr] C,Z

            Instr(Mnemonic.and, F, Imm),  // 4 2 AND  F, expr C,Z
            Instr(Mnemonic.or, F, Imm),  // 4 2 OR  F, expr C,Z
            Instr(Mnemonic.xor, F, Imm),  // 4 2 XOR  F, expr C,Z
            Instr(Mnemonic.cpl, A),  // 4 1 CPL  AZ
            Instr(Mnemonic.inc, A),  // 4 1 INC  A C,Z
            Instr(Mnemonic.inc, X),  // 4 1 INC  X C,Z
            Instr(Mnemonic.inc, Mem),  // 7 2 INC [expr] C,Z
            Instr(Mnemonic.inc, MemX),  // 8 2 INC [X + expr] C,Z

            Instr(Mnemonic.dec, A),  // 4 1 DEC  A C,Z
            Instr(Mnemonic.dec, X),     // 4 1 DEC X C, Z
            Instr(Mnemonic.dec, Mem),  // 7 2 DEC [expr] C,Z
            Instr(Mnemonic.dec, MemX),  // 8 2 DEC [X + expr] C,Z
            Instr(Mnemonic.lcall, Addr),  // 7C 13 3 LCALL
            Instr(Mnemonic.ljmp, Addr),    // 7D 7 3 LJMP
            Instr(Mnemonic.reti, InstrClass.Transfer|InstrClass.Return),  // 10 1 RETI  C,Z
            Instr(Mnemonic.ret, InstrClass.Transfer | InstrClass.Return),  // 7F 8 1 RET

            jmp,
            jmp,
            jmp,
            jmp,

            jmp,
            jmp,
            jmp,
            jmp,

            jmp,
            jmp,
            jmp,
            jmp,
            
            jmp,
            jmp,
            jmp,
            jmp,

            call,
            call,
            call,
            call,
            
            call,
            call,
            call,
            call,

            call,
            call,
            call,
            call,
            
            call,
            call,
            call,
            call,

            jz,
            jz,
            jz,
            jz,
            
            jz,
            jz,
            jz,
            jz,

            jz,
            jz,
            jz,
            jz,
            
            jz,
            jz,
            jz,
            jz,

            jnz,
            jnz,
            jnz,
            jnz,
            
            jnz,
            jnz,
            jnz,
            jnz,

            jnz,
            jnz,
            jnz,
            jnz,
            
            jnz,
            jnz,
            jnz,
            jnz,

            jc,
            jc,
            jc,
            jc,
            
            jc,
            jc,
            jc,
            jc,

            jc,
            jc,
            jc,
            jc,
            
            jc,
            jc,
            jc,
            jc,

            jnc,
            jnc,
            jnc,
            jnc,
            
            jnc,
            jnc,
            jnc,
            jnc,

            jnc,
            jnc,
            jnc,
            jnc,
            
            jnc,
            jnc,
            jnc,
            jnc,

            jacc,
            jacc,
            jacc,
            jacc,
            jacc,
            jacc,
            jacc,
            jacc,

            jacc,
            jacc,
            jacc,
            jacc,
            jacc,
            jacc,
            jacc,
            jacc,

            index,
            index,
            index,
            index,
            index,
            index,
            index,
            index,

            index,
            index,
            index,
            index,
            index,
            index,
            index,
            index);
    }
}
