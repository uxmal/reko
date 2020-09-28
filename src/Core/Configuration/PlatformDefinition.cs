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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public PlatformHeuristics_v1 Heuristics { get; set; }

        public string TypeName { get; set; }

        public string MemoryMapFile { get; set; }

        public virtual List<TypeLibraryDefinition> TypeLibraries { get; internal set; }
        public virtual List<TypeLibraryDefinition> CharacteristicsLibraries { get; internal set; }
        public virtual List<PlatformArchitectureDefinition> Architectures { get; internal set; }
        public virtual List<SignatureFileDefinition> SignatureFiles { get; internal set; }
        public virtual Dictionary<string, object> Options { get; internal set; }

        public virtual IPlatform Load(IServiceProvider services, IProcessorArchitecture arch)
        {
            var type = Type.GetType(TypeName, true);
            if (type == null)
                throw new TypeLoadException(
                    string.Format("Unable to load {0} environment.", Description));
            var cs = type.GetConstructors();
            var platform = (Platform)Activator.CreateInstance(type, services, arch);
            LoadSettingsFromConfiguration(services, platform);
            return platform;
        }

        public void LoadSettingsFromConfiguration(IServiceProvider services, Platform platform)
        {
            platform.Name = this.Name;
            if (!string.IsNullOrEmpty(MemoryMapFile))
            {
                platform.MemoryMap = MemoryMap_v1.LoadMemoryMapFromFile(services, MemoryMapFile, platform);
            }
            platform.Description = this.Description;
            platform.Heuristics = LoadHeuristics(this.Heuristics);
        }

        private PlatformHeuristics LoadHeuristics(PlatformHeuristics_v1 heuristics)
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
                    .Select(p => LoadMaskedPattern(p))
                    .Where(p => p.Bytes != null)
                    .ToArray();
            }

            return new PlatformHeuristics
            {
                ProcedurePrologs = prologs
            };
        }

        public MaskedPattern LoadMaskedPattern(BytePattern_v1 sPattern)
        {
            List<byte> bytes = null;
            List<byte> mask = null;
            if (sPattern.Bytes == null)
                return null;
            if (sPattern.Mask == null)
            {
                bytes = new List<byte>();
                mask = new List<byte>();
                int shift = 4;
                int bb = 0;
                int mm = 0;
                for (int i = 0; i < sPattern.Bytes.Length; ++i)
                {
                    char c = sPattern.Bytes[i];
                    byte b;
                    if (BytePattern.TryParseHexDigit(c, out b))
                    {
                        bb = bb | (b << shift);
                        mm = mm | (0x0F << shift);
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
            }
            else
            {
                bytes = BytePattern.FromHexBytes(sPattern.Bytes);
                mask = BytePattern.FromHexBytes(sPattern.Mask);
            }
            if (bytes.Count == 0)
                return null;
            else
                return new MaskedPattern
                {
                    Bytes = bytes.ToArray(),
                    Mask = mask.ToArray()
                };

        }


        public override string ToString()
        {
            return Description;
        }
    }
}
