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

using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class CallingConventionMatcherTests
    {
        private Mock<IPlatform> platform;
        private Mock<IProcessorArchitecture> arch;
        private readonly RegisterStorage r1 = new("r1", 1, 0, PrimitiveType.Word32);
        private readonly RegisterStorage r2 = new("r2", 2, 0, PrimitiveType.Word32);
        private readonly RegisterStorage r3 = new("r3", 3, 0, PrimitiveType.Word32);
        private readonly RegisterStorage r4 = new("r4", 4, 0, PrimitiveType.Word32);
        private readonly RegisterStorage r5 = new("r5", 5, 0, PrimitiveType.Word32);
        private readonly RegisterStorage r8 = new("r8", 8, 0, PrimitiveType.Word32);
        private readonly ICallingConvention cdecl = new CdeclConvention();
        private readonly ICallingConvention pascal = new PascalConvention();

        private class PascalConvention : ICallingConvention
        {
            public PascalConvention()
            {
            }

            public string Name => "pascal";

            public IComparer<Identifier> InArgumentComparer { get; set; }

            public IComparer<Identifier> OutArgumentComparer { get; set; }

            public void Generate(ICallingConventionBuilder ccr, int retAddressOnStack, DataType dtRet, DataType dtThis, List<DataType> dtParams)
            {
                throw new NotImplementedException();
            }

            public bool IsArgument(Storage stg)
            {
                if (stg is RegisterStorage reg)
                {
                    return 2 <= reg.Number && reg.Number <= 5;
                }
                return stg is StackStorage;
            }

            public bool IsOutArgument(Storage stg)
            {
                if (stg is RegisterStorage reg)
                {
                    return 1 <= reg.Number && reg.Number <= 2;
                }
                return false;
            }
        }

        private class CdeclConvention : ICallingConvention
        {
            public string Name => "cdecl";

            public IComparer<Identifier> InArgumentComparer => throw new NotImplementedException();

            public IComparer<Identifier> OutArgumentComparer => throw new NotImplementedException();

            public void Generate(ICallingConventionBuilder ccr, int retAddressOnStack, DataType dtRet, DataType dtThis, List<DataType> dtParams)
            {
                throw new NotImplementedException();
            }

            public bool IsArgument(Storage stg)
            {
                if (stg is RegisterStorage reg)
                {
                    return 1 <= reg.Number && reg.Number <= 4;
                }
                return stg is StackStorage;
            }

            public bool IsOutArgument(Storage stg)
            {
                if (stg is RegisterStorage reg)
                {
                    return 1 <= reg.Number && reg.Number <= 2;
                }
                return false;
            }
        }

        [SetUp]
        public void Setup()
        {
            this.platform = new Mock<IPlatform>(MockBehavior.Strict);
            platform.Setup(p => p.CallingConventions).Returns(new Dictionary<string, IReadOnlyCollection<string>>
            {
                { "test", new List<string> { "cdecl", "pascal"} }
            });
            platform.Setup(p => p.DefaultCallingConvention).Returns("cdecl");
            platform.Setup(p => p.GetCallingConvention("cdecl")).Returns(cdecl);
            platform.Setup(p => p.GetCallingConvention("pascal")).Returns(pascal);
            this.arch = new Mock<IProcessorArchitecture>(MockBehavior.Strict);
            arch.Setup(a => a.Name).Returns("test");
        }

        private FunctionType Func(Storage arg1, Storage ret) {
            return FunctionType.Create(
                new Identifier("", ret.DataType, ret),
                new Identifier(arg1.Name, arg1.DataType, arg1));
        }

        private FunctionType Func(Storage arg1, Storage arg2, Storage ret)
        {
            return FunctionType.Create(
                new Identifier("", ret.DataType, ret),
                new Identifier(arg1.Name, arg1.DataType, arg1),
                new Identifier(arg2.Name, arg2.DataType, arg2));
        }

        private FunctionType Func(Storage arg1, Storage arg2, Storage arg3, Storage ret)
        {
            return FunctionType.Create(
                new Identifier("", ret.DataType, ret),
                new Identifier(arg1.Name, arg1.DataType, arg1),
                new Identifier(arg2.Name, arg2.DataType, arg2),
                new Identifier(arg3.Name, arg3.DataType, arg3));
        }

        private void Given_PlatformNoDefaultConvention()
        {
            platform.Setup(p => p.DetermineCallingConvention(
                It.IsAny<FunctionType>(),
                It.IsAny<IProcessorArchitecture>()))
                .Returns((ICallingConvention)null);
        }

        [Test]
        public void Ccm_NoCallingConvention()
        {
            Given_PlatformNoDefaultConvention();

            // r8 is not part of any calling convention.
            var ccm = new CallingConventionMatcher(platform.Object);
            var func = Func(r1, r2, r8, r1);

            var cc = ccm.DetermineCallingConvention(func, arch.Object);

            Assert.IsNull(cc);
            platform.Verify();
        }

        [Test]
        public void Ccm_CdeclCallingConvention()
        {
            Given_PlatformNoDefaultConvention();

            // r1 only exists in the 'cdecl' convention.
            var ccm = new CallingConventionMatcher(platform.Object);
            var func = Func(r1, r2, r1);

            var cc = ccm.DetermineCallingConvention(func, arch.Object);

            Assert.AreEqual("cdecl", cc.Name);
            platform.Verify();
        }

        [Test]
        public void PascalCallingConvention()
        {
            Given_PlatformNoDefaultConvention();

            // r5 only exists in the 'cdecl' convention.
            var ccm = new CallingConventionMatcher(platform.Object);
            var func = Func(r5, r3, r2, r1);

            var cc = ccm.DetermineCallingConvention(func, arch.Object);

            Assert.AreEqual("pascal", cc.Name);
            platform.Verify();
        }
    }
}
