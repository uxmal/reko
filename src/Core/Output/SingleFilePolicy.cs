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
using System.IO;
using System.Linq;

namespace Reko.Core.Output
{
    /// <summary>
    /// This class implements the ad-hoc policy originally used by Reko,
    /// namely all code goes into a single file.
    /// </summary>
    public class SingleFilePolicy : OutputFilePolicy
    {
        private string defaultFile;
        private string defaultDataFile;

        public SingleFilePolicy(Program program) : base(program)
        {
            this.defaultFile = ""; 
            this.defaultDataFile = "";
        }

        public override Dictionary<string, IDictionary<Address, IAddressable>> GetObjectPlacements(string fileExtension, IEventListener listener)
        {
            this.defaultFile = Path.ChangeExtension(program.Name, fileExtension);
            this.defaultDataFile = Path.ChangeExtension(program.Name, "globals" + fileExtension);

            // Find the segment for each procedure
            var result = new Dictionary<string, IDictionary<Address, IAddressable>>();
            foreach (var proc in program.Procedures.Values)
            {
                PlaceObject(proc, defaultFile, result);
            }

            // Place all global objects.
            var wl = WorkList.Create(
                MakeGlobalWorkItems()
                .Concat(MakeSegmentWorkitems()));
            var objectTracer = new GlobalObjectTracer(program, wl, listener);
            while (wl.TryGetWorkItem(out var item))
            {
                var (field, addr) = item;
                var globalVar = new GlobalVariable(addr, field.DataType, program.NamingPolicy.GlobalName(field));
                PlaceObject(globalVar, defaultDataFile, result);
                objectTracer.TraceObject(field.DataType, addr);
            }
            return result;
        }

        public override Dictionary<string, Dictionary<ImageSegment, List<ImageMapItem>>> GetItemPlacements(string fileExtension)
        {
            var filename = Path.ChangeExtension(program.Name, fileExtension);
            var mappedItems = program.GetItemsBySegment();

            return new Dictionary<string, Dictionary<ImageSegment, List<ImageMapItem>>>
            {
                { filename, mappedItems }
            };
        }

    }
}
