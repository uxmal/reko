#region License
/*
 * Copyright (C) 2021-2025 Sven Almgren.
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

using System.IO;
using System.Linq;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// ProjectVersionMigrator provides a set of methods to migrate older projects to later versions.
    /// </summary>
    public static class ProjectVersionMigrator
    {
        /// <summary>
        /// Migrate Project_v4 to Project_v5.
        /// </summary>
        public static Project_v5 MigrateProject(Project_v4 v4) => new Project_v5()
        {
            ArchitectureName = v4.ArchitectureName,
            PlatformName = v4.PlatformName,
            InputFiles = v4.InputFiles.Select(i => MigrateDecompilerInput(i)).ToList(),
            MetadataFiles = v4.MetadataFiles,
            AssemblerFiles = v4.AssemblerFiles,
        };

        /// <summary>
        /// Migrate DecompilerInput_v4 to DecompilerInput_v5.
        /// </summary>
        /// <remarks>
        /// Ignored fields (They existed in v4 but are unused in current code):
        ///     IntermediateFilename
        ///     GlobalsFilename
        /// </remarks>
        public static DecompilerInput_v5 MigrateDecompilerInput(DecompilerInput_v4 v4)
        {
            var v5 = new DecompilerInput_v5()
            {
                Filename = v4.Filename,
                Comment = v4.Comment,
                DisassemblyDirectory = v4.DisassemblyFilename is not null ? Path.GetDirectoryName(v4.DisassemblyFilename) : null,
                SourceDirectory = v4.OutputFilename is not null ? Path.GetDirectoryName(v4.OutputFilename) : null,
                IncludeDirectory = v4.TypesFilename is not null ? Path.GetDirectoryName(v4.TypesFilename) : null,
                ResourcesDirectory = v4.ResourcesDirectory,
                User = v4.User ?? new UserData_v4
                {
                    ExtractResources = true,
                }
            };

            if (string.IsNullOrWhiteSpace(v5.User.OutputFilePolicy))
                v5.User.OutputFilePolicy = Program.SingleFilePolicy;

            return v5;
        }
    }
}
