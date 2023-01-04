#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.X86
{
    public class SaeOperand : MachineOperand
    {
        public SaeOperand(EvexRoundMode roundmode)
        {
            this.RoundMode = roundmode;
            this.Width = VoidType.Instance;
        }

        public DataType Width { get; set; }
        public EvexRoundMode RoundMode { get; }

        public void Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString(RoundMode switch
            {
                EvexRoundMode.Sae => "{sae}",
                EvexRoundMode.RnSae => "{rn-sae}",
                EvexRoundMode.RdSae => "{rd-sae}",
                EvexRoundMode.RuSae => "{ru-sae}",
                EvexRoundMode.RzSae => "{rz-sae}",
                _ => $"<{RoundMode}>"
            });
        }

        public string ToString(MachineInstructionRendererOptions options)
        {
            throw new NotImplementedException();
        }
    }

    [Flags]
    public enum EvexRoundMode : byte
    {
        None,
        Sae = 0x40,
        RnSae,
        RdSae,
        RuSae,
        RzSae,
    }
}
