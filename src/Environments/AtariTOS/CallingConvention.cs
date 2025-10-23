#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Arch.Motorola.M68k.Machine;
using Reko.Core;
using Reko.Core.Expressions;

#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Collections.Generic;

namespace Reko.Environments.AtariTOS
{
    /// <summary>
    /// The default AtariST calling convention:
    /// all parameters on stack, return value in D0.
    /// </summary>
    public class CallingConvention : AbstractCallingConvention
    {
        public CallingConvention(IProcessorArchitecture architecture)
            : base("")
        {
        }

        public override void Generate(
            ICallingConventionBuilder ccr, 
            int retAddressOnStack, 
            DataType? dtRet,
            DataType? dtThis, 
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 4);
            foreach (var dtParam in dtParams)
            {
                ccr.StackParam(dtParam);
            }
            if (dtRet is not null)
            {
                ccr.RegParam(Registers.d0);
            }
        }

        public override bool IsOutArgument(Storage stg)
        {
            return stg is RegisterStorage reg &&
                reg == Registers.d0;
        }

        public override bool IsArgument(Storage stg)
        {
            return stg is StackStorage;
        }
    }
}