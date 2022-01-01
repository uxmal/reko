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
using Reko.Core.Memory;
using Reko.ImageLoaders.Omf;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Reko.ImageLoaders.Omf.RecordType;

namespace Reko.UnitTests.ImageLoaders.Omf
{
    [TestFixture]
    public class OmfLoaderTests
    {
        private LeImageWriter writer;
        private Mocks.FakePlatform platform;

        [SetUp]
        public void Setup()
        {
            this.writer = new LeImageWriter();
            var sc = new ServiceContainer();
            var arch = new Mocks.FakeArchitecture(sc);
            this.platform = new Mocks.FakePlatform(sc, arch);
        }

        private void Given_Omf(RecordType rt, ushort length, params Action [] mutators)
        {
            writer.WriteByte((byte)rt);
            writer.WriteLeUInt16(length);
            foreach (var mutator in mutators)
            {
                mutator();
            }
        }

        private Action B(params byte [] b)
        {
            return () => { writer.WriteBytes(b); };
        }

        private Action W(ushort n)
        {
            return () => { writer.WriteLeUInt16(n); };
        }

        private Action DW(uint n)
        {
            return () => { writer.WriteLeUInt32(n); };
        }

        private Action S(string s)
        {
            return () =>
            {
                var len = (byte) s.Length;
                writer.WriteByte(len);
                writer.WriteString(s, Encoding.ASCII);
            };
        }

        private Action Pad(uint padding)
        {
            return () =>
            {
                writer.WriteBytes(0, padding);
            };
        }

        private Action Align(int aligment)
        {
            return () =>
            {
                while ((writer.Position % aligment) != 0)
                {
                    writer.WriteByte(0);
                }
            };
        }

        [Test]
        public void Omfldr_Library_Single_Record()
        {
            Given_Omf(LibraryHeader, 0xD, DW(0x10), W(1), B(0), Pad(6));
            Given_Omf(THEADR, 0x0C, S("ANSIINJECT"), B(42));
            Given_Omf(COMENT, 0x1B,
                B(0x80), B(0xA0), B(0x01),
                    B(0x01),
                    S("ANSIINJECT"),
                    S("ANSICALL"),
                    W(1),
                    B(42));
            Given_Omf(COMENT, 0x08,
                B(0xC0, 0xFE), DW(0x12345678), B(0), B(42));
            Given_Omf(MODEND, 0x02, B(0), B(42), Align(0x10));
            Given_Omf(LibraryEnd, 0x0, Align(0x10));

            var omf = new OmfLoader(platform.Services, ImageLocation.FromUri("file:foo.lib"), writer.ToArray());
            var typelib = omf.Load(platform, new TypeLibrary());
            var module = typelib.Modules["ANSICALL"];
            var svc = module.ServicesByOrdinal[1];
            Assert.AreEqual("ANSIINJECT", svc.Name);
        }

/*
00000000 F0 0D 00 
            00 3C 01 00 59 00 00 00 00 00 00 00 00 ....<..Y........

00000010 80 0C 00 
            0A 41 4E 53 49 49 4E 4A 45 43 54 82 ....ANSIINJECT..
            
         88 1B 00  <- coment
            80
            A0 <- OMF extension
                01 <- IMPDEF 
                01  = use ordinal
                0A 41 4E 53 49 49 4E 4A 45 43 54 .......ANSIINJECT
                08 41 4E 53 49 43 41 4C 4C      ANSICALL
                01 00  - ordinal
                F9     
         88 08 00  
            C0 FE 54 48 19 FA 59 AA
         8A 02 00 00 74 00 00 00

00000050 80 0C 00 
            0A 41 4E 53 49 4B 45 59 44 45 46 87  ....ANSIKEYDEF..
         88 1B 00 
            80 
            A0 
                01 
                01 
                0A 41 4E 53 49 4B 45 59 44 45 .......ANSIKEYDEF
                08 41 4E 53 49 43 41 4C 4C 
                02 00 FD 
           88 08 00 
              C0 FE 54 48 19 FA 59 AA 
           8A 02 00 00 74 00 00 00 

00000090 80 0C 00 
            0A 41 4E 53 49 49 4E 54 45 52 50 6D ....ANSIINTERPm.
         88 1B 00 
            80 
            A0 
                01 
                01 
                0A 41 4E 53 49 49 4E 54 45 52 .......ANSIINTERP
                08 41 4E 53 49 43 41 4C 4C      P.ANSICALL......
                03 00 E2 
          88 08 00 
             C0 
             FE 54  - time stamp
             48 19 FA 59 
             AA 
         8A 02 00 00 74 00 00 00 

000000D0 80 0A 00 
            08 44 4F 53 43 57 41 49 54 10   ....DOSCWAIT....
         88 19 00 
            80 
            A0 
                01 
                01 
                08 44 4F 53 43 57 41 49 54 .....DOSCWAIT
                08 44 4F 53 43 41 4C 4C 53 DOSCALLS..x.....TH
                02 00 78 
          88 08 00 C0 FE 54 48 
0000:0100 19 FA 59 AA 8A 02 00 00 74 00 00 00 00 00 00 00 ..Y.....t.......
0000:0110 80 11 00 0F 44 4F 53 45 4E 54 45 52 43 52 49 54 ....DOSENTERCRIT
0000:0120 53 45 43 EF 88 20 00 80 A0 01 01 0F 44 4F 53 45 SEC.. ......DOSE
0000:0130 4E 54 45 52 43 52 49 54 53 45 43 08 44 4F 53 43 NTERCRITSEC.DOSC
0000:0140 41 4C 4C 53 03 00 56 88 08 00 C0 FE 54 48 19 FA ALLS..V.....TH..
0000:0150 59 AA 8A 02 00 00 74 00 00 00 00 00 00 00 00 00 Y.....t.........
0000:0160 80 09 00 07 44 4F 53 45 58 49 54 50 88 18 00 80 ....DOSEXITP....
0000:0170 A0 01 01 07 44 4F 53 45 58 49 54 08 44 4F 53 43 ....DOSEXIT.DOSC
0000:0180 41 4C 4C 53 05 00 B5 88 08 00 C0 FE 54 48 19 FA ALLS........TH..
0000:0190 59 AA 8A 02 00 00 74 00 00 00 00 00 00 00 00 00 Y.....t.........
0000:01A0 80 10 00 0E 44 4F 53 45 58 49 54 43 52 49 54 53 ....DOSEXITCRITS
0000:01B0 45 43 35 88 1F 00 80 A0 01 01 0E 44 4F 53 45 58 EC5........DOSEX
0000:01C0 49 54 43 52 49 54 53 45 43 08 44 4F 53 43 41 4C ITCRITSEC.DOSCAL
0000:01D0 4C 53 06 00 99 88 08 00 C0 FE 54 48 19 FA 59 AA LS........TH..Y.
0000:01E0 8A 02 00 00 74 00 00 00 00 00 00 00 00 00 00 00 ....t...........
0000:01F0 80 0D 00 0B 44 4F 53 45 58 49 54 4C 49 53 54 0C ....DOSEXITLIST.
*/

    }
}
