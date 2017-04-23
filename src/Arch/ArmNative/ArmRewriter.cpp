#include "stdafx.h"
#include "reko.h"
#include "arm.h"

#include "ArmRewriter.h"

ArmRewriter::ArmRewriter(
	void * rawBytes,
	int length,
	IRtlEmitter * emitter,
	IFrame * frame,
	IRewriterHost * host)
: m(*emitter),
host(*host),
frame(*frame)
{
	auto hcap = ::LoadLibrary(L"cs_open");
	::GetProcAddress(hcap, "cs_open");
	csh hcapstone;
	cs_open(CS_ARCH_ARM, CS_MODE_ARM, &hcapstone); 
}

void ArmRewriter::Next()
{
	//this->instr = dasm->Next();
	switch (instr.Id)
	{
	default:

	case Opcode::ADR:
	case Opcode::AESD:
	case Opcode::AESE:
	case Opcode::AESIMC:
	case Opcode::AESMC:
	case Opcode::BKPT:
	case Opcode::BXJ:
	case Opcode::CDP:
	case Opcode::CDP2:
	case Opcode::CLREX:
	case Opcode::CRC32B:
	case Opcode::CRC32CB:
	case Opcode::CRC32CH:
	case Opcode::CRC32CW:
	case Opcode::CRC32H:
	case Opcode::CRC32W:
	case Opcode::DBG:
	case Opcode::DSB:
	case Opcode::FLDMDBX:
	case Opcode::FLDMIAX:
	case Opcode::VMRS:
	case Opcode::FSTMDBX:
	case Opcode::FSTMIAX:
	case Opcode::HINT:
	case Opcode::HLT:
	case Opcode::ISB:
	case Opcode::LDA:
	case Opcode::LDAB:
	case Opcode::LDAEX:
	case Opcode::LDAEXB:
	case Opcode::LDAEXD:
	case Opcode::LDAEXH:
	case Opcode::LDAH:
	case Opcode::LDC2L:
	case Opcode::LDC2:
	case Opcode::LDCL:
	case Opcode::LDC:
	case Opcode::LDMDA:
	case Opcode::LDRBT:
	case Opcode::LDREX:
	case Opcode::LDREXB:
	case Opcode::LDREXD:
	case Opcode::LDREXH:
	case Opcode::LDRHT:
	case Opcode::LDRSBT:
	case Opcode::LDRSHT:
	case Opcode::LDRT:
	case Opcode::MCR2:
	case Opcode::MCRR:
	case Opcode::MCRR2:
	case Opcode::MRC2:
	case Opcode::MRRC:
	case Opcode::MRRC2:
	case Opcode::PKHBT:
	case Opcode::PKHTB:
	case Opcode::PLDW:
	case Opcode::PLD:
	case Opcode::PLI:
	case Opcode::QADD:
	case Opcode::QADD16:
	case Opcode::QADD8:
	case Opcode::QASX:
	case Opcode::QDADD:
	case Opcode::QDSUB:
	case Opcode::QSAX:
	case Opcode::QSUB:
	case Opcode::QSUB16:
	case Opcode::QSUB8:
	case Opcode::RBIT:
	case Opcode::REV16:
	case Opcode::REVSH:
	case Opcode::RFEDA:
	case Opcode::RFEDB:
	case Opcode::RFEIA:
	case Opcode::RFEIB:
	case Opcode::RSC:
	case Opcode::SADD16:
	case Opcode::SADD8:
	case Opcode::SASX:
	case Opcode::SDIV:
	case Opcode::SEL:
	case Opcode::SETEND:
	case Opcode::SHA1C:
	case Opcode::SHA1H:
	case Opcode::SHA1M:
	case Opcode::SHA1P:
	case Opcode::SHA1SU0:
	case Opcode::SHA1SU1:
	case Opcode::SHA256H:
	case Opcode::SHA256H2:
	case Opcode::SHA256SU0:
	case Opcode::SHA256SU1:
	case Opcode::SHADD16:
	case Opcode::SHADD8:
	case Opcode::SHASX:
	case Opcode::SHSAX:
	case Opcode::SHSUB16:
	case Opcode::SHSUB8:
	case Opcode::SMC:
	case Opcode::SMLABB:
	case Opcode::SMLABT:
	case Opcode::SMLAD:
	case Opcode::SMLADX:
	case Opcode::SMLAL:
	case Opcode::SMLALBB:
	case Opcode::SMLALBT:
	case Opcode::SMLALD:
	case Opcode::SMLALDX:
	case Opcode::SMLALTB:
	case Opcode::SMLALTT:
	case Opcode::SMLATB:
	case Opcode::SMLATT:
	case Opcode::SMLAWB:
	case Opcode::SMLAWT:
	case Opcode::SMLSD:
	case Opcode::SMLSDX:
	case Opcode::SMLSLD:
	case Opcode::SMLSLDX:
	case Opcode::SMMLA:
	case Opcode::SMMLAR:
	case Opcode::SMMLS:
	case Opcode::SMMLSR:
	case Opcode::SMMUL:
	case Opcode::SMMULR:
	case Opcode::SMUAD:
	case Opcode::SMUADX:
	case Opcode::SMULBT:
	case Opcode::SMULTB:
	case Opcode::SMULTT:
	case Opcode::SMULWB:
	case Opcode::SMULWT:
	case Opcode::SMUSD:
	case Opcode::SMUSDX:
	case Opcode::SRSDA:
	case Opcode::SRSDB:
	case Opcode::SRSIA:
	case Opcode::SRSIB:
	case Opcode::SSAT:
	case Opcode::SSAT16:
	case Opcode::SSAX:
	case Opcode::SSUB16:
	case Opcode::SSUB8:
	case Opcode::STC2L:
	case Opcode::STC2:
	case Opcode::STCL:
	case Opcode::STC:
	case Opcode::STL:
	case Opcode::STLB:
	case Opcode::STLEX:
	case Opcode::STLEXB:
	case Opcode::STLEXD:
	case Opcode::STLEXH:
	case Opcode::STLH:
	case Opcode::STMDA:
	case Opcode::STRBT:
	case Opcode::STREX:
	case Opcode::STREXB:
	case Opcode::STREXD:
	case Opcode::STREXH:
	case Opcode::STRHT:
	case Opcode::STRT:
	case Opcode::SWP:
	case Opcode::SWPB:
	case Opcode::SXTAB16:
	case Opcode::SXTB16:
	case Opcode::TRAP:
	case Opcode::UADD16:
	case Opcode::UADD8:
	case Opcode::UASX:
	case Opcode::UDF:
	case Opcode::UDIV:
	case Opcode::UHADD16:
	case Opcode::UHADD8:
	case Opcode::UHASX:
	case Opcode::UHSAX:
	case Opcode::UHSUB16:
	case Opcode::UHSUB8:
	case Opcode::UMAAL:
	case Opcode::UQADD16:
	case Opcode::UQADD8:
	case Opcode::UQASX:
	case Opcode::UQSAX:
	case Opcode::UQSUB16:
	case Opcode::UQSUB8:
	case Opcode::USAD8:
	case Opcode::USADA8:
	case Opcode::USAT:
	case Opcode::USAT16:
	case Opcode::USAX:
	case Opcode::USUB16:
	case Opcode::USUB8:
	case Opcode::UXTAB16:
	case Opcode::UXTB16:
	case Opcode::VABAL:
	case Opcode::VABA:
	case Opcode::VABDL:
	case Opcode::VABD:
	case Opcode::VABS:
	case Opcode::VACGE:
	case Opcode::VACGT:
	case Opcode::VADD:
	case Opcode::VADDHN:
	case Opcode::VADDL:
	case Opcode::VADDW:
	case Opcode::VAND:
	case Opcode::VBIC:
	case Opcode::VBIF:
	case Opcode::VBIT:
	case Opcode::VBSL:
	case Opcode::VCEQ:
	case Opcode::VCGE:
	case Opcode::VCGT:
	case Opcode::VCLE:
	case Opcode::VCLS:
	case Opcode::VCLT:
	case Opcode::VCLZ:
	case Opcode::VCMP:
	case Opcode::VCMPE:
	case Opcode::VCNT:
	case Opcode::VCVTA:
	case Opcode::VCVTB:
	case Opcode::VCVT:
	case Opcode::VCVTM:
	case Opcode::VCVTN:
	case Opcode::VCVTP:
	case Opcode::VCVTT:
	case Opcode::VDIV:
	case Opcode::VDUP:
	case Opcode::VEOR:
	case Opcode::VEXT:
	case Opcode::VFMA:
	case Opcode::VFMS:
	case Opcode::VFNMA:
	case Opcode::VFNMS:
	case Opcode::VHADD:
	case Opcode::VHSUB:
	case Opcode::VLD1:
	case Opcode::VLD2:
	case Opcode::VLD3:
	case Opcode::VLD4:
	case Opcode::VLDMDB:
	case Opcode::VLDR:
	case Opcode::VMAXNM:
	case Opcode::VMAX:
	case Opcode::VMINNM:
	case Opcode::VMIN:
	case Opcode::VMLA:
	case Opcode::VMLAL:
	case Opcode::VMLS:
	case Opcode::VMLSL:
	case Opcode::VMOVL:
	case Opcode::VMOVN:
	case Opcode::VMSR:
	case Opcode::VMUL:
	case Opcode::VMULL:
	case Opcode::VMVN:
	case Opcode::VNEG:
	case Opcode::VNMLA:
	case Opcode::VNMLS:
	case Opcode::VNMUL:
	case Opcode::VORN:
	case Opcode::VORR:
	case Opcode::VPADAL:
	case Opcode::VPADDL:
	case Opcode::VPADD:
	case Opcode::VPMAX:
	case Opcode::VPMIN:
	case Opcode::VQABS:
	case Opcode::VQADD:
	case Opcode::VQDMLAL:
	case Opcode::VQDMLSL:
	case Opcode::VQDMULH:
	case Opcode::VQDMULL:
	case Opcode::VQMOVUN:
	case Opcode::VQMOVN:
	case Opcode::VQNEG:
	case Opcode::VQRDMULH:
	case Opcode::VQRSHL:
	case Opcode::VQRSHRN:
	case Opcode::VQRSHRUN:
	case Opcode::VQSHL:
	case Opcode::VQSHLU:
	case Opcode::VQSHRN:
	case Opcode::VQSHRUN:
	case Opcode::VQSUB:
	case Opcode::VRADDHN:
	case Opcode::VRECPE:
	case Opcode::VRECPS:
	case Opcode::VREV16:
	case Opcode::VREV32:
	case Opcode::VREV64:
	case Opcode::VRHADD:
	case Opcode::VRINTA:
	case Opcode::VRINTM:
	case Opcode::VRINTN:
	case Opcode::VRINTP:
	case Opcode::VRINTR:
	case Opcode::VRINTX:
	case Opcode::VRINTZ:
	case Opcode::VRSHL:
	case Opcode::VRSHRN:
	case Opcode::VRSHR:
	case Opcode::VRSQRTE:
	case Opcode::VRSQRTS:
	case Opcode::VRSRA:
	case Opcode::VRSUBHN:
	case Opcode::VSELEQ:
	case Opcode::VSELGE:
	case Opcode::VSELGT:
	case Opcode::VSELVS:
	case Opcode::VSHLL:
	case Opcode::VSHL:
	case Opcode::VSHRN:
	case Opcode::VSHR:
	case Opcode::VSLI:
	case Opcode::VSQRT:
	case Opcode::VSRA:
	case Opcode::VSRI:
	case Opcode::VST1:
	case Opcode::VST2:
	case Opcode::VST3:
	case Opcode::VST4:
	case Opcode::VSTMDB:
	case Opcode::VSTR:
	case Opcode::VSUB:
	case Opcode::VSUBHN:
	case Opcode::VSUBL:
	case Opcode::VSUBW:
	case Opcode::VSWP:
	case Opcode::VTBL:
	case Opcode::VTBX:
	case Opcode::VCVTR:
	case Opcode::VTRN:
	case Opcode::VTST:
	case Opcode::VUZP:
	case Opcode::VZIP:
	case Opcode::ADDW:
	case Opcode::ASR:
	case Opcode::DCPS1:
	case Opcode::DCPS2:
	case Opcode::DCPS3:
	case Opcode::IT:
	case Opcode::LSL:
	case Opcode::LSR:
	case Opcode::ASRS:
	case Opcode::LSRS:
	case Opcode::ORN:
	case Opcode::ROR:
	case Opcode::RRX:
	case Opcode::SUBS:
	case Opcode::SUBW:
	case Opcode::TBB:
	case Opcode::TBH:
	case Opcode::CBNZ:
	case Opcode::CBZ:
	case Opcode::MOVS:
	case Opcode::YIELD:
	case Opcode::WFE:
	case Opcode::WFI:
	case Opcode::SEV:
	case Opcode::SEVL:
	case Opcode::VPUSH:
	case Opcode::VPOP:
		NotImplementedYet();
		break;

	case Opcode::ADC: RewriteAdcSbc(&IRtlEmitter::IAdd); break;
	case Opcode::ADD: RewriteBinOp(&IRtlEmitter::IAdd, instr.ArchitectureDetail.UpdateFlags); break;
	case Opcode::AND: RewriteBinOp(&IRtlEmitter::And, instr.ArchitectureDetail.UpdateFlags); break;
	case Opcode::EOR: RewriteBinOp(&IRtlEmitter::Xor, instr.ArchitectureDetail.UpdateFlags); break;
	case Opcode::B: RewriteB(false); break;
	case Opcode::BFC: RewriteBfc(); break;
	case Opcode::BFI: RewriteBfi(); break;
	case Opcode::BIC: RewriteBic(); break;
	case Opcode::BL: RewriteB(true); break;
	case Opcode::BLX: RewriteB(true); break;
	case Opcode::BX: RewriteB(false); break;
	case Opcode::CLZ: RewriteClz(); break;
	case Opcode::CMN: RewriteCmn(); break;
	case Opcode::CMP: RewriteCmp(); break;
	case Opcode::CPS: RewriteCps(); break;
	case Opcode::DMB: RewriteDmb(); break;
	case Opcode::LDR: RewriteLdr(PrimitiveType::Word32); break;
	case Opcode::LDRB: RewriteLdr(PrimitiveType::Byte); break;
	case Opcode::LDRH: RewriteLdr(PrimitiveType::UInt16); break;
	case Opcode::LDRSB: RewriteLdr(PrimitiveType::SByte); break;
	case Opcode::LDRSH: RewriteLdr(PrimitiveType::Int16); break;
	case Opcode::LDRD: RewriteLdrd(); break;
	case Opcode::LDM: RewriteLdm(0); break;
	case Opcode::LDMDB: RewriteLdm(0); break;
	case Opcode::LDMIB: RewriteLdm(4); break;
	case Opcode::NOP: m.Nop(); break;
	case Opcode::MCR: RewriteMcr(); break;
	case Opcode::MLA: RewriteMultiplyAccumulate(&IRtlEmitter::IAdd); break;
	case Opcode::MLS: RewriteMultiplyAccumulate(&IRtlEmitter::ISub); break;
	case Opcode::MOV: RewriteMov(); break;
	case Opcode::MOVT: RewriteMovt(); break;
	case Opcode::MOVW: RewriteMov(); break;
	case Opcode::MRC: RewriteMrc(); break;
	case Opcode::MRS: RewriteMrs(); break;
	case Opcode::MSR: RewriteMsr(); break;
	case Opcode::MUL: RewriteBinOp(&IRtlEmitter::IMul, instr.ArchitectureDetail.UpdateFlags); break;
	case Opcode::MVN: RewriteUnaryOp(&IRtlEmitter::Not); break;
	case Opcode::ORR: RewriteBinOp(&IRtlEmitter::Or, false); break;
	case Opcode::POP: RewritePop(); break;
	case Opcode::PUSH: RewritePush(); break;
	case Opcode::REV: RewriteRev(); break;
	case Opcode::RSB: RewriteRevBinOp(&IRtlEmitter::ISub, instr.ArchitectureDetail.UpdateFlags); break;
	case Opcode::SBC: RewriteAdcSbc(&IRtlEmitter::ISub); break;
	case Opcode::SBFX: RewriteSbfx(); break;
	case Opcode::SMULBB: RewriteMulbb(false, false, PrimitiveType::Int16, &IRtlEmitter::SMul); break;
	case Opcode::SMULL: RewriteMull(PrimitiveType::Int64, &IRtlEmitter::SMul); break;
	case Opcode::STM: RewriteStm(); break;
	case Opcode::STMDB: RewriteStm(); break;
	case Opcode::STMIB: RewriteStmib(); break;
	case Opcode::STR: RewriteStr(PrimitiveType::Word32); break;
	case Opcode::STRB: RewriteStr(PrimitiveType::Byte); break;
	case Opcode::STRD: RewriteStrd(); break;
	case Opcode::STRH: RewriteStr(PrimitiveType::UInt16); break;
	case Opcode::SUB: RewriteBinOp(&IRtlEmitter::ISub, instr.ArchitectureDetail.UpdateFlags); break;
	case Opcode::SVC: RewriteSvc(); break;
	case Opcode::SXTAB: RewriteXtab(PrimitiveType::SByte); break;
	case Opcode::SXTAH: RewriteXtab(PrimitiveType::Int16); break;
	case Opcode::SXTB: RewriteXtb(PrimitiveType::SByte); break;
	case Opcode::SXTH: RewriteXtb(PrimitiveType::Int16); break;
	case Opcode::TEQ: RewriteTeq(); break;
	case Opcode::TST: RewriteTst(); break;
	case Opcode::UBFX: RewriteUbfx(); break;
	case Opcode::UMLAL: RewriteUmlal(); break;
	case Opcode::UMULL: RewriteMull(PrimitiveType::UInt64, &IRtlEmitter::UMul); break;
	case Opcode::UXTAB: RewriteXtab(PrimitiveType::Byte); break;
	case Opcode::UXTAH: RewriteXtab(PrimitiveType::UInt16); break;
	case Opcode::UXTB: RewriteXtb(PrimitiveType::Byte); break;
	case Opcode::UXTH: RewriteXtb(PrimitiveType::UInt16); break;

	case Opcode::VLDMIA: RewriteVldmia(); break;
	case Opcode::VMOV: RewriteVmov(); break;
	case Opcode::VSTMIA: RewriteVstmia(); break;

	}
}

void ArmRewriter::NotImplementedYet()
{
	char buf[200];	//$TODO: hello buffer overflow!
	::snprintf(buf, sizeof(buf), "Rewriting ARM opcode '%s' is not supported yet.", instr.Mnemonic);
	host.Error(
		instr.Address,
		buf);
	m.Invalid();
}

void ArmRewriter::MaybeUpdateFlags(IExpression * opDst)
{
	if (instr.ArchitectureDetail.UpdateFlags)
	{
		m.Assign(frame.EnsureFlagGroup((int)ArmRegister::CPSR, 0x1111, "NZCV", PrimitiveType::Byte), m.Cond(opDst));
	}
}

void ArmRewriter::RewriteB(bool link)
{
	IExpression * dst;
	bool dstIsAddress;
	if (Dst().Type == ArmInstructionOperandType::Immediate)
	{
		dst = m.Ptr32(Dst().ImmediateValue);
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
		if (instr.ArchitectureDetail.CodeCondition == ArmCodeCondition::AL)
		{
			m.Call(dst, 0);
		}
		else
		{
			//$TODO: conditional code.
			//m.If(TestCond(instr.ArchitectureDetail.CodeCondition), new RtlCall(dst, 0, RtlClass::Transfer));
		}
	}
	else
	{
		if (instr.ArchitectureDetail.CodeCondition == ArmCodeCondition::AL)
		{
			m.SetRtlClass(RtlClass::Transfer);
			m.Goto(dst);
		}
		else
		{
			m.SetRtlClass(RtlClass::ConditionalTransfer);
			if (dstIsAddress)
			{
				m.Branch(TestCond(instr.ArchitectureDetail.CodeCondition), dst, RtlClass::ConditionalTransfer);
			}
			else
			{
				//$TODO: conditional code
				//m.If(TestCond(instr.ArchitectureDetail.CodeCondition), new RtlGoto(dst, RtlClass::ConditionalTransfer));
			}
		}
	}
}

void ArmRewriter::AddConditional(void (*mkInstr)())
{
	//if (instr.ArchitectureDetail.CodeCondition != ArmCodeCondition.AL)
	//{
	//	rtlInstr = new RtlIf(TestCond(instr.ArchitectureDetail.CodeCondition), rtlInstr);
	//}
	//ric.Instructions.Add(rtlInstr);
}

void ArmRewriter::ConditionalAssign(IExpression * dst, IExpression * src)
{
	/*RtlInstruction rtlInstr = new RtlAssignment(dst, src);
	if (instr.ArchitectureDetail.CodeCondition != ArmCodeCondition::AL)
	{
		rtlInstr = new RtlIf(TestCond(instr.ArchitectureDetail.CodeCondition), rtlInstr);
	}
	ric.Instructions.Add(rtlInstr);*/
}

// If a conditional ARM instruction is encountered, generate an IL
// instruction to skip the remainder of the instruction cluster.
void ArmRewriter::ConditionalSkip()
{
	auto cc = instr.ArchitectureDetail.CodeCondition;
	if (cc == ArmCodeCondition::AL)
		return; // never skip!
	m.BranchInMiddleOfInstruction(
		TestCond(Invert(cc)),
		m.Ptr32(instr.Address + 4),
		RtlClass::ConditionalTransfer);
}

ArmCodeCondition ArmRewriter::Invert(ArmCodeCondition cc)
{
	switch (cc)
	{
	case ArmCodeCondition::EQ: return ArmCodeCondition::NE;
	case ArmCodeCondition::NE: return ArmCodeCondition::EQ;
	case ArmCodeCondition::HS: return ArmCodeCondition::LO;
	case ArmCodeCondition::LO: return ArmCodeCondition::HS;
	case ArmCodeCondition::MI: return ArmCodeCondition::PL;
	case ArmCodeCondition::PL: return ArmCodeCondition::MI;
	case ArmCodeCondition::VS: return ArmCodeCondition::VC;
	case ArmCodeCondition::VC: return ArmCodeCondition::VS;
	case ArmCodeCondition::HI: return ArmCodeCondition::LS;
	case ArmCodeCondition::LS: return ArmCodeCondition::HI;
	case ArmCodeCondition::GE: return ArmCodeCondition::LT;
	case ArmCodeCondition::LT: return ArmCodeCondition::GE;
	case ArmCodeCondition::GT: return ArmCodeCondition::LE;
	case ArmCodeCondition::LE: return ArmCodeCondition::GT;
	case ArmCodeCondition::AL: return ArmCodeCondition::Invalid;
	}
	return ArmCodeCondition::Invalid;
}

IExpression * ArmRewriter::Operand(const ArmInstructionOperand & op)
{
	switch (op.Type)
	{
	case ArmInstructionOperandType::Register:
	{
		auto reg = frame.EnsureRegister((int)op.RegisterValue);
		return MaybeShiftOperand(reg, op);
	}
	case ArmInstructionOperandType::SysRegister:
	{
		auto sysreg = frame.EnsureRegister((int)op.SysRegisterValue);
		return sysreg;
	}
	case ArmInstructionOperandType::Immediate:
		return m.Word32(op.ImmediateValue);
	case ArmInstructionOperandType::CImmediate:
	case ArmInstructionOperandType::PImmediate:
		return m.Byte((uint8_t)op.ImmediateValue);
	case ArmInstructionOperandType::Memory:
	{
		auto baseReg = Reg(op.MemoryValue.BaseRegister);
		auto ea = baseReg;
		if (op.MemoryValue.BaseRegister == ArmRegister::PC)
		{
			// PC-relative address
			if (op.MemoryValue.Displacement != 0)
			{
				auto dst = (uint32_t)((int32_t)instr.Address + op.MemoryValue.Displacement) + 8u;
				return m.Mem(SizeFromLoadStore(), m.Ptr32(dst));
			}
		}
		if (op.MemoryValue.Displacement != 0 && instr.IsLastOperand(op))
		{
			auto offset = m.Int32(op.MemoryValue.Displacement);
			ea = op.MemoryValue.IndexRegisterScale < 0
				? m.ISub(ea, offset)
				: m.IAdd(ea, offset);
		}
		if (instr.IsLastOperand(op) && instr.ArchitectureDetail.WriteBack)
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
	switch (instr.Id)
	{
	case Opcode::LDR: return PrimitiveType::Word32;
	case Opcode::LDRB: return PrimitiveType::Byte;
	case Opcode::LDRD: return PrimitiveType::Word64;
	case Opcode::LDRH: return PrimitiveType::Word16;
	case Opcode::LDRSB: return PrimitiveType::SByte;
	case Opcode::LDRSH: return PrimitiveType::Int16;
	case Opcode::STR: return PrimitiveType::Word32;
	case Opcode::STRB: return PrimitiveType::Byte;
	case Opcode::STRD: return PrimitiveType::Word64;
	case Opcode::STRH: return PrimitiveType::Word16;
	}
	//assert(false && instr.Id.ToString());
	return PrimitiveType::Void;
}


IExpression * ArmRewriter::MaybeShiftOperand(IExpression * exp, ArmInstructionOperand op)
{
	switch (op.Shifter.Type)
	{
	case ArmShifterType::ASR: return m.Sar(exp, op.Shifter.Value);
	case ArmShifterType::LSL: return m.Shl(exp, op.Shifter.Value);
	case ArmShifterType::LSR: return m.Shr(exp, op.Shifter.Value);
	case ArmShifterType::ROR: return m.Ror(exp, m.Int32(op.Shifter.Value));
	case ArmShifterType::RRX: return m.Rrc(exp, m.Int32(op.Shifter.Value));
	case ArmShifterType::ASR_REG: return m.Sar(exp, Reg(op.Shifter.Value));
	case ArmShifterType::LSL_REG: return m.Shl(exp, Reg(op.Shifter.Value));
	case ArmShifterType::LSR_REG: return m.Shr(exp, Reg(op.Shifter.Value));
	case ArmShifterType::ROR_REG: return m.Ror(exp, Reg(op.Shifter.Value));
	case ArmShifterType::RRX_REG: return m.Rrc(exp, Reg(op.Shifter.Value));
	default: return exp;
	}
}

void ArmRewriter::MaybePostOperand(const ArmInstructionOperand & op)
{
	if (instr.IsLastOperand(op))
		return;
	if (op.Type != ArmInstructionOperandType::Memory)
		return;
	auto lastOp = instr.ArchitectureDetail.Operands[instr.ArchitectureDetail.Length - 1];
	auto baseReg = Reg(op.MemoryValue.BaseRegister);
	auto offset = Operand(lastOp);
	auto ea = lastOp.IsSubtracted
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

IExpression * ArmRewriter::TestCond(ArmCodeCondition cond)
{
	switch (cond)
	{
	//default:
	//	throw new NotImplementedException(string.Format("ARM condition code {0} not implemented.", cond));
	case ArmCodeCondition::HS:
		return m.Test(ConditionCode::UGE, FlagGroup(FlagM::CF, "C", PrimitiveType::Byte));
	case ArmCodeCondition::LO:
		return m.Test(ConditionCode::ULT, FlagGroup(FlagM::CF, "C", PrimitiveType::Byte));
	case ArmCodeCondition::EQ:
		return m.Test(ConditionCode::EQ, FlagGroup(FlagM::ZF, "Z", PrimitiveType::Byte));
	case ArmCodeCondition::GE:
		return m.Test(ConditionCode::GE, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV", PrimitiveType::Byte));
	case ArmCodeCondition::GT:
		return m.Test(ConditionCode::GT, FlagGroup(FlagM::NF | FlagM::ZF | FlagM::VF, "NZV", PrimitiveType::Byte));
	case ArmCodeCondition::HI:
		return m.Test(ConditionCode::UGT, FlagGroup(FlagM::ZF | FlagM::CF, "ZC", PrimitiveType::Byte));
	case ArmCodeCondition::LE:
		return m.Test(ConditionCode::LE, FlagGroup(FlagM::ZF | FlagM::CF | FlagM::VF, "NZV", PrimitiveType::Byte));
	case ArmCodeCondition::LS:
		return m.Test(ConditionCode::ULE, FlagGroup(FlagM::ZF | FlagM::CF, "ZC", PrimitiveType::Byte));
	case ArmCodeCondition::LT:
		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF | FlagM::VF, "NV", PrimitiveType::Byte));
	case ArmCodeCondition::MI:
		return m.Test(ConditionCode::LT, FlagGroup(FlagM::NF, "N", PrimitiveType::Byte));
	case ArmCodeCondition::PL:
		return m.Test(ConditionCode::GT, FlagGroup(FlagM::NF | FlagM::ZF, "NZ", PrimitiveType::Byte));
	case ArmCodeCondition::NE:
		return m.Test(ConditionCode::NE, FlagGroup(FlagM::ZF, "Z", PrimitiveType::Byte));
	case ArmCodeCondition::VS:
		return m.Test(ConditionCode::OV, FlagGroup(FlagM::VF, "V", PrimitiveType::Byte));
	}
	return 0;
}

IExpression * ArmRewriter::FlagGroup(FlagM bits, const char * name, PrimitiveType type)
{
	return frame.EnsureFlagGroup((int)ArmRegister::CPSR, (int) bits, name, type);
}

void ArmRewriter::RewriteSvc()
{
	//$TODO
	//m.SideEffect(m.Fn(
	//	host.EnsurePseudoProcedure(PseudoProcedure.Syscall, VoidType.Instance, 2),
	//	Operand(Dst)));
}
