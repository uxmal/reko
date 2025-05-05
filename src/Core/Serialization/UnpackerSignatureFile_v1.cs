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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Format of the unpacker signature file.
    /// </summary>
    [Serializable()]
    [XmlRoot(ElementName="SIGNATURES", Namespace = "", IsNullable = false)]
    public partial class UnpackerSignatureFile_v1
    {
        /// <summary>
        /// Signatures contained in the file.
        /// </summary>
        [XmlElement("ENTRY", Form = XmlSchemaForm.Unqualified)]
        public UnpackerSignature_v1[]? Signatures;
    }

    /// <summary>
    /// Serialization format for a single unpacker signature.
    /// </summary>

    [Serializable]
    public partial class UnpackerSignature_v1
    {
        /// <summary>
        /// Name of the signature.
        /// </summary>
        [XmlElement(ElementName = "NAME", Form = XmlSchemaForm.Unqualified)]
        public string? Name;

        /// <summary>
        /// Comments about the signature.
        /// </summary>
        [XmlElement(ElementName = "COMMENTS", Form = XmlSchemaForm.Unqualified)]
        public string? Comments;

        /// <summary>
        /// Pattern to identify the unpacker.
        /// </summary>
        [XmlElement(ElementName = "ENTRYPOINT", Form = XmlSchemaForm.Unqualified)]
        public string? EntryPoint;

        /// <summary>
        /// Pattern to identify the program entry.
        /// </summary>
        [XmlElement(ElementName = "ENTIREPE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string? EntirePE;
    }
}
