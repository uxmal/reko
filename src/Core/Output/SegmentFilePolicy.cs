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
    public class SegmentFilePolicy : OutputFilePolicy
    {
        private const int MaxKbSize = 64;
        
        private string defaultFile;
        private string progname;
        private Dictionary<ImageSegment, string> segmentFilenames;

        public SegmentFilePolicy(Program program) : base(program)
        {
        }

        public override Dictionary<string, List<Procedure>> GetProcedurePlacements(string fileExtension)
        {
            // Default file if we cannot find segment.
            this.defaultFile = Path.ChangeExtension(program.Name, fileExtension);
            this.progname = Path.GetFileNameWithoutExtension(program.Name);
            this.segmentFilenames = new Dictionary<ImageSegment, string>();

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

        private string DetermineFilename(Procedure proc)
        {
            if (program.SegmentMap.TryFindSegment(proc.EntryAddress, out var seg))
            {
                if (!segmentFilenames.TryGetValue(seg, out var segFilename))
                {
                    var sanitizedSegName = SanitizeSegmentName(seg.Name);
                    segFilename = $"{progname}_{sanitizedSegName}";
                    segmentFilenames.Add(seg, segFilename);
                }
                return segFilename;
            }
            else
            {
                // No known segment for this procedure, so add it to the default.
                return this.defaultFile;
            }
        }

        private string SanitizeSegmentName(string name)
        {
            var sb = new StringBuilder();
            foreach (var ch in name)
            {
                if (Char.IsLetterOrDigit(ch))
                    sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}
