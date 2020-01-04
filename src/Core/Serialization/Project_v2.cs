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
    /// also have to modify the ProjectSerializer to handle the new format.</remarks>
    [XmlRoot(ElementName = "project", Namespace = "http://schemata.jklnet.org/Decompiler/v2")]
    public class Project_v2 : SerializedProject
    {
        public const string FileExtension = ".dcproject";

        public Project_v2()
        {
            this.Inputs = new List<ProjectFile_v2>();
            this.Output = new DecompilerOutput_v1();
        }

        [XmlElement("input", typeof(DecompilerInput_v2))]
        [XmlElement("metadata",typeof(MetadataFile_v2))]
        [XmlElement("asm", typeof(AssemblerFile_v2))]
        public List<ProjectFile_v2> Inputs;

        [XmlElement("output")]
        public DecompilerOutput_v1 Output;

        /// <summary>
        /// Procedures that have been either marked by the user, or have been annotated by the user.
        /// </summary>
        [XmlElement("procedure", typeof(Procedure_v1))]
        public List<Procedure_v1> UserProcedures;

        [XmlElement("call", typeof(SerializedCall_v1))]
        public List<SerializedCall_v1> UserCalls;

        public override T Accept<T>(ISerializedProjectVisitor<T> visitor)
        {
            return visitor.VisitProject_v2(this);
        }
    }

    public abstract class ProjectFile_v2
    {
        [XmlElement("filename")]
        public string Filename;

        public abstract T Accept<T>(IProjectFileVisitor_v2<T> visitor);
    }

    public interface IProjectFileVisitor_v2<T>
    {
        T VisitInputFile(DecompilerInput_v2 input);
        T VisitMetadataFile(MetadataFile_v2 metadata);
        T VisitAssemblerFile(AssemblerFile_v2 asm);
    }

    public class DecompilerInput_v2 : ProjectFile_v2
    {
        public DecompilerInput_v2()
        {
            UserGlobalData = new List<GlobalDataItem_v2>(); 
        }

        [XmlElement("address")]
        public string Address;

        [XmlElement("comment")]
        public string Comment;

        [XmlElement("processor")]
        public string Processor;

        [XmlElement("procedure")]
        public List<Procedure_v1> UserProcedures;

        [XmlElement("call")]
        public List<SerializedCall_v1> UserCalls;

        [XmlElement("global")]
        public List<GlobalDataItem_v2> UserGlobalData;

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

        [XmlElement("onLoad")]
        public Script_v2 OnLoadedScript;

        [XmlElement("options")]
        public ProgramOptions_v2 Options;

        public override T Accept<T>(IProjectFileVisitor_v2<T> visitor)
        {
            return visitor.VisitInputFile(this);
        }
    }

    public class MetadataFile_v2 : ProjectFile_v2
    {
        [XmlElement("loader")]
        public string LoaderTypeName;

        [XmlElement("module")]
        public string ModuleName;

        public override T Accept<T>(IProjectFileVisitor_v2<T> visitor)
        {
            return visitor.VisitMetadataFile(this);
        }
    }

    public class AssemblerFile_v2 : ProjectFile_v2
    {
        [XmlElement("assembler")]
        public string Assembler;

        public override T Accept<T>(IProjectFileVisitor_v2<T> visitor)
        {
            return visitor.VisitAssemblerFile(this);
        }
    }

    public class Script_v2
    {
        [XmlAttribute]
        public bool Enabled;

        [XmlAttribute]
        public string Interpreter;

        [XmlText]
        public string Script;
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

        [XmlElement("global-vars")]
        public string GlobalsFilename;
    }
}
