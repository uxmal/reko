#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Linq;

namespace Reko.Libraries.Microchip
{

    /// <summary>
    /// Associates a unique processor ID to a PIC name. (used for COFF).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class PICPart
    {
        public PICPart()
        {
        }

        public PICPart(string name, int id)
        {
            Name = name;
            ProcID = id;
        }

        [XmlAttribute(AttributeName = "procid", Form = XmlSchemaForm.None, Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        public string ProcIDFormatted { get => $"0x{ProcID:X}"; set => ProcID = value.ToInt32Ex(); }

        /// <summary>
        /// Gets the unique processor ID for this PIC.
        /// </summary>
        [XmlIgnore]
        public int ProcID { get; private set; }

        /// <summary>
        /// Gets the name of the PIC
        /// </summary>
        [XmlAttribute(AttributeName = "name", Form = XmlSchemaForm.None, Namespace = "")]
        public string Name { get; set; }

    }

    /// <summary>
    /// List of (Processor ID/PIC name) pairs.
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public sealed class PartInfo
    {

        [XmlAttribute(AttributeName = "version", Form = XmlSchemaForm.None, Namespace = "")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "defaultlegacy", Form = XmlSchemaForm.None, Namespace = "")]
        public string DefaultLegacyFormatted { get => $"0x{DefaultLegacy:X}"; set { } }

        [XmlIgnore]
        public int DefaultLegacy => 0x8452;

        [XmlAttribute(AttributeName = "defaultextended", Form = XmlSchemaForm.None, Namespace = "")]
        public string DefaultExtendedFormatted { get => $"0x{DefaultExtended:X}"; set { } }

        [XmlIgnore]
        public int DefaultExtended => 0x4620;

        /// <summary>
        /// Gets the list of PIC parts.
        /// </summary>
        [XmlElement(ElementName = "Part", Form = XmlSchemaForm.None, Namespace = "", Type = typeof(PICPart))]
        public List<PICPart> Parts { get; set; } = new List<PICPart>();

        public IEnumerable<string> PICNamesList(Func<string, bool> filter)
            => Parts.Where(p => filter(p.Name)).Select(w => w.Name).OrderByNatural(n => n);

        public IEnumerable<string> PICNamesList()
            => Parts.Select(w => w.Name).OrderByNatural(n => n);

        public string GetPicName (int id)
            => Parts.Where(p => p.ProcID == id).FirstOrDefault()?.Name;

        public int? GetPICProcID(string picName)
            => Parts.Where(p => p.Name == picName).FirstOrDefault()?.ProcID;

    }

}
