#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Reko.Arch.PowerPC
{
    public class PowerPcDisassembler : DisassemblerBase<PowerPcInstruction>
    {
        private PowerPcArchitecture arch;
        private EndianImageReader rdr;
        private PrimitiveType defaultWordWidth;
        private PowerPcInstruction instrCur;
        private Address addr;

        public PowerPcDisassembler(PowerPcArchitecture arch, EndianImageReader rdr, PrimitiveType defaultWordWidth)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.defaultWordWidth = defaultWordWidth;
        }

        public override PowerPcInstruction DisassembleInstruction()
        {
            if (!rdr.IsValid)
                return null;
            this.addr = rdr.Address;
            uint wInstr = rdr.ReadUInt32();
            try
            {
                instrCur = oprecs[wInstr >> 26].Decode(this, wInstr);
            }
            catch
            {
                instrCur = new PowerPcInstruction(Opcode.illegal);
            }
            instrCur.Address = addr;
            instrCur.Length = 4;
            return instrCur;
        }

        private PowerPcInstruction DecodeOperands(Opcode opcode, uint wInstr, string opFmt)
        {
            var ops = new List<MachineOperand>();
            bool setsCR0 = (wInstr & 1) != 0;
            bool allowSetCR0 = false;
            MachineOperand op = null;
            for (int i = 0; i < opFmt.Length; ++i)
            {
                switch (opFmt[i])
                {
                default: throw new NotImplementedException(string.Format("Operator format {0}", opFmt[i]));
                case ',':
                    continue;
                case '.':   // If the instructions LSB is '1', then set the setsCR0
                    allowSetCR0 = setsCR0;
                    continue;
                case ':':    // Force the setsCR0 flag to '1'.
                    allowSetCR0 = setsCR0 = true;
                    continue;
                case 'E':
                    switch (opFmt[++i])
                    {
                    case '1': op = MemOff(wInstr >> 21, wInstr); break;
                    case '2': op = MemOff(wInstr >> 16, wInstr); break;
                    case '3': op = MemOff(wInstr >> 11, wInstr); break;
                    case '4': op = MemOff(wInstr >> 6, wInstr); break;
                    default: throw new NotImplementedException(string.Format("Register field {0}.", opFmt[i]));
                    }
                    break;
                case 'c':
                    switch (opFmt[++i])
                    {
                    case '1': op = this.CRegFromBits(wInstr >> 21); break;
                    case '2': op = this.CRegFromBits(wInstr >> 16); break;
                    case '3': op = this.CRegFromBits(wInstr >> 11); break;
                    case '4': op = this.CRegFromBits(wInstr >> 6); break;
                    default: throw new NotImplementedException(string.Format("c field {0}.", opFmt[i]));
                    }
                    break;
                case 'C':   // CR field in certain opcodes.
                    switch (opFmt[++i])
                    {
                    case '1': op = this.CRegFromBits((wInstr >> 23) & 0x07); break;
                    case '2': op = this.CRegFromBits((wInstr >> 18) & 0x07); break;
                    default: throw new NotImplementedException(string.Format("C field {0}.", opFmt[i]));
                        throw new FormatException("Invalid CRx format specification.");
                    }
                    break;
                case 'f':
                    switch (opFmt[++i])
                    {
                    case '1': op = this.FRegFromBits(wInstr >> 21); break;
                    case '2': op = this.FRegFromBits(wInstr >> 16); break;
                    case '3': op = this.FRegFromBits(wInstr >> 11); break;
                    case '4': op = this.FRegFromBits(wInstr >> 6); break;
                    default: throw new NotImplementedException(string.Format("Register field {0}.", opFmt[i]));
                    }
                    break;
                case 'r':
                    switch (opFmt[++i])
                    {
                    case '1': op = this.RegFromBits(wInstr >> 21); break;
                    case '2': op = this.RegFromBits(wInstr >> 16); break;
                    case '3': op = this.RegFromBits(wInstr >> 11); break;
                    case '4': op = this.RegFromBits(wInstr >> 6); break;
                    default: throw new NotImplementedException(string.Format("Register field {0}.", opFmt[i]));
                    }
                    break;
                case 'v':
                    switch (opFmt[++i])
                    {
                    case '1': op = this.VRegFromBits((wInstr >> 21) & 0x1F); break;
                    case '2': op = this.VRegFromBits((wInstr >> 16) & 0x1F); break;
                    case '3': op = this.VRegFromBits((wInstr >> 11) & 0x1F); break;
                    case '4': op = this.VRegFromBits((wInstr >> 6) & 0x1F); break;
                    default: throw new NotImplementedException(string.Format("Register field {0}.", opFmt[i]));
                    }
                    break;
                case 'I':
                    switch (opFmt[++i])
                    {
                    case '1': op = ImmediateOperand.Byte((byte)((wInstr >> 21) & 0x1F)); break;
                    case '2': op = ImmediateOperand.Byte((byte)((wInstr >> 16) & 0x1F)); break;
                    case '3': op = ImmediateOperand.Byte((byte)((wInstr >> 11) & 0x1F)); break;
                    case '4': op = ImmediateOperand.Byte((byte)((wInstr >> 6) & 0x1F)); break;
                    case '5': op = ImmediateOperand.Byte((byte)((wInstr >> 1) & 0x1F)); break;
                    default: throw new NotImplementedException(string.Format("Bitfield {0}.", opFmt[i]));
                    }
                    break;
                case 'M':   // Condition register fields.
                    op = ImmediateOperand.Byte((byte)((wInstr >> 12) & 0xFF)); break;
                case 'S':
                    op = new ImmediateOperand(Constant.Int16((short) wInstr));
                    break;
                case 'U':
                    op = new ImmediateOperand(Constant.Word16((ushort)wInstr));
                    break;
                case 'u':
                    op = GetImmediateUnsignedField(opFmt, ref i, wInstr);
                    break;
                case 's':
                    op = GetImmediateSignedField(opFmt, ref i, wInstr);
                    break;
                case 'W':   // VMX extension to access 128 vector regs
                    switch (opFmt[++i])
                    {
                    case 'd': op = VRegFromBits(((wInstr >> 21) & 0x1Fu) | ((wInstr & 0xCu) << 3)); break;
//| A | 0 0 0 0 | a | 1 | VDh | VBh |
                    case 'a': op = VRegFromBits(((wInstr >> 16) & 0x1Fu) | ((wInstr >> 4) & 0x40) | (wInstr & 0x20)); break;
                    case 'b': op = VRegFromBits(((wInstr >> 11) & 0x1Fu) | ((wInstr & 0x3) << 5)); break;
                        throw new NotImplementedException();
                    }
                    break;
                case 'X': // Special format used by the CMP[L][I] instructions.
                    op = CRegFromBits((wInstr >> 23) & 0x7);
                    break;
                }
                ops.Add(op);
            }
            return new PowerPcInstruction(opcode)
            {
                Address = addr,
                Length = 4,
                op1 = ops.Count > 0 ? ops[0] : null,
                op2 = ops.Count > 1 ? ops[1] : null,
                op3 = ops.Count > 2 ? ops[2] : null,
                op4 = ops.Count > 3 ? ops[3] : null,
                op5 = ops.Count > 4 ? ops[4] : null,
                setsCR0 = setsCR0 & allowSetCR0,
            };
        }

        private MachineOperand MemOff(uint reg, uint wInstr)
        {
            var d = Constant.Int32((short)wInstr);
            return new MemoryOperand(PrimitiveType.Word32, arch.Registers[(int) reg & 0x1F], d);
        }

        private RegisterOperand CRegFromBits(uint r)
        {
            return new RegisterOperand(arch.CrRegisters[(int) r & 0x1F]);
        }

        private RegisterOperand RegFromBits(uint r)
        {
            return new RegisterOperand(arch.Registers[(int)r & 0x1F]);
        }

        private RegisterOperand FRegFromBits(uint r)
        {
            return new RegisterOperand(arch.FpRegisters[(int) r & 0x1F]);
        }

        private RegisterOperand VRegFromBits(uint r)
        {
            return new RegisterOperand(arch.VecRegisters[(int)r]);
        }

        private ImmediateOperand GetImmediateUnsignedField(string fmt, ref int i, uint wInstr)
        {
            int offset = 0;
            while (Char.IsDigit(fmt[++i]))
            {
                offset = offset * 10 + (fmt[i] - '0');
            }
            ++i;
            int size =0;
            while (i < fmt.Length && Char.IsDigit(fmt[i]))
            {
                size = size * 10 + (fmt[i] - '0');
                if (i >= fmt.Length)
                    break;
                ++i;
            }
            uint mask = (1u << size) - 1u;
            return new ImmediateOperand(Constant.Byte((byte)((wInstr >> offset) & mask)));
        }

        private ImmediateOperand GetImmediateSignedField(string fmt, ref int i, uint wInstr)
        {
            int offset = 0;
            while (Char.IsDigit(fmt[++i]))
            {
                offset = offset * 10 + (fmt[i] - '0');
            }
            ++i;
            int size = 0;
            while (i < fmt.Length && Char.IsDigit(fmt[i]))
            {
                size = size * 10 + (fmt[i] - '0');
                if (i >= fmt.Length)
                    break;
                ++i;
            }
            uint mask = (1u << (1 + size)) - 1u;
            uint x = (wInstr >> offset) & mask;

            uint m = 1u << (size - 1);
            sbyte r = (sbyte) ((x ^ m) - m);
            return new ImmediateOperand(Constant.SByte(r));
        }

        private PowerPcInstruction EmitUnknown(uint instr)
        {
#if DEBUG
//            Debug.WriteLine(
//$@"        [Test]
//        public void PPCDis_{instr:X8}()
//        {{
//            AssertCode(0x{instr:X8}, ""@@@"");
//        }}
//");
#endif
            return new PowerPcInstruction(Opcode.illegal);
        }


        private abstract class OpRec
        {
            public abstract PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr);
        }

        private class InvalidOpRec  : OpRec
        {
            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                Debug.Print("Unknown PowerPC instruction {0:X8}", wInstr);
                return dasm.EmitUnknown(wInstr);
            }
        }

        private class DOpRec : OpRec
        {
            public readonly Opcode opcode;
            public readonly string opFmt;

            public DOpRec(Opcode opcode, string opFmt)
            {
                this.opcode = opcode;
                this.opFmt = opFmt;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                return dasm.DecodeOperands(opcode, wInstr, opFmt);
            }
        }

        private class DSOpRec : OpRec
        {
            public readonly Opcode opcode0;
            public readonly Opcode opcode1;
            public readonly string opFmt;

            public DSOpRec(Opcode opcode0, Opcode opcode1, string opFmt)
            {
                this.opcode0 = opcode0;
                this.opcode1 = opcode1;
                this.opFmt = opFmt;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                Opcode opcode = ((wInstr & 1) == 0) ? opcode0 : opcode1;
                wInstr &= ~3u;
                return dasm.DecodeOperands(opcode, wInstr, opFmt);
            }
        }

        private class MDOpRec : OpRec
        {
            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                // Only supported on 64-bit arch.
                if (dasm.defaultWordWidth.BitSize == 32)
                {
                    return new PowerPcInstruction(Opcode.illegal);
                }
                else
                {
                    Opcode opcode;
                    switch ((wInstr >> 1) & 0xF)
                    {
                    case 0: case 1: opcode = Opcode.rldicl; break;
                    case 2: case 3: opcode = Opcode.rldicr; break;
                    case 4: case 5: opcode = Opcode.rldic; break;
                    case 6: case 7: opcode = Opcode.rldimi; break;
                    case 8: opcode = Opcode.rldcl; break;
                    case 9: opcode = Opcode.rldcr; break;
                    default: return new PowerPcInstruction(Opcode.illegal);
                    }
                    
                    wInstr &= ~1u;
                    return new PowerPcInstruction(opcode)
                    {
                        op1 = dasm.RegFromBits(wInstr >> 16),
                        op2 = dasm.RegFromBits(wInstr >> 21),
                        op3 = ImmediateOperand.Byte((byte)((wInstr >> 11) & 0x1F | (wInstr << 4) & 0x20)),
                        op4 = ImmediateOperand.Byte((byte)((wInstr >> 6) & 0x1F | (wInstr & 0x20))),
                    };
                }
            }
        }

        private class AOpRec : OpRec
        {
            private Dictionary<uint, DOpRec> xOpRecs;

            public AOpRec(Dictionary<uint, DOpRec> xOpRecs)
            {
                this.xOpRecs = xOpRecs;
            }
        
            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                return xOpRecs[(wInstr >> 1) & 0x3FF].Decode(dasm, wInstr);
            }
        }

        private class XOpRec : OpRec
        {
            private Dictionary<uint, OpRec> xOpRecs;

            public XOpRec(Dictionary<uint, OpRec> xOpRecs)
            {
                this.xOpRecs = xOpRecs;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var xOp = (wInstr >> 1) & 0x3FF;
                if (xOpRecs.TryGetValue(xOp, out var opRec))
                {
                    return opRec.Decode(dasm, wInstr);
                }
                else
                {
                    Debug.Print("Unknown PowerPC X instruction {0:X8} {1:X2}-{2:X3} ({2})", wInstr, wInstr >> 26, xOp);
                    return dasm.EmitUnknown(wInstr);
                }
            }
        }

        private class FpuOpRec : OpRec
        {
            private Dictionary<uint, OpRec> fpuOpRecs;
            private int shift;
            private uint mask;

            public FpuOpRec(int shift, uint mask, Dictionary<uint, OpRec> fpuOpRecs)
            {
                this.shift = shift;
                this.mask = mask;
                this.fpuOpRecs = fpuOpRecs;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var x = (wInstr >> shift) & mask;
                if (fpuOpRecs.TryGetValue(x, out OpRec opRec))
                {
                    return opRec.Decode(dasm, wInstr);
                }
                else
                    return new PowerPcInstruction(Opcode.illegal);
            }
        }

        private class XlOpRecAux : DOpRec
        {
            private Opcode opLink;

            public XlOpRecAux(Opcode opcode,Opcode opLink, string opFmt)
                : base(opcode, opFmt)
            {
                this.opLink = opLink;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                return dasm.DecodeOperands((wInstr&1)!=0 ? opLink : opcode, wInstr, opFmt);
            }
        }

        private class FpuOpRecAux : OpRec
        {
            private Opcode opcode;
            private string opFmt;

            public FpuOpRecAux(Opcode opcode, string opFmt)
            {
                this.opcode = opcode;
                this.opFmt = opFmt;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                return dasm.DecodeOperands(opcode, wInstr, opFmt);
            }
        }

        private class IOpRec : OpRec
        {
            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var opcode = (wInstr & 1)== 1 ? Opcode.bl : Opcode.b;
                var uOffset = wInstr & 0x03FFFFFC;
                if ((uOffset & 0x02000000) != 0)
                    uOffset |= 0xFF000000;
                var baseAddr = (wInstr & 2) != 0 ? Address.Create(dasm.defaultWordWidth, 0) : dasm.rdr.Address - 4;
                return new PowerPcInstruction(opcode)
                {
                    op1 = new AddressOperand(baseAddr + uOffset),
                };
            }
        }

        private class BOpRec : OpRec
        {
            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                bool link = (wInstr & 1) != 0;
                var uOffset = wInstr & 0x0000FFFC;
                if ((uOffset & 0x8000) != 0)
                    uOffset |= 0xFFFF0000;
                var grfBi = (wInstr >> 16) & 0x1F;
                var grfBo = (wInstr >> 21) & 0x1F;
                var crf = grfBi >> 2;

                Opcode opcode;
                var baseAddr = (wInstr & 2) != 0 ? Address.Create(dasm.defaultWordWidth, 0) : dasm.rdr.Address - 4;
                var dst = new AddressOperand(baseAddr + uOffset);
                if ((grfBo & 0x10) != 0)
                {
                    // Unconditionals.
                    if ((grfBo & 0x04) != 0)
                    {
                        return new PowerPcInstruction(link ? Opcode.bl : Opcode.b)
                        {
                            op1 = dst
                        };
                    }
                    else
                    {
                        return new PowerPcInstruction(
                            ((grfBo & 2) != 0)
                                ? (link ? Opcode.bdzl : Opcode.bdz)
                                : (link ? Opcode.bdnzl : Opcode.bdnz))
                        {
                            op1 = dst,
                        };
                    }
                }
                else
                {
                    opcode = Opcode.illegal;
                    // Decrement also
                    switch (grfBo)
                    {
                    case 0:
                    case 1:
                        opcode = (link ? Opcode.bdnzfl : Opcode.bdnzf);
                        break;
                    case 2:
                    case 3:
                        opcode = (link ? Opcode.bdzfl : Opcode.bdzf);
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        switch (grfBi & 3)
                        {
                        default:
                            throw new NotImplementedException();
                            //return new PowerPcInstruction(link ? Opcode.bcl : Opcode.bc)
                            //{
                            //    op1 = new ImmediateOperand(Constant.Byte((byte)((wInstr >> 21) & 0x1F))),
                            //    op2 = new ImmediateOperand(Constant.Byte((byte)((wInstr >> 16) & 0x1F))),
                            //    op3 = dst
                            //};
                        case 0: opcode = link ? Opcode.bgel : Opcode.bge; break;
                        case 1: opcode = link ? Opcode.blel : Opcode.ble; break;
                        case 2: opcode = link ? Opcode.bnel : Opcode.bne; break;
                        case 3: opcode = link ? Opcode.bnsl : Opcode.bns; break;
                        }
                        return new PowerPcInstruction(opcode) { 
                            op1 = (grfBi > 3) ? new RegisterOperand(dasm.arch.CrRegisters[(int)grfBi >> 2]) : (MachineOperand) dst,
                            op2 = (grfBi > 3)? dst : (MachineOperand) null,
                        };
                    case 8:
                    case 9:
                        opcode = (link ? Opcode.bdnztl : Opcode.bdnzt);
                        break;
                    case 0xA:
                    case 0xB:
                        opcode = (link ? Opcode.bdztl : Opcode.bdzt);
                        break;
                    case 0xC:
                    case 0xD:
                    case 0xE:
                    case 0xF:
                        switch (grfBi & 0x3)
                        {
                        default: throw new NotImplementedException();
                            //return new PowerPcInstruction(link ? Opcode.bcl : Opcode.bc)
                            //{
                            //    op1 = new ImmediateOperand(Constant.Byte((byte)((wInstr >> 21) & 0x1F))),
                            //    op2 = new ImmediateOperand(Constant.Byte((byte)((wInstr >> 16) & 0x1F))),
                            //    op3 = dst
                            //};
                        case 0: opcode = link ? Opcode.bltl : Opcode.blt; break;
                        case 1: opcode = link ? Opcode.bgtl : Opcode.bgt; break;
                        case 2: opcode = link ? Opcode.beql : Opcode.beq; break;
                        case 3: opcode = link ? Opcode.bsol : Opcode.bso; break;
                        }
                        return new PowerPcInstruction(opcode)
                        {
                            op1 = (grfBi > 3) ? new RegisterOperand(dasm.arch.CrRegisters[(int)grfBi >> 2]) : (MachineOperand)dst,
                            op2 = (grfBi > 3) ? dst : (MachineOperand)null,
                        };
                    }
                    return new PowerPcInstruction(opcode)
                    {
                        op1 = new ConditionOperand(grfBi),
                        op2 = dst
                    };
                }
            }
        }

        private class BclrOpRec : OpRec
        {
            public BclrOpRec()
            {
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                bool link = (wInstr & 1) != 0;
                var opcode = link ? Opcode.blrl : Opcode.blr;
                var crBit = (wInstr >> 16) & 0x1F;
                var crf = crBit >> 2;
                var condCode = ((wInstr >> 22) & 4) | (crBit & 0x3);
                var bo = (wInstr >> 21) & 0x1F;
                if ((bo & 0x14) == 0x14)
                    return new PowerPcInstruction(Opcode.blr);

                switch (condCode)
                {
                default:
                    return new PowerPcInstruction(link ? Opcode.bclrl : Opcode.bclr)
                    {
                        op1 = new ImmediateOperand(Constant.Byte((byte)((wInstr >> 21) & 0x1F))),
                        op2 = new ImmediateOperand(Constant.Byte((byte)((wInstr >> 16) & 0x1F))),
                    };
                case 0: opcode = link ? Opcode.bgelrl : Opcode.bgelr; break;
                case 1: opcode = link ? Opcode.blelrl : Opcode.blelr; break;
                case 2: opcode = link ? Opcode.bnelrl : Opcode.bnelr; break;
                case 3: opcode = link ? Opcode.bnslrl : Opcode.bnslr; break;
                case 4: opcode = link ? Opcode.bltlrl : Opcode.bltlr; break;
                case 5: opcode = link ? Opcode.bgtlrl : Opcode.bgtlr; break;
                case 6: opcode = link ? Opcode.beqlrl : Opcode.beqlr; break;
                case 7: opcode = link ? Opcode.bsolrl : Opcode.bsolr; break;
                }
                return new PowerPcInstruction(opcode)
                {
                    op1 = dasm.CRegFromBits(crf),
                };
            }
        }

        private class XfxOpRec : DOpRec
        {
            public XfxOpRec(Opcode opcode, string fmt) : base(opcode, fmt)
            {

            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var reg = dasm.RegFromBits(wInstr >> 21);
                var spr = (wInstr >> 11) & 0x3FF;
                return new PowerPcInstruction(opcode)
                {
                    op1 = reg,
                    op2 = new ImmediateOperand(Constant.Word16((ushort)spr))
                };
            }
        }

        private class SprOpRec : OpRec
        {
            private bool to;

            public SprOpRec(bool to)
            {
                this.to = to;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                MachineOperand op1 = dasm.RegFromBits(wInstr >> 21);
                MachineOperand op2 = null;
                var spr = (wInstr >> 11) & 0x3FF;
                Opcode opcode ;
                switch (spr)
                {
                case 0x0100: opcode = to ? Opcode.mtlr : Opcode.mflr; break;
                case 0x0120: opcode = to ? Opcode.mtctr : Opcode.mfctr; break;
                default:
                    opcode = to ? Opcode.mtspr : Opcode.mfspr;
                    op2 = op1;
                    op1 = ImmediateOperand.UInt32(spr);
                    break;
                }
                return new PowerPcInstruction(opcode)
                {
                    op1 = op1,
                    op2 = op2
                };
            }
        }

        private class CmpOpRec : DOpRec
        {
            public CmpOpRec(Opcode op, string format)  :base(op, format)
            {}

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var l = ((wInstr >> 21) & 1) != 0;
                var op = Opcode.illegal;
                switch (this.opcode)
                {
                default: throw new NotImplementedException();
                case Opcode.cmp: op = l ? Opcode.cmpl : Opcode.cmp; break;
                case Opcode.cmpi: op = l ? Opcode.cmpi : Opcode.cmpwi; break;
                case Opcode.cmpl: op = l ? Opcode.cmpl : Opcode.cmplw; break;
                case Opcode.cmpli: op = l ? Opcode.cmpli : Opcode.cmplwi; break;
                }
                return dasm.DecodeOperands(op, wInstr, opFmt);
            }
        }

        private class VXOpRec : OpRec
        {
            private Dictionary<uint, OpRec> vxOpRecs;
            private Dictionary<uint, OpRec> vaOpRecs;

            public VXOpRec(Dictionary<uint, OpRec> vxOpRecs, Dictionary<uint, OpRec> vaOpRecs)
            {
                this.vxOpRecs = vxOpRecs;
                this.vaOpRecs = vaOpRecs;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var xOp = wInstr & 0x7FFu;
                if (vxOpRecs.TryGetValue(xOp, out OpRec opRec))
                {
                    return opRec.Decode(dasm, wInstr);
                }
                else if (vaOpRecs.TryGetValue(wInstr & 0x3Fu, out opRec))
                {
                    return opRec.Decode(dasm, wInstr);
                }
                else
                {
                    Debug.Print("Unknown PowerPC VX instruction {0:X8} {1:X2}-{2:X3} ({2})", wInstr, wInstr >> 26, xOp);
                    return dasm.EmitUnknown(wInstr);
                }
            }
        }

        private class XSOpRec : DOpRec
        {
            public XSOpRec(Opcode opcode, string format) : base(opcode, format) { }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var instr = base.Decode(dasm, wInstr);
                var c = ((ImmediateOperand)instr.op3).Value.ToInt32();
                if ((wInstr & 2) != 0)
                    c += 32;
                instr.op3 = new ImmediateOperand(Constant.Byte((byte)c));
                return instr;

            }
        }

        private class XX3OpRec : OpRec
        {
            private Dictionary<uint, OpRec> decoders;

            public XX3OpRec(Dictionary<uint, OpRec> xoprecs)
            {
                this.decoders = xoprecs;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var subOp = (wInstr & 0xFFFF) >> 3;
                if (decoders.TryGetValue(subOp, out var decoder))
                {
                    return decoder.Decode(dasm, wInstr);
                }
                else
                {
                    Debug.Print("Unknown PowerPC XX3 instruction {0:X8} {1:X2}-{2:X3} ({2})", wInstr, wInstr >> 26, subOp);
                    return dasm.EmitUnknown(wInstr);
                }
            }
        }

        private class VMXDecoder : OpRec
        {
            private uint mask;
            public Dictionary<uint, OpRec> decoders;

            public VMXDecoder(uint mask, Dictionary<uint, OpRec> decoders)
            {
                this.mask = mask;
                this.decoders = decoders;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var key = (wInstr >> 0x4) & mask;
                if (decoders.TryGetValue(key, out OpRec decoder))
                {
                    return decoder.Decode(dasm, wInstr);
                }
                else
                {
                    Debug.Print("Unknown PowerPC VMX instruction {0:X8} {1:X2}-{2:X3} ({2})", wInstr, wInstr >> 26, key);
                    return dasm.EmitUnknown(wInstr);
                }
            }
        }


        static OpRec[] oprecs;

        static PowerPcDisassembler()
        {
            oprecs = new OpRec[] {
                // 00
                new InvalidOpRec(),
                new InvalidOpRec(),
                new InvalidOpRec(),
                new DOpRec(Opcode.twi, "I1,r2,S"),
                new VXOpRec(
                    new Dictionary<uint, OpRec>     // 4
                    {
/*

Conventions:

   VD128, VS128:  5 lower bits of a VMX128 vector register 
                  number
   VDh:	          upper 2 bits of VD128
                  (so register number is (VDh << 5 | VD128))
   VA128:         same as VD128
   A:             bit 6 of VA128
   a:             bit 5 of VA128
                  (so register number is (A<<6 | a<<5 | VA128))
   VB128:         same as VD128
   VBh:           same as VDh
   VC128:         3 bits of a VMX128 vector register number
                  (you can only use vr0-vr7 here)
   RA, RB:        general purpose register number
   UIMM:          unsigned immediate value
   SIMM:          signed immediate value
   PERMh:         upper 3 bits of a permutation
   PERMl:         lower 5 bits of a permutation
   x, y, z:       unknown immediate values
=================================================================
   lvewx128                  Load Vector128 Element Word Indexed
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 1 0 0 0|VDh|1 1|
   lvewx128      vr(VD128), r(RA), r(RB)
=================================================================
   lvlx128                           Load Vector128 Left Indexed
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 0 0 0 0 0 0|VDh|1 1|
   lvlx128       vr(VD128), r(RA), r(RB)*/
                { 0x403, new DOpRec(Opcode.lvlx128, "Wd,r2,r3") },
                { 0x407, new DOpRec(Opcode.lvlx128, "Wd,r2,r3") },
                { 0x40B, new DOpRec(Opcode.lvlx128, "Wd,r2,r3") },
                { 0x40F, new DOpRec(Opcode.lvlx128, "Wd,r2,r3") },
   /*
=================================================================
   lvrx128                          Load Vector128 Right Indexed
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 0 0 0 1 0 0|VDh|1 1|
   lvrx128       vr(VD128), r(RA), r(RB) */
                { 0x443, new DOpRec(Opcode.lvrx128, "Wd,r2,r3") },
                { 0x447, new DOpRec(Opcode.lvrx128, "Wd,r2,r3") },
                { 0x44B, new DOpRec(Opcode.lvrx128, "Wd,r2,r3") },
                { 0x44F, new DOpRec(Opcode.lvrx128, "Wd,r2,r3") },
   /*
=================================================================
   lvlxl128                      Load Vector128 Left Indexed LRU
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 1 0 0 0 0 0|VDh|1 1|
   lvlxl128      vr(VD128), r(RA), r(RB)
=================================================================
   lvrxl128                     Load Vector128 Right Indexed LRU
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |1 1 0 0 1 0 0|VDh|1 1|
   lvrxl128      vr(VD128), r(RA), r(RB)
=================================================================
   lvsl128                         Load Vector128 for Shift Left
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 0 0 0 0|VDh|1 1|
   lvsl128       vr(VD128), r(RA), r(RB)
=================================================================
   lvsr128                        Load Vector128 for Shift Right
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 0 1 0 0|VDh|1 1|
   lvsr128       vr(VD128), r(RA), r(RB)
=================================================================
   lvx128                                 Load Vector128 Indexed
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 0 0 1 1 0 0|VDh|1 1|
   lvx128        vr(VD128), r(RA), r(RB)
=================================================================*/
                { 0x0C3, new DOpRec(Opcode.lvx128, "Wd,r2,r3") },
                { 0x0C7, new DOpRec(Opcode.lvx128, "Wd,r2,r3") },
                { 0x0CB, new DOpRec(Opcode.lvx128, "Wd,r2,r3") },
                { 0x0CF, new DOpRec(Opcode.lvx128, "Wd,r2,r3") },
/*
   lvxl128                            Load Vector128 Indexed LRU
|0 0 0 1 0 0|  VD128  |   RA    |   RB    |0 1 0 1 1 0 0|VDh|1 1|
   lvxl128       vr(VD128), r(RA), r(RB)
=================================================================
   stewx128                 Store Vector128 Element Word Indexed
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |0 1 1 0 0 0 0|VDh|1 1|
   stvewx128     vr(VS128), r(RA), r(RB)
=================================================================
   stvlx128                         Store Vector128 Left Indexed
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 0 1 0 0 0 0|VDh|1 1|
   stvlx128      vr(VS128), r(RA), r(RB)
*/
                { 0x503, new DOpRec(Opcode.stvlx128, "Wd,r2,r3") },
                { 0x507, new DOpRec(Opcode.stvlx128, "Wd,r2,r3") },
                { 0x50B, new DOpRec(Opcode.stvlx128, "Wd,r2,r3") },
                { 0x50F, new DOpRec(Opcode.stvlx128, "Wd,r2,r3") },

/*=================================================================
   stvlxl128                    Store Vector128 Left Indexed LRU
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 1 1 0 0 0 0|VDh|1 1|
   lvlxl128      vr(VS128), r(RA), r(RB)
=================================================================
   stvrx128                        Store Vector128 Right Indexed
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 0 1 0 1 0 0|VDh|1 1|
   stvrx128       vr(VS128), r(RA), r(RB)
*/
                { 0x543, new DOpRec(Opcode.stvrx128, "Wd,r2,r3") },
                { 0x547, new DOpRec(Opcode.stvrx128, "Wd,r2,r3") },
                { 0x54B, new DOpRec(Opcode.stvrx128, "Wd,r2,r3") },
                { 0x54F, new DOpRec(Opcode.stvrx128, "Wd,r2,r3") },

/*=================================================================
   stvrxl128                   Store Vector128 Right Indexed LRU
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |1 1 1 0 1 0 0|VDh|1 1|
   stvrxl128     vr(VS128), r(RA), r(RB)
=================================================================
   stvx128                               Store Vector128 Indexed
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |0 0 1 1 1 0 0|VDh|1 1|
   stvx128       vr(VS128), r(RA), r(RB)*/
                { 0x1C3, new DOpRec(Opcode.stvx128, "Wd,r2,r3") },
                { 0x1C7, new DOpRec(Opcode.stvx128, "Wd,r2,r3") },
                { 0x1CB, new DOpRec(Opcode.stvx128, "Wd,r2,r3") },
                { 0x1CF, new DOpRec(Opcode.stvx128, "Wd,r2,r3") },

/*=================================================================
   stvxl128                          Store Vector128 Indexed LRU
|0 0 0 1 0 0|  VS128  |   RA    |   RB    |0 1 1 1 1 0 0|VDh|1 1|
   stvxl128      vr(VS128), r(RA), r(RB)
=================================================================
   vsldoi128                         Vector128 Shift Left Double 
                                              by Octet Immediate
|0 0 0 1 0 0|  VD128  |  VA128  |  VB128  |A|  SHB  |a|1|VDh|VBh|
   vsldoi128     vr(VD128), vr(VA128), vr(VB128), SHB

=================================================================
   vaddfp128                        Vector128 Add Floating Point
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 0|a|1|VDh|VBh|
   vaddfp128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vand128                                 Vector128 Logical AND
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 0 0|a|1|VDh|VBh|
   vand128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vandc128                                Vector128 Logical AND 
                                                 with Complement
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|1|VDh|VBh|
   vandc128      vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vmaddcfp128                            Vector128 Multiply Add 
                                                  Floating Point
|0 0 0 1 0 1|  VDS128 |  VA128  |  VB128  |A|0 1 0 0|a|1|VDh|VBh|
   vmaddcfp128   vr(VDS128), vr(VA128), vr(VSD128), vr(VB128)
=================================================================
   vmaddfp128                             Vector128 Multiply Add 
                                                  Floating Point
|0 0 0 1 0 1|  VDS128 |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|
   vmaddfp128    vr(VDS128), vr(VA128), vr(VB128), vr(VDS128)
=================================================================
   vmsum3fp128                      Vector128 Multiply Sum 3-way 
                                                  Floating Point
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 0|a|1|VDh|VBh|
   vmsub3fp128   vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vmsum4fp128                      Vector128 Multiply Sum 4-way 
                                                  Floating-Point
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|
   vmsub4fp128   vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vmulfp128                                  Vector128 Multiply
                                                  Floating-Point
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 1 0|a|1|VDh|VBh|
   vmulfp128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vnmsubfp128              Vector128 Negative Multiply-Subtract 
                                                  Floating Point
|0 0 0 1 0 1|  VDS128 |  VA128  |  VB128  |A|0 1 0 1|a|1|VDh|VBh|
   vnmsubfp128   vr(VDS128), vr(VA128), vr(VB128), vr(VDS128)
=================================================================
   vnor128                                 Vector128 Logical NOR
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|1|VDh|VBh|
   vnor128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vor128                                   Vector128 Logical OR
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|1|VDh|VBh|
   vor128        vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vperm128                                Vector128 Permutation
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0| VC  |a|0|VDh|VBh|
   vperm128      vr(VD128), vr(VA128), vr(VB128), vr(VC)
=================================================================
   vpkshss128                    Vector128 Pack Signed Half Word 
                                                 Signed Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 0 0|a|0|VDh|VBh|
   vpkshss128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkshus128                    Vector128 Pack Signed Half Word 
                                               Unsigned Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 0 1|a|0|VDh|VBh|
   vpkshus128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkswss128                         Vector128 Pack Signed Word
                                                 Signed Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|
   vpkswss128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkswus128                         Vector128 Pack Signed Word   
                                               Unsigned Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|
   vpkswus128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkuhum128                  Vector128 Pack Unsigned Half Word 
                                                 Unsigned Modulo
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|
   vpkuhum128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkuhus128                  Vector128 Pack Unsigned Half Word 
                                               Unsigned Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|
   vpkuhus128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkuwum128                       Vector128 Pack Unsigned Word
                                                 Unsigned Modulo
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 1 0|a|0|VDh|VBh|
   vpkuwum128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpkuwus128                       Vector128 Pack Unsigned Word
                                               Unsigned Saturate
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 1 1|a|0|VDh|VBh|
   vpkuwus128    vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vsel128                                      Vector128 Select
|0 0 0 1 0 1| VDS128  |  VA128  |  VB128  |A|1 1 0 1|a|1|VDh|VBh|
   vsel128       vr(VDS128), vr(VA128), vr(VB128), vr(VDS128)
=================================================================
   vslo128                            Vector128 Shift Left Octet
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 1 0|a|1|VDh|VBh|
   vslo128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vsubfp128                   Vector128 Subtract Floating Point
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 1|a|1|VDh|VBh|
   vsubfp128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vxor128                                 Vector128 Logical XOR
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|1|VDh|VBh|
   vxor128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vrlw128                            Vector128 Rotate Left Word
|0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 1|a|1|VDh|VBh|
   vrlw128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vcfpsxws128          Vector128 Convert From Floating-Point to 
                                Signed Fixed-Point Word Saturate
|0 0 0 1 1 0|  VD128  |  SIMM   |  VB128  |0 1 0 0 0 1 1|VDh|VBh|
   vcfpsxws128   vr(VD128), vr(VB128), SIMM
=================================================================
   vcfpuxws128          Vector128 Convert From Floating-Point to 
                              Unsigned Fixed-Point Word Saturate
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 0 1 1 1|VDh|VBh|
   vcfpuxws128   vr(VD128), vr(VB128), UIMM
=================================================================
   vcmpbfp128                           Vector128 Compare Bounds 
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1|R|a|0|VDh|VBh|
   vcmpbfp128    vr(VD128), vr(VA128), vr(VB128)         (R == 0)
   vcmpbfp128.   vr(VD128), vr(VA128), vr(VB128)         (R == 1)
=================================================================
   vcmpeqfp128                        Vector128 Compare Equal-to
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 0|R|a|0|VDh|VBh|
   vcmpeqfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
   vcmpeqfp128.  vr(VD128), vr(VA128), vr(VB128)         (R == 1)
=================================================================
   vcmpequw128                        Vector128 Compare Equal-to 
                                                   Unsigned Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 0|R|a|0|VDh|VBh|
   vcmpequw128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
   vcmpequw128.  vr(VD128), vr(VA128), vr(VB128)         (R == 1)
=================================================================
   vcmpgefp128                    Vector128 Compare Greater-Than-
                                      or-Equal-to Floating Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1|R|a|0|VDh|VBh|
   vcmpgefp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
   vcmpgefp128.  vr(VD128), vr(VA128), vr(VB128)         (R == 1)
=================================================================
   vcmpgtfp128                    Vector128 Compare Greater-Than 
                                                  Floating-Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0|R|a|0|VDh|VBh|
   vcmpgtfp128   vr(VD128), vr(VA128), vr(VB128)         (R == 0)
   vcmpgtfp128.  vr(VD128), vr(VA128), vr(VB128)         (R == 1)
=================================================================
   vcsxwfp128          Vector128 Convert From Signed Fixed-Point 
                                          Word to Floating-Point
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 1 0 1 1|VDh|VBh|
   vcsxwfp128    vr(VD128), vr(VB128), SIMM
=================================================================
   vcuxwfp128        Vector128 Convert From Unsigned Fixed-Point
                                          Word to Floating-Point
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |0 1 0 1 1 1 1|VDh|VBh|
   vcuxwfp128    vr(VD128), vr(VB128), UIMM
=================================================================
   vexptefp128                Vector128 2 Raised to the Exponent 
                                         Estimate Floating Point
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 0 1 1|VDh|VBh|
   vexptefp128   vr(VD128), vr(VB128)
=================================================================
   vlogefp128                            Vector128 Log2 Estimate 
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 1 1 1 1|VDh|VBh|
   vlogefp128    vr(VD128), vr(VB128)
=================================================================
   vmaxfp128                                   Vector128 Maximum 
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 0|a|0|VDh|VBh|
   vmaxfp128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vminfp128                                   Vector128 Minimum
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|0|VDh|VBh|
   vminfp128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vmrghw128                           Vector128 Merge High Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|
   vmrghw128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vmrglw128                            Vector128 Merge Low Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|
   vmrglw128     vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vpermwi128                 Vector128 Permutate Word Immediate
|0 0 0 1 1 0|  VD128  |  PERMl  |  VB128  |0|1|PERMh|0|1|VDh|VBh|
   vpermwi128    vr(VD128), vr(VB128), (PERMh << 5 | PERMl)
=================================================================
   vpkd3d128                 Vector128 Pack D3Dtype, Rotate Left 
                                       Immediate and Mask Insert
|0 0 0 1 1 0|  VD128  |  x  | y |  VB128  |1 1 0| z |0 1|VDh|VBh|
   vpkd3d128     vr(VD128), vr(VB128), x, y, z
=================================================================
   vrefp128                        Vector128 Reciprocal Estimate 
                                                  Floating Point
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 0 1 1|VDh|VBh|
   vrefp128      vr(VD128), vr(VB128)
=================================================================
   vrfim128                    Vector128 Round to Floating-Point 
                                              Integer toward -oo
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 0 0 1 1|VDh|VBh|
   vrfim128      vr(VD128), vr(VB128)
=================================================================
   vrfin128                    Vector128 Round to Floating-Point 
                                          Integer toward Nearest
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 0 1 1 1|VDh|VBh|
   vrfin128      vr(VD128), vr(VB128)
=================================================================
   vrfip128                    Vector128 Round to Floating-Point 
                                              Integer toward +oo
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 0 1 1|VDh|VBh|
   vrfip128      vr(VD128), vr(VB128)
=================================================================
   vrfiz128                    Vector128 Round to Floating-Point 
                                             Integer toward Zero
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 1 1 1|VDh|VBh|
   vrfiz128      vr(VD128), vr(VB128)
=================================================================
   vrlimi128                     Vector128 Rotate Left Immediate 
                                                 and Mask Insert
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|
   vrlimi128     vr(VD128), vr(VB128), UIMM, z
=================================================================
   vrsqrtefp128                 Vector128 Reciprocal Square Root 
                                         Estimate Floating Point
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |1 1 0 0 1 1 1|VDh|VBh|
   vrsqrtefp128  vr(VD128), vr(VB128)
=================================================================
   vslw128                             Vector128 Shift Left Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|
   vslw128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vspltisw128                         Vector128 Splat Immediate 
                                                     Signed Word
|0 0 0 1 1 0|  VD128  |  SIMM   |  VB128  |1 1 1 0 1 1 1|VDh|VBh|
   vspltisw128   vr(VD128), vr(VB128), SIMM
=================================================================
   vspltw128                                Vector128 Splat Word
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 0 0 1 1|VDh|VBh|
   vspltw128     vr(VD128), vr(VB128), UIMM
=================================================================
   vsraw128                                Vector128 Shift Right 
                                                 Arithmetic Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 0 1|a|1|VDh|VBh|
   vsraw128      vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vsro128                           Vector128 Shift Right Octet
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 1 1|a|1|VDh|VBh|
   vsro128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vsrw128                            Vector128 Shift Right Word
|0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|
   vsrw128       vr(VD128), vr(VA128), vr(VB128)
=================================================================
   vupkd3d128                           Vector128 Unpack D3Dtype
|0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 1 1 1 1|VDh|VBh|
   vupkd3d128    vr(VD128), vr(VB128), UIMM
=================================================================
   vupkhsb128                                   Vector128 Unpack 
                                                High Signed Byte
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 0 0 0|VDh|VBh|
   vupkhsb128    vr(VD128), vr(VB128)
=================================================================
   vupklsb128                                   Vector128 Unpack 
                                                 Low Signed Byte
|0 0 0 1 1 0|  VD128  |0 0 0 0 0|  VB128  |0 1 1 1 1 0 0|VDh|VBh|
   vupkhsb128    vr(VD128), vr(VB128)
*/
                        { 0x000, new DOpRec(Opcode.vaddubm, "v1,v2,v3") },
                        { 0x002, new DOpRec(Opcode.vmaxub, "v1,v2,v3") },
                        { 0x00A, new DOpRec(Opcode.vaddfp, "v1,v2,v3") },
                       // { 0x010, new DOpRec(Opcode.mulhhwu, "r1,r2,r3") },
                        { 0x020, new DOpRec(Opcode.vmhaddshs, "v1,v2,v3,v4") },
                        { 0x022, new DOpRec(Opcode.vmladduhm, "v1,v2,v3,v4") },
                        { 0x042, new DOpRec(Opcode.vmaxuh, "v1,v2,v3") },
                        { 0x04A, new DOpRec(Opcode.vsubfp, "v1,v2,v3") },
                        { 0x080, new DOpRec(Opcode.vadduwm, "v1,v2,v3") },
                        { 0x086, new DOpRec(Opcode.vcmpequw, "v1,v2,v3") },
                        { 0x08C, new DOpRec(Opcode.vmrghw, "v1,v2,v3") },
                        { 0x0C6, new DOpRec(Opcode.vcmpeqfp, "v1,v2,v3") },
                        { 0x0E2, new DOpRec(Opcode.vmladduhm, "v1,v2,v3,v4") },
                        { 0x100, new DOpRec(Opcode.vadduqm, "v1,v2,v3") },
                        { 0x10A, new DOpRec(Opcode.vrefp, "v1,v3") },
                        { 0x14A, new DOpRec(Opcode.vrsqrtefp, "v1,v3") },
                        { 0x184, new DOpRec(Opcode.vslw, "v1,v2,v3") },
                        { 0x18C, new DOpRec(Opcode.vmrglw, "v1,v2,v3") },
                        { 0x200, new DOpRec(Opcode.vaddubs, "v1,v2,v3") },
                        { 0x28C, new DOpRec(Opcode.vspltw, "v1,v3,u16:2") },
                        { 0x2C6, new DOpRec(Opcode.vcmpgtfp, "v1,v2,v3") },
                        { 0x34A, new DOpRec(Opcode.vcfsx, "v1,v3,u16:5") },
                        { 0x38C, new DOpRec(Opcode.vspltisw, "v1,s16:5") },
                        { 0x3CA, new DOpRec(Opcode.vctsxs, "v1,v3,u16:5") },
                        { 0x401, new DOpRec(Opcode.bcdadd, ":v1,v2,v3,u9:1") },
                        { 0x404, new DOpRec(Opcode.vand, "v1,v2,v3") },
                        { 0x406, new DOpRec(Opcode.vcmpequb, ":v1,v2,v3") },
                        { 0x444, new DOpRec(Opcode.vandc, "v1,v2,v3") },
                        { 0x484, new DOpRec(Opcode.vor, "v1,v2,v3") },
                        { 0x4C4, new DOpRec(Opcode.vxor, "v1,v2,v3")},
                        { 0x4C6, new DOpRec(Opcode.vcmpeqfp, ":v1,v2,v3") },
                        { 0x4C7, new DOpRec(Opcode.vcmpequd, ":v1,v2,v3") },
                        // These aren't present for the VMX extension of PowerPC architecture
                        //{ 0x503, new DOpRec(Opcode.evmhessfaaw, "r1,r2,r3") },
                        //{ 0x50B, new DOpRec(Opcode.evmhesmfaaw, "r1,r2,r3") },
                        { 0x686, new DOpRec(Opcode.vcmpgtuw, ":v1,v2,v3") },
                        { 0x6C6, new DOpRec(Opcode.vcmpgtfp, ":v1,v2,v3") },
                    },
                    new Dictionary<uint, OpRec>
                    {
                        { 0x02A, new DOpRec(Opcode.vsel, "v1,v2,v3,v4") },
                        { 0x02B, new DOpRec(Opcode.vperm, "v1,v2,v3,v4") },
                        { 0x02C, new DOpRec(Opcode.vsldoi, "v1,v2,v3,u6:5") },
                        { 0x02E, new DOpRec(Opcode.vmaddfp, "v1,v2,v4,v3") },
                        { 0x02F, new DOpRec(Opcode.vnmsubfp, "v1,v2,v4,v3") }
                    }),
                new VMXDecoder(0x3D, new Dictionary<uint, OpRec>
                {
                    { 0x01, new DOpRec(Opcode.vaddfp128, "Wd,Wa,Wb") },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 0|a|1|VDh|VBh|    vaddfp128     vr(VD128), vr(VA128), vr(VB128)
                    { 0x03, new DOpRec(Opcode.vaddfp128, "Wd,Wa,Wb") },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 0 0|a|1|VDh|VBh|    vaddfp128     vr(VD128), vr(VA128), vr(VB128)

                    { 0x09, new DOpRec(Opcode.vmulfp128, "Wd,Wa,Wb") },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 1 0|a|1|VDh|VBh|    vmulfp128     vr(VD128), vr(VA128), vr(VB128)
                    { 0x0B, new DOpRec(Opcode.vmulfp128, "Wd,Wa,Wb") },         // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 0 1 0|a|1|VDh|VBh|    vmulfp128     vr(VD128), vr(VA128), vr(VB128)

                    { 0x1D, new DOpRec(Opcode.vmsub4fp128, "Wd,Wa,Wb") },       // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vmsub4fp128   vr(VD128), vr(VA128), vr(VB128)
                    { 0x1F, new DOpRec(Opcode.vmsub4fp128, "Wd,Wa,Wb") },       // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|0 1 1 1|a|1|VDh|VBh|    vmsub4fp128   vr(VD128), vr(VA128), vr(VB128)

                    { 0x2D, new DOpRec(Opcode.vor128, "Wd,Wa,Wb") },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|1|VDh|VBh|    vor128        vr(VD128), vr(VA128), vr(VB128)
                    { 0x2F, new DOpRec(Opcode.vor128, "Wd,Wa,Wb") },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 0 1 1|a|1|VDh|VBh|    vor128        vr(VD128), vr(VA128), vr(VB128)

                    { 0x31, new DOpRec(Opcode.vxor128, "Wd,Wa,Wb") },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|1|VDh|VBh|    vxor128       vr(VD128), vr(VA128), vr(VB128)
                    { 0x33, new DOpRec(Opcode.vxor128, "Wd,Wa,Wb") },            // |0 0 0 1 0 1|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|1|VDh|VBh|    vxor128       vr(VD128), vr(VA128), vr(VB128)

                }),
                new VMXDecoder(0x7F, new Dictionary<uint, OpRec>
                {
                    { 0x0D, new DOpRec(Opcode.vslw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                    { 0x0F, new DOpRec(Opcode.vslw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                    { 0x4D, new DOpRec(Opcode.vslw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)
                    { 0x4F, new DOpRec(Opcode.vslw128, "Wd,Wa,Wb") },           // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|0 0 1 1|a|1|VDh|VBh|    vslw128       vr(VD128), vr(VA128), vr(VB128)

                    { 0x30, new DOpRec(Opcode.vmrghw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                    { 0x32, new DOpRec(Opcode.vmrghw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                    { 0x70, new DOpRec(Opcode.vmrghw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)
                    { 0x72, new DOpRec(Opcode.vmrghw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 0|a|0|VDh|VBh|    vmrghw128     vr(VD128), vr(VA128), vr(VB128)

                    { 0x34, new DOpRec(Opcode.vmrglw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                    { 0x36, new DOpRec(Opcode.vmrglw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                    { 0x74, new DOpRec(Opcode.vmrglw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)
                    { 0x76, new DOpRec(Opcode.vmrglw128, "Wd,Wa,Wb") },         // |0 0 0 1 1 0|  VD128  |  VA128  |  VB128  |A|1 1 0 1|a|0|VDh|VBh|    vmrglw128     vr(VD128), vr(VA128), vr(VB128)

                    { 0x77, new DOpRec(Opcode.vspltisw128, "Wd,Wb,s16:5") },

                    { 0x71, new DOpRec(Opcode.vrlimi128, "Wd,Wb,u16:5,u14:2") },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                    { 0x75, new DOpRec(Opcode.vrlimi128, "Wd,Wb,u16:5,u14:2") },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                    { 0x79, new DOpRec(Opcode.vrlimi128, "Wd,Wb,u16:5,u14:2") },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z
                    { 0x7D, new DOpRec(Opcode.vrlimi128, "Wd,Wb,u16:5,u14:2") },    // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1| z |0 1|VDh|VBh|    vrlimi128     vr(VD128), vr(VB128), UIMM, z

                    { 0x73, new DOpRec(Opcode.vspltw128, "Wd,Wb,u16:5") },      // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 0 0 1 1|VDh|VBh|    vspltw128     vr(VD128), vr(VB128), UIMM

                    { 0x7F, new DOpRec(Opcode.vupkd3d128, "Wd,Wb,u16:5") },     // |0 0 0 1 1 0|  VD128  |  UIMM   |  VB128  |1 1 1 1 1 1 1|VDh|VBh|    vupkd3d128    vr(VD128), vr(VB128), UIMM
                }),
                new DOpRec(Opcode.mulli, "r1,r2,S"),

                new DOpRec(Opcode.subfic, "r1,r2,S"),
                new InvalidOpRec(),
                new CmpOpRec(Opcode.cmpli, "C1,r2,U"),
                new CmpOpRec(Opcode.cmpi, "C1,r2,S"),
                new DOpRec(Opcode.addic, "r1,r2,S"),
                new DOpRec(Opcode.addic, ":r1,r2,S"),
                new DOpRec(Opcode.addi, "r1,r2,S"),
                new DOpRec(Opcode.addis, "r1,r2,S"),
                // 10
                new BOpRec(),
                new DOpRec(Opcode.sc, ""),
                new IOpRec(),
                new XOpRec(new Dictionary<uint, OpRec>()
                {
                    { 0, new DOpRec(Opcode.mcrf, "C1,C2")},
                    { 16, new BclrOpRec() },
                    { 33, new DOpRec(Opcode.crnor, "I1,I2,I3") },
                    { 50, new DOpRec(Opcode.rfi, "") },
                    { 0x96, new DOpRec(Opcode.isync, "") },
                    { 449, new DOpRec(Opcode.cror, "I1,I2,I3") },
                    { 0x0C1, new DOpRec(Opcode.crxor, "I1,I2,I3") },
                    { 0x121, new DOpRec(Opcode.creqv, "I1,I2,I3") },
                    { 0x210, new XlOpRecAux(Opcode.bcctr, Opcode.bctrl, "I1,I2")}
                }),
                new DOpRec(Opcode.rlwimi, "r2,r1,I3,I4,I5"),
                new DOpRec(Opcode.rlwinm, "r2,r1,I3,I4,I5"),
                new InvalidOpRec(),
                new DOpRec(Opcode.rlwnm, "r2,r1,r3,I4,I5"),

                new DOpRec(Opcode.ori, "r2,r1,U"),
                new DOpRec(Opcode.oris, "r2,r1,U"),
                new DOpRec(Opcode.xori, "r2,r1,U"),
                new DOpRec(Opcode.xoris, "r2,r1,U"),
                new DOpRec(Opcode.andi, ":r2,r1,U"),
                new DOpRec(Opcode.andis, ":r2,r1,U"),
                new MDOpRec(),
                new XOpRec(new Dictionary<uint, OpRec>      // 1F
                {
                    { 0, new CmpOpRec(Opcode.cmp, "C1,r2,r3") },
                    { 4, new DOpRec(Opcode.tw, "I1,r2,r3") },
                    { 0x006, new DOpRec(Opcode.lvsl, "v1,r2,r3") },
                    { 0x008, new DOpRec(Opcode.subfc, "r1,r2,r3")},
                    { 0x00A, new DOpRec(Opcode.addc, "r1,r2,r3")},
                    { 0x00B, new DOpRec(Opcode.mulhwu, ".r1,r2,r3")},
                    { 0x013, new DOpRec(Opcode.mfcr, "r1") },
                    { 0x015, new DOpRec(Opcode.ldx, "r1,r2,r3") },
                    { 0x017, new DOpRec(Opcode.lwzx, "r1,r2,r3") },
                    { 0x018, new DOpRec(Opcode.slw, ".r2,r1,r3") },
                    { 0x01A, new DOpRec(Opcode.cntlzw, "r2,r1") },
                    { 0x01B, new DOpRec(Opcode.sld, ".r2,r1,r3") },
                    { 0x01C, new DOpRec(Opcode.and, ".r2,r1,r3")},
                    { 0x020, new CmpOpRec(Opcode.cmpl, "C1,r2,r3") },
                    { 0x014, new DOpRec(Opcode.lwarx, "r1,r2,r3") },
                    { 0x028, new DOpRec(Opcode.subf, ".r1,r2,r3")},
                    { 0x035, new DOpRec(Opcode.ldux, "r1,r2,r3")},
                    { 0x036, new DOpRec(Opcode.dcbst, "r2,r3")},
                    { 0x037, new DOpRec(Opcode.lwzux, "r1,r2,r3")},
                    { 0x03A, new DOpRec(Opcode.cntlzd, "r2,r1") },
                    { 0x03C, new DOpRec(Opcode.andc, ".r2,r1,r3") },
                    { 0x047, new DOpRec(Opcode.lvewx, "v1,r2,r3") },
                    { 0x04B, new DOpRec(Opcode.mulhw, ".r1,r2,r3") },
                    { 0x053, new DOpRec(Opcode.mfmsr, "r1") },
                    { 0x054, new DOpRec(Opcode.ldarx, "r1,r2,r3") },
                    { 0x056, new DOpRec(Opcode.dcbf, "r2,r3") },
                    { 0x057, new DOpRec(Opcode.lbzx, "r1,r2,r3") },
                    { 0x067, new DOpRec(Opcode.lvx, "v1,r2,r3") },
                    { 0x068, new DOpRec(Opcode.neg, "r1,r2") },
                    { 0x077, new DOpRec(Opcode.lbzux, "r1,r2,r3") },
                    { 124, new DOpRec(Opcode.nor, ".r2,r1,r3") },
                    { 0x088, new DOpRec(Opcode.subfe, "r1,r2,r3") },
                    { 0x08A, new DOpRec(Opcode.adde, ".r1,r2,r3") },
                    { 0x090, new DOpRec(Opcode.mtcrf, "M,r1") },
                    { 0x092, new DOpRec(Opcode.mtmsr, "r1,u16:1") },
                    { 0x095, new DOpRec(Opcode.stdx, "r1,r2,r3") },
                    { 0x096, new DOpRec(Opcode.stwcx, ".r1,r2,r3") },
                    { 0x097, new DOpRec(Opcode.stwx, "r1,r2,r3") },
                    { 0x0C7, new DOpRec(Opcode.stvewx, "v1,r2,r3")},
                    { 0x0B2, new DOpRec(Opcode.mtmsrd, "r1,u16:1") },
                    { 0x0B7, new DOpRec(Opcode.stwux, "r1,r2,r3") },
                    { 0x0E7, new DOpRec(Opcode.stvx, "v1,r2,r3") },
                    { 215, new DOpRec(Opcode.stbx, "r1,r2,r3") },
                    { 235, new DOpRec(Opcode.mullw, ".r1,r2,r3") },
                    { 0x0C8, new DOpRec(Opcode.subfze, ".r1,r2") },
                    { 0x0CA, new DOpRec(Opcode.addze, ".r1,r2") },
                    { 0x0D6, new DOpRec(Opcode.stdcx, ":r1,r2,r3") },
                    { 0x0E9, new DOpRec(Opcode.mulld, ".r1,r2,r3")},
                    { 0x0EA, new DOpRec(Opcode.addme, ".r1,r2")},
                    { 0x0F6, new DOpRec(Opcode.dcbtst, "r2,r3") },
                    { 247, new DOpRec(Opcode.stbux, "r1,r2,r3") },
                    { 0x10A, new DOpRec(Opcode.add, ".r1,r2,r3") },
                    { 279, new DOpRec(Opcode.lhzx, "r1,r2,r3") },
                    { 0x116, new DOpRec(Opcode.dcbt, "r2,r3,u21:4") },
                    { 0x11C, new DOpRec(Opcode.eqv, "r1,r2,r3") },
                    { 316, new DOpRec(Opcode.xor, ".r2,r1,r3") },
                    { 444, new DOpRec(Opcode.or, ".r2,r1,r3") },

                    { 0x153, new SprOpRec(false) },
                    { 0x155, new DOpRec(Opcode.lwax, "r1,r2,r3") },
                    { 0x157, new DOpRec(Opcode.lhax, "r1,r2,r3") },
                    { 0x173, new XfxOpRec(Opcode.mftb, "r1,X3") },
                    { 0x177, new DOpRec(Opcode.lhaux, "r1,r2,r3") },
                    { 0x197, new DOpRec(Opcode.sthx, "r1,r2,r3") },
                    { 0x19C, new DOpRec(Opcode.orc, ".r2,r1,r3") },
                    { 0x1C9, new DOpRec(Opcode.divdu, ".r1,r2,r3") },
                    { 0x1CB, new DOpRec(Opcode.divwu, ".r1,r2,r3") },
                    { 0x1DC, new DOpRec(Opcode.nand, ".r2,r1,r3") },
                    { 0x1D3, new SprOpRec(true) },
                    { 0x1D6, new DOpRec(Opcode.dcbi, "r2,r3")},
                    { 0x1E9, new DOpRec(Opcode.divd, ".r1,r2,r3")},
                    { 0x1EB, new DOpRec(Opcode.divw, ".r1,r2,r3")},
                    { 0x207, new DOpRec(Opcode.lvlx, "r1,r2,r3") },
                    { 0x216, new DOpRec(Opcode.lwbrx, "r1,r2,r3") },
                    { 0x217, new DOpRec(Opcode.lfsx, "f1,r2,r3") },
                    { 0x218, new DOpRec(Opcode.srw, ".r2,r1,r3") },
                    { 0x21B, new DOpRec(Opcode.srd, ".r2,r1,r3") },
                    { 0x255, new DOpRec(Opcode.lswi, "r1,r2,I3") },
                    { 0x237, new DOpRec(Opcode.lfsux, "f1,r2,r3") },
                    { 0x257, new DOpRec(Opcode.lfdx, "f1,r2,r3") },
                    { 0x277, new DOpRec(Opcode.lfdux, "f1,r2,r3") },
                    { 0x256, new DOpRec(Opcode.sync, "") },
                    { 0x296, new DOpRec(Opcode.stwbrx, ".r2,r1,r3") },
                    { 0x297, new DOpRec(Opcode.stfsx, "f1,r2,r3") },
                    { 0x2B7, new DOpRec(Opcode.stfsux, "f1,r2,r3") },
                    { 0x2D5, new DOpRec(Opcode.stswi, "r1,r2,I3") },
                    { 0x2D7, new DOpRec(Opcode.stfdx, "f1,r2,r3") },
                    { 0x316, new DOpRec(Opcode.lhbrx, "r1,r2,r3")},
                    { 0x318, new DOpRec(Opcode.sraw, ".r2,r1,r2")},
                    { 0x31A, new DOpRec(Opcode.srad, ".r2,r1,r2")},
                    { 0x33A, new DOpRec(Opcode.sradi, ".r2,r1,I3") },
                    { 0x338, new DOpRec(Opcode.srawi, "r2,r1,I3") },
                    { 0x33B, new XSOpRec(Opcode.sradi, ".r2,r1,I3") },
                    { 0x356, new DOpRec(Opcode.eieio, "") },
                    { 0x39A, new DOpRec(Opcode.extsh, ".r2,r1")},
                    { 0x3BA, new DOpRec(Opcode.extsb, ".r2,r1")},
                    { 0x3D7, new DOpRec(Opcode.stfiwx, "f1,r2,r3")},
                    { 0x3D6, new DOpRec(Opcode.icbi, "r2,r3")},
                    { 0x3DA, new DOpRec(Opcode.extsw, ".r2,r1")},
                    { 0x3F6, new DOpRec(Opcode.dcbz, "r2,r3") }
                }),
                // 20
                new DOpRec(Opcode.lwz, "r1,E2"),
                new DOpRec(Opcode.lwzu, "r1,E2"),
                new DOpRec(Opcode.lbz, "r1,E2"),
                new DOpRec(Opcode.lbzu, "r1,E2"),
                new DOpRec(Opcode.stw, "r1,E2"),
                new DOpRec(Opcode.stwu, "r1,E2"),
                new DOpRec(Opcode.stb, "r1,E2"),
                new DOpRec(Opcode.stbu, "r1,E2"),

                new DOpRec(Opcode.lhz, "r1,E2"),
                new DOpRec(Opcode.lhzu, "r1,E2"),
                new DOpRec(Opcode.lha, "r1,E2"),
                new DOpRec(Opcode.lhau, "r1,E2"),
                new DOpRec(Opcode.sth, "r1,E2"),
                new DOpRec(Opcode.sthu, "r1,E2"),
                new DOpRec(Opcode.lmw, "r1,E2"),
                new DOpRec(Opcode.stmw, "r1,E2"),
                // 30
                new DOpRec(Opcode.lfs, "f1,E2"),
                new DOpRec(Opcode.lfsu, "f1,E2"),
                new DOpRec(Opcode.lfd, "f1,E2"),
                new DOpRec(Opcode.lfdu, "f1,E2"),
                new DOpRec(Opcode.stfs, "f1,E2"),
                new DOpRec(Opcode.stfsu, "f1,E2"),
                new DOpRec(Opcode.stfd, "f1,E2"),
                new DOpRec(Opcode.stfdu, "f1,E2"),

                new DOpRec(Opcode.lq, "r1,E2"),
                new InvalidOpRec(),
                new DSOpRec(Opcode.ld, Opcode.ldu, "r1,E2"),
                new FpuOpRec(1, 0x1F, new Dictionary<uint, OpRec>()
                {
                    { 18, new FpuOpRecAux(Opcode.fdivs, ".f1,f2,f3") },
                    { 20, new FpuOpRecAux(Opcode.fsubs, ".f1,f2,f3") },
                    { 21, new FpuOpRecAux(Opcode.fadds, ".f1,f2,f3") },
                    { 22, new FpuOpRecAux(Opcode.fsqrts, ".f1,f3") },
                    { 24, new FpuOpRecAux(Opcode.fres, ".f1,f3") },
                    { 25, new FpuOpRecAux(Opcode.fmuls, ".f1,f2,f4") },
                    { 28, new FpuOpRecAux(Opcode.fmsubs, ".f1,f2,f4,f3") },
                    { 29, new FpuOpRecAux(Opcode.fmadds, ".f1,f2,f4,f3") },
                    { 30, new FpuOpRecAux(Opcode.fnmsubs, ".f1,f2,f3,f4") },
                    { 31, new FpuOpRecAux(Opcode.fnmadds, ".f1,f2,f3,f4") },
                }),

                new XX3OpRec(new Dictionary<uint, OpRec>                // 3C
                {
                    { 0x00, new DOpRec(Opcode.xsaddsp, "v1,v2,v3") },
                    { 0x01, new DOpRec(Opcode.xsmaddasp, "v1,v2,v3") },
                    //{ 0x02, new DOpRec(Opcode.xxsldwi, "v1,v2,v3") },       //$TODO need extra work.
                    { 0x09, new DOpRec(Opcode.xsmaddmsp, "v1,v2,v3") },
                }),
                new InvalidOpRec(),
                new DSOpRec(Opcode.std, Opcode.stdu, "r1,E2"),
                new FpuOpRec(1, 0x1F, new Dictionary<uint, OpRec>()     // 3F
                {
                    { 0x00, new FpuOpRec(6, 0x1F, new Dictionary<uint, OpRec>
                        {
                            { 0, new FpuOpRecAux(Opcode.fcmpu, "C1,f2,f3") },
                            { 1, new FpuOpRecAux(Opcode.fcmpo, "C1,f2,f3") },
                            //{ 2, new FpuOpRecAux(Opcode.mcrfs)}
                        })
                    },
                    { 0x06, new FpuOpRec(6, 0x1F, new Dictionary<uint,OpRec>
                        {
                            //{ 1, new FpuOpRecAux(Opcode.mtfsb1 },
                            //{ 2, new FpuOpRecAux(Opcode.mtfsb0 },
                            //{ 4, new FpuOpRecAux(Opcode.mtfsfi }
                        })
                    },
                    { 0x07, new FpuOpRec(6, 0x1F, new Dictionary<uint,OpRec>
                        {
                            { 0x12, new FpuOpRecAux(Opcode.mffs, ".f1" )},
                            { 0x16, new FpuOpRecAux(Opcode.mtfsf, "u17:8,f3" )},
                        })
                    },
                    { 0x08, new FpuOpRec(6, 0x1F, new Dictionary<uint,OpRec>
                        {
                            { 1, new FpuOpRecAux(Opcode.fneg, ".f1,f3") },
                            { 2, new FpuOpRecAux(Opcode.fmr, ".f1,f3" )},
                            { 4, new FpuOpRecAux(Opcode.fnabs, ".f1,f3") },
                            { 8, new FpuOpRecAux(Opcode.fabs, ".f1,f3") },
                        })
                    },
                    { 0x0C, new FpuOpRec(6, 0x1F, new Dictionary<uint,OpRec>
                        {
                            { 0, new FpuOpRecAux(Opcode.frsp, ".f1,f3") },
                        })
                    },
                    { 0x0E, new FpuOpRec(6, 0x1F, new Dictionary<uint,OpRec>
                        {
                            { 0x00, new FpuOpRecAux(Opcode.fctiw, ".f1,f3") },
                            { 0x19, new FpuOpRecAux(Opcode.fctid, ".f1,f3") },
                            { 0x1A, new FpuOpRecAux(Opcode.fcfid,  ".f1,f3") },
                        })
                    },
                    { 0x0F, new FpuOpRec(6, 0x1F, new Dictionary<uint,OpRec>
                        {
                            { 0x00, new FpuOpRecAux(Opcode.fctiwz,  ".f1,f3") },
                            { 0x19, new FpuOpRecAux(Opcode.fctidz,   ".f1,f3") },
                        })
                    },

                    { 18, new FpuOpRecAux(Opcode.fdiv, ".f1,f2,f3") },
                    { 20, new FpuOpRecAux(Opcode.fsub, ".f1,f2,f3") },
                    { 21, new FpuOpRecAux(Opcode.fadd, ".f1,f2,f3") },
                    { 0x16, new FpuOpRecAux(Opcode.fsqrt, ".f1,f3") },
                    { 0x17, new FpuOpRecAux(Opcode.fsel, ".f1,f2,f4,f3") },
                    { 0x19, new FpuOpRecAux(Opcode.fmul, ".f1,f2,f4") },
                    { 0x1A, new FpuOpRecAux(Opcode.frsqrte, ".f1,f3") },

                    { 0x1C, new FpuOpRecAux(Opcode.fmsub, ".f1,f2,f4,f3") },

                    { 0x1D, new FpuOpRecAux(Opcode.fmadd, ".f1,f2,f4,f3") },
                    { 0x1E, new FpuOpRecAux(Opcode.fnmsub, ".f1,f2,f4,f3") },
                    { 0x1F, new FpuOpRecAux(Opcode.fnmadd, ".f1,f2,f4,f3") },
                })

            };
        }
    }
}
/*
new DOpRec(2,tdi),	// Trap Doubleword Immediate
new DOpRec(3,twi),	// Trap Word Immediate
new DOpRec(7,mulli),	// Multiply Low Immediate
new DOprec(8, SR, Opcode.subfic),  //  Subtract From Immediate Carrying
new DOpRec(10,61,cmpli),	// Compare Logical Immediate
new DOpRec(11,60,cmpi),	// Compare Immediate
new DOpRec(12,SR,Opcode.52),	// addic Add Immediate Carrying
new DOpRec(13,SR,Opcode.52),	// addic. Add Immediate Carrying and Record
new DOpRec(14,51,addi),	// Add Immediate
new DOpRec(15,51,addis),	// Add Immediate Shifted
new BOpRec(16,CT), //bc[l][a] // Branch Conditional
new ScOpRec(17, 26, Opcode.sc ), // System Call
new IOpRec(18, b), // b[l][a]              // Branch
new XlOpRec(19,0,Opcode.mcrf),	// Move Condition Register Field
new XlOpRec(19,16, CT, Opcode.bclr[l]   // Branch Conditional to Link Register
new XlOpRec(19,18,Opcode.rfid),	// rfidReturn from Interrupt Doubleword
new XlOpRec(19,33,Opcode.crnor),	// Condition Register NOR
new XlOpRec(19,129,Opcode.crandc),	// Condition Register AND with Complement
new XlOpRec(19,150,Opcode.isync),	// Instruction Synchronize
new XlOpRec(19,193,Opcode.crxor),	// Condition Register XOR
new XlOpRec(19,225,Opcode.crnand),	// Condition Register NAND
new XlOpRec(19,257,Opcode.crand),	// Condition Register AND
new XlOpRec(19,274,Opcode.hrfid),	// Hypervisor Return from Interrupt Doubleword
new XlOpRec(19,289,Opcode.creqv),	// Condition Register Equivalent
new XlOpRec(19,417,Opcode.crorc),	// Condition Register OR with Complement
new XlOpRec(19,449,Opcode.cror),	// Condition Register OR
new XlOpRec(19, 528, CT), bcctr[l] // Branch Conditional to Count Register
M 20 SR 76 rlwimi[.] Rotate Left Word Immediate then Mask Insert
M 21 SR 73 rlwinm[.] Rotate Left Word Immediate then AND with Mask
M 23 SR 75 rlwnm[.] Rotate Left Word then AND with Mask
new DOpRec(24,66,ori),	// OR Immediate
new DOpRec(25,66,oris),	// OR Immediate Shifted
new DOpRec(26,66,xori),	// XOR Immediate
new DOpRec(27,66,xoris),	// XOR Immediate Shifted
new DOpRec(28,SR,Opcode.65),	// andi. AND Immediate
new DOpRec(29,SR,Opcode.65),	// andis. AND Immediate Shifted
MD 30 0 SR 72 rldicl[.] Rotate Left Doubleword Immediate then Clear Left
MD 30 1 SR 72 rldicr[.] Rotate Left Doubleword Immediate then Clear Right
MD 30 2 SR 73 rldic[.] Rotate Left Doubleword Immediate then Clear
MD 30 3 SR 76 rldimi[.] Rotate Left Doubleword Immediate then Mask Insert
MDS 30 8 SR 74 rldcl[.] Rotate Left Doubleword then Clear Left

MDS 30 9 SR 75 rldcr[.] Rotate Left Doubleword then Clear Right
X 31 0 60 cmp Compare
X 31 4 64 tw Trap Word
XO 31 8 SR 53 subfc[o][.] Subtract From Carrying
XO 31 9 SR 57 mulhdu[.] Multiply High Doubleword Unsigned
XO 31 10 SR 53 addc[o][.] Add Carrying
XO 31 11 SR 57 mulhwu[.] Multiply High Word Unsigned
XFX 31 19 83 mfcr Move From Condition Register
XFX 31 19 124 mfocrf Move From One Condition Register Field
X 31 20 II lwarx Load Word And Reserve Indexed
X 31 21 39 ldx Load Doubleword Indexed
X 31 23 37 lwzx Load Word and Zero Indexed
X 31 24 SR 77 slw[.] Shift Left Word
X 31 26 SR 70 cntlzw[.] Count Leading Zeros Word
X 31 27 SR 77 sld[.] Shift Left Doubleword
X 31 28 SR 67 and[.] AND
X 31 32 61 cmpl Compare Logical
XO 31 40 SR 52 subf[o][.] Subtract From
X 31 53 39 ldux Load Doubleword with Update Indexed
X 31 54 II dcbst Data Cache Block Store
X 31 55 37 lwzux Load Word and Zero with Update Indexed
X 31 58 SR 70 cntlzd[.] Count Leading Zeros Doubleword
X 31 60 SR 68 andc[.] AND with Complement
X 31 68 64 td Trap Doubleword
XO 31 73 SR 57 mulhd[.] Multiply High Doubleword
XO 31 75 SR 57 mulhw[.] Multiply High Word
X 31 83 III mfmsr Move From Machine State Register
X 31 84 II ldarx Load Doubleword And Reserve Indexed
X 31 86 II dcbf Data Cache Block Flush
X 31 87 34 lbzx Load Byte and Zero Indexed
XO 31 104 SR 55 neg[o][.] Negate
X 31 119 34 lbzux Load Byte and Zero with Update Indexed
X 31 122 70 popcntb Population Count Bytes
X 31 124 SR 68 nor[.] NOR
XO 31 136 SR 54 subfe[o][.] Subtract From Extended
XO 31 138 SR 54 adde[o][.] Add Extended
XFX 31 144 83 mtcrf Move To Condition Register Fields
XFX 31 144 124 mtocrf Move To One Condition Register Field
X 31 146 III mtmsr Move To Machine State Register
X 31 149 43 stdx Store Doubleword Indexed
X 31 150 II stwcx. Store Word Conditional Indexed
X 31 151 42 stwx Store Word Indexed
X 31 178 III mtmsrd Move To Machine State Register Doubleword
X 31 181 43 stdux Store Doubleword with Update Indexed
X 31 183 42 stwux Store Word with Update Indexed
XO 31 200 SR 55 subfze[o][.] Subtract From Zero Extended
XO 31 202 SR 55 addze[o][.] Add to Zero Extended
X 31 210 32 III mtsr Move To Segment Register
X 31 214 II stdcx. Store Doubleword Conditional Indexed
X 31 215 40 stbx Store Byte Indexed
XO 31 232 SR 54 subfme[o][.] Subtract From Minus One Extended
XO 31 233 SR 56 mulld[o][.] Multiply Low Doubleword
XO 31 234 SR 54 addme[o][.] Add to Minus One Extended
XO 31 235 SR 56 mullw[o][.] Multiply Low Word
X 31 242 32 III mtsrin Move To Segment Register Indirect
X 31 246 II dcbtst Data Cache Block Touch for Store
X 31 247 40 stbux Store Byte with Update Indexed
XO 31 266 SR 52 add[o][.] Add
X 31 278 II dcbt Data Cache Block Touch
X 31 279 35 lhzx Load Halfword and Zero Indexed
X 31 284 SR 68 eqv[.] Equivalent
X 31 306 64 III tlbie TLB Invalidate Entry

X 31 310 II eciwx External Control In Word Indexed
X 31 311 35 lhzux Load Halfword and Zero with Update Indexed
X 31 316 SR 67 xor[.] XOR
XFX 31 339 82 mfspr Move From Special Purpose Register
X 31 341 38 lwax Load Word Algebraic Indexed
X 31 343 36 lhax Load Halfword Algebraic Indexed
X 31 370 III tlbia TLB Invalidate All
XFX 31 371 II mftb Move From Time Base
X 31 373 38 lwaux Load Word Algebraic with Update Indexed
X 31 375 36 lhaux Load Halfword Algebraic with Update Indexed
X 31 402 III slbmte SLB Move To Entry
X 31 407 41 sthx Store Halfword Indexed
X 31 412 SR 68 orc[.] OR with Complement
XS 31 413 SR 79 sradi[.] Shift Right Algebraic Doubleword Immediate
X 31 434 III slbie SLB Invalidate Entry
X 31 438 II ecowx External Control Out Word Indexed
X 31 439 41 sthux Store Halfword with Update Indexed
X 31 444 SR 67 or[.] OR
XO 31 457 SR 59 divdu[o][.] Divide Doubleword Unsigned
XO 31 459 SR 59 divwu[o][.] Divide Word Unsigned
XFX 31 467 81 mtspr Move To Special Purpose Register
X 31 476 SR 67 nand[.] NAND
XO 31 489 SR 58 divd[o][.] Divide Doubleword
XO 31 491 SR 58 divw[o][.] Divide Word
X 31 498 III slbia SLB Invalidate All
X 31 512 135 mcrxr Move to Condition Register from XER
X 31 533 48 lswx Load String Word Indexed
X 31 534 44 lwbrx Load Word Byte-Reverse Indexed
X 31 535 104 lfsx Load Floating-Point Single Indexed
X 31 536 SR 78 srw[.] Shift Right Word
X 31 539 SR 78 srd[.] Shift Right Doubleword
X 31 566 III tlbsync TLB Synchronize
X 31 567 104 lfsux Load Floating-Point Single with Update Indexed
X 31 595 32 III mfsr Move From Segment Register
X 31 597 48 lswi Load String Word Immediate
X 31 598 II sync Synchronize
X 31 599 105 lfdx Load Floating-Point Double Indexed
X 31 631 105 lfdux Load Floating-Point Double with Update Indexed
X 31 659 32 III mfsrin Move From Segment Register Indirect
X 31 661 49 stswx Store String Word Indexed
X 31 662 45 stwbrx Store Word Byte-Reverse Indexed
X 31 663 107 stfsx Store Floating-Point Single Indexed
X 31 695 107 stfsux Store Floating-Point Single with Update Indexed
X 31 725 49 stswi Store String Word Immediate
X 31 727 108 stfdx Store Floating-Point Double Indexed
X 31 759 108 stfdux Store Floating-Point Double with Update Indexed
X 31 790 44 lhbrx Load Halfword Byte-Reverse Indexed
X 31 792 SR 80 sraw[.] Shift Right Algebraic Word
X 31 794 SR 80 srad[.] Shift Right Algebraic Doubleword
X 31 824 SR 79 srawi[.] Shift Right Algebraic Word Immediate
X 31 851 III slbmfev SLB Move From Entry VSID
X 31 854 II eieio Enforce In-order Execution of I/O
X 31 915 III slbmfee SLB Move From Entry ESID
X 31 918 45 sthbrx Store Halfword Byte-Reverse Indexed
X 31 922 SR 69 extsh[.] Extend Sign Halfword
X 31 954 SR 69 extsb[.] Extend Sign Byte
X 31 982 II icbi Instruction Cache Block Invalidate
X 31 983 109 stfiwx Store Floating-Point as Integer Word Indexed
X 31 986 SR 69 extsw[.] Extend Sign Word
X 31 1014 II dcbz Data Cache Block set to Zero
new DOpRec(32,37,lwz),	// Load Word and Zero
new DOpRec(33,37,lwzu),	// Load Word and Zero with Update

new DOpRec(34,34,lbz),	// Load Byte and Zero
new DOpRec(35,34,lbzu),	// Load Byte and Zero with Update
new DOpRec(36,42,stw),	// Store Word
new DOpRec(37,42,stwu),	// Store Word with Update
new DOpRec(38,40,stb),	// Store Byte
new DOpRec(39,40,stbu),	// Store Byte with Update
new DOpRec(40,35,lhz),	// Load Halfword and Zero
new DOpRec(41,35,lhzu),	// Load Halfword and Zero with Update
new DOpRec(42,36,lha),	// Load Halfword Algebraic
new DOpRec(43,36,lhau),	// Load Halfword Algebraic with Update
new DOpRec(44,41,sth),	// Store Halfword
new DOpRec(45,41,Opcode.sthu),	// Store Halfword with Update
new DOpRec(46,46,Opcode.lmw),	// Load Multiple Word
new DOpRec(47,46,Opcode.stmw),	// Store Multiple Word
new DOpRec(48,104,Opcode.lfs),	// Load Floating-Point Single
new DOpRec(49,104,Opcode.lfsu),	// Load Floating-Point Single with Update
new DOpRec(50,105,Opcode.lfd),	// Load Floating-Point Double
new DOpRec(51,105,Opcode.lfdu),	// Load Floating-Point Double with Update
new DOpRec(52,107,Opcode.stfs),	// Store Floating-Point Single
new DOpRec(53,107,Opcode.stfsu),	// Store Floating-Point Single with Update
new DOpRec(54,108,Opcode.stfd),	// Store Floating-Point Double
new DOpRec(55,108,Opcode.stfdu),	// Store Floating-Point Double with Update
DS 58 0 39 ld Load Doubleword
DS 58 1 39 ldu Load Doubleword with Update
DS 58 2 38 lwa Load Word Algebraic
A 59 18 112 fdivs[.] Floating Divide Single
A 59 20 111 fsubs[.] Floating Subtract Single
A 59 21 111 fadds[.] Floating Add Single
A 59 22 125 fsqrts[.] Floating Square Root Single
A 59 24 125 fres[.] Floating Reciprocal Estimate Single
A 59 25 112 fmuls[.] Floating Multiply Single
A 59 26 126 frsqrtes[.] Floating Reciprocal Square Root Estimate Single
A 59 28 113 fmsubs[.] Floating Multiply-Subtract Single
A 59 29 113 fmadds[.] Floating Multiply-Add Single
A 59 30 114 fnmsubs[.] Floating Negative Multiply-Subtract Single
A 59 31 114 fnmadds[.] Floating Negative Multiply-Add Single
DS 62 0 43 std Store Doubleword
DS 62 1 43 stdu Store Doubleword with Update
X 63 0 119 fcmpu Floating Compare Unordered
X 63 12 115 frsp[.] Floating Round to Single-Precision
X 63 14 117 fctiw[.] Floating Convert To Integer Word
X 63 15 117 fctiwz[.] Floating Convert To Integer Word with round toward Zero
A 63 18 112 fdiv[.] Floating Divide
A 63 20 111 fsub[.] Floating Subtract
A 63 21 111 fadd[.] Floating Add
A 63 22 125 fsqrt[.] Floating Square Root
A 63 23 126 fsel[.] Floating Select
A 63 24 125 fre[.] Floating Reciprocal Estimate
A 63 25 112 fmul[.] Floating Multiply
A 63 26 126 frsqrte[.] Floating Reciprocal Square Root Estimate
A 63 28 113 fmsub[.] Floating Multiply-Subtract
A 63 29 113 fmadd[.] Floating Multiply-Add
A 63 30 114 fnmsub[.] Floating Negative Multiply-Subtract
A 63 31 114 fnmadd[.] Floating Negative Multiply-Add
X 63 32 119 fcmpo Floating Compare Ordered
X 63 38 122 mtfsb1[.] Move To FPSCR Bit 1
X 63 40 110 fneg[.] Floating Negate
X 63 64 120 mcrfs Move to Condition Register from FPSCR
X 63 70 122 mtfsb0[.] Move To FPSCR Bit 0
X 63 72 110 fmr[.] Floating Move Register
X 63 134 121 mtfsfi[.] Move To FPSCR Field Immediate
X 63 136 110 fnabs[.] Floating Negative Absolute Value
    }
}
*/
