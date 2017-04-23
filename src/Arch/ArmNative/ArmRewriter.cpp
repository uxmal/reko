#include "stdafx.h"
#include "reko.h"

#include "ArmRewriter.h"

ArmRewriter::ArmRewriter(
	void * rawBytes,
	int length,
	IRtlEmitter * emitter,
	IFrame * frame,
	IRewriterHost * host)
:
	m(*emitter),
	host(*host),
	frame(*frame)
{
	auto hcap = ::LoadLibrary(L"cs_open");
	::GetProcAddress(hcap, "cs_open");
	csh hcapstone;
	cs_open(CS_ARCH_ARM, CS_MODE_ARM, &hcapstone); 
	cs_option(hcapstone, CS_OPT_DETAIL, CS_OPT_ON);
}

void ArmRewriter::Next()
{
	uint64_t addr;
	cs_disasm_iter(0, 0, 0, &addr, &instr);
	switch (instr.id)
	{
	default:

	case ARM_INS_ADR:
	case ARM_INS_AESD:
	case ARM_INS_AESE:
	case ARM_INS_AESIMC:
	case ARM_INS_AESMC:
	case ARM_INS_BKPT:
	case ARM_INS_BXJ:
	case ARM_INS_CDP:
	case ARM_INS_CDP2:
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
	case ARM_INS_VMRS:
	case ARM_INS_FSTMDBX:
	case ARM_INS_FSTMIAX:
	case ARM_INS_HINT:
	case ARM_INS_HLT:
	case ARM_INS_ISB:
	case ARM_INS_LDA:
	case ARM_INS_LDAB:
	case ARM_INS_LDAEX:
	case ARM_INS_LDAEXB:
	case ARM_INS_LDAEXD:
	case ARM_INS_LDAEXH:
	case ARM_INS_LDAH:
	case ARM_INS_LDC2L:
	case ARM_INS_LDC2:
	case ARM_INS_LDCL:
	case ARM_INS_LDC:
	case ARM_INS_LDMDA:
	case ARM_INS_LDRBT:
	case ARM_INS_LDREX:
	case ARM_INS_LDREXB:
	case ARM_INS_LDREXD:
	case ARM_INS_LDREXH:
	case ARM_INS_LDRHT:
	case ARM_INS_LDRSBT:
	case ARM_INS_LDRSHT:
	case ARM_INS_LDRT:
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
	case ARM_INS_QADD:
	case ARM_INS_QADD16:
	case ARM_INS_QADD8:
	case ARM_INS_QASX:
	case ARM_INS_QDADD:
	case ARM_INS_QDSUB:
	case ARM_INS_QSAX:
	case ARM_INS_QSUB:
	case ARM_INS_QSUB16:
	case ARM_INS_QSUB8:
	case ARM_INS_RBIT:
	case ARM_INS_REV16:
	case ARM_INS_REVSH:
	case ARM_INS_RFEDA:
	case ARM_INS_RFEDB:
	case ARM_INS_RFEIA:
	case ARM_INS_RFEIB:
	case ARM_INS_RSC:
	case ARM_INS_SADD16:
	case ARM_INS_SADD8:
	case ARM_INS_SASX:
	case ARM_INS_SDIV:
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
	case ARM_INS_SMLABB:
	case ARM_INS_SMLABT:
	case ARM_INS_SMLAD:
	case ARM_INS_SMLADX:
	case ARM_INS_SMLAL:
	case ARM_INS_SMLALBB:
	case ARM_INS_SMLALBT:
	case ARM_INS_SMLALD:
	case ARM_INS_SMLALDX:
	case ARM_INS_SMLALTB:
	case ARM_INS_SMLALTT:
	case ARM_INS_SMLATB:
	case ARM_INS_SMLATT:
	case ARM_INS_SMLAWB:
	case ARM_INS_SMLAWT:
	case ARM_INS_SMLSD:
	case ARM_INS_SMLSDX:
	case ARM_INS_SMLSLD:
	case ARM_INS_SMLSLDX:
	case ARM_INS_SMMLA:
	case ARM_INS_SMMLAR:
	case ARM_INS_SMMLS:
	case ARM_INS_SMMLSR:
	case ARM_INS_SMMUL:
	case ARM_INS_SMMULR:
	case ARM_INS_SMUAD:
	case ARM_INS_SMUADX:
	case ARM_INS_SMULBT:
	case ARM_INS_SMULTB:
	case ARM_INS_SMULTT:
	case ARM_INS_SMULWB:
	case ARM_INS_SMULWT:
	case ARM_INS_SMUSD:
	case ARM_INS_SMUSDX:
	case ARM_INS_SRSDA:
	case ARM_INS_SRSDB:
	case ARM_INS_SRSIA:
	case ARM_INS_SRSIB:
	case ARM_INS_SSAT:
	case ARM_INS_SSAT16:
	case ARM_INS_SSAX:
	case ARM_INS_SSUB16:
	case ARM_INS_SSUB8:
	case ARM_INS_STC2L:
	case ARM_INS_STC2:
	case ARM_INS_STCL:
	case ARM_INS_STC:
	case ARM_INS_STL:
	case ARM_INS_STLB:
	case ARM_INS_STLEX:
	case ARM_INS_STLEXB:
	case ARM_INS_STLEXD:
	case ARM_INS_STLEXH:
	case ARM_INS_STLH:
	case ARM_INS_STMDA:
	case ARM_INS_STRBT:
	case ARM_INS_STREX:
	case ARM_INS_STREXB:
	case ARM_INS_STREXD:
	case ARM_INS_STREXH:
	case ARM_INS_STRHT:
	case ARM_INS_STRT:
	case ARM_INS_SWP:
	case ARM_INS_SWPB:
	case ARM_INS_SXTAB16:
	case ARM_INS_SXTB16:
	case ARM_INS_TRAP:
	case ARM_INS_UADD16:
	case ARM_INS_UADD8:
	case ARM_INS_UASX:
	case ARM_INS_UDF:
	case ARM_INS_UDIV:
	case ARM_INS_UHADD16:
	case ARM_INS_UHADD8:
	case ARM_INS_UHASX:
	case ARM_INS_UHSAX:
	case ARM_INS_UHSUB16:
	case ARM_INS_UHSUB8:
	case ARM_INS_UMAAL:
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
	case ARM_INS_USUB16:
	case ARM_INS_USUB8:
	case ARM_INS_UXTAB16:
	case ARM_INS_UXTB16:
	case ARM_INS_VABAL:
	case ARM_INS_VABA:
	case ARM_INS_VABDL:
	case ARM_INS_VABD:
	case ARM_INS_VABS:
	case ARM_INS_VACGE:
	case ARM_INS_VACGT:
	case ARM_INS_VADD:
	case ARM_INS_VADDHN:
	case ARM_INS_VADDL:
	case ARM_INS_VADDW:
	case ARM_INS_VAND:
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
	case ARM_INS_VCMP:
	case ARM_INS_VCMPE:
	case ARM_INS_VCNT:
	case ARM_INS_VCVTA:
	case ARM_INS_VCVTB:
	case ARM_INS_VCVT:
	case ARM_INS_VCVTM:
	case ARM_INS_VCVTN:
	case ARM_INS_VCVTP:
	case ARM_INS_VCVTT:
	case ARM_INS_VDIV:
	case ARM_INS_VDUP:
	case ARM_INS_VEOR:
	case ARM_INS_VEXT:
	case ARM_INS_VFMA:
	case ARM_INS_VFMS:
	case ARM_INS_VFNMA:
	case ARM_INS_VFNMS:
	case ARM_INS_VHADD:
	case ARM_INS_VHSUB:
	case ARM_INS_VLD1:
	case ARM_INS_VLD2:
	case ARM_INS_VLD3:
	case ARM_INS_VLD4:
	case ARM_INS_VLDMDB:
	case ARM_INS_VLDR:
	case ARM_INS_VMAXNM:
	case ARM_INS_VMAX:
	case ARM_INS_VMINNM:
	case ARM_INS_VMIN:
	case ARM_INS_VMLA:
	case ARM_INS_VMLAL:
	case ARM_INS_VMLS:
	case ARM_INS_VMLSL:
	case ARM_INS_VMOVL:
	case ARM_INS_VMOVN:
	case ARM_INS_VMSR:
	case ARM_INS_VMUL:
	case ARM_INS_VMULL:
	case ARM_INS_VMVN:
	case ARM_INS_VNEG:
	case ARM_INS_VNMLA:
	case ARM_INS_VNMLS:
	case ARM_INS_VNMUL:
	case ARM_INS_VORN:
	case ARM_INS_VORR:
	case ARM_INS_VPADAL:
	case ARM_INS_VPADDL:
	case ARM_INS_VPADD:
	case ARM_INS_VPMAX:
	case ARM_INS_VPMIN:
	case ARM_INS_VQABS:
	case ARM_INS_VQADD:
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
	case ARM_INS_VQSHL:
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
	case ARM_INS_VRSHL:
	case ARM_INS_VRSHRN:
	case ARM_INS_VRSHR:
	case ARM_INS_VRSQRTE:
	case ARM_INS_VRSQRTS:
	case ARM_INS_VRSRA:
	case ARM_INS_VRSUBHN:
	case ARM_INS_VSELEQ:
	case ARM_INS_VSELGE:
	case ARM_INS_VSELGT:
	case ARM_INS_VSELVS:
	case ARM_INS_VSHLL:
	case ARM_INS_VSHL:
	case ARM_INS_VSHRN:
	case ARM_INS_VSHR:
	case ARM_INS_VSLI:
	case ARM_INS_VSQRT:
	case ARM_INS_VSRA:
	case ARM_INS_VSRI:
	case ARM_INS_VST1:
	case ARM_INS_VST2:
	case ARM_INS_VST3:
	case ARM_INS_VST4:
	case ARM_INS_VSTMDB:
	case ARM_INS_VSTR:
	case ARM_INS_VSUB:
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
	case ARM_INS_ADDW:
	case ARM_INS_ASR:
	case ARM_INS_DCPS1:
	case ARM_INS_DCPS2:
	case ARM_INS_DCPS3:
	case ARM_INS_IT:
	case ARM_INS_LSL:
	case ARM_INS_LSR:
	case ARM_INS_ASRS:
	case ARM_INS_LSRS:
	case ARM_INS_ORN:
	case ARM_INS_ROR:
	case ARM_INS_RRX:
	case ARM_INS_SUBS:
	case ARM_INS_SUBW:
	case ARM_INS_TBB:
	case ARM_INS_TBH:
	case ARM_INS_CBNZ:
	case ARM_INS_CBZ:
	case ARM_INS_MOVS:
	case ARM_INS_YIELD:
	case ARM_INS_WFE:
	case ARM_INS_WFI:
	case ARM_INS_SEV:
	case ARM_INS_SEVL:
	case ARM_INS_VPUSH:
	case ARM_INS_VPOP:
		NotImplementedYet();
		break;

	case ARM_INS_ADC: RewriteAdcSbc(&IRtlEmitter::IAdd); break;
	case ARM_INS_ADD: RewriteBinOp(&IRtlEmitter::IAdd, instr.detail->arm.update_flags); break;
	case ARM_INS_AND: RewriteBinOp(&IRtlEmitter::And, instr.detail->arm.update_flags); break;
	case ARM_INS_EOR: RewriteBinOp(&IRtlEmitter::Xor, instr.detail->arm.update_flags); break;
	case ARM_INS_B: RewriteB(false); break;
	case ARM_INS_BFC: RewriteBfc(); break;
	case ARM_INS_BFI: RewriteBfi(); break;
	case ARM_INS_BIC: RewriteBic(); break;
	case ARM_INS_BL: RewriteB(true); break;
	case ARM_INS_BLX: RewriteB(true); break;
	case ARM_INS_BX: RewriteB(false); break;
	case ARM_INS_CLZ: RewriteClz(); break;
	case ARM_INS_CMN: RewriteCmn(); break;
	case ARM_INS_CMP: RewriteCmp(); break;
	case ARM_INS_CPS: RewriteCps(); break;
	case ARM_INS_DMB: RewriteDmb(); break;
	case ARM_INS_LDR: RewriteLdr(PrimitiveType::Word32); break;
	case ARM_INS_LDRB: RewriteLdr(PrimitiveType::Byte); break;
	case ARM_INS_LDRH: RewriteLdr(PrimitiveType::UInt16); break;
	case ARM_INS_LDRSB: RewriteLdr(PrimitiveType::SByte); break;
	case ARM_INS_LDRSH: RewriteLdr(PrimitiveType::Int16); break;
	case ARM_INS_LDRD: RewriteLdrd(); break;
	case ARM_INS_LDM: RewriteLdm(0); break;
	case ARM_INS_LDMDB: RewriteLdm(0); break;
	case ARM_INS_LDMIB: RewriteLdm(4); break;
	case ARM_INS_NOP: m.Nop(); break;
	case ARM_INS_MCR: RewriteMcr(); break;
	case ARM_INS_MLA: RewriteMultiplyAccumulate(&IRtlEmitter::IAdd); break;
	case ARM_INS_MLS: RewriteMultiplyAccumulate(&IRtlEmitter::ISub); break;
	case ARM_INS_MOV: RewriteMov(); break;
	case ARM_INS_MOVT: RewriteMovt(); break;
	case ARM_INS_MOVW: RewriteMov(); break;
	case ARM_INS_MRC: RewriteMrc(); break;
	case ARM_INS_MRS: RewriteMrs(); break;
	case ARM_INS_MSR: RewriteMsr(); break;
	case ARM_INS_MUL: RewriteBinOp(&IRtlEmitter::IMul, instr.detail->arm.update_flags); break;
	case ARM_INS_MVN: RewriteUnaryOp(&IRtlEmitter::Not); break;
	case ARM_INS_ORR: RewriteBinOp(&IRtlEmitter::Or, false); break;
	case ARM_INS_POP: RewritePop(); break;
	case ARM_INS_PUSH: RewritePush(); break;
	case ARM_INS_REV: RewriteRev(); break;
	case ARM_INS_RSB: RewriteRevBinOp(&IRtlEmitter::ISub, instr.detail->arm.update_flags); break;
	case ARM_INS_SBC: RewriteAdcSbc(&IRtlEmitter::ISub); break;
	case ARM_INS_SBFX: RewriteSbfx(); break;
	case ARM_INS_SMULBB: RewriteMulbb(false, false, PrimitiveType::Int16, &IRtlEmitter::SMul); break;
	case ARM_INS_SMULL: RewriteMull(PrimitiveType::Int64, &IRtlEmitter::SMul); break;
	case ARM_INS_STM: RewriteStm(); break;
	case ARM_INS_STMDB: RewriteStm(); break;
	case ARM_INS_STMIB: RewriteStmib(); break;
	case ARM_INS_STR: RewriteStr(PrimitiveType::Word32); break;
	case ARM_INS_STRB: RewriteStr(PrimitiveType::Byte); break;
	case ARM_INS_STRD: RewriteStrd(); break;
	case ARM_INS_STRH: RewriteStr(PrimitiveType::UInt16); break;
	case ARM_INS_SUB: RewriteBinOp(&IRtlEmitter::ISub, instr.detail->arm.update_flags); break;
	case ARM_INS_SVC: RewriteSvc(); break;
	case ARM_INS_SXTAB: RewriteXtab(PrimitiveType::SByte); break;
	case ARM_INS_SXTAH: RewriteXtab(PrimitiveType::Int16); break;
	case ARM_INS_SXTB: RewriteXtb(PrimitiveType::SByte); break;
	case ARM_INS_SXTH: RewriteXtb(PrimitiveType::Int16); break;
	case ARM_INS_TEQ: RewriteTeq(); break;
	case ARM_INS_TST: RewriteTst(); break;
	case ARM_INS_UBFX: RewriteUbfx(); break;
	case ARM_INS_UMLAL: RewriteUmlal(); break;
	case ARM_INS_UMULL: RewriteMull(PrimitiveType::UInt64, &IRtlEmitter::UMul); break;
	case ARM_INS_UXTAB: RewriteXtab(PrimitiveType::Byte); break;
	case ARM_INS_UXTAH: RewriteXtab(PrimitiveType::UInt16); break;
	case ARM_INS_UXTB: RewriteXtb(PrimitiveType::Byte); break;
	case ARM_INS_UXTH: RewriteXtb(PrimitiveType::UInt16); break;

	case ARM_INS_VLDMIA: RewriteVldmia(); break;
	case ARM_INS_VMOV: RewriteVmov(); break;
	case ARM_INS_VSTMIA: RewriteVstmia(); break;

	}
}

void ArmRewriter::NotImplementedYet()
{
	char buf[200];	//$TODO: hello buffer overflow!
	::snprintf(buf, sizeof(buf), "Rewriting ARM opcode '%s' is not supported yet.", instr.mnemonic);
	host.Error(
		instr.address,
		buf);
	m.Invalid();
}

IExpression * ArmRewriter::NZCV()
{
	return frame.EnsureFlagGroup((int)ARM_REG_CPSR, 0x1111, "NZCV", PrimitiveType::Byte);
}

void ArmRewriter::MaybeUpdateFlags(IExpression * opDst)
{
	if (instr.detail->arm.update_flags)
	{
		m.Assign(NZCV(), m.Cond(opDst));
	}
}

void ArmRewriter::RewriteB(bool link)
{
	IExpression * dst;
	bool dstIsAddress;
	if (Dst().type == ARM_OP_IMM)
	{
		dst = m.Ptr32(Dst().imm);
		dstIsAddress = true;
	}
	else
	{
		dst = Operand(Dst());
		dstIsAddress = false;
	}
	if (link)
	{
		m.SetRtlClass(RtlClass::Transfer);
		if (instr.detail->arm.cc == ARM_CC_AL)
		{
			m.Call(dst, 0);
		}
		else
		{
			//$TODO: conditional code.
			//m.If(TestCond(instr.detail->arm.CodeCondition), new RtlCall(dst, 0, RtlClass::Transfer));
		}
	}
	else
	{
		if (instr.detail->arm.cc == ARM_CC_AL)
		{
			m.SetRtlClass(RtlClass::Transfer);
			m.Goto(dst);
		}
		else
		{
			m.SetRtlClass(RtlClass::ConditionalTransfer);
			if (dstIsAddress)
			{
				m.Branch(TestCond(instr.detail->arm.cc), dst, RtlClass::ConditionalTransfer);
			}
			else
			{
				//$TODO: conditional code
				//m.If(TestCond(instr.detail->arm.CodeCondition), new RtlGoto(dst, RtlClass::ConditionalTransfer));
			}
		}
	}
}

void ArmRewriter::AddConditional(void (*mkInstr)())
{
	//if (instr.detail->arm.CodeCondition != ArmCodeCondition.AL)
	//{
	//	rtlInstr = new RtlIf(TestCond(instr.detail->arm.CodeCondition), rtlInstr);
	//}
	//ric.Instructions.Add(rtlInstr);
}

void ArmRewriter::ConditionalAssign(IExpression * dst, IExpression * src)
{
	/*RtlInstruction rtlInstr = new RtlAssignment(dst, src);
	if (instr.detail->arm.CodeCondition != ArmCodeCondition::AL)
	{
		rtlInstr = new RtlIf(TestCond(instr.detail->arm.CodeCondition), rtlInstr);
	}
	ric.Instructions.Add(rtlInstr);*/
}

// If a conditional ARM instruction is encountered, generate an IL
// instruction to skip the remainder of the instruction cluster.
void ArmRewriter::ConditionalSkip()
{
	auto cc = instr.detail->arm.cc;
	if (cc == ARM_CC_AL)
		return; // never skip!
	m.BranchInMiddleOfInstruction(
		TestCond(Invert(cc)),
		m.Ptr32(static_cast<uint32_t>(instr.address) + 4),
		RtlClass::ConditionalTransfer);
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
	return &op == &instr.detail->arm.operands[instr.detail->arm.op_count - 1];
}

IExpression * ArmRewriter::Operand(const cs_arm_op & op)
{
	switch (op.type)
	{
	case ARM_OP_REG:
	{
		auto reg = frame.EnsureRegister(op.reg);
		return MaybeShiftOperand(reg, op);
	}
	case ARM_OP_SYSREG:
	{
		auto sysreg = frame.EnsureRegister(op.reg);
		return sysreg;
	}
	case ARM_OP_IMM:
		return m.Word32(op.imm);
	case ARM_OP_CIMM:
	case ARM_OP_PIMM:
		return m.Byte((uint8_t)op.imm);
	case ARM_OP_MEM:
	{
		auto baseReg = Reg(op.mem.base);
		auto ea = baseReg;
		if (op.mem.base == ARM_REG_PC)
		{
			// PC-relative address
			if (op.mem.disp != 0)
			{
				auto dst = (uint32_t)((int32_t)instr.address + op.mem.disp) + 8u;
				return m.Mem(SizeFromLoadStore(), m.Ptr32(dst));
			}
		}
		if (op.mem.disp != 0 && IsLastOperand(op))
		{
			auto offset = m.Int32(op.mem.disp);
			ea = op.mem.scale < 0
				? m.ISub(ea, offset)
				: m.IAdd(ea, offset);
		}
		if (IsLastOperand(op) && instr.detail->arm.writeback)
		{
			m.Assign(baseReg, ea);
			ea = baseReg;
		}
		return m.Mem(SizeFromLoadStore(), ea);
	}
	}
	//$TODO
	//throw new NotImplementedException(op.Type.ToString());
	return 0;
}

PrimitiveType ArmRewriter::SizeFromLoadStore()
{
	switch (instr.id)
	{
	case ARM_INS_LDR: return PrimitiveType::Word32;
	case ARM_INS_LDRB: return PrimitiveType::Byte;
	case ARM_INS_LDRD: return PrimitiveType::Word64;
	case ARM_INS_LDRH: return PrimitiveType::Word16;
	case ARM_INS_LDRSB: return PrimitiveType::SByte;
	case ARM_INS_LDRSH: return PrimitiveType::Int16;
	case ARM_INS_STR: return PrimitiveType::Word32;
	case ARM_INS_STRB: return PrimitiveType::Byte;
	case ARM_INS_STRD: return PrimitiveType::Word64;
	case ARM_INS_STRH: return PrimitiveType::Word16;
	}
	//assert(false && instr.Id.ToString());
	return PrimitiveType::Void;
}


IExpression * ArmRewriter::MaybeShiftOperand(IExpression * exp, const cs_arm_op & op)
{
	switch (op.shift.type)
	{
	case ARM_SFT_ASR: return m.Sar(exp, op.shift.value);
	case ARM_SFT_LSL: return m.Shl(exp, op.shift.value);
	case ARM_SFT_LSR: return m.Shr(exp, op.shift.value);
	case ARM_SFT_ROR: return m.Ror(exp, m.Int32(op.shift.value));
	case ARM_SFT_RRX: return m.Rrc(exp, m.Int32(op.shift.value));
	case ARM_SFT_ASR_REG: return m.Sar(exp, Reg(op.shift.value));
	case ARM_SFT_LSL_REG: return m.Shl(exp, Reg(op.shift.value));
	case ARM_SFT_LSR_REG: return m.Shr(exp, Reg(op.shift.value));
	case ARM_SFT_ROR_REG: return m.Ror(exp, Reg(op.shift.value));
	case ARM_SFT_RRX_REG: return m.Rrc(exp, Reg(op.shift.value));
	default: return exp;
	}
}

void ArmRewriter::MaybePostOperand(const cs_arm_op & op)
{
	if (IsLastOperand(op))
		return;
	if (op.type != ARM_OP_MEM)
		return;
	auto lastOp = instr.detail->arm.operands[instr.detail->arm.op_count - 1];
	auto baseReg = Reg(op.mem.base);
	auto offset = Operand(lastOp);
	auto ea = lastOp.subtracted
		? m.ISub(baseReg, offset)
		: m.IAdd(baseReg, offset);
	m.Assign(baseReg, ea);
#if NYI
	if (memOp == null || memOp.Offset == null)
		return;
	if (memOp.Preindexed)
		return;
	Expression baseReg = frame.EnsureRegister(memOp.Base);
	auto offset = Operand(memOp.Offset);
	auto ea = memOp.Subtract
		? emitter.ISub(baseReg, offset)
		: emitter.IAdd(baseReg, offset);
	emitter.Assign(baseReg, ea);
#endif
}

IExpression * ArmRewriter::TestCond(arm_cc cond)
{
	switch (cond)
	{
	//default:
	//	throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
	case ARM_CC_HS:
		return m.Test(ConditionCode::UGE, FlagGroup(FlagM::CF, "C", PrimitiveType::Byte));
	case ARM_CC_LO:
		return m.Test(ConditionCode::ULT, FlagGroup(FlagM::CF, "C", PrimitiveType::Byte));
	case ARM_CC_EQ:
		return m.Test(ConditionCode::EQ, FlagGroup(FlagM::ZF, "Z", PrimitiveType::Byte));
	case ARM_CC_GE:
		return m.Test(ConditionCode::GE, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV", PrimitiveType::Byte));
	case ARM_CC_GT:
		return m.Test(ConditionCode::GT, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV", PrimitiveType::Byte));
	case ARM_CC_HI:
		return m.Test(ConditionCode::UGT, FlagGroup(FlagM::ZF | FlagM::CF, "ZC", PrimitiveType::Byte));
	case ARM_CC_LE:
		return m.Test(ConditionCode::LE, FlagGroup(FlagM::ZF | FlagM::CF | FlagM::VF, "NZV", PrimitiveType::Byte));
	case ARM_CC_LS:
		return m.Test(ConditionCode::ULE, FlagGroup(FlagM::ZF | FlagM::CF, "ZC", PrimitiveType::Byte));
	case ARM_CC_LT:
		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF | FlagM::VF, "NV", PrimitiveType::Byte));
	case ARM_CC_MI:
		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF, "N", PrimitiveType::Byte));
	case ARM_CC_PL:
		return m.Test(ConditionCode::GT, FlagGroup(FlagM::NF | FlagM::ZF, "NZ", PrimitiveType::Byte));
	case ARM_CC_NE:
		return m.Test(ConditionCode::NE, FlagGroup(FlagM::ZF, "Z", PrimitiveType::Byte));
	case ARM_CC_VS:
		return m.Test(ConditionCode::OV, FlagGroup(FlagM::VF, "V", PrimitiveType::Byte));
	}
	return 0;
}

IExpression * ArmRewriter::FlagGroup(FlagM bits, const char * name, PrimitiveType type)
{
	return frame.EnsureFlagGroup(ARM_REG_CPSR, (int) bits, name, type);
}

void ArmRewriter::RewriteSvc()
{
	//$TODO
	//m.SideEffect(m.Fn(
	//	host.EnsurePseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, 2),
	//	Operand(Dst)));
}
