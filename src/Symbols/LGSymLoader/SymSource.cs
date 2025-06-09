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

using Reko.Core;
using Reko.Core.IO;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Reko.Symbols.LGSymLoader
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	[Endian(Endianness.LittleEndian)]
	public struct SymHeader
	{
		public UInt32 magic;
		public UInt32 unknown;
		public UInt32 size;
		public UInt32 n_symbols;
		public UInt32 tail_size;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[Endian(Endianness.LittleEndian)]
	public struct SymEntry
	{
		public UInt32 addr;
		public UInt32 end;
		public UInt32 sym_name_off;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[Endian(Endianness.LittleEndian)]
	public struct DwarfEntry
	{
		public UInt32 d1;
		public UInt32 d2;
	}


	public class LGSymSource : ISymbolSource
	{
		public const UInt32 SYM_MAGIC = 0xB12791EE;

		private readonly MemoryMappedFile mf;
		private readonly Stream stream;
		private readonly BinaryReader rdr;
        private readonly long filesize;
		private SymHeader hdr;

		private UInt32 n_dwarf_lst;
		private UInt32 dwarf_data_size;

		private long hash_offset;
		private long syms_offset;
		private long dwarf_list_offset;
		private long dwarf_data_offset;
		private long sym_names_offset;

		public LGSymSource(string filename)
        {
			mf = MemoryMappedFile.CreateFromFile(filename, FileMode.Open);
			stream = mf.CreateViewStream();
            this.filesize = new FileInfo(filename).Length;
			rdr = new BinaryReader(stream);
		}

        public LGSymSource(Stream stm)
        {
            stream = stm;
            rdr = new BinaryReader(stm);
            this.filesize = stm.Length;
            mf = null!;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            stream?.Dispose();
            mf?.Dispose();
        }

		public bool CanLoad(string filename, byte[]? fileContents = null)
        {
			try {
                this.hdr = rdr.ReadStruct<SymHeader>();
				if (hdr.magic != SYM_MAGIC)
					return false;

                var expected = hdr.size + Marshal.SizeOf(hdr);
                if (expected != this.filesize)
					return false;

				if ((hdr.tail_size + Marshal.SizeOf(typeof(SymEntry)) * hdr.n_symbols) != hdr.size)
					return false;

				long offset = Marshal.SizeOf(hdr);

				syms_offset = offset;
				offset += Marshal.SizeOf(typeof(SymEntry)) * hdr.n_symbols;

				rdr.BaseStream.Seek(offset, SeekOrigin.Begin);
				UInt32 has_hash = rdr.ReadUInt32();
				offset += Marshal.SizeOf(has_hash);

				if(has_hash != 2 && has_hash != 0)
					return false;

				if (has_hash == 2)
				{
					hash_offset = offset;
					offset += Marshal.SizeOf(typeof(UInt32)) * ((hdr.n_symbols + 1) & (~0 - 1));
				}

				rdr.BaseStream.Seek(offset, SeekOrigin.Begin);
				UInt32 has_dwarf = rdr.ReadUInt32();

				if(has_dwarf == 1)
				{
					offset += Marshal.SizeOf(has_dwarf);

					rdr.BaseStream.Seek(offset, SeekOrigin.Begin);
					n_dwarf_lst = rdr.ReadUInt32();
					offset += Marshal.SizeOf(n_dwarf_lst);

					rdr.BaseStream.Seek(offset, SeekOrigin.Begin);
					dwarf_data_size = rdr.ReadUInt32();
					offset += Marshal.SizeOf(dwarf_data_size);

					dwarf_list_offset = offset;
					offset += Marshal.SizeOf(typeof(DwarfEntry)) * n_dwarf_lst;

					dwarf_data_offset = offset;
					offset += dwarf_data_size;
				}

                sym_names_offset = offset;
				return true;

			} catch {
				return false;
			}
		}

		public List<ImageSymbol> GetAllSymbols() {
			var symbols = new List<ImageSymbol>();
            for (uint i = 0; i < hdr.n_symbols; i++)
            {
                rdr.BaseStream.Seek(syms_offset + Marshal.SizeOf(typeof(SymEntry)) * i, SeekOrigin.Begin);
                SymEntry? s = rdr.ReadStruct<SymEntry>();
                if (s is null)
                    break;

                var sym = s.Value;
                rdr.BaseStream.Seek(sym_names_offset + sym.sym_name_off, SeekOrigin.Begin);
                string sym_name = rdr.ReadNullTerminatedString();

                //$BUG: how do we get the architecture?
                symbols.Add(ImageSymbol.Create(
                    SymbolType.Unknown,
                    null!,
                    Address.Ptr32(sym.addr),
                    sym_name,
                    new UnknownType((int)(sym.end - sym.addr))));
            }

			return symbols;
		}
	}
}
