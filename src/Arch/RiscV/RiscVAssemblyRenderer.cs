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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Reko.Arch.RiscV
{
    public class RiscVAssemblyRenderer
    {
        private static readonly Dictionary<Mnemonic, string> mnemonicNames;
        private static readonly char[] floatSpecials = new char[] { '.', 'e', 'E' };

        public static RiscVAssemblyRenderer Renderer { get; } = new RiscVAssemblyRenderer();

        static RiscVAssemblyRenderer()
        {
            mnemonicNames = new Dictionary<Mnemonic, string>
            {
                { Mnemonic.c_add, "c.add" },
                { Mnemonic.c_addi, "c.addi" },
                { Mnemonic.c_addiw, "c.addiw" },
                { Mnemonic.c_addw, "c.addw" },
                { Mnemonic.c_and, "c.and" },
                { Mnemonic.c_andi, "c.andi" },
                { Mnemonic.c_beqz,  "c.beqz" },
                { Mnemonic.c_bnez,  "c.bnez" },
                { Mnemonic.c_addi16sp, "c.addi16sp" },
                { Mnemonic.c_addi4spn, "c.addi4spn" },
                { Mnemonic.c_fld, "c.fld" },
                { Mnemonic.c_fldsp, "c.fldsp" },
                { Mnemonic.c_flw, "c.flw" },
                { Mnemonic.c_flwsp, "c.flwsp" },
                { Mnemonic.c_fsd, "c.fsd" },
                { Mnemonic.c_fsdsp, "c.fsdsp" },
                { Mnemonic.c_j, "c.j" },
                { Mnemonic.c_jr, "c.jr" },
                { Mnemonic.c_jalr, "c.jalr" },
                { Mnemonic.c_ld, "c.ld" },
                { Mnemonic.c_ldsp, "c.ldsp" },
                { Mnemonic.c_lwsp, "c.lwsp" },
                { Mnemonic.c_li, "c.li" },
                { Mnemonic.c_lui, "c.lui" },
                { Mnemonic.c_lw, "c.lw" },
                { Mnemonic.c_mv, "c.mv" },
                { Mnemonic.c_or, "c.or" },
                { Mnemonic.c_sdsp, "c.sdsp" },
                { Mnemonic.c_swsp, "c.swsp" },
                { Mnemonic.c_slli, "c.slli" },
                { Mnemonic.c_sd, "c.sd" },
                { Mnemonic.c_srai, "c.srai" },
                { Mnemonic.c_srli, "c.srli" },
                { Mnemonic.c_sub, "c.sub" },
                { Mnemonic.c_subw, "c.subw" },
                { Mnemonic.c_sw, "c.sw" },
                { Mnemonic.c_xor, "c.xor" },



                // RV32I Base Instruction Set
                { Mnemonic.fence_tso, "fence.tso" },



                // RV32/RV64 Zifencei Standard Extension
                { Mnemonic.fence_i, "fence.i" },



                // RV32A Standard Extension
                { Mnemonic.lr_w, "lr.w" },
                { Mnemonic.sc_w, "sc.w" },
                { Mnemonic.amoswap_w, "amoswap.w" },
                { Mnemonic.amoadd_w, "amoadd.w" },
                { Mnemonic.amoxor_w, "amoxor.w" },
                { Mnemonic.amoand_w, "amoand.w" },
                { Mnemonic.amoor_w, "amoor.w" },
                { Mnemonic.amomin_w, "amomin.w" },
                { Mnemonic.amomax_w, "amomax.w" },
                { Mnemonic.amominu_w, "amominu.w" },
                { Mnemonic.amomaxu_w, "amomaxu.w" },



                // RV64A Standard Extension (in addition to RV32A)
                { Mnemonic.lr_d, "lr.d" },
                { Mnemonic.sc_d, "sc.d" },
                { Mnemonic.amoswap_d, "amoswap.d" },
                { Mnemonic.amoadd_d, "amoadd.d" },
                { Mnemonic.amoxor_d, "amoxor.d" },
                { Mnemonic.amoand_d, "amoand.d" },
                { Mnemonic.amoor_d, "amoor.d" },
                { Mnemonic.amomin_d, "amomin.d" },
                { Mnemonic.amomax_d, "amomax.d" },
                { Mnemonic.amominu_d, "amominu.d" },
                { Mnemonic.amomaxu_d, "amomaxu.d" },



                // RV32F Standard Extension
                { Mnemonic.fmadd_s, "fmadd.s" },
                { Mnemonic.fmsub_s, "fmsub.s" },
                { Mnemonic.fnmsub_s, "fnmsub.s" },
                { Mnemonic.fnmadd_s, "fnmadd.s" },
                { Mnemonic.fadd_s, "fadd.s" },
                { Mnemonic.fsub_s, "fsub.s" },
                { Mnemonic.fmul_s, "fmul.s" },
                { Mnemonic.fdiv_s, "fdiv.s" },
                { Mnemonic.fsqrt_s, "fsqrt.s" },
                { Mnemonic.fsgnj_s, "fsgnj.s" },
                { Mnemonic.fsgnjn_s, "fsgnjn.s" },
                { Mnemonic.fsgnjx_s, "fsgnjx.s" },
                { Mnemonic.fmin_s, "fmin.s" },
                { Mnemonic.fmax_s, "fmax.s" },
                { Mnemonic.fcvt_w_s, "fcvt.w.s" },
                { Mnemonic.fcvt_wu_s, "fcvt.wu.s" },
                { Mnemonic.fmv_x_w, "fmv.x.w" },
                { Mnemonic.feq_s, "feq.s" },
                { Mnemonic.flt_s, "flt.s" },
                { Mnemonic.fle_s, "fle.s" },
                { Mnemonic.fclass_s, "fclass.s" },
                { Mnemonic.fcvt_s_w, "fcvt.s.w" },
                { Mnemonic.fcvt_s_wu, "fcvt.s.wu" },
                { Mnemonic.fmv_w_x, "fmv.w.x" },



                // RV64F Standard Extension (in addition to RV32F)
                { Mnemonic.fcvt_l_s, "fcvt.l.s" },
                { Mnemonic.fcvt_lu_s, "fcvt.lu.s" },
                { Mnemonic.fcvt_s_l, "fcvt.s.l" },
                { Mnemonic.fcvt_s_lu, "fcvt.s.lu" },



                // RV32D Standard Extension
                { Mnemonic.fmadd_d, "fmadd.d" },
                { Mnemonic.fmsub_d, "fmsub.d" },
                { Mnemonic.fnmsub_d, "fnmsub.d" },
                { Mnemonic.fnmadd_d, "fnmadd.d" },
                { Mnemonic.fadd_d, "fadd.d" },
                { Mnemonic.fsub_d, "fsub.d" },
                { Mnemonic.fmul_d, "fmul.d" },
                { Mnemonic.fdiv_d, "fdiv.d" },
                { Mnemonic.fsqrt_d, "fsqrt.d" },
                { Mnemonic.fsgnj_d, "fsgnj.d" },
                { Mnemonic.fsgnjn_d, "fsgnjn.d" },
                { Mnemonic.fsgnjx_d, "fsgnjx.d" },
                { Mnemonic.fmin_d, "fmin.d" },
                { Mnemonic.fmax_d, "fmax.d" },
                { Mnemonic.fcvt_s_d, "fcvt.s.d" },
                { Mnemonic.fcvt_d_s, "fcvt.d.s" },
                { Mnemonic.feq_d, "feq.d" },
                { Mnemonic.flt_d, "flt.d" },
                { Mnemonic.fle_d, "fle.d" },
                { Mnemonic.fclass_d, "fclass.d" },
                { Mnemonic.fcvt_w_d, "fcvt.w.d" },
                { Mnemonic.fcvt_wu_d, "fcvt.wu.d" },
                { Mnemonic.fcvt_d_w, "fcvt.d.w" },
                { Mnemonic.fcvt_d_wu, "fcvt.d.wu" },



                // RV64D Standard Extension (in addition to RV32D)
                { Mnemonic.fcvt_l_d, "fcvt.l.d" },
                { Mnemonic.fcvt_lu_d, "fcvt.lu.d" },
                { Mnemonic.fmv_x_d, "fmv.x.d" },
                { Mnemonic.fcvt_d_l, "fcvt.d.l" },
                { Mnemonic.fcvt_d_lu, "fcvt.d.lu" },
                { Mnemonic.fmv_d_x, "fmv.d.x" },



                // RV32Q Standard Extension
                { Mnemonic.fmadd_q, "fmadd.q" },
                { Mnemonic.fmsub_q, "fmsub.q" },
                { Mnemonic.fnmsub_q, "fnmsub.q" },
                { Mnemonic.fnmadd_q, "fnmadd.q" },
                { Mnemonic.fadd_q, "fadd.q" },
                { Mnemonic.fsub_q, "fsub.q" },
                { Mnemonic.fmul_q, "fmul.q" },
                { Mnemonic.fdiv_q, "fdiv.q" },
                { Mnemonic.fsqrt_q, "fsqrt.q" },
                { Mnemonic.fsgnj_q, "fsgnj.q" },
                { Mnemonic.fsgnjn_q, "fsgnjn.q" },
                { Mnemonic.fsgnjx_q, "fsgnjx.q" },
                { Mnemonic.fmin_q, "fmin.q" },
                { Mnemonic.fmax_q, "fmax.q" },
                { Mnemonic.fcvt_s_q, "fcvt.s.q" },
                { Mnemonic.fcvt_q_s, "fcvt.q.s" },
                { Mnemonic.fcvt_d_q, "fcvt.d.q" },
                { Mnemonic.fcvt_q_d, "fcvt.q.d" },
                { Mnemonic.feq_q, "feq.q" },
                { Mnemonic.flt_q, "flt.q" },
                { Mnemonic.fle_q, "fle.q" },
                { Mnemonic.fclass_q, "fclass.q" },
                { Mnemonic.fcvt_w_q, "fcvt.w.q" },
                { Mnemonic.fcvt_wu_q, "fcvt.wu.q" },
                { Mnemonic.fcvt_q_w, "fcvt.q.w" },
                { Mnemonic.fcvt_q_wu, "fcvt.q.wu" },



                // RV64Q Standard Extension (in addition to RV32Q)
                { Mnemonic.fcvt_l_q, "fcvt.l.q" },
                { Mnemonic.fcvt_lu_q, "fcvt.lu.q" },
                { Mnemonic.fcvt_q_l, "fcvt.q.l" },
                { Mnemonic.fcvt_q_lu, "fcvt.q.lu" },

            };
        }

        public string FormatValue(Constant c, bool forceSignForSignedIntegers = false)
        {
            var pt = (PrimitiveType) c.DataType;
            if (pt.Domain == Domain.SignedInt)
            {
                return FormatSignedValue(c.ToInt64(), forceSignForSignedIntegers);
            }
            else if (pt.Domain == Domain.Real)
            {
                var str = c.ToReal64().ToString("G", CultureInfo.InvariantCulture);
                if (str.IndexOfAny(floatSpecials) < 0)
                {
                    return str + ".0";
                }
                return str;
            }
            else
                return FormatUnsignedValue(c.ToUInt64());
        }

        public void Render(RiscVInstruction instr, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderMnemonic(instr, renderer);

            if (instr.Operands.Length > 0)
            {
                renderer.Tab();
                RenderOperands(instr, options, renderer);
            }
        }

        protected virtual void RenderMnemonic(RiscVInstruction instr, MachineInstructionRenderer renderer)
        {
            var sb = new StringBuilder();
            if (mnemonicNames.TryGetValue(instr.Mnemonic, out string? name))
            {
                sb.Append(name);
            }
            else
            {
                sb.Append(instr.Mnemonic.ToString());
                sb.Replace('_', '.');
            }

            if (instr.Acquire || instr.Release)
                sb.Append(".");
            if (instr.Acquire)
                sb.Append("aq");
            if (instr.Release)
                sb.Append("rl");

            renderer.WriteMnemonic(sb.ToString());
        }

        protected virtual void RenderOperands(RiscVInstruction instr, MachineInstructionRendererOptions options, MachineInstructionRenderer renderer)
        {
            var operandSeparator = "";
            for (int i = 0; i < instr.Operands.Length; ++i)
            {
                renderer.WriteString(operandSeparator);
                operandSeparator = options.OperandSeparator ?? ",";

                var operand = instr.Operands[i];
                RenderOperand(instr, operand, renderer, options);
            }
        }

        protected virtual void RenderOperand(
            RiscVInstruction instr,
            MachineOperand op,
            MachineInstructionRenderer renderer,
            MachineInstructionRendererOptions options)
        {
            switch (op)
            {
            case RegisterStorage rop:
                RenderRegister(rop.Name, renderer);
                return;
            case ImmediateOperand immop:
                RenderImmediate(immop, renderer);
                return;
            case AddressOperand addrop:
                //$TODO: 32-bit?
                if (addrop.Width.BitSize == 32)
                {
                    renderer.WriteAddress(string.Format("0x{0:X8}", addrop.Address.ToLinear()), addrop.Address);
                } else {
                    renderer.WriteAddress(string.Format("0x{0:X16}", addrop.Address.ToLinear()), addrop.Address);
                }
                    return;
            case MemoryOperand memop:
                RenderMemoryOperand(memop, renderer, options);
                return;
            }
            throw new NotImplementedException($"Risc-V operand type {op.GetType().Name} not implemented yet.");
        }

        protected virtual void RenderMemoryOperand(
            MemoryOperand memop,
            MachineInstructionRenderer renderer,
            MachineInstructionRendererOptions options)
        {
            if(memop.Offset == 0)
            {
                renderer.WriteFormat("({0:X})", memop.Base);
            }
            else
            {
                var offset = FormatSignedValue(memop.Offset);
                renderer.WriteFormat("{0}({1:X})", offset, memop.Base);
            }
        }

        protected virtual void RenderRegister(string regName, MachineInstructionRenderer renderer)
        {
            renderer.WriteString(regName);
        }

        protected virtual void RenderImmediate(
            ImmediateOperand imm,
            MachineInstructionRenderer renderer)
        {
            var pt = imm.Value.DataType;
            if (pt.Domain == Domain.Offset)
                renderer.WriteString(FormatUnsignedValue(imm.Value.ToUInt64(), "0x{0:X}"));
            else
            {
                var s = FormatValue(imm.Value);
                if (pt.Domain == Domain.Pointer)
                    renderer.WriteAddress(s, Address.FromConstant(imm.Value));
                else
                    renderer.WriteString(s);
            }
        }

        protected string FormatSignedValue(long n, bool forceSign = false)
        {
            string sign = "";
            if (n < 0)
            {
                sign = "-";
                n = -n;
            }
            else if (forceSign)
            {
                sign = "+";
            }
            var sb = new StringBuilder();
            sb.AppendFormat("{0}0x{1:X}", sign, n);

            return sb.ToString();
        }

        protected string FormatUnsignedValue(ulong n, string format = "0x{0:X}")
        {
            return string.Format(format, n);
        }
    }
}
