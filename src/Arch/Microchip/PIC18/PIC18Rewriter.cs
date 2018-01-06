using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Microchip.Crownking;

namespace Reko.Arch.Microchip.PIC18
{
    public class PIC18Rewriter : IEnumerable<RtlInstructionCluster>
    {
        private PIC18Architecture arch;
        private IStorageBinder binder;
        private IRewriterHost host;
        private PIC18Disassembler disasm;
        private IEnumerator<PIC18Instruction> dasm;
        private ProcessorState state;
        private PIC18Instruction instr;
        private RtlClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private List<RtlInstructionCluster> clusters;
        private RtlEmitter m;

        public PIC18Rewriter(PIC18Architecture arch, EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.disasm = new PIC18Disassembler(arch, rdr);
            this.dasm = disasm.GetEnumerator();
            this.state = state;
            this.binder = binder;
            this.host = host;
        }

        /// <summary>
        /// Gets or sets the PIC18 execution mode.
        /// </summary>
        public PICExecMode ExecMode
        {
            get
            {
                return disasm.ExecMode;
            }
            set
            {
                disasm.ExecMode = value;
            }
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instr = dasm.Current;
                var addr = instr.Address;
                var len = instr.Length;
                rtlc = RtlClass.Linear;
                rtlInstructions = new List<RtlInstruction>();
                m = new RtlEmitter(rtlInstructions);
                switch (instr.Opcode)
                {
                    default:
                        throw new AddressCorrelatedException(addr, $"Rewriting of PIC18 instruction '{instr.Opcode}' is not implemented yet.");

                    case Opcode.invalid:
                        m.Invalid(); break;

                    case Opcode.ADDFSR:
                        break;

                    case Opcode.ADDLW:
                        RewriteADDLW(); break;

                    case Opcode.ADDULNK:
                        break;

                    case Opcode.ADDWF:
                        break;

                    case Opcode.ADDWFC:
                        break;

                    case Opcode.ANDLW:
                        RewriteANDLW(); break;

                    case Opcode.ANDWF:
                        break;

                    case Opcode.BC:
                        break;

                    case Opcode.BCF:
                        break;

                    case Opcode.BN:
                        break;

                    case Opcode.BNC:
                        break;

                    case Opcode.BNN:
                        break;

                    case Opcode.BNOV:
                        break;

                    case Opcode.BNZ:
                        break;

                    case Opcode.BOV:
                        break;

                    case Opcode.BRA:
                        break;

                    case Opcode.BSF:
                        break;

                    case Opcode.BTFSC:
                        break;

                    case Opcode.BTFSS:
                        break;

                    case Opcode.BTG:
                        break;

                    case Opcode.BZ:
                        break;

                    case Opcode.CALL:
                        break;

                    case Opcode.CALLW:
                        break;

                    case Opcode.CLRF:
                        break;

                    case Opcode.CLRWDT:
                        break;

                    case Opcode.COMF:
                        break;

                    case Opcode.CPFSEQ:
                        break;

                    case Opcode.CPFSGT:
                        break;

                    case Opcode.CPFSLT:
                        break;

                    case Opcode.DAW:
                        break;

                    case Opcode.DCFSNZ:
                        break;

                    case Opcode.DECF:
                        break;

                    case Opcode.DECFSZ:
                        break;

                    case Opcode.GOTO:
                        break;

                    case Opcode.INCF:
                        break;

                    case Opcode.INCFSZ:
                        break;

                    case Opcode.INFSNZ:
                        break;

                    case Opcode.IORLW:
                        RewriteIORLW(); break;

                    case Opcode.IORWF:
                        break;

                    case Opcode.LFSR:
                        break;

                    case Opcode.MOVF:
                        break;

                    case Opcode.MOVFF:
                        break;

                    case Opcode.MOVFFL:
                        break;

                    case Opcode.MOVLB:
                        break;

                    case Opcode.MOVLW:
                        RewriteMOVLW(); break;

                    case Opcode.MOVSF:
                        break;

                    case Opcode.MOVSFL:
                        break;

                    case Opcode.MOVSS:
                        break;

                    case Opcode.MOVWF:
                        RewriteMOVWF(); break;

                    case Opcode.MULLW:
                        break;

                    case Opcode.MULWF:
                        break;

                    case Opcode.NEGF:
                        break;

                    case Opcode.NOP:
                        m.Nop(); break;

                    case Opcode.POP:
                        break;

                    case Opcode.PUSH:
                        break;

                    case Opcode.PUSHL:
                        break;

                    case Opcode.RCALL:
                        break;

                    case Opcode.RESET:
                        break;

                    case Opcode.RETFIE:
                        break;

                    case Opcode.RETLW:
                        break;

                    case Opcode.RETURN:
                        break;

                    case Opcode.RLCF:
                        break;

                    case Opcode.RLNCF:
                        break;

                    case Opcode.RRCF:
                        break;

                    case Opcode.RRNCF:
                        break;

                    case Opcode.SETF:
                        break;

                    case Opcode.SLEEP:
                        break;

                    case Opcode.SUBFSR:
                        break;

                    case Opcode.SUBFWB:
                        break;

                    case Opcode.SUBLW:
                        break;

                    case Opcode.SUBULNK:
                        break;

                    case Opcode.SUBWF:
                        break;

                    case Opcode.SUBWFB:
                        break;

                    case Opcode.SWAPF:
                        break;

                    case Opcode.TBLRD:
                        break;

                    case Opcode.TBLWT:
                        break;

                    case Opcode.TSTFSZ:
                        break;

                    case Opcode.XORLW:
                        RewriteXORLW(); break;

                    case Opcode.XORWF:
                        break;
                }
                yield return new RtlInstructionCluster(addr, len, rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
            yield break; ;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); ;
        }

        public Identifier FlagGroup(FlagM flags)
        {
            return binder.EnsureFlagGroup(Registers.status, (uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }

        private void AssignCond(FlagM flags, Expression dst)
        {
            m.Assign(FlagGroup(flags), m.Cond(dst));
        }

        private Expression RewriteOp(PIC18OperandImpl op)
        {
            var imm4Op = op as PIC18Immed4Operand;
            if (imm4Op != null)
                return imm4Op.Imm4;
            var imm6Op = op as PIC18Immed6Operand;
            if (imm6Op != null)
                return imm6Op.Imm6;
            var imm8Op = op as PIC18Immed8Operand;
            if (imm8Op != null)
                return imm8Op.Imm8;
            var imm12Op = op as PIC18Immed12Operand;
            if (imm6Op != null)
                return imm12Op.Imm12;
            var imm14Op = op as PIC18Immed14Operand;
            if (imm14Op != null)
                return imm14Op.Imm14;
            var bankaccess = op as PIC18DataBankAccessOperand;
            if (bankaccess != null)
                return GetMemoryAccess(bankaccess);
            var fsridxOp = op as PIC18FSR2IdxOperand;
            if (fsridxOp != null)
                return fsridxOp.Offset;
            var fsrnumOp = op as PIC18FSRNumOperand;
            if (fsrnumOp != null)
                return fsrnumOp.FSRNum;
            var tblincrmod = op as PIC18TableReadWriteOperand;
            if (tblincrmod != null)
                return tblincrmod.TBLIncrMode;
            var mem12bitaddr = op as PIC18Memory12bitAbsAddrOperand;
            if (mem12bitaddr != null)
                return mem12bitaddr.Addr12;
            var mem14bitaddr = op as PIC18Memory14bitAbsAddrOperand;
            if (mem14bitaddr != null)
                return mem14bitaddr.Addr14;
            var fsr2idx = op as PIC18FSR2IdxOperand;
            if (fsr2idx != null)
                return fsr2idx.Offset;
            var shadow = op as PIC18ShadowOperand;
            if (shadow != null)
                return shadow.IsShadow;

            /*
            var rOp = op as RegisterOperand;
            if (rOp != null)
                return binder.EnsureRegister(rOp.Register);
            var memOp = op as MemoryOperand;
            if (memOp != null)
            {
                Identifier bReg = null;
                if (memOp.Base != null)
                    bReg = binder.EnsureRegister(memOp.Base);
                if (memOp.Offset == null)
                {
                    return m.Load(memOp.Width, bReg);
                }
                else if (bReg == null)
                {
                    return m.Load(memOp.Width, memOp.Offset);
                }
                else
                {
                    int s = memOp.Offset.ToInt32();
                    if (s > 0)
                    {
                        return m.Load(memOp.Width, m.IAdd(bReg, s));
                    }
                    else if (s < 0)
                    {
                        return m.Load(memOp.Width, m.ISub(bReg, -s));
                    }
                    else
                    {
                        return m.Load(memOp.Width, bReg);
                    }
                }
            }
             */
            throw new NotImplementedException($"Rewriting of PIC18 operand type {op.GetType().FullName} is not implemented yet.");
        }

        private Expression GetMemoryAccess(PIC18DataBankAccessOperand mem)
        {
            var regaddr = mem.MemAddr;
            if ((mem.ExecMode == PICExecMode.Extended) && mem.IsAccessRAM.ToBoolean() &&  (regaddr.ToByte() <= 0x5F))
            {
                var fsr2h = binder.EnsureRegister(Registers.fsr2h);
                var fsr2l = binder.EnsureRegister(Registers.fsr2l);
            }
            else
            {
                if (mem.IsAccessRAM.ToBoolean())
                { }
                    else
                { }
            }
            return null;
        }

        private void RewriteADDLW()
        {
            var dst = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            m.Assign(dst, m.IAdd(dst, src));
            AssignCond(FlagM.C | FlagM.DC | FlagM.Z | FlagM.OV | FlagM.N, dst);
        }

        private void RewriteANDLW()
        {
            var dst = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            m.Assign(dst, m.And(dst, src));
            AssignCond(FlagM.Z | FlagM.N, dst);
        }

        private void RewriteIORLW()
        {
            var dst = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            m.Assign(dst, m.Or(dst, src));
            AssignCond(FlagM.Z | FlagM.N, dst);
        }

        private void RewriteMOVLW()
        {
            var dst = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            m.Assign(dst, src);
        }

        private void RewriteXORLW()
        {
            var dst = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            m.Assign(dst, m.Xor(dst, src));
            AssignCond(FlagM.Z | FlagM.N, dst);
        }

        private void RewriteMOVWF()
        {
            var dst = binder.EnsureRegister(Registers.wreg);
            var src = RewriteOp(instr.op1);
            m.Assign(dst, src);
        }

    }

}
