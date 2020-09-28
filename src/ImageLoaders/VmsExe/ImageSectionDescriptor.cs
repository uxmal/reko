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

namespace Reko.ImageLoaders.VmsExe
{
    public class ImageSectionDescriptor
    {
        public ushort Size { get; internal set; }
        public ushort NumPages { get; internal set; }
        public uint StartVPage { get; internal set; }
        public uint Flags { get; internal set; }
        public uint RvaFile { get; internal set; }
        public uint GlobalSectionIdent { get; internal set; }
        public string SectionName { get; internal set; }

        public override string ToString()
        {
            return string.Format(
                "Isd: {0:X4} {1:X4} {2:X8} {3:X8} {4:X8} {5:X8} {6}",
                Size,
                NumPages,
                StartVPage,
                Flags,
                RvaFile,
                GlobalSectionIdent,
                SectionName ?? "<none>");
        }
    }

}