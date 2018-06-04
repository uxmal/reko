#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
         ICollection<LoaderConfiguration> GetImageLoaders();
         ICollection<Architecture> GetArchitectures();
         ICollection<OperatingEnvironment> GetEnvironments();
         ICollection<SignatureFile> GetSignatureFiles();
         ICollection<AssemblerElement> GetAssemblers();
         ICollection<RawFileElement> GetRawFiles();

         OperatingEnvironment GetEnvironment(string envName);
         IProcessorArchitecture GetArchitecture(string archLabel);
         ICollection<SymbolSource> GetSymbolSources();
         Assembler GetAssembler(string assemblerName);
         RawFileElement GetRawFile(string rawFileFormat);

         IEnumerable<UiStyle> GetDefaultPreferences ();

         /// <summary>
         /// Given a relative path with respect to the installation directory, 
         /// returns the absolute path.
         /// </summary>
         /// <param name="path"></param>
         /// <returns></returns>
         string GetInstallationRelativePath(params string [] pathComponents);
        LoaderConfiguration GetImageLoader(string loader);
    }

    public class RekoConfigurationService : IConfigurationService
    {
        private List<LoaderConfiguration> loaders;
        private List<SignatureFile> sigFiles;
        private List<Architecture> architectures;
        private List<OperatingEnvironment> opEnvs;
        private List<AssemblerElement> asms;
        private List<SymbolSource> symSources;
        private List<RawFileElement> rawFiles;
        private UiPreferencesConfiguration uiPreferences;

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

        private LoaderConfiguration LoadLoaderConfiguration(RekoLoader l)
        {
            return new LoaderElementImpl
            {
                Argument = l.Argument,
                Extension = l.Extension,
                Offset = l.Offset,
                Label = l.Label,
                MagicNumber = l.MagicNumber,
                TypeName = l.Type
            };
        }

        private SignatureFile LoadSignatureFile(SignatureFile_v1 sSig)
        {
            return new SignatureFileElement
            {
                Filename = sSig.Filename,
                Label = sSig.Label,
                Type = sSig.Type,
            };
        }

        private Architecture LoadArchitecture(Architecture_v1 sArch)
        {
            return new ArchitectureElement
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

        private OperatingEnvironment LoadEnvironment(Environment_v1 env)
        {
            return new OperatingEnvironmentElement
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

        private AssemblerElement LoadAssembler(Assembler_v1 sAsm)
        {
            return new AssemblerElementImpl
            {
                Description = sAsm.Description,
                Name = sAsm.Name,
                TypeName = sAsm.Type,
            };
        }

        private SymbolSource LoadSymbolSource(SymbolSource_v1 sSymSrc)
        {
            return new SymbolSourceDefinition
            {
                Description = sSymSrc.Description,
                Name = sSymSrc.Name,
                Extension = sSymSrc.Extension,
                TypeName = sSymSrc.Type,
            };
        }

        private ITypeLibraryElement LoadTypeLibraryReference(TypeLibraryReference_v1 tlibRef)
        {
            return new TypeLibraryElement
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
        private IPlatformArchitectureElement LoadPlatformArchitecture(PlatformArchitecture_v1 spa)
        {
            var sTrashedRegs = spa.TrashedRegisters ?? "";
            var sLibraries = spa.TypeLibraries ?? new TypeLibraryReference_v1[0];
            return new PlatformArchitectureElement
            {
                Name = spa.Name,
                TrashedRegisters = sTrashedRegs
                    .Split(',')
                    .Select(s => s.Trim())
                    .ToList(),
                TypeLibraries = LoadCollection(sLibraries, LoadTypeLibraryReference)
            };
        }

        private RawFileElement LoadRawFile(RawFile_v1 sRaw)
        {
            return new RawFileElementImpl
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

        private EntryPointElement LoadEntryPoint(EntryPoint_v1 sEntry)
        {
            if (sEntry == null)
            {
                return new EntryPointElement
                {
                    Address = null,
                    Follow = false,
                    Name = null,
                };
            }
            return new EntryPointElement
            {
                Address = sEntry.Address,
                Follow = sEntry.Follow,
                Name = sEntry.Name,
            };
        }

        private UiStyle LoadUiStyle(StyleDefinition_v1 sUiStyle)
        {
            return new UiStyleElement
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
        /// <returns></returns>
        public static RekoConfigurationService Load()
        {
            var configFileName = ConfigurationManager.AppSettings["RekoConfiguration"];
            if (configFileName == null)
                throw new ApplicationException("Missing app setting 'RekoConfiguration' in configuration file.");
            return Load(configFileName);
        }

        public static RekoConfigurationService Load(string configFileName)
        {
            var appConfig = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            var appDir = Path.GetDirectoryName(appConfig);
            configFileName = Path.Combine(appDir, configFileName);

            using (var stm = File.Open(configFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var ser = new XmlSerializer(typeof(RekoConfiguration_v1));
                var sConfig = (RekoConfiguration_v1)ser.Deserialize(stm);
                return new RekoConfigurationService(sConfig);
            }
        }

        public virtual ICollection<LoaderConfiguration> GetImageLoaders()
        {
            return loaders;
        }

        public virtual ICollection<SignatureFile> GetSignatureFiles()
        {
            return sigFiles;
        }

        public virtual ICollection<Architecture> GetArchitectures()
        {
            return architectures;
        }

        public virtual ICollection<SymbolSource> GetSymbolSources()
        {
            return symSources;
        }

        public virtual ICollection<OperatingEnvironment> GetEnvironments()
        {
            return opEnvs;
        }

        public virtual ICollection<AssemblerElement> GetAssemblers()
        {
            return asms;
        }

        public virtual ICollection<RawFileElement> GetRawFiles()
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

        public OperatingEnvironment GetEnvironment(string envName)
        {
            var env = GetEnvironments()
                .Where(e => e.Name == envName).SingleOrDefault();
            if (env != null)
                return env;

            return new OperatingEnvironmentElement
            {
                TypeName = typeof(DefaultPlatform).FullName,
            };
        }

        public virtual LoaderConfiguration GetImageLoader(string loaderName)
        {
            return loaders.FirstOrDefault(ldr => ldr.Label == loaderName);
        }

        public virtual RawFileElement GetRawFile(string rawFileFormat)
        {
            return GetRawFiles()
                .Where(r => r.Name == rawFileFormat)
                .SingleOrDefault();
        }

        public IEnumerable<UiStyle> GetDefaultPreferences()
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
