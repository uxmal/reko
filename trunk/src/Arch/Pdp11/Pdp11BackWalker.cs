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
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.Pdp11
{
    public class Pdp11BackWalker : BackWalker
    {
        public Pdp11BackWalker(ProgramImage img)
            : base(img)
        {
        }


        public override List<BackwalkOperation> BackWalk(Address addrFrom, IBackWalkHost host)
        {
            throw new NotImplementedException();
        }

        public override Address MakeAddress(Decompiler.Core.Types.PrimitiveType size, ImageReader rdr, ushort segBase)
        {
            throw new NotImplementedException();
        }

        public override MachineRegister IndexRegister
        {
            get { throw new NotImplementedException(); }
        }
    }
}
