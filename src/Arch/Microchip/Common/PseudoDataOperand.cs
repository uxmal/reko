#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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

namespace Reko.Arch.Microchip.Common
{
    public abstract class PseudoDataOperand : MachineOperand
    {
        #region Properties

        public ushort[] Values { get; }

        #endregion

        #region Constructors

        public PseudoDataOperand(byte config) : base(PrimitiveType.Byte)
        {
            Values = new ushort[1] { config };
        }

        public PseudoDataOperand(ushort idlocs) : base(PrimitiveType.UInt16)
        {
            Values = new ushort[1] { idlocs };
        }

        public PseudoDataOperand(params byte[] db) : base(PrimitiveType.Byte)
        {
            if (db is null)
                throw new ArgumentNullException(nameof(db));
            Values = new ushort[db.Length];
            Array.Copy(db, Values, Values.Length);
        }

        public PseudoDataOperand(params ushort[] dw) : base(PrimitiveType.UInt16)
        {
            if (dw is null)
                throw new ArgumentNullException(nameof(dw));
            Values = new ushort[dw.Length];
            Array.Copy(dw, Values, Values.Length);
        }

        #endregion

    }

}
