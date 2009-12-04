/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Environments.Msdos;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.ImageLoaders.MzExe
{
    /// <summary>
    /// Loader for MS-DOS executables packed with EXEPACK.
    /// </summary>
    public class ExePackLoader : ImageLoader
    {
        private IProcessorArchitecture arch;
        private Platform platform;

        private uint exeHdrSize;
        private uint hdrOffset;
        private uint relocationsOffset;

        private ushort ip;				// 0000
        private ushort cs;				// 0002
        private ushort cbExepackHeader;	// 0006
        private ushort sp;				// 0008
        private ushort ss;				// 000A
        private ushort cpUncompressed;	// 000C

        private ProgramImage imgU;

        public ExePackLoader(IServiceProvider services, ExeImageLoader exe, byte[] imgRaw)
            : base(services, imgRaw)
        {
            arch = new IntelArchitecture(ProcessorMode.Real);
            platform = new MsdosPlatform(arch);

            this.exeHdrSize = (uint)(exe.e_cparHeader * 0x10U);
            this.hdrOffset = (uint)(exe.e_cparHeader + exe.e_cs) * 0x10U;
            ImageReader rdr = new ImageReader(RawImage, hdrOffset);
            this.ip = rdr.ReadLeUint16();
            this.cs = rdr.ReadLeUint16();
            rdr.ReadLeUint16();
            this.cbExepackHeader = rdr.ReadLeUint16();
            this.sp = rdr.ReadLeUint16();
            this.ss = rdr.ReadLeUint16();
            this.cpUncompressed = rdr.ReadLeUint16();

            int offset = ExePackHeaderOffset(exe);
            if (ProgramImage.CompareArrays(imgRaw, offset, signature, signature.Length))
            {
                relocationsOffset = 0x012D;
            }
            else if (ProgramImage.CompareArrays(imgRaw, offset, signature2, signature2.Length))
            {
                relocationsOffset = 0x0125;
            }
            else
                throw new ApplicationException("Not a recognized EXEPACK image.");
        }

        public override IProcessorArchitecture Architecture
        {
            get { return new IntelArchitecture(ProcessorMode.Real); }
        }

        public override Platform Platform
        {
            get { return platform; }
        }

        static public bool IsCorrectUnpacker(ExeImageLoader exe, byte[] rawImg)
        {
            int offset = ExePackHeaderOffset(exe);
            return ProgramImage.CompareArrays(rawImg, offset, signature, signature.Length) ||
                   ProgramImage.CompareArrays(rawImg, offset, signature2, signature2.Length);
        }

        private static int ExePackHeaderOffset(ExeImageLoader exe)
        {
            return (exe.e_cparHeader + exe.e_cs) * 0x10 + exe.e_ip;
        }


        public override ProgramImage Load(Address addr)
        {
            byte[] abC = RawImage;
            byte[] abU = new byte[cpUncompressed * 0x10U + ExeImageLoader.CbPsp];
            Array.Copy(abC, exeHdrSize, abU, ExeImageLoader.CbPsp, abC.Length - exeHdrSize);
            imgU = new ProgramImage(addr, abU);

            int SI = (int)hdrOffset - 1;
            while (abC[SI] == 0xFF)
            {
                --SI;
            }

            int DI = abU.Length - 1;
            byte op;
            do
            {
                op = abC[SI];
                int cx = ProgramImage.ReadLeUint16(abC, SI - 2);
                SI -= 3;
                if ((op & 0xFE) == 0xB0)
                {
                    byte b = abC[SI--];
                    while (cx != 0)
                    {
                        abU[DI--] = b;
                        --cx;
                    }
                }
                else
                {
                    if ((op & 0xFE) != 0xB2)
                        throw new ApplicationException("Packed file is corrupt.");
                    while (cx != 0)
                    {
                        abU[DI--] = abC[SI--];
                        --cx;
                    }
                }
            } while ((op & 1) == 0);
            return imgU;
        }

        public override Address PreferredBaseAddress
        {
            get { return new Address(0x800, 0); }
        }

        public override void Relocate(Address addrLoad, List<EntryPoint> entryPoints, RelocationDictionary relocations)
        {
            ImageMap imageMap = imgU.Map;
            ImageReader rdr = new ImageReader(RawImage, hdrOffset + relocationsOffset);
            ushort segCode = (ushort)(addrLoad.Selector + (ExeImageLoader.CbPsp >> 4));
            ushort dx = 0;
            for (; ; )
            {
                int cx = rdr.ReadLeUint16();
                if (cx != 0)
                {
                    int relocBase = ExeImageLoader.CbPsp + dx * 0x10;
                    do
                    {
                        ushort relocOff = rdr.ReadLeUint16();
                        ushort seg = imgU.FixupLeUint16(relocBase + relocOff, segCode);
                        imageMap.AddSegment(new Address(seg, 0), seg.ToString("X4"), AccessMode.ReadWrite);
                    } while (--cx != 0);
                }
                if (dx == 0xF000)
                    break;
                dx += (ushort)0x1000U;
            }

            this.cs += segCode;
            imageMap.AddSegment(new Address(cs, 0), cs.ToString("X4"), AccessMode.ReadWrite);
            this.ss += segCode;
            IntelState state = new IntelState();
            state.Set(Registers.ds, new Constant(PrimitiveType.Word16, addrLoad.Selector));
            state.Set(Registers.es, new Constant(PrimitiveType.Word16, addrLoad.Selector));
            state.Set(Registers.cs, new Constant(PrimitiveType.Word16, cs));
            state.Set(Registers.ss, new Constant(PrimitiveType.Word16, ss));
            state.Set(Registers.bx, new Constant(PrimitiveType.Word16, 0));
            entryPoints.Add(new EntryPoint(new Address(cs, ip), state));
        }

        private static byte[] signature = 
		{
						0x8C, 0xC0, 0x05, 0x10, 0x00, 0x0E, 0x1F, 0xA3, 0x04, 0x00, 0x03, 0x06, 0x0C, 0x00,
			0x8E, 0xC0, 0x8B, 0x0E, 0x06, 0x00, 0x8B, 0xF9, 0x4F, 0x8B, 0xF7, 0xFD, 0xF3, 0xA4, 0x8B, 0x16,
			0x0E, 0x00, 0x50, 0xB8, 0x38, 0x00, 0x50, 0xCB, 0x8C, 0xC3, 0x8C, 0xD8, 0x2B, 0xC2, 0x8E, 0xD8,
			0x8E, 0xC0, 0xBF, 0x0F, 0x00, 0xB9, 0x10, 0x00, 0xB0, 0xFF, 0xF3, 0xAE, 0x47, 0x8B, 0xF7, 0x8B,
			0xC3, 0x2B, 0xC2, 0x8E, 0xC0, 0xBF, 0x0F, 0x00, 0xB1, 0x04, 0x8B, 0xC6, 0xF7, 0xD0, 0xD3, 0xE8,
			0x74, 0x09, 0x8C, 0xDA, 0x2B, 0xD0, 0x8E, 0xDA, 0x83, 0xCE, 0xF0, 0x8B, 0xC7, 0xF7, 0xD0, 0xD3,
			0xE8, 0x74, 0x09, 0x8C, 0xC2, 0x2B, 0xD0, 0x8E, 0xC2, 0x83, 0xCF, 0xF0, 0xAC, 0x8A, 0xD0, 0x4E,
			0xAD, 0x8B, 0xC8, 0x46, 0x8A, 0xC2, 0x24, 0xFE, 0x3C, 0xB0, 0x75, 0x06, 0xAC, 0xF3, 0xAA, 0xEB,
			0x07, 0x90
		};

        private static byte[] signature2 = 
        {
            0x8C, 0xC0, 0x05, 0x10, 0x00, 0x0E, 0x1F, 0xA3, 0x04, 0x00, 0x03, 0x06, 0x0C, 0x00, 0x8E, 0xC0,
            0x8B, 0x0E, 0x06, 0x00, 0x8B, 0xF9, 0x4F, 0x8B, 0xF7, 0xFD, 0xF3, 0xA4, 0x50, 0xB8, 0x32, 0x00,
            0x50, 0xCB, 0x8C, 0xC3, 0x8C, 0xD8, 0x48, 0x8E, 0xD8, 0x8E, 0xC0, 0xBF, 0x0F, 0x00, 0xB9, 0x10,
            0x00, 0xB0, 0xFF, 0xF3, 0xAE, 0x47, 0x8B, 0xF7, 0x8B, 0xC3, 0x48, 0x8E, 0xC0, 0xBF, 0x0F, 0x00,
            0xB1, 0x04, 0x8B, 0xC6, 0xF7, 0xD0, 0xD3, 0xE8, 0x74, 0x09, 0x8C, 0xDA, 0x2B, 0xD0, 0x8E, 0xDA,
            0x83, 0xCE, 0xF0, 0x8B, 0xC7, 0xF7, 0xD0, 0xD3, 0xE8, 0x74, 0x09, 0x8C, 0xC2, 0x2B, 0xD0, 0x8E,
            0xC2, 0x83, 0xCF, 0xF0, 0xAC, 0x8A, 0xD0, 0x4E, 0xAD, 0x8B, 0xC8, 0x46, 0x8A, 0xC2, 0x24, 0xFE,
            0x3C, 0xB0, 0x75, 0x06, 0xAC, 0xF3, 0xAA, 0xEB, 0x07, 0x90, 0x3C, 0xB2, 0x75, 0x6B, 0xF3, 0xA4,
            0x8A, 0xC2, 0xA8, 0x01, 0x74, 0xBA, 0xBE, 0x25, 0x01, 0x0E, 0x1F, 0x8B, 0x1E, 0x04, 0x00, 0xFC,
            0x33, 0xD2, 0xAD, 0x8B, 0xC8, 0xE3, 0x13, 0x8B, 0xC2, 0x03, 0xC3, 0x8E, 0xC0, 0xAD, 0x8B, 0xF8,
            0x83, 0xFF, 0xFF, 0x74, 0x11, 0x26, 0x01, 0x1D, 0xE2, 0xF3, 0x81, 0xFA, 0x00, 0xF0, 0x74, 0x16,
            0x81, 0xC2, 0x00, 0x10, 0xEB, 0xDC, 0x8C, 0xC0, 0x40, 0x8E, 0xC0, 0x83, 0xEF, 0x10, 0x26, 0x01,
            0x1D, 0x48, 0x8E, 0xC0, 0xEB, 0xE2, 0x8B, 0xC3, 0x8B, 0x3E, 0x08, 0x00, 0x8B, 0x36, 0x0A, 0x00,
            0x03, 0xF0, 0x01, 0x06, 0x02, 0x00, 0x2D, 0x10, 0x00, 0x8E, 0xD8, 0x8E, 0xC0, 0xBB, 0x00, 0x00,
            0xFA, 0x8E, 0xD6, 0x8B, 0xE7, 0xFB, 0x2E, 0xFF, 0x2F,        
        };
    }
}
