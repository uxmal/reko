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
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Environments.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Reko.ImageLoaders.MzExe.Ne;
using Reko.Core.Loading;
using Reko.Core.Diagnostics;

namespace Reko.ImageLoaders.MzExe
{
    /// <summary>
    /// Loades New Executable files.
    /// </summary>
    /// <remarks>
    /// https://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm
    /// </remarks>
    public class NeImageLoader : ProgramImageLoader
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(NeImageLoader), "NE Image loader tracing")
        {
            Level = TraceLevel.Warning
        };

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

        private enum NE_TARGETOS : byte
        {
            Unknown = 0,
            Os2 = 1,
            Windows = 2,
            EuropeanDos = 3,
            Windows386 = 4,
            Borland = 5
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

        // Lowest 3 bits of a selector to the Local Descriptor Table requesting
        // ring-3 privilege level.
        const ushort LDT_RPL3 = 0x7;
        const ushort RPL_MASK = 0x3;        // Requested privilege level
        const ushort TI_BIT = 0x04;         // TI = Table indicator.

        private readonly SortedList<Address, ImageSymbol> imageSymbols;
        private readonly Dictionary<uint, (Address, ImportReference)> importStubs;
        private readonly IEventListener listener;
        private readonly uint lfaNew;
        private ByteMemoryArea mem;
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
        private Address? addrEntry;
        private List<ImageSymbol?> entryPoints;
        private NE_TARGETOS bTargetOs;
        private ushort cResourceTableEntries;

        public NeImageLoader(IServiceProvider services, ImageLocation imageLocation, byte[] rawBytes, uint e_lfanew)
            : base(services, imageLocation, rawBytes)
        {
            this.listener = Services.RequireService<IEventListener>();
            this.lfaNew = e_lfanew;
            this.importStubs = new Dictionary<uint, (Address, ImportReference)>();
            this.imageSymbols = new SortedList<Address, ImageSymbol>();
            this.arch = null!;
            this.entryPoints = null!;
            this.mem = null!;
            this.moduleNames = null!;
            this.platform = null!;
            this.segmentMap = null!;
            this.segments = null!;
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
            if (!rdr.TryReadUInt16(out cResourceTableEntries))
                return false;
            if (!rdr.TryReadByte(out byte targetOs))
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

            bTargetOs = (NE_TARGETOS) targetOs;

            LoadModuleTable(this.lfaNew + offModuleReferenceTable, cModules);
            this.segments = ReadSegmentTable(this.lfaNew + offSegTable, cSeg);
            var names = new Dictionary<int, string>();
            LoadEntryNames(this.lfaNew + this.offResidentNameTable, names);
            LoadNonresidentNames(this.offNonResidentNameTable, names);
            this.entryPoints = LoadEntryPoints(this.lfaNew + this.offEntryTable, names);
            LoadSegments(segments);

            // On OS/2 drivers CS is equal to the ring it runs on
            if (cs >= 1)
                this.addrEntry = segments[cs - 1].Address! + ip;
            return true;
        }

        public override Program LoadProgram(Address? addrLoad)
        {
            var cfgSvc = Services.RequireService<IConfigurationService>();
            this.arch = cfgSvc.GetArchitecture("x86-protected-16")!;
            var rdr = new LeImageReader(RawImage, this.lfaNew);
            if (!LoadNeHeader(rdr))
                throw new BadImageFormatException("Unable to read NE header.");

            switch (bTargetOs)
            {
                case NE_TARGETOS.Windows:
                case NE_TARGETOS.Windows386:
                    this.platform = cfgSvc.GetEnvironment("win16").Load(Services, arch);
                    break;
                case NE_TARGETOS.EuropeanDos:
                    this.platform = cfgSvc.GetEnvironment("ms-dos").Load(Services, arch);
                    break;
                case NE_TARGETOS.Os2:
                    this.platform = cfgSvc.GetEnvironment("os2-16").Load(Services, arch);
                    break;
                default:
                    // Not implemented
                    break;
            }

            var program = new Program(
                new ByteProgramMemory(this.segmentMap),
                arch,
                platform);

            var rsrcLoader = new ResourceLoader(RawImage, this.lfaNew + offRsrcTable, cResourceTableEntries);

            switch (bTargetOs)
            {
            case NE_TARGETOS.Windows:
            case NE_TARGETOS.Windows386:
                if (offRsrcTable != offResidentNameTable) // Some NE images contain no resources (indicated by offRsrcTable == offResidentNameTable)
                {
                    program.Resources.AddRange(rsrcLoader.LoadResources());
                }
                break;
            case NE_TARGETOS.Os2:
                program.Resources.AddRange(rsrcLoader.LoadOs2Resources(segments, cSeg, cbFileAlignmentShift));
                break;
            default:
                // Don't support resources
                break;
            }

            foreach (var impRef in this.importStubs.Values)
            {
                program.ImportReferences.Add(impRef.Item1, impRef.Item2);
            }

            if (addrEntry.HasValue)
            {
                entryPoints.Add(ImageSymbol.Procedure(program.Architecture, addrEntry.Value));
            }
            foreach (var ep in entryPoints.Where(e => e is not null && e.Type != SymbolType.Data))
            {
                program.EntryPoints.TryAdd(ep!.Address, ep);
            }
            foreach (var sym in imageSymbols)
            {
                program.ImageSymbols[sym.Key] = sym.Value;
            }
            return program;
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

        public List<ImageSymbol?> LoadEntryPoints(uint offEntryTable, Dictionary<int, string> names)
        {
            return LoadEntryPoints(offEntryTable, this.segments, names, this.arch);
        }

        public List<ImageSymbol?> LoadEntryPoints(
            uint offEntryTable, 
            NeSegment [] segments, 
            Dictionary<int, string> names,
            IProcessorArchitecture arch)
        {
            trace.Inform("== Loading entry points from Offset {0:X}", offEntryTable);
            var rdr = new LeImageReader(RawImage, offEntryTable);

            var entries = new List<ImageSymbol?>();
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
                        var addr = seg.Address! + entry.offset;

                        if (!names.TryGetValue(bundleOrdinal + i, out string? name))
                        {
                            name = null;
                        }
                        ImageSymbol ep;
                        if (seg.IsData)
                        {
                            ep = ImageSymbol.DataObject(arch, addr, name, new UnknownType());
                            ep.Ordinal = bundleOrdinal + i;
                        }
                        else
                        {
                            ep = ImageSymbol.Procedure(arch, addr, name);
                            ep.Ordinal = bundleOrdinal + i;
                            ep.Type = SymbolType.Procedure;
                            ep.ProcessorState = arch.CreateProcessorState();
                        }
                        entries.Add(ep);
                        imageSymbols[ep.Address] = ep;
                        trace.Verbose("   {0:X2} {1} {2} - {3}", segNum, ep.Address, ep.Name!, ep.Ordinal.HasValue ? ep.Ordinal.Value.ToString() : "");
                    }
                }
                else
                {
                    // We have unused entries, they have to occupy a space in the resulting entries table.
                    entries.AddRange(Enumerable.Range(0, cBundleEntries).Select(x => (ImageSymbol?) null));
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

            public bool IsData => (Flags & NE_STFLAGS_DATA) != 0;
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
            this.segmentMap = new SegmentMap(segFirst.Address!);
            foreach (var segment in segments)
            {
                this.mem = new ByteMemoryArea(
                    segment.Address!, 
                    new byte[Math.Max(segment.Alloc, segment.DataLength)]);
                LoadSegment(segment, mem);
            }
        }

        private NeSegment[] ReadSegmentTable(uint offset, int cSeg)
        {
            var segs = new List<NeSegment>(cSeg);
            var rdr = new LeImageReader(RawImage, offset);
            uint linAddress = 0x2000;
            trace.Verbose("== Segment table ");
            trace.Verbose("  Address:Offs Len  Flag Alloc");
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

                // Segment Bits 11-12 encode the RPL for the segment.
                var rpl = (ushort) (((seg.Flags >> 11) & RPL_MASK) | TI_BIT);

                // We allocate segments on 4 kb boundaries for convenience,
                // but protected mode code must never assume addresses are
                // linear.
                // Align to 4kb boundary.
                cbSegmentPage = (cbSegmentPage + 0xFFFu) & ~0xFFFu;
                seg.LinearAddress = linAddress;
                seg.Address = Address.ProtectedSegPtr((ushort)((linAddress >> 9) | rpl), 0);
                trace.Verbose("{0}:{1:X4} {2:X4} {3:X4} {4:X4}",
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

        private bool LoadSegment(NeSegment neSeg, ByteMemoryArea mem)
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
                neSeg.Address!.Selector!.Value.ToString("X4"),
                access);
            var stg = new TemporaryStorage(
                string.Format("seg{0:X4}", seg.Address!.Selector!.Value),
                0,
                PrimitiveType.SegmentSelector);
            seg.Identifier = Identifier.Create(stg);
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
            Address address = default;
            NeRelocationEntry rep;
            trace.Verbose("== Relocating segment {0}", seg.Address!);
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
                trace.Verbose("  {0}", WriteRelocationEntry(rep));

                // Get the target address corresponding to this entry.

                // If additive, there is no target chain list. Instead, add source
                //  and target.
                bool additive = (rep.relocation_type & NE_RELTYPE.ADDITIVE) != 0;
                (Address, ImportReference) impRef;
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
                        importStubs.Add(lp, (
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
                        importStubs.Add(lp, (
                            address,
                            new NamedImportReference(address, module, fnName, SymbolType.ExternalProcedure)));
                        addrImportStubs += 8;
                    }
                    break;
                case NE_RELTYPE.INTERNAL:
                    if ((rep.target1 & 0xff) == 0xff)
                    {
                        address = this.entryPoints[rep.target2 - 1]!.Address;
                    }
                    else
                    {
                        address = segments[rep.target1 - 1]!.Address! + rep.target2;
                    }
                    trace.Verbose("    {0}: {1:X4}:{2:X4} {3}",
                          i + 1,
                          address.Selector!.Value, 
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
                    listener.Error(
                        string.Format(
                            "Module {0}: unknown relocation address type {1:X2}. Please report",
                            module, rep.address_type));
                    return false;
                }

                if (additive)
                {
                    var sp = seg.Address! + offset;
                    trace.Verbose("    {0} (contains: {1:X4})", sp, mem.ReadLeUInt16(sp));
                    byte b;
                    ushort w;
                    switch (rep.address_type & (NE_RADDR) 0x7f)
                    {
                    case NE_RADDR.LOWBYTE:
                        b = mem.ReadByte(sp);
                        mem.WriteByte(sp, (byte)(b + address!.Offset));
                        break;
                    case NE_RADDR.OFFSET16:
                        w = mem.ReadLeUInt16(sp);
                        mem.WriteLeUInt16(sp, (ushort)(w + address!.Offset));
                        break;
                    case NE_RADDR.POINTER32:
                        w = mem.ReadLeUInt16(sp);
                        mem.WriteLeUInt16(sp, (ushort)(w + address!.Offset));
                        mem.WriteLeUInt16(sp + 2, address.Selector!.Value);
                        break;
                    case NE_RADDR.SELECTOR:
                        // Borland creates additive records with offset zero. Strange, but OK.
                        w = mem.ReadLeUInt16(sp);
                        if (w != 0)
                            listener.Error(string.Format("Additive selector to {0:X4}. Please report.", w));
                        else
                            mem.WriteLeUInt16(sp, address!.Selector!.Value);
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
                        var sp = seg.Address! + offset;
                        ushort next_offset = mem.ReadLeUInt16(sp);
                        trace.Verbose("    {0} (contains: {1:X4})", sp, next_offset);
                        switch (rep.address_type & (NE_RADDR) 0x7f)
                        {
                        case NE_RADDR.LOWBYTE:
                            mem.WriteByte(sp, (byte)address!.Offset);
                            break;
                        case NE_RADDR.OFFSET16:
                            mem.WriteLeUInt16(sp, (ushort)address!.Offset);
                            break;
                        case NE_RADDR.POINTER32:
                            mem.WriteLeUInt16(sp, (ushort)address!.Offset);
                            mem.WriteLeUInt16(sp + 2, address.Selector!.Value);
                            break;
                        case NE_RADDR.SELECTOR:
                            mem.WriteLeUInt16(sp, address!.Selector!.Value);
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
            listener.Warn("{0}: unknown ADDR TYPE {1},  " +
                "TYPE {2},  OFFSET {3:X4},  TARGET {4:X4} {5:X4}",
                seg.Address!.Selector!, rep.address_type, rep.relocation_type,
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
