#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Gui.TextViewing;
using System;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    public class AvaloniaTextSpanFactory : TextSpanFactory
    {
        public override ITextSpan CreateAddressSpan(string sAddress, Address address, string style)
        {
            throw new NotImplementedException();
        }

        public override ITextSpan CreateAddressTextSpan(Address addr, string formattedAddress)
        {
            throw new NotImplementedException();
        }

        public override ITextSpan CreateEmptyTextSpan()
        {
            throw new NotImplementedException();
        }

        public override ITextSpan CreateInstructionTextSpan(MachineInstruction instr, string bytes, string style)
        {
            throw new NotImplementedException();
        }

        public override ITextSpan CreateMemoryTextSpan(string text, string style)
        {
            throw new NotImplementedException();
        }

        public override ITextSpan CreateMemoryTextSpan(Address addr, string text, string style)
        {
            throw new NotImplementedException();
        }

        public override ITextSpan CreateProcedureTextSpan(ProcedureBase proc, Address addr)
        {
            throw new NotImplementedException();
        }

        public override ITextSpan CreateTextSpan(string text, string? style)
        {
            throw new NotImplementedException();
        }
    }
}
