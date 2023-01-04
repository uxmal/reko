#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

using Reko.Core.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Reko.Core.Serialization
{
    public class ProjectPersister
    {
        public ProjectPersister(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; private set; }

        /// <summary>
        /// Takes a project-relative path, and returns it as an absolute path.
        /// </summary>
        /// <param name="projectAbsPath"></param>
        /// <param name="projectRelative"></param>
        /// <returns></returns>
        public static string? ConvertToAbsolutePath(
            string projectAbsPath, string? projectRelative)
        {
            if (projectRelative is null)
                return null;
            var dir = Path.GetDirectoryName(projectAbsPath)!;
            if (string.IsNullOrEmpty(projectRelative))
                return dir;
            var combined = Path.Combine(dir, projectRelative);
            return Path.GetFullPath(combined);
        }

        public static ImageLocation? ConvertToAbsoluteLocation(
            ImageLocation projectLocation, string? projectRelativeUri)
        {
            if (projectLocation is null || projectRelativeUri is null)
                return null;
            var projectDir = Path.GetDirectoryName(projectLocation.FilesystemPath)!;
            return new ImageLocation(projectDir).Combine(projectRelativeUri);
        }

        /// <summary>
        /// Takes an absolute path and returns it as a project-relative path.
        /// </summary>
        /// <param name="projectAbsPath"></param>
        /// <param name="absPath"></param>
        /// <returns></returns>
        public string ConvertToProjectRelativePath(string projectAbsPath, string absPath)
        {
            if (string.IsNullOrEmpty(absPath))
                return absPath;
            var fsSvc = Services.RequireService<IFileSystemService>();
            return fsSvc.MakeRelativePath(projectAbsPath, absPath);
        }

        public string? ConvertToProjectRelativeUri(ImageLocation projectUri, ImageLocation? absoluteUri)
        {
            if (absoluteUri is null)
                return null;
            return projectUri.MakeRelativeUri(absoluteUri);
        }
    }
}