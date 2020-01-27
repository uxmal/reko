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

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.MzExe
{
    /// <summary>
    /// Loades New Executable files.
    /// </summary>
    /// <remarks>
    /// https://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm
    /// </remarks>
    public class NeImageLoader : ImageLoader
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(NeImageLoader), "NE Image loader tracing", "Verbose");

        // Relocation address types
        [Flags]
        public enum NE_RADDR : byte
        {
            LOWBYTE = 0,
            SELECTOR = 2,
            POINTER32 = 3,
            OFFSET16 = 5,
            POINTER48 = 11,
            OFFSET32 = 13,
        }

        // Relocation types
        [Flags]
        public enum NE_RELTYPE : byte
        {
            INTERNAL = 0,
            ORDINAL = 1,
            NAME = 2,
            OSFIXUP = 3,
            ADDITIVE = 4,
        }

        // Segment table flags
        const ushort NE_STFLAGS_DATA = 0x0001;
        const ushort NE_STFLAGS_REALMODE = 0x0004;
        const ushort NE_STFLAGS_ITERATED = 0x0008;
        const ushort NE_STFLAGS_MOVEABLE = 0x0010;
        const ushort NE_STFLAGS_SHAREABLE = 0x0020;
        const ushort NE_STFLAGS_PRELOAD = 0x0040;
        const ushort NE_STFLAGS_EXECUTE = 0x00080;
        const ushort NE_STFLAGS_RELOCATIONS = 0x0100;
        const ushort NE_STFLAGS_DEBUG_INFO = 0x0200;
        const ushort NE_STFLAGS_DPL = 0x0C00;
        const ushort NE_STFLAGS_DISCARDABLE = 0x1000;
        const ushort NE_STFLAGS_DISCARD_PRIORITY = 0xE000;

        // Resource types
        const ushort NE_RSCTYPE_CURSOR            =0x8001;
        const ushort NE_RSCTYPE_BITMAP            =0x8002;
        const ushort NE_RSCTYPE_ICON              =0x8003;
        const ushort NE_RSCTYPE_MENU              =0x8004;
        const ushort NE_RSCTYPE_DIALOG            =0x8005;
        const ushort NE_RSCTYPE_STRING            =0x8006;
        const ushort NE_RSCTYPE_FONTDIR           =0x8007;
        const ushort NE_RSCTYPE_FONT              =0x8008;
        const ushort NE_RSCTYPE_ACCELERATOR       =0x8009;
        const ushort NE_RSCTYPE_RCDATA            =0x800a;
        const ushort NE_RSCTYPE_GROUP_CURSOR      =0x800c;
        const ushort NE_RSCTYPE_GROUP_ICON        =0x800e;
        const ushort NE_RSCTYPE_SCALABLE_FONTPATH = 0x80cc;

        // Lowest 3 bits of a selector to the Local Descriptor Table requesting
        // ring-3 privilege level.
        const ushort LDT_RPL3 = 0x7;

        private readonly SortedList<Address, ImageSymbol> imageSymbols;
        private readonly Dictionary<uint, Tuple<Address, ImportReference>> importStubs;
        private readonly IDiagnosticsService diags;
        private readonly uint lfaNew;
        private MemoryArea mem;
        private SegmentMap segmentMap;
        private List<string> moduleNames;
        private NeSegment[] segments;
        private ushort cbFileAlignmentShift;
        private ushort cSeg;
        private ushort offImportedNamesTable;
        private ushort offEntryTable;
        private ushort offResidentNameTable;
        private uint offNonResidentNameTable;
        private ushort offRsrcTable;
        private Address addrImportStubs;
        private IProcessorArchitecture arch;
        private IPlatform platform;
        private Address addrEntry;
        private List<ImageSymbol> entryPoints;

        public NeImageLoader(IServiceProvider services, string filename, byte[] rawBytes, uint e_lfanew)
            : base(services, filename, rawBytes)
        {
            this.diags = Services.RequireService<IDiagnosticsService>();
            this.lfaNew = e_lfanew;
            this.importStubs = new Dictionary<uint, Tuple<Address, ImportReference>>();
            this.imageSymbols = new SortedList<Address, ImageSymbol>();
        }

        public override Address PreferredBaseAddress
        {
            get { return Address.ProtectedSegPtr(0xF, 0); }
            set
            {
                throw new NotImplementedException();
            }
        }

        private bool LoadNeHeader(EndianImageReader rdr)
        {
            if (!rdr.TryReadLeUInt16(out ushort magic) || magic != 0x454E)
                throw new BadImageFormatException("Not a valid NE header.");
            if (!rdr.TryReadLeUInt16(out ushort linker))
                return false;
            if (!rdr.TryReadLeUInt16(out offEntryTable))
                return false;
            if (!rdr.TryReadLeUInt16(out ushort cbEntryTable))
                return false;
            if (!rdr.TryReadLeUInt32(out uint crc))
                return false;
            if (!rdr.TryReadByte(out byte bProgramFlags))
                return false;
            if (!rdr.TryReadByte(out byte bAppFlags))
                return false;
            if (!rdr.TryReadUInt16(out ushort iSegAutoData))
                return false;
            if (!rdr.TryReadUInt16(out ushort cbHeapSize))
                return false;
            if (!rdr.TryReadUInt16(out ushort cbStackSize))
                return false;
            if (!rdr.TryReadUInt16(out ushort ip) || !rdr.TryReadUInt16(out ushort cs))
                return false;
            if (!rdr.TryReadUInt16(out ushort sp) || !rdr.TryReadUInt16(out ushort ss))
                return false;
            if (!rdr.TryReadUInt16(out cSeg))
                return false;
            if (!rdr.TryReadUInt16(out ushort cModules))
                return false;
            if (!rdr.TryReadUInt16(out ushort cbNonResidentNames))
                return false;
            if (!rdr.TryReadUInt16(out ushort offSegTable))
                return false;
            if (!rdr.TryReadUInt16(out offRsrcTable))
                return false;
            if (!rdr.TryReadUInt16(out offResidentNameTable))
                return false;
            if (!rdr.TryReadUInt16(out ushort offModuleReferenceTable))
                return false;
            if (!rdr.TryReadUInt16(out offImportedNamesTable))
                return false;
            if (!rdr.TryReadUInt32(out this.offNonResidentNameTable))
                return false;
            if (!rdr.TryReadUInt16(out ushort cMoveableEntryPoints))
                return false;
            if (!rdr.TryReadUInt16(out cbFileAlignmentShift))
                return false;
            if (!rdr.TryReadUInt16(out ushort cResourceTableEntries))
                return false;
            if (!rdr.TryReadByte(out byte bTargetOs))
                return false;
            if (!rdr.TryReadByte(out byte bOsExeFlags))
                return false;
            if (!rdr.TryReadUInt16(out ushort offGanglands))
                return false;
            if (!rdr.TryReadUInt16(out ushort cbGanglands))
                return false;
            if (!rdr.TryReadUInt16(out ushort cbMinCodeSwapArea))
                return false;
            if (!rdr.TryReadUInt16(out ushort wWindowsVersion))
                return false;

            LoadModuleTable(this.lfaNew + offModuleReferenceTable, cModules);
            this.segments = ReadSegmentTable(this.lfaNew + offSegTable, cSeg);
            var names = new Dictionary<int, string>();
            LoadEntryNames(this.lfaNew + this.offResidentNameTable, names);
            LoadNonresidentNames(this.offNonResidentNameTable, names);
            this.entryPoints = LoadEntryPoints(this.lfaNew + this.offEntryTable, names);
            LoadSegments(segments);
            this.addrEntry = segments[cs - 1].Address + ip;
            return true;
        }

        /// <summary>
        /// Reads in the NE image resources.
        /// </summary>
        //$REFACTOR: resource loading seems to want to belong in a separate file.
        private void LoadResources(List<ProgramResource> resources)
        {
            var rsrcTable = new LeImageReader(RawImage, this.lfaNew + offRsrcTable);
            var rdr = rsrcTable.Clone();
            ushort size_shift = rdr.ReadLeUInt16();
            ProgramResourceGroup bitmaps = null;
            ProgramResourceGroup iconGroups = null;
            ProgramResourceGroup icons = null;
            var iconIds = new Dictionary<ushort, ProgramResourceInstance>();
            ushort rsrcType = rdr.ReadLeUInt16();
            while (rsrcType != 0) 
            {
                var resGrp = new ProgramResourceGroup { Name = GetResourceType(rsrcType) };
                if (rsrcType == NE_RSCTYPE_GROUP_ICON)
                    iconGroups = resGrp;
                else if (rsrcType == NE_RSCTYPE_ICON)
                    icons = resGrp;
                else if (rsrcType == NE_RSCTYPE_BITMAP)
                    bitmaps = resGrp;

                ushort typeCount = rdr.ReadLeUInt16();
                uint resLoader = rdr.ReadLeUInt32();
                for (int count = typeCount; count > 0; --count)
                {
                    ushort nameOffset = rdr.ReadLeUInt16();
                    ushort nameLength = rdr.ReadLeUInt16();
                    ushort nameFlags = rdr.ReadLeUInt16();
                    ushort nameId = rdr.ReadLeUInt16();
                    ushort nameHandle = rdr.ReadLeUInt16();
                    ushort nameUsage = rdr.ReadLeUInt16();

                    string resname;
                    if ((nameId & 0x8000) != 0)
                    {
                        resname = (nameId & ~0x8000).ToString();
                    }
                    else
                    {
                        resname = ReadByteLengthString(rsrcTable, nameId);
                    }

                    var offset = (uint)nameOffset << size_shift;
                    var rdrRsrc = new LeImageReader(base.RawImage, offset);
                    var rsrc = rdrRsrc.ReadBytes((uint)nameLength << size_shift);

                    var rsrcInstance = new ProgramResourceInstance
                    {
                        Name = resname,
                        Type = "Win16_" + resGrp.Name,
                        Bytes = rsrc,
                    };
                    resGrp.Resources.Add(rsrcInstance);

                    if (rsrcType == NE_RSCTYPE_ICON)
                        iconIds[(ushort)(nameId & ~0x8000)] = rsrcInstance;
                }
                resources.Add(resGrp);

                rsrcType = rdr.ReadLeUInt16();
            }

            PostProcessIcons(iconGroups, icons, iconIds, resources);
            PostProcessBitmaps(bitmaps);
        }

        /// <summary>
        /// Build icons out of the icon groups + icons.
        /// </summary>
        /// <param name="iconGroups"></param>
        /// <param name="icons"></param>
        private void PostProcessIcons(
            ProgramResourceGroup iconGroups, 
            ProgramResourceGroup icons, 
            Dictionary<ushort, ProgramResourceInstance> iconIds,
            List<ProgramResource> resources)
        {
            if (icons == null)
            {
                if (iconGroups != null)
                    resources.Remove(iconGroups);
                return;
            }
            if (iconGroups == null)
            {
                if (icons == null)
                    resources.Remove(icons);
                return;
            }

            foreach (ProgramResourceInstance iconGroup in iconGroups.Resources)
            {
                var r = new BinaryReader(new MemoryStream(iconGroup.Bytes));
                var stm = new MemoryStream();
                var w = new BinaryWriter(stm);

                // Copy the group header
                w.Write(r.ReadInt16());
                w.Write(r.ReadInt16());
                short dirEntries = r.ReadInt16();
                w.Write(dirEntries);

                var icIds = new List<Tuple<ushort, int>>();
                for (int i = 0; i < dirEntries; ++i)
                {
                    w.Write(r.ReadInt32());
                    w.Write(r.ReadInt32());
                    w.Write(r.ReadInt32());
                    var iconId = r.ReadUInt16();
                    w.Flush();
                    icIds.Add(Tuple.Create(iconId, (int)stm.Position));
                    w.Write(0);
                }
                foreach (var id in icIds)
                {
                    var icon = iconIds[id.Item1];
                    var icOffset = (int) w.Seek(0, SeekOrigin.Current);
                    w.Seek(id.Item2, SeekOrigin.Begin);
                    w.Write(icOffset);
                    w.Seek(icOffset, SeekOrigin.Begin);
                    w.Write(icon.Bytes);
                }
                iconGroup.Bytes = stm.ToArray();
                iconGroup.Type = "Win16_ICON";
            }
            iconGroups.Name = "ICON";
            resources.Remove(icons);
        }

        void PostProcessBitmaps(ProgramResourceGroup bitmaps)
        {
            if (bitmaps == null)
                return;
            foreach (ProgramResourceInstance bitmap in bitmaps.Resources)
            {
                var stm = new MemoryStream();
                var bw = new BinaryWriter(stm);

                bw.Write('B');
                bw.Write('M');
                bw.Write(14 + bitmap.Bytes.Length);
                bw.Write(0);
                bw.Write(14);
                bw.Write(bitmap.Bytes, 0, bitmap.Bytes.Length);
                bw.Flush();
                bitmap.Bytes = stm.ToArray();
            }
        }

        string ReadByteLengthString(EndianImageReader rdr, int offset)
        {
            var clone = rdr.Clone();
            clone.Offset = clone.Offset + offset;
            var len = clone.ReadByte();
            var abStr = clone.ReadBytes(len);
            return Encoding.ASCII.GetString(abStr);
        }

        private string GetResourceName(ushort id)
        {
            switch (id)
            {
            case NE_RSCTYPE_CURSOR: return "CURSOR";
            case NE_RSCTYPE_BITMAP: return "BITMAP";
            case NE_RSCTYPE_ICON: return "ICON";
            case NE_RSCTYPE_MENU: return "MENU";
            case NE_RSCTYPE_DIALOG: return "DIALOG";
            case NE_RSCTYPE_STRING: return "STRING";
            case NE_RSCTYPE_FONTDIR: return "FONTDIR";
            case NE_RSCTYPE_FONT: return "FONT";
            case NE_RSCTYPE_ACCELERATOR: return "ACCELERATOR";
            case NE_RSCTYPE_RCDATA: return "RCDATA";
            case NE_RSCTYPE_GROUP_CURSOR: return "CURSOR_GROUP";
            case NE_RSCTYPE_GROUP_ICON: return "ICON_GROUP";
            default: return id.ToString("X4");
            }
        }

        private string GetResourceType(ushort id)
        {
            switch (id)
            {
            case NE_RSCTYPE_BITMAP: return "Windows.BMP";
            case NE_RSCTYPE_ICON: return "Windows.ICO";
            default: return id.ToString("X4");
            }
        }

        public override Program Load(Address addrLoad)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            this.arch = cfgSvc.GetArchitecture("x86-protected-16");
            this.platform = cfgSvc.GetEnvironment("win16").Load(Services, arch);
            var rdr = new LeImageReader(RawImage, this.lfaNew);
            if (!LoadNeHeader(rdr))
                throw new BadImageFormatException("Unable to read NE header.");

            var program = new Program(
                this.segmentMap,
                arch,
                platform);
            program.Resources.Name = "NE resources";
            LoadResources(program.Resources.Resources);
            foreach (var impRef in this.importStubs.Values)
            {
                program.ImportReferences.Add(impRef.Item1, impRef.Item2);
            }
            return program;
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            entryPoints.Add(ImageSymbol.Procedure(program.Architecture, addrEntry));
            return new RelocationResults(
                entryPoints,
                imageSymbols);
        }

        private Dictionary<int,string> LoadNonresidentNames(uint offNonResidentNameTable, Dictionary<int,string> dict)
        {
            var rdr = new LeImageReader(RawImage, offNonResidentNameTable);
            while (rdr.TryReadByte(out byte nameLen))
            {
                if (nameLen == 0)
                    break;
                var abName = rdr.ReadBytes(nameLen);
                var name = Encoding.ASCII.GetString(abName);
                if (!rdr.TryReadLeInt16(out short ordinal))
                    break;
                dict[ordinal] = name;
            }
            return dict;
        }

        public Dictionary<int, string> LoadEntryNames(uint offset, Dictionary<int, string> dict)
        {
            var rdr = new LeImageReader(RawImage, offset);
            for (;;)
            {
                var cChar = rdr.ReadByte();
                if (cChar == 0)
                    break;
                var abName = rdr.ReadBytes(cChar);
                var name = Encoding.ASCII.GetString(abName);
                int ordinal = rdr.ReadLeInt16();
                dict[ordinal] = name;
            }
            return dict;
        }

        public List<ImageSymbol> LoadEntryPoints(uint offEntryTable, Dictionary<int, string> names)
        {
            return LoadEntryPoints(offEntryTable, this.segments, names, this.arch);
        }

        public List<ImageSymbol> LoadEntryPoints(
            uint offEntryTable, 
            NeSegment [] segments, 
            Dictionary<int, string> names,
            IProcessorArchitecture arch)
        {
            DebugEx.Inform(trace, "== Loading entry points from offset {0:X}", offEntryTable);
            var rdr = new LeImageReader(RawImage, offEntryTable);

            var entries = new List<ImageSymbol>();
            int bundleOrdinal = 1;
            int nextbundleOrdinal = 1;
            for (; ; )
            {
                var cBundleEntries = rdr.ReadByte();
                if (cBundleEntries == 0)
                    break;
                nextbundleOrdinal = bundleOrdinal + cBundleEntries;
                var segNum = rdr.ReadByte();
                if (segNum != 0)
                {
                    // If segNum had been 0, it would have 
                    // meant that all we want to do is allocate 
                    // (skip) some ordinal numbers. Since it wasn't 0,
                    // we proceed to generate entry points.
                    for (int i = 0; i < cBundleEntries; ++i)
                    {
                        byte flags = rdr.ReadByte();
                        (byte iSeg, ushort offset) entry;
                        if (segNum == 0xFF)
                        {
                            entry = ReadMovableSegmentEntry(rdr);
                        }
                        else
                        {
                            entry = ReadFixedSegmentEntry(rdr, segNum);
                        }
                        var seg = segments[entry.iSeg - 1];
                        var addr = seg.Address + entry.offset;
                        var ep = ImageSymbol.Procedure(arch, addr);
                        if (names.TryGetValue(bundleOrdinal + i, out string name))
                        {
                            ep.Name = name;
                        }
                        ep.Type = SymbolType.Procedure;
                        ep.ProcessorState = arch.CreateProcessorState();
                        imageSymbols[ep.Address] = ep;
                        entries.Add(ep);
                        DebugEx.Verbose(trace, "   {0:X2} {1} {2} - {3}", segNum, ep.Address, ep.Name, bundleOrdinal + i);
                    }
                }
                bundleOrdinal = nextbundleOrdinal;
            }
            return entries; 
        }

        private (byte, ushort) ReadFixedSegmentEntry(LeImageReader rdr, byte iSeg)
        {
            var offset = rdr.ReadUInt16();
            return (iSeg, offset);
        }

        private (byte, ushort) ReadMovableSegmentEntry(LeImageReader rdr)
        {
            var int3f = rdr.ReadLeUInt16();
            var iSeg = rdr.ReadByte();
            var offset = rdr.ReadLeUInt16();
            return (iSeg, offset);
        }


        public class NeSegment
        {
            public ushort DataOffset;
            public ushort DataLength;
            public ushort Flags;
            public ushort Alloc;

            public uint LinearAddress;
            public Address Address;
        }

        void LoadModuleTable(uint offset, int cModules)
        {
            var rdr = new LeImageReader(RawImage, offset);
            this.moduleNames = new List<string>();
            for (int i = 0; i < cModules; ++i)
            {
                uint nameOffset = rdr.ReadLeUInt16();
                if (nameOffset == 0)
                    break;
                nameOffset += lfaNew + this.offImportedNamesTable;
                var rdrName = new LeImageReader(RawImage, nameOffset);
                byte length = rdrName.ReadByte();
                byte[] abModuleName = rdrName.ReadBytes(length);
                var moduleName = Encoding.ASCII.GetString(abModuleName);
                moduleNames.Add(moduleName);
            }
        }

        private void LoadSegments(NeSegment[] segments)
        {
            var segFirst = segments[0];
            var segLast = segments[segments.Length - 1];
            this.segmentMap = new SegmentMap(segFirst.Address);
            foreach (var segment in segments)
            {
                this.mem = new MemoryArea(
                    segment.Address, 
                    new byte[Math.Max(segment.Alloc, segment.DataLength)]);
                LoadSegment(segment, mem, segmentMap);
            }
        }

        private NeSegment[] ReadSegmentTable(uint offset, int cSeg)
        {
            var segs = new List<NeSegment>(cSeg);
            var rdr = new LeImageReader(RawImage, offset);
            uint linAddress = 0x2000;
            Debug.Print("== Segment table ");
            Debug.Print("  Address:Offs Len  Flag Alloc");
            for (int iSeg = 0; iSeg < cSeg; ++iSeg)
            {
                var seg = new NeSegment
                {
                    DataOffset = rdr.ReadLeUInt16(),
                    DataLength = rdr.ReadLeUInt16(),
                    Flags = rdr.ReadLeUInt16(),
                    Alloc = rdr.ReadLeUInt16()
                };
                uint cbSegmentPage = Math.Max(seg.Alloc, seg.DataLength);
                // We allocate segments on 4 kb boundaries for convenience,
                // but protected mode code must never assume addresses are
                // linear.
                // Align to 4kb boundary.
                cbSegmentPage = (cbSegmentPage + 0xFFFu) & ~0xFFFu;
                seg.LinearAddress = linAddress;
                seg.Address = Address.ProtectedSegPtr((ushort)((linAddress >> 9) | LDT_RPL3), 0);
                Debug.Print("{0}:{1:X4} {2:X4} {3:X4} {4:X4}",
                    seg.Address,
                    seg.DataOffset,
                    seg.DataLength,
                    seg.Flags,
                    seg.Alloc);
                segs.Add(seg);
                linAddress += cbSegmentPage;
            }

            // Generate pseudo-segment for imports with in a segment that isn't
            // one of the "real" segments loaded above.
            addrImportStubs = Address.ProtectedSegPtr((ushort)((linAddress >> 9) | LDT_RPL3), 0);

            return segs.ToArray();
        }

        private bool LoadSegment(NeSegment neSeg, MemoryArea mem, SegmentMap imageMap)
        {
            Array.Copy(
                RawImage,
                (uint) neSeg.DataOffset << this.cbFileAlignmentShift,
                mem.Bytes,
                neSeg.LinearAddress - (int)mem.BaseAddress.ToLinear(),
                neSeg.DataLength);

            AccessMode access =
                (neSeg.Flags & 1) != 0
                    ? AccessMode.ReadWrite
                    : AccessMode.ReadExecute;
            var seg = this.segmentMap.AddSegment(
                mem,
                neSeg.Address.Selector.Value.ToString("X4"),
                access);
            var stg = new TemporaryStorage(
                string.Format("seg{0:X4}", seg.Address.Selector.Value),
                0,
                PrimitiveType.SegmentSelector);
            seg.Identifier = new Identifier(seg.Name, stg.DataType, stg);
            if ((neSeg.Flags & NE_STFLAGS_RELOCATIONS) == 0)
                return true;
            var rdr = new LeImageReader(
                RawImage,
                neSeg.DataLength + ((uint)neSeg.DataOffset << this.cbFileAlignmentShift));
            int count = rdr.ReadLeInt16();
            return ApplyRelocations(rdr, count, neSeg);
        }

#if NE_
        static void NE_FixupSegmentPrologs(NE_MODULE* pModule, WORD segnum)
        {
            SEGTABLEENTRY* pSegTable = NE_SEG_TABLE(pModule);
            ET_BUNDLE* bundle;
            ET_ENTRY* entry;
            WORD dgroup, num_entries, sel = SEL(pSegTable[segnum - 1].hSeg);
            BYT* pSeg, *pFunc;


            if (pSegTable[segnum - 1].flags & NE_SEGFLAGS_DATA)
            {
                pSegTable[segnum - 1].flags |= NE_SEGFLAGS_LOADED;
                return;
            }

            if (!pModule->ne_autodata) return;


            if (!pSegTable[pModule.ne_autodata - 1].hSeg) return;
            dgroup = SEL(pSegTable[pModule.ne_autodata - 1].hSeg);


            pSeg = MapSL(MAKESEGPTR(sel, 0));


            bundle = (ET_BUNDLE*)((BYTE*)pModule + pModule.ne_enttab);


            do {
                Debug.Print("num_entries: {0}, bundle: {1}, next: {2:X4}, pSeg: {3}", bundle.last - bundle.first, bundle, bundle.next, pSeg);
                if (!(num_entries = bundle.last - bundle.first))
                    return;
                entry = (ET_ENTRY*)((BYTE*)bundle + 6);
                while (num_entries--)
                {
                    /*TRACE("entry: %p, entry.segnum: %d, entry.offs: %04x\n", entry, entry.segnum, entry.offs);*/
                    if (entry.segnum == segnum)
                    {
                        pFunc = pSeg + entry.offs;
                        TRACE("pFunc: %p, *(DWORD *)pFunc: %08x, num_entries: %d\n", pFunc, *(DWORD*)pFunc, num_entries);
                        if (*(pFunc + 2) == 0x90)
                        {
                            if (*(WORD*)pFunc == 0x581e) /* push ds, pop ax */
                            {
                                TRACE("patch %04x:%04x -> mov ax, ds\n", sel, entry.offs);
                                *(WORD*)pFunc = 0xd88c; /* mov ax, ds */
                            }


                            if (*(WORD*)pFunc == 0xd88c)
                            {
                                if ((entry.flags & 2)) /* public data ? */
                                {
                                    TRACE("patch %04x:%04x -> mov ax, dgroup [%04x]\n", sel, entry.offs, dgroup);
                                    *pFunc = 0xb8; /* mov ax, */
                                    *(WORD*)(pFunc + 1) = dgroup;
                                }
                                else if ((pModule.ne_flags & NE_FFLAGS_MULTIPLEDATA)
                                         && (entry.flags & 1)) /* exported ? */
                                {
                                    TRACE("patch %04x:%04x -> nop, nop\n", sel, entry.offs);
                                    *(WORD*)pFunc = 0x9090; /* nop, nop */
                                }
                            }
                        }
                    }
                    entry++;
                }
            } while ((bundle.next) && (bundle = ((ET_BUNDLE*)((BYTE*)pModule + bundle.next))));
        }
#endif

        public class NeRelocationEntry
        {
            public NE_RADDR address_type;       // Relocation address type
            public NE_RELTYPE relocation_type;  // Relocation type
            public ushort offset;               // Offset in segment to fixup
            public ushort target1;              // Target specification
            public ushort target2;              // Target specification
        }

        /// <summary>
        /// Apply relocations to a segment.
        /// </summary>
        private bool ApplyRelocations(EndianImageReader rdr, int cRelocations, NeSegment seg)
        {
            Address address = null;
            NeRelocationEntry rep = null;
            Debug.Print("== Relocating segment {0}", seg.Address);
            for (int i = 0; i < cRelocations; i++)
            {
                rep = new NeRelocationEntry
                {
                    address_type = (NE_RADDR) rdr.ReadByte(),
                    relocation_type = (NE_RELTYPE)rdr.ReadByte(),
                    offset = rdr.ReadLeUInt16(),
                    target1 = rdr.ReadLeUInt16(),
                    target2 = rdr.ReadLeUInt16(),
                };
                Debug.Print("  {0}", WriteRelocationEntry(rep));

                // Get the target address corresponding to this entry.

                // If additive, there is no target chain list. Instead, add source
                //  and target.
                bool additive = (rep.relocation_type & NE_RELTYPE.ADDITIVE) != 0;
                Tuple<Address, ImportReference> impRef;
                uint lp;
                string module = "";
                switch (rep.relocation_type & (NE_RELTYPE)3)
                {
                case NE_RELTYPE.ORDINAL:
                    module = moduleNames[rep.target1 - 1];
                    // Synthesize an import 
                    lp = ((uint)rep.target1 << 16) | rep.target2;
                    if (importStubs.TryGetValue(lp, out impRef))
                    {
                        address = impRef.Item1;
                    }
                    else
                    {
                        address = addrImportStubs;
                        importStubs.Add(lp, new Tuple<Address, ImportReference>(
                            address,
                            new OrdinalImportReference(address, module, rep.target2, SymbolType.ExternalProcedure)));
                        addrImportStubs += 8;
                    }
                    break;

                case NE_RELTYPE.NAME:
                    module = moduleNames[rep.target1 - 1];
                    uint offName = lfaNew + this.offImportedNamesTable + rep.target2;
                    var nameRdr = new LeImageReader(RawImage, offName);
                    byte fnNameLength = nameRdr.ReadByte();
                    var abFnName = nameRdr.ReadBytes(fnNameLength);
                    // Synthesize the import.
                    lp = ((uint)rep.target1 << 16) | rep.target2;
                    if (importStubs.TryGetValue(lp, out impRef))
                    {
                        address = impRef.Item1;
                    }
                    else
                    {
                        address = addrImportStubs;
                        string fnName = Encoding.ASCII.GetString(abFnName);
                        importStubs.Add(lp, new Tuple<Address, ImportReference>(
                            address,
                            new NamedImportReference(address, module, fnName, SymbolType.ExternalProcedure)));
                    }
                    break;
                case NE_RELTYPE.INTERNAL:
                    if ((rep.target1 & 0xff) == 0xff)
                    {
                        address = this.entryPoints[rep.target2 - 1].Address;
                    }
                    else
                    {
                        address = segments[rep.target1 - 1].Address + rep.target2;
                    }
                    Debug.Print("    {0}: {1:X4}:{2:X4} {3}",
                          i + 1,
                          address.Selector.Value, 
                          address.Offset,
                          "");
                    break;
                case NE_RELTYPE.OSFIXUP:
                    /* Relocation type 7:
                     *
                     *    These appear to be used as fixups for the Windows
                     * floating point emulator.  Let's just ignore them and
                     * try to use the hardware floating point.  Linux should
                     * successfully emulate the coprocessor if it doesn't
                     * exist.
                     */
                    /*
                   TRACE("%d: TYPE %d, OFFSET %04x, TARGET %04x %04x %s\n",
                         i + 1, rep->relocation_type, rep->offset,
                         rep->target1, rep->target2,
                         NE_GetRelocAddrName( rep->address_type, additive ) );
                   */
                    continue;
                }
                ushort offset = rep.offset;

                // Apparently, high bit of address_type is sometimes set;
                // we ignore it for now.
                if (rep.address_type > NE_RADDR.OFFSET32)
                {
                    diags.Error(
                        string.Format(
                            "Module {0}: unknown relocation address type {1:X2}. Please report",
                            module, rep.address_type));
                    return false;
                }

                if (additive)
                {
                    var sp = seg.Address + offset;
                    Debug.Print("    {0} (contains: {1:X4})", sp, mem.ReadLeUInt16(sp));
                    byte b;
                    ushort w;
                    switch (rep.address_type & (NE_RADDR) 0x7f)
                    {
                    case NE_RADDR.LOWBYTE:
                        b = mem.ReadByte(sp);
                        mem.WriteByte(sp, (byte)(b + address.Offset));
                        break;
                    case NE_RADDR.OFFSET16:
                        w = mem.ReadLeUInt16(sp);
                        mem.WriteLeUInt16(sp, (ushort)(w + address.Offset));
                        break;
                    case NE_RADDR.POINTER32:
                        w = mem.ReadLeUInt16(sp);
                        mem.WriteLeUInt16(sp, (ushort)(w + address.Offset));
                        mem.WriteLeUInt16(sp + 2, address.Selector.Value);
                        break;
                    case NE_RADDR.SELECTOR:
                        // Borland creates additive records with offset zero. Strange, but OK.
                        w = mem.ReadLeUInt16(sp);
                        if (w != 0)
                            diags.Error(string.Format("Additive selector to {0:X4}. Please report.", w));
                        else
                            mem.WriteLeUInt16(sp, address.Selector.Value);
                        break;
                    default:
                        goto unknown;
                    }
                }
                else
                {
                    // Non-additive fixup.
                    do
                    {
                        var sp = seg.Address + offset;
                        ushort next_offset = mem.ReadLeUInt16(sp);
                        Debug.Print("    {0} (contains: {1:X4})", sp, next_offset);
                        switch (rep.address_type & (NE_RADDR) 0x7f)
                        {
                        case NE_RADDR.LOWBYTE:
                            mem.WriteByte(sp, (byte)address.Offset);
                            break;
                        case NE_RADDR.OFFSET16:
                            mem.WriteLeUInt16(sp, (ushort)address.Offset);
                            break;
                        case NE_RADDR.POINTER32:
                            mem.WriteLeUInt16(sp, (ushort)address.Offset);
                            mem.WriteLeUInt16(sp + 2, address.Selector.Value);
                            break;
                        case NE_RADDR.SELECTOR:
                            mem.WriteLeUInt16(sp, address.Selector.Value);
                            break;
                        default:
                            goto unknown;
                        }
                        if (next_offset == offset) break;  // avoid infinite loop 
                        if (next_offset >= seg.Alloc)
                            break;
                        offset = next_offset;
                    } while (offset != 0xffff);
                }
            }
            return true;

            unknown:
            var svc = Services.RequireService<IDiagnosticsService>();
            svc.Warn("{0}: unknown ADDR TYPE {1},  " +
                "TYPE {2},  OFFSET {3:X4},  TARGET {4:X4} {5:X4}",
                seg.Address.Selector, rep.address_type, rep.relocation_type,
                rep.offset, rep.target1, rep.target2);
            return false;
        }

        private string WriteRelocationEntry(NeRelocationEntry re)
        {
            switch (re.relocation_type & (NE_RELTYPE)3)
            {
            //case NE_RELTYPE.NAME:
            case NE_RELTYPE.ORDINAL:
                return string.Format("{0,8} {1,8} {2:X4} {3:X4} {4:X4}",
                    re.address_type,
                    re.relocation_type,
                    re.offset,
                    moduleNames[re.target1 - 1],
                    re.target2);
            default:
                return string.Format("{0,8} {1,8} {2:X4} {3:X4} {4:X4}",
                    re.address_type,
                    re.relocation_type,
                    re.offset,
                    re.target1,
                    re.target2);
            }
        }
    }
}
