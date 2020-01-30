using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PowerPC
{
    public partial class PowerPcDisassembler
    {
        public abstract class Decoder
        {
            public abstract PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr);

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

        public class InvalidDecoder : Decoder
        {
            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
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

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                EmitUnitTest(wInstr);
                return dasm.CreateInvalidInstruction();
            }

            [Conditional("DEBUG")]
            private void EmitUnitTest(uint wInstr)
            {
                Debug.Print("    // {0}", message);
                Debug.Print("    [Test]");
                Debug.Print("    public void PPCDis_{0:X8}()", wInstr);
                Debug.Print("    {");
                Debug.Print("        AssertCode(0x{0:X8}, \"@@@\");", wInstr);
                Debug.Print("    }");
                Debug.Print("");
            }
        }

        public class MaskDecoder : Decoder
        {
            private readonly int shift;
            private readonly uint mask;
            private readonly Decoder[] decoders;

            public MaskDecoder(int ppcBitPosition, int bits, params Decoder[] decoders)
            {
                // Convert awkward PPC bit numbering to something sane.
                this.shift = 32 - (ppcBitPosition + bits);
                this.mask = (1u << bits) - 1;
                Debug.Assert(decoders.Length == (1 << bits));
                this.decoders = decoders;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var bitfield = (wInstr >> shift) & mask;
                return decoders[bitfield].Decode(dasm, wInstr);
            }
        }

        public class FnDecoder : Decoder
        {
            private readonly Func<PowerPcDisassembler, uint, PowerPcInstruction> decoder;

            public FnDecoder(Func<PowerPcDisassembler, uint, PowerPcInstruction> decoder)
            {
                this.decoder = decoder;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
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

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                return DecodeOperands(wInstr, dasm, iclass, mnemonic, mutators);
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

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                Mnemonic mnemonic = ((wInstr & 1) == 0) ? mnemonic0 : mnemonic1;
                wInstr &= ~3u;
                return DecodeOperands(wInstr & ~3u, dasm, iclass, mnemonic, mutators);
            }
        }

        public class MDDecoder : Decoder
        {
            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
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

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                return xDecoders[(wInstr >> 1) & 0x3FF].Decode(dasm, wInstr);
            }
        }

        public class XDecoder : Decoder
        {
            private readonly Dictionary<uint, Decoder> xDecoders;

            public XDecoder(Dictionary<uint, Decoder> xDecoders)
            {
                this.xDecoders = xDecoders;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var xOp = (wInstr >> 1) & 0x3FF;
                if (xDecoders.TryGetValue(xOp, out var decoder))
                {
                    return decoder.Decode(dasm, wInstr);
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

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var x = (wInstr >> shift) & mask;
                if (decoders.TryGetValue(x, out Decoder decoder))
                {
                    return decoder.Decode(dasm, wInstr);
                }
                else
                {
                    return dasm.CreateInvalidInstruction();
                }
            }
        }

        public class XlDecoderAux : InstrDecoder
        {
            private readonly Mnemonic opLink;

            public XlDecoderAux(Mnemonic mnemonic, Mnemonic opLink, params Mutator<PowerPcDisassembler> [] mutators)
                : base(mnemonic, mutators)
            {
                this.opLink = opLink;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                bool link = (wInstr & 1) != 0;
                var mnemonic = link ? this.opLink : this.mnemonic;
                var iclass = link ? InstrClass.Transfer | InstrClass.Call : InstrClass.Transfer;
                foreach (var m in mutators)
                {
                    if (!m(wInstr, dasm))
                        return dasm.CreateInvalidInstruction();
                }
                return dasm.MakeInstruction(iclass, mnemonic);
            }
        }

        public class IDecoder : Decoder
        {
            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var mnemonic = (wInstr & 1) == 1 ? Mnemonic.bl : Mnemonic.b;
                var iclass = (wInstr & 1) == 1 ? InstrClass.Transfer | InstrClass.Call : InstrClass.Transfer;
                var uOffset = wInstr & 0x03FFFFFC;
                if ((uOffset & 0x02000000) != 0)
                    uOffset |= 0xFF000000;
                var baseAddr = (wInstr & 2) != 0 ? Address.Create(dasm.defaultWordWidth, 0) : dasm.rdr.Address - 4;
                return new PowerPcInstruction(mnemonic)
                {
                    InstructionClass = iclass,
                    Operands = new MachineOperand[] { new AddressOperand(baseAddr + uOffset) },
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

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                uint link = (wInstr & 1);
                var uOffset = wInstr & 0x0000FFFC;
                if ((uOffset & 0x8000) != 0)
                    uOffset |= 0xFFFF0000;
                var grfBi = (wInstr >> 16) & 0x1F;
                var grfBo = (wInstr >> 21) & 0x1F;
                var crf = grfBi >> 2;

                Mnemonic mnemonic;
                InstrClass iclass = link == 1 ? InstrClass.Transfer | InstrClass.Call : InstrClass.Transfer;
                var ops = new List<MachineOperand>();
                var baseAddr = (wInstr & 2) != 0 ? Address.Create(dasm.defaultWordWidth, 0) : dasm.rdr.Address - 4;
                var dst = new AddressOperand(baseAddr + uOffset);
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
                        ops.Add(new RegisterOperand(dasm.arch.CrRegisters[(int)grfBi >> 2]));
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
                        ops.Add(new RegisterOperand(dasm.arch.CrRegisters[(int)grfBi >> 2]));
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

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
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
                        InstructionClass = InstrClass.Transfer,
                        Operands = new MachineOperand[0]
                    };
                }

                var iclass = link ? InstrClass.Call : 0;
                switch (condCode)
                {
                default:
                    return new PowerPcInstruction(link ? Mnemonic.bclrl : Mnemonic.bclr)
                    {
                        InstructionClass = iclass | InstrClass.Transfer,
                        Operands = new MachineOperand[]
                        {
                            new ImmediateOperand(Constant.Byte((byte)((wInstr >> 21) & 0x1F))),
                            new ImmediateOperand(Constant.Byte((byte)((wInstr >> 16) & 0x1F))),
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
                    InstructionClass = iclass | InstrClass.ConditionalTransfer,
                    Operands = new MachineOperand[] { dasm.CRegFromBits(crf) },
                };
            }
        }

        public class XfxDecoder : InstrDecoder
        {
            public XfxDecoder(Mnemonic mnemonic, params Mutator<PowerPcDisassembler>[] mutators) : base(mnemonic, mutators)
            {
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var reg = dasm.RegFromBits(wInstr >> 21);
                var spr = (wInstr >> 11) & 0x3FF;
                return new PowerPcInstruction(mnemonic)
                {
                    InstructionClass = base.iclass,
                    Operands = new MachineOperand[] {
                        reg,
                        new ImmediateOperand(Constant.Word16((ushort)spr))
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

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var ops = new List<MachineOperand> { dasm.RegFromBits(wInstr >> 21) };
                var spr = ((wInstr >> 16) & 0x1F) | ((wInstr >> 6) & 0x3E0);
                Mnemonic mnemonic;
                switch (spr)
                {
                case 0x08: mnemonic = to ? Mnemonic.mtlr : Mnemonic.mflr; break;
                case 0x09: mnemonic = to ? Mnemonic.mtctr : Mnemonic.mfctr; break;
                default:
                    mnemonic = to ? Mnemonic.mtspr : Mnemonic.mfspr;
                    ops.Insert(0, ImmediateOperand.UInt32(spr));
                    break;
                }
                return new PowerPcInstruction(mnemonic)
                {
                    InstructionClass = InstrClass.Linear,
                    Operands = ops.ToArray()
                };
            }
        }

        public class CmpDecoder : InstrDecoder
        {
            public CmpDecoder(Mnemonic op, params Mutator<PowerPcDisassembler>[] mutators) : base(op, mutators)
            { }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
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

        public class VXDecoder : Decoder
        {
            private Dictionary<uint, Decoder> vxDecoders;
            private Dictionary<uint, Decoder> vaDecoders;

            public VXDecoder(Dictionary<uint, Decoder> vxDecoders, Dictionary<uint, Decoder> vaDecoders)
            {
                this.vxDecoders = vxDecoders;
                this.vaDecoders = vaDecoders;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var xOp = wInstr & 0x7FFu;
                if (vxDecoders.TryGetValue(xOp, out Decoder decoder))
                {
                    return decoder.Decode(dasm, wInstr);
                }
                else if (vaDecoders.TryGetValue(wInstr & 0x3Fu, out decoder))
                {
                    return decoder.Decode(dasm, wInstr);
                }
                else
                {
                    Debug.Print("Unknown PowerPC VX instruction {0:X8} {1:X2}-{2:X3} ({2})", wInstr, wInstr >> 26, xOp);
                    return dasm.EmitUnknown(wInstr);
                }
            }
        }

        public class XSDecoder : InstrDecoder
        {
            public XSDecoder(Mnemonic mnemonic, params Mutator<PowerPcDisassembler>[] mutators) : base(mnemonic, mutators) { }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var instr = base.Decode(dasm, wInstr);
                var c = ((ImmediateOperand)instr.Operands[2]).Value.ToInt32();
                if ((wInstr & 2) != 0)
                    c += 32;
                instr.Operands[2] = new ImmediateOperand(Constant.Byte((byte)c));
                return instr;
            }
        }

        public class XX3Decoder : Decoder
        {
            private readonly Dictionary<uint, Decoder> decoders;

            public XX3Decoder(Dictionary<uint, Decoder> decoders)
            {
                this.decoders = decoders;
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

        public class VMXDecoder : Decoder
        {
            private readonly uint mask;
            public Dictionary<uint, Decoder> decoders;

            public VMXDecoder(uint mask, Dictionary<uint, Decoder> decoders)
            {
                this.mask = mask;
                this.decoders = decoders;
            }

            public override PowerPcInstruction Decode(PowerPcDisassembler dasm, uint wInstr)
            {
                var key = (wInstr >> 0x4) & mask;
                if (decoders.TryGetValue(key, out Decoder decoder))
                {
                    return decoder.Decode(dasm, wInstr);
                }
                else
                {
                    Debug.Print("Unknown PowerPC VMX instruction {0:X8} {1:X2}-{2:X3} ({2})", wInstr, wInstr >> 26, key);
                    return dasm.EmitUnknown(wInstr);
                }
            }

            public static PowerPcInstruction DecodeVperm128(PowerPcDisassembler dasm, uint wInstr)
            {
                var instr = Decoder.DecodeOperands(
                    wInstr, dasm, 
                    InstrClass.Linear, Mnemonic.vperm128,
                    new Mutator<PowerPcDisassembler>[] { Wd, Wa, Wb, v3_6 });
                return instr;
            }
        }
    }
}
