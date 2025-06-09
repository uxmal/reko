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

using Reko.Core.Collections;
using Reko.Core.Configuration;
using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Scripts;
using Reko.Core.Services;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Loads a Reko decompiler project file. May optionally ask the user
    /// for help.
    /// </summary>
    public class ProjectLoader : ProjectPersister
    {
        /// <summary>
        /// Event that is raised when a program is loaded.
        /// </summary>
        public event EventHandler<Program>? ProgramLoaded;

        private readonly ILoader loader;
        private readonly Project project;
        private readonly IEventListener listener;
        private IPlatform? platform;
        private IProcessorArchitecture? arch;

        /// <summary>
        /// Constructs an instance of <see cref="ProjectLoader"/>.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance to use.</param>
        /// <param name="loader"><see cref="ILoader"/> instance to use.</param>
        /// <param name="location"><see cref="ImageLocation"/> of the project file.</param>
        /// <param name="listener"><see cref="IEventListener"/> interface to use when
        /// communicating errors.
        /// </param>
        public ProjectLoader(
            IServiceProvider services,
            ILoader loader,
            ImageLocation location,
            IEventListener listener)
            : this(services, loader, new Project(location), listener)
        {
        }

        /// <summary>
        /// Constructs an instance of <see cref="ProjectLoader"/>.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance to use.</param>
        /// <param name="loader"><see cref="ILoader"/> instance to use.</param>
        /// <param name="project">Empty <see cref="Project"/> object.</param>
        /// <param name="listener"><see cref="IEventListener"/> interface to use when
        /// communicating errors.
        /// </param>
        public ProjectLoader(
            IServiceProvider services,
            ILoader loader,
            Project project,
            IEventListener listener)
            : base(services)
        {
            this.loader = loader;
            this.project = project;
            this.listener = listener;
        }

        /// <summary>
        /// Attempts to load the image as a project file.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Project? LoadProject(byte[] image)
        {
            if (!IsXmlFile(image))
                return null;
            try
            {
                Stream stm = new MemoryStream(image);
                return LoadProject(stm);
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
        /// <param name="image">Bytes of the loaded file.</param>
        /// <returns>True if an XML processing instruction was encountered; otherwise false.
        /// </returns>
        private static bool IsXmlFile(byte[] image)
        {
            if (ByteMemoryArea.CompareArrays(image, 0, new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C }, 5)) // <?xml
                return true;
            return false;
        }

        /// <summary>
        /// Loads a project from the file system.
        /// </summary>
        /// <returns></returns>
        public Project? LoadProject()
        {
            var filename = project.Location.FilesystemPath;
            var fsSvc = Services.RequireService<IFileSystemService>();
            using var stm = fsSvc.CreateFileStream(filename, FileMode.Open, FileAccess.Read);
            return LoadProject(stm);
        }

        private static readonly (Type, string)[] supportedProjectFileFormats =
        {
            (typeof(Project_v5), SerializedLibrary.Namespace_v5),
            (typeof(Project_v4), SerializedLibrary.Namespace_v4),
        };

        /// <summary>
        /// Loads a .dcproject from a stream.
        /// </summary>
        /// <param name="stm"></param>
        /// <returns>
        /// The Project if the file format was recognized, otherwise null.
        /// </returns>
        public Project? LoadProject(Stream stm)
        {
            var rdr = new XmlTextReader(stm);
            foreach (var fileFormat in supportedProjectFileFormats)
            {
                XmlSerializer ser = SerializedLibrary.CreateSerializer(fileFormat.Item1, fileFormat.Item2);
                if (ser.CanDeserialize(rdr))
                {
                    var deser = new Deserializer(this);
                    var project = ((SerializedProject)ser.Deserialize(rdr)!).Accept(deser);
                    return project;
                }
            }
            return null;
        }

        // Avoid reflection by using the visitor pattern.
        class Deserializer : ISerializedProjectVisitor<Project>
        {
            private readonly ProjectLoader outer;

            public Deserializer(ProjectLoader outer)
            {
                this.outer = outer;
            }
            public Project VisitProject_v4(Project_v4 sProject) => outer.LoadProject(sProject);
            public Project VisitProject_v5(Project_v5 sProject) => outer.LoadProject(sProject);
        }

        /// <summary>
        /// Loads a Project object from its serialized representation. First loads the
        /// common architecture and platform, then metadata, and finally any programs.
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public Project LoadProject(Project_v4 sp)
        {
            return LoadProject(ProjectVersionMigrator.MigrateProject(sp));
        }

        /// <summary>
        /// Loads a Project object from its serialized representation. First loads the
        /// common architecture and platform, then metadata, and finally any programs.
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public Project LoadProject(Project_v5 sp)
        {
            if (string.IsNullOrWhiteSpace(sp.ArchitectureName))
                sp.ArchitectureName = GuessProjectProcessorArchitecture(project.Location, sp.InputFiles);
            if (string.IsNullOrWhiteSpace(sp.ArchitectureName))
                throw new ApplicationException("Missing <arch> in project file. Please specify.");
            if (string.IsNullOrWhiteSpace(sp.PlatformName))
                sp.PlatformName = GuessProjectPlatform(project.Location, sp.InputFiles);
            var cfgSvc = Services.RequireService<IConfigurationService>();
            var arch = cfgSvc.GetArchitecture(sp.ArchitectureName);
            this.arch = arch ?? throw new ApplicationException(
                    string.Format("Unknown architecture '{0}' in project file.",
                        sp.ArchitectureName ?? "(null)"));
            var env = cfgSvc.GetEnvironment(sp.PlatformName!);
            if (env is null)
                throw new ApplicationException(
                    string.Format("Unknown operating environment '{0}' in project file.",
                        sp.PlatformName ?? "(null)"));
            this.platform = env.Load(Services, arch);
            this.project.LoadedMetadata = this.platform.CreateMetadata();
            var typelibs = sp.MetadataFiles.Select(m => VisitMetadataFile(m));
            var programs = sp.InputFiles.Select(s => VisitInputFile(s));
            var scripts = sp.ScriptFiles
                .Select(s => VisitScriptFile(s))
                .OfType<ScriptFile>();
            sp.AssemblerFiles.Select(s => VisitAssemblerFile(s));
            project.MetadataFiles.AddRange(typelibs);
            project.ScriptFiles.AddRange(scripts);
            project.Programs.AddRange(programs);
            return this.project;
        }

        private string? GuessProjectProcessorArchitecture(ImageLocation projectLocation, IEnumerable<DecompilerInput_v5> inputs)
        {
            foreach (var input in inputs)
            {
                string? processorArchitecture = input.User?.Processor?.Name;

                if (processorArchitecture is not null)
                    return processorArchitecture;
            }

            foreach (var input in inputs)
            {
                var program = LoadProgram(projectLocation, input);
                return program.Architecture.Name;
            }
            return null;
        }

        private string? GuessProjectPlatform(ImageLocation projectLocation, IEnumerable<DecompilerInput_v5> inputs)
        {
            foreach (var input in inputs)
            {
                string? platform = input.User?.PlatformOptions?.Name;

                if (platform is not null)
                    return platform;
            }

            foreach (var input in inputs)
            {
                var program = LoadProgram(projectLocation, input);
                return program.Platform.Name;
            }
            return null;
        }

        private Program LoadProgram(ImageLocation projectLocation, DecompilerInput_v5 sInput)
        {
            ImageLocation binLocation;
            if (!string.IsNullOrEmpty(sInput.Location))
            {
                binLocation = ConvertToAbsoluteLocation(projectLocation, sInput.Location)!;
            }
            else
            {
                // Only legacy project files use sInput.Filename.
                binLocation = ConvertToAbsoluteLocation(projectLocation, sInput.Filename)!;
            }
            var sUser = sInput.User ?? new UserData_v4
            {
                ExtractResources = true,
                OutputFilePolicy = Program.SingleFilePolicy,
            };
            var address = LoadAddress(sUser, this.arch);
            var archOptions = XmlOptions.LoadIntoDictionary(sUser.Processor?.Options, StringComparer.OrdinalIgnoreCase);
            var userSegments = sUser.Segments is null
                ? new List<UserSegment>()
                : sUser.Segments
                    .Select(s => LoadUserSegment(s))
                    .Where(s => s is not null)
                    .ToList()!;
            Program program;
            if (!string.IsNullOrEmpty(sUser.Loader))
            {
                // The presence of an explicit loader name prompts us to
                // use the LoadRawImage path.
                var archName = sUser.Processor?.Name;
                var platform = sUser.PlatformOptions?.Name;
                var bytes = loader.LoadImageBytes(binLocation);
                program = loader.ParseRawImage(bytes, address, new LoadDetails
                {
                    Location = binLocation,
                    LoaderName = sUser.Loader,
                    ArchitectureName = archName,
                    ArchitectureOptions = archOptions,
                    PlatformName = platform,
                    LoadAddress = sUser.LoadAddress,
                    Segments = userSegments,
                });
            }
            else
            {
                if (loader.Load(binLocation, null, address) is not Program p)
                {
                    // A previous save of the project was able to read the file, 
                    // but now we can't...
                    throw new InvalidOperationException($"Previously saved location {binLocation} doesn't lead to a decompileable file image.");
                }
                program = p;
            }
            return program;
        }

        /// <summary>
        /// Processes a project input file.
        /// </summary>
        /// <param name="sInput">Input file to process.</param>
        /// <returns>Returns a <see cref="Program"/>.</returns>
        public Program VisitInputFile(DecompilerInput_v5 sInput)
        {
            ImageLocation binAbsLocation;  // file: URL to the location of the file.
            string projectPath = project.Location.FilesystemPath;
            //$REVIEW: common code begins here
            if (!string.IsNullOrEmpty(sInput.Location))
            {
                binAbsLocation = ConvertToAbsoluteLocation(project.Location, sInput.Location)!;
            }
            else if (!string.IsNullOrEmpty(sInput.Filename))
            {
                // No location present, this is an older project file.
                var binAbsPath = ConvertToAbsolutePath(projectPath, sInput.Filename);
                binAbsLocation = ImageLocation.FromUri(binAbsPath!);
            }
            else
            {
                throw new BadImageFormatException("Missing Uri property.");
            }
            //$REVIEW: common code ends here.
            var sUser = sInput.User ?? new UserData_v4
            {
                ExtractResources = true,
                OutputFilePolicy = Program.SingleFilePolicy,
            };
            var address = LoadAddress(sUser, this.arch);
            Program program = LoadProgram(project.Location, sInput);
            LoadUserData(sUser, program, program.User, project.Location);
            program.Location = binAbsLocation;
            program.DisassemblyDirectory = ConvertToAbsolutePath(projectPath, sInput.DisassemblyDirectory)!;
            program.SourceDirectory = ConvertToAbsolutePath(projectPath, sInput.SourceDirectory)!;
            program.IncludeDirectory = ConvertToAbsolutePath(projectPath, sInput.IncludeDirectory)!;
            program.ResourcesDirectory = ConvertToAbsolutePath(projectPath, sInput.ResourcesDirectory)!;
            program.EnsureDirectoryNames(program.Location);
            program.User.LoadAddress = address;
            ProgramLoaded?.Invoke(this, program);
            return program;
        }

        private Address? LoadAddress(UserData_v4 user, IProcessorArchitecture? arch)
        {
            if (user is null || arch is null || user.LoadAddress is null)
                return null;
            if (!arch.TryParseAddress(user.LoadAddress, out Address addr))
                return null;
            return addr;
        }

        /// <summary>
        /// Loads the user-specified data.
        /// </summary>
        /// <param name="sUser">User data from the persistence layer.</param>
        /// <param name="program">The partially loaded Program.</param>
        /// <param name="user">UserData object that will be modified.</param>
        /// <param name="projectLocation">The <see cref="ImageLocation" /> of the project hosting
        /// this program.</param>
        public void LoadUserData(UserData_v4 sUser, Program program, UserData user, ImageLocation projectLocation)
        {
            if (sUser is null)
                return;
            user.OnLoadedScript = sUser.OnLoadedScript;
            if (sUser.Processor is not null)
            {
                user.Processor = sUser.Processor.Name;
                if (program.Architecture is null && !string.IsNullOrEmpty(user.Processor))
                {
                    program.Architecture = Services.RequireService<IConfigurationService>().GetArchitecture(user.Processor!)!;
                }
                //$BUG: what if architecture isn't supported? fail the whole thing?
                program.Architecture!.LoadUserOptions(XmlOptions.LoadIntoDictionary(sUser.Processor.Options, StringComparer.OrdinalIgnoreCase));
            }
            if (sUser.PlatformOptions is not null)
            {
                user.Environment = sUser.PlatformOptions.Name;
                program.Platform.LoadUserOptions(XmlOptions.LoadIntoDictionary(sUser.PlatformOptions.Options, StringComparer.OrdinalIgnoreCase));
            }
            if (sUser.Procedures is not null)
            {
                user.Procedures = sUser.Procedures
                    .Select(sup => LoadUserProcedure(program, sup))
                    .Where(sup => sup is not null)
                    .ToSortedList(k => k!.Address, v => v!);
                user.ProcedureSourceFiles = user.Procedures
                    .Where(kv => !string.IsNullOrEmpty(kv.Value.OutputFile))
                    .ToDictionary(kv => kv.Key!, kv => ConvertToAbsolutePath(projectLocation.FilesystemPath, kv.Value.OutputFile)!);
            }
            if (sUser.GlobalData is not null)
            {
                user.Globals = sUser.GlobalData
                    .Select(LoadUserGlobal)
                    .Where(c => c is not null)
                    .ToSortedList(k => k!.Address!, v => v!);
            }
            if (sUser.Annotations is not null)
            {
                user.Annotations = new AnnotationList(sUser.Annotations
                    .Select(LoadAnnotation)
                    .Where(a => a is not null)
                    .ToList()!);
            }
            if (sUser.Heuristics is not null)
            {
                user.Heuristics.UnionWith(sUser.Heuristics
                    .Where(h => !(h.Name is null))
                    .Select(h => h.Name!));
            }
            if (sUser.AggressiveBranchRemoval)
            {
                user.Heuristics.Add("aggressive-branch-removal");
            }

            if (sUser.TextEncoding is not null)
            {
                Encoding? enc = null;
                try
                {
                    enc = Encoding.GetEncoding(sUser.TextEncoding);
                }
                catch
                {
                    listener.Warn(
                        "Unknown text encoding '{0}'. Defaulting to platform text encoding.", 
                        sUser.TextEncoding);
                }
                user.TextEncoding = enc;
            }
            program.EnvironmentMetadata = project.LoadedMetadata;
            if (sUser.Calls is not null)
            {
                program.User.Calls = sUser.Calls
                    .Select(c => LoadUserCall(c, program))
                    .Where(c => c is not null)
                    .ToSortedList(k => k!.Address!, v => v!);
            }
            if (sUser.RegisterValues is not null)
            {
                program.User.RegisterValues = LoadRegisterValues(sUser.RegisterValues);
            }
            if (sUser.JumpTables is not null)
            {
                program.User.JumpTables = sUser.JumpTables.Select(LoadJumpTable_v4)
                    .Where(t => t is not null)
                    .ToSortedList(k => k!.Address, v => v)!;
            }
            if (sUser.IndirectJumps is not null)
            {
                program.User.IndirectJumps = sUser.IndirectJumps
                    .Select(ij => LoadIndirectJump_v4(ij, program))
                    .Where(ij => ij.Item2 is not null)
                    .ToSortedList(k => k!.Item1!, v => v!.Item2)!;
            }
            if (sUser.Segments is not null)
            {
                program.User.Segments = sUser.Segments
                    .Select(s => LoadUserSegment(s))
                    .Where(s => s is not null)
                    .ToList()!;
            }
            if (sUser.DebugTraceProcedures is not null)
            {
                program.User.DebugTraceProcedures = sUser.DebugTraceProcedures
                    .Where(s => s is not null)
                    .ToHashSet()!;
            }
            if (sUser.BlockLabels is not null)
            {
                program.User.BlockLabels = sUser.BlockLabels
                    .Where(u => u.Location is not null)
                    .ToDictionary(u => u.Location!, u => u.Name!);
            }
            program.User.ShowAddressesInDisassembly = sUser.ShowAddressesInDisassembly;
            program.User.ShowBytesInDisassembly = sUser.ShowBytesInDisassembly;
            program.User.RenderInstructionsCanonically = sUser.RenderInstructionsCanonically;
            program.User.ExtractResources = sUser.ExtractResources;
            // Backwards compatibility: older versions used single file policy.
            program.User.OutputFilePolicy = sUser.OutputFilePolicy ?? Program.SingleFilePolicy;
        }

        private Annotation? LoadAnnotation(Annotation_v3 annotation)
        {
            if (!arch!.TryParseAddress(annotation.Address, out var address))
                return null;
            return new Annotation(address!, annotation.Text ?? "");
        }

        private SortedList<Address, List<UserRegisterValue>> LoadRegisterValues(
            RegisterValue_v2[] sRegValues)
        {
            Storage? GetStorage(string? name)
            {
                if (name is null)
                    return null;
                if (platform.Architecture.TryGetRegister(name, out var reg))
                    return reg;
                return platform.Architecture.GetFlagGroup(name);
            }

            var allLists = new SortedList<Address, List<UserRegisterValue>>();
            foreach (var sRegValue in sRegValues)
            {
                if (sRegValue is not null && platform!.TryParseAddress(sRegValue.Address, out Address addr))
                {
                    if (!allLists.TryGetValue(addr, out var list))
                    {
                        list = new List<UserRegisterValue>();
                        allLists.Add(addr, list);
                    }
                    var stg = GetStorage(sRegValue.Register);
                    if (stg is not null)
                    {
                        var c = sRegValue.Value != "*"
                            ? Constant.Create(stg.DataType, Convert.ToUInt64(sRegValue.Value, 16))
                            : InvalidConstant.Create(stg.DataType);
                        list.Add(new UserRegisterValue(stg, c));
                    }
                }
            }
            return allLists;
        }

        private ImageMapVectorTable? LoadJumpTable_v4(JumpTable_v4 sTable)
        {
            if (platform is null || !platform.TryParseAddress(sTable.TableAddress, out Address addr))
                return null;
            var listAddrDst = new List<Address>();
            if (sTable.Destinations is not null)
            {
                foreach (var item in sTable.Destinations)
                {
                    if (!platform.TryParseAddress(item, out Address addrDst))
                        break;
                    listAddrDst.Add(addrDst);
                }
            }
            return new ImageMapVectorTable(addr, listAddrDst.ToArray(), 0);
        }

        private UserGlobal? LoadUserGlobal(GlobalDataItem_v2 sGlobal)
        {
            if (!arch!.TryParseAddress(sGlobal.Address, out var address))
                return null; // TODO: Emit warning?

            if (sGlobal.DataType is null)
                throw new ArgumentException("Missing required field DataType");

            string name = GlobalNameOrDefault(sGlobal, address);

            var ug = new UserGlobal(address, name, sGlobal.DataType!)
            {
                Comment = sGlobal.Comment,
            };
            return ug;
        }

        private string GlobalNameOrDefault(GlobalDataItem_v2 sGlobal, Address address)
        {
            if (!string.IsNullOrWhiteSpace(sGlobal.Name))
                return sGlobal.Name;

            return UserGlobal.GenerateDefaultName(address);
        }

        private UserCallData? LoadUserCall(SerializedCall_v1 call, Program program)
        {
            if (!program.Platform.TryParseAddress(call.InstructionAddress, out Address addr))
                return null;

            var procSer = program.CreateProcedureSerializer();
            FunctionType? sig = null;
            if (call.Signature is not null)
            {
                sig = procSer.Deserialize(
                   call.Signature,
                   program.Architecture.CreateFrame());
            }
            return new UserCallData
            {
                Address = addr,
                Comment = call.Comment,
                NoReturn = call.NoReturn,
                Signature = sig,
            };
        }

        private (Address, UserIndirectJump?) LoadIndirectJump_v4(IndirectJump_v4 indirJump, Program program)
        {
            if (!platform!.TryParseAddress(indirJump.InstructionAddress, out Address addrInstr))
                return (default, null);
            if (!platform.TryParseAddress(indirJump.TableAddress, out Address addrTable))
                return (default, null);
            if (!program.User.JumpTables.TryGetValue(addrTable, out var table))
                return (default, null);
            if (indirJump.IndexRegister is null)
                return (default, null);
            var reg = program.Architecture.GetRegister(indirJump.IndexRegister);
            if (reg is null)
                return (default, null);
            return (addrInstr, new UserIndirectJump
            {
                Address = addrInstr,
                Table = table,
                IndexRegister = reg,
            });
        }

        /// <summary>
        /// Loads a user segment from the serialized representation.
        /// </summary>
        /// <param name="sSegment">Serialized representation of a user segment.
        /// </param>
        /// <returns>A deserialized user segment.
        /// </returns>
        public UserSegment? LoadUserSegment(Segment_v4 sSegment)
        {
            if (!platform!.TryParseAddress(sSegment.Address, out Address addr))
                return null;
            ulong offset;
            if (string.IsNullOrEmpty(sSegment.Offset))
            {
                offset = 0;
            }
            else
            {
                if (!ulong.TryParse(sSegment.Offset, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out offset))
                    return null;
            }
            if (string.IsNullOrEmpty(sSegment.Length))
                return null;
            if (!uint.TryParse(sSegment.Length, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint length))
                return null;

            var arch = Services.RequireService<IConfigurationService>().GetArchitecture(sSegment.Architecture!);

            var access = LoadAccessMode(sSegment.Access);

            return new UserSegment
            {
                Name = sSegment.Name,
                Address = addr,
                Offset = offset,
                Length = length,
                Architecture = arch,
                AccessMode = access
            };
        }

        private AccessMode LoadAccessMode(string? sMode)
        {
            if (string.IsNullOrWhiteSpace(sMode))
                return AccessMode.ReadWriteExecute;
            AccessMode mode = 0;
            foreach (char c in sMode!)
            {
                switch (c)
                {
                case 'r': mode |= AccessMode.Read; break;
                case 'w': mode |= AccessMode.Write; break;
                case 'x': mode |= AccessMode.Execute; break;
                }
            }
            return mode;
        }

        private UserProcedure? LoadUserProcedure(
            Program program,
            Procedure_v1 sup)
        {
            if (!program.Architecture.TryParseAddress(sup.Address, out Address addr))
                return null;

            if (!sup.Decompile && sup.Signature is null && string.IsNullOrEmpty(sup.CSignature))
            {
                listener.Warn(
                    listener.CreateAddressNavigator(program, addr),
                    "User procedure '{0}' has been marked 'no decompile' but its signature " +
                    "has not been specified.",
                    sup.Name ?? "<unnamed>");
            }

            string name = sup.Name ?? NamingPolicy.Instance.ProcedureName(addr);

            var up = new UserProcedure(addr, name)
            {
                Ordinal = sup.Ordinal,
                Signature = sup.Signature,
                Characteristics = sup.Characteristics ?? new ProcedureCharacteristics(),
                Decompile = sup.Decompile,
                Assume = sup.Assume?.ToList() ?? new List<RegisterValue_v2>(),
                CSignature = sup.CSignature,
                OutputFile = sup.OutputFile
            };

            return up;
        }

        /// <summary>
        /// Processes a loaded metadata descriptor.
        /// </summary>
        /// <param name="sMetadata">Metadata descriptor.</param>
        /// <returns>Loaded metadata file.
        /// </returns>
        public MetadataFile VisitMetadataFile(MetadataFile_v3 sMetadata)
        {
            //$BUG: what if both sMetaData.Uri and sMetata.Filename are null?
            ImageLocation metadataUri;
            if (!string.IsNullOrEmpty(sMetadata.Location))
            {
                metadataUri = ConvertToAbsoluteLocation(project.Location, sMetadata.Location)!;
            }
            else
            {
                metadataUri = ConvertToAbsoluteLocation(project.Location, sMetadata.Filename)!;
            }
            return LoadMetadataFile(metadataUri);
        }

        /// <summary>
        /// Loads a metadata file from the given location.
        /// If it fails, it will return an empty TypeLibrary object.
        /// </summary>
        /// <param name="metadataUri"></param>
        /// <returns></returns>
        public MetadataFile LoadMetadataFile(ImageLocation metadataUri)
        {
            var platform = DeterminePlatform();
            this.project.LoadedMetadata = 
                loader.LoadMetadata(metadataUri, platform, this.project.LoadedMetadata)
                ?? new TypeLibrary();   // was able to load before, but not now?
            return new MetadataFile
            {
                Location = metadataUri,
            };
        }

        private IPlatform DeterminePlatform()
        {
            // If a platform was defined for the whole project use that.
            if (this.platform is not null)
                return this.platform;

            // Otherwise try to guess the platform or ask the user.
            // (this code will soon go away).
            var platformsInUse = project.Programs.Select(p => p.Platform).Distinct().ToArray();
            if (platformsInUse.Length == 1 && platformsInUse[0] is not null)
                return platformsInUse[0];
            if (platformsInUse.Length == 0)
                throw new NotImplementedException("Must specify platform for project.");
            throw new NotImplementedException("Multiple platforms possible; not implemented yet.");
        }

        /// <summary>
        /// Processes a script file.
        /// </summary>
        /// <param name="sScript">Script file definition.</param>
        /// <returns>Loaded script file.</returns>
        public ScriptFile? VisitScriptFile(ScriptFile_v5 sScript)
        {
            ImageLocation scriptLocation;
            if (!string.IsNullOrEmpty(sScript.Location))
            {
                scriptLocation = ConvertToAbsoluteLocation(project.Location, sScript.Location)!;
            }
            else if (!string.IsNullOrEmpty(sScript.Filename))
            {
                // No URI present, this is an older project file.
                var scriptAbsPath = ConvertToAbsolutePath(project.Location.FilesystemPath, sScript.Filename);
                scriptLocation = ImageLocation.FromUri(scriptAbsPath!);
            }
            else 
                return null;
            return loader.LoadScript(scriptLocation);
        }

        /// <summary>
        /// TODO
        /// </summary>
        public Program VisitAssemblerFile(AssemblerFile_v3 sAsmFile)
        {
            throw new NotImplementedException("return loader.AssembleExecutable(sAsmFile.Filename, sAsmFile.Assembler, null);");
        }
    }
}
