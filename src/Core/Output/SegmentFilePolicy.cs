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

using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.Core.Output
{
    /// <summary>
    /// This <see cref="OutputFilePolicy" /> places procedures and global data objects in
    /// files that are named after the segments of the decompiled binary.
    /// </summary>
    public class SegmentFilePolicy : OutputFilePolicy
    {
        private const int MaxChunkSize = 64 * 1024;
        
        private string defaultFile;
        private string progname;
        private Dictionary<ImageSegment, string> segmentFilenames;

        public SegmentFilePolicy(Program program) : base(program)
        {
            this.segmentFilenames = new Dictionary<ImageSegment, string>();
            this.progname = Path.GetFileNameWithoutExtension(program.Name);
        }

        public override Dictionary<string, List<Procedure>> GetProcedurePlacements(string fileExtension)
        {
            // Default file if we cannot find segment.
            this.defaultFile = Path.ChangeExtension(program.Name, fileExtension);

            // Find the segment for each procedure
            var result = new Dictionary<string, List<Procedure>>();
            foreach (var proc in program.Procedures.Values)
            {
                var filename = DetermineFilename(proc);
                filename = Path.ChangeExtension(filename, fileExtension);
                if (!result.TryGetValue(filename, out var procs))
                {
                    procs = new List<Procedure>();
                    result.Add(filename, procs);
                }
                procs.Add(proc);
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
                    var filename = DetermineFilename(item, segment);
                    filename = Path.ChangeExtension(filename, fileExtension);
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

        private string DetermineFilename(Procedure proc)
        {
            if (program.User.Procedures.TryGetValue(proc.EntryAddress, out var userProc) &&
                !string.IsNullOrWhiteSpace(userProc.OutputFile))
            {
                return userProc.OutputFile;
            }
            if (program.User.ProcedureSourceFiles.TryGetValue(proc.EntryAddress, out var sourcefile))
            {
                return sourcefile;
            }

            if (program.SegmentMap.TryFindSegment(proc.EntryAddress, out var seg))
            {
                return FilenameBasedOnSegment(proc.EntryAddress, seg);
            }
            else
            {
                // No known segment for this procedure, so add it to the default.
                return this.defaultFile;
            }
        }

        private string DetermineFilename(ImageMapItem item, ImageSegment seg)
        {
            return FilenameBasedOnSegment(item.Address, seg);
        }

        private string FilenameBasedOnSegment(Address addr, ImageSegment seg)
        {
            if (!segmentFilenames.TryGetValue(seg, out var filename))
            {
                var sanitizedSegName = SanitizeSegmentName(seg.Name);
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
            return filename;
        }

        private string SanitizeSegmentName(string name)
        {
            var sb = new StringBuilder();
            int n = name.Length;
            int i;

            // Skip leading unprintables.
            for (i = 0; i < n; ++i)
            {
                if (char.IsLetterOrDigit(name[i]))
                    break;
            }

            bool emitUnderscore = false;
            for (; i < n; ++i)
            {
                char ch = name[i];
                if (char.IsLetterOrDigit(ch))
                {
                    if (emitUnderscore)
                    {
                        sb.Append('_');
                        emitUnderscore = false;
                    }
                    sb.Append(ch);
                }
                else
                {
                    emitUnderscore = true;
                }
            }
            return sb.ToString();
        }
    }
}
