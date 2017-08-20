#region License
/* 
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

using System;
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System.Collections.Generic;
using Reko.Core.Expressions;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class Arm32CallingConvention : CallingConvention
    {
        private static string[] argRegs = { "r0", "r1", "r2", "r3" };
        private IProcessorArchitecture arch;

        public Arm32CallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
        }

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            int ncrn = 0;
            int nsaa = 0;
            // mem arg forb ret val

            Storage ret = null;
            if (dtRet != null)
            {
                ret = GetReturnRegister(dtRet.BitSize);
            }

            var parameters = new List<Storage>();
            foreach (var dt in dtParams)
            {
                var sizeInWords = (dt.Size + 3) / 4;

                if (sizeInWords == 2 && (ncrn & 1) == 1)
                    ++ncrn;
                Storage arg;
                if (sizeInWords <= 4 - ncrn)
                {
                    if (sizeInWords == 2)
                    {
                        arg = new SequenceStorage(
                            arch.GetRegister(argRegs[ncrn]),
                            arch.GetRegister(argRegs[ncrn + 1]));
                        ncrn += 2;
                    }
                    else
                    {
                        arg = arch.GetRegister(argRegs[ncrn]);
                        ncrn += 1;
                    }
                }
                else
                {
                    arg = new StackArgumentStorage((nsaa + 4) * 4, dt);
                    nsaa += AlignedStackArgumentSize(dt);
                }
                parameters.Add(arg);
            }
            return new CallingConventionResult
            {
                Return = ret,
                ImplicitThis = null,    //$TODO!
                Parameters = parameters,
                StackDelta = 0,
                FpuStackDelta = 0
            };
        }

        private int AlignedStackArgumentSize(DataType dt)
        {
            return ((dt.Size + 3) / 4) * 4; 
        }

        public Storage GetReturnRegister(int bitSize)
        {
            if (bitSize <= 32)
            {
                return arch.GetRegister("r0");
            }
            else if (bitSize <= 64)
            {
                return new SequenceStorage(
                    arch.GetRegister("r1"),
                    arch.GetRegister("r0"));
            }
            else
                throw new NotSupportedException(string.Format("Return values of {0} bits are not supported.", bitSize));
        }
    }
}