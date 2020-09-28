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

using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Typing
{
    public class Fragments
    {
        public static void MemStore(ProcedureBuilder m)
        {
            m.Assign(m.Frame.EnsureRegister(m.Architecture.StackRegister), m.Frame.FramePointer);
            m.MStore(m.Word32(0x3120), m.Word32(0));
            m.MStore(m.Word32(0x3120), m.IAdd(m.Mem32(m.Word32(0x3120)), 1));
            m.Return();
        }
    }
}
