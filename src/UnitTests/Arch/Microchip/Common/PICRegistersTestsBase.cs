#region License
/* 
 * Copyright (C) 2017-2023 Christian Hostelet.
 * inspired by work of:
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

using Reko.Arch.MicrochipPIC.Common;
using Reko.Core;
using Reko.Libraries.Microchip;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Microchip.Common
{
    public class PICRegistersTestsBase
    {

        protected IPICProcessorModel picMode;
        protected PICArchitecture arch;
        protected Address baseAddr = PICProgAddress.Ptr(0x200);

        protected void SetPICModel(string picName, PICExecMode mode)
        {
            arch = new PICArchitecture(new ServiceContainer(), "pic", new Dictionary<string, object>()) { Options = new PICArchitectureOptions(picName, mode) };
            arch.CreatePICProcessorModel();
            PICMemoryDescriptor.ExecMode = mode;
        }

    }

}
