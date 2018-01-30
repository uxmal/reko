#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Environments.Msdos;
using Reko.Core;
using System;
using System.Collections.Generic;
using Reko.Core.Configuration;
using System.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.ImageLoaders.MzExe
{
	/// <summary>
	/// Loads MS-DOS binary executables that haven't had any packing or encryption
    /// done to them.
	/// </summary>
	public class MsdosImageLoader : ImageLoader
	{
        private IProcessorArchitecture arch;
        private IPlatform platform;
		private ExeImageLoader exe;
		private MemoryArea imgLoaded;
        private SegmentMap segmentMap;

		public MsdosImageLoader(IServiceProvider services, string filename, ExeImageLoader exe) : base(services, filename, exe.RawImage)
		{
			this.exe = exe;
            var cfgSvc = services.RequireService<IConfigurationService>();
            this.arch = cfgSvc.GetArchitecture("x86-real-16");
            this.platform = cfgSvc.GetEnvironment("ms-dos")
                .Load(services, arch);
		}

		public override Address PreferredBaseAddress
		{
			get { return Address.SegPtr(0x0800, 0); }
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            int iImageStart = (exe.e_cparHeader * 0x10);
            int cbImageSize = exe.e_cpImage * ExeImageLoader.CbPageSize - iImageStart;
            byte[] bytes = new byte[cbImageSize];
            int cbCopy = Math.Min(cbImageSize, RawImage.Length - iImageStart);
            Array.Copy(RawImage, iImageStart, bytes, 0, cbCopy);
            imgLoaded = new MemoryArea(addrLoad, bytes);
            segmentMap = new SegmentMap(addrLoad);
            return new Program(segmentMap, arch, platform);
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
		{
			SegmentMap imageMap = segmentMap;
			EndianImageReader rdr = new LeImageReader(exe.RawImage, exe.e_lfaRelocations);
            var relocations = imgLoaded.Relocations;
			int i = exe.e_cRelocations;
            var segments = new Dictionary<Address, ushort>();
            var linBase = addrLoad.ToLinear();
			while (i != 0)
			{
				uint offset = rdr.ReadLeUInt16();
				ushort segOffset = rdr.ReadLeUInt16();
				offset += segOffset * 0x0010u;

				ushort seg = (ushort) (imgLoaded.ReadLeUInt16(offset) + addrLoad.Selector.Value);
				imgLoaded.WriteLeUInt16(offset, seg);
				relocations.AddSegmentReference(offset + linBase, seg);

                var segment = new ImageSegment(
                    seg.ToString("X4"),
                    Address.SegPtr(seg, 0),
                    imgLoaded, 
                    AccessMode.ReadWriteExecute);
                segment = segmentMap.AddSegment(segment);
                segments[segment.Address] = seg;
				--i;
			}
		
            // Create an identifier for each segment.
            foreach (var de in segments)
            {
                var tmp = new TemporaryStorage(
                    string.Format("seg{0:X4}", de.Value),
                    0,
                    PrimitiveType.SegmentSelector);
                segmentMap.Segments[de.Key].Identifier = new Identifier(
                    tmp.Name,
                    PrimitiveType.SegmentSelector,
                    tmp);
            }

			// Found the start address.

			Address addrStart = Address.SegPtr((ushort)(exe.e_cs + addrLoad.Selector.Value), exe.e_ip);
			segmentMap.AddSegment(new ImageSegment(
                addrStart.Selector.Value.ToString("X4"),
                Address.SegPtr(addrStart.Selector.Value, 0),
                imgLoaded,
                AccessMode.ReadWriteExecute));
            DumpSegments(imageMap);

            var ep = new ImageSymbol(addrStart)
            {
                Type = SymbolType.Procedure,
                ProcessorState = arch.CreateProcessorState()
            };
            var sym = platform.FindMainProcedure(program, addrStart);
            var results = new RelocationResults(
                new List<ImageSymbol> { ep },
                new SortedList<Address, ImageSymbol> { { ep.Address, ep } });
            if (sym != null)
            {
                results.Symbols[sym.Address] = sym;
                ep.NoDecompile = true;
            }

			try
			{
				LoadDebugSymbols(results.Symbols, addrLoad);
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to load Borland debug symbols: {0}", e.Message);
			}
            return results;
		}

        private void LoadDebugSymbols(SortedList<Address, ImageSymbol> symbols, Address addrLoad)
        {
            //$REVIEW: this is hardwired. some kind of generic "sniffing" mechanism needs to be implemented.
            // We don't want to load every registered symbol provider, though. Perhaps
            // load symbols in a separate AppDomain, marshal all the symbols across,
            // then discard the appdomain?
            var borsymLdr = new Borland.SymbolLoader(exe, RawImage, addrLoad);
            if (borsymLdr.LoadDebugHeader())
            {
                var syms = borsymLdr.LoadSymbols();
                foreach (var sym in syms)
                {
                    symbols[sym.Key] = sym.Value;
                }
            }
        }

        [Conditional("DEBUG")]
        public void DumpSegments(SegmentMap segmentMap)
        {
            foreach (var seg in this.segmentMap.Segments.Values)
            {
                Debug.Print("{0} {1} size:{2}", seg.Name, seg.Address, seg.Size);
            }
        }
	}
}
