#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PowerPC
{
    using Decoder = Decoder<PowerPcDisassembler, Mnemonic, PowerPcInstruction>;
    
    public partial class PowerPcDisassembler
    {
        public class InvalidDecoder : Decoder
        {
            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                return dasm.CreateInvalidInstruction();
            }
        }

        public class NyiDecoder : Decoder
        {
            private readonly string message;

            public NyiDecoder(string message)
            {
                this.message = message;
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                return dasm.NotYetImplemented(message);
            }

            public override string ToString()
            {
                return $"Nyi: {message}";
            }
        }

        public class FnDecoder : Decoder
        {
            private readonly Func<PowerPcDisassembler, uint, PowerPcInstruction> decoder;

            public FnDecoder(Func<PowerPcDisassembler, uint, PowerPcInstruction> decoder)
            {
                this.decoder = decoder;
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                return decoder(dasm, wInstr);
            }
        }

        public class InstrDecoder : Decoder
        {
            public readonly Mnemonic mnemonic;
            public readonly InstrClass iclass;
            public readonly Mutator<PowerPcDisassembler>[] mutators;

            public InstrDecoder(Mnemonic mnemonic, Mutator<PowerPcDisassembler> [] mutators, InstrClass iclass = InstrClass.Linear)
            {
                this.mnemonic = mnemonic;
                this.iclass = iclass;
                this.mutators = mutators;
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                return DecodeOperands(wInstr, dasm, iclass, mnemonic, mutators);
            }

            public static PowerPcInstruction DecodeOperands(
                uint wInstr,
                PowerPcDisassembler dasm,
                InstrClass iclass,
                Mnemonic mnemonic,
                Mutator<PowerPcDisassembler>[] mutators)
            {
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                    {
                        return dasm.CreateInvalidInstruction();
                    }
                }
                return dasm.MakeInstruction(iclass, mnemonic);
            }
        }

        public class DSDecoder : Decoder
        {
            public readonly Mnemonic mnemonic0;
            public readonly Mnemonic mnemonic1;
            public readonly InstrClass iclass;
            public readonly Mutator<PowerPcDisassembler> []mutators;

            public DSDecoder(Mnemonic mnemonic0, Mnemonic mnemonic1, params Mutator<PowerPcDisassembler> [] mutators)
            {
                this.mnemonic0 = mnemonic0;
                this.mnemonic1 = mnemonic1;
                this.iclass = InstrClass.Linear;
                this.mutators = mutators;
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                Mnemonic mnemonic = ((wInstr & 1) == 0) ? mnemonic0 : mnemonic1;
                wInstr &= ~3u;
                return InstrDecoder.DecodeOperands(wInstr & ~3u, dasm, iclass, mnemonic, mutators);
            }
        }

        public class MDDecoder : Decoder
        {
            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                // Only supported on 64-bit arch.
                if (dasm.defaultWordWidth.BitSize == 32)
                {
                    return dasm.CreateInvalidInstruction();
                }
                else
                {
                    Mnemonic mnemonic;
                    switch ((wInstr >> 1) & 0xF)
                    {
                    case 0: case 1: mnemonic = Mnemonic.rldicl; break;
                    case 2: case 3: mnemonic = Mnemonic.rldicr; break;
                    case 4: case 5: mnemonic = Mnemonic.rldic; break;
                    case 6: case 7: mnemonic = Mnemonic.rldimi; break;
                    case 8: mnemonic = Mnemonic.rldcl; break;
                    case 9: mnemonic = Mnemonic.rldcr; break;
                    default: return dasm.CreateInvalidInstruction();
                    }

                    wInstr &= ~1u;
                    return new PowerPcInstruction(mnemonic)
                    {
                        InstructionClass = InstrClass.Linear,
                        Operands = new MachineOperand[]
                        {
                            dasm.RegFromBits(wInstr >> 16),
                            dasm.RegFromBits(wInstr >> 21),
                            ImmediateOperand.Byte((byte)((wInstr >> 11) & 0x1F | (wInstr << 4) & 0x20)),
                            ImmediateOperand.Byte((byte)((wInstr >> 6) & 0x1F | (wInstr & 0x20))),
                        }
                    };
                }
            }
        }

        public class ADecoder : Decoder
        {
            private readonly Dictionary<uint, InstrDecoder> xDecoders;

            public ADecoder(Dictionary<uint, InstrDecoder> xDecoders)
            {
                this.xDecoders = xDecoders;
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                return xDecoders[(wInstr >> 1) & 0x3FF].Decode(wInstr, dasm);
            }
        }

        public class XDecoder : Decoder
        {
            private readonly Dictionary<uint, Decoder> xDecoders;

            public XDecoder(Dictionary<uint, Decoder> xDecoders)
            {
                this.xDecoders = xDecoders;
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                var xOp = (wInstr >> 1) & 0x3FF;
                DumpMaskedInstruction(32, wInstr, 0x3FF << 1, "  XDecoder");
                if (xDecoders.TryGetValue(xOp, out var decoder))
                {
                    return decoder.Decode(wInstr, dasm);
                }
                else
                {
                    Debug.Print("Unknown PowerPC X instruction {0:X8} {1:X2}-{2:X3} ({2})", wInstr, wInstr >> 26, xOp);
                    return dasm.EmitUnknown(wInstr);
                }
            }
        }

        public class FpuDecoder : Decoder
        {
            private readonly Dictionary<uint, Decoder> decoders;
            private readonly int shift;
            private readonly uint mask;

            public FpuDecoder(int shift, uint mask, Dictionary<uint, Decoder> decoders)
            {
                this.shift = shift;
                this.mask = mask;
                this.decoders = decoders;
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                var x = (wInstr >> shift) & mask;
                if (decoders.TryGetValue(x, out Decoder? decoder))
                {
                    return decoder.Decode(wInstr, dasm);
                }
                else
                {
                    return dasm.CreateInvalidInstruction();
                }
            }
        }

        public class IDecoder : Decoder
        {
            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                var mnemonic = (wInstr & 1) == 1 ? Mnemonic.bl : Mnemonic.b;
                var iclass = (wInstr & 1) == 1 ? InstrClass.Transfer | InstrClass.Call : InstrClass.Transfer;
                var sOffset = (int) Bits.SignExtend(wInstr & 0x03FF_FFFC, 26);
                var baseAddr = (wInstr & 2) != 0 ? Address.Create(dasm.defaultWordWidth, 0) : dasm.rdr.Address - 4;
                return new PowerPcInstruction(mnemonic)
                {
                    InstructionClass = iclass,
                    Operands = new MachineOperand[] { new AddressOperand(baseAddr + sOffset) },
                };
            }
        }

        public class BDecoder : Decoder
        {
            private static readonly Mnemonic[] opcBdnzf =
            {
                Mnemonic.bdnzf, Mnemonic.bdnzfl
            };

            private static readonly Mnemonic[] opcBdzf =
            {
                Mnemonic.bdzf, Mnemonic.bdzfl
            };

            private static readonly Mnemonic[,] opcBNcc =
            {
                { Mnemonic.bge, Mnemonic.bgel },
                { Mnemonic.ble, Mnemonic.blel },
                { Mnemonic.bne, Mnemonic.blel },
                { Mnemonic.bns, Mnemonic.bnsl },
            };

            private static readonly Mnemonic[,] opcBcc =
            {
                { Mnemonic.blt, Mnemonic.bgel },
                { Mnemonic.bgt, Mnemonic.blel },
                { Mnemonic.beq, Mnemonic.blel },
                { Mnemonic.bso, Mnemonic.bnsl },
            };

            private static readonly Mnemonic[] opcBdnzt =
            {
                Mnemonic.bdnzt, Mnemonic.bdnztl
            };

            private static readonly Mnemonic[] opcBdzt =
            {
                Mnemonic.bdzt, Mnemonic.bdztl
            };

            private static readonly Mnemonic[] opcBdnz =
            {
                Mnemonic.bdnz, Mnemonic.bdnzl
            };

            private static readonly Mnemonic[] opcBdz =
            {
                Mnemonic.bdz, Mnemonic.bdzl
            };

            private static readonly Mnemonic[] opcB =
            {
                Mnemonic.b, Mnemonic.bl
            };

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                uint link = (wInstr & 1);
                var sOffset = (short)(wInstr & 0x0000FFFC);
                var grfBi = (wInstr >> 16) & 0x1F;
                var grfBo = (wInstr >> 21) & 0x1F;
                var crf = grfBi >> 2;

                Mnemonic mnemonic;
                InstrClass iclass = link == 1 ? InstrClass.Transfer | InstrClass.Call : InstrClass.Transfer;
                var ops = new List<MachineOperand>();
                var baseAddr = (wInstr & 2) != 0 ? Address.Create(dasm.defaultWordWidth, 0) : dasm.rdr.Address - 4;
                var dst = new AddressOperand(baseAddr + sOffset);
                switch (grfBo)
                {
                case 0:
                case 1:
                    // Decrement ctr, branch if ctr != 0 and condition is false
                    mnemonic = opcBdnzf[link];
                    iclass |= InstrClass.Conditional;
                    ops.Add(new ConditionOperand(grfBi));
                    ops.Add(dst);
                    break;
                case 2:
                case 3:
                    // Decrement ctr, branch if ctr == 0 and condition is false
                    mnemonic = opcBdzf[link];
                    iclass |= InstrClass.Conditional;
                    ops.Add(new ConditionOperand(grfBi));
                    ops.Add(dst);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                    // Branch if condition is false
                    mnemonic = opcBNcc[grfBi & 0b11, link];
                    iclass |= InstrClass.Conditional;
                    if (grfBi >= 4)
                    {
                        ops.Add(dasm.arch.CrRegisters[(int)grfBi >> 2]);
                    }
                    ops.Add(dst);
                    break;
                case 8:
                case 9:
                    // Decrement ctr, branch if ctr != 0 and condition is true
                    mnemonic = opcBdnzt[link];
                    iclass |= InstrClass.Conditional;
                    ops.Add(new ConditionOperand(grfBi));
                    ops.Add(dst);
                    break;
                case 0xA:
                case 0xB:
                    // Decrement ctr, branch if ctr == 0 and condition is true
                    mnemonic = opcBdzt[link];
                    iclass |= InstrClass.Conditional;
                    ops.Add(new ConditionOperand(grfBi));
                    ops.Add(dst);
                    break;
                case 0xC:
                case 0xD:
                case 0xE:
                case 0xF:
                    // Branch if condition is true.
                    mnemonic = opcBcc[grfBi & 0b11, link];
                    iclass |= InstrClass.Conditional;
                    if (grfBi >= 4)
                    {
                        ops.Add(dasm.arch.CrRegisters[(int)grfBi >> 2]);
                    }
                    ops.Add(dst);
                    break;
                case 0b10000:
                case 0b10001:
                case 0b11000:
                case 0b11001:
                    // Decrement ctr, Branch if ctr != 0
                    mnemonic = opcBdnz[link];
                    iclass |= InstrClass.Conditional;
                    ops.Add(dst);
                    break;
                case 0b10010:
                case 0b10011:
                case 0b11010:
                case 0b11011:
                    // Decrement ctr, Branch if ctr == 0
                    mnemonic = opcBdz[link];
                    iclass |= InstrClass.Conditional;
                    ops.Add(dst);
                    break;
                default:
                    mnemonic = opcB[link];
                    ops.Add(dst);
                    break;
                }
                return new PowerPcInstruction(mnemonic)
                {
                    InstructionClass = iclass,
                    Operands = ops.ToArray()
                };
            }
        }

        public class BclrDecoder : Decoder
        {
            public BclrDecoder()
            {
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                bool link = (wInstr & 1) != 0;
                var mnemonic = link ? Mnemonic.blrl : Mnemonic.blr;
                var crBit = (wInstr >> 16) & 0x1F;
                var crf = crBit >> 2;
                var condCode = ((wInstr >> 22) & 4) | (crBit & 0x3);
                var bo = (wInstr >> 21) & 0x1F;
                if ((bo & 0x14) == 0x14)
                {
                    return new PowerPcInstruction(Mnemonic.blr)
                    {
                        InstructionClass = InstrClass.Transfer|InstrClass.Return,
                        Operands = Array.Empty<MachineOperand>()
                    };
                }

                var iclass = link 
                    ? InstrClass.Transfer | InstrClass.Call 
                    : InstrClass.Transfer | InstrClass.Return;
                switch (condCode)
                {
                default:
                    return new PowerPcInstruction(link ? Mnemonic.bclrl : Mnemonic.bclr)
                    {
                        InstructionClass = iclass,
                        Operands = new MachineOperand[]
                        {
                            ImmediateOperand.Byte((byte)((wInstr >> 21) & 0x1F)),
                            ImmediateOperand.Byte((byte)((wInstr >> 16) & 0x1F)),
                        }
                    };
                case 0: mnemonic = link ? Mnemonic.bgelrl : Mnemonic.bgelr; break;
                case 1: mnemonic = link ? Mnemonic.blelrl : Mnemonic.blelr; break;
                case 2: mnemonic = link ? Mnemonic.bnelrl : Mnemonic.bnelr; break;
                case 3: mnemonic = link ? Mnemonic.bnslrl : Mnemonic.bnslr; break;
                case 4: mnemonic = link ? Mnemonic.bltlrl : Mnemonic.bltlr; break;
                case 5: mnemonic = link ? Mnemonic.bgtlrl : Mnemonic.bgtlr; break;
                case 6: mnemonic = link ? Mnemonic.beqlrl : Mnemonic.beqlr; break;
                case 7: mnemonic = link ? Mnemonic.bsolrl : Mnemonic.bsolr; break;
                }
                return new PowerPcInstruction(mnemonic)
                {
                    InstructionClass = iclass | InstrClass.Conditional,
                    Operands = new MachineOperand[] { dasm.CRegFromBits(crf) },
                };
            }
        }

        public class XfxDecoder : InstrDecoder
        {
            public XfxDecoder(Mnemonic mnemonic, params Mutator<PowerPcDisassembler>[] mutators) : base(mnemonic, mutators)
            {
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                var reg = dasm.RegFromBits(wInstr >> 21);
                var spr = (wInstr >> 11) & 0x3FF;
                return new PowerPcInstruction(mnemonic)
                {
                    InstructionClass = base.iclass,
                    Operands = new MachineOperand[] {
                        reg,
                        ImmediateOperand.Word16((ushort) spr)
                    }
                };
            }
        }

        public class SprDecoder : Decoder
        {
            private readonly bool to;
            
            public SprDecoder(bool to)
            {
                this.to = to;
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                //var ops = new List<MachineOperand> { dasm.RegFromBits(wInstr >> 21) };
                var spr = ((wInstr >> 16) & 0x1F) | ((wInstr >> 6) & 0x3E0);
                Mnemonic mnemonic;
                switch (spr)
                {
                case 0x08: mnemonic = to ? Mnemonic.mtlr : Mnemonic.mflr;
                    r1(wInstr, dasm);
                    break;
                case 0x09: mnemonic = to ? Mnemonic.mtctr : Mnemonic.mfctr; 
                    r1(wInstr, dasm);
                    break;
                default:
                    mnemonic = to ? Mnemonic.mtspr : Mnemonic.mfspr;
                    SPR(wInstr, dasm);
                    r1(wInstr, dasm);
                    break;
                }
                return new PowerPcInstruction(mnemonic)
                {
                    InstructionClass = InstrClass.Linear,
                    Operands = dasm.ops.ToArray()
                };
            }
        }

        public class CmpDecoder : InstrDecoder
        {
            public CmpDecoder(Mnemonic op, params Mutator<PowerPcDisassembler>[] mutators) : base(op, mutators)
            { }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                var l = ((wInstr >> 21) & 1) != 0;
                var op = Mnemonic.illegal;
                switch (this.mnemonic)
                {
                default: throw new NotImplementedException();
                case Mnemonic.cmp: op = l ? Mnemonic.cmpl : Mnemonic.cmp; break;
                case Mnemonic.cmpi: op = l ? Mnemonic.cmpi : Mnemonic.cmpwi; break;
                case Mnemonic.cmpl: op = l ? Mnemonic.cmpl : Mnemonic.cmplw; break;
                case Mnemonic.cmpli: op = l ? Mnemonic.cmpli : Mnemonic.cmplwi; break;
                }
                return DecodeOperands(wInstr, dasm, iclass, op, mutators);
            }
        }

        public class XX3Decoder : Decoder
        {
            private readonly Dictionary<uint, Decoder> decoders;

            public XX3Decoder(Dictionary<uint, Decoder> decoders)
            {
                this.decoders = decoders;
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                var subOp = (wInstr & 0xFFFF) >> 3;
                if (decoders.TryGetValue(subOp, out var decoder))
                {
                    return decoder.Decode(wInstr, dasm);
                }
                else
                {
                    Debug.Print("Unknown PowerPC XX3 instruction {0:X8} {1:X2}-{2:X3} ({2})", wInstr, wInstr >> 26, subOp);
                    return dasm.EmitUnknown(wInstr);
                }
            }
        }

        public class VMXDecoder : Decoder
        {
            private readonly uint mask;
            public readonly Dictionary<uint, Decoder> decoders;

            public VMXDecoder(uint mask, Dictionary<uint, Decoder> decoders)
            {
                this.mask = mask;
                this.decoders = decoders;
            }

            public override PowerPcInstruction Decode(uint wInstr, PowerPcDisassembler dasm)
            {
                var key = (wInstr >> 0x4) & mask;
                DumpMaskedInstruction(32, wInstr, mask << 4, "  VMX");
                if (decoders.TryGetValue(key, out Decoder? decoder))
                {
                    return decoder.Decode(wInstr, dasm);
                }
                else
                {
                    Debug.Print("Unknown PowerPC VMX instruction {0:X8} {1:X2}-{2:X3} ({2})", wInstr, wInstr >> 26, key);
                    return dasm.EmitUnknown(wInstr);
                }
            }

            public static PowerPcInstruction DecodeVperm128(PowerPcDisassembler dasm, uint wInstr)
            {
                var instr = InstrDecoder.DecodeOperands(
                    wInstr, dasm, 
                    InstrClass.Linear, Mnemonic.vperm128,
                    new Mutator<PowerPcDisassembler>[] { Wd, Wa, Wb, v3_6 });
                return instr;
            }
        }
    }
}
