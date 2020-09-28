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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    [Serializable()]
    [XmlRoot(ElementName="SIGNATURES", Namespace = "", IsNullable = false)]
    public partial class UnpackerSignatureFile_v1
    {
        [XmlElement("ENTRY", Form = XmlSchemaForm.Unqualified)]
        public UnpackerSignature_v1[] Signatures;
    }

    [Serializable]
    public partial class UnpackerSignature_v1
    {
        [XmlElement(ElementName = "NAME", Form = XmlSchemaForm.Unqualified)]
        public string Name;

        [XmlElement(ElementName = "COMMENTS", Form = XmlSchemaForm.Unqualified)]
        public string Comments;

        [XmlElement(ElementName = "ENTRYPOINT", Form = XmlSchemaForm.Unqualified)]
        public string EntryPoint;

        [XmlElement(ElementName = "ENTIREPE", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EntirePE;
    }
}
