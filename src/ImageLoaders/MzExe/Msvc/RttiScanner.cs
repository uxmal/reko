#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Diagnostics;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Core.Types;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.MzExe.Msvc
{
    /// <summary>
    /// Scans read-only segments for MSVC RTTI "complete object locators"
    /// </summary>
    public class RttiScanner
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(RttiScanner), nameof(RttiScanner))
        {
            Level = TraceLevel.Verbose
        };

        private readonly Program program;
        private readonly IProcessorArchitecture arch;
        private readonly IRttiHelper rttiHelper;
        private readonly IEventListener listener;
        private readonly ImageSegment[] roSegments;

        public RttiScanner(Program program, IEventListener listener)
        {
            this.program = program;
            this.arch = program.Architecture;
            this.rttiHelper = program.Architecture.PointerType.BitSize == 64
                ? new RttiHelper64(program.SegmentMap.BaseAddress)
                : new RttiHelper32();

            this.listener = listener;
            this.roSegments = program.SegmentMap.Segments.Values.Where(s => !s.IsWriteable).ToArray();
        }

        public RttiScannerResults Scan()
        {
            var results = new RttiScannerResults();
            ScanCompleteObjectLocators(results);
            ScanVFTables(results);
            Report(results);
            return results;
        }

        /// <summary>
        /// Scans the given image segments for MSVC VFTables, placing any discovered
        /// VFTable address in <see cref="RttiScannerResults.VFTables"/> and also generates
        /// image symbols for the VFtables in <see cref="RttiScannerResults.Symbols"/>.
        /// </summary>
        /// <remarks>
        /// This method must be called after the <see cref="CompleteObjectLocator"/>s have 
        /// been found.
        /// MSVC lays out vtables as arrays of (pointer-to-procedure), and at position
        /// -1 of these arrays, it places a pointer to a <see cref="CompleteObjectLocator"/>.
        /// The algorithm is crude but effective: scan the memory area for a pointer to a complete
        /// object locator, after which there may be 0 or more pointers to executable memory.
        /// These pointers are assumed to be the members of the vtable.
        /// </remarks>
        public void ScanVFTables(RttiScannerResults results)
        {
            foreach (var seg in roSegments)
            {
                trace.Inform("MSVC RTTI: Scanning segment {0} ({1}) for vtables.", seg.Name, seg.Address);
                var rdr = program.CreateImageReader(seg.Address, seg.EndAddress);
                while (rttiHelper.TryReadPointer(rdr, out Address? addr))
                {
                    if (!results.CompleteObjectLocators.TryGetValue(addr, out var col))
                        continue;
                    if (!results.TypeDescriptors.TryGetValue(col.TypeDescriptorAddress, out var td))
                    {
                        trace.Warn("*** Couldn't find type descriptor at {0} from COL at {1}", col.TypeDescriptorAddress, col.Address);
                        continue;
                    }
                    var vftableEntries = new List<Address>();
                    // We're past the COL, the following entries are pointers to functions.
                    var addrVFTable = rdr.Address;
                    while (rttiHelper.TryReadPointer(rdr, out var addrFn))
                    {
                        if (!program.SegmentMap.IsExecutableAddress(addrFn))
                        {
                            rdr.Seek(addrFn.DataType.Size);   // Might be the COL of the next vtable.
                            break;
                        }
                        vftableEntries.Add(addrFn);
                    }
                    MakeSymbolsForVftable(results, col, td, addrVFTable, vftableEntries);
                }
            }
        }

        private void MakeSymbolsForVftable(
            RttiScannerResults results,
            CompleteObjectLocator col,
            TypeDescriptor td,
            Address addrVFTable,
            List<Address> vftableEntries)
        {
            if (vftableEntries.Count == 0 || results.VFTables.ContainsKey(addrVFTable))
                return;

            // Make a symbol for the vftable itself.
            trace.Verbose("VFTable for {0} at {1} - {2} entries", td.Name, addrVFTable, vftableEntries.Count);
            results.VFTables.Add(addrVFTable, vftableEntries);
            var vftableName = $"??_7{td.Name.Substring(4)}6B@";
            var vftableType = new ArrayType(
                    new Pointer(new CodeType(), program.Architecture.PointerType.BitSize),
                    vftableEntries.Count);
            var vftableSymbol = ImageSymbol.DataObject(
                arch,
                addrVFTable,
                vftableName,
                vftableType);
            results.Symbols.Add(vftableSymbol);

            foreach (var pfn in vftableEntries)
            {
                results.Symbols.Add(ImageSymbol.Procedure(arch, pfn));
            }
        }

        private void ScanCompleteObjectLocators(RttiScannerResults results)
        {
            foreach (var seg in roSegments)
            {
                trace.Inform("MSVC RTTI: Scanning segment {0} ({1})", seg.Name, seg.Address);
                var rdr = program.CreateImageReader(seg.Address);
                while (rdr.IsValid)
                {
                    var offset = rdr.Offset;
                    if (SniffForCompleteObjectLocator(rdr))
                    {
                        var copy = rdr.Clone();
                        copy.Offset = offset;
                        ScanCompleteObjectLocator(copy, results);
                    }
                    else
                    {
                        rdr.Offset = offset + 1;
                    }
                }
            }
        }

        private void ScanCompleteObjectLocator(EndianImageReader rdr, RttiScannerResults results)
        {
            var addrCol = rdr.Address;
            if (results.CompleteObjectLocators.ContainsKey(addrCol))
                return;

            var col = CompleteObjectLocator.Read(rdr, rttiHelper);
            if (col is null)
            {
                var loc = listener.CreateAddressNavigator(program, addrCol);
                listener.Warn(loc, "Found a complete object locator but was unable to parse it.");
                return;
            }
            results.CompleteObjectLocators.Add(col.Address, col);
            results.Symbols.Add(
                CreateImageSymbol(col.Address, rdr.Address - col.Address));

            if (results.TypeDescriptors.ContainsKey(col.TypeDescriptorAddress))
                return;
            rdr = program.CreateImageReader(col.TypeDescriptorAddress);
            var typedesc = TypeDescriptor.Read(rdr, rttiHelper);
            if (typedesc is null)
            {
                var loc = listener.CreateAddressNavigator(program, col.TypeDescriptorAddress);
                listener.Warn(loc, "Found a complete object locator but was unable to parse it.");
                return;
            }
            trace.Verbose("   vftable at: {0}", typedesc.VFTableAddress);
            results.TypeDescriptors.Add(col.TypeDescriptorAddress, typedesc);
            var typedescName = $"??_R0{typedesc.Name}@8";
            results.Symbols.Add(
                CreateImageSymbol(
                    typedesc.Address,
                    rdr.Address - typedesc.Address,
                    typedescName));
        }

        private ImageSymbol CreateImageSymbol(Address addr, long size, string? name = null)
        {
            var sym = ImageSymbol.DataObject(
                program.Architecture,
                addr,
                name,
                new UnknownType((int)size));
            return sym;
        }

        private bool IsValidPointer([NotNullWhen(true)] Address? ptr)
        {
            if (ptr is null)
                return false;
            return program.SegmentMap.IsValidAddress(ptr);
        }

        private bool SniffForCompleteObjectLocator(EndianImageReader rdr)
        {
            var addr = rdr.Address;
            if (!rdr.TryReadLeUInt32(out uint signature))
                return false;
            // 32-bit COL's start with 0x00000000
            // 64-bit COL's start with 0x00000001
            if (signature != rttiHelper.ColSignature)
                return false;
            rdr.Offset += 8;        // Skip to type descriptor ptr.
            var ptrTypeDescriptor = rttiHelper.ReadOffsetPointer(rdr);
            var ptrClassDescriptor = rttiHelper.ReadOffsetPointer(rdr);
            if (!IsValidPointer(ptrTypeDescriptor) || !IsValidPointer(ptrClassDescriptor))
                return false;

            // Sniff the type descriptor looking for the class name, which
            // always is prefixed with ".?A"

            var rdrTd = program.CreateImageReader(ptrTypeDescriptor);
            if (!rttiHelper.TryReadPointer(rdrTd, out var ptrVftable))
                return false;
            if (!IsValidPointer(ptrVftable))
                return false;
            rdrTd.Seek(ptrVftable.DataType.Size);

            // Read the first 3 bytes
            if (!rdrTd.TryPeekLeUInt32(0, out uint sName))
                return false;
            bool found = ((sName & 0xFFFFFF) == 0x413F2E); // '.?A'
            TraceString(found, addr, rdrTd);
            return found;
        }

        private void Report(RttiScannerResults results)
        {
            if (results.CompleteObjectLocators.Count == 0)
                return;
            listener.Info("MSVC RTTI scan found {0} complete object locators", results.CompleteObjectLocators.Count);
        }

        [Conditional("DEBUG")]
        private void TraceString(bool found, Address addrCol, EndianImageReader rdr)
        {
            if (!found || !trace.TraceVerbose)
                return;
            var str = rdr.ReadCString(PrimitiveType.Char, Encoding.UTF8).ToString();
            trace.Verbose("MSVC RTTI: found Complete object locator at {0}, for {1}",
                addrCol, str);
        }
    }
}
