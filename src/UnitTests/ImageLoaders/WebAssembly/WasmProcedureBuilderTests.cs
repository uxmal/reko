#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.ImageLoaders.WebAssembly;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.ImageLoaders.WebAssembly
{
    [TestFixture]
    public class WasmProcedureBuilderTests
    {
        private readonly WasmArchitecture arch;
        private List<FunctionType> funcTypes;
        private Dictionary<int, ProcedureBase> mpFunidxToProc;

        public WasmProcedureBuilderTests()
        {
            this.arch = new WasmArchitecture(new ServiceContainer(), "wasm", new());
        }

        [SetUp]
        public void Setup()
        {
            this.funcTypes = new List<FunctionType>();
            this.mpFunidxToProc = new Dictionary<int, ProcedureBase>();
        }

        private static DataType DataTypeFromValType(int valType)
        {
            switch (valType)
            {
            case 127: return PrimitiveType.Word32;
            default:
                throw new NotImplementedException();
            }
        }

        private void Given_FuncType(int[] inputTypes, int outputType)
        {
            static Identifier Arg(int valType, int iParam)
            {
                var dt = DataTypeFromValType(valType);
                return new Identifier($"param{iParam}", dt, null!);
            }

            var parameters = inputTypes.Select(Arg).ToArray();
            var retvalue = outputType != 0
                ? new Identifier("", DataTypeFromValType(outputType), null!)
                : null;
            var ft = new FunctionType(retvalue, parameters);
            funcTypes.Add(ft);
        }

        private FunctionDefinition FnDef(LocalVariable[] localVariables, params byte[] bytes)
        {
            mpFunidxToProc.Add(0, Procedure.Create(arch, "fn00000", Address.Ptr32(0), arch.CreateFrame()));
            return new FunctionDefinition(0, bytes.Length, localVariables, bytes);
        }

        private void RunTest(string sExpected, FunctionDefinition fnDef)
        {
            var wasmFile = new WasmFile(new()
            {
                new TypeSection(".types", Array.Empty<byte>(), this.funcTypes)
            });
            var pb = new WasmProcedureBuilder(fnDef, arch, wasmFile, mpFunidxToProc);
            var proc = pb.GenerateProcedure();
            var sw = new StringWriter();
            sw.WriteLine();
            proc.Write(false, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        [Test]
        public void Waspb_SimpleReturn()
        {
            Given_FuncType(new[] { 127 }, 127);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
l00000000:
	v2 = 1<32>
	return v2
fn00000_exit:
";
            RunTest(sExp, FnDef(
                Array.Empty<LocalVariable>(),
                new byte[]
                {
                    65,1,
                    11
                }));
        }

        [Test]
        public void Waspb_i32_sub()
        {
            Given_FuncType(new[] { 127 }, 127);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
l00000000:
	v2 = param0
	v3 = 0xFFFFFFD6<32>
	v4 = v2 - v3
	return v4
fn00000_exit:
";
            RunTest(sExp, FnDef(
                Array.Empty<LocalVariable>(),
                new byte[]
                {
                    32, 0,
                    65, 86,
                    107,
                    11
                }));
        }
    }
}
