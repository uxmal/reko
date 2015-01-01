#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core
{
    /// <summary>
    /// Every ImageLoader must return a LoaderResults object.
    /// </summary>
    public class LoaderResults
    {
        public readonly LoadedImage Image; 
        public readonly ImageMap ImageMap;
        public readonly IProcessorArchitecture Architecture;
        public readonly Platform Platform;

        public LoaderResults(
            LoadedImage image, 
            ImageMap map, 
            IProcessorArchitecture arch,
            Platform platform)
        {
            this.Image = image;
            this.ImageMap = map;
            this.Architecture = arch;
            this.Platform = platform;
        }
    }
}
