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

using Reko.Core;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Arm.AArch32
{
    /// <summary>
    /// The default calling convention on Arm 32 as defined by ARM Limited
    /// </summary>
    /// <remarks>
    /// http://infocenter.arm.com/help/topic/com.arm.doc.ihi0042f/IHI0042F_aapcs.pdf
    /// </remarks>
    public class Arm32CallingConvention : CallingConvention
    {
        private RegisterStorage[] argRegs;

        public Arm32CallingConvention()
        {
            this.argRegs = new[] { Registers.r0, Registers.r1, Registers.r2, Registers.r3 };
        }

        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 0);

            if (dtRet != null)
            {
                SetReturnRegister(ccr, dtRet.BitSize);
            }

            // The first four registers r0 - r3 are used to pass argument values into a subroutine and to return a result
            // value from a function.
            int ncrn = 0;

            foreach (var dt in dtParams)
            {
                var sizeInWords = (dt.Size + 3) / 4;

                if (sizeInWords == 2 && (ncrn & 1) == 1)
                    ++ncrn;
                if (sizeInWords <= argRegs.Length - ncrn)
                {
                    if (sizeInWords == 2)
                    {
                        ccr.SequenceParam(
                            argRegs[ncrn],
                            argRegs[ncrn + 1]);
                        ncrn += 2;
                    }
                    else
                    {
                        ccr.RegParam(argRegs[ncrn]);
                        ncrn += 1;
                    }
                }
                else
                {
                    ccr.StackParam(dt);
                }
            }
        }

        public void SetReturnRegister(ICallingConventionEmitter ccr, int bitSize)
        {
            if (bitSize <= 32)
            {
                ccr.RegReturn(argRegs[0]);
            }
            else if (bitSize <= 64)
            {
                ccr.SequenceReturn(argRegs[1], argRegs[0]);
            }
            else
                throw new NotSupportedException(string.Format("Return values of {0} bits are not supported.", bitSize));
        }

        public bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return argRegs.Contains(reg);
            }
            //$TODO: handle stack args.
            return false;
        }

        public bool IsOutArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return argRegs[0] == reg || argRegs[1] == reg;
            }
            return false;
        }
    }
}
