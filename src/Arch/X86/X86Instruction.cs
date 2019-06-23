#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Types;
using Reko.Core.Machine;
using System;
using System.Text;
using System.Collections.Generic;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Models an X86 instruction.
    /// </summary>
    public class X86Instruction : MachineInstruction
	{
        private const InstrClass CondLinear = InstrClass.Conditional | InstrClass.Linear;
        private const InstrClass CondTransfer = InstrClass.Conditional | InstrClass.Transfer;
        private const InstrClass LinkTransfer = InstrClass.Call | InstrClass.Transfer;
        private const InstrClass Transfer = InstrClass.Transfer;

		public Opcode code;		        // Opcode of the instruction.
        public int repPrefix;           // 0 = no prefix, 2 = repnz, 3 = repz
		public PrimitiveType dataWidth;	// Width of the data (if it's a word).
		public PrimitiveType addrWidth;	// width of the address mode.	// TODO: belongs in MemoryOperand
		public MachineOperand op1;
		public MachineOperand op2;
		public MachineOperand op3;

		public X86Instruction(Opcode code, InstrClass iclass, PrimitiveType dataWidth, PrimitiveType addrWidth, params MachineOperand [] ops)
		{
			this.code = code;
            this.InstructionClass = iclass;
			this.dataWidth = dataWidth;
			this.addrWidth = addrWidth;
			if (ops.Length >= 1)
			{
				op1 = ops[0];
				if (ops.Length >= 2)
				{
					op2 = ops[1];
					if (ops.Length == 3)
						op3 = ops[2];
					else if (ops.Length >= 4)
						throw new ArgumentException("Too many operands.");
				}
			}
		}

        public override int OpcodeAsInteger => (int) code;

		private bool NeedsExplicitMemorySize()
		{
			if (code == Opcode.movsx || code == Opcode.movzx)
				return true;
            if (code == Opcode.lea ||
                code == Opcode.lds ||
                code == Opcode.les ||
                code == Opcode.lfs || 
                code == Opcode.lgs || 
                code == Opcode.lss)
                return false;
            if (Operands >= 2 && op1.Width.Size != op2.Width.Size)
                return true;
			return 
				 (Operands < 1 || !ImplicitWidth(op1)) &&
				 (Operands < 2 || !ImplicitWidth(op2)) &&
				 (Operands < 3 || !ImplicitWidth(op3));
		}

        public byte Operands
        {
            get { 
                if (op1 == null)
                return 0;
                if (op2 == null)
                    return 1;
                if (op3 == null)
                    return 2;
                return 3;
            }
        }

        public override MachineOperand GetOperand(int i)
        {
            switch (i)
            {
            case 0: return op1;
            case 1: return op2;
            case 2: return op3;
            default: return null;
            }
        }

        public override void Render(MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (repPrefix == 3)
            {
                writer.WriteOpcode("rep");
                writer.WriteChar(' ');
            }
            else if (repPrefix == 2)
            {
                writer.WriteOpcode("repne");
                writer.WriteChar(' ');
            }

            // Get opcode. 

            string s = code.ToString();
			switch (code)
			{
			case Opcode.cwd:
				if (dataWidth == PrimitiveType.Word32)
				{
					s = "cdq";
				}
				break;
			case Opcode.cbw:
				if (dataWidth == PrimitiveType.Word32)
				{
					s = "cwde";
				}
				break;
			case Opcode.ins:
			case Opcode.outs:
			case Opcode.movs:
			case Opcode.cmps:
			case Opcode.stos:
			case Opcode.lods:
			case Opcode.scas:
				switch (dataWidth.Size)
				{
				case 1: s += 'b'; break;
				case 2: s += 'w'; break;
				case 4: s += 'd'; break;
				case 8: s += 'q'; break;
                default: throw new ArgumentOutOfRangeException();
				}
				break;
			}
			writer.WriteOpcode(s);

            if (NeedsExplicitMemorySize())
            {
                options |= MachineInstructionWriterOptions.ExplicitOperandSize;
            }

			if (Operands >= 1)
			{
                writer.Tab();
                Write(op1, writer, options);
				if (Operands >= 2)
				{
					writer.WriteChar(',');
					Write(op2, writer, options);
					if (Operands >= 3)
					{
						writer.WriteString(",");
						Write(op3, writer, options);
					}
				}
			}
		}

        private void Write(MachineOperand op, MachineInstructionWriter writer, MachineInstructionWriterOptions options)
        {
            if (op is MemoryOperand memOp)
            {
                if (memOp.Base == Registers.rip)
                {
                    var addr = this.Address + this.Length + memOp.Offset.ToInt32();
                    if ((options & MachineInstructionWriterOptions.ResolvePcRelativeAddress) != 0)
                    {
                        writer.WriteString("[");
                        writer.WriteAddress(addr.ToString(), addr);
                        writer.WriteString("]");
                        writer.AddAnnotation(op.ToString());
                    }
                    else
                    {
                        op.Write(writer, options);
                        writer.AddAnnotation(addr.ToString());
                    }
                    return;
                }
            }
            op.Write(writer, options);
        }

 

        // Returns the condition codes that an instruction modifies.

        public static FlagM DefCc(Opcode opcode)
		{
			switch (opcode)
			{
			case Opcode.aaa:
			case Opcode.aas:
				return FlagM.CF;
			case Opcode.aad:
			case Opcode.aam:
				return FlagM.SF|FlagM.ZF;
			case Opcode.bt:
			case Opcode.bts:
			case Opcode.btc:
			case Opcode.btr:
				return FlagM.CF;
			case Opcode.clc:
			case Opcode.cmc:
			case Opcode.stc:
				return FlagM.CF;
			case Opcode.cld:
			case Opcode.std:
				return FlagM.DF;
			case Opcode.daa:
			case Opcode.das:
				return FlagM.CF|FlagM.SF|FlagM.ZF;
			case Opcode.adc:
			case Opcode.add:
			case Opcode.sbb:
			case Opcode.sub:
			case Opcode.cmp:
			case Opcode.cmpsb:
			case Opcode.cmps:
			case Opcode.div:
			case Opcode.idiv:
			case Opcode.imul:
			case Opcode.mul:
			case Opcode.neg:
			case Opcode.scas:
			case Opcode.scasb:
				return FlagM.CF|FlagM.SF|FlagM.ZF|FlagM.OF;
			case Opcode.dec:
			case Opcode.inc:
				return FlagM.SF|FlagM.ZF|FlagM.OF;
			case Opcode.rcl:
			case Opcode.rcr:
			case Opcode.rol:
			case Opcode.ror:
				return FlagM.OF|FlagM.CF;		// if shift count > 1, FlagM.OF is not set.
			case Opcode.sar:
			case Opcode.shl:
			case Opcode.shr:
				return FlagM.OF|FlagM.SF|FlagM.ZF|FlagM.CF;	// if shift count > 1, FlagM.OF is not set.
			case Opcode.shld:
			case Opcode.shrd:
				return FlagM.SF|FlagM.ZF|FlagM.CF;
			case Opcode.bsf:
			case Opcode.bsr:
				return FlagM.ZF;
			case Opcode.and:
			case Opcode.or:
            case Opcode.sahf:
            case Opcode.test:
            case Opcode.xadd:
            case Opcode.xor:
				return FlagM.OF|FlagM.SF|FlagM.ZF|FlagM.CF;
			default:
				return 0;
			}
		}

		// Returns the condition codes an instruction uses.

		public static FlagM UseCc(Opcode opcode)
		{
			switch (opcode)
			{
			case Opcode.adc:
			case Opcode.sbb:
				return FlagM.CF;
			case Opcode.daa:
			case Opcode.das:
				return FlagM.CF;
			case Opcode.ins:
			case Opcode.insb:
			case Opcode.lods:
			case Opcode.lodsb:
			case Opcode.movs:
			case Opcode.movsb:
			case Opcode.outs:
			case Opcode.outsb:
			case Opcode.scas:
			case Opcode.scasb:
			case Opcode.stos:
			case Opcode.stosb:
				return FlagM.DF;
            case Opcode.cmova:
            case Opcode.ja:
			case Opcode.seta:
				return FlagM.CF|FlagM.ZF;
            case Opcode.cmovbe:
            case Opcode.jbe:
			case Opcode.setbe:
				return FlagM.CF|FlagM.ZF;
            case Opcode.cmovc:
            case Opcode.jc:
			case Opcode.setc:
				return FlagM.CF;
            case Opcode.cmovg:
            case Opcode.jg:
			case Opcode.setg:
				return FlagM.SF|FlagM.OF|FlagM.ZF;
            case Opcode.cmovge:
            case Opcode.jge:
			case Opcode.setge:
				return FlagM.SF|FlagM.OF;
			case Opcode.cmovl:
            case Opcode.jl:
            case Opcode.setl:
				return FlagM.SF|FlagM.OF;
            case Opcode.cmovle:
            case Opcode.jle:
			case Opcode.setle:
				return FlagM.SF|FlagM.OF|FlagM.ZF;
            case Opcode.cmovnc:
            case Opcode.jnc:
            case Opcode.setnc:
				return FlagM.CF;
            case Opcode.cmovno:
            case Opcode.jno:
			case Opcode.setno:
				return FlagM.OF;
            case Opcode.cmovns:
            case Opcode.jns:
			case Opcode.setns:
				return FlagM.SF;
            case Opcode.cmovnz:
            case Opcode.jnz:
			case Opcode.setnz:
				return FlagM.ZF;
            case Opcode.cmovo:
            case Opcode.jo:
			case Opcode.seto:
				return FlagM.OF;
            case Opcode.cmovpe:
            case Opcode.jpe:
			case Opcode.setpe:
            case Opcode.cmovpo:
            case Opcode.jpo:
			case Opcode.setpo:
                return FlagM.PF;
            case Opcode.cmovs:
            case Opcode.js:
			case Opcode.sets:
				return FlagM.SF;
            case Opcode.cmovz:
            case Opcode.jz:
			case Opcode.setz:
				return FlagM.ZF;
			case Opcode.lahf:
				return FlagM.CF|FlagM.SF|FlagM.ZF|FlagM.OF;
			case Opcode.loope:
			case Opcode.loopne:
				return FlagM.ZF;
			case Opcode.rcl:
			case Opcode.rcr:
				return FlagM.CF;
			default:
				return 0;
			}
		}

		private bool ImplicitWidth(MachineOperand op)
		{
			return op is RegisterOperand || op is X86AddressOperand || op is FpuOperand;
		}
	}
}
