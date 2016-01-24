#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    public class Project 
    {
        public Project()
        {
            Programs = new ObservableRangeCollection<Program>();
            MetadataFiles = new ObservableRangeCollection<MetadataFile>();
            Programs.CollectionChanged += Programs_CollectionChanged;
        }

        public ObservableRangeCollection<Program> Programs { get; private set; }
        public ObservableRangeCollection<MetadataFile> MetadataFiles { get; private set; }

        private TypeLibrary metadata;

        public TypeLibrary Metadata {
            get
            {
                if (metadata == null)
                {
                    var platform = DeterminePlatform();
                    if (platform == null)
                        return new TypeLibrary();
                    return platform.CreateMetadata();
                }
                return metadata;
            }
        }


        private IPlatform DeterminePlatform()
        {
            var platformsInUse = Programs.Select(p => p.Platform).Distinct().ToArray();
            if (platformsInUse.Length == 1 && platformsInUse[0] != null)
                return platformsInUse[0];
            return null;
        }


        public void LoadMetadataFile(ILoader loader, IPlatform platform, string filename)
        {
            if (metadata == null)
            {
                metadata = platform.CreateMetadata();
            }
            loader.LoadMetadata(filename, platform, metadata);
            foreach(var program in Programs)
            {
                program.Metadata = metadata;
            }
        }

        private void Programs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (metadata == null)
                return;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var program = (Program)item;
                        program.Metadata = metadata;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

    }
}
