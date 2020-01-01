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
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
		private MemoryArea imgLoaded;
        private SegmentMap segmentMap;
        private Address addrStackTop;
        private ushort segPsp;

        public MsdosImageLoader(ExeImageLoader exe) 
            : base(exe.Services, exe.Filename, exe.RawImage)
		{
			this.ExeLoader = exe;
            var cfgSvc = Services.RequireService<IConfigurationService>();
            this.arch = cfgSvc.GetArchitecture("x86-real-16");
            this.platform = cfgSvc.GetEnvironment("ms-dos")
                .Load(Services, arch);
		}

        public ExeImageLoader ExeLoader { get; }

		public override Address PreferredBaseAddress
		{
			get { return Address.SegPtr(0x0800, 0); }
            set { throw new NotImplementedException(); }
        }

        public override Program Load(Address addrLoad)
        {
            //$TODO: no space allocated for the PSP!
            this.segPsp = (ushort)(addrLoad.Selector.Value - 0x10);
            var ss = (ushort) (ExeLoader.e_ss + addrLoad.Selector.Value);
            this.addrStackTop = Address.SegPtr(ss, ExeLoader.e_sp);

            int iImageStart = (ExeLoader.e_cparHeader * 0x10);
            int cbImageSize = ExeLoader.e_cpImage * ExeImageLoader.CbPageSize - iImageStart;
            // The +4 is room for a far return address at the top of the stack.
            int offsetStackTop = (int)(addrStackTop - addrLoad) + 4;
            cbImageSize = Math.Max(cbImageSize, offsetStackTop);
            byte[] bytes = new byte[cbImageSize];
            int cbCopy = Math.Min(cbImageSize, RawImage.Length - iImageStart);
            Array.Copy(RawImage, iImageStart, bytes, 0, cbCopy);
            imgLoaded = new MemoryArea(addrLoad, bytes);
            this.segmentMap = new SegmentMap(addrLoad);
            return new Program(segmentMap, arch, platform);
        }

        public override ImageSegment AddSegmentReference(Address addr, ushort seg)
        {
            return AddSegmentReference(addr.ToLinear(), seg);
        }

        private ImageSegment AddSegmentReference(ulong linAddr, ushort seg)
        {
            var relocations = imgLoaded.Relocations;
            relocations.AddSegmentReference(linAddr, seg);

            var addrSeg = Address.SegPtr(seg, 0);
            return segmentMap.AddOverlappingSegment(
                    seg.ToString("X4"),
                    imgLoaded,
                    addrSeg,
                    AccessMode.ReadWriteExecute);
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
		{
			SegmentMap imageMap = segmentMap;
            EndianImageReader rdr = new LeImageReader(ExeLoader.RawImage, ExeLoader.e_lfaRelocations);
            int i = ExeLoader.e_cRelocations;
            var segments = new Dictionary<Address, ushort>();
            var linBase = addrLoad.ToLinear();
			while (i != 0)
			{
				uint offset = rdr.ReadLeUInt16();
				ushort segOffset = rdr.ReadLeUInt16();
				offset += segOffset * 0x0010u;

				ushort seg = (ushort) (imgLoaded.ReadLeUInt16(offset) + addrLoad.Selector.Value);
				imgLoaded.WriteLeUInt16(offset, seg);

                var segment = AddSegmentReference(offset + linBase, seg);

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

            Address addrStart = Address.SegPtr((ushort) (ExeLoader.e_cs + addrLoad.Selector.Value), ExeLoader.e_ip);
			segmentMap.AddSegment(new ImageSegment(
                addrStart.Selector.Value.ToString("X4"),
                Address.SegPtr(addrStart.Selector.Value, 0),
                imgLoaded,
                AccessMode.ReadWriteExecute));
            DumpSegments(imageMap);

            var ep = CreateEntryPointSymbol(addrLoad, addrStart, addrStackTop);
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
			catch (Exception ex)
			{
                var listener = Services.RequireService<DecompilerEventListener>();
                listener.Error(
                    new NullCodeLocation(Filename),
                    ex,
                    "Detected debug symbols but failed to load them.");
			}
            return results;
		}

        private ImageSymbol CreateEntryPointSymbol(Address addrLoad, Address addrStart, Address addrStackTop)
        {
            var state = arch.CreateProcessorState();
            state.SetInstructionPointer(addrStart);
            state.SetRegister(Registers.cs, Constant.UInt16(addrLoad.Selector.Value));
            state.SetRegister(Registers.ss, Constant.UInt16((ushort) addrStackTop.Selector.Value));
            state.SetRegister(Registers.sp, Constant.UInt16((ushort) addrStackTop.Offset));
            state.SetRegister(Registers.ds, Constant.UInt16(segPsp));
            var ep = ImageSymbol.Procedure(arch, addrStart, state: state);
            return ep;
        }

        private void LoadDebugSymbols(SortedList<Address, ImageSymbol> symbols, Address addrLoad)
        {
            //$REVIEW: this is hardwired. some kind of generic "sniffing" mechanism needs to be implemented.
            // We don't want to load every registered symbol provider, though. Perhaps
            // load symbols in a separate AppDomain, marshal all the symbols across,
            // then discard the appdomain?
            var borsymLdr = new Borland.SymbolLoader(arch, ExeLoader, RawImage, addrLoad);
            if (borsymLdr.LoadDebugHeader())
            {
                var syms = borsymLdr.LoadSymbols();
                foreach (var sym in syms)
                {
                    symbols[sym.Key] = sym.Value;
                }
            }
            var codeviewLdr = new CodeView.CodeViewLoader(arch, RawImage, addrLoad);
            if (codeviewLdr.LoadCodeViewInfo())
            {
                var syms = codeviewLdr.LoadSymbols();
                foreach (var sym in syms)
                {
                    symbols[sym.Key] = sym.Value;
        }
            }
        }

        [Conditional("DEBUG")]
        public void DumpSegments(SegmentMap segmentMap)
        {
  			Debug.Print("Found {0} segments: ", segmentMap.Segments.Values.Count);
            foreach (var seg in segmentMap.Segments.Values)
            {
                Debug.Print("  {0} {1} size:{2}", seg.Name, seg.Address, seg.Size);
            }
        }
	}
}
