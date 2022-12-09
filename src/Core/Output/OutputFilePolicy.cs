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

using Reko.Core.Collections;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Output
{
    /// <summary>
    /// This class implements a policy that partitions <see cref="Procedure"/>s and 
    /// data objects into separate output files.
    /// </summary>
    public abstract class OutputFilePolicy
    {
        protected readonly Program program;

        /// <summary>
        /// Creates an output policy based on the user's preferences, defaulting to the old
        /// <see cref="SingleFilePolicy"/>.
        /// </summary>
        public static OutputFilePolicy CreateOutputPolicy(IServiceProvider services, Program program, string? sPolicy)
        {
            switch (sPolicy)
            {
            case Program.SingleFilePolicy:
                return new SingleFilePolicy(program);
            case Program.SegmentFilePolicy:
            case null:
                return new SegmentFilePolicy(program);
            default:
                if (!string.IsNullOrEmpty(sPolicy))
                {
                    var pluginLoader = services.RequireService<IPluginLoaderService>();
                    var type = pluginLoader.GetType(sPolicy);
                    return (OutputFilePolicy) Activator.CreateInstance(type, program)!;
                }
                else
                {
                    return new SingleFilePolicy(program);
                }
            }
        }

        public OutputFilePolicy(Program program)
        {
            this.program = program;
        }

        /// <summary>
        /// Returns a placement mapping for rendering high-level items.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public abstract Dictionary<string, IDictionary<Address, IAddressable>> GetObjectPlacements(
            string fileExtension,
            DecompilerEventListener listener);

        /// <summary>
        /// Returns a placement mapping for rendering low-level items.
        /// </summary>
        /// <param name="fileExtension">File extension to use on the files in
        /// the mapping.</param>
        /// <returns></returns>
        public abstract Dictionary<string, Dictionary<ImageSegment, List<ImageMapItem>>> GetItemPlacements(
            string fileExtension);

        public IEnumerable<(StructureField, Address)> MakeGlobalWorkItems()
        {
            var globals = program.TypeStore.TryGetTypeVariable(program.Globals, out var tvGlobals) &&
                tvGlobals.DataType is not null
                ? tvGlobals.DataType
                : program.Globals.DataType;
            if (globals is Pointer pt)
            {
                var strGlobals = pt.Pointee.ResolveAs<StructureType>();
                if (strGlobals != null)
                {
                    return strGlobals.Fields.Select(field =>
                        (field,
                         program.Platform.MakeAddressFromLinear((ulong) field.Offset, false)));
                }
            }
            return new (StructureField, Address)[0];
        }

        public IEnumerable<(StructureField, Address)> MakeSegmentWorkitems()
        {
            var typeStore = program.TypeStore;
            foreach (var segment in program.SegmentMap.Segments.Values)
            {
                if (!segment.Address.Selector.HasValue)
                    continue;
                if (segment.Identifier is not null && 
                    typeStore.TryGetTypeVariable(segment.Identifier, out var tvSegment) &&
                    tvSegment.Class.DataType is StructureType strType)
                {
                    foreach (var field in strType.Fields)
                    {
                        var addrField = segment.Address + field.Offset;
                        yield return (field, addrField);
                    }
                }
            }
        }

        /// <summary>
        /// Given an addressable object <paramref name="addressable"/> with
        /// its filename <paramref name="filename" /> and address <paramref name="addr"/>,
        /// create a placement for it in the <paramref name="result"/> dictionary.
        /// </summary>
        /// <param name="addressable"></param>
        /// <param name="filename"></param>
        /// <param name="result"></param>
        public void PlaceObject(
            IAddressable addressable,
            string filename,
            Dictionary<string, IDictionary<Address, IAddressable>> result)
        {
            if (!result.TryGetValue(filename, out var objects))
            {
                objects = new BTreeDictionary<Address, IAddressable>();
                result.Add(filename, objects);
            }
            if (!objects.ContainsKey(addressable.Address))
            {
                objects.Add(addressable.Address, addressable);
            }
        }
    }
}
