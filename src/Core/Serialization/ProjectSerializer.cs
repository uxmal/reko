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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
    public class ProjectSerializer : IProjectFileVisitor_v2<ProjectFile>
    {
        public void Save(Project_v2 project, TextWriter sw)
        {
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(Project_v2));
            ser.Serialize(sw, project);
        }

        public Project LoadProject(string filename)
        {
            using (FileStream stm = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return LoadProject(stm);
            }
        }

        public Project LoadProject(Stream stm)
        {
            var rdr = new XmlTextReader(stm);
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(Project_v2));
            if (ser.CanDeserialize(rdr))
                return LoadProject((Project_v2) ser.Deserialize(rdr));
            ser = SerializedLibrary.CreateSerializer_v1(typeof(Project_v1));
            if (ser.CanDeserialize(rdr))
                return LoadProject((Project_v1) ser.Deserialize(rdr));
            return null;
        }

        public Project LoadProject(Project_v2 sp)
        {
            var inputFiles = sp.Inputs.Select(s => LoadInputFile(s));
            var project = new Project();
            project.InputFiles.AddRange(inputFiles);
            return project;
        }

        private ProjectFile LoadInputFile(ProjectFile_v2 sInput)
        {
            return sInput.Accept<ProjectFile>(this);
        }

        public ProjectFile VisitInputFile(DecompilerInput_v2 sInput)
        {
            var file = new InputFile
            {
                BaseAddress = Address.Parse(sInput.Address, 16),
                Filename = sInput.Filename,
            };
            if (sInput.UserProcedures != null)
            {
                file.UserProcedures = sInput.UserProcedures
                        .Select(sup => new KeyValuePair<Address, Procedure_v1>(
                            Address.Parse(sup.Address, 16),
                            sup))
                        .Where(kv => kv.Key != null)
                        .ToSortedList(kv => kv.Key, kv => kv.Value);
            }
            return file;
        }

        public ProjectFile VisitMetadataFile(MetadataFile_v2 sMetadata)
        {
            throw new NotImplementedException();
        }


        public Project LoadProject(Project_v1 sp)
        {
            InputFile inputFile = new InputFile
            {
                BaseAddress = Address.Parse(sp.Input.Address, 16),
                Filename = sp.Input.Filename,
                DisassemblyFilename = sp.Output.DisassemblyFilename,
                IntermediateFilename = sp.Output.IntermediateFilename,
                OutputFilename = sp.Output.OutputFilename,
                TypesFilename = sp.Output.TypesFilename,
                UserProcedures = sp.UserProcedures
                    .Select(sup => new KeyValuePair<Address, Procedure_v1>(
                        Address.Parse(sup.Address, 16),
                        sup))
                    .Where(kv => kv.Key != null)
                    .ToSortedList(kv => kv.Key, kv => kv.Value)
            };

            foreach (var uc in sp.UserCalls)
            {
                var addr = Address.Parse(uc.InstructionAddress, 16);
                if (addr != null)
                    inputFile.UserCalls.Add(addr, uc);
            }
            return new Project
            {
                InputFiles = { inputFile }
            };
        }
    }
}
