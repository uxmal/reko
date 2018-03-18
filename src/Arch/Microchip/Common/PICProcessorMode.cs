#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Expressions;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;

namespace Reko.Arch.Microchip.Common
{
    using PIC16;
    using PIC18;

    public abstract class PICProcessorMode
    {
        private static Dictionary<InstructionSetID, PICProcessorMode> modes = new Dictionary<InstructionSetID, PICProcessorMode>()
            {
                { InstructionSetID.PIC16, new PIC16BasicMode() },
                { InstructionSetID.PIC16_ENHANCED, new PIC16EnhancedMode() },
                { InstructionSetID.PIC16_FULLFEATURED, new PIC16FullMode() },
                { InstructionSetID.PIC18, new PIC18LegacyMode() },
                { InstructionSetID.PIC18_EXTENDED, new PIC18EggMode() },
                { InstructionSetID.PIC18_ENHANCED, new PIC18EnhancedMode() },
            };

        protected PICProcessorMode()
        { }

        public static PICProcessorMode Create(string picName)
        {
            if (string.IsNullOrEmpty(picName))
                throw new ArgumentNullException(nameof(picName));
            var db = PICCrownking.GetDB();
            if (db is null)
                throw new InvalidOperationException("Can't get PIC database.");
            var pic = db.GetPIC(picName);
            if (pic is null)
                return null;
            if (modes.TryGetValue(pic.GetInstructionSetID, out PICProcessorMode mode))
            {
                mode.PICDescriptor = pic;
                return mode;
            }
            return null;
        }

        public PIC PICDescriptor { get; private set; }

        public string PICName => PICDescriptor?.Name;

        public abstract PICDisassemblerBase CreateDisassembler(PICArchitecture arch, EndianImageReader rdr);

        public abstract void CreateRegisters(PIC pic);

        public abstract PICRewriter CreateRewriter(PICArchitecture arch, PICDisassemblerBase dasm, PICProcessorState state, IStorageBinder binder, IRewriterHost host);

        public abstract PICProcessorState CreateProcessorState(PICArchitecture arch);

        public abstract Address MakeAddressFromConstant(Constant c);

    }

}
