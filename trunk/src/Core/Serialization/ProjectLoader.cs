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

using Decompiler.Core;
using Decompiler.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
    public class ProjectLoader 
    {
        private ILoader loader;

        public ProjectLoader(ILoader loader)
        {
            this.loader = loader;
        }

        /// <summary>
        /// Attempts to load the image as a project file.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static Project LoadProject(byte[] image, ILoader loader)
        {
            if (!IsXmlFile(image))
                return null;
            try
            {
                Stream stm = new MemoryStream(image);
                return new ProjectLoader(loader).LoadProject(stm);
            }
            catch (XmlException)
            {
                return null;
            }
        }

        /// <summary>
        /// Peeks at the beginning of the image to determine if it's an XML file.
        /// </summary>
        /// <remarks>
        /// We do not attempt to handle UTF-8 encoded Unicode BOM characters.
        /// </remarks>
        /// <param name="image"></param>
        /// <returns></returns>
        private static bool IsXmlFile(byte[] image)
        {
            if (LoadedImage.CompareArrays(image, 0, new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C }, 5)) // <?xml
                return true;
            return false;
        }

        public void Save(Project_v2 project, TextWriter sw)
        {
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v1(typeof(Project_v2));
            ser.Serialize(sw, project);
        }

        public Project LoadProject(string filename)
        {
            using (var fstm = new FileStream(filename, FileMode.Open))
            {
                return LoadProject(fstm);
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
            var typelibs = sp.Inputs.OfType<MetadataFile_v2>().Select(m => VisitMetadataFile(m));
            var programs = sp.Inputs.OfType<DecompilerInput_v2>().Select(s => VisitInputFile(s));
            var project = new Project();
            project.Programs.AddRange(programs);
            project.MetaDataFiles.AddRange(typelibs);
            return project;
        }

        public Program VisitInputFile(DecompilerInput_v2 sInput)
        {
            var bytes = loader.LoadImageBytes(sInput.Filename, 0);
            var program = loader.LoadExecutable(sInput.Filename, bytes, Address.Parse(sInput.Address, 16));
            program.Filename = sInput.Filename;
            if (sInput.UserProcedures != null)
            {
                program.UserProcedures = sInput.UserProcedures
                        .Select(sup => new KeyValuePair<Address, Procedure_v1>(
                            Address.Parse(sup.Address, 16),
                            sup))
                        .Where(kv => kv.Key != null)
                        .ToSortedList(kv => kv.Key, kv => kv.Value);
            }
            if (sInput.UserGlobalData != null)
            {
                program.UserGlobalData = sInput.UserGlobalData
                    .Select(sud => new KeyValuePair<Address, GlobalDataItem_v2>(
                        Address.Parse(sud.Address, 16),
                        new GlobalDataItem_v2
                        {
                            Address = sud.Address,
                            DataType = sud.DataType,
                        }))
                        .ToSortedList(kv => kv.Key, kv => kv.Value);
            }
            return program;
        }

        public MetadataFile VisitMetadataFile(MetadataFile_v2 sMetadata)
        {
            return new MetadataFile
            {
                Filename = sMetadata.Filename,
                LibraryName = sMetadata.LibraryName,
                TypeLibrary = loader.LoadMetadata(sMetadata.Filename)
            };
        }

        public Project LoadProject(Project_v1 sp)
        {
            Program program;
            if (sp.Input != null && !string.IsNullOrEmpty(sp.Input.Filename))
            {
                var bytes = loader.LoadImageBytes(sp.Input.Filename, 0);
                program = loader.LoadExecutable(sp.Input.Filename, bytes, Address.Parse(sp.Input.Address, 16));
                program.Filename = sp.Input.Filename;
            }
            else
            {
                program = new Program();
            }
            if (sp.Output != null)
            {

                program.DisassemblyFilename = sp.Output.DisassemblyFilename;
                program.IntermediateFilename = sp.Output.IntermediateFilename;
                program.OutputFilename = sp.Output.OutputFilename;
                program.TypesFilename = sp.Output.TypesFilename;
                program.GlobalsFilename = sp.Output.GlobalsFilename;
            }
            program.UserProcedures = sp.UserProcedures
                    .Select(sup => new KeyValuePair<Address, Procedure_v1>(
                        Address.Parse(sup.Address, 16),
                        sup))
                    .Where(kv => kv.Key != null)
                    .ToSortedList(kv => kv.Key, kv => kv.Value);

            foreach (var uc in sp.UserCalls)
            {
                var addr = Address.Parse(uc.InstructionAddress, 16);
                if (addr != null)
                    program.UserCalls.Add(addr, uc);
            }

            return new Project
            {
                Programs = { program }
            };
        }
    }
}
