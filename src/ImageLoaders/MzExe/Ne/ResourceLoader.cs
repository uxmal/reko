using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Reko.Core;
using Reko.Core.Memory;

namespace Reko.ImageLoaders.MzExe.Ne
{
    public class ResourceLoader
    {
        // Resource types
        const ushort NE_RSCTYPE_CURSOR = 0x8001;
        const ushort NE_RSCTYPE_BITMAP = 0x8002;
        const ushort NE_RSCTYPE_ICON = 0x8003;
        const ushort NE_RSCTYPE_MENU = 0x8004;
        const ushort NE_RSCTYPE_DIALOG = 0x8005;
        const ushort NE_RSCTYPE_STRING = 0x8006;
        const ushort NE_RSCTYPE_FONTDIR = 0x8007;
        const ushort NE_RSCTYPE_FONT = 0x8008;
        const ushort NE_RSCTYPE_ACCELERATOR = 0x8009;
        const ushort NE_RSCTYPE_RCDATA = 0x800a;
        const ushort NE_RSCTYPE_GROUP_CURSOR = 0x800c;
        const ushort NE_RSCTYPE_GROUP_ICON = 0x800e;
        const ushort NE_RSCTYPE_SCALABLE_FONTPATH = 0x80cc;

        /// <summary>OS/2 Resource types. Common between NE and LX files.</summary>
        public enum Os2ResourceTypes : ushort
        {
            /// <summary>mouse pointer shape</summary>
            RT_POINTER = 1,
            /// <summary>bitmap</summary>
            RT_BITMAP = 2,
            /// <summary>menu template</summary>
            RT_MENU = 3,
            /// <summary>dialog template</summary>
            RT_DIALOG = 4,
            /// <summary>string tables</summary>
            RT_STRING = 5,
            /// <summary>font directory</summary>
            RT_FONTDIR = 6,
            /// <summary>font</summary>
            RT_FONT = 7,
            /// <summary>accelerator tables</summary>
            RT_ACCELTABLE = 8,
            /// <summary>binary data</summary>
            RT_RCDATA = 9,
            /// <summary>error msg     tables</summary>
            RT_MESSAGE = 10,
            /// <summary>dialog include file name</summary>
            RT_DLGINCLUDE = 11,
            /// <summary>key to vkey tables</summary>
            RT_VKEYTBL = 12,
            /// <summary>key to UGL tables</summary>
            RT_KEYTBL = 13,
            /// <summary>glyph to character tables</summary>
            RT_CHARTBL = 14,
            /// <summary>screen display information</summary>
            RT_DISPLAYINFO = 15,
            /// <summary>function key area short form</summary>
            RT_FKASHORT = 16,
            /// <summary>function key area long form</summary>
            RT_FKALONG = 17,
            /// <summary>Help table for IPFC</summary>
            RT_HELPTABLE = 18,
            /// <summary>Help subtable for IPFC</summary>
            RT_HELPSUBTABLE = 19,
            /// <summary>DBCS uniq/font driver directory</summary>
            RT_FDDIR = 20,
            /// <summary>DBCS uniq/font driver</summary>
            RT_FD = 21
        }

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
            ProgramResourceGroup? bitmaps = null;
            ProgramResourceGroup? iconGroups = null;
            ProgramResourceGroup? icons = null;
            var iconIds = new Dictionary<ushort, ProgramResourceInstance>();
            ushort rsrcType = rdr.ReadLeUInt16();
            while (rsrcType != 0)
            {
                var resGrp = new ProgramResourceGroup { Name = GetResourceType(rsrcType) };
                if (rsrcType == NE_RSCTYPE_GROUP_ICON)
                    iconGroups = resGrp;
                else if (rsrcType == NE_RSCTYPE_ICON)
                    icons = resGrp;
                else if (rsrcType == NE_RSCTYPE_BITMAP)
                    bitmaps = resGrp;

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
                        Type = "Win16_" + resGrp.Name,
                        Bytes = rsrc,
                    };
                    resGrp.Resources.Add(rsrcInstance);

                    if (rsrcType == NE_RSCTYPE_ICON)
                        iconIds[(ushort) (nameId & ~0x8000)] = rsrcInstance;
                }
                resources.Add(resGrp);

                rsrcType = rdr.ReadLeUInt16();
            }

            PostProcessIcons(iconGroups, icons, iconIds, resources);
            PostProcessBitmaps(bitmaps);

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

            SortedDictionary<ushort, ProgramResourceGroup> os2Resources =
                new SortedDictionary<ushort, ProgramResourceGroup>();

            for (int i = 0; i < entries.Length; i++)
            {
                os2Resources.TryGetValue(entries[i].etype, out ProgramResourceGroup resGrp);

                if (resGrp == null) resGrp = new ProgramResourceGroup { Name = GetOs2ResourceName(entries[i].etype) };

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
        ///     Gets the name of a resource type according to its identifier
        /// </summary>
        /// <returns>The resource type name.</returns>
        /// <param name="id">Resource type identifier.</param>
        private string GetOs2ResourceName(ushort id)
        {
            switch (id)
            {
                case (int) Os2ResourceTypes.RT_POINTER: return "RT_POINTER";
                case (int) Os2ResourceTypes.RT_BITMAP: return "RT_BITMAP";
                case (int) Os2ResourceTypes.RT_MENU: return "RT_MENU";
                case (int) Os2ResourceTypes.RT_DIALOG: return "RT_DIALOG";
                case (int) Os2ResourceTypes.RT_STRING: return "RT_STRING";
                case (int) Os2ResourceTypes.RT_FONTDIR: return "RT_FONTDIR";
                case (int) Os2ResourceTypes.RT_FONT: return "RT_FONT";
                case (int) Os2ResourceTypes.RT_ACCELTABLE: return "RT_ACCELTABLE";
                case (int) Os2ResourceTypes.RT_RCDATA: return "RT_RCDATA";
                case (int) Os2ResourceTypes.RT_MESSAGE: return "RT_MESSAGE";
                case (int) Os2ResourceTypes.RT_DLGINCLUDE: return "RT_DLGINCLUDE";
                case (int) Os2ResourceTypes.RT_VKEYTBL: return "RT_VKEYTBL";
                case (int) Os2ResourceTypes.RT_KEYTBL: return "RT_KEYTBL";
                case (int) Os2ResourceTypes.RT_CHARTBL: return "RT_CHARTBL";
                case (int) Os2ResourceTypes.RT_DISPLAYINFO: return "RT_DISPLAYINFO";
                case (int) Os2ResourceTypes.RT_FKASHORT: return "RT_FKASHORT";
                case (int) Os2ResourceTypes.RT_FKALONG: return "RT_FKALONG";
                case (int) Os2ResourceTypes.RT_HELPTABLE: return "RT_HELPTABLE";
                case (int) Os2ResourceTypes.RT_HELPSUBTABLE: return "RT_HELPSUBTABLE";
                case (int) Os2ResourceTypes.RT_FDDIR: return "RT_FDDIR";
                case (int) Os2ResourceTypes.RT_FD: return "RT_FD";
                default: return $"{id}";
            }
        }

        private string GetResourceName(ushort id)
        {
            switch (id)
            {
            case NE_RSCTYPE_CURSOR: return "CURSOR";
            case NE_RSCTYPE_BITMAP: return "BITMAP";
            case NE_RSCTYPE_ICON: return "ICON";
            case NE_RSCTYPE_MENU: return "MENU";
            case NE_RSCTYPE_DIALOG: return "DIALOG";
            case NE_RSCTYPE_STRING: return "STRING";
            case NE_RSCTYPE_FONTDIR: return "FONTDIR";
            case NE_RSCTYPE_FONT: return "FONT";
            case NE_RSCTYPE_ACCELERATOR: return "ACCELERATOR";
            case NE_RSCTYPE_RCDATA: return "RCDATA";
            case NE_RSCTYPE_GROUP_CURSOR: return "CURSOR_GROUP";
            case NE_RSCTYPE_GROUP_ICON: return "ICON_GROUP";
            default: return id.ToString("X4");
            }
        }

        private string GetResourceType(ushort id)
        {
            switch (id)
            {
            case NE_RSCTYPE_BITMAP: return "Windows.BMP";
            case NE_RSCTYPE_ICON: return "Windows.ICO";
            default: return id.ToString("X4");
            }
        }

        /// <summary>
        /// Build icons out of the icon groups + icons.
        /// </summary>
        /// <param name="iconGroups"></param>
        /// <param name="icons"></param>
        private void PostProcessIcons(
            ProgramResourceGroup? iconGroups,
            ProgramResourceGroup? icons,
            Dictionary<ushort, ProgramResourceInstance> iconIds,
            List<ProgramResource> resources)
        {
            if (icons == null)
            {
                if (iconGroups != null)
                    resources.Remove(iconGroups);
                return;
            }
            if (iconGroups == null)
            {
                if (icons == null)
                    resources.Remove(icons!);
                return;
            }

            foreach (ProgramResourceInstance iconGroup in iconGroups.Resources)
            {
                var r = new BinaryReader(new MemoryStream(iconGroup.Bytes));
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
                    w.Write(icon.Bytes);
                }
                iconGroup.Bytes = stm.ToArray();
                iconGroup.Type = "Win16_ICON";
            }
            iconGroups.Name = "ICON";
            resources.Remove(icons);
        }

        void PostProcessBitmaps(ProgramResourceGroup? bitmaps)
        {
            if (bitmaps == null)
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