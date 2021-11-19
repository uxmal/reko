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

using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace Reko.Core
{
    /// <summary>
    /// Utility class for maniupulating Reko-style URLs
    /// </summary>
    public static class UriTools
    {
        private const string FileScheme = "file:";

        /// <summary>
        /// Returns a file system path from a 'file:' URI, stripping off
        /// any fragments introduced with '#'.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string FilePathFromUri(RekoUri uri)
        {
            var sUri = uri.ExtractString();
            if (!sUri.StartsWith(FileScheme))
                return sUri;
            int iStartFilename = FindFilenameStart(sUri);
            int iHash = sUri.IndexOf('#', iStartFilename);
            string filename = (iHash < 0)
                ? sUri.Substring(iStartFilename)
                : sUri.Substring(iStartFilename, iHash - iStartFilename);
            return WebUtility.UrlDecode(filename);
        }

        private static int FindFilenameStart(string uri)
        {
            // This get hairy with legacy Microsoft UNC conventions, but the canonical
            // format for a _local_ file is file:///abspath, while a remote file is
            // file://server/abspath. For now just deal with local files.
            var iTripleSlash = uri.IndexOf("///");
            if (iTripleSlash > 0)
            {
                // Check for Windows drive letter
                int iColon = iTripleSlash + 3 + 1;
                if (uri.Length > iColon && uri[iColon] == ':')
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
            return 5;
        }

        internal static RekoUri? Combine(RekoUri absoluteUri, string relativeUri)
        {
            if (relativeUri.StartsWith(FileScheme))
                relativeUri = relativeUri.Substring(FileScheme.Length);
            var sb = new StringBuilder(absoluteUri.ExtractString());
            if (sb[^1] != '/')
                sb.Append('/');
            sb.Append(relativeUri);
            return new RekoUri(sb.ToString());
        }

        public static RekoUri UriFromFilePath(string filename)
        {
            if (filename.StartsWith(FileScheme))
            {
                Debug.Print("{0}.{1}: {2} already is a URI.", 
                    nameof(UriTools), nameof(UriFromFilePath), filename);
                return new RekoUri(filename);
            }
            var sb = new StringBuilder(FileScheme);
            string[] segments;
            if (filename.Length > 2 && filename[1] == ':')
            {
                // Very likely a Windows absolute filename.
                sb.Append("///");
                sb.Append(filename[0..2]);
                filename = filename.Substring(2).Replace('\\', '/');
            }
            else if (filename.Length > 0 && filename[0] == '/')
            {
                // Absolute POSIX path
                sb.Append("//");
            }
            else
            { 
                // Relative POSIX or Windows path.
                //$REVIEW We replace '\' with '/' -- this may cause problems
                // with Unix files with backslashes in their name.
                // Suggestions?
                filename = filename.Replace('\\', '/');
            }
            segments = filename.Split('/');
            string sep = "";
            foreach (var seg in segments)
            {
                sb.Append(sep);
                sep = "/";
                sb.Append(WebUtility.UrlEncode(seg));
            }
            return new RekoUri(sb.ToString()); //$DEBUG
        }

        public static string? MakeRelativeUri(RekoUri fromUri, RekoUri toUri)
        {
            var fromPath = fromUri?.ExtractString();
            var toPath = toUri?.ExtractString();
            if (fromPath is null || toPath is null)
                return null;
            Debug.Assert(fromPath.StartsWith(FileScheme));
            Debug.Assert(toPath.StartsWith(FileScheme));

            int iLastDir = -1;
            int i;
            for (i = 0; i < fromPath.Length && i < toPath.Length; ++i)
            {
                if (fromPath[i] != toPath[i])
                    break;
                if (fromPath[i] == '/')
                    iLastDir = i + 1;
            }
            var sb = new StringBuilder(FileScheme);
            if (iLastDir <= 1)
                return toPath;
            for (i = iLastDir; i < fromPath.Length; ++i)
            {
                if (fromPath[i] == '/')
                {
                    sb.Append("../");
                }
            }
            sb.Append(toPath.Substring(iLastDir));
            return sb.ToString();
        }

        public static bool UriHasFragments(RekoUri uri)
        {
            var sUri = uri.ExtractString();
            return sUri.Contains('#');
        }

        /// <summary>
        /// Parses a uri into a set fragments, using '#' as the fragment separator. The 
        /// fragments are UrlDecoded.
        /// </summary>
        /// <param name="uri">file: uri, optionally containing fragments separated by '#'.</param>
        /// <returns>An array of strings, always of at least size 1.
        /// </returns>
        public static string[] ParseUriIntoFragments(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentException(nameof(uri));
            if (!uri.StartsWith(FileScheme))
                return new[] { uri };
            return ParseUriIntoFragments(new RekoUri(uri));
        }

        public static string[] ParseUriIntoFragments(RekoUri urix)
        {
            var uri = urix.ExtractString();
            // The first fragment needs its 'file:' prefix removed.
            var fragments = new List<string>();
            var filename = FilePathFromUri(urix);
            fragments.Add(filename);
            int iStart = uri.IndexOf('#') + 1;  // Skip past the '#'
            while (iStart > 0)
            {
                int iEnd = uri.IndexOf('#', iStart);
                string encodedFragment;
                if (iEnd < 0)
                    encodedFragment = uri.Substring(iStart);
                else
                    encodedFragment = uri.Substring(iStart, iEnd - iStart);
                fragments.Add(WebUtility.UrlDecode(encodedFragment));
                iStart = iEnd + 1;
            }
            return fragments.ToArray();
        }

        public static string ParseLastFragment(RekoUri urix)
        {
            var uri = urix.ExtractString();
            // Strip any 'file': scheme 
            int iStart = 0;
            if (uri.StartsWith(FileScheme))
                iStart = 5;
            int iEnd = uri.IndexOf('#', iStart);
            string fragment;
            if (iEnd >= 0)
            {
                fragment = uri.Substring(iEnd + 1);
            }
            else
            {
                fragment = uri.Substring(iStart);
            }
            return WebUtility.UrlDecode(fragment);
        }

        /// <summary>
        /// Appends a file path to the <paramref name="baseUri"/>, URL encoding
        /// as necessary.
        /// </summary>
        /// <param name="baseUri">Base URI to append the fragment to.</param>
        /// <param name="path">File path to append.</param>
        /// <returns>A new URI.</returns>
        public static RekoUri AppendPathAsFragment(RekoUri baseUri, string path)
        {
            var newUri = String.Concat(baseUri.ExtractString(), "#", WebUtility.UrlDecode(path));
            return new RekoUri(newUri);
        }
    }
}
