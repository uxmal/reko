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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.OpenRISC.Aeon
{
    /// <summary>
    /// Calling convention for <see cref="AeonArchitecture"/>.
    /// </summary>
    public class AeonCallingConvention : CallingConvention
    {
        private readonly RegisterStorage[] iregs;

        public AeonCallingConvention(AeonArchitecture arch)
        {
            this.iregs = Registers.GpRegisters[3..9];
        }

        public void Generate(ICallingConventionEmitter ccr, int retAddressOnStack, DataType? dtRet, DataType? dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 24); //$REVIEW: verify this.
            if (dtRet is not null)
            {
                if (dtRet.BitSize <= 32)
                {
                    ccr.RegReturn(Registers.GpRegisters[3]);
                }
                else if (dtRet.BitSize <= 64)
                {
                    ccr.SequenceReturn(Registers.GpRegisters[3], Registers.GpRegisters[4]);
                }
                else
                    throw new NotImplementedException("Aeon return value larger than 64 bits not implemented yet.");
            }
            int i = 0;
            foreach (var dtParam in dtParams)
            {
                //$REVIEW: floats?
                if (i < iregs.Length)
                {
                    if (dtParam.BitSize <= 32) 
                    {
                        ccr.RegParam(iregs[i++]);
                    }
                    else if (dtParam.BitSize <= 64 && i < iregs.Length - 1)
                    {
                        ccr.SequenceParam(iregs[i], iregs[i + 1]);
                        i += 2;
                    }
                    else
                    {
                        throw new NotImplementedException("Aeon paramter larger than 64 bits not implemented yet.");
                    }
                }
            }
        }

        public bool IsArgument(Storage stg)
        {
            throw new NotImplementedException();
        }

        public bool IsOutArgument(Storage stg)
        {
            throw new NotImplementedException();
        }
    }
}
