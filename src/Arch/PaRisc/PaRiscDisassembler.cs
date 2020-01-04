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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.PaRisc
{
    using Decoder = Decoder<PaRiscDisassembler, Mnemonic, PaRiscInstruction>;

    public class PaRiscDisassembler : DisassemblerBase<PaRiscInstruction, Mnemonic>
    {
        private const InstrClass TD = InstrClass.Transfer | InstrClass.Delay;
        private const InstrClass CTD = InstrClass.ConditionalTransfer | InstrClass.Delay;

        private static readonly MaskDecoder rootDecoder;
        private static readonly Decoder invalid;

        private readonly PaRiscArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly bool is64bit;
        private readonly PrimitiveType dtSignedWord;

        private Address addr;
        private readonly List<MachineOperand> ops;
        private bool annul;
        private bool zero;
        private SignExtension signExtend;
        private ConditionOperand cond;
        private AddrRegMod addrMod;
        private int coprocessor;
        private FpFormat fpFormat;
        private FpFormat fpFormatDst;
        private CacheHint cacheHint;

        public PaRiscDisassembler(PaRiscArchitecture arch, EndianImageReader rdr)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.ops = new List<MachineOperand>();
            is64bit = arch.Is64Bit();
            this.dtSignedWord = is64bit ? PrimitiveType.Int64 : PrimitiveType.Int32;
        }

        public override PaRiscInstruction DisassembleInstruction()
        {
            this.addr = rdr.Address;
            this.ops.Clear();
            this.annul = false;
            this.zero = false;
            this.signExtend = 0;
            this.cond = null;
            this.coprocessor = -1;
            this.addrMod = AddrRegMod.None;
            this.fpFormat = FpFormat.None;
            this.fpFormatDst = FpFormat.None;
            this.cacheHint = CacheHint.None;

            if (!rdr.TryReadBeUInt32(out uint uInstr))
                return null;
            var instr = rootDecoder.Decode(uInstr, this);
            instr.Address = this.addr;
            instr.Length = (int) (rdr.Address - addr);
            instr.InstructionClass |= uInstr == 0 ? InstrClass.Zero : 0;
            return instr;
        }

        public override PaRiscInstruction MakeInstruction(InstrClass iclass, Mnemonic mnemonic)
        {
            return new PaRiscInstruction
            {
                InstructionClass = iclass,
                Mnemonic = mnemonic,
                Coprocessor = this.coprocessor,
                Condition = this.cond,
                Operands = this.ops.ToArray(),
                Annul = this.annul,
                Zero = this.zero,
                Sign = this.signExtend,
                BaseReg = this.addrMod,
                FpFmt = this.fpFormat,
                FpFmtDst = this.fpFormatDst,
                CacheHint = this.cacheHint
            };
        }

        public override PaRiscInstruction CreateInvalidInstruction()
        {
            return invalid.Decode(0, this);
        }

        /// <summary>
        /// Create a Reko bitfield using PA Risc bit position and bit length.
        /// </summary>
        /// <remarks>
        /// PA Risc instruction bits are numbered from the MSB to LSB, but 
        /// Reko bitfields are numbered from LSB to MSB.
        /// </summary>
        private static Bitfield BeField(int bitPos, int bitLength)
        {
            return new Bitfield(32 - (bitPos + bitLength), bitLength);
        }

        private static Bitfield[] BeFields(params (int bitPos, int bitLength)[] flds)
        {
            return flds.Select(f => BeField(f.bitPos, f.bitLength)).ToArray();
        }

        /// <summary>
        /// Tests if the bit at big-endian position <paramref name="bitPos"/> of the word 
        /// <paramref name="u"/> is set.
        /// </summary>
        private static bool BeBitSet(uint u, int bitPos)
        {
            return Bits.IsBitSet(u, 31 - bitPos);
        }

        /// <summary>
        /// Unsigned immediate field.
        /// </summary>
        private static Mutator<PaRiscDisassembler> u(int bitPos, int bitLength, PrimitiveType dt)
        {
            var field = BeField(bitPos, bitLength);
            return (u, d) =>
            {
                var v = field.Read(u);
                Decoder.DumpMaskedInstruction(u, field.Mask << field.Position, "Unsigned immediate");
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, v)));
                return true;
            };
        }

        /// <summary>
        /// Unsigned immediate fields.
        /// </summary>
        private static Mutator<PaRiscDisassembler> u(Bitfield[] fields, Func<uint, Bitfield[], uint> mutator)
        {
            return (u, d) =>
            {
                var n = mutator(u, fields);
                d.ops.Add(new ImmediateOperand(Constant.Create(PrimitiveType.Word32, n)));
                return true;
            };
        }
        private static Mutator<PaRiscDisassembler> u8(int bitPos, int bitLength) => u(bitPos, bitLength, PrimitiveType.Byte);
        private static Mutator<PaRiscDisassembler> u16(int bitPos, int bitLength) => u(bitPos, bitLength, PrimitiveType.Word16);

        private static Mutator<PaRiscDisassembler> u8From31(int bitPos, int bitLength)
        {
            var field = BeField(bitPos, bitLength);
            return (u, d) =>
            {
                var v = 31 - field.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(PrimitiveType.Byte, v)));
                return true;
            };
        }

        private static Mutator<PaRiscDisassembler> u8From32(int bitPos, int bitLength)
        {
            var field = BeField(bitPos, bitLength);
            return (u, d) =>
            {
                var v = 32 - field.Read(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(PrimitiveType.Byte, v)));
                return true;
            };
        }

        private static Mutator<PaRiscDisassembler> Left(Bitfield[] fields, Func<uint, Bitfield[], uint> mutator, int sh)
        {
            return (u, d) =>
            {
                var n = mutator(u, fields);
                d.ops.Add(new LeftImmediateOperand((int)n << sh));
                return true;
            };
        }

        /// <summary>
        /// Signed immediate constant.
        /// </summary>
        private static Mutator<PaRiscDisassembler> s(int bitPos, int bitLength, PrimitiveType dt)
        {
            var field = BeField(bitPos, bitLength);
            return (u, d) =>
            {
                var v = field.ReadSigned(u);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, v)));
                return true;
            };
        }
        private static Mutator<PaRiscDisassembler> s32(int bitPos, int bitLength) => s(bitPos, bitLength, PrimitiveType.Int32);


        /// <summary>
        /// Wonky "low sign extend" format. 
        /// </summary>
        /// <remarks>
        /// From the manual:
        /// sign_ext(cat(x{ len - 1},x{ 0..len - 2}),len))
        /// </remarks>
        private static Mutator<PaRiscDisassembler> lse(int bitPos, int bitLength)
        {
            var fields = BeFields((bitPos + bitLength - 1, 1), (bitPos, bitLength - 1));
            return (u, d) =>
            {
                var v = Bitfield.ReadSignedFields(fields, u);
                d.ops.Add(new ImmediateOperand(Constant.Create(d.dtSignedWord, v)));
                return true;
            };
        }

        /// <summary>
        /// Signed immediate constant spread across multiple fields with a postprocessor.
        /// </summary>
        private static Mutator<PaRiscDisassembler> s(PrimitiveType dt, Bitfield[] fields, Func<bool, uint, Bitfield[], uint> mutator)
        {
            return (u, d) =>
            {
                var v = mutator(d.is64bit, u, fields);
                d.ops.Add(new ImmediateOperand(Constant.Create(dt, v)));
                return true;
            };
        }

        /// <summary>
        /// Bit position used by the 'bb' instruction.
        /// </summary>
        private static Mutator<PaRiscDisassembler> bb_bitpos()
        {
            var pField = BeField(6, 5);
            var dField = BeField(18, 1);
            return (u, dasm) =>
            {
                var pos = pField.Read(u);
                var imm = ImmediateOperand.UInt32(pos);
                dasm.ops.Add(imm);
                return true;
            };
        }

        /// <summary>
        /// Fixed shift amount 
        /// </summary>
        private static Mutator<PaRiscDisassembler> shamt(Bitfield[] fields)
        {
            return (u, d) =>
            {
                var sh = Bitfield.ReadFields(fields, u);
                sh = 63 - sh;
                var imm = ImmediateOperand.UInt32(sh);
                d.ops.Add(imm);
                return true;
            };
        }


        // Registers

        private static Mutator<PaRiscDisassembler> r(int bitPos, int bitLength)
        {
            var field = BeField(bitPos, bitLength);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.ops.Add(new RegisterOperand(Registers.GpRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<PaRiscDisassembler> r6 = r(6, 5);
        private static readonly Mutator<PaRiscDisassembler> r11 = r(11, 5);
        private static readonly Mutator<PaRiscDisassembler> r27 = r(27, 5);

        private static Mutator<PaRiscDisassembler> fr(int bitPos, int bitLength)
        {
            var field = BeField(bitPos, bitLength);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.ops.Add(new RegisterOperand(Registers.FpRegs[iReg]));
                return true;
            };
        }
        private static readonly Mutator<PaRiscDisassembler> fr6 = fr(6, 5);
        private static readonly Mutator<PaRiscDisassembler> fr11 = fr(11, 5);
        private static readonly Mutator<PaRiscDisassembler> fr27 = fr(27, 5);

        /// <summary>
        /// Single precision floating point register.
        /// </summary>
        private static Mutator<PaRiscDisassembler> frsng(params (int bitPos, int bitLength)[] flds)
        {
            var fields = BeFields(flds);
            return (u, d) =>
            {
                var iReg = Bitfield.ReadFields(fields, u);
                d.ops.Add(new RegisterOperand(Registers.FpRegs32[iReg]));
                return true;
            };
        }
        private static readonly Mutator<PaRiscDisassembler> fr6_24 = frsng((6, 5), (24,1));
        private static readonly Mutator<PaRiscDisassembler> fr6_25 = frsng((6, 5), (25,1));
        private static readonly Mutator<PaRiscDisassembler> fr24_6 = frsng((24,1), (6, 5));
        private static readonly Mutator<PaRiscDisassembler> fr11_19 = frsng((11, 5), (19,1));
        private static readonly Mutator<PaRiscDisassembler> fr24_11 = frsng((24,1), (11, 5));
        private static readonly Mutator<PaRiscDisassembler> fr25_27 = frsng((25,1), (27, 5));
        private static readonly Mutator<PaRiscDisassembler> fr6_20 = frsng((20, 1), (6, 5));
        private static readonly Mutator<PaRiscDisassembler> fr27_18 = frsng((18, 1), (27, 5));

        /// <summary>
        /// Floating point register used in multiple operation instruction.
        /// </summary>
        private static Mutator<PaRiscDisassembler> fmo(int bitpos)
        {
            var field_dbl = BeField(bitpos, 5);
            return (u, d) =>
            {
                var iReg = field_dbl.Read(u);
                RegisterStorage freg;
                if (d.fpFormat == FpFormat.dbl)
                {
                    freg = Registers.FpRegs[iReg];
                }
                else
                {
                    if (iReg == 0)
                        return false;       // 16L is invalid according to HP.
                    var bank = iReg & 0x10;
                    iReg = (iReg & 0x0F) | 0x10;
                    freg = Registers.FpRegs32[iReg | bank];
                }
                d.ops.Add(new RegisterOperand(freg));
                return true;
            };
        }
        private static Mutator<PaRiscDisassembler> fmo6 = fmo(6);
        private static Mutator<PaRiscDisassembler> fmo11 = fmo(11);
        private static Mutator<PaRiscDisassembler> fmo16 = fmo(16);
        private static Mutator<PaRiscDisassembler> fmo21 = fmo(21);
        private static Mutator<PaRiscDisassembler> fmo27 = fmo(27);


        /// <summary>
        /// Specific register.
        /// </summary>
        private static Mutator<PaRiscDisassembler> reg(RegisterStorage r)
        {
            return (u, d) =>
            {
                d.ops.Add(new RegisterOperand(r));
                return true;
            };
        }

        /// <summary>
        /// Address space register.
        /// </summary>
        private static Mutator<PaRiscDisassembler> sr(int bitPos)
        {
            var field = BeField(bitPos, 3);
            return (u, d) =>
            {
                var iReg = field.Read(u);
                d.ops.Add(new RegisterOperand(Registers.SpaceRegs[iReg]));
                return true;
            };
        }

        /// <summary>
        /// Control register
        /// </summary>
        private static Mutator<PaRiscDisassembler> cr(int bitpos, int bitlen)
        {
            var field = BeField(bitpos, bitlen);
            return (u, d) =>
            {
                var iReg = (int)field.Read(u);
                if (!Registers.ControlRegisters.TryGetValue(iReg, out var cReg))
                {
                    return false;
                }
                d.ops.Add(new RegisterOperand(cReg));
                return true;
            };
        }

        /// <summary>
        /// Conditional field.
        /// </summary>
        private static Mutator<PaRiscDisassembler> cf(int bitPos, int bitLen, ConditionOperand[] conds)
        {
            Debug.Assert(conds.Length == (1 << bitLen));
            var field = BeField(bitPos, bitLen);
            return (u, d) =>
            {
                Decoder.DumpMaskedInstruction(u, field.Mask << field.Position, "conditional field");
                var iCond = field.Read(u);
                var cond = conds[iCond];
                if (cond == null)
                    return false;
                if (cond.Type != ConditionType.Never)
                {
                    d.cond = cond;
                }
                return true;
            };
        }
        // Table D-3 in PA-RISC 2.0 Manual
        private static Mutator<PaRiscDisassembler> cf16_cmpsub_32 = cf(16, 4, new[]
        {
            ConditionOperand.Never,
            ConditionOperand.Tr,
            ConditionOperand.Eq,
            ConditionOperand.Ne,

            ConditionOperand.Lt,
            ConditionOperand.Ge,
            ConditionOperand.Le,
            ConditionOperand.Gt,

            ConditionOperand.Ult,
            ConditionOperand.Uge,
            ConditionOperand.Ule,
            ConditionOperand.Ugt,

            ConditionOperand.Sv,
            ConditionOperand.Nsv,
            ConditionOperand.Odd,
            ConditionOperand.Even,
        });
        // Table D-4 in PA-RISC 2.0 Manual
        private static Mutator<PaRiscDisassembler> cf16_cmpsub_64 = cf(16, 4, new[]
        {
            ConditionOperand.Never64,
            ConditionOperand.Tr64,
            ConditionOperand.Eq64,
            ConditionOperand.Ne64,

            ConditionOperand.Lt64,
            ConditionOperand.Ge64,
            ConditionOperand.Le64,
            ConditionOperand.Gt64,

            ConditionOperand.Ult64,
            ConditionOperand.Uge64,
            ConditionOperand.Ule64,
            ConditionOperand.Ugt64,

            ConditionOperand.Sv64,
            ConditionOperand.Nsv64,
            ConditionOperand.Odd64,
            ConditionOperand.Even64,
        });

        private static Mutator<PaRiscDisassembler> cf16_add_32 = cf(16, 4, new[]
        {
            ConditionOperand.Never,
            ConditionOperand.Tr,
            ConditionOperand.Eq,
            ConditionOperand.Ne,

            ConditionOperand.Lt,
            ConditionOperand.Ge,
            ConditionOperand.Le,
            ConditionOperand.Gt,

            ConditionOperand.Nuv,
            ConditionOperand.Uv,
            ConditionOperand.Znv,
            ConditionOperand.Vnz,

            ConditionOperand.Sv,
            ConditionOperand.Nsv,
            ConditionOperand.Odd,
            ConditionOperand.Even,
        });
        private static Mutator<PaRiscDisassembler> cf16_add_64 = cf(16, 4, new[]
        {
            ConditionOperand.Never64,
            ConditionOperand.Tr64,
            ConditionOperand.Eq64,
            ConditionOperand.Ne64,

            ConditionOperand.Lt64,
            ConditionOperand.Ge64,
            ConditionOperand.Le64,
            ConditionOperand.Gt64,

            ConditionOperand.Nuv64,
            ConditionOperand.Uv64,
            ConditionOperand.Znv64,
            ConditionOperand.Vnz64,

            ConditionOperand.Sv64,
            ConditionOperand.Nsv64,
            ConditionOperand.Odd64,
            ConditionOperand.Even64,
        });
        private static Mutator<PaRiscDisassembler> cf16_log_32 = cf(16, 4, new[]
        {
            ConditionOperand.Never,
            ConditionOperand.Tr,
            ConditionOperand.Eq,
            ConditionOperand.Ne,

            ConditionOperand.Lt,
            ConditionOperand.Ge,
            ConditionOperand.Le,
            ConditionOperand.Gt,

            null,
            null,
            null,
            null,

            null,
            null,
            ConditionOperand.Odd,
            ConditionOperand.Even,
        });
        private static Mutator<PaRiscDisassembler> cf16_log_64 = cf(16, 4, new[]
        {
            ConditionOperand.Never64,
            ConditionOperand.Tr64,
            ConditionOperand.Eq64,
            ConditionOperand.Ne64,

            ConditionOperand.Lt64,
            ConditionOperand.Ge64,
            ConditionOperand.Le64,
            ConditionOperand.Gt64,

            null,
            null,
            null,
            null,

            null,
            null,
            ConditionOperand.Odd64,
            ConditionOperand.Even64,
        });

        private static Mutator<PaRiscDisassembler> cf_add()
        {
            return (u, d) =>
            {
                var is64bit = BeBitSet(u, 26);
                var fn = is64bit ? cf16_add_64 : cf16_add_32;
                return fn(u, d);
            };
        }
        private static Mutator<PaRiscDisassembler> cf_log()
        {
            return (u, d) =>
            {
                var is64bit = BeBitSet(u, 26);
                var fn = is64bit ? cf16_log_64 : cf16_log_32;
                return fn(u, d);
            };
        }
        private static Mutator<PaRiscDisassembler> cf_cmpsub()
        {
            return (u, d) =>
            {
                var is64bit = BeBitSet(u, 26);
                var fn = is64bit ? cf16_cmpsub_64 : cf16_cmpsub_32;
                return fn(u, d);
            };
        }

        private static Mutator<PaRiscDisassembler> cf16_cmp32_t = cf(16, 3, new[]
        {
            ConditionOperand.Never,
            ConditionOperand.Eq,
            ConditionOperand.Lt,
            ConditionOperand.Le,

            ConditionOperand.Ult,
            ConditionOperand.Ule,
            ConditionOperand.Sv,
            ConditionOperand.Odd,
        });
        private static Mutator<PaRiscDisassembler> cf16_cmp32_f = cf(16, 3, new[]
        {
            ConditionOperand.Tr,
            ConditionOperand.Ne,
            ConditionOperand.Ge,
            ConditionOperand.Gt,

            ConditionOperand.Uge,
            ConditionOperand.Ugt,
            ConditionOperand.Nsv,
            ConditionOperand.Even,
        });
        private static Mutator<PaRiscDisassembler> cf16_cmp64_t = cf(16, 3, new[]
{
            ConditionOperand.Never,
            ConditionOperand.Eq64,
            ConditionOperand.Lt64,
            ConditionOperand.Le64,

            ConditionOperand.Ult64,
            ConditionOperand.Ule64,
            ConditionOperand.Sv64,
            ConditionOperand.Odd64,
        });
        private static Mutator<PaRiscDisassembler> cf16_cmp64_f = cf(16, 3, new[]
        {
            ConditionOperand.Tr64,
            ConditionOperand.Ne64,
            ConditionOperand.Ge64,
            ConditionOperand.Gt64,

            ConditionOperand.Uge64,
            ConditionOperand.Ugt64,
            ConditionOperand.Nsv64,
            ConditionOperand.Even64,
        });

        private static Mutator<PaRiscDisassembler> cf16_shext = cf(16, 3, new[]
        {
            ConditionOperand.Never,
            ConditionOperand.Eq,
            ConditionOperand.Lt,
            ConditionOperand.Odd,

            ConditionOperand.Tr,
            ConditionOperand.Ne,
            ConditionOperand.Ge,
            ConditionOperand.Even,
        });

        private static Mutator<PaRiscDisassembler> cf16_add_3 = cf(16, 3, new[]
        {
            ConditionOperand.Never,
            ConditionOperand.Eq,
            ConditionOperand.Lt,
            ConditionOperand.Le,
            ConditionOperand.Nuv,
            ConditionOperand.Znv,
            ConditionOperand.Sv,
            ConditionOperand.Odd
        });

        private static Mutator<PaRiscDisassembler> cf16_add_3_neg = cf(16, 3, new[]
        {
            ConditionOperand.Tr,
            ConditionOperand.Ne,
            ConditionOperand.Ge,
            ConditionOperand.Gt,
            ConditionOperand.Uv,
            ConditionOperand.Vnz,
            ConditionOperand.Nsv,
            ConditionOperand.Even
        });

        private static Mutator<PaRiscDisassembler> cf16_add_3_64 = cf(16, 3, new[]
        {
            ConditionOperand.Never,
            ConditionOperand.Eq,   
            ConditionOperand.Lt,   
            ConditionOperand.Le,   
            ConditionOperand.Nuv,  
            ConditionOperand.Eq64, 
            ConditionOperand.Lt64, 
            ConditionOperand.Le64, 
        });

        private static Mutator<PaRiscDisassembler> cf16_add_3_neg_64 = cf(16, 3, new[]
        {
            ConditionOperand.Tr,
            ConditionOperand.Ne,
            ConditionOperand.Ge,
            ConditionOperand.Gt,
            ConditionOperand.Uv,
            ConditionOperand.Ne64,
            ConditionOperand.Ge64,
            ConditionOperand.Gt64
        });

        // Table D-15 of PA-RISC 2.0 manual
        private static readonly Mutator<PaRiscDisassembler> cf16_bb_1 = cf(16, 3, new[]
        {
            null,
            null,
            ConditionOperand.Lt,
            ConditionOperand.Lt64,

            null,
            null,
            ConditionOperand.Ge,
            ConditionOperand.Ge64,
        });

        private static readonly Mutator<PaRiscDisassembler> cf16_sh_ex_dp_3 = cf(16, 3, new[]
        {
            ConditionOperand.Never64,
            ConditionOperand.Eq64,
            ConditionOperand.Lt64,
            ConditionOperand.Odd64,

            ConditionOperand.Tr64,
            ConditionOperand.Ne64,
            ConditionOperand.Ge64,
            ConditionOperand.Even64,
        });

        private static bool cfadd_bitsize(uint uInstr, PaRiscDisassembler dasm)
        {
            if (dasm.is64bit)
            {
                return cf16_add_3_64(uInstr, dasm);
            }
            else
            {
                return cf16_add_3(uInstr, dasm);
            }
        }

        private static bool cfadd_bitsize_neg(uint uInstr, PaRiscDisassembler dasm)
        {
            if (dasm.is64bit)
            {
                return cf16_add_3_neg_64(uInstr, dasm);
            }
            else
            {
                return cf16_add_3_neg(uInstr, dasm);
            }
        }

        private static readonly Mutator<PaRiscDisassembler> cf27_fp = cf(27, 5, new[]
        {
            new ConditionOperand(ConditionType.FpFalseQ, "false?"),
            new ConditionOperand(ConditionType.FpFalse, "false"),
            new ConditionOperand(ConditionType.Fp2, "?"),
            new ConditionOperand(ConditionType.Fp3, "!<=>"),

            new ConditionOperand(ConditionType.Fp4, "="),
            new ConditionOperand(ConditionType.Fp5, "=T"),
            new ConditionOperand(ConditionType.Fp6, "?="),
            new ConditionOperand(ConditionType.Fp7, "!<>"),

            new ConditionOperand(ConditionType.Fp8, "!?>="),
            new ConditionOperand(ConditionType.Fp9, "<"),
            new ConditionOperand(ConditionType.Fp10, "?<"),
            new ConditionOperand(ConditionType.Fp11, "!>="),

            new ConditionOperand(ConditionType.Fp12, "!?>"),
            new ConditionOperand(ConditionType.Fp13, "<="),
            new ConditionOperand(ConditionType.Fp14, "?<="),
            new ConditionOperand(ConditionType.Fp15, "!>"),

            new ConditionOperand(ConditionType.Fp16, "!?<="),
            new ConditionOperand(ConditionType.Fp17, ">"),
            new ConditionOperand(ConditionType.Fp18, "?>"),
            new ConditionOperand(ConditionType.Fp19, "!<="),

            new ConditionOperand(ConditionType.Fp20, "!?<"),
            new ConditionOperand(ConditionType.Fp21, ">="),
            new ConditionOperand(ConditionType.Fp22, "?>="),
            new ConditionOperand(ConditionType.Fp23, "!<"),

            new ConditionOperand(ConditionType.Fp24, "!?="),
            new ConditionOperand(ConditionType.Fp25, "<>"),
            new ConditionOperand(ConditionType.Fp26, "!="),
            new ConditionOperand(ConditionType.Fp27, "!=T"),

            new ConditionOperand(ConditionType.Fp28, "!?"),
            new ConditionOperand(ConditionType.Fp29, "<=>"),
            new ConditionOperand(ConditionType.Fp30, "true?"),
            new ConditionOperand(ConditionType.Fp31, "true"),
        });


        /// <summary>
        /// Register indirect with displacement with space register
        /// </summary>
        private static Mutator<PaRiscDisassembler> M(PrimitiveType dt, int baseRegPos, Bitfield [] dispFields, Func<bool, uint,Bitfield[],uint> permutator)
        {
            var baseRegField = BeField(baseRegPos, 5);
            var totalLength = dispFields.Sum(f => f.Length);
            return (u, d) =>
            {
                var disp = (int) Bits.SignExtend(permutator(d.is64bit, u, dispFields), totalLength);
                var iBaseReg = baseRegField.Read(u);
                var am = Bitfield.ReadFields(amFields, u);
                d.ops.Add(MemoryOperand.Indirect(dt, disp, Registers.GpRegs[iBaseReg]));
                return true;
            };
        }

        /// <summary>
        /// Register indirect with displacement with space register and base register update
        /// </summary>
        private static Mutator<PaRiscDisassembler> Mam(PrimitiveType dt, int baseRegPos, Bitfield[] dispFields, Func<bool, uint, Bitfield[], uint> permutator)
        {
            var baseRegField = BeField(baseRegPos, 5);
            var totalLength = dispFields.Sum(f => f.Length);
            return (u, d) =>
            {
                var disp = (int) Bits.SignExtend(permutator(d.is64bit, u, dispFields), totalLength);
                var iBaseReg = baseRegField.Read(u);
                var am = Bitfield.ReadFields(amFields, u);
                d.addrMod = disp > 0
                    ? AddrRegMod.ma
                    : AddrRegMod.mb;
                d.ops.Add(MemoryOperand.Indirect(dt, disp, Registers.GpRegs[iBaseReg]));
                return true;
            };
        }

        // Table H-1 in PA-RISC 2.0 manual
        private static readonly AddrRegMod[] baseRegMods = new[]
        {
            AddrRegMod.None,
            AddrRegMod.ma,
            AddrRegMod.None,
            AddrRegMod.mb
        };

        // Table H-1 in PA-RISC 2.0 manual
        private static readonly AddrRegMod[] indexRegMods = new[]
        {
            AddrRegMod.None,
            AddrRegMod.m,       // Modify base register
            AddrRegMod.s,       // Pre-shift index register by data size
            AddrRegMod.sm,      // Pre-shift index register and modify base register.
        };

        // Table 7-7 in PA-RISC 2.0. manual
        private static readonly AddrRegMod[] stbyMods = new[]
        {
            AddrRegMod.None,
            AddrRegMod.b_m,     // Begin case, modify baseregister
            AddrRegMod.e,       // End case
            AddrRegMod.e_m,     // End case, modify base register.
        };


        private static readonly Bitfield[] amFields = BeFields((18, 1), (26, 1));

        /// <summary>
        /// Register indirect with short displacement with space register
        /// _OR_
        /// Register indexed indirect.
        /// </summary>
        private static Mutator<PaRiscDisassembler> Mshort(int dispFieldPos, PrimitiveType dt, AddrRegMod[] baseRegMods = null)
        {
            var dispField = BeField(dispFieldPos, 5);
            var baseRegField = BeField(6, 5);
            var spaceRegField = BeField(16, 2);
            baseRegMods = baseRegMods ?? PaRiscDisassembler.baseRegMods;
            return (u, d) =>
            {
                var iBaseReg = baseRegField.Read(u);
                var baseReg = Registers.GpRegs[iBaseReg];
                var iSpaceReg = spaceRegField.Read(u);
                var spaceReg = Registers.SpaceRegs[iSpaceReg];
                MemoryOperand mem;
                if (BeBitSet(u, 19))
                {
                    // Short displacement
                    var disp = low_sign_ext(dispField, u);
                    var am = Bitfield.ReadFields(amFields, u);
                    d.addrMod = (am == 1 && disp == 0)
                        ? AddrRegMod.o
                        : baseRegMods[am];
                    mem = MemoryOperand.Indirect(dt, disp, baseReg, spaceReg);
                }
                else
                {
                    // Index
                    var iIndexReg = dispField.Read(u);
                    var indexReg = Registers.GpRegs[iIndexReg];
                    var um = Bitfield.ReadFields(amFields, u);
                    d.addrMod = indexRegMods[um];
                    mem = MemoryOperand.Indexed(dt, baseReg, indexReg, spaceReg);
                }
                d.ops.Add(mem);
                return true;
            };
        }

        /// <summary>
        /// Register indirect with displacement with space register
        /// </summary>
        private static Mutator<PaRiscDisassembler> M(PrimitiveType dt, int dispPos, int dispLen, int baseRegPos, int spacePos)
        {
            var dispField = BeField(dispPos, dispLen);
            var baseRegField = BeField(baseRegPos, 5);
            var spaceRegField = BeField(spacePos, 2);
            return (u, d) =>
            {
                var disp = dispField.ReadSigned(u);
                var iBaseReg = baseRegField.Read(u);
                var iSpaceReg = spaceRegField.Read(u);
                var am = Bitfield.ReadFields(amFields, u);
                d.addrMod = (am == 1 && disp == 0)
                    ? AddrRegMod.o
                    : baseRegMods[am];
                d.ops.Add(MemoryOperand.Indirect(dt, disp, Registers.GpRegs[iBaseReg], Registers.SpaceRegs[iSpaceReg]));
                return true;
            };
        }

        /// <summary>
        /// Register indirect, with offset in multiple fields, shifted by the element size
        /// </summary>
        private static Mutator<PaRiscDisassembler> Msh(PrimitiveType dt, Bitfield[] fields, int baseRegPos, int spacePos)
        {
            var baseRegField = BeField(baseRegPos, 5);
            var spaceRegField = BeField(spacePos, 2);
            return (u, d) =>
            {
                var disp = Bitfield.ReadSignedFields(fields, u) * dt.Size;
                var iBaseReg = baseRegField.Read(u);
                var iSpaceReg = spaceRegField.Read(u);
                d.ops.Add(MemoryOperand.Indirect(dt, disp, Registers.GpRegs[iBaseReg], Registers.SpaceRegs[iSpaceReg]));
                return true;
            };
        }

        /// <summary>
        /// Register indirect indexed
        /// </summary>
        private static Mutator<PaRiscDisassembler> Mx(PrimitiveType dt, int baseRegPos, int idxRegPos)
        {
            var baseRegField = BeField(baseRegPos, 5);
            var idxRegField = BeField(idxRegPos, 5);
            return (u, d) =>
            {
                var iBaseReg = baseRegField.Read(u);
                var iIdxReg = idxRegField.Read(u);
                d.ops.Add(MemoryOperand.Indexed(dt, Registers.GpRegs[iBaseReg], Registers.GpRegs[iIdxReg]));
                return true;
            };
        }

        /// <summary>
        /// Register indirect indexed with address space
        /// </summary>
        private static Mutator<PaRiscDisassembler> Mx(PrimitiveType dt, int baseRegPos, int idxRegPos, int spaceRegPos)
        {
            var baseRegField = BeField(baseRegPos, 5);
            var idxRegField = BeField(idxRegPos, 5);
            var spaceRegField = BeField(spaceRegPos, 2);
            return (u, d) =>
            {
                var iBaseReg = baseRegField.Read(u);
                var iIdxReg = idxRegField.Read(u);
                var iSpaceRegField = spaceRegField.Read(u);
                var um = Bitfield.ReadFields(amFields, u);
                d.addrMod = indexRegMods[um];
                d.ops.Add(MemoryOperand.Indexed(dt, Registers.GpRegs[iBaseReg], Registers.GpRegs[iIdxReg], Registers.SpaceRegs[iSpaceRegField]));
                return true;
            };
        }

        /// <summary>
        /// PC-relative address.
        /// </summary>
        private static Mutator<PaRiscDisassembler> PcRel(Func<uint, Bitfield[],uint> permutator, Bitfield[] fields)
        {
            int totalWidth = fields.Sum(f => f.Length);
            return (u, d) =>
            {
                var offset = (int)Bits.SignExtend(permutator(u, fields), totalWidth) * 4 + 8;
                var addrDst = d.addr + offset;
                d.ops.Add(AddressOperand.Create(addrDst));
                return true;
            };
        }

        /// <summary>
        /// Annul bit.
        /// </summary>
        private static Mutator<PaRiscDisassembler> Annul(int bitPos)
        {
            return (u, d) =>
            {
                d.annul = Bits.IsBitSet(u, 31 - bitPos);
                return true;
            };
        }

        /// <summary>
        /// Co-processor selector.
        /// </summary>
        private static Mutator<PaRiscDisassembler> cop(int bitPos, int bitLength)
        {
            var field = BeField(bitPos, bitLength);
            return (u, d) =>
            {
                d.coprocessor = (int) field.Read(u);
                return true;
            };
        }

        /// <summary>
        /// Cache control hint completer.
        /// </summary>
        private static Mutator<PaRiscDisassembler> cc(int bitpos, CacheHint[] cacheHints)
        {
            var ccField = BeField(bitpos, 2);
            return (u, d) =>
            {
                var cc = ccField.Read(u);
                d.cacheHint = cacheHints[cc];
                return true;
            };
        }

        private static readonly Mutator<PaRiscDisassembler> cc_loads = cc(20, new[]
        {
            CacheHint.None,
            CacheHint.None, // Reserved treated as None
            CacheHint.sl,
            CacheHint.None, // Reserved treated as None
        });

        private static readonly Mutator<PaRiscDisassembler> cc_stores = cc(20, new[]
        {
            CacheHint.None,
            CacheHint.bc,
            CacheHint.sl,
            CacheHint.None, // Reserved treated as None
        });

        // Set the 'z' completer
        private static Mutator<PaRiscDisassembler> z(int bitPos)
        {
            var field = BeField(bitPos, 1);
            return (u, d) =>
            {
                d.zero = field.Read(u) == 0;
                return true;
            };
        }

        // Set the 'sign-extend' completer
        private static Mutator<PaRiscDisassembler> se(int bitPos)
        {
            var field = BeField(bitPos, 1);
            return (u, d) =>
            {
                d.signExtend = field.Read(u) == 0 
                    ? SignExtension.u
                    : SignExtension.s;
                return true;
            };
        }

        /// <summary>
        /// Set the 'fpFormat' completer
        /// </summary>
        private static Mutator<PaRiscDisassembler> fpFmt(Bitfield field, FpFormat[] fpFormats)
        {
            return (uint uInstr, PaRiscDisassembler dasm) =>
            {
                Decoder.DumpMaskedInstruction(uInstr, field.Mask << field.Position, "FP format completer");
                var uFormat = field.Read(uInstr);
                dasm.fpFormat = fpFormats[uFormat];
                return dasm.fpFormat != FpFormat.None;
            };
        }

        /// <summary>
        /// Set the 'fpFormatDst' completer
        /// </summary>
        private static Mutator<PaRiscDisassembler> fpFmtD(Bitfield field, FpFormat[] fpFormats)
        {
            return (uint uInstr, PaRiscDisassembler dasm) =>
            {
                Decoder.DumpMaskedInstruction(uInstr, field.Mask << field.Position, "FP dst format completer");
                var uFormat = field.Read(uInstr);
                dasm.fpFormatDst = fpFormats[uFormat];
                return dasm.fpFormatDst != FpFormat.None;
            };
        }
        private static readonly Mutator<PaRiscDisassembler> fpFmt2 = fpFmt(BeField(19, 2), new[] {
            FpFormat.sgl,
            FpFormat.dbl,
            FpFormat.None,
            FpFormat.quad
        });
        private static readonly Mutator<PaRiscDisassembler> fpFmt1 = fpFmt(BeField(26, 1), new[] {
            FpFormat.dbl,
            FpFormat.sgl,
        });
        private static readonly Mutator<PaRiscDisassembler> fpFmt20_1 = fpFmt(BeField(20, 1), new[] {
            FpFormat.dbl,
            FpFormat.sgl,
        });

        /// <summary>
        /// Convert floats
        /// </summary>
        private static readonly FpFormat[] cvf = new[]
        {
            FpFormat.sgl,
            FpFormat.dbl,
            FpFormat.None,
            FpFormat.quad
        };

        /// <summary>
        /// Convert signed ints
        /// </summary>
        private static readonly FpFormat[] cvx = new[]
        {
            FpFormat.w,
            FpFormat.dw,
            FpFormat.None,
            FpFormat.qw
        };

        /// <summary>
        /// Convert unsigned ints
        /// </summary>
        private static readonly FpFormat[] cvu = new[]
        {
            FpFormat.uw,
            FpFormat.udw,
            FpFormat.None,
            FpFormat.uqw
        };

        private static readonly Mutator<PaRiscDisassembler> cvf_s = fpFmt(BeField(19, 2), cvf);
        private static readonly Mutator<PaRiscDisassembler> cvf_d = fpFmtD(BeField(17, 2), cvf);
        private static readonly Mutator<PaRiscDisassembler> cvx_s = fpFmt(BeField(19, 2), cvx);
        private static readonly Mutator<PaRiscDisassembler> cvx_d = fpFmtD(BeField(17, 2), cvx);
        private static readonly Mutator<PaRiscDisassembler> cvu_s = fpFmt(BeField(19, 2), cvu);
        private static readonly Mutator<PaRiscDisassembler> cvu_d = fpFmtD(BeField(17, 2), cvu);

        private static bool Eq0(uint u) => u == 0;
        private static bool IsFpuProcessor(uint u) => (u & ~1) == 0;

        // Assembles a 6-bit extract/deposit length specifier:
        private static uint assemble_6(uint u, Bitfield[] fields)
        {
            var x = fields[0].Read(u);
            var y = fields[1].Read(u);
            return 32 * x + (32 - y);
        }

        private static uint assemble_12(uint u, Bitfield[] fields)
        {
            // return(cat(y,x{10},x{0..9}))
            var x = fields[0].Read(u);
            var y = fields[1].Read(u);
            var p = y;
            p = (p << fields[0].Length) 
                | ((x << 10) & 0b1_00000_00000)
                | ((x >> 1) & 0b0_11111_11111);
            return p;
        }

        private static uint assemble_16(bool is64bit, uint u, Bitfield[] fields)
        {
            var x = fields[0].Read(u);
            var y = fields[1].Read(u);
            uint y_13 = y & 1;
            if (is64bit)
            {
                // return(cat(y{13},xor(y{13},x{0}),xor(y{13},x{1}),y{0..12}))

                uint x_0 = (x >> 1) & 1;
                uint x_1 = x & 1;
                uint p = y_13;
                p = (p << 1) | (y_13 ^ x_0);
                p = (p << 1) | (y_13 ^ x_1);
                p = (p << 13) | (y >> 1);
                return p;
            }
            else
            {
                uint p = ((8 - y_13) & 7) << 13;
                p = p | (y >> 1);
                return Bits.SignExtend(p, 16);
            }
        }

        private static uint assemble_16a(bool is64Bit, uint u, Bitfield[] fields)
        {
            var x = fields[0].Read(u);
            var y = fields[1].Read(u);
            var z = fields[2].Read(u);
            var p = ((8 - z) & 7) << 13;
            if (is64Bit)
            {
                // return(cat(z,xor(z,x{0}),xor(z,x{1}),y,0{0..1}))
                p = p ^ (x << 13);
            }
            p = p | (y << 2);
            return p;
        }

        private static uint assemble_17(uint u, Bitfield[] fields)
        {
            var x = fields[0].Read(u);
            var y = fields[1].Read(u);
            var z = fields[2].Read(u);
            var p = z;
            p = (p << fields[0].Length) | x;
            p = (p << fields[1].Length)
                | ((y << 10) & 0b1_00000_00000)
                | ((y >> 1)  & 0b0_11111_11111);
            return p;
        }

        private static Bitfield[] asm21_fields =
        {
            BeField(11+20, 1),
            BeField(11+9, 11),
            BeField(11+5, 2),
            BeField(11+0, 5),
            BeField(11+7, 2)
        };

        private static uint assemble_21(uint u, Bitfield[] fields)
        {
            var x = fields[0].Read(u);
            var p = Bitfield.ReadFields(asm21_fields, x);
            return p;
        }

        private static uint assemble_22(uint u, Bitfield[] fields)
        {
            var a = fields[0].Read(u);
            var b = fields[1].Read(u);
            var c = fields[2].Read(u);
            var d = fields[3].Read(u);
            var p = d;
            p = (p << fields[0].Length) | a;
            p = (p << fields[1].Length) | b;
            p = (p << fields[2].Length)
                | ((c << 10) & 0b1_00000_00000)
                | ((c >> 1) & 0b0_11111_11111);
            return p;
        }

        private static int low_sign_ext(Bitfield field, uint u)
        {
            var x = field.Read(u);
            var x_lsb = x & 1;
            var p = (x >> 1) | (x << (field.Length- 1));
            return (int)Bits.SignExtend(p, field.Length);
        }

        private static Func<bool, uint, Bitfield[], uint> low_sign_ext(int width)
        {
            return (is64bit, u, fields) =>
            {
                var x = fields[0].Read(u);
                var x_lsb = x & 1;
                var p = (x >> 1) | (x << (width - 1));
                return Bits.SignExtend(p, width);
            };
        }
        private static readonly Func<bool, uint, Bitfield[], uint> low_sign_ext5 = low_sign_ext(5);
        private static readonly Func<bool, uint, Bitfield[], uint> low_sign_ext11 = low_sign_ext(11);

        private class MaskDecoder : Decoder
        {
            private readonly Bitfield bitfield;
            private readonly Decoder[] decoders;

            public MaskDecoder(Bitfield bitfield, Decoder [] decoders)
            {
                this.bitfield = bitfield;
                this.decoders = decoders;
            }

            public override PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm)
            {
                DumpMaskedInstruction(uInstr, bitfield.Mask << bitfield.Position, "");
                uint code = bitfield.Read(uInstr);
                return decoders[code].Decode(uInstr, dasm);
            }
        }

        private class ConditionalDecoder : Decoder
        {
            private readonly Bitfield field;
            private readonly Predicate<uint> predicate;
            private readonly Decoder trueDecoder;
            private readonly Decoder falseDecoder;

            public ConditionalDecoder(int bitPos, int bitLength, Predicate<uint> predicate, Decoder trueDecoder, Decoder falseDecoder)
            {
                this.field = BeField(bitPos, bitLength);
                this.predicate = predicate;
                this.trueDecoder = trueDecoder;
                this.falseDecoder = falseDecoder;
            }

            public override PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm)
            {
                DumpMaskedInstruction(uInstr, field.Mask << field.Position, "");
                var u = field.Read(uInstr);
                var cond = predicate(u);
                var decoder = cond ? trueDecoder : falseDecoder;
                return decoder.Decode(uInstr, dasm);
            }
        }

        private class NyiDecoder : Decoder
        {
            private readonly Mnemonic mnemonic;
            private readonly string message;

            public NyiDecoder(Mnemonic mnemonic, string message)
            {
                this.mnemonic = mnemonic;
                this.message = !string.IsNullOrEmpty(message) ? message : mnemonic.ToString();
            }

            public override PaRiscInstruction Decode(uint uInstr, PaRiscDisassembler dasm)
            {
                dasm.EmitUnitTest(
                    dasm.arch.Name,
                    uInstr.ToString("X8"),
                    message,
                    "PaRiscDis",
                    dasm.addr,
                    Console =>
                    {
                        Console.WriteLine($"    AssertCode(\"@@@\", 0x{uInstr:X8});");
                        Console.WriteLine();
                    });
                return new PaRiscInstruction
                {
                    InstructionClass = 0,
                    Coprocessor = -1,
                    Mnemonic = mnemonic,
                    Operands = new MachineOperand[0]
                };
            }

            [Conditional("DEBUG")]
            private void EmitUnitTest(uint wInstr)
            {
                Console.WriteLine("    // Reko: a decoder for hppa instruction {0} at address(.*?) has not");
                Console.WriteLine("    // {0}", message);
                Console.WriteLine("    [Test]");
                Console.WriteLine("    public void PaRiscDis_{0:X8}()", wInstr);
                Console.WriteLine("    {");
                Console.WriteLine("        AssertCode(\"@@@\", \"{0:X8}\");", wInstr);
                Console.WriteLine("    }");
                Console.WriteLine("");
            }
        }


        private static Decoder Instr(Mnemonic opcode, params Mutator<PaRiscDisassembler> [] mutators)
        {
            return new InstrDecoder<PaRiscDisassembler,Mnemonic,PaRiscInstruction>(InstrClass.Linear, opcode, mutators);
        }

        private static Decoder Instr(Mnemonic opcode, InstrClass iclass, params Mutator<PaRiscDisassembler>[] mutators)
        {
            return new InstrDecoder<PaRiscDisassembler, Mnemonic, PaRiscInstruction>(iclass, opcode, mutators);
        }

        // Note: the bit positions are big-endian to follow the PA-RISC manual.
        private static MaskDecoder Mask(
            int bitPos,
            int bitLength,
            params Decoder[] decoders)
        {
            Debug.Assert(1 << bitLength == decoders.Length, $"Expected {1 << bitLength} decoders but saw {decoders.Length}");
            return new MaskDecoder(BeField(bitPos, bitLength), decoders);
        }

        private static MaskDecoder Mask(
            int bitPos,
            int bitLength,
            Decoder defaultDecoder,
            params (int, Decoder)[] sparseDecoders)
        {
            var decoders = Enumerable.Range(0, 1 << bitLength).Select(n => defaultDecoder).ToArray();
            foreach (var (i, d) in sparseDecoders)
            {
                decoders[i] = d;
            }
            return new MaskDecoder(BeField(bitPos, bitLength), decoders);
        }

        private static ConditionalDecoder Cond(
            int bitPos,
            int bitLength,
            Predicate<uint> predicate,
            Decoder trueDecoder,
            Decoder falseDecoder)
        {
            return new ConditionalDecoder(bitPos, bitLength, predicate, trueDecoder, falseDecoder);
        }

        private static NyiDecoder Nyi(string message)
        {
            return new NyiDecoder(Mnemonic.invalid, message);
        }

        private static NyiDecoder Nyi(Mnemonic opcode, string message)
        {
            return new NyiDecoder(opcode, message);
        }


        static PaRiscDisassembler()
        {
            invalid = Instr(Mnemonic.invalid, InstrClass.Invalid);

            var systemOp = Mask(19, 8, invalid,
                (0x00, Instr(Mnemonic.@break, InstrClass.Call|InstrClass.Transfer, u8(27,5), u16(6,13))),
                (0x20, Nyi(Mnemonic.sync, "")),
                (0x20, Nyi(Mnemonic.syncdma, "")),
                (0x60, Instr(Mnemonic.rfi, InstrClass.System | InstrClass.Transfer)),
                (0x65, Instr(Mnemonic.rfi_r, InstrClass.System | InstrClass.Transfer)),
                (0x6B, Nyi(Mnemonic.ssm, "")),
                (0x73, Nyi(Mnemonic.rsm, "")),
                (0xC3, Instr(Mnemonic.mtsm, InstrClass.System|InstrClass.Transfer, r11)),
                (0x85, Cond(16,2, Eq0,
                    Instr(Mnemonic.ldsid, r6,r27),
                    Instr(Mnemonic.ldsid, sr(16),r27))),
                (0xC1, Instr(Mnemonic.mtsp, r(11,5),sr(16))),
                (0x25, Nyi(Mnemonic.mfsp, "")),
                (0xC2, Instr(Mnemonic.mtctl, r11, cr(6,5))),
                (0x45, Mask(17, 1, 
                    Instr(Mnemonic.mfctl, cr(6,5), r27),
                    Instr(Mnemonic.mfctl_w, cr(6,5), r27))));

            var memMgmt = Mask(19, 1,
                Mask(19, 7, invalid,
                    (0x20, Nyi(Mnemonic.iitlbt, "")),
                    (0x08, Nyi(Mnemonic.pitlb, "")),
                    (0x09, Nyi(Mnemonic.pitlbe, "")),
                    (0x18, Nyi(Mnemonic.pitlb_l, "")),
                    (0x0A, Nyi(Mnemonic.fic_0a, "")),
                    (0x0B, Nyi(Mnemonic.fice, ""))),
                Mask(18, 8, invalid,
                    (0x60, Nyi(Mnemonic.idtlbt, "")),
                    (0x49, Nyi(Mnemonic.pdtlbe, "")),
                    (0x48, Instr(Mnemonic.pdtlb, Mx(PrimitiveType.Word32, 6, 11, 16))),
                    (0x58, Instr(Mnemonic.pdtlb_l, Mx(PrimitiveType.Word32, 6, 11, 16))),
                    (0x4A, Nyi(Mnemonic.fdc, "(index)")),
                    (0xCA, Nyi(Mnemonic.fdc, "(imm)")),
                    (0x4B, Nyi(Mnemonic.fdce, "")),
                    (0x4E, Nyi(Mnemonic.pdc, "")),
                    (0x4F, Nyi(Mnemonic.fic, "")),
                    (0x46, Nyi(Mnemonic.probe, "")),
                    (0xC6, Nyi(Mnemonic.probei, "")),
                    (0x47, Nyi(Mnemonic.probe, "")),
                    (0xC7, Nyi(Mnemonic.probei, "")),
                    (0x4D, Nyi(Mnemonic.lpa, "")),
                    (0x4C, Nyi(Mnemonic.lci, ""))));


            var arithLog = Mask(20, 6, invalid,
                (0x00, Instr(Mnemonic.andcm, cf_log(), r11, r6, r27)),

                (0x04, Instr(Mnemonic.hsub_us, r11, r6, r27)),
                (0x05, Instr(Mnemonic.hsub_ss, r11, r6, r27)),
                (0x07, Instr(Mnemonic.hsub, r11, r6, r27)),

                (0x08, Instr(Mnemonic.and, cf_log(), r11, r6, r27)),
                (0x09, Instr(Mnemonic.or, cf_log(), r11, r6, r27)),
                (0x0A, Instr(Mnemonic.xor, cf_log(), r11, r6, r27)),
                (0x0B, Instr(Mnemonic.havg, r11, r6, r27)),

                (0x0C, Instr(Mnemonic.hadd_us, r11, r6, r27)),
                (0x0D, Instr(Mnemonic.hadd_ss, r11, r6, r27)),
                (0x0E, Instr(Mnemonic.uxor, cf_log(), r11, r6, r27)),
                (0x0F, Instr(Mnemonic.hadd, r11, r6, r27)),

                (0x10, Instr(Mnemonic.sub, cf_log(), r11, r6, r27)),
                (0x11, Instr(Mnemonic.ds, r11, r6, r27)),
                (0x13, Instr(Mnemonic.sub_tc, cf_cmpsub(), r11, r6, r27)),

                (0x14, Instr(Mnemonic.sub_b, cf_cmpsub(), r11, r6, r27)),
                (0x15, Instr(Mnemonic.hshradd, r11, u(24, 2, PrimitiveType.Int32), r6, r27)),
                (0x16, Instr(Mnemonic.hshradd, r11, u(24, 2, PrimitiveType.Int32), r6, r27)),
                (0x17, Instr(Mnemonic.hshradd, r11, u(24, 2, PrimitiveType.Int32), r6, r27)),

                (0x18, Instr(Mnemonic.add, cf_add(), r11, r6, r27)),
                (0x19, Instr(Mnemonic.shladd, r11, u(24, 2, PrimitiveType.Byte), r6, r27)),
                (0x1A, Instr(Mnemonic.shladd, r11, u(24, 2, PrimitiveType.Byte), r6, r27)),
                (0x1B, Instr(Mnemonic.shladd, r11, u(24, 2, PrimitiveType.Byte), r6, r27)),

                (0x1C, Instr(Mnemonic.add_c, cf_add(), r11, r6, r27)),
                (0x1D, Instr(Mnemonic.hshladd, r11, u(24, 2, PrimitiveType.Int32), r6, r27)),
                (0x1E, Instr(Mnemonic.hshladd, r11, u(24, 2, PrimitiveType.Int32), r6, r27)),
                (0x1F, Instr(Mnemonic.hshladd, r11, u(24, 2, PrimitiveType.Int32), r6, r27)),

                (0x22, Instr(Mnemonic.cmpclr, cf_cmpsub(), r11, r6, r27)),

                (0x26, Instr(Mnemonic.uaddcm, r11, r6, r27)),
                (0x27, Instr(Mnemonic.uaddcm_tc, r11, r6, r27)),

                (0x28, Instr(Mnemonic.add_l, cf_add(), r11, r6, r27)),
                (0x29, Instr(Mnemonic.shladd, r11, u(24, 2, PrimitiveType.Byte), r6, r27)),
                (0x2A, Instr(Mnemonic.shladd, r11, u(24, 2, PrimitiveType.Byte), r6, r27)),
                (0x2B, Instr(Mnemonic.shladd, r11, u(24, 2, PrimitiveType.Byte), r6, r27)),

                (0x2E, Instr(Mnemonic.dcor)),
                (0x2F, Instr(Mnemonic.dcor_i)),

                (0x30, Instr(Mnemonic.sub_tsv, cf_cmpsub(), r11, r6, r27)),
                (0x33, Instr(Mnemonic.sub_tsv_tc, cf_cmpsub(), r11, r6, r27)),

                (0x34, Instr(Mnemonic.sub_b_tsv, cf_cmpsub(), r11, r6, r27)),

                (0x38, Instr(Mnemonic.add_tsv, cf_add(), r11, r6, r27)),
                (0x39, Instr(Mnemonic.shladd_tsv, r11, u(24, 2, PrimitiveType.Byte), r6, r27)),
                (0x3A, Instr(Mnemonic.shladd_tsv, r11, u(24, 2, PrimitiveType.Byte), r6, r27)),
                (0x3B, Instr(Mnemonic.shladd_tsv, r11, u(24, 2, PrimitiveType.Byte), r6, r27)),

                (0x3C, Instr(Mnemonic.add_c_tsv, cf_add(), r11, r6, r27)));

            var indexMem = Mask(19, 1,  // opc=3
                Mask(22, 4, invalid,
                    (0x0, Instr(Mnemonic.ldb, Mx(PrimitiveType.Byte, 6, 11, 16), r27)),
                    (0x1, Instr(Mnemonic.ldh, Mx(PrimitiveType.Word16, 6, 11, 16), r27)),
                    (0x2, Instr(Mnemonic.ldw, Mx(PrimitiveType.Word32, 6, 11, 16), r27)),
                    (0x3, Instr(Mnemonic.ldd, Mx(PrimitiveType.Word32, 6, 11, 16), r27)),
                    (0x4, Instr(Mnemonic.ldda, Mx(PrimitiveType.Word32, 6, 11, 16), r27)),
                    (0x5, Instr(Mnemonic.ldcd, Mx(PrimitiveType.Word32, 6, 11, 16), r27)),
                    (0x6, Instr(Mnemonic.ldwa, Mx(PrimitiveType.Word32, 6, 11, 16), r27)),
                    (0x7, Instr(Mnemonic.ldcw, Mx(PrimitiveType.Word32, 6, 11, 16), r27))),
                Mask(22, 4, invalid,
                    (0x0, Instr(Mnemonic.ldb, cc_loads, Mshort(11, PrimitiveType.Byte), r27)),
                    (0x1, Instr(Mnemonic.ldh, cc_loads, Mshort(11, PrimitiveType.Word16), r27)),
                    (0x2, Instr(Mnemonic.ldw, cc_loads, Mshort(11, PrimitiveType.Word32), r27)),
                    (0x3, Instr(Mnemonic.ldd, cc_loads, Mshort(11, PrimitiveType.Word64), r27)),
                    (0x4, Instr(Mnemonic.ldda, cc_loads, Mshort(11, PrimitiveType.Word64), r27)),
                    (0x5, Instr(Mnemonic.ldcd, cc_loads, Mshort(11, PrimitiveType.Word64), r27)),
                    (0x6, Instr(Mnemonic.ldwa, cc_loads, Mshort(11, PrimitiveType.Word64), r27)),
                    (0x7, Instr(Mnemonic.ldcw, cc_loads, Mshort(11, PrimitiveType.Word64), r27)),
                    (0x8, Instr(Mnemonic.stb, cc_stores, r11, Mshort(27, PrimitiveType.Byte))),
                    (0x9, Instr(Mnemonic.sth, cc_stores, r11, Mshort(27, PrimitiveType.Word16))),
                    (0xA, Instr(Mnemonic.stw, cc_stores, r11, Mshort(27, PrimitiveType.Word32))),
                    (0xB, Instr(Mnemonic.std, cc_stores, r11, Mshort(27, PrimitiveType.Word64))),
                    (0xC, Instr(Mnemonic.stby, cc_stores, r11, Mshort(27, PrimitiveType.Byte, stbyMods))),
                    (0xD, Instr(Mnemonic.stdby, cc_stores, r11, Mshort(27, PrimitiveType.Byte, stbyMods))),
                    (0xE, Instr(Mnemonic.stwa, cc_stores, r11, Mshort(27, PrimitiveType.Word32))),
                    (0xF, Instr(Mnemonic.stda, cc_stores, r11, Mshort(27, PrimitiveType.Word64)))));

            var spopN = Mask(21, 2,
                Instr(Mnemonic.spop0, u(23, 3, PrimitiveType.UInt32), u(27, 5, PrimitiveType.UInt32), Annul(26)),
                Instr(Mnemonic.spop1),
                Instr(Mnemonic.spop2),
                Instr(Mnemonic.spop3));

            var coprW = Cond(23, 3, IsFpuProcessor,
                Mask(22, 1,
                    Instr(Mnemonic.fldw, cc_loads, Mshort(11, PrimitiveType.Real32), fr25_27),
                    Instr(Mnemonic.fstw, cc_stores, fr25_27, Mshort(11, PrimitiveType.Real32))),
                Mask(22, 1,
                    Instr(Mnemonic.cldw, cc_loads, cop(23, 3), Mshort(11, PrimitiveType.Word32), r27),
                    Instr(Mnemonic.cstw, cc_stores, cop(23, 3), r27, Mshort(11, PrimitiveType.Word32))));

            var coprDW = Cond(23, 3, IsFpuProcessor,
                Mask(22, 1,
                    Instr(Mnemonic.fldd, cc_loads, Mshort(11, PrimitiveType.Real64), fr27),
                    Instr(Mnemonic.fstd, cc_stores, fr27, Mshort(11, PrimitiveType.Real64))),
                Mask(22, 1,
                    Instr(Mnemonic.cldd, cc_loads,cop(23, 3), Mshort(11, PrimitiveType.Word64), r27),
                    Instr(Mnemonic.cstd, cc_stores, cop(23, 3), r27, Mshort(11, PrimitiveType.Word64))));

            var copr = Mask(21, 2,
                Mask(16, 3,
                    Cond(6, 26, Eq0,
                        Instr(Mnemonic.fid),
                        invalid),
                    invalid,
                    Instr(Mnemonic.fcpy, fpFmt2, fr6, fr27),
                    Instr(Mnemonic.fabs, fpFmt2, fr6, fr27),

                    Instr(Mnemonic.fsqrt, fpFmt2, fr6, fr27),
                    Nyi("frnd"),
                    Instr(Mnemonic.fneg, fpFmt2, fr6, fr27),
                    Instr(Mnemonic.fnegabs, fpFmt2, fr6, fr27)),
                Mask(14, 3,
                    Instr(Mnemonic.fcnvff, cvf_s,cvf_d,fr6,fr27),
                    Instr(Mnemonic.fcnvxf, cvx_s,cvf_d,fr6,fr27),
                    Instr(Mnemonic.fcnvfx, cvf_s,cvx_d,fr6,fr27),
                    Instr(Mnemonic.fcnvfxt, cvf_s,cvx_d,fr6,fr27),

                    invalid,
                    Instr(Mnemonic.fcnv, cvu_s,cvf_d,fr6,fr27),
                    Instr(Mnemonic.fcnv, cvf_s,cvu_d,fr6,fr27),
                    Instr(Mnemonic.fcnv_t, cvf_s,cvu_d,fr6,fr27)),
                Nyi("FP 0C two"),
                Mask(16, 3,
                    Instr(Mnemonic.fadd, fpFmt2, fr6, fr11, fr27),
                    Instr(Mnemonic.fsub, fpFmt2, fr6, fr11, fr27),
                    Instr(Mnemonic.fmpy, fpFmt2, fr6, fr11, fr27),
                    Instr(Mnemonic.fdiv, fpFmt2, fr6, fr11, fr27),

                    invalid,
                    invalid,
                    invalid,
                    invalid));
            var float0EDecoder = Mask(21, 2,
                Mask(16, 3,
                    invalid,
                    invalid,
                    Instr(Mnemonic.fcpy, fpFmt20_1, fr6_24, fr25_27),
                    Nyi(Mnemonic.fabs, ""),

                    Nyi(Mnemonic.fsqrt, ""),
                    Nyi(Mnemonic.frnd, ""),
                    Nyi(Mnemonic.fneg, ""),
                    Nyi(Mnemonic.fnegabs, "")),
                Mask(14, 3,
                    Instr(Mnemonic.fcnv, cvf_s, cvf_d, fr6_20, fr27_18),
                    Instr(Mnemonic.fcnv, cvx_s, cvf_d, fr6_20, fr27_18),
                    Instr(Mnemonic.fcnv, cvf_s, cvx_d, fr6_20, fr27_18),
                    Instr(Mnemonic.fcnv_t, cvf_s, cvx_d, fr6_20, fr27_18),

                    invalid,
                    Instr(Mnemonic.fcnv, cvu_s, cvf_d, fr6_20, fr27_18),
                    Instr(Mnemonic.fcnv, cvf_s, cvu_d, fr6_20, fr27_18),
                    Instr(Mnemonic.fcnv_t, cvf_s, cvu_d, fr6_20, fr27_18)),
                Instr(Mnemonic.fcmp, cf27_fp, fr25_27, fr11_19),
                Mask(23, 1,
                    Mask(16, 3,
                        Instr(Mnemonic.fadd, fpFmt20_1),
                        Instr(Mnemonic.fsub, fpFmt20_1),
                        Instr(Mnemonic.fmpy, fpFmt20_1, fr24_6, fr24_11, fr25_27),
                        Instr(Mnemonic.fdiv, fpFmt20_1),
                        invalid,
                        invalid,
                        invalid,
                        invalid),
                    Instr(Mnemonic.xmpyu, fr6_25, fr11_19, fr27)));
            var productSpecific = Nyi("productSpecific");
            var subi = Mask(20, 1,
                Instr(Mnemonic.subi, cf16_cmpsub_32, s(PrimitiveType.Int32, BeFields((21, 11)), low_sign_ext11), r6, r11),
                Instr(Mnemonic.subi_tsv, cf16_cmpsub_32, s(PrimitiveType.Int32, BeFields((21, 11)), low_sign_ext11), r6, r11));
            var extract_34 = Mask(19, 3,
                Instr(Mnemonic.shrpw, cf16_sh_ex_dp_3, r11, r6, r27),
                Cond(23, 4, Eq0,
                    Instr(Mnemonic.shrpd, cf16_sh_ex_dp_3, r11, r6, reg(Registers.SAR), r27),
                    Instr(Mnemonic.shrpd, cf16_sh_ex_dp_3, r11, r6, shamt(BeFields((20,1),(22,5))), r27)),
                Instr(Mnemonic.shrpw, cf16_sh_ex_dp_3, r11, r6, r27),
                Instr(Mnemonic.shrpd, cf16_sh_ex_dp_3, r11, r6, r27),

                Nyi("extract-100"),
                Nyi("extract-101"),
                Instr(Mnemonic.extrw, cf16_shext, se(21), r6, u(22, 5,PrimitiveType.Byte), u8From32(27, 5), r11),
                Instr(Mnemonic.extrw, cf16_shext, se(21), r6, u(22, 5,PrimitiveType.Byte), u8From32(27, 5), r11));
            var deposit_35 = Mask(19, 2,
                Instr(Mnemonic.depw, cf16_shext, z(21), r11, u8From31(22, 5), u(BeFields((0, 0), (27, 5)), assemble_6), r6),
                Instr(Mnemonic.depw, cf16_shext, z(21), r11, u8From31(22, 5), u(BeFields((0, 0), (27, 5)), assemble_6), r6),
                Nyi("depwi-var"),
                Instr(Mnemonic.depwi, cf16_shext, z(21), lse(11, 5), u8From31(22, 5), u(BeFields((0, 0), (27, 5)), assemble_6), r6));
            var extract_3C = Nyi("extract_3C");
            var deposit_3D = Nyi("deposit_3D");
            var multimedia = Mask(16, 1,
                Nyi(Mnemonic.permh, ""),
                Mask(17, 2,
                    Mask(20, 2,
                        Nyi(Mnemonic.mixw_l, ""),
                        Nyi(Mnemonic.mixh_l, ""),
                        Nyi(Mnemonic.hshl, ""),
                        invalid),
                    invalid,
                    Mask(20, 2,
                        Nyi(Mnemonic.mixw_r, ""),
                        Nyi(Mnemonic.mixh_r, ""),
                        Nyi(Mnemonic.hshr_u, ""),
                        Nyi(Mnemonic.hshr_s, "")),
                    invalid));

            var branch = Mask(16, 3,
                Instr(Mnemonic.b_l, PcRel(assemble_17, BeFields((11,5),(19,11),(31,1))),r6, Annul(30)),
                Nyi(Mnemonic.b_gate, ""),
                Nyi(Mnemonic.blr, ""),
                invalid,

                Instr(Mnemonic.b_l_push, r11,r6,Annul(30)),
                Instr(Mnemonic.b_l, PcRel(assemble_22, BeFields((6,5),(11,5),(19,11),(31,1))),reg(Registers.GpRegs[2]), Annul(30)),
                Mask(19, 1,
                    Instr(Mnemonic.bv, Mx(PrimitiveType.Ptr32, 6, 11), Annul(30)),
                    Instr(Mnemonic.bv, Mx(PrimitiveType.Ptr32, 6, 11), Annul(30))),
                Nyi(Mnemonic.bve_l, ""));

            rootDecoder = Mask(0, 6,
                systemOp,
                memMgmt,
                arithLog,
                indexMem,

                spopN,
                Instr(Mnemonic.diag, InstrClass.Linear|InstrClass.System, u(6, 26, PrimitiveType.Int32)),
                Instr(Mnemonic.fmpyadd, fpFmt1, fmo6,fmo11,fmo27,fmo21,fmo16),
                invalid,

                Instr(Mnemonic.ldil, u(11, 21, PrimitiveType.Word32), r6),
                coprW,
                Instr(Mnemonic.addil, Left(BeFields((11,21)), assemble_21, 11), r6,reg(Registers.GpRegs[1])),
                coprDW,

                copr,
                Instr(Mnemonic.ldo, M(PrimitiveType.Word32, 6, BeFields((16,2),(18,14)), assemble_16),r11),
                float0EDecoder,
                productSpecific,

                // 10
                Instr(Mnemonic.ldb, M(PrimitiveType.Byte, 6, BeFields((16, 2), (18, 14)), assemble_16), r11),
                Instr(Mnemonic.ldh, M(PrimitiveType.Word16, 6, BeFields((16, 2), (18, 14)), assemble_16), r11),
                Instr(Mnemonic.ldw, M(PrimitiveType.Word32, 6, BeFields((16, 2), (18, 14)), assemble_16), r11),
                Instr(Mnemonic.ldw, Mam(PrimitiveType.Word32, 6, BeFields((16,2), (18,11), (31,1)), assemble_16a), r11),

                invalid,
                invalid,
                invalid,
                invalid,

                Instr(Mnemonic.stb, r11,M(PrimitiveType.Byte, 6, BeFields((16,2), (18,14)),assemble_16)),
                Instr(Mnemonic.sth, r11,M(PrimitiveType.Word16, 6, BeFields((16,2), (18,14)),assemble_16)),
                Instr(Mnemonic.stw, r11,M(PrimitiveType.Word32, 6, BeFields((16, 2), (18, 14)), assemble_16)),
                Instr(Mnemonic.stw, r11,Mam(PrimitiveType.Word32, 6, BeFields((16, 2), (18, 11), (31,1)), assemble_16a)),

                invalid,
                invalid,
                invalid,
                invalid,

                // 20
                Instr(Mnemonic.cmpb,  CTD, cf16_cmp32_t,r11,r6,PcRel(assemble_12, BeFields((19,11),(31,1))), Annul(30)),
                Instr(Mnemonic.cmpib, CTD, cf16_cmp32_t,s(11,5,PrimitiveType.Word32),r6,PcRel(assemble_12, BeFields((19, 11), (31, 1))), Annul(30)),
                Instr(Mnemonic.cmpb,  CTD, cf16_cmp32_f,r11,r6,PcRel(assemble_12, BeFields((19, 11), (31, 1))), Annul(30)),
                Instr(Mnemonic.cmpib, CTD, cf16_cmp32_f,s(11,5,PrimitiveType.Word32),r6,PcRel(assemble_12, BeFields((19, 11), (31, 1))), Annul(30)),

                Mask(19, 2, 
                    Instr(Mnemonic.cmpiclr, InstrClass.Linear|InstrClass.Annul, cf16_cmp32_t, lse(20, 11), r6, r11),
                    Instr(Mnemonic.cmpiclr, InstrClass.Linear|InstrClass.Annul, cf16_cmp64_t, lse(20, 11), r6, r11),
                    Instr(Mnemonic.cmpiclr, InstrClass.Linear|InstrClass.Annul, cf16_cmp32_f, lse(20, 11), r6, r11),
                    Instr(Mnemonic.cmpiclr, InstrClass.Linear|InstrClass.Annul, cf16_cmp64_f, lse(20, 11), r6, r11)),
                subi,
                Instr(Mnemonic.fmpysub, fpFmt1, fmo6,fmo11,fmo27,fmo21,fmo16),
                invalid,

                Instr(Mnemonic.addb, CTD, cf16_add_3, r11, r6,PcRel(assemble_12, BeFields((19,11),(31,1))), Annul(30)),
                Instr(Mnemonic.addib, CTD, cfadd_bitsize, lse(11,5),r6,PcRel(assemble_12, BeFields((19,11),(31,1))),Annul(30)),
                Instr(Mnemonic.addb, CTD, cf16_add_3_neg, r11, r6, PcRel(assemble_12, BeFields((19, 11), (31, 1))), Annul(30)),
                Instr(Mnemonic.addib, CTD, cfadd_bitsize_neg, lse(11,5),r6,PcRel(assemble_12, BeFields((19,11),(31,1))),Annul(30)),

                Mask(20, 1,
                    Instr(Mnemonic.addi_tc, cf16_add_32, s(PrimitiveType.Int32, BeFields((21, 11)), low_sign_ext11), r6, r11),
                    Instr(Mnemonic.addi_tsv_tc, cf16_add_32, s(PrimitiveType.Int32, BeFields((21, 11)), low_sign_ext11), r6, r11)),
                Mask(20, 1,
                    Instr(Mnemonic.addi, cf16_add_32, s(PrimitiveType.Int32, BeFields((21, 11)), low_sign_ext11), r6, r11),
                    Instr(Mnemonic.addi_tsv, cf16_add_32, s(PrimitiveType.Int32, BeFields((21, 11)), low_sign_ext11), r6, r11)),
                invalid,
                invalid,

                // 30
                Nyi(Mnemonic.bvb, ""),
                Instr(Mnemonic.bb, CTD, cf16_bb_1, r11, bb_bitpos(), PcRel(assemble_12, BeFields((19, 11), (31, 1))), Annul(30)),
                Instr(Mnemonic.movb, CTD, cf16_shext, r11,r6,PcRel(assemble_12, BeFields((19, 11), (31, 1))), Annul(30)),
                Instr(Mnemonic.movib, CTD, cf16_shext, lse(11,5),r6,PcRel(assemble_12, BeFields((19, 11), (31, 1))), Annul(30)),

                extract_34,
                deposit_35,
                invalid,
                invalid,

                Instr(Mnemonic.be,  TD, Msh(PrimitiveType.Ptr32, BeFields((11,5),(19,11),(31,1)),6,16),Annul(30)),
                Instr(Mnemonic.be_l,TD, Msh(PrimitiveType.Ptr32, BeFields((11,5),(19,11),(31,1)),6,16),Annul(30)),
                branch,
                invalid,

                extract_3C,
                deposit_3D,
                multimedia,
                invalid);
        }
    }
}
