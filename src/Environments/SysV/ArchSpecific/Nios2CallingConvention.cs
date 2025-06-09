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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.SysV.ArchSpecific
{
    public class Nios2CallingConvention : AbstractCallingConvention
    {
        private IProcessorArchitecture arch;
        private RegisterStorage retLo;
        private RegisterStorage retHi;
        private RegisterStorage[] iregs;

        public Nios2CallingConvention(IProcessorArchitecture arch) : base("")
        {
            this.arch = arch;
            this.retLo = arch.GetRegister("r2")!;
            this.retHi = arch.GetRegister("r3")!;
            this.iregs = new[] { "r4", "r5", "r6", "r7" }
                .Select(n => arch.GetRegister(n)!)
                .ToArray();
        }

        public override void Generate(
            ICallingConventionBuilder ccr,
            int retAddressOnStack,
            DataType? dtRet,
            DataType? dtThis,
            List<DataType> dtParams)
        {
            ccr.LowLevelDetails(4, 0);
            //Return values of types up to 8 bytes are returned in r2 and r3.For return values
            //greater than 8 bytes, the caller must allocate memory for the result and must pass
            //the address of the result memory as a hidden zero argument.
            int i = 0;
            if (dtRet is not null)
            {
                if (dtRet.Size <= 4)
                    ccr.RegReturn(retLo);
                else if (dtRet.Size <= 8)
                    ccr.SequenceReturn(retHi, retLo);
                else
                {
                    ccr.RegParam(iregs[i++]);
                }
            }

            foreach (var dtParam in dtParams)
            {
                if (dtParam.Size <= 4)
                {
                    if (i < iregs.Length)
                        ccr.RegParam(iregs[i++]);
                    else
                        ccr.StackParam(dtParam);
                } else
                {
                    //$TODO: wider args
                    ccr.StackParam(dtParam);
                }
            }
            //The first 16 bytes to a function are passed in registers r4 through r7. The arguments
            //are passed as if a structure containing the types of the arguments were constructed,
            //and the first 16 bytes of the structure are located in r4 through r7.
            //A simple example:
            //            int function(int a, int b);
            //The equivalent structure representing the arguments is:
            //    struct { int a; int b; };
            //The first 16 bytes of the struct are assigned to r4 through r7.Therefore r4 is
            //assigned the value of a and r5 the value of b.
            //The first 16 bytes to a function taking variable arguments are passed the same way as
            //a function not taking variable arguments.The called function must clean up the stack
            //as necessary to support the variable arguments.
            //Refer to Stack Frame for a Function with Variable Arguments
        }

        public override bool IsArgument(Storage stg)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsOutArgument(Storage stg)
        {
            throw new System.NotImplementedException();
        }
    }
}