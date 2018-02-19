#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Code;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.UnitTests.Mocks;
using Reko.Scanning;
using Rhino.Mocks;
using Reko.Evaluation;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class Backwalker2Tests
    {
        private MockRepository mr;
        private FakeArchitecture arch;
        private ProcedureBuilder m;
        private IBackWalkHost<Block, Instruction> host;
        private ExpressionSimplifier eval;

        [SetUp]
        public void Setup()
        {
            this.mr = new MockRepository();
            this.arch = new FakeArchitecture();
            this.m = new ProcedureBuilder(arch);
            this.host = mr.Stub<IBackWalkHost<Block, Instruction>>();
            this.eval = null;
        }

        [Test]
        public void BwDetermineVars_JmpIndirectReg()
        {
            var r2 = m.Reg32("r2", 2);
            var xfer = new RtlGoto(r2, RtlClass.Transfer);
            mr.ReplayAll();

            var bw = new Backwalker2<Block, Instruction>(host, xfer, eval);
            Assert.AreEqual(1, bw.IndexVariables.Count);
            Assert.AreEqual(true, bw.IndexVariables.ContainsKey(r2));
        }

        [Test]
        public void BwDetermineVars_JmpIndirectMem()
        {
            var r2 = m.Reg32("r2", 2);
            var xfer = new RtlGoto(m.Mem32(r2), RtlClass.Transfer);
            mr.ReplayAll();

            var bw = new Backwalker2<Block, Instruction>(host, xfer, eval);
            Assert.AreEqual(1, bw.IndexVariables.Count);
            Assert.AreEqual(true, bw.IndexVariables.ContainsKey(r2));
        }

        // Test cases
        // A one-level jump table from MySQL. JTT represents the jump table.
        // mov ebp,[rsp + 0xf8]         : 0 ≤ rdx==[rsp+0xf8]==ebp≤ 5
        // cmp ebp, 5                   : 0 ≤%ebp≤ 5 
        // ja 43a4ab 
        // lea rax,[0x525e8f + rip]             : rax = 0x9602a0, rcx = 0x43a116
        // lea rcx,[rip -0x302]                 : JTT = 0x43a116 + [0x9602a0 + rdx×8]       : rax, [rsp 
        // movsx rdx,dword ptr [rsp + 0xF8]     : rdx==[rsp+0xf8]                           : raxl, [rsp + f8], rcx 
        // add rcx, [rax + rdx*8]               : JTT = rcx + [rax + rdx×8]                 : rax, rdx, rcx
        // jmp rcx                              : JTT = rcx                                 : ecx

        // cmp eax,0xa9                         :  0 ≤ eax  ≤ 0xa9                          : eax [0, 0xAA)
        // ja 0x41677e                          :                                           : ecx, eax, CZ
        // movzx ecx,byte ptr [0x416bd4 + eax]  : ecx = [0x416bd4 + eax]                    : eax
        //                                        JTT = [0x416bc0 + [0x416bd4 + eax] × 4]
        // jmp [0x416bc0 + ecx *4]              : JTT = [0x416bc0+ecx×4]                    : ecx


        // A one-level jump table.
        // The input upper bound to this jump table must be inferred.
        // In addition, the input is right shifted to get the index into the table

        // movzx eax,byte ptr [edi]     : 0 ≤ eax ≤ 255                                     : rax [0, 255]
        // shr al,4                     : rax = rax >> 4                                    : al ~ rax
        //                              : JTT = [0x495e30 + (rax >> 4)×8]
        // jmpq [0x495e30 + rax * 8]    : JTT = [0x495e30 + rax×8]                          : rax



        // Unoptimized x86 code
        // cmp dword ptr [ebp + 8], 0xA : 0 <= [ebp + 8] <= 0xA                     : [ebp + 8] [0, 0x0A]
        // ja default                                                               : [ebp + 8], CZ
        // movzx edx, byte ptr[ebp + 8] : edx = ZEX([(ebp + 8)], 8)                 : [ebp + 8]
        //                                JTT = [0x023450 + ZEX([ebp + 8)], 8) * 4] 
        // jmp [0x00234500 + edx * 4]   : JTT = [0x0023450 + edx * 4]               : edx

        // M68k relative jump code.
        //  corresponds to
        // cmpi.l #$00000028,d1         : 0 <= d1 <= 0x28
        // bgt $00106C66
        // add.l d1,d1                  : d1 = d1 * 2
        //                                JTT = 0x0010000 + SEXT:([0x10006 + d1*2],16)
        // move.w (06,pc,d1),d1         : JTT = 0x0010000 + SEXT:([0x10006 + d1],16)
        // jmp.l (pc,d1.w)              : JTT = 0x0010000 + SEXT(d1, 16)

        //  m.Assign(d1, m.IAdd(d1, d1));
        //  m.Assign(CVZNX, m.Cond(d1));
        //  m.Assign(v82,m.LoadW(m.IAdd(m.Word32(0x001066A4), d1)));
        //  m.Assign(d1, m.Dpb(d1, v82, 0));
        //  m.Assign(CVZN, m.Cond(v82));
        //  var block = m.CurrentBlock;
        //  var xfer = new RtlGoto(
        //      m.IAdd(
        //          m.Word32(0x001066A2), 
        //          m.Cast(PrimitiveType.Int32, m.Cast(PrimitiveType.Int16, d1))),
        //      RtlClass.Transfer);

        // Use case located by @smx-smx:  the rep movsd shouldn't affect the value of edx nor ecx.

        //007861C8 shr ecx,02
        //007861CB and edx,03
        //007861CE cmp ecx,08
        //007861D1 jc 007861FC
        //007861D3 rep movsd 
        //007861D5 jmp dword ptr[007862E8 + edx * 4]


        // cmp [ebp-66],1D
        // mov edx,[ebp-66]
        // movzx eax,byte ptr [edx + 0x10000]
        // jmp [eax + 0x12000]


    }
}
