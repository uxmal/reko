using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;

namespace Reko.ImageLoaders.MzExe.Ne
{
    public class ResourceLoader
    {
        private byte[] RawImage;
        private uint OffRsrcTable;
        private ushort ResourceTableEntries;

        public ResourceLoader(byte[] rawImage, uint offRsrcTable, ushort cResourceTableEntries)
        {
            RawImage = rawImage;
            OffRsrcTable = offRsrcTable;
            ResourceTableEntries = cResourceTableEntries;
        }

        /// <summary>
        /// Reads in the NE image resources.
        /// </summary>
        public List<ProgramResource> LoadResources()
        {
            List<ProgramResource> resources = new List<ProgramResource>();
            var rsrcTable = new LeImageReader(RawImage, OffRsrcTable);
            var rdr = rsrcTable.Clone();
            ushort size_shift = rdr.ReadLeUInt16();
            var resourceGroups = new Dictionary<ResourceType, ProgramResourceGroup>();
            var iconIds = new Dictionary<ushort, ProgramResourceInstance>();
            ushort rsrcType = rdr.ReadLeUInt16();
            while (rsrcType != 0)
            {
                var rt = Win16ResourceType.FromInt(rsrcType);
                if (!resourceGroups.TryGetValue(rt, out var resGrp))
                {
                    resGrp = new ProgramResourceGroup { Name = rt.Name };
                    resourceGroups.Add(rt, resGrp);
                }
                ushort typeCount = rdr.ReadLeUInt16();
                uint resLoader = rdr.ReadLeUInt32();
                for (int count = typeCount; count > 0; --count)
                {
                    ushort nameOffset = rdr.ReadLeUInt16();
                    ushort nameLength = rdr.ReadLeUInt16();
                    ushort nameFlags = rdr.ReadLeUInt16();
                    ushort nameId = rdr.ReadLeUInt16();
                    ushort nameHandle = rdr.ReadLeUInt16();
                    ushort nameUsage = rdr.ReadLeUInt16();

                    string resname;
                    if ((nameId & 0x8000) != 0)
                    {
                        resname = (nameId & ~0x8000).ToString();
                    }
                    else
                    {
                        resname = ReadByteLengthString(rsrcTable, nameId);
                    }

                    var offset = (uint) nameOffset << size_shift;
                    var rdrRsrc = new LeImageReader(RawImage, offset);
                    var rsrc = rdrRsrc.ReadBytes((uint) nameLength << size_shift);

                    var rsrcInstance = new ProgramResourceInstance
                    {
                        Name = resname,
                        Type = resGrp.Name,
                        Bytes = rsrc,
                        FileExtension = rt.FileExtension,
                    };
                    resGrp.Resources.Add(rsrcInstance);

                    if (rt == Win16ResourceType.Icon)
                        iconIds[(ushort) (nameId & ~0x8000)] = rsrcInstance;
                }
                resources.Add(resGrp);
                rsrcType = rdr.ReadLeUInt16();
            }

            PostProcessIcons(resourceGroups, iconIds, resources);
            PostProcessBitmaps(resourceGroups);

            return resources;
        }

        public struct ResourceTableEntry
        {
            public ushort etype;
            public ushort ename;
        }

        /// <summary>
        /// Reads in the NE resources from OS/2.
        /// </summary>
        public List<ProgramResource> LoadOs2Resources(NeImageLoader.NeSegment[] segments, ushort segmentCount, ushort alignmentShift)
        {
            List<ProgramResource> resources = new List<ProgramResource>();
            var rsrcTable = new LeImageReader(RawImage, OffRsrcTable);
            var rdr = rsrcTable.Clone();

            ResourceTableEntry[] entries = new ResourceTableEntry[ResourceTableEntries];
            for (int i = 0; i < entries.Length; i++)
            {
                entries[i].etype = rdr.ReadUInt16();
                entries[i].ename = rdr.ReadUInt16();
            }

            NeImageLoader.NeSegment[] resourceSegments = new NeImageLoader.NeSegment[ResourceTableEntries];
            Array.Copy(segments, segmentCount - ResourceTableEntries, resourceSegments, 0,
                ResourceTableEntries);
            NeImageLoader.NeSegment[] realSegments = new NeImageLoader.NeSegment[segmentCount - ResourceTableEntries];
            Array.Copy(segments, 0, realSegments, 0, realSegments.Length);
            segments = realSegments;

            var os2Resources = new SortedDictionary<ushort, ProgramResourceGroup>();

            for (int i = 0; i < entries.Length; i++)
            {
                os2Resources.TryGetValue(entries[i].etype, out ProgramResourceGroup? resGrp);
                var rt = Os2ResourceType.FromInt(entries[i].etype);
                if (resGrp is null) resGrp = new ProgramResourceGroup { Name = rt.Name };

                uint length = resourceSegments[i].DataLength;
                var dataOffset = (uint) (resourceSegments[i].DataOffset << alignmentShift);

                if (length == 0) length = 65536;
                if (dataOffset == 0) dataOffset = 65536;

                if ((resourceSegments[i].Flags & (ushort) 0x4000) != 0)
                    length <<= alignmentShift;

                var rdrRsrc = new LeImageReader(RawImage, dataOffset);
                var rsrc = rdrRsrc.ReadBytes(length);

                var rsrcInstance = new ProgramResourceInstance
                {
                    Name = $"{entries[i].ename}",
                    Type = "Os2_" + resGrp.Name,
                    Bytes = rsrc,
                    FileExtension = rt.FileExtension
                };
                resGrp.Resources.Add(rsrcInstance);

                os2Resources.Remove(entries[i].etype);
                os2Resources[entries[i].etype]= resGrp;
            }

            foreach (var kvp in os2Resources)
            {
                resources.Add(kvp.Value);
            }

            return resources;
        }

        string ReadByteLengthString(EndianImageReader rdr, int offset)
        {
            var clone = rdr.Clone();
            clone.Offset = clone.Offset + offset;
            var len = clone.ReadByte();
            var abStr = clone.ReadBytes(len);
            return Encoding.ASCII.GetString(abStr);
        }

        /// <summary>
        /// Build icons out of the icon groups + icons.
        /// </summary>
        /// <param name="iconGroups"></param>
        /// <param name="icons"></param>
        private void PostProcessIcons(
            Dictionary<ResourceType, ProgramResourceGroup> resGrps,
            Dictionary<ushort, ProgramResourceInstance> iconIds,
            List<ProgramResource> resources)
        {
            resGrps.TryGetValue(Win16ResourceType.GroupIcon, out ProgramResourceGroup? iconGroups);
            resGrps.TryGetValue(Win16ResourceType.Icon, out ProgramResourceGroup? icons);
            if (icons is null)
            {
                if (iconGroups is not null)
                    resources.Remove(iconGroups);
                return;
            }
            if (iconGroups is null)
            {
                if (icons is null)
                    resources.Remove(icons!);
                return;
            }

            foreach (ProgramResourceInstance iconGroup in iconGroups.Resources)
            {
                var r = new BinaryReader(new MemoryStream(iconGroup.Bytes!));
                var stm = new MemoryStream();
                var w = new BinaryWriter(stm);

                // Copy the group header
                w.Write(r.ReadInt16());
                w.Write(r.ReadInt16());
                short dirEntries = r.ReadInt16();
                w.Write(dirEntries);

                var icIds = new List<(ushort, int)>();
                for (int i = 0; i < dirEntries; ++i)
                {
                    w.Write(r.ReadInt32());
                    w.Write(r.ReadInt32());
                    w.Write(r.ReadInt32());
                    var iconId = r.ReadUInt16();
                    w.Flush();
                    icIds.Add((iconId, (int) stm.Position));
                    w.Write(0);
                }
                foreach (var id in icIds)
                {
                    var icon = iconIds[id.Item1];
                    var icOffset = (int) w.Seek(0, SeekOrigin.Current);
                    w.Seek(id.Item2, SeekOrigin.Begin);
                    w.Write(icOffset);
                    w.Seek(icOffset, SeekOrigin.Begin);
                    w.Write(icon.Bytes!);
                }
                iconGroup.Bytes = stm.ToArray();
                iconGroup.Type = "ICON";
                iconGroup.FileExtension = ".ico";
            }
            iconGroups.Name = "ICON";
            resources.Remove(icons);
        }

        void PostProcessBitmaps(Dictionary<ResourceType, ProgramResourceGroup> resGrps)
        {
            if (!resGrps.TryGetValue(Win16ResourceType.Bitmap, out ProgramResourceGroup? bitmaps))
                return;
            foreach (ProgramResourceInstance bitmap in bitmaps.Resources)
            {
                var stm = new MemoryStream();
                var bw = new BinaryWriter(stm);

                bw.Write('B');
                bw.Write('M');
                bw.Write(14 + bitmap.Bytes!.Length);
                bw.Write(0);
                bw.Write(14);
                bw.Write(bitmap.Bytes, 0, bitmap.Bytes.Length);
                bw.Flush();
                bitmap.Bytes = stm.ToArray();
            }
        }
    }
}