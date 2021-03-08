#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
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
         ICollection<RawFileDefinition> GetRawFiles();

         PlatformDefinition GetEnvironment(string envName);
         IProcessorArchitecture? GetArchitecture(string archLabel);
        IProcessorArchitecture? GetArchitecture(string archLabel, string? modelName);
        IProcessorArchitecture? GetArchitecture(string archLabel, Dictionary<string, object>? options);

         ICollection<SymbolSourceDefinition> GetSymbolSources();
         RawFileDefinition? GetRawFile(string rawFileFormat);

        IEnumerable<UiStyleDefinition> GetDefaultPreferences();

         /// <summary>
         /// Given a relative path with respect to the installation directory, 
         /// returns the absolute path.
         /// </summary>
         /// <param name="path"></param>
         /// <returns></returns>
        string GetInstallationRelativePath(params string[] pathComponents);
         LoaderDefinition? GetImageLoader(string loader);
    }

    public class RekoConfigurationService : IConfigurationService
    {
        private readonly string configFileRoot;
        private readonly IPluginLoaderService pluginSvc;
        private readonly IServiceProvider services;
        private readonly List<LoaderDefinition> loaders;
        private readonly List<SignatureFileDefinition> sigFiles;
        private readonly List<ArchitectureDefinition> architectures;
        private readonly List<PlatformDefinition> opEnvs;
        private readonly List<SymbolSourceDefinition> symSources;
        private readonly List<RawFileDefinition> rawFiles;
        private readonly UiPreferencesConfiguration uiPreferences;

        public RekoConfigurationService(IServiceProvider services, string rekoConfigPath, RekoConfiguration_v1 config)
        {
            var pluginSvc = services.GetService<IPluginLoaderService>();
            if (pluginSvc is null)
                pluginSvc = new PluginLoaderService();
            this.pluginSvc = pluginSvc;
            this.configFileRoot = Path.GetDirectoryName(rekoConfigPath);
            this.services = services;
            this.loaders = LoadCollection(config.Loaders, LoadLoaderConfiguration);
            this.sigFiles = LoadCollection(config.SignatureFiles, LoadSignatureFile);
            this.architectures = LoadCollection(config.Architectures, LoadArchitecture);
            this.opEnvs = LoadCollection(config.Environments, LoadEnvironment);
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
                TypeName = sSig.TypeName,
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
                Models = LoadDictionary(sArch.Models, m => m.Name!, LoadModelDefinition)
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
                Choices = sOption.Choices ?? new ListOption_v1[0]
            };
        }

        private ModelDefinition LoadModelDefinition(ModelDefinition_v1 sModel)
        {
            return new ModelDefinition
            {
                Name = sModel.Name,
                Options = sModel.Options != null
                    ? sModel.Options.ToList()
                    : new List<ListOption_v1>()
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
                CaseInsensitive = env.CaseInsensitive,
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

        private EntryPointDefinition LoadEntryPoint(EntryPoint_v1? sEntry)
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

        private List<TDst> LoadCollection<TSrc, TDst>(TSrc[]? sItems, Func<TSrc, TDst> fn)
        {
            if (sItems == null)
                return new List<TDst>();
            else
                return sItems.Select(fn).ToList();
        }

        private Dictionary<string, TValue> LoadDictionary<TSrc, TValue>(
            TSrc[]? sItems, 
            Func<TSrc, string> fnKey, 
            Func<TSrc, TValue> fnValue)
        {
            if (sItems == null)
                return new Dictionary<string, TValue>();
            else
                return sItems.ToDictionary(fnKey, fnValue);
        }

        /// <summary>
        /// Load the reko.config file.
        /// </summary>
        /// <remarks>
        /// For now, we assume the config file is called "reko.config" located in the same directory as the assembly we're executing.
        /// It's possible that on Un*x systems, the Reko binary could be installed in /usr/bin, while the configuration
        /// file is located somewhere else.
        /// </remarks>
        /// <returns>A loaded instance of <see cref="RekoConfigurationService"/>.</returns>
        public static RekoConfigurationService Load(IServiceProvider services)
        {
            string configFileName = "reko.config";
            return Load(services, configFileName);
        }

        /// <summary>
        /// Loads the Reko configuration settings from the provided <paramref name="configFileName"/> file.
        /// </summary>
        /// <param name="services">Environmental services.</param>
        /// <param name="configFileName">Path to the reko.config file name. If the path is not absolute, it 
        /// will be taken relative to the current executing assembly.</param>
        /// <returns>A loaded instance of <see cref="RekoConfigurationService"/>.</returns>
        public static RekoConfigurationService Load(IServiceProvider services, string configFileName)
        {
            if (!Path.IsPathRooted(configFileName))
            {
                var appDir = AppDomain.CurrentDomain.BaseDirectory;
                configFileName = Path.Combine(appDir, configFileName);
            }

            using var stm = File.Open(configFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var ser = new XmlSerializer(typeof(RekoConfiguration_v1));
            var sConfig = (RekoConfiguration_v1) ser.Deserialize(stm);
            var cfg = new RekoConfigurationService(services, configFileName, sConfig);
            return cfg;
        }

        /// <summary>
        /// Converts a number, which is expressed as a string, to a numeric value.
        /// </summary>
        /// <param name="sNumber">Number, expressed as a string, with an optional prefix
        /// to represent different bases</param>
        /// <returns>The converted number, or 0 of the string couldn't be interpreted as a 
        /// number.</returns>
        private long ConvertNumber(string? sNumber)
        {
            if (string.IsNullOrEmpty(sNumber))
                return 0;
            sNumber = sNumber!.Trim();
            if (sNumber.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Int64.TryParse(
                    sNumber[2..],
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

        public virtual ICollection<RawFileDefinition> GetRawFiles()
        {
            return rawFiles;
        }

        public IProcessorArchitecture? GetArchitecture(string archLabel)
        {
            return GetArchitecture(archLabel, new Dictionary<string, object>());
        }

        public IProcessorArchitecture? GetArchitecture(string archLabel, string? modelName)
        {
            if (modelName is null)
                return GetArchitecture(archLabel, new Dictionary<string, object>());
            var elem = GetArchitectures()
                .Where(e => e.Name == archLabel).SingleOrDefault();
            if (elem == null)
                return null;
            ModelDefinition? model;
            if (elem.Models is null)
            {
                model = null;
            }
            else
            {
                elem.Models.TryGetValue(modelName, out model);
            }
            var options = new Dictionary<string, object>();
            if (model is null)
            {
                var listener = services.GetService<DecompilerEventListener>() ??
                    new NullDecompilerEventListener();
                listener.Warn($"Model '{modelName}' is not defined for architecture '{archLabel}'.");
            }
            else if (model.Options != null)
            {
                foreach (var opt in model.Options)
                {
                    if (opt.Text != null && opt.Value != null)
                    {
                        options[opt.Text] = opt.Value;
                    }
                }
            }
            options[ProcessorOption.Model] = modelName;
            return GetArchitecture(archLabel, options);
        }

        public IProcessorArchitecture? GetArchitecture(string archLabel, Dictionary<string, object>? options)
        {
            var elem = GetArchitectures()
                .Where(e => e.Name == archLabel).SingleOrDefault();
            if (elem is null || elem.TypeName is null)
                return null;
            options ??= new Dictionary<string, object>();
            var t = pluginSvc.GetType(elem.TypeName);
            if (t is null)
                return null;
            var arch = (IProcessorArchitecture)Activator.CreateInstance(
                t, 
                this.services, 
                elem.Name, 
                options ?? new Dictionary<string, object>());
            arch.Description = elem.Description;
            return arch;
        }

        public PlatformDefinition GetEnvironment(string envName)
        {
            var env = GetEnvironments()
                .Where(e => e.Name == envName).SingleOrDefault();
            if (env != null)
                return env;

            return new PlatformDefinition
            {
                TypeName = typeof(DefaultPlatform).AssemblyQualifiedName,
            };
        }

        public virtual LoaderDefinition GetImageLoader(string loaderName)
        {
            return loaders.FirstOrDefault(ldr => ldr.Label == loaderName);
        }

        public virtual RawFileDefinition? GetRawFile(string rawFileFormat)
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
            var installationRelvativePath = new List<string> { configFileRoot };
            installationRelvativePath.AddRange(pathComponents);
            return MakeInstallationRelativePath(installationRelvativePath.ToArray());
        }

        public static string MakeInstallationRelativePath(string[] pathComponents)
        {
            var filename = Path.Combine(pathComponents);
            if (!Path.IsPathRooted(filename))
            {
                string assemblyPath = typeof(RekoConfigurationService).Assembly.Location;
                return Path.Combine(
                    Path.GetDirectoryName(assemblyPath),
                    filename);
            }
            return filename;
        }
    }
}
