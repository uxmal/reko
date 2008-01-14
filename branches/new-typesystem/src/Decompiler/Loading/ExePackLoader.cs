/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Arch.Intel;
using System;
using System.Collections;

namespace Decompiler.Loading
{
	/// <summary>
	/// Loader for MS-DOS executables packed with EXEPACK.
	/// </summary>
	public class ExePackLoader : ImageLoader
	{
		private uint exeHdrSize;
		private uint hdrOffset;

		private ushort ip;				// 0000
		private ushort cs;				// 0002
		private ushort cbExepackHeader;	// 0006
		private ushort sp;				// 0008
		private ushort ss;				// 000A
		private ushort cpUncompressed;	// 000C

		private ProgramImage imgU;

		public ExePackLoader(ExeImageLoader exe, byte [] imgRaw) : base(imgRaw)
		{
			this.exeHdrSize = (uint) (exe.e_cparHeader * 0x10U);
			this.hdrOffset = (uint) (exe.e_cparHeader + exe.e_cs) * 0x10U;
			ImageReader rdr = new ImageReader(RawImage, hdrOffset);
			this.ip = rdr.ReadUShort();
			this.cs = rdr.ReadUShort();
					  rdr.ReadUShort();
			this.cbExepackHeader = rdr.ReadUShort();
			this.sp = rdr.ReadUShort();
			this.ss = rdr.ReadUShort();
			this.cpUncompressed = rdr.ReadUShort();
		}

		static public bool IsCorrectUnpacker(ExeImageLoader exe, byte [] rawImg)
		{
			int offset = (exe.e_cparHeader + exe.e_cs) * 0x10 + exe.e_ip;
			return CompareEqual(rawImg, offset, signature, signature.Length);
		}


		public override ProgramImage Load(Address addr)
		{
			byte [] abC = RawImage;
			byte [] abU = new byte[cpUncompressed * 0x10U + ExeImageLoader.CbPsp];
			Array.Copy(abC, exeHdrSize, abU, ExeImageLoader.CbPsp, abC.Length - exeHdrSize);
			imgU = new ProgramImage(addr, abU);

			int SI = (int) hdrOffset - 1;
			while (abC[SI] == 0xFF)
			{
				--SI;
			}

			int DI = abU.Length - 1;
			byte op;
			do
			{
				op = abC[SI];
				int cx = ProgramImage.ReadUShort(abC, SI - 2);
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
						throw new ApplicationException("Packed file is corrupt");
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

		public override void Relocate(Address addrLoad, ArrayList entryPoints)
		{
			ImageMap imageMap = imgU.Map;
			ImageReader rdr = new ImageReader(RawImage, hdrOffset + 0x012Du);
			ushort segCode = (ushort) (addrLoad.seg + (ExeImageLoader.CbPsp >> 4));
			ushort dx = 0;
			for (;;)
			{
				int cx = rdr.ReadUShort();
				if (cx != 0)
				{
					int relocBase = ExeImageLoader.CbPsp + dx * 0x10; 
					do
					{
						ushort relocOff = rdr.ReadUShort();
						ushort seg = imgU.FixupUShort(relocBase + relocOff, segCode);
						imageMap.AddSegment(new Address(seg, 0), seg.ToString("X4"), AccessMode.ReadWrite);
					} while (--cx != 0);
				}       
				if (dx == 0xF000)
					break;
				dx += (ushort) 0x1000U;
			}

			this.cs += segCode;
			imageMap.AddSegment(new Address(cs, 0), cs.ToString("X4"), AccessMode.ReadWrite);
			this.ss += segCode;
			IntelState state = new IntelState();
			state.Set(Registers.ds, new Value(PrimitiveType.Word16, addrLoad.seg));
			state.Set(Registers.es, new Value(PrimitiveType.Word16, addrLoad.seg));
			state.Set(Registers.cs, new Value(PrimitiveType.Word16, cs));
			state.Set(Registers.ss, new Value(PrimitiveType.Word16, ss));
			state.Set(Registers.bx, new Value(PrimitiveType.Word16, 0));
			entryPoints.Add(new EntryPoint(new Address(cs, ip), state));
		}

		private static byte [] signature = 
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
	}
}
