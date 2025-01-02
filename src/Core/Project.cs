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

using Reko.Core.Collections;
using Reko.Core.Loading;
using Reko.Core.Scripts;
using System.Diagnostics;
using System.IO;

namespace Reko.Core
{
    /// <summary>
    /// A Reko decompiler project.
    /// </summary>
    public class Project : ILoadedImage
    {
        public Project(ImageLocation location)
        {
            Programs = new ObservableRangeCollection<Program>();
            MetadataFiles = new ObservableRangeCollection<MetadataFile>();
            ScriptFiles = new ConcurrentObservableCollection<ScriptFile>();
            LoadedMetadata = new TypeLibrary();
            Location = location;
        }

        /// <summary>
        /// This constructor is used by unit tests only, when an image location
        /// is not relevant.
        /// </summary>
        public Project() : this(default!)
        {
        }

        /// <summary>
        /// Creates a <see cref="Project"/> from a single <see cref="Program"/>.
        /// </summary>
        /// <param name="program">The <see cref="Program"/> from which to build the project.</param>
        /// <returns>A <see cref="Project"/> instance.</returns>
        public static Project FromSingleProgram(Program program)
        {
            Debug.Assert(program.Location is not null, "Program is missing a location.");
            var projectFileName = Path.ChangeExtension(
                program.Location.FilesystemPath,
                Serialization.Project_v5.FileExtension);
            var project = new Project(ImageLocation.FromUri(projectFileName));
            project.AddProgram(program.Location, program);
            project.LoadedMetadata = program.Platform.CreateMetadata();
            program.EnvironmentMetadata = project.LoadedMetadata;
            program.User.ExtractResources = true;
            return project;
        }

        /// <summary>
        /// The URI from which this project was loaded.
        /// </summary>
        public ImageLocation Location { get; }

        /// <summary>
        /// A list of binaries that are to be decompiled.
        /// </summary>
        public ObservableRangeCollection<Program> Programs { get; }

        /// <summary>
        /// A list of user-provided metadata files that aid in the process
        /// of decompilation.
        /// </summary>
        public ObservableRangeCollection<MetadataFile> MetadataFiles { get; }

        /// <summary>
        /// A list of user-provided script files that can customize the process
        /// of decompilation.
        /// </summary>
        public ConcurrentObservableCollection<ScriptFile> ScriptFiles { get; }

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
