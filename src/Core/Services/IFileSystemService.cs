#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.IO;
using System.Text;
using System.Xml;

namespace Reko.Core.Services
{
    /// <summary>
    /// Abstracts away file system accesses.
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>
        /// Opens a stream on a specified file.
        /// </summary>
        /// <param name="filename">File to open.</param>
        /// <param name="mode"><see cref="FileMode"/> to open the file in.</param>
        /// <returns>A <see cref="Stream"/> for the file.
        /// </returns>
        Stream CreateFileStream(string filename, FileMode mode);

        /// <summary>
        /// Opens a stream on a specified file.
        /// </summary>
        /// <param name="filename">File to open.</param>
        /// <param name="mode"><see cref="FileMode"/> to open the file in.</param>
        /// <param name="access">Requested file access.</param>
        /// <returns>A <see cref="Stream"/> for the file.
        /// </returns>
        Stream CreateFileStream(string filename, FileMode mode, FileAccess access);

        /// <summary>
        /// Opens a stream on a specified file.
        /// </summary>
        /// <param name="filename">File to open.</param>
        /// <param name="mode"><see cref="FileMode"/> to open the file in.</param>
        /// <param name="access">Requested file access.</param>
        /// <param name="share">Requested file share mode.</param>
        /// <returns>A <see cref="Stream"/> for the file.
        /// </returns>
        Stream CreateFileStream(string filename, FileMode mode, FileAccess access, FileShare share);

        /// <summary>
        /// Opens a text writer on a specified file.
        /// </summary>
        /// <param name="filename">File to open.</param>
        /// <param name="append">If true appends text to the end of an existing file;
        /// otherwise, overwrites an existing file.</param>
        /// <param name="encoding">Text encoding to use.</param>
        /// <returns>A <see cref="TextWriter"/> for the file.
        /// </returns>

        TextWriter CreateStreamWriter(string filename, bool append, Encoding encoding);

        /// <summary>
        /// Opens a text reader on a specified file.
        /// </summary>
        /// <param name="fileLocation">Path to the file to open.</param>
        /// <param name="encoding">Text encoding to use when decoding the file.</param>
        /// <returns>
        /// A <see cref="TextReader"/> for the file.
        /// </returns>
        TextReader CreateStreamReader(string fileLocation, Encoding encoding);

        /// <summary>
        /// Creates an XML writer for a specified file.
        /// </summary>
        /// <param name="filename">Path to the file.</param>
        /// <returns>An <see cref="XmlWriter"/> instance to write to.</returns>
        XmlWriter CreateXmlWriter(string filename);

        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="dirPath">Path to the directory.</param>
        void CreateDirectory(string dirPath);

        /// <summary>
        /// Gets the current directory.
        /// </summary>
        /// <returns></returns>
        string GetCurrentDirectory();

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="filename">Path to the file to be deleted.
        /// </param>
        void DeleteFile(string filename);

        /// <summary>
        /// Copies a file.
        /// </summary>
        /// <param name="sourceFileName">Path to the source file.</param>
        /// <param name="destFileName">Path to the destination file.</param>
        /// <param name="overwrite">If true, overwrites any file at the destination; otherwise
        /// throws an exception.</param>
        void CopyFile(
            string sourceFileName, string destFileName, bool overwrite);

        /// <summary>
        /// Returns true if the specified file exists.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>True if the file exists; otherwise false.</returns>
        bool FileExists(string filePath);

        /// <summary>
        /// Gets the files in a directory matching the given pattern.
        /// </summary>
        /// <param name="dir">Path to the directory.</param>
        /// <param name="pattern">The pattern to match.</param>
        /// <returns>A list of file names.</returns>
        string[] GetFiles(string dir, string pattern);

        /// <summary>
        /// Returns true if the specified path is rooted.
        /// </summary>
        /// <param name="path">Path to test.</param>
        /// <returns>True if the path is rooted; otherwise false.</returns>
        bool IsPathRooted(string path);

        /// <summary>
        /// Given two paths, returns a relative path from the first to the second.
        /// </summary>
        /// <param name="fromPath">Reference path.</param>
        /// <param name="toPath">Path to make relative to the reference path.</param>
        /// <returns>A path relative to <paramref name="fromPath"/> to <paramref name="toPath"/>.
        /// </returns>
        string MakeRelativePath(string fromPath, string toPath);

        /// <summary>
        /// Read all bytes from a file.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        /// <returns>The complete contents of a file.
        /// </returns>
        byte[] ReadAllBytes(string filePath);

        /// <summary>
        /// Writes all bytes to a file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="bytes">The complete, new contents of the file to bet written.
        /// </param>

        void WriteAllBytes(string path, byte[] bytes);

        /// <summary>
        /// Writes all text to a file.
        /// </summary>
        /// <param name="filename">Path to the file.
        /// </param>
        /// <param name="text">The complete, new contents of the file to be written.
        /// </param>
        void WriteAllText(string filename, string text);

        /// <summary>
        /// Appends text to a file.
        /// </summary>
        /// <param name="filename">Path to the fole.</param>
        /// <param name="text">Text to append to the file.
        /// </param>
        void AppendAllText(string filename, string text);
    }

    /// <summary>
    /// Actual implementation of <see cref="IFileSystemService"/>.
    /// </summary>
    public class FileSystemService : IFileSystemService
    {
        private readonly char sepChar;

        /// <summary>
        /// Creates an instance of the <see cref="FileSystemService"/> class.
        /// </summary>
        public FileSystemService()
        {
            this.sepChar = Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Creates an instance of the <see cref="FileSystemService"/> class.
        /// </summary>
        /// <param name="sepChar">Path separator character to use.</param>
        public FileSystemService(char sepChar)
        {
            this.sepChar = sepChar;
        }

        /// <inheritdoc/>
        public void AppendAllText(string path, string contents)
        {
            File.AppendAllText(path, contents);
        }

        /// <inheritdoc/>
        public Stream CreateFileStream(string filename, FileMode mode)
        {
            return new FileStream(filename, mode);
        }

        /// <inheritdoc/>
        public Stream CreateFileStream(string filename, FileMode mode, FileAccess access)
        {
            return new FileStream(filename, mode, access);
        }

        /// <inheritdoc/>
        public Stream CreateFileStream(string filename, FileMode mode, FileAccess access, FileShare share)
        {
            return new FileStream(filename, mode, access, share);
        }

        /// <inheritdoc/>
        public TextWriter CreateStreamWriter(string filename, bool append, Encoding enc)
        {
            return new StreamWriter(filename, append, enc);
        }

        /// <inheritdoc/>
        public TextReader CreateStreamReader(string filename, Encoding enc)
        {
            return new StreamReader(filename, enc);
        }

        /// <inheritdoc/>
        public XmlWriter CreateXmlWriter(string filename)
        {
            return new XmlTextWriter(filename, new UTF8Encoding(false))
            {
                Formatting = Formatting.Indented
            };
        }

        /// <inheritdoc/>
        public void CreateDirectory(string dirPath)
        {
            Directory.CreateDirectory(dirPath);
        }

        /// <inheritdoc/>
        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        /// <inheritdoc/>
        public void CopyFile(
            string sourceFileName, string destFileName, bool overwrite)
        {
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        /// <inheritdoc/>
        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        /// <inheritdoc/>
        public string[] GetFiles(string directory, string pattern)
        {
            return Directory.GetFiles(directory, pattern);
        }

        /// <inheritdoc/>
        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <inheritdoc/>
        public bool IsPathRooted(string path)
        {
            return Path.IsPathRooted(path);
        }

        /// <inheritdoc/>
        public string MakeRelativePath(string fromPath, string toPath)
        {
            int iLastDir = -1;
            int i;
            for (i = 0; i < fromPath.Length && i < toPath.Length; ++i)
            {
                if (fromPath[i] != toPath[i])
                    break;
                if (fromPath[i] == this.sepChar)
                    iLastDir = i + 1;
            }
            var sb = new StringBuilder();
            if (iLastDir <= 1)
                return toPath;
            for (i = iLastDir; i < fromPath.Length; ++i)
            {
                if (fromPath[i] == this.sepChar)
                {
                    sb.Append("..");
                    sb.Append(sepChar);
                }
            }
            sb.Append(toPath.AsSpan(iLastDir));
            return sb.ToString();
        }

        /// <inheritdoc/>
        public byte[] ReadAllBytes(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        /// <inheritdoc/>
        public void WriteAllBytes(string filePath, byte[] bytes)
        {
            File.WriteAllBytes(filePath, bytes);
        }

        /// <inheritdoc/>
        public void WriteAllText(string filePath, string contents)
        {
            File.WriteAllText(filePath, contents);
        }
    }
}
