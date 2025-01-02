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

using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.ImageLoaders.MzExe.Ne
{
    public static class Win16ResourceType
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

        public static ResourceType Cursor { get; } = new ResourceType(0x8001, "CURSOR", ".cur");
        public static ResourceType Bitmap { get; } = new ResourceType(0x8002, "BITMAP", ".bmp");
        public static ResourceType Icon { get; } = new ResourceType(0x8003, "ICON", ".ico");
        public static ResourceType Menu { get; } = new ResourceType(0x8004, "MENU", ".mnu");
        public static ResourceType Dialog { get; } = new ResourceType(0x8005, "DIALOG", ".dlg");
        public static ResourceType String { get; } = new ResourceType(0x8006, "STRING", ".str");
        public static ResourceType Fontdir { get; } = new ResourceType(0x8007, "FONTDIR", ".fontdir");
        public static ResourceType Font { get; } = new ResourceType(0x8008, "FONT", ".fnt");
        public static ResourceType Accelerator { get; } = new ResourceType(0x8009, "ACCELERATOR", ".acc");
        public static ResourceType Rcdata { get; } = new ResourceType(0x800a, "RCDATA", ".rc");
        public static ResourceType GroupCursor { get; } = new ResourceType(0x800c, "GROUP_CURSOR", ".cur");
        public static ResourceType GroupIcon { get; } = new ResourceType(0x800e, "GROUP_ICON", ".ico");
        public static ResourceType ScalableFontpath { get; } = new ResourceType(0x80cc, "SCALABLE_FONTPATH", ".fontpath");

        private static readonly Dictionary<int, ResourceType> knownTypes;

        public static ResourceType FromInt(int rsrcType)
        {
            if (knownTypes.TryGetValue(rsrcType, out ResourceType rt))
                return rt;
            return new ResourceType(rsrcType, rsrcType.ToString("X4"), ".dat");
        }

        static Win16ResourceType()
        {
            knownTypes = ResourceType.MakeDictionary(typeof(Win16ResourceType));
        }
    }
}
