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
using Reko.Core.Loading;
using Reko.Core.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// This class saves a <see cref="Project"/> to an XML file.
    /// </summary>
    public class ProjectSaver : ProjectPersister
    {
        /// <summary>
        /// Creates an instance of the <see cref="ProjectSaver"/> class.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance.</param>
        public ProjectSaver(IServiceProvider services) : base(services)
        {
        }

        /// <summary>
        /// Saves a V5 project to the given <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="sProject">Serialize project to save.
        /// </param>
        /// <param name="xw">Output sink.</param>
        public void Save(Project_v5 sProject, XmlWriter xw)
        {
            var ser = SerializedLibrary.CreateSerializer_v5(typeof(Project_v5));
            ser.Serialize(xw, sProject);
        }

        /// <summary>
        /// Given a <see cref="Project"/> serializes it into a <see cref="Project_v5"/>. 
        /// </summary>
        /// <param name="projectLocation">The URI of the project file.</param>
        /// <param name="project">A <see cref="Project"/> instance.</param>
        /// <returns></returns>
        public Project_v5 Serialize(ImageLocation projectLocation, Project project)
        {
            var inputFiles = project.Programs.Select(
                p => VisitProgram(projectLocation, p)).ToList();
            var metadataFiles = project.MetadataFiles.Select(
                m => VisitMetadataFile(projectLocation, m)).ToList();
            var scriptFiles = project.ScriptFiles.Select(
                s => VisitScriptFile(projectLocation, s)).ToList();
            var sp = new Project_v5
            {
                // ".Single()" because there can be only one Architecture and Platform, realistically.
                ArchitectureName = project.Programs.Select(p => p.Architecture.Name).Distinct().SingleOrDefault(),
                PlatformName = project.Programs.Select(p => p.Platform.Name).Distinct().SingleOrDefault(),   
                InputFiles = inputFiles,
                MetadataFiles = metadataFiles,
                ScriptFiles = scriptFiles,
            };
            return sp;
        }

        /// <inheritdoc/>
        public DecompilerInput_v5 VisitProgram(ImageLocation projectLocation, Program program)
        {
            var projectPath = projectLocation.FilesystemPath;
            return new DecompilerInput_v5
            {
                Location = ConvertToProjectRelativeUri(projectLocation, program.Location),
                User = new UserData_v4
                {
                    Loader = program.User.Loader,
                    Procedures = program.User.Procedures
                        .Select(up => SerializeUserProcedure(up.Value))
                        .ToList(),
                    Processor = SerializeProcessorOptions(program.User, program.Architecture),
                    PlatformOptions = SerializePlatformOptions(program.User, program.Platform),
                    LoadAddress = program.User.LoadAddress?.ToString(),
                    Calls = program.User.Calls
                        .Select(uc => SerializeUserCall(program, uc.Value))
                        .Where(uc => uc is not null)
                        .Select(uc => uc!)
                        .ToList(),
                    IndirectJumps = program.User.IndirectJumps.Select(SerializeIndirectJump).ToList(),
                    JumpTables = program.User.JumpTables.Select(SerializeJumpTable).ToList(),
                    GlobalData = program.User.Globals.Select(i => SerializeGlobal(i.Value)).ToList(),
                    OnLoadedScript = program.User.OnLoadedScript,
                    Heuristics = program.User.Heuristics
                        .Select(h => new Heuristic_v3 { Name = h }).ToList(),
                    Annotations = program.User.Annotations.Select(SerializeAnnotation).ToList(),
                    TextEncoding = program.User.TextEncoding != Encoding.ASCII ? program.User.TextEncoding?.WebName : null,
                    RegisterValues = SerializeRegisterValues(program.User.RegisterValues),
                    RenderInstructionsCanonically = program.User.RenderInstructionsCanonically,
                    ShowAddressesInDisassembly = program.User.ShowAddressesInDisassembly,
                    ShowBytesInDisassembly = program.User.ShowBytesInDisassembly,
                    Segments = program.User.Segments.Select(SerializeSegment).ToList(),
                    ExtractResources = program.User.ExtractResources,
                    OutputFilePolicy = program.User.OutputFilePolicy,
                    DebugTraceProcedures = program.User.DebugTraceProcedures.ToList(),
                },
                DisassemblyDirectory =  ConvertToProjectRelativePath(projectPath, program.DisassemblyDirectory),
                SourceDirectory =       ConvertToProjectRelativePath(projectPath, program.SourceDirectory),
                IncludeDirectory =      ConvertToProjectRelativePath(projectPath, program.IncludeDirectory),
                ResourcesDirectory =    ConvertToProjectRelativePath(projectPath, program.ResourcesDirectory),
            };
        }

        private Procedure_v1 SerializeUserProcedure(UserProcedure up)
        {
            var sp = new Procedure_v1()
            {
                Name = up.Name,
                Ordinal = up.Ordinal,
                Signature = up.Signature,
                Characteristics = SerializeProcedureCharacteristics(up.Characteristics),
                Address = up.Address.ToString(),
                Decompile = up.Decompile,
                Assume = up.Assume.Count != 0 ? up.Assume.ToArray() : null,
                CSignature = up.CSignature,
                OutputFile = up.OutputFile,
            };

            return sp;
        }

        private ProcedureCharacteristics? SerializeProcedureCharacteristics(ProcedureCharacteristics? pc)
        {
            if (pc is null || pc.IsDefaultCharactaristics)
                return null;

            return pc;
        }

        private RegisterValue_v2[] SerializeRegisterValues(SortedList<Address, List<UserRegisterValue>> registerValues)
        {
            var sRegValues = new List<RegisterValue_v2>();
            foreach (var de in registerValues)
            {
                var sAddr = de.Key.ToString();
                foreach (var rv in de.Value)
                {
                    var reg = rv.Register;
                    var regValue = rv.Value;
                    if (reg is null || regValue is null)
                        continue;
                    var value = regValue.IsValid
                        ? string.Format($"{{0:X{reg.DataType.Size * 2}}}", regValue.ToUInt64())
                        : "*";
                    sRegValues.Add(new RegisterValue_v2
                    {
                        Address = sAddr,
                        Register = reg.Name,
                        Value = value,
                    });
                }
            }
            return sRegValues.ToArray();
        }

        private SerializedCall_v1? SerializeUserCall(Program program, UserCallData? uc)
        {
            if (uc is null)
                return null;
            var procser = program.CreateProcedureSerializer();
            SerializedSignature? ssig = null;
            if (uc.Signature is not null)
            {
                ssig = procser.Serialize(uc.Signature);
            }
            return new SerializedCall_v1
            {
                InstructionAddress = uc.Address.ToString(),
                Comment = uc.Comment,
                NoReturn = uc.NoReturn,
                Signature = ssig,
            };
        }

        private IndirectJump_v4 SerializeIndirectJump(KeyValuePair<Address, UserIndirectJump> de)
        {
            return new IndirectJump_v4
            {
                InstructionAddress = de.Key.ToString(),
                TableAddress = de.Value.Address.ToString(),
                IndexRegister = de.Value.IndexRegister?.Name,
            };
        }

        private JumpTable_v4 SerializeJumpTable(KeyValuePair<Address, ImageMapVectorTable> de)
        {
            return new JumpTable_v4
            {
                TableAddress = de.Key.ToString(),
                Destinations = de.Value.Addresses.Select(a => a.ToString()).ToArray(),
            };
        }

        private GlobalDataItem_v2 SerializeGlobal(UserGlobal global)
        {
            return new GlobalDataItem_v2
            {
                Address = global.Address.ToString(),
                Comment = global.Comment,
                DataType = global.DataType,
                Name  = global.Name,
            };
        }

        private Segment_v4 SerializeSegment(UserSegment segment)
        {
            string access = ((segment.AccessMode & AccessMode.Read) != 0 ? "r" : "") +
                            ((segment.AccessMode & AccessMode.Write) != 0 ? "w" : "") +
                            ((segment.AccessMode & AccessMode.Execute) != 0 ? "x" : "");
            return new Segment_v4
            {
                Address = segment.Address.ToString(),
                Offset = segment.Offset.ToString("X"),
                Length = segment.Length.ToString("X"),
                Name = segment.Name,
                Architecture = segment.Architecture?.ToString(),
                Access = access,
            };
        }

        private Annotation_v3 SerializeAnnotation(Annotation arg)
        {
            return new Annotation_v3
            {
                Address = arg.Address.ToString(),
                Text = arg.Text,
            };
        }

        private ProcessorOptions_v4? SerializeProcessorOptions(UserData user, IProcessorArchitecture architecture)
        {
            if (architecture is null)
                return null;
            var options = architecture.SaveUserOptions();
            if (string.IsNullOrEmpty(user.Processor) && options is null)
                return null;
            else
            {
                var doc = new XmlDocument();
                var xml = SerializeValue(options, doc);
                var elems = xml is not null
                    ? xml.ChildNodes.OfType<XmlElement>().ToArray()
                    : Array.Empty<XmlElement>();
                return new ProcessorOptions_v4 {
                    Name = user.Processor,
                    Options = elems
                };
            }
        }

        private PlatformOptions_v4? SerializePlatformOptions(UserData user, IPlatform platform)
        {
            if (platform is null)
                return null;
            var dictionary = platform.SaveUserOptions();
            if (dictionary is null)
            {
                if (string.IsNullOrEmpty(user.Environment))
                    return null;
                else
                    return new PlatformOptions_v4
                    {
                        Name = user.Environment
                    };
            }
            var doc = new XmlDocument();
            return new PlatformOptions_v4
            {
                Name = user.Environment,
                Options = SerializeValue(dictionary, doc)!
                    .ChildNodes
                    .OfType<XmlElement>()
                    .ToArray()
            };
        }

        private XmlElement? SerializeOptionValue(string key, object value, XmlDocument doc)
        {
            var el = SerializeValue(value, doc);
            if (el is not null)
            {
                el.SetAttribute("key", "", key);
            }
            return el;
        }

        private XmlElement? SerializeValue(object? value, XmlDocument doc)
        {
            if (value is null)
                return null;
            XmlElement el;
            switch (value)
            {
            case string sValue:
                el = doc.CreateElement("item", SerializedLibrary.Namespace_v5);
                el.InnerXml = sValue;
                return el;
            case IDictionary dict:
                el = doc.CreateElement("dict", SerializedLibrary.Namespace_v5);
                foreach (DictionaryEntry de in dict)
                {
                    var sub = SerializeValue(de.Value, doc);
                    if (sub is not null)
                    {
                        sub.SetAttribute("key", de.Key.ToString());
                        el.AppendChild(sub);
                    }
                }
                return el;
            case IEnumerable ienum:
                el = doc.CreateElement("list", SerializedLibrary.Namespace_v5);
                foreach (var oValue in ienum)
                {
                    el.AppendChild(SerializeValue(oValue, doc)!);
                }
                return el;
            default:
                el = doc.CreateElement("item", SerializedLibrary.Namespace_v5);
                el.InnerText = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)!;
                return el;
            }
        }

        /// <inheritdoc />
        public MetadataFile_v3 VisitMetadataFile(ImageLocation projectLocation, MetadataFile metadata)
        {
            return new MetadataFile_v3
            {
                Location = ConvertToProjectRelativeUri(projectLocation, metadata.Location!),
                ModuleName = metadata.ModuleName,
            };
        }

        private ScriptFile_v5 VisitScriptFile(ImageLocation projectLocation, ScriptFile script)
        {
            return new ScriptFile_v5
            {
                Location = ConvertToProjectRelativeUri(projectLocation, script.Location!),
            };
        }
    }
}
