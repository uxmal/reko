#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Reko.Core;
using Reko.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Reko.Core.Services;
using System.Diagnostics;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Loads a Reko decompiler project file. May optionally ask the user
    /// for help.
    /// </summary>
    public class ProjectLoader : ProjectPersister
    {
        public event EventHandler<ProgramEventArgs> ProgramLoaded;
        public event EventHandler<TypeLibraryEventArgs> TypeLibraryLoaded;

        private ILoader loader;
        private Project project;

        public ProjectLoader(IServiceProvider services, ILoader loader)
            : this(services, loader, new Project())
        {
        }

        public ProjectLoader(IServiceProvider services, ILoader loader, Project project)
            : base(services)
        {
            this.loader = loader;
            this.project = project;
        }

        /// <summary>
        /// Attempts to load the image as a project file.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="loader"></param>
        /// <returns></returns>
        public Project LoadProject(string fileName, byte[] image)
        {
            if (!IsXmlFile(image))
                return null;
            try
            {
                Stream stm = new MemoryStream(image);
                return LoadProject(fileName, stm);
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

        public Project LoadProject(string filename)
        {
            var fsSvc = Services.RequireService<IFileSystemService>();
            using (var stm = fsSvc.CreateFileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return LoadProject(filename, stm);
            }
        }

        /// <summary>
        /// Loads a .dcproject from a stream.
        /// </summary>
        /// <param name="stm"></param>
        /// <returns></returns>
        public Project LoadProject(string filename, Stream stm)
        {
            var rdr = new XmlTextReader(stm);
            XmlSerializer ser = SerializedLibrary.CreateSerializer_v3(typeof(Project_v3));
            if (ser.CanDeserialize(rdr))
                return LoadProject(filename,(Project_v3) ser.Deserialize(rdr));
            ser = SerializedLibrary.CreateSerializer_v2(typeof(Project_v2));
            if (ser.CanDeserialize(rdr))
                return LoadProject((Project_v2) ser.Deserialize(rdr));
            return null;
        }

        /// <summary>
        /// Loads a Project object from its serialized representation. First loads the programs
        /// and then any extra metadata files.
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public Project LoadProject(string filename, Project_v3 sp)
        {
            var typelibs = sp.Inputs.OfType<MetadataFile_v3>().Select(m => VisitMetadataFile(filename, m));
            var programs = sp.Inputs.OfType<DecompilerInput_v3>().Select(s => VisitInputFile(filename, s));
            var asm = sp.Inputs.OfType<AssemblerFile_v3>().Select(s => VisitAssemblerFile(s));
            project.Programs.AddRange(programs);
            project.MetadataFiles.AddRange(typelibs);
            return this.project;
        }

        /// <summary>
        /// Loads a Project object from its serialized representation. First loads the programs
        /// and then any extra metadata files.
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public Project LoadProject(Project_v2 sp)
        {
            var typelibs = sp.Inputs.OfType<MetadataFile_v2>().Select(m => VisitMetadataFile(m));
            var programs = sp.Inputs.OfType<DecompilerInput_v2>().Select(s => VisitInputFile(s));
            var asm = sp.Inputs.OfType<AssemblerFile_v2>().Select(s => VisitAssemblerFile(s));
            project.Programs.AddRange(programs);
            project.MetadataFiles.AddRange(typelibs);
            return this.project;
        }

        public Program VisitInputFile(string projectFilePath, DecompilerInput_v3 sInput)
        {
            var bytes = loader.LoadImageBytes(ConvertToAbsolutePath(projectFilePath, sInput.Filename), 0);
            var address = LoadAddress(sInput.User);
            Program program;
            if (sInput.User.Processor != null && sInput.User.PlatformOptions.Name != null)
            {
                var arch = sInput.User.Processor.Name;
                var platform = sInput.User.PlatformOptions.Name;
                program = loader.LoadRawImage(sInput.Filename, bytes, arch, platform, address);
            }
            else
            {
                program = loader.LoadExecutable(sInput.Filename, bytes, address);

            }
            program.Filename = ConvertToAbsolutePath(projectFilePath, sInput.Filename);
            program.DisassemblyFilename = ConvertToAbsolutePath(projectFilePath, sInput.DisassemblyFilename);
            program.IntermediateFilename = ConvertToAbsolutePath(projectFilePath, sInput.IntermediateFilename);
            program.OutputFilename = ConvertToAbsolutePath(projectFilePath, sInput.OutputFilename);
            program.TypesFilename = ConvertToAbsolutePath(projectFilePath, sInput.TypesFilename);
            program.GlobalsFilename = ConvertToAbsolutePath(projectFilePath, sInput.GlobalsFilename);
            program.EnsureFilenames(program.Filename);
            LoadUserData(sInput.User, program, program.User);
            ProgramLoaded.Fire(this, new ProgramEventArgs(program));
            return program;
        }

        private Address LoadAddress(UserData_v3 user)
        {
            if (user == null || user.LoadAddress == null || user.Processor == null)
                return null;
            Address addr;
            if (!Services.RequireService<IConfigurationService>()
                .GetArchitecture(user.Processor.Name)
                .TryParseAddress(user.LoadAddress, out addr))
                return null;
            return addr;
        }

        public void LoadUserData(UserData_v3 sUser, Program program, UserData user)
        {
            if (sUser == null)
                return;
            user.OnLoadedScript = sUser.OnLoadedScript;
            if (sUser.Processor != null)
            {
                program.User.Processor = sUser.Processor.Name;
                if (program.Architecture == null && !string.IsNullOrEmpty(program.User.Processor))
                {
                    program.Architecture = Services.RequireService<IConfigurationService>().GetArchitecture(program.User.Processor);
                }
                //program.Architecture.LoadUserOptions();       //$TODO
            }
            if (sUser.Procedures != null)
            {
                user.Procedures = sUser.Procedures
                    .Select(sup =>
                    {
                        Address addr;
                        program.Architecture.TryParseAddress(sup.Address, out addr);
                        return new KeyValuePair<Address, Procedure_v1>(addr, sup);
                    })
                    .Where(kv => kv.Key != null)
                    .ToSortedList(kv => kv.Key, kv => kv.Value);
            }

            if (sUser.PlatformOptions != null)
            {
                program.User.Environment = sUser.PlatformOptions.Name;
                program.Platform.LoadUserOptions(LoadPlatformOptions(sUser.PlatformOptions.Options));
            }
            if (sUser.GlobalData != null)
            {
                user.Globals = sUser.GlobalData
                    .Select(sud =>
                    {
                        Address addr;
                        program.Architecture.TryParseAddress(sud.Address, out addr);
                        return new KeyValuePair<Address, GlobalDataItem_v2>(
                            addr,
                            sud);
                    })
                    .Where(kv => kv.Key != null)
                   .ToSortedList(kv => kv.Key, kv => kv.Value);
            }
            foreach (var kv in user.Globals)
            {
                var dt = kv.Value.DataType.BuildDataType(program.TypeFactory);
                var item = new ImageMapItem((uint)dt.Size)
                {
                    Address = kv.Key,
                    DataType = dt,
                };
                if (item.Size > 0)
                {
                    program.ImageMap.AddItemWithSize(kv.Key, item);
                }
                else
                {
                    program.ImageMap.AddItem(kv.Key, item);
                }
            }
            if (sUser.Heuristics != null)
            {
                user.Heuristics.UnionWith(sUser.Heuristics.Select(h => h.Name));
            }
        }

        private object ReadItem(XmlElement element)
        {
            if (element.Name == "item")
            {
                return element.InnerText;
            } else if (element.Name == "list")
            {
                return element.ChildNodes
                    .OfType<XmlElement>()
                    .Select(e => ReadItem(e))
                    .ToList();
            }
            else if (element.Name == "dict")
            {
                return ReadDictionaryElements(
                    element.ChildNodes.OfType<XmlElement>());
            }
            throw new NotSupportedException();
        }

        private Dictionary<string,object> ReadDictionaryElements(IEnumerable<XmlElement> elements)
        {
            return elements.ToDictionary(
                e => e.Attributes["key"] != null ? e.Attributes["key"].Value : null,
                e => ReadItem(e));
        }

        private Dictionary<string, object> LoadPlatformOptions(XmlElement[] options)
        {
            if (options == null)
                return new Dictionary<string, object>();
            return ReadDictionaryElements(options);
        }

        public Program VisitInputFile(DecompilerInput_v2 sInput)
        {
            var bytes = loader.LoadImageBytes(sInput.Filename, 0);
            var program = loader.LoadExecutable(sInput.Filename, bytes, null);
            program.Filename = sInput.Filename;
            LoadUserData(sInput, program, program.User);

            program.DisassemblyFilename = sInput.DisassemblyFilename;
            program.IntermediateFilename = sInput.IntermediateFilename;
            program.OutputFilename = sInput.OutputFilename;
            program.TypesFilename = sInput.TypesFilename;
            program.GlobalsFilename = sInput.GlobalsFilename;
            program.EnsureFilenames(sInput.Filename);
            ProgramLoaded.Fire(this, new ProgramEventArgs(program));
            return program;
        }

        private void LoadUserData(DecompilerInput_v2 sInput, Program program, UserData user)
        {
            if (sInput.UserProcedures != null)
            {
                user.Procedures = sInput.UserProcedures
                        .Select(sup =>
                        {
                            Address addr;
                            program.Architecture.TryParseAddress(sup.Address, out addr);
                            return new KeyValuePair<Address, Procedure_v1>(addr, sup);
                        })
                        .Where(kv => kv.Key != null)
                        .ToSortedList(kv => kv.Key, kv => kv.Value);
            }
            if (sInput.UserGlobalData != null)
            {
                user.Globals = sInput.UserGlobalData
                    .Select(sud =>
                    {
                        Address addr;
                        program.Architecture.TryParseAddress(sud.Address, out addr);
                        return new KeyValuePair<Address, GlobalDataItem_v2>(
                            addr,
                            sud);
                    })
                    .Where(kv => kv.Key != null)
                   .ToSortedList(kv => kv.Key, kv => kv.Value);
            }
            foreach (var kv in user.Globals)
            {
                var dt = kv.Value.DataType.BuildDataType(program.TypeFactory);
                var item = new ImageMapItem((uint)dt.Size)
                {
                    Address = kv.Key,
                    DataType = dt,
                };
                if (item.Size > 0)
                {
                    program.ImageMap.AddItemWithSize(kv.Key, item);
                }
                else
                {
                    program.ImageMap.AddItem(kv.Key, item);
                }
            }
            user.OnLoadedScript = sInput.OnLoadedScript;
            if (sInput.Options != null)
            {
                program.User.Heuristics.Add("shingle");
            }
        }

        public MetadataFile VisitMetadataFile(string projectFilePath, MetadataFile_v3 sMetadata)
        {
            string filename = ConvertToAbsolutePath(projectFilePath, sMetadata.Filename);
            return LoadMetadataFile(filename);
        }

        public MetadataFile VisitMetadataFile(MetadataFile_v2 sMetadata)
        {
            string filename = sMetadata.Filename;
            return LoadMetadataFile(filename);
        }

        public MetadataFile LoadMetadataFile(string filename)
        {
            var platform = DeterminePlatform(filename);
            var typeLib = loader.LoadMetadata(filename, platform);
            TypeLibraryLoaded.Fire(this, new TypeLibraryEventArgs(typeLib));
            return new MetadataFile
            {
                Filename = filename,
                ModuleName = typeLib.ModuleName,
                TypeLibrary = typeLib
            };
        }

        private Platform DeterminePlatform(string filename)
        {
            var platformsInUse = project.Programs.Select(p => p.Platform).Distinct().ToArray();
            if (platformsInUse.Length == 1 && platformsInUse[0] != null)
                return platformsInUse[0];
            Platform platform = null;
            if (platformsInUse.Length == 0)
            {
                var oSvc = Services.GetService<IOracleService>();
                if (oSvc != null)
                {
                    platform = oSvc.QueryPlatform(string.Format(
                        "Please specify with operating environment should be used with metadata file {0}.",
                        filename));
                }
                Debug.Print("Got platform <{0}>", platform);
                return platform;
            }
            throw new NotImplementedException("Multiple platforms possible; not implemented yet.");
        }

        public Program VisitAssemblerFile(AssemblerFile_v3 sAsmFile)
        {
            return loader.AssembleExecutable(sAsmFile.Filename, sAsmFile.Assembler, null);
        }

        public Program VisitAssemblerFile(AssemblerFile_v2 sAsmFile)
        {
            return loader.AssembleExecutable(sAsmFile.Filename, sAsmFile.Assembler, null);
        }
    }

    public class ProgramEventArgs : EventArgs
    {
        public ProgramEventArgs(Program program)
        {
            this.Program = program;
        }

        public Program Program { get; private set; }
    }

    public class TypeLibraryEventArgs : EventArgs
    {
        public TypeLibraryEventArgs(TypeLibrary typelib)
        { 
            this.TypeLibrary = typelib; 
        }

        public TypeLibrary TypeLibrary { get; private set; }
    }
}
