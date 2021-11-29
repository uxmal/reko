#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

using Reko.Core.Lib;
using Reko.Core.Loading;
using Reko.Core.Scripts;

namespace Reko.Core
{
    /// <summary>
    /// A Reko decompiler project.
    /// </summary>
    public class Project : ILoadedImage
    {
        public Project()
        {
            Programs = new ObservableRangeCollection<Program>();
            MetadataFiles = new ObservableRangeCollection<MetadataFile>();
            ScriptFiles = new ConcurrentObservableCollection<ScriptFile>();
            LoadedMetadata = new TypeLibrary();
            Location = default!;
        }

        /// <summary>
        /// The URI from which this project was loaded.
        /// </summary>
        public ImageLocation Location { get; set; }

        /// <summary>
        /// A list of binaries that are to be decompiled.
        /// </summary>
        public ObservableRangeCollection<Program> Programs { get; private set; }
        /// <summary>
        /// A list of user-provided metadata files that aid in the process
        /// of decompilation.
        /// </summary>
        public ObservableRangeCollection<MetadataFile> MetadataFiles { get; private set; }

        /// <summary>
        /// A list of user-provided script files that can customize the process
        /// of decompilation.
        /// </summary>
        public readonly ConcurrentObservableCollection<ScriptFile> ScriptFiles;

        /// <summary>
        /// All the metadata collected from both platforms and user-provided metadata
        /// files.
        /// </summary>
        public TypeLibrary LoadedMetadata { get; set; }

        public T Accept<T, C>(ILoadedImageVisitor<T, C> visitor, C context)
            => visitor.VisitProject(this, context);

        public void AddProgram(ImageLocation programLocation, Program program)
        {
            program.Location = programLocation;
            program.EnsureDirectoryNames(programLocation);
            this.Programs.Add(program);
        }

        /// <summary>
        /// Call event handlers defined at user-defined scripts.
        /// </summary>
        /// <param name="event">
        /// Fired event.
        /// </param>
        public void FireScriptEvent(ScriptEvent @event)
        {
            foreach (var scriptFile in ScriptFiles)
            {
                foreach (var program in Programs)
                {
                    scriptFile.FireEvent(@event, program);
                }
            }
        }
    }
}
