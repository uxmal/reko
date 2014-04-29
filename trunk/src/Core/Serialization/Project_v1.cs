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
    [XmlRoot(ElementName = "project", Namespace = "http://schemata.jklnet.org/Decompiler")]
    public class Project_v1
    {
        public const string FileExtension = ".dcproject";

        public Project_v1()
        {
            this.Input = new DecompilerInput_v1();
            this.Output = new DecompilerOutput_v1();
            this.UserProcedures = new List<SerializedProcedure>();
            this.UserCalls = new List<SerializedCall_v1>();
        }

        [XmlElement("input")]
        public DecompilerInput_v1 Input;

        [XmlElement("output")]
        public DecompilerOutput_v1 Output;

        [XmlElement("procedure", typeof(SerializedProcedure))]
        public List<SerializedProcedure> UserProcedures;

        [XmlElement("call", typeof(SerializedCall_v1))]
        public List<SerializedCall_v1> UserCalls;
    }

    public class DecompilerInput_v1
    {
        [XmlElement("filename")]
        public string Filename;

        [XmlElement("address")]
        public string Address;

        [XmlElement("comment")]
        public string Comment;

        [XmlElement("processor")]
        public string Processor;

        [XmlElement("procedure")]
        public List<SerializedProcedure> UserProcedures;

        [XmlElement("call")]
        public List<SerializedCall_v1> UserCalls;

        [XmlElement("disassembly")]
        public string DisassemblyFilename;

        [XmlElement("intermediate-code")]
        public string IntermediateFilename;

        [XmlElement("output")]
        public string OutputFilename;

        [XmlElement("types-file")]
        public string TypesFilename;
    }

    public class DecompilerOutput_v1
    {
        [XmlElement("disassembly")]
        public string DisassemblyFilename;

        /// <summary>
        /// If not null, specifies the file name for intermediate code.
        /// </summary>
        [XmlElement("intermediate-code")]
        public string IntermediateFilename;

        [XmlElement("output")]
        public string OutputFilename;

        [XmlElement("types-file")]
        public string TypesFilename;
    }

    public abstract class SerializedProcedureBase_v1
    {
        /// <summary>
        /// The name of a procedure.
        /// </summary>
        [XmlAttribute("name")]
        public string Name;

        /// <summary>
        /// Procedure signature. If non-null, the user has specified a signature. If null, the
        /// signature is unknown.
        /// </summary>
        [XmlElement("signature")]
        public SerializedSignature Signature;

        [XmlElement("characteristics")]
        public ProcedureCharacteristics Characteristics;
    }

    public class SerializedProcedure : SerializedProcedureBase_v1
    {
        /// <summary>
        /// Address of the procedure.
        /// </summary>
        [XmlElement("address")]
        public string Address;

        /// <summary>
        /// Property that indicated whether the procedure body is to be decompiled or not. If false, it is recommended
        /// that the Signature property be set.
        /// </summary>
        [XmlElement("decompile")]
        [DefaultValue(true)]
        public bool Decompile = true;
    }
}
