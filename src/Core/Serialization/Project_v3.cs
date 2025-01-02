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

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    public class Heuristic_v3
    {
        [XmlAttribute("name")]
        public string? Name;
    }

    public class MetadataFile_v3
    {
        [XmlElement("location")]
        public string? Location;

        // Kept for backwards compatibility only, use Location field wherever possible.
        [XmlElement("filename")]
        public string? Filename;

        [XmlElement("loader")]
        public string? LoaderTypeName;

        [XmlElement("module")]
        public string? ModuleName;
    }

    public class AssemblerFile_v3
    {
        [XmlElement("filename")]
        public string? Filename;

        [XmlElement("assembler")]
        public string? Assembler;
    }

    public class Annotation_v3
    {
        [XmlAttribute("addr")]
        public string? Address;

        [XmlText]
        public string? Text;
    }
}
