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
using Reko.Arch.Arm;
using Reko.Arch.Arm.AArch32;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Rtl;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    [Category(Categories.Capstone)]
    public class T32RewriterTests : RewriterTestBase
    {
        private ThumbArchitecture arch;
        private Address baseAddress;

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => baseAddress;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            AArch32ProcessorState state = new AArch32ProcessorState(arch);
            return arch.CreateRewriter(new LeImageReader(mem, 0), state, binder, host);
        }

        private class FakeRewriterHost : IRewriterHost
        {
            public PseudoProcedure EnsurePseudoProcedure(string name, DataType returnType, int arity)
            {
                return new PseudoProcedure(name, returnType, arity);
            }

            public Expression CallIntrinsic(string name, FunctionType fnType, params Expression[] args)
            {
                throw new NotImplementedException();
            }

            public Expression PseudoProcedure(string name, DataType returnType, params Expression[] args)
            {
                throw new NotImplementedException();
            }

            public Expression PseudoProcedure(string name, ProcedureCharacteristics c, DataType returnType, params Expression[] args)
            {
                throw new NotImplementedException();
            }

            public IProcessorArchitecture GetArchitecture(string archLabel)
            {
                throw new NotImplementedException();
            }

            public Expression GetImport(Address addrThunk, Address addrInstr)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetImportedProcedure(IProcessorArchitecture arch, Address addrThunk, Address addrInstr)
            {
                throw new NotImplementedException();
            }

            public ExternalProcedure GetInterceptedCall(IProcessorArchitecture arch, Address addrImportThunk)
            {
                throw new NotImplementedException();
            }

            public void Error(Address address, string format, params object[] args)
            {
                throw new NotImplementedException();
            }

            public void Warn(Address address, string format, params object[] args)
            {
                throw new NotImplementedException();
            }
        }

        #region Lots of Thumb!
        public const string ThumbBlock =
@"
  0040100C: F242 1C21 mov         r12,#0x2121
  00401010: F2C0 0C40 movt        r12,#0x40
  00401014: 4760      bx          r12
  00401016: 0000      movs        r0,r0
  00401018: F242 1C79 mov         r12,#0x2179
  0040101C: F2C0 0C40 movt        r12,#0x40
  00401020: 4760      bx          r12

  0040103C: B081      sub         sp,sp,#4
  0040103E: F24A 0C20 mov         r12,#0xA020
  00401042: F2C0 0C40 movt        r12,#0x40
  00401046: F8DC C000 ldr         r12,[r12]
  0040104A: EBAD 0C0C sub         r12,sp,r12
  0040104E: F8CD C000 str         r12,[sp]
  00401052: 4770      bx          lr
  00401054: F24A 0C20 mov         r12,#0xA020
  00401058: F2C0 0C40 movt        r12,#0x40
  0040105C: 9B00      ldr         r3,[sp]
  0040105E: F8DC C000 ldr         r12,[r12]
  00401062: EBAD 0303 sub         r3,sp,r3
  00401066: 4563      cmp         r3,r12
  00401068: D101      bne         0040106E
  0040106A: B001      add         sp,sp,#4
  0040106C: 4770      bx          lr
  0040106E: 4618      mov         r0,r3
  00401070: B510      push        {r4,lr}
  00401072: 466C      mov         r4,sp
  00401074: 46EC      mov         r12,sp
  00401076: F02C 0C07 bic         r12,r12,#7
  0040107A: 46E5      mov         sp,r12
  0040107C: F000 F810 bl          004010A0
  00401080: 46A5      mov         sp,r4
  00401082: E8BD 4010 pop         {r4,lr}
  00401086: 4770      bx          lr

  004010A0: F8DF C00C ldr         r12,004010B0
  004010A4: F8DC C000 ldr         r12,[r12]
  004010A8: 4560      cmp         r0,r12
  004010AA: D103      bne         004010B4
  004010AC: 4770      bx          lr
  004010AE: 0000      movs        r0,r0
  004010B0: A020      adr         r0,00401134
  004010B2: 0040      lsls        r0,r0,#1
  004010B4: B508      push        {r3,lr}
  004010B6: B082      sub         sp,sp,#8
  004010B8: F8DF C01C ldr         r12,004010D8
  004010BC: F8DC C000 ldr         r12,[r12]
  004010C0: F8CD C004 str         r12,[sp,#4]
  004010C4: F8DF C014 ldr         r12,004010DC
  004010C8: F8DC C000 ldr         r12,[r12]
  004010CC: F8CD C000 str         r12,[sp]
  004010D0: F002 FF04 bl          00403EDC
  004010D4: DEFE      __debugbreak
  004010D6: 0000      movs        r0,r0
  004010D8: A024      adr         r0,0040116C
  004010DA: 0040      lsls        r0,r0,#1
  004010DC: A020      adr         r0,00401160
  004010DE: 0040      lsls        r0,r0,#1

  00402120: B403      push        {r0,r1}
  00402122: E92D 4830 push        {r4,r5,r11,lr}
  00402126: F10D 0B08 add         r11,sp,#8
  0040212A: B084      sub         sp,sp,#0x10
  0040212C: 4668      mov         r0,sp
  0040212E: F04F 31CC mov         r1,#0xCCCCCCCC
  00402132: 2210      movs        r2,#0x10
  00402134: F000 F8D2 bl          004022DC
  00402138: 9B08      ldr         r3,[sp,#0x20]
  0040213A: 2B00      cmp         r3,#0
  0040213C: D102      bne         00402144
  0040213E: 2300      movs        r3,#0
  00402140: 9300      str         r3,[sp]
  00402142: E013      b           0040216C
  00402144: 9B08      ldr         r3,[sp,#0x20]
  00402146: 2B01      cmp         r3,#1
  00402148: D102      bne         00402150
  0040214A: 2301      movs        r3,#1
  0040214C: 9300      str         r3,[sp]
  0040214E: E00D      b           0040216C
  00402150: 9B08      ldr         r3,[sp,#0x20]
  00402152: 1E58      subs        r0,r3,#1
  00402154: F7FE FF5A bl          0040100C
  00402158: 9001      str         r0,[sp,#4]
  0040215A: 9B08      ldr         r3,[sp,#0x20]
  0040215C: 1E98      subs        r0,r3,#2
  0040215E: F7FE FF55 bl          0040100C
  00402162: 9002      str         r0,[sp,#8]
  00402164: 9A01      ldr         r2,[sp,#4]
  00402166: 9B02      ldr         r3,[sp,#8]
  00402168: 4413      add         r3,r3,r2
  0040216A: 9300      str         r3,[sp]
  0040216C: 9800      ldr         r0,[sp]
  0040216E: B004      add         sp,sp,#0x10
  00402170: E8BD 0830 pop         {r4,r5,r11}
  00402174: F85D FB0C ldr         pc,[sp],#0xC
  00402178: B403      push        {r0,r1}
  0040217A: E92D 4830 push        {r4,r5,r11,lr}
  0040217E: F10D 0B08 add         r11,sp,#8
  00402182: B084      sub         sp,sp,#0x10
  00402184: 4668      mov         r0,sp
  00402186: F04F 31CC mov         r1,#0xCCCCCCCC
  0040218A: 2210      movs        r2,#0x10
  0040218C: F000 F8A6 bl          004022DC
  00402190: 2300      movs        r3,#0
  00402192: 9300      str         r3,[sp]
  00402194: E002      b           0040219C
  00402196: 9B00      ldr         r3,[sp]
  00402198: 1C5B      adds        r3,r3,#1
  0040219A: 9300      str         r3,[sp]
  0040219C: 9B00      ldr         r3,[sp]
  0040219E: 2B0A      cmp         r3,#0xA
  004021A0: D209      bcs         004021B6
  004021A2: 9800      ldr         r0,[sp]
  004021A4: F7FE FF32 bl          0040100C
  004021A8: 9002      str         r0,[sp,#8]
  004021AA: 9902      ldr         r1,[sp,#8]
  004021AC: 480C      ldr         r0,004021E0
  004021AE: 4B0A      ldr         r3,004021D8
  004021B2: 4798      blx         r3
  004021B4: E7EF      b           00402196
  004021B6: 4809      ldr         r0,004021DC
  004021B8: 4B07      ldr         r3,004021D8
  004021BC: 4798      blx         r3
  004021BE: 4B05      ldr         r3,004021D4
  004021C2: 4798      blx         r3
  004021C4: 2300      movs        r3,#0
  004021C6: 9301      str         r3,[sp,#4]
  004021C8: 9801      ldr         r0,[sp,#4]
  004021CA: B004      add         sp,sp,#0x10
  004021CC: E8BD 0830 pop         {r4,r5,r11}
  004021D0: F85D FB0C ldr         pc,[sp],#0xC
  004021D4: C110      stm         r1!,{r4}
  004021D6: 0040      lsls        r0,r0,#1
  004021D8: C108      stm         r1!,{r3}
  004021DA: 0040      lsls        r0,r0,#1
  004021DC: 785C      ldrb        r4,[r3,#1]
  004021DE: 0040      lsls        r0,r0,#1
  004021E0: 7858      ldrb        r0,[r3,#1]
  004021E2: 0040      lsls        r0,r0,#1

  00402230: F24C 1C10 mov         r12,#0xC110
  00402234: F2C0 0C40 movt        r12,#0x40
  00402238: F8DC F000 ldr         pc,[r12]
  0040223C: F24C 1C08 mov         r12,#0xC108
  00402240: F2C0 0C40 movt        r12,#0x40
  00402244: F8DC F000 ldr         pc,[r12]
  00402248: E92D 480C push        {r2,r3,r11,lr}
  0040224C: F10D 0B08 add         r11,sp,#8
  00402250: F24A 1230 mov         r2,#0xA130
  00402254: F2C0 0240 movt        r2,#0x40
  00402258: 7813      ldrb        r3,[r2]
  0040225A: B95B      cbnz        r3,00402274
  0040225C: 2301      movs        r3,#1
  0040225E: 7013      strb        r3,[r2]
  00402260: 2300      movs        r3,#0
  00402262: 9300      str         r3,[sp]
  00402264: 2301      movs        r3,#1
  00402266: 2200      movs        r2,#0
  00402268: 2100      movs        r1,#0
  0040226A: 2000      movs        r0,#0
  0040226C: F000 FB3E bl          004028EC
  00402270: F000 FB1A bl          004028A8
  00402274: E8BD 880C pop         {r2,r3,r11,pc}

  004022A8: E92D 480C push        {r2,r3,r11,lr}
  004022AC: F10D 0B08 add         r11,sp,#8
  004022B0: 2301      movs        r3,#1
  004022B2: 2200      movs        r2,#0
  004022B4: 2100      movs        r1,#0
  004022B6: 2000      movs        r0,#0
  004022B8: 9300      str         r3,[sp]
  004022BA: F000 FB17 bl          004028EC
  004022BE: E8BD 880C pop         {r2,r3,r11,pc}
  004022C2: 0000      movs        r0,r0
  004022C4: 0000      movs        r0,r0
  004022C6: 0000      movs        r0,r0
  004022C8: 0000      movs        r0,r0
  004022CA: 0000      movs        r0,r0
  004022CC: 0000      movs        r0,r0
  004022CE: 0000      movs        r0,r0
  004022D0: 0000      movs        r0,r0
  004022D2: 0000      movs        r0,r0
  004022D4: 0000      movs        r0,r0
  004022D6: 0000      movs        r0,r0
  004022D8: 0000      movs        r0,r0
  004022DA: 0000      movs        r0,r0
  004022DC: F24C 1C04 mov         r12,#0xC104
  004022E0: F2C0 0C40 movt        r12,#0x40
  004022E4: F8DC F000 ldr         pc,[r12]
  004022E8: E92D 4800 push        {r11,lr}
  004022EC: 46EB      mov         r11,sp
  004022EE: B084      sub         sp,sp,#0x10
  004022F0: F000 FA34 bl          0040275C
  004022F4: 9000      str         r0,[sp]
  004022F6: 9A00      ldr         r2,[sp]
  004022F8: 4B29      ldr         r3,004023A0
  004022FA: 601A      str         r2,[r3]
  004022FC: 2001      movs        r0,#1
  004022FE: 4B27      ldr         r3,0040239C
  00402302: 4798      blx         r3
  00402304: F06F 0000 mvn         r0,#0
  00402308: 4B23      ldr         r3,00402398
  0040230C: 4798      blx         r3
  0040230E: 9001      str         r0,[sp,#4]
  00402310: 9A01      ldr         r2,[sp,#4]
  00402312: 4B20      ldr         r3,00402394
  00402314: 601A      str         r2,[r3]
  00402316: 4B1F      ldr         r3,00402394
  0040231A: 4B1D      ldr         r3,00402390
  0040231C: 601A      str         r2,[r3]
  0040231E: 4B1B      ldr         r3,0040238C
  00402322: 4B19      ldr         r3,00402388
  00402326: 601A      str         r2,[r3]
  00402328: 4B16      ldr         r3,00402384
  0040232C: 4B14      ldr         r3,00402380
  00402330: 601A      str         r2,[r3]
  00402332: F000 FDE9 bl          00402F08
  00402336: F000 FB57 bl          004029E8
  0040233A: 4B10      ldr         r3,0040237C
  0040233E: 2B00      cmp         r3,#0
  00402340: D103      bne         0040234A
  00402342: 480D      ldr         r0,00402378
  00402344: 4B0B      ldr         r3,00402374
  00402348: 4798      blx         r3
  0040234A: 4B09      ldr         r3,00402370
  0040234E: F1B3 3FFF cmp         r3,#0xFFFFFFFF
  00402352: D104      bne         0040235E
  00402354: F06F 0000 mvn         r0,#0
  00402358: 4B04      ldr         r3,0040236C
  0040235C: 4798      blx         r3
  0040235E: 2300      movs        r3,#0
  00402360: 9302      str         r3,[sp,#8]
  00402362: 9802      ldr         r0,[sp,#8]
  00402364: B004      add         sp,sp,#0x10
  00402366: E8BD 8800 pop         {r11,pc}
  0040236A: DEFE      __debugbreak
  0040236C: C0D8      stm         r0!,{r3,r4,r6,r7}
  0040236E: 0040      lsls        r0,r0,#1
  00402370: A004      adr         r0,00402384
  00402372: 0040      lsls        r0,r0,#1
  00402374: C0D4      stm         r0!,{r2,r4,r6,r7}
  00402376: 0040      lsls        r0,r0,#1
  00402378: 2CC9      cmp         r4,#0xC9
  0040237A: 0040      lsls        r0,r0,#1
  0040237C: A000      adr         r0,00402380
  0040237E: 0040      lsls        r0,r0,#1
  00402380: C0BC      stm         r0!,{r2-r5,r7}
  00402382: 0040      lsls        r0,r0,#1
  00402384: A15C      adr         r1,004024F8
  00402386: 0040      lsls        r0,r0,#1
  00402388: C0C0      stm         r0!,{r6,r7}
  0040238A: 0040      lsls        r0,r0,#1
  0040238C: A168      adr         r1,00402530
  0040238E: 0040      lsls        r0,r0,#1
  00402390: A38C      adr         r3,004025C4
  00402392: 0040      lsls        r0,r0,#1
  00402394: A37C      adr         r3,00402588
  00402396: 0040      lsls        r0,r0,#1
  00402398: C04C      stm         r0!,{r2,r3,r6}
  0040239A: 0040      lsls        r0,r0,#1
  0040239C: C0F0      stm         r0!,{r4-r7}
  0040239E: 0040      lsls        r0,r0,#1
  004023A0: A13C      adr         r1,00402494
  004023A2: 0040      lsls        r0,r0,#1

  004023EC: E92D 4810 push        {r4,r11,lr}
  004023F0: F10D 0B04 add         r11,sp,#4
  004023F4: B083      sub         sp,sp,#0xC
  004023F6: 4817      ldr         r0,00402454
  004023F8: F000 FD60 bl          00402EBC
  004023FC: 4B14      ldr         r3,00402450
  00402400: 4B12      ldr         r3,0040244C
  00402402: 601A      str         r2,[r3]
  00402404: 4B11      ldr         r3,0040244C
  00402406: 9300      str         r3,[sp]
  00402408: 4B0F      ldr         r3,00402448
  0040240C: 4A0D      ldr         r2,00402444
  0040240E: 490C      ldr         r1,00402440
  00402410: 480A      ldr         r0,0040243C
  00402412: 4C09      ldr         r4,00402438
  00402416: 47A0      blx         r4
  00402418: 9002      str         r0,[sp,#8]
  0040241A: 9A02      ldr         r2,[sp,#8]
  0040241C: 4B05      ldr         r3,00402434
  0040241E: 601A      str         r2,[r3]
  00402420: 4B04      ldr         r3,00402434
  00402424: 2B00      cmp         r3,#0
  00402426: DA02      bge         0040242E
  00402428: 2008      movs        r0,#8
  0040242A: F000 FAD7 bl          004029DC
  0040242E: B003      add         sp,sp,#0xC
  00402430: E8BD 8810 pop         {r4,r11,pc}
  00402434: A14C      adr         r1,00402568
  00402436: 0040      lsls        r0,r0,#1
  00402438: C0F4      stm         r0!,{r2,r4-r7}
  0040243A: 0040      lsls        r0,r0,#1
  0040243C: A140      adr         r1,00402540
  0040243E: 0040      lsls        r0,r0,#1
  00402440: A144      adr         r1,00402554
  00402442: 0040      lsls        r0,r0,#1
  00402444: A148      adr         r1,00402568
  00402446: 0040      lsls        r0,r0,#1
  00402448: A160      adr         r1,004025CC
  0040244A: 0040      lsls        r0,r0,#1
  0040244C: A150      adr         r1,00402590
  0040244E: 0040      lsls        r0,r0,#1
  00402450: A164      adr         r1,004025E4
  00402452: 0040      lsls        r0,r0,#1
  00402454: 2F39      cmp         r7,#0x39
  00402456: 0040      lsls        r0,r0,#1

  00402488: E92D 4800 push        {r11,lr}
  0040248C: 46EB      mov         r11,sp
  0040248E: B082      sub         sp,sp,#8
  00402490: EE1D 3F50 mrc         p15,#0,r3,c13,c0,#2
  00402494: 9300      str         r3,[sp]
  00402496: 9B00      ldr         r3,[sp]
  00402498: 9301      str         r3,[sp,#4]
  0040249A: 9801      ldr         r0,[sp,#4]
  0040249C: B002      add         sp,sp,#8
  0040249E: E8BD 8800 pop         {r11,pc}
  004024A2: 0000      movs        r0,r0
  004024A4: E92D 4890 push        {r4,r7,r11,lr}
  004024A8: F10D 0B08 add         r11,sp,#8
  004024AC: B094      sub         sp,sp,#0x50
  004024AE: 466F      mov         r7,sp
  004024B0: B082      sub         sp,sp,#8
  004024B2: 2300      movs        r3,#0
  004024B4: 607B      str         r3,[r7,#4]
  004024B6: F7FF FFE7 bl          00402488
  004024BA: 62B8      str         r0,[r7,#0x28]
  004024BC: 6ABB      ldr         r3,[r7,#0x28]
  004024BE: 1D1B      adds        r3,r3,#4
  004024C2: 60BB      str         r3,[r7,#8]
  004024C4: 2300      movs        r3,#0
  004024C6: 60FB      str         r3,[r7,#0xC]
  004024C8: F3BF 8F5B dmb         ish
  004024CC: 4875      ldr         r0,004026A4
  004024CE: 68B9      ldr         r1,[r7,#8]
  004024D0: E850 2F00 ldrex       r2,[r0]
  004024D4: 2A00      cmp         r2,#0
  004024D6: D103      bne         004024E0
  004024D8: E840 1300 strex       r3,r1,[r0]
  004024DC: 2B00      cmp         r3,#0
  004024DE: D1F7      bne         004024D0
  004024E0: 4613      mov         r3,r2
  004024E2: 643B      str         r3,[r7,#0x40]
  004024E4: F3BF 8F5B dmb         ish
  004024E8: 6C3B      ldr         r3,[r7,#0x40]
  004024EA: 607B      str         r3,[r7,#4]
  004024EC: 687B      ldr         r3,[r7,#4]
  004024EE: 2B00      cmp         r3,#0
  004024F0: D007      beq         00402502
  004024F2: 687A      ldr         r2,[r7,#4]
  004024F4: 68BB      ldr         r3,[r7,#8]
  004024F6: 429A      cmp         r2,r3
  004024F8: D102      bne         00402500
  004024FA: 2301      movs        r3,#1
  004024FC: 60FB      str         r3,[r7,#0xC]
  004024FE: E000      b           00402502
  00402500: E7E2      b           004024C8
  00402502: 4B6D      ldr         r3,004026B8
  00402506: 623B      str         r3,[r7,#0x20]
  00402508: F3BF 8F5B dmb         ish
  0040250C: 6A3B      ldr         r3,[r7,#0x20]
  0040250E: 2B01      cmp         r3,#1
  00402510: D103      bne         0040251A
  00402512: 201F      movs        r0,#0x1F
  00402514: F000 FA62 bl          004029DC
  00402518: E01D      b           00402556
  0040251A: 4B67      ldr         r3,004026B8
  0040251E: 63BB      str         r3,[r7,#0x38]
  00402520: F3BF 8F5B dmb         ish
  00402524: 6BBB      ldr         r3,[r7,#0x38]
  00402526: 2B00      cmp         r3,#0
  00402528: D112      bne         00402550
  0040252A: F3BF 8F5B dmb         ish
  0040252E: 4A62      ldr         r2,004026B8
  00402530: 2301      movs        r3,#1
  00402532: 6013      str         r3,[r2]
  00402534: 4964      ldr         r1,004026C8
  00402536: 4863      ldr         r0,004026C4
  00402538: F000 FD22 bl          00402F80
  0040253C: 6178      str         r0,[r7,#0x14]
  0040253E: 697B      ldr         r3,[r7,#0x14]
  00402540: 2B00      cmp         r3,#0
  00402542: D004      beq         0040254E
  00402544: 23FF      movs        r3,#0xFF
  00402546: 64BB      str         r3,[r7,#0x48]
  00402548: 6CBB      ldr         r3,[r7,#0x48]
  0040254A: 613B      str         r3,[r7,#0x10]
  0040254C: E08D      b           0040266A
  0040254E: E002      b           00402556
  00402550: 4A4A      ldr         r2,0040267C
  00402552: 2301      movs        r3,#1
  00402554: 6013      str         r3,[r2]
  00402556: 4B58      ldr         r3,004026B8
  0040255A: 633B      str         r3,[r7,#0x30]
  0040255C: F3BF 8F5B dmb         ish
  00402560: 6B3B      ldr         r3,[r7,#0x30]
  00402562: 2B01      cmp         r3,#1
  00402564: D108      bne         00402578
  00402566: 4956      ldr         r1,004026C0
  00402568: 4854      ldr         r0,004026BC
  0040256A: F000 FD0F bl          00402F8C
  0040256E: F3BF 8F5B dmb         ish
  00402572: 4A51      ldr         r2,004026B8
  00402574: 2302      movs        r3,#2
  00402576: 6013      str         r3,[r2]
  00402578: 4B4F      ldr         r3,004026B8
  0040257C: 61BB      str         r3,[r7,#0x18]
  0040257E: F3BF 8F5B dmb         ish
  00402582: 69BB      ldr         r3,[r7,#0x18]
  00402584: 2B02      cmp         r3,#2
  00402586: D010      beq         004025AA
  00402588: 4B4A      ldr         r3,004026B4
  0040258A: 9301      str         r3,[sp,#4]
  0040258C: 4B48      ldr         r3,004026B0
  0040258E: 9300      str         r3,[sp]
  00402590: 2300      movs        r3,#0
  00402592: F240 2229 mov         r2,#0x229
  00402596: 4945      ldr         r1,004026AC
  00402598: 2002      movs        r0,#2
  0040259A: 4C43      ldr         r4,004026A8
  0040259E: 47A0      blx         r4
  004025A0: 61F8      str         r0,[r7,#0x1C]
  004025A2: 69FB      ldr         r3,[r7,#0x1C]
  004025A4: 2B01      cmp         r3,#1
  004025A6: D100      bne         004025AA
  004025A8: DEFE      __debugbreak
  004025AA: 68FB      ldr         r3,[r7,#0xC]
  004025AC: 2B00      cmp         r3,#0
  004025AE: D10B      bne         004025C8
  004025B0: F3BF 8F5B dmb         ish
  004025B4: 483B      ldr         r0,004026A4
  004025B6: 2100      movs        r1,#0
  004025B8: E850 2F00 ldrex       r2,[r0]
  004025BC: E840 1300 strex       r3,r1,[r0]
  004025C0: 2B00      cmp         r3,#0
  004025C2: D1F9      bne         004025B8
  004025C4: F3BF 8F5B dmb         ish
  004025C8: 4B35      ldr         r3,004026A0
  004025CC: 2B00      cmp         r3,#0
  004025CE: D00C      beq         004025EA
  004025D0: 4833      ldr         r0,004026A0
  004025D2: F000 FA6B bl          00402AAC
  004025D6: 6278      str         r0,[r7,#0x24]
  004025D8: 6A7B      ldr         r3,[r7,#0x24]
  004025DA: 2B00      cmp         r3,#0
  004025DC: D005      beq         004025EA
  004025DE: 2200      movs        r2,#0
  004025E0: 2102      movs        r1,#2
  004025E2: 2000      movs        r0,#0
  004025E4: 4B2E      ldr         r3,004026A0
  004025E8: 4798      blx         r3
  004025EA: 2001      movs        r0,#1
  004025EC: 4B2B      ldr         r3,0040269C
  004025F0: 4798      blx         r3
  004025F2: 4B28      ldr         r3,00402694
  004025F6: 4B28      ldr         r3,00402698
  004025FA: 601A      str         r2,[r3]
  004025FC: 4B25      ldr         r3,00402694
  00402600: 4B23      ldr         r3,00402690
  00402604: 4B21      ldr         r3,0040268C
  00402608: F7FE FD06 bl          00401018
  0040260C: 62F8      str         r0,[r7,#0x2C]
  0040260E: 6AFA      ldr         r2,[r7,#0x2C]
  00402610: 4B18      ldr         r3,00402674
  00402612: 601A      str         r2,[r3]
  00402614: 4B1B      ldr         r3,00402684
  00402618: 2B00      cmp         r3,#0
  0040261A: D104      bne         00402626
  0040261C: 4B15      ldr         r3,00402674
  00402620: 4B19      ldr         r3,00402688
  00402624: 4798      blx         r3
  00402626: 4B15      ldr         r3,0040267C
  0040262A: 2B00      cmp         r3,#0
  0040262C: D102      bne         00402634
  0040262E: 4B12      ldr         r3,00402678
  00402632: 4798      blx         r3
  00402634: E016      b           00402664
  00402636: 6038      str         r0,[r7]
  0040263A: 64FB      str         r3,[r7,#0x4C]
  0040263C: 6CFA      ldr         r2,[r7,#0x4C]
  0040263E: 4B0D      ldr         r3,00402674
  00402640: 601A      str         r2,[r3]
  00402642: 4B10      ldr         r3,00402684
  00402646: 2B00      cmp         r3,#0
  00402648: D104      bne         00402654
  0040264A: 4B0A      ldr         r3,00402674
  0040264E: 4B0C      ldr         r3,00402680
  00402652: 4798      blx         r3
  00402654: 4B09      ldr         r3,0040267C
  00402658: 2B00      cmp         r3,#0
  0040265A: D102      bne         00402662
  0040265C: 4B06      ldr         r3,00402678
  00402660: 4798      blx         r3
  00402662: E7FF      b           00402664
  00402664: 4B03      ldr         r3,00402674
  00402668: 613B      str         r3,[r7,#0x10]
  0040266A: 6938      ldr         r0,[r7,#0x10]
  0040266C: B016      add         sp,sp,#0x58
  0040266E: E8BD 8890 pop         {r4,r7,r11,pc}
  00402672: DEFE      __debugbreak
  00402674: A138      adr         r1,00402758
  00402676: 0040      lsls        r0,r0,#1
  00402678: C0DC      stm         r0!,{r2-r4,r6,r7}
  0040267A: 0040      lsls        r0,r0,#1
  0040267C: A134      adr         r1,00402750
  0040267E: 0040      lsls        r0,r0,#1
  00402680: C0E0      stm         r0!,{r5-r7}
  00402682: 0040      lsls        r0,r0,#1
  00402684: A13C      adr         r1,00402778
  00402686: 0040      lsls        r0,r0,#1
  00402688: C0E4      stm         r0!,{r2,r5-r7}
  0040268A: 0040      lsls        r0,r0,#1
  0040268C: A140      adr         r1,00402790
  0040268E: 0040      lsls        r0,r0,#1
  00402690: A144      adr         r1,004027A4
  00402692: 0040      lsls        r0,r0,#1
  00402694: A148      adr         r1,004027B8
  00402696: 0040      lsls        r0,r0,#1
  00402698: C0C4      stm         r0!,{r2,r6,r7}
  0040269A: 0040      lsls        r0,r0,#1
  0040269C: C0E8      stm         r0!,{r3,r5-r7}
  0040269E: 0040      lsls        r0,r0,#1
  004026A0: A390      adr         r3,004028E4
  004026A2: 0040      lsls        r0,r0,#1
  004026A4: A368      adr         r3,00402848
  004026A6: 0040      lsls        r0,r0,#1
  004026A8: C0EC      stm         r0!,{r2,r3,r5-r7}
  004026AA: 0040      lsls        r0,r0,#1
  004026AC: 78B8      ldrb        r0,[r7,#2]
  004026AE: 0040      lsls        r0,r0,#1
  004026B0: 78B0      ldrb        r0,[r6,#2]
  004026B2: 0040      lsls        r0,r0,#1
  004026B4: 7860      ldrb        r0,[r4,#1]
  004026B6: 0040      lsls        r0,r0,#1
  004026B8: A378      adr         r3,0040289C
  004026BA: 0040      lsls        r0,r0,#1
  004026BC: 7000      strb        r0,[r0]
  004026BE: 0040      lsls        r0,r0,#1
  004026C0: 7208      strb        r0,[r1,#8]
  004026C2: 0040      lsls        r0,r0,#1
  004026C4: 730C      strb        r4,[r1,#0xC]
  004026C6: 0040      lsls        r0,r0,#1
  004026C8: 771C      strb        r4,[r3,#0x1C]
  004026CA: 0040      lsls        r0,r0,#1
  004026CC: 0000      movs        r0,r0
  004026CE: 0000      movs        r0,r0
  004026D0: 0000      movs        r0,r0
  004026D2: 0000      movs        r0,r0
  004026D4: 0000      movs        r0,r0
  004026D6: 0000      movs        r0,r0
  004026D8: 0000      movs        r0,r0
  004026DA: 0000      movs        r0,r0
  004026DC: 0000      movs        r0,r0
  004026DE: 0000      movs        r0,r0
  004026E0: 0000      movs        r0,r0
  004026E2: 0000      movs        r0,r0
  004026E4: 0000      movs        r0,r0
  004026E6: 0000      movs        r0,r0
  004026E8: 0000      movs        r0,r0
  004026EA: 0000      movs        r0,r0
  004026EC: 0000      movs        r0,r0
  004026EE: 0000      movs        r0,r0
  004026F0: 0000      movs        r0,r0
  004026F2: 0000      movs        r0,r0
  004026F4: 0000      movs        r0,r0
  004026F6: 0000      movs        r0,r0
  004026F8: 0000      movs        r0,r0
  004026FA: 0000      movs        r0,r0
  004026FC: 0000      movs        r0,r0
  004026FE: 0000      movs        r0,r0
  00402700: 0000      movs        r0,r0
  00402702: 0000      movs        r0,r0
  00402704: 0000      movs        r0,r0
  00402706: 0000      movs        r0,r0
  00402708: 0000      movs        r0,r0
  0040270A: 0000      movs        r0,r0
  0040270C: 0000      movs        r0,r0
  0040270E: 0000      movs        r0,r0
  00402710: 0000      movs        r0,r0
  00402712: 0000      movs        r0,r0
  00402714: 0000      movs        r0,r0
  00402716: 0000      movs        r0,r0
  00402718: 0000      movs        r0,r0
  0040271A: 0000      movs        r0,r0
  0040271C: 0000      movs        r0,r0
  0040271E: 0000      movs        r0,r0
  00402720: 0000      movs        r0,r0
  00402722: 0000      movs        r0,r0
  00402724: 0000      movs        r0,r0
  00402726: 0000      movs        r0,r0
  00402728: 0000      movs        r0,r0
  0040272A: 0000      movs        r0,r0
  0040272C: 0000      movs        r0,r0
  0040272E: 0000      movs        r0,r0
  00402730: 0000      movs        r0,r0
  00402732: 0000      movs        r0,r0
  00402734: 0000      movs        r0,r0
  00402736: 0000      movs        r0,r0
  00402738: 0000      movs        r0,r0
  0040273A: 0000      movs        r0,r0
  0040273C: 0000      movs        r0,r0
  0040273E: 0000      movs        r0,r0
  00402740: 0000      movs        r0,r0
  00402742: 0000      movs        r0,r0
  00402744: 0000      movs        r0,r0
  00402746: 0000      movs        r0,r0
  00402748: 0000      movs        r0,r0
  0040274A: 0000      movs        r0,r0
  0040274C: 0000      movs        r0,r0
  0040274E: 0000      movs        r0,r0
  00402750: 0000      movs        r0,r0
  00402752: 0000      movs        r0,r0
  00402754: 0000      movs        r0,r0
  00402756: 0000      movs        r0,r0
  00402758: 0000      movs        r0,r0
  0040275A: 0000      movs        r0,r0
  0040275C: E92D 4800 push        {r11,lr}
  00402760: 46EB      mov         r11,sp
  00402762: B084      sub         sp,sp,#0x10
  00402764: 4B1E      ldr         r3,004027E0
  00402766: 9302      str         r3,[sp,#8]
  00402768: 9B02      ldr         r3,[sp,#8]
  0040276A: 881A      ldrh        r2,[r3]
  0040276C: F645 234D mov         r3,#0x5A4D
  00402770: 429A      cmp         r2,r3
  00402772: D002      beq         0040277A
  00402774: 2300      movs        r3,#0
  00402776: 9300      str         r3,[sp]
  00402778: E02D      b           004027D6
  0040277A: 9B02      ldr         r3,[sp,#8]
  0040277C: 333C      adds        r3,r3,#0x3C
  0040277E: 9A02      ldr         r2,[sp,#8]
  00402782: 4413      add         r3,r3,r2
  00402784: 9301      str         r3,[sp,#4]
  00402786: 9B01      ldr         r3,[sp,#4]
  0040278A: F244 5350 mov         r3,#0x4550
  0040278E: 429A      cmp         r2,r3
  00402790: D002      beq         00402798
  00402792: 2300      movs        r3,#0
  00402794: 9300      str         r3,[sp]
  00402796: E01E      b           004027D6
  00402798: 9B01      ldr         r3,[sp,#4]
  0040279A: 3318      adds        r3,r3,#0x18
  0040279C: 881A      ldrh        r2,[r3]
  0040279E: F240 130B mov         r3,#0x10B
  004027A2: 429A      cmp         r2,r3
  004027A4: D002      beq         004027AC
  004027A6: 2300      movs        r3,#0
  004027A8: 9300      str         r3,[sp]
  004027AA: E014      b           004027D6
  004027AC: 9B01      ldr         r3,[sp,#4]
  004027AE: 3374      adds        r3,r3,#0x74
  004027B2: 2B0E      cmp         r3,#0xE
  004027B4: D802      bhi         004027BC
  004027B6: 2300      movs        r3,#0
  004027B8: 9300      str         r3,[sp]
  004027BA: E00C      b           004027D6
  004027BC: 9B01      ldr         r3,[sp,#4]
  004027BE: 3378      adds        r3,r3,#0x78
  004027C0: 3370      adds        r3,r3,#0x70
  004027C4: 2B00      cmp         r3,#0
  004027C6: D002      beq         004027CE
  004027C8: 2301      movs        r3,#1
  004027CA: 9303      str         r3,[sp,#0xC]
  004027CC: E001      b           004027D2
  004027CE: 2300      movs        r3,#0
  004027D0: 9303      str         r3,[sp,#0xC]
  004027D2: 9B03      ldr         r3,[sp,#0xC]
  004027D4: 9300      str         r3,[sp]
  004027D6: 9800      ldr         r0,[sp]
  004027D8: B004      add         sp,sp,#0x10
  004027DA: E8BD 8800 pop         {r11,pc}
  004027DE: DEFE      __debugbreak
  004027E0: 0000      movs        r0,r0
  004027E2: 0040      lsls        r0,r0,#1
  004027E4: E92D 4800 push        {r11,lr}
  004027E8: 46EB      mov         r11,sp
  004027EA: B082      sub         sp,sp,#8
  004027EC: F000 FA04 bl          00402BF8
  004027F0: F7FF FE58 bl          004024A4
  004027F4: 9000      str         r0,[sp]
  004027F6: 9B00      ldr         r3,[sp]
  004027F8: 9301      str         r3,[sp,#4]
  004027FA: 9801      ldr         r0,[sp,#4]
  004027FC: B002      add         sp,sp,#8
  004027FE: E8BD 8800 pop         {r11,pc}
  00402802: 0000      movs        r0,r0
  00402804: 0000      movs        r0,r0
  00402806: 0000      movs        r0,r0
  00402808: 0000      movs        r0,r0
  0040280A: 0000      movs        r0,r0
  0040280C: 0000      movs        r0,r0
  0040280E: 0000      movs        r0,r0
  00402810: 0000      movs        r0,r0
  00402812: 0000      movs        r0,r0
  00402814: 0000      movs        r0,r0
  00402816: 0000      movs        r0,r0
  00402818: 0000      movs        r0,r0
  0040281A: 0000      movs        r0,r0
  0040281C: 0000      movs        r0,r0
  0040281E: 0000      movs        r0,r0
  00402820: 0000      movs        r0,r0
  00402822: 0000      movs        r0,r0
  00402824: 0000      movs        r0,r0
  00402826: 0000      movs        r0,r0
  00402828: 0000      movs        r0,r0
  0040282A: 0000      movs        r0,r0
  0040282C: 0000      movs        r0,r0
  0040282E: 0000      movs        r0,r0
  00402830: 0000      movs        r0,r0
  00402832: 0000      movs        r0,r0
  00402834: E92D 4800 push        {r11,lr}
  00402838: 46EB      mov         r11,sp
  0040283A: F24A 1354 mov         r3,#0xA154
  0040283E: F2C0 0340 movt        r3,#0x40
  00402844: E8BD 8800 pop         {r11,pc}
  00402848: E92D 4800 push        {r11,lr}
  0040284C: 46EB      mov         r11,sp
  0040284E: F24A 1358 mov         r3,#0xA158
  00402852: F2C0 0340 movt        r3,#0x40
  00402858: E8BD 8800 pop         {r11,pc}
  0040285C: E92D 4800 push        {r11,lr}
  00402860: 46EB      mov         r11,sp
  00402862: 2804      cmp         r0,#4
  00402864: D807      bhi         00402876
  00402866: F647 1310 mov         r3,#0x7910
  0040286A: F2C0 0340 movt        r3,#0x40
  0040286E: F853 0020 ldr         r0,[r3,r0,lsl #2]
  00402872: E8BD 8800 pop         {r11,pc}
  00402876: 2000      movs        r0,#0
  00402878: E8BD 8800 pop         {r11,pc}
  0040287C: 0000      movs        r0,r0
  0040287E: 0000      movs        r0,r0
  00402880: E92D 4800 push        {r11,lr}
  00402884: 46EB      mov         r11,sp
  00402886: 2005      movs        r0,#5
  00402888: E8BD 8800 pop         {r11,pc}
  0040288C: E92D 4800 push        {r11,lr}
  00402890: 46EB      mov         r11,sp
  00402892: F24A 1254 mov         r2,#0xA154
  00402896: F2C0 0240 movt        r2,#0x40
  0040289A: 4603      mov         r3,r0
  0040289E: 6013      str         r3,[r2]
  004028A0: 2300      movs        r3,#0
  004028A2: 6053      str         r3,[r2,#4]
  004028A4: E8BD 8800 pop         {r11,pc}
  004028A8: E92D 4800 push        {r11,lr}
  004028AC: 46EB      mov         r11,sp
  004028AE: F24A 1254 mov         r2,#0xA154
  004028B2: F2C0 0240 movt        r2,#0x40
  004028B6: 4603      mov         r3,r0
  004028B8: 6850      ldr         r0,[r2,#4]
  004028BA: 6053      str         r3,[r2,#4]
  004028BC: 2300      movs        r3,#0
  004028BE: 6013      str         r3,[r2]
  004028C0: E8BD 8800 pop         {r11,pc}
  004028C4: E92D 4800 push        {r11,lr}
  004028C8: 46EB      mov         r11,sp
  004028CA: 4602      mov         r2,r0
  004028CC: 2A04      cmp         r2,#4
  004028CE: D809      bhi         004028E4
  004028D0: F24A 030C mov         r3,#0xA00C
  004028D4: F2C0 0340 movt        r3,#0x40
  004028D8: F853 0020 ldr         r0,[r3,r0,lsl #2]
  004028DC: F843 1022 str         r1,[r3,r2,lsl #2]
  004028E0: E8BD 8800 pop         {r11,pc}
  004028E4: F06F 0000 mvn         r0,#0
  004028E8: E8BD 8800 pop         {r11,pc}
  004028EC: F24C 1C00 movw        r12,#0xC100
  004028F0: F2C0 0C40 movt        r12,#0x40
  004028F4: F8DC F000 ldr         pc,[r12]
  004028F8: B403      push        {r0,r1}
  004028FA: E92D 4800 push        {r11,lr}
  004028FE: 46EB      mov         r11,sp
  00402900: B082      sub         sp,sp,#8
  00402902: 9B04      ldr         r3,[sp,#0x10]
  00402908: 4B1C      ldr         r3,0040297C
  0040290A: 429A      cmp         r2,r3
  0040290C: D126      bne         0040295C
  0040290E: 9B04      ldr         r3,[sp,#0x10]
  00402912: 3310      adds        r3,r3,#0x10
  00402916: 2B04      cmp         r3,#4
  00402918: D120      bne         0040295C
  0040291A: 9B04      ldr         r3,[sp,#0x10]
  0040291E: 3314      adds        r3,r3,#0x14
  00402922: 4B15      ldr         r3,00402978
  00402924: 429A      cmp         r2,r3
  00402926: D014      beq         00402952
  00402928: 9B04      ldr         r3,[sp,#0x10]
  0040292C: 3314      adds        r3,r3,#0x14
  00402930: 4B10      ldr         r3,00402974
  00402932: 429A      cmp         r2,r3
  00402934: D00D      beq         00402952
  00402936: 9B04      ldr         r3,[sp,#0x10]
  0040293A: 3314      adds        r3,r3,#0x14
  0040293E: 4B0C      ldr         r3,00402970
  00402940: 429A      cmp         r2,r3
  00402942: D006      beq         00402952
  00402944: 9B04      ldr         r3,[sp,#0x10]
  00402948: 3314      adds        r3,r3,#0x14
  0040294C: 4B07      ldr         r3,0040296C
  0040294E: 429A      cmp         r2,r3
  00402950: D104      bne         0040295C
  00402952: F000 FED1 bl          004036F8
  00402956: 2301      movs        r3,#1
  00402958: 9300      str         r3,[sp]
  0040295A: E001      b           00402960
  0040295C: 2300      movs        r3,#0
  0040295E: 9300      str         r3,[sp]
  00402960: 9800      ldr         r0,[sp]
  00402962: B002      add         sp,sp,#8
  00402964: F85D BB04 pop         {r11}
  00402968: F85D FB0C ldr         pc,[sp],#0xC
  0040296C: 4000      ands        r0,r0,r0
  0040296E: 0199      lsls        r1,r3,#6
  00402970: 0522      lsls        r2,r4,#0x14
  00402972: 1993      adds        r3,r2,r6
  00402974: 0521      lsls        r1,r4,#0x14
  00402976: 1993      adds        r3,r2,r6
  00402978: 0520      lsls        r0,r4,#0x14
  0040297A: 1993      adds        r3,r2,r6
  0040297C: 7363      strb        r3,[r4,#0xD]
  0040297E: E06D      b           00402A5C
  00402980: 0000      movs        r0,r0
  00402982: 0000      movs        r0,r0
  00402984: 0000      movs        r0,r0
  00402986: 0000      movs        r0,r0
  00402988: 0000      movs        r0,r0
  0040298A: 0000      movs        r0,r0
  0040298C: 0000      movs        r0,r0
  0040298E: 0000      movs        r0,r0
  00402990: 0000      movs        r0,r0
  00402992: 0000      movs        r0,r0
  00402994: 0000      movs        r0,r0
  00402996: 0000      movs        r0,r0
  00402998: E92D 4800 push        {r11,lr}
  0040299C: 46EB      mov         r11,sp
  0040299E: B082      sub         sp,sp,#8
  004029A0: 4804      ldr         r0,004029B4
  004029A2: F000 FEAF bl          00403704
  004029A6: 2300      movs        r3,#0
  004029A8: 9300      str         r3,[sp]
  004029AA: 9800      ldr         r0,[sp]
  004029AC: B002      add         sp,sp,#8
  004029AE: E8BD 8800 pop         {r11,pc}
  004029B2: DEFE      __debugbreak
  004029B4: 28F9      cmp         r0,#0xF9
  004029B6: 0040      lsls        r0,r0,#1
  004029B8: 0000      movs        r0,r0
  004029BA: 0000      movs        r0,r0
  004029BC: 0000      movs        r0,r0
  004029BE: 0000      movs        r0,r0
  004029C0: 0000      movs        r0,r0
  004029C2: 0000      movs        r0,r0
  004029C4: 0000      movs        r0,r0
  004029C6: 0000      movs        r0,r0
  004029C8: 0000      movs        r0,r0
  004029CA: 0000      movs        r0,r0
  004029CC: 0000      movs        r0,r0
  004029CE: 0000      movs        r0,r0
  004029D0: F24C 0CFC mov         r12,#0xC0FC
  004029D4: F2C0 0C40 movt        r12,#0x40
  004029D8: F8DC F000 ldr         pc,[r12]
  004029DC: F24C 0CF8 mov         r12,#0xC0F8
  004029E0: F2C0 0C40 movt        r12,#0x40
  004029E4: F8DC F000 ldr         pc,[r12]
  004029E8: E92D 4800 push        {r11,lr}
  004029EC: 46EB      mov         r11,sp
  004029EE: B082      sub         sp,sp,#8
  004029F0: 2300      movs        r3,#0
  004029F2: 9300      str         r3,[sp]
  004029F4: 9800      ldr         r0,[sp]
  004029F6: B002      add         sp,sp,#8
  004029F8: E8BD 8800 pop         {r11,pc}
  004029FC: F24C 0CF4 mov         r12,#0xC0F4
  00402A00: F2C0 0C40 movt        r12,#0x40
  00402A04: F8DC F000 ldr         pc,[r12]
  00402A08: F24C 0CF0 mov         r12,#0xC0F0
  00402A0C: F2C0 0C40 movt        r12,#0x40
  00402A10: F8DC F000 ldr         pc,[r12]
  00402A14: F24C 0CEC mov         r12,#0xC0EC
  00402A18: F2C0 0C40 movt        r12,#0x40
  00402A1C: F8DC F000 ldr         pc,[r12]
  00402A20: F24C 0CE8 mov         r12,#0xC0E8
  00402A24: F2C0 0C40 movt        r12,#0x40
  00402A28: F8DC F000 ldr         pc,[r12]
  00402A2C: B403      push        {r0,r1}
  00402A2E: E92D 4800 push        {r11,lr}
  00402A32: 46EB      mov         r11,sp
  00402A34: B084      sub         sp,sp,#0x10
  00402A36: 9B06      ldr         r3,[sp,#0x18]
  00402A38: 333C      adds        r3,r3,#0x3C
  00402A3A: 9A06      ldr         r2,[sp,#0x18]
  00402A3E: 4413      add         r3,r3,r2
  00402A40: 9301      str         r3,[sp,#4]
  00402A42: 2300      movs        r3,#0
  00402A44: 9302      str         r3,[sp,#8]
  00402A46: 9B01      ldr         r3,[sp,#4]
  00402A48: F103 0218 add         r2,r3,#0x18
  00402A4C: 9B01      ldr         r3,[sp,#4]
  00402A4E: 3314      adds        r3,r3,#0x14
  00402A50: 881B      ldrh        r3,[r3]
  00402A52: 4413      add         r3,r3,r2
  00402A54: 9300      str         r3,[sp]
  00402A56: E005      b           00402A64
  00402A58: 9B02      ldr         r3,[sp,#8]
  00402A5A: 1C5B      adds        r3,r3,#1
  00402A5C: 9302      str         r3,[sp,#8]
  00402A5E: 9B00      ldr         r3,[sp]
  00402A60: 3328      adds        r3,r3,#0x28
  00402A62: 9300      str         r3,[sp]
  00402A64: 9B01      ldr         r3,[sp,#4]
  00402A66: 1D9B      adds        r3,r3,#6
  00402A68: 881A      ldrh        r2,[r3]
  00402A6A: 9B02      ldr         r3,[sp,#8]
  00402A6C: 4293      cmp         r3,r2
  00402A6E: D214      bcs         00402A9A
  00402A70: 9B00      ldr         r3,[sp]
  00402A72: 330C      adds        r3,r3,#0xC
  00402A74: 9A07      ldr         r2,[sp,#0x1C]
  00402A78: 429A      cmp         r2,r3
  00402A7A: D30D      bcc         00402A98
  00402A7C: 9B00      ldr         r3,[sp]
  00402A7E: F103 020C add         r2,r3,#0xC
  00402A82: 9B00      ldr         r3,[sp]
  00402A84: 3308      adds        r3,r3,#8
  00402A8A: 441A      add         r2,r2,r3
  00402A8C: 9B07      ldr         r3,[sp,#0x1C]
  00402A8E: 4293      cmp         r3,r2
  00402A90: D202      bcs         00402A98
  00402A92: 9B00      ldr         r3,[sp]
  00402A94: 9303      str         r3,[sp,#0xC]
  00402A96: E002      b           00402A9E
  00402A98: E7DE      b           00402A58
  00402A9A: 2300      movs        r3,#0
  00402A9C: 9303      str         r3,[sp,#0xC]
  00402A9E: 9803      ldr         r0,[sp,#0xC]
  00402AA0: B004      add         sp,sp,#0x10
  00402AA2: F85D BB04 pop         {r11}
  00402AA6: F85D FB0C ldr         pc,[sp],#0xC
  00402AAA: 0000      movs        r0,r0
  00402AAC: B403      push        {r0,r1}
  00402AAE: E92D 4880 push        {r7,r11,lr}
  00402AB2: F10D 0B04 add         r11,sp,#4
  00402AB6: B08F      sub         sp,sp,#0x3C
  00402AB8: 466F      mov         r7,sp
  00402ABA: 4B1F      ldr         r3,00402B38
  00402ABC: 607B      str         r3,[r7,#4]
  00402ABE: 6878      ldr         r0,[r7,#4]
  00402AC0: F000 F854 bl          00402B6C
  00402AC4: 6178      str         r0,[r7,#0x14]
  00402AC6: 697B      ldr         r3,[r7,#0x14]
  00402AC8: 2B00      cmp         r3,#0
  00402ACA: D104      bne         00402AD6
  00402ACC: 2300      movs        r3,#0
  00402ACE: 61BB      str         r3,[r7,#0x18]
  00402AD0: 69BB      ldr         r3,[r7,#0x18]
  00402AD2: 603B      str         r3,[r7]
  00402AD4: E029      b           00402B2A
  00402AD6: 6CBA      ldr         r2,[r7,#0x48]
  00402AD8: 687B      ldr         r3,[r7,#4]
  00402ADA: 1AD3      subs        r3,r2,r3
  00402ADC: 61FB      str         r3,[r7,#0x1C]
  00402ADE: 69F9      ldr         r1,[r7,#0x1C]
  00402AE0: 6878      ldr         r0,[r7,#4]
  00402AE2: F7FF FFA3 bl          00402A2C
  00402AE6: 6238      str         r0,[r7,#0x20]
  00402AE8: 6A3B      ldr         r3,[r7,#0x20]
  00402AEA: 60BB      str         r3,[r7,#8]
  00402AEC: 68BB      ldr         r3,[r7,#8]
  00402AEE: 2B00      cmp         r3,#0
  00402AF0: D104      bne         00402AFC
  00402AF2: 2300      movs        r3,#0
  00402AF4: 627B      str         r3,[r7,#0x24]
  00402AF6: 6A7B      ldr         r3,[r7,#0x24]
  00402AF8: 603B      str         r3,[r7]
  00402AFA: E016      b           00402B2A
  00402AFC: 68BB      ldr         r3,[r7,#8]
  00402AFE: 3324      adds        r3,r3,#0x24
  00402B02: F013 4F00 tst         r3,#0x80000000
  00402B06: D102      bne         00402B0E
  00402B08: 2301      movs        r3,#1
  00402B0A: 60FB      str         r3,[r7,#0xC]
  00402B0C: E001      b           00402B12
  00402B0E: 2300      movs        r3,#0
  00402B10: 60FB      str         r3,[r7,#0xC]
  00402B12: 68FB      ldr         r3,[r7,#0xC]
  00402B14: 62BB      str         r3,[r7,#0x28]
  00402B16: 6ABB      ldr         r3,[r7,#0x28]
  00402B18: 603B      str         r3,[r7]
  00402B1A: E006      b           00402B2A
  00402B1C: E005      b           00402B2A
  00402B1E: 2300      movs        r3,#0
  00402B20: 637B      str         r3,[r7,#0x34]
  00402B22: 6B7B      ldr         r3,[r7,#0x34]
  00402B24: 603B      str         r3,[r7]
  00402B26: E000      b           00402B2A
  00402B28: E7FF      b           00402B2A
  00402B2C: B00F      add         sp,sp,#0x3C
  00402B2E: E8BD 0880 pop         {r7,r11}
  00402B32: F85D FB0C ldr         pc,[sp],#0xC
  00402B36: DEFE      __debugbreak
  00402B38: 0000      movs        r0,r0
  00402B3A: 0040      lsls        r0,r0,#1
  00402B3C: 0000      movs        r0,r0
  00402B3E: 0000      movs        r0,r0
  00402B40: 0000      movs        r0,r0
  00402B42: 0000      movs        r0,r0
  00402B44: 0000      movs        r0,r0
  00402B46: 0000      movs        r0,r0
  00402B48: 0000      movs        r0,r0
  00402B4A: 0000      movs        r0,r0
  00402B4C: 0000      movs        r0,r0
  00402B4E: 0000      movs        r0,r0
  00402B50: 0000      movs        r0,r0
  00402B52: 0000      movs        r0,r0
  00402B54: 0000      movs        r0,r0
  00402B56: 0000      movs        r0,r0
  00402B58: 0000      movs        r0,r0
  00402B5A: 0000      movs        r0,r0
  00402B5C: 0000      movs        r0,r0
  00402B5E: 0000      movs        r0,r0
  00402B60: 0000      movs        r0,r0
  00402B62: 0000      movs        r0,r0
  00402B64: 0000      movs        r0,r0
  00402B66: 0000      movs        r0,r0
  00402B68: 0000      movs        r0,r0
  00402B6A: 0000      movs        r0,r0
  00402B6C: B403      push        {r0,r1}
  00402B6E: E92D 4800 push        {r11,lr}
  00402B72: 46EB      mov         r11,sp
  00402B74: B084      sub         sp,sp,#0x10
  00402B76: 9B06      ldr         r3,[sp,#0x18]
  00402B78: 9301      str         r3,[sp,#4]
  00402B7A: 9B01      ldr         r3,[sp,#4]
  00402B7C: 881A      ldrh        r2,[r3]
  00402B7E: F645 234D mov         r3,#0x5A4D
  00402B82: 429A      cmp         r2,r3
  00402B84: D002      beq         00402B8C
  00402B86: 2300      movs        r3,#0
  00402B88: 9300      str         r3,[sp]
  00402B8A: E01C      b           00402BC6
  00402B8C: 9B01      ldr         r3,[sp,#4]
  00402B8E: 333C      adds        r3,r3,#0x3C
  00402B90: 9A01      ldr         r2,[sp,#4]
  00402B94: 4413      add         r3,r3,r2
  00402B96: 9302      str         r3,[sp,#8]
  00402B98: 9B02      ldr         r3,[sp,#8]
  00402B9C: F244 5350 mov         r3,#0x4550
  00402BA0: 429A      cmp         r2,r3
  00402BA2: D002      beq         00402BAA
  00402BA4: 2300      movs        r3,#0
  00402BA6: 9300      str         r3,[sp]
  00402BA8: E00D      b           00402BC6
  00402BAA: 9B02      ldr         r3,[sp,#8]
  00402BAC: 3318      adds        r3,r3,#0x18
  00402BAE: 9303      str         r3,[sp,#0xC]
  00402BB0: 9B03      ldr         r3,[sp,#0xC]
  00402BB2: 881A      ldrh        r2,[r3]
  00402BB4: F240 130B mov         r3,#0x10B
  00402BB8: 429A      cmp         r2,r3
  00402BBA: D002      beq         00402BC2
  00402BBC: 2300      movs        r3,#0
  00402BBE: 9300      str         r3,[sp]
  00402BC0: E001      b           00402BC6
  00402BC2: 2301      movs        r3,#1
  00402BC4: 9300      str         r3,[sp]
  00402BC6: 9800      ldr         r0,[sp]
  00402BC8: B004      add         sp,sp,#0x10
  00402BCA: F85D BB04 pop         {r11}
  00402BCE: F85D FB0C ldr         pc,[sp],#0xC
  00402BD2: 0000      movs        r0,r0
  00402BD4: F24C 0CE4 mov         r12,#0xC0E4
  00402BD8: F2C0 0C40 movt        r12,#0x40
  00402BDC: F8DC F000 ldr         pc,[r12]
  00402BE0: F24C 0CE0 mov         r12,#0xC0E0
  00402BE4: F2C0 0C40 movt        r12,#0x40
  00402BE8: F8DC F000 ldr         pc,[r12]
  00402BEC: F24C 0CDC mov         r12,#0xC0DC
  00402BF0: F2C0 0C40 movt        r12,#0x40
  00402BF4: F8DC F000 ldr         pc,[r12]
  00402BF8: E92D 4800 push        {r11,lr}
  00402BFC: 46EB      mov         r11,sp
  00402BFE: B08A      sub         sp,sp,#0x28
  00402C00: 2300      movs        r3,#0
  00402C02: 9304      str         r3,[sp,#0x10]
  00402C04: 2300      movs        r3,#0
  00402C06: 9305      str         r3,[sp,#0x14]
  00402C08: 4B27      ldr         r3,00402CA8
  00402C0C: 4B28      ldr         r3,00402CB0
  00402C0E: 429A      cmp         r2,r3
  00402C10: D005      beq         00402C1E
  00402C12: 4B25      ldr         r3,00402CA8
  00402C16: 43DA      mvns        r2,r3
  00402C18: 4B22      ldr         r3,00402CA4
  00402C1A: 601A      str         r2,[r3]
  00402C1C: E03F      b           00402C9E
  00402C1E: A804      add         r0,sp,#0x10
  00402C20: 4B28      ldr         r3,00402CC4
  00402C24: 4798      blx         r3
  00402C26: 9B04      ldr         r3,[sp,#0x10]
  00402C28: 9300      str         r3,[sp]
  00402C2A: 9A00      ldr         r2,[sp]
  00402C2C: 9B05      ldr         r3,[sp,#0x14]
  00402C2E: 4053      eors        r3,r3,r2
  00402C30: 9300      str         r3,[sp]
  00402C32: 4B23      ldr         r3,00402CC0
  00402C36: 4798      blx         r3
  00402C38: 9001      str         r0,[sp,#4]
  00402C3A: 9A00      ldr         r2,[sp]
  00402C3C: 9B01      ldr         r3,[sp,#4]
  00402C3E: 4053      eors        r3,r3,r2
  00402C40: 9300      str         r3,[sp]
  00402C42: 4B1E      ldr         r3,00402CBC
  00402C46: 4798      blx         r3
  00402C48: 9002      str         r0,[sp,#8]
  00402C4A: 9A00      ldr         r2,[sp]
  00402C4C: 9B02      ldr         r3,[sp,#8]
  00402C4E: 4053      eors        r3,r3,r2
  00402C50: 9300      str         r3,[sp]
  00402C52: 4B19      ldr         r3,00402CB8
  00402C56: 4798      blx         r3
  00402C58: 9107      str         r1,[sp,#0x1C]
  00402C5A: 9006      str         r0,[sp,#0x18]
  00402C5C: 9A06      ldr         r2,[sp,#0x18]
  00402C5E: 9B00      ldr         r3,[sp]
  00402C60: 4053      eors        r3,r3,r2
  00402C62: 9300      str         r3,[sp]
  00402C64: A808      add         r0,sp,#0x20
  00402C66: 4B13      ldr         r3,00402CB4
  00402C6A: 4798      blx         r3
  00402C6C: 9A00      ldr         r2,[sp]
  00402C6E: 9B08      ldr         r3,[sp,#0x20]
  00402C70: 4053      eors        r3,r3,r2
  00402C72: 9300      str         r3,[sp]
  00402C74: 9A00      ldr         r2,[sp]
  00402C76: 9B09      ldr         r3,[sp,#0x24]
  00402C78: 4053      eors        r3,r3,r2
  00402C7A: 9300      str         r3,[sp]
  00402C7C: 9A00      ldr         r2,[sp]
  00402C7E: 466B      mov         r3,sp
  00402C80: 4053      eors        r3,r3,r2
  00402C82: 9300      str         r3,[sp]
  00402C84: 9A00      ldr         r2,[sp]
  00402C86: 4B0A      ldr         r3,00402CB0
  00402C88: 429A      cmp         r2,r3
  00402C8A: D101      bne         00402C90
  00402C8C: 4B07      ldr         r3,00402CAC
  00402C8E: 9300      str         r3,[sp]
  00402C90: 9A00      ldr         r2,[sp]
  00402C92: 4B05      ldr         r3,00402CA8
  00402C94: 601A      str         r2,[r3]
  00402C96: 9B00      ldr         r3,[sp]
  00402C98: 43DA      mvns        r2,r3
  00402C9A: 4B02      ldr         r3,00402CA4
  00402C9C: 601A      str         r2,[r3]
  00402C9E: B00A      add         sp,sp,#0x28
  00402CA0: E8BD 8800 pop         {r11,pc}
  00402CA4: A024      adr         r0,00402D38
  00402CA6: 0040      lsls        r0,r0,#1
  00402CA8: A020      adr         r0,00402D2C
  00402CAA: 0040      lsls        r0,r0,#1
  00402CAC: E64F      b           0040294E
  00402CAE: BB40      cbnz        r0,00402D02
  00402CB0: E64E      b           00402950
  00402CB2: BB40      cbnz        r0,00402D06
  00402CB4: C048      stm         r0!,{r3,r6}
  00402CB6: 0040      lsls        r0,r0,#1
  00402CB8: C038      stm         r0!,{r3-r5}
  00402CBA: 0040      lsls        r0,r0,#1
  00402CBC: C044      stm         r0!,{r2,r6}
  00402CBE: 0040      lsls        r0,r0,#1
  00402CC0: C040      stm         r0!,{r6}
  00402CC2: 0040      lsls        r0,r0,#1
  00402CC4: C03C      stm         r0!,{r2-r5}
  00402CC6: 0040      lsls        r0,r0,#1
  00402CC8: B403      push        {r0,r1}
  00402CCA: E92D 4800 push        {r11,lr}
  00402CCE: 46EB      mov         r11,sp
  00402CD0: B082      sub         sp,sp,#8
  00402CD2: 2300      movs        r3,#0
  00402CD4: 9300      str         r3,[sp]
  00402CD6: 9800      ldr         r0,[sp]
  00402CD8: B002      add         sp,sp,#8
  00402CDA: F85D BB04 pop         {r11}
  00402CDE: F85D FB0C ldr         pc,[sp],#0xC
  00402CE2: 0000      movs        r0,r0
  00402CE4: E92D 4810 push        {r4,r11,lr}
  00402CE8: F10D 0B04 add         r11,sp,#4
  00402CEC: B087      sub         sp,sp,#0x1C
  00402CEE: 4B28      ldr         r3,00402D90
  00402CF2: 2B00      cmp         r3,#0
  00402CF4: D003      beq         00402CFE
  00402CF6: 4B25      ldr         r3,00402D8C
  00402CFA: 2B00      cmp         r3,#0
  00402CFC: D117      bne         00402D2E
  00402CFE: 4B24      ldr         r3,00402D90
  00402D02: 2B00      cmp         r3,#0
  00402D04: D103      bne         00402D0E
  00402D06: 4B21      ldr         r3,00402D8C
  00402D0A: 2B00      cmp         r3,#0
  00402D0C: D00F      beq         00402D2E
  00402D0E: 4B27      ldr         r3,00402DAC
  00402D10: 9301      str         r3,[sp,#4]
  00402D12: 4B25      ldr         r3,00402DA8
  00402D14: 9300      str         r3,[sp]
  00402D16: 2300      movs        r3,#0
  00402D18: 2245      movs        r2,#0x45
  00402D1A: 4922      ldr         r1,00402DA4
  00402D1C: 2002      movs        r0,#2
  00402D1E: 4C20      ldr         r4,00402DA0
  00402D22: 47A0      blx         r4
  00402D24: 9004      str         r0,[sp,#0x10]
  00402D26: 9B04      ldr         r3,[sp,#0x10]
  00402D28: 2B01      cmp         r3,#1
  00402D2A: D100      bne         00402D2E
  00402D2C: DEFE      __debugbreak
  00402D2E: 4B18      ldr         r3,00402D90
  00402D32: 2B00      cmp         r3,#0
  00402D34: D002      beq         00402D3C
  00402D36: 2300      movs        r3,#0
  00402D38: 9303      str         r3,[sp,#0xC]
  00402D3A: E022      b           00402D82
  00402D3C: 234D      movs        r3,#0x4D
  00402D3E: 9300      str         r3,[sp]
  00402D40: 4B16      ldr         r3,00402D9C
  00402D42: 2202      movs        r2,#2
  00402D44: 2104      movs        r1,#4
  00402D46: 2020      movs        r0,#0x20
  00402D48: 4C13      ldr         r4,00402D98
  00402D4C: 47A0      blx         r4
  00402D4E: 9005      str         r0,[sp,#0x14]
  00402D50: 9B05      ldr         r3,[sp,#0x14]
  00402D52: 9302      str         r3,[sp,#8]
  00402D54: 9802      ldr         r0,[sp,#8]
  00402D56: 4B0F      ldr         r3,00402D94
  00402D5A: 4798      blx         r3
  00402D5C: 9006      str         r0,[sp,#0x18]
  00402D5E: 9A06      ldr         r2,[sp,#0x18]
  00402D60: 4B0B      ldr         r3,00402D90
  00402D62: 601A      str         r2,[r3]
  00402D64: 4B0A      ldr         r3,00402D90
  00402D68: 4B08      ldr         r3,00402D8C
  00402D6A: 601A      str         r2,[r3]
  00402D6C: 9B02      ldr         r3,[sp,#8]
  00402D6E: 2B00      cmp         r3,#0
  00402D70: D102      bne         00402D78
  00402D72: 2318      movs        r3,#0x18
  00402D74: 9303      str         r3,[sp,#0xC]
  00402D76: E004      b           00402D82
  00402D78: 9A02      ldr         r2,[sp,#8]
  00402D7A: 2300      movs        r3,#0
  00402D7C: 6013      str         r3,[r2]
  00402D7E: 2300      movs        r3,#0
  00402D80: 9303      str         r3,[sp,#0xC]
  00402D82: 9803      ldr         r0,[sp,#0xC]
  00402D84: B007      add         sp,sp,#0x1C
  00402D86: E8BD 8810 pop         {r4,r11,pc}
  00402D8A: DEFE      __debugbreak
  00402D8C: A37C      adr         r3,00402F80
  00402D8E: 0040      lsls        r0,r0,#1
  00402D90: A38C      adr         r3,00402FC4
  00402D92: 0040      lsls        r0,r0,#1
  00402D94: C04C      stm         r0!,{r2,r3,r6}
  00402D96: 0040      lsls        r0,r0,#1
  00402D98: C0A0      stm         r0!,{r5,r7}
  00402D9A: 0040      lsls        r0,r0,#1
  00402D9C: 7B0C      ldrb        r4,[r1,#0xC]
  00402D9E: 0040      lsls        r0,r0,#1
  00402DA0: C0EC      stm         r0!,{r2,r3,r5-r7}
  00402DA2: 0040      lsls        r0,r0,#1
  00402DA4: 7AB0      ldrb        r0,[r6,#0xA]
  00402DA6: 0040      lsls        r0,r0,#1
  00402DA8: 78B0      ldrb        r0,[r6,#2]
  00402DAA: 0040      lsls        r0,r0,#1
  00402DAC: 79E8      ldrb        r0,[r5,#7]
  00402DAE: 0040      lsls        r0,r0,#1
  00402DB0: B403      push        {r0,r1}
  00402DB2: E92D 4880 push        {r7,r11,lr}
  00402DB6: F10D 0B04 add         r11,sp,#4
  00402DBA: B08D      sub         sp,sp,#0x34
  00402DBC: 466F      mov         r7,sp
  00402DBE: 2300      movs        r3,#0
  00402DC0: 60BB      str         r3,[r7,#8]
  00402DC2: 4B29      ldr         r3,00402E68
  00402DC6: 4B29      ldr         r3,00402E6C
  00402DCA: 4798      blx         r3
  00402DCC: 6138      str         r0,[r7,#0x10]
  00402DCE: 693B      ldr         r3,[r7,#0x10]
  00402DD0: 603B      str         r3,[r7]
  00402DD4: F1B3 3FFF cmp         r3,#0xFFFFFFFF
  00402DD8: D107      bne         00402DEA
  00402DDA: 6C38      ldr         r0,[r7,#0x40]
  00402DDC: 4B24      ldr         r3,00402E70
  00402DE0: 4798      blx         r3
  00402DE2: 6178      str         r0,[r7,#0x14]
  00402DE4: 697B      ldr         r3,[r7,#0x14]
  00402DE6: 60FB      str         r3,[r7,#0xC]
  00402DE8: E034      b           00402E54
  00402DEA: 2008      movs        r0,#8
  00402DEC: F000 FC90 bl          00403710
  00402DF0: 4B1D      ldr         r3,00402E68
  00402DF4: 4B1D      ldr         r3,00402E6C
  00402DF8: 4798      blx         r3
  00402DFA: 61B8      str         r0,[r7,#0x18]
  00402DFC: 69BB      ldr         r3,[r7,#0x18]
  00402DFE: 603B      str         r3,[r7]
  00402E00: 4B17      ldr         r3,00402E60
  00402E04: 4B19      ldr         r3,00402E6C
  00402E08: 4798      blx         r3
  00402E0A: 61F8      str         r0,[r7,#0x1C]
  00402E0C: 69FB      ldr         r3,[r7,#0x1C]
  00402E0E: 607B      str         r3,[r7,#4]
  00402E10: 6C38      ldr         r0,[r7,#0x40]
  00402E12: 4B14      ldr         r3,00402E64
  00402E16: 4798      blx         r3
  00402E18: 6238      str         r0,[r7,#0x20]
  00402E1A: 1D3A      adds        r2,r7,#4
  00402E1C: 4639      mov         r1,r7
  00402E1E: 6A38      ldr         r0,[r7,#0x20]
  00402E20: F000 FC88 bl          00403734
  00402E24: 6278      str         r0,[r7,#0x24]
  00402E26: 6A7B      ldr         r3,[r7,#0x24]
  00402E28: 60BB      str         r3,[r7,#8]
  00402E2C: 4B0D      ldr         r3,00402E64
  00402E30: 4798      blx         r3
  00402E32: 62B8      str         r0,[r7,#0x28]
  00402E34: 6ABA      ldr         r2,[r7,#0x28]
  00402E36: 4B0C      ldr         r3,00402E68
  00402E38: 601A      str         r2,[r3]
  00402E3A: 6878      ldr         r0,[r7,#4]
  00402E3C: 4B09      ldr         r3,00402E64
  00402E40: 4798      blx         r3
  00402E42: 62F8      str         r0,[r7,#0x2C]
  00402E44: 6AFA      ldr         r2,[r7,#0x2C]
  00402E46: 4B06      ldr         r3,00402E60
  00402E48: 601A      str         r2,[r3]
  00402E4A: 2008      movs        r0,#8
  00402E4C: F000 FC66 bl          0040371C
  00402E50: 68BB      ldr         r3,[r7,#8]
  00402E52: 60FB      str         r3,[r7,#0xC]
  00402E54: 68F8      ldr         r0,[r7,#0xC]
  00402E56: B00D      add         sp,sp,#0x34
  00402E58: E8BD 0880 pop         {r7,r11}
  00402E5C: F85D FB0C ldr         pc,[sp],#0xC
  00402E60: A37C      adr         r3,00403054
  00402E62: 0040      lsls        r0,r0,#1
  00402E64: C04C      stm         r0!,{r2,r3,r6}
  00402E66: 0040      lsls        r0,r0,#1
  00402E68: A38C      adr         r3,0040309C
  00402E6A: 0040      lsls        r0,r0,#1
  00402E6C: C034      stm         r0!,{r2,r4,r5}
  00402E6E: 0040      lsls        r0,r0,#1
  00402E70: C0A4      stm         r0!,{r2,r5,r7}
  00402E72: 0040      lsls        r0,r0,#1

  00402EBC: B403      push        {r0,r1}
  00402EBE: E92D 4800 push        {r11,lr}
  00402EC2: 46EB      mov         r11,sp
  00402EC4: B084      sub         sp,sp,#0x10
  00402EC6: 9806      ldr         r0,[sp,#0x18]
  00402EC8: F7FF FF72 bl          00402DB0
  00402ECC: 9001      str         r0,[sp,#4]
  00402ECE: 9B01      ldr         r3,[sp,#4]
  00402ED0: 2B00      cmp         r3,#0
  00402ED2: D103      bne         00402EDC
  00402ED4: F06F 0300 mvn         r3,#0
  00402ED8: 9300      str         r3,[sp]
  00402EDA: E001      b           00402EE0
  00402EDC: 2300      movs        r3,#0
  00402EDE: 9300      str         r3,[sp]
  00402EE0: 9B00      ldr         r3,[sp]
  00402EE2: 9302      str         r3,[sp,#8]
  00402EE4: 9802      ldr         r0,[sp,#8]
  00402EE6: B004      add         sp,sp,#0x10
  00402EE8: F85D BB04 pop         {r11}
  00402EEC: F85D FB0C ldr         pc,[sp],#0xC
  00402EF0: 0000      movs        r0,r0
  00402EF2: 0000      movs        r0,r0
  00402EF4: 0000      movs        r0,r0
  00402EF6: 0000      movs        r0,r0
  00402EF8: 0000      movs        r0,r0
  00402EFA: 0000      movs        r0,r0
  00402EFC: 0000      movs        r0,r0
  00402EFE: 0000      movs        r0,r0
  00402F00: 0000      movs        r0,r0
  00402F02: 0000      movs        r0,r0
  00402F04: 0000      movs        r0,r0
  00402F06: 0000      movs        r0,r0
  00402F08: E92D 4830 push        {r4,r5,r11,lr}
  00402F0C: F10D 0B08 add         r11,sp,#8
  00402F10: F248 7390 mov         r3,#0x8790
  00402F14: F2C0 0340 movt        r3,#0x40
  00402F18: F648 1598 mov         r5,#0x8998
  00402F1C: F2C0 0540 movt        r5,#0x40
  00402F20: 1D1C      adds        r4,r3,#4
  00402F22: 1D1B      adds        r3,r3,#4
  00402F24: 42AB      cmp         r3,r5
  00402F26: D205      bcs         00402F34
  00402F28: F854 3B04 ldr         r3,[r4],#4
  00402F2C: B103      cbz         r3,00402F30
  00402F2E: 4798      blx         r3
  00402F30: 42AC      cmp         r4,r5
  00402F32: D3F9      bcc         00402F28
  00402F34: E8BD 8830 pop         {r4,r5,r11,pc}
  00402F38: E92D 4830 push        {r4,r5,r11,lr}
  00402F3C: F10D 0B08 add         r11,sp,#8
  00402F40: F648 239C mov         r3,#0x8A9C
  00402F44: F2C0 0340 movt        r3,#0x40
  00402F48: F648 45A4 mov         r5,#0x8CA4
  00402F4C: F2C0 0540 movt        r5,#0x40
  00402F50: 1D1C      adds        r4,r3,#4
  00402F52: 1D1B      adds        r3,r3,#4
  00402F54: 42AB      cmp         r3,r5
  00402F56: D205      bcs         00402F64
  00402F58: F854 3B04 ldr         r3,[r4],#4
  00402F5C: B103      cbz         r3,00402F60
  00402F5E: 4798      blx         r3
  00402F60: 42AC      cmp         r4,r5
  00402F62: D3F9      bcc         00402F58
  00402F64: E8BD 8830 pop         {r4,r5,r11,pc}
  00402F68: F24C 0CD8 mov         r12,#0xC0D8
  00402F6C: F2C0 0C40 movt        r12,#0x40
  00402F70: F8DC F000 ldr         pc,[r12]
  00402F74: F24C 0CD4 mov         r12,#0xC0D4
  00402F78: F2C0 0C40 movt        r12,#0x40
  00402F7C: F8DC F000 ldr         pc,[r12]
  00402F80: F24C 0CD0 mov         r12,#0xC0D0
  00402F84: F2C0 0C40 movt        r12,#0x40
  00402F88: F8DC F000 ldr         pc,[r12]
  00402F8C: F24C 0CCC mov         r12,#0xC0CC
  00402F90: F2C0 0C40 movt        r12,#0x40
  00402F94: F8DC F000 ldr         pc,[r12]
  00402F98: F24C 0CC8 mov         r12,#0xC0C8
  00402F9C: F2C0 0C40 movt        r12,#0x40
  00402FA0: F8DC F000 ldr         pc,[r12]
  00402FA4: E92D 4890 push        {r4,r7,r11,lr}
  00402FA8: F10D 0B08 add         r11,sp,#8
  00402FAC: B088      sub         sp,sp,#0x20
  00402FAE: 466F      mov         r7,sp
  00402FB0: 2300      movs        r3,#0
  00402FB2: 703B      strb        r3,[r7]
  00402FB4: F241 0301 mov         r3,#0x1001
  00402FB8: 60BB      str         r3,[r7,#8]
  00402FBA: 60F8      str         r0,[r7,#0xC]
  00402FBC: 463B      mov         r3,r7
  00402FBE: 613B      str         r3,[r7,#0x10]
  00402FC0: F117 0308 adds        r3,r7,#8
  00402FC4: 2206      movs        r2,#6
  00402FC6: 2100      movs        r1,#0
  00402FC8: 4806      ldr         r0,00402FE4
  00402FCA: F24C 042C mov         r4,#0xC02C
  00402FCE: F2C0 0440 movt        r4,#0x40
  00402FD4: 47A0      blx         r4
  00402FD6: 7838      ldrb        r0,[r7]
  00402FD8: E000      b           00402FDC
  00402FDA: 6878      ldr         r0,[r7,#4]
  00402FDC: B008      add         sp,sp,#0x20
  00402FDE: E8BD 8890 pop         {r4,r7,r11,pc}
  00402FE2: DEFE      __debugbreak
  00402FE4: 1388      asrs        r0,r1,#0xE
  00402FE6: 406D      eors        r5,r5,r5
  00402FE8: E92D 4890 push        {r4,r7,r11,lr}
  00402FEC: F10D 0B08 add         r11,sp,#8
  00402FF0: B088      sub         sp,sp,#0x20
  00402FF2: 466F      mov         r7,sp
  00402FF4: 2400      movs        r4,#0
  00402FF6: 703C      strb        r4,[r7]
  00402FF8: F241 0402 mov         r4,#0x1002
  00402FFC: 60BC      str         r4,[r7,#8]
  00402FFE: 60F8      str         r0,[r7,#0xC]
  00403000: 6139      str         r1,[r7,#0x10]
  00403002: 617A      str         r2,[r7,#0x14]
  00403004: 463A      mov         r2,r7
  00403006: 61BA      str         r2,[r7,#0x18]
  00403008: 61FB      str         r3,[r7,#0x1C]
  0040300A: F117 0308 adds        r3,r7,#8
  0040300E: 2206      movs        r2,#6
  00403010: 2100      movs        r1,#0
  00403012: 4806      ldr         r0,0040302C
  00403014: F24C 042C mov         r4,#0xC02C
  00403018: F2C0 0440 movt        r4,#0x40
  0040301E: 47A0      blx         r4
  00403020: 7838      ldrb        r0,[r7]
  00403022: E000      b           00403026
  00403024: 6878      ldr         r0,[r7,#4]
  00403026: B008      add         sp,sp,#0x20
  00403028: E8BD 8890 pop         {r4,r7,r11,pc}
  0040302C: 1388      asrs        r0,r1,#0xE
  0040302E: 406D      eors        r5,r5,r5
  00403030: E92D 48F0 push        {r4-r7,r11,lr}
  00403034: F10D 0B10 add         r11,sp,#0x10
  00403038: F7FE F800 bl          0040103C
  0040303C: B0D9      sub         sp,sp,#0x164
  0040303E: F24A 030C mov         r3,#0xA00C
  00403042: F2C0 0340 movt        r3,#0x40
  00403046: 460E      mov         r6,r1
  00403048: 4607      mov         r7,r0
  0040304A: 691D      ldr         r5,[r3,#0x10]
  0040304C: F1B5 3FFF cmp         r5,#0xFFFFFFFF
  00403050: D057      beq         00403102
  00403052: B926      cbnz        r6,0040305E
  00403054: F647 73A8 mov         r3,#0x7FA8
  00403058: F2C0 0340 movt        r3,#0x40
  0040305C: E04C      b           004030F8
  0040305E: F647 73F4 mov         r3,#0x7FF4
  00403062: F2C0 0340 movt        r3,#0x40
  00403066: 9205      str         r2,[sp,#0x14]
  00403068: F248 0290 mov         r2,#0x8090
  0040306C: F2C0 0240 movt        r2,#0x40
  00403070: 9306      str         r3,[sp,#0x18]
  00403072: F248 0300 movw        r3,#0x8000
  00403076: F2C0 0340 movt        r3,#0x40
  0040307A: F106 0420 add         r4,r6,#0x20
  0040307E: A81C      add         r0,sp,#0x70
  00403080: 9304      str         r3,[sp,#0x10]
  00403082: 68F3      ldr         r3,[r6,#0xC]
  00403084: 21F4      movs        r1,#0xF4
  00403086: 9401      str         r4,[sp,#4]
  00403088: 3B24      subs        r3,r3,#0x24
  0040308A: 9303      str         r3,[sp,#0xC]
  0040308C: F248 032C mov         r3,#0x802C
  00403090: F2C0 0340 movt        r3,#0x40
  00403094: 9302      str         r3,[sp,#8]
  00403096: F248 0334 mov         r3,#0x8034
  0040309A: F2C0 0340 movt        r3,#0x40
  0040309E: 9300      str         r3,[sp]
  004030A0: F248 0348 mov         r3,#0x8048
  004030A4: F2C0 0340 movt        r3,#0x40
  004030A8: F000 FEAA bl          00403E00
  004030AC: 68F3      ldr         r3,[r6,#0xC]
  004030AE: A90E      add         r1,sp,#0x38
  004030B0: A808      add         r0,sp,#0x20
  004030B2: 3B24      subs        r3,r3,#0x24
  004030B4: 4622      mov         r2,r4
  004030B6: F000 F931 bl          0040331C
  004030BA: A81C      add         r0,sp,#0x70
  004030BC: F000 F96C bl          00403398
  004030C0: 4604      mov         r4,r0
  004030C2: A81C      add         r0,sp,#0x70
  004030C4: F000 F968 bl          00403398
  004030C8: F248 03A4 mov         r3,#0x80A4
  004030CC: F2C0 0340 movt        r3,#0x40
  004030D0: F248 02AC mov         r2,#0x80AC
  004030D4: F2C0 0240 movt        r2,#0x40
  004030D8: F1C4 01F4 rsb         r1,r4,#0xF4
  004030DC: 9302      str         r3,[sp,#8]
  004030DE: AB0E      add         r3,sp,#0x38
  004030E0: 9301      str         r3,[sp,#4]
  004030E2: F248 03A8 mov         r3,#0x80A8
  004030E6: F2C0 0340 movt        r3,#0x40
  004030EA: AC1C      add         r4,sp,#0x70
  004030EC: 4420      add         r0,r0,r4
  004030EE: 9300      str         r3,[sp]
  004030F0: AB08      add         r3,sp,#0x20
  004030F2: F000 FE85 bl          00403E00
  004030F6: AB1C      add         r3,sp,#0x70
  004030F8: 2204      movs        r2,#4
  004030FA: 4629      mov         r1,r5
  004030FC: 4638      mov         r0,r7
  004030FE: F000 F957 bl          004033B0
  00403102: B059      add         sp,sp,#0x164
  00403104: F7FD FFA6 bl          00401054
  00403108: E8BD 88F0 pop         {r4-r7,r11,pc}

  0040319C: E92D 4818 push        {r3,r4,r11,lr}
  004031A0: F10D 0B08 add         r11,sp,#8
  004031A4: 2904      cmp         r1,#4
  004031A6: D814      bhi         004031D2
  004031A8: F24A 030C mov         r3,#0xA00C
  004031AC: F2C0 0340 movt        r3,#0x40
  004031B0: F853 4021 ldr         r4,[r3,r1,lsl #2]
  004031B4: F647 333C mov         r3,#0x7B3C
  004031B8: F2C0 0340 movt        r3,#0x40
  004031BC: F1B4 3FFF cmp         r4,#0xFFFFFFFF
  004031C0: F853 3021 ldr         r3,[r3,r1,lsl #2]
  004031C4: D010      beq         004031E8
  004031C6: 460A      mov         r2,r1
  004031C8: 4621      mov         r1,r4
  004031CA: F000 F8F1 bl          004033B0
  004031CE: E8BD 8818 pop         {r3,r4,r11,pc}
  004031D2: F647 333C mov         r3,#0x7B3C
  004031D6: F2C0 0340 movt        r3,#0x40
  004031DA: 2401      movs        r4,#1
  004031DC: 2105      movs        r1,#5
  004031DE: 695B      ldr         r3,[r3,#0x14]
  004031E0: 460A      mov         r2,r1
  004031E2: 4621      mov         r1,r4
  004031E4: F000 F8E4 bl          004033B0
  004031E8: E8BD 8818 pop         {r3,r4,r11,pc}
  004031EC: 0000      movs        r0,r0
  004031EE: 0000      movs        r0,r0
  004031F0: 0000      movs        r0,r0
  004031F2: 0000      movs        r0,r0
  004031F4: 0000      movs        r0,r0
  004031F6: 0000      movs        r0,r0
  004031F8: 0000      movs        r0,r0
  004031FA: 0000      movs        r0,r0
  004031FC: 0000      movs        r0,r0
  004031FE: 0000      movs        r0,r0
  00403200: 0000      movs        r0,r0
  00403202: 0000      movs        r0,r0
  00403204: E92D 49F0 push        {r4-r8,r11,lr}
  00403208: F10D 0B14 add         r11,sp,#0x14
  0040320C: F7FD FF16 bl          0040103C
  00403210: F5AD 6D80 sub         sp,sp,#0x400
  00403214: F24A 030C mov         r3,#0xA00C
  00403218: F2C0 0340 movt        r3,#0x40
  0040321C: 460C      mov         r4,r1
  0040321E: 4680      mov         r8,r0
  00403220: 689F      ldr         r7,[r3,#8]
  00403222: F1B7 3FFF cmp         r7,#0xFFFFFFFF
  00403226: D037      beq         00403298
  00403228: F991 3000 ldrsb       r3,[r1]
  0040322C: B353      cbz         r3,00403284
  0040322E: 4608      mov         r0,r1
  00403230: F000 F8B2 bl          00403398
  00403234: F100 032D add         r3,r0,#0x2D
  00403238: F5B3 6F80 cmp         r3,#0x400
  0040323C: D822      bhi         00403284
  0040323E: F647 356C mov         r5,#0x7B6C
  00403242: F2C0 0540 movt        r5,#0x40
  00403246: 4668      mov         r0,sp
  00403248: 466E      mov         r6,sp
  0040324A: 4629      mov         r1,r5
  0040324C: F000 FE2E bl          00403EAC
  00403250: 4668      mov         r0,sp
  00403252: 1E42      subs        r2,r0,#1
  00403254: F812 3F01 ldrb        r3,[r2,#1]!
  00403258: 2B00      cmp         r3,#0
  0040325A: D1FB      bne         00403254
  0040325C: F814 3B01 ldrb        r3,[r4],#1
  00403260: 2B00      cmp         r3,#0
  00403262: F802 3B01 strb        r3,[r2],#1
  00403266: D1F9      bne         0040325C
  00403268: F105 011C add         r1,r5,#0x1C
  0040326C: 1E42      subs        r2,r0,#1
  0040326E: F812 3F01 ldrb        r3,[r2,#1]!
  00403272: 2B00      cmp         r3,#0
  00403274: D1FB      bne         0040326E
  00403276: F811 3B01 ldrb        r3,[r1],#1
  0040327A: 2B00      cmp         r3,#0
  0040327C: F802 3B01 strb        r3,[r2],#1
  00403280: D1F9      bne         00403276
  00403282: E003      b           0040328C
  00403284: F647 7678 mov         r6,#0x7F78
  00403288: F2C0 0640 movt        r6,#0x40
  0040328C: 4633      mov         r3,r6
  0040328E: 2202      movs        r2,#2
  00403290: 4639      mov         r1,r7
  00403292: 4640      mov         r0,r8
  00403294: F000 F88C bl          004033B0
  00403298: F50D 6D80 add         sp,sp,#0x400
  0040329C: F7FD FEDA bl          00401054
  004032A0: E8BD 89F0 pop         {r4-r8,r11,pc}

  0040331C: E92D 4FFE push        {r1-r11,lr}
  00403320: F10D 0B28 add         r11,sp,#0x28
  00403324: 461E      mov         r6,r3
  00403326: 2500      movs        r5,#0
  00403328: 2700      movs        r7,#0
  0040332A: 1A13      subs        r3,r2,r0
  0040332C: 9300      str         r3,[sp]
  0040332E: 469A      mov         r10,r3
  00403330: 4688      mov         r8,r1
  00403332: 9001      str         r0,[sp,#4]
  00403334: 4681      mov         r9,r0
  00403336: 2E10      cmp         r6,#0x10
  00403338: BF38      it          cc
  0040333A: 4632      movcc       r2,r6
  0040333C: BF28      it          cs
  0040333E: 2210      movcs       r2,#0x10
  00403340: 4295      cmp         r5,r2
  00403342: D211      bcs         00403368
  00403344: F81A 4009 ldrb        r4,[r10,r9]
  00403348: F647 72A0 mov         r2,#0x7FA0
  0040334C: F2C0 0240 movt        r2,#0x40
  00403350: F1C7 0131 rsb         r1,r7,#0x31
  00403354: 4623      mov         r3,r4
  00403356: EB07 0008 add         r0,r7,r8
  0040335A: F000 FD51 bl          00403E00
  0040335E: 1C6D      adds        r5,r5,#1
  00403360: 1CFF      adds        r7,r7,#3
  00403362: F809 4B01 strb        r4,[r9],#1
  00403366: E7E6      b           00403336
  00403368: F8DD A004 ldr         r10,[sp,#4]
  0040336C: 2200      movs        r2,#0
  0040336E: EB05 0345 add         r3,r5,r5,lsl #1
  00403372: F805 200A strb        r2,[r5,r10]
  00403376: F803 2008 strb        r2,[r3,r8]
  0040337A: E8BD 8FFE pop         {r1-r11,pc}

  00403398: E92D 4800 push        {r11,lr}
  0040339C: 46EB      mov         r11,sp
  0040339E: 4602      mov         r2,r0
  004033A0: F912 3B01 ldrsb       r3,[r2],#1
  004033A4: 2B00      cmp         r3,#0
  004033A6: D1FB      bne         004033A0
  004033A8: 1A13      subs        r3,r2,r0
  004033AA: 1E58      subs        r0,r3,#1
  004033AC: E8BD 8800 pop         {r11,pc}
  004033B0: E92D 4FF0 push        {r4-r11,lr}
  004033B4: F10D 0B1C add         r11,sp,#0x1C
  004033B8: F7FD FE40 bl          0040103C
  004033BC: F6AD 6D48 sub         sp,sp,#0xE48
  004033C0: 4698      mov         r8,r3
  004033C2: 9305      str         r3,[sp,#0x14]
  004033C4: 4691      mov         r9,r2
  004033C6: 468A      mov         r10,r1
  004033C8: 4606      mov         r6,r0
  004033CA: 2700      movs        r7,#0
  004033CC: F7FF FA3C bl          00402848
  004033D0: 4605      mov         r5,r0
  004033D2: B91D      cbnz        r5,004033DC
  004033D4: 4630      mov         r0,r6
  004033D6: F7FF FA2D bl          00402834
  004033DA: 4607      mov         r7,r0
  004033DC: F24C 0428 mov         r4,#0xC028
  004033E0: F2C0 0440 movt        r4,#0x40
  004033E4: 2300      movs        r3,#0
  004033E6: 9301      str         r3,[sp,#4]
  004033EA: 9300      str         r3,[sp]
  004033EC: 43DB      mvns        r3,r3
  004033EE: 4642      mov         r2,r8
  004033F0: 2100      movs        r1,#0
  004033F2: F64F 50E9 mov         r0,#0xFDE9
  004033F6: 47A0      blx         r4
  004033F8: F5B0 7F00 cmp         r0,#0x200
  004033FC: D213      bcs         00403426
  004033FE: F24C 0428 mov         r4,#0xC028
  00403402: F2C0 0440 movt        r4,#0x40
  00403406: F60D 2348 add         r3,sp,#0xA48
  0040340A: 9001      str         r0,[sp,#4]
  0040340E: 9300      str         r3,[sp]
  00403410: F06F 0300 mvn         r3,#0
  00403414: 4642      mov         r2,r8
  00403416: 2100      movs        r1,#0
  00403418: F64F 50E9 mov         r0,#0xFDE9
  0040341C: 47A0      blx         r4
  0040341E: B110      cbz         r0,00403426
  00403420: F60D 2448 add         r4,sp,#0xA48
  00403424: E003      b           0040342E
  00403426: F647 6480 movw        r4,#0x7E80
  0040342A: F2C0 0440 movt        r4,#0x40
  0040342E: F241 0002 mov         r0,#0x1002
  00403432: F7FF FDB7 bl          00402FA4
  00403436: B170      cbz         r0,00403456
  00403438: F647 3354 mov         r3,#0x7B54
  0040343C: F2C0 0340 movt        r3,#0x40
  00403440: 4632      mov         r2,r6
  00403442: 4648      mov         r0,r9
  00403444: F853 1029 ldr         r1,[r3,r9,lsl #2]
  00403448: 4623      mov         r3,r4
  0040344A: F7FF FDCD bl          00402FE8
  0040344E: 2800      cmp         r0,#0
  00403450: D16E      bne         00403530
  00403452: 2300      movs        r3,#0
  00403454: E000      b           00403458
  00403456: 2301      movs        r3,#1
  00403458: B90F      cbnz        r7,0040345E
  0040345A: 2D00      cmp         r5,#0
  0040345C: D067      beq         0040352E
  0040345E: B13B      cbz         r3,00403470
  00403460: F24C 0330 mov         r3,#0xC030
  00403464: F2C0 0340 movt        r3,#0x40
  0040346A: 4798      blx         r3
  0040346C: 2800      cmp         r0,#0
  0040346E: D15E      bne         0040352E
  00403470: AB06      add         r3,sp,#0x18
  00403472: 9300      str         r3,[sp]
  00403474: F44F 7282 mov         r2,#0x104
  00403478: AB04      add         r3,sp,#0x10
  0040347A: A988      add         r1,sp,#0x220
  0040347C: 1F30      subs        r0,r6,#4
  0040347E: 9201      str         r2,[sp,#4]
  00403480: F000 FB4E bl          00403B20
  00403484: B16D      cbz         r5,004034A2
  00403486: F647 63F0 mov         r3,#0x7EF0
  0040348A: F2C0 0340 movt        r3,#0x40
  0040348E: 9A04      ldr         r2,[sp,#0x10]
  00403490: 4650      mov         r0,r10
  00403492: 9300      str         r3,[sp]
  00403494: AB06      add         r3,sp,#0x18
  00403496: A988      add         r1,sp,#0x220
  00403498: 9402      str         r4,[sp,#8]
  0040349A: F8CD 9004 str         r9,[sp,#4]
  0040349E: 47A8      blx         r5
  004034A0: E043      b           0040352A
  004034A2: F24C 0424 mov         r4,#0xC024
  004034A6: F2C0 0440 movt        r4,#0x40
  004034AA: 2300      movs        r3,#0
  004034AC: F50D 6285 add         r2,sp,#0x428
  004034B2: F647 7630 mov         r6,#0x7F30
  004034B6: F2C0 0640 movt        r6,#0x40
  004034BA: 9303      str         r3,[sp,#0xC]
  004034BC: 9302      str         r3,[sp,#8]
  004034BE: F240 380A mov         r8,#0x30A
  004034C2: 9200      str         r2,[sp]
  004034C4: 43DB      mvns        r3,r3
  004034C6: AA88      add         r2,sp,#0x220
  004034C8: 2100      movs        r1,#0
  004034CA: F64F 50E9 mov         r0,#0xFDE9
  004034CE: F8CD 8004 str         r8,[sp,#4]
  004034D2: 47A0      blx         r4
  004034D4: B108      cbz         r0,004034DA
  004034D6: F50D 6685 add         r6,sp,#0x428
  004034DA: F24C 0424 mov         r4,#0xC024
  004034DE: F2C0 0440 movt        r4,#0x40
  004034E2: 2300      movs        r3,#0
  004034E4: F50D 62E7 add         r2,sp,#0x738
  004034EA: F647 7544 mov         r5,#0x7F44
  004034EE: F2C0 0540 movt        r5,#0x40
  004034F2: 9303      str         r3,[sp,#0xC]
  004034F4: 9302      str         r3,[sp,#8]
  004034F6: 9200      str         r2,[sp]
  004034F8: 43DB      mvns        r3,r3
  004034FA: AA06      add         r2,sp,#0x18
  004034FC: 2100      movs        r1,#0
  004034FE: F64F 50E9 mov         r0,#0xFDE9
  00403502: F8CD 8004 str         r8,[sp,#4]
  00403506: 47A0      blx         r4
  00403508: B108      cbz         r0,0040350E
  0040350A: F50D 65E7 add         r5,sp,#0x738
  0040350E: F647 7258 mov         r2,#0x7F58
  00403512: F2C0 0240 movt        r2,#0x40
  00403516: 9B05      ldr         r3,[sp,#0x14]
  00403518: 4631      mov         r1,r6
  0040351A: 9200      str         r2,[sp]
  0040351C: 9A04      ldr         r2,[sp,#0x10]
  0040351E: 9302      str         r3,[sp,#8]
  00403520: 462B      mov         r3,r5
  00403522: 4650      mov         r0,r10
  00403524: F8CD 9004 str         r9,[sp,#4]
  00403528: 47B8      blx         r7
  0040352A: 2801      cmp         r0,#1
  0040352C: D100      bne         00403530
  0040352E: DEFE      __debugbreak
  00403530: F60D 6D48 add         sp,sp,#0xE48
  00403534: F7FD FD8E bl          00401054
  00403538: E8BD 8FF0 pop         {r4-r11,pc}

  004035E4: E92D 48F0 push        {r4-r7,r11,lr}
  004035E8: F10D 0B10 add         r11,sp,#0x10
  004035EC: F7FD FD26 bl          0040103C
  004035F0: F2AD 4D04 sub         sp,sp,#0x404
  004035F4: F24A 030C mov         r3,#0xA00C
  004035F8: F2C0 0340 movt        r3,#0x40
  004035FC: 4604      mov         r4,r0
  004035FE: 68DF      ldr         r7,[r3,#0xC]
  00403600: F1B7 3FFF cmp         r7,#0xFFFFFFFF
  00403604: D035      beq         00403672
  00403606: B34C      cbz         r4,0040365C
  00403608: F7FF FEC6 bl          00403398
  0040360C: F100 033A add         r3,r0,#0x3A
  00403610: F5B3 6F80 cmp         r3,#0x400
  00403614: D822      bhi         0040365C
  00403616: F647 359C mov         r5,#0x7B9C
  0040361A: F2C0 0540 movt        r5,#0x40
  0040361E: 4668      mov         r0,sp
  00403620: 466E      mov         r6,sp
  00403622: 4629      mov         r1,r5
  00403624: F000 FC42 bl          00403EAC
  00403628: 4668      mov         r0,sp
  0040362A: 1E42      subs        r2,r0,#1
  0040362C: F812 3F01 ldrb        r3,[r2,#1]!
  00403630: 2B00      cmp         r3,#0
  00403632: D1FB      bne         0040362C
  00403634: F814 3B01 ldrb        r3,[r4],#1
  00403638: 2B00      cmp         r3,#0
  0040363A: F802 3B01 strb        r3,[r2],#1
  0040363E: D1F9      bne         00403634
  00403640: F105 0110 add         r1,r5,#0x10
  00403644: 1E42      subs        r2,r0,#1
  00403646: F812 3F01 ldrb        r3,[r2,#1]!
  0040364A: 2B00      cmp         r3,#0
  0040364C: D1FB      bne         00403646
  0040364E: F811 3B01 ldrb        r3,[r1],#1
  00403652: 2B00      cmp         r3,#0
  00403654: F802 3B01 strb        r3,[r2],#1
  00403658: D1F9      bne         0040364E
  0040365A: E003      b           00403664
  0040365C: F248 06B8 mov         r6,#0x80B8
  00403660: F2C0 0640 movt        r6,#0x40
  00403664: F8DD 041C ldr         r0,[sp,#0x41C]
  00403668: 4633      mov         r3,r6
  0040366A: 2203      movs        r2,#3
  0040366C: 4639      mov         r1,r7
  0040366E: F7FF FE9F bl          004033B0
  00403672: F20D 4D04 add         sp,sp,#0x404
  00403676: F7FD FCED bl          00401054
  0040367A: E8BD 88F0 pop         {r4-r7,r11,pc}

  004036F8: F24C 0C90 mov         r12,#0xC090
  004036FC: F2C0 0C40 movt        r12,#0x40
  00403700: F8DC F000 ldr         pc,[r12]
  00403704: F24C 0C94 mov         r12,#0xC094
  00403708: F2C0 0C40 movt        r12,#0x40
  0040370C: F8DC F000 ldr         pc,[r12]
  00403710: F24C 0C98 mov         r12,#0xC098
  00403714: F2C0 0C40 movt        r12,#0x40
  00403718: F8DC F000 ldr         pc,[r12]
  0040371C: F24C 0C9C mov         r12,#0xC09C
  00403720: F2C0 0C40 movt        r12,#0x40
  00403724: F8DC F000 ldr         pc,[r12]
  00403728: F24C 0CA0 mov         r12,#0xC0A0
  0040372C: F2C0 0C40 movt        r12,#0x40
  00403730: F8DC F000 ldr         pc,[r12]
  00403734: F24C 1C0C mov         r12,#0xC10C
  00403738: F2C0 0C40 movt        r12,#0x40
  0040373C: F8DC F000 ldr         pc,[r12]
  00403740: E92D 4800 push        {r11,lr}
  00403744: 46EB      mov         r11,sp
  00403746: F7FD FC79 bl          0040103C
  0040374A: F2AD 4D14 sub         sp,sp,#0x414
  0040374E: F24A 1270 mov         r2,#0xA170
  00403752: F2C0 0240 movt        r2,#0x40
  00403756: 7813      ldrb        r3,[r2]
  00403758: B133      cbz         r3,00403768
  0040375A: 2000      movs        r0,#0
  0040375C: F20D 4D14 add         sp,sp,#0x414
  00403760: F7FD FC78 bl          00401054
  00403764: E8BD 8800 pop         {r11,pc}
  00403768: 2301      movs        r3,#1
  0040376A: 7013      strb        r3,[r2]
  0040376C: F000 F86E bl          0040384C
  00403770: 2800      cmp         r0,#0
  00403772: D1F3      bne         0040375C
  00403774: F24C 0304 mov         r3,#0xC004
  00403778: F2C0 0340 movt        r3,#0x40
  0040377C: F248 102C mov         r0,#0x812C
  00403780: F2C0 0040 movt        r0,#0x40
  00403786: 4798      blx         r3
  00403788: B1E0      cbz         r0,004037C4
  0040378A: F24C 0308 mov         r3,#0xC008
  0040378E: F2C0 0340 movt        r3,#0x40
  00403792: F44F 7282 mov         r2,#0x104
  00403796: 4669      mov         r1,sp
  0040379A: 4798      blx         r3
  0040379C: B190      cbz         r0,004037C4
  0040379E: A982      add         r1,sp,#0x208
  004037A0: 4668      mov         r0,sp
  004037A2: F44F 7282 mov         r2,#0x104
  004037A6: F000 F935 bl          00403A14
  004037AA: B158      cbz         r0,004037C4
  004037AC: F24C 0350 mov         r3,#0xC050
  004037B0: F2C0 0340 movt        r3,#0x40
  004037B4: F44F 6210 mov         r2,#0x900
  004037B8: 2100      movs        r1,#0
  004037BC: A882      add         r0,sp,#0x208
  004037BE: 4798      blx         r3
  004037C0: 2800      cmp         r0,#0
  004037C2: D1CB      bne         0040375C
  004037C4: F24C 0350 mov         r3,#0xC050
  004037C8: F2C0 0340 movt        r3,#0x40
  004037CC: F248 2050 mov         r0,#0x8250
  004037D0: F2C0 0040 movt        r0,#0x40
  004037D4: F44F 6220 mov         r2,#0xA00
  004037DA: 2100      movs        r1,#0
  004037DC: 4798      blx         r3
  004037DE: F20D 4D14 add         sp,sp,#0x414
  004037E2: F7FD FC37 bl          00401054
  004037E6: E8BD 8800 pop         {r11,pc}
  
  0040384C: E92D 48F0 push        {r4-r7,r11,lr}
  00403850: F10D 0B10 add         r11,sp,#0x10
  00403854: F7FD FBF2 bl          0040103C
  00403858: F5AD 7D09 sub         sp,sp,#0x224
  0040385C: F24C 0350 mov         r3,#0xC050
  00403860: F2C0 0340 movt        r3,#0x40
  00403864: F248 1048 mov         r0,#0x8148
  00403868: F2C0 0040 movt        r0,#0x40
  0040386C: F44F 6200 mov         r2,#0x800
  00403872: 2100      movs        r1,#0
  00403874: 4798      blx         r3
  00403876: 4605      mov         r5,r0
  00403878: B9AD      cbnz        r5,004038A6
  0040387A: F24C 0320 mov         r3,#0xC020
  0040387E: F2C0 0340 movt        r3,#0x40
  00403884: 4798      blx         r3
  00403886: 2857      cmp         r0,#0x57
  00403888: D148      bne         0040391C
  0040388A: F24C 0350 mov         r3,#0xC050
  0040388E: F2C0 0340 movt        r3,#0x40
  00403892: F248 1048 mov         r0,#0x8148
  00403896: F2C0 0040 movt        r0,#0x40
  0040389A: 2200      movs        r2,#0
  0040389E: 2100      movs        r1,#0
  004038A0: 4798      blx         r3
  004038A2: 4605      mov         r5,r0
  004038A4: B3D5      cbz         r5,0040391C
  004038A6: F24C 0300 movw        r3,#0xC000
  004038AA: F2C0 0340 movt        r3,#0x40
  004038AE: F248 1190 mov         r1,#0x8190
  004038B2: F2C0 0140 movt        r1,#0x40
  004038B6: 4628      mov         r0,r5
  004038BA: 4798      blx         r3
  004038BC: 4604      mov         r4,r0
  004038BE: B36C      cbz         r4,0040391C
  004038C0: F24C 0300 movw        r3,#0xC000
  004038C4: F2C0 0340 movt        r3,#0x40
  004038C8: F248 11A0 mov         r1,#0x81A0
  004038CC: F2C0 0140 movt        r1,#0x40
  004038D0: 4628      mov         r0,r5
  004038D4: 4798      blx         r3
  004038D6: 4606      mov         r6,r0
  004038D8: B306      cbz         r6,0040391C
  004038DA: F24C 0300 movw        r3,#0xC000
  004038DE: F2C0 0340 movt        r3,#0x40
  004038E2: F248 11B4 mov         r1,#0x81B4
  004038E6: F2C0 0140 movt        r1,#0x40
  004038EA: 4628      mov         r0,r5
  004038EE: 4798      blx         r3
  004038F0: 4607      mov         r7,r0
  004038F2: B19F      cbz         r7,0040391C
  004038F4: F248 11C0 mov         r1,#0x81C0
  004038F8: F2C0 0140 movt        r1,#0x40
  004038FC: AB03      add         r3,sp,#0xC
  004038FE: F04F 4000 mov         r0,#0x80000000
  00403902: 9300      str         r3,[sp]
  00403904: 2301      movs        r3,#1
  00403906: 2200      movs        r2,#0
  00403908: 1C80      adds        r0,r0,#2
  0040390A: 47A0      blx         r4
  0040390C: B168      cbz         r0,0040392A
  0040390E: F24C 030C mov         r3,#0xC00C
  00403912: F2C0 0340 movt        r3,#0x40
  00403916: 4628      mov         r0,r5
  0040391A: 4798      blx         r3
  0040391C: 2000      movs        r0,#0
  0040391E: F50D 7D09 add         sp,sp,#0x224
  00403922: F7FD FB97 bl          00401054
  00403926: E8BD 88F0 pop         {r4-r7,r11,pc}
  0040392A: F44F 7302 mov         r3,#0x208
  0040392E: 9302      str         r3,[sp,#8]
  00403930: AB02      add         r3,sp,#8
  00403932: 9301      str         r3,[sp,#4]
  00403934: F248 211C mov         r1,#0x821C
  00403938: F2C0 0140 movt        r1,#0x40
  0040393C: 9803      ldr         r0,[sp,#0xC]
  0040393E: AB06      add         r3,sp,#0x18
  00403940: 9300      str         r3,[sp]
  00403942: AB04      add         r3,sp,#0x10
  00403944: 2200      movs        r2,#0
  00403946: 47B0      blx         r6
  00403948: 4604      mov         r4,r0
  0040394A: 9803      ldr         r0,[sp,#0xC]
  0040394C: 47B8      blx         r7
  0040394E: F24C 030C mov         r3,#0xC00C
  00403952: F2C0 0340 movt        r3,#0x40
  00403956: 4628      mov         r0,r5
  0040395A: 4798      blx         r3
  0040395C: 2C00      cmp         r4,#0
  0040395E: D1DD      bne         0040391C
  00403960: 9B04      ldr         r3,[sp,#0x10]
  00403962: 2B01      cmp         r3,#1
  00403964: D1DA      bne         0040391C
  00403966: 9B02      ldr         r3,[sp,#8]
  00403968: F013 0F01 tst         r3,#1
  0040396C: D1D6      bne         0040391C
  0040396E: 085A      lsrs        r2,r3,#1
  00403970: 2A02      cmp         r2,#2
  00403972: D3D3      bcc         0040391C
  00403974: 1E52      subs        r2,r2,#1
  00403976: AB06      add         r3,sp,#0x18
  00403978: EB03 0142 add         r1,r3,r2,lsl #1
  0040397C: F833 3012 ldrh        r3,[r3,r2,lsl #1]
  00403980: 2B00      cmp         r3,#0
  00403982: D1CB      bne         0040391C
  00403984: F831 3C02 ldrh        r3,[r1,#-2]
  00403988: 2B5C      cmp         r3,#0x5C
  0040398A: D002      beq         00403992
  0040398C: 235C      movs        r3,#0x5C
  0040398E: 800B      strh        r3,[r1]
  00403990: 1C52      adds        r2,r2,#1
  00403992: F1C2 33FF rsb         r3,r2,#0xFFFFFFFF
  00403996: 2B12      cmp         r3,#0x12
  00403998: D3C0      bcc         0040391C
  0040399A: F102 0311 add         r3,r2,#0x11
  0040399E: F5B3 7F82 cmp         r3,#0x104
  004039A2: D8BB      bhi         0040391C
  004039A4: AB06      add         r3,sp,#0x18
  004039A6: EB03 0242 add         r2,r3,r2,lsl #1
  004039AA: F248 1108 mov         r1,#0x8108
  004039AE: F2C0 0140 movt        r1,#0x40
  004039B2: F102 0022 add         r0,r2,#0x22
  004039B6: F831 3B02 ldrh        r3,[r1],#2
  004039BA: F822 3B02 strh        r3,[r2],#2
  004039BE: 4282      cmp         r2,r0
  004039C0: D1F9      bne         004039B6
  004039C2: F24C 0350 mov         r3,#0xC050
  004039C6: F2C0 0340 movt        r3,#0x40
  004039CA: 2100      movs        r1,#0
  004039CC: F44F 6210 mov         r2,#0x900
  004039D2: A806      add         r0,sp,#0x18
  004039D4: 4798      blx         r3
  004039D6: F50D 7D09 add         sp,sp,#0x224
  004039DA: F7FD FB3B bl          00401054
  004039DE: E8BD 88F0 pop         {r4-r7,r11,pc}

  00403A14: E92D 4870 push        {r4-r6,r11,lr}
  00403A18: F10D 0B0C add         r11,sp,#0xC
  00403A1C: F7FD FB0E bl          0040103C
  00403A20: F5AD 6DC4 sub         sp,sp,#0x620
  00403A24: AB08      add         r3,sp,#0x20
  00403A26: 9303      str         r3,[sp,#0xC]
  00403A28: AB88      add         r3,sp,#0x220
  00403A2A: 9301      str         r3,[sp,#4]
  00403A2C: 4615      mov         r5,r2
  00403A2E: 460E      mov         r6,r1
  00403A30: F44F 7480 mov         r4,#0x100
  00403A34: F50D 6384 add         r3,sp,#0x420
  00403A38: A906      add         r1,sp,#0x18
  00403A3A: 2203      movs        r2,#3
  00403A3C: 9404      str         r4,[sp,#0x10]
  00403A3E: 9402      str         r4,[sp,#8]
  00403A40: 9400      str         r4,[sp]
  00403A42: F000 FA45 bl          00403ED0
  00403A46: B130      cbz         r0,00403A56
  00403A48: 2000      movs        r0,#0
  00403A4A: F50D 6DC4 add         sp,sp,#0x620
  00403A4E: F7FD FB01 bl          00401054
  00403A52: E8BD 8870 pop         {r4-r6,r11,pc}
  00403A56: F248 223C mov         r2,#0x823C
  00403A5A: F2C0 0240 movt        r2,#0x40
  00403A5E: A888      add         r0,sp,#0x220
  00403A60: 2109      movs        r1,#9
  00403A62: F000 FA29 bl          00403EB8
  00403A66: 2800      cmp         r0,#0
  00403A68: D1EE      bne         00403A48
  00403A6A: F248 2234 mov         r2,#0x8234
  00403A6E: F2C0 0240 movt        r2,#0x40
  00403A72: A808      add         r0,sp,#0x20
  00403A74: 2104      movs        r1,#4
  00403A76: F000 FA1F bl          00403EB8
  00403A7A: 2800      cmp         r0,#0
  00403A7C: D1E4      bne         00403A48
  00403A7E: AB08      add         r3,sp,#0x20
  00403A80: 9301      str         r3,[sp,#4]
  00403A82: AB88      add         r3,sp,#0x220
  00403A84: 9300      str         r3,[sp]
  00403A86: F50D 6384 add         r3,sp,#0x420
  00403A8A: AA06      add         r2,sp,#0x18
  00403A8C: 4629      mov         r1,r5
  00403A8E: 4630      mov         r0,r6
  00403A90: F000 FA18 bl          00403EC4
  00403A94: 2800      cmp         r0,#0
  00403A96: D1D7      bne         00403A48
  00403A98: 2001      movs        r0,#1
  00403A9A: F50D 6DC4 add         sp,sp,#0x620
  00403A9E: F7FD FAD9 bl          00401054
  00403AA2: E8BD 8870 pop         {r4-r6,r11,pc}
  
  00403B20: B40F      push        {r0-r3}
  00403B22: E92D 4FF0 push        {r4-r11,lr}
  00403B26: F10D 0B1C add         r11,sp,#0x1C
  00403B2A: B097      sub         sp,sp,#0x5C
  00403B2C: F24C 0410 mov         r4,#0xC010
  00403B30: F2C0 0440 movt        r4,#0x40
  00403B34: 4605      mov         r5,r0
  00403B36: F04F 0800 mov         r8,#0
  00403B3C: F8C3 8000 str         r8,[r3]
  00403B40: 1E6D      subs        r5,r5,#1
  00403B42: 910D      str         r1,[sp,#0x34]
  00403B44: F8A1 8000 strh        r8,[r1]
  00403B48: 221C      movs        r2,#0x1C
  00403B4A: A910      add         r1,sp,#0x40
  00403B4C: 4628      mov         r0,r5
  00403B4E: 930C      str         r3,[sp,#0x30]
  00403B50: 47A0      blx         r4
  00403B52: B930      cbnz        r0,00403B62
  00403B54: 2600      movs        r6,#0
  00403B56: 4630      mov         r0,r6
  00403B58: B017      add         sp,sp,#0x5C
  00403B5A: E8BD 0FF0 pop         {r4-r11}
  00403B5E: F85D FB14 ldr         pc,[sp],#0x14
  00403B62: F24C 0308 mov         r3,#0xC008
  00403B66: F2C0 0340 movt        r3,#0x40
  00403B6A: 9A25      ldr         r2,[sp,#0x94]
  00403B6C: 9924      ldr         r1,[sp,#0x90]
  00403B70: 9811      ldr         r0,[sp,#0x44]
  00403B72: 4798      blx         r3
  00403B74: 2800      cmp         r0,#0
  00403B76: D0ED      beq         00403B54
  00403B78: 9911      ldr         r1,[sp,#0x44]
  00403B7A: F645 234D mov         r3,#0x5A4D
  00403B7E: 880A      ldrh        r2,[r1]
  00403B80: 429A      cmp         r2,r3
  00403B82: D1E7      bne         00403B54
  00403B84: 6BCB      ldr         r3,[r1,#0x3C]
  00403B86: 2B00      cmp         r3,#0
  00403B88: DDE4      ble         00403B54
  00403B8A: 585A      ldr         r2,[r3,r1]
  00403B8C: 185C      adds        r4,r3,r1
  00403B8E: F244 5350 mov         r3,#0x4550
  00403B92: 429A      cmp         r2,r3
  00403B94: D1DE      bne         00403B54
  00403B96: 8AA3      ldrh        r3,[r4,#0x14]
  00403B98: 88E0      ldrh        r0,[r4,#6]
  00403B9A: 1A69      subs        r1,r5,r1
  00403B9C: 4423      add         r3,r3,r4
  00403B9E: 2700      movs        r7,#0
  00403BA0: 2500      movs        r5,#0
  00403BA2: B160      cbz         r0,00403BBE
  00403BA4: F103 0218 add         r2,r3,#0x18
  00403BA8: 68D3      ldr         r3,[r2,#0xC]
  00403BAA: 4299      cmp         r1,r3
  00403BAC: D303      bcc         00403BB6
  00403BAE: 1ACF      subs        r7,r1,r3
  00403BB0: 6893      ldr         r3,[r2,#8]
  00403BB2: 4299      cmp         r1,r3
  00403BB4: D303      bcc         00403BBE
  00403BB6: 1C6D      adds        r5,r5,#1
  00403BB8: 3228      adds        r2,r2,#0x28
  00403BBA: 4285      cmp         r5,r0
  00403BBC: D3F4      bcc         00403BA8
  00403BBE: 4285      cmp         r5,r0
  00403BC0: D0C8      beq         00403B54
  00403BC2: F24A 1671 mov         r6,#0xA171
  00403BC6: F2C0 0640 movt        r6,#0x40
  00403BCA: F24A 146C mov         r4,#0xA16C
  00403BCE: F2C0 0440 movt        r4,#0x40
  00403BD2: 1C6D      adds        r5,r5,#1
  00403BD4: 7833      ldrb        r3,[r6]
  00403BD6: B953      cbnz        r3,00403BEE
  00403BDA: 2B00      cmp         r3,#0
  00403BDC: D1BA      bne         00403B54
  00403BDE: F7FF FDAF bl          00403740
  00403BE2: 6020      str         r0,[r4]
  00403BE4: 2800      cmp         r0,#0
  00403BE6: D0B5      beq         00403B54
  00403BE8: 2301      movs        r3,#1
  00403BEA: 7033      strb        r3,[r6]
  00403BEC: E000      b           00403BF0
  00403BF0: F24C 0300 movw        r3,#0xC000
  00403BF4: F2C0 0340 movt        r3,#0x40
  00403BF8: F248 2164 mov         r1,#0x8264
  00403BFC: F2C0 0140 movt        r1,#0x40
  00403C02: 4798      blx         r3
  00403C04: 4604      mov         r4,r0
  00403C06: 2C00      cmp         r4,#0
  00403C08: D0A4      beq         00403B54
  00403C0A: AB08      add         r3,sp,#0x20
  00403C0C: 9303      str         r3,[sp,#0xC]
  00403C0E: 9824      ldr         r0,[sp,#0x90]
  00403C10: AB0F      add         r3,sp,#0x3C
  00403C12: 9300      str         r3,[sp]
  00403C14: 2300      movs        r3,#0
  00403C16: 2200      movs        r2,#0
  00403C18: 2100      movs        r1,#0
  00403C1A: F8CD 8008 str         r8,[sp,#8]
  00403C1E: F8CD 8004 str         r8,[sp,#4]
  00403C22: 47A0      blx         r4
  00403C24: 2800      cmp         r0,#0
  00403C26: D095      beq         00403B54
  00403C28: 9B08      ldr         r3,[sp,#0x20]
  00403C2A: 2600      movs        r6,#0
  00403C2C: 4618      mov         r0,r3
  00403C32: 4798      blx         r3
  00403C34: 4B6B      ldr         r3,00403DE4
  00403C36: 4298      cmp         r0,r3
  00403C38: F040 80C8 bne         00403DCC
  00403C3C: 9C08      ldr         r4,[sp,#0x20]
  00403C3E: F248 2278 mov         r2,#0x8278
  00403C42: F2C0 0240 movt        r2,#0x40
  00403C46: AB0A      add         r3,sp,#0x28
  00403C48: 4620      mov         r0,r4
  00403C4C: 2100      movs        r1,#0
  00403C4E: 69E4      ldr         r4,[r4,#0x1C]
  00403C50: 47A0      blx         r4
  00403C52: 2800      cmp         r0,#0
  00403C54: F000 80BA beq         00403DCC
  00403C58: 9C0A      ldr         r4,[sp,#0x28]
  00403C5A: AB07      add         r3,sp,#0x1C
  00403C5C: 463A      mov         r2,r7
  00403C5E: 4620      mov         r0,r4
  00403C62: B2A9      uxth        r1,r5
  00403C64: F8CD 8008 str         r8,[sp,#8]
  00403C68: 6A24      ldr         r4,[r4,#0x20]
  00403C6A: F8CD 8004 str         r8,[sp,#4]
  00403C6E: F8CD 8000 str         r8,[sp]
  00403C72: 47A0      blx         r4
  00403C74: 2800      cmp         r0,#0
  00403C76: F000 80A4 beq         00403DC2
  00403C7A: 2300      movs        r3,#0
  00403C7C: 9305      str         r3,[sp,#0x14]
  00403C7E: 9B07      ldr         r3,[sp,#0x1C]
  00403C80: A905      add         r1,sp,#0x14
  00403C82: 4618      mov         r0,r3
  00403C86: 6E9B      ldr         r3,[r3,#0x68]
  00403C88: 4798      blx         r3
  00403C8A: 2800      cmp         r0,#0
  00403C8C: F000 8094 beq         00403DB8
  00403C90: 9B05      ldr         r3,[sp,#0x14]
  00403C92: 2B00      cmp         r3,#0
  00403C94: F000 8090 beq         00403DB8
  00403C98: 4618      mov         r0,r3
  00403C9C: 46B0      mov         r8,r6
  00403C9E: 689B      ldr         r3,[r3,#8]
  00403CA0: 4798      blx         r3
  00403CA2: 2800      cmp         r0,#0
  00403CA4: D075      beq         00403D92
  00403CA6: F10D 0918 add         r9,sp,#0x18
  00403CAA: F10D 0A2C add         r10,sp,#0x2C
  00403CAE: 9C05      ldr         r4,[sp,#0x14]
  00403CB0: 2300      movs        r3,#0
  00403CB2: 9302      str         r3,[sp,#8]
  00403CB4: 4620      mov         r0,r4
  00403CB8: AB04      add         r3,sp,#0x10
  00403CBA: AA09      add         r2,sp,#0x24
  00403CBC: 68E4      ldr         r4,[r4,#0xC]
  00403CBE: 2100      movs        r1,#0
  00403CC0: F8CD 9004 str         r9,[sp,#4]
  00403CC4: F8CD A000 str         r10,[sp]
  00403CC8: 47A0      blx         r4
  00403CCA: 2800      cmp         r0,#0
  00403CCC: D06F      beq         00403DAE
  00403CCE: F8BD 3010 ldrh        r3,[sp,#0x10]
  00403CD2: 42AB      cmp         r3,r5
  00403CD4: D106      bne         00403CE4
  00403CD6: 9A09      ldr         r2,[sp,#0x24]
  00403CD8: 42BA      cmp         r2,r7
  00403CDA: D803      bhi         00403CE4
  00403CDC: 9B0B      ldr         r3,[sp,#0x2C]
  00403CDE: 4413      add         r3,r3,r2
  00403CE0: 429F      cmp         r7,r3
  00403CE2: D307      bcc         00403CF4
  00403CE4: 9B05      ldr         r3,[sp,#0x14]
  00403CE6: 4618      mov         r0,r3
  00403CEA: 689B      ldr         r3,[r3,#8]
  00403CEC: 4798      blx         r3
  00403CEE: 2800      cmp         r0,#0
  00403CF0: D1DD      bne         00403CAE
  00403CF2: E04E      b           00403D92
  00403CF4: 9A06      ldr         r2,[sp,#0x18]
  00403CF6: 2A00      cmp         r2,#0
  00403CF8: D059      beq         00403DAE
  00403CFA: F06F 4360 mvn         r3,#0xE0000000
  00403CFE: 429A      cmp         r2,r3
  00403D00: D255      bcs         00403DAE
  00403D02: F24C 0314 mov         r3,#0xC014
  00403D06: F2C0 0340 movt        r3,#0x40
  00403D0C: 4798      blx         r3
  00403D0E: 9B06      ldr         r3,[sp,#0x18]
  00403D10: 2100      movs        r1,#0
  00403D12: 00DA      lsls        r2,r3,#3
  00403D14: F24C 031C mov         r3,#0xC01C
  00403D18: F2C0 0340 movt        r3,#0x40
  00403D1E: 4798      blx         r3
  00403D20: 4680      mov         r8,r0
  00403D22: 2800      cmp         r0,#0
  00403D24: D043      beq         00403DAE
  00403D26: 9C05      ldr         r4,[sp,#0x14]
  00403D28: 9002      str         r0,[sp,#8]
  00403D2A: AB06      add         r3,sp,#0x18
  00403D2C: 4620      mov         r0,r4
  00403D30: 9301      str         r3,[sp,#4]
  00403D32: 2500      movs        r5,#0
  00403D34: 68E4      ldr         r4,[r4,#0xC]
  00403D36: 2300      movs        r3,#0
  00403D38: 2200      movs        r2,#0
  00403D3A: A90E      add         r1,sp,#0x38
  00403D3C: 9500      str         r5,[sp]
  00403D3E: 47A0      blx         r4
  00403D40: B338      cbz         r0,00403D92
  00403D42: 9B09      ldr         r3,[sp,#0x24]
  00403D44: 1AFC      subs        r4,r7,r3
  00403D46: F8D8 3000 ldr         r3,[r8]
  00403D4A: 429C      cmp         r4,r3
  00403D4C: D321      bcc         00403D92
  00403D4E: 9806      ldr         r0,[sp,#0x18]
  00403D50: 2201      movs        r2,#1
  00403D52: 2801      cmp         r0,#1
  00403D54: D908      bls         00403D68
  00403D56: F108 0108 add         r1,r8,#8
  00403D5A: F851 3B08 ldr         r3,[r1],#8
  00403D5E: 429C      cmp         r4,r3
  00403D60: D302      bcc         00403D68
  00403D62: 1C52      adds        r2,r2,#1
  00403D64: 4282      cmp         r2,r0
  00403D66: D3F8      bcc         00403D5A
  00403D68: EB08 03C2 add         r3,r8,r2,lsl #3
  00403D6C: F853 3C04 ldr         r3,[r3,#-4]
  00403D70: 9A0C      ldr         r2,[sp,#0x30]
  00403D72: 9C07      ldr         r4,[sp,#0x1C]
  00403D74: F023 437F bic         r3,r3,#0xFF000000
  00403D78: 6013      str         r3,[r2]
  00403D7A: 4620      mov         r0,r4
  00403D7E: 9A0D      ldr         r2,[sp,#0x34]
  00403D80: 990E      ldr         r1,[sp,#0x38]
  00403D82: 6F24      ldr         r4,[r4,#0x70]
  00403D84: AB22      add         r3,sp,#0x88
  00403D86: 9502      str         r5,[sp,#8]
  00403D88: 9501      str         r5,[sp,#4]
  00403D8A: 9500      str         r5,[sp]
  00403D8C: 47A0      blx         r4
  00403D8E: B100      cbz         r0,00403D92
  00403D90: 2601      movs        r6,#1
  00403D92: F24C 0314 mov         r3,#0xC014
  00403D96: F2C0 0340 movt        r3,#0x40
  00403D9C: 4798      blx         r3
  00403D9E: F24C 0318 mov         r3,#0xC018
  00403DA2: F2C0 0340 movt        r3,#0x40
  00403DA6: 4642      mov         r2,r8
  00403DA8: 2100      movs        r1,#0
  00403DAC: 4798      blx         r3
  00403DAE: 9B05      ldr         r3,[sp,#0x14]
  00403DB0: 4618      mov         r0,r3
  00403DB6: 4798      blx         r3
  00403DB8: 9B07      ldr         r3,[sp,#0x1C]
  00403DBA: 4618      mov         r0,r3
  00403DBE: 6C1B      ldr         r3,[r3,#0x40]
  00403DC0: 4798      blx         r3
  00403DC2: 9B0A      ldr         r3,[sp,#0x28]
  00403DC4: 4618      mov         r0,r3
  00403DC8: 6B9B      ldr         r3,[r3,#0x38]
  00403DCA: 4798      blx         r3
  00403DCC: 9B08      ldr         r3,[sp,#0x20]
  00403DCE: 4618      mov         r0,r3
  00403DD2: 6ADB      ldr         r3,[r3,#0x2C]
  00403DD4: 4798      blx         r3
  00403DD6: 4630      mov         r0,r6
  00403DD8: B017      add         sp,sp,#0x5C
  00403DDA: E8BD 0FF0 pop         {r4-r11}
  00403DDE: F85D FB14 ldr         pc,[sp],#0x14
  00403DE2: DEFE      __debugbreak
  00403DE4: 9141      str         r1,[sp,#0x104]
  00403DE6: 0132      lsls        r2,r6,#4
  00403DE8: 0000      movs        r0,r0
  00403DEA: 0000      movs        r0,r0
  00403DEC: 0000      movs        r0,r0
  00403DEE: 0000      movs        r0,r0
  00403DF0: 0000      movs        r0,r0
  00403DF2: 0000      movs        r0,r0
  00403DF4: 0000      movs        r0,r0
  00403DF6: 0000      movs        r0,r0
  00403DF8: 0000      movs        r0,r0
  00403DFA: 0000      movs        r0,r0
  00403DFC: 0000      movs        r0,r0
  00403DFE: 0000      movs        r0,r0
  00403E00: F24C 0CA8 mov         r12,#0xC0A8
  00403E04: F2C0 0C40 movt        r12,#0x40
  00403E08: F8DC F000 ldr         pc,[r12]
  00403E0C: B40F      push        {r0-r3}
  00403E0E: E92D 4800 push        {r11,lr}
  00403E12: 46EB      mov         r11,sp
  00403E14: B082      sub         sp,sp,#8
  00403E16: 9B07      ldr         r3,[sp,#0x1C]
  00403E18: 331C      adds        r3,r3,#0x1C
  00403E1C: 9907      ldr         r1,[sp,#0x1C]
  00403E1E: 9805      ldr         r0,[sp,#0x14]
  00403E20: F000 F814 bl          00403E4C
  00403E24: 2301      movs        r3,#1
  00403E26: 9300      str         r3,[sp]
  00403E28: 9800      ldr         r0,[sp]
  00403E2A: B002      add         sp,sp,#8
  00403E2C: F85D BB04 pop         {r11}
  00403E30: F85D FB14 ldr         pc,[sp],#0x14

  00403E4C: B40F      push        {r0-r3}
  00403E4E: E92D 4800 push        {r11,lr}
  00403E52: 46EB      mov         r11,sp
  00403E54: B084      sub         sp,sp,#0x10
  00403E56: 9B08      ldr         r3,[sp,#0x20]
  00403E5A: F023 0303 bic         r3,r3,#3
  00403E5E: 9302      str         r3,[sp,#8]
  00403E60: 9B06      ldr         r3,[sp,#0x18]
  00403E62: 9301      str         r3,[sp,#4]
  00403E64: 9A01      ldr         r2,[sp,#4]
  00403E66: 9B02      ldr         r3,[sp,#8]
  00403E68: 4413      add         r3,r3,r2
  00403E6C: 9300      str         r3,[sp]
  00403E6E: 9B08      ldr         r3,[sp,#0x20]
  00403E72: F013 0F01 tst         r3,#1
  00403E76: D005      beq         00403E84
  00403E78: 9A01      ldr         r2,[sp,#4]
  00403E7A: 9B02      ldr         r3,[sp,#8]
  00403E7C: 441A      add         r2,r2,r3
  00403E7E: 9B00      ldr         r3,[sp]
  00403E80: 1AD3      subs        r3,r2,r3
  00403E82: 9300      str         r3,[sp]
  00403E84: 9800      ldr         r0,[sp]
  00403E86: F7FD F90B bl          004010A0
  00403E8A: B004      add         sp,sp,#0x10
  00403E8C: F85D BB04 pop         {r11}
  00403E90: F85D FB14 ldr         pc,[sp],#0x14

  00403EAC: F24C 0CAC mov         r12,#0xC0AC
  00403EB0: F2C0 0C40 movt        r12,#0x40
  00403EB4: F8DC F000 ldr         pc,[r12]
  00403EB8: F24C 0CB0 mov         r12,#0xC0B0
  00403EBC: F2C0 0C40 movt        r12,#0x40
  00403EC0: F8DC F000 ldr         pc,[r12]
  00403EC4: F24C 0CB4 mov         r12,#0xC0B4
  00403EC8: F2C0 0C40 movt        r12,#0x40
  00403ECC: F8DC F000 ldr         pc,[r12]
  00403ED0: F24C 0CB8 mov         r12,#0xC0B8
  00403ED4: F2C0 0C40 movt        r12,#0x40
  00403ED8: F8DC F000 ldr         pc,[r12]
  00403EDC: B403      push        {r0,r1}
  00403EDE: E92D 4800 push        {r11,lr}
  00403EE2: 46EB      mov         r11,sp
  00403EE4: 2002      movs        r0,#2
  00403EE6: DEFB      __fastfail
  00403EE8: E92D 4800 push        {r11,lr}
  00403EEC: 46EB      mov         r11,sp
  00403EEE: 2008      movs        r0,#8
  00403EF0: F000 F80E bl          00403F10
  00403EF4: DEFE      __debugbreak

  00403F10: B403      push        {r0,r1}
  00403F12: E92D 4800 push        {r11,lr}
  00403F16: 46EB      mov         r11,sp
  00403F18: 9802      ldr         r0,[sp,#8]
  00403F1A: DEFB      __fastfail
  00403F1C: B40F      push        {r0-r3}
  00403F1E: E92D 4800 push        {r11,lr}
  00403F22: 46EB      mov         r11,sp
  00403F24: 9802      ldr         r0,[sp,#8]
  00403F26: DEFB      __fastfail
  00403F28: F24C 0C4C mov         r12,#0xC04C
  00403F2C: F2C0 0C40 movt        r12,#0x40
  00403F30: F8DC F000 ldr         pc,[r12]
  00403F34: F24C 0C48 mov         r12,#0xC048
  00403F38: F2C0 0C40 movt        r12,#0x40
  00403F3C: F8DC F000 ldr         pc,[r12]
  00403F40: F24C 0C44 mov         r12,#0xC044
  00403F44: F2C0 0C40 movt        r12,#0x40
  00403F48: F8DC F000 ldr         pc,[r12]
  00403F4C: F24C 0C40 mov         r12,#0xC040
  00403F50: F2C0 0C40 movt        r12,#0x40
  00403F54: F8DC F000 ldr         pc,[r12]
  00403F58: F24C 0C3C mov         r12,#0xC03C
  00403F5C: F2C0 0C40 movt        r12,#0x40
  00403F60: F8DC F000 ldr         pc,[r12]
  00403F64: F24C 0C38 mov         r12,#0xC038
  00403F68: F2C0 0C40 movt        r12,#0x40
  00403F6C: F8DC F000 ldr         pc,[r12]
  00403F70: F24C 0C34 mov         r12,#0xC034
  00403F74: F2C0 0C40 movt        r12,#0x40
  00403F78: F8DC F000 ldr         pc,[r12]
  00403F7C: F24C 0C30 mov         r12,#0xC030
  00403F80: F2C0 0C40 movt        r12,#0x40
  00403F84: F8DC F000 ldr         pc,[r12]
  00403F88: F24C 0C2C mov         r12,#0xC02C
  00403F8C: F2C0 0C40 movt        r12,#0x40
  00403F90: F8DC F000 ldr         pc,[r12]
  00403F94: F24C 0C28 mov         r12,#0xC028
  00403F98: F2C0 0C40 movt        r12,#0x40
  00403F9C: F8DC F000 ldr         pc,[r12]
  00403FA0: F24C 0C24 mov         r12,#0xC024
  00403FA4: F2C0 0C40 movt        r12,#0x40
  00403FA8: F8DC F000 ldr         pc,[r12]
  00403FAC: F24C 0C20 mov         r12,#0xC020
  00403FB0: F2C0 0C40 movt        r12,#0x40
  00403FB4: F8DC F000 ldr         pc,[r12]
  00403FB8: F24C 0C1C mov         r12,#0xC01C
  00403FBC: F2C0 0C40 movt        r12,#0x40
  00403FC0: F8DC F000 ldr         pc,[r12]
  00403FC4: F24C 0C18 mov         r12,#0xC018
  00403FC8: F2C0 0C40 movt        r12,#0x40
  00403FCC: F8DC F000 ldr         pc,[r12]
  00403FD0: F24C 0C14 mov         r12,#0xC014
  00403FD4: F2C0 0C40 movt        r12,#0x40
  00403FD8: F8DC F000 ldr         pc,[r12]
  00403FDC: F24C 0C10 mov         r12,#0xC010
  00403FE0: F2C0 0C40 movt        r12,#0x40
  00403FE4: F8DC F000 ldr         pc,[r12]
  00403FE8: F24C 0C0C mov         r12,#0xC00C
  00403FEC: F2C0 0C40 movt        r12,#0x40
  00403FF0: F8DC F000 ldr         pc,[r12]
  00403FF4: F24C 0C08 mov         r12,#0xC008
  00403FF8: F2C0 0C40 movt        r12,#0x40
  00403FFC: F8DC F000 ldr         pc,[r12]
  00404000: F24C 0C04 mov         r12,#0xC004
  00404004: F2C0 0C40 movt        r12,#0x40
  00404008: F8DC F000 ldr         pc,[r12]
  0040400C: F24C 0C00 movw        r12,#0xC000
  00404010: F2C0 0C40 movt        r12,#0x40
  00404014: F8DC F000 ldr         pc,[r12]
  00404018: F24C 0C50 mov         r12,#0xC050
  0040401C: F2C0 0C40 movt        r12,#0x40
  00404020: F8DC F000 ldr         pc,[r12]

  004057A8: B510      push        {r4,lr}
  004057AA: B082      sub         sp,sp,#8
  004057AC: 6038      str         r0,[r7]
  004057B0: 637B      str         r3,[r7,#0x34]
  004057B8: 63FB      str         r3,[r7,#0x3C]
  004057BA: 6B79      ldr         r1,[r7,#0x34]
  004057BC: 6BF8      ldr         r0,[r7,#0x3C]
  004057BE: F7FD F907 bl          004029D0
  004057C2: 6478      str         r0,[r7,#0x44]
  004057C4: 6C78      ldr         r0,[r7,#0x44]
  004057C6: B002      add         sp,sp,#8
  004057C8: BD10      pop         {r4,pc}

  004057E4: 62F8      str         r0,[r7,#0x2C]
  004057E6: 6AFB      ldr         r3,[r7,#0x2C]
  004057EC: 633B      str         r3,[r7,#0x30]
  004057EE: 6B3A      ldr         r2,[r7,#0x30]
  004057F0: 4B04      ldr         r3,00405804
  004057F2: 429A      cmp         r2,r3
  004057F4: D102      bne         004057FC
  004057F6: 2301      movs        r3,#1
  004057F8: 613B      str         r3,[r7,#0x10]
  004057FA: E001      b           00405800
  004057FC: 2300      movs        r3,#0
  004057FE: 613B      str         r3,[r7,#0x10]
  00405800: 6938      ldr         r0,[r7,#0x10]
  00405802: 4770      bx          lr
  00405804: 0005      movs        r5,r0
  00405806: C000      ?stm        r0!,{}
  00405808: B510      push        {r4,lr}
  0040580A: 2008      movs        r0,#8
  0040580C: F7FD FF86 bl          0040371C
  00405810: E7FF      b           00405812
  00405812: BD10      pop         {r4,pc}

  00405830: 4B03      ldr         r3,00405840
  00405832: 429A      cmp         r2,r3
  00405834: D101      bne         0040583A
  00405836: 2001      movs        r0,#1
  00405838: E000      b           0040583C
  0040583A: 2000      movs        r0,#0
  0040583C: 4770      bx          lr
  0040583E: DEFE      __debugbreak
  00405840: 1388      asrs        r0,r1,#0xE
  00405842: 406D      eors        r5,r5,r5
  00405848: 4B03      ldr         r3,00405858
  0040584A: 429A      cmp         r2,r3
  0040584C: D101      bne         00405852
  0040584E: 2001      movs        r0,#1
  00405850: E000      b           00405854
  00405852: 2000      movs        r0,#0
  00405854: 4770      bx          lr
  00405856: DEFE      __debugbreak
  00405858: 1388      asrs        r0,r1,#0xE
  0040585A: 406D      eors        r5,r5,r5
  0040585C: 0000      movs        r0,r0
";
        public const string oThumbBlock =
@"  00401000: 0000      movs        r0,r0
  00401002: 0000      movs        r0,r0
  00401004: 0000      movs        r0,r0
  00401006: 0000      movs        r0,r0
  00401008: 0000      movs        r0,r0
  0040100A: 0000      movs        r0,r0
  0040100C: F242 1C21 mov         r12,#0x2121
  00401010: F2C0 0C40 movt        r12,#0x40
  00401014: 4760      bx          r12

  00401024: B081      sub         sp,sp,#4
  00401026: F24A 0C20 mov         r12,#0xA020
  0040102A: F2C0 0C40 movt        r12,#0x40
  0040102E: F8DC C000 ldr         r12,[r12]
  00401032: EBAD 0C0C sub         r12,sp,r12
  00401036: F8CD C000 str         r12,[sp]
  0040103A: 4770      bx          lr
  0040103C: F24A 0C20 mov         r12,#0xA020
  00401040: F2C0 0C40 movt        r12,#0x40
  00401044: 9B00      ldr         r3,[sp]
  00401046: F8DC C000 ldr         r12,[r12]
  0040104A: EBAD 0303 sub         r3,sp,r3
  0040104E: 4563      cmp         r3,r12
  00401050: D101      bne         00401056
  00401052: B001      add         sp,sp,#4
  00401054: 4770      bx          lr

  00401056: 4618      mov         r0,r3
  00401058: B510      push        {r4,lr}
  0040105A: 466C      mov         r4,sp
  0040105C: 46EC      mov         r12,sp
  0040105E: F02C 0C07 bic         r12,r12,#7
  00401062: 46E5      mov         sp,r12
  00401064: F000 F810 bl          00401088
  00401068: 46A5      mov         sp,r4
  0040106A: E8BD 4010 pop         {r4,lr}
  0040106E: 4770      bx          lr

  00401088: F8DF C00C ldr         r12,00401098
  0040108C: F8DC C000 ldr         r12,[r12]
  00401090: 4560      cmp         r0,r12
  00401092: D103      bne         0040109C
  00401094: 4770      bx          lr
  00401096: 0000      movs        r0,r0
  00401098: A020      adr         r0,0040111C
  0040109A: 0040      lsls        r0,r0,#1
  0040109C: B508      push        {r3,lr}
  0040109E: B082      sub         sp,sp,#8
  004010A0: F8DF C01C ldr         r12,004010C0
  004010A4: F8DC C000 ldr         r12,[r12]
  004010A8: F8CD C004 str         r12,[sp,#4]
  004010AC: F8DF C014 ldr         r12,004010C4
  004010B0: F8DC C000 ldr         r12,[r12]
  004010B4: F8CD C000 str         r12,[sp]
  004010B8: F002 FEA0 bl          00403DFC
  004010BC: DEFE      __debugbreak
  004010BE: 0000      movs        r0,r0
  004010C0: A024      adr         r0,00401154
  004010C2: 0040      lsls        r0,r0,#1
  004010C4: A020      adr         r0,00401148
  004010C6: 0040      lsls        r0,r0,#1
  
  00402120: B403      push        {r0,r1}
  00402122: E92D 4830 push        {r4,r5,r11,lr}
  00402126: F10D 0B08 add         r11,sp,#8
  0040212A: B082      sub         sp,sp,#8
  0040212C: F04F 30CC mov         r0,#0xCCCCCCCC
  00402130: 9000      str         r0,[sp]
  00402132: 9001      str         r0,[sp,#4]
  00402134: 4806      ldr         r0,00402150
  00402136: 4B05      ldr         r3,0040214C
  0040213A: 4798      blx         r3
  0040213C: 2300      movs        r3,#0
  0040213E: 9300      str         r3,[sp]
  00402140: 9800      ldr         r0,[sp]
  00402142: B002      add         sp,sp,#8
  00402144: E8BD 0830 pop         {r4,r5,r11}
  00402148: F85D FB0C ldr         pc,[sp],#0xC
  0040214C: C108      stm         r1!,{r3}
  0040214E: 0040      lsls        r0,r0,#1
  00402150: 7858      ldrb        r0,[r3,#1]
  00402152: 0040      lsls        r0,r0,#1

  00402168: F2C0 0C40 movt        r12,#0x40
  0040216C: F8DC F000 ldr         pc,[r12]
  00402170: E92D 480C push        {r2,r3,r11,lr}
  00402174: F10D 0B08 add         r11,sp,#8
  00402178: F24A 1230 mov         r2,#0xA130
  0040217C: F2C0 0240 movt        r2,#0x40
  00402180: 7813      ldrb        r3,[r2]
  00402182: B95B      cbnz        r3,0040219C
  00402184: 2301      movs        r3,#1
  00402186: 7013      strb        r3,[r2]
  00402188: 2300      movs        r3,#0
  0040218A: 9300      str         r3,[sp]
  0040218C: 2301      movs        r3,#1
  0040218E: 2200      movs        r2,#0
  00402190: 2100      movs        r1,#0
  00402192: 2000      movs        r0,#0
  00402194: F000 FB3A bl          0040280C
  00402198: F000 FB16 bl          004027C8
  0040219C: E8BD 880C pop         {r2,r3,r11,pc}

  004021D0: E92D 480C push        {r2,r3,r11,lr}
  004021D4: F10D 0B08 add         r11,sp,#8
  004021D8: 2301      movs        r3,#1
  004021DA: 2200      movs        r2,#0
  004021DC: 2100      movs        r1,#0
  004021DE: 2000      movs        r0,#0
  004021E0: 9300      str         r3,[sp]
  004021E2: F000 FB13 bl          0040280C
  004021E6: E8BD 880C pop         {r2,r3,r11,pc}

  00402204: E92D 4800 push        {r11,lr}
  00402208: 46EB      mov         r11,sp
  0040220A: B084      sub         sp,sp,#0x10
  0040220C: F000 FA34 bl          00402678
  00402210: 9000      str         r0,[sp]
  00402212: 9A00      ldr         r2,[sp]
  00402214: 4B29      ldr         r3,004022BC
  00402216: 601A      str         r2,[r3]
  00402218: 2001      movs        r0,#1
  0040221A: 4B27      ldr         r3,004022B8
  0040221E: 4798      blx         r3
  00402220: F06F 0000 mvn         r0,#0
  00402224: 4B23      ldr         r3,004022B4
  00402228: 4798      blx         r3
  0040222A: 9001      str         r0,[sp,#4]
  0040222C: 9A01      ldr         r2,[sp,#4]
  0040222E: 4B20      ldr         r3,004022B0
  00402230: 601A      str         r2,[r3]
  00402232: 4B1F      ldr         r3,004022B0
  00402236: 4B1D      ldr         r3,004022AC
  00402238: 601A      str         r2,[r3]
  0040223A: 4B1B      ldr         r3,004022A8
  0040223E: 4B19      ldr         r3,004022A4
  00402242: 601A      str         r2,[r3]
  00402244: 4B16      ldr         r3,004022A0
  00402248: 4B14      ldr         r3,0040229C
  0040224C: 601A      str         r2,[r3]
  0040224E: F000 FDEB bl          00402E28
  00402252: F000 FB59 bl          00402908
  00402256: 4B10      ldr         r3,00402298
  0040225A: 2B00      cmp         r3,#0
  0040225C: D103      bne         00402266
  0040225E: 480D      ldr         r0,00402294
  00402260: 4B0B      ldr         r3,00402290
  00402264: 4798      blx         r3
  00402266: 4B09      ldr         r3,0040228C
  0040226A: F1B3 3FFF cmp         r3,#0xFFFFFFFF
  0040226E: D104      bne         0040227A
  00402270: F06F 0000 mvn         r0,#0
  00402274: 4B04      ldr         r3,00402288
  00402278: 4798      blx         r3
  0040227A: 2300      movs        r3,#0
  0040227C: 9302      str         r3,[sp,#8]
  0040227E: 9802      ldr         r0,[sp,#8]
  00402280: B004      add         sp,sp,#0x10
  00402282: E8BD 8800 pop         {r11,pc}
  00402286: DEFE      __debugbreak
  00402288: C0D8      stm         r0!,{r3,r4,r6,r7}
  0040228A: 0040      lsls        r0,r0,#1
  0040228C: A004      adr         r0,004022A0
  0040228E: 0040      lsls        r0,r0,#1
  00402290: C0D4      stm         r0!,{r2,r4,r6,r7}
  00402292: 0040      lsls        r0,r0,#1
  00402294: 2BE9      cmp         r3,#0xE9
  00402296: 0040      lsls        r0,r0,#1
  00402298: A000      adr         r0,0040229C
  0040229A: 0040      lsls        r0,r0,#1
  0040229C: C0BC      stm         r0!,{r2-r5,r7}
  0040229E: 0040      lsls        r0,r0,#1
  004022A0: A15C      adr         r1,00402414
  004022A2: 0040      lsls        r0,r0,#1
  004022A4: C0C0      stm         r0!,{r6,r7}
  004022A6: 0040      lsls        r0,r0,#1
  004022A8: A168      adr         r1,0040244C
  004022AA: 0040      lsls        r0,r0,#1
  004022AC: A38C      adr         r3,004024E0
  004022AE: 0040      lsls        r0,r0,#1
  004022B0: A37C      adr         r3,004024A4
  004022B2: 0040      lsls        r0,r0,#1
  004022B4: C04C      stm         r0!,{r2,r3,r6}
  004022B6: 0040      lsls        r0,r0,#1
  004022B8: C0F0      stm         r0!,{r4-r7}
  004022BA: 0040      lsls        r0,r0,#1
  004022BC: A13C      adr         r1,004023B0
  004022BE: 0040      lsls        r0,r0,#1

  00402308: E92D 4810 push        {r4,r11,lr}
  0040230C: F10D 0B04 add         r11,sp,#4
  00402310: B083      sub         sp,sp,#0xC
  00402312: 4817      ldr         r0,00402370
  00402314: F000 FD62 bl          00402DDC
  00402318: 4B14      ldr         r3,0040236C
  0040231C: 4B12      ldr         r3,00402368
  0040231E: 601A      str         r2,[r3]
  00402320: 4B11      ldr         r3,00402368
  00402322: 9300      str         r3,[sp]
  00402324: 4B0F      ldr         r3,00402364
  00402328: 4A0D      ldr         r2,00402360
  0040232A: 490C      ldr         r1,0040235C
  0040232C: 480A      ldr         r0,00402358
  0040232E: 4C09      ldr         r4,00402354
  00402332: 47A0      blx         r4
  00402334: 9002      str         r0,[sp,#8]
  00402336: 9A02      ldr         r2,[sp,#8]
  00402338: 4B05      ldr         r3,00402350
  0040233A: 601A      str         r2,[r3]
  0040233C: 4B04      ldr         r3,00402350
  00402340: 2B00      cmp         r3,#0
  00402342: DA02      bge         0040234A
  00402344: 2008      movs        r0,#8
  00402346: F000 FAD9 bl          004028FC
  0040234A: B003      add         sp,sp,#0xC
  0040234C: E8BD 8810 pop         {r4,r11,pc}
  00402350: A14C      adr         r1,00402484
  00402352: 0040      lsls        r0,r0,#1
  00402354: C0F4      stm         r0!,{r2,r4-r7}
  00402356: 0040      lsls        r0,r0,#1
  00402358: A140      adr         r1,0040245C
  0040235A: 0040      lsls        r0,r0,#1
  0040235C: A144      adr         r1,00402470
  0040235E: 0040      lsls        r0,r0,#1
  00402360: A148      adr         r1,00402484
  00402362: 0040      lsls        r0,r0,#1
  00402364: A160      adr         r1,004024E8
  00402366: 0040      lsls        r0,r0,#1
  00402368: A150      adr         r1,004024AC
  0040236A: 0040      lsls        r0,r0,#1
  0040236C: A164      adr         r1,00402500
  0040236E: 0040      lsls        r0,r0,#1
  00402370: 2E59      cmp         r6,#0x59
  00402372: 0040      lsls        r0,r0,#1

  004023A4: E92D 4800 push        {r11,lr}
  004023A8: 46EB      mov         r11,sp
  004023AA: B082      sub         sp,sp,#8
  004023AC: EE1D 3F50 mrc         p15,#0,r3,c13,c0,#2
  004023B0: 9300      str         r3,[sp]
  004023B2: 9B00      ldr         r3,[sp]
  004023B4: 9301      str         r3,[sp,#4]
  004023B6: 9801      ldr         r0,[sp,#4]
  004023B8: B002      add         sp,sp,#8
  004023BA: E8BD 8800 pop         {r11,pc}
  004023BE: 0000      movs        r0,r0
  004023C0: E92D 4890 push        {r4,r7,r11,lr}
  004023C4: F10D 0B08 add         r11,sp,#8
  004023C8: B094      sub         sp,sp,#0x50
  004023CA: 466F      mov         r7,sp
  004023CC: B082      sub         sp,sp,#8
  004023CE: 2300      movs        r3,#0
  004023D0: 607B      str         r3,[r7,#4]
  004023D2: F7FF FFE7 bl          004023A4
  004023D6: 62B8      str         r0,[r7,#0x28]
  004023D8: 6ABB      ldr         r3,[r7,#0x28]
  004023DA: 1D1B      adds        r3,r3,#4
  004023DE: 60BB      str         r3,[r7,#8]
  004023E0: 2300      movs        r3,#0
  004023E2: 60FB      str         r3,[r7,#0xC]
  004023E4: F3BF 8F5B dmb         ish
  004023E8: 4875      ldr         r0,004025C0
  004023EA: 68B9      ldr         r1,[r7,#8]
  004023EC: E850 2F00 ldrex       r2,[r0]
  004023F0: 2A00      cmp         r2,#0
  004023F2: D103      bne         004023FC
  004023F4: E840 1300 strex       r3,r1,[r0]
  004023F8: 2B00      cmp         r3,#0
  004023FA: D1F7      bne         004023EC
  004023FC: 4613      mov         r3,r2
  004023FE: 643B      str         r3,[r7,#0x40]
  00402400: F3BF 8F5B dmb         ish
  00402404: 6C3B      ldr         r3,[r7,#0x40]
  00402406: 607B      str         r3,[r7,#4]
  00402408: 687B      ldr         r3,[r7,#4]
  0040240A: 2B00      cmp         r3,#0
  0040240C: D007      beq         0040241E
  0040240E: 687A      ldr         r2,[r7,#4]
  00402410: 68BB      ldr         r3,[r7,#8]
  00402412: 429A      cmp         r2,r3
  00402414: D102      bne         0040241C
  00402416: 2301      movs        r3,#1
  00402418: 60FB      str         r3,[r7,#0xC]
  0040241A: E000      b           0040241E
  0040241C: E7E2      b           004023E4
  0040241E: 4B6D      ldr         r3,004025D4
  00402422: 623B      str         r3,[r7,#0x20]
  00402424: F3BF 8F5B dmb         ish
  00402428: 6A3B      ldr         r3,[r7,#0x20]
  0040242A: 2B01      cmp         r3,#1
  0040242C: D103      bne         00402436
  0040242E: 201F      movs        r0,#0x1F
  00402430: F000 FA64 bl          004028FC
  00402434: E01D      b           00402472
  00402436: 4B67      ldr         r3,004025D4
  0040243A: 63BB      str         r3,[r7,#0x38]
  0040243C: F3BF 8F5B dmb         ish
  00402440: 6BBB      ldr         r3,[r7,#0x38]
  00402442: 2B00      cmp         r3,#0
  00402444: D112      bne         0040246C
  00402446: F3BF 8F5B dmb         ish
  0040244A: 4A62      ldr         r2,004025D4
  0040244C: 2301      movs        r3,#1
  0040244E: 6013      str         r3,[r2]
  00402450: 4964      ldr         r1,004025E4
  00402452: 4863      ldr         r0,004025E0
  00402454: F000 FD24 bl          00402EA0
  00402458: 6178      str         r0,[r7,#0x14]
  0040245A: 697B      ldr         r3,[r7,#0x14]
  0040245C: 2B00      cmp         r3,#0
  0040245E: D004      beq         0040246A
  00402460: 23FF      movs        r3,#0xFF
  00402462: 64BB      str         r3,[r7,#0x48]
  00402464: 6CBB      ldr         r3,[r7,#0x48]
  00402466: 613B      str         r3,[r7,#0x10]
  00402468: E08D      b           00402586
  0040246A: E002      b           00402472
  0040246C: 4A4A      ldr         r2,00402598
  0040246E: 2301      movs        r3,#1
  00402470: 6013      str         r3,[r2]
  00402472: 4B58      ldr         r3,004025D4
  00402476: 633B      str         r3,[r7,#0x30]
  00402478: F3BF 8F5B dmb         ish
  0040247C: 6B3B      ldr         r3,[r7,#0x30]
  0040247E: 2B01      cmp         r3,#1
  00402480: D108      bne         00402494
  00402482: 4956      ldr         r1,004025DC
  00402484: 4854      ldr         r0,004025D8
  00402486: F000 FD11 bl          00402EAC
  0040248A: F3BF 8F5B dmb         ish
  0040248E: 4A51      ldr         r2,004025D4
  00402490: 2302      movs        r3,#2
  00402492: 6013      str         r3,[r2]
  00402494: 4B4F      ldr         r3,004025D4
  00402498: 61BB      str         r3,[r7,#0x18]
  0040249A: F3BF 8F5B dmb         ish
  0040249E: 69BB      ldr         r3,[r7,#0x18]
  004024A0: 2B02      cmp         r3,#2
  004024A2: D010      beq         004024C6
  004024A4: 4B4A      ldr         r3,004025D0
  004024A6: 9301      str         r3,[sp,#4]
  004024A8: 4B48      ldr         r3,004025CC
  004024AA: 9300      str         r3,[sp]
  004024AC: 2300      movs        r3,#0
  004024AE: F240 2229 mov         r2,#0x229
  004024B2: 4945      ldr         r1,004025C8
  004024B4: 2002      movs        r0,#2
  004024B6: 4C43      ldr         r4,004025C4
  004024BA: 47A0      blx         r4
  004024BC: 61F8      str         r0,[r7,#0x1C]
  004024BE: 69FB      ldr         r3,[r7,#0x1C]
  004024C0: 2B01      cmp         r3,#1
  004024C2: D100      bne         004024C6
  004024C4: DEFE      __debugbreak
  004024C6: 68FB      ldr         r3,[r7,#0xC]
  004024C8: 2B00      cmp         r3,#0
  004024CA: D10B      bne         004024E4
  004024CC: F3BF 8F5B dmb         ish
  004024D0: 483B      ldr         r0,004025C0
  004024D2: 2100      movs        r1,#0
  004024D4: E850 2F00 ldrex       r2,[r0]
  004024D8: E840 1300 strex       r3,r1,[r0]
  004024DC: 2B00      cmp         r3,#0
  004024DE: D1F9      bne         004024D4
  004024E0: F3BF 8F5B dmb         ish
  004024E4: 4B35      ldr         r3,004025BC
  004024E8: 2B00      cmp         r3,#0
  004024EA: D00C      beq         00402506
  004024EC: 4833      ldr         r0,004025BC
  004024EE: F000 FA6D bl          004029CC
  004024F2: 6278      str         r0,[r7,#0x24]
  004024F4: 6A7B      ldr         r3,[r7,#0x24]
  004024F6: 2B00      cmp         r3,#0
  004024F8: D005      beq         00402506
  004024FA: 2200      movs        r2,#0
  004024FC: 2102      movs        r1,#2
  004024FE: 2000      movs        r0,#0
  00402500: 4B2E      ldr         r3,004025BC
  00402504: 4798      blx         r3
  00402506: 2001      movs        r0,#1
  00402508: 4B2B      ldr         r3,004025B8
  0040250C: 4798      blx         r3
  0040250E: 4B28      ldr         r3,004025B0
  00402512: 4B28      ldr         r3,004025B4
  00402516: 601A      str         r2,[r3]
  00402518: 4B25      ldr         r3,004025B0
  0040251C: 4B23      ldr         r3,004025AC
  00402520: 4B21      ldr         r3,004025A8
  00402524: F7FE FD72 bl          0040100C
  00402528: 62F8      str         r0,[r7,#0x2C]
  0040252A: 6AFA      ldr         r2,[r7,#0x2C]
  0040252C: 4B18      ldr         r3,00402590
  0040252E: 601A      str         r2,[r3]
  00402530: 4B1B      ldr         r3,004025A0
  00402534: 2B00      cmp         r3,#0
  00402536: D104      bne         00402542
  00402538: 4B15      ldr         r3,00402590
  0040253C: 4B19      ldr         r3,004025A4
  00402540: 4798      blx         r3
  00402542: 4B15      ldr         r3,00402598
  00402546: 2B00      cmp         r3,#0
  00402548: D102      bne         00402550
  0040254A: 4B12      ldr         r3,00402594
  0040254E: 4798      blx         r3
  00402550: E016      b           00402580
  00402552: 6038      str         r0,[r7]
  00402556: 64FB      str         r3,[r7,#0x4C]
  00402558: 6CFA      ldr         r2,[r7,#0x4C]
  0040255A: 4B0D      ldr         r3,00402590
  0040255C: 601A      str         r2,[r3]
  0040255E: 4B10      ldr         r3,004025A0
  00402562: 2B00      cmp         r3,#0
  00402564: D104      bne         00402570
  00402566: 4B0A      ldr         r3,00402590
  0040256A: 4B0C      ldr         r3,0040259C
  0040256E: 4798      blx         r3
  00402570: 4B09      ldr         r3,00402598
  00402574: 2B00      cmp         r3,#0
  00402576: D102      bne         0040257E
  00402578: 4B06      ldr         r3,00402594
  0040257C: 4798      blx         r3
  0040257E: E7FF      b           00402580
  00402580: 4B03      ldr         r3,00402590
  00402584: 613B      str         r3,[r7,#0x10]
  00402586: 6938      ldr         r0,[r7,#0x10]
  00402588: B016      add         sp,sp,#0x58
  0040258A: E8BD 8890 pop         {r4,r7,r11,pc}
  0040258E: DEFE      __debugbreak
  00402590: A138      adr         r1,00402674
  00402592: 0040      lsls        r0,r0,#1
  00402594: C0DC      stm         r0!,{r2-r4,r6,r7}
  00402596: 0040      lsls        r0,r0,#1
  00402598: A134      adr         r1,0040266C
  0040259A: 0040      lsls        r0,r0,#1
  0040259C: C0E0      stm         r0!,{r5-r7}
  0040259E: 0040      lsls        r0,r0,#1
  004025A0: A13C      adr         r1,00402694
  004025A2: 0040      lsls        r0,r0,#1
  004025A4: C0E4      stm         r0!,{r2,r5-r7}
  004025A6: 0040      lsls        r0,r0,#1
  004025A8: A140      adr         r1,004026AC
  004025AA: 0040      lsls        r0,r0,#1
  004025AC: A144      adr         r1,004026C0
  004025AE: 0040      lsls        r0,r0,#1
  004025B0: A148      adr         r1,004026D4
  004025B2: 0040      lsls        r0,r0,#1
  004025B4: C0C4      stm         r0!,{r2,r6,r7}
  004025B6: 0040      lsls        r0,r0,#1
  004025B8: C0E8      stm         r0!,{r3,r5-r7}
  004025BA: 0040      lsls        r0,r0,#1
  004025BC: A390      adr         r3,00402800
  004025BE: 0040      lsls        r0,r0,#1
  004025C0: A368      adr         r3,00402764
  004025C2: 0040      lsls        r0,r0,#1
  004025C4: C0EC      stm         r0!,{r2,r3,r5-r7}
  004025C6: 0040      lsls        r0,r0,#1
  004025C8: 78C0      ldrb        r0,[r0,#3]
  004025CA: 0040      lsls        r0,r0,#1
  004025CC: 78B8      ldrb        r0,[r7,#2]
  004025CE: 0040      lsls        r0,r0,#1
  004025D0: 7868      ldrb        r0,[r5,#1]
  004025D2: 0040      lsls        r0,r0,#1
  004025D4: A378      adr         r3,004027B8
  004025D6: 0040      lsls        r0,r0,#1
  004025D8: 7000      strb        r0,[r0]
  004025DA: 0040      lsls        r0,r0,#1
  004025DC: 7208      strb        r0,[r1,#8]
  004025DE: 0040      lsls        r0,r0,#1
  004025E0: 730C      strb        r4,[r1,#0xC]
  004025E2: 0040      lsls        r0,r0,#1
  004025E4: 771C      strb        r4,[r3,#0x1C]
  004025E6: 0040      lsls        r0,r0,#1

  00402678: E92D 4800 push        {r11,lr}
  0040267C: 46EB      mov         r11,sp
  0040267E: B084      sub         sp,sp,#0x10
  00402680: 4B1E      ldr         r3,004026FC
  00402682: 9302      str         r3,[sp,#8]
  00402684: 9B02      ldr         r3,[sp,#8]
  00402686: 881A      ldrh        r2,[r3]
  00402688: F645 234D mov         r3,#0x5A4D
  0040268C: 429A      cmp         r2,r3
  0040268E: D002      beq         00402696
  00402690: 2300      movs        r3,#0
  00402692: 9300      str         r3,[sp]
  00402694: E02D      b           004026F2
  00402696: 9B02      ldr         r3,[sp,#8]
  00402698: 333C      adds        r3,r3,#0x3C
  0040269A: 9A02      ldr         r2,[sp,#8]
  0040269E: 4413      add         r3,r3,r2
  004026A0: 9301      str         r3,[sp,#4]
  004026A2: 9B01      ldr         r3,[sp,#4]
  004026A6: F244 5350 mov         r3,#0x4550
  004026AA: 429A      cmp         r2,r3
  004026AC: D002      beq         004026B4
  004026AE: 2300      movs        r3,#0
  004026B0: 9300      str         r3,[sp]
  004026B2: E01E      b           004026F2
  004026B4: 9B01      ldr         r3,[sp,#4]
  004026B6: 3318      adds        r3,r3,#0x18
  004026B8: 881A      ldrh        r2,[r3]
  004026BA: F240 130B mov         r3,#0x10B
  004026BE: 429A      cmp         r2,r3
  004026C0: D002      beq         004026C8
  004026C2: 2300      movs        r3,#0
  004026C4: 9300      str         r3,[sp]
  004026C6: E014      b           004026F2
  004026C8: 9B01      ldr         r3,[sp,#4]
  004026CA: 3374      adds        r3,r3,#0x74
  004026CE: 2B0E      cmp         r3,#0xE
  004026D0: D802      bhi         004026D8
  004026D2: 2300      movs        r3,#0
  004026D4: 9300      str         r3,[sp]
  004026D6: E00C      b           004026F2
  004026D8: 9B01      ldr         r3,[sp,#4]
  004026DA: 3378      adds        r3,r3,#0x78
  004026DC: 3370      adds        r3,r3,#0x70
  004026E0: 2B00      cmp         r3,#0
  004026E2: D002      beq         004026EA
  004026E4: 2301      movs        r3,#1
  004026E6: 9303      str         r3,[sp,#0xC]
  004026E8: E001      b           004026EE
  004026EA: 2300      movs        r3,#0
  004026EC: 9303      str         r3,[sp,#0xC]
  004026EE: 9B03      ldr         r3,[sp,#0xC]
  004026F0: 9300      str         r3,[sp]
  004026F2: 9800      ldr         r0,[sp]
  004026F4: B004      add         sp,sp,#0x10
  004026F6: E8BD 8800 pop         {r11,pc}
  004026FA: DEFE      __debugbreak
  004026FC: 0000      movs        r0,r0
  004026FE: 0040      lsls        r0,r0,#1
  00402700: E92D 4800 push        {r11,lr}
  00402704: 46EB      mov         r11,sp
  00402706: B082      sub         sp,sp,#8
  00402708: F000 FA06 bl          00402B18
  0040270C: F7FF FE58 bl          004023C0
  00402710: 9000      str         r0,[sp]
  00402712: 9B00      ldr         r3,[sp]
  00402714: 9301      str         r3,[sp,#4]
  00402716: 9801      ldr         r0,[sp,#4]
  00402718: B002      add         sp,sp,#8
  0040271A: E8BD 8800 pop         {r11,pc}
  0040271E: 0000      movs        r0,r0
  00402720: 0000      movs        r0,r0
  00402722: 0000      movs        r0,r0
  00402724: 0000      movs        r0,r0
  00402726: 0000      movs        r0,r0
  00402728: 0000      movs        r0,r0
  0040272A: 0000      movs        r0,r0
  0040272C: 0000      movs        r0,r0
  0040272E: 0000      movs        r0,r0
  00402730: 0000      movs        r0,r0
  00402732: 0000      movs        r0,r0
  00402734: 0000      movs        r0,r0
  00402736: 0000      movs        r0,r0
  00402738: 0000      movs        r0,r0
  0040273A: 0000      movs        r0,r0
  0040273C: 0000      movs        r0,r0
  0040273E: 0000      movs        r0,r0
  00402740: 0000      movs        r0,r0
  00402742: 0000      movs        r0,r0
  00402744: 0000      movs        r0,r0
  00402746: 0000      movs        r0,r0
  00402748: 0000      movs        r0,r0
  0040274A: 0000      movs        r0,r0
  0040274C: 0000      movs        r0,r0
  0040274E: 0000      movs        r0,r0
  00402750: E92D 4800 push        {r11,lr}
  00402754: 46EB      mov         r11,sp
  00402756: F24A 1354 mov         r3,#0xA154
  0040275A: F2C0 0340 movt        r3,#0x40
  00402760: E8BD 8800 pop         {r11,pc}
  00402764: E92D 4800 push        {r11,lr}
  00402768: 46EB      mov         r11,sp
  0040276A: F24A 1358 mov         r3,#0xA158
  0040276E: F2C0 0340 movt        r3,#0x40
  00402774: E8BD 8800 pop         {r11,pc}
  00402778: E92D 4800 push        {r11,lr}
  0040277C: 46EB      mov         r11,sp
  0040277E: 2804      cmp         r0,#4
  00402780: D807      bhi         00402792
  00402782: F647 1318 mov         r3,#0x7918
  00402786: F2C0 0340 movt        r3,#0x40
  0040278A: F853 0020 ldr         r0,[r3,r0,lsl #2]
  0040278E: E8BD 8800 pop         {r11,pc}
  00402792: 2000      movs        r0,#0
  00402794: E8BD 8800 pop         {r11,pc}
  00402798: 0000      movs        r0,r0
  0040279A: 0000      movs        r0,r0
  0040279C: 0000      movs        r0,r0
  0040279E: 0000      movs        r0,r0
  004027A0: E92D 4800 push        {r11,lr}
  004027A4: 46EB      mov         r11,sp
  004027A6: 2005      movs        r0,#5
  004027A8: E8BD 8800 pop         {r11,pc}
  004027AC: E92D 4800 push        {r11,lr}
  004027B0: 46EB      mov         r11,sp
  004027B2: F24A 1254 mov         r2,#0xA154
  004027B6: F2C0 0240 movt        r2,#0x40
  004027BA: 4603      mov         r3,r0
  004027BE: 6013      str         r3,[r2]
  004027C0: 2300      movs        r3,#0
  004027C2: 6053      str         r3,[r2,#4]
  004027C4: E8BD 8800 pop         {r11,pc}
  004027C8: E92D 4800 push        {r11,lr}
  004027CC: 46EB      mov         r11,sp
  004027CE: F24A 1254 mov         r2,#0xA154
  004027D2: F2C0 0240 movt        r2,#0x40
  004027D6: 4603      mov         r3,r0
  004027D8: 6850      ldr         r0,[r2,#4]
  004027DA: 6053      str         r3,[r2,#4]
  004027DC: 2300      movs        r3,#0
  004027DE: 6013      str         r3,[r2]
  004027E0: E8BD 8800 pop         {r11,pc}
  004027E4: E92D 4800 push        {r11,lr}
  004027E8: 46EB      mov         r11,sp
  004027EA: 4602      mov         r2,r0
  004027EC: 2A04      cmp         r2,#4
  004027EE: D809      bhi         00402804
  004027F0: F24A 030C mov         r3,#0xA00C
  004027F4: F2C0 0340 movt        r3,#0x40
  004027F8: F853 0020 ldr         r0,[r3,r0,lsl #2]
  004027FC: F843 1022 str         r1,[r3,r2,lsl #2]
  00402800: E8BD 8800 pop         {r11,pc}
  00402804: F06F 0000 mvn         r0,#0
  00402808: E8BD 8800 pop         {r11,pc}
  0040280C: F24C 1C00 movw        r12,#0xC100
  00402810: F2C0 0C40 movt        r12,#0x40
  00402814: F8DC F000 ldr         pc,[r12]
  00402818: B403      push        {r0,r1}
  0040281A: E92D 4800 push        {r11,lr}
  0040281E: 46EB      mov         r11,sp
  00402820: B082      sub         sp,sp,#8
  00402822: 9B04      ldr         r3,[sp,#0x10]

  00402828: 4B1C      ldr         r3,0040289C
  0040282A: 429A      cmp         r2,r3
  0040282C: D126      bne         0040287C
  0040282E: 9B04      ldr         r3,[sp,#0x10]
  00402832: 3310      adds        r3,r3,#0x10
  00402836: 2B04      cmp         r3,#4
  00402838: D120      bne         0040287C
  0040283A: 9B04      ldr         r3,[sp,#0x10]
  0040283E: 3314      adds        r3,r3,#0x14
  00402842: 4B15      ldr         r3,00402898
  00402844: 429A      cmp         r2,r3
  00402846: D014      beq         00402872
  00402848: 9B04      ldr         r3,[sp,#0x10]
  0040284C: 3314      adds        r3,r3,#0x14
  00402850: 4B10      ldr         r3,00402894
  00402852: 429A      cmp         r2,r3
  00402854: D00D      beq         00402872
  00402856: 9B04      ldr         r3,[sp,#0x10]
  0040285A: 3314      adds        r3,r3,#0x14
  0040285E: 4B0C      ldr         r3,00402890
  00402860: 429A      cmp         r2,r3
  00402862: D006      beq         00402872
  00402864: 9B04      ldr         r3,[sp,#0x10]
  00402868: 3314      adds        r3,r3,#0x14
  0040286C: 4B07      ldr         r3,0040288C
  0040286E: 429A      cmp         r2,r3
  00402870: D104      bne         0040287C
  00402872: F000 FED1 bl          00403618
  00402876: 2301      movs        r3,#1
  00402878: 9300      str         r3,[sp]
  0040287A: E001      b           00402880
  0040287C: 2300      movs        r3,#0
  0040287E: 9300      str         r3,[sp]
  00402880: 9800      ldr         r0,[sp]
  00402882: B002      add         sp,sp,#8
  00402884: F85D BB04 pop         {r11}
  00402888: F85D FB0C ldr         pc,[sp],#0xC
  0040288C: 4000      ands        r0,r0,r0
  0040288E: 0199      lsls        r1,r3,#6
  00402890: 0522      lsls        r2,r4,#0x14
  00402892: 1993      adds        r3,r2,r6
  00402894: 0521      lsls        r1,r4,#0x14
  00402896: 1993      adds        r3,r2,r6
  00402898: 0520      lsls        r0,r4,#0x14
  0040289A: 1993      adds        r3,r2,r6
  0040289C: 7363      strb        r3,[r4,#0xD]
  0040289E: E06D      b           0040297C
  004028A0: 0000      movs        r0,r0
  004028A2: 0000      movs        r0,r0
  004028A4: 0000      movs        r0,r0
  004028A6: 0000      movs        r0,r0
  004028A8: 0000      movs        r0,r0
  004028AA: 0000      movs        r0,r0
  004028AC: 0000      movs        r0,r0
  004028AE: 0000      movs        r0,r0
  004028B0: 0000      movs        r0,r0
  004028B2: 0000      movs        r0,r0
  004028B4: 0000      movs        r0,r0
  004028B6: 0000      movs        r0,r0
  004028B8: E92D 4800 push        {r11,lr}
  004028BC: 46EB      mov         r11,sp
  004028BE: B082      sub         sp,sp,#8
  004028C0: 4804      ldr         r0,004028D4
  004028C2: F000 FEAF bl          00403624
  004028C6: 2300      movs        r3,#0
  004028C8: 9300      str         r3,[sp]
  004028CA: 9800      ldr         r0,[sp]
  004028CC: B002      add         sp,sp,#8
  004028CE: E8BD 8800 pop         {r11,pc}
  004028D2: DEFE      __debugbreak
  004028D4: 2819      cmp         r0,#0x19
  004028D6: 0040      lsls        r0,r0,#1
  004028D8: 0000      movs        r0,r0
  004028DA: 0000      movs        r0,r0
  004028DC: 0000      movs        r0,r0
  004028DE: 0000      movs        r0,r0
  004028E0: 0000      movs        r0,r0
  004028E2: 0000      movs        r0,r0
  004028E4: 0000      movs        r0,r0
  004028E6: 0000      movs        r0,r0
  004028E8: 0000      movs        r0,r0
  004028EA: 0000      movs        r0,r0
  004028EC: 0000      movs        r0,r0
  004028EE: 0000      movs        r0,r0
  004028F0: F24C 0CFC mov         r12,#0xC0FC
  004028F4: F2C0 0C40 movt        r12,#0x40
  004028F8: F8DC F000 ldr         pc,[r12]
  004028FC: F24C 0CF8 mov         r12,#0xC0F8
  00402900: F2C0 0C40 movt        r12,#0x40
  00402904: F8DC F000 ldr         pc,[r12]
  00402908: E92D 4800 push        {r11,lr}
  0040290C: 46EB      mov         r11,sp
  0040290E: B082      sub         sp,sp,#8
  00402910: 2300      movs        r3,#0
  00402912: 9300      str         r3,[sp]
  00402914: 9800      ldr         r0,[sp]
  00402916: B002      add         sp,sp,#8
  00402918: E8BD 8800 pop         {r11,pc}
  0040291C: F24C 0CF4 mov         r12,#0xC0F4
  00402920: F2C0 0C40 movt        r12,#0x40
  00402924: F8DC F000 ldr         pc,[r12]
  00402928: F24C 0CF0 mov         r12,#0xC0F0
  0040292C: F2C0 0C40 movt        r12,#0x40
  00402930: F8DC F000 ldr         pc,[r12]
  00402934: F24C 0CEC mov         r12,#0xC0EC
  00402938: F2C0 0C40 movt        r12,#0x40
  0040293C: F8DC F000 ldr         pc,[r12]
  00402940: F24C 0CE8 mov         r12,#0xC0E8
  00402944: F2C0 0C40 movt        r12,#0x40
  00402948: F8DC F000 ldr         pc,[r12]
  0040294C: B403      push        {r0,r1}
  0040294E: E92D 4800 push        {r11,lr}
  00402952: 46EB      mov         r11,sp
  00402954: B084      sub         sp,sp,#0x10
  00402956: 9B06      ldr         r3,[sp,#0x18]
  00402958: 333C      adds        r3,r3,#0x3C
  0040295A: 9A06      ldr         r2,[sp,#0x18]
  0040295E: 4413      add         r3,r3,r2
  00402960: 9301      str         r3,[sp,#4]
  00402962: 2300      movs        r3,#0
  00402964: 9302      str         r3,[sp,#8]
  00402966: 9B01      ldr         r3,[sp,#4]
  00402968: F103 0218 add         r2,r3,#0x18
  0040296C: 9B01      ldr         r3,[sp,#4]
  0040296E: 3314      adds        r3,r3,#0x14
  00402970: 881B      ldrh        r3,[r3]
  00402972: 4413      add         r3,r3,r2
  00402974: 9300      str         r3,[sp]
  00402976: E005      b           00402984
  00402978: 9B02      ldr         r3,[sp,#8]
  0040297A: 1C5B      adds        r3,r3,#1
  0040297C: 9302      str         r3,[sp,#8]
  0040297E: 9B00      ldr         r3,[sp]
  00402980: 3328      adds        r3,r3,#0x28
  00402982: 9300      str         r3,[sp]
  00402984: 9B01      ldr         r3,[sp,#4]
  00402986: 1D9B      adds        r3,r3,#6
  00402988: 881A      ldrh        r2,[r3]
  0040298A: 9B02      ldr         r3,[sp,#8]
  0040298C: 4293      cmp         r3,r2
  0040298E: D214      bcs         004029BA
  00402990: 9B00      ldr         r3,[sp]
  00402992: 330C      adds        r3,r3,#0xC
  00402994: 9A07      ldr         r2,[sp,#0x1C]
  00402998: 429A      cmp         r2,r3
  0040299A: D30D      bcc         004029B8
  0040299C: 9B00      ldr         r3,[sp]
  0040299E: F103 020C add         r2,r3,#0xC
  004029A2: 9B00      ldr         r3,[sp]
  004029A4: 3308      adds        r3,r3,#8
  004029AA: 441A      add         r2,r2,r3
  004029AC: 9B07      ldr         r3,[sp,#0x1C]
  004029AE: 4293      cmp         r3,r2
  004029B0: D202      bcs         004029B8
  004029B2: 9B00      ldr         r3,[sp]
  004029B4: 9303      str         r3,[sp,#0xC]
  004029B6: E002      b           004029BE
  004029B8: E7DE      b           00402978
  004029BA: 2300      movs        r3,#0
  004029BC: 9303      str         r3,[sp,#0xC]
  004029BE: 9803      ldr         r0,[sp,#0xC]
  004029C0: B004      add         sp,sp,#0x10
  004029C2: F85D BB04 pop         {r11}
  004029C6: F85D FB0C ldr         pc,[sp],#0xC
  004029CA: 0000      movs        r0,r0
  004029CC: B403      push        {r0,r1}
  004029CE: E92D 4880 push        {r7,r11,lr}
  004029D2: F10D 0B04 add         r11,sp,#4
  004029D6: B08F      sub         sp,sp,#0x3C
  004029D8: 466F      mov         r7,sp
  004029DA: 4B1F      ldr         r3,00402A58
  004029DC: 607B      str         r3,[r7,#4]
  004029DE: 6878      ldr         r0,[r7,#4]
  004029E0: F000 F854 bl          00402A8C
  004029E4: 6178      str         r0,[r7,#0x14]
  004029E6: 697B      ldr         r3,[r7,#0x14]
  004029E8: 2B00      cmp         r3,#0
  004029EA: D104      bne         004029F6
  004029EC: 2300      movs        r3,#0
  004029EE: 61BB      str         r3,[r7,#0x18]
  004029F0: 69BB      ldr         r3,[r7,#0x18]
  004029F2: 603B      str         r3,[r7]
  004029F4: E029      b           00402A4A
  004029F6: 6CBA      ldr         r2,[r7,#0x48]
  004029F8: 687B      ldr         r3,[r7,#4]
  004029FA: 1AD3      subs        r3,r2,r3
  004029FC: 61FB      str         r3,[r7,#0x1C]
  004029FE: 69F9      ldr         r1,[r7,#0x1C]
  00402A00: 6878      ldr         r0,[r7,#4]
  00402A02: F7FF FFA3 bl          0040294C
  00402A06: 6238      str         r0,[r7,#0x20]
  00402A08: 6A3B      ldr         r3,[r7,#0x20]
  00402A0A: 60BB      str         r3,[r7,#8]
  00402A0C: 68BB      ldr         r3,[r7,#8]
  00402A0E: 2B00      cmp         r3,#0
  00402A10: D104      bne         00402A1C
  00402A12: 2300      movs        r3,#0
  00402A14: 627B      str         r3,[r7,#0x24]
  00402A16: 6A7B      ldr         r3,[r7,#0x24]
  00402A18: 603B      str         r3,[r7]
  00402A1A: E016      b           00402A4A
  00402A1C: 68BB      ldr         r3,[r7,#8]
  00402A1E: 3324      adds        r3,r3,#0x24
  00402A22: F013 4F00 tst         r3,#0x80000000
  00402A26: D102      bne         00402A2E
  00402A28: 2301      movs        r3,#1
  00402A2A: 60FB      str         r3,[r7,#0xC]
  00402A2C: E001      b           00402A32
  00402A2E: 2300      movs        r3,#0
  00402A30: 60FB      str         r3,[r7,#0xC]
  00402A32: 68FB      ldr         r3,[r7,#0xC]
  00402A34: 62BB      str         r3,[r7,#0x28]
  00402A36: 6ABB      ldr         r3,[r7,#0x28]
  00402A38: 603B      str         r3,[r7]
  00402A3A: E006      b           00402A4A
  00402A3C: E005      b           00402A4A
  00402A3E: 2300      movs        r3,#0
  00402A40: 637B      str         r3,[r7,#0x34]
  00402A42: 6B7B      ldr         r3,[r7,#0x34]
  00402A44: 603B      str         r3,[r7]
  00402A46: E000      b           00402A4A
  00402A48: E7FF      b           00402A4A
  00402A4C: B00F      add         sp,sp,#0x3C
  00402A4E: E8BD 0880 pop         {r7,r11}
  00402A52: F85D FB0C ldr         pc,[sp],#0xC
  00402A56: DEFE      __debugbreak
  00402A58: 0000      movs        r0,r0
  00402A5A: 0040      lsls        r0,r0,#1
  00402A5C: 0000      movs        r0,r0
  00402A5E: 0000      movs        r0,r0
  00402A60: 0000      movs        r0,r0
  00402A62: 0000      movs        r0,r0
  00402A64: 0000      movs        r0,r0
  00402A66: 0000      movs        r0,r0
  00402A68: 0000      movs        r0,r0
  00402A6A: 0000      movs        r0,r0
  00402A6C: 0000      movs        r0,r0
  00402A6E: 0000      movs        r0,r0
  00402A70: 0000      movs        r0,r0
  00402A72: 0000      movs        r0,r0
  00402A74: 0000      movs        r0,r0
  00402A76: 0000      movs        r0,r0
  00402A78: 0000      movs        r0,r0
  00402A7A: 0000      movs        r0,r0
  00402A7C: 0000      movs        r0,r0
  00402A7E: 0000      movs        r0,r0
  00402A80: 0000      movs        r0,r0
  00402A82: 0000      movs        r0,r0
  00402A84: 0000      movs        r0,r0
  00402A86: 0000      movs        r0,r0
  00402A88: 0000      movs        r0,r0
  00402A8A: 0000      movs        r0,r0
  00402A8C: B403      push        {r0,r1}
  00402A8E: E92D 4800 push        {r11,lr}
  00402A92: 46EB      mov         r11,sp
  00402A94: B084      sub         sp,sp,#0x10
  00402A96: 9B06      ldr         r3,[sp,#0x18]
  00402A98: 9301      str         r3,[sp,#4]
  00402A9A: 9B01      ldr         r3,[sp,#4]
  00402A9C: 881A      ldrh        r2,[r3]
  00402A9E: F645 234D mov         r3,#0x5A4D
  00402AA2: 429A      cmp         r2,r3
  00402AA4: D002      beq         00402AAC
  00402AA6: 2300      movs        r3,#0
  00402AA8: 9300      str         r3,[sp]
  00402AAA: E01C      b           00402AE6
  00402AAC: 9B01      ldr         r3,[sp,#4]
  00402AAE: 333C      adds        r3,r3,#0x3C
  00402AB0: 9A01      ldr         r2,[sp,#4]
  00402AB4: 4413      add         r3,r3,r2
  00402AB6: 9302      str         r3,[sp,#8]
  00402AB8: 9B02      ldr         r3,[sp,#8]
  00402ABC: F244 5350 mov         r3,#0x4550
  00402AC0: 429A      cmp         r2,r3
  00402AC2: D002      beq         00402ACA
  00402AC4: 2300      movs        r3,#0
  00402AC6: 9300      str         r3,[sp]
  00402AC8: E00D      b           00402AE6
  00402ACA: 9B02      ldr         r3,[sp,#8]
  00402ACC: 3318      adds        r3,r3,#0x18
  00402ACE: 9303      str         r3,[sp,#0xC]
  00402AD0: 9B03      ldr         r3,[sp,#0xC]
  00402AD2: 881A      ldrh        r2,[r3]
  00402AD4: F240 130B mov         r3,#0x10B
  00402AD8: 429A      cmp         r2,r3
  00402ADA: D002      beq         00402AE2
  00402ADC: 2300      movs        r3,#0
  00402ADE: 9300      str         r3,[sp]
  00402AE0: E001      b           00402AE6
  00402AE2: 2301      movs        r3,#1
  00402AE4: 9300      str         r3,[sp]
  00402AE6: 9800      ldr         r0,[sp]
  00402AE8: B004      add         sp,sp,#0x10
  00402AEA: F85D BB04 pop         {r11}
  00402AEE: F85D FB0C ldr         pc,[sp],#0xC
  00402AF2: 0000      movs        r0,r0
  00402AF4: F24C 0CE4 mov         r12,#0xC0E4
  00402AF8: F2C0 0C40 movt        r12,#0x40
  00402AFC: F8DC F000 ldr         pc,[r12]
  00402B00: F24C 0CE0 mov         r12,#0xC0E0
  00402B04: F2C0 0C40 movt        r12,#0x40
  00402B08: F8DC F000 ldr         pc,[r12]
  00402B0C: F24C 0CDC mov         r12,#0xC0DC
  00402B10: F2C0 0C40 movt        r12,#0x40
  00402B14: F8DC F000 ldr         pc,[r12]
  00402B18: E92D 4800 push        {r11,lr}
  00402B1C: 46EB      mov         r11,sp
  00402B1E: B08A      sub         sp,sp,#0x28
  00402B20: 2300      movs        r3,#0
  00402B22: 9304      str         r3,[sp,#0x10]
  00402B24: 2300      movs        r3,#0
  00402B26: 9305      str         r3,[sp,#0x14]
  00402B28: 4B27      ldr         r3,00402BC8
  00402B2C: 4B28      ldr         r3,00402BD0
  00402B2E: 429A      cmp         r2,r3
  00402B30: D005      beq         00402B3E
  00402B32: 4B25      ldr         r3,00402BC8
  00402B36: 43DA      mvns        r2,r3
  00402B38: 4B22      ldr         r3,00402BC4
  00402B3A: 601A      str         r2,[r3]
  00402B3C: E03F      b           00402BBE
  00402B3E: A804      add         r0,sp,#0x10
  00402B40: 4B28      ldr         r3,00402BE4
  00402B44: 4798      blx         r3
  00402B46: 9B04      ldr         r3,[sp,#0x10]
  00402B48: 9300      str         r3,[sp]
  00402B4A: 9A00      ldr         r2,[sp]
  00402B4C: 9B05      ldr         r3,[sp,#0x14]
  00402B4E: 4053      eors        r3,r3,r2
  00402B50: 9300      str         r3,[sp]
  00402B52: 4B23      ldr         r3,00402BE0
  00402B56: 4798      blx         r3
  00402B58: 9001      str         r0,[sp,#4]
  00402B5A: 9A00      ldr         r2,[sp]
  00402B5C: 9B01      ldr         r3,[sp,#4]
  00402B5E: 4053      eors        r3,r3,r2
  00402B60: 9300      str         r3,[sp]
  00402B62: 4B1E      ldr         r3,00402BDC
  00402B66: 4798      blx         r3
  00402B68: 9002      str         r0,[sp,#8]
  00402B6A: 9A00      ldr         r2,[sp]
  00402B6C: 9B02      ldr         r3,[sp,#8]
  00402B6E: 4053      eors        r3,r3,r2
  00402B70: 9300      str         r3,[sp]
  00402B72: 4B19      ldr         r3,00402BD8
  00402B76: 4798      blx         r3
  00402B78: 9107      str         r1,[sp,#0x1C]
  00402B7A: 9006      str         r0,[sp,#0x18]
  00402B7C: 9A06      ldr         r2,[sp,#0x18]
  00402B7E: 9B00      ldr         r3,[sp]
  00402B80: 4053      eors        r3,r3,r2
  00402B82: 9300      str         r3,[sp]
  00402B84: A808      add         r0,sp,#0x20
  00402B86: 4B13      ldr         r3,00402BD4
  00402B8A: 4798      blx         r3
  00402B8C: 9A00      ldr         r2,[sp]
  00402B8E: 9B08      ldr         r3,[sp,#0x20]
  00402B90: 4053      eors        r3,r3,r2
  00402B92: 9300      str         r3,[sp]
  00402B94: 9A00      ldr         r2,[sp]
  00402B96: 9B09      ldr         r3,[sp,#0x24]
  00402B98: 4053      eors        r3,r3,r2
  00402B9A: 9300      str         r3,[sp]
  00402B9C: 9A00      ldr         r2,[sp]
  00402B9E: 466B      mov         r3,sp
  00402BA0: 4053      eors        r3,r3,r2
  00402BA2: 9300      str         r3,[sp]
  00402BA4: 9A00      ldr         r2,[sp]
  00402BA6: 4B0A      ldr         r3,00402BD0
  00402BA8: 429A      cmp         r2,r3
  00402BAA: D101      bne         00402BB0
  00402BAC: 4B07      ldr         r3,00402BCC
  00402BAE: 9300      str         r3,[sp]
  00402BB0: 9A00      ldr         r2,[sp]
  00402BB2: 4B05      ldr         r3,00402BC8
  00402BB4: 601A      str         r2,[r3]
  00402BB6: 9B00      ldr         r3,[sp]
  00402BB8: 43DA      mvns        r2,r3
  00402BBA: 4B02      ldr         r3,00402BC4
  00402BBC: 601A      str         r2,[r3]
  00402BBE: B00A      add         sp,sp,#0x28
  00402BC0: E8BD 8800 pop         {r11,pc}
  00402BC4: A024      adr         r0,00402C58
  00402BC6: 0040      lsls        r0,r0,#1
  00402BC8: A020      adr         r0,00402C4C
  00402BCA: 0040      lsls        r0,r0,#1
  00402BCC: E64F      b           0040286E
  00402BCE: BB40      cbnz        r0,00402C22
  00402BD0: E64E      b           00402870
  00402BD2: BB40      cbnz        r0,00402C26
  00402BD4: C048      stm         r0!,{r3,r6}
  00402BD6: 0040      lsls        r0,r0,#1
  00402BD8: C038      stm         r0!,{r3-r5}
  00402BDA: 0040      lsls        r0,r0,#1
  00402BDC: C044      stm         r0!,{r2,r6}
  00402BDE: 0040      lsls        r0,r0,#1
  00402BE0: C040      stm         r0!,{r6}
  00402BE2: 0040      lsls        r0,r0,#1
  00402BE4: C03C      stm         r0!,{r2-r5}
  00402BE6: 0040      lsls        r0,r0,#1
  00402BE8: B403      push        {r0,r1}
  00402BEA: E92D 4800 push        {r11,lr}
  00402BEE: 46EB      mov         r11,sp
  00402BF0: B082      sub         sp,sp,#8
  00402BF2: 2300      movs        r3,#0
  00402BF4: 9300      str         r3,[sp]
  00402BF6: 9800      ldr         r0,[sp]
  00402BF8: B002      add         sp,sp,#8
  00402BFA: F85D BB04 pop         {r11}
  00402BFE: F85D FB0C ldr         pc,[sp],#0xC
  00402C02: 0000      movs        r0,r0
  00402C04: E92D 4810 push        {r4,r11,lr}
  00402C08: F10D 0B04 add         r11,sp,#4
  00402C0C: B087      sub         sp,sp,#0x1C
  00402C0E: 4B28      ldr         r3,00402CB0
  00402C12: 2B00      cmp         r3,#0
  00402C14: D003      beq         00402C1E
  00402C16: 4B25      ldr         r3,00402CAC
  00402C1A: 2B00      cmp         r3,#0
  00402C1C: D117      bne         00402C4E
  00402C1E: 4B24      ldr         r3,00402CB0
  00402C22: 2B00      cmp         r3,#0
  00402C24: D103      bne         00402C2E
  00402C26: 4B21      ldr         r3,00402CAC
  00402C2A: 2B00      cmp         r3,#0
  00402C2C: D00F      beq         00402C4E
  00402C2E: 4B27      ldr         r3,00402CCC
  00402C30: 9301      str         r3,[sp,#4]
  00402C32: 4B25      ldr         r3,00402CC8
  00402C34: 9300      str         r3,[sp]
  00402C36: 2300      movs        r3,#0
  00402C38: 2245      movs        r2,#0x45
  00402C3A: 4922      ldr         r1,00402CC4
  00402C3C: 2002      movs        r0,#2
  00402C3E: 4C20      ldr         r4,00402CC0
  00402C42: 47A0      blx         r4
  00402C44: 9004      str         r0,[sp,#0x10]
  00402C46: 9B04      ldr         r3,[sp,#0x10]
  00402C48: 2B01      cmp         r3,#1
  00402C4A: D100      bne         00402C4E
  00402C4C: DEFE      __debugbreak
  00402C4E: 4B18      ldr         r3,00402CB0
  00402C52: 2B00      cmp         r3,#0
  00402C54: D002      beq         00402C5C
  00402C56: 2300      movs        r3,#0
  00402C58: 9303      str         r3,[sp,#0xC]
  00402C5A: E022      b           00402CA2
  00402C5C: 234D      movs        r3,#0x4D
  00402C5E: 9300      str         r3,[sp]
  00402C60: 4B16      ldr         r3,00402CBC
  00402C62: 2202      movs        r2,#2
  00402C64: 2104      movs        r1,#4
  00402C66: 2020      movs        r0,#0x20
  00402C68: 4C13      ldr         r4,00402CB8
  00402C6C: 47A0      blx         r4
  00402C6E: 9005      str         r0,[sp,#0x14]
  00402C70: 9B05      ldr         r3,[sp,#0x14]
  00402C72: 9302      str         r3,[sp,#8]
  00402C74: 9802      ldr         r0,[sp,#8]
  00402C76: 4B0F      ldr         r3,00402CB4
  00402C7A: 4798      blx         r3
  00402C7C: 9006      str         r0,[sp,#0x18]
  00402C7E: 9A06      ldr         r2,[sp,#0x18]
  00402C80: 4B0B      ldr         r3,00402CB0
  00402C82: 601A      str         r2,[r3]
  00402C84: 4B0A      ldr         r3,00402CB0
  00402C88: 4B08      ldr         r3,00402CAC
  00402C8A: 601A      str         r2,[r3]
  00402C8C: 9B02      ldr         r3,[sp,#8]
  00402C8E: 2B00      cmp         r3,#0
  00402C90: D102      bne         00402C98
  00402C92: 2318      movs        r3,#0x18
  00402C94: 9303      str         r3,[sp,#0xC]
  00402C96: E004      b           00402CA2
  00402C98: 9A02      ldr         r2,[sp,#8]
  00402C9A: 2300      movs        r3,#0
  00402C9C: 6013      str         r3,[r2]
  00402C9E: 2300      movs        r3,#0
  00402CA0: 9303      str         r3,[sp,#0xC]
  00402CA2: 9803      ldr         r0,[sp,#0xC]
  00402CA4: B007      add         sp,sp,#0x1C
  00402CA6: E8BD 8810 pop         {r4,r11,pc}
  00402CAA: DEFE      __debugbreak
  00402CAC: A37C      adr         r3,00402EA0
  00402CAE: 0040      lsls        r0,r0,#1
  00402CB0: A38C      adr         r3,00402EE4
  00402CB2: 0040      lsls        r0,r0,#1
  00402CB4: C04C      stm         r0!,{r2,r3,r6}
  00402CB6: 0040      lsls        r0,r0,#1
  00402CB8: C098      stm         r0!,{r3,r4,r7}
  00402CBA: 0040      lsls        r0,r0,#1
  00402CBC: 7B14      ldrb        r4,[r2,#0xC]
  00402CBE: 0040      lsls        r0,r0,#1
  00402CC0: C0EC      stm         r0!,{r2,r3,r5-r7}
  00402CC2: 0040      lsls        r0,r0,#1
  00402CC4: 7AB8      ldrb        r0,[r7,#0xA]
  00402CC6: 0040      lsls        r0,r0,#1
  00402CC8: 78B8      ldrb        r0,[r7,#2]
  00402CCA: 0040      lsls        r0,r0,#1
  00402CCC: 79F0      ldrb        r0,[r6,#7]
  00402CCE: 0040      lsls        r0,r0,#1
  00402CD0: B403      push        {r0,r1}
  00402CD2: E92D 4880 push        {r7,r11,lr}
  00402CD6: F10D 0B04 add         r11,sp,#4
  00402CDA: B08D      sub         sp,sp,#0x34
  00402CDC: 466F      mov         r7,sp
  00402CDE: 2300      movs        r3,#0
  00402CE0: 60BB      str         r3,[r7,#8]
  00402CE2: 4B29      ldr         r3,00402D88
  00402CE6: 4B29      ldr         r3,00402D8C
  00402CEA: 4798      blx         r3
  00402CEC: 6138      str         r0,[r7,#0x10]
  00402CEE: 693B      ldr         r3,[r7,#0x10]
  00402CF0: 603B      str         r3,[r7]
  00402CF4: F1B3 3FFF cmp         r3,#0xFFFFFFFF
  00402CF8: D107      bne         00402D0A
  00402CFA: 6C38      ldr         r0,[r7,#0x40]
  00402CFC: 4B24      ldr         r3,00402D90
  00402D00: 4798      blx         r3
  00402D02: 6178      str         r0,[r7,#0x14]
  00402D04: 697B      ldr         r3,[r7,#0x14]
  00402D06: 60FB      str         r3,[r7,#0xC]
  00402D08: E034      b           00402D74
  00402D0A: 2008      movs        r0,#8
  00402D0C: F000 FC90 bl          00403630
  00402D10: 4B1D      ldr         r3,00402D88
  00402D14: 4B1D      ldr         r3,00402D8C
  00402D18: 4798      blx         r3
  00402D1A: 61B8      str         r0,[r7,#0x18]
  00402D1C: 69BB      ldr         r3,[r7,#0x18]
  00402D1E: 603B      str         r3,[r7]
  00402D20: 4B17      ldr         r3,00402D80
  00402D24: 4B19      ldr         r3,00402D8C
  00402D28: 4798      blx         r3
  00402D2A: 61F8      str         r0,[r7,#0x1C]
  00402D2C: 69FB      ldr         r3,[r7,#0x1C]
  00402D2E: 607B      str         r3,[r7,#4]
  00402D30: 6C38      ldr         r0,[r7,#0x40]
  00402D32: 4B14      ldr         r3,00402D84
  00402D36: 4798      blx         r3
  00402D38: 6238      str         r0,[r7,#0x20]
  00402D3A: 1D3A      adds        r2,r7,#4
  00402D3C: 4639      mov         r1,r7
  00402D3E: 6A38      ldr         r0,[r7,#0x20]
  00402D40: F000 FC88 bl          00403654
  00402D44: 6278      str         r0,[r7,#0x24]
  00402D46: 6A7B      ldr         r3,[r7,#0x24]
  00402D48: 60BB      str         r3,[r7,#8]
  00402D4C: 4B0D      ldr         r3,00402D84
  00402D50: 4798      blx         r3
  00402D52: 62B8      str         r0,[r7,#0x28]
  00402D54: 6ABA      ldr         r2,[r7,#0x28]
  00402D56: 4B0C      ldr         r3,00402D88
  00402D58: 601A      str         r2,[r3]
  00402D5A: 6878      ldr         r0,[r7,#4]
  00402D5C: 4B09      ldr         r3,00402D84
  00402D60: 4798      blx         r3
  00402D62: 62F8      str         r0,[r7,#0x2C]
  00402D64: 6AFA      ldr         r2,[r7,#0x2C]
  00402D66: 4B06      ldr         r3,00402D80
  00402D68: 601A      str         r2,[r3]
  00402D6A: 2008      movs        r0,#8
  00402D6C: F000 FC66 bl          0040363C
  00402D70: 68BB      ldr         r3,[r7,#8]
  00402D72: 60FB      str         r3,[r7,#0xC]
  00402D74: 68F8      ldr         r0,[r7,#0xC]
  00402D76: B00D      add         sp,sp,#0x34
  00402D78: E8BD 0880 pop         {r7,r11}
  00402D7C: F85D FB0C ldr         pc,[sp],#0xC
  00402D80: A37C      adr         r3,00402F74
  00402D82: 0040      lsls        r0,r0,#1
  00402D84: C04C      stm         r0!,{r2,r3,r6}
  00402D86: 0040      lsls        r0,r0,#1
  00402D88: A38C      adr         r3,00402FBC
  00402D8A: 0040      lsls        r0,r0,#1
  00402D8C: C034      stm         r0!,{r2,r4,r5}
  00402D8E: 0040      lsls        r0,r0,#1
  00402D90: C104      stm         r1!,{r2}
  00402D92: 0040      lsls        r0,r0,#1
  00402D94: 0000      movs        r0,r0
  00402D96: 0000      movs        r0,r0
  00402D98: 0000      movs        r0,r0
  00402D9A: 0000      movs        r0,r0
  00402D9C: 0000      movs        r0,r0
  00402D9E: 0000      movs        r0,r0
  00402DA0: 0000      movs        r0,r0
  00402DA2: 0000      movs        r0,r0
  00402DA4: 0000      movs        r0,r0
  00402DA6: 0000      movs        r0,r0
  00402DA8: 0000      movs        r0,r0
  00402DAA: 0000      movs        r0,r0
  00402DAC: 0000      movs        r0,r0
  00402DAE: 0000      movs        r0,r0
  00402DB0: 0000      movs        r0,r0
  00402DB2: 0000      movs        r0,r0
  00402DB4: 0000      movs        r0,r0
  00402DB6: 0000      movs        r0,r0
  00402DB8: 0000      movs        r0,r0
  00402DBA: 0000      movs        r0,r0
  00402DBC: 0000      movs        r0,r0
  00402DBE: 0000      movs        r0,r0
  00402DC0: 0000      movs        r0,r0
  00402DC2: 0000      movs        r0,r0
  00402DC4: 0000      movs        r0,r0
  00402DC6: 0000      movs        r0,r0
  00402DC8: 0000      movs        r0,r0
  00402DCA: 0000      movs        r0,r0
  00402DCC: 0000      movs        r0,r0
  00402DCE: 0000      movs        r0,r0
  00402DD0: 0000      movs        r0,r0
  00402DD2: 0000      movs        r0,r0
  00402DD4: 0000      movs        r0,r0
  00402DD6: 0000      movs        r0,r0
  00402DD8: 0000      movs        r0,r0
  00402DDA: 0000      movs        r0,r0
  00402DDC: B403      push        {r0,r1}
  00402DDE: E92D 4800 push        {r11,lr}
  00402DE2: 46EB      mov         r11,sp
  00402DE4: B084      sub         sp,sp,#0x10
  00402DE6: 9806      ldr         r0,[sp,#0x18]
  00402DE8: F7FF FF72 bl          00402CD0
  00402DEC: 9001      str         r0,[sp,#4]
  00402DEE: 9B01      ldr         r3,[sp,#4]
  00402DF0: 2B00      cmp         r3,#0
  00402DF2: D103      bne         00402DFC
  00402DF4: F06F 0300 mvn         r3,#0
  00402DF8: 9300      str         r3,[sp]
  00402DFA: E001      b           00402E00
  00402DFC: 2300      movs        r3,#0
  00402DFE: 9300      str         r3,[sp]
  00402E00: 9B00      ldr         r3,[sp]
  00402E02: 9302      str         r3,[sp,#8]
  00402E04: 9802      ldr         r0,[sp,#8]
  00402E06: B004      add         sp,sp,#0x10
  00402E08: F85D BB04 pop         {r11}
  00402E0C: F85D FB0C ldr         pc,[sp],#0xC
  00402E10: 0000      movs        r0,r0
  00402E12: 0000      movs        r0,r0
  00402E14: 0000      movs        r0,r0
  00402E16: 0000      movs        r0,r0
  00402E18: 0000      movs        r0,r0
  00402E1A: 0000      movs        r0,r0
  00402E1C: 0000      movs        r0,r0
  00402E1E: 0000      movs        r0,r0
  00402E20: 0000      movs        r0,r0
  00402E22: 0000      movs        r0,r0
  00402E24: 0000      movs        r0,r0
  00402E26: 0000      movs        r0,r0
  00402E28: E92D 4830 push        {r4,r5,r11,lr}
  00402E2C: F10D 0B08 add         r11,sp,#8
  00402E30: F248 739C mov         r3,#0x879C
  00402E34: F2C0 0340 movt        r3,#0x40
  00402E38: F648 15A4 mov         r5,#0x89A4
  00402E3C: F2C0 0540 movt        r5,#0x40
  00402E40: 1D1C      adds        r4,r3,#4
  00402E42: 1D1B      adds        r3,r3,#4
  00402E44: 42AB      cmp         r3,r5
  00402E46: D205      bcs         00402E54
  00402E48: F854 3B04 ldr         r3,[r4],#4
  00402E4C: B103      cbz         r3,00402E50
  00402E4E: 4798      blx         r3
  00402E50: 42AC      cmp         r4,r5
  00402E52: D3F9      bcc         00402E48
  00402E54: E8BD 8830 pop         {r4,r5,r11,pc}
  00402E58: E92D 4830 push        {r4,r5,r11,lr}
  00402E5C: F10D 0B08 add         r11,sp,#8
  00402E60: F648 23A8 mov         r3,#0x8AA8
  00402E64: F2C0 0340 movt        r3,#0x40
  00402E68: F648 45B0 mov         r5,#0x8CB0
  00402E6C: F2C0 0540 movt        r5,#0x40
  00402E70: 1D1C      adds        r4,r3,#4
  00402E72: 1D1B      adds        r3,r3,#4
  00402E74: 42AB      cmp         r3,r5
  00402E76: D205      bcs         00402E84
  00402E78: F854 3B04 ldr         r3,[r4],#4
  00402E7C: B103      cbz         r3,00402E80
  00402E7E: 4798      blx         r3
  00402E80: 42AC      cmp         r4,r5
  00402E82: D3F9      bcc         00402E78
  00402E84: E8BD 8830 pop         {r4,r5,r11,pc}
  00402E88: F24C 0CD8 mov         r12,#0xC0D8
  00402E8C: F2C0 0C40 movt        r12,#0x40
  00402E90: F8DC F000 ldr         pc,[r12]
  00402E94: F24C 0CD4 mov         r12,#0xC0D4
  00402E98: F2C0 0C40 movt        r12,#0x40
  00402E9C: F8DC F000 ldr         pc,[r12]
  00402EA0: F24C 0CD0 mov         r12,#0xC0D0
  00402EA4: F2C0 0C40 movt        r12,#0x40
  00402EA8: F8DC F000 ldr         pc,[r12]
  00402EAC: F24C 0CCC mov         r12,#0xC0CC
  00402EB0: F2C0 0C40 movt        r12,#0x40
  00402EB4: F8DC F000 ldr         pc,[r12]
  00402EB8: F24C 0CC8 mov         r12,#0xC0C8
  00402EBC: F2C0 0C40 movt        r12,#0x40
  00402EC0: F8DC F000 ldr         pc,[r12]
  00402EC4: E92D 4890 push        {r4,r7,r11,lr}
  00402EC8: F10D 0B08 add         r11,sp,#8
  00402ECC: B088      sub         sp,sp,#0x20
  00402ECE: 466F      mov         r7,sp
  00402ED0: 2300      movs        r3,#0
  00402ED2: 703B      strb        r3,[r7]
  00402ED4: F241 0301 mov         r3,#0x1001
  00402ED8: 60BB      str         r3,[r7,#8]
  00402EDA: 60F8      str         r0,[r7,#0xC]
  00402EDC: 463B      mov         r3,r7
  00402EDE: 613B      str         r3,[r7,#0x10]
  00402EE0: F117 0308 adds        r3,r7,#8
  00402EE4: 2206      movs        r2,#6
  00402EE6: 2100      movs        r1,#0
  00402EE8: 4806      ldr         r0,00402F04
  00402EEA: F24C 042C mov         r4,#0xC02C
  00402EEE: F2C0 0440 movt        r4,#0x40
  00402EF4: 47A0      blx         r4
  00402EF6: 7838      ldrb        r0,[r7]
  00402EF8: E000      b           00402EFC
  00402EFA: 6878      ldr         r0,[r7,#4]
  00402EFC: B008      add         sp,sp,#0x20
  00402EFE: E8BD 8890 pop         {r4,r7,r11,pc}
  00402F02: DEFE      __debugbreak
  00402F04: 1388      asrs        r0,r1,#0xE
  00402F06: 406D      eors        r5,r5,r5
  00402F08: E92D 4890 push        {r4,r7,r11,lr}
  00402F0C: F10D 0B08 add         r11,sp,#8
  00402F10: B088      sub         sp,sp,#0x20
  00402F12: 466F      mov         r7,sp
  00402F14: 2400      movs        r4,#0
  00402F16: 703C      strb        r4,[r7]
  00402F18: F241 0402 mov         r4,#0x1002
  00402F1C: 60BC      str         r4,[r7,#8]
  00402F1E: 60F8      str         r0,[r7,#0xC]
  00402F20: 6139      str         r1,[r7,#0x10]
  00402F22: 617A      str         r2,[r7,#0x14]
  00402F24: 463A      mov         r2,r7
  00402F26: 61BA      str         r2,[r7,#0x18]
  00402F28: 61FB      str         r3,[r7,#0x1C]
  00402F2A: F117 0308 adds        r3,r7,#8
  00402F2E: 2206      movs        r2,#6
  00402F30: 2100      movs        r1,#0
  00402F32: 4806      ldr         r0,00402F4C
  00402F34: F24C 042C mov         r4,#0xC02C
  00402F38: F2C0 0440 movt        r4,#0x40
  00402F3E: 47A0      blx         r4
  00402F40: 7838      ldrb        r0,[r7]
  00402F42: E000      b           00402F46
  00402F44: 6878      ldr         r0,[r7,#4]
  00402F46: B008      add         sp,sp,#0x20
  00402F48: E8BD 8890 pop         {r4,r7,r11,pc}
  00402F4C: 1388      asrs        r0,r1,#0xE
  00402F4E: 406D      eors        r5,r5,r5
  00402F50: E92D 48F0 push        {r4-r7,r11,lr}
  00402F54: F10D 0B10 add         r11,sp,#0x10
  00402F58: F7FE F864 bl          00401024
  00402F5C: B0D9      sub         sp,sp,#0x164
  00402F5E: F24A 030C mov         r3,#0xA00C
  00402F62: F2C0 0340 movt        r3,#0x40
  00402F66: 460E      mov         r6,r1
  00402F68: 4607      mov         r7,r0
  00402F6A: 691D      ldr         r5,[r3,#0x10]
  00402F6C: F1B5 3FFF cmp         r5,#0xFFFFFFFF
  00402F70: D057      beq         00403022
  00402F72: B926      cbnz        r6,00402F7E
  00402F74: F647 73B0 mov         r3,#0x7FB0
  00402F78: F2C0 0340 movt        r3,#0x40
  00402F7C: E04C      b           00403018
  00402F7E: F647 73FC mov         r3,#0x7FFC
  00402F82: F2C0 0340 movt        r3,#0x40
  00402F86: 9205      str         r2,[sp,#0x14]
  00402F88: F248 0298 mov         r2,#0x8098
  00402F8C: F2C0 0240 movt        r2,#0x40
  00402F90: 9306      str         r3,[sp,#0x18]
  00402F92: F248 0308 mov         r3,#0x8008
  00402F96: F2C0 0340 movt        r3,#0x40
  00402F9A: F106 0420 add         r4,r6,#0x20
  00402F9E: A81C      add         r0,sp,#0x70
  00402FA0: 9304      str         r3,[sp,#0x10]
  00402FA2: 68F3      ldr         r3,[r6,#0xC]
  00402FA4: 21F4      movs        r1,#0xF4
  00402FA6: 9401      str         r4,[sp,#4]
  00402FA8: 3B24      subs        r3,r3,#0x24
  00402FAA: 9303      str         r3,[sp,#0xC]
  00402FAC: F248 0334 mov         r3,#0x8034
  00402FB0: F2C0 0340 movt        r3,#0x40
  00402FB4: 9302      str         r3,[sp,#8]
  00402FB6: F248 033C mov         r3,#0x803C
  00402FBA: F2C0 0340 movt        r3,#0x40
  00402FBE: 9300      str         r3,[sp]
  00402FC0: F248 0350 mov         r3,#0x8050
  00402FC4: F2C0 0340 movt        r3,#0x40
  00402FC8: F000 FEAA bl          00403D20
  00402FCC: 68F3      ldr         r3,[r6,#0xC]
  00402FCE: A90E      add         r1,sp,#0x38
  00402FD0: A808      add         r0,sp,#0x20
  00402FD2: 3B24      subs        r3,r3,#0x24
  00402FD4: 4622      mov         r2,r4
  00402FD6: F000 F931 bl          0040323C
  00402FDA: A81C      add         r0,sp,#0x70
  00402FDC: F000 F96C bl          004032B8
  00402FE0: 4604      mov         r4,r0
  00402FE2: A81C      add         r0,sp,#0x70
  00402FE4: F000 F968 bl          004032B8
  00402FE8: F248 03AC mov         r3,#0x80AC
  00402FEC: F2C0 0340 movt        r3,#0x40
  00402FF0: F248 02B4 mov         r2,#0x80B4
  00402FF4: F2C0 0240 movt        r2,#0x40
  00402FF8: F1C4 01F4 rsb         r1,r4,#0xF4
  00402FFC: 9302      str         r3,[sp,#8]
  00402FFE: AB0E      add         r3,sp,#0x38
  00403000: 9301      str         r3,[sp,#4]
  00403002: F248 03B0 mov         r3,#0x80B0
  00403006: F2C0 0340 movt        r3,#0x40
  0040300A: AC1C      add         r4,sp,#0x70
  0040300C: 4420      add         r0,r0,r4
  0040300E: 9300      str         r3,[sp]
  00403010: AB08      add         r3,sp,#0x20
  00403012: F000 FE85 bl          00403D20
  00403016: AB1C      add         r3,sp,#0x70
  00403018: 2204      movs        r2,#4
  0040301A: 4629      mov         r1,r5
  0040301C: 4638      mov         r0,r7
  0040301E: F000 F957 bl          004032D0
  00403022: B059      add         sp,sp,#0x164
  00403024: F7FE F80A bl          0040103C
  00403028: E8BD 88F0 pop         {r4-r7,r11,pc}
  0040302C: 0000      movs        r0,r0
  0040302E: 0000      movs        r0,r0
  00403030: 0000      movs        r0,r0
  00403032: 0000      movs        r0,r0
  00403034: 0000      movs        r0,r0
  00403036: 0000      movs        r0,r0
  00403038: 0000      movs        r0,r0
  0040303A: 0000      movs        r0,r0
  0040303C: 0000      movs        r0,r0
  0040303E: 0000      movs        r0,r0
  00403040: 0000      movs        r0,r0
  00403042: 0000      movs        r0,r0
  00403044: 0000      movs        r0,r0
  00403046: 0000      movs        r0,r0
  00403048: 0000      movs        r0,r0
  0040304A: 0000      movs        r0,r0
  0040304C: 0000      movs        r0,r0
  0040304E: 0000      movs        r0,r0
  00403050: 0000      movs        r0,r0
  00403052: 0000      movs        r0,r0
  00403054: 0000      movs        r0,r0
  00403056: 0000      movs        r0,r0
  00403058: 0000      movs        r0,r0
  0040305A: 0000      movs        r0,r0
  0040305C: 0000      movs        r0,r0
  0040305E: 0000      movs        r0,r0
  00403060: 0000      movs        r0,r0
  00403062: 0000      movs        r0,r0
  00403064: 0000      movs        r0,r0
  00403066: 0000      movs        r0,r0
  00403068: 0000      movs        r0,r0
  0040306A: 0000      movs        r0,r0
  0040306C: 0000      movs        r0,r0
  0040306E: 0000      movs        r0,r0
  00403070: 0000      movs        r0,r0
  00403072: 0000      movs        r0,r0
  00403074: 0000      movs        r0,r0
  00403076: 0000      movs        r0,r0
  00403078: 0000      movs        r0,r0
  0040307A: 0000      movs        r0,r0
  0040307C: 0000      movs        r0,r0
  0040307E: 0000      movs        r0,r0
  00403080: 0000      movs        r0,r0
  00403082: 0000      movs        r0,r0
  00403084: 0000      movs        r0,r0
  00403086: 0000      movs        r0,r0
  00403088: 0000      movs        r0,r0
  0040308A: 0000      movs        r0,r0
  0040308C: 0000      movs        r0,r0
  0040308E: 0000      movs        r0,r0
  00403090: 0000      movs        r0,r0
  00403092: 0000      movs        r0,r0
  00403094: 0000      movs        r0,r0
  00403096: 0000      movs        r0,r0
  00403098: 0000      movs        r0,r0
  0040309A: 0000      movs        r0,r0
  0040309C: 0000      movs        r0,r0
  0040309E: 0000      movs        r0,r0
  004030A0: 0000      movs        r0,r0
  004030A2: 0000      movs        r0,r0
  004030A4: 0000      movs        r0,r0
  004030A6: 0000      movs        r0,r0
  004030A8: 0000      movs        r0,r0
  004030AA: 0000      movs        r0,r0
  004030AC: 0000      movs        r0,r0
  004030AE: 0000      movs        r0,r0
  004030B0: 0000      movs        r0,r0
  004030B2: 0000      movs        r0,r0
  004030B4: 0000      movs        r0,r0
  004030B6: 0000      movs        r0,r0
  004030B8: 0000      movs        r0,r0
  004030BA: 0000      movs        r0,r0
  004030BC: E92D 4818 push        {r3,r4,r11,lr}
  004030C0: F10D 0B08 add         r11,sp,#8
  004030C4: 2904      cmp         r1,#4
  004030C6: D814      bhi         004030F2
  004030C8: F24A 030C mov         r3,#0xA00C
  004030CC: F2C0 0340 movt        r3,#0x40
  004030D0: F853 4021 ldr         r4,[r3,r1,lsl #2]
  004030D4: F647 3344 mov         r3,#0x7B44
  004030D8: F2C0 0340 movt        r3,#0x40
  004030DC: F1B4 3FFF cmp         r4,#0xFFFFFFFF
  004030E0: F853 3021 ldr         r3,[r3,r1,lsl #2]
  004030E4: D010      beq         00403108
  004030E6: 460A      mov         r2,r1
  004030E8: 4621      mov         r1,r4
  004030EA: F000 F8F1 bl          004032D0
  004030EE: E8BD 8818 pop         {r3,r4,r11,pc}
  004030F2: F647 3344 mov         r3,#0x7B44
  004030F6: F2C0 0340 movt        r3,#0x40
  004030FA: 2401      movs        r4,#1
  004030FC: 2105      movs        r1,#5
  004030FE: 695B      ldr         r3,[r3,#0x14]
  00403100: 460A      mov         r2,r1
  00403102: 4621      mov         r1,r4
  00403104: F000 F8E4 bl          004032D0
  00403108: E8BD 8818 pop         {r3,r4,r11,pc}


  00403124: E92D 49F0 push        {r4-r8,r11,lr}
  00403128: F10D 0B14 add         r11,sp,#0x14
  0040312C: F7FD FF7A bl          00401024
  00403130: F5AD 6D80 sub         sp,sp,#0x400
  00403134: F24A 030C mov         r3,#0xA00C
  00403138: F2C0 0340 movt        r3,#0x40
  0040313C: 460C      mov         r4,r1
  0040313E: 4680      mov         r8,r0
  00403140: 689F      ldr         r7,[r3,#8]
  00403142: F1B7 3FFF cmp         r7,#0xFFFFFFFF
  00403146: D037      beq         004031B8
  00403148: F991 3000 ldrsb       r3,[r1]
  0040314C: B353      cbz         r3,004031A4
  0040314E: 4608      mov         r0,r1
  00403150: F000 F8B2 bl          004032B8
  00403154: F100 032D add         r3,r0,#0x2D
  00403158: F5B3 6F80 cmp         r3,#0x400
  0040315C: D822      bhi         004031A4
  0040315E: F647 3574 mov         r5,#0x7B74
  00403162: F2C0 0540 movt        r5,#0x40
  00403166: 4668      mov         r0,sp
  00403168: 466E      mov         r6,sp
  0040316A: 4629      mov         r1,r5
  0040316C: F000 FE2E bl          00403DCC
  00403170: 4668      mov         r0,sp
  00403172: 1E42      subs        r2,r0,#1
  00403174: F812 3F01 ldrb        r3,[r2,#1]!
  00403178: 2B00      cmp         r3,#0
  0040317A: D1FB      bne         00403174
  0040317C: F814 3B01 ldrb        r3,[r4],#1
  00403180: 2B00      cmp         r3,#0
  00403182: F802 3B01 strb        r3,[r2],#1
  00403186: D1F9      bne         0040317C
  00403188: F105 011C add         r1,r5,#0x1C
  0040318C: 1E42      subs        r2,r0,#1
  0040318E: F812 3F01 ldrb        r3,[r2,#1]!
  00403192: 2B00      cmp         r3,#0
  00403194: D1FB      bne         0040318E
  00403196: F811 3B01 ldrb        r3,[r1],#1
  0040319A: 2B00      cmp         r3,#0
  0040319C: F802 3B01 strb        r3,[r2],#1
  004031A0: D1F9      bne         00403196
  004031A2: E003      b           004031AC
  004031A4: F647 7680 movw        r6,#0x7F80
  004031A8: F2C0 0640 movt        r6,#0x40
  004031AC: 4633      mov         r3,r6
  004031AE: 2202      movs        r2,#2
  004031B0: 4639      mov         r1,r7
  004031B2: 4640      mov         r0,r8
  004031B4: F000 F88C bl          004032D0
  004031B8: F50D 6D80 add         sp,sp,#0x400
  004031BC: F7FD FF3E bl          0040103C
  004031C0: E8BD 89F0 pop         {r4-r8,r11,pc}

  0040323C: E92D 4FFE push        {r1-r11,lr}
  00403240: F10D 0B28 add         r11,sp,#0x28
  00403244: 461E      mov         r6,r3
  00403246: 2500      movs        r5,#0
  00403248: 2700      movs        r7,#0
  0040324A: 1A13      subs        r3,r2,r0
  0040324C: 9300      str         r3,[sp]
  0040324E: 469A      mov         r10,r3
  00403250: 4688      mov         r8,r1
  00403252: 9001      str         r0,[sp,#4]
  00403254: 4681      mov         r9,r0
  00403256: 2E10      cmp         r6,#0x10
  00403258: BF38      it          cc
  0040325A: 4632      movcc       r2,r6
  0040325C: BF28      it          cs
  0040325E: 2210      movcs       r2,#0x10
  00403260: 4295      cmp         r5,r2
  00403262: D211      bcs         00403288
  00403264: F81A 4009 ldrb        r4,[r10,r9]
  00403268: F647 72A8 mov         r2,#0x7FA8
  0040326C: F2C0 0240 movt        r2,#0x40
  00403270: F1C7 0131 rsb         r1,r7,#0x31
  00403274: 4623      mov         r3,r4
  00403276: EB07 0008 add         r0,r7,r8
  0040327A: F000 FD51 bl          00403D20
  0040327E: 1C6D      adds        r5,r5,#1
  00403280: 1CFF      adds        r7,r7,#3
  00403282: F809 4B01 strb        r4,[r9],#1
  00403286: E7E6      b           00403256
  00403288: F8DD A004 ldr         r10,[sp,#4]
  0040328C: 2200      movs        r2,#0
  0040328E: EB05 0345 add         r3,r5,r5,lsl #1
  00403292: F805 200A strb        r2,[r5,r10]
  00403296: F803 2008 strb        r2,[r3,r8]
  0040329A: E8BD 8FFE pop         {r1-r11,pc}
  0040329E: 0000      movs        r0,r0
  004032A0: 0000      movs        r0,r0
  004032A2: 0000      movs        r0,r0
  004032A4: 0000      movs        r0,r0
  004032A6: 0000      movs        r0,r0
  004032A8: 0000      movs        r0,r0
  004032AA: 0000      movs        r0,r0
  004032AC: 0000      movs        r0,r0
  004032AE: 0000      movs        r0,r0
  004032B0: 0000      movs        r0,r0
  004032B2: 0000      movs        r0,r0
  004032B4: 0000      movs        r0,r0
  004032B6: 0000      movs        r0,r0
  004032B8: E92D 4800 push        {r11,lr}
  004032BC: 46EB      mov         r11,sp
  004032BE: 4602      mov         r2,r0
  004032C0: F912 3B01 ldrsb       r3,[r2],#1
  004032C4: 2B00      cmp         r3,#0
  004032C6: D1FB      bne         004032C0
  004032C8: 1A13      subs        r3,r2,r0
  004032CA: 1E58      subs        r0,r3,#1
  004032CC: E8BD 8800 pop         {r11,pc}
  004032D0: E92D 4FF0 push        {r4-r11,lr}
  004032D4: F10D 0B1C add         r11,sp,#0x1C
  004032D8: F7FD FEA4 bl          00401024
  004032DC: F6AD 6D48 sub         sp,sp,#0xE48
  004032E0: 4698      mov         r8,r3
  004032E2: 9305      str         r3,[sp,#0x14]
  004032E4: 4691      mov         r9,r2
  004032E6: 468A      mov         r10,r1
  004032E8: 4606      mov         r6,r0
  004032EA: 2700      movs        r7,#0
  004032EC: F7FF FA3A bl          00402764
  004032F0: 4605      mov         r5,r0
  004032F2: B91D      cbnz        r5,004032FC
  004032F4: 4630      mov         r0,r6
  004032F6: F7FF FA2B bl          00402750
  004032FA: 4607      mov         r7,r0
  004032FC: F24C 0428 mov         r4,#0xC028
  00403300: F2C0 0440 movt        r4,#0x40
  00403304: 2300      movs        r3,#0
  00403306: 9301      str         r3,[sp,#4]
  0040330A: 9300      str         r3,[sp]
  0040330C: 43DB      mvns        r3,r3
  0040330E: 4642      mov         r2,r8
  00403310: 2100      movs        r1,#0
  00403312: F64F 50E9 mov         r0,#0xFDE9
  00403316: 47A0      blx         r4
  00403318: F5B0 7F00 cmp         r0,#0x200
  0040331C: D213      bcs         00403346
  0040331E: F24C 0428 mov         r4,#0xC028
  00403322: F2C0 0440 movt        r4,#0x40
  00403326: F60D 2348 add         r3,sp,#0xA48
  0040332A: 9001      str         r0,[sp,#4]
  0040332E: 9300      str         r3,[sp]
  00403330: F06F 0300 mvn         r3,#0
  00403334: 4642      mov         r2,r8
  00403336: 2100      movs        r1,#0
  00403338: F64F 50E9 mov         r0,#0xFDE9
  0040333C: 47A0      blx         r4
  0040333E: B110      cbz         r0,00403346
  00403340: F60D 2448 add         r4,sp,#0xA48
  00403344: E003      b           0040334E
  00403346: F647 6488 mov         r4,#0x7E88
  0040334A: F2C0 0440 movt        r4,#0x40
  0040334E: F241 0002 mov         r0,#0x1002
  00403352: F7FF FDB7 bl          00402EC4
  00403356: B170      cbz         r0,00403376
  00403358: F647 335C mov         r3,#0x7B5C
  0040335C: F2C0 0340 movt        r3,#0x40
  00403360: 4632      mov         r2,r6
  00403362: 4648      mov         r0,r9
  00403364: F853 1029 ldr         r1,[r3,r9,lsl #2]
  00403368: 4623      mov         r3,r4
  0040336A: F7FF FDCD bl          00402F08
  0040336E: 2800      cmp         r0,#0
  00403370: D16E      bne         00403450
  00403372: 2300      movs        r3,#0
  00403374: E000      b           00403378
  00403376: 2301      movs        r3,#1
  00403378: B90F      cbnz        r7,0040337E
  0040337A: 2D00      cmp         r5,#0
  0040337C: D067      beq         0040344E
  0040337E: B13B      cbz         r3,00403390
  00403380: F24C 0330 mov         r3,#0xC030
  00403384: F2C0 0340 movt        r3,#0x40
  0040338A: 4798      blx         r3
  0040338C: 2800      cmp         r0,#0
  0040338E: D15E      bne         0040344E
  00403390: AB06      add         r3,sp,#0x18
  00403392: 9300      str         r3,[sp]
  00403394: F44F 7282 mov         r2,#0x104
  00403398: AB04      add         r3,sp,#0x10
  0040339A: A988      add         r1,sp,#0x220
  0040339C: 1F30      subs        r0,r6,#4
  0040339E: 9201      str         r2,[sp,#4]
  004033A0: F000 FB4E bl          00403A40
  004033A4: B16D      cbz         r5,004033C2
  004033A6: F647 63F8 mov         r3,#0x7EF8
  004033AA: F2C0 0340 movt        r3,#0x40
  004033AE: 9A04      ldr         r2,[sp,#0x10]
  004033B0: 4650      mov         r0,r10
  004033B2: 9300      str         r3,[sp]
  004033B4: AB06      add         r3,sp,#0x18
  004033B6: A988      add         r1,sp,#0x220
  004033B8: 9402      str         r4,[sp,#8]
  004033BA: F8CD 9004 str         r9,[sp,#4]
  004033BE: 47A8      blx         r5
  004033C0: E043      b           0040344A
  004033C2: F24C 0424 mov         r4,#0xC024
  004033C6: F2C0 0440 movt        r4,#0x40
  004033CA: 2300      movs        r3,#0
  004033CC: F50D 6285 add         r2,sp,#0x428
  004033D2: F647 7638 mov         r6,#0x7F38
  004033D6: F2C0 0640 movt        r6,#0x40
  004033DA: 9303      str         r3,[sp,#0xC]
  004033DC: 9302      str         r3,[sp,#8]
  004033DE: F240 380A mov         r8,#0x30A
  004033E2: 9200      str         r2,[sp]
  004033E4: 43DB      mvns        r3,r3
  004033E6: AA88      add         r2,sp,#0x220
  004033E8: 2100      movs        r1,#0
  004033EA: F64F 50E9 mov         r0,#0xFDE9
  004033EE: F8CD 8004 str         r8,[sp,#4]
  004033F2: 47A0      blx         r4
  004033F4: B108      cbz         r0,004033FA
  004033F6: F50D 6685 add         r6,sp,#0x428
  004033FA: F24C 0424 mov         r4,#0xC024
  004033FE: F2C0 0440 movt        r4,#0x40
  00403402: 2300      movs        r3,#0
  00403404: F50D 62E7 add         r2,sp,#0x738
  0040340A: F647 754C mov         r5,#0x7F4C
  0040340E: F2C0 0540 movt        r5,#0x40
  00403412: 9303      str         r3,[sp,#0xC]
  00403414: 9302      str         r3,[sp,#8]
  00403416: 9200      str         r2,[sp]
  00403418: 43DB      mvns        r3,r3
  0040341A: AA06      add         r2,sp,#0x18
  0040341C: 2100      movs        r1,#0
  0040341E: F64F 50E9 mov         r0,#0xFDE9
  00403422: F8CD 8004 str         r8,[sp,#4]
  00403426: 47A0      blx         r4
  00403428: B108      cbz         r0,0040342E
  0040342A: F50D 65E7 add         r5,sp,#0x738
  0040342E: F647 7260 mov         r2,#0x7F60
  00403432: F2C0 0240 movt        r2,#0x40
  00403436: 9B05      ldr         r3,[sp,#0x14]
  00403438: 4631      mov         r1,r6
  0040343A: 9200      str         r2,[sp]
  0040343C: 9A04      ldr         r2,[sp,#0x10]
  0040343E: 9302      str         r3,[sp,#8]
  00403440: 462B      mov         r3,r5
  00403442: 4650      mov         r0,r10
  00403444: F8CD 9004 str         r9,[sp,#4]
  00403448: 47B8      blx         r7
  0040344A: 2801      cmp         r0,#1
  0040344C: D100      bne         00403450
  0040344E: DEFE      __debugbreak
  00403450: F60D 6D48 add         sp,sp,#0xE48
  00403454: F7FD FDF2 bl          0040103C
  00403458: E8BD 8FF0 pop         {r4-r11,pc}


  00403504: E92D 48F0 push        {r4-r7,r11,lr}
  00403508: F10D 0B10 add         r11,sp,#0x10
  0040350C: F7FD FD8A bl          00401024
  00403510: F2AD 4D04 sub         sp,sp,#0x404
  00403514: F24A 030C mov         r3,#0xA00C
  00403518: F2C0 0340 movt        r3,#0x40
  0040351C: 4604      mov         r4,r0
  0040351E: 68DF      ldr         r7,[r3,#0xC]
  00403520: F1B7 3FFF cmp         r7,#0xFFFFFFFF
  00403524: D035      beq         00403592
  00403526: B34C      cbz         r4,0040357C
  00403528: F7FF FEC6 bl          004032B8
  0040352C: F100 033A add         r3,r0,#0x3A
  00403530: F5B3 6F80 cmp         r3,#0x400
  00403534: D822      bhi         0040357C
  00403536: F647 35A4 mov         r5,#0x7BA4
  0040353A: F2C0 0540 movt        r5,#0x40
  0040353E: 4668      mov         r0,sp
  00403540: 466E      mov         r6,sp
  00403542: 4629      mov         r1,r5
  00403544: F000 FC42 bl          00403DCC
  00403548: 4668      mov         r0,sp
  0040354A: 1E42      subs        r2,r0,#1
  0040354C: F812 3F01 ldrb        r3,[r2,#1]!
  00403550: 2B00      cmp         r3,#0
  00403552: D1FB      bne         0040354C
  00403554: F814 3B01 ldrb        r3,[r4],#1
  00403558: 2B00      cmp         r3,#0
  0040355A: F802 3B01 strb        r3,[r2],#1
  0040355E: D1F9      bne         00403554
  00403560: F105 0110 add         r1,r5,#0x10
  00403564: 1E42      subs        r2,r0,#1
  00403566: F812 3F01 ldrb        r3,[r2,#1]!
  0040356A: 2B00      cmp         r3,#0
  0040356C: D1FB      bne         00403566
  0040356E: F811 3B01 ldrb        r3,[r1],#1
  00403572: 2B00      cmp         r3,#0
  00403574: F802 3B01 strb        r3,[r2],#1
  00403578: D1F9      bne         0040356E
  0040357A: E003      b           00403584
  0040357C: F248 06C0 mov         r6,#0x80C0
  00403580: F2C0 0640 movt        r6,#0x40
  00403584: F8DD 041C ldr         r0,[sp,#0x41C]
  00403588: 4633      mov         r3,r6
  0040358A: 2203      movs        r2,#3
  0040358C: 4639      mov         r1,r7
  0040358E: F7FF FE9F bl          004032D0
  00403592: F20D 4D04 add         sp,sp,#0x404
  00403596: F7FD FD51 bl          0040103C
  0040359A: E8BD 88F0 pop         {r4-r7,r11,pc}

  00403618: F24C 0CB8 mov         r12,#0xC0B8
  0040361C: F2C0 0C40 movt        r12,#0x40
  00403620: F8DC F000 ldr         pc,[r12]
  00403624: F24C 0CA0 mov         r12,#0xC0A0
  00403628: F2C0 0C40 movt        r12,#0x40
  0040362C: F8DC F000 ldr         pc,[r12]
  00403630: F24C 0C90 mov         r12,#0xC090
  00403634: F2C0 0C40 movt        r12,#0x40
  00403638: F8DC F000 ldr         pc,[r12]
  0040363C: F24C 0C94 mov         r12,#0xC094
  00403640: F2C0 0C40 movt        r12,#0x40
  00403644: F8DC F000 ldr         pc,[r12]
  00403648: F24C 0C98 mov         r12,#0xC098
  0040364C: F2C0 0C40 movt        r12,#0x40
  00403650: F8DC F000 ldr         pc,[r12]
  00403654: F24C 0C9C mov         r12,#0xC09C
  00403658: F2C0 0C40 movt        r12,#0x40
  0040365C: F8DC F000 ldr         pc,[r12]
  00403660: E92D 4800 push        {r11,lr}
  00403664: 46EB      mov         r11,sp
  00403666: F7FD FCDD bl          00401024
  0040366A: F2AD 4D14 sub         sp,sp,#0x414
  0040366E: F24A 1270 mov         r2,#0xA170
  00403672: F2C0 0240 movt        r2,#0x40
  00403676: 7813      ldrb        r3,[r2]
  00403678: B133      cbz         r3,00403688
  0040367A: 2000      movs        r0,#0
  0040367C: F20D 4D14 add         sp,sp,#0x414
  00403680: F7FD FCDC bl          0040103C
  00403684: E8BD 8800 pop         {r11,pc}
  00403688: 2301      movs        r3,#1
  0040368A: 7013      strb        r3,[r2]
  0040368C: F000 F86E bl          0040376C
  00403690: 2800      cmp         r0,#0
  00403692: D1F3      bne         0040367C
  00403694: F24C 0304 mov         r3,#0xC004
  00403698: F2C0 0340 movt        r3,#0x40
  0040369C: F248 1034 mov         r0,#0x8134
  004036A0: F2C0 0040 movt        r0,#0x40
  004036A6: 4798      blx         r3
  004036A8: B1E0      cbz         r0,004036E4
  004036AA: F24C 0308 mov         r3,#0xC008
  004036AE: F2C0 0340 movt        r3,#0x40
  004036B2: F44F 7282 mov         r2,#0x104
  004036B6: 4669      mov         r1,sp
  004036BA: 4798      blx         r3
  004036BC: B190      cbz         r0,004036E4
  004036BE: A982      add         r1,sp,#0x208
  004036C0: 4668      mov         r0,sp
  004036C2: F44F 7282 mov         r2,#0x104
  004036C6: F000 F935 bl          00403934
  004036CA: B158      cbz         r0,004036E4
  004036CC: F24C 0350 mov         r3,#0xC050
  004036D0: F2C0 0340 movt        r3,#0x40
  004036D4: F44F 6210 mov         r2,#0x900
  004036D8: 2100      movs        r1,#0
  004036DC: A882      add         r0,sp,#0x208
  004036DE: 4798      blx         r3
  004036E0: 2800      cmp         r0,#0
  004036E2: D1CB      bne         0040367C
  004036E4: F24C 0350 mov         r3,#0xC050
  004036E8: F2C0 0340 movt        r3,#0x40
  004036EC: F248 2058 mov         r0,#0x8258
  004036F0: F2C0 0040 movt        r0,#0x40
  004036F4: F44F 6220 mov         r2,#0xA00
  004036FA: 2100      movs        r1,#0
  004036FC: 4798      blx         r3
  004036FE: F20D 4D14 add         sp,sp,#0x414
  00403702: F7FD FC9B bl          0040103C
  00403706: E8BD 8800 pop         {r11,pc}
  0040370A: 0000      movs        r0,r0
  0040370C: 0000      movs        r0,r0
  0040370E: 0000      movs        r0,r0
  00403710: 0000      movs        r0,r0
  00403712: 0000      movs        r0,r0
  00403714: 0000      movs        r0,r0
  00403716: 0000      movs        r0,r0
  00403718: 0000      movs        r0,r0
  0040371A: 0000      movs        r0,r0
  0040371C: 0000      movs        r0,r0
  0040371E: 0000      movs        r0,r0
  00403720: 0000      movs        r0,r0
  00403722: 0000      movs        r0,r0
  00403724: 0000      movs        r0,r0
  00403726: 0000      movs        r0,r0
  00403728: 0000      movs        r0,r0
  0040372A: 0000      movs        r0,r0
  0040372C: 0000      movs        r0,r0
  0040372E: 0000      movs        r0,r0
  00403730: 0000      movs        r0,r0
  00403732: 0000      movs        r0,r0
  00403734: 0000      movs        r0,r0
  00403736: 0000      movs        r0,r0
  00403738: 0000      movs        r0,r0
  0040373A: 0000      movs        r0,r0
  0040373C: 0000      movs        r0,r0
  0040373E: 0000      movs        r0,r0
  00403740: 0000      movs        r0,r0
  00403742: 0000      movs        r0,r0
  00403744: 0000      movs        r0,r0
  00403746: 0000      movs        r0,r0
  00403748: 0000      movs        r0,r0
  0040374A: 0000      movs        r0,r0
  0040374C: 0000      movs        r0,r0
  0040374E: 0000      movs        r0,r0
  00403750: 0000      movs        r0,r0
  00403752: 0000      movs        r0,r0
  00403754: 0000      movs        r0,r0
  00403756: 0000      movs        r0,r0
  00403758: 0000      movs        r0,r0
  0040375A: 0000      movs        r0,r0
  0040375C: 0000      movs        r0,r0
  0040375E: 0000      movs        r0,r0
  00403760: 0000      movs        r0,r0
  00403762: 0000      movs        r0,r0
  00403764: 0000      movs        r0,r0
  00403766: 0000      movs        r0,r0
  00403768: 0000      movs        r0,r0
  0040376A: 0000      movs        r0,r0
  0040376C: E92D 48F0 push        {r4-r7,r11,lr}
  00403770: F10D 0B10 add         r11,sp,#0x10
  00403774: F7FD FC56 bl          00401024
  00403778: F5AD 7D09 sub         sp,sp,#0x224
  0040377C: F24C 0350 mov         r3,#0xC050
  00403780: F2C0 0340 movt        r3,#0x40
  00403784: F248 1050 mov         r0,#0x8150
  00403788: F2C0 0040 movt        r0,#0x40
  0040378C: F44F 6200 mov         r2,#0x800
  00403792: 2100      movs        r1,#0
  00403794: 4798      blx         r3
  00403796: 4605      mov         r5,r0
  00403798: B9AD      cbnz        r5,004037C6
  0040379A: F24C 0320 mov         r3,#0xC020
  0040379E: F2C0 0340 movt        r3,#0x40
  004037A4: 4798      blx         r3
  004037A6: 2857      cmp         r0,#0x57
  004037A8: D148      bne         0040383C
  004037AA: F24C 0350 mov         r3,#0xC050
  004037AE: F2C0 0340 movt        r3,#0x40
  004037B2: F248 1050 mov         r0,#0x8150
  004037B6: F2C0 0040 movt        r0,#0x40
  004037BA: 2200      movs        r2,#0
  004037BE: 2100      movs        r1,#0
  004037C0: 4798      blx         r3
  004037C2: 4605      mov         r5,r0
  004037C4: B3D5      cbz         r5,0040383C
  004037C6: F24C 0300 movw        r3,#0xC000
  004037CA: F2C0 0340 movt        r3,#0x40
  004037CE: F248 1198 mov         r1,#0x8198
  004037D2: F2C0 0140 movt        r1,#0x40
  004037D6: 4628      mov         r0,r5
  004037DA: 4798      blx         r3
  004037DC: 4604      mov         r4,r0
  004037DE: B36C      cbz         r4,0040383C
  004037E0: F24C 0300 movw        r3,#0xC000
  004037E4: F2C0 0340 movt        r3,#0x40
  004037E8: F248 11A8 mov         r1,#0x81A8
  004037EC: F2C0 0140 movt        r1,#0x40
  004037F0: 4628      mov         r0,r5
  004037F4: 4798      blx         r3
  004037F6: 4606      mov         r6,r0
  004037F8: B306      cbz         r6,0040383C
  004037FA: F24C 0300 movw        r3,#0xC000
  004037FE: F2C0 0340 movt        r3,#0x40
  00403802: F248 11BC mov         r1,#0x81BC
  00403806: F2C0 0140 movt        r1,#0x40
  0040380A: 4628      mov         r0,r5
  0040380E: 4798      blx         r3
  00403810: 4607      mov         r7,r0
  00403812: B19F      cbz         r7,0040383C
  00403814: F248 11C8 mov         r1,#0x81C8
  00403818: F2C0 0140 movt        r1,#0x40
  0040381C: AB03      add         r3,sp,#0xC
  0040381E: F04F 4000 mov         r0,#0x80000000
  00403822: 9300      str         r3,[sp]
  00403824: 2301      movs        r3,#1
  00403826: 2200      movs        r2,#0
  00403828: 1C80      adds        r0,r0,#2
  0040382A: 47A0      blx         r4
  0040382C: B168      cbz         r0,0040384A
  0040382E: F24C 030C mov         r3,#0xC00C
  00403832: F2C0 0340 movt        r3,#0x40
  00403836: 4628      mov         r0,r5
  0040383A: 4798      blx         r3
  0040383C: 2000      movs        r0,#0
  0040383E: F50D 7D09 add         sp,sp,#0x224
  00403842: F7FD FBFB bl          0040103C
  00403846: E8BD 88F0 pop         {r4-r7,r11,pc}
  0040384A: F44F 7302 mov         r3,#0x208
  0040384E: 9302      str         r3,[sp,#8]
  00403850: AB02      add         r3,sp,#8
  00403852: 9301      str         r3,[sp,#4]
  00403854: F248 2124 mov         r1,#0x8224
  00403858: F2C0 0140 movt        r1,#0x40
  0040385C: 9803      ldr         r0,[sp,#0xC]
  0040385E: AB06      add         r3,sp,#0x18
  00403860: 9300      str         r3,[sp]
  00403862: AB04      add         r3,sp,#0x10
  00403864: 2200      movs        r2,#0
  00403866: 47B0      blx         r6
  00403868: 4604      mov         r4,r0
  0040386A: 9803      ldr         r0,[sp,#0xC]
  0040386C: 47B8      blx         r7
  0040386E: F24C 030C mov         r3,#0xC00C
  00403872: F2C0 0340 movt        r3,#0x40
  00403876: 4628      mov         r0,r5
  0040387A: 4798      blx         r3
  0040387C: 2C00      cmp         r4,#0
  0040387E: D1DD      bne         0040383C
  00403880: 9B04      ldr         r3,[sp,#0x10]
  00403882: 2B01      cmp         r3,#1
  00403884: D1DA      bne         0040383C
  00403886: 9B02      ldr         r3,[sp,#8]
  00403888: F013 0F01 tst         r3,#1
  0040388C: D1D6      bne         0040383C
  0040388E: 085A      lsrs        r2,r3,#1
  00403890: 2A02      cmp         r2,#2
  00403892: D3D3      bcc         0040383C
  00403894: 1E52      subs        r2,r2,#1
  00403896: AB06      add         r3,sp,#0x18
  00403898: EB03 0142 add         r1,r3,r2,lsl #1
  0040389C: F833 3012 ldrh        r3,[r3,r2,lsl #1]
  004038A0: 2B00      cmp         r3,#0
  004038A2: D1CB      bne         0040383C
  004038A4: F831 3C02 ldrh        r3,[r1,#-2]
  004038A8: 2B5C      cmp         r3,#0x5C
  004038AA: D002      beq         004038B2
  004038AC: 235C      movs        r3,#0x5C
  004038AE: 800B      strh        r3,[r1]
  004038B0: 1C52      adds        r2,r2,#1
  004038B2: F1C2 33FF rsb         r3,r2,#0xFFFFFFFF
  004038B6: 2B12      cmp         r3,#0x12
  004038B8: D3C0      bcc         0040383C
  004038BA: F102 0311 add         r3,r2,#0x11
  004038BE: F5B3 7F82 cmp         r3,#0x104
  004038C2: D8BB      bhi         0040383C
  004038C4: AB06      add         r3,sp,#0x18
  004038C6: EB03 0242 add         r2,r3,r2,lsl #1
  004038CA: F248 1110 mov         r1,#0x8110
  004038CE: F2C0 0140 movt        r1,#0x40
  004038D2: F102 0022 add         r0,r2,#0x22
  004038D6: F831 3B02 ldrh        r3,[r1],#2
  004038DA: F822 3B02 strh        r3,[r2],#2
  004038DE: 4282      cmp         r2,r0
  004038E0: D1F9      bne         004038D6
  004038E2: F24C 0350 mov         r3,#0xC050
  004038E6: F2C0 0340 movt        r3,#0x40
  004038EA: 2100      movs        r1,#0
  004038EC: F44F 6210 mov         r2,#0x900
  004038F2: A806      add         r0,sp,#0x18
  004038F4: 4798      blx         r3
  004038F6: F50D 7D09 add         sp,sp,#0x224
  004038FA: F7FD FB9F bl          0040103C
  004038FE: E8BD 88F0 pop         {r4-r7,r11,pc}
  00403902: 0000      movs        r0,r0
  00403904: 0000      movs        r0,r0
  00403906: 0000      movs        r0,r0
  00403908: 0000      movs        r0,r0
  0040390A: 0000      movs        r0,r0
  0040390C: 0000      movs        r0,r0
  0040390E: 0000      movs        r0,r0
  00403910: 0000      movs        r0,r0
  00403912: 0000      movs        r0,r0
  00403914: 0000      movs        r0,r0
  00403916: 0000      movs        r0,r0
  00403918: 0000      movs        r0,r0
  0040391A: 0000      movs        r0,r0
  0040391C: 0000      movs        r0,r0
  0040391E: 0000      movs        r0,r0
  00403920: 0000      movs        r0,r0
  00403922: 0000      movs        r0,r0
  00403924: 0000      movs        r0,r0
  00403926: 0000      movs        r0,r0
  00403928: 0000      movs        r0,r0
  0040392A: 0000      movs        r0,r0
  0040392C: 0000      movs        r0,r0
  0040392E: 0000      movs        r0,r0
  00403930: 0000      movs        r0,r0
  00403932: 0000      movs        r0,r0
  00403934: E92D 4870 push        {r4-r6,r11,lr}
  00403938: F10D 0B0C add         r11,sp,#0xC
  0040393C: F7FD FB72 bl          00401024
  00403940: F5AD 6DC4 sub         sp,sp,#0x620
  00403944: AB08      add         r3,sp,#0x20
  00403946: 9303      str         r3,[sp,#0xC]
  00403948: AB88      add         r3,sp,#0x220
  0040394A: 9301      str         r3,[sp,#4]
  0040394C: 4615      mov         r5,r2
  0040394E: 460E      mov         r6,r1
  00403950: F44F 7480 mov         r4,#0x100
  00403954: F50D 6384 add         r3,sp,#0x420
  00403958: A906      add         r1,sp,#0x18
  0040395A: 2203      movs        r2,#3
  0040395C: 9404      str         r4,[sp,#0x10]
  0040395E: 9402      str         r4,[sp,#8]
  00403960: 9400      str         r4,[sp]
  00403962: F000 FA45 bl          00403DF0
  00403966: B130      cbz         r0,00403976
  00403968: 2000      movs        r0,#0
  0040396A: F50D 6DC4 add         sp,sp,#0x620
  0040396E: F7FD FB65 bl          0040103C
  00403972: E8BD 8870 pop         {r4-r6,r11,pc}
  00403976: F248 2244 mov         r2,#0x8244
  0040397A: F2C0 0240 movt        r2,#0x40
  0040397E: A888      add         r0,sp,#0x220
  00403980: 2109      movs        r1,#9
  00403982: F000 FA29 bl          00403DD8
  00403986: 2800      cmp         r0,#0
  00403988: D1EE      bne         00403968
  0040398A: F248 223C mov         r2,#0x823C
  0040398E: F2C0 0240 movt        r2,#0x40
  00403992: A808      add         r0,sp,#0x20
  00403994: 2104      movs        r1,#4
  00403996: F000 FA1F bl          00403DD8
  0040399A: 2800      cmp         r0,#0
  0040399C: D1E4      bne         00403968
  0040399E: AB08      add         r3,sp,#0x20
  004039A0: 9301      str         r3,[sp,#4]
  004039A2: AB88      add         r3,sp,#0x220
  004039A4: 9300      str         r3,[sp]
  004039A6: F50D 6384 add         r3,sp,#0x420
  004039AA: AA06      add         r2,sp,#0x18
  004039AC: 4629      mov         r1,r5
  004039AE: 4630      mov         r0,r6
  004039B0: F000 FA18 bl          00403DE4
  004039B4: 2800      cmp         r0,#0
  004039B6: D1D7      bne         00403968
  004039B8: 2001      movs        r0,#1
  004039BA: F50D 6DC4 add         sp,sp,#0x620
  004039BE: F7FD FB3D bl          0040103C
  004039C2: E8BD 8870 pop         {r4-r6,r11,pc}

  00403A40: B40F      push        {r0-r3}
  00403A42: E92D 4FF0 push        {r4-r11,lr}
  00403A46: F10D 0B1C add         r11,sp,#0x1C
  00403A4A: B097      sub         sp,sp,#0x5C
  00403A4C: F24C 0410 mov         r4,#0xC010
  00403A50: F2C0 0440 movt        r4,#0x40
  00403A54: 4605      mov         r5,r0
  00403A56: F04F 0800 mov         r8,#0
  00403A5C: F8C3 8000 str         r8,[r3]
  00403A60: 1E6D      subs        r5,r5,#1
  00403A62: 910D      str         r1,[sp,#0x34]
  00403A64: F8A1 8000 strh        r8,[r1]
  00403A68: 221C      movs        r2,#0x1C
  00403A6A: A910      add         r1,sp,#0x40
  00403A6C: 4628      mov         r0,r5
  00403A6E: 930C      str         r3,[sp,#0x30]
  00403A70: 47A0      blx         r4
  00403A72: B930      cbnz        r0,00403A82
  00403A74: 2600      movs        r6,#0
  00403A76: 4630      mov         r0,r6
  00403A78: B017      add         sp,sp,#0x5C
  00403A7A: E8BD 0FF0 pop         {r4-r11}
  00403A7E: F85D FB14 ldr         pc,[sp],#0x14
  00403A82: F24C 0308 mov         r3,#0xC008
  00403A86: F2C0 0340 movt        r3,#0x40
  00403A8A: 9A25      ldr         r2,[sp,#0x94]
  00403A8C: 9924      ldr         r1,[sp,#0x90]
  00403A90: 9811      ldr         r0,[sp,#0x44]
  00403A92: 4798      blx         r3
  00403A94: 2800      cmp         r0,#0
  00403A96: D0ED      beq         00403A74
  00403A98: 9911      ldr         r1,[sp,#0x44]
  00403A9A: F645 234D mov         r3,#0x5A4D
  00403A9E: 880A      ldrh        r2,[r1]
  00403AA0: 429A      cmp         r2,r3
  00403AA2: D1E7      bne         00403A74
  00403AA4: 6BCB      ldr         r3,[r1,#0x3C]
  00403AA6: 2B00      cmp         r3,#0
  00403AA8: DDE4      ble         00403A74
  00403AAA: 585A      ldr         r2,[r3,r1]
  00403AAC: 185C      adds        r4,r3,r1
  00403AAE: F244 5350 mov         r3,#0x4550
  00403AB2: 429A      cmp         r2,r3
  00403AB4: D1DE      bne         00403A74
  00403AB6: 8AA3      ldrh        r3,[r4,#0x14]
  00403AB8: 88E0      ldrh        r0,[r4,#6]
  00403ABA: 1A69      subs        r1,r5,r1
  00403ABC: 4423      add         r3,r3,r4
  00403ABE: 2700      movs        r7,#0
  00403AC0: 2500      movs        r5,#0
  00403AC2: B160      cbz         r0,00403ADE
  00403AC4: F103 0218 add         r2,r3,#0x18
  00403AC8: 68D3      ldr         r3,[r2,#0xC]
  00403ACA: 4299      cmp         r1,r3
  00403ACC: D303      bcc         00403AD6
  00403ACE: 1ACF      subs        r7,r1,r3
  00403AD0: 6893      ldr         r3,[r2,#8]
  00403AD2: 4299      cmp         r1,r3
  00403AD4: D303      bcc         00403ADE
  00403AD6: 1C6D      adds        r5,r5,#1
  00403AD8: 3228      adds        r2,r2,#0x28
  00403ADA: 4285      cmp         r5,r0
  00403ADC: D3F4      bcc         00403AC8
  00403ADE: 4285      cmp         r5,r0
  00403AE0: D0C8      beq         00403A74
  00403AE2: F24A 1671 mov         r6,#0xA171
  00403AE6: F2C0 0640 movt        r6,#0x40
  00403AEA: F24A 146C mov         r4,#0xA16C
  00403AEE: F2C0 0440 movt        r4,#0x40
  00403AF2: 1C6D      adds        r5,r5,#1
  00403AF4: 7833      ldrb        r3,[r6]
  00403AF6: B953      cbnz        r3,00403B0E
  00403AFA: 2B00      cmp         r3,#0
  00403AFC: D1BA      bne         00403A74
  00403AFE: F7FF FDAF bl          00403660
  00403B02: 6020      str         r0,[r4]
  00403B04: 2800      cmp         r0,#0
  00403B06: D0B5      beq         00403A74
  00403B08: 2301      movs        r3,#1
  00403B0A: 7033      strb        r3,[r6]
  00403B0C: E000      b           00403B10
  00403B10: F24C 0300 movw        r3,#0xC000
  00403B14: F2C0 0340 movt        r3,#0x40
  00403B18: F248 216C mov         r1,#0x826C
  00403B1C: F2C0 0140 movt        r1,#0x40
  00403B22: 4798      blx         r3
  00403B24: 4604      mov         r4,r0
  00403B26: 2C00      cmp         r4,#0
  00403B28: D0A4      beq         00403A74
  00403B2A: AB08      add         r3,sp,#0x20
  00403B2C: 9303      str         r3,[sp,#0xC]
  00403B2E: 9824      ldr         r0,[sp,#0x90]
  00403B30: AB0F      add         r3,sp,#0x3C
  00403B32: 9300      str         r3,[sp]
  00403B34: 2300      movs        r3,#0
  00403B36: 2200      movs        r2,#0
  00403B38: 2100      movs        r1,#0
  00403B3A: F8CD 8008 str         r8,[sp,#8]
  00403B3E: F8CD 8004 str         r8,[sp,#4]
  00403B42: 47A0      blx         r4
  00403B44: 2800      cmp         r0,#0
  00403B46: D095      beq         00403A74
  00403B48: 9B08      ldr         r3,[sp,#0x20]
  00403B4A: 2600      movs        r6,#0
  00403B4C: 4618      mov         r0,r3
  00403B52: 4798      blx         r3
  00403B54: 4B6B      ldr         r3,00403D04
  00403B56: 4298      cmp         r0,r3
  00403B58: F040 80C8 bne         00403CEC
  00403B5C: 9C08      ldr         r4,[sp,#0x20]
  00403B5E: F248 2280 mov         r2,#0x8280
  00403B62: F2C0 0240 movt        r2,#0x40
  00403B66: AB0A      add         r3,sp,#0x28
  00403B68: 4620      mov         r0,r4
  00403B6C: 2100      movs        r1,#0
  00403B6E: 69E4      ldr         r4,[r4,#0x1C]
  00403B70: 47A0      blx         r4
  00403B72: 2800      cmp         r0,#0
  00403B74: F000 80BA beq         00403CEC
  00403B78: 9C0A      ldr         r4,[sp,#0x28]
  00403B7A: AB07      add         r3,sp,#0x1C
  00403B7C: 463A      mov         r2,r7
  00403B7E: 4620      mov         r0,r4
  00403B82: B2A9      uxth        r1,r5
  00403B84: F8CD 8008 str         r8,[sp,#8]
  00403B88: 6A24      ldr         r4,[r4,#0x20]
  00403B8A: F8CD 8004 str         r8,[sp,#4]
  00403B8E: F8CD 8000 str         r8,[sp]
  00403B92: 47A0      blx         r4
  00403B94: 2800      cmp         r0,#0
  00403B96: F000 80A4 beq         00403CE2
  00403B9A: 2300      movs        r3,#0
  00403B9C: 9305      str         r3,[sp,#0x14]
  00403B9E: 9B07      ldr         r3,[sp,#0x1C]
  00403BA0: A905      add         r1,sp,#0x14
  00403BA2: 4618      mov         r0,r3
  00403BA6: 6E9B      ldr         r3,[r3,#0x68]
  00403BA8: 4798      blx         r3
  00403BAA: 2800      cmp         r0,#0
  00403BAC: F000 8094 beq         00403CD8
  00403BB0: 9B05      ldr         r3,[sp,#0x14]
  00403BB2: 2B00      cmp         r3,#0
  00403BB4: F000 8090 beq         00403CD8
  00403BB8: 4618      mov         r0,r3
  00403BBC: 46B0      mov         r8,r6
  00403BBE: 689B      ldr         r3,[r3,#8]
  00403BC0: 4798      blx         r3
  00403BC2: 2800      cmp         r0,#0
  00403BC4: D075      beq         00403CB2
  00403BC6: F10D 0918 add         r9,sp,#0x18
  00403BCA: F10D 0A2C add         r10,sp,#0x2C
  00403BCE: 9C05      ldr         r4,[sp,#0x14]
  00403BD0: 2300      movs        r3,#0
  00403BD2: 9302      str         r3,[sp,#8]
  00403BD4: 4620      mov         r0,r4
  00403BD8: AB04      add         r3,sp,#0x10
  00403BDA: AA09      add         r2,sp,#0x24
  00403BDC: 68E4      ldr         r4,[r4,#0xC]
  00403BDE: 2100      movs        r1,#0
  00403BE0: F8CD 9004 str         r9,[sp,#4]
  00403BE4: F8CD A000 str         r10,[sp]
  00403BE8: 47A0      blx         r4
  00403BEA: 2800      cmp         r0,#0
  00403BEC: D06F      beq         00403CCE
  00403BEE: F8BD 3010 ldrh        r3,[sp,#0x10]
  00403BF2: 42AB      cmp         r3,r5
  00403BF4: D106      bne         00403C04
  00403BF6: 9A09      ldr         r2,[sp,#0x24]
  00403BF8: 42BA      cmp         r2,r7
  00403BFA: D803      bhi         00403C04
  00403BFC: 9B0B      ldr         r3,[sp,#0x2C]
  00403BFE: 4413      add         r3,r3,r2
  00403C00: 429F      cmp         r7,r3
  00403C02: D307      bcc         00403C14
  00403C04: 9B05      ldr         r3,[sp,#0x14]
  00403C06: 4618      mov         r0,r3
  00403C0A: 689B      ldr         r3,[r3,#8]
  00403C0C: 4798      blx         r3
  00403C0E: 2800      cmp         r0,#0
  00403C10: D1DD      bne         00403BCE
  00403C12: E04E      b           00403CB2
  00403C14: 9A06      ldr         r2,[sp,#0x18]
  00403C16: 2A00      cmp         r2,#0
  00403C18: D059      beq         00403CCE
  00403C1A: F06F 4360 mvn         r3,#0xE0000000
  00403C1E: 429A      cmp         r2,r3
  00403C20: D255      bcs         00403CCE
  00403C22: F24C 0314 mov         r3,#0xC014
  00403C26: F2C0 0340 movt        r3,#0x40
  00403C2C: 4798      blx         r3
  00403C2E: 9B06      ldr         r3,[sp,#0x18]
  00403C30: 2100      movs        r1,#0
  00403C32: 00DA      lsls        r2,r3,#3
  00403C34: F24C 031C mov         r3,#0xC01C
  00403C38: F2C0 0340 movt        r3,#0x40
  00403C3E: 4798      blx         r3
  00403C40: 4680      mov         r8,r0
  00403C42: 2800      cmp         r0,#0
  00403C44: D043      beq         00403CCE
  00403C46: 9C05      ldr         r4,[sp,#0x14]
  00403C48: 9002      str         r0,[sp,#8]
  00403C4A: AB06      add         r3,sp,#0x18
  00403C4C: 4620      mov         r0,r4
  00403C50: 9301      str         r3,[sp,#4]
  00403C52: 2500      movs        r5,#0
  00403C54: 68E4      ldr         r4,[r4,#0xC]
  00403C56: 2300      movs        r3,#0
  00403C58: 2200      movs        r2,#0
  00403C5A: A90E      add         r1,sp,#0x38
  00403C5C: 9500      str         r5,[sp]
  00403C5E: 47A0      blx         r4
  00403C60: B338      cbz         r0,00403CB2
  00403C62: 9B09      ldr         r3,[sp,#0x24]
  00403C64: 1AFC      subs        r4,r7,r3
  00403C66: F8D8 3000 ldr         r3,[r8]
  00403C6A: 429C      cmp         r4,r3
  00403C6C: D321      bcc         00403CB2
  00403C6E: 9806      ldr         r0,[sp,#0x18]
  00403C70: 2201      movs        r2,#1
  00403C72: 2801      cmp         r0,#1
  00403C74: D908      bls         00403C88
  00403C76: F108 0108 add         r1,r8,#8
  00403C7A: F851 3B08 ldr         r3,[r1],#8
  00403C7E: 429C      cmp         r4,r3
  00403C80: D302      bcc         00403C88
  00403C82: 1C52      adds        r2,r2,#1
  00403C84: 4282      cmp         r2,r0
  00403C86: D3F8      bcc         00403C7A
  00403C88: EB08 03C2 add         r3,r8,r2,lsl #3
  00403C8C: F853 3C04 ldr         r3,[r3,#-4]
  00403C90: 9A0C      ldr         r2,[sp,#0x30]
  00403C92: 9C07      ldr         r4,[sp,#0x1C]
  00403C94: F023 437F bic         r3,r3,#0xFF000000
  00403C98: 6013      str         r3,[r2]
  00403C9A: 4620      mov         r0,r4
  00403C9E: 9A0D      ldr         r2,[sp,#0x34]
  00403CA0: 990E      ldr         r1,[sp,#0x38]
  00403CA2: 6F24      ldr         r4,[r4,#0x70]
  00403CA4: AB22      add         r3,sp,#0x88
  00403CA6: 9502      str         r5,[sp,#8]
  00403CA8: 9501      str         r5,[sp,#4]
  00403CAA: 9500      str         r5,[sp]
  00403CAC: 47A0      blx         r4
  00403CAE: B100      cbz         r0,00403CB2
  00403CB0: 2601      movs        r6,#1
  00403CB2: F24C 0314 mov         r3,#0xC014
  00403CB6: F2C0 0340 movt        r3,#0x40
  00403CBC: 4798      blx         r3
  00403CBE: F24C 0318 mov         r3,#0xC018
  00403CC2: F2C0 0340 movt        r3,#0x40
  00403CC6: 4642      mov         r2,r8
  00403CC8: 2100      movs        r1,#0
  00403CCC: 4798      blx         r3
  00403CCE: 9B05      ldr         r3,[sp,#0x14]
  00403CD0: 4618      mov         r0,r3
  00403CD6: 4798      blx         r3
  00403CD8: 9B07      ldr         r3,[sp,#0x1C]
  00403CDA: 4618      mov         r0,r3
  00403CDE: 6C1B      ldr         r3,[r3,#0x40]
  00403CE0: 4798      blx         r3
  00403CE2: 9B0A      ldr         r3,[sp,#0x28]
  00403CE4: 4618      mov         r0,r3
  00403CE8: 6B9B      ldr         r3,[r3,#0x38]
  00403CEA: 4798      blx         r3
  00403CEC: 9B08      ldr         r3,[sp,#0x20]
  00403CEE: 4618      mov         r0,r3
  00403CF2: 6ADB      ldr         r3,[r3,#0x2C]
  00403CF4: 4798      blx         r3
  00403CF6: 4630      mov         r0,r6
  00403CF8: B017      add         sp,sp,#0x5C
  00403CFA: E8BD 0FF0 pop         {r4-r11}
  00403CFE: F85D FB14 ldr         pc,[sp],#0x14
  00403D02: DEFE      __debugbreak
  00403D04: 9141      str         r1,[sp,#0x104]
  00403D06: 0132      lsls        r2,r6,#4

  00403D20: F24C 0CA4 mov         r12,#0xC0A4
  00403D24: F2C0 0C40 movt        r12,#0x40
  00403D28: F8DC F000 ldr         pc,[r12]
  00403D2C: B40F      push        {r0-r3}
  00403D2E: E92D 4800 push        {r11,lr}
  00403D32: 46EB      mov         r11,sp
  00403D34: B082      sub         sp,sp,#8
  00403D36: 9B07      ldr         r3,[sp,#0x1C]
  00403D38: 331C      adds        r3,r3,#0x1C
  00403D3C: 9907      ldr         r1,[sp,#0x1C]
  00403D3E: 9805      ldr         r0,[sp,#0x14]
  00403D40: F000 F814 bl          00403D6C
  00403D44: 2301      movs        r3,#1
  00403D46: 9300      str         r3,[sp]
  00403D48: 9800      ldr         r0,[sp]
  00403D4A: B002      add         sp,sp,#8
  00403D4C: F85D BB04 pop         {r11}
  00403D50: F85D FB14 ldr         pc,[sp],#0x14

  00403D6C: B40F      push        {r0-r3}
  00403D6E: E92D 4800 push        {r11,lr}
  00403D72: 46EB      mov         r11,sp
  00403D74: B084      sub         sp,sp,#0x10
  00403D76: 9B08      ldr         r3,[sp,#0x20]
  00403D7A: F023 0303 bic         r3,r3,#3
  00403D7E: 9302      str         r3,[sp,#8]
  00403D80: 9B06      ldr         r3,[sp,#0x18]
  00403D82: 9301      str         r3,[sp,#4]
  00403D84: 9A01      ldr         r2,[sp,#4]
  00403D86: 9B02      ldr         r3,[sp,#8]
  00403D88: 4413      add         r3,r3,r2
  00403D8C: 9300      str         r3,[sp]
  00403D8E: 9B08      ldr         r3,[sp,#0x20]
  00403D92: F013 0F01 tst         r3,#1
  00403D96: D005      beq         00403DA4
  00403D98: 9A01      ldr         r2,[sp,#4]
  00403D9A: 9B02      ldr         r3,[sp,#8]
  00403D9C: 441A      add         r2,r2,r3
  00403D9E: 9B00      ldr         r3,[sp]
  00403DA0: 1AD3      subs        r3,r2,r3
  00403DA2: 9300      str         r3,[sp]
  00403DA4: 9800      ldr         r0,[sp]
  00403DA6: F7FD F96F bl          00401088
  00403DAA: B004      add         sp,sp,#0x10
  00403DAC: F85D BB04 pop         {r11}
  00403DB0: F85D FB14 ldr         pc,[sp],#0x14

  00403DCC: F24C 0CA8 mov         r12,#0xC0A8
  00403DD0: F2C0 0C40 movt        r12,#0x40
  00403DD4: F8DC F000 ldr         pc,[r12]
  00403DD8: F24C 0CAC mov         r12,#0xC0AC
  00403DDC: F2C0 0C40 movt        r12,#0x40
  00403DE0: F8DC F000 ldr         pc,[r12]
  00403DE4: F24C 0CB0 mov         r12,#0xC0B0
  00403DE8: F2C0 0C40 movt        r12,#0x40
  00403DEC: F8DC F000 ldr         pc,[r12]
  00403DF0: F24C 0CB4 mov         r12,#0xC0B4
  00403DF4: F2C0 0C40 movt        r12,#0x40
  00403DF8: F8DC F000 ldr         pc,[r12]
  00403DFC: B403      push        {r0,r1}
  00403DFE: E92D 4800 push        {r11,lr}
  00403E02: 46EB      mov         r11,sp
  00403E04: 2002      movs        r0,#2
  00403E06: DEFB      __fastfail
  00403E08: E92D 4800 push        {r11,lr}
  00403E0C: 46EB      mov         r11,sp
  00403E0E: 2008      movs        r0,#8
  00403E10: F000 F80E bl          00403E30
  00403E14: DEFE      __debugbreak

  00403E30: B403      push        {r0,r1}
  00403E32: E92D 4800 push        {r11,lr}
  00403E36: 46EB      mov         r11,sp
  00403E38: 9802      ldr         r0,[sp,#8]
  00403E3A: DEFB      __fastfail
  00403E3C: B40F      push        {r0-r3}
  00403E3E: E92D 4800 push        {r11,lr}
  00403E42: 46EB      mov         r11,sp
  00403E44: 9802      ldr         r0,[sp,#8]
  00403E46: DEFB      __fastfail
  00403E48: F24C 0C4C mov         r12,#0xC04C
  00403E4C: F2C0 0C40 movt        r12,#0x40
  00403E50: F8DC F000 ldr         pc,[r12]
  00403E54: F24C 0C48 mov         r12,#0xC048
  00403E58: F2C0 0C40 movt        r12,#0x40
  00403E5C: F8DC F000 ldr         pc,[r12]
  00403E60: F24C 0C44 mov         r12,#0xC044
  00403E64: F2C0 0C40 movt        r12,#0x40
  00403E68: F8DC F000 ldr         pc,[r12]
  00403E6C: F24C 0C40 mov         r12,#0xC040
  00403E70: F2C0 0C40 movt        r12,#0x40
  00403E74: F8DC F000 ldr         pc,[r12]
  00403E78: F24C 0C3C mov         r12,#0xC03C
  00403E7C: F2C0 0C40 movt        r12,#0x40
  00403E80: F8DC F000 ldr         pc,[r12]
  00403E84: F24C 0C38 mov         r12,#0xC038
  00403E88: F2C0 0C40 movt        r12,#0x40
  00403E8C: F8DC F000 ldr         pc,[r12]
  00403E90: F24C 0C34 mov         r12,#0xC034
  00403E94: F2C0 0C40 movt        r12,#0x40
  00403E98: F8DC F000 ldr         pc,[r12]
  00403E9C: F24C 0C30 mov         r12,#0xC030
  00403EA0: F2C0 0C40 movt        r12,#0x40
  00403EA4: F8DC F000 ldr         pc,[r12]
  00403EA8: F24C 0C2C mov         r12,#0xC02C
  00403EAC: F2C0 0C40 movt        r12,#0x40
  00403EB0: F8DC F000 ldr         pc,[r12]
  00403EB4: F24C 0C28 mov         r12,#0xC028
  00403EB8: F2C0 0C40 movt        r12,#0x40
  00403EBC: F8DC F000 ldr         pc,[r12]
  00403EC0: F24C 0C24 mov         r12,#0xC024
  00403EC4: F2C0 0C40 movt        r12,#0x40
  00403EC8: F8DC F000 ldr         pc,[r12]
  00403ECC: F24C 0C20 mov         r12,#0xC020
  00403ED0: F2C0 0C40 movt        r12,#0x40
  00403ED4: F8DC F000 ldr         pc,[r12]
  00403ED8: F24C 0C1C mov         r12,#0xC01C
  00403EDC: F2C0 0C40 movt        r12,#0x40
  00403EE0: F8DC F000 ldr         pc,[r12]
  00403EE4: F24C 0C18 mov         r12,#0xC018
  00403EE8: F2C0 0C40 movt        r12,#0x40
  00403EEC: F8DC F000 ldr         pc,[r12]
  00403EF0: F24C 0C14 mov         r12,#0xC014
  00403EF4: F2C0 0C40 movt        r12,#0x40
  00403EF8: F8DC F000 ldr         pc,[r12]
  00403EFC: F24C 0C10 mov         r12,#0xC010
  00403F00: F2C0 0C40 movt        r12,#0x40
  00403F04: F8DC F000 ldr         pc,[r12]
  00403F08: F24C 0C0C mov         r12,#0xC00C
  00403F0C: F2C0 0C40 movt        r12,#0x40
  00403F10: F8DC F000 ldr         pc,[r12]
  00403F14: F24C 0C08 mov         r12,#0xC008
  00403F18: F2C0 0C40 movt        r12,#0x40
  00403F1C: F8DC F000 ldr         pc,[r12]
  00403F20: F24C 0C04 mov         r12,#0xC004
  00403F24: F2C0 0C40 movt        r12,#0x40
  00403F28: F8DC F000 ldr         pc,[r12]
  00403F2C: F24C 0C00 movw        r12,#0xC000
  00403F30: F2C0 0C40 movt        r12,#0x40
  00403F34: F8DC F000 ldr         pc,[r12]
  00403F38: F24C 0C50 mov         r12,#0xC050
  00403F3C: F2C0 0C40 movt        r12,#0x40
  00403F40: F8DC F000 ldr         pc,[r12]
  
  004056BC: B510      push        {r4,lr}
  004056BE: B082      sub         sp,sp,#8
  004056C0: 6038      str         r0,[r7]
  004056C4: 637B      str         r3,[r7,#0x34]
  004056CC: 63FB      str         r3,[r7,#0x3C]
  004056CE: 6B79      ldr         r1,[r7,#0x34]
  004056D0: 6BF8      ldr         r0,[r7,#0x3C]
  004056D2: F7FD F90D bl          004028F0
  004056D6: 6478      str         r0,[r7,#0x44]
  004056D8: 6C78      ldr         r0,[r7,#0x44]
  004056DA: B002      add         sp,sp,#8
  004056DC: BD10      pop         {r4,pc}

  004056F8: 62F8      str         r0,[r7,#0x2C]
  004056FA: 6AFB      ldr         r3,[r7,#0x2C]
  00405700: 633B      str         r3,[r7,#0x30]
  00405702: 6B3A      ldr         r2,[r7,#0x30]
  00405704: 4B04      ldr         r3,00405718
  00405706: 429A      cmp         r2,r3
  00405708: D102      bne         00405710
  0040570A: 2301      movs        r3,#1
  0040570C: 613B      str         r3,[r7,#0x10]
  0040570E: E001      b           00405714
  00405710: 2300      movs        r3,#0
  00405712: 613B      str         r3,[r7,#0x10]
  00405714: 6938      ldr         r0,[r7,#0x10]
  00405716: 4770      bx          lr
  00405718: 0005      movs        r5,r0
  0040571A: C000      ?stm        r0!,{}
  0040571C: B510      push        {r4,lr}
  0040571E: 2008      movs        r0,#8
  00405720: F7FD FF8C bl          0040363C
  00405724: E7FF      b           00405726
  00405726: BD10      pop         {r4,pc}

  00405744: 4B03      ldr         r3,00405754
  00405746: 429A      cmp         r2,r3
  00405748: D101      bne         0040574E
  0040574A: 2001      movs        r0,#1
  0040574C: E000      b           00405750
  0040574E: 2000      movs        r0,#0
  00405750: 4770      bx          lr
  00405752: DEFE      __debugbreak
  00405754: 1388      asrs        r0,r1,#0xE
  00405756: 406D      eors        r5,r5,r5
  0040575C: 4B03      ldr         r3,0040576C
  0040575E: 429A      cmp         r2,r3
  00405760: D101      bne         00405766
  00405762: 2001      movs        r0,#1
  00405764: E000      b           00405768
  00405766: 2000      movs        r0,#0
  00405768: 4770      bx          lr
  0040576A: DEFE      __debugbreak
  0040576C: 1388      asrs        r0,r1,#0xE
  0040576E: 406D      eors        r5,r5,r5
  00405770: 0000      movs        r0,r0
  ";
        #endregion

        [SetUp]
        public void Setup()
        {
            baseAddress = Address.Ptr32(0x00100000);
            arch = new ThumbArchitecture("arm-thumb");
        }

        private void Given_Address(uint uAddr)
        {
            this.baseAddress = Address.Ptr32(uAddr);
        }

        //[Test]
        public void ThumbRw_regression()
        {
            var code = ThumbBlock
                .Replace("\r\n", "\n")
                .Split('\n')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .SelectMany(s =>
                {
                    var ss = s.Split(':')[1]
                    .Trim()
                    .Remove(9)
                    .Replace(" ", "");
                    return ss.Length == 8
                        ? new[] { ss.Substring(0, 4), ss.Substring(4, 4) }
                        : new[] { ss.Substring(0, 4) };
                })
                .Select(s => Convert.ToUInt16(s, 16))
                .SelectMany(s => new byte[] {
					(byte) s,
					(byte) (s >> 8)
				})
                .ToArray();
            var image = new MemoryArea(Address.Ptr32(0x00401000), code);
            var rw = arch.CreateRewriter(image.CreateLeReader(0), new AArch32ProcessorState(arch), arch.CreateFrame(), new FakeRewriterHost());
            {
                foreach (var rtc in rw)
                {
                    Console.WriteLine(rtc.Address);
                    foreach (var rtl in rtc.Instructions)
                    {
                        Console.WriteLine("    {0}", rtl);
                    }
                }
            }
        }

        [Test]
        public void ThumbRw_push()
        {
            Given_UInt16s(0xE92D, 0x4800); // "push.w\t{fp,lr}"
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|sp = sp - 8",
                "2|L--|Mem0[sp:word32] = fp",
                "3|L--|Mem0[sp + 4:word32] = lr");
        }

        [Test]
        public void ThumbRw_pop()
        {
            Given_UInt16s(0xE8BD, 0x8800); // pop.w\t{fp,pc}
            AssertCode(
                "0|T--|00100000(4): 3 instructions",
                "1|L--|fp = Mem0[sp:word32]",
                "2|L--|sp = sp + 8",
                "3|T--|return (0,0)");
        }

        [Test]
        public void ThumbRw_mov()
        {
            Given_UInt16s(0x46EB); // mov\tfp,sp
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|fp = sp");
        }

        [Test]
        public void ThumbRw_sub_sp()
        {
            Given_UInt16s(0xB082); // sub\tsp,#8
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|sp = sp - 8");
        }

        [Test]
        public void ThumbRw_bl()
        {
            Given_UInt16s(0xF000, 0xFA06); // bl\t$00100410
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 00100410 (0)");
        }

        [Test]
        public void ThumbRw_str()
        {
            Given_UInt16s(0x9000); // str\tr0,[sp]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[sp:word32] = r0");
        }

        [Test]
        public void ThumbRw_ldr()
        {
            Given_UInt16s(0x9B00); // ldr\tr3,[sp]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = Mem0[sp:word32]");
        }

        [Test]
        public void ThumbRw_ldr_displacement()
        {
            Given_UInt16s(0x9801); // ldr\tr0,[sp,#4]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = Mem0[sp + 4:word32]");
        }

        [Test]
        public void ThumbRw_add()
        {
            Given_UInt16s(0xB002); // add\tsp,#8
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|sp = sp + 8");
        }

        [Test]
        public void ThumbRw_addw()
        {
            Given_UInt16s(0xF60D, 0x2348);  // add         r3,sp,#0xA48
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = sp + 0x00000A48");
        }

        [Test]
        public void ThumbRw_movw()
        {
            Given_UInt16s(0xF24C, 0x1C00); // movw        r12,#0xC100
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ip = 0x0000C100");
        }

        [Test]
        public void ThumbRw_movt()
        {
            Given_UInt16s(0xF2C0, 0x0C40);  // movt        r12,#0x40
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ip = DPB(ip, 0x0040, 16)");
        }

        [Test]
        public void ThumbRw_bx()
        {
            Given_UInt16s(0x4760);  // bx          r12
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto ip");
        }

        [Test]
        public void ThumbRw_cmp()
        {
            Given_UInt16s(0x4563);  // cmp         r3,r12
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|NZCV = cond(r3 - ip)");
        }

        [Test]
        public void ThumbRw_clz()
        {
            Given_HexString("B2FA82F2");	// clz r2, r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = __clz(r2)");
        }

        [Test]
        public void ThumbRw_cmn()
        {
            Given_HexString("EA42");	// cmn r2, r5
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|NZCV = cond(r2 + r5)");
        }

        [Test]
        public void ThumbRw_bne()
        {
            Given_UInt16s(0xD101);  // bne         00401056
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100006");
        }

        [Test]
        public void ThumbRw_bic()
        {
            Given_UInt16s(0xF02C, 0x0C07);  // bic         r12,r12,#7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ip = ip & ~0x00000007");
        }
        
        [Test]
        public void ThumbRw_adr()
        {
            Given_UInt16s(0xA020);  // adr         r0,0040111C
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = 00100080");
        }

        [Test]
        public void ThumbRw_lsls()
        {
            Given_UInt16s(0x0040);  // lsls        r0,#1
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r0 = r0 << 1",
                "2|L--|NZC = cond(r0)");
        }

        [Test]
        public void ThumbRw_lsls_long()
        {
            Given_HexString("12FA00F0");    // lsl r0,r2,r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = r2 << r0",
                "2|L--|NZC = cond(r0)");
        }



        [Test]
        public void ThumbRw_trap()
        {
            Given_UInt16s(0xDEFE);  // __debugbreak
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__syscall(0x000000FE)");
        }

        [Test]
        public void ThumbRw_blx()
        {
            Given_UInt16s(0x4798);  // blx         r3
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call r3 (0)");
        }

        [Test]
        public void ThumbRw_stm()
        {
            Given_UInt16s(0xC108);  // stm         r1!,{r3}
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|Mem0[r1:word32] = r3",
                "2|L--|r1 = r1 + 4");
        }

        [Test]
        public void ThumbRw_ldrb()
        {
            Given_UInt16s(0x7858);  // ldrb        r0,[r3,#1]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = (word32) Mem0[r3 + 1:byte]");
        }

        [Test]
        public void ThumbRw_cbnz()
        {
            Given_UInt16s(0xB95B);  // cbnz        r3,0040219C)
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (r3 != 0x00000000) branch 0010001A");
        }

        [Test]
        public void ThumbRw_strb()
        {
            Given_UInt16s(0x7013);  // strb        r3,[r2]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r2:byte] = (byte) r3");
        }

        [Test]
        public void ThumbRw_mvn()
        {
            Given_UInt16s(0xF06F, 0x0000);  // mvn         r0,#0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = ~0x00000000");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_dmbc()
        {
            Given_UInt16s(0xF3BF, 0x8F5B);  // dmb         ish
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|mrc(p15, 0x00, r3, c13, c0, 0x02);");
        }

        [Test]
        public void ThumbRw_ands()
        {
            Given_UInt16s(0x4000);  // ands        r0,r0,r0
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r0 = r0 & r0",
                "2|L--|NZC = cond(r0)");
        }

        [Test]
        public void ThumbRw_tstw()
        {
            Given_UInt16s(0xF013, 0x4F00);  // tst         r3,#0x80000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZC = cond(r3 & 0x80000000)");
        }
        
        [Test]
        public void ThumbRw_eors()
        {
            Given_UInt16s(0x4053);  // eors        r3,r3,r2
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r3 = r3 ^ r2",
                "2|L--|NZC = cond(r3)");
        }

        [Test]
        public void ThumbRw_cbz()
        {
            Given_UInt16s(0xB103);  // cbz         r3,00402E50
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (r3 == 0x00000000) branch 00100004");
        }

        [Test]
        public void ThumbRw_asrs()
        {
            Given_UInt16s(0x1388);  // asrs        r0,r1,#0xE
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r0 = r1 >> 14",
                "2|L--|NZC = cond(r0)");
        }

        [Test]
        public void ThumbRw_rsb()
        {
            Given_UInt16s(0xF1C4, 0x01F4);   // rsb         r1,r4,#0xF4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = 0x000000F4 - r4");
        }


        [Test]
        public void ThumbRw_subw()
        {
            Given_UInt16s(0xF6AD, 0x6D48);  // sub         sp,sp,#0xE48
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|sp = sp - 0x00000E48");
        }

        [Test]
        public void ThumbRw_svc()
        {
            Given_HexString("4EDF");	// svc #0x4e
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|L--|__syscall(0x0000004E)");
        }

        [Test]
        public void ThumbRw_strh()
        {
            Given_UInt16s(0x800B);  // strh        r3,[r1]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r1:word16] = (uint16) r3");
        }

        [Test]
        public void ThumbRw_uxtb()
        {
            Given_HexString("E8B2");	// uxtb r0, r5
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = (uint32) (byte) r5");
        }

        [Test]
        public void ThumbRw_uxth()
        {
            Given_UInt16s(0xB2A9);  // uxth        r1,r5
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = (uint32) (uint16) r5");
        }

        [Test]
        public void ThumbRw__fastfail()
        {
            Given_UInt16s(0xDEFB);  // __fastfail
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__syscall(0x000000FB)");
        }

        public void ThumbRw_ldrb_preIndex()
        {
            Given_UInt16s(0xF812, 0x3F01);  // ldrb        r3,[r2,#1]!
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = r2 + 1",
                "2|L--|r3 = (uint32) Mem0[r2:byte]");
        }

        [Test]
        public void ThumbRw_ldr_postIndex()
        {
            Given_UInt16s(0xF85D, 0xFB0C);  // ldr         pc,[sp],#0xC
            AssertCode(
                "0|T--|00100000(4): 3 instructions",
                "1|L--|v4 = Mem0[sp:word32]",
                "2|L--|sp = sp + 12",
                "3|T--|goto v4");
        }

        [Test]
        public void ThumbRw_ldrd()
        {
            Given_HexString("DDE90C23");	// ldrd r2, r3, [sp, #0x30]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3_r2 = Mem0[sp + 48:word64]");
        }

        [Test]
        public void ThumbRw_ldrsb()
        {
            Given_UInt16s(0xF991, 0x3000);  // ldrsb       r3,[r1]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = (word32) Mem0[r1:int8]");
        }


        [Test]
        public void ThumbRw_ldrsh()
        {
            Given_HexString("B6F94946");	// ldrsh.w r4, [r6, #0x649]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = (word32) Mem0[r6 + 1609:int16]");
        }

        [Test]
        public void ThumbRw_strb_preIndex()
        {
            Given_UInt16s(0xF801, 0x0F01);  // strb.w r0, [r1, #1]!
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1 + 1",
                "2|L--|Mem0[r1:byte] = (byte) r0");
        }

        [Test]
        public void ThumbRw_strb_postIndex()
        {
            Given_UInt16s(0xF802, 0x3B01);  // strb        r3,[r2],#1
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r2:byte] = (byte) r3",
                "2|L--|r2 = r2 + 1");
        }

        [Test]
        public void ThumbRw_ldr_pc()
        {
            Given_UInt16s(0xF8DC, 0xF000);  // ldr         pc,[r12]
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto Mem0[ip:word32]");
        }

        [Test]
        public void ThumbRw_it_mi()
        {
            Given_UInt16s(
                0xBF48,    // it    mi
                0x4632);   // mov   r2,r6
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop",        // placeholder.
                "2|L--|00100002(2): 2 instructions",
                "3|T--|if (Test(GE,N)) branch 00100004",
                "4|L--|r2 = r6");
        }

        [Test]
        public void ThumbRw_itt_mi()
        {
            Given_UInt16s(
                0xBF44,     // itt  mi
                0x4632,     // mov  r2,r6
                0x4633);    // mov  r3,r6
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop",
                "2|L--|00100002(2): 2 instructions",
                "3|T--|if (Test(GE,N)) branch 00100004",
                "4|L--|r2 = r6",
                "5|L--|00100004(2): 2 instructions",
                "6|T--|if (Test(GE,N)) branch 00100006",
                "7|L--|r3 = r6");
        }

        [Test]
        public void ThumbRw_ite_eq()
        {
            Given_UInt16s(
                0xBF0C,     // ite  eq
                0x4632,     // mov  r2,r6
                0x4633);    // mov  r3,r6
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop",
                "2|L--|00100002(2): 2 instructions",
                "3|T--|if (Test(NE,Z)) branch 00100004",
                "4|L--|r2 = r6",
                "5|L--|00100004(2): 2 instructions",
                "6|T--|if (Test(EQ,Z)) branch 00100006",
                "7|L--|r3 = r6");
        }

        [Test]
        public void ThumbRw_mcr()
        {
            Given_HexString("01EE100F");	// mcr p15, #0, r0, c1, c0, #0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mcr(p15, 0x00000000, r0, cr1, cr0, 0x00000000)");
        }

        [Test]
        public void ThumbRw_mrc()
        {
            Given_UInt16s(0xEE1D, 0x3F50);  // mrc         p15,#0,r3,c13,c0,#2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = __mrc(p15, 0x00000000, cr13, cr0, 0x00000002)");
        }

        [Test]
        public void ThumbRw_vnmls()
        {
            Given_HexString("11EE0B0A");	// vnmls.f32 s0, s2, s22
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = __vmls_f32(s2, s22)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vrhadd()
        {
            Given_HexString("10FF2FE1");	// vrhadd.u16 d14, d0, d31
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_ror()
        {
            Given_HexString("E041");	// rors r0, r4
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r0 = __ror(r0, r4)",
                "2|L--|NZC = cond(r0)");
        }

        [Test]
        public void ThumbRw_rev()
        {
            Given_HexString("FFBA");	// revsh r7, r7
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = __rev(r7)");
        }

        [Test]
        public void ThumbRw_rev_2()
        {
            Given_HexString("19BA");	// rev r1, r3
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = __rev(r3)");
        }

        [Test]
        public void ThumbRw_umull()
        {
            Given_HexString("A2FB0030");	// umull r3, r0, r2, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0_r3 = r0 *u r2");
        }

        [Test]
        public void ThumbRw_mul()
        {
            Given_HexString("03FB00F3");	// mul r3, r3, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 * r0");
        }

        [Test]
        public void ThumbRw_nop()
        {
            Given_HexString("00BF");	// nop 
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        [Ignore("Appears to be an incorrect decoding by Capstone")]
        public void ThumbRw_ldc()
        {
            Given_HexString("3AED2046");	// ldc p6, c4, [sl, #-0x80]!
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r10 = r10 + -128",
                "2|L--|v3 = Mem0[r10:word32]",
                "3|L--|p6 = __ldc(0x04, v3)");
        }

        [Test]
        [Ignore("Does ARM T32 contain this?")]
        public void ThumbRw_cdp()
        {
	        Given_HexString("11EE4000");    // cdp p0, #1, c0, c1, c0, #2
	        AssertCode(
		        "0|L--|00100000(4): 1 instructions",
		        "1|L--|__cdp(p0, 0x00000001, 0x00, 0x01, 0x00, 0x00000002)");
        }

        [Test]
        [Ignore("Does ARM T32 contain this?")]
        public void ThumbRw_cdp2()
        {
            Given_HexString("7BFE0DF0");    // cdp2 p0, #7, c15, c11, c13, #0
	        AssertCode(
		        "0|L--|00100000(4): 1 instructions",
		        "1|L--|__cdp2(p0, 0x00000007, 0x0F, 0x0B, 0x0D, 0x00000000)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_stc2l()
        {
            Given_HexString("44FD0128");	// stc2l p8, c2, [r4, #-4]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_ldc2()
        {
            Given_HexString("31FC0128");	// ldc2 p8, c2, [r1], #-4
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r1 = r1",
                "2|L--|v3 = Mem0[r1:word32]",
                "3|L--|p8 = __ldc2(0x02, v3)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_ldcl()
        {
            Given_HexString("D8EC3846");	// ldcl p6, c4, [r8], {0x38}
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = Mem0[r8:word32]",
                "2|L--|p6 = __ldcl(0x04, v3)");
        }

        [Test]
        public void ThumbRw_adc()
        {
            Given_HexString("49F1FF37");	// adc r7, sb, #-1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = r9 + 0xFFFFFFFF + C");
        }

        [Test]
        public void ThumbRw_strd()
        {
            Given_HexString("CDE90067");	// strd r6, r7, [sp]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp:word64] = r7_r6");
        }

        [Test]
        public void ThumbRw_sbc()
        {
            Given_HexString("7CEB3646");	// sbcs.w r6, ip, r16, ror #16
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r6 = ip - __ror(r6, 0x00000010) - C",
                "2|L--|NZCV = cond(r6)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vtbl()
        {
            Given_HexString("BBFF0CAB");	// vtbl.8 d10, {d11, d12, d13, d14}, d12
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vsubw()
        {
            Given_HexString("88FF0023");	// vsubw.u8 q1, q4, d0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q1 = __vsubw_u8(q4, d0)");
        }

        [Test]
        public void ThumbRw_smmla()
        {
            Given_HexString("57FB0021");	// smmla r1, r7, r0, r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (int32) (r7 *s r0 >> 32) + r2");
        }

        [Test]
        public void ThumbRw_smmls()
        {
            Given_HexString("62FB0646");	// smmls r6, r2, r6, r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = (int32) (r2 *s r6 >> 32) - r4");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vld1()
        {
            Given_HexString("A8F93148");	// vld1.32 {d4[0]}, [r8:0x20], r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vst3_16()
        {
            Given_HexString("C3F90446");	// vst3.16 {d20[0], d21[0], d22[0]}, [r3], r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vst3_8()
        {
            Given_HexString("04F910B5");	// mls r5, r4, r0, r9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = f9 - r4 * r0");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_mcr2()
        {
            Given_HexString("62FE3B46");	// mcr2 p6, #3, r4, c2, c11, #1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_pld()
        {
            Given_HexString("91F8D4F8");	// pld [r1, #0x8d4]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_ldc2l()
        {
            Given_HexString("F4FD04F0");	// ldc2l p0, c15, [r4, #0x10]!
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_stc2()
        {
            Given_HexString("AFFD1199");	// stc2 p9, c9, [pc, #0x44]!
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_sxtb()
        {
            Given_HexString("59B2");	// sxtb r1, r3
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = (int32) (int8) r3");
        }


        [Test]
        public void ThumbRw_sxth()
        {
            Given_HexString("08B2");	// sxth r0, r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = (int32) (int16) r1");
        }

        [Test]
        public void ThumbRw_ssat()
        {
            Given_HexString("03F31343");	// ssat r3, #0x14, r3, lsl #0x10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = __ssat(0x00000014, r3 << 16)",
                "2|L--|Q = cond(r3)");
        }

        [Test]
        public void ThumbRw_mla()
        {
            Given_HexString("00FB0210");	// mla r0, r0, r2, r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r1 + r0 * r2");
        }

        [Test]
        public void ThumbRw_bkpt()
        {
            Given_HexString("22BE");	// bkpt #0x22
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__breakpoint()");
        }

        [Test]
        public void ThumbRw_vshr_imm()
        {
            Given_HexString("F3FF14F0");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d31 = __vshr_i32(d4, 19)");
        }

        [Test]
        public void ThumbRw_vqadd()
        {
            Given_HexString("0CFF14F0");	// vqadd.u8 d15, d12, d4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d15 = __vqadd_u8(d12, d4)");
        }

        [Test]
        public void ThumbRw_vqadd_2()
        {
            Given_HexString("7FFF3700");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = __vqadd_u64(d15, d23)");
        }

        [Test]
        public void ThumbRw_smladx()
        {
            Given_HexString("20FB1EB0");	// smladx r0, r0, pc, fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 + ((int16) r0 *s (lr >> 16) + (r0 >> 16) *s (int16) lr)");
        }


        [Test]
        public void ThumbRw_vstr()
        {
            Given_HexString("44ED204A");	// vstr s9, [r4, #-0x80]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r4 - 128:word32] = s9");
        }

        [Test]
        public void ThumbRw_vsub()
        {
            Given_HexString("52FF2068");	// vsub.i16 d22, d2, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d22 = __vsub_i16(d2, d16)");
        }

        [Test]
        public void ThumbRw_vldr()
        {
            Given_HexString("5AED114B");	// vldr d20, [sl, #-0x44]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d20 = Mem0[r10 - 68:word64]");
        }

        [Test]
        public void ThumbRw_uadd8()
        {
            Given_HexString("84FA42F0");	// uadd8 r0, r4, r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __uadd_i8(r4, r2)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vld3()
        {
            Given_HexString("66F98245");	// vld3.32 {d20, d22, d24}, [r6], r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_dsb()
        {
            Given_HexString("BFF34F8F");	// dsb sy
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dsb_sy()");
        }

        [Test]
        public void ThumbRw_sbfx()
        {
            Given_HexString("48F70400");	// sbfx r0, r8, #0, #5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = (int32) SLICE(r8, ui5, 0)");
        }

        [Test]
        public void ThumbRw_ubfx()
        {
            Given_HexString("C2F30745");	// ubfx r5, r2, #0x10, #8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = (uint32) SLICE(r2, ui8, 16)");
        }

        [Test]
        public void ThumbRw_uxtab()
        {
            Given_HexString("5BFA94F8");	// uxtab r8, fp, r4, ror #8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = fp + (byte) (r4 >>u 8)");
        }

        [Test]
        public void ThumbRw_uxtah()
        {
            Given_HexString("11FAFEF7");	// uxtah r7, r1, lr, ror #24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = r1 + (uint16) (lr >>u 24)");
        }

        [Test]
        public void ThumbRw_smulbb()
        {
            Given_HexString("15FB02F6");	// smulbb r6, r5, r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = (int16) r5 *s (int16) r2");
        }

        [Test]
        public void ThumbRw_tbb()
        {
            Given_HexString("DFE802F0");	// tbb [pc, r2]
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 0x00100004 + Mem0[0x00100004 + r2:byte] * 0x02");
        }


        [Test]
        public void ThumbRw_tbh()
        {
            Given_HexString("DFE813F0");	// tbh [pc, r3, lsl #1]
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 0x00100004 + Mem0[0x00100004 + r3 * 0x00000002:word16] * 0x0002");
        }

        [Test]
        public void ThumbRw_usat()
        {
            Given_HexString("83F35B09");	// usat sb, #0x1b, r3, lsl #1
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r9 = __usat(0x0000001C, r3 << 1)",
                "2|L--|Q = cond(r9)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vld4()
        {
            Given_HexString("ACF9BAE7");	// vld4.16 {d14[2], d16[2], d18[2], d20[2]}, [ip:0x40], sl
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vrsubhn()
        {
            Given_HexString("94FF2046");	// vrsubhn.i32 d4, q2, q8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vselge()
        {
            Given_HexString("6FFEA26A");	// vselge.f32 s13, s31, s5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s13 = __vselge_f32(s31, s5)");
        }

        [Test]
        public void ThumbRw_vcge()
        {
            Given_HexString("44FF051E");	// vcge.f32 d17, d4, d5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d17 = __vcge_f32(d4, d5)");
        }

        [Test]
        public void ThumbRw_vselgt()
        {
            Given_HexString("38FEA26A");	// vselgt.f32 s12, s17, s5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s12 = __vselgt_f32(s17, s5)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vst4()
        {
            Given_HexString("44F94FF0");	// vst4.16 {d31, d0, d1, d2}, [r4]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_stcl()
        {
            Given_HexString("E8ED2368");	// stcl p8, c6, [r8, #0x8c]!
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_stc()
        {
            Given_HexString("88ED3D5E");	// stc p14, c5, [r8, #0xf4]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__stc(p14, cr5, Mem0[r8 + 244:word32])");
        }

        [Test]
        public void ThumbRw_smlawb()
        {
            Given_HexString("32FB0020");	// smlawb r0, r2, r0, r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = (r2 *s (int16) r0 >> 16) + r2");
        }


        [Test]
        public void ThumbRw_smlabb()
        {
            Given_HexString("13FB0746");	// smlabb r6, r3, r7, r4
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r6 = (int16) r3 *s (int16) r7 + r4");
        }

        [Test]
        public void ThumbRw_smlatt()
        {
            Given_HexString("12FB3A46");	// smlatt r6, r2, sl, r4
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r6 = (int16) (r2 >> 16) *s (int16) (r10 >> 16) + r4",
                "2|L--|Q = cond(r6)");
        }

        [Test]
        public void ThumbRw_vmov_imm()
        {
            Given_HexString("83FF13F0");	// vmov.i32 d15, #0xb3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d15 = 0x000000B3000000B3");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vst1()
        {
            Given_HexString("84F90020");	// vst1.8 {d2[0]}, [r4], r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_mcrr2()
        {
            Given_HexString("4FFC3AF0");	// mcrr2 p0, #3, pc, pc, c10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_umaal()
        {
            Given_HexString("E0FB6320");	// umaal r2, r0, r0, r3
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = r0 *u r3",
                "2|L--|v2 = v2 + (uint64) r0",
                "3|L--|r0_r2 = v2 + (uint64) r2");
        }

        [Test]
        public void ThumbRw_umlal()
        {
            Given_HexString("E3FB03E7");	// umlal lr, r7, r3, r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7_lr = r3 *u r3 + r7_lr");
        }

        [Test]
        public void ThumbRw_vmlal()
        {
            Given_HexString("C8FF0128");	// vmlal.u8 q9, d8, d1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q9 = __vmlal_u8(d8, d1)");
        }

        [Test]
        public void ThumbRw_vmlsl()
        {
            Given_HexString("EAFFE346");	// vmlsl.u32 q10, d26, d3[1]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q10 = __vmlsl_u32(d26, d3[1])");
        }

        [Test]
        public void ThumbRw_vmvn_imm()
        {
            Given_HexString("82FF3146");	// vmvn.i32 d4, #0xa1000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d4 = __vmvn_imm_i32(0xA1000000A1000000)");
        }

        [Test]
        public void ThumbRw_vceq_i32()
        {
            Given_HexString("21FF1048");	// vceq.i32 d4, d1, d0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d4 = __vceq_i32(d1, d0)");
        }

        [Test]
        public void ThumbRw_vqshl()
        {
            Given_HexString("81EFFEE7");	// vqshl.s64 q7, q15, #1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q7 = __vqshl_i64(q15, 1)");
        }

        [Test]
        public void ThumbRw_smulbt()
        {
            Given_HexString("17FB16F0");	// smulbt r0, r7, r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = (int16) r7 *s (int16) (r6 >> 16)");
        }

        [Test]
        public void ThumbRw_smull()
        {
            Given_HexString("8AFB0028");	// smull r2, r8, r10, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8_r2 = r0 *s r10");
        }

        [Test]
        public void ThumbRw_smlsd()
        {
            Given_HexString("41FB0446");	// smlsd r6, r1, r4, r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = r6 + ((int16) r1 *s (int16) r4 - (r1 >> 16) *s (r4 >> 16))");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_mrc2()
        {
            Given_HexString("3CFE714D");	// mrc2 p13, #1, r4, c12, c1, #3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_mrrc2()
        {
            Given_HexString("54FC1348");	// mrrc2 p8, #1, r4, r4, c3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vmin()
        {
            Given_HexString("62FF3846");	// vmin.u32 d20, d2, d24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d20 = __vmin_u32(d2, d24)");
        }

        [Test]
        public void ThumbRw_ldm()
        {
            Given_HexString("1ECC");	// ldm r4, {r1, r2, r3, r4}
            AssertCode(
                "0|L--|00100000(2): 4 instructions",
                "1|L--|r1 = Mem0[r4:word32]",
                "2|L--|r2 = Mem0[r4 + 4:word32]",
                "3|L--|r3 = Mem0[r4 + 8:word32]",
                "4|L--|r4 = Mem0[r4 + 12:word32]");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_ldmdb()
        {
            Given_HexString("36E97D4B");	// ldmdb r6!, {r0, r2, r3, r4, r5, r6, r8, sb, fp, lr}
            AssertCode(
                "0|L--|00100000(4): 11 instructions",
                "1|L--|r0 = Mem0[r6 - 4:word32]",
                "2|L--|r2 = Mem0[r6 - 8:word32]",
                "3|L--|r3 = Mem0[r6 - 12:word32]",
                "4|L--|r4 = Mem0[r6 - 16:word32]",
                "5|L--|r5 = Mem0[r6 - 20:word32]",
                "6|L--|r6 = Mem0[r6 - 24:word32]",
                "7|L--|r8 = Mem0[r6 - 28:word32]",
                "8|L--|r8 = Mem0[r6 - 32:word32]",
                "9|L--|r9 = Mem0[r6 - 36:word32]",
                "10|L--|r10 = Mem0[r4 + 36:word32]",
                "11|L--|r11 = Mem0[r4 + 40:word32]");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vrev64()
        {
            Given_HexString("B0FF0300");	// vrev64.8 d0, d3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vst2()
        {
            Given_HexString("46F94748");	// vst2.16 {d20, d21}, [r6], r7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_smlad()
        {
            Given_HexString("2AFB049A");	// smlad sl, sl, r4, sb
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = r10 + ((int16) r10 *s (int16) r4 + (r10 >> 16) *s (r4 >> 16))");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_uhsub16()
        {
            Given_HexString("DCFA6FF0");	// uhsub16 r0, ip, pc
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_sxtab16()
        {
            Given_HexString("2BFADBF8");	// sxtab16 r8, fp, fp, ror #8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __sxtab16(fp, __ror(fp, 8))");
        }

        [Test]
        public void ThumbRw_shsub8()
        {
            Given_HexString("C7FA26F4");	// shsub8 r4, r7, r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __shsub_s8(r7, r6)");
        }

        [Test]
        public void ThumbRw_orrs()
        {
            Given_HexString("0443");	// orrs r4, r0
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r4 = r4 | r0",
                "2|L--|NZC = cond(r4)");
        }

        [Test]
        public void ThumbRw_orn()
        {
            Given_HexString("64F40300");	// orn r0, r4, #0x830000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r4 | ~0x00830000");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vswp()
        {
            Given_HexString("B2FF0300");	// vswp d0, d3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vqrshrun()
        {
            Given_HexString("E2FF7E48");	// vqrshrun.s64 d20, q15, #0x1e
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_ssat16()
        {
            Given_HexString("22F30300");	// ssat16 r0, #3, r2
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = __ssat16(0x00000003, r2)",
                "2|L--|Q = cond(r0)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_fstmiax()
        {
            Given_HexString("88EC179B");	// fstmiax r8, {d9, d10, d11, d12, d13, d14, d15, d16, d17, d18, d19}
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_smmlsr()
        {
            Given_HexString("6BFB1E4B");	// smmlsr fp, fp, lr, r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vcvt()
        {
            Given_HexString("FBFF2046");	// vcvt.f32.s32 d20, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d20 = (real32) d16");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_mcrr()
        {
            Given_HexString("40EC0600");	// mcrr p0, #0, r0, r0, c6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vsubl()
        {
            Given_HexString("E1FFA042");	// vsubl.u32 q10, d17, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q10 = __vsubl_u32(d17, d16)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vqshrn()
        {
            Given_HexString("B8FF1249");	// vqshrn.u64 d4, q1, #8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vhadd()
        {
            Given_HexString("41FF02B0");	// vhadd.u8 d27, d1, d2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d27 = __vhadd_u8(d1, d2)");
        }

        [Test]
        public void ThumbRw_vhsub()
        {
            Given_HexString("4AFF8642");	// vhsub.u8 d20, d26, d6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d20 = __vhsub_u8(d26, d6)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vext()
        {
            Given_HexString("F4EF0300");	// vext.32 d16, d4, d3, #0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vaba()
        {
            Given_HexString("1EFF9BE7");	// vaba.u16 d14, d30, d11
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d14 = __vaba_u16(d30, d11)");
        }

        [Test]
        public void ThumbRw_vorr_imm()
        {
            Given_HexString("C4FF104B");	// vorr.i16 d20, #0xc000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d20 = d20 | 0xC000C000C000C000");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_smulwb()
        {
            Given_HexString("32FB0FF0");	// smulwb r0, r2, pc
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_usada8()
        {
            Given_HexString("77FB0030");	// usada8 r0, r7, r0, r3
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = r7",
                "2|L--|v6 = r0",
                "3|L--|r0 = __usada8(v5, v6)");
        }

        [Test]
        public void ThumbRw_vselvs()
        {
            Given_HexString("1AFE224B");	// vselvs.f64 d4, d10, d18
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d4 = __vselvs_f64(d10, d18)");
        }

        [Test]
        public void ThumbRw_vseleq()
        {
            Given_HexString("0FFE204B");	// vseleq.f64 d4, d15, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d4 = __vseleq_f64(d15, d16)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vld2()
        {
            Given_HexString("AAF909F5");	// vld2.16 {d15[0], d16[0]}, [sl], sb
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vshl()
        {
            Given_HexString("FCEF3335");	// vshl.i32 d19, d19, #0x1c
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d19 = __vshl_i32(d19, 28)");
        }

        [Test]
        public void ThumbRw_vsra()
        {
            Given_HexString("D9FF10F1");	// vsra.u16 d31, d0, #9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d31 = __vsra_i16(d0, 9)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vqrdmulh()
        {
            Given_HexString("1DFF099B");	// vqrdmulh.s16 d9, d13, d9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vqshlu()
        {
            Given_HexString("BCFF3846");	// vqshlu.s32 d4, d24, #0x1c
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vbit()
        {
            Given_HexString("69FFB8F1");	// vbit d31, d25, d24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d31 = __vbit(d25, d24)");
        }

        [Test]
        public void ThumbRw_smlal()
        {
            Given_HexString("CCFB0BB0");	// smlal fp, r0, ip, fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fp_r0 = ip *s fp + fp_r0");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vtbx()
        {
            Given_HexString("BDFFC5F8");	// vtbx.8 d15, {d29}, d5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vabd()
        {
            Given_HexString("28FFC6E7");	// vabd.u32 q7, q12, q3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q7 = __vabd_u32(q12, q3)");
        }

        [Test]
        public void ThumbRw_vmull()
        {
            Given_HexString("E3FF2B8C");	// vmull.u32 q12, d3, d27
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q12 = __vmull_u32(d3, d27)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vmull_polynomial()
        {
            Given_HexString("E3FF2B8E");	// vmull.p64 q12, d3, d27
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q12 = __vmull_p64(d3, d27)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vqshrun()
        {
            Given_HexString("D8FF3E48");	// vqshrun.s32 d20, q15, #8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void ThumbRw_vmax()
        {
            Given_HexString("29FF2046");	// vmax.u32 d4, d9, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d4 = __vmax_u32(d9, d16)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vmaxnm()
        {
            Given_HexString("CBFE029B");	// vmaxnm.f64 d25, d11, d2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_smusd()
        {
            Given_HexString("48FB00F1");	// smusd r1, r8, r0
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r8, int16, 0) *s SLICE(r0, int16, 0)",
                "2|L--|v6 = SLICE(r8, int16, 16) *s SLICE(r0, int16, 16)",
                "3|L--|r1 = v5 - v6");
        }

        [Test]
        public void ThumbRw_sxtab()
        {
            Given_HexString("42FA83F1");	// sxtab r1, r2, r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2 + (int8) r3");
        }

        [Test]
        public void ThumbRw_sxtah()
        {
            Given_HexString("00FAFEFF");	// sxtah pc, r0, lr, ror #24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|pc = r0 + (int16) (lr >>u 24)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_rev16()
        {
            Given_HexString("70BA");	// rev16 r0, r6
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_stmdb()
        {
            Given_HexString("06E90F00");	// stmdb r6, {r0, r1, r2, r3}
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|L--|Mem0[r6 + -16:word32] = r0",
                "2|L--|Mem0[r6 + -12:word32] = r1",
                "3|L--|Mem0[r6 + -8:word32] = r2",
                "4|L--|Mem0[r6 + -4:word32] = r3");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_pli()
        {
            Given_HexString("9DF9BAF1");	// pli [sp, #0x1ba]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_smlatb()
        {
            Given_HexString("17FB2046");	// smlatb r6, r7, r0, r4
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r6 = (int16) (r7 >> 16) *s (int16) r0 + r4",
                "2|L--|Q = cond(r6)");
        }

        [Test]
        public void ThumbRw_vsri()
        {
            Given_HexString("ECFF3104");	// vsri.32 d16, d17, #0x14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = __vsri_i32(d17, 20)");
        }

        [Test]
        public void ThumbRw_bfc()
        {
            Given_HexString("6FF30001");	// bfc r1, #0, #1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r1 & 0xFFFFFFFE");
        }

        [Test]
        public void ThumbRw_bfi()
        {
            Given_HexString("63F31F43");	// bfi r3, r3, #0x10, #0x10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = SLICE(r3, ui16, 0)",
                "2|L--|r3 = DPB(r3, v3, 16)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vqrshl()
        {
            Given_HexString("18FFB945");	// vqrshl.u16 d4, d25, d24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_sdiv()
        {
            Given_HexString("95FBF3F7");	// sdiv r7, r5, r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = r5 / r3");
        }

        [Test]
        public void ThumbRw_smlaltb()
        {
            Given_HexString("C9FBA0E6");	// smlaltb lr, r6, sb, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|lr_r6 = (int16) (r9 >> 16) *s (int16) r0 + lr_r6");
        }

        [Test]
        public void ThumbRw_smlaltt()
        {
            Given_HexString("C3FBB168");	// smlaltt r6, r8, r3, r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6_r8 = (int16) (r3 >> 16) *s (int16) (r1 >> 16) + r6_r8");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vabdl()
        {
            Given_HexString("DDFFA7E7");	// vabdl.u16 q15, d29, d23
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_pkhbt()
        {
            Given_HexString("C6EA002E");	// pkhbt lr, r6, r0, lsl #8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|lr = __pkhbt(r6, r0 << 0x00000008)");
        }

        [Test]
        public void ThumbRw_vrshr()
        {
            Given_HexString("A1FFB442");	// vrshr.u64 d4, d20, #0x1f
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d4 = __vrshr_u64(d20, 31)");
        }

        [Test]
        public void ThumbRw_smlawt()
        {
            Given_HexString("34FB1020");	// smlawt r0, r4, r0, r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = (r4 *s (int16) (r0 >> 16) >> 16) + r2");
        }

 

        [Test]
        public void ThumbRw_vmul()
        {
            Given_HexString("46FF114D");	// vmul.f32 d20, d6, d1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d20 = __vmul_f32(d6, d1)");
        }

        [Test]
        public void ThumbRw_smlalbb()
        {
            Given_HexString("C4FB8A4C");	// smlalbb r4, ip, r4, sl
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4_ip = (int16) r4 *s (int16) r10 + r4_ip");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vminnm()
        {
            Given_HexString("86FE4A4B");	// vminnm.f64 d4, d6, d10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vpadal()
        {
            Given_HexString("F0FF0546");	// vpadal.s8 d20, d5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vcgt_f32()
        {
            Given_HexString("26FFA18E");	// vcgt.f32 d8, d22, d17
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d8 = __vcgt_f32(d22, d17)");
        }

        [Test]
        public void ThumbRw_vpmax()
        {
            Given_HexString("56FF289A");	// vpmax.u16 d25, d6, d24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d25 = __vpmax_u16(d6, d24)");
        }

        [Test]
        public void ThumbRw_vpmin()
        {
            Given_HexString("2BFF00BF");	// vpmin.f32 d11, d11, d0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d11 = __vpmin_f32(d11, d0)");
        }

        [Test]
        public void ThumbRw_vsli()
        {
            Given_HexString("BDFFB4F5");	// vsli.64 d15, d20, #0x3d
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d15 = __vsli_i64(d20, 61)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_pldw()
        {
            Given_HexString("B9F800F0");	// pldw [sb]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_pkhtb()
        {
            Given_HexString("C0EA2046");	// pkhtb r6, r0, r0, asr #0x10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = __pkhtb(r0, r0 >> 0x00000010)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_uxtab16()
        {
            Given_HexString("33FAFFF7");	// uxtab16 r7, r3, pc, ror #24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void ThumbRw_vabs()
        {
            Given_HexString("B9FF2043");	// vabs.s32 d4, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d4 = __vabs_i32(d16)");
        }

        [Test]
        public void ThumbRw_vadd()
        {
            Given_HexString("4EEF01A8");	// vadd.i8 d26, d14, d1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d26 = __vadd_i8(d14, d1)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vaddl()
        {
            Given_HexString("E5FF0020");	// vaddl.u32 q9, d5, d0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q9 = __vadd_l(d5, d0)");
        }

        [Test]
        public void ThumbRw_vaddw()
        {
            Given_HexString("E2FF0021");	// vaddw.u32 q9, q1, d0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q9 = __vaddw_u32(q1, d0)");
        }

        [Test]
        public void ThumbRw_vbic()
        {
            Given_HexString("C1FF7DE5");	// vbic.i32 q15, #0x9d0000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q15 = __vbic_i32(q15, 0x009D0000009D0000)");
        }

        [Test]
        public void ThumbRw_vrsra()
        {
            Given_HexString("BFFF18B3");	// vrsra.u32 d11, d8, #1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d11 = __vrsra_u32(d8, 1)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vqrshrn()
        {
            Given_HexString("BBFF58B9");	// vqrshrn.u64 d11, q4, #5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void ThumbRw_qadd8()
        {
            Given_HexString("8AFA1AF0");	// qadd8 r0, sl, sl
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __qadd_s8(r10, r10)");
        }

        [Test]
        public void ThumbRw_qsax()
        {
            Given_HexString("E0FA19F0");	// qsax r0, r0, sb
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __qsax(r0, r9)");
        }

        [Test]
        public void ThumbRw_qsub16()
        {
            Given_HexString("D5FA18F4");	// qsub16 r4, r5, r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __qsub_s16(r5, r8)");
        }

        [Test]
        public void ThumbRw_sadd8()
        {
            Given_HexString("8AFA00F0");	// sadd8 r0, r10, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __sadd_s8(r10, r0)");
        }

        [Test]
        public void ThumbRw_sasx()
        {
            Given_HexString("AEFA0EF0");	// sasx r0, lr, lr
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __sasx(lr, lr)");
        }

        [Test]
        public void ThumbRw_vrshl()
        {
            Given_HexString("03FF0DE5");	// vrshl.u8 d14, d13, d3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d14 = __vrshl_u8(d13, d3)");
        }

        [Test]
        public void ThumbRw_usax()
        {
            Given_HexString("EEFA40F6");	// usax r6, lr, r0
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = SLICE(lr, ui16, 0) + SLICE(r0, ui16, 16)",
                "2|L--|v3 = SLICE(lr, ui16, 16) - SLICE(r0, ui16, 0)",
                "3|L--|r6 = SEQ(v3, v2)");
        }

        [Test]
        public void ThumbRw_vpadd()
        {
            Given_HexString("04FF002D");	// vpadd.f32 d2, d4, d0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d2 = __vpadd_f32(d4, d0)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_uqadd16()
        {
            Given_HexString("92FA55F8");	// uqadd16 r8, r2, r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vrev16()
        {
            Given_HexString("F0FF08B1");	// vrev16.8 d27, d8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vshll()
        {
            Given_HexString("FAFF1E4A");	// vshll.u32 q10, d14, #0x1a
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q10 = __vshll_u32(d14, 26)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vqdmulh()
        {
            Given_HexString("5AEF029B");	// vqdmulh.s16 d25, d10, d2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_usat16()
        {
            Given_HexString("ACF30200");	// usat16 r0, #2, ip
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = __usat16(0x00000002, ip)",
                "2|L--|Q = cond(r0)");
        }

        [Test]
        public void ThumbRw_smlabt()
        {
            Given_HexString("1EFB1A68");	// smlabt r8, lr, sl, r6
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r8 = (int16) lr *s (int16) (r10 >> 16) + r6");
        }

        [Test]
        [Ignore(Categories.FailedTests)]

        public void ThumbRw_vsubhn()
        {
            Given_HexString("ECEF2046");	// vsubhn.i64 d20, q6, q8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_uasx()
        {
            Given_HexString("A4FA42F0");	// uasx r0, r4, r2
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = SLICE(r4, ui16, 0) - SLICE(r2, ui16, 16)",
                "2|L--|v3 = SLICE(r4, ui16, 16) + SLICE(r2, ui16, 0)",
                "3|L--|r0 = SEQ(v3, v2)");
        }

        [Test]
        public void ThumbRw_vbif()
        {
            Given_HexString("3FFFBBF1");	// vbif d15, d31, d27
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d15 = __vbif(d31, d27)");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vbsl()
        {
            Given_HexString("14FFBBF1");	// vbsl d15, d20, d27
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_strht()
        {
            Given_HexString("23F8236E");	// strht r6, [r3, #0x23]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r3 + 35:word16] = (uint16) r6");
        }

        [Test]
        public void ThumbRw_strbt()
        {
            Given_HexString("08F8031E");	// strbt r1, [r8, #3]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r8 + 3:byte] = (byte) r1");
        }

        [Test]
        public void ThumbRw_smlsdx()
        {
            Given_HexString("4CFB143D");	// smlsdx sp, ip, r4, r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|sp = sp + ((int16) ip *s (r4 >> 16) - (ip >> 16) *s (int16) r4)");
        }

        [Test]
        public void ThumbRw_vdup()
        {
            Given_HexString("FFFF2BFC");	// vdup.8 d31, d27[7]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d31 = __vdup_8(d27[7])");
        }

        [Test]
        public void ThumbRw_veor()
        {
            Given_HexString("46FF5421");	// veor q9, q3, q2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q9 = q3 ^ q2");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vabal()
        {
            Given_HexString("A1FF8045");	// vabal.u32 q2, d17, d0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }


        [Test]
        public void ThumbRw_smlsld()
        {
            Given_HexString("D2FBC927");	// smlsld lr, r7, r2, sb
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7_r2 = r7_r2 + ((int16) r2 *s (int16) r9 - (r2 >> 16) *s (r9 >> 16))");
        }

        [Test]
        public void ThumbRw_smlsldx()
        {
            Given_HexString("D2FBDD28");	// smlsldx r2, r8, r2, sp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8_r2 = r8_r2 + ((int16) r2 *s (sp >> 16) - (r2 >> 16) *s (int16) sp)");
        }

        [Test]
        public void ThumbRw_smmul()
        {
            Given_HexString("5AFB04F1");	// smmul r1, r10, r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (int32) (r10 *s r4 >> 32)");
        }

        [Test]
        public void ThumbRw_smuad()
        {
            Given_HexString("22FB04F1");	// smuad r1, r2, r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r1 + ((int16) r2 *s (int16) r4 + (r2 >> 16) *s (r4 >> 16))");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_vraddhn()
        {
            Given_HexString("C4FF2044");	// vraddhn.i16 d20, q2, q8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_smlald()
        {
            Given_HexString("C3FBC0B5");	// smlald fp, r5, r3, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5_fp = r5_fp + ((int16) r3 *s (int16) r0 + (r3 >> 16) *s (r0 >> 16))");
        }

        [Test]
        public void ThumbRw_smlald_pc()
        {
            Given_HexString("CFFBC0B5");	// smlald fp, r5, pc, r0
            AssertCode(
                "0|---|00100000(4): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void ThumbRw_smlaldx()
        {
            Given_HexString("C6FBDE4B");	// smlaldx r4, fp, r6, lr
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fp_r4 = fp_r4 + ((int16) r6 *s (lr >> 16) + (r6 >> 16) *s (int16) lr)");
        }

        [Test]
        public void ThumbRw_vclt()
        {
            Given_HexString("F9FF0646");	// vclt.f32 d20, d6, #0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d20 = __vclt_f32(d6, 0x0000000000000000)");
        }

        [Test]
        public void ThumbRw_usub8()
        {
            Given_HexString("C6FA43F0");	// usub8 r0, r6, r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __usub_i8(r6, r3)");
        }

        [Test]
        public void ThumbRw_usub16()
        {
            Given_HexString("D7FA40F6");	// usub16 r6, r7, r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = __usub_i16(r7, r0)");
        }

        [Test]
        public void ThumbRw_vtst()
        {
            Given_HexString("0EEFB088");	// vtst.8 d8, d30, d16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d8 = __vtst_i8(d30, d16)");
        }

        [Test]
        public void ThumbRw_cps()
        {
            Given_HexString("66B6");	// cpsie ai
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__cps()");
        }

        [Test]
        public void ThumbRw_yield()
        {
            Given_HexString("10BF");	// yield 
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__yield()");
        }

        [Test]
        [Ignore(Categories.FailedTests)]
        public void ThumbRw_sev()
        {
            Given_HexString("40BF");	// sev 
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ThumbRw_vmla()
        {
            Given_HexString("A0EFE541");	// vmla.f32 d4, d16, d5[1]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d4 = __vmla_f32(d16, d21)");
        }

        [Test]
        public void ThumbRw_vmls_i16()
        {
            Given_HexString("1CFF2469");	// vmls.u16 d6, d12, d20
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d6 = __vmls_u16(d12, d20)");
        }

        [Test]
        public void ThumbRw_mov_r0_r0()
        {
            Given_HexString("0000");	// mov r0,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0");
        }

        [Test]
        public void ThumbRw_wfi()
        {
            Given_HexString("30BF");
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__wait_for_interrupt()");
        }

        [Test]
        public void ThumbRw_ldr_literal_at_offset_0002()
        {
            Given_Address(0x00100002);
            Given_HexString("004A");
            AssertCode(
                "0|L--|00100002(2): 1 instructions",
                "1|L--|r2 = Mem0[0x00100004:word32]");
        }

        [Test]
        public void ThumbRw_bx_lr()
        {
            Given_HexString("7047");// bx lr
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void ThumbRw_bne_backwards()
        {
            Given_HexString("FED1");    // bne 
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100000");
        }



        [Test]
        public void ThumbRw_hlt()
        {
            Given_HexString("86BA");
            AssertCode(
                "0|H--|00100000(2): 1 instructions",
                "1|H--|__hlt()");
        }


        [Test]
        public void ThumbRw_movs_w()
        {
            Given_HexString("5FEA5B0B");    // movs.w\tfp,fp,lsr #1
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|fp = fp >> 0x00000001",
                "2|L--|NZC = cond(fp)");
        }

        [Test]
        public void ThumbRw_eor_w_lsl()
        {
            Given_HexString("80EA8000");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 ^ r0 << 0x00000002");
        }

        [Test]
        public void ThumbRw_teqs_w()
        {
            Given_HexString("9CEA000F");    // teqs.w\tip,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZC = cond(ip ^ r0)");
        }

        [Test]
        public void ThumbRw_adds_w()
        {
            Given_HexString("10EB0208"); // adds.w\tr8,r0,r2
            AssertCode(
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|r8 = r0 + r2");
        }

        [Test]
        public void ThumbRw_adcs_rxx()
        {
            Given_HexString("54EB3200"); // adcs.w\tr0,r4,r2,rrx
            AssertCode(
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|r0 = r4 + __rcr(r2, 0x00000001, C) + C");
        }

        [Test]
        public void ThumbRw_subs_w()
        {
            Given_HexString("B6EB0A08"); // subs.w\tr8,r6,r10
            AssertCode(
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|r8 = r6 - r10",
                 "2|L--|NZCV = cond(r8)");
        }

        [Test]
        public void ThumbRw_sub_w()
        {
            Given_HexString("A9EB0809"); // sub.w\tr9,r9,r8
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|r9 = r9 - r8");
        }

        [Test]
        public void ThumbRw_cmp_w_lsl()
        {
            Given_HexString("B0EB450F"); // cmp.w\tr0,r5,lsl #1"
            AssertCode(
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|NZCV = cond(r0 - (r5 << 0x00000001))");
        }

  
    }
}
