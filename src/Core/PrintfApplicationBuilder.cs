#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
 
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Builds a function application from a call site and a callee, with the
    /// assumption that the last parameter, whose name is '...', is the start
    /// of the varargs part. The preceding parameter must therefore be the
    /// format string. If this string is fed with a constant integer, we walk
    /// the memory at that address to see if we can use the format string.
    /// </summary>
    public class PrintfApplicationBuilder : FrameApplicationBuilder
    {
        public PrintfApplicationBuilder(
            IProcessorArchitecture arch, Frame frame, CallSite site, Expression callee, bool ensureVariables) :
                    base(arch, frame, site, callee, ensureVariables)
        {

        }

        public override List<Expression> BindArguments(FunctionType sigCallee, ProcedureCharacteristics chr)
        {
            var actuals = new List<Expression>();
            int i;
            for (i = 0; i < sigCallee.Parameters.Length-1; ++i)
            {
                var formalArg = sigCallee.Parameters[i];
                var actualArg = formalArg.Storage.Accept(this);
                if (formalArg.Storage is OutArgumentStorage)
                {
                    actuals.Add(new OutArgument(base.arch.FramePointerType, actualArg));
                }
                else
                {
                    actuals.Add(actualArg);
                }
            }
            return actuals;
        }
    }
}
