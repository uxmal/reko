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
using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Core.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class SparcCallingConvention : CallingConvention
    {
        private RegisterStorage[] regs;
        private RegisterStorage iret;
        private RegisterStorage fret0;
        private RegisterStorage fret1;

        private IProcessorArchitecture arch;

        public SparcCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.regs = new[] { "o0", "o1", "o2", "o3", "o4", "o5" }
                .Select(r => arch.GetRegister(r))
                .ToArray();
            this.iret = arch.GetRegister("o0");
            this.fret0 = arch.GetRegister("f0");
            this.fret1 = arch.GetRegister("f1");
        }

        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(arch.WordWidth.Size, 0x0018);
            if (dtRet != null)
            {
                SetReturnRegister(ccr, dtRet);
            }

            int ir = 0;
            for (int iArg = 0; iArg < dtParams.Count; ++iArg)
            {
                var dtArg = dtParams[iArg];
                var prim = dtArg as PrimitiveType;
                if (dtArg.Size <= 8)
                {
                    if (ir >= regs.Length)
                    {
                        ccr.StackParam(dtArg);
                    }
                    else
                    {
                        ccr.RegParam(regs[ir]);
                        ++ir;
                    }
                }
                else
                    throw new NotImplementedException();
            }
        }

        public void SetReturnRegister(ICallingConventionEmitter ccr, DataType dtArg)
        {
            var ptArg = dtArg as PrimitiveType;
            if (ptArg != null)
            {
                if (ptArg.Domain == Domain.Real)
                {
                    var f0 = fret0;
                    if (ptArg.Size <= 4)
                    {
                        ccr.RegReturn(f0);
                        return;
                    }
                    var f1 = fret1;
                    ccr.SequenceReturn(f1, f0);
                    return;
                }
                ccr.RegReturn(iret);
                return;
            }
            else if (dtArg is Pointer)
            {
                ccr.RegReturn(iret);
                return;
            }
            else if (dtArg.Size <= this.arch.WordWidth.Size)
            {
                ccr.RegReturn(iret);
                return;
            }
            throw new NotImplementedException();
        }

        public bool IsArgument(Storage stg)
        {
            if (stg is RegisterStorage reg)
            {
                return regs.Contains(reg);
            }
            //$TODO: handle stack args.
            return false;
        }

        public bool IsOutArgument(Storage stg)
        {
            return iret == stg || fret0 == stg || fret1 == stg;
        }
    }
}
