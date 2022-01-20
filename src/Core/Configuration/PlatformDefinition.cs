#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core.Configuration
{
    public class PlatformDefinition
    {
        public PlatformDefinition()
        {
            this.TypeLibraries = new List<TypeLibraryDefinition>();
            this.CharacteristicsLibraries = new List<TypeLibraryDefinition>();
            this.SignatureFiles = new List<SignatureFileDefinition>();
            this.Architectures = new List<PlatformArchitectureDefinition>();
            this.Options = new Dictionary<string, object>();
        }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public PlatformHeuristics_v1? Heuristics { get; set; }

        public string? TypeName { get; set; }

        public string? MemoryMapFile { get; set; }

        public bool CaseInsensitive { get; set; }

        public virtual List<TypeLibraryDefinition> TypeLibraries { get; internal set; }
        public virtual List<TypeLibraryDefinition> CharacteristicsLibraries { get; internal set; }
        public virtual List<PlatformArchitectureDefinition> Architectures { get; internal set; }
        public virtual List<SignatureFileDefinition> SignatureFiles { get; internal set; }
        public virtual Dictionary<string, object> Options { get; internal set; }

        public virtual IPlatform Load(IServiceProvider services, IProcessorArchitecture arch)
        {
            if (TypeName is null)
                throw new InvalidOperationException("Platform configuration TypeName has no value.");
            var svc = services.RequireService<IPluginLoaderService>();
            var type = svc.GetType(TypeName);
            if (type is null)
                throw new TypeLoadException(
                    string.Format("Unable to load {0} environment.", Description));
            var platform = (Platform)Activator.CreateInstance(type, services, arch);
            LoadSettingsFromConfiguration(services, platform);
            return platform;
        }

        public void LoadSettingsFromConfiguration(IServiceProvider services, Platform platform)
        {
            platform.Name = this.Name!;
            if (!string.IsNullOrEmpty(MemoryMapFile))
            {
                var cfgSvc = services.RequireService<IConfigurationService>();
                var fsSvc = services.RequireService<IFileSystemService>();
                var listener = services.RequireService<DecompilerEventListener>();
                try
                {
                    var filePath = cfgSvc.GetInstallationRelativePath(MemoryMapFile!);
                    using var stm = fsSvc.CreateFileStream(filePath, FileMode.Open, FileAccess.Read);
                    platform.MemoryMap = MemoryMap_v1.Deserialize(stm);
                }
                catch (Exception ex)
                {
                    listener.Error(ex, "Unable to open memory map file '{0}.", MemoryMapFile!);
                }
            }
            platform.PlatformProcedures = LoadPlatformProcedures(platform);
            platform.Description = this.Description!;
            platform.Heuristics = LoadHeuristics(this.Heuristics);
        }

        private PlatformHeuristics LoadHeuristics(PlatformHeuristics_v1? heuristics)
        {
            if (heuristics == null)
            {
                return new PlatformHeuristics
                {
                    ProcedurePrologs = new MaskedPattern[0],
                };
            }
            MaskedPattern[] prologs;
            if (heuristics.ProcedurePrologs == null)
            {
                prologs = new MaskedPattern[0];
            }
            else
            {
                prologs = heuristics.ProcedurePrologs
                    .Select(p => LoadMaskedPattern(p)!)
                    .Where(p => p != null && p.Bytes != null)
                    .ToArray();
            }

            return new PlatformHeuristics
            {
                ProcedurePrologs = prologs
            };
        }

        public MaskedPattern? LoadMaskedPattern(BytePattern_v1 sPattern)
        {
            if (sPattern.Bytes == null)
                return null;
            if (sPattern.Mask == null)
            {
                var bytes = new List<byte>();
                var mask = new List<byte>();
                int shift = 4;
                int bb = 0;
                int mm = 0;
                for (int i = 0; i < sPattern.Bytes.Length; ++i)
                {
                    char c = sPattern.Bytes[i];
                    if (BytePattern.TryParseHexDigit(c, out byte b))
                    {
                        bb |= (b << shift);
                        mm |= (0x0F << shift);
                        shift -= 4;
                        if (shift < 0)
                        {
                            bytes.Add((byte)bb);
                            mask.Add((byte)mm);
                            shift = 4;
                            bb = mm = 0;
                        }
                    }
                    else if (c == '?' || c == '.')
                    {
                        shift -= 4;
                        if (shift < 0)
                        {
                            bytes.Add((byte)bb);
                            mask.Add((byte)mm);
                            shift = 4;
                            bb = mm = 0;
                        }
                    }
                }
                Debug.Assert(bytes.Count == mask.Count);
                if (bytes.Count == 0)
                    return null;
                return new MaskedPattern
                {
                    Bytes = bytes.ToArray(),
                    Mask = mask.ToArray()
                };
            }
            else
            {
                var bytes = BytePattern.FromHexBytes(sPattern.Bytes);
                var mask = BytePattern.FromHexBytes(sPattern.Mask);
                if (bytes.Length == 0)
                    return null;
                return new MaskedPattern
                {
                    Bytes = bytes.ToArray(),
                    Mask = mask.ToArray()
                };
            }
        }

        private Dictionary<Address, ExternalProcedure> LoadPlatformProcedures(Platform platform)
        {
            if (platform.MemoryMap != null && platform.MemoryMap.Segments != null)
            {
                platform.EnsureTypeLibraries(platform.Name);
                var tser = new TypeLibraryDeserializer(platform, true, platform.Metadata);
                var sser = new ProcedureSerializer(platform, tser, platform.DefaultCallingConvention);
                return platform.MemoryMap.Segments.SelectMany(s => s.Procedures)
                    .OfType<Procedure_v1>()
                    .Where(p => p.Name != null)
                    .Select(p =>
                        (addr: platform.Architecture.TryParseAddress(p.Address, out var addr) ? addr : null,
                         ext:  new ExternalProcedure(
                             p.Name!,
                             sser.Deserialize(p.Signature, platform.Architecture.CreateFrame())
                                ?? new Types.FunctionType())))
                    .Where(p => p.addr != null)
                    .ToDictionary(p => p.addr!, p => p.ext);
            }
            else
            {
                return new Dictionary<Address, ExternalProcedure>();
            }
        }

        public override string ToString()
        {
            return Description ?? "";
        }
    }
}
