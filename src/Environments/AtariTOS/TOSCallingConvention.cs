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

using Reko.Core;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Arch.M68k;
using System;
using Reko.Core.Expressions;
using System.Collections.Generic;

namespace Reko.Environments.AtariTOS
{
    // The TOSCall calling convention takes all arguments on stack, ignoring 
    // the first one since it is the system call selector.
    public class TOSCallingConvention : CallingConvention
    {
        private ArgumentDeserializer argDeser;
        private IProcessorArchitecture arch;

        public TOSCallingConvention(IProcessorArchitecture arch)
        {
            this.arch = arch;
        }

        public override CallingConventionResult Generate(DataType dtRet, DataType dtThis, List<DataType> dtParams)
        {
            int stackOffset = 4 + 4;   // Skip the system call selector + return address.
            Storage ret = null;
            if (dtRet != null)
            {
                ret = GetReturnRegister(dtRet);
            }

            var args = new List<Storage>();
            if (dtThis != null)
            {
                throw new NotImplementedException("C++ implicit `this` arguments are not implemented for Atari TOS.");
            }
            for (int iArg = 0; iArg < dtParams.Count; ++iArg)
            {
                var dtParam = dtParams[iArg];
                Storage arg = new StackArgumentStorage(stackOffset, dtParam);
                stackOffset += Align(dtParam.Size, 4);
                args.Add(arg);
            }

            return new CallingConventionResult
            {
                Return = ret,
                ImplicitThis = null, //$TODO
                Parameters = args,

                // AFAIK the calling convention on Atari TOS is caller-cleanup, 
                // so the only thing we clean up is the return value on the stack.
                StackDelta = 4,
                FpuStackDelta = 0
            };
        }

        public Storage GetReturnRegister(DataType dt)
        {
            return Registers.d0;
        }
    }
}