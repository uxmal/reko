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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Types;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class SuperHCallingConvention : CallingConvention
    {
        private IProcessorArchitecture arch;
        private RegisterStorage[] iregs;
        private RegisterStorage[] fregs;
        private RegisterStorage[] dregs;

        public SuperHCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
            this.iregs = NamesToRegs("r4", "r5", "r6", "r7");
            this.fregs = NamesToRegs("fr4", "fr5", "fr6", "fr7");
            this.dregs = NamesToRegs("dr4", "dr6");
        }

        private RegisterStorage[] NamesToRegs(params string [] regNames)
        {
            return regNames.Select(n => arch.GetRegister(n)).ToArray();
        }


        // https://en.wikipedia.org/wiki/Calling_convention#SuperH
        // https://www.renesas.com/en-eu/doc/products/tool/001/rej10b0152_sh.pdf
        public void Generate(ICallingConventionEmitter ccr, DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 0x14);
            if (dtRet != null && !(dtRet is VoidType))
            {
                RegisterStorage reg;
                var pt = dtRet as PrimitiveType;
                if (pt != null && pt.Domain == Domain.Real)
                {
                    if (pt.Size == 4)
                    {
                        reg = arch.GetRegister("fr0");
                    }
                    else
                    {
                        reg = arch.GetRegister("dr0");
                    }
                    ccr.RegReturn(reg);
                }
                else
                {
                    if (dtRet.Size > 4)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        ccr.RegReturn(arch.GetRegister("r0"));
                    }
                }
            }
            int ir = 0;
            int fr = 0;
            if (dtThis != null)
            {
                ccr.ImplicitThisRegister(iregs[ir]);
                ++ir;
            }
            foreach (var dtParam in dtParams)
            {
                var pt = dtParam as PrimitiveType;
                if (pt != null && pt.Domain == Domain.Real)
                {
                    if (pt.Size == 4)
                    {
                        if (fr < fregs.Length)
                        {
                            ccr.RegParam(fregs[fr]);
                            ++fr;
                        }
                        else
                        {
                            ccr.StackParam(dtParam);
                        }
                    }
                    else
                    {
                        var dr = (fr + 1) >> 1;
                        if (dr < dregs.Length)
                        {
                            ccr.RegParam(dregs[dr]);
                            fr = 2 * (dr + 1);
                        }
                        else
                        {
                            ccr.StackParam(dtParam);
                        }
                    }
                }
                else if (ir >= iregs.Length || dtParam.Size > 4)
                {
                    ccr.StackParam(dtParam);
                }
                else 
                {
                    ccr.RegParam(iregs[ir]);
                    ++ir;
                }
            }
        }
    }
}
