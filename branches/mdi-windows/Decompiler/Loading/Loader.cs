/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Configuration;
using Decompiler.Core;
using Decompiler.Core.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Decompiler.Loading
{
	/// <summary>
	/// Loads an image into memory, and deduces the processor architecture 
	/// from the image contents.
	/// </summary>
	public class Loader : LoaderBase
	{
        private IDecompilerConfigurationService config;
        private DecompilerEventListener eventListener;
        private IServiceProvider serviceProvider;

        public Loader(IDecompilerConfigurationService config, IServiceProvider services)
        {
            this.config = config;
            this.serviceProvider = services;
            this.eventListener = (DecompilerEventListener) services.GetService(typeof(DecompilerEventListener));

        }

        public override Program Load(byte[] image, Address addrLoad)
        {
            return LoadExecutableFile(image, addrLoad);
        }

        private ImageLoader FindImageLoader(byte[] rawBytes)
        {
            foreach (LoaderElement e in config.GetImageLoaders())
            {
                if (ImageBeginsWithMagicNumber(rawBytes, e.MagicNumber))
                    return CreateImageLoader(e.TypeName, rawBytes);
            }
            eventListener.AddWarningDiagnostic(new Address(0), "The format of the file is unknown; you will need to specify it manually.");
            return new NullLoader(null, rawBytes);
        }



		/// <summary>
		/// Loads the <paramref>binaryFile</paramref> into memory without any 
		/// relocation or other processing. The beginning of the
		/// image is assumed to be at <paramref>addrBase</paramref.
		/// </summary>
		/// <param name="binaryFile"></param>
		/// <param name="addrBase"></param>
        //$REVIEW: move to platform-specific place.
        //public void LoadComBinary(string binaryFile, Address addrBase)
        //{
        //    byte [] rawBytes = LoadImageBytes(binaryFile, 0x100);
        //    Program.Image = new ProgramImage(addrBase, rawBytes);
        //    Program.Architecture = new IntelArchitecture(ProcessorMode.Real);
        //    Program.Platform = new Arch.Intel.MsDos.MsdosPlatform(Program.Architecture);
        //    EntryPoints.Add(new EntryPoint(addrBase + 0x0100, Program.Architecture.CreateProcessorState()));
        //}

		/// <summary>
		/// Loads the image into memory, unpacking it if necessary. Then, relocate the image.
		/// Relocation gives us a chance to determine the addresses of interesting items.
		/// </summary>
        /// <param name="rawBytes">Image of the executeable file.</param>
        /// <param name="addrLoad">Address into which to load the file.</param>
		public Program LoadExecutableFile(byte [] rawBytes, Address addrLoad)
		{
            ImageLoader loader = FindImageLoader(rawBytes);
            if (addrLoad == null)
			{
				addrLoad = loader.PreferredBaseAddress;     //$REVIEW: Should be a configuration property.
			}

            Program prog = new Program();
            prog.Image = loader.Load(addrLoad);
            prog.Architecture = loader.Architecture;
            prog.Platform = loader.Platform;

		    RelocationDictionary relocations = new RelocationDictionary();
			loader.Relocate(addrLoad, EntryPoints, relocations);
            CopyImportThunks(loader.ImportThunks, prog);
            return prog;
        }



        public bool ImageBeginsWithMagicNumber(byte [] image, string magicNumber)
        {
            byte[] magic = ConvertHexStringToBytes(magicNumber);
            if (image.Length < magic.Length)
                return false;
            for (int i = 0; i < magic.Length; ++i)
            {
                if (magic[i] != image[i])
                    return false;
            }
            return true;

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

        public virtual ImageLoader CreateImageLoader(string typeName, byte[] bytes)
        {
            Type t = Type.GetType(typeName);
            if (t == null)
                throw new ApplicationException(string.Format("Unable to find loader {0}.", typeName));
            ConstructorInfo ci = t.GetConstructor(new Type[] { typeof (IServiceProvider), typeof(byte[]) });
            return (ImageLoader) ci.Invoke(new object[] { this.serviceProvider, bytes });
        }
	}
}

