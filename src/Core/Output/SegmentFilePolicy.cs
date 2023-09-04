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

using Reko.Core.Collections;
using Reko.Core.Loading;
using Reko.Core.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Core.Output
{
    /// <summary>
    /// This <see cref="OutputFilePolicy" /> places <see cref="IAddressable"/> objects in
    /// files that are named after the segments of the decompiled binary.
    /// </summary>
    public class SegmentFilePolicy : OutputFilePolicy
    {
        private const int MaxChunkSize = 64 * 1024;

        private readonly string progname;
        private readonly Dictionary<ImageSegment, string> segmentFilenames;
        private string defaultFile;
        private string defaultDataFile;

        public SegmentFilePolicy(Program program) : base(program)
        {
            this.segmentFilenames = new Dictionary<ImageSegment, string>();
            this.progname = Path.GetFileNameWithoutExtension(program.Name);
            this.defaultFile = "";
            this.defaultDataFile = "";
        }

        public override Dictionary<string, IDictionary<Address, IAddressable>> GetObjectPlacements(
            string fileExtension,
            IEventListener listener)
        {
            Debug.Assert(fileExtension.Length > 0 && fileExtension[0] == '.');

            // Default file if we cannot find segment.
            this.defaultFile = Path.ChangeExtension(program.Name, fileExtension);
            this.defaultDataFile = Path.ChangeExtension(program.Name, "globals" + fileExtension);

            // Find the segment for each procedure
            var result = new Dictionary<string, IDictionary<Address,IAddressable>>();
            foreach (var proc in program.Procedures.Values)
            {
                var filename = DetermineFilename(proc, fileExtension);
                PlaceObject(proc, filename, result);
            }

            // Place all global objects.
            var wl = WorkList.Create(
                MakeGlobalWorkItems()
                .Concat(MakeSegmentWorkitems()));
            var objectTracer = new GlobalObjectTracer(program, wl, listener);
            while (wl.TryGetWorkItem(out var item))
            {
                var (field, addr) = item;
                var filename = DetermineFilename(addr, fileExtension);
                var globalVar = new GlobalVariable(addr, field.DataType, program.NamingPolicy.GlobalName(field));
                PlaceObject(globalVar, filename, result);
                objectTracer.TraceObject(field.DataType, addr);
            }
            return result;
        }

        public override Dictionary<string, Dictionary<ImageSegment, List<ImageMapItem>>> GetItemPlacements(string fileExtension)
        {
            var mappedItems = program.GetItemsBySegment();
            var result = new Dictionary<string, Dictionary<ImageSegment, List<ImageMapItem>>>();
            foreach (var entry in mappedItems)
            {
                var segment = entry.Key;
                foreach (var item in entry.Value)
                {
                    var filename = DetermineFilename(item, segment, fileExtension);
                    if (!result.TryGetValue(filename, out var segments))
                    {
                        segments = new Dictionary<ImageSegment, List<ImageMapItem>>();
                        result.Add(filename, segments);
                    }
                    if (!segments.TryGetValue(segment, out var items))
                    {
                        items = new List<ImageMapItem>();
                        segments.Add(segment, items);
                    }
                    items.Add(item);
                }
            }
            return result;
        }

        private string DetermineFilename(Procedure proc, string fileExtension)
        {
            if (program.User.Procedures.TryGetValue(proc.EntryAddress, out var userProc) &&
                !string.IsNullOrWhiteSpace(userProc.OutputFile))
            {
                return Path.ChangeExtension(userProc.OutputFile!, fileExtension);
            }
            if (program.User.ProcedureSourceFiles.TryGetValue(proc.EntryAddress, out var sourcefile))
            {
                return Path.ChangeExtension(sourcefile, fileExtension);
            }

            if (program.SegmentMap.TryFindSegment(proc.EntryAddress, out var seg))
            {
                return FilenameBasedOnSegment(proc.EntryAddress, seg, fileExtension);
            }
            else
            {
                // No known segment for this procedure, so add it to the default.
                return this.defaultFile;
            }
        }

        private string DetermineFilename(Address addr, string fileExtension)
        {
            //$TODO: user placements.
            if (program.SegmentMap.TryFindSegment(addr, out var seg))
            {
                return FilenameBasedOnSegment(addr, seg, fileExtension);
            }
            else
            {
                // No known segment for this procedure, so add it to the default.
                return this.defaultDataFile;
            }
        }

        private string DetermineFilename(ImageMapItem item, ImageSegment seg, string fileExtension)
        {
            return FilenameBasedOnSegment(item.Address, seg, fileExtension);
        }

        private string FilenameBasedOnSegment(Address addr, ImageSegment seg, string fileExtension)
        {
            if (!segmentFilenames.TryGetValue(seg, out var filename))
            {
                var sanitizedSegName = NamingPolicy.SanitizeIdentifierName(seg.Name);
                filename = $"{progname}_{sanitizedSegName}";
                segmentFilenames.Add(seg, filename);
            }
            // If the segment is large, we need to subdivide it.
            if (seg.Size > MaxChunkSize)
            {
                var offset = addr - seg.Address;
                var chunk = offset / MaxChunkSize;
                filename = $"{filename}_{chunk:X4}";
            }
            return Path.ChangeExtension(filename, fileExtension);
        }

    }
}
