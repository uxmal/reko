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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.ImageLoaders.Hunk
{
    public class HunkFile
    {
        public FileType type;
        public List<Hunk> hunks;
        public HeaderHunk header;
        public List<List<Hunk>> segments;
        public Hunk overlay;
        public List<HeaderHunk> overlay_headers;
        public List<List<List<Hunk>>> overlay_segments;
        public List<Lib> libs;
        public List<Unit> units;

        public HunkFile()
        {
            this.type = FileType.TYPE_UNKNOWN;
            this.hunks = new List<Hunk>();
            this.header = null;
            this.segments = new List<List<Hunk>>();
            this.overlay = null;
            this.overlay_headers = null;
            this.overlay_segments = null;
            this.libs = null;
            this.units = null;

        }
    }

}
