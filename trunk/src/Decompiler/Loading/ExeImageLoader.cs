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

using Decompiler.Arch.Intel;
using Decompiler.Arch.Intel.MsDos;
using Decompiler.Core;
using System;
using System.Collections;

namespace Decompiler.Loading
{
	/// <summary>
	/// Loads Microsoft EXE image files
	/// </summary>
	public class ExeImageLoader : ImageLoader
	{
		private Program prog;
		private ImageLoader ldrDeferred;

		public ushort	e_magic;                     // Magic number
		public ushort   e_cbLastPage;                // Bytes on last page of file
		public ushort   e_cpImage;                   // Pages in file
		public ushort   e_cRelocations;              // Relocations

		public ushort   e_cparHeader;                // Size of header in paragraphs
		public ushort   e_minalloc;                  // Minimum extra paragraphs needed
		public ushort   e_maxalloc;                  // Maximum extra paragraphs needed
		public ushort   e_ss;                        // Initial (relative) SS value

		public ushort   e_sp;                        // Initial SP value
		public ushort   e_csum;                      // Checksum
		public ushort   e_ip;                        // Initial IP value
		public ushort   e_cs;                        // Initial (relative) CS value

		public ushort   e_lfaRelocations;			 // File address of relocation table
		public ushort   e_ovno;                      // Overlay number
		public ushort [] e_res;                      // Reserved words
		public ushort   e_oemid;                     // OEM identifier (for e_oeminfo)
		public ushort   e_oeminfo;                   // OEM information; e_oemid specific
		public ushort [] e_res2;                     // Reserved words
		public uint      e_lfanew;                    // File address of new exe header

		private const int MarkZbikowski = (('Z' << 8) | 'M');		// 'MZ' magic number expressed in little-endian.

		public const int CbPsp = 0x0100;			// Program segment prefix size in bytes.
		public const int CbPageSize = 0x0200;		// MSDOS pages are 512 bytes.

		public ExeImageLoader(Program prog, byte [] image) : base(image)
		{
			this.prog = prog;

			ReadCommonExeFields();	
		
			if (e_magic != MarkZbikowski)
				throw new FormatException("Image is not an MSDOS executable image.");

			if (IsPortableExecutable)
			{
				// Win32 executable.
				ldrDeferred = new PeImageLoader(prog, image, e_lfanew);
			}
			else if (IsNewExecutable)
			{
				// Windows 16-bit or OS/2 executable.
				prog.Architecture = new IntelArchitecture(ProcessorMode.ProtectedSegmented);
				throw new NotImplementedException("NE executable loading not implemented.");
			}
			else
			{
				// A real-mode executable (16-bit x86 code).

				IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
				prog.Architecture = arch;
				prog.Platform = new MsdosPlatform(arch);

				// Detect if it is a compressed image.

				if (LzExeUnpacker.IsCorrectUnpacker(this, image))
				{
					ldrDeferred = new LzExeUnpacker(this, image);
				}
				else if (PkLiteUnpacker.IsCorrectUnpacker(this, image))
				{
					ldrDeferred = new PkLiteUnpacker(this, image);
				}
				else if (ExePackLoader.IsCorrectUnpacker(this, image))
				{
					ldrDeferred = new ExePackLoader(this, image);
				}
				else
				{
					// Uncompressed MS-DOS executable.
					ldrDeferred = new MsdosImageLoader(prog, this);
				}
			}
		}

		public bool IsNewExecutable
		{
			get { return (uint) RawImage.Length > (uint) (e_lfanew + 1) && RawImage[e_lfanew] == 'N' && RawImage[e_lfanew+1] == 'E'; }
		}

		public bool IsRealModeExecutable
		{
			get { return e_lfanew == 0; }
		}

		public bool IsPortableExecutable
		{
			get { return (uint) RawImage.Length > (uint) (e_lfanew + 1) && RawImage[e_lfanew] == 'P' && RawImage[e_lfanew+1] == 'E'; }
		}


		/// <summary>
		/// Loads a Microsoft .EXE file. There are several widely varying sub-formats,
		/// so we need to discover what flavour it is before we can proceed.
		/// </summary>
		public override ProgramImage Load(Address addrLoad)
		{
			return ldrDeferred.Load(addrLoad);
		}

		public override Address PreferredBaseAddress
		{
			get { return ldrDeferred.PreferredBaseAddress; }
		}

		public void ReadCommonExeFields()
		{
			ImageReader rdr = new ImageReader(RawImage, 0);

			e_magic = rdr.ReadUShort();        
			e_cbLastPage = rdr.ReadUShort();         
			e_cpImage = rdr.ReadUShort();           
			e_cRelocations = rdr.ReadUShort();         
			e_cparHeader = rdr.ReadUShort();      
			e_minalloc = rdr.ReadUShort();     
			e_maxalloc = rdr.ReadUShort();     
			e_ss = rdr.ReadUShort();           
			e_sp = rdr.ReadUShort();           
			e_csum = rdr.ReadUShort();         
			e_ip = rdr.ReadUShort();              
			e_cs = rdr.ReadUShort();              
			e_lfaRelocations = rdr.ReadUShort();          
			e_ovno = rdr.ReadUShort();            
			e_res = new ushort[4];
			for (int i = 0; i != 4; ++i)
			{
				e_res[i] = rdr.ReadUShort();          
			}
			e_oemid = rdr.ReadUShort();           
			e_oeminfo = rdr.ReadUShort();         
			e_res2 = new ushort[10];
			for (int i = 0; i != 10; ++i)
			{
				e_res2[i] = rdr.ReadUShort();        
			}
			e_lfanew = rdr.ReadUint();          
		}

		public override void Relocate(Address addrLoad, ArrayList entryPoints)
		{
			ldrDeferred.Relocate(addrLoad, entryPoints);
		}

		void RelocateNewExe(object neHeader)
		{
		}
	}
}
