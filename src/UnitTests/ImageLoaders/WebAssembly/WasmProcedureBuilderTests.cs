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
using ReactiveUI;
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
        private List<FunctionDefinition> funcindex;
        private List<GlobalEntry> globals;
        private Address addrGlobal;
        private Dictionary<int, ProcedureBase> mpFunidxToProc;
        private Dictionary<int, Address> mpGlobIdxToAddr;

        public WasmProcedureBuilderTests()
        {
            this.arch = new WasmArchitecture(new ServiceContainer(), "wasm", new());
        }

        [SetUp]
        public void Setup()
        {
            this.funcTypes = new List<FunctionType>();
            this.funcindex = new List<FunctionDefinition>();
            this.globals = new List<GlobalEntry>();
            this.mpFunidxToProc = new Dictionary<int, ProcedureBase>();
            this.mpGlobIdxToAddr = new Dictionary<int, Address>();
            this.addrGlobal = Address.Ptr32(0x2000);
        }

        private static DataType DataTypeFromValType(int valType)
        {
            switch (valType)
            {
            case 127: return PrimitiveType.Word32;
            case 126: return PrimitiveType.Word64;
            case 125: return PrimitiveType.Real32;
            case 124: return PrimitiveType.Real64;
            default:
                throw new NotImplementedException($"Unknown value type {valType:X}.");
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

        private void Given_Global(int valType, bool mutable)
        {
            int idxGlobal = globals.Count;
            var ge = new GlobalEntry
            {
                 Type= (DataTypeFromValType(valType), mutable),
            };
            globals.Add(ge);
            mpGlobIdxToAddr.Add(idxGlobal, addrGlobal);
            addrGlobal += ge.Type.Item1.Size;
        }

        private void Given_ImportFunc(string env, string name, uint typeIndex)
        {
            var def = new FunctionDefinition(-1, -1, Array.Empty<LocalVariable>(), Array.Empty<byte>())
            {
                Name = name,
                TypeIndex = typeIndex
            };
            mpFunidxToProc.Add(funcindex.Count, new ExternalProcedure(name, this.funcTypes[(int)typeIndex]));
            funcindex.Add(def);
        }

        private FunctionDefinition FnDef(uint typeIndex, LocalVariable[] localVariables, params byte[] bytes)
        {
            int idxFunc = funcindex.Count;
            mpFunidxToProc.Add(idxFunc, Procedure.Create(arch, "fn00000", Address.Ptr32(0), arch.CreateFrame()));
            var fnDef = new FunctionDefinition(0, bytes.Length, localVariables, bytes)
            {
                TypeIndex = typeIndex,
                FunctionIndex = idxFunc,
            };
            funcindex.Add(fnDef);
            return fnDef;
        }

        private void RunTest(string sExpected, FunctionDefinition fnDef)
        {
            var wasmFile = new WasmFile(new()
            {
                new TypeSection(".types", Array.Empty<byte>(), this.funcTypes)
            });
            wasmFile.FunctionIndex.AddRange(this.funcindex);
            wasmFile.GlobalIndex.AddRange(this.globals);
            var pb = new WasmProcedureBuilder(fnDef, arch, wasmFile, mpFunidxToProc, mpGlobIdxToAddr);
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
	// succ:  l00000000
l00000000:
	v2 = 1<32>
	return v2
	// succ:  fn00000_exit
fn00000_exit:
";
            RunTest(sExp, FnDef(
                0,
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
	// succ:  l00000000
l00000000:
	v2 = param0
	v3 = 0xFFFFFFD6<32>
	v4 = v2 - v3
	return v4
	// succ:  fn00000_exit
fn00000_exit:
";
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[]
                {
                    32, 0,
                    65, 86,
                    107,
                    11
                }));
        }

        [Test]
        public void Waspb_i64_eq()
        {
            Given_FuncType(new[] { 127 }, 127);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v2 = param0
	v3 = 0xFFFFFFD6<32>
	v4 = v2 == v3
	return v4
	// succ:  fn00000_exit
fn00000_exit:
";
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[]
                {
                    32, 0,
                    65, 86,
                    81,
                    11
                }));
        }

        [Test]
        public void Waspb_i64_extend_u()
        {
            Given_FuncType(new[] { 127 }, 126);
            var sExp = @"
// fn00000
// Return size: 0
word64 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v2 = param0
	v3 = 0xFFFFFFD6<32>
	v4 = v2 - v3
	v5 = CONVERT(v4, word32, uint64)
	return v5
	// succ:  fn00000_exit
fn00000_exit:
";
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[]
                {
                    32, 0,
                    65, 86,
                    107,
                    173,
                    11
                }));
        }

        [Test]
        public void Waspb_call_with_float_constant()
        {
            Given_FuncType(new[] { 127, 125 }, 127);
            Given_FuncType(new[] { 127 }, 127);
            Given_ImportFunc("env", "extfun", 1);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v2 = param0
	v3 = 1<32>
	v4 = v2 + v3
	v5 = 3.14F
	v6 = extfun(v5)
	return v6
	// succ:  fn00000_exit
fn00000_exit:
";
            RunTest(sExp, FnDef(
                1,
                Array.Empty<LocalVariable>(),
                new byte[]
                {
                    32,0,           // get.local 0
                    65,1,           // i32.const 1
                    106,            // i32.add
                    67,0xC3,0xF5,0x48,0x40, // f32.const 3.14F
                    16,0,           // call 0
                    11              // end
                }));
        }

        [Test]
        public void Waspb_get_set_global()
        {
            Given_FuncType(new[] { 127 }, 0);
            Given_Global(127, true);
            Given_Global(127, true);
            var sExp = @"
// fn00000
// Return size: 0
void fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v2 = Mem0[0x00002000<p32>:word32]
	v3 = param0
	v4 = v2 + v3
	Mem0[0x00002004<p32>:word32] = v4
	return
	// succ:  fn00000_exit
fn00000_exit:
";
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[]
                {
                    35,0,       // get.global 0
                    32,0,       // get.local 0
                    106,        // i32.add
                    36,1,       // set.global 1
                    11          // end
                }));
        }

        [Test]
        public void Waspb_dup_tee_set_global()
        {
            Given_FuncType(new[] { 125 }, 125);
            var sExp = @"
// fn00000
// Return size: 0
real32 fn00000(real32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v4 = param0
	loc1 = v4
	v5 = loc1
	v6 = v4 * v5
	loc2 = v6
	v7 = loc1
	v8 = loc2
	v9 = v7 + v8
	return v9
	// succ:  fn00000_exit
fn00000_exit:
";
            RunTest(sExp, FnDef(
                0,
                new LocalVariable[]
                {
                    new LocalVariable(PrimitiveType.Real32),
                    new LocalVariable(PrimitiveType.Real32)
                },
                new byte[]
                {
                    32,0,       // get.local0
                    34,1,        // tee.local 1
                    32,1,        // get.local 1
                    148,         // f32.mul
                    33,2,        // set.local 2
                    32,1,        // get.local 1
                    32,2,        // get.local 2
                    146,        // f32.add
                    11          // end
                }));
        }

        [Test]
        public void Waspb_if()
        {
            Given_FuncType(new[] { 127 }, 127);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v4 = param0
	v5 = 0<32>
	v6 = v4 < v5
	branch !v6 l0000000E
	// succ:  l00000007 l0000000E
l00000007:
	v7 = 0<32>
	v8 = param0
	v9 = v7 - v8
	param0 = v9
	// succ:  l0000000E
l0000000E:
	v10 = param0
	return v10
	// succ:  fn00000_exit
fn00000_exit:
";
            RunTest(sExp, FnDef(
                0,
                new LocalVariable[]
                {
                    new LocalVariable(PrimitiveType.Real32),
                    new LocalVariable(PrimitiveType.Real32)
                },
                new byte[]
                {
                    32,0,       // get.local0
                    65,0,       // i32.const 0
                    83,         // i32.lt_s
                    4,64,       // if

                    65,0,       // i32.const 0
                    32,0,       // get.local 0
                    107,        // i32.sub
                    33,0,        // set.local 0
                    11,         // end

                    32,0,       // get.local 0
                    11,         // end
                }));
        }

        [Test]
        public void Waspb_if_else()
        {
            Given_FuncType(new[] { 127 }, 127);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = 0<32>
	v5 = v3 < v4
	branch !v5 l0000000E
	// succ:  l00000007 l0000000E
l00000007:
	v6 = 0<32>
	v7 = param0
	v8 = v6 - v7
	loc1 = v8
	goto l00000013
	// succ:  l00000013
l0000000E:
	v9 = param0
	loc1 = v9
	// succ:  l00000013
l00000013:
	v10 = loc1
	return v10
	// succ:  fn00000_exit
fn00000_exit:
";
            RunTest(sExp, FnDef(
                0,
                new LocalVariable[]
                {
                    new LocalVariable(PrimitiveType.Real32)
                },
                new byte[]
                {
                    32,0,       // get.local0
                    65,0,       // i32.const 0
                    83,         // i32.lt_s
                    4,64,       // if

                    65,0,       // i32.const 0
                    32,0,       // get.local 0
                    107,        // i32.sub
                    33,1,        // set.local 1

                    5,          // else
                    32,0,        // get.local 0
                    33,1,       // set.local 1
                    11,         // end

                    32,1,       // get.local 1
                    11,         // end
                }));
        }

        [Test]
        public void Waspb_InfiniteLoop()
        {
            var sExp = @"
// fn00000
// Return size: 0
void fn00000()
fn00000_entry:
	// succ:  l00000000
l00000000:
	v2 = 0x10<32>
	puts(v2)
	goto l00000000
	// succ:  l00000000
fn00000_exit:
";
            Given_FuncType(Array.Empty<int>(), 0);
            Given_FuncType(new[] { 127 }, 0);
            Given_ImportFunc("env", "puts", 1);
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[] {
                    3, 64,      // loop,
                        65, 16, // i32.const
                        16, 0,  // call
                        12, 0,  // br 0
                    11,         // end

                    11
                }));
        }

        [Test]
        public void Waspb_WhileLoop()
        {
            var sExp = @"
// fn00000
// Return size: 0
void fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v2 = param0
	v3 = v2 == 0<32>
	branch v3 l00000000
	// succ:  l00000007 l00000000
l00000007:
	v4 = 0x10<32>
	puts(v4)
	v5 = param0
	v6 = 0xFFFFFFFF<32>
	v7 = v5 + v6
	param0 = v7
	branch v7 l00000007
	// succ:  l00000016 l00000007
l00000016:
	// succ:  l00000017
l00000017:
	return
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { 127 }, 0);
            Given_FuncType(new[] { 127 }, 0);
            Given_ImportFunc("env", "puts", 1);
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[] {
                    2,64,           // block 0
                
                        32,0,           // local.get 0
                        69,             // i32.eqz
                        13,0,           // br.if 0
                
                        3,64,           // loop
                            65,16,          // i32.const
                            16,0,           // call
                
                            32,0,           // local.get
                            65,127,         // i32.const 0xFFFFFFFF
                            106,            // i32.add
                            34,0,           // local.tee
                            13,0,           // br.if 0
                
                        11,             // end
                    11,                 
                    11,
                }));
        }

        [Test]
        public void Waspb_DoWhileLoop()
        {
            var sExp = @"
// fn00000
// Return size: 0
void fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v2 = 0x10<32>
	puts(v2)
	v3 = param0
	v4 = 0xFFFFFFFF<32>
	v5 = v3 + v4
	param0 = v5
	branch v5 l00000000
	// succ:  l0000000F l00000000
l0000000F:
	return
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { 127 }, 0);
            Given_FuncType(new[] { 127 }, 0);
            Given_ImportFunc("env", "puts", 1);
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[] {
                    3, 64,          // loop
                        65, 16,     // i32.const
                        16, 0,      // call
                        32, 0,      // local.get
                        65, 127,    // i32.const -1
                        106,        // i32.add
                        34, 0,      // local.tee
                        13, 0,      // br_if 0
                    11,
                    11,
                }));
        }
    }
}
