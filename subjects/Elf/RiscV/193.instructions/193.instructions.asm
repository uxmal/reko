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
.attribute       5, "rv64gaicmdfqh_zicsr_zfh_zihintpause_svinval_zmmul_zfa_zvbb_zicbop_zfh_zicbom_zicboz_zicond_zawrs_zbb_zbkb_zba_zbc_zbs_zbs_zbkx_zknd_zkne_zknh_zksed_zksh"

_start:

#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV32I Base Instruction Set
#════════════════════════════════════════════════════════════════════════════════════════════════════
lui zero,0x80000                                          #  LUI                  U          80000037 >>    imm[31:12] rd 0110111
auipc s3,0xfffb                                           #  AUIPC                U          0fffb997 >>    imm[31:12] rd 0010111
#----------------------------------------------------------------------------------------------------
## jal ra,0x800000e6                                      #  JAL                  J          0de000ef >>    imm[20|10:1|11|19:12] rd 1101111
#----------------------------------------------------------------------------------------------------
## jalr tp,0x5(s10)                                       #  JALR                 I          005d0267 >>    imm[11:0] rs1 000 rd 1100111
#----------------------------------------------------------------------------------------------------
## beq a0,a6,0x800091ac                                   #  BEQ                  B          013a0463 >>    imm[12|10:5] rs2 rs1 000 imm[4:1|11] 1100011
## bne s4,s3,0x8000926a                                   #  BNE                  B          01051463 >>    imm[12|10:5] rs2 rs1 001 imm[4:1|11] 1100011
## blt s0,t3,0x80002da4                                   #  BLT                  B          00c44463 >>    imm[12|10:5] rs2 rs1 100 imm[4:1|11] 1100011
## bge s0,a2,0x80002ca8                                   #  BGE                  B          01c45463 >>    imm[12|10:5] rs2 rs1 101 imm[4:1|11] 1100011
## bltu a4,a0,0x800018f2                                  #  BLTU                 B          00556463 >>    imm[12|10:5] rs2 rs1 110 imm[4:1|11] 1100011
## bgeu a0,t0,0x800012c2                                  #  BGEU                 B          00a77463 >>    imm[12|10:5] rs2 rs1 111 imm[4:1|11] 1100011
#----------------------------------------------------------------------------------------------------
lb sp,-0x83(s2)                                           #  LB                   I          08090103 >>    imm[11:0] rs1 000 rd 0000011
lh t6,-0x7ec(t5)                                          #  LH                   I          7ecf1f83 >>    imm[11:0] rs1 001 rd 0000011
lw s4,-0x7e7(s3)                                          #  LW                   I          7e89aa03 >>    imm[11:0] rs1 010 rd 0000011
lbu a5,-0x2a5(a5)                                         #  LBU                  I          d5b7c783 >>    imm[11:0] rs1 100 rd 0000011
lhu s11,-0x624(sp)                                        #  LHU                  I          62415d83 >>    imm[11:0] rs1 101 rd 0000011
#----------------------------------------------------------------------------------------------------
sb a5,-0x731(a4)                                          #  SB                   S          d2f70aa3 >>    imm[11:5] rs2 rs1 000 imm[4:0] 0100011
sh a0,-0x653(s1)                                          #  SH                   S          7ea49623 >>    imm[11:5] rs2 rs1 001 imm[4:0] 0100011
sw s2,-0x7e0(sp)                                          #  SW                   S          7f212023 >>    imm[11:5] rs2 rs1 010 imm[4:0] 0100011
#----------------------------------------------------------------------------------------------------
addi t5,a5,-0xd4                                          #  ADDI                 I          f2c78f13 >>    imm[11:0] rs1 000 rd 0010011
slti a1,t3,-0x5c7                                         #  SLTI                 I          a39e2593 >>    imm[11:0] rs1 010 rd 0010011
sltiu s4,s6,-0x3b0                                        #  SLTIU                I          3b0b3a13 >>    imm[11:0] rs1 011 rd 0010011
xori s0,t1,-0x3fd                                         #  XORI                 I          3fd34413 >>    imm[11:0] rs1 100 rd 0010011
ori a4,s6,-0x437                                          #  ORI                  I          400b6713 >>    imm[11:0] rs1 110 rd 0010011
andi t2,a6,-0x745                                         #  ANDI                 I          70087393 >>    imm[11:0] rs1 111 rd 0010011
#----------------------------------------------------------------------------------------------------
slli a0,s6,0x10                                           #  SLLI                 R          010b1513 >>    0000000 shamt rs1 001 rd 0010011
srli s1,s9,0x1a                                           #  SRLI                 R          017cd493 >>    0000000 shamt rs1 101 rd 0010011
srai s1,s11,0x1f                                          #  SRAI                 R          41fdd493 >>    0100000 shamt rs1 101 rd 0010011
add t6,a7,t5                                              #  ADD                  R          01e88fb3 >>    0000000 rs2 rs1 000 rd 0110011
sub s11,s7,t1                                             #  SUB                  R          406b8db3 >>    0100000 rs2 rs1 000 rd 0110011
sll s11,s7,t1                                             #  SLL                  R          006b9db3 >>    0000000 rs2 rs1 001 rd 0110011
slt t2,t4,t6                                              #  SLT                  R          01fea3b3 >>    0000000 rs2 rs1 010 rd 0110011
sltu a3,s6,a7                                             #  SLTU                 R          011b36b3 >>    0000000 rs2 rs1 011 rd 0110011
xor t5,t6,t3                                              #  XOR                  R          01cfcf33 >>    0000000 rs2 rs1 100 rd 0110011
srl t5,t6,t3                                              #  SRL                  R          01cfdf33 >>    0000000 rs2 rs1 101 rd 0110011
sra t6,s3,sp                                              #  SRA                  R          4029dfb3 >>    0100000 rs2 rs1 101 rd 0110011
or a1,a2,t5                                               #  OR                   R          01e665b3 >>    0000000 rs2 rs1 110 rd 0110011
and s5,s7,t4                                              #  AND                  R          01dbfab3 >>    0000000 rs2 rs1 111 rd 0110011
#----------------------------------------------------------------------------------------------------
unimp                   #pseudo csrrw zero,cycle,zero     #  UNIMP                .          c0001073 >>    110000000000 00000 001 00000 1110011
fence w,i                                                 #  FENCE                I          0180000f >>    fm pred succ rs1 000 rd 0001111
fence.tso                                                 #  FENCE.TSO            I          8330000f >>    1000 0011 0011 00000 000 00000 0001111
pause                                                     #  PAUSE                I          0100000f >>    0000 0001 0000 00000 000 00000 0001111
ecall                                                     #  ECALL                I          00000073 >>    000000000000 00000 000 00000 1110011
ebreak                                                    #  EBREAK               I          00100073 >>    000000000001 00000 000 00000 1110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# GCC extension  / aliases?
#════════════════════════════════════════════════════════════════════════════════════════════════════
## zext.b a1,a2                                           #  ZEXT.B
## zext.h a2,a3                                           #  ZEXT.H
## zext.w a3,a4                                           #  ZEXT.W
## zext.q a4,a5                                           #  ZEXT.W


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV32  3.3 Machine-Mode Privileged Instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════

# Trap-Return Instructions
## uret                                                   #  URET                 I          00200073 >>    0000000 00010 00000 000 00000 1110011
## sret                                                   #  SRET                 I          10200073 >>    0001000 00010 00000 000 00000 1110011
## hret                                                   #  HRET                 I          20200073 >>    0010000 00010 00000 000 00000 1110011
## mret                                                   #  MRET                 I          30200073 >>    0011000 00010 00000 000 00000 1110011
## dret                                                   #  DRET                 I          7b200073 >>    0111101 10010 00000 000 00000 1110011

# Interrupt-Management Instructions
wfi                                                       #  WFI                  I          10500073 >>    0001000 00101 00000 000 00000 1110011

# Supervisor Memory-Management Instructions
sfence.vm                                                 #  SFENCE.VM            I          10400073 >>    0001000 00100 00000 000 00000 1110011
sfence.vm a3                                              #  SFENCE.VM            I          10468073 >>    0001000 00100 rs1 000 00000 1110011
sfence.vma                                                #  SFENCE.VMA           R          12000073 >>    0001001 00000 00000 000 00000 1110011
sfence.vma a7,a4                                          #  SFENCE.VMA           R          12e88073 >>    0001001 rs2 rs1 000 00000 1110011
sinval.vma a5,a6                                          #  SINVAL.VMA           R          17078073 >>    0001011 rs2 rs1 000 00000 1110011
sfence.w.inval                                            #  SFENCE.W.INVAL       I          18000073 >>    0001100 00000 00000 000 00000 1110011
sfence.inval.ir                                           #  SFENCE.INVAL.IR      I          18100073 >>    0001100 00001 00000 000 00000 1110011

# Hypervisor Memory-Management Instructions
hfence.vvma a1,a3                                         #  HFENCE.VVMA          R          22d58073 >>    0010001 rs2 rs1 000 00000 1110011
hfence.gvma a4,a5                                         #  HFENCE.GVMA          R          62f70073 >>    0110001 rs2 rs1 000 00000 1110011
hinval.vvma a3,a2                                         #  HINVAL.VVMA          R          26c68073 >>    0010011 rs2 rs1 000 00000 1110011
hinval.gvma a7,a1                                         #  HINVAL.GVMA          R          66b88073 >>    0110011 rs2 rs1 000 00000 1110011

# Hypervisor Virtual-Machine Load and Store Instructions
hlv.b a7,(a5)                                             #  HLV.B                I          6007c8f3 >>    0110000 00000 rs1 100 rd 1110011
hlv.bu a6,(a2)                                            #  HLV.BU               I          60164873 >>    0110000 00001 rs1 100 rd 1110011
hlv.h a4,(a3)                                             #  HLV.H                I          6406c773 >>    0110010 00000 rs1 100 rd 1110011
hlv.hu sp,(a4)                                            #  HLV.HU               I          64174173 >>    0110010 00001 rs1 100 rd 1110011
hlvx.hu a7,(sp)                                           #  HLVX.HU              I          643148f3 >>    0110010 00011 rs1 100 rd 1110011
hlv.w a6,(s1)                                             #  HLV.W                I          6804c873 >>    0110100 00000 rs1 100 rd 1110011
hlvx.wu gp,(t4)                                           #  HLVX.WU              I          683ec1f3 >>    0110100 00011 rs1 100 rd 1110011
hsv.b t3,(a2)                                             #  HSV.B                R          63c64073 >>    0110001 rs2 rs1 100 00000 1110011
hsv.h a2,(t6)                                             #  HSV.H                R          66cfc073 >>    0110011 rs2 rs1 100 00000 1110011
hsv.w t4,(t3)                                             #  HSV.W                R          6bde4073 >>    0110101 rs2 rs1 100 00000 1110011

# Hypervisor Virtual-Machine Load and Store Instructions, RV64 only
hlv.wu t3,(a7)                                            #  HLV.WU               I          6818ce73 >>    0110100 00001 rs1 100 rd 1110011
hlv.d a7,(t2)                                             #  HLV.D                I          6c03c8f3 >>    0110110 00000 rs1 100 rd 1110011
hsv.d tp,(gp)                                             #  HSV.D                R          6e41c073 >>    0110111 rs2 rs1 100 00000 1110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV64I Base Instruction Set (in addition to RV32I)
#════════════════════════════════════════════════════════════════════════════════════════════════════
lwu t5,-0xc(ra)                                           #  LWU                  I          ff40ef03 >>    imm[11:0] rs1 110 rd 0000011
ld t6,-0x2e8(a0)                                          #  LD                   I          0f853f83 >>    imm[11:0] rs1 011 rd 0000011
#----------------------------------------------------------------------------------------------------
sd t5,-0x4d8(sp)                                          #  SD                   S          4de13c23 >>    imm[11:5] rs2 rs1 011 imm[4:0] 0100011
#----------------------------------------------------------------------------------------------------
slli a4,a3,0x10                                           #  SLLI                 R          01069713 >>    000000 shamt rs1 001 rd 0010011
srli s6,t1,0x10                                           #  SRLI                 R          01035b13 >>    000000 shamt rs1 101 rd 0010011
srai a2,a6,0x10                                           #  SRAI                 R          41085613 >>    010000 shamt rs1 101 rd 0010011
#----------------------------------------------------------------------------------------------------
addiw t0,a0,-0x800                                        #  ADDIW                I          8005029b >>    imm[11:0] rs1 000 rd 0011011
#----------------------------------------------------------------------------------------------------
slliw t5,ra,0x1f                                          #  SLLIW                R          01f09f1b >>    0000000 shamt rs1 001 rd 0011011
srliw t5,ra,0x1f                                          #  SRLIW                R          01f0df1b >>    0000000 shamt rs1 101 rd 0011011
sraiw zero,ra,0x1c                                        #  SRAIW                R          41c0d01b >>    0100000 shamt rs1 101 rd 0011011
addw t5,ra,sp                                             #  ADDW                 R          00208f3b >>    0000000 rs2 rs1 000 rd 0111011
subw a0,a5,a4                                             #  SUBW                 R          40e7853b >>    0100000 rs2 rs1 000 rd 0111011
sllw t5,ra,sp                                             #  SLLW                 R          00209f3b >>    0000000 rs2 rs1 001 rd 0111011
srlw t4,a3,sp                                             #  SRLW                 R          0026debb >>    0000000 rs2 rs1 101 rd 0111011
sraw t5,ra,sp                                             #  SRAW                 R          4020df3b >>    0100000 rs2 rs1 101 rd 0111011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV32/RV64 Zifencei Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
fence.i                                                   #  FENCE.I              I          0000100f >>    imm[11:0] rs1 001 rd 0001111


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV32/RV64 Zicsr Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
csrrw s2,0x315,t0                                         #  CSRRW                I          31529973 >>    csr rs1 001 rd 1110011
csrrs a1,sstatus,a5                                       #  CSRRS                I          1007a5f3 >>    csr rs1 010 rd 1110011
csrrc tp,0x5f0,s0                                         #  CSRRC                I          5f043273 >>    csr rs1 011 rd 1110011
#----------------------------------------------------------------------------------------------------
csrrwi tp,0x5f0,0x1c                                      #  CSRRWI               I          5f0e5273 >>    csr uimm 101 rd 1110011
csrrsi s2,0x6e6,0x1e                                      #  CSRRSI               I          6e6f6973 >>    csr uimm 110 rd 1110011
csrrci s0,0x6a5,0x1a                                      #  CSRRCI               I          6a5d7473 >>    csr uimm 111 rd 1110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV32M Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
mul a0,s5,s7                                              #  MUL                  R          037a8533 >>    0000001 rs2 rs1 000 rd 0110011
mulh t5,a7,s1                                             #  MULH                 R          02989f33 >>    0000001 rs2 rs1 001 rd 0110011
mulhsu t3,sp,a6                                           #  MULHSU               R          03012e33 >>    0000001 rs2 rs1 010 rd 0110011
mulhu t3,sp,a6                                            #  MULHU                R          03013e33 >>    0000001 rs2 rs1 011 rd 0110011
div a5,a3,a2                                              #  DIV                  R          02c6c7b3 >>    0000001 rs2 rs1 100 rd 0110011
divu t4,t3,t2                                             #  DIVU                 R          027e5eb3 >>    0000001 rs2 rs1 101 rd 0110011
rem a4,a1,a2                                              #  REM                  R          02c5e733 >>    0000001 rs2 rs1 110 rd 0110011
remu a6,a1,t2                                             #  REMU                 R          0275f833 >>    0000001 rs2 rs1 111 rd 0110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV64M Standard Extension (in addition to RV32M)
#════════════════════════════════════════════════════════════════════════════════════════════════════
mulw t4,a7,sp                                             #  MULW                 R          02288ebb >>    0000001 rs2 rs1 000 rd 0111011
divw t3,a6,ra                                             #  DIVW                 R          02184e3b >>    0000001 rs2 rs1 100 rd 0111011
divuw t5,a4,sp                                            #  DIVUW                R          02275f3b >>    0000001 rs2 rs1 101 rd 0111011
remw t2,a2,ra                                             #  REMW                 R          021663bb >>    0000001 rs2 rs1 110 rd 0111011
remuw t1,a4,gp                                            #  REMUW                R          0237733b >>    0000001 rs2 rs1 111 rd 0111011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV32A Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
lr.w.aqrl a0,(a0)                                         #  LR.W                 R          1605252f >>    00010 aq rl 00000 rs1 010 rd 0101111
#----------------------------------------------------------------------------------------------------
sc.w.aqrl a4,zero,(a0)                                    #  SC.W                 R          1e05272f >>    00011 aq rl rs2 rs1 010 rd 0101111
#----------------------------------------------------------------------------------------------------
amoswap.w a4,a1,(a3)                                      #  AMOSWAP.W            R          08b6a72f >>    00001 aq rl rs2 rs1 010 rd 0101111
amoadd.w.rl gp,s10,(t5)                                   #  AMOADD.W             R          03af21af >>    00000 aq rl rs2 rs1 010 rd 0101111
amoxor.w a3,a2,(a5)                                       #  AMOXOR.W             R          20c7a6af >>    00100 aq rl rs2 rs1 010 rd 0101111
amoand.w.aqrl t3,s2,(t5)                                  #  AMOAND.W             R          672f2e2f >>    01100 aq rl rs2 rs1 010 rd 0101111
amoor.w a2,a3,(a1)                                        #  AMOOR.W              R          40d5a62f >>    01000 aq rl rs2 rs1 010 rd 0101111
amomin.w a5,t1,(a2)                                       #  AMOMIN.W             R          806627af >>    10000 aq rl rs2 rs1 010 rd 0101111
amomax.w t1,t3,(a5)                                       #  AMOMAX.W             R          a1c7a32f >>    10100 aq rl rs2 rs1 010 rd 0101111
amominu.w a1,t4,(t3)                                      #  AMOMINU.W            R          c1de25af >>    11000 aq rl rs2 rs1 010 rd 0101111
amomaxu.w a3,t2,(ra)                                      #  AMOMAXU.W            R          e070a6af >>    11100 aq rl rs2 rs1 010 rd 0101111


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV64A Standard Extension (in addition to RV32A)
#════════════════════════════════════════════════════════════════════════════════════════════════════
lr.d.aqrl a1,(a4)                                         #  LR.D                 R          160735af >>    00010 aq rl 00000 rs1 011 rd 0101111
#----------------------------------------------------------------------------------------------------
sc.d.aqrl a2,a3,(a2)                                      #  SC.D                 R          1ed6362f >>    00011 aq rl rs2 rs1 011 rd 0101111
#----------------------------------------------------------------------------------------------------
amoswap.d a4,a1,(a3)                                      #  AMOSWAP.D            R          08b6b72f >>    00001 aq rl rs2 rs1 011 rd 0101111
amoadd.d t4,a2,(gp)                                       #  AMOADD.D             R          00c1beaf >>    00000 aq rl rs2 rs1 011 rd 0101111
amoxor.d a3,a2,(a5)                                       #  AMOXOR.D             R          20c7b6af >>    00100 aq rl rs2 rs1 011 rd 0101111
amoand.d.aqrl t3,s2,(t5)                                  #  AMOAND.D             R          672f3e2f >>    01100 aq rl rs2 rs1 011 rd 0101111
amoor.d a2,a3,(a1)                                        #  AMOOR.D              R          40d5b62f >>    01000 aq rl rs2 rs1 011 rd 0101111
amomin.d a5,t1,(a2)                                       #  AMOMIN.D             R          806637af >>    10000 aq rl rs2 rs1 011 rd 0101111
amomax.d t1,t3,(a5)                                       #  AMOMAX.D             R          a1c7b32f >>    10100 aq rl rs2 rs1 011 rd 0101111
amominu.d a1,t4,(t3)                                      #  AMOMINU.D            R          c1de35af >>    11000 aq rl rs2 rs1 011 rd 0101111
amomaxu.d a3,t2,(ra)                                      #  AMOMAXU.D            R          e070b6af >>    11100 aq rl rs2 rs1 011 rd 0101111


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV32F Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
flw fa3,-0x54(a0)                                         #  FLW                  I          05452687 >>    imm[11:0] rs1 010 rd 0000111
#----------------------------------------------------------------------------------------------------
fsw fs1,-0x48(a1)                                         #  FSW                  S          0495a427 >>    imm[11:5] rs2 rs1 010 imm[4:0] 0100111
#----------------------------------------------------------------------------------------------------
fmadd.s fs10,ft10,ft5,fa0,rmm                             #  FMADD.S              R4         505f4d43 >>    rs3 00 rs2 rs1 rm rd 1000011
fmsub.s ft8,ft10,fs11,ft7,rdn                             #  FMSUB.S              R4         39bf2e47 >>    rs3 00 rs2 rs1 rm rd 1000111
fnmsub.s fs7,fa4,fs3,fs0,rne                              #  FNMSUB.S             R4         41370bcb >>    rs3 00 rs2 rs1 rm rd 1001011
fnmadd.s fa4,ft4,fs5,fs0,rmm                              #  FNMADD.S             R4         4152474f >>    rs3 00 rs2 rs1 rm rd 1001111
#----------------------------------------------------------------------------------------------------
fadd.s fs2,fa6,ft5,rmm                                    #  FADD.S               R          00584953 >>    0000000 rs2 rs1 rm rd 1010011
fsub.s ft3,ft0,ft1,dyn                                    #  FSUB.S               R          081071d3 >>    0000100 rs2 rs1 rm rd 1010011
fmul.s ft3,ft0,ft1,rtz                                    #  FMUL.S               R          101011d3 >>    0001000 rs2 rs1 rm rd 1010011
fdiv.s fs4,fs6,fs3,rne                                    #  FDIV.S               R          193b0a53 >>    0001100 rs2 rs1 rm rd 1010011
fsqrt.s ft3,ft0,dyn                                       #  FSQRT.S              I          580071d3 >>    0101100 00000 rs1 rm rd 1010011
fsgnj.s ft0,ft1,ft2                                       #  FSGNJ.S              R          20208053 >>    0010000 rs2 rs1 000 rd 1010011
fsgnjn.s ft3,ft7,ft4                                      #  FSGNJN.S             R          204391d3 >>    0010000 rs2 rs1 001 rd 1010011
fsgnjx.s ft8,ft2,ft5                                      #  FSGNJX.S             R          20512e53 >>    0010000 rs2 rs1 010 rd 1010011
fmin.s ft6,ft0,ft1                                        #  FMIN.S               R          28100353 >>    0010100 rs2 rs1 000 rd 1010011
fmax.s ft5,ft3,ft2                                        #  FMAX.S               R          282192d3 >>    0010100 rs2 rs1 001 rd 1010011
fcvt.w.s a1,fa5,rtz                                       #  FCVT.W.S             I          c00795d3 >>    1100000 00000 rs1 rm rd 1010011
fcvt.wu.s a0,ft0,rtz                                      #  FCVT.WU.S            I          c0101553 >>    1100000 00001 rs1 rm rd 1010011
fmv.x.w a0,ft3                                            #  FMV.X.W              I          e0018553 >>    1110000 00000 rs1 000 rd 1010011
feq.s a0,ft0,ft1                                          #  FEQ.S                R          a0102553 >>    1010000 rs2 rs1 010 rd 1010011
flt.s a1,ft2,ft3                                          #  FLT.S                R          a03115d3 >>    1010000 rs2 rs1 001 rd 1010011
fle.s a2,ft5,ft4                                          #  FLE.S                R          a0428653 >>    1010000 rs2 rs1 000 rd 1010011
fclass.s a3,fa2                                           #  FCLASS.S             I          e00616d3 >>    1110000 00000 rs1 001 rd 1010011
fcvt.s.w ft1,a5,rmm                                       #  FCVT.S.W             I          d007c0d3 >>    1101000 00000 rs1 rm rd 1010011
fcvt.s.wu fs0,a2,rdn                                      #  FCVT.S.WU            I          d0162453 >>    1101000 00001 rs1 rm rd 1010011
fmv.w.x ft11,a3                                           #  FMV.W.X              I          f0068fd3 >>    1111000 00000 rs1 000 rd 1010011
#----------------------------------------------------------------------------------------------------
## fmv.x.s              #renamed => fmv.x.w
## fmv.s.x              #renamed => fmv.w.x
## fmv.s                #pseudo
## fneg.s               #pseudo
## fabs.s               #pseudo
## fgt.s                #pseudo
## fge.s                #pseudo
## mffsr                #renamed => frcsr
## mtfsr                #renamed => fscsr
#----------------------------------------------------------------------------------------------------
## frcsr a7             #pseudo    csrrs rd,fcsr,zero     #  FRCSR
## frsr t3              #alias     => frcsr               #  FRSR
## fscsr sp             #pseudo    csrrw zero,fcsr,rs     #  FSCSR
## fssr t6              #alias     => fscsr               #  FSSR
## frrm s5              #pseudo    csrrs rd,frm,zero      #  FRRM
## fsrm t0,t1           #pseudo    csrrw rd,frm,rs        #  FSRM
## fsrmi s3,0x1f        #pseudo    csrrwi rd,frm,uimm     #  FSRMI
## frflags gp           #pseudo    csrrs rd,fflags,zero   #  FRFLAGS
## fsflags tp           #pseudo    csrrw zero,fflags,rs   #  FSFLAGS
## fsflagsi a4,0x1d     #pseudo    csrrwi rd,fflags,uimm  #  FSFLAGSI


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV64F Standard Extension (in addition to RV32F)
#════════════════════════════════════════════════════════════════════════════════════════════════════
fcvt.l.s a0,ft11,rdn                                      #  FCVT.L.S             I          c02fa553 >>    1100000 00010 rs1 rm rd 1010011
fcvt.lu.s a0,ft9,rup                                      #  FCVT.LU.S            I          c03eb553 >>    1100000 00011 rs1 rm rd 1010011
fcvt.s.l ft7,a7,rmm                                       #  FCVT.S.L             I          d028c3d3 >>    1101000 00010 rs1 rm rd 1010011
fcvt.s.lu ft4,a3,rdn                                      #  FCVT.S.LU            I          d036a253 >>    1101000 00011 rs1 rm rd 1010011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV32D Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
fld fa2,-0x4e0(a5)                                        #  FLD                  I          4e07b607 >>    imm[11:0] rs1 011 rd 0000111
#----------------------------------------------------------------------------------------------------
fsd fs4,-0x520(sp)                                        #  FSD                  S          53413027 >>    imm[11:5] rs2 rs1 011 imm[4:0] 0100111
#----------------------------------------------------------------------------------------------------
fmadd.d fs6,ft10,ft5,fa0,rmm                              #  FMADD.D              R4         525f4b43 >>    rs3 01 rs2 rs1 rm rd 1000011
fmsub.d ft0,ft0,fs0,fa6,rne                               #  FMSUB.D              R4         82800047 >>    rs3 01 rs2 rs1 rm rd 1000111
fnmsub.d fs0,ft4,ft2,fa5,rdn                              #  FNMSUB.D             R4         7a22244b >>    rs3 01 rs2 rs1 rm rd 1001011
fnmadd.d fa2,ft10,ft5,fa0,rmm                             #  FNMADD.D             R4         525f464f >>    rs3 01 rs2 rs1 rm rd 1001111
#----------------------------------------------------------------------------------------------------
fadd.d fa7,ft2,ft3,rup                                    #  FADD.D               R          023138d3 >>    0000001 rs2 rs1 rm rd 1010011
fsub.d ft3,ft0,ft1,dyn                                    #  FSUB.D               R          0a1071d3 >>    0000101 rs2 rs1 rm rd 1010011
fmul.d ft5,ft4,fa6,rne                                    #  FMUL.D               R          130202d3 >>    0001001 rs2 rs1 rm rd 1010011
fdiv.d fa1,fa5,fa7,rtz                                    #  FDIV.D               R          1b1795d3 >>    0001101 rs2 rs1 rm rd 1010011
fsqrt.d fa4,fa2,rdn                                       #  FSQRT.D              I          5a062753 >>    0101101 00000 rs1 rm rd 1010011
fsgnj.d ft4,fa1,fs2                                       #  FSGNJ.D              R          23258253 >>    0010001 rs2 rs1 000 rd 1010011
fsgnjn.d ft3,fs1,fa2                                      #  FSGNJN.D             R          22c491d3 >>    0010001 rs2 rs1 001 rd 1010011
fsgnjx.d fs0,ft3,fa7                                      #  FSGNJX.D             R          2311a453 >>    0010001 rs2 rs1 010 rd 1010011
fmin.d ft3,ft0,ft1                                        #  FMIN.D               R          2a1001d3 >>    0010101 rs2 rs1 000 rd 1010011
fmax.d ft2,ft0,ft4                                        #  FMAX.D               R          2a401153 >>    0010101 rs2 rs1 001 rd 1010011
fcvt.s.d fa5,fa2,rne                                      #  FCVT.S.D             I          401607d3 >>    0100000 00001 rs1 rm rd 1010011
fcvt.d.s fa3,fa4                                          #  FCVT.D.S             I          420706d3 >>    0100001 00000 rs1 rm rd 1010011
feq.d a3,fa4,fa2                                          #  FEQ.D                R          a2c726d3 >>    1010001 rs2 rs1 010 rd 1010011
flt.d a5,fa5,fa4                                          #  FLT.D                R          a2e797d3 >>    1010001 rs2 rs1 001 rd 1010011
fle.d a0,fa2,ft1                                          #  FLE.D                R          a2160553 >>    1010001 rs2 rs1 000 rd 1010011
fclass.d a0,fa3                                           #  FCLASS.D             I          e2069553 >>    1110001 00000 rs1 001 rd 1010011
fcvt.w.d a7,ft2,rmm                                       #  FCVT.W.D             I          c20148d3 >>    1100001 00000 rs1 rm rd 1010011
fcvt.wu.d a5,fs3,rup                                      #  FCVT.WU.D            I          c219b7d3 >>    1100001 00001 rs1 rm rd 1010011
fcvt.d.w fa5,zero                                         #  FCVT.D.W             I          d20007d3 >>    1101001 00000 rs1 rm rd 1010011
fcvt.d.wu fa4,a3                                          #  FCVT.D.WU            I          d2168753 >>    1101001 00001 rs1 rm rd 1010011
#----------------------------------------------------------------------------------------------------
## fmv.d                #pseudo
## fneg.d               #pseudo
## fabs.d               #pseudo
## fgt.d                #pseudo
## fge.d                #pseudo


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV64D Standard Extension (in addition to RV32D)
#════════════════════════════════════════════════════════════════════════════════════════════════════
fcvt.l.d ra,ft1,rdn                                       #  FCVT.L.D             I          c220a0d3 >>    1100001 00010 rs1 rm rd 1010011
fcvt.lu.d a0,fa3,rtz                                      #  FCVT.LU.D            I          c2369553 >>    1100001 00011 rs1 rm rd 1010011
fmv.x.d a2,ft3                                            #  FMV.X.D              I          e2018653 >>    1110001 00000 rs1 000 rd 1010011
fcvt.d.l ft1,a3,rmm                                       #  FCVT.D.L             I          d226c0d3 >>    1101001 00010 rs1 rm rd 1010011
fcvt.d.lu ft0,a2,rdn                                      #  FCVT.D.LU            I          d2362053 >>    1101001 00011 rs1 rm rd 1010011
fmv.d.x ft2,a5                                            #  FMV.D.X              I          f2078153 >>    1111001 00000 rs1 000 rd 1010011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV32Q Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
flq fs6,-0x490(tp)                                        #  FLQ                  I          b7024b07 >>    imm[11:0] rs1 100 rd 0000111
#----------------------------------------------------------------------------------------------------
fsq fs7,-0x55f(s3)                                        #  FSQ                  S          5579cfa7 >>    imm[11:5] rs2 rs1 100 imm[4:0] 0100111
#----------------------------------------------------------------------------------------------------
fmadd.q fa0,fs6,fs0,fs6,rup                               #  FMADD.Q              R4         b68b3543 >>    rs3 11 rs2 rs1 rm rd 1000011
fmsub.q ft7,ft4,fa1,fs9,rne                               #  FMSUB.Q              R4         ceb203c7 >>    rs3 11 rs2 rs1 rm rd 1000111
fnmsub.q fs0,ft4,fs2,fa5,rdn                              #  FNMSUB.Q             R4         7f22244b >>    rs3 11 rs2 rs1 rm rd 1001011
fnmadd.q ft10,fa4,fa1,fs3,rtz                             #  FNMADD.Q             R4         9eb71f4f >>    rs3 11 rs2 rs1 rm rd 1001111
#----------------------------------------------------------------------------------------------------
fadd.q ft7,fa3,fs2,rne                                    #  FADD.Q               R          072683d3 >>    0000011 rs2 rs1 rm rd 1010011
fsub.q ft8,fs1,ft10,rtz                                   #  FSUB.Q               R          0fe49e53 >>    0000111 rs2 rs1 rm rd 1010011
fmul.q ft8,fs0,fa7,rup                                    #  FMUL.Q               R          17143e53 >>    0001011 rs2 rs1 rm rd 1010011
fdiv.q fs10,fs9,fs11,rmm                                  #  FDIV.Q               R          1fbccd53 >>    0001111 rs2 rs1 rm rd 1010011
fsqrt.q fs10,fs11,dyn                                     #  FSQRT.Q              I          5e0dfd53 >>    0101111 00000 rs1 rm rd 1010011
fsgnj.q ft7,ft3,ft2                                       #  FSGNJ.Q              R          262183d3 >>    0010011 rs2 rs1 000 rd 1010011
fsgnjn.q fs0,ft1,fa2                                      #  FSGNJN.Q             R          26c09453 >>    0010011 rs2 rs1 001 rd 1010011
fsgnjx.q fs2,fa3,ft3                                      #  FSGNJX.Q             R          2636a953 >>    0010011 rs2 rs1 010 rd 1010011
fmin.q fa0,ft3,fs7                                        #  FMIN.Q               R          2f718553 >>    0010111 rs2 rs1 000 rd 1010011
fmax.q ft0,fs4,fa2                                        #  FMAX.Q               R          2eca1053 >>    0010111 rs2 rs1 001 rd 1010011
fcvt.s.q fs10,fs9,rdn                                     #  FCVT.S.Q             I          403cad53 >>    0100000 00011 rs1 rm rd 1010011
fcvt.q.s fs11,fs4                                         #  FCVT.Q.S             I          460a0dd3 >>    0100011 00000 rs1 rm rd 1010011
fcvt.d.q fs7,fs6,rup                                      #  FCVT.D.Q             I          423b3bd3 >>    0100001 00011 rs1 rm rd 1010011
fcvt.q.d fs3,fs5                                          #  FCVT.Q.D             I          461a89d3 >>    0100011 00001 rs1 rm rd 1010011
feq.q t2,fs3,ft2                                          #  FEQ.Q                R          a629a3d3 >>    1010011 rs2 rs1 010 rd 1010011
flt.q s0,ft7,fs10                                         #  FLT.Q                R          a7a39453 >>    1010011 rs2 rs1 001 rd 1010011
fle.q s2,fa0,ft7                                          #  FLE.Q                R          a6750953 >>    1010011 rs2 rs1 000 rd 1010011
fclass.q a3,fa6                                           #  FCLASS.Q             I          e60816d3 >>    1110011 00000 rs1 001 rd 1010011
fcvt.w.q zero,ft3,rne                                     #  FCVT.W.Q             I          c6018053 >>    1100011 00000 rs1 rm rd 1010011
fcvt.wu.q ra,fs4,rtz                                      #  FCVT.WU.Q            I          c61a10d3 >>    1100011 00001 rs1 rm rd 1010011
fcvt.q.w fa7,t1                                           #  FCVT.Q.W             I          d60308d3 >>    1101011 00000 rs1 rm rd 1010011
fcvt.q.wu ft5,tp                                          #  FCVT.Q.WU            I          d61202d3 >>    1101011 00001 rs1 rm rd 1010011
#----------------------------------------------------------------------------------------------------
## fgt.q                #pseudo
## fge.q                #pseudo


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV64Q Standard Extension (in addition to RV32Q)
#════════════════════════════════════════════════════════════════════════════════════════════════════
fcvt.l.q gp,fa2,rdn                                       #  FCVT.L.Q             R          c62621d3 >>    1100011 00010 rs1 rm rd 1010011
fcvt.lu.q ra,ft3,rne                                      #  FCVT.LU.Q            R          c63180d3 >>    1100011 00011 rs1 rm rd 1010011
fcvt.q.l fs3,t3,rtz                                       #  FCVT.Q.L             R          d62e19d3 >>    1101011 00010 rs1 rm rd 1010011
fcvt.q.lu fa5,sp,rmm                                      #  FCVT.Q.LU            R          d63147d3 >>    1101011 00011 rs1 rm rd 1010011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV32Zfh Standard Extension
#════════════════════════════════════════════════════════════════════════════════════════════════════
flh fa4,-0xc(a7)                                          #  FLH                  I          00c89707 >>    imm[11:0] rs1 001 rd 0000111
fsh fa3,-0x15(a5)                                         #  FSH                  S          00d79aa7 >>    imm[11:5] rs2 rs1 001 imm[4:0] 0100111
fmadd.h fa4,fa2,fa1,fa7,rne                               #  FMADD.H              R4         8cb60743 >>    rs3 10 rs2 rs1 rm rd 1000011
fmsub.h fa3,fa1,fa6,fa2,rdn                               #  FMSUB.H              R4         6505a6c7 >>    rs3 10 rs2 rs1 rm rd 1000111
fnmsub.h fa4,fa3,fa5,fa6,rtz                              #  FNMSUB.H             R4         84f6974b >>    rs3 10 rs2 rs1 rm rd 1001011
fnmadd.h fa1,fa7,fa4,fa2,rup                              #  FNMADD.H             R4         64e8b5cf >>    rs3 10 rs2 rs1 rm rd 1001111
fadd.h fa2,fa3,fa6,rmm                                    #  FADD.H               R          0506c653 >>    0000010 rs2 rs1 rm rd 1010011
fsub.h fa3,fa4,fa6,dyn                                    #  FSUB.H               R          0d0776d3 >>    0000110 rs2 rs1 rm rd 1010011
fmul.h fa2,fa5,fa4,rdn                                    #  FMUL.H               R          14e7a653 >>    0001010 rs2 rs1 rm rd 1010011
fdiv.h fa4,fa6,fa7,rne                                    #  FDIV.H               R          1d180753 >>    0001110 rs2 rs1 rm rd 1010011
fsqrt.h fa3,fa6,rne                                       #  FSQRT.H              I          5c0806d3 >>    0101110 00000 rs1 rm rd 1010011
fsgnj.h fa5,fa2,fa1                                       #  FSGNJ.H              R          24b607d3 >>    0010010 rs2 rs1 000 rd 1010011
fsgnjn.h fa4,fa5,fa7                                      #  FSGNJN.H             R          25179753 >>    0010010 rs2 rs1 001 rd 1010011
fsgnjx.h fa3,fa4,fa2                                      #  FSGNJX.H             R          24c726d3 >>    0010010 rs2 rs1 010 rd 1010011
fmin.h fa4,fa3,fa5                                        #  FMIN.H               R          2cf68753 >>    0010110 rs2 rs1 000 rd 1010011
fmax.h fa1,fa7,fa2                                        #  FMAX.H               R          2cc895d3 >>    0010110 rs2 rs1 001 rd 1010011
fcvt.s.h fa2,fa5                                          #  FCVT.S.H             I          40278653 >>    0100000 00010 rs1 rm rd 1010011
fcvt.h.s fa1,fa4,rtz                                      #  FCVT.H.S             I          440715d3 >>    0100010 00000 rs1 rm rd 1010011
fcvt.d.h fa4,fa7                                          #  FCVT.D.H             I          42288753 >>    0100001 00010 rs1 rm rd 1010011
fcvt.h.d fa3,fa4,rup                                      #  FCVT.H.D             I          441736d3 >>    0100010 00001 rs1 rm rd 1010011
fcvt.q.h fa7,fa6                                          #  FCVT.Q.H             I          462808d3 >>    0100011 00010 rs1 rm rd 1010011
fcvt.h.q fa7,fa2,rmm                                      #  FCVT.H.Q             I          443648d3 >>    0100010 00011 rs1 rm rd 1010011
feq.h a5,fa1,fa5                                          #  FEQ.H                R          a4f5a7d3 >>    1010010 rs2 rs1 010 rd 1010011
flt.h a3,fa3,fa6                                          #  FLT.H                R          a50696d3 >>    1010010 rs2 rs1 001 rd 1010011
fle.h a7,fa2,fa4                                          #  FLE.H                R          a4e608d3 >>    1010010 rs2 rs1 000 rd 1010011
fclass.h a2,fa4                                           #  FCLASS.H             I          e4071653 >>    1110010 00000 rs1 001 rd 1010011
fcvt.w.h a7,fa1,dyn                                       #  FCVT.W.H             I          c405f8d3 >>    1100010 00000 rs1 rm rd 1010011
fcvt.wu.h a4,fa3,rtz                                      #  FCVT.WU.H            I          c4169753 >>    1100010 00001 rs1 rm rd 1010011
fmv.x.h a5,fa1                                            #  FMV.X.H              I          e40587d3 >>    1110010 00000 rs1 000 rd 1010011
fcvt.h.w fa3,a4,rup                                       #  FCVT.H.W             I          d40736d3 >>    1101010 00000 rs1 rm rd 1010011
fcvt.h.wu fa6,a1,rdn                                      #  FCVT.H.WU            I          d415a853 >>    1101010 00001 rs1 rm rd 1010011
fmv.h.x fa2,a6                                            #  FMV.H.X              I          f4080653 >>    1111010 00000 rs1 000 rd 1010011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# RV64Zfh Standard Extension (in addition to RV32Zfh)
#════════════════════════════════════════════════════════════════════════════════════════════════════
fcvt.l.h a7,fa4,rtz                                       #  FCVT.L.H             I          c42718d3 >>    1100010 00010 rs1 rm rd 1010011
fcvt.lu.h a1,fa5,rne                                      #  FCVT.LU.H            I          c43785d3 >>    1100010 00011 rs1 rm rd 1010011
fcvt.h.l fa3,a6,rdn                                       #  FCVT.H.L             I          d42826d3 >>    1101010 00010 rs1 rm rd 1010011
fcvt.h.lu fa2,a7,rmm                                      #  FCVT.H.LU            I          d438c653 >>    1101010 00011 rs1 rm rd 1010011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Compressed instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
c.unimp                                                   #  C.UNIMP              .          0000     >>    0000 0000 0000 0000
c.ebreak                                                  #  C.EBREAK             CR         9002     >>    100 1 00000 00000 10
## c.jr ra                                                #  C.JR                 CR         8082     >>    100 0 rs1 00000 10
## c.jalr a1                                              #  C.JALR               CR         9582     >>    100 1 rs1 00000 10
c.j target1                                               #  C.J TARGET           CJ         a019     >>    101 imm[11|4|9:8|10|6|7|3:1|5] 01

target1:
## c.jal                 #RV32 only                       #  C.JAL                CJ         2009     >>    001 imm[11|4|9:8|10|6|7|3:1|5] 01
c.beqz s0,target2                                         #  C.BEQZ               CB         c011     >>    110 imm[8|4:3] rs1.3 imm[7:6|2:1|5] 01

target2:
c.bnez a2,target3                                         #  C.BNEZ               CB         e209     >>    1110 imm[8|4:3] rs1.3 imm[7:6|2:1|5] 01

target3:
c.lwsp ra,0xd4(sp)                                        #  C.LWSP               CI         40de     >>    010 uimm[5] rd uimm[4:2|7:6] 10
c.lw s1,0x14(a0)                                          #  C.LW                 CL         4944     >>    010 uimm[5:3] rs1 uimm[2|6] rd 00
c.swsp ra,0x1c(sp)                                        #  C.SWSP               CSS        ce06     >>    110 uimm[5:2|7:6] rs2 10
c.sw s0,0x44(s1)                                          #  C.SW                 CS         c0a0     >>    110 uimm[5:3] rs1 uimm[2|6] rs2 00
c.nop                                                     #  C.NOP                CI         0001     >>    000 imm[5] 00000 imm[4:0] 01
c.mv tp,ra                                                #  C.MV                 CR         8206     >>    100 0 rd rs2 10
c.lui gp,0xfffe0                                          #  C.LUI                CI         61ed     >>    011 imm[17] rd imm[16:12] 01
c.li ra,-0x1f                                             #  C.LI                 CI         5085     >>    010 imm[5] rd imm[4:0] 01
c.addi4spn a3,sp,0x4                                      #  C.ADDI4SPN           CIW        0040     >>    000 uimm[5:4|9:6|2|3] rd 00
c.addi16sp sp,-0x1c0                                      #  C.ADDI16SP           CI         xxxx     >>    011 imm[9] 00010 imm[4|6|8:7|5] 01
c.add ra,s0                                               #  C.ADD                CR         908a     >>    100 1 rs1/rd rs2 10
c.addi a1,-0x1e                                           #  C.ADDI               CI         05f1     >>    000 imm[5] rs1/rd imm[4:0] 01
c.sub s1,a1                                               #  C.SUB                CA         8c8d     >>    100 0 11 rs1/rd.3 00 rs2.3 01
c.and s0,a2                                               #  C.AND                CA         8c71     >>    100 0 11 rs1/rd.3 11 rs2.3 01
c.or s0,a3                                                #  C.OR                 CA         8c55     >>    100 0 11 rs1/rd.3 10 rs2.3 01
c.xor s1,a4                                               #  C.XOR                CA         8cb9     >>    100 0 11 rs1/rd.3 01 rs2.3 01
c.slli ra,0xa                                             #  C.SLLI               CI         xxxx     >>    000 uimm[5] rs1/rd uimm[4:0] 10
c.srli s1,0xb                                             #  C.SRLI               CB         xxxx     >>    100 imm[5] 00 rs1/rd.3 imm[4:0] 01
c.srai a1,0xc                                             #  C.SRAI               CB         xxxx     >>    100 uimm[5] 01 rs1/rd.3 uimm[4:0] 01
c.slli64 a5                                               #  C.SLLI64             CB         0782     >>    000 0 rs1/rd 00000 10
c.srli64 a2                                               #  C.SRLI64             CB         8201     >>    100 0 00 rs1/rd.3 00000 01
c.srai64 a3                                               #  C.SRAI64             CB         8681     >>    100 0 01 rs1/rd.3 00000 01
c.andi s0,-0x1c                                           #  C.ANDI               CB         887d     >>    100 uimm[5] 10 rs1/rd.3 uimm[4:0] 01
c.addiw a1,-0x17                                          #  C.ADDIW              CI         25d5     >>    001 imm[5] rs1/rd imm[4:0] 01
c.addw s0,s1                                              #  C.ADDW               CA         9c25     >>    100 1 11 rs1/rd.3 01 rs2.3 01
c.subw s1,s0                                              #  C.SUBW               CA         9c81     >>    100 1 11 rs1/rd.3 00 rs2.3 01
c.ldsp ra,0xd8(sp)                                        #  C.LDSP               CI         60ee     >>    011 uimm[5] rd uimm[4:3|8:6] 10
## c.lqsp gp,0x4f(sp)    #RV128C only                     #  C.LQSP               CI         xxxx     >>    001 uimm[5] rd uimm[4|9:6] 10
c.ld s1,0x18(a0)                                          #  C.LD                 CL         xxxx     >>    011 uimm[5:3] rs1 uimm[7:6] rd 00
## c.lq s2,0x1a(a1)      #RV128C only                     #  C.LQ                 CL         xxxx     >>    001 uimm[5:4|8] rs1 uimm[7:6] rd 00
c.sdsp ra,0x20(sp)                                        #  C.SDSP               CSS        xxxx     >>    111 uimm[5:3|8:6] rs2 10
## c.sqsp s2,0X2a(sp)    #RV128C only                     #  C.SQSP               CSS        xxxx     >>    101 uimm[5:4|9:6] rs2 10
c.sd s1,0x28(a1)                                          #  C.SD                 CS         xxxx     >>    111 uimm[5:3] rs1 uimm[7:6] rs2 00
## c.sq s2,0xcc(a2)      #RV128C only                     #  C.SQ                 CS         xxxx     >>    101 uimm[5:4|8] rs1 uimm[7:6] rs2 00
c.fldsp fa1,0xe8(sp)                                      #  C.FLDSP              CI         xxxx     >>    001 uimm[5] rd uimm[4:3|8:6] 10
c.fld fa3,0x38(s0)                                        #  C.FLD                CL         xxxx     >>    001 uimm[5:3] rs1 uimm[7:6] rd 00
c.fsdsp fa1,0x28(sp)                                      #  C.FSDSP              CSS        xxxx     >>    101 uimm[5:3|8:6] rs2 10
c.fsd fa1,0x60(s0)                                        #  C.FSD                CS         xxxx     >>    101 uimm[5:3] rs1 uimm[7:6] rs2 00
## c.flwsp fa1,8(sp)     #RV32 only                       #  C.FLWSP              CI         65a2     >>    011 uimm[5] rd uimm[4:2|7:6] 10
## c.flw fa4,0(s0)       #RV32 only                       #  C.FLW                CL         6018     >>    011 uimm[5:3] rs1 uimm[2|6] rd 00
## c.fswsp ft5,0(sp)     #RV32 only                       #  C.FSWSP              CSS        e016     >>    111 uimm[5:2|7:6] rs2 10
## c.fsw fa3,0(s0)       #RV32 only                       #  C.FSW                CS         e014     >>    111 uimm[5:3] rs1 uimm[2|6] rs2 00


#════════════════════════════════════════════════════════════════════════════════════════════════════
# HINT instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
prefetch.i -0x20(x1)                                      #  PREFETCH.I           .          0200e013 >>     0000001 00000 00001 110 00000 0010011
prefetch.r -2048(x16)                                     #  PREFETCH.R           .          80186013 >>     1000000 00001 10000 110 00000 0010011
prefetch.w -0x7e0(x31)                                    #  PREFETCH.W           .          7e3fe013 >>     0111111 00011 11111 110 00000 0010011
pause                                                     #  PAUSE                I          0100000f >>     0000000 10000 00000 000 00000 0001111


#════════════════════════════════════════════════════════════════════════════════════════════════════
# C.HINT instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
c.nop 28                                                  #  C.NOP                CI         0071     >>    0000 0000 0111 0001
c.addi s1,0x0                                             #  C.ADDI               CI         0481     >>    0000 0100 1000 0001
c.li zero,0x7                                             #  C.LI                 CI         401d     >>    0100 0000 0001 1101
c.lui zero,0x1a                                           #  C.LUI                CI         xxxx     >>    0110 0000 0010 1101
c.mv zero,a5                                              #  C.MV                 CR         803e     >>    1000 0000 0011 1110
c.add zero,s2                                             #  C.ADD                CR         904a     >>    1001 0000 0100 1010
c.add zero,sp                                             #  C.NTL.P1             CR         900a     >>    1001 0000 0000 1010
c.add zero,gp                                             #  C.NTL.PALL           CR         900e     >>    1001 0000 0000 1110
c.add zero,tp                                             #  C.NTL.S1             CR         9012     >>    1001 0000 0001 0010
c.add zero,t0                                             #  C.NTL.ALL            CR         9016     >>    1001 0000 0001 0110


#════════════════════════════════════════════════════════════════════════════════════════════════════
# ZCB instructions ( needs gcc 2.42 )
#════════════════════════════════════════════════════════════════════════════════════════════════════
## c.lbu x8,(a5)                                          #  C.LBU                .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.lhu s0,(a5)                                          #  C.LHU                .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.lh s1,33(a1)                                         #  C.LH                 CL         xxxx     >>    xxxx xxxx xxxx xxxx
## c.sb a0,(s6)                                           #  C.SB                 .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.sh a1,(a5)                                           #  C.SH                 .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.not a4                                               #  C.NOT                .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.mul s1,a2                                            #  C.MUL                .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.sext.b a1                                            #  C.SEXT.B             .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.sext.h a3                                            #  C.SEXT.H             .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.zext.h a4                                            #  C.ZEXT.H             .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.zext.w a5                                            #  C.ZEXT.W             .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.zext.b t1                                            #  C.ZEXT.B             .          xxxx     >>    xxxx xxxx xxxx xxxx
## c.sext.w t5                                            #  C.SEXT.W             .          xxxx     >>    xxxx xxxx xxxx xxxx


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zicbom and Zicboz instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
# https://github.com/riscv/riscv-CMOs/blob/master/cmobase/insns/cbo.clean.adoc
cbo.clean 0(x3)                                           #  CBO.CLEAN            .          001f200f >>    000000000001 rs1   010 00000 0001111
cbo.flush 0(x10)                                          #  CBO.FLUSH            .          002f200f >>    000000000010 rs1   010 00000 0001111
cbo.inval 0(x27)                                          #  CBO.INVAL            .          000f200f >>    000000000000 rs1   010 00000 0001111
cbo.zero 0(x31)                                           #  CBO.ZERO             .          004f200f >>    000000000100 rs1   010 00000 0001111


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zawrs instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
# https://github.com/riscv/riscv-zawrs/blob/main/zawrs.adoc
wrs.nto                                                   #  WRS.NTO              .          00d00073 >>    000000001101 00000 000 00000 1110011
wrs.sto                                                   #  WRS.STO              .          01d00073 >>    000000011101 00000 000 00000 1110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zicond instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
# https://github.com/riscv/riscv-zicond/blob/main/zicondops.adoc
czero.eqz a0,a1,a2                                        #  CZERO.EQZ            R          0ec5d533 >>    0000111 rs2 rs1 101 rd 0110011
czero.nez a0,a3,a4                                        #  CZERO.NEZ            R          0ee6f533 >>    0000111 rs2 rs1 111 rd 0110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zfa instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
fli.s ft3,0x1p-3                                          #  FLI.S                I          f01381d3 >>    1111000 00001 rs1 000 rd 1010011
fli.d ft7,0.25                                            #  FLI.D                I          f21403d3 >>    1111001 00001 rs1 000 rd 1010011
fli.q ft5,0.25                                            #  FLI.Q                I          f61402d3 >>    1111011 00001 rs1 000 rd 1010011
fli.h ft11,8.0                                            #  FLI.H                I          f41c0fd3 >>    1111010 00001 rs1 000 rd 1010011
fminm.s ft3,ft0,ft1                                       #  FMINM.S              R          281021d3 >>    0010100 rs2 rs1 010 rd 1010011
fmaxm.s ft7,ft1,ft2                                       #  FMAXM.S              R          2820b3d3 >>    0010100 rs2 rs1 011 rd 1010011
fminm.d ft3,ft0,ft1                                       #  FMINM.D              R          2a1021d3 >>    0010101 rs2 rs1 010 rd 1010011
fmaxm.d ft2,ft0,ft4                                       #  FMAXM.D              R          2a403153 >>    0010101 rs2 rs1 011 rd 1010011
fminm.q fs1,ft3,fa7                                       #  FMINM.Q              R          2f11a4d3 >>    0010111 rs2 rs1 010 rd 1010011
fmaxm.q ft3,fa4,ft5                                       #  FMAXM.Q              R          2e5731d3 >>    0010111 rs2 rs1 011 rd 1010011
fminm.h fa4,fa3,fa5                                       #  FMINM.H              R          2cf6a753 >>    0010110 rs2 rs1 010 rd 1010011
fmaxm.h fa1,fa7,fa2                                       #  FMAXM.H              R          2cc8b5d3 >>    0010110 rs2 rs1 011 rd 1010011
fround.s ft5,ft3,rtz                                      #  FROUND.S             I          404192d3 >>    0100000 00100 rs1 rm rd 1010011
froundnx.s ft5,ft3,rup                                    #  FROUNDNX.S           I          4051b2d3 >>    0100000 00101 rs1 rm rd 1010011
fround.d ft3,ft0,rne                                      #  FROUND.D             I          424001d3 >>    0100001 00100 rs1 rm rd 1010011
froundnx.d ft3,ft0,rtz                                    #  FROUNDNX.D           I          425011d3 >>    0100001 00101 rs1 rm rd 1010011
fround.q ft7,ft1,rdn                                      #  FROUND.Q             I          4640a3d3 >>    0100011 00100 rs1 rm rd 1010011
froundnx.q ft7,ft1,rmm                                    #  FROUNDNX.Q           I          4650c3d3 >>    0100011 00101 rs1 rm rd 1010011
fround.h ft6,ft0,rmm                                      #  FROUND.H             I          44404353 >>    0100010 00100 rs1 rm rd 1010011
froundnx.h ft6,ft0,dyn                                    #  FROUNDNX.H           I          44507353 >>    0100010 00101 rs1 rm rd 1010011
fcvtmod.w.d a3,ft1,rtz                                    #  FCVTMOD.W.D          I          c28096d3 >>    1100001 01000 rs1 001 rd 1010011
## fmvh.x.d a2,ft1       #RV32 only                       #  FMVH.X.D             I          e2108653 >>    1110001 00001 rs1 000 rd 1010011
## fmvp.d.x ft3,a4,a7    #RV32 only                       #  FMVP.D.X             R          b31701d3 >>    1011001 rs2 rs1 000 rd 1010011
fmvh.x.q a1,ft1                                           #  FMVH.X.Q             R          e61085d3 >>    1110011 rs2 rs1 000 rd 1010011
fmvp.q.x ft2,a3,a5                                        #  FMVP.Q.X             R          b6f68153 >>    1011011 rs2 rs1 000 rd 1010011
fltq.s a1,ft7,ft3                                         #  FLTQ.S               R          a033d5d3 >>    1010000 rs2 rs1 101 rd 1010011
fleq.s a2,ft11,ft7                                        #  FLEQ.S               R          a07fc653 >>    1010000 rs2 rs1 100 rd 1010011
fltq.d a3,ft0,ft5                                         #  FLTQ.D               R          a25056d3 >>    1010001 rs2 rs1 101 rd 1010011
fleq.d a4,ft8,ft4                                         #  FLEQ.D               R          a24e4753 >>    1010001 rs2 rs1 100 rd 1010011
fltq.q a5,ft3,ft11                                        #  FLTQ.Q               R          a7f1d7d3 >>    1010011 rs2 rs1 101 rd 1010011
fleq.q a6,ft6,ft8                                         #  FLEQ.Q               R          a7c34853 >>    1010011 rs2 rs1 100 rd 1010011
fltq.h a7,ft5,ft2                                         #  FLTQ.H               R          a422d8d3 >>    1010010 rs2 rs1 101 rd 1010011
fleq.h a5,ft2,ft11                                        #  FLEQ.H               R          a5f147d3 >>    1010010 rs2 rs1 100 rd 1010011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zvbb instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
vandn.vv v1,v8,v12,v0.t                                   #  VANDN.VV             .          048600d7 >>    000001 vm vs2 vs1 000 vd 1010111
vandn.vx v2,v7,a1,v0.t                                    #  VANDN.VX             .          0475c157 >>    000001 vm vs2 rs1 100 vd 1010111
vbrev.v v3,v6,v0.t                                        #  VBREV.V              .          486521d7 >>    010010 vm vs2 01010 010 vd 1010111
vbrev8.v v4,v5,v0.t                                       #  VBREV8.V             .          48542257 >>    010010 vm vs2 01000 010 vd 1010111
vrev8.v v5,v4,v0.t                                        #  VREV8.V              .          4844a2d7 >>    010010 vm vs2 01001 010 vd 1010111
vclz.v v6,v3,v0.t                                         #  VCLZ.V               .          48362357 >>    010010 vm vs2 01100 010 vd 1010111
vctz.v v7,v2,v0.t                                         #  VCTZ.V               .          4826a3d7 >>    010010 vm vs2 01101 010 vd 1010111
vcpop.v v8,v1,v0.t                                        #  VCPOP.V              .          48172457 >>    010010 vm vs2 01110 010 vd 1010111
vrol.vv v9,v8,v12,v0.t                                    #  VROL.VV              .          548604d7 >>    010101 vm vs2 vs1 000 vd 1010111
vrol.vx v10,v7,a1,v0.t                                    #  VROL.VX              .          5475c557 >>    010101 vm vs2 rs1 100 vd 1010111
vror.vv v11,v6,v12,v0.t                                   #  VROR.VV              .          506605d7 >>    010100 vm vs2 vs1 000 vd 1010111
vror.vx v12,v5,a1,v0.t                                    #  VROR.VX              .          5055c657 >>    010100 vm vs2 rs1 100 vd 1010111
vror.vi v13,v4,63,v0.t                                    #  VROR.VI              .          544fb6d7 >>    010101 vm vs2 uimm 011 vd 1010111
vwsll.vv v14,v3,v12,v0.t                                  #  VWSLL.VV             .          d4360757 >>    110101 vm vs2 vs1 000 vd 1010111
vwsll.vx v15,v2,a1,v0.t                                   #  VWSLL.VX             .          d425c7d7 >>    110101 vm vs2 rs1 100 vd 1010111
vwsll.vi v16,v1,31,v0.t                                   #  VWSLL.VI             .          d41fb857 >>    110101 vm vs2 uimm 011 vd 1010111


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zbb or zbkb instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
clz a0,a1                                                 #  CLZ                  .          60059513 >>    0110000 00000 rs1 001 rd 0010011
ctz a1,a2                                                 #  CTZ                  .          60161593 >>    0110000 00001 rs1 001 rd 0010011
cpop a2,a3                                                #  CPOP                 .          60269613 >>    0110000 00010 rs1 001 rd 0010011
min a3,a1,a2                                              #  MIN                  R          0ac5c6b3 >>    0000101 rs2 rs1 100 rd 0110011
max a4,a1,a2                                              #  MAX                  R          0ac5e733 >>    0000101 rs2 rs1 110 rd 0110011
minu a5,a1,a2                                             #  MINU                 R          0ac5d7b3 >>    0000101 rs2 rs1 101 rd 0110011
maxu a6,a1,a2                                             #  MAXU                 R          0ac5f833 >>    0000101 rs2 rs1 111 rd 0110011
sext.b a7,a5                                              #  SEXT.B               .          60479893 >>    0110000 00100 rs1 001 rd 0010011
sext.h a1,a2                                              #  SEXT.H               .          60561593 >>    0110000 00101 rs1 001 rd 0010011
zext.h a2,a3                                              #  ZEXT.H               R          0806c63b >>    0000100 rs2 rs1 100 rd 0111011
orc.b a3,a2                                               #  ORC.B                .          28765693 >>    00101 imm rs1 101 rd 0010011
clzw a5,a7                                                #  CLZW                 .          6008979b >>    0110000 00000 rs1 001 rd 0011011
ctzw a6,a3                                                #  CTZW                 .          6016981b >>    0110000 00001 rs1 001 rd 0011011
cpopw a7,a5                                               #  CPOPW                .          6027989b >>    0110000 00010 rs1 001 rd 0011011
brev8 a5,sp                                               #  BREV8                .          68715793 >>    011010000111 rs 101 rd 0010011
## zip a2,a5             #RV32 only                       #  ZIP                  .          08f79613 >>    000010001111 rs1 001 rd 0010011
## unzip a1,a6           #RV32 only                       #  UNZIP                .          08f85593 >>    000010001111 rs1 101 rd 0010011
pack a3,a1,a2                                             #  PACK                 R          08c5c6b3 >>    0000100 rs2 rs1 100 rd 0110011
packh a7,a1,a2                                            #  PACKH                R          08c5f8b3 >>    0000100 rs2 rs1 111 rd 0110011
packw a6,a1,a2                                            #  PACKW                R          08c5c83b >>    0000100 rs2 rs1 100 rd 0111011
andn a4,a1,a2                                             #  ANDN                 R          40c5f733 >>    0100000 rs2 rs1 111 rd 0110011
orn a2,a1,a2                                              #  ORN                  R          40c5e633 >>    0100000 rs2 rs1 110 rd 0110011
xnor a7,a1,a2                                             #  XNOR                 R          40c5c8b3 >>    0100000 rs2 rs1 100 rd 0110011
rol a2,a1,a2                                              #  ROL                  R          60c59633 >>    0110000 rs2 rs1 001 rd 0110011
rori a5,a1,2                                              #  RORI                 .          6025d793 >>    011000 shamt rs1 101 rd 0010011
ror a3,a1,a2                                              #  ROR                  R          60c5d6b3 >>    0110000 rs2 rs1 101 rd 0110011
## rev8 a6,a7            #RV32 encoding                   #  REV8                 .          6988d813 >>    011010011000 rs 101 rd 0010011
rev8 a6,a7               #RV64 encoding                   #  REV8                 .          6b88d813 >>    011010111000 rs 101 rd 0010011
rolw a4,a1,a2                                             #  ROLW                 R          60c5973b >>    0110000 rs2 rs1 001 rd 0111011
roriw a6,a1,2                                             #  RORIW                .          6025d81b >>    0110000 shamti rs1 101 rd 0011011
rorw a2,a1,a2                                             #  RORW                 R          60c5d63b >>    0110000 rs2 rs1 101 rd 0111011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zba instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
sh1add a6,a3,a2                                           #  SH1ADD               R          20c6a833 >>    0010000 rs2 rs1 010 rd 0110011
sh2add a7,a1,a5                                           #  SH2ADD               R          20f5c8b3 >>    0010000 rs2 rs1 100 rd 0110011
sh3add a3,a5,a7                                           #  SH3ADD               R          2117e6b3 >>    0010000 rs2 rs1 110 rd 0110011
sh1add.uw a5,a6,a2                                        #  SH1ADD.UW            R          20c827bb >>    0010000 rs2 rs1 010 rd 0111011
sh2add.uw a5,a7,a4                                        #  SH2ADD.UW            R          20e8c7bb >>    0010000 rs2 rs1 100 rd 0111011
sh3add.uw a7,a1,a5                                        #  SH3ADD.UW            R          20f5e8bb >>    0010000 rs2 rs1 110 rd 0111011
zext.w a4,a3             #pseudo ( add.uw )               #  ZEXT.W               R          0806873b >>    0000100 rs2 rs1 000 rd 0111011
add.uw a6,a7,a1                                           #  ADD.UW               R          08b8883b >>    0000100 rs2 rs1 000 rd 0111011
slli.uw a1,a3,7                                           #  SLLI.UW              .          0876959b >>    00001 imm rs1 001 rd 0011011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zbc or zbkc instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
clmul a7,a1,a2                                            #  CLMUL                R          0ac598b3 >>    0000101 rs2 rs1 001 rd 0110011
clmulh a3,a7,a5                                           #  CLMULH               R          0af8b6b3 >>    0000101 rs2 rs1 011 rd 0110011
clmulr a4,a3,a7                                           #  CLMULR               R          0b16a733 >>    0000101 rs2 rs1 010 rd 0110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zbs instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
bclri a7,a1,31                                            #  BCLRI                .          49f59893 >>    01001 imm rs1 001 rd 0010011
bclr a5,a3,a7                                             #  BCLR                 R          491697b3 >>    0100100 rs2 rs1 001 rd 0110011
bseti a4,a7,3                                             #  BSETI                .          28389713 >>    00101 imm rs1 001 rd 0010011
bset a6,a5,a3                                             #  BSET                 R          28d79833 >>    0010100 rs2 rs1 001 rd 0110011
binvi a5,a7,17                                            #  BINVI                .          69189793 >>    01101 imm rs1 001 rd 0010011
binv a1,a6,a3                                             #  BINV                 R          68d815b3 >>    0110100 rs2 rs1 001 rd 0110011
bexti a0,a3,63                                            #  BEXTI                .          4bf6d513 >>    01001 imm rs1 101 rd 0010011
bext a3,a1,a2                                             #  BEXT                 R          48c5d6b3 >>    0100100 rs2 rs1 101 rd 0110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zbkx instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
xperm4 a3,a6,a2                                           #  XPERM4 / XPERM.N     R          28c826b3 >>    0010100 rs2 rs1 010 rd 0110011
xperm8 a4,a1,a3                                           #  XPERM8 / XPERM.B     R          28d5c733 >>    0010100 rs2 rs1 100 rd 0110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zknd instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
## aes32dsi a7,a5,a2,2   #RV32 only                       #  AES32DSI             .          aac788b3 >>    bs 10101 rs2 rs1 000 rd 0110011
## aes32dsmi a5,a3,a2,2  #RV32 only                       #  AES32DSMI            .          aec687b3 >>    bs 10111 rs2 rs1 000 rd 0110011
aes64ds a4,a6,a2                                          #  AES64DS              .          3ac80733 >>    0011101 rs2 rs1 000 ds 0110011
aes64dsm a3,a4,a2                                         #  AES64DSM             .          3ec706b3 >>    0011111 rs2 rs1 000 ds 0110011
aes64im a7,a2                                             #  AES64IM              .          30061893 >>    001100000000 rs1 001 rd 0010011
aes64ks1i a1,a6,4                                         #  AES64KS1I            .          31481593 >>    00110001 rnum rs1 001 rd 0010011
aes64ks2 a6,a1,a2                                         #  AES64KS2             R          7ec58833 >>    0111111 rs2 rs1 000 rd 0110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zkne instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
## aes32esi a7,a3,a2,2   #RV32 only                       #  AES32ESI             .          a2c688b3 >>    bs 10001 rs2 rs1 000 rd 0110011
## aes32esmi a3,a6,a2,2  #RV32 only                       #  AES32ESMI            .          a6c806b3 >>    bs 10011 rs2 rs1 000 rd 0110011
aes64es a4,a1,a2                                          #  AES64ES              R          32c58733 >>    0011001 rs2 rs1 000 rd 0110011
aes64esm a5,a6,a2                                         #  AES64ESM             R          36c807b3 >>    0011011 rs2 rs1 000 rd 0110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zknh instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
sha256sig0 a7,a5                                          #  SHA256SIG0           .          10279893 >>    000100000010 rs1 001 rd 0010011
sha256sig1 a5,a6                                          #  SHA256SIG1           .          10381793 >>    000100000011 rs1 001 rd 0010011
sha256sum0 a3,a4                                          #  SHA256SUM0           .          10071693 >>    000100000000 rs1 001 rd 0010011
sha256sum1 a6,a3                                          #  SHA256SUM1           .          10169813 >>    000100000001 rs1 001 rd 0010011
## sha512sig0h a5,a3,a2     #RV32 only                    #  SHA512SIG0H          R          5cc687b3 >>    0101110 rs2 rs1 000 rd 0110011
## sha512sig0l a3,a6,a1     #RV32 only                    #  SHA512SIG0L          R          54b806b3 >>    0101010 rs2 rs1 000 rd 0110011
## sha512sig1h a7,a2,a6     #RV32 only                    #  SHA512SIG1H          R          5f0608b3 >>    0101111 rs2 rs1 000 rd 0110011
## sha512sig1l a1,a5,a3     #RV32 only                    #  SHA512SIG1L          R          56d785b3 >>    0101011 rs2 rs1 000 rd 0110011
## sha512sum0r a6,a1,a4     #RV32 only                    #  SHA512SUM0R          R          50e58833 >>    0101000 rs2 rs1 000 rd 0110011
## sha512sum1r a3,a0,a5     #RV32 only                    #  SHA512SUM1R          R          52f506b3 >>    0101001 rs2 rs1 000 rd 0110011
sha512sig0 a1,a3                                          #  SHA512SIG0           .          10669593 >>    000100000110 rs1 001 rd 0010011
sha512sig1 a6,a0                                          #  SHA512SIG1           .          10751813 >>    000100000111 rs1 001 rd 0010011
sha512sum0 a3,a7                                          #  SHA512SUM0           .          10489693 >>    000100000100 rs1 001 rd 0010011
sha512sum1 a1,a4                                          #  SHA512SUM1           .          10571593 >>    000100000101 rs1 001 rd 0010011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zksed instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
sm4ed a4,a1,a7,2                                          #  SM4ED                .          b1158733 >>    bs 11000 rs2 rs1 000 rd 0110011
sm4ks a3,a0,a5,2                                          #  SM4KS                .          b4f506b3 >>    bs 11010 rs2 rs1 000 rd 0110011


#════════════════════════════════════════════════════════════════════════════════════════════════════
# Zksh instructions
#════════════════════════════════════════════════════════════════════════════════════════════════════
sm3p0 a7,a5                                               #  SM3P0                .          10879893 >>    000100001000  rs1 001 rd 0010011
sm3p1 a0,a4                                               #  SM3P1                .          10971513 >>    000100001001  rs1 001 rd 0010011
