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

using Reko.Core.Machine;
using Reko.Core.Plugins;
using Reko.Core.Serialization;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
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
        /// <summary>
        /// Gets a list of available image loaders.
        /// </summary>
        ICollection<LoaderDefinition> GetImageLoaders();

        /// <summary>
        /// Gets a list of available processor architectures.
        /// </summary>
        ICollection<ArchitectureDefinition> GetArchitectures();

        /// <summary>
        /// Gets a list of available operating systems or environments.
        /// </summary>
        ICollection<PlatformDefinition> GetEnvironments();

        /// <summary>
        /// Gets a list of available signature files.
        /// </summary>
        ICollection<SignatureFileDefinition> GetSignatureFiles();

        /// <summary>
        /// Gets a list of known raw file formats.
        /// </summary>
        ICollection<RawFileDefinition> GetRawFiles();

        /// <summary>
        /// Given an platform or environment name, gets its corresponding platform definition.
        /// </summary>
        /// <param name="envName">Name of the environment.</param>
        /// <returns>A platform definition for that environment.</returns>
        PlatformDefinition GetEnvironment(string envName);

        /// <summary>
        /// Given the name of an architecture, creates an instance of that architecture.
        /// </summary>
        /// <param name="archLabel">The name of the architecture to load (e.g. "z80").
        /// The list of supported architectures can be found in the reko.config file.
        /// Architectures may also be available as plugins (see <see cref="IPlugin"/> 
        /// for the extension mechanism).</param>
        /// <returns>An instance of the requested architecture, or null if it couldn't 
        /// be created.
        /// </returns>
        IProcessorArchitecture? GetArchitecture(string archLabel);

        /// <summary>
        /// Given the name of an architecture, and an architecture-specific
        /// model name, creates an instance of that architecture.
        /// </summary>
        /// <param name="archLabel">The name of the architecture to load (e.g. "z80").
        /// The list of supported architectures can be found in the reko.config file.
        /// Architectures may also be available as plugins (see <see cref="IPlugin"/> 
        /// for the extension mechanism).</param>
        /// <param name="modelName">A CPU model name. This is used when an architecture
        /// exists in multiple versions (e.g. 8086, 80186, 80286, Pentium) and you 
        /// wish a specific version. 
        /// </param>
        /// <returns>An instance of the requested architecture, or null if it couldn't 
        /// be created.
        /// </returns>
        IProcessorArchitecture? GetArchitecture(string archLabel, string? modelName);

        /// <summary>
        /// Given the name of an architecture, and a dictionary of architecture-specific
        /// options, creates an instance of that architecture.
        /// </summary>
        /// <param name="archLabel">The name of the architecture to load (e.g. "z80").
        /// The list of supported architectures can be found in the reko.config file.
        /// Architectures may also be available as plugins (see <see cref="IPlugin"/> 
        /// for the extension mechanism).</param>
        /// <param name="options">A dictionary of architecture-specific options, or 
        /// null if there are none.
        /// </param>
        /// <returns>An instance of the requested architecture, or null if it couldn't 
        /// be created.
        /// </returns>
        IProcessorArchitecture? GetArchitecture(string archLabel, Dictionary<string, object>? options);

        /// <summary>
        /// Gets a list of available symbol sources.
        /// </summary>
        ICollection<SymbolSourceDefinition> GetSymbolSources();

        /// <summary>
        /// For a given raw file format identifier, get its corresponding description.
        /// </summary>
        /// <param name="rawFileFormat"></param>
        /// <returns>The raw file definition if it exists.</returns>
        RawFileDefinition? GetRawFile(string rawFileFormat);

        /// <summary>
        /// Get the default UI preferences.
        /// </summary>
        IEnumerable<UiStyleDefinition> GetDefaultPreferences();

        /// <summary>
        /// Given a relative path with respect to the installation directory, 
        /// returns the absolute path.
        /// </summary>
        /// <param name="pathComponents">The components of the path.</param>
        /// <returns>An installation-relative file path.</returns>
        string GetInstallationRelativePath(params string[] pathComponents);

        /// <summary>
        /// Given an image loader id, gets its definition.
        /// </summary>
        /// <param name="loader">Image format ID.</param>
        /// <returns>The corresponding <see cref="LoaderDefinition"/> if one exists;
        /// otherwise null.
        /// </returns>
        LoaderDefinition? GetImageLoader(string loader);

        /// <summary>
        /// Loads a memory map file.
        /// </summary>
        /// <param name="memoryMapFile">Installation-relative path to the
        /// memory map file.
        /// </param>
        /// <returns>An <see cref="MemoryMap_v1"/> instance if successful; otherwise
        /// null.
        /// </returns>
        MemoryMap_v1? LoadMemoryMap(string memoryMapFile);
    }

    /// <summary>
    /// Implementation of <see cref="IConfigurationService"/>.
    /// </summary>
    public class RekoConfigurationService : IConfigurationService
    {
        private const string PluginsDirectory = "plugins";

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
        private bool pluginsLoaded;

        /// <summary>
        /// Creates an instanc of the Reko configuration service.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance.</param>
        /// <param name="rekoConfigPath">File system path to the <c>reko.config</c> file.</param>
        /// <param name="config">Deserialized contents of <c>reko.config</c>.</param>
        public RekoConfigurationService(IServiceProvider services, string rekoConfigPath, RekoConfiguration_v1 config)
        {
            var pluginSvc = services.GetService<IPluginLoaderService>();
            if (pluginSvc is null)
                pluginSvc = new PluginLoaderService();
            this.pluginSvc = pluginSvc;
            this.configFileRoot = Path.GetDirectoryName(rekoConfigPath)!;
            this.services = services;
            this.loaders = LoadCollection(config.Loaders, LoadLoaderConfiguration);
            this.sigFiles = LoadCollection(config.SignatureFiles, LoadSignatureFile);
            this.architectures = LoadCollection(config.Architectures, LoadArchitecture);
            this.opEnvs = LoadCollection(config.Environments, LoadEnvironment);
            this.symSources = LoadCollection(config.SymbolSources, LoadSymbolSource);
            this.rawFiles = LoadCollection(config.RawFiles, LoadRawFile);
            this.uiPreferences = new UiPreferencesConfiguration();
            this.uiPreferences.Styles = LoadCollection(config.UiPreferences?.Styles, LoadUiStyle);
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
            var ad = new ArchitectureDefinition
            {
                Description = sArch.Description,
                Name = sArch.Name,
                TypeName = sArch.Type,
                Options = LoadCollection(sArch.Options, LoadPropertyOption),
                Models = LoadDictionary(sArch.Models, m => m.Name!, LoadModelDefinition),
                ProcedurePrologs = LoadMaskedPatterns(sArch.ProcedurePrologs),
                Aliases = LoadDelimiterSeparatedValues(sArch.Aliases, ',')
                    .ToHashSet(),
                MemoryMapFile = sArch.MemoryMapFile
            };
            return ad;
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
                Choices = sOption.Choices ?? Array.Empty<ListOption_v1>()
            };
        }

        /// <summary>
        /// Loads a memory map file.
        /// </summary>
        /// <param name="path">Path where the memory map is locatd.</param>
        /// <returns></returns>
        public MemoryMap_v1? LoadMemoryMap(string path)
        {
            var fsSvc = services.GetService<IFileSystemService>();
            if (fsSvc is null)
                return null;
            var filePath = GetInstallationRelativePath(path);
            using var stm = fsSvc.CreateFileStream(filePath, FileMode.Open, FileAccess.Read);
            return MemoryMap_v1.Deserialize(stm);
        }

        private ModelDefinition LoadModelDefinition(ModelDefinition_v1 sModel)
        {
            return new ModelDefinition
            {
                Name = sModel.Name,
                Options = sModel.Options is not null
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
                Options = env.Options is not null
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
            var sPreservedRegs = spa.PreservedRegisters ?? "";
            return new PlatformArchitectureDefinition
            {
                Name = spa.Name,
                CallingConventions = LoadDelimiterSeparatedValues(spa.CallingConventions, ',')
                    .ToList(),
                DefaultCallingConvention = spa.DefaultCallingConvention,
                TrashedRegisters = LoadDelimiterSeparatedValues(spa.TrashedRegisters, ',')
                    .ToList(),
                PreservedRegisters = LoadDelimiterSeparatedValues(spa.PreservedRegisters, ',')
                    .ToList(),
                TypeLibraries = LoadCollection(spa.TypeLibraries, LoadTypeLibraryReference),
                ProcedurePrologs = LoadMaskedPatterns(spa.ProcedurePrologs)
            };
        }

        private static List<MaskedPattern> LoadMaskedPatterns(BytePattern_v1[]? patterns)
        {
            if (patterns is null)
            {
                return new();
            }
            else
            {
                return patterns
                    .Select(p => MaskedPattern.Load(p.Bytes, p.Mask, p.Endianness)!)
                    .Where(p => p is not null && p.Bytes is not null)
                    .ToList();
            }
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

        private static EntryPointDefinition LoadEntryPoint(EntryPoint_v1? sEntry)
        {
            if (sEntry is null)
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
                FontSize = sUiStyle.FontSize <= 0.0 ? sUiStyle.FontSize : null,
                ForeColor = sUiStyle.ForeColor,
                TextAlign = sUiStyle.TextAlign,
                Width = sUiStyle.Width,
                PaddingTop = sUiStyle.PaddingTop,
                PaddingLeft = sUiStyle.PaddingLeft,
                PaddingBottom = sUiStyle.PaddingBottom,
                PaddingRight = sUiStyle.PaddingRight,
            };
        }

        private IEnumerable<string> LoadDelimiterSeparatedValues(string? serializedVaue, char delimiter)
        {
            if (string.IsNullOrEmpty(serializedVaue))
                return Array.Empty<string>();
            return serializedVaue
                .Split(delimiter)
                .Select(s => s.Trim());

        }
        private static List<TDst> LoadCollection<TSrc, TDst>(TSrc[]? sItems, Func<TSrc, TDst> fn)
        {
            if (sItems is null)
                return new List<TDst>();
            else
                return sItems.Select(fn).ToList();
        }

        private static Dictionary<string, TValue> LoadDictionary<TSrc, TValue>(
            TSrc[]? sItems,
            Func<TSrc, string> fnKey,
            Func<TSrc, TValue> fnValue)
        {
            if (sItems is null)
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
            var sConfig = (RekoConfiguration_v1) ser.Deserialize(stm)!;
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
        private static long ConvertNumber(string? sNumber)
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

        /// <inheritdoc/>
        public virtual ICollection<LoaderDefinition> GetImageLoaders()
        {
            EnsurePlugins();
            return loaders;
        }

        /// <inheritdoc/>
        public virtual ICollection<SignatureFileDefinition> GetSignatureFiles()
        {
            return sigFiles;
        }

        /// <inheritdoc/>
        public virtual ICollection<ArchitectureDefinition> GetArchitectures()
        {
            EnsurePlugins();
            return architectures;
        }

        /// <inheritdoc/>
        public virtual ICollection<SymbolSourceDefinition> GetSymbolSources()
        {
            return symSources;
        }

        /// <inheritdoc/>
        public virtual ICollection<PlatformDefinition> GetEnvironments()
        {
            return opEnvs;
        }

        /// <inheritdoc/>
        public virtual ICollection<RawFileDefinition> GetRawFiles()
        {
            return rawFiles;
        }

        /// <inheritdoc/>
        public IProcessorArchitecture? GetArchitecture(string archLabel)
        {
            return GetArchitecture(archLabel, new Dictionary<string, object>());
        }

        /// <inheritdoc/>
        public IProcessorArchitecture? GetArchitecture(string archLabel, string? modelName)
        {
            if (modelName is null)
                return GetArchitecture(archLabel, new Dictionary<string, object>());
            var elem = FindArchitectureByLabel(archLabel);
            if (elem is null)
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
                var listener = services.GetService<IEventListener>() ??
                    new NullEventListener();
                listener.Warn($"Model '{modelName}' is not defined for architecture '{archLabel}'.");
            }
            else if (model.Options is not null)
            {
                foreach (var opt in model.Options)
                {
                    if (opt.Text is not null && opt.Value is not null)
                    {
                        options[opt.Text] = opt.Value;
                    }
                }
            }
            options[ProcessorOption.Model] = modelName;
            return GetArchitecture(archLabel, options);
        }

        /// <inheritdoc/>
        public IProcessorArchitecture? GetArchitecture(string archLabel, Dictionary<string, object>? options)
        {
            var elem = FindArchitectureByLabel(archLabel);
            if (elem is null)
                return null;
            var t = elem.Type;
            if (t is null)
            {
                if (elem.TypeName is null)
                    return null;
                t = pluginSvc.GetType(elem.TypeName);
                if (t is null)
                    return null;
                elem.Type = t;
            }
            options ??= new Dictionary<string, object>();
            var arch = (IProcessorArchitecture) Activator.CreateInstance(
                t,
                this.services,
                elem.Name,
                options)!;
            arch.Description = elem.Description;
            if (arch is ProcessorArchitecture a)
            {
                a.ProcedurePrologs = elem.ProcedurePrologs.ToArray();
                if (!string.IsNullOrEmpty(elem.MemoryMapFile))
                {
                    a.MemoryMap = this.LoadMemoryMap(elem.MemoryMapFile);
                }
            }
            return arch;
        }

        private ArchitectureDefinition? FindArchitectureByLabel(string archLabel)
        {
            return GetArchitectures()
                .Where(e => e.Name == archLabel ||
                            e.Aliases.Contains(archLabel)).SingleOrDefault();
        }

        /// <inheritdoc/>
        public PlatformDefinition GetEnvironment(string envName)
        {
            var env = GetEnvironments()
                .Where(e => e.Name == envName).SingleOrDefault();
            if (env is not null)
                return env;

            return new PlatformDefinition
            {
                TypeName = typeof(DefaultPlatform).AssemblyQualifiedName,
            };
        }

        /// <inheritdoc/>
        public virtual LoaderDefinition? GetImageLoader(string loaderName)
        {
            return GetImageLoaders().FirstOrDefault(ldr => ldr.Label == loaderName);
        }

        /// <inheritdoc/>
        public virtual RawFileDefinition? GetRawFile(string rawFileFormat)
        {
            return GetRawFiles()
                .Where(r => r.Name == rawFileFormat)
                .SingleOrDefault();
        }

        /// <inheritdoc/>
        public IEnumerable<UiStyleDefinition> GetDefaultPreferences()
        {
            return uiPreferences.Styles;
        }

        /// <inheritdoc/>
        public string GetInstallationRelativePath(params string[] pathComponents)
        {
            var installationRelvativePath = new List<string> { configFileRoot };
            installationRelvativePath.AddRange(pathComponents);
            return MakeInstallationRelativePath(installationRelvativePath.ToArray());
        }

        /// <summary>
        /// Makes an installation-relative path into an absolute path.
        /// </summary>
        /// <param name="pathComponents">Components of the path.</param>
        /// <returns>An absolute path.</returns>
        public static string MakeInstallationRelativePath(string[] pathComponents)
        {
            var filename = Path.Combine(pathComponents);
            if (!Path.IsPathRooted(filename))
            {
                string assemblyPath = typeof(RekoConfigurationService).Assembly.Location;
                return Path.Combine(
                    Path.GetDirectoryName(assemblyPath)!,
                    filename);
            }
            return filename;
        }

        private void EnsurePlugins()
        {
            if (pluginsLoaded)
                return;
            pluginsLoaded = true;
            var plugins = LoadPluginsFromDirectory(GetInstallationRelativePath(PluginsDirectory));
            foreach (var plugin in plugins)
            {
                this.architectures.AddRange(plugin.Architectures);
                this.loaders.AddRange(plugin.Loaders);
            }
        }

        private List<IPlugin> LoadPluginsFromDirectory(string directoryName)
        {
            var result = new List<IPlugin>();
            if (!Directory.Exists(directoryName))
                return result;
            foreach (var file in Directory.GetFiles(directoryName))
            {
                try
                {
                    var asmName = Path.Combine(directoryName, file);
                    using (var fs = File.OpenRead(asmName))
                    {
                        if (fs.ReadByte() != 'M' && fs.ReadByte() != 'Z')
                            continue;
                    }
                    var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(asmName);
                    foreach (var pluginType in asm.GetTypes()
                        .Where(t => typeof(IPlugin).IsAssignableFrom(t) &&
                                    t.IsClass && !t.IsAbstract))
                    {
                        if (Activator.CreateInstance(pluginType) is IPlugin plugin)
                            result.Add(plugin);
                    }
                }
                catch
                {
                }
            }
            return result;
        }
    }
}
