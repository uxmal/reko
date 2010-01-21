/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Pdp11
{
    class Pdp11ProcessorState : ProcessorState
    {
        public ProcessorState Clone()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Set(MachineRegister r, Constant v)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SetInstructionPointer(Address addr)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Constant Get(MachineRegister r)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
