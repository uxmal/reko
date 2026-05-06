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

using Reko.Core.Machine;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.X86
{
    /// <summary>
    /// Renders V20/V30 instructions using the NEC mnemonic names from the
    /// NEC hardware documentation. Extends Intel syntax with V20 aliases.
    /// </summary>
    public class V20AssemblyRenderer : IntelAssemblyRenderer
    {
        public static readonly V20AssemblyRenderer Instance = new V20AssemblyRenderer();

        /// <summary>
        /// Maps standard x86 mnemonics to the NEC V20/V30 equivalents used
        /// in the NEC hardware manual.
        /// </summary>
        private static readonly Dictionary<Mnemonic, string> mnemonicAliases = new()
        {
            // Conditional jumps renamed by NEC
            { Mnemonic.jc,  "jb"  },
            { Mnemonic.jnc, "jnb" },
            { Mnemonic.jz,  "je"  },
            { Mnemonic.jnz, "jne" },
            { Mnemonic.jpe, "jp"  },
            { Mnemonic.jpo, "jnp" },
            // Block (string) instructions renamed by NEC V20/V30
            { Mnemonic.movs,  "movbk" },
            { Mnemonic.movsb, "movbk" },
            { Mnemonic.cmps,  "cmpbk" },
            { Mnemonic.cmpsb, "cmpbk" },
            { Mnemonic.lods,  "ldm"   },
            { Mnemonic.lodsb, "ldm"   },
            { Mnemonic.stos,  "stm"   },
            { Mnemonic.stosb, "stm"   },
            { Mnemonic.ins,   "inm"   },
            { Mnemonic.insb,  "inm"   },
            { Mnemonic.outs,  "outm"  },
            { Mnemonic.outsb, "outm"  },
            { Mnemonic.scas,  "cmpm"  },
            { Mnemonic.scasb, "cmpm"  },
        };

        public override void Render(X86Instruction instr, MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            RenderPrefixV20(instr, renderer);

            if (IsStringInstructionV20(instr))
            {
                RenderStringInstruction(instr, renderer, options);
                return;
            }

            var s = new StringBuilder();
            RenderMnemonic(instr, s);
            renderer.WriteMnemonic(s.ToString());

            if (instr.Operands.Length > 0)
            {
                renderer.Tab();
                RenderOperands(instr, options, renderer);
            }
        }

        protected override void RenderMnemonic(X86Instruction instr, StringBuilder s)
        {
            if (mnemonicAliases.TryGetValue(instr.Mnemonic, out var alias))
            {
                s.Append(alias);
                // Append size suffix for word/dword/qword block instructions
                if (IsStringInstructionV20(instr))
                {
                    switch (instr.DataWidth.Size)
                    {
                    case 1: break;
                    case 2: s.Append('w'); break;
                    case 4: s.Append('d'); break;
                    case 8: s.Append('q'); break;
                    }
                }
            }
            else
                base.RenderMnemonic(instr, s);
        }

        private static void RenderPrefixV20(X86Instruction instr, MachineInstructionRenderer renderer)
        {
            if (instr.RepPrefix == 3)
            {
                renderer.WriteMnemonic(IsCompareStringInstruction(instr) ? "repc" : "rep");
                renderer.WriteChar(' ');
            }
            else if (instr.RepPrefix == 2)
            {
                renderer.WriteMnemonic(IsCompareStringInstruction(instr) ? "repnc" : "repne");
                renderer.WriteChar(' ');
            }
        }

        private static bool IsCompareStringInstruction(X86Instruction instr)
        {
            return instr.Mnemonic == Mnemonic.cmps ||
                   instr.Mnemonic == Mnemonic.cmpsb ||
                   instr.Mnemonic == Mnemonic.scas ||
                   instr.Mnemonic == Mnemonic.scasb;
        }

        private static bool IsStringInstructionV20(X86Instruction instr)
        {
            switch (instr.Mnemonic)
            {
            case Mnemonic.ins:
            case Mnemonic.insb:
            case Mnemonic.outs:
            case Mnemonic.outsb:
            case Mnemonic.movs:
            case Mnemonic.movsb:
            case Mnemonic.cmps:
            case Mnemonic.cmpsb:
            case Mnemonic.stos:
            case Mnemonic.stosb:
            case Mnemonic.lods:
            case Mnemonic.lodsb:
            case Mnemonic.scas:
            case Mnemonic.scasb:
                return true;
            default:
                return false;
            }
        }
    }
}
