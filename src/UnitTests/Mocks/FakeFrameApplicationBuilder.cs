#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Core.Code;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Mocks
{
    public class FakeFrameApplicationBuilder : FrameApplicationBuilder
    {
        private IDictionary<Identifier, Expression> returnValues;

        public FakeFrameApplicationBuilder(
            IProcessorArchitecture arch,
            IStorageBinder binder,
            CallSite site,
            Expression callee) : base(arch, binder, site, callee, false)
        {
            this.returnValues = new Dictionary<Identifier, Expression>();
        }

        public override Expression Bind(Identifier id)
        {
            throw new NotImplementedException();
        }

        public override OutArgument BindOutArg(Identifier id)
        {
            throw new NotImplementedException();
        }

        public override Expression BindReturnValue(Identifier id)
        {
            if (returnValues.ContainsKey(id))
                return returnValues[id];
            return Constant.Invalid;
        }

        public void Test_AddReturnValue(Identifier id, Expression returnValue)
        {
            returnValues.Add(id, returnValue);
        }
    }
}
