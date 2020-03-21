using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Reko.Core;

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

        private byte[] RawImage;
        private uint OffRsrcTable;

        public ResourceLoader(byte[] rawImage, uint offRsrcTable)
        {
            RawImage = rawImage;
            OffRsrcTable = offRsrcTable;
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
            ProgramResourceGroup bitmaps = null;
            ProgramResourceGroup iconGroups = null;
            ProgramResourceGroup icons = null;
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
        string ReadByteLengthString(EndianImageReader rdr, int offset)
        {
            var clone = rdr.Clone();
            clone.Offset = clone.Offset + offset;
            var len = clone.ReadByte();
            var abStr = clone.ReadBytes(len);
            return Encoding.ASCII.GetString(abStr);
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
            ProgramResourceGroup iconGroups,
            ProgramResourceGroup icons,
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
                    resources.Remove(icons);
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

                var icIds = new List<Tuple<ushort, int>>();
                for (int i = 0; i < dirEntries; ++i)
                {
                    w.Write(r.ReadInt32());
                    w.Write(r.ReadInt32());
                    w.Write(r.ReadInt32());
                    var iconId = r.ReadUInt16();
                    w.Flush();
                    icIds.Add(Tuple.Create(iconId, (int) stm.Position));
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

        void PostProcessBitmaps(ProgramResourceGroup bitmaps)
        {
            if (bitmaps == null)
                return;
            foreach (ProgramResourceInstance bitmap in bitmaps.Resources)
            {
                var stm = new MemoryStream();
                var bw = new BinaryWriter(stm);

                bw.Write('B');
                bw.Write('M');
                bw.Write(14 + bitmap.Bytes.Length);
                bw.Write(0);
                bw.Write(14);
                bw.Write(bitmap.Bytes, 0, bitmap.Bytes.Length);
                bw.Flush();
                bitmap.Bytes = stm.ToArray();
            }
        }
    }
}