#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
        private readonly DecompilerEventListener listener;
        private readonly ImageSegment[] roSegments;

        public RttiScanner(Program program, DecompilerEventListener listener)
        {
            this.program = program;
            this.listener = listener;
            this.roSegments = program.SegmentMap.Segments.Values.Where(s => !s.IsWriteable).ToArray();
        }

        public RttiScannerResults Scan()
        {
            var results = new RttiScannerResults();
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
            Report(results);
            return results;
        }

        private void ScanCompleteObjectLocator(EndianImageReader rdr, RttiScannerResults results)
        {
            var addrCol = rdr.Address;
            if (results.CompleteObjectLocators.ContainsKey(addrCol))
                return;

            var col = CompleteObjectLocator.Read(rdr, Address.Ptr32);
            if (col is null)
            {
                var loc = listener.CreateAddressNavigator(program, addrCol);
                listener.Warn(loc, "Found a complete object locator but was unable to parse it.");
                return;
            }
            results.CompleteObjectLocators.Add(col.Address, col);
            CreateImageMapItem(col.Address, rdr.Address - col.Address);

            if (results.TypeDescriptors.ContainsKey(col.TypeDescriptorAddress))
                return;
            rdr = program.CreateImageReader(col.TypeDescriptorAddress);
            var typedesc = TypeDescriptor.Read(rdr, Address.Ptr32);
            if (typedesc is null)
            {
                var loc = listener.CreateAddressNavigator(program, col.TypeDescriptorAddress);
                listener.Warn(loc, "Found a complete object locator but was unable to parse it.");
                return;
            }
            results.TypeDescriptors.Add(col.TypeDescriptorAddress, typedesc);
            CreateImageMapItem(typedesc.Address, rdr.Address - typedesc.Address);
        }

        private void CreateImageMapItem(Address addr, long size)
        {
            var item = new ImageMapItem(addr, (uint)size);
            program.ImageMap.AddItemWithSize(addr, item);
        }

        private Address? ReadOffsetPointer(EndianImageReader rdr)
        {
            if (!rdr.TryReadLeUInt32(out uint ptr))
                return null;
            return Address.Ptr32(ptr);
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
            if (signature != 0)     // 32-bit COL's start with 0x00000000
                return false;
            rdr.Offset += 8;        // Skip to type descriptor ptr.
            var ptrTypeDescriptor = ReadOffsetPointer(rdr);
            var ptrClassDescriptor = ReadOffsetPointer(rdr);
            if (!IsValidPointer(ptrTypeDescriptor) || !IsValidPointer(ptrClassDescriptor))
                return false;

            // Sniff the type descriptor looking for the class name, which
            // always is prefixed with ".?A"

            var rdrTd = program.CreateImageReader(ptrTypeDescriptor);
            var ptrVftable = ReadOffsetPointer(rdrTd);
            if (!IsValidPointer(ptrVftable))
                return false;
            rdrTd.Offset += 4;
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
