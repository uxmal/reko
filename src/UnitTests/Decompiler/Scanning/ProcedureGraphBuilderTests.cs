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

using Moq;
using NUnit.Framework;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Types;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Reko.UnitTests.Decompiler.Scanning
{
    [TestFixture]
    public class ProcedureGraphBuilderTests
    {
        private readonly Identifier r1;
        private readonly Identifier r2;
        private readonly Identifier C;
        private readonly Identifier NZVC;
        private ScanResultsV2 sr;
        private List<RtlProcedure> procs;
        private Program program;

        public ProcedureGraphBuilderTests()
        {
            r1 = Identifier.Create(new RegisterStorage("r1", 1, 0, PrimitiveType.Word32));
            r2 = Identifier.Create(new RegisterStorage("r2", 2, 0, PrimitiveType.Word32));
            var status = new RegisterStorage("status", 32, 0, PrimitiveType.Word32);
            C = Identifier.Create(new FlagGroupStorage(status, 0b0001, "C", PrimitiveType.Bool));
            NZVC = Identifier.Create(new FlagGroupStorage(status, 0b1111, "NZVC", PrimitiveType.Byte));
        }

        [SetUp]
        public void Setup()
        {
            var arch = new Mock<IProcessorArchitecture>();
            var platform = new Mock<IPlatform>();
            Frame MakeFrame() => new Frame(arch.Object, PrimitiveType.Ptr32);
            arch.Setup(a => a.CreateFrame()).Returns(MakeFrame);
            arch.Setup(a => a.Name).Returns("testArch");
            arch.Setup(a => a.StackRegister).Returns(new RegisterStorage("sp", 31, 0, PrimitiveType.Ptr32));

            platform.Setup(p => p.Architecture).Returns(arch.Object);

            this.sr = new ScanResultsV2();
            this.procs = new List<RtlProcedure>();
            this.program = new Program
            {
                Architecture = arch.Object,
                Platform = platform.Object,
                ImageMap = new ImageMap(Address.Ptr32(0x1000))
            };
        }

        private void Given_Procedure(params uint[] uAddrs)
        {
            var entry = sr.Blocks[Address.Ptr32(uAddrs[0])];
            var blocks = uAddrs.Select(u => sr.Blocks[Address.Ptr32(u)]).ToHashSet();
            var proc = new RtlProcedure(program.Architecture, entry.Address, blocks);
            this.procs.Add(proc);
        }

        private void Given_Block(uint uAddr, params Action<RtlEmitter>[] trace)
        {
            var addrBlock = Address.Ptr32(uAddr);
            var id = NamingPolicy.Instance.BlockName(addrBlock);
            var instrs = new List<RtlInstruction>();
            var m = new RtlEmitter(instrs);
            var addr = addrBlock;
            var clusters = new List<RtlInstructionCluster>();
            foreach (var builder in trace)
            {
                builder(m);
                var cluster = new RtlInstructionCluster(addrBlock, 4, instrs.ToArray());
                addr += 4;
                instrs.Clear();
                clusters.Add(cluster);
            }
            var block = new RtlBlock(
                program.Architecture,
                addrBlock, 
                id,
                (int)(addr-addrBlock), 
                addr,
                clusters);
            sr.Blocks.TryAdd(addrBlock, block);
        }

        private void Given_Edge(uint uAddrFrom, uint uAddrTo)
        {
            var addrFrom = Address.Ptr32(uAddrFrom);
            var addrTo = Address.Ptr32(uAddrTo);
            if (!sr.Successors.TryGetValue(addrFrom, out var succ))
            {
                succ = new List<Address>();
                sr.Successors.TryAdd(addrFrom, succ);
            }
            succ.Add(addrTo);
        }

        private void RunTest(string sExp)
        {
            var pgb = new ProcedureGraphBuilder(sr, program);
            pgb.Build(procs);
            var sw = new StringWriter();

            foreach (var proc in program.Procedures.Values)
            {
                sw.WriteLine();
                sw.WriteLine("// {0} ({1})", proc.Name, proc.EntryAddress);
                proc.Write(false, sw);
            }
            sw.WriteLine();
            program.CallGraph.Write(sw);

            var sActual = sw.ToString();
            if (sExp != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        public void Pgb_AssignConst()
        {
            Given_Block(0x1000, 
                m => { m.Assign(r2, 0xD2); },
                m => { m.Return(0, 0); }
            );
            Given_Procedure(0x1000);

            var sExp =
            #region Expected
@"
// fn00001000 (00001000)
// fn00001000
// Return size: 0
define fn00001000
fn00001000_entry:
	sp = fp
	// succ:  l00001000
l00001000:
	r2 = 0xD2<32>
	return
	// succ:  fn00001000_exit
fn00001000_exit:

Procedure fn00001000 calls:
";
            #endregion

            RunTest(sExp);
        }

        [Test]
        public void Pgb_AssignTestAndBranch() 
        {
            Given_Block(0x1000,
                m => { m.Assign(r2, 0); },
                m => { m.Assign(NZVC, m.Cond(m.ISub(r2, 0xD2))); },
                m => { m.Branch(m.Test(ConditionCode.ULT, C), Address.Ptr32(0x1010)); });
            Given_Edge(0x1000, 0x100C);
            Given_Edge(0x1000, 0x1010);

            Given_Block(0x100C,
                m => { m.Assign(r2, 1); });
            Given_Edge(0x100C, 0x1010);

            Given_Block(0x1010,
                m => { m.Return(0, 0); }
            );
            Given_Procedure(0x1000, 0x100C, 0x1010);

            var sExp =
            #region Expected
                @"
// fn00001000 (00001000)
// fn00001000
// Return size: 0
define fn00001000
fn00001000_entry:
	sp = fp
	// succ:  l00001000
l00001000:
	r2 = 0<32>
	NZVC = cond(r2 - 0xD2<32>)
	branch Test(ULT,C) l00001010
	// succ:  l0000100C l00001010
l0000100C:
	r2 = 1<32>
	// succ:  l00001010
l00001010:
	return
	// succ:  fn00001000_exit
fn00001000_exit:

Procedure fn00001000 calls:
";
            #endregion

            RunTest(sExp);
        }

        [Test]
        public void Pgb_Loop()
        {
            Given_Block(0x1000,
                m => { m.Assign(r1, 0); },
                m => { m.Goto(Address.Ptr32(0x1010)); });
            Given_Edge(0x1000, 0x1010);

            Given_Block(0x1008,
                m => { m.Assign(r1, m.Mem32(r2)); },
                m => { m.Assign(r2, m.IAdd(r2, 4)); });
            Given_Edge(0x1008, 0x1010);

            Given_Block(0x1010,
                m => { m.Branch(
                            m.Ne(r1, m.Word32(0x00123400)),
                            Address.Ptr32(0x1008)); });
            Given_Edge(0x1010, 0x1014);
            Given_Edge(0x1010, 0x1008);

            Given_Block(0x1014,
                m => { m.Return(0, 0); });

            Given_Procedure(0x1000, 0x1008, 0x1010, 0x1014);

            #region Expected
            string sExp =
@"
// fn00001000 (00001000)
// fn00001000
// Return size: 0
define fn00001000
fn00001000_entry:
	sp = fp
	// succ:  l00001000
l00001000:
	r1 = 0<32>
	goto 0x00001010<p32>
	goto l00001010
	// succ:  l00001010
l00001008:
	r1 = Mem0[r2:word32]
	r2 = r2 + 4<32>
	// succ:  l00001010
l00001010:
	branch r1 != 0x123400<32> l00001008
	// succ:  l00001014 l00001008
l00001014:
	return
	// succ:  fn00001000_exit
fn00001000_exit:

Procedure fn00001000 calls:
";
            #endregion

            RunTest(sExp);
        }

        [Test]
        public void Pgb_Calls()
        {
            Given_Block(0x1000,
                m => m.Assign(r1, m.Mem32(r2)),
                m => m.Call(Address.Ptr32(0x1010), 0));
            Given_Edge(0x1000, 0x1008);

            Given_Block(0x1008,
                m => m.Assign(m.Mem32(m.IAddS(r2, 8)), r1),
                m => m.Return(0, 0));
            Given_Procedure(0x1000, 0x1008);


            Given_Block(0x1010,
                m => m.Assign(r1, m.IAdd(r1, 1)),
                m => m.Return(0, 0));
            Given_Procedure(0x1010);

            var sExp =
            #region Expected
@"
// fn00001000 (00001000)
// fn00001000
// Return size: 0
define fn00001000
fn00001000_entry:
	sp = fp
	// succ:  l00001000
l00001000:
	r1 = Mem0[r2:word32]
	call fn00001010 (retsize: 0;)
	// succ:  l00001008
l00001008:
	Mem0[r2 + 8<i32>:word32] = r1
	return
	// succ:  fn00001000_exit
fn00001000_exit:

// fn00001010 (00001010)
// fn00001010
// Return size: 0
define fn00001010
fn00001010_entry:
	sp = fp
	// succ:  l00001010
l00001010:
	r1 = r1 + 1<32>
	return
	// succ:  fn00001010_exit
fn00001010_exit:

Procedure fn00001000 calls:
	fn00001010
Procedure fn00001010 calls:
Statement 00001000 call fn00001010 (retsize: 0;) calls:
	fn00001010
";
            #endregion

            RunTest(sExp);
        }
    }
}
