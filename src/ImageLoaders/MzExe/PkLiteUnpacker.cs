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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Environments.Msdos;
using System;
using System.Collections.Generic;

namespace Reko.ImageLoaders.MzExe
{
	/// <summary>
	/// A loader that understands how to unpack a binary packed with PkLite.
	/// </summary>
	public class PkLiteUnpacker : ImageLoader
	{
        private IProcessorArchitecture arch;
        private IPlatform platform;

		private byte [] abU;
		private MemoryArea imgU;
        private SegmentMap segmentMap;
		private ushort pklCs;
		private ushort pklIp;
		private BitStream bitStm;

		private const uint signatureOffset = 0x1C;
		private const uint PspSize = 0x0100;

		public PkLiteUnpacker(MsdosImageLoader loader) 
            : base(loader.Services, loader.Filename, loader.RawImage)
		{
            var cfgSvc = Services.RequireService<IConfigurationService>();
            this.arch = cfgSvc.GetArchitecture("x86-real-16");
            platform = cfgSvc.GetEnvironment("ms-dos")
                .Load(Services, arch);

			uint pkLiteHdrOffset = (uint) (loader.ExeLoader.e_cparHeader * 0x10);

			if (RawImage[pkLiteHdrOffset] != 0xB8)
				throw new ApplicationException(string.Format("Expected MOV AX,XXXX at offset 0x{0:X4}.", pkLiteHdrOffset));
			uint cparUncompressed = MemoryArea.ReadLeUInt16(RawImage, pkLiteHdrOffset + 1);
			abU = new byte[cparUncompressed * 0x10U];

			if (RawImage[pkLiteHdrOffset + 0x04C] != 0x83)
				throw new ApplicationException(string.Format("Expected ADD BX,+XX at offset 0x{0:X4}.", pkLiteHdrOffset + 0x04C));
			uint offCompressedData = pkLiteHdrOffset + RawImage[pkLiteHdrOffset + 0x04E] * 0x10u - PspSize;
			bitStm = new BitStream(RawImage, (int) offCompressedData);
		}

		static public bool IsCorrectUnpacker(ExeImageLoader exe, byte [] rawImg)
		{
			if (exe.e_ovno != 0)
				return false;

			return MemoryArea.CompareArrays(rawImg, (int) signatureOffset, signature, signature.Length);
		}

        public override Program Load(Address addrLoad)
		{
			uint dst = PspSize;

			for (;;)
			{
				if (bitStm.GetBit() == 0)
				{
					abU[dst++] = bitStm.GetByte();
					continue;
				}

				// Read span length

				int CX = 0;
				int BX = bitStm.AccumulateBit(0);				// bx= [0-1]
				BX = bitStm.AccumulateBit(BX);					// bx= [0-3]
				if (BX < 0x02)
				{
					BX = bitStm.AccumulateBit(BX);			// bx=[0-3]
					if (BX != 0)
					{
						BX = bitStm.AccumulateBit(BX);			// bx=[2-7]
						if (BX >= 0x05)
						{
							BX = bitStm.AccumulateBit(BX);		// bx=[0xA - 0xF]
							if (BX > 0x0C)
							{
								BX &= 0x03;
								BX = bitStm.AccumulateBit(BX);
								if (BX >= 0x05)
							    {
									BX = bitStm.AccumulateBit(BX);
									if (BX > 0x0C)
									{
										BX = BX & 0x03;
										BX = bitStm.AccumulateBit(BX);
										if (BX >= 0x05)
										{
											BX = bitStm.AccumulateBit(BX);
										}
										CX = ab0211[BX + 0x0B];
										goto l00C5;
									}
								}
								CX = ab0211[BX];
								if (CX != 0x19)
									goto l00C5;


								byte AL = bitStm.GetByte();
								CX += AL;
								if (AL < 0xFE)
									goto l00C5;

								/*
													ES = ES + ((DI - 0x2000) >> 4);
													DI = (DI & 0F) + 0x2000;

													DS = DS + (SI >> 4);
													SI = SI & 0x0F;
													DS = DS + BX;

								*/					
								if (AL == 0xFF)			// done!
									goto l01C8;

								continue;

							}
						}
					}
					BX = ab0206[BX];
				}
				CX = BX;

				// Read offset in dictionary.

			l00C5:
				BX = 0;
				if (CX == +02)
				{
					dst = CopyDictionaryWord(abU, BX, CX, bitStm, dst);
					continue;
				}
				if (bitStm.GetBit() != 0)
				{
					dst = CopyDictionaryWord(abU, BX, CX, bitStm, dst);
					continue;
				}

				BX = bitStm.AccumulateBit(BX);	// bx: [0-1]
				BX = bitStm.AccumulateBit(BX);	// bx: [0-3]
				BX = bitStm.AccumulateBit(BX);	// bx: [0-7]
				if (BX < 02)
				{
					dst = CopyDictionaryWord2(abU, BX, CX, bitStm, dst);
					continue;
				}

				BX = bitStm.AccumulateBit(BX);
				if (BX < 08)
				{
					dst = CopyDictionaryWord2(abU, BX, CX, bitStm, dst);
					continue;
				}

				BX = bitStm.AccumulateBit(BX);
				if (BX < 0x17)
				{
					dst = CopyDictionaryWord2(abU, BX, CX, bitStm, dst);
					continue;
				}

				BX = bitStm.AccumulateBit(BX);
				BX = (BX & 0x00DF) << 8;
				dst = CopyDictionaryWord(abU, BX, CX, bitStm, dst);
				continue;


			}
		

/*
l01C8:
		  5B            POP	BX
2DE9:01C9 8BEB          MOV	BP,BX				// unpackedBase
2DE9:01CB 83C310        ADD	BX,+10				// BX => unpackedBase + 0x100
*/
			l01C8:
			imgU = new MemoryArea(addrLoad, abU);
            segmentMap = new SegmentMap(imgU.BaseAddress,
                new ImageSegment("image", imgU, AccessMode.ReadWriteExecute));
			return new Program(segmentMap, arch, platform);
		}

		public uint CopyDictionaryWord(byte [] abU, int offset, int bytes, BitStream stm, uint dst)
		{
			offset |= stm.GetByte();
			var src = dst - offset;
			do 
			{
				abU[dst++] = abU[src++];
			} while (--bytes != 0);
			return dst;
		}
	
		public uint CopyDictionaryWord2(byte [] abU, int BX, int bytes, BitStream stm, uint dst)
		{
			BX = (ushort) (ab022C[BX] << 8);
			return CopyDictionaryWord(abU, BX, bytes, stm, dst);
		}

		public override Address PreferredBaseAddress
		{
			get { return Address.SegPtr(0x800, 0); }
            set { throw new NotImplementedException(); }
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
		{
            var relocations = imgU.Relocations;
			ushort segCode = (ushort) (addrLoad.Selector.Value + (PspSize >> 4));
			for (;;)
			{
				int relocs = (ushort) bitStm.GetByte();
				if (relocs == 0)
					break;

				uint relocBase = PspSize + bitStm.GetWord() * 0x10u;
				do
				{
					ushort relocOff = bitStm.GetWord();
					ushort seg = imgU.ReadLeUInt16(relocBase + relocOff);
					seg = (ushort) (seg + segCode);

					imgU.WriteLeUInt16(relocBase + relocOff, seg);
					relocations.AddSegmentReference(relocBase + relocOff, seg);
					segmentMap.AddOverlappingSegment(seg.ToString("X4"), imgU, Address.SegPtr(seg, 0), AccessMode.ReadWriteExecute);
				} while (--relocs != 0);
			}

			ushort pklSs = (ushort) (bitStm.GetWord() + segCode);
			ushort pklSp = (ushort) bitStm.GetWord();
			pklCs = (ushort) (bitStm.GetWord() + segCode);
			pklIp = bitStm.GetWord();

			var state = arch.CreateProcessorState();
			state.SetRegister(Registers.ds, Constant.Word16(addrLoad.Selector.Value));
			state.SetRegister(Registers.es, Constant.Word16(addrLoad.Selector.Value));
			state.SetRegister(Registers.cs, Constant.Word16(pklCs));
			state.SetRegister(Registers.ax, Constant.Word16(0));
			state.SetRegister(Registers.bx, Constant.Word16(0));
			state.SetRegister(Registers.cx, Constant.Word16(0));
			state.SetRegister(Registers.dx, Constant.Word16(0));
			state.SetRegister(Registers.bp, Constant.Word16(0));
			state.SetRegister(Registers.sp, Constant.Word16(pklSp));
			state.SetRegister(Registers.si, Constant.Word16(0));
			state.SetRegister(Registers.di, Constant.Word16(0));

            var sym = ImageSymbol.Procedure(arch, Address.SegPtr(pklCs, pklIp), state: state);
            return new RelocationResults(
                new List<ImageSymbol> { sym },
                new SortedList<Address, ImageSymbol> { { sym.Address, sym } });
		}


		private static byte [] signature =
		{
			0x05, 0x21, 0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45		// 'PKLITE'
		};

		private static byte [] ab0206 = 
		{
			0x04, 0x00, 0x05, 0x06,  0x07, 0x00, 0x00, 0x00,  
			0x00, 0x00, 0x08, 0x09,  0x0A, 0x0B, 0x0C, 0x19
		};


		private static byte [] ab0211 = 
		{

			0x09,
			0x0A,
			0x0B,
			0x0C,
	
			0x19,
			0x00,
			0x00,
			0x00,
	
			0x00,
			0x00,
			0x0D,
			0x0E,
	
			0x0F,
			0x10,
			0x11,
			0x12,
	
			0x00,
			0x00,
			0x00,
			0x00,
	
			0x00,
			0x13,
			0x14,
			0x15,
	
			0x16,
			0x17,
			0x18,
		};

		private static byte [] ab022C =
		{
			0x01, 0x02, 0x00, 0x00,  0x03, 0x04, 0x05, 0x06,  0x00, 0x00, 0x00, 0x00,  0x00, 0x00, 0x00, 0x00,
			0x07, 0x08, 0x09, 0x0A,  0x0B, 0x0C, 0x0D, 0x00,
		};
	}
}
