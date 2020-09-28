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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core;
using Reko.Core.Types;
using Reko.Core.Expressions;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Fragments
{
    public class FramePointerFragment : ProcedureBuilder
    {
        private TypeFactory factory;

        public FramePointerFragment(TypeFactory factory)
        {
            this.factory = factory;
        }

        protected override void BuildBody()
        {
            Identifier frame = Declare(new StructureType("frame_t", 0), "frame");
            Identifier framep = Local32("framep");
            Assign(Frame.EnsureRegister(Architecture.StackRegister), Frame.FramePointer);
            Assign(framep, AddrOf(PrimitiveType.Ptr32, frame));
            MStore(IAdd(framep, 4), Mem(PrimitiveType.Word32, IAdd(framep, 8)));
        }
    }
}