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

using Reko.Core.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Reko.ImageLoaders.MzExe.Pe
{
    /// <summary>
    /// Registry of resource types found in PE files.
    /// </summary>
    public static class PeResourceType
    {
        // https://docs.microsoft.com/en-us/windows/win32/menurc/resource-types
        const int RT_NEWRESOURCE = 0x2000;
        const int RT_ERROR = 0x7fff;
        const int RT_CURSOR = 1;
        const int RT_BITMAP = 2;
        const int RT_ICON = 3;
        const int RT_MENU = 4;
        const int RT_DIALOG = 5;
        const int RT_STRING = 6;
        const int RT_FONTDIR = 7;
        const int RT_FONT = 8;
        const int RT_ACCELERATOR = 9;
        const int RT_RCDATA = 10;
        const int RT_MESSAGETABLE = 11;
        const int RT_GROUP_CURSOR = 12;
        const int RT_GROUP_ICON = 14;
        const int RT_VERSION = 16;
        const int RT_MANIFEST = 24;
        const int RT_NEWBITMAP = (RT_BITMAP | RT_NEWRESOURCE);
        const int RT_NEWMENU = (RT_MENU | RT_NEWRESOURCE);
        const int RT_NEWDIALOG = (RT_DIALOG | RT_NEWRESOURCE);

        private static Dictionary<int, ResourceType> knownTypes;

        public static ResourceType CURSOR { get; } = new ResourceType(RT_CURSOR, "CURSOR", ".cur");
        public static ResourceType BITMAP { get; } = new ResourceType(RT_BITMAP, "BITMAP", ".bmp");
        public static ResourceType ICON { get; } = new ResourceType(RT_ICON, "ICON", ".ico");
        public static ResourceType MENU { get; } = new ResourceType(RT_MENU, "MENU", ".mnu");
        public static ResourceType DIALOG { get; } = new ResourceType(RT_DIALOG, "DIALOG", ".dlg");
        public static ResourceType STRING { get; } = new ResourceType(RT_STRING, "STRING", ".str");
        public static ResourceType FONTDIR { get; } = new ResourceType(RT_FONTDIR, "FONTDIR", ".fontdir");
        public static ResourceType FONT { get; } = new ResourceType(RT_FONT, "FONT", ".font");
        public static ResourceType ACCELERATOR { get; } = new ResourceType(RT_ACCELERATOR, "ACCELERATOR", ".acc");
        public static ResourceType RCDATA { get; } = new ResourceType(RT_RCDATA, "RCDATA", ".rc");
        public static ResourceType MESSAGETABLE { get; } = new ResourceType(RT_MESSAGETABLE, "MESSAGETABLE", ".msgs");
        public static ResourceType GROUP_CURSOR { get; } = new ResourceType(RT_GROUP_CURSOR, "GROUP_CURSOR", ".cur");
        public static ResourceType GROUP_ICON { get; } = new ResourceType(RT_GROUP_ICON, "GROUP_ICON", ".ico");
        public static ResourceType VERSION { get; } = new ResourceType(RT_VERSION, "VERSION", ".ver");
        public static ResourceType MANIFEST { get; } = new ResourceType(RT_MANIFEST, "MANIFEST", ".xml");
        public static ResourceType NEWBITMAP { get; } = new ResourceType(RT_NEWBITMAP, "NEWBITMAP", ".bmp");
        public static ResourceType NEWMENU { get; } = new ResourceType(RT_NEWMENU, "NEWMENU", ".mnu");
        public static ResourceType NEWDIALOG { get; } = new ResourceType(RT_NEWDIALOG, "NEWDIALOG", ".dlg");

        public static ResourceType FromInt(int id)
        {
            if (knownTypes.TryGetValue(id, out ResourceType rt))
                return rt;
            return new ResourceType(id, id.ToString(), ".dat");
        }
        static PeResourceType()
        {
            knownTypes = ResourceType.MakeDictionary(typeof(PeResourceType));
        }
    }
}
