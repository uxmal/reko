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

        [XmlElement(ElementName = "types")]
        [XmlArray( Namespace = SerializedLibrary.Namespace_v4)]
        public SerializedType[] Types;

        [XmlElement("segment")]
        public MemorySegment_v1[] Segments;

        /// <summary>
        /// Loads an image map from a file containing the XML description of the
        /// segments inside.
        /// <param name="svc"></param>
        /// <param name="mmapFileName"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static MemoryMap_v1 LoadMemoryMapFromFile(IServiceProvider svc, string mmapFileName, IPlatform platform)
        {
            var cfgSvc = svc.RequireService<IConfigurationService>();
            var fsSvc = svc.RequireService<IFileSystemService>();
            var diagSvc = svc.RequireService<IDiagnosticsService>();
            try
            {
                var filePath = cfgSvc.GetInstallationRelativePath(mmapFileName);
                var ser = SerializedLibrary.CreateSerializer(
                    typeof(MemoryMap_v1),
                    SerializedLibrary.Namespace_v4);
                using (var stm = fsSvc.CreateFileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var mmap = (MemoryMap_v1)ser.Deserialize(stm);
                    return mmap;
                }
            }
            catch (Exception ex)
            {
                diagSvc.Error(ex, string.Format("Unable to open memory map file '{0}.", mmapFileName));
                return null;
            }
        }

        public static ImageSegment LoadSegment(MemorySegment_v1 segment, IPlatform platform, IDiagnosticsService diagSvc)
        {
            if (!platform.TryParseAddress(segment.Address, out var addr))
            {
                diagSvc.Warn(
                    "Unable to parse address '{0}' in memory map segment {1}.",
                    segment.Address,
                    segment.Name);
                return null;
            }
            if (!uint.TryParse(segment.Size, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var size))
            {
                diagSvc.Warn(
                    "Unable to parse hexadecimal size '{0}' in memory map segment {1}.",
                    segment.Size,
                    segment.Name);
                return null;
            }

            var mem = new MemoryArea(addr, new byte[size]);
            return new ImageSegment(segment.Name, mem, ConvertAccess(segment.Attributes));
        }

        public static AccessMode ConvertAccess(string attributes)
        {
            var mode = AccessMode.Read;
            if (attributes == null)
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

    public partial class MemorySegment_v1
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("addr")]
        public string Address;

        [XmlAttribute("size")]
        public string Size;

        [XmlAttribute("attr")]
        public string Attributes;

        [XmlElement("description")]
        public string Description;


        [XmlElement("procedure", typeof(Procedure_v1))]
        [XmlElement("service", typeof(SerializedService))]
        [XmlElement("dispatch-procedure", typeof(DispatchProcedure_v1))]
        public List<ProcedureBase_v1> Procedures;

        [XmlElement("global", typeof(GlobalVariable_v1))]
        public List<GlobalVariable_v1> Globals;
    }
}