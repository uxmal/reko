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
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serialization format for decompiler projects.
    /// </summary>
    /// <remarks>
    /// Note that you may safely *add* attributes and elements to the serialization
    /// format. However, should you *rename* or delete XML nodes, you must copy the serialization
    /// file format into a new file, bump the namespace identifier and the class name. You will
    /// also have to modify the ProjectSerializer to handle the new format.
    /// </remarks>
    [XmlRoot(ElementName = "project", Namespace = "http://schemata.jklnet.org/Reko/v4")]
    public class Project_v4 : SerializedProject
    {
        public const string FileExtension = ".dcproject";

        public Project_v4()
        {
            this.Inputs = new List<ProjectFile_v3>();
        }

        [XmlElement("arch")]
        public string ArchitectureName;

        [XmlElement("platform")]
        public string PlatformName;

        [XmlElement("input", typeof(DecompilerInput_v4))]
        [XmlElement("metadata", typeof(MetadataFile_v3))]
        [XmlElement("asm", typeof(AssemblerFile_v3))]
        public List<ProjectFile_v3> Inputs;

        public override T Accept<T>(ISerializedProjectVisitor<T> visitor)
        {
            return visitor.VisitProject_v4(this);
        }
    }

    public class DecompilerInput_v4 : ProjectFile_v3
    {
        public DecompilerInput_v4()
        {
            User = new UserData_v4();
        }

        [XmlElement("comment")]
        public string Comment;

        [XmlElement("disassembly")]
        public string DisassemblyFilename;

        [XmlElement("intermediate-code")]
        public string IntermediateFilename;

        [XmlElement("output")]
        public string OutputFilename;

        [XmlElement("types-file")]
        public string TypesFilename;

        [XmlElement("global-vars")]
        public string GlobalsFilename;

        [XmlElement("resources")]
        public string ResourcesDirectory;

        [XmlElement("user")]
        public UserData_v4 User;

        public override T Accept<T>(IProjectFileVisitor_v3<T> visitor)
        {
            return visitor.VisitInputFile(this);
        }
    }

    public class UserData_v4
    {
        public UserData_v4()
        {
            Procedures = new List<Procedure_v1>();
            GlobalData = new List<GlobalDataItem_v2>();
            Heuristics = new List<Heuristic_v3>();
            JumpTables = new List<JumpTable_v4>();
            Annotations = new List<Annotation_v3>();
            Calls = new List<SerializedCall_v1>();
            IndirectJumps = new List<IndirectJump_v4>();
            Segments = new List<Segment_v4>();
        }

        [XmlElement("address")]
        public string LoadAddress;

        [XmlElement("loader")]
        public string Loader;

        [XmlElement("processor")]
        public ProcessorOptions_v4 Processor;

        [XmlElement("platform")]
        public PlatformOptions_v4 PlatformOptions;

        [XmlElement("procedure")]
        public List<Procedure_v1> Procedures;

        [XmlElement("call")]
        public List<SerializedCall_v1> Calls;

        [XmlElement("indirectJump")]
        public List<IndirectJump_v4> IndirectJumps;
       
        [XmlElement("global")]
        public List<GlobalDataItem_v2> GlobalData;

        [XmlElement("heuristic")]
        public List<Heuristic_v3> Heuristics;

        [XmlElement("onLoad")]
        public Script_v2 OnLoadedScript;

        [XmlElement("jumpTable")]
        public List<JumpTable_v4> JumpTables;

        [XmlElement("annotation")]
        public List<Annotation_v3> Annotations;

        [XmlElement("textEncoding")]
        public string TextEncoding;

        [XmlArray("registerValues")]
        [XmlArrayItem("assume")]
        public RegisterValue_v2[] RegisterValues;

        [XmlElement("segment")]
        public List<Segment_v4> Segments;

        [XmlElement("dasmAddress")]
        [DefaultValue(false)]
        public bool ShowAddressesInDisassembly;

        [XmlElement("dasmBytes")]
        [DefaultValue(false)]
        public bool ShowBytesInDisassembly;

        [XmlElement("extractResources")]
        [DefaultValue(true)]
        public bool ExtractResources;
    }

    public class PlatformOptions_v4
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAnyElement]
        public XmlElement[] Options;
    }

    public class ProcessorOptions_v4
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAnyElement]
        public XmlElement[] Options;
    }

    public class JumpTable_v4
    {
        [XmlAttribute("tableAddress")]
        public string TableAddress;

        [XmlElement("dst")]
        public string[] Destinations;
    }

    public class IndirectJump_v4
    {
        [XmlAttribute("instrAdress")]
        public string InstructionAddress;

        [XmlAttribute("tableAddress")]
        public string TableAddress;

        [XmlAttribute("idxReg")]
        public string IndexRegister;
    }

    public class Segment_v4
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("offset")]
        public string Offset;

        [XmlAttribute("length")]
        public string Length;

        [XmlAttribute("addr")]
        public string Address;

        [XmlAttribute("arch")]
        public string Architecture;

        [XmlAttribute("access")]
        public string Access;
    }
}