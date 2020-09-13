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
using Reko.Core.Types;
using Reko.Core.Machine;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Models an X86 instruction.
    /// </summary>
    public class X86Instruction : MachineInstruction
	{
		public Mnemonic Mnemonic { get; set; }  // Instruction mnemonic.
        public int repPrefix;                   // 0 = no prefix, 2 = repnz, 3 = repz
		public PrimitiveType dataWidth;	        // Width of the data (if it's a word).
		public PrimitiveType addrWidth;	        // width of the address mode.	// TODO: belongs in MemoryOperand

		public X86Instruction(Mnemonic mnemonic, InstrClass iclass, PrimitiveType dataWidth, PrimitiveType addrWidth, params MachineOperand [] ops)
		{
			this.Mnemonic = mnemonic;
            this.InstructionClass = iclass;
			this.dataWidth = dataWidth;
			this.addrWidth = addrWidth;
            this.Operands = ops;
		}

        public override int MnemonicAsInteger => (int) Mnemonic;
        public override string MnemonicAsString => Mnemonic.ToString();


        public string ToString(string syntax)
        {
            var options = new MachineInstructionRendererOptions(syntax: syntax);
            return base.ToString(options);
        }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            var syntax = ChooseSyntax(options);
            syntax.Render(this, renderer, options);
		}

        private X86AssemblyRenderer ChooseSyntax(MachineInstructionRendererOptions options)
        {
            if (string.IsNullOrEmpty(options.Syntax))
                return X86AssemblyRenderer.Intel;
            switch (options.Syntax![0])
            {
            case 'I': case 'i': return X86AssemblyRenderer.Intel;
            case 'N': case 'n': return X86AssemblyRenderer.Nasm;
            default: return X86AssemblyRenderer.Intel;
            }
        }

        /// <summary>
        /// Returns the condition codes that an instruction modifies.
        /// </summary>
        public static FlagM DefCc(Mnemonic mnemonic)
		{
			switch (mnemonic)
			{
			case Mnemonic.aaa:
			case Mnemonic.aas:
            case Mnemonic.adcx:
				return FlagM.CF;
			case Mnemonic.aad:
			case Mnemonic.aam:
				return FlagM.SF|FlagM.ZF;
            case Mnemonic.adox:
                return FlagM.OF;
			case Mnemonic.bt:
			case Mnemonic.bts:
			case Mnemonic.btc:
			case Mnemonic.btr:
				return FlagM.CF;
			case Mnemonic.clc:
			case Mnemonic.cmc:
			case Mnemonic.stc:
				return FlagM.CF;
			case Mnemonic.cld:
			case Mnemonic.std:
				return FlagM.DF;
			case Mnemonic.daa:
			case Mnemonic.das:
				return FlagM.CF|FlagM.SF|FlagM.ZF;
			case Mnemonic.adc:
			case Mnemonic.add:
			case Mnemonic.sbb:
			case Mnemonic.sub:
			case Mnemonic.cmp:
			case Mnemonic.cmpsb:
			case Mnemonic.cmps:
			case Mnemonic.div:
			case Mnemonic.idiv:
			case Mnemonic.imul:
			case Mnemonic.mul:
			case Mnemonic.neg:
			case Mnemonic.scas:
			case Mnemonic.scasb:
				return FlagM.CF|FlagM.SF|FlagM.ZF|FlagM.OF;
			case Mnemonic.dec:
			case Mnemonic.inc:
				return FlagM.SF|FlagM.ZF|FlagM.OF;
			case Mnemonic.rcl:
			case Mnemonic.rcr:
			case Mnemonic.rol:
			case Mnemonic.ror:
				return FlagM.OF|FlagM.CF;		// if shift count > 1, FlagM.OF is not set.
			case Mnemonic.sar:
			case Mnemonic.shl:
			case Mnemonic.shr:
				return FlagM.OF|FlagM.SF|FlagM.ZF|FlagM.CF;	// if shift count > 1, FlagM.OF is not set.
			case Mnemonic.shld:
			case Mnemonic.shrd:
				return FlagM.SF|FlagM.ZF|FlagM.CF;
			case Mnemonic.bsf:
			case Mnemonic.bsr:
				return FlagM.ZF;
			case Mnemonic.and:
			case Mnemonic.andn:
			case Mnemonic.or:
            case Mnemonic.sahf:
            case Mnemonic.test:
            case Mnemonic.xadd:
            case Mnemonic.xor:
				return FlagM.OF|FlagM.SF|FlagM.ZF|FlagM.CF;
			default:
				return 0;
			}
		}

        /// <summary>
        /// Returns the condition codes an instruction uses.
        /// </summary>
        public static FlagM UseCc(Mnemonic mnemonic)
		{
			switch (mnemonic)
			{
			case Mnemonic.adc:
			case Mnemonic.adcx:
			case Mnemonic.sbb:
				return FlagM.CF;
			case Mnemonic.daa:
			case Mnemonic.das:
				return FlagM.CF;
			case Mnemonic.ins:
			case Mnemonic.insb:
			case Mnemonic.lods:
			case Mnemonic.lodsb:
			case Mnemonic.movs:
			case Mnemonic.movsb:
			case Mnemonic.outs:
			case Mnemonic.outsb:
			case Mnemonic.scas:
			case Mnemonic.scasb:
			case Mnemonic.stos:
			case Mnemonic.stosb:
				return FlagM.DF;
            case Mnemonic.cmova:
            case Mnemonic.ja:
			case Mnemonic.seta:
				return FlagM.CF|FlagM.ZF;
            case Mnemonic.cmovbe:
            case Mnemonic.jbe:
			case Mnemonic.setbe:
				return FlagM.CF|FlagM.ZF;
            case Mnemonic.cmovc:
            case Mnemonic.jc:
			case Mnemonic.setc:
				return FlagM.CF;
            case Mnemonic.cmovg:
            case Mnemonic.jg:
			case Mnemonic.setg:
				return FlagM.SF|FlagM.OF|FlagM.ZF;
            case Mnemonic.cmovge:
            case Mnemonic.jge:
			case Mnemonic.setge:
				return FlagM.SF|FlagM.OF;
			case Mnemonic.cmovl:
            case Mnemonic.jl:
            case Mnemonic.setl:
				return FlagM.SF|FlagM.OF;
            case Mnemonic.cmovle:
            case Mnemonic.jle:
			case Mnemonic.setle:
				return FlagM.SF|FlagM.OF|FlagM.ZF;
            case Mnemonic.cmovnc:
            case Mnemonic.jnc:
            case Mnemonic.setnc:
				return FlagM.CF;
            case Mnemonic.cmovno:
            case Mnemonic.jno:
			case Mnemonic.setno:
				return FlagM.OF;
            case Mnemonic.cmovns:
            case Mnemonic.jns:
			case Mnemonic.setns:
				return FlagM.SF;
            case Mnemonic.cmovnz:
            case Mnemonic.jnz:
			case Mnemonic.setnz:
				return FlagM.ZF;
            case Mnemonic.cmovo:
            case Mnemonic.jo:
			case Mnemonic.seto:
				return FlagM.OF;
            case Mnemonic.cmovpe:
            case Mnemonic.jpe:
			case Mnemonic.setpe:
            case Mnemonic.cmovpo:
            case Mnemonic.jpo:
			case Mnemonic.setpo:
                return FlagM.PF;
            case Mnemonic.cmovs:
            case Mnemonic.js:
			case Mnemonic.sets:
				return FlagM.SF;
            case Mnemonic.cmovz:
            case Mnemonic.jz:
			case Mnemonic.setz:
				return FlagM.ZF;
			case Mnemonic.lahf:
				return FlagM.CF|FlagM.SF|FlagM.ZF|FlagM.OF;
			case Mnemonic.loope:
			case Mnemonic.loopne:
				return FlagM.ZF;
			case Mnemonic.rcl:
			case Mnemonic.rcr:
				return FlagM.CF;
			default:
				return 0;
			}
		}
	}
}
