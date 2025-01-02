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

using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.ImageLoaders.Archives.Lbr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Archives;

public class LbrLoader : ImageLoader
{
    // https://cdn.preterhuman.net/texts/computing/programming/FORMATS/ludef5.txt


    public LbrLoader(IServiceProvider services, ImageLocation imageLocation, byte[] imgRaw) : base(services, imageLocation, imgRaw)
    {
    }

    public override ILoadedImage Load(Address? addrLoad)
    {
        var rdr = new ByteImageReader(RawImage);
        var result = new LbrArchive(ImageLocation, RawImage, new());
        ReadDirectory(result, rdr);
        return result;
    }

    private void ReadDirectory(LbrArchive archive, ByteImageReader rdr)
    {
        if (!DirectoryEntry.TryRead(archive, this.ImageLocation, rdr, out var firstEntry))
            return;
        if (firstEntry.Status != Status.Active ||
            !string.IsNullOrWhiteSpace(firstEntry.Name) ||
            !string.IsNullOrWhiteSpace(firstEntry.Extension))
        {
            return;
        }
        var cbDirectory = firstEntry.Length * DirectoryEntry.SectorSize;
        var dirEntries = archive.RootEntries;
        while (rdr.Offset < cbDirectory)
        {
            var offset = rdr.Offset;
            if (!DirectoryEntry.TryRead(archive, ImageLocation, rdr, out var entry))
            {
                Services.GetService<IEventListener>()?.Warn($"Unable to read LBR directory entry at offset {offset}.");
                break;
            }
            if (entry.Status == Status.Active)
            {
                dirEntries.Add(entry);
            }
        }
    }
}
