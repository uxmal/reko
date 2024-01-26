#region License
#
# Copyright (C) 1999-2023 Gregor Alujevič.
#
# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2, or (at your option)
# any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; see the file COPYING.  If not, write to
# the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
#
#endregion


.section .init
.global _start
.align 2 # align code on 2^n => ( 4 bytes )
.attribute      5, "rv64gaimdfqh_zicsr_zfh_zihintpause_svinval_zmmul_zfa"
_start:

#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV32I Base Instruction Set
#════════════════════════════════════════════════════════════════════════════════════════════════════
lui zero,0x80000                                          #  LUI                  U                  //    imm[31:12] rd 0110111
auipc s3,0xfffb                                           #  AUIPC                U                  //    imm[31:12] rd 0010111
#----------------------------------------------------------------------------------------------------
#jal ra,0x800000e6                                         #  JAL                  J                  //    imm[20|10:1|11|19:12] rd 1101111
#----------------------------------------------------------------------------------------------------
#jalr tp,5(s10)                                            #  JALR                 I                  //    imm[11:0] rs1 000 rd 1100111
#----------------------------------------------------------------------------------------------------
#beq a0,a6,0x800091ac                                      #  BEQ                  B                  //    imm[12|10:5] rs2 rs1 000 imm[4:1|11] 1100011
#bne s4,s3,0x8000926a                                      #  BNE                  B                  //    imm[12|10:5] rs2 rs1 001 imm[4:1|11] 1100011
#blt s0,t3,0x80002da4                                      #  BLT                  B                  //    imm[12|10:5] rs2 rs1 100 imm[4:1|11] 1100011
#bge s0,a2,0x80002ca8                                      #  BGE                  B                  //    imm[12|10:5] rs2 rs1 101 imm[4:1|11] 1100011
#bltu a4,a0,0x800018f2                                     #  BLTU                 B                  //    imm[12|10:5] rs2 rs1 110 imm[4:1|11] 1100011
#bgeu a0,t0,0x800012c2                                     #  BGEU                 B                  //    imm[12|10:5] rs2 rs1 111 imm[4:1|11] 1100011
#----------------------------------------------------------------------------------------------------
lb sp,128(s2)                                             #  LB                   I                  //    imm[11:0] rs1 000 rd 0000011
lh t6,2028(t5)                                            #  LH                   I                  //    imm[11:0] rs1 001 rd 0000011
lw s4,2024(s3)                                            #  LW                   I                  //    imm[11:0] rs1 010 rd 0000011
lbu a5,-677(a5)                                           #  LBU                  I                  //    imm[11:0] rs1 100 rd 0000011
lhu s11,1572(sp)                                          #  LHU                  I                  //    imm[11:0] rs1 101 rd 0000011
#----------------------------------------------------------------------------------------------------
sb a5,-715(a4)                                            #  SB                   S                  //    imm[11:5] rs2 rs1 000 imm[4:0] 0100011
sh a0,2028(s1)                                            #  SH                   S                  //    imm[11:5] rs2 rs1 001 imm[4:0] 0100011
sw s2,2016(sp)                                            #  SW                   S                  //    imm[11:5] rs2 rs1 010 imm[4:0] 0100011
#----------------------------------------------------------------------------------------------------
addi t5,a5,-212                                           #  ADDI                 I                  //    imm[11:0] rs1 000 rd 0010011
slti a1,t3,-1479                                          #  SLTI                 I                  //    imm[11:0] rs1 010 rd 0010011
sltiu s4,s6,944                                           #  SLTIU                I                  //    imm[11:0] rs1 011 rd 0010011
xori s0,t1,1021                                           #  XORI                 I                  //    imm[11:0] rs1 100 rd 0010011
ori a4,s6,1024                                            #  ORI                  I                  //    imm[11:0] rs1 110 rd 0010011
andi t2,a6,1792                                           #  ANDI                 I                  //    imm[11:0] rs1 111 rd 0010011
#----------------------------------------------------------------------------------------------------
slli a0,s6,0x10                                           #  SLLI                 R                  //    0000000 shamt rs1 001 rd 0010011
srli s8,s9,0x17                                           #  SRLI                 R                  //    0000000 shamt rs1 101 rd 0010011
srai s8,s11,0x1f                                          #  SRAI                 R                  //    0100000 shamt rs1 101 rd 0010011
add t6,a7,t5                                              #  ADD                  R                  //    0000000 rs2 rs1 000 rd 0110011
sub s11,s7,t1                                             #  SUB                  R                  //    0100000 rs2 rs1 000 rd 0110011
sll s11,s7,t1                                             #  SLL                  R                  //    0000000 rs2 rs1 001 rd 0110011
slt t2,t4,t6                                              #  SLT                  R                  //    0000000 rs2 rs1 010 rd 0110011
sltu a3,s6,a7                                             #  SLTU                 R                  //    0000000 rs2 rs1 011 rd 0110011
xor t5,t6,t3                                              #  XOR                  R                  //    0000000 rs2 rs1 100 rd 0110011
srl t5,t6,t3                                              #  SRL                  R                  //    0000000 rs2 rs1 101 rd 0110011
sra t6,s3,sp                                              #  SRA                  R                  //    0100000 rs2 rs1 101 rd 0110011
or a1,a2,t5                                               #  OR                   R                  //    0000000 rs2 rs1 110 rd 0110011
and s5,s7,t4                                              #  AND                  R                  //    0000000 rs2 rs1 111 rd 0110011
#----------------------------------------------------------------------------------------------------
fence w,i                                                 #  FENCE                I                  //    fm pred succ rs1 000 rd 0001111
fence.tso                                                 #  FENCE.TSO            I                  //    1000 0011 0011 00000 000 00000 0001111
pause                                                     #  PAUSE                I                  //    0000 0001 0000 00000 000 00000 0001111
ecall                                                     #  ECALL                I                  //    000000000000 00000 000 00000 1110011
ebreak                                                    #  EBREAK               I                  //    000000000001 00000 000 00000 1110011

#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV32  3.3 Machine-Mode Privileged Instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════

# Trap-Return Instructions
#uret                                                      #  URET                 I                  //    0000000 00010 00000 000 00000 1110011
#sret                                                      #  SRET                 I                  //    0001000 00010 00000 000 00000 1110011
#hret                                                      #  HRET                 I                  //    0010000 00010 00000 000 00000 1110011
#mret                                                      #  MRET                 I                  //    0011000 00010 00000 000 00000 1110011

# Interrupt-Management Instructions
wfi                                                       #  WFI                  I                  //    0001000 00101 00000 000 00000 1110011

# Supervisor Memory-Management Instructions
sfence.vm a3                                              #  SFENCE.VM            I                  //    0001000 00100 rs1 000 00000 1110011
sfence.vma a7,a4                                          #  SFENCE.VMA           R                  //    0001001 rs2 rs1 000 00000 1110011
sinval.vma a5,a6                                          #  SINVAL.VMA           R                  //    0001011 rs2 rs1 000 00000 1110011
sfence.w.inval                                            #  SFENCE.W.INVAL       I                  //    0001100 00000 00000 000 00000 1110011
sfence.inval.ir                                           #  SFENCE.INVAL.IR      I                  //    0001100 00001 00000 000 00000 1110011

# Hypervisor Memory-Management Instructions
hfence.vvma a1,a3                                         #  HFENCE.VVMA          R                  //    0010001 rs2 rs1 000 00000 1110011
hfence.gvma a4,a5                                         #  HFENCE.GVMA          R                  //    0110001 rs2 rs1 000 00000 1110011
hinval.vvma a3,a2                                         #  HINVAL.VVMA          R                  //    0010011 rs2 rs1 000 00000 1110011
hinval.gvma a7,a1                                         #  HINVAL.GVMA          R                  //    0110011 rs2 rs1 000 00000 1110011

# Hypervisor Virtual-Machine Load and Store Instructions
hlv.b a7,(a5)                                             #  HLV.B                I                  //    0110000 00000 rs1 100 rd 1110011
hlv.bu a6,(a2)                                            #  HLV.BU               I                  //    0110000 00001 rs1 100 rd 1110011
hlv.h a4,(a3)                                             #  HLV.H                I                  //    0110010 00000 rs1 100 rd 1110011
hlv.hu sp,(a4)                                            #  HLV.HU               I                  //    0110010 00001 rs1 100 rd 1110011
hlvx.hu a7,(sp)                                           #  HLVX.HU              I                  //    0110010 00011 rs1 100 rd 1110011
hlv.w a6,(s1)                                             #  HLV.W                I                  //    0110100 00000 rs1 100 rd 1110011
hlvx.wu gp,(t4)                                           #  HLVX.WU              I                  //    0110100 00011 rs1 100 rd 1110011
hsv.b t3,(a2)                                             #  HSV.B                R                  //    0110001 rs2 rs1 100 00000 1110011
hsv.h a2,(t6)                                             #  HSV.H                R                  //    0110011 rs2 rs1 100 00000 1110011
hsv.w t4,(t3)                                             #  HSV.W                R                  //    0110101 rs2 rs1 100 00000 1110011

# Hypervisor Virtual-Machine Load and Store Instructions, RV64 only
hlv.wu t3,(a7)                                            #  HLV.WU               I                  //    0110100 00001 rs1 100 rd 1110011
hlv.d a7,(t2)                                             #  HLV.D                I                  //    0110110 00000 rs1 100 rd 1110011
hsv.d tp,(gp)                                             #  HSV.D                R                  //    0110111 rs2 rs1 100 00000 1110011

#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV64I Base Instruction Set (in addition to RV32I)
#════════════════════════════════════════════════════════════════════════════════════════════════════
lwu t5,-12(ra)                                            #  LWU                  I                  //    imm[11:0] rs1 110 rd 0000011
ld t6,248(a0)                                             #  LD                   I                  //    imm[11:0] rs1 011 rd 0000011
#----------------------------------------------------------------------------------------------------
sd t5,240(sp)                                             #  SD                   S                  //    imm[11:5] rs2 rs1 011 imm[4:0] 0100011
#----------------------------------------------------------------------------------------------------
slli a4,a3,0x10                                           #  SLLI                 R                  //    000000 shamt rs1 001 rd 0010011
srli s6,t1,0x10                                           #  SRLI                 R                  //    000000 shamt rs1 101 rd 0010011
srai a2,a6,0x10                                           #  SRAI                 R                  //    010000 shamt rs1 101 rd 0010011
#----------------------------------------------------------------------------------------------------
addiw t0,a0,-2048                                         #  ADDIW                I                  //    imm[11:0] rs1 000 rd 0011011
#----------------------------------------------------------------------------------------------------
slliw t5,ra,0x1f                                          #  SLLIW                R                  //    0000000 shamt rs1 001 rd 0011011
srliw t5,ra,0x1f                                          #  SRLIW                R                  //    0000000 shamt rs1 101 rd 0011011
sraiw zero,ra,0x1c                                        #  SRAIW                R                  //    0100000 shamt rs1 101 rd 0011011
addw t5,ra,sp                                             #  ADDW                 R                  //    0000000 rs2 rs1 000 rd 0111011
subw a0,a5,a4                                             #  SUBW                 R                  //    0100000 rs2 rs1 000 rd 0111011
sllw t5,ra,sp                                             #  SLLW                 R                  //    0000000 rs2 rs1 001 rd 0111011
srlw t4,a3,sp                                             #  SRLW                 R                  //    0000000 rs2 rs1 101 rd 0111011
sraw t5,ra,sp                                             #  SRAW                 R                  //    0100000 rs2 rs1 101 rd 0111011



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV32/RV64 Zifencei Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
fence.i                                                   #  FENCE.I              I                  //    imm[11:0] rs1 001 rd 0001111



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV32/RV64 Zicsr Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
csrrw s2,0x315,t0                                         #  CSRRW                I                  //    csr rs1 001 rd 1110011
csrrs a1,sstatus,a5                                       #  CSRRS                I                  //    csr rs1 010 rd 1110011
csrrc tp,0x5f0,s0                                         #  CSRRC                I                  //    csr rs1 011 rd 1110011
#----------------------------------------------------------------------------------------------------
csrrwi tp,0x5f0,28                                        #  CSRRWI               I                  //    csr uimm 101 rd 1110011
csrrsi s2,0x6e6,30                                        #  CSRRSI               I                  //    csr uimm 110 rd 1110011
csrrci s0,0x6a5,26                                        #  CSRRCI               I                  //    csr uimm 111 rd 1110011



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV32M Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
mul a0,s5,s7                                              #  MUL                  R                  //    0000001 rs2 rs1 000 rd 0110011
mulh t5,a7,s1                                             #  MULH                 R                  //    0000001 rs2 rs1 001 rd 0110011
mulhsu t3,sp,a6                                           #  MULHSU               R                  //    0000001 rs2 rs1 010 rd 0110011
mulhu t3,sp,a6                                            #  MULHU                R                  //    0000001 rs2 rs1 011 rd 0110011
div a5,a3,a2                                              #  DIV                  R                  //    0000001 rs2 rs1 100 rd 0110011
divu t4,t3,t2                                             #  DIVU                 R                  //    0000001 rs2 rs1 101 rd 0110011
rem a4,a1,a2                                              #  REM                  R                  //    0000001 rs2 rs1 110 rd 0110011
remu a6,a1,t2                                             #  REMU                 R                  //    0000001 rs2 rs1 111 rd 0110011



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV64M Standard Extension (in addition to RV32M)
#════════════════════════════════════════════════════════════════════════════════════════════════════
mulw t4,a7,sp                                             #  MULW                 R                  //    0000001 rs2 rs1 000 rd 0111011
divw t3,a6,ra                                             #  DIVW                 R                  //    0000001 rs2 rs1 100 rd 0111011
divuw t5,a4,sp                                            #  DIVUW                R                  //    0000001 rs2 rs1 101 rd 0111011
remw t2,a2,ra                                             #  REMW                 R                  //    0000001 rs2 rs1 110 rd 0111011
remuw t1,a4,gp                                            #  REMUW                R                  //    0000001 rs2 rs1 111 rd 0111011



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV32A Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
lr.w.aqrl a0,(a0)                                         #  LR.W                 R                  //    00010 aq rl 00000 rs1 010 rd 0101111
#----------------------------------------------------------------------------------------------------
sc.w a4,zero,(a0)                                         #  SC.W                 R                  //    00011 aq rl rs2 rs1 010 rd 0101111
amoswap.w a4,a1,(a3)                                      #  AMOSWAP.W            R                  //    00001 aq rl rs2 rs1 010 rd 0101111
amoadd.w.rl gp,s10,(t5)                                   #  AMOADD.W             R                  //    00000 aq rl rs2 rs1 010 rd 0101111
amoxor.w a3,a2,(a5)                                       #  AMOXOR.W             R                  //    00100 aq rl rs2 rs1 010 rd 0101111
amoand.w.aqrl t3,s2,(t5)                                  #  AMOAND.W             R                  //    01100 aq rl rs2 rs1 010 rd 0101111
amoor.w a2,a3,(a1)                                        #  AMOOR.W              R                  //    01000 aq rl rs2 rs1 010 rd 0101111
amomin.w a5,t1,(a2)                                       #  AMOMIN.W             R                  //    10000 aq rl rs2 rs1 010 rd 0101111
amomax.w t1,t3,(a5)                                       #  AMOMAX.W             R                  //    10100 aq rl rs2 rs1 010 rd 0101111
amominu.w a1,t4,(t3)                                      #  AMOMINU.W            R                  //    11000 aq rl rs2 rs1 010 rd 0101111
amomaxu.w a3,t2,(ra)                                      #  AMOMAXU.W            R                  //    11100 aq rl rs2 rs1 010 rd 0101111



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV64A Standard Extension (in addition to RV32A)
#════════════════════════════════════════════════════════════════════════════════════════════════════
lr.d.aqrl a1,(a4)                                         #  LR.D                 R                  //    00010 aq rl 00000 rs1 011 rd 0101111
#----------------------------------------------------------------------------------------------------
sc.d.aqrl a2,a3,(a2)                                      #  SC.D                 R                  //    00011 aq rl rs2 rs1 011 rd 0101111
amoswap.d a4,a1,(a3)                                      #  AMOSWAP.D            R                  //    00001 aq rl rs2 rs1 011 rd 0101111
amoadd.d t4,a2,(gp)                                       #  AMOADD.D             R                  //    00000 aq rl rs2 rs1 011 rd 0101111
amoxor.d a3,a2,(a5)                                       #  AMOXOR.D             R                  //    00100 aq rl rs2 rs1 011 rd 0101111
amoand.d.aqrl t3,s2,(t5)                                  #  AMOAND.D             R                  //    01100 aq rl rs2 rs1 011 rd 0101111
amoor.d a2,a3,(a1)                                        #  AMOOR.D              R                  //    01000 aq rl rs2 rs1 011 rd 0101111
amomin.d a5,t1,(a2)                                       #  AMOMIN.D             R                  //    10000 aq rl rs2 rs1 011 rd 0101111
amomax.d t1,t3,(a5)                                       #  AMOMAX.D             R                  //    10100 aq rl rs2 rs1 011 rd 0101111
amominu.d a1,t4,(t3)                                      #  AMOMINU.D            R                  //    11000 aq rl rs2 rs1 011 rd 0101111
amomaxu.d a3,t2,(ra)                                      #  AMOMAXU.D            R                  //    11100 aq rl rs2 rs1 011 rd 0101111



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV32F Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
flw fa3,84(a0)                                            #  FLW                  I                  //    imm[11:0] rs1 010 rd 0000111
#----------------------------------------------------------------------------------------------------
fsw fs1,72(a1)                                            #  FSW                  S                  //    imm[11:5] rs2 rs1 010 imm[4:0] 0100111
#----------------------------------------------------------------------------------------------------
fmadd.s fs10,ft10,ft5,fa0,rmm                             #  FMADD.S              R4                 //    rs3 00 rs2 rs1 rm rd 1000011
fmsub.s ft8,ft10,fs11,ft7,rdn                             #  FMSUB.S              R4                 //    rs3 00 rs2 rs1 rm rd 1000111
fnmsub.s fs7,fa4,fs3,fs0,rne                              #  FNMSUB.S             R4                 //    rs3 00 rs2 rs1 rm rd 1001011
fnmadd.s fa4,ft4,fs5,fs0,rmm                              #  FNMADD.S             R4                 //    rs3 00 rs2 rs1 rm rd 1001111
#----------------------------------------------------------------------------------------------------
fadd.s fs2,fa6,ft5,rmm                                    #  FADD.S               R                  //    0000000 rs2 rs1 rm rd 1010011
fsub.s ft3,ft0,ft1                                        #  FSUB.S               R                  //    0000100 rs2 rs1 rm rd 1010011
fmul.s ft3,ft0,ft1                                        #  FMUL.S               R                  //    0001000 rs2 rs1 rm rd 1010011
fdiv.s fs4,fs6,fs3,rne                                    #  FDIV.S               R                  //    0001100 rs2 rs1 rm rd 1010011
fsqrt.s ft3,ft0                                           #  FSQRT.S              I                  //    0101100 00000 rs1 rm rd 1010011
fsgnj.s ft0,ft1,ft2                                       #  FSGNJ.S              R                  //    0010000 rs2 rs1 000 rd 1010011
fsgnjn.s ft3,ft7,ft4                                      #  FSGNJN.S             R                  //    0010000 rs2 rs1 001 rd 1010011
fsgnjx.s ft8,ft2,ft5                                      #  FSGNJX.S             R                  //    0010000 rs2 rs1 010 rd 1010011
fmin.s ft6,ft0,ft1                                        #  FMIN.S               R                  //    0010100 rs2 rs1 000 rd 1010011
fmax.s ft5,ft3,ft2                                        #  FMAX.S               R                  //    0010100 rs2 rs1 001 rd 1010011
fminm.s ft3,ft0,ft1                                       #  FMINM.S              R                  //    0010100 rs2 rs1 010 rd 1010011
fmaxm.s ft7,ft1,ft2                                       #  FMAXM.S              R                  //    0010100 rs2 rs1 011 rd 1010011
fcvt.w.s a1,fa5,rtz                                       #  FCVT.W.S             I                  //    1100000 00000 rs1 rm rd 1010011
fcvt.wu.s a0,ft0,rtz                                      #  FCVT.WU.S            I                  //    1100000 00001 rs1 rm rd 1010011
fmv.x.w a0,ft3                                            #  FMV.X.W              I                  //    1110000 00000 rs1 000 rd 1010011
feq.s a0,ft0,ft1                                          #  FEQ.S                R                  //    1010000 rs2 rs1 010 rd 1010011
flt.s a1,ft2,ft3                                          #  FLT.S                R                  //    1010000 rs2 rs1 001 rd 1010011
fle.s a2,ft5,ft4                                          #  FLE.S                R                  //    1010000 rs2 rs1 000 rd 1010011
fclass.s a3,fa2                                           #  FCLASS.S             I                  //    1110000 00000 rs1 001 rd 1010011
fcvt.s.w ft1,a5,rmm                                       #  FCVT.S.W             I                  //    1101000 00000 rs1 rm rd 1010011
fcvt.s.wu fs0,a2,rdn                                      #  FCVT.S.WU            I                  //    1101000 00001 rs1 rm rd 1010011
fmv.w.x ft11,a3                                           #  FMV.W.X              I                  //    1111000 00000 rs1 000 rd 1010011
fli.s ft3,0x1p-3                                          #  FLI.S                I                  //    1111000 00001 rs1 000 rd 1010011

fround.s ft5,ft3,rtz                                      #  FROUND.S             I                  //    0100000 00100 rs1 rm rd 1010011
froundnx.s ft5,ft3,rup                                    #  FROUNDNX.S           I                  //    0100000 00101 rs1 rm rd 1010011
fround.h ft6,ft0,rmm                                      #  FROUND.H             I                  //    0100010 00100 rs1 rm rd 1010011
froundnx.h ft6,ft0,dyn                                    #  FROUNDNX.H           I                  //    0100010 00101 rs1 rm rd 1010011
fround.d ft3,ft0,rne                                      #  FROUND.D             I                  //    0100001 00100 rs1 rm rd 1010011
froundnx.d ft3,ft0,rtz                                    #  FROUNDNX.D           I                  //    0100001 00101 rs1 rm rd 1010011
fround.q ft7,ft1,rdn                                      #  FROUND.Q             I                  //    0100011 00100 rs1 rm rd 1010011
froundnx.q ft7,ft1,rmm                                    #  FROUNDNX.Q           I                  //    0100011 00101 rs1 rm rd 1010011


#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV64F Standard Extension (in addition to RV32F)
#════════════════════════════════════════════════════════════════════════════════════════════════════
fcvt.l.s a0,ft11,rdn                                      #  FCVT.L.S             I                  //    1100000 00010 rs1 rm rd 1010011
fcvt.lu.s a0,ft9,rup                                      #  FCVT.LU.S            I                  //    1100000 00011 rs1 rm rd 1010011
fcvt.s.l ft7,a7,rmm                                       #  FCVT.S.L             I                  //    1101000 00010 rs1 rm rd 1010011
fcvt.s.lu ft4,a3,rdn                                      #  FCVT.S.LU            I                  //    1101000 00011 rs1 rm rd 1010011



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV32D Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
fld fa2,248(a5)                                           #  FLD                  I                  //    imm[11:0] rs1 011 rd 0000111
#----------------------------------------------------------------------------------------------------
fsd fs4,312(sp)                                           #  FSD                  S                  //    imm[11:5] rs2 rs1 011 imm[4:0] 0100111
#----------------------------------------------------------------------------------------------------
fmadd.d fs6,ft10,ft5,fa0,rmm                              #  FMADD.D              R4                 //    rs3 01 rs2 rs1 rm rd 1000011
fmsub.d ft0,ft0,fs0,fa6,rne                               #  FMSUB.D              R4                 //    rs3 01 rs2 rs1 rm rd 1000111
fnmsub.d fs0,ft4,ft2,fa5,rdn                              #  FNMSUB.D             R4                 //    rs3 01 rs2 rs1 rm rd 1001011
fnmadd.d fa2,ft10,ft5,fa0,rmm                             #  FNMADD.D             R4                 //    rs3 01 rs2 rs1 rm rd 1001111
#----------------------------------------------------------------------------------------------------
fadd.d fa7,ft2,ft3,rup                                    #  FADD.D               R                  //    0000001 rs2 rs1 rm rd 1010011
fsub.d ft3,ft0,ft1                                        #  FSUB.D               R                  //    0000101 rs2 rs1 rm rd 1010011
fmul.d ft5,ft4,fa6,rne                                    #  FMUL.D               R                  //    0001001 rs2 rs1 rm rd 1010011
fdiv.d fa1,fa5,fa7                                        #  FDIV.D               R                  //    0001101 rs2 rs1 rm rd 1010011
fsqrt.d fa4,fa2                                           #  FSQRT.D              I                  //    0101101 00000 rs1 rm rd 1010011
fsgnj.d ft4,fa1,fs2                                       #  FSGNJ.D              R                  //    0010001 rs2 rs1 000 rd 1010011
fsgnjn.d ft3,fs1,fa2                                      #  FSGNJN.D             R                  //    0010001 rs2 rs1 001 rd 1010011
fsgnjx.d fs0,ft3,fa7                                      #  FSGNJX.D             R                  //    0010001 rs2 rs1 010 rd 1010011
fmin.d ft3,ft0,ft1                                        #  FMIN.D               R                  //    0010101 rs2 rs1 000 rd 1010011
fmax.d ft2,ft0,ft4                                        #  FMAX.D               R                  //    0010101 rs2 rs1 001 rd 1010011
fminm.d ft3,ft0,ft1                                       #  FMINM.D              R                  //    0010101 rs2 rs1 010 rd 1010011
fmaxm.d ft2,ft0,ft4                                       #  FMAXM.D              R                  //    0010101 rs2 rs1 011 rd 1010011
fcvt.s.d fa5,fa2,rne                                      #  FCVT.S.D             I                  //    0100000 00001 rs1 rm rd 1010011
fcvt.d.s fa3,fa4                                          #  FCVT.D.S             I                  //    0100001 00000 rs1 rm rd 1010011
feq.d a3,fa4,fa2                                          #  FEQ.D                R                  //    1010001 rs2 rs1 010 rd 1010011
flt.d a5,fa5,fa4                                          #  FLT.D                R                  //    1010001 rs2 rs1 001 rd 1010011
fle.d a0,fa2,ft1                                          #  FLE.D                R                  //    1010001 rs2 rs1 000 rd 1010011
fclass.d a0,fa3                                           #  FCLASS.D             I                  //    1110001 00000 rs1 001 rd 1010011
fcvt.w.d a7,ft2,rmm                                       #  FCVT.W.D             I                  //    1100001 00000 rs1 rm rd 1010011
fcvt.wu.d a5,fs3,rup                                      #  FCVT.WU.D            I                  //    1100001 00001 rs1 rm rd 1010011
fcvt.d.w fa5,zero                                         #  FCVT.D.W             I                  //    1101001 00000 rs1 rm rd 1010011
fcvt.d.wu fa4,a3                                          #  FCVT.D.WU            I                  //    1101001 00001 rs1 rm rd 1010011
fli.d ft7,0.25                                            #  FLI.D                I                  //    1111001 00001 rs1 000 rd 1010011



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV64D Standard Extension (in addition to RV32D)
#════════════════════════════════════════════════════════════════════════════════════════════════════
fcvt.l.d ra,ft1,rdn                                       #  FCVT.L.D             I                  //    1100001 00010 rs1 rm rd 1010011
fcvt.lu.d a0,fa3,rtz                                      #  FCVT.LU.D            I                  //    1100001 00011 rs1 rm rd 1010011
fmv.x.d a2,ft3                                            #  FMV.X.D              I                  //    1110001 00000 rs1 000 rd 1010011
fcvt.d.l ft1,a3,rmm                                       #  FCVT.D.L             I                  //    1101001 00010 rs1 rm rd 1010011
fcvt.d.lu ft0,a2,rdn                                      #  FCVT.D.LU            I                  //    1101001 00011 rs1 rm rd 1010011
fmv.d.x ft2,a5                                            #  FMV.D.X              I                  //    1111001 00000 rs1 000 rd 1010011



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV32Q Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
flq fs6,-1168(tp)                                         #  FLQ                  I                  //    imm[11:0] rs1 100 rd 0000111
#----------------------------------------------------------------------------------------------------
fsq fs7,1375(s3)                                          #  FSQ                  S                  //    imm[11:5] rs2 rs1 100 imm[4:0] 0100111
#----------------------------------------------------------------------------------------------------
fmadd.q fa0,fs6,fs0,fs6,rup                               #  FMADD.Q              R4                 //    rs3 11 rs2 rs1 rm rd 1000011
fmsub.q ft7,ft4,fa1,fs9,rne                               #  FMSUB.Q              R4                 //    rs3 11 rs2 rs1 rm rd 1000111
fnmsub.q fs0,ft4,fs2,fa5,rdn                              #  FNMSUB.Q             R4                 //    rs3 11 rs2 rs1 rm rd 1001011
fnmadd.q ft10,fa4,fa1,fs3,rtz                             #  FNMADD.Q             R4                 //    rs3 11 rs2 rs1 rm rd 1001111
#----------------------------------------------------------------------------------------------------
fadd.q ft7,fa3,fs2,rne                                    #  FADD.Q               R                  //    0000011 rs2 rs1 rm rd 1010011
fsub.q ft8,fs8,ft10,rtz                                   #  FSUB.Q               R                  //    0000111 rs2 rs1 rm rd 1010011
fmul.q ft8,fs0,fa7,rup                                    #  FMUL.Q               R                  //    0001011 rs2 rs1 rm rd 1010011
fdiv.q fs10,fs9,fs11,rmm                                  #  FDIV.Q               R                  //    0001111 rs2 rs1 rm rd 1010011
fsqrt.q fs10,fs11                                         #  FSQRT.Q              I                  //    0101111 00000 rs1 rm rd 1010011
fsgnj.q ft7,ft3,ft2                                       #  FSGNJ.Q              R                  //    0010011 rs2 rs1 000 rd 1010011
fsgnjn.q fs0,ft1,fa2                                      #  FSGNJN.Q             R                  //    0010011 rs2 rs1 001 rd 1010011
fsgnjx.q fs2,fa3,ft3                                      #  FSGNJX.Q             R                  //    0010011 rs2 rs1 010 rd 1010011
fmin.q fa0,ft3,fs7                                        #  FMIN.Q               R                  //    0010111 rs2 rs1 000 rd 1010011
fmax.q ft0,fs4,fa2                                        #  FMAX.Q               R                  //    0010111 rs2 rs1 001 rd 1010011
fminm.q fs1,ft3,fa7                                       #  FMINM.Q              R                  //    0010111 rs2 rs1 010 rd 1010011
fmaxm.q ft3,fa4,ft5                                       #  FMAXM.Q              R                  //    0010111 rs2 rs1 011 rd 1010011
fcvt.s.q fs10,fs9,rdn                                     #  FCVT.S.Q             I                  //    0100000 00011 rs1 rm rd 1010011
fcvt.q.s fs11,fs4                                         #  FCVT.Q.S             I                  //    0100011 00000 rs1 rm rd 1010011
fcvt.d.q fs7,fs6,rup                                      #  FCVT.D.Q             I                  //    0100001 00011 rs1 rm rd 1010011
fcvt.q.d fs3,fs5                                          #  FCVT.Q.D             I                  //    0100011 00001 rs1 rm rd 1010011
feq.q t2,fs3,ft2                                          #  FEQ.Q                R                  //    1010011 rs2 rs1 010 rd 1010011
flt.q s0,ft7,fs10                                         #  FLT.Q                R                  //    1010011 rs2 rs1 001 rd 1010011
fle.q s2,fa0,ft7                                          #  FLE.Q                R                  //    1010011 rs2 rs1 000 rd 1010011
fclass.q a3,fa6                                           #  FCLASS.Q             I                  //    1110011 00000 rs1 001 rd 1010011
fcvt.w.q zero,ft3,rne                                     #  FCVT.W.Q             I                  //    1100011 00000 rs1 rm rd 1010011
fcvt.wu.q ra,fs4,rtz                                      #  FCVT.WU.Q            I                  //    1100011 00001 rs1 rm rd 1010011
fcvt.q.w fa7,t1                                           #  FCVT.Q.W             I                  //    1101011 00000 rs1 rm rd 1010011
fcvt.q.wu ft5,tp                                          #  FCVT.Q.WU            I                  //    1101011 00001 rs1 rm rd 1010011
fli.q ft5,0.25                                            #  FLI.Q                I                  //    1111011 00001 rs1 000 rd 1010011



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV64Q Standard Extension (in addition to RV32Q)
#════════════════════════════════════════════════════════════════════════════════════════════════════
fcvt.l.q gp,fa2,rdn                                       #  FCVT.L.Q             R                  //    1100011 00010 rs1 rm rd 1010011
fcvt.lu.q ra,ft3,rne                                      #  FCVT.LU.Q            R                  //    1100011 00011 rs1 rm rd 1010011
fcvt.q.l fs3,t3,rtz                                       #  FCVT.Q.L             R                  //    1101011 00010 rs1 rm rd 1010011
fcvt.q.lu fa5,sp,rmm                                      #  FCVT.Q.LU            R                  //    1101011 00011 rs1 rm rd 1010011



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV32Zfh Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
flh fa4,12(a7)                                            #  FLH                  I                  //    imm[11:0] rs1 001 rd 0000111
fsh fa3,21(a5)                                            #  FSH                  S                  //    imm[11:5] rs2 rs1 001 imm[4:0] 0100111
fmadd.h fa4,fa2,fa1,fa7,rne                               #  FMADD.H              R4                 //    rs3 10 rs2 rs1 rm rd 1000011
fmsub.h fa3,fa1,fa6,fa2,rdn                               #  FMSUB.H              R4                 //    rs3 10 rs2 rs1 rm rd 1000111
fnmsub.h fa4,fa3,fa5,fa6,rtz                              #  FNMSUB.H             R4                 //    rs3 10 rs2 rs1 rm rd 1001011
fnmadd.h fa1,fa7,fa4,fa2,rup                              #  FNMADD.H             R4                 //    rs3 10 rs2 rs1 rm rd 1001111
fadd.h fa2,fa3,fa6,rmm                                    #  FADD.H               R                  //    0000010 rs2 rs1 rm rd 1010011
fsub.h fa3,fa4,fa6                                        #  FSUB.H               R                  //    0000110 rs2 rs1 rm rd 1010011
fmul.h fa2,fa5,fa4,rdn                                    #  FMUL.H               R                  //    0001010 rs2 rs1 rm rd 1010011
fdiv.h fa4,fa6,fa7,rne                                    #  FDIV.H               R                  //    0001110 rs2 rs1 rm rd 1010011
fsqrt.h fa3,fa6,rne                                       #  FSQRT.H              I                  //    0101110 00000 rs1 rm rd 1010011
fsgnj.h fa5,fa2,fa1                                       #  FSGNJ.H              R                  //    0010010 rs2 rs1 000 rd 1010011
fsgnjn.h fa4,fa5,fa7                                      #  FSGNJN.H             R                  //    0010010 rs2 rs1 001 rd 1010011
fsgnjx.h fa3,fa4,fa2                                      #  FSGNJX.H             R                  //    0010010 rs2 rs1 010 rd 1010011
fmin.h fa4,fa3,fa5                                        #  FMIN.H               R                  //    0010110 rs2 rs1 000 rd 1010011
fmax.h fa1,fa7,fa2                                        #  FMAX.H               R                  //    0010110 rs2 rs1 001 rd 1010011
fminm.h fa4,fa3,fa5                                       #  FMINM.H              R                  //    0010110 rs2 rs1 010 rd 1010011
fmaxm.h fa1,fa7,fa2                                       #  FMAXM.H              R                  //    0010110 rs2 rs1 011 rd 1010011
fcvt.s.h fa2,fa5                                          #  FCVT.S.H             I                  //    0100000 00010 rs1 rm rd 1010011
fcvt.h.s fa1,fa4                                          #  FCVT.H.S             I                  //    0100010 00000 rs1 rm rd 1010011
fcvt.d.h fa4,fa7                                          #  FCVT.D.H             I                  //    0100001 00010 rs1 rm rd 1010011
fcvt.h.d fa3,fa4,rdn                                      #  FCVT.H.D             I                  //    0100010 00001 rs1 rm rd 1010011
fcvt.q.h fa7,fa6                                          #  FCVT.Q.H             I                  //    0100011 00010 rs1 rm rd 1010011
fcvt.h.q fa7,fa2,rdn                                      #  FCVT.H.Q             I                  //    0100010 00011 rs1 rm rd 1010011
feq.h a5,fa1,fa5                                          #  FEQ.H                R                  //    1010010 rs2 rs1 010 rd 1010011
flt.h a3,fa3,fa6                                          #  FLT.H                R                  //    1010010 rs2 rs1 001 rd 1010011
fle.h a7,fa2,fa4                                          #  FLE.H                R                  //    1010010 rs2 rs1 000 rd 1010011
fclass.h a2,fa4                                           #  FCLASS.H             I                  //    1110010 00000 rs1 001 rd 1010011
fcvt.w.h a7,fa1                                           #  FCVT.W.H             I                  //    1100010 00000 rs1 rm rd 1010011
fcvt.wu.h a4,fa3                                          #  FCVT.WU.H            I                  //    1100010 00001 rs1 rm rd 1010011
fmv.x.h a5,fa1                                            #  FMV.X.H              I                  //    1110010 00000 rs1 000 rd 1010011
fcvt.h.w fa3,a4                                           #  FCVT.H.W             I                  //    1101010 00000 rs1 rm rd 1010011
fcvt.h.wu fa6,a1                                          #  FCVT.H.WU            I                  //    1101010 00001 rs1 rm rd 1010011
fmv.h.x fa2,a6                                            #  FMV.H.X              I                  //    1111010 00000 rs1 000 rd 1010011
fli.h ft11,8.0                                            #  FLI.H                I                  //    1111010 00001 rs1 000 rd 1010011



#════════════════════════════════════════════════════════════════════════════════════════════════════
#RV64Zfh Standard Extension (in addition to RV32Zfh)
#════════════════════════════════════════════════════════════════════════════════════════════════════
fcvt.l.h a7,fa4,rtz                                       #  FCVT.L.H             I                  //    1100010 00010 rs1 rm rd 1010011
fcvt.lu.h a1,fa5,rne                                      #  FCVT.LU.H            I                  //    1100010 00011 rs1 rm rd 1010011
fcvt.h.l fa3,a6,rdn                                       #  FCVT.H.L             I                  //    1101010 00010 rs1 rm rd 1010011
fcvt.h.lu fa2,a7,rmm                                      #  FCVT.H.LU            I                  //    1101010 00011 rs1 rm rd 1010011
