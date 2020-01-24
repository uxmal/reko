#region License
/* 
 * Copyright (c) 2017-2020 Christian Hostelet.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.gnu.org/licenses/gpl-2.0.html.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * If applicable, add the following below the header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */

#endregion

namespace Reko.Libraries.Microchip
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;
    using System.Linq;

    /// <summary>
    /// Associates a unique processor ID to a PIC name. (used for COFF).
    /// </summary>
    [Serializable(), XmlType(AnonymousType = true, Namespace = "")]
    public sealed class PICPart
    {
        /// <summary>
        /// Default constructor. For serialization.
        /// </summary>
        public PICPart() { }

        /// <summary>
        /// Instantiates a new PIC identification (proc_id, architecture, name, cloned)
        /// </summary>
        /// <param name="name">The name of the PIC.</param>
        /// <param name="id">The COFF identifier of the PIC.</param>
        /// <param name="arch">The archictecture name of the PIC.</param>
        /// <param name="clonedfrom">The cloned PIC name if this PIC is a clone.</param>
        public PICPart(string name, uint id, string arch, string clonedfrom)
        {
            Name = name;
            ProcID = id;
            Arch = arch;
            ClonedFrom = clonedfrom;
        }

        /// <summary> Gets the architecture of the PIC. </summary>
        [XmlAttribute(AttributeName = "arch", Namespace = "")]
        public string Arch { get; set; }

        /// <summary> Gets the unique processor ID for this PIC. </summary>
        [XmlIgnore] public uint ProcID { get; private set; }

        /// <summary> XML attribute formatting property. </summary>
        [XmlAttribute(AttributeName = "procid", Namespace = "")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never), EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Needed for serialization")]
        public string ProcIDFormatted
        {
            get => $"0x{ProcID:X4}";
            set => ProcID = value.ToUInt32Ex();
        }

        /// <summary> Gets the name of the PIC </summary>
        [XmlAttribute(AttributeName = "name", Namespace = "")]
        public string Name { get; set; }

        /// <summary> Gets the cloned PIC name if this PIC is a clone. </summary>
        [XmlAttribute(AttributeName = "clonedfrom", Namespace = "")]
        public string ClonedFrom { get; set; }

        /// <summary> XML attribute conditional presence. </summary>
        public bool ShouldSerializeClonedFrom() => !string.IsNullOrWhiteSpace(ClonedFrom);

    }

    /// <summary> List of (Processor ID/PIC name) pairs. </summary>
    [Serializable(),
        XmlType(AnonymousType = true, Namespace = "")]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public sealed class PICPartInfo
    {
        /// <summary>
        /// Gets or sets the version of the PIC parts information file.
        /// </summary>
        [XmlAttribute(AttributeName = "version", Namespace = "")]
        public string Version { get; set; }

        /// <summary>
        /// Gets the default PIC ID for legacy execution mode.
        /// </summary>
        [XmlIgnore]
        public int DefaultLegacy => 0x8452;

        /// <summary>
        /// Gets the default PIC ID for extended execution mode.
        /// </summary>
        [XmlIgnore]
        public int DefaultExtended => 0x4620;

        /// <summary> Helper property. </summary>
        [XmlAttribute(AttributeName = "defaultlegacy", Namespace = "")]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Needed for serialization")]
        public string defaultlegacyFormatted { get => $"0x{DefaultLegacy:X}"; set { } }

        /// <summary> Helper property. </summary>
        [XmlAttribute(AttributeName = "defaultextended", Namespace = "")]
        [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Needed for serialization")]
        public string defaultextendedFormatted { get => $"0x{DefaultExtended:X}"; set { } }

        /// <summary> Gets the list of PIC parts. </summary>
        [XmlElement(ElementName = "Part", Namespace = "", Type = typeof(PICPart))]
        public List<PICPart> Parts { get; set; } = new List<PICPart>();

        /// <summary>
        /// Gets the naturally-sorted list of PIC names obeying to a criteria.
        /// </summary>
        /// <param name="filter">Specifies the criteria filter.</param>
        public IEnumerable<string> PICNamesList(Func<string, bool> filter)
            => Parts.Where(p => filter(p.Name)).Select(w => w.Name).OrderByNatural(n => n);

        /// <summary>
        /// Gets the naturally-sorted list of PIC names.
        /// </summary>
        public IEnumerable<string> PICNamesList()
            => Parts.Select(w => w.Name).OrderByNatural(n => n);

        /// <summary>
        /// Gets the name of a PIC given its part ID.
        /// </summary>
        /// <param name="id">The part identifier.</param>
        public string GetPICName(int id)
            => Parts.Where(p => p.ProcID == id).FirstOrDefault()?.Name;

        /// <summary>
        /// Gets part identifier of a PIC given its name.
        /// </summary>
        /// <param name="picName">Name of the PIC.</param>
        public uint? GetPICProcID(string picName)
            => Parts.Where(p => p.Name == picName).FirstOrDefault()?.ProcID;

    }

}
