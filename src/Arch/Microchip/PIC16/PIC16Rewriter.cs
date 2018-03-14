using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Libraries.Microchip;

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    public class PIC16Rewriter : IEnumerable<RtlInstructionCluster>
    {

        private PIC16Architecture arch;
        private IStorageBinder binder;
        private IRewriterHost host;
        private PIC16DisassemblerBase disasm;
        private IEnumerator<PIC16Instruction> dasm;
        private EndianImageReader rdr;
        private PIC16State state;
        private PIC16Instruction instrCurr;
        private RtlClass rtlc;
        private List<RtlInstruction> rtlInstructions;
        private RtlEmitter m;
        private Identifier Wreg;    // cached WREG register identifier

        public PIC16Rewriter(PIC16Architecture arch, EndianImageReader rdr, PIC16State state, IStorageBinder binder, IRewriterHost host)
        {
            this.arch = arch;
            this.rdr = rdr;
            this.state = state;
            this.binder = binder;
            this.host = host;
            disasm = new PIC16DisassemblerBase(arch, rdr);
            dasm = disasm.GetEnumerator();
        }

        public IEnumerator<RtlInstructionCluster> GetEnumerator()
        {
            while (dasm.MoveNext())
            {
                instrCurr = dasm.Current;
                var addr = instrCurr.Address;
                var len = instrCurr.Length;
                rtlc = RtlClass.Linear;
                rtlInstructions = new List<RtlInstruction>();
                m = new RtlEmitter(rtlInstructions);
                Wreg = binder.EnsureRegister(PIC16Registers.WREG);

                switch (instrCurr.Opcode)
                {
                    default:
                        throw new AddressCorrelatedException(addr, $"Rewriting of PIC16 instruction '{instrCurr.Opcode}' is not implemented yet.");

                    case Opcode.invalid:
                    case Opcode.unaligned:
                        m.Invalid();
                        break;

                    case Opcode.ADDFSR:
                    case Opcode.ADDLW:
                    case Opcode.ADDWF:
                    case Opcode.ADDWFC:
                    case Opcode.ANDLW:
                    case Opcode.ANDWF:
                    case Opcode.ASRF:
                    case Opcode.BCF:
                    case Opcode.BRA:
                    case Opcode.BRW:
                    case Opcode.BSF:
                    case Opcode.BTFSC:
                    case Opcode.BTFSS:
                    case Opcode.CALL:
                    case Opcode.CALLW:
                    case Opcode.CLRF:
                    case Opcode.CLRW:
                    case Opcode.CLRWDT:
                    case Opcode.COMF:
                    case Opcode.DECF:
                    case Opcode.DECFSZ:
                    case Opcode.GOTO:
                    case Opcode.INCF:
                    case Opcode.INCFSZ:
                    case Opcode.IORLW:
                    case Opcode.IORWF:
                    case Opcode.LSLF:
                    case Opcode.LSRF:
                    case Opcode.MOVF:
                    case Opcode.MOVIW:
                    case Opcode.MOVLB:
                    case Opcode.MOVLP:
                    case Opcode.MOVLW:
                    case Opcode.MOVWF:
                    case Opcode.MOVWI:
                    case Opcode.NOP:
                    case Opcode.OPTION:
                    case Opcode.RESET:
                    case Opcode.RETFIE:
                    case Opcode.RETLW:
                    case Opcode.RETURN:
                    case Opcode.RLF:
                    case Opcode.RRF:
                    case Opcode.SLEEP:
                    case Opcode.SUBLW:
                    case Opcode.SUBWF:
                    case Opcode.SUBWFB:
                    case Opcode.SWAPF:
                    case Opcode.TRIS:
                    case Opcode.XORLW:
                    case Opcode.XORWF:

                    // Pseudo-instructions
                    case Opcode.__CONFIG:
                    case Opcode.DA:
                    case Opcode.DB:
                    case Opcode.DE:
                    case Opcode.DT:
                    case Opcode.DTM:
                    case Opcode.DW:
                    case Opcode.__IDLOCS:
                        m.Invalid();
                        break;
                }
                yield return new RtlInstructionCluster(addr, len, rtlInstructions.ToArray())
                {
                    Class = rtlc
                };
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Helpers

        private Identifier FlagGroup(FlagM flags)
        {
            return binder.EnsureFlagGroup(PIC16Registers.STATUS, (uint)flags, arch.GrfToString((uint)flags), PrimitiveType.Byte);
        }

        private ArrayAccess PushToHWStackAccess()
        {
            var stkptr = binder.EnsureRegister(arch.StackRegister);
            var slot = m.ARef(PrimitiveType.Ptr32, PIC16Registers.GlobalStack, stkptr);
            m.Assign(stkptr, m.IAdd(stkptr, Constant.Byte(1)));
            return slot;
        }

        private ArrayAccess PopFromHWStackAccess()
        {
            var stkptr = binder.EnsureRegister(arch.StackRegister);
            m.Assign(stkptr, m.ISub(stkptr, Constant.Byte(1)));
            var slot = m.ARef(PrimitiveType.Ptr32, PIC16Registers.GlobalStack, stkptr);
            return slot;
        }

        private static MemoryAccess DataMem8(Expression ea)
            => new MemoryAccess(PIC16Registers.GlobalData, ea, PrimitiveType.Byte);

        private Expression GetFSRRegister(MachineOperand op)
        {
            if (op is PIC16FSROperand fsr)
            {
                switch (fsr.FSRNum.ToByte())
                {
                    case 0:
                        return binder.EnsureRegister(PIC16Registers.FSR0);
                    case 1:
                        return binder.EnsureRegister(PIC16Registers.FSR1);
                    default:
                        throw new InvalidOperationException($"Invalid FSR number: {fsr.FSRNum.ToByte()}");
                }
            }
            else
                throw new InvalidOperationException($"Invalid FSR operand.");

        }

        private Expression GetImmediateValue(MachineOperand op)
        {
            switch (op)
            {
                case PIC16ImmediateOperand imm:
                    return imm.ImmediateValue;

                default:
                    throw new InvalidOperationException($"Invalid immediate operand.");
            }
        }

        private Expression GetProgramAddress(MachineOperand op)
        {
            switch (op)
            {
                case PIC16ProgAddrOperand paddr:
                    return PICProgAddress.Ptr(paddr.CodeTarget);

                default:
                    throw new InvalidOperationException($"Invalid program address operand.");
            }
        }

        private Constant GetBitMask(MachineOperand op, bool revert)
        {
            switch (op)
            {
                case PIC16DataBitOperand bitaddr:
                    int mask = (1 << bitaddr.BitNumber.ToByte());
                    if (revert)
                        mask = ~mask;
                    return Constant.Byte((byte)mask);

                default:
                    throw new InvalidOperationException("Invalid bit number operand.");
            }
        }

        #endregion

        #region Rewrite methods

        #endregion

    }

}
