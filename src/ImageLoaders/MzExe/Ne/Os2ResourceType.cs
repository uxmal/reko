using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.MzExe.Ne
{
    public static class Os2ResourceType
    {
        /// <summary>
        /// OS/2 Resource types. Common between NE and LX files.
        /// </summary>

        /// <summary>mouse pointer shape</summary>
        public static ResourceType Pointer = new ResourceType(1, "POINTER", ".xxx");
        /// <summary>bitmap</summary>
        public static ResourceType Bitmap = new ResourceType(2, "BITMAP", ".xxx");
        /// <summary>menu template</summary>
        public static ResourceType Menu = new ResourceType(3, "MENU", ".xxx");
        /// <summary>dialog template</summary>
        public static ResourceType Dialog = new ResourceType(4, "DIALOG", ".xxx");
        /// <summary>string tables</summary>
        public static ResourceType String = new ResourceType(5, "STRING", ".xxx");
        /// <summary>font directory</summary>
        public static ResourceType Fontdir = new ResourceType(6, "FONTDIR", ".xxx");
        /// <summary>font</summary>
        public static ResourceType Font = new ResourceType(7, "FONT", ".xxx");
        /// <summary>accelerator tables</summary>
        public static ResourceType Acceltable = new ResourceType(8, "ACCELTABLE", ".xxx");
        /// <summary>binary data</summary>
        public static ResourceType RcData = new ResourceType(9, "RCDATA", ".xxx");
        /// <summary>error msg     tables</summary>
        public static ResourceType Message = new ResourceType(10, "MESSAGE", ".xxx");
        /// <summary>dialog include file name</summary>
        public static ResourceType DlgInclude = new ResourceType(11, "DLGINCLUDE", ".xxx");
        /// <summary>key to vkey tables</summary>
        public static ResourceType VKeyTable = new ResourceType(12, "VKEYTBL", ".xxx");
        /// <summary>key to UGL tables</summary>
        public static ResourceType KeyTable = new ResourceType(13, "KEYTBL", ".xxx");
        /// <summary>glyph to character tables</summary>
        public static ResourceType CharTable = new ResourceType(14, "CHARTBL", ".xxx");
        /// <summary>screen display information</summary>
        public static ResourceType DisplayInfo = new ResourceType(15, "DISPLAYINFO", ".xxx");
        /// <summary>function key area short form</summary>
        public static ResourceType FKAShort = new ResourceType(16, "FKASHORT", ".xxx");
        /// <summary>function key area long form</summary>
        public static ResourceType FKAlong = new ResourceType(17, "FKALONG", ".xxx");
        /// <summary>Help table for IPFC</summary>
        public static ResourceType HelpTable = new ResourceType(18, "HELPTABLE", ".xxx");
        /// <summary>Help subtable for IPFC</summary>
        public static ResourceType HelpSubtable = new ResourceType(19, "HELPSUBTABLE", ".xxx");
        /// <summary>DBCS uniq/font driver directory</summary>
        public static ResourceType FontDriverDir = new ResourceType(20, "FDDIR", ".xxx");
        /// <summary>DBCS uniq/font driver</summary>
        public static ResourceType FontDriver = new ResourceType(21, "FD", ".xxx");

        private static readonly Dictionary<int, ResourceType> knownTypes;

        public static ResourceType FromInt(int rsrcType)
        {
            if (knownTypes.TryGetValue(rsrcType, out ResourceType rt))
                return rt;
            return new ResourceType(rsrcType, rsrcType.ToString("X4"), ".dat");
        }

        static Os2ResourceType()
        {
            knownTypes = ResourceType.MakeDictionary(typeof(Os2ResourceType));
        }

    }
}
