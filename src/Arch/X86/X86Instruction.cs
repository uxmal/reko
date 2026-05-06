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
using Reko.Core.Types;
using Reko.Core.Machine;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Models an X86 instruction.
    /// </summary>
    public class X86Instruction : MachineInstruction
	{
        /// <summary>
        /// Instruction mnemonic.
        /// </summary>
		public Mnemonic Mnemonic { get; set; }

        /// <summary>
        /// Width of the data (if it's a word).
        /// </summary>
        public DataType DataWidth { get; set; }

        /// <summary>
        /// Width of the address mode.
        /// </summary>
        public PrimitiveType AddressWidth { get; set; }

        /// <summary>
        /// <c>rep</c> prefix used in the instruction:
        /// 0 = no prefix, 2 = <c>repnz</c>, 3 = <c>repz</c>.
        /// </summary>
        public int RepPrefix { get; set; }

        public byte OpMask { get; set; }        // EVEX Mask register to use.
        public byte MergingMode { get; set; }   // EVEX merging mode
        public bool Broadcast { get; set; }     // EVEX broadcast flag
        public EvexRoundMode RoundMode { get; set; }
        //$PERF: is it worth it to pack the rarely used bit- and byte-sized fields into a single word?

        /// <summary>
        /// True if instruction does not modify flags -- the Intel APX "NF" bit.
        /// </summary>
        public bool NoFlags { get; set; }

		/// <summary>
		/// The preferred syntax to use when displaying this instruction.
        /// A value of <see cref="char.MinValue"/> means no perferred syntax is specified.
		/// </summary>
		public char PreferredSyntax { get; set; }

        public X86Instruction(Mnemonic mnemonic, InstrClass iclass, DataType dataWidth, PrimitiveType addrWidth, params MachineOperand [] ops)
		{
			this.Mnemonic = mnemonic;
            this.InstructionClass = iclass;
			this.DataWidth = dataWidth;
			this.AddressWidth = addrWidth;
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
            char syntax = this.PreferredSyntax;
            if (!string.IsNullOrEmpty(options.Syntax))
                syntax = options.Syntax[0];
            switch (syntax)
            {
            case 'A': case 'a': return X86AssemblyRenderer.Att;
            case 'I': case 'i': return X86AssemblyRenderer.Intel;
            case 'N': case 'n': return X86AssemblyRenderer.Nasm;
            case 'V': case 'v': return V20AssemblyRenderer.Instance;
            default: return X86AssemblyRenderer.Intel;
            }
        }

        /// <summary>
        /// Returns the condition codes that an instruction modifies.
        /// </summary>
        public static FlagGroupStorage? DefCc(Mnemonic mnemonic, RegisterBank registers)
		{
			switch (mnemonic)
			{
			case Mnemonic.aaa:
			case Mnemonic.aas:
            case Mnemonic.adcx:
				return registers.C;
			case Mnemonic.aad:
			case Mnemonic.aam:
                return registers.SZ;
            case Mnemonic.adox:
                return registers.O;
			case Mnemonic.bt:
			case Mnemonic.bts:
			case Mnemonic.btc:
			case Mnemonic.btr:
				return registers.C;
			case Mnemonic.clc:
			case Mnemonic.cmc:
			case Mnemonic.stc:
				return registers.C;
			case Mnemonic.cld:
			case Mnemonic.std:
				return registers.D;
			case Mnemonic.daa:
			case Mnemonic.das:
                return registers.SCZ;
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
			case Mnemonic.scas:
			case Mnemonic.scasb:
                return registers.SCZO;
			case Mnemonic.dec:
			case Mnemonic.inc:
			case Mnemonic.neg:
                return registers.SZO;
			case Mnemonic.rcl:
			case Mnemonic.rcr:
			case Mnemonic.rol:
			case Mnemonic.ror:
				return registers.CO;		// if shift count > 1, FlagM.OF is not set.
			case Mnemonic.sar:
			case Mnemonic.shl:
			case Mnemonic.shr:
				return registers.SCZO;	// if shift count > 1, FlagM.OF is not set.
			case Mnemonic.shld:
			case Mnemonic.shrd:
                return registers.SCZ;
			case Mnemonic.bsf:
			case Mnemonic.bsr:
				return registers.Z;
			case Mnemonic.and:
			case Mnemonic.andn:
			case Mnemonic.or:
            case Mnemonic.sahf:
            case Mnemonic.xadd:
            case Mnemonic.xor:
                return registers.SCZO;
            case Mnemonic.test:
                return registers.SCZOP;
			default:
				return null;
			}
		}

        /// <summary>
        /// Returns the condition codes an instruction uses.
        /// </summary>
        public static FlagGroupStorage? UseCc(Mnemonic mnemonic, RegisterBank registers)
		{
			switch (mnemonic)
			{
			case Mnemonic.adc:
			case Mnemonic.adcx:
			case Mnemonic.sbb:
				return registers.C;
			case Mnemonic.daa:
			case Mnemonic.das:
				return registers.C;
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
				return registers.D;
            case Mnemonic.cmova:
            case Mnemonic.ja:
			case Mnemonic.seta:
                return registers.CZ;
            case Mnemonic.cmovbe:
            case Mnemonic.jbe:
			case Mnemonic.setbe:
				return registers.CZ;
            case Mnemonic.cmovc:
            case Mnemonic.jc:
			case Mnemonic.setc:
				return registers.C;
            case Mnemonic.cmovg:
            case Mnemonic.jg:
			case Mnemonic.setg:
				return registers.SZO;
            case Mnemonic.cmovge:
            case Mnemonic.jge:
			case Mnemonic.setge:
				return registers.SO;
			case Mnemonic.cmovl:
            case Mnemonic.jl:
            case Mnemonic.setl:
				return registers.SO;
            case Mnemonic.cmovle:
            case Mnemonic.jle:
			case Mnemonic.setle:
                return registers.SZO;
            case Mnemonic.cmovnc:
            case Mnemonic.jnc:
            case Mnemonic.setnc:
				return registers.C;
            case Mnemonic.cmovno:
            case Mnemonic.jno:
			case Mnemonic.setno:
				return registers.O;
            case Mnemonic.cmovns:
            case Mnemonic.jns:
			case Mnemonic.setns:
				return registers.S;
            case Mnemonic.cmovnz:
            case Mnemonic.jnz:
			case Mnemonic.setnz:
				return registers.Z;
            case Mnemonic.cmovo:
            case Mnemonic.jo:
			case Mnemonic.seto:
				return registers.O;
            case Mnemonic.cmovpe:
            case Mnemonic.jpe:
			case Mnemonic.setpe:
            case Mnemonic.cmovpo:
            case Mnemonic.jpo:
			case Mnemonic.setpo:
                return registers.P;
            case Mnemonic.cmovs:
            case Mnemonic.js:
			case Mnemonic.sets:
				return registers.S;
            case Mnemonic.cmovz:
            case Mnemonic.jz:
			case Mnemonic.setz:
				return registers.Z;
			case Mnemonic.lahf:
				return registers.SCZO;
			case Mnemonic.loope:
			case Mnemonic.loopne:
				return registers.Z;
			case Mnemonic.rcl:
			case Mnemonic.rcr:
				return registers.C;
			default:
				return null;
			}
		}
	}
}
