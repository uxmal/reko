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

using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serialization format for decompiler projects. 
    /// </summary>
    /// <remarks>
    /// This version added support for putting outputs in user-specified files.
    /// 
    /// Note that you may safely *add* attributes and elements to the
    /// serializationformat. However, should you *rename* or delete XML nodes,
    /// you must copy the serialization file format into a new file, bump the
    /// namespace identifier and the class name. You will also have to modify
    /// the ProjectSerializer to handle the new format.
    /// </remarks>
    [XmlRoot(ElementName = "project", Namespace = "http://schemata.jklnet.org/Reko/v5")]
    public class Project_v5 : SerializedProject
    {
        /// <summary>
        /// File extension for Reko decompiler project files.
        /// </summary>
        public const string FileExtension = ".dcproject";

        /// <summary>
        /// Creates an instance of <see cref="Project_v5"/>.
        /// </summary>
        public Project_v5()
        {
            this.InputFiles = new List<DecompilerInput_v5>();
            this.MetadataFiles = new List<MetadataFile_v3>();
            this.AssemblerFiles = new List<AssemblerFile_v3>();
            this.ScriptFiles = new List<ScriptFile_v5>();
        }

        /// <summary>
        /// Name of the default architecture.
        /// </summary>
        [XmlElement("arch")]
        public string? ArchitectureName;

        /// <summary>
        /// Name of the platform used in this project.
        /// </summary>
        [XmlElement("platform")]
        public string? PlatformName;

        /// <summary>
        /// Input files to be decompiled.
        /// </summary>
        [XmlElement("input", typeof(DecompilerInput_v5))]
        public List<DecompilerInput_v5> InputFiles;

        /// <summary>
        /// User-specified metadata files.
        /// </summary>
        [XmlElement("metadata", typeof(MetadataFile_v3))]
        public List<MetadataFile_v3> MetadataFiles;

        /// <summary>
        /// Files to assemble.
        /// </summary>
        [XmlElement("asm", typeof(AssemblerFile_v3))]
        public List<AssemblerFile_v3> AssemblerFiles;

        /// <summary>
        /// Script files to be executed.
        /// </summary>
        [XmlElement("script", typeof(ScriptFile_v5))]
        public List<ScriptFile_v5> ScriptFiles;

        /// <inheritdoc/>
        public override T Accept<T>(ISerializedProjectVisitor<T> visitor)
        {
            return visitor.VisitProject_v5(this);
        }
    }

    /// <summary>
    /// Serialization format for decompiler input files.
    /// </summary>
    public class DecompilerInput_v5
    {
        /// <summary>
        /// Creates an instance of <see cref="DecompilerInput_v5"/>.
        /// </summary>
        public DecompilerInput_v5()
        {
            User = new UserData_v4();
        }

        /// <summary>
        /// Location of this input file.
        /// </summary>
        [XmlElement("location")]
        public string? Location;

        /// <summary>
        /// Kept for backwards compatibility only, use Location field wherever possible.
        /// </summary>
        [XmlElement("filename")]
        public string? Filename;

        /// <summary>
        /// Comment for this input file.
        /// </summary>
        [XmlElement("comment")]
        public string? Comment;

        /// <summary>
        /// Directory where the disassembly output will be written.
        /// </summary>
        [XmlElement("asmDir")]
        public string? DisassemblyDirectory;

        /// <summary>
        /// Directory where the source code output will be written.
        /// </summary>
        [XmlElement("srcDir")]
        public string? SourceDirectory;

        /// <summary>
        /// Directory where the include files will be written.
        /// </summary>
        [XmlElement("includeDir")]
        public string? IncludeDirectory;

        /// <summary>
        /// Directory where the resources will be written.
        /// </summary>
        [XmlElement("resources")]
        public string? ResourcesDirectory;

        /// <summary>
        /// User-specified data.
        /// </summary>
        [XmlElement("user")]
        public UserData_v4? User;
    }

    /// <summary>
    /// Serialization format for script files.
    /// </summary>
    public class ScriptFile_v5
    {
        /// <summary>
        /// Location of the file.
        /// </summary>
        [XmlElement("location")]
        public string? Location;

        /// <summary>
        /// Kept for backwards compatibility only, use Location field wherever possible.
        /// </summary>
        [XmlElement("filename")]
        public string? Filename;
    }
}