#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Configuration;
using Decompiler.Core.Services;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Loading
{
    /// <summary>
    /// Base class that abstracts the process of loading the "code", in whatever format it is in,
    /// into a program.
    /// </summary>
    public abstract class LoaderBase
    {
        private List<EntryPoint> entryPoints;

        public LoaderBase(IServiceProvider services)
        {
            this.Services = services;
            this.entryPoints = new List<EntryPoint>();
            this.ConfigurationService = services.GetService<IDecompilerConfigurationService>();
            this.EventListener = services.GetService<DecompilerEventListener>();
        }

        public List<EntryPoint> EntryPoints
        {
            get { return entryPoints; }
        }

        public IDecompilerConfigurationService ConfigurationService { get; private set; }
        public DecompilerEventListener EventListener { get; private set; }
        public IServiceProvider Services { get; private set; }

        public abstract Program Load(string fileName, byte[] imageFile, Address userSpecifiedAddress);

        /// <summary>
        /// Loads the contents of a file with the specified filename into an array 
        /// of bytes, optionally at the offset <paramref>offset</paramref>.
        /// </summary>
        /// <param name="fileName">File to open.</param>
        /// <param name="offset">The offset into the array into which the file will be loaded.</param>
        /// <returns>An array of bytes with the file contents at the specified offset.</returns>
        public virtual byte[] LoadImageBytes(string fileName, int offset)
        {
            using (FileStream stm = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new Byte[stm.Length + offset];
                stm.Read(bytes, offset, (int)stm.Length);
                return bytes;
            }
        }

        /// <summary>
        /// Locates an image loader from the Decompiler configuration file, based on the magic number in the
        /// begining of the program image.
        /// </summary>
        /// <param name="rawBytes"></param>
        /// <returns>An appropriate image loader if known, a NullLoader if the image format is unknown.</returns>
        public T FindImageLoader<T>(string fileName, byte[] rawBytes, Func<T> defaultLoader)
        {
            foreach (LoaderElement e in ConfigurationService.GetImageLoaders())
            {
                if (!string.IsNullOrEmpty(e.MagicNumber) &&
                    ImageHasMagicNumber(rawBytes, e.MagicNumber, e.Offset)
                    ||
                    (!string.IsNullOrEmpty(e.Extension) &&
                        fileName.EndsWith(e.Extension, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return CreateImageLoader<T>(e.TypeName, rawBytes);
                }
            }

            EventListener.AddDiagnostic(
                new NullCodeLocation(""),
                new ErrorDiagnostic("The format of the file is unknown."));
            return defaultLoader();
        }


        public bool ImageHasMagicNumber(byte[] image, string magicNumber, string sOffset)
        {
            int offset = ConvertOffset(sOffset);
            byte[] magic = ConvertHexStringToBytes(magicNumber);
            if (image.Length < magic.Length + offset)
                return false;

            for (int i = 0, j = offset; i < magic.Length; ++i, ++j)
            {
                if (magic[i] != image[j])
                    return false;
            }
            return true;
        }


                private int ConvertOffset(string sOffset)
        {
            if (string.IsNullOrEmpty(sOffset))
                return 0;
            int offset;
            if (Int32.TryParse(sOffset, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out offset))
                return offset;
            return 0;
        }

        private byte[] ConvertHexStringToBytes(string hexString)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < hexString.Length; i += 2)
            {
                uint hi = HexDigit(hexString[i]);
                uint lo = HexDigit(hexString[i + 1]);
                bytes.Add((byte) ((hi << 4) | lo));
            }
            return bytes.ToArray();
        }

        private uint HexDigit(char digit)
        {
            switch (digit)
            {
            case '0': case '1': case '2': case '3': case '4': 
            case '5': case '6': case '7': case '8': case '9':
                return (uint) (digit - '0');
            case 'A': case 'B': case 'C': case 'D': case 'E': case 'F':
                return (uint) ((digit - 'A') + 10);
            case 'a': case 'b': case 'c': case 'd': case 'e': case 'f':
                return (uint) ((digit - 'a') + 10);
            default:
                throw new ArgumentException(string.Format("Invalid hexadecimal digit '{0}'.", digit));
            }
        }

        public T CreateImageLoader<T>(string typeName, byte[] bytes)
        {
            Type t = Type.GetType(typeName);
            if (t == null)
                throw new ApplicationException(string.Format("Unable to find loader {0}.", typeName));
            return (T) Activator.CreateInstance(t, this.Services, bytes);
        }

        protected void CopyImportThunks(Dictionary<uint, PseudoProcedure> importThunks, Program prog)
        {
            if (importThunks == null)
                return;

            foreach (KeyValuePair<uint, PseudoProcedure> item in importThunks)
            {
                prog.ImportThunks.Add(item.Key, item.Value);
            }
        }
    }
}
 