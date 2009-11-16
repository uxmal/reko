/* 
 * Copyright (C) 1999-2009 John Källén.
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
        private string filename;
        private DecompilerConfiguration config;
        private DecompilerEventListener eventListener;
        private DecompilerProject project;
        private Program prog;

        public Loader(string filename, DecompilerConfiguration config, DecompilerEventListener eventListener)
        {
            this.filename = filename;
            this.config = config;
            this.eventListener = eventListener;
        }

        /// <summary>
        /// Loads the file and returns a decompiler project.
        /// </summary>
        /// <remarks>
        /// The file can either be an executable or a decompiler project file.
        /// </remarks>
        public override void Load(Address addrLoad)
        {
            byte[] image = LoadImageBytes(filename, 0);
            bool isXmlFile = IsXmlFile(image);
            if (isXmlFile)
            {
                XmlSerializer ser = new XmlSerializer(typeof(DecompilerProject));
                this.project = (DecompilerProject) ser.Deserialize(new MemoryStream(image));
                addrLoad = project.Input.BaseAddress;
                prog = LoadExecutableFile(
                    LoadImageBytes(project.Input.Filename, 0),
                    project.Input.BaseAddress);
            }
            else
            {
                // Wasn't a project, so make a blank one.

                this.project = new DecompilerProject();

                SetDefaultFilenames(filename, project);
                this.prog = LoadExecutableFile(image, addrLoad);
                project.Input.BaseAddress = prog.Image.BaseAddress;
            }
        }

        public override Program Program
        {
            get { return prog; }
        }

        public override DecompilerProject Project
        {
             get { return project; }
        }

        private ImageLoader FindImageLoader(byte[] rawBytes)
        {
            foreach (LoaderElement e in config.GetImageLoaders())
            {
                if (ImageBeginsWithMagicNumber(rawBytes, e.MagicNumber))
                    return CreateImageLoader(e.TypeName, rawBytes);
            }
            eventListener.WriteDiagnostic(DiagnosticOld.Warning, new Address(0), "The format of the file {0} is unknown; you will need to specify it manually.", filename);
            return new NullLoader(rawBytes);
        }



		private static bool IsXmlFile(byte[] image)
		{
			bool isXmlFile = ProgramImage.CompareArrays(image, 0, new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C }, 5);	// <?xml
			return isXmlFile;
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

            prog = new Program();
            prog.Image = loader.Load(addrLoad);
            prog.Architecture = loader.Architecture;
            prog.Platform = loader.Platform;

		    RelocationDictionary relocations = new RelocationDictionary();
			loader.Relocate(addrLoad, EntryPoints, relocations);

            return prog;
        }


		/// <summary>
		/// Loads the contents of a file with the specified filename into an array 
		/// of bytes, optionally at the offset <paramref>offset</paramref>.
		/// </summary>
		/// <param name="fileName">File to open.</param>
		/// <param name="offset">The offset into the array into which the file will be loaded.</param>
		/// <returns>An array of bytes with the file contents at the specified offset.</returns>
		public virtual byte [] LoadImageBytes(string fileName, int offset)
		{
			using (FileStream stm = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				byte [] bytes = new Byte[stm.Length + offset];
				stm.Read(bytes, offset, (int) stm.Length);
				return bytes;
			}
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
            ConstructorInfo ci = t.GetConstructor(new Type[] { typeof(byte[]) });
            return (ImageLoader) ci.Invoke(new object[] { bytes });
        }
	}
}

