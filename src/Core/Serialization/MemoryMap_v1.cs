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

using Reko.Core.Configuration;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Describes the absolute memory layout of a particular platform.
    /// </summary>
    [XmlRoot(ElementName = "memory", Namespace = SerializedLibrary.Namespace_v4)]
    public class MemoryMap_v1
    {
        //$REVIEW: tantalizing similarity to SerializedLibrary. 

        /// <summary>
        /// Data types of this memory map.
        /// </summary>
        [XmlElement(ElementName = "types")]
        [XmlArray( Namespace = SerializedLibrary.Namespace_v4)]
        public SerializedType[]? Types;

        /// <summary>
        /// Segments in the memory map.
        /// </summary>
        [XmlElement("segment")]
        public MemorySegment_v1[]? Segments;

        /// <summary>
        /// Loads an image map from a <see cref="Stream"/> containing the XML description of the
        /// segments inside.
        /// </summary>
        /// <paramref name="stm">The stream from which to deserialize the library.</paramref>
        public static MemoryMap_v1? Deserialize(Stream stm)
        {
            var ser = SerializedLibrary.CreateSerializer(
                typeof(MemoryMap_v1),
                SerializedLibrary.Namespace_v4);
            var mmap = (MemoryMap_v1?) ser.Deserialize(stm);
            return mmap;
        }

        /// <summary>
        /// Loads an <see cref="ImageSegment"/> based on the given <see cref="MemorySegment_v1"/>.
        /// </summary>
        /// <param name="segment">Memory segment to load.</param>
        /// <param name="platform">Current platform.</param>
        /// <param name="listener">Event listener to report errors to.</param>
        /// <returns></returns>
        public static ImageSegment? LoadSegment(MemorySegment_v1 segment, IPlatform platform, IEventListener listener)
        {
            if (segment.Name is null)
            {
                listener.Warn("Memory map segments must have names.");
                return null;
            }

            if (!platform.TryParseAddress(segment.Address, out var addr))
            {
                listener.Warn(
                    "Unable to parse address '{0}' in memory map segment {1}.",
                    segment.Address!,
                    segment.Name!);
                return null;
            }
            string? sSize = segment.Size;
            if (sSize is not null && sSize.StartsWith("0x"))
            {
                sSize = sSize.Substring(2);
            }
            if (!uint.TryParse(sSize, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var size))
            {
                listener.Warn(
                    "Unable to parse hexadecimal size '{0}' in memory map segment {1}.",
                    segment.Size!,
                    segment.Name!);
                return null;
            }

            var mem = new ByteMemoryArea(addr, new byte[size]);
            return new ImageSegment(segment.Name, mem, ConvertAccess(segment.Attributes));
        }

        private static AccessMode ConvertAccess(string? attributes)
        {
            var mode = AccessMode.Read;
            if (attributes is null)
                return mode;
            foreach (var ch in attributes)
            {
                switch (ch)
                {
                case 'r': mode |= AccessMode.Read; break;
                case 'w': mode |= AccessMode.Write; break;
                case 'x': mode |= AccessMode.Execute; break;
                }
            }
            return mode;
        }
    }

    /// <summary>
    /// Serialized description of a memory map segment.
    /// </summary>
    public partial class MemorySegment_v1
    {
        /// <summary>
        /// Name of the segment.
        /// </summary>
        [XmlAttribute("name")]
        public string? Name;

        /// <summary>
        /// Address of the segment.
        /// </summary>
        [XmlAttribute("addr")]
        public string? Address;

        /// <summary>
        /// Size in storage units of the segment.
        /// </summary>
        [XmlAttribute("size")]
        public string? Size;

        /// <summary>
        /// Protection attributes of the segment.
        /// </summary>
        [XmlAttribute("attr")]
        public string? Attributes;

        /// <summary>
        /// Description of the segment.
        /// </summary>
        [XmlElement("description")]
        public string? Description;

        /// <summary>
        /// Procedures in the segment.
        /// </summary>
        [XmlElement("procedure", typeof(Procedure_v1))]
        [XmlElement("service", typeof(SerializedService))]
        [XmlElement("dispatch-procedure", typeof(DispatchProcedure_v1))]
        public List<ProcedureBase_v1>? Procedures;

        /// <summary>
        /// Global variables in the segment.
        /// </summary>
        [XmlElement("global", typeof(GlobalVariable_v1))]
        public List<GlobalVariable_v1>? Globals;
    }
}