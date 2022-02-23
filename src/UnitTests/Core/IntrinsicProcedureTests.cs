#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class IntrinsicProcedureTests
    {
        private static readonly DataType t = new TypeReference("T");
        private static readonly DataType u = new TypeReference("U");

        [Test]
        public void Intrinsic_Generic()
        {
            var args = new DataType[] { t, u };
            var sig = FunctionType.Func(
                new Identifier("", u, null!),
                new Identifier("arg1", t, null!),
                new Identifier("arg2", t, null!));

            var generic = new IntrinsicProcedure("GenericIntrinsic", args, false, false, sig);

            Assert.AreEqual("U GenericIntrinsic<T,U>(T arg1, T arg2)", generic.ToString());
        }

        [Test]
        public void Intrinsic_MakeGenericInstance()
        {
            var args = new DataType[] { t, u };
            var sig = FunctionType.Func(
                new Identifier("", u, null!),
                new Identifier("arg1", t, null!),
                new Identifier("arg2", t, null!));

            var generic = new IntrinsicProcedure("GenericIntrinsic", args, false, false, sig);
            var instance = generic.MakeInstance(PrimitiveType.Int32, PrimitiveType.Real32);

            Assert.AreEqual("real32 GenericIntrinsic<int32,real32>(int32 arg1, int32 arg2)", instance.ToString());
        }

        [Test]
        public void Intrinsic_MakeGenericInstance_AreSame()
        {
            var args = new DataType[] { t, u };
            var sig = FunctionType.Func(
                new Identifier("", u, null!),
                new Identifier("arg1", t, null!),
                new Identifier("arg2", t, null!));

            var generic = new IntrinsicProcedure("GenericIntrinsic", args, false, false, sig);
            var instance1 = generic.MakeInstance(PrimitiveType.Int32, PrimitiveType.Real32);
            var instance2 = generic.MakeInstance(PrimitiveType.Int32, PrimitiveType.Real32);

            Assert.AreSame(instance1, instance2, "Expected the instances to be identical");
        }

    }
}
