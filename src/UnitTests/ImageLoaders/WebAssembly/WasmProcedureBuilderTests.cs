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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.ImageLoaders.WebAssembly;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.ImageLoaders.WebAssembly
{
    using static Reko.ImageLoaders.WebAssembly.Mnemonic;

    [TestFixture]
    public class WasmProcedureBuilderTests
    {
        private Program program;
        private readonly WasmArchitecture arch;
        private List<FunctionType> funcTypes;
        private List<FunctionDefinition> funcindex;
        private List<GlobalEntry> globals;
        private Address addrGlobal;
        private Dictionary<int, ProcedureBase> mpFunidxToProc;
        private Dictionary<int, Address> mpGlobIdxToAddr;

        private const int i32 = 127;
        private const int i64 = 126;
        private const int f32 = 125;
        private const int f64 = 124;

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
            this.program = new Program();
        }

        private static DataType DataTypeFromValType(int valType)
        {
            switch (valType)
            {
            case i32: return PrimitiveType.Word32;
            case i64: return PrimitiveType.Word64;
            case 125: return PrimitiveType.Real32;
            case 124: return PrimitiveType.Real64;
            default:
                throw new NotImplementedException($"Unknown value type {valType:X}.");
            }
        }

        private static byte[] ByteCode(params object[] objects)
        {
            var result = new List<byte>();
            foreach (var o in objects)
            {
                switch (o)
                {
                case Mnemonic mn:
                    //$TODO long mnemonics.
                    result.Add((byte) mn);
                    break;
                case int i:
                    result.Add((byte) i);
                    break;
                case double d:
                    var dBits = BitConverter.DoubleToUInt64Bits(d);
                    // add the bits little-endian
                    for (int i = 0; i < 8; ++i, dBits >>= 8)
                    {
                        result.Add((byte) dBits);
                    }
                    break;
                default:
                    throw new NotImplementedException();
                }
            }
            return result.ToArray();
        }

        private void Given_FuncType(int[] inputTypes, int outputType)
        {
            static Identifier Arg(int valType, int iParam)
            {
                var dt = DataTypeFromValType(valType);
                return new Identifier($"param{iParam}", dt, null!);
            }

            var parameters = inputTypes.Select(Arg).ToArray();
            Identifier[] retvalues = outputType != 0
                ? [ new Identifier("", DataTypeFromValType(outputType), null!) ]
                : [];
            var ft = new FunctionType(parameters, retvalues);
            funcTypes.Add(ft);
        }

        private void Given_Global(int valType, bool mutable)
        {
            int idxGlobal = globals.Count;
            var ge = new GlobalEntry
            {
                Type = (DataTypeFromValType(valType), mutable),
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
            mpFunidxToProc.Add(funcindex.Count, new ExternalProcedure(name, this.funcTypes[(int) typeIndex]));
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
            Given_FuncType(new[] { i32 }, i32);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = 1<32>
	return v3
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
            Given_FuncType(new[] { i32 }, i32);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = 0xFFFFFFD6<32>
	v3 = v3 - v4
	return v3
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
            Given_FuncType(new[] { i32 }, i32);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = 0xFFFFFFD6<32>
	v5 = v3 == v4
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
                    81,
                    11
                }));
        }

        [Test]
        public void Waspb_i64_extend_u()
        {
            Given_FuncType(new[] { i32 }, i64);
            var sExp = @"
// fn00000
// Return size: 0
word64 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = 0xFFFFFFD6<32>
	v3 = v3 - v4
	v5 = CONVERT(v3, word32, uint64)
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
            Given_FuncType(new[] { i32, 125 }, i32);
            Given_FuncType(new[] { i32 }, i32);
            Given_ImportFunc("env", "extfun", 0);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = 1<32>
	v3 = v3 + v4
	v5 = 3.14F
	v3 = extfun(v3, v5)
	return v3
	// succ:  fn00000_exit
fn00000_exit:
";
            RunTest(sExp, FnDef(
                1,
                Array.Empty<LocalVariable>(),
                ByteCode(
                    get_local,0,
                    i32_const,1,
                    i32_add,
                    f32_const,0xC3,0xF5,0x48,0x40, // 3.14F
                    call,0,
                    end
                )));
        }

        [Test]
        public void Waspb_get_set_global()
        {
            Given_FuncType(new[] { i32 }, 0);
            Given_Global(i32, true);
            Given_Global(i32, true);
            var sExp = @"
// fn00000
// Return size: 0
void fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = Mem0[0x00002000<p32>:word32]
	v4 = param0
	v3 = v3 + v4
	Mem0[0x00002004<p32>:word32] = v3
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
	v5 = param0
	loc1 = v5
	v6 = loc1
	v5 = v5 * v6
	loc2 = v5
	v5 = loc1
	v6 = loc2
	v5 = v5 + v6
	return v5
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
            Given_FuncType(new[] { i32 }, i32);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v5 = param0
	v6 = 0<32>
	v7 = v5 < v6
	branch !v7 l0000000F
	// succ:  l00000007 l0000000F
l00000007:
	v5 = 0<32>
	v6 = param0
	v5 = v5 - v6
	param0 = v5
	// succ:  l0000000F
l0000000F:
	v5 = param0
	return v5
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
            Given_FuncType(new[] { i32 }, i32);
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
	branch !v6 l0000000F
	// succ:  l00000007 l0000000F
l00000007:
	v4 = 0<32>
	v5 = param0
	v4 = v4 - v5
	loc1 = v4
	goto l00000013
	// succ:  l00000013
l0000000F:
	v4 = param0
	loc1 = v4
	// succ:  l00000013
l00000013:
	v7 = loc1
	return v7
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
	// succ:  l00000002
l00000002:
	v3 = 0x10<32>
	puts(v3)
	goto l00000002
	// succ:  l00000002
fn00000_exit:
";
            Given_FuncType(Array.Empty<int>(), 0);
            Given_FuncType(new[] { i32 }, 0);
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
	v3 = param0
	v4 = v3 == 0<32>
	branch v4 l00000017
	// succ:  l00000007 l00000017
l00000007:
	// succ:  l00000009
l00000009:
	v3 = 0x10<32>
	puts(v3)
	v3 = param0
	v5 = 0xFFFFFFFF<32>
	v3 = v3 + v5
	param0 = v3
	branch v3 l00000009
	// succ:  l00000016 l00000009
l00000016:
	// succ:  l00000017
l00000017:
	return
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { i32 }, 0);
            Given_FuncType(new[] { i32 }, 0);
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
                            65,i32,         // i32.const 0xFFFFFFFF
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
	// succ:  l00000002
l00000002:
	v3 = 0x10<32>
	puts(v3)
	v3 = param0
	v4 = 0xFFFFFFFF<32>
	v3 = v3 + v4
	param0 = v3
	branch v3 l00000002
	// succ:  l0000000F l00000002
l0000000F:
	return
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { i32 }, 0);
            Given_FuncType(new[] { i32 }, 0);
            Given_ImportFunc("env", "puts", 1);
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[] {
                    3, 64,          // loop
                        65, 16,     // i32.const
                        16, 0,      // call
                        32, 0,      // local.get
                        65, i32,    // i32.const -1
                        106,        // i32.add
                        34, 0,      // local.tee
                        13, 0,      // br_if 0
                    11,
                    11,
                }));
        }

        [Test]
        public void Waspb_Store32()
        {
            var sExp = @"
// fn00000
// Return size: 0
void fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = param0
	v4 = Mem0[v4 + 8<u32>:word32]
	Mem0[v3 + 4<u32>:word32] = v4
	return
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { i32 }, 0);
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[] {
                32,0,   // local.get
                32,0,
                40,2,8, // i32.load
                54,2,4, // i32.store
                11
            }));
        }

        [Test]
        public void Waspb_Store8()
        {
            var sExp = @"
// fn00000
// Return size: 0
void fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = param0
	v5 = Mem0[v4 + 8<u32>:byte]
	v4 = CONVERT(v5, byte, word32)
	v6 = SLICE(v4, byte, 0)
	Mem0[v3 + 4<u32>:byte] = v6
	return
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { i32 }, 0);
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[] {
                32,0,   // local.get
                32,0,
                45,2,8, // i32.load8_u
                58,2,4, // i32.store8
                11
            }));
        }

        [Test]
        public void Waspb_Select()
        {
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = 1<32>
	v4 = 0xFFFFFFFF<32>
	v5 = param0
	v6 = v5 == 0<32>
	v3 = v6 ? v3 : v4
	return v3
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { i32 }, i32);
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[] {
                    65,1,   // i32.const
                    65,i32, // i32.const
                    32,0,   // local.get
                    69,     // i32.eqz
                    27,     // select
                    11
                }));
        }

        [Test]
        [Ignore("for now")]
        public void Waspb_call_indirect_with_float_constant()
        {
            Given_FuncType(new[] { i32, 125 }, i32);
            Given_FuncType(new[] { i32 }, i32);
            //Given_Table(new[]
            //{
            //    1
            //});
            Given_ImportFunc("env", "extfun", 1);
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = 1<32>
	v5 = v3 + v4
	v6 = 3.14F
	v7 = extfun(v6)
	return v7
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
        /*
         
         (module
 (type $FUNCSIG$ii (func (param i32) (result i32)))
 (type $FUNCSIG$v (func))
 (import "env" "foo" (func $foo (param i32) (result i32)))
 (import "env" "bar" (func $bar (param i32) (result i32)))
 (table 3 3 anyfunc)
 (elem (i32.const 0) $__wasm_nullptr $__importThunk_foo $__importThunk_bar)
 (memory $0 1)
 (data (i32.const 12) "\01\00\00\00\02\00\00\00")
 (export "memory" (memory $0))
 (export "choice" (func $choice))
 (func $choice (; 2 ;) (param $0 i32) (result i32)
  (call_indirect (type $FUNCSIG$ii)
   (get_local $0)
   (i32.load
    (i32.add
     (i32.shl
      (get_local $0)
      (i32.const 2)
     )
     (i32.const 12)
    )
   )
  )
 )
 (func $__wasm_nullptr (; 3 ;) (type $FUNCSIG$v)
  (unreachable)
 )
 (func $__importThunk_foo (; 4 ;) (type $FUNCSIG$ii) (param $0 i32) (result i32)
  (call $foo
   (get_local $0)
  )
 )
 (func $__importThunk_bar (; 5 ;) (type $FUNCSIG$ii) (param $0 i32) (result i32)
  (call $bar
   (get_local $0)
  )
 )
)


0,97,115,109,1,0,0,0,
1,9,
    2,
        96,1,i32,1,i32,
        96,0,0,
2,21,
    2,
        3,101,110,118,3,102,111,111,0,0,
        3,101,110,118,3,98,97,114,0,0,
3,5,
    4,0,1,0,0,
4,5,        // table
    1,112,1,3,3,
5,3,
    1,0,1,
7,19,
    2,
        6,109,101,109,111,114,121,2,0,
        6,99,104,111,105,99,101,0,2,
9,9,
    1,
        0,65,0,11,3,3,4,5,
10,38,
    4,
        18,
            0,
            32,0,
            32,0,
            65,2,
            116,
            65,12,
            106,
            40,2,0,
            17,0,0,
            11,
    3,
        0,
            0,
            11,
    6,
        0,
        32,0,
        16,0,
        11,
    6,
        0,
        32,0,
        16,1,
        11,
11,14,
    1,0,65,12,11,8,1,0,0,0,2,0,0,0]);
        */

        [Test]
        public void Waspb_Switch()
        {
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0, word32 param1, word32 param2)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = 4<32>
	v5 = v3 >u v4
	branch v5 l0000001F
	// succ:  l0000000F l0000001F
l0000000F:
	v3 = param0
	switch (v3) { l0000001B l0000001B l00000023 l0000002A l00000031 }
	// succ:  l0000001B l0000001B l00000023 l0000002A l00000031
l0000001B:
	v3 = param0
	return v3
	// succ:  fn00000_exit
l0000001F:
	v3 = 0xFFFFFFFF<32>
	return v3
	// succ:  fn00000_exit
l00000023:
	v3 = param2
	v4 = param1
	v3 = v3 + v4
	return v3
	// succ:  fn00000_exit
l0000002A:
	v3 = param1
	v4 = param2
	v3 = v3 - v4
	return v3
	// succ:  fn00000_exit
l00000031:
	v3 = param1
	v4 = param2
	v3 = v3 - v4
	v4 = param1
	v3 = v3 * v4
	return v3
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { i32, i32, i32 }, i32);
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                new byte[]
                {
2,64,                       // block
    2,64,                   // block
        2,64,               // block
            2,64,           // block
                32,0,       // local.get
                65,4,       // i32.const
                75,         // i32.gt_u
                13,0,       // br_if 0
                2,64,       // block
                    32,0,   // local.get
                    14,5, 0,0,2,3,4, 1, // br_table
                11,         // end
                32,0,
                15,     // return param0;
            11,
            65,i32,     // return -1
            15,
        11,
        32,2,           // return a + b
        32,1,
        106,
        15,
    11,
    32,1,
    32,2,
    107,                // return a - b
    15,
11,
32,1,
32,2,
107,            // (A - B) * A
32,1,
108,
11 }));

        }

        [Test]
        public void Waspb_IfWithOutput()
        {
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = 0<32>
	v5 = v3 >= v4
	branch !v5 l0000000A
	// succ:  l00000007 l0000000A
l00000007:
	v3 = param0
	goto l0000000F
	// succ:  l0000000F
l0000000A:
	v3 = 0<32>
	v4 = param0
	v3 = v3 - v4
	// succ:  l0000000F
l0000000F:
	return v3
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { i32 }, i32);
            RunTest(sExp, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                ByteCode(
                    get_local, 0,
                    i32_const, 0,
                    i32_ge_s,
                    @if, i32,
                        get_local, 0,
                    @else,
                        i32_const, 0,
                        get_local, 0,
                        i32_sub,
                    end,
                    end
                )));
        }

        [Test]
        public void Waspb_loop_regression()
        {
            var sExp = @"
// fn00000
// Return size: 0
word32 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v9 = 0.0
	loc3 = v9
	v9 = 0.0
	loc3 = v9
	v9 = 0.0
	loc4 = v9
	v9 = 0.0
	loc5 = v9
	v10 = 0<32>
	loc2 = v10
	// succ:  l00000032
l00000032:
	v11 = loc2
	v12 = 1<32>
	v10 = v11 + v12
	loc2 = v10
	v12 = param0
	v13 = v10 != v12
	branch v13 l00000032
	// succ:  l0000003E l00000032
l0000003E:
	v10 = 1<32>
	return v10
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { i32 }, i32);
            RunTest(sExp, FnDef(
                0,
                new LocalVariable[] {
                    new LocalVariable(PrimitiveType.Int32),
                    new LocalVariable(PrimitiveType.Int32),
                    new LocalVariable(PrimitiveType.Int32),
                    new LocalVariable(PrimitiveType.Real64),
                    new LocalVariable(PrimitiveType.Real64),
                    new LocalVariable(PrimitiveType.Real64),
                },
                ByteCode(
                    f64_const, 0.0,
                    set_local, 0x03,
                    f64_const, 0.0 ,
                    set_local, 0x3 ,
                    f64_const, 0.0  ,
                    set_local, 0x4 ,
                    f64_const, 0.0 ,
                    set_local, 0x5 ,
                    i32_const, 0x0 ,
                    set_local, 0x2 ,
                    loop, 0x40,
                        get_local, 0x2,
                        i32_const, 0x1,
                        i32_add,
                        tee_local, 0x2,
                        get_local, 0x0,
                        i32_ne,
                        br_if, 0x0,
                    end,
                    i32_const, 1,
                    end)));
        }

        [Test]
        public void Wasmp_f64_convert_s_i32()
        {
            var sExpected = @"
// fn00000
// Return size: 0
real64 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = CONVERT(v3, int32, real64)
	return v4
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { i32 }, 124);
            RunTest(sExpected, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                ByteCode(
                    get_local, 0,
                    f64_convert_s_i32,
                    end)));
        }

        [Test]
        public void Wasmp_f64_convert_u_i32()
        {
            var sExpected = @"
// fn00000
// Return size: 0
real64 fn00000(word32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = CONVERT(v3, uint32, real64)
	return v4
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { i32 }, f64);
            RunTest(sExpected, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                ByteCode(
                    get_local, 0,
                    f64_convert_u_i32,
                    end)));
        }

        [Test]
        public void Wasmp_f64_abs()
        {
            var sExpected = @"
// fn00000
// Return size: 0
real64 fn00000(real64 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v3 = fabs(v3)
	return v3
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { f64 }, f64);
            RunTest(sExpected, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                ByteCode(
                    get_local, 0,
                    f64_abs,
                    end)));
        }

        [Test]
        public void Wasmp_f64_promote_f32()
        {
            var sExpected = @"
// fn00000
// Return size: 0
real64 fn00000(real32 param0)
fn00000_entry:
	// succ:  l00000000
l00000000:
	v3 = param0
	v4 = CONVERT(v3, real32, real64)
	return v4
	// succ:  fn00000_exit
fn00000_exit:
";
            Given_FuncType(new[] { f32 }, f64);
            RunTest(sExpected, FnDef(
                0,
                Array.Empty<LocalVariable>(),
                ByteCode(
                    get_local, 0,
                    f64_promote_f32,
                    end)));
        }

    }
}
