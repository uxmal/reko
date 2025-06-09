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

#include "stdafx.h"
#include "reko.h"

#include "functions.h"
#include "ComBase.h"
#include "ArmRewriter.h"


ArmRewriter::ArmRewriter(
	cs_mode mode,
	const uint8_t * rawBytes,
	size_t availableBytes,
	uint64_t address,
	INativeRtlEmitter * emitter,
	INativeTypeFactory * ntf,
	INativeRewriterHost * host)
:
	rawBytes(rawBytes),
	available(availableBytes),
	address(address),
	m(*emitter),
	ntf(*ntf),
	host(host),
	instr(nullptr)
{
	//Dump(".ctor: %08x", this);
	auto ec = cs_open(CS_ARCH_ARM, mode, &hcapstone); 
	ec = cs_option(hcapstone, CS_OPT_DETAIL, CS_OPT_ON);
	this->instr = cs_malloc(hcapstone);
	++s_count;
}


STDMETHODIMP ArmRewriter::QueryInterface(REFIID riid, void ** ppvOut)
{
	//Dump("QI: %08x %d", this, cRef);
	*ppvOut = nullptr;
	if (riid == IID_IUnknown || riid == IID_INativeRewriter)
	{
		AddRef();
		*ppvOut = static_cast<INativeRewriter *>(this);
		return S_OK;
	}
	return E_NOINTERFACE;
}

STDMETHODIMP_(int32_t) ArmRewriter::Next()
{
	if (available <= 0)
		return S_FALSE;			// No more work to do.
	auto addrInstr = address;
	bool f = cs_disasm_iter(hcapstone, &rawBytes, &available, &address, instr);
	if (!f)
	{
		// Failed to disassemble the instruction because it was invalid.
		m.Invalid();
		address += 4;
		available -= 4;
		m.FinishCluster(InstrClass::Invalid, addrInstr, 4);
		return S_OK;
	}
	// Most instructions are linear.
	rtlClass = InstrClass::Linear;
	
	// Most instructions have a conditional mode of operation.
	//$TODO: make sure non-conditional instructions are handled correctly here.
	ConditionalSkip(false);	
	switch (instr->id)
	{
	default:
	case ARM_INS_AESD:
	case ARM_INS_AESE:
	case ARM_INS_AESIMC:
	case ARM_INS_AESMC:
	case ARM_INS_BKPT:
	case ARM_INS_BXJ:
	case ARM_INS_CLREX:
	case ARM_INS_CRC32B:
	case ARM_INS_CRC32CB:
	case ARM_INS_CRC32CH:
	case ARM_INS_CRC32CW:
	case ARM_INS_CRC32H:
	case ARM_INS_CRC32W:
	case ARM_INS_DBG:
	case ARM_INS_DSB:
	case ARM_INS_FLDMDBX:
	case ARM_INS_FLDMIAX:
	case ARM_INS_FSTMDBX:
	case ARM_INS_FSTMIAX:
	case ARM_INS_HLT:
	case ARM_INS_ISB:
	case ARM_INS_LDA:
	case ARM_INS_LDAB:
	case ARM_INS_LDAEX:
	case ARM_INS_LDAEXB:
	case ARM_INS_LDAEXD:
	case ARM_INS_LDAEXH:
	case ARM_INS_LDAH:
	case ARM_INS_LDREXB:
	case ARM_INS_LDREXD:
	case ARM_INS_LDREXH:
	case ARM_INS_MCR2:
	case ARM_INS_MCRR:
	case ARM_INS_MCRR2:
	case ARM_INS_MRC2:
	case ARM_INS_MRRC:
	case ARM_INS_MRRC2:
	case ARM_INS_PKHBT:
	case ARM_INS_PKHTB:
	case ARM_INS_PLDW:
	case ARM_INS_PLD:
	case ARM_INS_PLI:
	case ARM_INS_QASX:
	case ARM_INS_QSAX:
	case ARM_INS_QSUB8:
	case ARM_INS_RBIT:
	case ARM_INS_REV16:
	case ARM_INS_REVSH:
	case ARM_INS_RFEDA:
	case ARM_INS_RFEDB:
	case ARM_INS_RFEIA:
	case ARM_INS_RFEIB:
	case ARM_INS_SASX:
	case ARM_INS_SEL:
	case ARM_INS_SETEND:
	case ARM_INS_SHA1C:
	case ARM_INS_SHA1H:
	case ARM_INS_SHA1M:
	case ARM_INS_SHA1P:
	case ARM_INS_SHA1SU0:
	case ARM_INS_SHA1SU1:
	case ARM_INS_SHA256H:
	case ARM_INS_SHA256H2:
	case ARM_INS_SHA256SU0:
	case ARM_INS_SHA256SU1:
	case ARM_INS_SHADD16:
	case ARM_INS_SHADD8:
	case ARM_INS_SHASX:
	case ARM_INS_SHSAX:
	case ARM_INS_SHSUB16:
	case ARM_INS_SHSUB8:
	case ARM_INS_SMC:
	case ARM_INS_SMLSD:
	case ARM_INS_SMLSDX:
	case ARM_INS_SMMLA:
	case ARM_INS_SMMLAR:
	case ARM_INS_SMMLS:
	case ARM_INS_SMMLSR:
	case ARM_INS_SMMUL:
	case ARM_INS_SMMULR:
	case ARM_INS_SMUAD:
	case ARM_INS_SMUADX:
	case ARM_INS_SMUSD:
	case ARM_INS_SMUSDX:
	case ARM_INS_SRSDA:
	case ARM_INS_SRSDB:
	case ARM_INS_SRSIA:
	case ARM_INS_SRSIB:
	case ARM_INS_SSAT:
	case ARM_INS_SSAT16:
	case ARM_INS_SSAX:
	case ARM_INS_STL:
	case ARM_INS_STLB:
	case ARM_INS_STLEX:
	case ARM_INS_STLEXB:
	case ARM_INS_STLEXD:
	case ARM_INS_STLEXH:
	case ARM_INS_STLH:
	case ARM_INS_STREXB:
	case ARM_INS_STREXD:
	case ARM_INS_STREXH:
	case ARM_INS_SXTAB16:
	case ARM_INS_SXTB16:
	case ARM_INS_UASX:
	case ARM_INS_UHADD16:
	case ARM_INS_UHADD8:
	case ARM_INS_UHASX:
	case ARM_INS_UHSAX:
	case ARM_INS_UHSUB16:
	case ARM_INS_UHSUB8:
	case ARM_INS_UQADD16:
	case ARM_INS_UQADD8:
	case ARM_INS_UQASX:
	case ARM_INS_UQSAX:
	case ARM_INS_UQSUB16:
	case ARM_INS_UQSUB8:
	case ARM_INS_USAD8:
	case ARM_INS_USADA8:
	case ARM_INS_USAT:
	case ARM_INS_USAT16:
	case ARM_INS_USAX:
	case ARM_INS_UXTAB16:
	case ARM_INS_UXTB16:
	case ARM_INS_VABAL:
	case ARM_INS_VABA:
	case ARM_INS_VABDL:
	case ARM_INS_VABD:
	case ARM_INS_VACGE:
	case ARM_INS_VACGT:
	case ARM_INS_VADDHN:
	case ARM_INS_VADDL:
	case ARM_INS_VADDW:
	case ARM_INS_VBIC:
	case ARM_INS_VBIF:
	case ARM_INS_VBIT:
	case ARM_INS_VBSL:
	case ARM_INS_VCEQ:
	case ARM_INS_VCGE:
	case ARM_INS_VCGT:
	case ARM_INS_VCLE:
	case ARM_INS_VCLS:
	case ARM_INS_VCLT:
	case ARM_INS_VCLZ:
	case ARM_INS_VCNT:
	case ARM_INS_VCVTA:
	case ARM_INS_VCVTB:
	case ARM_INS_VCVTM:
	case ARM_INS_VCVTN:
	case ARM_INS_VCVTP:
	case ARM_INS_VCVTT:
	case ARM_INS_VFMA:
	case ARM_INS_VFMS:
	case ARM_INS_VFNMA:
	case ARM_INS_VFNMS:
	case ARM_INS_VLD1:
	case ARM_INS_VLD2:
	case ARM_INS_VLD3:
	case ARM_INS_VLD4:
	case ARM_INS_VLDMDB:
	case ARM_INS_VMAXNM:
	case ARM_INS_VMINNM:
	case ARM_INS_VMLAL:
	case ARM_INS_VMLSL:
	case ARM_INS_VMOVL:
	case ARM_INS_VMOVN:
	case ARM_INS_VMSR:
	case ARM_INS_VMULL:
	case ARM_INS_VORN:
	case ARM_INS_VPADAL:
	case ARM_INS_VPADDL:
	case ARM_INS_VQDMLAL:
	case ARM_INS_VQDMLSL:
	case ARM_INS_VQDMULH:
	case ARM_INS_VQDMULL:
	case ARM_INS_VQMOVUN:
	case ARM_INS_VQMOVN:
	case ARM_INS_VQNEG:
	case ARM_INS_VQRDMULH:
	case ARM_INS_VQRSHL:
	case ARM_INS_VQRSHRN:
	case ARM_INS_VQRSHRUN:
	case ARM_INS_VQSHLU:
	case ARM_INS_VQSHRN:
	case ARM_INS_VQSHRUN:
	case ARM_INS_VQSUB:
	case ARM_INS_VRADDHN:
	case ARM_INS_VRECPE:
	case ARM_INS_VRECPS:
	case ARM_INS_VREV16:
	case ARM_INS_VREV32:
	case ARM_INS_VREV64:
	case ARM_INS_VRHADD:
	case ARM_INS_VRINTA:
	case ARM_INS_VRINTM:
	case ARM_INS_VRINTN:
	case ARM_INS_VRINTP:
	case ARM_INS_VRINTR:
	case ARM_INS_VRINTX:
	case ARM_INS_VRINTZ:
	case ARM_INS_VRSHRN:
	case ARM_INS_VRSQRTE:
	case ARM_INS_VRSQRTS:
	case ARM_INS_VRSRA:
	case ARM_INS_VRSUBHN:
	case ARM_INS_VSELEQ:
	case ARM_INS_VSELGE:
	case ARM_INS_VSELGT:
	case ARM_INS_VSELVS:
	case ARM_INS_VSHLL:
	case ARM_INS_VSHRN:
	case ARM_INS_VSHR:
	case ARM_INS_VSLI:
	case ARM_INS_VSRA:
	case ARM_INS_VSRI:
	case ARM_INS_VST1:
	case ARM_INS_VST2:
	case ARM_INS_VST3:
	case ARM_INS_VST4:
	case ARM_INS_VSTMDB:
	case ARM_INS_VSUBHN:
	case ARM_INS_VSUBL:
	case ARM_INS_VSUBW:
	case ARM_INS_VSWP:
	case ARM_INS_VTBL:
	case ARM_INS_VTBX:
	case ARM_INS_VCVTR:
	case ARM_INS_VTRN:
	case ARM_INS_VTST:
	case ARM_INS_VUZP:
	case ARM_INS_VZIP:
	case ARM_INS_DCPS1:
	case ARM_INS_DCPS2:
	case ARM_INS_DCPS3:
	case ARM_INS_LSRS:
	case ARM_INS_ROR:
	case ARM_INS_RRX:
	case ARM_INS_SUBS:
	case ARM_INS_TBB:
	case ARM_INS_TBH:
	case ARM_INS_MOVS:
	case ARM_INS_WFE:
	case ARM_INS_WFI:
	case ARM_INS_SEV:
	case ARM_INS_SEVL:
		NotImplementedYet();
		break;
	case ARM_INS_ADC: RewriteAdcSbc(&INativeRtlEmitter::IAdd, false); break;
	case ARM_INS_ADD: RewriteBinOp(&INativeRtlEmitter::IAdd); break;
	case ARM_INS_ADDW: RewriteAddw(); break;
	case ARM_INS_ADR: RewriteAdr(); break;
	case ARM_INS_AND: RewriteLogical([](auto & m, auto a, auto b) { return m.And(a, b); }); break;
	case ARM_INS_ASR: RewriteShift([](auto & m, auto a, auto b) { return m.Sar(a, b); }); break;
	case ARM_INS_ASRS: RewriteShift([](auto & m, auto a, auto b) { return m.Sar(a, b); }); break;
	case ARM_INS_B: RewriteB(false); break;
	case ARM_INS_BFC: RewriteBfc(); break;
	case ARM_INS_BFI: RewriteBfi(); break;
	case ARM_INS_BIC: RewriteBic(); break;
	case ARM_INS_BL: RewriteB(true); break;
	case ARM_INS_BLX: RewriteB(true); break;
	case ARM_INS_BX: RewriteB(false); break;
	case ARM_INS_CBZ: RewriteCbnz([](auto & m, auto a) { return m.Eq0(a); }); break;
	case ARM_INS_CBNZ: RewriteCbnz([](auto & m, auto a) { return m.Ne0(a); }); break;
	case ARM_INS_CDP: RewriteCdp("__cdp"); break;
	case ARM_INS_CDP2: RewriteCdp("__cdp2"); break;
	case ARM_INS_CLZ: RewriteClz(); break;
	case ARM_INS_CMN: RewriteCmp(&INativeRtlEmitter::IAdd); break;
	case ARM_INS_CMP: RewriteCmp(&INativeRtlEmitter::ISub); break;
	case ARM_INS_CPS: RewriteCps(); break;
	case ARM_INS_DMB: RewriteDmb(); break;
	case ARM_INS_EOR: RewriteLogical([](auto & m, auto a, auto b) { return m.Xor(a, b); }); break;
	case ARM_INS_HINT: RewriteHint(); break;
	case ARM_INS_IT: RewriteIt(); return S_OK;
	case ARM_INS_LDC2L: RewriteLdc("__ldc2l"); break;
	case ARM_INS_LDC2: RewriteLdc("__ldc2"); break;
	case ARM_INS_LDCL: RewriteLdc("__ldcl"); break;
	case ARM_INS_LDC: RewriteLdc("__ldc"); break;
	case ARM_INS_LDM: RewriteLdm(0, &INativeRtlEmitter::IAdd); break;
	case ARM_INS_LDMDA: RewriteLdm(0, &INativeRtlEmitter::ISub); break;
	case ARM_INS_LDMDB: RewriteLdm(-4, &INativeRtlEmitter::ISub); break;
	case ARM_INS_LDMIB: RewriteLdm(4, &INativeRtlEmitter::IAdd); break;
	case ARM_INS_LDR: RewriteLdr(BaseType::Word32, BaseType::Word32); break;
	case ARM_INS_LDRT: RewriteLdr(BaseType::Word32, BaseType::Word32); break;
	case ARM_INS_LDRB: RewriteLdr(BaseType::Word32, BaseType::Byte); break;
	case ARM_INS_LDRBT: RewriteLdr(BaseType::Word32, BaseType::Byte); break;
	case ARM_INS_LDRH: RewriteLdr(BaseType::Word32, BaseType::UInt16); break;
	case ARM_INS_LDRHT: RewriteLdr(BaseType::Word32, BaseType::UInt16); break;
	case ARM_INS_LDRSB: RewriteLdr(BaseType::Word32, BaseType::SByte); break;
	case ARM_INS_LDRSBT: RewriteLdr(BaseType::Word32, BaseType::SByte); break;
	case ARM_INS_LDRSH: RewriteLdr(BaseType::Word32, BaseType::Int16); break;
	case ARM_INS_LDRSHT: RewriteLdr(BaseType::Word32, BaseType::Int16); break;
	case ARM_INS_LDRD: RewriteLdrd(); break;
	case ARM_INS_LDREX: RewriteLdrex(); break;
	case ARM_INS_LSL: RewriteShift([](auto & m, auto a, auto b) { return m.Shl(a, b); }); break;
	case ARM_INS_LSR: RewriteShift([](auto & m, auto a, auto b) { return m.Shr(a, b); }); break;
	case ARM_INS_NOP: m.Nop(); break;
	case ARM_INS_MCR: RewriteMcr(); break;
	case ARM_INS_MLA: RewriteMultiplyAccumulate(&INativeRtlEmitter::IAdd); break;
	case ARM_INS_MLS: RewriteMultiplyAccumulate(&INativeRtlEmitter::ISub); break;
	case ARM_INS_MOV: RewriteMov(); break;
	case ARM_INS_MOVT: RewriteMovt(); break;
	case ARM_INS_MOVW: RewriteMovw(); break;
	case ARM_INS_MRC: RewriteMrc(); break;
	case ARM_INS_MRS: RewriteMrs(); break;
	case ARM_INS_MSR: RewriteMsr(); break;
	case ARM_INS_MUL: RewriteBinOp(&INativeRtlEmitter::IMul); break;
	case ARM_INS_MVN: RewriteUnaryOp(&INativeRtlEmitter::Comp); break;
	case ARM_INS_ORN: RewriteLogical([](auto & m, auto a, auto b) { return m.Or(a, m.Comp(b)); }); break;
	case ARM_INS_ORR: RewriteLogical([](auto & m, auto a, auto b) { return m.Or(a, b); }); break;
	case ARM_INS_QADD: RewriteQAddSub(&INativeRtlEmitter::IAdd); break;
	case ARM_INS_QADD16: RewriteVectorBinOp("__qadd_%s", ARM_VECTORDATA_S16); break;
	case ARM_INS_QADD8: RewriteVectorBinOp("__qadd_%s", ARM_VECTORDATA_S8); break;
	case ARM_INS_QDADD: RewriteQDAddSub(&INativeRtlEmitter::IAdd); break;
	case ARM_INS_QDSUB: RewriteQDAddSub(&INativeRtlEmitter::ISub); break;
	case ARM_INS_QSUB: RewriteQAddSub(&INativeRtlEmitter::ISub); break;
	case ARM_INS_QSUB16: RewriteVectorBinOp("__qsub_%s", ARM_VECTORDATA_S16); break;
	case ARM_INS_POP: RewritePop(); break;
	case ARM_INS_PUSH: RewritePush(); break;
	case ARM_INS_REV: RewriteRev(); break;
	case ARM_INS_RSB: RewriteRevBinOp(&INativeRtlEmitter::ISub, instr->detail->arm.update_flags); break;
	case ARM_INS_RSC: RewriteAdcSbc(&INativeRtlEmitter::ISub, true); break;
	case ARM_INS_SADD16: RewriteVectorBinOp("__sadd16", ARM_VECTORDATA_S16); break;
	case ARM_INS_SADD8: RewriteVectorBinOp("__sadd8", ARM_VECTORDATA_S8); break;
	case ARM_INS_SBC: RewriteAdcSbc(&INativeRtlEmitter::ISub, false); break;
	case ARM_INS_SBFX: RewriteSbfx(); break;
	case ARM_INS_SDIV: RewriteDiv(&INativeRtlEmitter::SDiv); break;
	case ARM_INS_SMLABB: RewriteMla(false, false, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMLABT: RewriteMla(false, true, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMLALBB: RewriteMlal(false, false, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMLALBT: RewriteMlal(false, true, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMLALD: RewriteMlxd(false, BaseType::Int16, &INativeRtlEmitter::SMul, &INativeRtlEmitter::IAdd); break;
	case ARM_INS_SMLALDX: RewriteMlxd(true, BaseType::Int16, &INativeRtlEmitter::SMul, &INativeRtlEmitter::IAdd); break;
	case ARM_INS_SMLALTB: RewriteMlal(true, false, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMLALTT: RewriteMlal(true, true, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMLAL: RewriteSmlal(); break;
	case ARM_INS_SMLATB: RewriteMla(true, false, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMLATT: RewriteMla(true, true, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMLAWB: RewriteSmlaw(false); break;
	case ARM_INS_SMLAWT: RewriteSmlaw(true); break;
	case ARM_INS_SMLSLD: RewriteMlxd(false, BaseType::Int16, &INativeRtlEmitter::SMul, &INativeRtlEmitter::ISub); break;
	case ARM_INS_SMLSLDX: RewriteMlxd(true, BaseType::Int16, &INativeRtlEmitter::SMul, &INativeRtlEmitter::ISub); break;
	case ARM_INS_SMULBB: RewriteMulbb(false, false, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMULBT: RewriteMulbb(false, true, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMULWB: RewriteMulw(false); break;
	case ARM_INS_SMULWT: RewriteMulw(true); break;
	case ARM_INS_SMULTB: RewriteMulbb(true, false, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMULTT: RewriteMulbb(true, true, BaseType::Int16, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SMULL: RewriteMull(BaseType::Int64, &INativeRtlEmitter::SMul); break;
	case ARM_INS_SSUB16: RewriteVectorBinOp("__ssub16", ARM_VECTORDATA_S16); break;
	case ARM_INS_SSUB8: RewriteVectorBinOp("__ssub8", ARM_VECTORDATA_S8); break;
	case ARM_INS_STC2L: RewriteStc("__stc2l"); break;
	case ARM_INS_STC2: RewriteStc("__stc2"); break;
	case ARM_INS_STC: RewriteStc("__stc"); break;
	case ARM_INS_STCL: RewriteStc("__stcl"); break;
	case ARM_INS_STM: RewriteStm(0, true); break;
	case ARM_INS_STMDB: RewriteStm(-4, false); break;
	case ARM_INS_STMDA: RewriteStm(0, false); break;
	case ARM_INS_STMIB: RewriteStm(4, true); break;
	case ARM_INS_STR: RewriteStr(BaseType::Word32); break;
	case ARM_INS_STRB: RewriteStr(BaseType::Byte); break;
	case ARM_INS_STRBT: RewriteStr(BaseType::Byte); break;
	case ARM_INS_STRD: RewriteStrd(); break;
	case ARM_INS_STREX: RewriteStrex(); break;
	case ARM_INS_STRH: RewriteStr(BaseType::UInt16); break;
	case ARM_INS_STRHT: RewriteStr(BaseType::UInt16); break;
	case ARM_INS_STRT: RewriteStr(BaseType::Word32); break;
	case ARM_INS_SUB: RewriteBinOp(&INativeRtlEmitter::ISub); break;
	case ARM_INS_SUBW: RewriteSubw(); break;
	case ARM_INS_SVC: RewriteSvc(); break;
	case ARM_INS_SWP: RewriteSwp(BaseType::Word32); break;
	case ARM_INS_SWPB: RewriteSwp(BaseType::Byte); break;
	case ARM_INS_SXTAB: RewriteXtab(BaseType::SByte); break;
	case ARM_INS_SXTAH: RewriteXtab(BaseType::Int16); break;
	case ARM_INS_SXTB: RewriteXtb(BaseType::SByte, BaseType::Int32); break;
	case ARM_INS_SXTH: RewriteXtb(BaseType::Int16, BaseType::Int32); break;
	case ARM_INS_TEQ: RewriteTeq(); break;
	case ARM_INS_TRAP: RewriteTrap(); break;
	case ARM_INS_TST: RewriteTst(); break;
	case ARM_INS_UADD16: RewriteVectorBinOp("__uadd16", ARM_VECTORDATA_I16); break;
	case ARM_INS_UADD8: RewriteVectorBinOp("__uadd8", ARM_VECTORDATA_I8); break;
	case ARM_INS_UBFX: RewriteUbfx(); break;
	case ARM_INS_UDF: RewriteUdf(); break;
	case ARM_INS_UDIV: RewriteDiv(&INativeRtlEmitter::UDiv); break;
	case ARM_INS_UMAAL: RewriteUmaal(); break;
	case ARM_INS_UMLAL: RewriteUmlal(); break;
	case ARM_INS_UMULL: RewriteMull(BaseType::UInt64, &INativeRtlEmitter::UMul); break;
	case ARM_INS_USUB16: RewriteVectorBinOp("__usub16", ARM_VECTORDATA_I16); break;
	case ARM_INS_USUB8: RewriteVectorBinOp("__usub8", ARM_VECTORDATA_I8); break;
	case ARM_INS_UXTAB: RewriteXtab(BaseType::Byte); break;
	case ARM_INS_UXTAH: RewriteXtab(BaseType::UInt16); break;
	case ARM_INS_UXTB: RewriteXtb(BaseType::Byte, BaseType::UInt32); break;
	case ARM_INS_UXTH: RewriteXtb(BaseType::UInt16, BaseType::UInt32); break;
	case ARM_INS_YIELD: RewriteYield(); break;



	case ARM_INS_VABS: RewriteVectorUnaryOp("__vabs_%s"); break;
	case ARM_INS_VADD: RewriteVectorBinOp("__vadd_%s"); break;
	case ARM_INS_VAND: RewriteVecBinOp(&INativeRtlEmitter::And); break;
	case ARM_INS_VCMP: RewriteVcmp(); break;
	case ARM_INS_VCMPE: RewriteVcmp(); break;
	case ARM_INS_VCVT: RewriteVcvt(); break;
	case ARM_INS_VDIV: RewriteVecBinOp(&INativeRtlEmitter::FDiv); break;
	case ARM_INS_VDUP: RewriteVdup(); break;
	case ARM_INS_VEOR: RewriteVecBinOp(&INativeRtlEmitter::Xor); break;
	case ARM_INS_VEXT: RewriteVext(); break;
	case ARM_INS_VHADD: RewriteVectorBinOp("__vhadd_%s"); break;
	case ARM_INS_VHSUB: RewriteVectorBinOp("__vhsub_%s"); break;
	case ARM_INS_VLDMIA: RewriteVldmia(); break;
	case ARM_INS_VLDR: RewriteVldr(); break;
	case ARM_INS_VMAX: RewriteVectorBinOp("__vmax_%s"); break;
	case ARM_INS_VMIN: RewriteVectorBinOp("__vmin_%s"); break;
	case ARM_INS_VMOV: RewriteVmov(); break;
	case ARM_INS_VMLA: RewriteVectorBinOp("__vmla_%s"); break;
	case ARM_INS_VMLS: RewriteVectorBinOp("__vmls_%s"); break;
	case ARM_INS_VMRS: RewriteVmrs(); break;
	case ARM_INS_VMVN: RewriteVmvn(); break;
	case ARM_INS_VMUL: RewriteVectorBinOp("__vmul_%s"); break;
	case ARM_INS_VORR: RewriteVecBinOp(&INativeRtlEmitter::Or); break;
	case ARM_INS_VNEG: RewriteVectorUnaryOp("__vneg_%s"); break;
	case ARM_INS_VNMLA: RewriteVectorBinOp("__vnmla_%s"); break;
	case ARM_INS_VNMLS: RewriteVectorBinOp("__vnmls_%s"); break;
	case ARM_INS_VNMUL: RewriteVectorBinOp("__vnmul_%s"); break;
	case ARM_INS_VPADD: RewriteVectorBinOp("__vpadd_%s"); break;
	case ARM_INS_VPMAX: RewriteVectorBinOp("__vpmax_%s"); break;
	case ARM_INS_VPMIN: RewriteVectorBinOp("__vpmin_%s"); break;
	case ARM_INS_VPOP: RewriteVpop(); break;
	case ARM_INS_VPUSH: RewriteVpush(); break;
	case ARM_INS_VQABS: RewriteVectorBinOp("__vqabs_%s"); break;
	case ARM_INS_VQADD: RewriteVectorBinOp("__vqadd_%s"); break;
	case ARM_INS_VQSHL: RewriteVectorBinOp("__vqshl_%s"); break;
	case ARM_INS_VRSHL: RewriteVectorBinOp("__vrshl_%s"); break;
	case ARM_INS_VRSHR: RewriteVectorBinOp("__vrshr_%s"); break;
	case ARM_INS_VSTMIA: RewriteVstmia(); break;
	case ARM_INS_VSQRT: RewriteVsqrt(); break;
	case ARM_INS_VSHL: RewriteVectorBinOp("__vshl_%s"); break;
	case ARM_INS_VSTR: RewriteVstr(); break;
	case ARM_INS_VSUB:  RewriteVectorBinOp("__vsub_%s"); break;
	}
	PostRewrite();
	m.FinishCluster(rtlClass, addrInstr, instr->size);
	return S_OK;
}

void ArmRewriter::NotImplementedYet()
{
	char buf[200];	//$TODO: hello buffer overflow!
	::snprintf(buf, sizeof(buf), "Rewriting ARM opcode '%s' is not supported yet.", instr->mnemonic);
	EmitUnitTest();
	host->Error(
		instr->address,
		buf);
	m.Invalid();
}

HExpr ArmRewriter::NZC()
{
	return host->EnsureFlagGroup((int)ARM_REG_CPSR, 0xE, "NZC");
}

HExpr ArmRewriter::NZCV()
{
	return host->EnsureFlagGroup((int)ARM_REG_CPSR, 0xF, "NZCV");
}

HExpr ArmRewriter::Q()
{
	return host->EnsureFlagGroup((int)ARM_REG_CPSR, 0x10, "Q");
}

void ArmRewriter::MaybeUpdateFlags(HExpr opDst)
{
	if (instr->detail->arm.update_flags)
	{
		m.Assign(NZCV(), m.Cond(opDst));
	}
}

void ArmRewriter::RewriteB(bool link)
{
	HExpr dst;
	bool dstIsAddress;
	if (Dst().type == ARM_OP_IMM)
	{
		dst = m.Ptr32(Dst().imm);
		dstIsAddress = true;
	}
	else
	{
		dst = Operand(Dst(), BaseType::Word32, true);
		dstIsAddress = false;
	}
	if (link)
	{
		rtlClass = (InstrClass)((int)InstrClass::Transfer | (int)InstrClass::Call);
		if (instr->detail->arm.cc == ARM_CC_AL)
		{
			m.Call(dst, 0);
		}
		else
		{
			rtlClass = InstrClass::ConditionalTransfer;
			ConditionalSkip(true);
			m.Call(dst, 0);
		}
	}
	else
	{
		if (instr->detail->arm.cc == ARM_CC_AL)
		{
			rtlClass = InstrClass::Transfer;
			m.Goto(dst);
		}
		else
		{
			rtlClass = InstrClass::ConditionalTransfer;
			if (dstIsAddress)
			{
				m.Branch(TestCond(instr->detail->arm.cc), dst, InstrClass::ConditionalTransfer);
			}
			else
			{
				ConditionalSkip(true);
				m.Goto(dst);
			}
		}
	}
}

void ArmRewriter::RewriteCbnz(HExpr(*ctor)(INativeRtlEmitter & m, HExpr e))
{
	rtlClass = InstrClass::ConditionalTransfer;
	auto cond = Operand(Dst(), BaseType::Word32, true);
	m.Branch(ctor(m, Operand(Dst())),
		m.Ptr32((uint32_t)Src1().imm),
		InstrClass::ConditionalTransfer);
}

// If a conditional ARM instruction is encountered, generate an IL
// instruction to skip the remainder of the instruction cluster.
void ArmRewriter::ConditionalSkip(bool force)
{
	auto cc = instr->detail->arm.cc;
	if (!force)
	{
		if (cc == ARM_CC_AL)
			return; // never skip!
		if (instr->id == ARM_INS_B ||
			instr->id == ARM_INS_BL ||
			instr->id == ARM_INS_BLX ||
			instr->id == ARM_INS_BX)
		{
			// These instructions handle the branching themselves.
			return;
		}
	}
	m.BranchInMiddleOfInstruction(
		TestCond(Invert(cc)),
		m.Ptr32(static_cast<uint32_t>(instr->address) + 4),
		InstrClass::ConditionalTransfer);
}

HExpr ArmRewriter::EffectiveAddress(const arm_op_mem & mem)
{
	auto baseReg = Reg(mem.base);
	auto ea = baseReg;
	if (mem.disp > 0)
	{
		ea = m.IAdd(ea, m.Int32(mem.disp));
	}
	else if (mem.disp < 0)
	{
		ea = m.ISub(ea, m.Int32(-mem.disp));
	}
	else if (mem.index != ARM_REG_INVALID)
	{
		ea = m.IAdd(ea, Reg(mem.index));
	}
	return ea;
}

arm_cc ArmRewriter::Invert(arm_cc cc)
{
	switch (cc)
	{
	case ARM_CC_EQ: return ARM_CC_NE;
	case ARM_CC_NE: return ARM_CC_EQ;
	case ARM_CC_HS: return ARM_CC_LO;
	case ARM_CC_LO: return ARM_CC_HS;
	case ARM_CC_MI: return ARM_CC_PL;
	case ARM_CC_PL: return ARM_CC_MI;
	case ARM_CC_VS: return ARM_CC_VC;
	case ARM_CC_VC: return ARM_CC_VS;
	case ARM_CC_HI: return ARM_CC_LS;
	case ARM_CC_LS: return ARM_CC_HI;
	case ARM_CC_GE: return ARM_CC_LT;
	case ARM_CC_LT: return ARM_CC_GE;
	case ARM_CC_GT: return ARM_CC_LE;
	case ARM_CC_LE: return ARM_CC_GT;
	case ARM_CC_AL: return ARM_CC_INVALID;
	}
	return ARM_CC_INVALID;
}

bool ArmRewriter::IsLastOperand(const cs_arm_op & op)
{
	return &op == &instr->detail->arm.operands[instr->detail->arm.op_count - 1];
}

HExpr ArmRewriter::Operand(const cs_arm_op & op, BaseType dt, bool write)
{
	switch (op.type)
	{
	case ARM_OP_REG:
	{
		if (!write && op.reg == ARM_REG_PC)
		{
			//$TODO: look at Thumb manual to see if the + 8 also applies.
			auto dst = (uint32_t)((int32_t)instr->address + op.mem.disp) + 8u;
			auto ea = m.Ptr32(dst);
			return ea;
		}
		auto reg = Reg(op.reg);
		return MaybeShiftOperand(reg, op);
	}
	case ARM_OP_SYSREG:
	{
		auto reg = op.reg;
		switch (reg)
		{
		case ARM_SYSREG_SPSR_C:
		case ARM_SYSREG_SPSR_X:
		case ARM_SYSREG_SPSR_S:
		case ARM_SYSREG_SPSR_F:
			reg = ARM_REG_SPSR;
			break;
		case ARM_SYSREG_CPSR_C:
		case ARM_SYSREG_CPSR_X:
		case ARM_SYSREG_CPSR_S:
		case ARM_SYSREG_CPSR_F:
			reg = ARM_REG_CPSR;
		}

		auto sysreg = host->EnsureRegister(1, reg);
		return sysreg;
	}
	case ARM_OP_IMM:
		return m.Word32(op.imm);
	case ARM_OP_CIMM:
		return m.Byte((uint8_t)op.imm);
	case ARM_OP_PIMM:
		return host->EnsureRegister(2, op.imm);
	case ARM_OP_MEM:
	{
		auto baseReg = Reg(op.mem.base);
		auto ea = baseReg;
		if (op.mem.base == ARM_REG_PC)
		{
			// PC-relative address
			auto dst = (uint32_t)((int32_t)instr->address + op.mem.disp) + 8u;
			ea = m.Ptr32(dst);

			if (op.mem.index != ARM_REG_INVALID)
			{
				auto ireg = Reg(op.mem.index);
				if (op.shift.type == ARM_SFT_LSL)
				{
					ea = m.IAdd(ea, m.IMul(ireg, m.Int32(1 << op.shift.value)));
				}
				else
				{
					//$TODO: handle these (unlikely) cases!
				}
			}
			return m.Mem(dt, ea);
		}
		if (op.mem.disp != 0 && IsLastOperand(op))
		{
			auto offset = m.Int32(op.mem.disp);
			ea = op.mem.scale < 0
				? m.ISub(ea, offset)
				: m.IAdd(ea, offset);
		}
		if (IsLastOperand(op) && instr->detail->arm.writeback)
		{
			m.Assign(baseReg, ea);
			ea = baseReg;
		}
		return m.Mem(SizeFromLoadStore(), ea);
	}
	}
	//$TODO
	return HExpr();
}

BaseType ArmRewriter::SizeFromLoadStore()
{
	switch (instr->id)
	{
	case ARM_INS_LDC: return BaseType::Word32;
	case ARM_INS_LDC2: return BaseType::Word32;
	case ARM_INS_LDC2L: return BaseType::Word32;
	case ARM_INS_LDCL: return BaseType::Word32;
	case ARM_INS_LDR: return BaseType::Word32;
	case ARM_INS_LDRT: return BaseType::Word32;
	case ARM_INS_LDRB: return BaseType::Byte;
	case ARM_INS_LDRBT: return BaseType::Byte;
	case ARM_INS_LDRD: return BaseType::Word64;
	case ARM_INS_LDRH: return BaseType::Word16;
	case ARM_INS_LDRHT: return BaseType::Word16;
	case ARM_INS_LDRSB: return BaseType::SByte;
	case ARM_INS_LDRSBT: return BaseType::SByte;
	case ARM_INS_LDRSH: return BaseType::Int16;
	case ARM_INS_LDRSHT: return BaseType::Int16;
	case ARM_INS_STC: return BaseType::Word32;
	case ARM_INS_STC2: return BaseType::Word32;
	case ARM_INS_STC2L: return BaseType::Word32;
	case ARM_INS_STCL: return BaseType::Word32;
	case ARM_INS_STR: return BaseType::Word32;
	case ARM_INS_STRT: return BaseType::Word32;
	case ARM_INS_STRB: return BaseType::Byte;
	case ARM_INS_STRBT: return BaseType::Byte;
	case ARM_INS_STRD: return BaseType::Word64;
	case ARM_INS_STRH: return BaseType::Word16;
	case ARM_INS_STRHT: return BaseType::Word16;
	case ARM_INS_SWP: return BaseType::Word32;
	case ARM_INS_SWPB: return BaseType::Byte;
	case ARM_INS_VLDR: return register_types[instr->detail->arm.operands[0].reg];
	case ARM_INS_VSTR: return register_types[instr->detail->arm.operands[0].reg];
	}
	//assert(false && instr->Id.ToString());
	return BaseType::Void;
}


HExpr ArmRewriter::MaybeShiftOperand(HExpr exp, const cs_arm_op & op)
{
	switch (op.shift.type)
	{
	case ARM_SFT_ASR: return m.Sar(exp, m.Int32(op.shift.value));
	case ARM_SFT_LSL: return m.Shl(exp, m.Int32(op.shift.value));
	case ARM_SFT_LSR: return m.Shr(exp, m.Int32(op.shift.value));
	case ARM_SFT_ROR: return m.Ror(exp, m.Int32(op.shift.value));
	case ARM_SFT_RRX:
	{
		auto c = host->EnsureFlagGroup(ARM_REG_CPSR, (int)FlagM::CF, "C");
		return m.Rrc(exp, m.Int32(op.shift.value), c);
	}
	case ARM_SFT_ASR_REG: return m.Sar(exp, Reg(op.shift.value));
	case ARM_SFT_LSL_REG: return m.Shl(exp, Reg(op.shift.value));
	case ARM_SFT_LSR_REG: return m.Shr(exp, Reg(op.shift.value));
	case ARM_SFT_ROR_REG: return m.Ror(exp, Reg(op.shift.value));
	case ARM_SFT_RRX_REG:
	{
		auto c = host->EnsureFlagGroup(ARM_REG_CPSR, (int)FlagM::CF, "C");
		return m.Rrc(exp, Reg(op.shift.value), c);
	}
	default: return exp;
	}
}

void ArmRewriter::MaybePostOperand(const cs_arm_op & op)
{
	if (IsLastOperand(op))
		return;
	if (op.type != ARM_OP_MEM)
		return;
	auto lastOp = instr->detail->arm.operands[instr->detail->arm.op_count - 1];
	auto baseReg = Reg(op.mem.base);
	auto offset = Operand(lastOp);
	auto ea = lastOp.subtracted
		? m.ISub(baseReg, offset)
		: m.IAdd(baseReg, offset);
	m.Assign(baseReg, ea);
#if NYI
	if (memOp is null || memOp.Offset is null)
		return;
	if (memOp.Preindexed)
		return;
	Expression baseReg = host->EnsureRegister(memOp.Base);
	auto offset = Operand(memOp.Offset);
	auto ea = memOp.Subtract
		? emitter.ISub(baseReg, offset)
		: emitter.IAdd(baseReg, offset);
	emitter.Assign(baseReg, ea);
#endif
}

HExpr ArmRewriter::TestCond(arm_cc cond)
{
	switch (cond)
	{
	//default:
	//	throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
	case ARM_CC_HS:
		return m.Test(ConditionCode::UGE, FlagGroup(FlagM::CF, "C"));
	case ARM_CC_LO:
		return m.Test(ConditionCode::ULT, FlagGroup(FlagM::CF, "C"));
	case ARM_CC_EQ:
		return m.Test(ConditionCode::EQ, FlagGroup(FlagM::ZF, "Z"));
	case ARM_CC_GE:
		return m.Test(ConditionCode::GE, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV"));
	case ARM_CC_GT:
		return m.Test(ConditionCode::GT, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV"));
	case ARM_CC_HI:
		return m.Test(ConditionCode::UGT, FlagGroup(FlagM::ZF | FlagM::CF, "ZC"));
	case ARM_CC_LE:
		return m.Test(ConditionCode::LE, FlagGroup(FlagM::ZF | FlagM::CF | FlagM::VF, "NZV"));
	case ARM_CC_LS:
		return m.Test(ConditionCode::ULE, FlagGroup(FlagM::ZF | FlagM::CF, "ZC"));
	case ARM_CC_LT:
		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF | FlagM::VF, "NV"));
	case ARM_CC_MI:
		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF, "N"));
	case ARM_CC_PL:
		return m.Test(ConditionCode::GE, FlagGroup(FlagM::NF, "N"));
	case ARM_CC_NE:
		return m.Test(ConditionCode::NE, FlagGroup(FlagM::ZF, "Z"));
	case ARM_CC_VC:
		return m.Test(ConditionCode::NO, FlagGroup(FlagM::VF, "V"));
	case ARM_CC_VS:
		return m.Test(ConditionCode::OV, FlagGroup(FlagM::VF, "V"));
	}
	return HExpr();
}

HExpr ArmRewriter::FlagGroup(FlagM bits, const char * name)
{
	return host->EnsureFlagGroup(ARM_REG_CPSR, (int) bits, name);
}


void ArmRewriter::RewriteSwp(BaseType type)
{
	const char * fnName;
	if (type == BaseType::Byte)
	{
		fnName = "std::atomic_exchange<byte>";
	}
	else
	{
		fnName = "std::atomic_exchange<int32_t>";
	}
	auto intrinsic = host->EnsureIntrinsicProcedure(fnName, false, type, 2);
	auto dst = Operand(Dst(), BaseType::Word32, true);
	m.AddArg(Operand(Src1()));
	m.AddArg(Operand(Src2()));
	m.Assign(dst, m.Fn(intrinsic));
}

#if (_DEBUG || DEBUG) && !MONODEVELOP
void ArmRewriter::EmitUnitTest()
{
	if (opcode_seen[instr->id])
		return;
	opcode_seen[instr->id] = 1;

	//var r2 = rdr.Clone();
	//r2.Offset -= dasm.Current.Length;
	auto bytes = &instr->bytes[0];
	wchar_t buf[256];
	::OutputDebugString(L"        [Test]\r\n");
	wsprintfW(buf,      L"        public void ArmRw_%S()\r\n", instr->mnemonic );
	::OutputDebugString(buf);
	::OutputDebugString(L"        {\r\n");
	wsprintfW(buf,      L"            BuildTest(0x%02x%02x%02x%02x);\t// %S %S\r\n",
		bytes[3], bytes[2], bytes[1], bytes[0],
		instr->mnemonic, instr->op_str);
	::OutputDebugString(buf);
	::OutputDebugString(L"            AssertCode(");
	::OutputDebugString(L"                \"0|L--|00100000(4): 1 instructions\",\r\n");
	::OutputDebugString(L"                \"1|L--|@@@\");\r\n");
	::OutputDebugString(L"        }\r\n");
	::OutputDebugString(L"\r\n");
}

int ArmRewriter::opcode_seen[ARM_INS_ENDING];
#endif


const BaseType ArmRewriter::register_types[] =
{
	BaseType::Void,			// ARM_REG_INVALID = 0,
	BaseType::Word32,		// ARM_REG_APSR,
	BaseType::Word32,		// ARM_REG_APSR_NZCV,
	BaseType::Word32,		// ARM_REG_CPSR,
	BaseType::Word32,		// ARM_REG_FPEXC,
	BaseType::Word32,		// ARM_REG_FPINST,
	BaseType::Word32,		// ARM_REG_FPSCR,
	BaseType::Word32,		// ARM_REG_FPSCR_NZCV,
	BaseType::Word32,		// ARM_REG_FPSID,
	BaseType::Word32,		// ARM_REG_ITSTATE,
	BaseType::Word32,		// ARM_REG_LR,
	BaseType::Word32,		// ARM_REG_PC,
	BaseType::Word32,		// ARM_REG_SP,
	BaseType::Word32,		// ARM_REG_SPSR,
	BaseType::Word64,		// ARM_REG_D0,
	BaseType::Word64,		// ARM_REG_D1,
	BaseType::Word64,		// ARM_REG_D2,
	BaseType::Word64,		// ARM_REG_D3,
	BaseType::Word64,		// ARM_REG_D4,
	BaseType::Word64,		// ARM_REG_D5,
	BaseType::Word64,		// ARM_REG_D6,
	BaseType::Word64,		// ARM_REG_D7,
	BaseType::Word64,		// ARM_REG_D8,
	BaseType::Word64,		// ARM_REG_D9,
	BaseType::Word64,		// ARM_REG_D10,
	BaseType::Word64,		// ARM_REG_D11,
	BaseType::Word64,		// ARM_REG_D12,
	BaseType::Word64,		// ARM_REG_D13,
	BaseType::Word64,		// ARM_REG_D14,
	BaseType::Word64,		// ARM_REG_D15,
	BaseType::Word64,		// ARM_REG_D16,
	BaseType::Word64,		// ARM_REG_D17,
	BaseType::Word64,		// ARM_REG_D18,
	BaseType::Word64,		// ARM_REG_D19,
	BaseType::Word64,		// ARM_REG_D20,
	BaseType::Word64,		// ARM_REG_D21,
	BaseType::Word64,		// ARM_REG_D22,
	BaseType::Word64,		// ARM_REG_D23,
	BaseType::Word64,		// ARM_REG_D24,
	BaseType::Word64,		// ARM_REG_D25,
	BaseType::Word64,		// ARM_REG_D26,
	BaseType::Word64,		// ARM_REG_D27,
	BaseType::Word64,		// ARM_REG_D28,
	BaseType::Word64,		// ARM_REG_D29,
	BaseType::Word64,		// ARM_REG_D30,
	BaseType::Word64,		// ARM_REG_D31,
	BaseType::Word32,		// ARM_REG_FPINST2,
	BaseType::Word32,		// ARM_REG_MVFR0,
	BaseType::Word32,		// ARM_REG_MVFR1,
	BaseType::Word32,		// ARM_REG_MVFR2,
	BaseType::Word128,		// ARM_REG_Q0,
	BaseType::Word128,		// ARM_REG_Q1,
	BaseType::Word128,		// ARM_REG_Q2,
	BaseType::Word128,		// ARM_REG_Q3,
	BaseType::Word128,		// ARM_REG_Q4,
	BaseType::Word128,		// ARM_REG_Q5,
	BaseType::Word128,		// ARM_REG_Q6,
	BaseType::Word128,		// ARM_REG_Q7,
	BaseType::Word128,		// ARM_REG_Q8,
	BaseType::Word128,		// ARM_REG_Q9,
	BaseType::Word128,		// ARM_REG_Q10,
	BaseType::Word128,		// ARM_REG_Q11,
	BaseType::Word128,		// ARM_REG_Q12,
	BaseType::Word128,		// ARM_REG_Q13,
	BaseType::Word128,		// ARM_REG_Q14,
	BaseType::Word128,		// ARM_REG_Q15,
	BaseType::Word32,		// ARM_REG_R0,
	BaseType::Word32,		// ARM_REG_R1,
	BaseType::Word32,		// ARM_REG_R2,
	BaseType::Word32,		// ARM_REG_R3,
	BaseType::Word32,		// ARM_REG_R4,
	BaseType::Word32,		// ARM_REG_R5,
	BaseType::Word32,		// ARM_REG_R6,
	BaseType::Word32,		// ARM_REG_R7,
	BaseType::Word32,		// ARM_REG_R8,
	BaseType::Word32,		// ARM_REG_R9,
	BaseType::Word32,		// ARM_REG_R10,
	BaseType::Word32,		// ARM_REG_R11,
	BaseType::Word32,		// ARM_REG_R12,
	BaseType::Word32,		// ARM_REG_S0,
	BaseType::Word32,		// ARM_REG_S1,
	BaseType::Word32,		// ARM_REG_S2,
	BaseType::Word32,		// ARM_REG_S3,
	BaseType::Word32,		// ARM_REG_S4,
	BaseType::Word32,		// ARM_REG_S5,
	BaseType::Word32,		// ARM_REG_S6,
	BaseType::Word32,		// ARM_REG_S7,
	BaseType::Word32,		// ARM_REG_S8,
	BaseType::Word32,		// ARM_REG_S9,
	BaseType::Word32,		// ARM_REG_S10,
	BaseType::Word32,		// ARM_REG_S11,
	BaseType::Word32,		// ARM_REG_S12,
	BaseType::Word32,		// ARM_REG_S13,
	BaseType::Word32,		// ARM_REG_S14,
	BaseType::Word32,		// ARM_REG_S15,
	BaseType::Word32,		// ARM_REG_S16,
	BaseType::Word32,		// ARM_REG_S17,
	BaseType::Word32,		// ARM_REG_S18,
	BaseType::Word32,		// ARM_REG_S19,
	BaseType::Word32,		// ARM_REG_S20,
	BaseType::Word32,		// ARM_REG_S21,
	BaseType::Word32,		// ARM_REG_S22,
	BaseType::Word32,		// ARM_REG_S23,
	BaseType::Word32,		// ARM_REG_S24,
	BaseType::Word32,		// ARM_REG_S25,
	BaseType::Word32,		// ARM_REG_S26,
	BaseType::Word32,		// ARM_REG_S27,
	BaseType::Word32,		// ARM_REG_S28,
	BaseType::Word32,		// ARM_REG_S29,
	BaseType::Word32,		// ARM_REG_S30,
	BaseType::Word32,		// ARM_REG_S31,
};

const int ArmRewriter::type_sizes[] = 
{
	0,						// Void,

	1,						// Bool,

	1,						// Byte,
	1,						// SByte,
	1,						// Char8,

	2,						// Int16,
	2,						// UInt16,
	2,						// Ptr16,
	2,						// Word16,

	4,						// Int32,
	4,						// UInt32,
	4,						// Ptr32,
	4,						// Word32,

	8,						// Int64,
	8,						// UInt64,
	8,						// Ptr64,
	8,						// Word64,
 
	16,						// Word128,

	4,						// Real32,
	8,						// Real64,
};

int32_t ArmRewriter::GetCount()
{
	return s_count;
}

int ArmRewriter::s_count = 0;
