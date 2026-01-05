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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Reko.Loading
{
    /// <summary>
    /// Loads unpacker signatures from the (large) XML files that
    /// store them.
    /// </summary>
    public class UnpackerSignatureLoader : SignatureLoader
    {
        /// <inheritdoc/>
        public override IEnumerable<ImageSignature> Load(string filename)
        {
            using TextReader txtRdr = CreateFileReader(filename);
            var serializer = new XmlSerializer(typeof(UnpackerSignatureFile_v1));
            var sigFile = (UnpackerSignatureFile_v1) serializer.Deserialize(txtRdr)!;
            var sigs = sigFile?.Signatures;
            if (sigs is null)
                return [];
            return sigs.Select(s => CreateSignature(s));
        }

        private static ImageSignature CreateSignature(UnpackerSignature_v1 sig)
        {
            return new ImageSignature
            {
                Name = sig.Name,
                Comments = sig.Comments,
                EntryPointPattern = sig.EntryPoint,
                ImagePattern = sig.EntirePE,
            };
        }

        /// <summary>
        /// Creates a text reader for the specified file.
        /// </summary>
        /// <param name="filename">File name.</param>
        /// <returns>A <see cref="TextReader"/> positioned at the beginning of 
        /// the file.
        /// </returns>
        public virtual TextReader CreateFileReader(string filename)
        {
            return new StreamReader(filename, Encoding.UTF8);
        }
    }
}
