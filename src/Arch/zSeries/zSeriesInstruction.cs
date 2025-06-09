#region License
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
#endregion

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Text;

namespace Reko.Arch.zSeries
{
#pragma warning disable IDE1006 // Naming Styles
    public class zSeriesInstruction : MachineInstruction
    {
        public Mnemonic Mnemonic { get; set; }
        public override int MnemonicAsInteger => (int) Mnemonic;
        public override string MnemonicAsString => Mnemonic.ToString();

        /// <summary>
        /// Determines the vector element size, if the instruction is a vector instruction.
        /// </summary>
        public PrimitiveType? ElementSize { get; set; }

        /// <summary>
        /// If true, only the zero-indexed element is used. 
        /// </summary>
        public bool SingleElement { get; set; }

        /// <summary>
        /// Some instructions optionally set the condition code.
        /// </summary>
        public bool SetConditionCode { get; set; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            WriteMnemonic(renderer);
            if (Operands.Length == 0)
                return;
            renderer.Tab();
            var sep = "";
            foreach (var op in Operands)
            {
                renderer.WriteString(sep);
                sep = ",";
                op.Render(renderer, options);
            }
        }

        private void WriteMnemonic(MachineInstructionRenderer renderer)
        {
            var sb = new StringBuilder();
            if (this.SingleElement)
                sb.Append('w');
            sb.Append(this.Mnemonic.ToString());
            if (this.ElementSize is not null)
            {
                if (this.ElementSize.Domain == Domain.Real)
                {
                    sb.Append("db");
                }
                else
                {
                    char suffix = this.ElementSize.Size switch
                    {
                        1 => 'b',
                        2 => 'h',
                        4 => 'f',
                        8 => 'g',
                        _ => '?'
                    };
                    sb.Append(suffix);
                }
            }
            if (this.SetConditionCode)
            {
                sb.Append('s');
            }
            renderer.WriteMnemonic(sb.ToString());
        }
    }
}