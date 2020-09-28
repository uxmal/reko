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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Output
{

    public class TypeReferenceFormatterTests
    {
        private Identifier Arg(string name, int regNo)
        {
            return new Identifier(name, PrimitiveType.Word32, new RegisterStorage("r" + regNo, regNo, 0, PrimitiveType.Word32));
        }

        [Test]
        public void Tyreffmt_ptr_function()
        {
            var sw = new StringWriter();
            var trf = new TypeReferenceFormatter(new TextFormatter(sw));
            trf.WriteDeclaration(
                new Pointer(
                        FunctionType.Action(new [] { Arg("arg0", 0) }), 
                        32),
                "pfn");
            Assert.AreEqual("void (* pfn)(word32 arg0)", sw.ToString());
        }

        [Test]
        public void Tyreffmt_ptr_function_with_eq_class()
        {
            var sw = new StringWriter();
            var trf = new TypeReferenceFormatter(new TextFormatter(sw));
            trf.WriteDeclaration(
                new Pointer(
                    new EquivalenceClass(
                        new TypeVariable(3),
                        FunctionType.Action(new[] { Arg("arg0", 0) })),
                        32),
                "pfn");
            Assert.AreEqual("void (* pfn)(word32 arg0)", sw.ToString());
        }

        [Test]
        public void Tyreffmt_ptr_TypeReference()
        {
            var sw = new StringWriter();
            var trf = new TypeReferenceFormatter(new TextFormatter(sw));
            trf.WriteDeclaration(
                new Pointer(
                    new TypeReference("LONG", PrimitiveType.Int32), 32),
                "l0");
            Assert.AreEqual("LONG * l0", sw.ToString());
        }
    }
}
