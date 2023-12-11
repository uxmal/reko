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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;

namespace Reko.Arch.Avr.Avr32
{
    public class RegisterPartOperand : AbstractMachineOperand
    {
        public RegisterPartOperand(RegisterStorage reg, RegisterPart part) : base(PrimitiveType.Word16)
        {
            this.Register = reg;
            this.Part = part;
        }

        public RegisterStorage Register { get; }
        public RegisterPart Part { get; }

        protected override void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.WriteString(Register.Name);
            renderer.WriteString(Part.Format());
        }
    }

    public enum RegisterPart
    {
        All = 0,
        Top = 1,
        Upper = 2,
        Lower = 3,
        Bottom = 4,
    }

    public static class RegisterPartExtensions
    {
        public static string Format(this RegisterPart part) =>
            part switch
            {
                RegisterPart.All => "",
                RegisterPart.Top => ":t",
                RegisterPart.Upper => ":u",
                RegisterPart.Lower => ":l",
                RegisterPart.Bottom => ":b",
                _ => throw new NotSupportedException()
            };
    }

}
