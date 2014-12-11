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
using Decompiler.Core.Assemblers;
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
    /// Class that abstracts the process of loading the "code", in whatever format it is in,
    /// into a program.
    /// </summary>
    public class Loader : ILoader
    {
        private IDecompilerConfigurationService cfgSvc;
        private DecompilerEventListener eventListener;

        public Loader(IServiceProvider services)
        {
            this.Services = services;
            this.cfgSvc = services.GetService<IDecompilerConfigurationService>();
        }

        public IServiceProvider Services { get; private set; }

        public Program AssembleExecutable(string fileName, Assembler asm, Address addrLoad)
        {
            var bytes = LoadImageBytes(fileName, 0);
            return AssembleExecutable(fileName, bytes, asm, addrLoad);
        }

        public Program AssembleExecutable(string fileName, byte[] image, Assembler asm, Address addrLoad)
        {
            var lr = asm.Assemble(addrLoad, new StreamReader(new MemoryStream(image), Encoding.UTF8));
            Program program = new Program(
                lr.Image,
                lr.Image.CreateImageMap(),
                lr.Architecture,
                lr.Platform);
            program.Name = Path.GetFileName(fileName);
            program.EntryPoints.AddRange(asm.EntryPoints);
            program.EntryPoints.Add(new EntryPoint(asm.StartAddress, program.Architecture.CreateProcessorState()));
            CopyImportReferences(asm.ImportReferences, program);
            return program;
        }

        /// <summary>
        /// Loads the image into memory, unpacking it if necessary. Then, relocate the image.
        /// Relocation gives us a chance to determine the addresses of interesting items.
        /// </summary>
        /// <param name="rawBytes">Image of the executeable file.</param>
        /// <param name="addrLoad">Address into which to load the file.</param>
        public Program LoadExecutable(string fileName, byte[] image, Address addrLoad)
        {
            ImageLoader imgLoader = FindImageLoader<ImageLoader>(fileName, image, () => new NullImageLoader(Services, image));
            if (addrLoad == null)
            {
                addrLoad = imgLoader.PreferredBaseAddress;     //$REVIEW: Should be a configuration property.
            }

            var result = imgLoader.Load(addrLoad);
            Program program = new Program(
                result.Image,
                result.ImageMap,
                result.Architecture,
                result.Platform);
            program.Name = Path.GetFileName(fileName);
            var relocations = imgLoader.Relocate(addrLoad);
            program.EntryPoints.AddRange(relocations.EntryPoints);
            CopyImportReferences(imgLoader.ImportReferences, program);
            return program;
        }

        /// <summary>
        /// Loads a metadata file into a type library.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public TypeLibrary LoadMetadata(string fileName)
        {
            var rawBytes = LoadImageBytes(fileName, 0);
            MetadataLoader mdLoader = FindImageLoader<MetadataLoader>(fileName, rawBytes, () => new NullMetadataLoader());
            var result = mdLoader.Load();
            return result;
        }

        /// <summary>
        /// Loads the contents of a file with the specified filename into an array 
        /// of bytes, optionally at the offset <paramref>offset</paramref>. No interpretation
        /// of those bytes is done.
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
            foreach (LoaderElement e in cfgSvc.GetImageLoaders())
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

            this.Services.RequireService<DecompilerEventListener>().AddDiagnostic(
                new NullCodeLocation(""),
                new ErrorDiagnostic("The format of the file is unknown."));
            return defaultLoader();
        }

        public bool ImageHasMagicNumber(byte[] image, string magicNumber, string sOffset)
        {
            int offset = ConvertOffset(sOffset);
            byte[] magic = ConvertHexStringToBytes(magicNumber);
            if (image.Length < offset + magic.Length)
                return false;

            for (int i = 0, j = offset; i < magic.Length; ++i, ++j)
            {
                if (magic[i] != image[j])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the offset, which is expressed as a string, into a hexadecimal value.
        /// </summary>
        /// <param name="sOffset"></param>
        /// <returns></returns>
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

        protected void CopyImportReferences(Dictionary<Address, ImportReference> importReference, Program prog)
        {
            if (importReference == null)
                return;

            foreach (var item in importReference)
            {
                prog.ImportReferences.Add(item.Key, item.Value);
            }
        }
    }
}
 