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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Path = System.IO.Path;

namespace Reko.Core
{
    /// <summary>
    /// This class represents the location of a image to be decompiled. 
    /// </summary>
    /// <remarks>
    /// Reko implements a custom URI scheme "archive:" which supports
    /// expressing files as a file system path followed by zero or more
    /// fragments separated by the '#' character. Each fragment corresponds
    /// to a path inside a file container (like an archive). Multiple fragments
    /// make it possible to refer to images stored in nested archives, if 
    /// needed.
    /// </remarks>
    public class ImageLocation : IComparable<ImageLocation>
    {
        private const string ArchiveScheme = "archive:";
        private const string FileScheme = "file:";

        /// <summary>
        /// Constructs an <see cref="ImageLocation"/> from a file system
        /// path and 0 or more archive-specific path fragments.
        /// </summary>
        /// <param name="filesystemPath">The path of a file.</param>
        /// <param name="fragments">An array of 0 or more archive fragments.</param>
        internal ImageLocation(string filesystemPath, params string[] fragments)
        {
            this.FilesystemPath = filesystemPath;
            this.Fragments = fragments;
        }

        /// <summary>
        /// Constructs an <see cref="ImageLocation"/> from a file system path.
        /// </summary>
        /// <param name="filesystemPath">A path into the file system.</param>
        /// <returns>An <see cref="ImageLocation"/>.
        /// </returns>
        /// <remarks>
        /// If the <paramref name="uri" /> is a simple file system string, an 
        /// <see cref="ImageLocation"/> is created from that string, with no 
        /// fragments. Otherwise the URI string is parsed for either "file:" or
        /// "archive:" prefixes.
        /// </remarks>
        public static ImageLocation FromUri(string uri)
        {
            if (uri is null)
                throw new ArgumentNullException(nameof(uri));
            if (uri.StartsWith(FileScheme))
            {
                var path = uri.Substring(FindFilenameStart(uri));
                return new ImageLocation(path);
            }
            else if (uri.StartsWith(ArchiveScheme))
            {
                int i = uri.IndexOf('#', ArchiveScheme.Length); // Skip the 'archive:' prefix.
                if (i < 0)
                    return new ImageLocation(WebUtility.UrlDecode(uri.Substring(ArchiveScheme.Length)));
                var fsPath = WebUtility.UrlDecode(uri.Substring(8, i - ArchiveScheme.Length));
                var fragments = uri.Substring(i + 1)
                    .Split('#')
                    .Select(f => WebUtility.UrlDecode(f))
                    .ToArray();
                return new ImageLocation(fsPath, fragments);
            }
            return new ImageLocation(uri);
        }

        public string FilesystemPath { get; }

        public string[] Fragments { get; }

        public bool HasFragments => this.Fragments.Length > 0;

        public ImageLocation AppendFragment(string name)
        {
            var newFragments = new string[this.Fragments.Length + 1];
            Array.Copy(this.Fragments, newFragments, this.Fragments.Length);
            newFragments[^1] = name;
            return new ImageLocation(this.FilesystemPath, newFragments);
        }

        public ImageLocation Combine(string relativeUri)
        {
            if (this.HasFragments)
                throw new InvalidOperationException("Cannot combine an ImageLocation with fragments.");
            var relative = FromUri(relativeUri);
            var fsPath = Path.Combine(this.FilesystemPath, relative.FilesystemPath);
            return new ImageLocation(fsPath, relative.Fragments);
        }

        public int CompareTo(ImageLocation? that) => throw new NotImplementedException();

        /// <summary>
        /// Determines whether this <see cref="ImageLocation"/> ends with the given string.
        /// </summary>
        /// <param name="s">The string to compare to the substring at the end of this instance.</param>
        /// <returns>True if there was a match, false if not.</returns>
        public bool EndsWith(string s)
        {
            var str = this.HasFragments
                ? Fragments[^1]
                : FilesystemPath;
            return str.EndsWith(s);
        }

        /// <summary>
        /// Determines whether this <see cref="ImageLocation"/> ends with the given string.
        /// </summary>
        /// <param name="s">The string to compare to the substring at the end of this instance.</param>
        /// <param name="c">String comparison to use.</param>
        /// <returns>True if there was a match, false if not.</returns>
        public bool EndsWith(string s, StringComparison c)
        {
            var str = this.HasFragments
                ? Fragments[^1]
                : FilesystemPath;
            return str.EndsWith(s, c);
        }

        /// <summary>
        /// Compares this <see cref="ImageLocation"/> with another object for equality.
        /// </summary>
        /// <param name="obj">Object with which to test equality.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is not ImageLocation that)
                return false;
            if (this.FilesystemPath != that.FilesystemPath)
                return false;
            if (this.Fragments.Length != that.Fragments.Length)
                return false;
            for (int i = 0; i < this.Fragments.Length; ++i)
            {
                if (this.Fragments[i] != that.Fragments[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Computes a hash code for this <see cref="ImageLocation"/>.
        /// </summary>
        /// <returns>A signed integer hash code.</returns>
        public override int GetHashCode()
        {
            int h = Fragments.Length;
            if (Fragments.Length > 0)
                h = (h * 17) ^ Fragments[^1].GetHashCode();

            h = (h * 5) ^ FilesystemPath.GetHashCode();
            return h;
        }

        private static int FindFilenameStart(string fileUri)
        {
            // This get hairy with legacy Microsoft UNC conventions, but the canonical
            // format for a _local_ file is file:///abspath, while a remote file is
            // file://server/abspath. For now just deal with local files.
            var iTripleSlash = fileUri.IndexOf("///");
            if (iTripleSlash > 0)
            {
                // Check for Windows drive letter
                int iColon = iTripleSlash + 3 + 1;
                if (fileUri.Length > iColon && fileUri[iColon] == ':')
                {
                    // We have file:///x:
                    return iTripleSlash + 3;
                }
                else
                {
                    // No windows drive letter, assume Unix absolute path.
                    return iTripleSlash + 2;
                }
            }
            // Skip the 'file:' scheme.
            return FileScheme.Length;
        }

        /// <summary>
        /// Returns just the file extension (including any '.') of this <see cref="ImageLocation"/>.
        /// </summary>
        /// <returns></returns>
        public string GetExtension()
        {
            var str = this.HasFragments
                ? Fragments[^1]
                : FilesystemPath;
            return Path.GetExtension(str);
        }

        /// <summary>
        /// Returns just a file name and extension for this <see cref="ImageLocation"/>.
        /// </summary>
        /// <returns></returns>
        public string GetFilename()
        {
            var str = this.HasFragments
                ? Fragments[^1]
                : FilesystemPath;
            return Path.GetFileName(str);
        }



        public string MakeRelativeUri(ImageLocation imageLocation)
        {
            if (this.HasFragments)
                throw new NotSupportedException("Making relative URI's to non-directories is not supported yet.");
            var dir = Path.GetDirectoryName(this.FilesystemPath)!;
            var fsAbsolute = Path.GetRelativePath(dir, imageLocation.FilesystemPath);
            return MakeUri(fsAbsolute, imageLocation.Fragments);
        }

        public override string ToString()
        {
            return MakeUri(this.FilesystemPath, Fragments);
        }

        private static string MakeUri(string fileSystemPath, string[] fragments)
        {
            if (fragments.Length == 0)
                return fileSystemPath;
            var sb = new StringBuilder();
            sb.Append("archive:");
            sb.Append(WebUtility.UrlEncode(fileSystemPath));
            for (int i = 0; i < fragments.Length; ++i)
            {
                sb.Append('#');
                sb.Append(WebUtility.UrlEncode(fragments[i]));
            }
            return sb.ToString();
        }
    }
}
