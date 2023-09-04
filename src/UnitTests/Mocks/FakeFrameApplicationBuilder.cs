#region License
/* 
 * Copyright (C) 1999-2023 Pavel Tomin.
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
using Reko.Core.Analysis;
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
        private IDictionary<Storage, Expression> returnValues;

        public FakeFrameApplicationBuilder(
            IProcessorArchitecture arch,
            IStorageBinder binder,
            CallSite site) : base(arch, binder, site)
        {
            this.returnValues = new Dictionary<Storage, Expression>();
        }

        public override Expression BindInArg(Storage stg)
        {
            throw new NotImplementedException();
        }

        public override OutArgument BindOutArg(Storage stg)
        {
            throw new NotImplementedException();
        }

        public override Expression BindReturnValue(Storage stg)
        {
            if (returnValues.ContainsKey(stg))
                return returnValues[stg];
            return InvalidConstant.Create(stg.DataType);
        }

        public void Test_AddReturnValue(Storage stg, Expression returnValue)
        {
            returnValues.Add(stg, returnValue);
        }
    }
}
