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

using Reko.Core.Serialization;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Reko.Core.Configuration
{
    /// <summary>
    /// Describes a <see cref="IPlatform"/>.
    /// </summary>
    public class PlatformDefinition
    {
        /// <summary>
        /// Creates a platform definition.
        /// </summary>
        public PlatformDefinition()
        {
            this.TypeLibraries = new List<TypeLibraryDefinition>();
            this.CharacteristicsLibraries = new List<TypeLibraryDefinition>();
            this.SignatureFiles = new List<SignatureFileDefinition>();
            this.Architectures = new List<PlatformArchitectureDefinition>();
            this.Options = new Dictionary<string, object>();
        }

        /// <summary>
        /// Programmatic identifier for the platform.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Human readable description of the platform.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Any platform-specific heuristics to use when disassembling
        /// binaries of this platfirm.
        /// </summary>
        public PlatformHeuristics_v1? Heuristics { get; set; }

        /// <summary>
        /// The CLR type name of this platform.
        /// </summary>
        public string? TypeName { get; set; }

        /// <summary>
        /// Optional absolute memory map for this platform.
        /// </summary>
        public string? MemoryMapFile { get; set; }

        /// <summary>
        /// True if symbol names are case insensitive.
        /// </summary>
        public bool CaseInsensitive { get; set; }

        /// <summary>
        /// Type libraries used by this platform.
        /// </summary>
        public virtual List<TypeLibraryDefinition> TypeLibraries { get; internal set; }

        /// <summary>
        /// Characteristics libraries used by this platform.
        /// </summary>
        public virtual List<TypeLibraryDefinition> CharacteristicsLibraries { get; internal set; }

        /// <summary>
        /// Architecture-specific settings for this platform.
        /// </summary>
        public virtual List<PlatformArchitectureDefinition> Architectures { get; internal set; }

        /// <summary>
        /// Signature files applicable to this platform.
        /// </summary>
        public virtual List<SignatureFileDefinition> SignatureFiles { get; internal set; }

        /// <summary>
        /// Platform-specific options.
        /// </summary>
        public virtual Dictionary<string, object> Options { get; internal set; }

        /// <summary>
        /// Creates an <see cref="IPlatform"/> instance, given a processor architecture.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance to
        /// inject services into the <see cref="IPlatform"/> instance.</param>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> instance to use.</param>
        /// <returns>An <see cref="IPlatform"/> instance.</returns>
        public virtual IPlatform Load(IServiceProvider services, IProcessorArchitecture arch)
        {
            if (TypeName is null)
                throw new InvalidOperationException("Platform configuration TypeName has no value.");
            var svc = services.RequireService<IPluginLoaderService>();
            var type = svc.GetType(TypeName);
            if (type is null)
                throw new TypeLoadException(
                    string.Format("Unable to load {0} environment.", Description));
            var platform = (Platform)Activator.CreateInstance(type, services, arch)!;
            LoadSettingsFromConfiguration(services, platform);
            return platform;
        }

        /// <summary>
        /// Initializes a <see cref="Platform"/> instance with settings.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> instance.</param>
        /// <param name="platform">Platform instance to initialize.</param>
        public void LoadSettingsFromConfiguration(IServiceProvider services, Platform platform)
        {
            platform.Name = this.Name!;
            if (!string.IsNullOrEmpty(MemoryMapFile))
            {
                var cfgSvc = services.RequireService<IConfigurationService>();
                platform.MemoryMap = cfgSvc.LoadMemoryMap(MemoryMapFile);
            }
            platform.PlatformProcedures = LoadPlatformProcedures(platform);
            platform.Description = this.Description!;
            platform.Heuristics = LoadHeuristics(this.Heuristics);
        }

        private PlatformHeuristics LoadHeuristics(PlatformHeuristics_v1? heuristics)
        {
            if (heuristics is null)
            {
                return new PlatformHeuristics
                {
                    ProcedurePrologs = Array.Empty<MaskedPattern>(),
                };
            }
            MaskedPattern[] prologs;
            if (heuristics.ProcedurePrologs is null)
            {
                prologs = Array.Empty<MaskedPattern>();
            }
            else
            {
                prologs = heuristics.ProcedurePrologs
                    .Select(p => MaskedPattern.Load(p.Bytes, p.Mask, p.Endianness)!)
                    .Where(p => p is not null && p.Bytes is not null)
                    .ToArray();
            }

            return new PlatformHeuristics
            {
                ProcedurePrologs = prologs
            };
        }

        private Dictionary<Address, ExternalProcedure> LoadPlatformProcedures(Platform platform)
        {
            if (platform.MemoryMap is not null && platform.MemoryMap.Segments is not null)
            {
                var metadata = platform.EnsureTypeLibraries(platform.Name);
                var tser = new TypeLibraryDeserializer(platform, true, metadata);
                var sser = new ProcedureSerializer(platform, tser, platform.DefaultCallingConvention);
                return platform.MemoryMap.Segments
                    .Where(s => s.Procedures is not null)
                    .SelectMany(s => s.Procedures!)
                    .OfType<Procedure_v1>()
                    .Where(p => p.Name is not null)
                    //$REVIEW: handle when addresses are not parseable.
                    .Select(p =>
                        (addr: platform.Architecture.TryParseAddress(p.Address, out var addr) ? (Address?)addr : null,
                         ext:  new ExternalProcedure(
                             p.Name!,
                             sser.Deserialize(p.Signature, platform.Architecture.CreateFrame())
                                ?? new Types.FunctionType())))
                    .Where(p => p.addr is not null)
                    .ToDictionary(p => p.addr!.Value, p => p.ext);
            }
            else
            {
                return new Dictionary<Address, ExternalProcedure>();
            }
        }

        /// <summary>
        /// Returns a string representation of the platform definition.
        /// </summary>
        public override string ToString()
        {
            return Description ?? "";
        }
    }
}
