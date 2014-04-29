#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

namespace Decompiler.Core.Serialization
{
    /// <summary>
    /// Seralization format for decompiler projects.
    /// </summary>
    /// <remarks>
    /// Note that you may safely *add* attributes and elements to the serialization
    /// format. However, should *rename* or delete XML nodes, you must copy the serialization
    /// file format into a new file, bump the namespace identifier and the class name. You will
    /// also have to modify the ProjectSerializer to handle the new format.</remarks>
    [XmlRoot(ElementName = "project", Namespace = "http://schemata.jklnet.org/Decompiler/v2")]
    public class Project_v2
    {
        public const string FileExtension = ".dcproject";

        public Project_v2()
        {
            this.Inputs = new List<DecompilerInput_v1>();
            this.Output = new DecompilerOutput_v1();
        }

        [XmlElement("input")]
        public List<DecompilerInput_v1> Inputs;

        [XmlElement("output")]
        public DecompilerOutput_v1 Output;

        [XmlElement("procedure", typeof(SerializedProcedure))]
        public List<SerializedProcedure> UserProcedures;

        [XmlElement("call", typeof(SerializedCall_v1))]
        public List<SerializedCall_v1> UserCalls;
    }
}
