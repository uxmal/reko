using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Arch.X86.Assembler;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.X86
{
    [TestFixture]
    public class X86Rewriter_16bitTests : Arch.RewriterTestBase
    {
        private readonly IntelArchitecture arch;
        private readonly Address addrBase;

        public X86Rewriter_16bitTests()
        {
            var sc = CreateServiceContainer();
            arch = new X86ArchitectureReal(sc, "x86-real-16", new Dictionary<string, object>());
            addrBase = Address.SegPtr(0x0C00, 0x0000);
        }

        private void Given_Asm(Action<X86Assembler> fn)
        {
            var asm = new X86Assembler(arch, addrBase, new List<ImageSymbol>());
            var host = new RewriterHost(arch, asm.ImportReferences);
            fn(asm);
            Given_MemoryArea(asm.GetImage().SegmentMap.Segments.Values.First().MemoryArea);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrBase;

        [Test]
        public void X86rw_adc()
        {
            Given_Asm(m =>
            {
                m.Adc(m.WordPtr(0x100), m.ax);
            });
            AssertCode(
                "0|L--|0C00:0000(4): 3 instructions",
                "1|L--|v6 = Mem0[ds:0x100<16>:word16] + ax + C",
                "2|L--|Mem0[ds:0x100<16>:word16] = v6",
                "3|L--|SCZO = cond(v6)");
        }

        [Test]
        public void X86rw_add_reg_mem()
        {
            Given_Asm(m =>
            {
                m.Add(m.ax, m.MemW(Registers.si, 4));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 2 instructions",
                "1|L--|ax = ax + Mem0[ds:si + 4<i16>:word16]",
                "2|L--|SCZO = cond(ax)");
        }

        [Test]
        public void X86rw_add_to_mem()
        {
            Given_Asm(m =>
            {
                m.Add(m.WordPtr(0x1000), 3);
            });
            AssertCode(
                "0|L--|0C00:0000(5): 3 instructions",
                "1|L--|v4 = Mem0[ds:0x1000<16>:word16] + 3<16>",
                "2|L--|Mem0[ds:0x1000<16>:word16] = v4",
                "3|L--|SCZO = cond(v4)");
        }

        /// <summary>
        /// Captures the side effect of setting OF = 0 and CF = 0
        /// </summary>
        [Test]
        public void X86rw_and()
        {
            Given_Asm(m =>
            {
                m.And(m.si, m.Imm(0x32));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 4 instructions",
                "1|L--|si = si & 0x32<16>",
                "2|L--|SZ = cond(si)",
                "3|L--|O = false",
                "4|L--|C = false");
        }

        [Test]
        public void X86rw_call_indirect()
        {
            Given_Asm(m =>
            {
                m.Call(m.bx);
                m.Call(m.WordPtr(m.bx, 4));
                m.Call(m.DwordPtr(m.bx, 8));
            });
            AssertCode(
                "0|T--|0C00:0000(2): 1 instructions",
                "1|T--|call 0xC00<16>:bx (2)",
                "2|T--|0C00:0002(3): 1 instructions",
                "3|T--|call 0xC00<16>:Mem0[ds:bx + 4<i16>:word16] (2)",
                "4|T--|0C00:0005(3): 1 instructions",
                "5|T--|call Mem0[ds:bx + 8<i16>:segptr32] (4)");
        }

        [Test]
        public void X86rw_call_near()
        {
            Given_Asm(m =>
            {
                m.Label("self");
                m.Call("self");
            });
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|call 0C00:0000 (2)");
        }

        [Test]
        public void X86Rw_cbw()
        {
            Given_HexString("98"); // cbw
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|ax = CONVERT(al, int8, int16)");
        }

        [Test]
        public void X86rw_cmp()
        {
            Given_Asm(m =>
            {
                m.Cmp(m.ebx, 3);
            });
            AssertCode(
                "0|L--|0C00:0000(4): 1 instructions",
                "1|L--|SCZO = cond(ebx - 3<32>)");
        }

        [Test]
        public void X86Rw_cwd()
        {
            Given_HexString("99"); // cwd
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|dx_ax = CONVERT(ax, int16, int32)");
        }

        [Test]
        public void X86rw_div_with_remainder()
        {
            Given_Asm(m =>
            {
                m.Div(m.cx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 4 instructions",
                "1|L--|v6 = dx_ax",
                "2|L--|dx = CONVERT(v6 %u cx, word32, uint16)",
                "3|L--|ax = CONVERT(v6 /u cx, word16, uint16)",
                "4|L--|SCZO = cond(ax)");
        }

        [Test]
        public void X86rw_enter()
        {
            Given_Asm(m =>
            {
                m.Enter(16, 0);
            });
            AssertCode(
                "0|L--|0C00:0000(4): 4 instructions",
                "1|L--|sp = sp - 2<i16>",
                "2|L--|Mem0[ss:sp:word16] = bp",
                "3|L--|bp = sp",
                "4|L--|sp = sp - 16<i16>");
        }

        [Test]
        public void X86rw_f2xm1()
        {
            Given_HexString("D9F0");
            AssertCode(     // f2xm1
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|ST[Top:real64] = pow(2.0, ST[Top:real64]) - 1.0");
        }

        [Test]
        public void X86rw_fiadd()
        {
            Given_Asm(m =>
            {
                m.Fiadd(m.WordPtr(m.bx, 0));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|ST[Top:real64] = ST[Top:real64] + CONVERT(Mem0[ds:bx:int16], int16, real64)");
        }

        [Test]
        public void X86rw_fld_real32()
        {
            Given_HexString("D94440"); // fld word ptr [foo]
            AssertCode(
                "0|L--|0C00:0000(3): 2 instructions",
                "1|L--|Top = Top - 1<i8>",
                "2|L--|ST[Top:real64] = CONVERT(Mem0[ds:si + 64<i16>:real32], real32, real64)");
        }

        [Test]
        public void X86rw_fmul()
        {
            Given_Asm(m =>
            {
                m.Fmul(m.St(1));
            });
            AssertCode(
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|ST[Top:real64] = ST[Top:real64] * ST[Top + 1<i8>:real64]");
        }

        [Test]
        public void X86rw_fptan()
        {
            Given_HexString("D9F2");
            AssertCode(     // fptan
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ST[Top:real64] = tan(ST[Top:real64])",
                "2|L--|Top = Top - 1<i8>",
                "3|L--|ST[Top:real64] = 1.0");
        }

        [Test]
        public void X86Rw_frstpm()
        {
            Given_HexString("DBE5");
            AssertCode(     // frstpm
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|__frstpm()");
        }

        [Test]
        public void X86rw_fyl2x()
        {
            Given_HexString("d9f1");
            AssertCode(
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ST[Top + 1<i8>:real64] = ST[Top + 1<i8>:real64] * lg2(ST[Top:real64])",
                "2|L--|FPUF = cond(ST[Top + 1<i8>:real64])",
                "3|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_fyl2xp1()
        {
            Given_HexString("D9F9");
            AssertCode(
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ST[Top + 1<i8>:real64] = ST[Top + 1<i8>:real64] * lg2(ST[Top:real64] + 1.0)",
                "2|L--|FPUF = cond(ST[Top + 1<i8>:real64])",
                "3|L--|Top = Top + 1<i8>");
        }

        [Test]
        public void X86rw_idiv_with_remainder()
        {
            Given_Asm(m =>
            {
                m.Idiv(m.cx);
            });
            AssertCode(
                    "0|L--|0C00:0000(2): 4 instructions",
                    "1|L--|v6 = dx_ax",
                    "2|L--|dx = CONVERT(v6 %s cx, word32, int16)",
                    "3|L--|ax = CONVERT(v6 /16 cx, word16, int16)",
                    "4|L--|SCZO = cond(ax)");
        }

        [Test]
        public void X86rw_imul()
        {
            Given_Asm(m =>
            {
                m.Imul(m.cx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 2 instructions",
                "1|L--|dx_ax = cx *s32 ax",
                "2|L--|SCZO = cond(dx_ax)");
        }

        [Test]
        public void X86rw_in()
        {
            Given_Asm(m =>
            {
                m.In(m.al, m.dx);
            });
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|al = __in<byte>(dx)");
        }

        [Test]
        public void X86rw_int()
        {
            Given_Asm(m =>
            {
                m.Mov(m.ax, 0x4C00);
                m.Int(0x21);
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|ax = 0x4C00<16>",
                "2|T--|0C00:0003(2): 1 instructions",
                "3|L--|__syscall<byte>(0x21<8>)");
        }

        [Test]
        public void X86rw_jcxz()
        {
            Given_Asm(m =>
            {
                m.Label("lupe");
                m.Jcxz("lupe");
            });
            AssertCode(
                "0|T--|0C00:0000(2): 1 instructions",
                "1|T--|if (cx == 0<16>) branch 0C00:0000");
        }

        [Test]
        public void X86rw_jmp()
        {
            Given_Asm(m =>
            {
                m.Label("lupe");
                m.Jmp("lupe");
            });
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|goto 0C00:0000");
        }

        [Test]
        public void X86Rw_jmpf_indirect()
        {
            Given_HexString("FF6F34");
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|goto Mem0[ds:bx + 52<i16>:segptr32]");
        }

        [Test]
        public void X86rw_jmp_indirect()
        {
            Given_Asm(m =>
            {
                m.Jmp(m.WordPtr(m.bx, 0x10));
            });
            AssertCode(
                "0|T--|0C00:0000(3): 1 instructions",
                "1|T--|goto Mem0[ds:bx + 16<i16>:word16]");
        }

        [Test]
        public void X86rw_jne()
        {
            Given_Asm(m =>
            {
                m.Label("lupe");
                m.Jnz("lupe");
                m.Xor(m.ax, m.ax);
            });
            AssertCode(
                "0|T--|0C00:0000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 0C00:0000",
                "2|L--|0C00:0002(2): 4 instructions",
                "3|L--|ax = ax ^ ax",
                "4|L--|SZ = cond(ax)",
                "5|L--|O = false",
                "6|L--|C = false");
        }

        [Test]
        public void X86rw_jpe_jpo()
        {
            Given_Asm(m =>
            {
                m.Label("foo");
                m.Jpe("foo");
                m.Jpo("foo");
            });
            AssertCode(
                "0|T--|0C00:0000(2): 1 instructions",
                "1|T--|if (Test(PE,P)) branch 0C00:0000",
                "2|T--|0C00:0002(2): 1 instructions",
                "3|T--|if (Test(PO,P)) branch 0C00:0000");
        }

        [Test]
        public void X86rw_lahf()
        {
            Given_Asm(m =>
            {
                m.Lahf();
            });
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|ah = SCZOP");
        } 
        
        [Test]
        public void X86rw_lea()
        {
            Given_Asm(m =>
            {
                m.Lea(m.bx, m.MemW(Registers.bx, 4));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|bx = bx + 4<i16>");
        }

        [Test]
        public void X86rw_les_bp()
        {
            Given_Asm(m =>
            {
                m.Les(m.bx, m.MemW(Registers.bp, 6));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|es_bx = Mem0[ss:bp + 6<i16>:segptr32]");
        }

        [Test]
        public void X86rw_les_bx()
        {
            Given_Asm(m =>
            {
                m.Les(m.bx, m.DwordPtr(m.bx, 0));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|es_bx = Mem0[ds:bx:segptr32]");
        }

        [Test]
        public void X86rw_loop()
        {
            Given_Asm(m =>
            {
                m.Label("lupe");
                m.Loop("lupe");
            });
            AssertCode(
                "0|T--|0C00:0000(2): 2 instructions",
                "1|L--|cx = cx - 1<16>",
                "2|T--|if (cx != 0<16>) branch 0C00:0000");
        }

        [Test]
        public void X86rw_loope()
        {
            Given_Asm(m =>
            {
                m.Label("lupe");
                m.Loope("lupe");
                m.Mov(m.bx, m.ax);
            });
            AssertCode(
                "0|T--|0C00:0000(2): 2 instructions",
                "1|L--|cx = cx - 1<16>",
                "2|T--|if (Test(EQ,Z) && cx != 0<16>) branch 0C00:0000",
                "3|L--|0C00:0002(2): 1 instructions",
                "4|L--|bx = ax");
        }

        [Test]
        public void X86rw_mov_ax_bx()
        {
            Given_Asm(m =>
            {
                m.Mov(m.ax, m.bx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|ax = bx");
        }

        [Test]
        public void X86rw_mov_stackArgument()
        {
            Given_Asm(m =>
            {
                m.Mov(m.ax, m.MemW(Registers.bp, -8));
            });
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|ax = Mem0[ss:bp - 8<i16>:word16]");
        }

        [Test]
        public void X86Rw_movsb()
        {
            Given_HexString("F3 A4");
            AssertCode(
                "0|L--|0C00:0000(2): 5 instructions",
                "1|L--|size = cx *u32 1<16>",
                "2|L--|memcpy(es:di, ds:si, size)",
                "3|L--|cx = 0<16>",
                "4|L--|si = si + SLICE(size, word16, 0)",
                "5|L--|di = di + SLICE(size, word16, 0)");
        }

        [Test]
        public void X86Rw_movsd_16bit()
        {
            Given_HexString("B9 00 40 66 F3 A5");
            AssertCode(
                "0|L--|0C00:0000(3): 1 instructions",
                "1|L--|cx = 0x4000<16>",
                "2|L--|0C00:0003(3): 5 instructions",
                "3|L--|size = cx *u32 4<16>",
                "4|L--|memcpy(es:di, ds:si, size)",
                "5|L--|cx = 0<16>",
                "6|L--|si = si + SLICE(size, word16, 0)",
                "7|L--|di = di + SLICE(size, word16, 0)");
        }

        [Test]
        public void X86Rw_movsb_segment_override()
        {
            Given_HexString("26 A4");
            AssertCode(     // movsb\tbyte ptr es:[di],byte ptr es:[si]
                "0|L--|0C00:0000(2): 4 instructions",
                "1|L--|v3 = Mem0[es:si:byte]",
                "2|L--|Mem0[es:di:byte] = v3",
                "3|L--|si = si + 1<i16>",
                "4|L--|di = di + 1<i16>");
        }

        [Test]
        public void X86rw_or()
        {
            Given_Asm(m =>
            {
                m.Or(m.ax, m.dx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 4 instructions",
                "1|L--|ax = ax | dx",
                "2|L--|SZ = cond(ax)",
                "3|L--|O = false",
                "4|L--|C = false");
        }

        [Test]
        public void X86rw_mul()
        {
            Given_Asm(m =>
            {
                m.Mul(m.cx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 2 instructions",
                "1|L--|dx_ax = cx *u32 ax",
                "2|L--|SCZO = cond(dx_ax)");
        }

        [Test]
        public void X86rw_neg()
        {
            // Intel manual states that the C flag is set 
            // to 0 if the _source_ operand is 0.
            Given_Asm(m =>
            {
                m.Neg(m.ecx);
            });
            AssertCode(
                "0|L--|0C00:0000(3): 3 instructions",
                "1|L--|C = ecx != 0<32>",
                "2|L--|ecx = -ecx",
                "3|L--|SZO = cond(ecx)");
        }

        [Test]
        public void X86rw_not()
        {
            Given_Asm(m =>
            {
                m.Not(m.bx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 1 instructions",
                "1|L--|bx = ~bx");
        }

        [Test]
        public void X86rw_out()
        {
            Given_Asm(m =>
            {
                m.Out(m.dx, m.al);
            });
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|__out<byte>(dx, al)");
        }

        [Test]
        public void X86Rw_phsubsw()
        {
            Given_HexString("0F380710");
            AssertCode(     // phsubsw  mm2,[bx+si]
                "0|L--|0C00:0000(4): 3 instructions",
                "1|L--|v7 = Mem0[ds:bx + si:word64]",
                "2|L--|mm2_v7 = SEQ(mm2, v7)",
                "3|L--|mm2 = __phsubs<int16[8],int16[4]>(mm2_v7)");
        }

        [Test]
        public void X86rw_push_cs_call_near()
        {
            Given_HexString("0EE84232");
            AssertCode(
                "0|T--|0C00:0000(4): 1 instructions",
                "1|T--|call 0C00:3246 (4)");
        }

        [Test]
        public void X86rw_push_pop()
        {
            Given_Asm(m =>
            {
                m.Push(m.eax);
                m.Pop(m.ebx);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 2 instructions",
                "1|L--|sp = sp - 4<i16>",
                "2|L--|Mem0[ss:sp:word32] = eax",
                "3|L--|0C00:0002(2): 2 instructions",
                "4|L--|ebx = Mem0[ss:sp:word32]",
                "5|L--|sp = sp + 4<i16>");
        }

        [Test]
        public void X86rw_pushf_popf()
        {
            Given_Asm((m) =>
            {
                m.Pushf();
                m.Popf();
            });
            AssertCode(
                "0|L--|0C00:0000(1): 2 instructions",
                "1|L--|sp = sp - 2<i16>",
                "2|L--|Mem0[ss:sp:word16] = SCZDOP",
                "3|L--|0C00:0001(1): 2 instructions",
                "4|L--|SCZDOP = Mem0[ss:sp:word16]",
                "5|L--|sp = sp + 2<i16>");
        }

        [Test]
        public void X86rw_rep_lodsw()
        {
            Given_Asm(m =>
            {
                m.Rep();
                m.Lodsw();
                m.Xor(m.ax, m.ax);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 5 instructions",
                "1|T--|if (cx == 0<16>) branch 0C00:0002",
                "2|L--|ax = Mem0[ds:si:word16]",
                "3|L--|si = si + 2<i16>",
                "4|L--|cx = cx - 1<16>",
                "5|T--|goto 0C00:0000",
                "6|L--|0C00:0002(2): 4 instructions",
                "7|L--|ax = ax ^ ax",
                "8|L--|SZ = cond(ax)",
                "9|L--|O = false",
                "10|L--|C = false");
        }

        [Test]
        public void X86rw_rep_scasb()
        {
            Given_Asm(m =>
            {
                m.Rep();
                m.Scasb();
                m.Ret();
            });
            AssertCode(
                "0|L--|0C00:0000(2): 5 instructions",
                "1|T--|if (cx == 0<16>) branch 0C00:0002",
                "2|L--|SCZO = cond(al - Mem0[es:di:byte])",
                "3|L--|di = di + 1<i16>",
                "4|L--|cx = cx - 1<16>",
                "5|T--|if (Test(NE,Z)) branch 0C00:0000",
                "6|R--|0C00:0002(1): 1 instructions",
                "7|R--|return (2,0)");
        }

        [Test]
        public void X86rw_ret()
        {
            Given_Asm(m =>
            {
                m.Ret();
            });
            AssertCode(
                "0|R--|0C00:0000(1): 1 instructions",
                "1|R--|return (2,0)");
        }

        [Test]
        public void X86rw_retn()
        {
            Given_Asm(m =>
            {
                m.Ret(8);
            });
            AssertCode(
                "0|R--|0C00:0000(3): 1 instructions",
                "1|R--|return (2,8)");
        }

        //$TODO: this special case should be handled usng
        // memory maps from msdps.
        [Test]
        public void X86rw_RealModeReboot()
        {
            Given_Asm(m =>
            {
                m.JmpF(Address.SegPtr(0xF000, 0xFFF0));
            });
            AssertCode(
                "0|T--|0C00:0000(5): 1 instructions",
                "1|L--|__bios_reboot()");
        }

        [Test]
        public void X86rw_sar()
        {
            Given_Asm(m =>
            {
                m.Sar(m.ax, 1);
                m.Sar(m.bx, m.cl);
                m.Sar(m.dx, 4);
            });
            AssertCode(
                "0|L--|0C00:0000(2): 3 instructions",
                "1|L--|ax = ax >> 1<16>",
                "2|L--|SCZ = cond(ax)",
                "3|L--|O = false",
                "4|L--|0C00:0002(2): 2 instructions",
                "5|L--|bx = bx >> cl",
                "6|L--|SCZ = cond(bx)",
                "7|L--|0C00:0004(3): 2 instructions",
                "8|L--|dx = dx >> 4<16>",
                "9|L--|SCZ = cond(dx)");
        }

        [Test]
        public void X86rw_shld()
        {
            Given_Asm(m =>
            {
                m.Shld(m.edx, m.eax, m.cl);
            });
            AssertCode(
                "0|L--|0C00:0000(4): 1 instructions",
                "1|L--|edx = __shld<word32>(edx, eax, cl)");
        }

        [Test]
        public void X86rw_shrd()
        {
            Given_Asm(m =>
            {
                m.Shrd(m.eax, m.edx, 4);
            });
            AssertCode(
                "0|L--|0C00:0000(5): 1 instructions",
                "1|L--|eax = __shrd<word32>(eax, edx, 4<8>)");
        }

        [Test]
        public void X86rw_sub()
        {
            Given_Asm(m =>
            {
                m.Sub(m.ecx, 0x12345);
            });
            AssertCode(
                "0|L--|0C00:0000(7): 2 instructions",
                "1|L--|ecx = ecx - 0x12345<32>",
                "2|L--|SCZO = cond(ecx)");
        }

        [Test(Description = "Captures the side effect of setting OF = 0 and CF = 0")]
        public void X86rw_test()
        {
            Given_Asm(m =>
            {
                m.Test(m.edi, m.Imm(0xFFFFFFFFu));
            });
            AssertCode(
                "0|L--|0C00:0000(7): 3 instructions",
                "1|L--|SZP = cond(edi & 0xFFFFFFFF<32>)",
                "2|L--|O = false",
                "3|L--|C = false");
        }

        [Test]
        public void X86rw_xlat()
        {
            Given_Asm(m =>
            {
                m.Xlat();
            });
            AssertCode(
                "0|L--|0C00:0000(1): 1 instructions",
                "1|L--|al = Mem0[ds:bx + CONVERT(al, uint8, uint16):byte]");
        }

        [Test]
        public void X86rw_xor()
        {
            Given_Asm(m =>
            {
                m.Xor(m.eax, m.eax);
            });
            AssertCode(
                "0|L--|0C00:0000(3): 4 instructions",
                "1|L--|eax = eax ^ eax",
                "2|L--|SZ = cond(eax)",
                "3|L--|O = false",
                "4|L--|C = false");
        }

    }
}
