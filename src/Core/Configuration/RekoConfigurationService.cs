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

using Reko.Core.Assemblers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Reko.Core.Configuration
{
    /// <summary>
    /// Client code uses this service to access information stored 
    /// in the reko.config file.
    /// </summary>
    public interface IConfigurationService
    {
         ICollection<LoaderDefinition> GetImageLoaders();
         ICollection<ArchitectureDefinition> GetArchitectures();
         ICollection<PlatformDefinition> GetEnvironments();
         ICollection<SignatureFileDefinition> GetSignatureFiles();
         ICollection<AssemblerDefinition> GetAssemblers();
         ICollection<RawFileDefinition> GetRawFiles();

         PlatformDefinition GetEnvironment(string envName);
         IProcessorArchitecture GetArchitecture(string archLabel);
         ICollection<SymbolSourceDefinition> GetSymbolSources();
         Assembler GetAssembler(string assemblerName);
         RawFileDefinition GetRawFile(string rawFileFormat);

         IEnumerable<UiStyleDefinition> GetDefaultPreferences ();

         /// <summary>
         /// Given a relative path with respect to the installation directory, 
         /// returns the absolute path.
         /// </summary>
         /// <param name="path"></param>
         /// <returns></returns>
         string GetInstallationRelativePath(params string [] pathComponents);
        LoaderDefinition GetImageLoader(string loader);
    }

    public class RekoConfigurationService : IConfigurationService
    {
        private readonly List<LoaderDefinition> loaders;
        private readonly List<SignatureFileDefinition> sigFiles;
        private readonly List<ArchitectureDefinition> architectures;
        private readonly List<PlatformDefinition> opEnvs;
        private readonly List<AssemblerDefinition> asms;
        private readonly List<SymbolSourceDefinition> symSources;
        private readonly List<RawFileDefinition> rawFiles;
        private readonly UiPreferencesConfiguration uiPreferences;

        public RekoConfigurationService(RekoConfiguration_v1 config)
        {
            this.loaders = LoadCollection(config.Loaders, LoadLoaderConfiguration);
            this.sigFiles = LoadCollection(config.SignatureFiles, LoadSignatureFile);
            this.architectures = LoadCollection(config.Architectures, LoadArchitecture);
            this.opEnvs = LoadCollection(config.Environments, LoadEnvironment);
            this.asms = LoadCollection(config.Assemblers, LoadAssembler);
            this.symSources = LoadCollection(config.SymbolSources, LoadSymbolSource);
            this.rawFiles = LoadCollection(config.RawFiles, LoadRawFile);
            this.uiPreferences = new UiPreferencesConfiguration();
            if (config.UiPreferences != null)
            {
                this.uiPreferences.Styles =
                    LoadCollection(config.UiPreferences.Styles, LoadUiStyle);
            };
        }

        private LoaderDefinition LoadLoaderConfiguration(RekoLoader l)
        {
            return new LoaderDefinition
            {
                Argument = l.Argument,
                Extension = l.Extension,
                Offset = ConvertNumber(l.Offset),
                Label = l.Label,
                MagicNumber = l.MagicNumber,
                TypeName = l.Type
            };
        }

        private SignatureFileDefinition LoadSignatureFile(SignatureFile_v1 sSig)
        {
            return new SignatureFileDefinition
            {
                Filename = sSig.Filename,
                Label = sSig.Label,
                Type = sSig.Type,
            };
        }

        private ArchitectureDefinition LoadArchitecture(Architecture_v1 sArch)
        {
            return new ArchitectureDefinition
            {
                Description = sArch.Description,
                Name = sArch.Name,
                TypeName = sArch.Type,
                Options = LoadCollection(sArch.Options, LoadPropertyOption),
            };
        }

        private PropertyOption LoadPropertyOption(PropertyOption_v1 sOption)
        {
            return new PropertyOption
            {
                Name = sOption.Name,
                Text = sOption.Text,
                Description = sOption.Description,
                Required = sOption.Required,
                TypeName = sOption.TypeName,
                Choices = sOption.Choices
            };
        }

        private PlatformDefinition LoadEnvironment(Environment_v1 env)
        {
            return new PlatformDefinition
            {
                Name = env.Name,
                Description = env.Description,
                MemoryMapFile = env.MemoryMap,
                TypeName = env.Type,
                Heuristics = env.Heuristics,
                TypeLibraries = LoadCollection(env.TypeLibraries, LoadTypeLibraryReference),
                CharacteristicsLibraries = LoadCollection(env.Characteristics, LoadTypeLibraryReference),
                Architectures = LoadCollection(env.Architectures, LoadPlatformArchitecture),
                Options = env.Options != null
                    ? XmlOptions.LoadIntoDictionary(env.Options
                        .SelectMany(o => o.ChildNodes.OfType<XmlElement>())
                        .ToArray(),
                        StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            };
        }

        private AssemblerDefinition LoadAssembler(Assembler_v1 sAsm)
        {
            return new AssemblerDefinition
            {
                Description = sAsm.Description,
                Name = sAsm.Name,
                TypeName = sAsm.Type,
            };
        }

        private SymbolSourceDefinition LoadSymbolSource(SymbolSource_v1 sSymSrc)
        {
            return new SymbolSourceDefinition
            {
                Description = sSymSrc.Description,
                Name = sSymSrc.Name,
                Extension = sSymSrc.Extension,
                TypeName = sSymSrc.Type,
            };
        }

        private TypeLibraryDefinition LoadTypeLibraryReference(TypeLibraryReference_v1 tlibRef)
        {
            return new TypeLibraryDefinition
            {
                Architecture = tlibRef.Arch,
                Loader = tlibRef.Loader,
                Module = tlibRef.Module,
                Name = tlibRef.Name,
            };
        }

        /// <summary>
        /// Loads processor-specific settings for a particular 
        /// platform.
        /// </summary>
        private PlatformArchitectureDefinition LoadPlatformArchitecture(PlatformArchitecture_v1 spa)
        {
            var sTrashedRegs = spa.TrashedRegisters ?? "";
            var sLibraries = spa.TypeLibraries ?? new TypeLibraryReference_v1[0];
            return new PlatformArchitectureDefinition
            {
                Name = spa.Name,
                TrashedRegisters = sTrashedRegs
                    .Split(',')
                    .Select(s => s.Trim())
                    .ToList(),
                TypeLibraries = LoadCollection(sLibraries, LoadTypeLibraryReference)
            };
        }

        private RawFileDefinition LoadRawFile(RawFile_v1 sRaw)
        {
            return new RawFileDefinition
            {
                Architecture = sRaw.Architecture,
                BaseAddress = sRaw.Base,
                Description = sRaw.Description,
                EntryPoint = LoadEntryPoint(sRaw.Entry),
                Environment = sRaw.Environment,
                Loader = sRaw.LoaderType,
                Name = sRaw.Name,
            };
        }

        private EntryPointDefinition LoadEntryPoint(EntryPoint_v1 sEntry)
        {
            if (sEntry == null)
            {
                return new EntryPointDefinition
                {
                    Address = null,
                    Follow = false,
                    Name = null,
                };
            }
            return new EntryPointDefinition
            {
                Address = sEntry.Address,
                Follow = sEntry.Follow,
                Name = sEntry.Name,
            };
        }

        private UiStyleDefinition LoadUiStyle(StyleDefinition_v1 sUiStyle)
        {
            return new UiStyleDefinition
            {
                Name = sUiStyle.Name,
                BackColor = sUiStyle.BackColor,
                Cursor = sUiStyle.Cursor,
                FontName = sUiStyle.Font,
                ForeColor = sUiStyle.ForeColor,
                TextAlign = sUiStyle.TextAlign,
                Width = sUiStyle.Width,
                PaddingTop = sUiStyle.PaddingTop,
                PaddingLeft = sUiStyle.PaddingLeft,
                PaddingBottom = sUiStyle.PaddingBottom,
                PaddingRight = sUiStyle.PaddingRight,
            };
        }

        private List<TDst> LoadCollection<TSrc, TDst>(TSrc[] sItems, Func<TSrc, TDst> fn)
        {
            if (sItems == null)
                return new List<TDst>();
            else
                return sItems.Select(fn).ToList();
        }

        /// <summary>
        /// Load the reko.config file.
        /// </summary>
        /// <remarks>
        /// For now, we assume the config file is called "reko.config" located in the same directory as the assembly we're executing.
        /// It's possible that on Un*x systems, the Reko binary could be installed in /usr/bin, while 
        /// </remarks>
        /// <returns></returns>
        public static RekoConfigurationService Load()
        {
            string configFileName = "reko.config";
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            configFileName = Path.Combine(appDir, configFileName);

            using (var stm = File.Open(configFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var ser = new XmlSerializer(typeof(RekoConfiguration_v1));
                var sConfig = (RekoConfiguration_v1)ser.Deserialize(stm);
                return new RekoConfigurationService(sConfig);
            }
        }

        private long ConvertNumber(string sNumber)
        {
            if (string.IsNullOrEmpty(sNumber))
                return 0;
            sNumber = sNumber.Trim();
            if (sNumber.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Int64.TryParse(
                    sNumber.Substring(2),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture,
                    out long offset))
                {
                    return offset;
                }
            }
            else
            {
                if (Int64.TryParse(sNumber, out long offset))
                {
                    return offset;
                }
            }
            return 0;
        }


        public virtual ICollection<LoaderDefinition> GetImageLoaders()
        {
            return loaders;
        }

        public virtual ICollection<SignatureFileDefinition> GetSignatureFiles()
        {
            return sigFiles;
        }

        public virtual ICollection<ArchitectureDefinition> GetArchitectures()
        {
            return architectures;
        }

        public virtual ICollection<SymbolSourceDefinition> GetSymbolSources()
        {
            return symSources;
        }

        public virtual ICollection<PlatformDefinition> GetEnvironments()
        {
            return opEnvs;
        }

        public virtual ICollection<AssemblerDefinition> GetAssemblers()
        {
            return asms;
        }

        public virtual ICollection<RawFileDefinition> GetRawFiles()
        {
            return rawFiles;
        }

        public IProcessorArchitecture GetArchitecture(string archLabel)
        {
            var elem = GetArchitectures()
                .Where(e => e.Name == archLabel).SingleOrDefault();
            if (elem == null)
                return null;

            Type t = Type.GetType(elem.TypeName, true);
            if (t == null)
                return null;
            var arch = (IProcessorArchitecture)Activator.CreateInstance(t, elem.Name);
            arch.Description = elem.Description;
            return arch;
        }

        public virtual Assembler GetAssembler(string asmLabel)
        {
            var elem = GetAssemblers()
                .Where(e => e.Name == asmLabel).SingleOrDefault();
            if (elem == null)
                return null;
            Type t = Type.GetType(elem.TypeName, true);
            return (Assembler)t.GetConstructor(Type.EmptyTypes).Invoke(null);
        }

        public PlatformDefinition GetEnvironment(string envName)
        {
            var env = GetEnvironments()
                .Where(e => e.Name == envName).SingleOrDefault();
            if (env != null)
                return env;

            return new PlatformDefinition
            {
                TypeName = typeof(DefaultPlatform).FullName,
            };
        }

        public virtual LoaderDefinition GetImageLoader(string loaderName)
        {
            return loaders.FirstOrDefault(ldr => ldr.Label == loaderName);
        }

        public virtual RawFileDefinition GetRawFile(string rawFileFormat)
        {
            return GetRawFiles()
                .Where(r => r.Name == rawFileFormat)
                .SingleOrDefault();
        }

        public IEnumerable<UiStyleDefinition> GetDefaultPreferences()
        {
            return uiPreferences.Styles;
        }

        public string GetInstallationRelativePath(string[] pathComponents)
        {
            return MakeInstallationRelativePath(pathComponents);
        }

        public static string MakeInstallationRelativePath(string[] pathComponents)
        {
            var filename = Path.Combine(pathComponents);
            if (!Path.IsPathRooted(filename))
            {
                string assemblyUri = typeof(RekoConfigurationService).Assembly.CodeBase;
                string assemblyPath = new Uri(assemblyUri).LocalPath;
                return Path.Combine(
                    Path.GetDirectoryName(assemblyPath),
                    filename);
            }
            return filename;
        }
    }
}
