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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Operators;
using Reko.Core.Rtl;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Reko.Arch.OpenRISC.Aeon
{
    public class AeonRewriter : IEnumerable<RtlInstructionCluster>
    {
        private readonly AeonArchitecture arch;
        private readonly EndianImageReader rdr;
        private readonly ProcessorState state;
        private readonly IStorageBinder binder;
        private readonly IRewriterHost host;
        private readonly LookaheadEnumerator<AeonInstruction> dasm;
        private readonly List<RtlInstruction> rtls;
        private readonly RtlEmitter m;
        private AeonInstruction instr;
        private Address addrInstr;
        private InstrClass iclass;

        public AeonRewriter(AeonArchitecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            dasm = new LookaheadEnumerator<AeonInstruction>(new AeonDisassembler(arch, rdr));
            rtls = new List<RtlInstruction>();
            m = new RtlEmitter(rtls);
            instr = default!;
            addrInstr = default!;
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                this.instr = dasm.Current;
                this.addrInstr = instr.Address;
                iclass = instr.InstructionClass;
                switch (instr.Mnemonic)
                {
                default:
                    EmitUnitTest();
                    iclass = InstrClass.Invalid;
                    m.Invalid();
                    break;
                case Mnemonic.bg_abs__: RewriteIntrinsic(CommonOps.Abs.MakeInstance(PrimitiveType.Int32)); break;
                case Mnemonic.bt_add__:
                case Mnemonic.bn_add: RewriteAddSub(m.IAdd); break;
                case Mnemonic.bn_add_s: RewriteIntrinsic(add_s_intrinsic); break;
                case Mnemonic.bt_add16: RewriteAdd16(); break;
                case Mnemonic.bn_addc__: RewriteAddSub(Addc); break;
                case Mnemonic.bn_addc_s_lwq__: RewriteIntrinsic(addc_s_lwq_intrinsic); break;
                case Mnemonic.bn_addc_s_lwqq__: RewriteIntrinsic(addc_s_lwqq_intrinsic); break;
                case Mnemonic.bg_addci__: RewriteAddi(Addc); break;
                case Mnemonic.bn_addcn_s__: RewriteIntrinsic(addcn_s_intrinsic); break;
                case Mnemonic.bn_addcqq_s__: RewriteIntrinsic(addcqq_s_intrinsic); break;
                case Mnemonic.bn_addcnqq_s__: RewriteIntrinsic(addcnqq_s_intrinsic); break;
                case Mnemonic.bt_addi__:
                case Mnemonic.bn_addi:
                case Mnemonic.bg_addi: RewriteAddi(m.AddSubSignedInt); break;
                case Mnemonic.bn_addp_s__: RewriteIntrinsic(addp_s_intrinsic); break;
                case Mnemonic.bn_addsf__: RewriteFpuOp(Operator.FAdd, PrimitiveType.Real32); break;
                case Mnemonic.bg_amac__: RewriteIntrinsic(amac_intrinsic); break;
                case Mnemonic.bg_amac_hh__: RewriteIntrinsic(amac_hh_intrinsic); break;
                case Mnemonic.bg_amac_lh__: RewriteIntrinsic(amac_lh_intrinsic); break;
                case Mnemonic.bg_amac_ll__: RewriteIntrinsic(amac_ll_intrinsic); break;
                case Mnemonic.bg_amac_wh__: RewriteIntrinsic(amac_wh_intrinsic); break;
                case Mnemonic.bg_amac_wl__: RewriteIntrinsic(amac_wl_intrinsic); break;
                case Mnemonic.bg_amacq_hl__: RewriteIntrinsic(amacq_hl_intrinsic); break;
                case Mnemonic.bg_amacr__: RewriteIntrinsic(amacr_intrinsic); break;
                case Mnemonic.bg_amacr_ex__: RewriteIntrinsic(amacr_ex_intrinsic); break;
                case Mnemonic.bg_amacrq__: RewriteIntrinsic(amacrq_intrinsic); break;
                case Mnemonic.bg_amacw__: RewriteIntrinsic(amacw_intrinsic); break;
                case Mnemonic.bg_amadd__: RewriteIntrinsic(amadd_intrinsic); break;
                case Mnemonic.bg_amsb__: RewriteIntrinsic(amsb_intrinsic); break;
                case Mnemonic.bg_amsb_lh__: RewriteIntrinsic(amsb_lh_intrinsic); break;
                case Mnemonic.bg_amsb_wh__: RewriteIntrinsic(amsb_wh_intrinsic); break;
                case Mnemonic.bg_amsb_wl__: RewriteIntrinsic(amsb_wl_intrinsic); break;
                case Mnemonic.bg_amsub_hl__: RewriteIntrinsic(amsub_hl_intrinsic); break;
                case Mnemonic.bg_amsub_wl__: RewriteIntrinsic(amsub_wl_intrinsic); break;
                case Mnemonic.bg_amul__: RewriteIntrinsic(amul_intrinsic); break;
                case Mnemonic.bg_amul_lh__: RewriteIntrinsic(amul_lh_intrinsic); break;
                case Mnemonic.bg_amul_ll__: RewriteIntrinsic(amul_ll_intrinsic); break;
                case Mnemonic.bg_amul_wh__: RewriteIntrinsic(amul_wh_intrinsic); break;
                case Mnemonic.bg_amul_wl__: RewriteIntrinsic(amul_wl_intrinsic); break;
                case Mnemonic.bg_amulqu__: RewriteIntrinsic(amulqu_intrinsic); break;
                case Mnemonic.bn_and: RewriteArithmetic(m.And); break;
                case Mnemonic.bn_andi:
                case Mnemonic.bg_andi: RewriteLogicalImm(m.And); break;
                case Mnemonic.bn_andn: RewriteAndn(); break;
                case Mnemonic.bg_b__bitseti__: Rewrite_b_bitset(); break;
                case Mnemonic.bg_beq__:
                case Mnemonic.bn_beqi__:
                case Mnemonic.bg_beqi__:
                case Mnemonic.bn_beqi____: RewriteBxx(m.Eq); break;
                case Mnemonic.bn_bf:
                case Mnemonic.bg_bf: RewriteBf(true); break;
                case Mnemonic.bg_bges__: RewriteBxx(m.Ge); break;
                case Mnemonic.bg_bgeu__:
                case Mnemonic.bg_bgeu____: RewriteBxx(m.Uge); break;
                case Mnemonic.bn_bgtui__: RewriteBxx(m.Ugt); break;
                case Mnemonic.bg_bgts__:
                case Mnemonic.bg_bgtsi__: RewriteBxx(m.Gt); break;
                case Mnemonic.bg_bgtui__: RewriteBxx(m.Ugt); break;
                case Mnemonic.bg_bleui__:
                case Mnemonic.bn_bleui__: RewriteBxx(m.Ule); break;
                case Mnemonic.bg_blesi__:
                case Mnemonic.bn_blesi____:
                case Mnemonic.bn_blesi__: RewriteBxx(m.Le); break;
                case Mnemonic.bg_bleu__: RewriteBxx(m.Ule); break;
                case Mnemonic.bg_bltsi__: RewriteBxx(m.Lt); break;
                case Mnemonic.bg_bltui__: RewriteBxx(m.Ult); break;
                case Mnemonic.bg_bne__: RewriteBxx(m.Ne); break;
                case Mnemonic.bn_bnei__: RewriteBxx(m.Ne); break;
                case Mnemonic.bg_bnf:
                case Mnemonic.bn_bnf__: RewriteBf(false); break;
                case Mnemonic.bn_bsa__: RewriteIntrinsic(bsa_intrinsic); break;
                case Mnemonic.bn_bsa_s__: RewriteIntrinsic(bsa_s_intrinsic); break;
                case Mnemonic.bn_bsl__: RewriteIntrinsic(bsl_intrinsic); break;
                case Mnemonic.bg_btb__: RewriteIntrinsic(btb_intrinsic); break;
                case Mnemonic.bg_chk_ll__: RewriteIntrinsic(chk_ll_intrinsic); break;
                case Mnemonic.bg_chk_lu__: RewriteIntrinsic(chk_lu_intrinsic); break;
                case Mnemonic.bn_clz__: RewriteIntrinsic(CommonOps.CountLeadingZeros); break;
                case Mnemonic.bn_cmov__: RewriteCmov(); break;
                case Mnemonic.bn_cmovii__: RewriteCmov(); break;
                case Mnemonic.bn_cmovir__: RewriteCmov(); break;
                case Mnemonic.bn_cmovri__: RewriteCmov(); break;
                case Mnemonic.bn_conjc__: RewriteIntrinsic(conjc_intrinsic); break;
                case Mnemonic.bn_crc_le__: RewriteIntrinsic(crc_le_intrinsic); break;
                case Mnemonic.bg_dep__:
                case Mnemonic.bg_depi__:
                case Mnemonic.bg_depr__: 
                case Mnemonic.bg_depri__: RewriteIntrinsic(dep_intrinsic); break;
                case Mnemonic.bn_df2sf__: RewriteIntrinsic(df2sf_intrinsic); break;
                case Mnemonic.bt_di__: RewriteIntrinsic(di_intrinsic); break;
                case Mnemonic.bg_divl__:
                case Mnemonic.bg_divlr__: RewriteIntrinsic(divl_intrinsic); break;
                case Mnemonic.bg_divlru__:
                case Mnemonic.bg_divlu__: RewriteIntrinsic(divlu_intrinsic); break;
                case Mnemonic.bn_divs__: RewriteArithmetic(m.SDiv); break;
                case Mnemonic.bn_divsf__: RewriteFpuOp(Operator.FDiv, PrimitiveType.Real32); break;
                case Mnemonic.bn_divu: RewriteArithmetic(m.UDiv); break;
                case Mnemonic.bg_dma_op__: RewriteIntrinsic(dma_op_intrinsic); break;
                case Mnemonic.bn_dsl__: RewriteIntrinsic(dsl_intrinsic); break;
                case Mnemonic.bn_dsli__: RewriteIntrinsic(dsli_intrinsic); break;
                case Mnemonic.bn_dsr__: RewriteIntrinsic(dsr_intrinsic); break;
                case Mnemonic.bn_dsri__: RewriteIntrinsic(dsri_intrinsic); break;
                case Mnemonic.bt_ei__: RewriteIntrinsic(ei_intrinsic); break;
                case Mnemonic.bn_exp__: RewriteIntrinsic(exp_intrinsic); break;
                case Mnemonic.bn_exp16__: RewriteIntrinsic(exp16_intrinsic); break;
                case Mnemonic.bn_entri__: RewriteEntri(); break;
                case Mnemonic.bg_ext__:
                case Mnemonic.bg_exti__: RewriteIntrinsic(ext_intrinsic); break;
                case Mnemonic.bg_extu__:
                case Mnemonic.bg_extui__: RewriteIntrinsic(extu_intrinsic); break;
                case Mnemonic.bn_extbs__: RewriteExt(PrimitiveType.Int8, PrimitiveType.Int32); break;
                case Mnemonic.bn_extbz__: RewriteExt(PrimitiveType.Byte, PrimitiveType.UInt32); break;
                case Mnemonic.bn_exths__: RewriteExt(PrimitiveType.Int16, PrimitiveType.Int32); break;
                case Mnemonic.bn_exthz__: RewriteExt(PrimitiveType.UInt16, PrimitiveType.UInt32); break;
                case Mnemonic.bn_ff1__: RewriteIntrinsic(CommonOps.FindFirstOne); break;
                case Mnemonic.bg_fft__: RewriteIntrinsic(fft_intrinsic); break;
                case Mnemonic.bn_fl1: RewriteIntrinsic(fl1_intrinsic); break;
                case Mnemonic.bn_flb: RewriteIntrinsic(flb_intrinsic); break;
                case Mnemonic.bg_flush_invalidate: RewriteCache(flush_invalidate_intrinsic, false); break;
                case Mnemonic.bg_flush_line: RewriteCache(flush_line_intrinsic, true); break;
                case Mnemonic.bg_i2df__: RewriteI2F(PrimitiveType.Real64); break;
                case Mnemonic.bn_i2sf__: RewriteI2F(PrimitiveType.Real32); break;
                case Mnemonic.bg_invalidate: RewriteCache(invalidate_intrinsic, false); break;
                case Mnemonic.bg_invalidate_line: RewriteCache(invalidate_line_intrinsic, true); break;
                case Mnemonic.bt_j:
                case Mnemonic.bn_j:
                case Mnemonic.bg_j: RewriteJ(); break;
                case Mnemonic.bg_jal:
                case Mnemonic.bn_jal__: RewriteJal(); break;
                case Mnemonic.bt_jalr__: RewriteJalr(); break;
                case Mnemonic.bt_jr: RewriteJr(); break;
                case Mnemonic.bg_lbs__:
                case Mnemonic.bn_lbs__: RewriteLoadExt(PrimitiveType.SByte); break;
                case Mnemonic.bn_lbz__:
                case Mnemonic.bg_lbz__: RewriteLoadExt(PrimitiveType.Byte); break;
                case Mnemonic.bg_ldww__: RewriteIntrinsic(ldww_intrinsic); break;
                case Mnemonic.bg_ldww2__: RewriteIntrinsic(ldww2_intrinsic); break;
                case Mnemonic.bg_ldww2x__: RewriteIntrinsic(ldww2x_intrinsic); break;
                case Mnemonic.bg_ldwwx__: RewriteIntrinsic(ldwwx_intrinsic); break;
                case Mnemonic.bg_lhs__: RewriteLoadExt(PrimitiveType.Int16); break;
                case Mnemonic.bn_lhz:
                case Mnemonic.bg_lhz__: RewriteLoadExt(PrimitiveType.UInt16); break;
                case Mnemonic.bg_loop__:
                case Mnemonic.bg_loopi__: RewriteIntrinsic(loop_intrinsic); break;
                case Mnemonic.bt_lwst____:
                case Mnemonic.bn_lwz:
                case Mnemonic.bg_lwz: RewriteLoadExt(PrimitiveType.Word32); break;
                case Mnemonic.bn_maddsf__: RewriteIntrinsic(maddsf_intrinsic); break;
                case Mnemonic.bg_max__:
                case Mnemonic.bg_maxi__: RewriteIntrinsic(CommonOps.Max, PrimitiveType.Int32); break;
                case Mnemonic.bg_maxu__:
                case Mnemonic.bg_maxui__: RewriteIntrinsic(CommonOps.Max, PrimitiveType.UInt32); break;
                case Mnemonic.bg_mfspr1__: RewriteMfspr(true); break;
                case Mnemonic.bg_mfspr: RewriteMfspr(false); break;
                case Mnemonic.bg_min__:
                case Mnemonic.bg_mini__: RewriteIntrinsic(CommonOps.Min, PrimitiveType.Int32); break;
                case Mnemonic.bg_minu__:
                case Mnemonic.bg_minui__: RewriteIntrinsic(CommonOps.Min, PrimitiveType.UInt32); break;
                case Mnemonic.bn_mlwz__: RewriteIntrinsic(mlwz_intrinsic); break;
                case Mnemonic.bt_mov: RewriteMov(); break;
                case Mnemonic.bt_mov16: RewriteMov16(); break;
                case Mnemonic.bt_movi__: RewriteMovi(); break;
                case Mnemonic.bn_movhi__:
                case Mnemonic.bg_movhi:
                case Mnemonic.bt_movhi__: RewriteMovhi(); break;
                case Mnemonic.bn_msw__: RewriteIntrinsic(msw_intrinsic); break;
                case Mnemonic.bg_mtspr1__: RewriteMtspr(true); break;
                case Mnemonic.bg_mtspr: RewriteMtspr(false); break;
                case Mnemonic.bn_mulsf__: RewriteFpuOp(Operator.FMul, PrimitiveType.Real32); break;
                case Mnemonic.bn_mul: RewriteMul32x32(m.SMul); break;
                case Mnemonic.bn_muladdh__: RewriteMuladd(muladdh_instrinsic); break;
                case Mnemonic.bn_muladdhx__: RewriteMuladd(muladdhx_instrinsic); break;
                //$REVIEW: determine whether this is signed (it probably is)
                case Mnemonic.bg_muli__: RewriteMul32x32(m.IMul); break;
                case Mnemonic.bn_mulsubh__: RewriteMuladd(mulsubh_instrinsic); break;
                case Mnemonic.bn_mulsubhx__: RewriteMuladd(mulsubx_instrinsic); break;
                case Mnemonic.bn_mulu____: RewriteMul32x32(m.UMul); break;
                case Mnemonic.bn_nand__: RewriteNand(); break;
                case Mnemonic.bt_nop:
                case Mnemonic.bn_nop: RewriteNop(); break;
                case Mnemonic.mv_opv__: RewriteIntrinsic(mv_opv_intrinsic); break;
                case Mnemonic.bn_or: RewriteArithmetic(m.Or); break;
                case Mnemonic.bn_ori:
                case Mnemonic.bg_ori: RewriteOri(m.Or); break;
                case Mnemonic.bn_pack_s__: RewriteIntrinsic(pack_s_instrinsic); break;
                case Mnemonic.bg_pamac__: RewriteIntrinsic(pamac_instrinsic); break;
                case Mnemonic.bg_pamacrq__: RewriteIntrinsic(pamacrq_instrinsic); break;
                case Mnemonic.bt_push__: RewriteIntrinsic(push_instrinsic); break;
                case Mnemonic.bn_remsf__: RewriteFpuOp(Operator.FMod, PrimitiveType.Real32); break;
                case Mnemonic.bt_return__: RewriteReturn(); break;
                case Mnemonic.bt_rfe: RewriteRfe(); break;
                case Mnemonic.bn_ror__:
                case Mnemonic.bn_rori__: RewriteIntrinsic(CommonOps.Ror); break;
                case Mnemonic.bn_rtnei__: RewriteRtnei(); break;
                case Mnemonic.bn_sat__: RewriteIntrinsic(sat_intrinsic); break;
                case Mnemonic.bn_satsu__: RewriteIntrinsic(satsu_intrinsic); break;
                case Mnemonic.bn_satu__: RewriteIntrinsic(satu_intrinsic); break;
                case Mnemonic.bn_satus__: RewriteIntrinsic(satus_intrinsic); break;
                case Mnemonic.bn_sf2df__: RewriteIntrinsic(sf2df_intrinsic); break;
                case Mnemonic.bn_sf2i__: RewriteF2I(PrimitiveType.Real32); break;
                case Mnemonic.bn_sfeqdf__: RewriteSfxx(m.FEq); break;
                case Mnemonic.bn_sfeqsf__: RewriteSfxx(m.FEq); break;
                case Mnemonic.bn_sfeq__: RewriteSfxx(m.Eq); break;
                case Mnemonic.bn_sfeqi: RewriteSfxx(m.Eq); break;
                case Mnemonic.bn_sfges____:
                case Mnemonic.bn_sfges__:
                case Mnemonic.bg_sfgesi__:
                case Mnemonic.bn_sfgesi__: RewriteSfxx(m.Ge); break;
                case Mnemonic.bn_sfgeu:
                case Mnemonic.bg_sfgeui__: RewriteSfxx(m.Uge); break;
                case Mnemonic.bn_sfgtdf__: RewriteSfxx(m.FGt); break;
                case Mnemonic.bn_sfgtsf__: RewriteSfxx(m.FGt); break;
                case Mnemonic.bt_sfgtsi_minus32769__: RewriteSff(sfgtsi_minus32769_intrinsic); break;
                case Mnemonic.bt_sfgtui_minus32769__: RewriteSff(sfgtui_minus32769_intrinsic); break;
                case Mnemonic.bg_sfgtui__:
                case Mnemonic.bn_sfgtui: RewriteSfxx(m.Ugt); break;
                case Mnemonic.bn_sfledf__: RewriteSfxx(m.FLe); break;
                case Mnemonic.bn_sflesf__: RewriteSfxx(m.FLe); break;
                case Mnemonic.bg_sfleui__:
                case Mnemonic.bn_sfleui__: RewriteSfxx(m.Ule); break;
                case Mnemonic.bg_sflesi__:
                case Mnemonic.bn_sflesi__: RewriteSfxx(m.Le); break;
                case Mnemonic.bn_sflts__: RewriteSfxx(m.Lt); break;
                case Mnemonic.bn_sfgtu: RewriteSfxx(m.Ugt); break;
                case Mnemonic.bn_sfne: RewriteSfxx(m.Ne); break;
                case Mnemonic.bn_sfnedf__: RewriteSfxx(m.FNe); break;
                case Mnemonic.bg_sfnei__:
                case Mnemonic.bn_sfnei__: RewriteSfxx(m.Ne); break;
                case Mnemonic.bn_sfnesf__: RewriteSfxx(m.FNe); break;
                case Mnemonic.bn_sb__:
                case Mnemonic.bg_sb__: RewriteStore(PrimitiveType.Byte); break;
                case Mnemonic.bn_sh__:
                case Mnemonic.bg_sh: RewriteStore(PrimitiveType.Word16); break;
                case Mnemonic.bn_sll__: RewriteShift(m.Shl); break;
                case Mnemonic.bn_slli__: RewriteShifti(m.Shl); break;
                case Mnemonic.bn_srl__: RewriteShift(m.Shr); break;
                case Mnemonic.bn_sra__: RewriteShift(m.Sar); break;
                case Mnemonic.bn_srai__: RewriteShifti(m.Sar); break;
                case Mnemonic.bn_srari__: RewriteShifti(m.Sar); break;  //$REVIEW: what's the difference between srai and srari?
                case Mnemonic.bn_srarzi__: RewriteIntrinsic(srarzi_intrinsic); break;
                case Mnemonic.bn_srli__: RewriteShifti(m.Shr); break;
                case Mnemonic.bn_srlri__: RewriteShifti(m.Shr); break;
                case Mnemonic.bn_sub: RewriteAddSub(m.ISub); break;
                case Mnemonic.bn_sub_s: RewriteIntrinsic(sub_s_intrinsic); break;
                case Mnemonic.bn_subb__: RewriteAddSub(Subb); break;
                case Mnemonic.bn_subc_s_lwq__: RewriteIntrinsic(sub_s_lwq_intrinsic); break;
                case Mnemonic.bn_subcn_s__: RewriteIntrinsic(subcn_s_intrinsic); break;
                case Mnemonic.bn_subcn_s_lwq__: RewriteIntrinsic(subcn_s_lwq_intrinsic); break;
                case Mnemonic.bn_subcnqq_s__: RewriteIntrinsic(subcnqq_s_intrinsic); break;
                case Mnemonic.bn_subcqq_s__: RewriteIntrinsic(subcqq_s_intrinsic); break;
                case Mnemonic.bn_subp_s__: RewriteIntrinsic(subp_s_intrinsic); break;
                case Mnemonic.bn_subsf__: RewriteFpuOp(Operator.FSub, PrimitiveType.Real32); break;
                case Mnemonic.bt_swst____:
                case Mnemonic.bn_sw:
                case Mnemonic.bg_sw: RewriteStore(PrimitiveType.Word32); break;
                case Mnemonic.bt_sys__: RewriteSideEffect(sys_intrinsic); break;
                case Mnemonic.bt_syncp__: RewriteSideEffect(syncp_intrinsic); break;
                case Mnemonic.bg_syncwritebuffer: RewriteSideEffect(syncwritebuffer_intrinsic); break;
                case Mnemonic.bt_trap: RewriteSideEffect(trap_intrinsic); break;
                case Mnemonic.bn_xor__:
                case Mnemonic.bg_xori__:
                case Mnemonic.bn_xori: RewriteArithmetic(m.Xor); break;
                    //$TODO: when all instructions are known this code can be removed.
                case Mnemonic.Nyi:
                    instr.Operands = Array.Empty<MachineOperand>();
                    RewriteUnknown();
                    break;
                }
                // Account for instruction fused into l.movhi
                var instrLength = (int) ((instr.Address - addrInstr) + instr.Length);
                yield return m.MakeCluster(addrInstr, instrLength, iclass);
                rtls.Clear();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EmitUnitTest()
        {
            var testgenSvc = arch.Services.GetService<ITestGenerationService>();
            testgenSvc?.ReportMissingRewriter("AeonRw", instr, instr.Mnemonic.ToString(), rdr, "");
        }

        private Expression Addc(Expression a, Expression b)
        {
            return m.IAdd(m.IAdd(a, b), binder.EnsureFlagGroup(Registers.CY));
        }

        private Expression Addc(Expression a, long b)
        {
            return m.IAdd(m.AddSubSignedInt(a, b), binder.EnsureFlagGroup(Registers.CY));
        }

        private Expression Subb(Expression a, Expression b)
        {
            return m.IAdd(m.ISub(a, b), binder.EnsureFlagGroup(Registers.CY));
        }


        private Expression EffectiveAddress(MemoryOperand mem)
        {
            Expression ea = binder.EnsureRegister(mem.Base!);
            if (mem.Offset != 0)
            {
                ea = m.AddSubSignedInt(ea, mem.Offset);
            }
            return ea;
        }

        private void MaybeSlice(DataType dt, Expression dst, Expression src)
        {
            if (dt.BitSize < src.DataType.BitSize)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, m.Slice(src, dt));
                src = tmp;
            }
            m.Assign(dst, src);
        }
        private void MaybeExtend(DataType dt, Expression dst, Expression src)
        {
            if (src.DataType.BitSize < 32)
            {
                var tmp = binder.CreateTemporary(dt);
                m.Assign(tmp, src);
                m.Assign(dst, m.Convert(tmp, tmp.DataType, dst.DataType));
            }
            else
            {
                m.Assign(dst, src);
            }
        }
        private Expression Op(int iop)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage reg:
                return binder.EnsureRegister(reg);
            case Constant imm:
                return imm;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                return m.Mem(mem.DataType, ea);
            default:
                throw new NotImplementedException($"Not impemented: {op.GetType().Name}.");
            }
        }

        private Expression OpOrZero(int iop)
        {
            var op = instr.Operands[iop];
            switch (op)
            {
            case RegisterStorage reg:
                if (reg.Number == 0)
                    return m.Word32(0);
                return binder.EnsureRegister(reg);
            case Constant imm:
                return imm;
            case Address addr:
                return addr;
            case MemoryOperand mem:
                var ea = EffectiveAddress(mem);
                return m.Mem(mem.DataType, ea);
            default:
                throw new NotImplementedException($"Not impemented: {op.GetType().Name}.");
            }
        }

        private void RewriteAddi(Func<Expression, long, Expression> fnAdd)
        {
            Expression left;
            int right;
            if (instr.Operands.Length == 2)
            {
                left = Op(0);
                right = ((Constant)instr.Operands[1]).ToInt32();
            }
            else
            {
                left = OpOrZero(1);
                right = ((Constant)instr.Operands[2]).ToInt32();
            }
            Expression src;
            if (left.IsZero)
            {
                src = m.Int32(right);
            }
            else
            {
                src = fnAdd(left, right);
            }
            var dst = Op(0);
            m.Assign(dst, src);
            m.Assign(binder.EnsureFlagGroup(Registers.CY), m.Cond(Registers.CY.DataType, dst));
            m.Assign(binder.EnsureFlagGroup(Registers.OV), m.Cond(Registers.OV.DataType, dst));
        }

        private void RewriteAddSub(Func<Expression, Expression, Expression> fn)
        {
            Expression left;
            Expression right;
            if (instr.Operands.Length == 2)
            {
                left = Op(0);
                right = OpOrZero(1);
            }
            else
            {
                left = OpOrZero(1);
                right = OpOrZero(2);
            }
            var dst = Op(0);
            m.Assign(dst, fn(left, right));
            m.Assign(binder.EnsureFlagGroup(Registers.CY), m.Cond(Registers.CY.DataType, dst));
            m.Assign(binder.EnsureFlagGroup(Registers.OV), m.Cond(Registers.OV.DataType, dst));
        }

        private void RewriteAdd16()
        {
            var dst = Op(0);
            m.Assign(dst, m.IAdd(dst, 16));
        }

        private void RewriteAndn()
        {
            Expression left = OpOrZero(1);
            Expression right = OpOrZero(2);
            Expression dst = Op(0);
            m.Assign(left, m.And(left, m.Comp(right)));
        }

        private void RewriteArithmetic(Func<Expression, Expression, Expression> fn)
        {
            Expression left;
            Expression right;
            if (instr.Operands.Length == 2)
            {
                left = Op(0);
                right = OpOrZero(1);
            }
            else
            {
                left = OpOrZero(1);
                right = OpOrZero(2);
            }
            var dst = Op(0);
            m.Assign(dst, fn(left, right));
        }

        private void Rewrite_b_bitset()
        {
            var exp = OpOrZero(0);
            var bit = Op(1);
            var target = (Address) instr.Operands[2];
            m.Branch(m.Fn(CommonOps.Bit, exp, bit), target);
        }

        private void RewriteBf(bool condition)
        {
            Expression c = binder.EnsureFlagGroup(Registers.F);
            if (!condition)
            {
                c = m.Not(c);
            }
            m.Branch(c, (Address) Op(0));
        }

        private void RewriteBxx(Func<Expression, Expression, Expression> cmp)
        {
            var left = OpOrZero(0);
            var right = OpOrZero(1);
            var target = (Address)instr.Operands[2];
            m.Branch(cmp(left, right), target);
        }

        private void RewriteCmov()
        {
            var cond = binder.EnsureFlagGroup(Registers.F);
            var thenOp = OpOrZero(1);
            var elseOp = OpOrZero(2);
            var dst = Op(0);
            m.Assign(dst, m.Conditional(dst.DataType, cond, thenOp, elseOp));
        }

        private void RewriteExt(PrimitiveType dtFrom, PrimitiveType dtTo)
        {
            var tmp = binder.CreateTemporary(dtFrom);
            m.Assign(tmp, m.Slice(OpOrZero(1), dtFrom));
            m.Assign(Op(0), m.Convert(tmp, dtFrom, dtTo));
        }

        private void RewriteF2I(PrimitiveType dtFrom)
        {
            var src = OpOrZero(1);
            var dst = Op(0);
            m.Assign(dst, m.Convert(src, dtFrom, PrimitiveType.Int32));
        }

        private void RewriteFpuOp(BinaryOperator op, PrimitiveType dt)
        {
            var left = OpOrZero(1);
            var right = OpOrZero(2);
            var dst = Op(0);
            m.Assign(dst, m.Bin(op, left, right));
        }

        private void RewriteLoadExt(PrimitiveType dt)
        {
            var src = Op(1);
            var dst = Op(0);
            MaybeExtend(dt, dst, src);
        }

        private void RewriteMfspr(bool twoOperands)
        {
            var dst = OpOrZero(0);
            Expression spr;
            if (twoOperands) {
                spr = ((Constant)instr.Operands[1]);
            } else {
                var sprReg = (RegisterStorage) instr.Operands[1];
                var sprImm = (Constant)instr.Operands[2];

                if (sprReg.Number == 0) {
                    spr = sprImm;
                } else if (sprImm.IsZero) {
                    spr = binder.EnsureRegister(sprReg);
                } else {
                    spr = m.Or(binder.EnsureRegister(sprReg), sprImm);
                }
            }

            m.Assign(dst, m.Fn(mfspr_intrinsic, spr));
        }

        private void RewriteMtspr(bool twoOperands)
        {
            Expression spr;
            Expression value;
            if (twoOperands) {
                value = OpOrZero(0);
                spr = ((Constant)instr.Operands[1]);
            } else {
                var sprReg = (RegisterStorage) instr.Operands[0];
                value = OpOrZero(1);
                var sprImm = (Constant)instr.Operands[2];
    
                if (sprReg.Number == 0) {
                    spr = sprImm;
                } else if (sprImm.IsZero) {
                    spr = binder.EnsureRegister(sprReg);
                } else {
                    spr = m.Or(binder.EnsureRegister(sprReg), sprImm);
                }
            }
    
            m.SideEffect(m.Fn(mtspr_intrinsic, spr, value));
        }

        private void RewriteMov()
        {
            var src = OpOrZero(1);
            var dst = Op(0);
            m.Assign(dst, src);
        }

        private void RewriteMov16()
        {
            var dst = Op(0);
            m.Assign(dst, 16);
        }


        private void RewriteMovi()
        {
            var imm = ((Constant)instr.Operands[1]).ToUInt32();
            var dst = Op(0);
            m.Assign(dst, m.Word32(imm));
        }

        private void RewriteMovhi()
        {
            var movhi = this.instr;
            var immHi = ((Constant)instr.Operands[1]).ToUInt32();
            var regHi = (RegisterStorage) instr.Operands[0];
            var dst = binder.EnsureRegister(regHi);
            m.Assign(dst, m.Word32(immHi << 16));

            if (dasm.TryPeek(1, out var lowInstr))
            {
                switch (lowInstr!.Mnemonic)
                {
                case Mnemonic.bg_lbs__:
                case Mnemonic.bn_lbz__:
                case Mnemonic.bg_lbz__:
                case Mnemonic.bn_lhz:
                case Mnemonic.bg_lhz__:
                case Mnemonic.bn_lwz:
                case Mnemonic.bg_lwz:
                    var memLd = (MemoryOperand) lowInstr.Operands[1];
                    if (memLd.Base != regHi)
                        return;
                    dasm.MoveNext();
                    this.instr = dasm.Current;
                    uint uFullWord = MovhiSequenceFuser.AddFullWord(movhi.Operands[1], memLd.Offset);
                    var ea = Address.Ptr32(uFullWord);
                    MaybeExtend(memLd.DataType, Op(0), m.Mem(memLd.DataType, ea));
                    break;
                case Mnemonic.bn_sb__:
                case Mnemonic.bg_sb__:
                case Mnemonic.bn_sh__:
                case Mnemonic.bg_sh:
                case Mnemonic.bn_sw:
                case Mnemonic.bg_sw:
                    var memSt = (MemoryOperand) lowInstr.Operands[0];
                    if (memSt.Base != regHi)
                        return;
                    dasm.MoveNext();
                    this.instr = dasm.Current;
                    uFullWord = MovhiSequenceFuser.AddFullWord(movhi.Operands[1], memSt.Offset);
                    ea = Address.Ptr32(uFullWord);
                    MaybeSlice(memSt.DataType, m.Mem(memSt.DataType, ea), Op(1));
                    break;
                case Mnemonic.bt_addi__:
                case Mnemonic.bn_addi:
                case Mnemonic.bg_addi:
                    var addRegIndex = lowInstr.Operands.Length == 2 ? 0 : 1;
                    var addReg = (RegisterStorage) lowInstr.Operands[addRegIndex];
                    if (addReg != regHi)
                        return;
                    dasm.MoveNext();
                    this.instr = dasm.Current;
                    var iop = lowInstr.Operands.Length == 2 ? 1 : 2;
                    var addImm = (Constant) lowInstr.Operands[iop];
                    uFullWord = MovhiSequenceFuser.AddFullWord(movhi.Operands[1], addImm.ToInt32());
                    m.Assign(Op(0), m.Word32(uFullWord));
                    break;
                case Mnemonic.bn_ori:
                case Mnemonic.bg_ori:
                    var orReg = (RegisterStorage) lowInstr.Operands[1];
                    if (orReg != regHi)
                        return;
                    dasm.MoveNext();
                    this.instr = dasm.Current;
                    iop = lowInstr.Operands.Length == 2 ? 1 : 2;
                    var orImm = (Constant) lowInstr.Operands[iop];
                    uFullWord = MovhiSequenceFuser.OrFullWord(movhi.Operands[1], orImm.ToUInt32());
                    m.Assign(Op(0), m.Word32(uFullWord));
                    break;
                }
            }
        }

        private void RewriteMul32x32(Func<Expression, Expression, Expression> fn)
        {
            var src1 = OpOrZero(1);
            var src2 = OpOrZero(2);
            var mul = fn(src1, src2);
            mul.DataType = PrimitiveType.Word64;
            var product = binder.CreateTemporary(mul.DataType);
            m.Assign(product, mul);
            // upper 32 bits of product appear to be stored in SPR 0x2808
            var hi = Registers.SpecialRegisters[0x2808];
            var dstReg = (RegisterStorage) instr.Operands[0];
            var hi_lo = binder.EnsureSequence(mul.DataType, hi, dstReg);

            m.Assign(hi_lo, product);
        }

        private void RewriteMuladd(IntrinsicProcedure intrinsic)
        {
            var left = OpOrZero(1);
            var right = OpOrZero(2);
            var dst = Op(0);
            m.Assign(dst, m.Fn(intrinsic, left, right));
        }

        private void RewriteNand()
        {
            var left = OpOrZero(1);
            var right = OpOrZero(2);
            var dst = Op(0);
            Expression exp;
            if (left == right)
            {
                exp = left;
            } else {
                exp = m.And(left, right);
            }
            m.Assign(dst, m.Comp(exp));
        }

        private void RewriteNop()
        {
            m.Nop();
        }

        private void RewriteOri(Func<Expression, Expression, Expression> fn)
        {
            Expression left;
            Expression right;
            if (instr.Operands.Length == 2)
            {
                left = Op(0);
                right = m.Word32(((Constant)instr.Operands[1]).ToUInt32());
            }
            else
            {
                left = OpOrZero(1);
                right = m.Word32(((Constant)instr.Operands[2]).ToUInt32());
            }
            Expression src;
            if (left.IsZero)
            {
                src = right;
            }
            else
            {
                src = fn(left, right);
            }
            m.Assign(Op(0), src);
        }

        private void RewriteLogicalImm(Func<Expression, Expression, Expression> fn)
        {
            var left = OpOrZero(1);
            var right = ((Constant)instr.Operands[2]).ToUInt32();
            var dst = Op(0);
            m.Assign(dst, fn(left, m.Word32(right)));
        }

        private void RewriteCache(IntrinsicProcedure intrinsic, bool usesWay)
        {
            var ea = m.AddrOf(PrimitiveType.Ptr32, Op(0));
            var args = new List<Expression> { ea };

            if (usesWay) {
                var way = Op(1);
                args.Add(way);
            }

            m.SideEffect(m.Fn(intrinsic, args.ToArray()));
        }

        private void RewriteI2F(PrimitiveType dtReal)
        {
            var src = OpOrZero(1);
            var dst = Op(0);
            m.Assign(dst, m.Convert(src, PrimitiveType.Int32, dtReal));
        }

        private void RewriteJ()
        {
            var target = Op(0);
            m.Goto(target);
        }

        private void RewriteJal()
        {
            var target = Op(0);
            m.Call(target, 0);
        }

        private void RewriteJr()
        {
            var op0 = (Identifier)Op(0);
            if ((int)op0.Storage.Domain == 9)
            {
                iclass = InstrClass.Transfer | InstrClass.Return;
                m.Return(0, 0);
                return;
            }
            m.Goto(op0);
        }

        private void RewriteJalr()
        {
            var target = Op(0);
            m.Call(target, 0);
        }

        private void RewriteReturn()
        {
            m.SideEffect(m.Fn(return_intrinsic));
            m.Return(0, 0);
        }

        private void RewriteRfe()
        {
            var epcr = binder.EnsureRegister(Registers.EPCR);
            var esr = binder.EnsureRegister(Registers.ESR);
            m.SideEffect(m.Fn(restore_exception_state, epcr, esr));
            m.Return(0, 0);
        }

        private void RewriteSfxx(Func<Expression, Expression, Expression> fn) 
        {
            var left = OpOrZero(0);
            var right = OpOrZero(1);
            var dst = binder.EnsureFlagGroup(Registers.F);
            m.Assign(dst, fn(left, right));
        }

        private void RewriteSff(IntrinsicProcedure intrinsic)
        {
            var exp = OpOrZero(0);
            var f = binder.EnsureFlagGroup(Registers.F);
            m.Assign(f, m.Fn(intrinsic, exp));
        }

        private void RewriteShift(Func<Expression, Expression, Expression> fn)
        {
            var left = OpOrZero(1);
            var right = OpOrZero(2);
            var dst = Op(0);
            m.Assign(dst, fn(left, right));
        }

        private void RewriteShifti(Func<Expression, Expression, Expression> fn)
        {
            var left = OpOrZero(1);
            var right = ((Constant)instr.Operands[2]).ToInt32();
            var dst = Op(0);
            m.Assign(dst, fn(left, m.Int32(right)));
        }

        private void RewriteEntri()
        {
            // F/I in docs
            var pushRegs = ((Constant)instr.Operands[0]).ToUInt32();
            // N/J in docs
            var stackSlots = ((Constant)instr.Operands[1]).ToUInt32();

            // r1 is stack pointer
            var stackPtr = binder.EnsureRegister(Registers.GpRegisters[1]);

            for (int i = 0; i < pushRegs; i++) {
                var ea = m.AddSubSignedInt(stackPtr, i * -4);
                var reg = binder.EnsureRegister(Registers.GpRegisters[9 + i]);
                m.Assign(m.Mem32(ea), reg);
            }

            var newStackPtr = m.ISub(stackPtr, (pushRegs + stackSlots) * 4);
            m.Assign(stackPtr, newStackPtr);
        }

        private void RewriteRtnei()
        {
            // F/I in docs
            var popRegs = ((Constant)instr.Operands[0]).ToUInt32();
            // N/J in docs
            var stackSlots = ((Constant)instr.Operands[1]).ToUInt32();

            // r1 is stack pointer
            var stackPtr = binder.EnsureRegister(Registers.GpRegisters[1]);

            for (int i = 0; i < popRegs; i++) {
                var ea = m.AddSubSignedInt(stackPtr, (popRegs - i - 1 + stackSlots) * 4);
                var reg = binder.EnsureRegister(Registers.GpRegisters[9 + i]);
                m.Assign(reg, m.Mem32(ea));
            }

            var newStackPtr = m.IAdd(stackPtr, (popRegs + stackSlots) * 4);
            m.Assign(stackPtr, newStackPtr);
        }

        private void RewriteSideEffect(IntrinsicProcedure intrinsic)
        {
            var args = new List<Expression> { };
            for (int iop = 0; iop < instr.Operands.Length; ++iop)
            {
                args.Add(OpOrZero(iop));
            }
            m.SideEffect(m.Fn(intrinsic, args.ToArray()));
        }

        private void RewriteIntrinsic(IntrinsicProcedure intrinsic, params DataType[] dts)
        {
            if (dts.Length > 0)
            {
                intrinsic = intrinsic.MakeInstance(dts);
            }
            List<Expression> args = [];
            if (intrinsic.ReturnType is null || intrinsic.ReturnType is VoidType)
            {
                for (int iop = 0; iop < instr.Operands.Length; ++iop)
                {
                    args.Add(OpOrZero(iop));
                }
                m.SideEffect(m.Fn(intrinsic, args.ToArray()));
            }
            else
            {
                for (int iop = 1; iop < instr.Operands.Length; ++iop)
                {
                    args.Add(OpOrZero(iop));
                }
                m.Assign(Op(0), m.Fn(intrinsic, args.ToArray()));
            }
        }

        private void RewriteStore(PrimitiveType dt)
        {
            var src = OpOrZero(1);
            var dst = Op(0);
            MaybeSlice(dt, dst, src);
        }

        //$TODO: remove this once all instructions are known. It
        // is a globally mutable cache, which will cause race
        // conditions in multithreaded environments.
        private readonly static ConcurrentDictionary<Mnemonic, IntrinsicProcedure> intrinsics = new();

        private void RewriteUnknown()
        {
            var args = new List<Expression> { };
            for (int iop = 0; iop < instr.Operands.Length; ++iop)
            {
                args.Add(Op(iop));
            }
            IntrinsicProcedure? intrinsic;
            while (!intrinsics.TryGetValue(instr.Mnemonic, out intrinsic))
            {
                var ib = new IntrinsicBuilder(instr.MnemonicAsString, true);
                foreach (var e in args)
                {
                    ib.Param(e.DataType);
                }
                intrinsic = ib.Void();
                if (intrinsics.TryAdd(instr.Mnemonic, intrinsic))
                    break;
            }
            m.SideEffect(m.Fn(intrinsic, args.ToArray()));
        }

        private static readonly IntrinsicProcedure add_s_intrinsic = IntrinsicBuilder.SideEffect("__add.s?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure addc_s_lwq_intrinsic = IntrinsicBuilder.SideEffect("__addc_s_lwq?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure addc_s_lwqq_intrinsic = IntrinsicBuilder.SideEffect("__addc_s_lwqq?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);

        private static readonly IntrinsicProcedure addcn_s_intrinsic = IntrinsicBuilder.SideEffect("__addcn_s")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure addcnqq_s_intrinsic = IntrinsicBuilder.SideEffect("__addcnqq_s")
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure addcqq_s_intrinsic = IntrinsicBuilder.SideEffect("__addcqq_s")
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure addp_s_intrinsic = IntrinsicBuilder.SideEffect("__addp_s")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amac_intrinsic = IntrinsicBuilder.SideEffect("__amac?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amac_hh_intrinsic = IntrinsicBuilder.SideEffect("__amac_hh?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amac_lh_intrinsic = IntrinsicBuilder.SideEffect("__amac_lh?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amac_ll_intrinsic = IntrinsicBuilder.SideEffect("__amac_ll?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amac_wh_intrinsic = IntrinsicBuilder.SideEffect("__amac_wh?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amac_wl_intrinsic = IntrinsicBuilder.SideEffect("__amac_wl?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amacq_hl_intrinsic = IntrinsicBuilder.SideEffect("__amacq_hl?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amacr_intrinsic = IntrinsicBuilder.SideEffect("__amacr?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amacr_ex_intrinsic = IntrinsicBuilder.SideEffect("__amacr_ex?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amacrq_intrinsic = IntrinsicBuilder.SideEffect("__amacrq?")
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amacw_intrinsic = IntrinsicBuilder.SideEffect("__amacw?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amadd_intrinsic = IntrinsicBuilder.SideEffect("__amadd?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amsb_intrinsic = IntrinsicBuilder.SideEffect("__amsb?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amsb_lh_intrinsic = IntrinsicBuilder.SideEffect("__amsb_lh?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amsb_wh_intrinsic = IntrinsicBuilder.SideEffect("__amsb_wh?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amsb_wl_intrinsic = IntrinsicBuilder.SideEffect("__amsb_wl?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amsub_hl_intrinsic = IntrinsicBuilder.SideEffect("__amsub_hl?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amsub_wl_intrinsic = IntrinsicBuilder.SideEffect("__amsub_wl?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amul_intrinsic = IntrinsicBuilder.SideEffect("__amul?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amul_lh_intrinsic = IntrinsicBuilder.SideEffect("__amul_lh?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amul_ll_intrinsic = IntrinsicBuilder.SideEffect("__amul_lh?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amul_wh_intrinsic = IntrinsicBuilder.SideEffect("__amul_wh?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amul_wl_intrinsic = IntrinsicBuilder.SideEffect("__amul_wl?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure amulqu_intrinsic = IntrinsicBuilder.SideEffect("__amulqu?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure bsa_intrinsic = IntrinsicBuilder.SideEffect("__bsa?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure bsa_s_intrinsic = IntrinsicBuilder.SideEffect("__bsa_s?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure bsl_intrinsic = IntrinsicBuilder.SideEffect("__bsl?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure btb_intrinsic = IntrinsicBuilder.SideEffect("__btb?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();

        private static readonly IntrinsicProcedure chk_ll_intrinsic = IntrinsicBuilder.SideEffect("__chk_ll?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure chk_lu_intrinsic = IntrinsicBuilder.SideEffect("__chk_lu?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure conjc_intrinsic = IntrinsicBuilder.SideEffect("__conjc?")
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure crc_le_intrinsic = IntrinsicBuilder.SideEffect("__crc_le?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);

        private static readonly IntrinsicProcedure dep_intrinsic = IntrinsicBuilder.SideEffect("__dep")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure df2sf_intrinsic = IntrinsicBuilder.SideEffect("__df2sf?")
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure di_intrinsic = new IntrinsicBuilder("__disable_interrupts", true)
            .Void();
        private static readonly IntrinsicProcedure divl_intrinsic = IntrinsicBuilder.SideEffect("__divl?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure divlu_intrinsic = IntrinsicBuilder.SideEffect("__divlu?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure dma_op_intrinsic = IntrinsicBuilder.SideEffect("__dma_op")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure dsl_intrinsic = IntrinsicBuilder.SideEffect("__dsl?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure dsr_intrinsic = IntrinsicBuilder.SideEffect("__dsr?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);

        private static readonly IntrinsicProcedure dsli_intrinsic = IntrinsicBuilder.SideEffect("__dsli")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure dsri_intrinsic = IntrinsicBuilder.SideEffect("__dsri")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);

        private static readonly IntrinsicProcedure ei_intrinsic = new IntrinsicBuilder("__enable_interrupts", true)
            .Void();
        private static readonly IntrinsicProcedure exp_intrinsic = new IntrinsicBuilder("__exp", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure exp16_intrinsic = new IntrinsicBuilder("__exp16", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure ext_intrinsic = new IntrinsicBuilder("__extract_bits", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure extu_intrinsic = new IntrinsicBuilder("__extract_unsigned_bits", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);

        private static readonly IntrinsicProcedure fft_intrinsic = new IntrinsicBuilder("__fft", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure fl1_intrinsic = new IntrinsicBuilder("__fl1", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure flb_intrinsic = new IntrinsicBuilder("__flb", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure flush_invalidate_intrinsic = new IntrinsicBuilder("__flush_invalidate", true)
            .Param(PrimitiveType.Ptr32)
            .Void();
        private static readonly IntrinsicProcedure flush_line_intrinsic = new IntrinsicBuilder("__flush_line", true)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.UInt32)
            .Void();

        private static readonly IntrinsicProcedure invalidate_intrinsic = new IntrinsicBuilder("__invalidate_line", true)
            .Param(PrimitiveType.Ptr32)
            .Void();

        private static readonly IntrinsicProcedure invalidate_line_intrinsic = new IntrinsicBuilder("__invalidate_line", true)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.UInt32)
            .Void();

        private static readonly IntrinsicProcedure ldww_intrinsic = new IntrinsicBuilder("__ldww?", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure ldww2_intrinsic = new IntrinsicBuilder("__ldww2?", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure ldww2x_intrinsic = new IntrinsicBuilder("__ldww2x?", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure ldwwx_intrinsic = new IntrinsicBuilder("__ldwwx?", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);

        //$TODO: what does this actually do?
        private static readonly IntrinsicProcedure loop_intrinsic = new IntrinsicBuilder("__loop?", true)
            .Param(PrimitiveType.Ptr32)
            .Param(PrimitiveType.UInt32)
            .Void();

        private static readonly IntrinsicProcedure maddsf_intrinsic = new IntrinsicBuilder("__maddsf?", true)
            .Param(PrimitiveType.Real32)
            .Param(PrimitiveType.Real32)
            .Returns(PrimitiveType.Real32);
        private static readonly IntrinsicProcedure mfspr_intrinsic = new IntrinsicBuilder("__move_from_spr", true)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure mtspr_intrinsic = new IntrinsicBuilder("__move_to_spr", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure mlwz_intrinsic = new IntrinsicBuilder("__mlwz?", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure msw_intrinsic = new IntrinsicBuilder("__msw?", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure muladdh_instrinsic = new IntrinsicBuilder("__muladdh", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure muladdhx_instrinsic = new IntrinsicBuilder("__muladdhx", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure mulsubh_instrinsic = new IntrinsicBuilder("__mulsubh", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure mulsubx_instrinsic = new IntrinsicBuilder("__mulsubhx", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);

        private static readonly IntrinsicProcedure pack_s_instrinsic = new IntrinsicBuilder("__pack_s?", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure pamac_instrinsic = new IntrinsicBuilder("__pamac?", false)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure pamacrq_instrinsic = new IntrinsicBuilder("__pamacrq?", false)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure push_instrinsic = new IntrinsicBuilder("__push?", true)
            .Param(PrimitiveType.Word32)
            .Void();

        // used in implementation of bt.rfe
        private static readonly IntrinsicProcedure restore_exception_state = new IntrinsicBuilder("__restore_exception_state", true)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure return_intrinsic = IntrinsicBuilder.SideEffect("__return?")
            .Void();

        private static readonly IntrinsicProcedure sat_intrinsic = IntrinsicBuilder.SideEffect("__sat")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure satsu_intrinsic = IntrinsicBuilder.SideEffect("__satsu")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure satu_intrinsic = IntrinsicBuilder.SideEffect("__satu")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure satus_intrinsic = IntrinsicBuilder.SideEffect("__satus")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure sf2df_intrinsic = IntrinsicBuilder.SideEffect("__sf2df")
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure sfgtsi_minus32769_intrinsic = IntrinsicBuilder.SideEffect("__sfgtsi_minus32769")
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Bool);
        private static readonly IntrinsicProcedure sfgtui_minus32769_intrinsic = IntrinsicBuilder.SideEffect("__sfgtui_minus32769")
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Bool);
        private static readonly IntrinsicProcedure srarzi_intrinsic = IntrinsicBuilder.SideEffect("__srarzi?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure sub_s_intrinsic = IntrinsicBuilder.SideEffect("__sub_s")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure sub_s_lwq_intrinsic = IntrinsicBuilder.SideEffect("__sub_s")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure subcn_s_intrinsic = IntrinsicBuilder.SideEffect("__subcn_s")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure subcn_s_lwq_intrinsic = IntrinsicBuilder.SideEffect("__subcn_s_lwq")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure subcnqq_s_intrinsic = IntrinsicBuilder.SideEffect("__subcnqq_s")
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure subcqq_s_intrinsic = IntrinsicBuilder.SideEffect("__subcqq_s")
            .Param(PrimitiveType.Word32)
            .Void();
        private static readonly IntrinsicProcedure subp_s_intrinsic = IntrinsicBuilder.SideEffect("__subp_s")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Returns(PrimitiveType.Word32);
        private static readonly IntrinsicProcedure sys_intrinsic = new IntrinsicBuilder("__sys", true)
            .Void();
        private static readonly IntrinsicProcedure syncp_intrinsic = new IntrinsicBuilder("__syncp", true)
            .Void();
        private static readonly IntrinsicProcedure syncwritebuffer_intrinsic = new IntrinsicBuilder("__syncwritebuffer", true)
            .Void();

        private static readonly IntrinsicProcedure trap_intrinsic = new IntrinsicBuilder("__trap", true)
            .Param(PrimitiveType.Word32)
            .Void();

        private static readonly IntrinsicProcedure mv_opv_intrinsic = IntrinsicBuilder.SideEffect("__mv_opv?")
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Param(PrimitiveType.Word32)
            .Void();
    }
}
