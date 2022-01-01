#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
    /// This version added support for putting outputs in user-specified files.
    /// 
    /// Note that you may safely *add* attributes and elements to the serialization
    /// format. However, should you *rename* or delete XML nodes, you must copy the serialization
    /// file format into a new file, bump the namespace identifier and the class name. You will
    /// also have to modify the ProjectSerializer to handle the new format.
    /// </remarks>
    [XmlRoot(ElementName = "project", Namespace = "http://schemata.jklnet.org/Reko/v5")]
    public class Project_v5 : SerializedProject
    {
        public const string FileExtension = ".dcproject";

        public Project_v5()
        {
            this.InputFiles = new List<DecompilerInput_v5>();
            this.MetadataFiles = new List<MetadataFile_v3>();
            this.AssemblerFiles = new List<AssemblerFile_v3>();
            this.ScriptFiles = new List<ScriptFile_v5>();
        }

        [XmlElement("arch")]
        public string? ArchitectureName;

        [XmlElement("platform")]
        public string? PlatformName;

        [XmlElement("input", typeof(DecompilerInput_v5))]
        public List<DecompilerInput_v5> InputFiles;
        [XmlElement("metadata", typeof(MetadataFile_v3))]
        public List<MetadataFile_v3> MetadataFiles;
        [XmlElement("asm", typeof(AssemblerFile_v3))]
        public List<AssemblerFile_v3> AssemblerFiles;
        [XmlElement("script", typeof(ScriptFile_v5))]
        public List<ScriptFile_v5> ScriptFiles;

        public override T Accept<T>(ISerializedProjectVisitor<T> visitor)
        {
            return visitor.VisitProject_v5(this);
        }
    }

    public class DecompilerInput_v5
    {
        public DecompilerInput_v5()
        {
            User = new UserData_v4();
        }

        [XmlElement("location")]
        public string? Location;

        // Kept for backwards compatibility only, use Location field wherever possible.
        [XmlElement("filename")]
        public string? Filename;

        [XmlElement("comment")]
        public string? Comment;

        [XmlElement("asmDir")]
        public string? DisassemblyDirectory;

        [XmlElement("srcDir")]
        public string? SourceDirectory;

        [XmlElement("includeDir")]
        public string? IncludeDirectory;

        [XmlElement("resources")]
        public string? ResourcesDirectory;

        [XmlElement("user")]
        public UserData_v4? User;
    }

    public class ScriptFile_v5
    {
        [XmlElement("location")]
        public string? Location;

        // Kept for backwards compatibility only, use Location field wherever possible.
        [XmlElement("filename")]
        public string? Filename;
    }
}