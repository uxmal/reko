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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Reko.Core.Serialization
{
    public class ProjectSaver : ProjectPersister
    {
        public ProjectSaver(IServiceProvider services) : base(services)
        {
        }

        public void Save(Project_v5 sProject, XmlWriter xw)
        {
            var ser = SerializedLibrary.CreateSerializer_v5(typeof(Project_v5));
            ser.Serialize(xw, sProject);
        }

        /// <summary>
        /// Given a <see cref="Project"/> serializes it into a <see cref="Project_v5"/>. 
        /// </summary>
        /// <param name="projectAbsPath"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        public Project_v5 Serialize(string projectAbsPath, Project project)
        {
            var inputs = new List<ProjectFile_v3>();
            inputs.AddRange(project.Programs.Select(p => VisitProgram(projectAbsPath, p)));
            inputs.AddRange(project.MetadataFiles.Select(m => VisitMetadataFile(projectAbsPath, m)));
            var sp = new Project_v5
            {
                // ".Single()" because there can be only one Architecture and Platform, realistically.
                ArchitectureName = project.Programs.Select(p => p.Architecture.Name).Distinct().SingleOrDefault(),
                PlatformName = project.Programs.Select(p => p.Platform.Name).Distinct().SingleOrDefault(),   
                Inputs = inputs
            };
            return sp;
        }

        public ProjectFile_v3 VisitProgram(string projectAbsPath, Program program)
        {
            return new DecompilerInput_v5
            {
                Filename = ConvertToProjectRelativePath(projectAbsPath, program.Filename),
                User = new UserData_v4
                {
                    Loader = program.User.Loader,
                    Procedures = program.User.Procedures
                        .Select(de => { de.Value.Address = de.Key.ToString(); return de.Value; })
                        .ToList(),
                    Processor = SerializeProcessorOptions(program.User, program.Architecture),
                    PlatformOptions = SerializePlatformOptions(program.User, program.Platform),
                    LoadAddress = program.User.LoadAddress?.ToString(),
                    Calls = program.User.Calls
                        .Select(uc => SerializeUserCall(program, uc.Value))
                        .Where(uc => uc != null)
                        .ToList(),
                    IndirectJumps = program.User.IndirectJumps.Select(SerializeIndirectJump).ToList(),
                    JumpTables = program.User.JumpTables.Select(SerializeJumpTable).ToList(),
                    GlobalData = program.User.Globals
                        .Select(de => new GlobalDataItem_v2
                        {
                            Address = de.Key.ToString(),
                            DataType = de.Value.DataType,
                            Name = GlobalName(de),
                        })
                        .ToList(),
                    OnLoadedScript = program.User.OnLoadedScript,
                    Heuristics = program.User.Heuristics
                        .Select(h => new Heuristic_v3 { Name = h }).ToList(),
                    Annotations = program.User.Annotations.Select(SerializeAnnotation).ToList(),
                    TextEncoding = program.User.TextEncoding != Encoding.ASCII ? program.User.TextEncoding.WebName : null,
                    RegisterValues = SerializeRegisterValues(program.User.RegisterValues),
                    ShowAddressesInDisassembly = program.User.ShowAddressesInDisassembly,
                    ShowBytesInDisassembly = program.User.ShowBytesInDisassembly,
                    ExtractResources = program.User.ExtractResources,
                },
                DisassemblyDirectory =  ConvertToProjectRelativePath(projectAbsPath, program.DisassemblyDirectory),
                SourceDirectory =       ConvertToProjectRelativePath(projectAbsPath, program.SourceDirectory),
                IncludeDirectory =      ConvertToProjectRelativePath(projectAbsPath, program.IncludeDirectory),
                ResourcesDirectory =    ConvertToProjectRelativePath(projectAbsPath, program.ResourcesDirectory),
            };
        }

        private RegisterValue_v2[] SerializeRegisterValues(SortedList<Address, List<UserRegisterValue>> registerValues)
        {
            var sRegValues = new List<RegisterValue_v2>();
            foreach (var de in registerValues)
            {
                var sAddr = de.Key.ToString();
                foreach (var rv in de.Value)
                {
                    sRegValues.Add(new RegisterValue_v2
                    {
                        Address = sAddr,
                        Register = rv.Register.Name,
                        Value = rv.Value.ToString().Replace("0x", ""),
                    });
                }
            }
            return sRegValues.ToArray();
        }

        private string GlobalName(KeyValuePair<Address, GlobalDataItem_v2> de)
        {
            var name = de.Value.Name;
            if (string.IsNullOrEmpty(name))
            {
                name = string.Format("g_{0:X}", de.Key.ToLinear());
            }
            return name;
        }

        private SerializedCall_v1 SerializeUserCall(Program program, UserCallData uc)
        {
            if (uc == null || uc.Address == null)
                return null;
            var procser = program.CreateProcedureSerializer();
            SerializedSignature ssig = null;
            if (uc.Signature != null)
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
                IndexRegister = de.Value.IndexRegister.Name,
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

        private Annotation_v3 SerializeAnnotation(Annotation arg)
        {
            return new Annotation_v3
            {
                Address = arg.Address.ToString(),
                Text = arg.Text,
            };
        }

        private ProcessorOptions_v4 SerializeProcessorOptions(UserData user, IProcessorArchitecture architecture)
        {
            if (architecture == null)
                return null;
            var options = architecture.SaveUserOptions();
            if (string.IsNullOrEmpty(user.Processor) && options == null)
                return null;
            else
            {
                var doc = new XmlDocument();
                var xml = SerializeValue(options, doc);
                var elems = xml != null
                    ? xml.ChildNodes.OfType<XmlElement>().ToArray()
                    : new XmlElement[0];
                return new ProcessorOptions_v4 {
                    Name = user.Processor,
                    Options = elems
                };
            }
        }

        private PlatformOptions_v4 SerializePlatformOptions(UserData user, IPlatform platform)
        {
            if (platform == null)
                return null;
            var dictionary = platform.SaveUserOptions();
            if (dictionary == null)
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
                Options = SerializeValue(dictionary, doc)
                    .ChildNodes
                    .OfType<XmlElement>()
                    .ToArray()
            };
        }

        private XmlElement SerializeOptionValue(string key, object value, XmlDocument doc)
        {
            var el = SerializeValue(value, doc);
            el.SetAttribute("key", "", key);
            return el;
        }

        private XmlElement SerializeValue(object value, XmlDocument doc)
        {
            if (value == null)
                return null;
            if (value is string sValue)
            {
                var el = doc.CreateElement("item", SerializedLibrary.Namespace_v5);
                el.InnerXml = (string)value;
                return el;
            }
            if (value is IDictionary dict)
            {
                var el = doc.CreateElement("dict", SerializedLibrary.Namespace_v5);
                foreach (DictionaryEntry de in dict)
                {
                    var sub = SerializeValue(de.Value, doc);
                    sub.SetAttribute("key", de.Key.ToString());
                    el.AppendChild(sub);
                }
                return el;
            }
            if (value is IEnumerable ienum)
            {
                var el = doc.CreateElement("list", SerializedLibrary.Namespace_v5);
                foreach (var oValue in ienum)
                {
                    el.AppendChild(SerializeValue(oValue, doc));
                }
                return el;
            }
            throw new NotSupportedException(value.GetType().Name);
        }

        public ProjectFile_v3 VisitMetadataFile(string projectAbsPath, MetadataFile metadata)
        {
            return new MetadataFile_v3
            {
                 Filename = ConvertToProjectRelativePath(projectAbsPath, metadata.Filename),
                  ModuleName = metadata.ModuleName,
            };
        }
    }
}
