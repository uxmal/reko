#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Environments.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.ImageLoaders.MzExe.Pe
{
    /// <summary>
    /// Loads resources from the resource section of a PE executable file.
    /// </summary>
    public class ResourceLoader
    {
        private readonly ByteMemoryArea imgLoaded;
        private readonly uint rvaResources;

        public ResourceLoader(ByteMemoryArea imgLoaded, uint rvaResources)
        {
            this.imgLoaded = imgLoaded;
            this.rvaResources = rvaResources;
        }

        /// <summary>
        /// Loads the resources from the resource file.
        /// </summary>
        /// <remarks>
        /// Although the design of the PE resource format is such that there can be 
        /// up to 255 levels of directories/leaf nodes, in practice PE files are organized 
        /// so that the resource are in three levels. The first level is the type of resource
        /// (icons, cursors, dialogs &c). The second level is the language level (e.g. localized 
        /// versions of dialogs in various languages). The leaf nodes of the resource tree
        /// are on the third level.
        /// </remarks>
        /// <returns>A list of <see cref="ProgramResource"/>s.</returns>
        public List<ProgramResource> Load()
        {
            if (rvaResources == 0)
                return new List<ProgramResource>();

            var rsrcSection = new LeImageReader(this.imgLoaded, rvaResources);
            var rdr = rsrcSection.Clone();

            return ReadResourceDirectory(rdr);
        }

        /// <summary>
        /// Reads a resource directory, starting at the position of the given image 
        /// reader.
        /// </summary>
        /// <param name="rdr">A little endian <see cref="EndianImageReader"/>.</param>
        /// <returns>A list of the resources found in the directory.</returns>
        public List<ProgramResource> ReadResourceDirectory(EndianImageReader rdr)
        {
            const uint DIR_MASK = 0x80000000;
            var flags = rdr.ReadUInt32();
            var date = rdr.ReadUInt32();
            var version = rdr.ReadUInt32();
            var cNameEntries = rdr.ReadUInt16();
            var cIdEntries = rdr.ReadUInt16();
            var entries = new List<ProgramResource>();

            // Read the named entries.
            for (int i = 0; i < cNameEntries; ++i)
            {
                var rvaName = rdr.ReadUInt32();
                var rvaEntry = rdr.ReadUInt32();
                var subRdr = new LeImageReader(imgLoaded, rvaResources + (rvaEntry & ~DIR_MASK));
                if ((rvaEntry & DIR_MASK) == 0)
                    throw new BadImageFormatException();
                if ((rvaName & DIR_MASK) != 0)
                {
                    var e = new ProgramResourceGroup
                    {
                        //Name = ReadResourceString(rvaName),
                        Name = ReadResourceUtf16leString(rvaResources + (rvaName & ~DIR_MASK)),
                    };
                    e.Resources.AddRange(ReadNameDirectory(subRdr, PeResourceType.FromInt(0)));
                    entries.Add(e);
                }
            }
            // Read the entries accessed by numeric ID.
            for (int i = 0; i < cIdEntries; ++i)
            {
                var id = rdr.ReadInt32();
                var rvaEntry = rdr.ReadUInt32();
                var subRdr = new LeImageReader(imgLoaded, rvaResources + (rvaEntry & ~DIR_MASK));
                if ((rvaEntry & DIR_MASK) == 0)
                    throw new BadImageFormatException();
                var rt = PeResourceType.FromInt(id);
                var e = new ProgramResourceGroup
                {
                    Name = rt.Name
                };
                e.Resources.AddRange(ReadNameDirectory(subRdr, rt));
                entries.Add(e);
            }
            return entries;
        }

        public List<ProgramResource> ReadNameDirectory(
            EndianImageReader rdr,
            ResourceType resourceType)
        {
            const uint DIR_MASK = 0x80000000;
            var flags = rdr.ReadUInt32();
            var date = rdr.ReadUInt32();
            var version = rdr.ReadUInt32();
            var cNameEntries = rdr.ReadUInt16();
            var cIdEntries = rdr.ReadUInt16();
            var entries = new List<ProgramResource>();
            for (int i = 0; i < cNameEntries; ++i)
            {
                var rvaName = rdr.ReadUInt32();
                var rvaEntry = rdr.ReadUInt32();
                var subRdr = new LeImageReader(imgLoaded, rvaResources + (rvaEntry & ~DIR_MASK));
                if ((rvaEntry & DIR_MASK) == 0)
                    throw new BadImageFormatException();
                var e = new ProgramResourceGroup
                {
                    Name = ReadResourceUtf16leString(rvaResources + (rvaName & ~DIR_MASK)),
                };
                e.Resources.AddRange(ReadLanguageDirectory(subRdr, resourceType, e.Name));
                entries.Add(e);
            }
            for (int i = 0; i < cIdEntries; ++i)
            {
                var id = rdr.ReadUInt32();
                var rvaEntry = rdr.ReadUInt32();
                var subRdr = new LeImageReader(imgLoaded, rvaResources + (rvaEntry & ~DIR_MASK));
                if ((rvaEntry & DIR_MASK) == 0)
                    throw new BadImageFormatException();
                var e = new ProgramResourceGroup
                {
                    Name = id.ToString(),
                };
                e.Resources.AddRange(ReadLanguageDirectory(subRdr, resourceType, e.Name));
                entries.Add(e);
            }
            return entries;
        }

        public List<ProgramResource> ReadLanguageDirectory(
            EndianImageReader rdr, 
            ResourceType resourceType, 
            string resourceId)
        {
            const uint DIR_MASK = 0x80000000;
            var flags = rdr.ReadUInt32();
            var date = rdr.ReadUInt32();
            var version = rdr.ReadUInt32();
            var cNameEntries = rdr.ReadUInt16();
            var cIdEntries = rdr.ReadUInt16();
            var entries = new List<ProgramResource>();
            for (int i = 0; i < cNameEntries; ++i)
            {
                var rvaName = rdr.ReadUInt32();
                var rvaEntry = rdr.ReadUInt32();
                var subRdr = new LeImageReader(imgLoaded, rvaResources + (rvaEntry & ~DIR_MASK));
                if ((rvaEntry & DIR_MASK) != 0)
                    throw new BadImageFormatException();
                entries.Add(ReadResourceEntry(subRdr, resourceId, ReadResourceString(rvaName), resourceType));
            }
            for (int i = 0; i < cIdEntries; ++i)
            {
                var id = rdr.ReadInt32();
                var rvaEntry = rdr.ReadUInt32();
                var subRdr = new LeImageReader(imgLoaded, rvaResources + (rvaEntry & ~DIR_MASK));
                if ((rvaEntry & DIR_MASK) != 0)
                    throw new BadImageFormatException();
                entries.Add(ReadResourceEntry(subRdr, resourceId, id.ToString(), resourceType));
            }
            return entries;
        }

        public ProgramResourceInstance ReadResourceEntry(
            EndianImageReader rdr, 
            string resourceId, 
            string sLcid, 
            ResourceType resourceType)
        {
            var rvaData = rdr.ReadUInt32();
            var size = rdr.ReadUInt32();
            var codepage = rdr.ReadInt32();
            var padding = rdr.ReadUInt32();
            var abResource = new byte[size];
            Array.Copy(imgLoaded.Bytes, (int) rvaData, abResource, 0, abResource.Length);

            if (resourceType == PeResourceType.BITMAP)
            {
                abResource = PostProcessBitmap(abResource);
            }

            string? encodingName = GetEncodingName(codepage);
            string? langTag = GetLanguageTag(sLcid);
            return new ProgramResourceInstance
            {
                Name = $"{resourceId}-{langTag}",
                Type = resourceType.Name,
                TextEncoding = encodingName,
                FileExtension = resourceType.FileExtension,
                Bytes = abResource,
            };
        }

        private string? GetEncodingName(int codepage)
        {
            if (codepage == 0)
                return null;
            if (CodePages.ToEncodings.TryGetValue(codepage, out string? encoding))
                return encoding;
            return $"CP{codepage}";
        }

        private string? GetLanguageTag(string sLcid)
        {
            if (!int.TryParse(sLcid, out int lcid))
                return sLcid;
            lcid &= 0b11_1111_1111;
            if (LocaleIds.ToLanguageTags.TryGetValue(lcid, out string? langTag))
                return langTag;
            return $"x{sLcid:X4}";
        }

        /// <summary>
        /// Rebuild the BITMAPFILEHEADER structure, which isn't present in
        /// resource files.
        /// </summary>
        /// <param name="abResource">DIB image (BITMAPINFOHEADER + bitmap data)</param>
        /// <returns>A byte image containing the BITMAPFILEHEADER which can be 
        /// saved as a .BMP file.</returns>
        private byte[] PostProcessBitmap(byte[] abResource)
        {
            var stm = new MemoryStream();
            var bw = new BinaryWriter(stm); // Always writes little-endian.

            bw.Write('B');
            bw.Write('M');
            bw.Write(14 + abResource.Length);
            bw.Write(0);
            bw.Write(14);
            bw.Write(abResource, 0, abResource.Length);
            bw.Flush();
            return stm.ToArray();
        }


        public string ReadResourceString(uint rva)
        {
            var rdr = new LeImageReader(imgLoaded, rva);
            var len = rdr.ReadLeInt16();
            var abStr = rdr.ReadBytes(len);
            return Encoding.ASCII.GetString(abStr);
        }

        public string ReadResourceUtf16leString(uint rva)
        {
            var rdr = new LeImageReader(imgLoaded, rva);
            var len = rdr.ReadLeInt16();
            var abStr = rdr.ReadBytes(len * 2);
            return Encoding.Unicode.GetString(abStr);
        }
    }

#if NIZ
        /**
     * Recursively load resources.  Currently the only resource type that is fully implemented is VS_VERSIONINFO.
     */
    private void traverse(String path, ImageResourceDirectory dir, IRandomAccess ra, long rba, long rva) throws IOException {
        ImageResourceDirectoryEntry[] entries = dir.getChildEntries();
        for (int i=0; i < entries.length; i++) {
            ImageResourceDirectoryEntry entry = entries[i];
            String name = entry.getName(ra, rba);
            if (entry.isDir()) {
                int type = entry.getType();
                if (path.length() == 0 && type >= 0 && type < Types.NAMES.length) {
                    name = Types.NAMES[type];
                }
                ra.seek(rba + entry.getOffset());
                traverse(path + name + "/", new ImageResourceDirectory(ra), ra, rba, rva);
            } else {
                ImageResourceDataEntry de = entry.getDataEntry(ra, rba, rva);
		if (path.startsWith(Types.NAMES[Types.RT_VERSION])) {
		    ra.seek(de.getDataAddress());
		    versionInfo = new VsVersionInfo(ra);
		    resources.putData(path + name, versionInfo);
		} else {
		    resources.putData(path + name, de);
		}
            }
        }
    }
#endif
}
