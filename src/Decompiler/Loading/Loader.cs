#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core;
using Reko.Core.Assemblers;
using Reko.Core.Configuration;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Reko.Loading
{
    /// <summary>
    /// Class that provides services for loading the "code", in whatever format it is in,
    /// into a program.
    /// </summary>
    public class Loader : ILoader
    {
        private IConfigurationService cfgSvc;
        private UnpackingService unpackerSvc;

        public Loader(IServiceProvider services)
        {
            this.Services = services;
            this.cfgSvc = services.RequireService<IConfigurationService>();
            this.unpackerSvc = new UnpackingService(services);
            this.unpackerSvc.LoadSignatureFiles();
            Services.RequireService<IServiceContainer>().AddService(typeof(IUnpackerService), unpackerSvc);
        }

        public string DefaultToFormat { get; set; }
        public IServiceProvider Services { get; private set; }

        public Program AssembleExecutable(string filename, string assemblerName, Address addrLoad)
        {
            var bytes = LoadImageBytes(filename, 0);
            var asm = cfgSvc.GetAssembler(assemblerName);
            if (asm == null)
                throw new ApplicationException(string.Format("Unknown assembler name '{0}'.", assemblerName));
            return AssembleExecutable(filename, bytes, asm, addrLoad);
        }

        public Program AssembleExecutable(string fileName, Assembler asm, Address addrLoad)
        {
            var bytes = LoadImageBytes(fileName, 0);
            return AssembleExecutable(fileName, bytes, asm, addrLoad);
        }

        public Program AssembleExecutable(string fileName, byte[] image, Assembler asm, Address addrLoad)
        {
            var program = asm.Assemble(addrLoad, new StreamReader(new MemoryStream(image), Encoding.UTF8));
            program.Name = Path.GetFileName(fileName);
            foreach (var sym in asm.ImageSymbols)
            {
                program.ImageSymbols[sym.Address] = sym;
            }
            foreach (var ep in asm.EntryPoints)
            {
                program.EntryPoints[ep.Address] = ep;
            }
            program.EntryPoints[asm.StartAddress] =
                new ImageSymbol(asm.StartAddress);
            CopyImportReferences(asm.ImportReferences, program);
            return program;
        }

        /// <summary>
        /// Loads the image into memory, unpacking it if necessary. Then, relocate the image.
        /// Relocation gives us a chance to determine the addresses of interesting items.
        /// </summary>
        /// <param name="rawBytes">Image of the executeable file.</param>
        /// <param name="loader">.NET Class name of a custom loader (may be null)</param>
        /// <param name="addrLoad">Address into which to load the file.</param>
        public Program LoadExecutable(string filename, byte[] image, string loader, Address addrLoad)
        {
            ImageLoader imgLoader;
            if (!string.IsNullOrEmpty(loader))
            {
                imgLoader = CreateCustomImageLoader(Services, loader, filename, image);
            }
            else
            {
                imgLoader = FindImageLoader(
                    filename,
                    image,
                    () => CreateDefaultImageLoader(filename, image));
            }
            if (addrLoad == null)
            {
                addrLoad = imgLoader.PreferredBaseAddress;     //$REVIEW: Should be a configuration property.
            }

            var program = imgLoader.Load(addrLoad);

            // Sanity check of the 'Needs' properties.
            if (program.NeedsScanning && !program.NeedsSsaTransform)
                throw new InvalidOperationException(
                    "A programming error has been detected. " +
                    "Image loader {0} has set the program.NeedsScanning " +
                    "and program.NeedsSsaTransform to inconsistent values");

            program.Name = Path.GetFileName(filename);
            if (program.NeedsScanning)
            {
            var relocations = imgLoader.Relocate(program, addrLoad);
            program.AddSymbols(relocations.Symbols.Values);
            foreach (var ep in relocations.EntryPoints)
            {
                program.EntryPoints[ep.Address] = ep;
            }
            program.ImageMap = program.SegmentMap.CreateImageMap();
            }
            return program;
        }

        /// <summary>
        /// Loads a Program from a flat image where all the metadata has been 
        /// supplied by the user in <paramref name="details"/>.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="image"></param>
        /// <param name="addrLoad"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public Program LoadRawImage(string filename, byte[] image, Address addrLoad, LoadDetails details)
        { 
            var arch = cfgSvc.GetArchitecture(details.ArchitectureName);
            var platform = cfgSvc.GetEnvironment(details.PlatformName).Load(Services, arch);
            if (addrLoad == null)
            {
                if (!arch.TryParseAddress(details.LoadAddress, out addrLoad))
                {
                    throw new ApplicationException(
                        "Unable to determine base address for executable. A default address should have been present in the reko.config file.");
                }
            }
            Program program;
            if (!string.IsNullOrEmpty(details.LoaderName))
            {
                var imgLoader = CreateCustomImageLoader(Services, details.LoaderName, filename, image);
                program = imgLoader.Load(addrLoad, arch, platform);
            }
            else
            {
                var segmentMap = CreatePlatformSegmentMap(platform, addrLoad, image);
                program = new Program(
                    segmentMap,
                    arch,
                    platform);
                Address addrEp;
                if (details.EntryPoint != null && arch.TryParseAddress(details.EntryPoint.Address, out addrEp))
                {
                    program.EntryPoints.Add(addrEp, new Core.ImageSymbol(addrEp) { Type = SymbolType.Procedure });
                }
            }
            program.Name = Path.GetFileName(filename);
            program.User.Processor = arch.Name;
            program.User.Environment = platform.Name;
            program.User.Loader = details.LoaderName;
            program.User.LoadAddress = addrLoad;
            program.ImageMap = program.SegmentMap.CreateImageMap();
            return program;
        }

        public ImageLoader CreateDefaultImageLoader(string filename, byte[] image)
        {
            var rawFile = cfgSvc.GetRawFile(DefaultToFormat);
            if (rawFile == null)
            {
                this.Services.RequireService<DecompilerEventListener>().Warn(
                    new NullCodeLocation(""),
                    "The format of the file is unknown.");
                return new NullImageLoader(Services, filename, image);
            }
            return CreateRawImageLoader(filename, image, rawFile);
        }

        private ImageLoader CreateRawImageLoader(string filename, byte[] image, RawFileElement rawFile)
        {
            var arch = cfgSvc.GetArchitecture(rawFile.Architecture);
            var env = cfgSvc.GetEnvironment(rawFile.Environment);
            IPlatform platform;
            if (env != null)
            {
                platform = env.Load(Services, arch);
            }
            else
            {
                platform = new DefaultPlatform(Services, arch);
            }

            Address entryAddr = null;
            Address baseAddr;
            if (arch.TryParseAddress(rawFile.BaseAddress, out baseAddr))
            {
                entryAddr = GetRawBinaryEntryAddress(rawFile, image, arch, baseAddr);
            }
            var imgLoader = new NullImageLoader(Services, filename, image)
            {
                Architecture = arch,
                Platform = platform,
                PreferredBaseAddress = entryAddr,
            };
            Address addrEp;
            if (rawFile.EntryPoint != null)
            {
                if (!string.IsNullOrEmpty(rawFile.EntryPoint.Address))
                {
                    arch.TryParseAddress(rawFile.EntryPoint.Address, out addrEp);
                }
                else
                {
                    addrEp = baseAddr;
                }
                imgLoader.EntryPoints.Add(new ImageSymbol(addrEp)
                {
                    Type = SymbolType.Procedure
                });
            }
            return imgLoader;
        }

        public static Address GetRawBinaryEntryAddress(
            RawFileElement rawFile,
            byte[] image, 
            IProcessorArchitecture arch, 
            Address baseAddr)
        {
            if (!string.IsNullOrEmpty(rawFile.EntryPoint.Address))
            {
                Address entryAddr;
                if (arch.TryParseAddress(rawFile.EntryPoint.Address, out entryAddr))
                {
                    if (rawFile.EntryPoint.Follow)
                    {
                        var rdr = arch.CreateImageReader(new MemoryArea(baseAddr, image), entryAddr);
                        return arch.ReadCodeAddress(0, rdr, arch.CreateProcessorState());
                    }
                }
                else
                {
                    return baseAddr;
                }
            }
            return baseAddr;
        }

        /// <summary>
        /// Loads a metadata file into a type library.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public TypeLibrary LoadMetadata(string fileName, IPlatform platform, TypeLibrary typeLib)
        {
            var rawBytes = LoadImageBytes(fileName, 0);
            var mdLoader = FindImageLoader<MetadataLoader>(fileName, rawBytes, () => new NullMetadataLoader());
            var result = mdLoader.Load(platform, typeLib);
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
            var fsSvc = Services.RequireService<IFileSystemService>();
            using (var stm = fsSvc.CreateFileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[stm.Length + offset];
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
        public T FindImageLoader<T>(string filename, byte[] rawBytes, Func<T> defaultLoader)
        {
            foreach (LoaderConfiguration e in cfgSvc.GetImageLoaders())
            {
                if (!string.IsNullOrEmpty(e.MagicNumber) &&
                    ImageHasMagicNumber(rawBytes, e.MagicNumber, e.Offset)
                    ||
                    (!string.IsNullOrEmpty(e.Extension) &&
                        filename.EndsWith(e.Extension, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return CreateImageLoader<T>(Services, e.TypeName, filename, rawBytes);
                }
            }
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

        public static uint HexDigit(char digit)
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

        public static T CreateImageLoader<T>(IServiceProvider services, string typeName, string filename, byte[] bytes)
        {
            Type t = Type.GetType(typeName);
            if (t == null)
                throw new ApplicationException(string.Format("Unable to find loader {0}.", typeName));
            return (T) Activator.CreateInstance(t, services, filename, bytes);
        }

        /// <summary>
        /// Expects the provided assembly to contain exactly one type that implements
        /// ImageLoader, then loads that.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="typeName"></param>
        /// <param name="filename"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static ImageLoader CreateCustomImageLoader(IServiceProvider services, string loader, string filename, byte[] bytes)
        {
            if (string.IsNullOrEmpty(loader))
            {
                return new NullImageLoader(services, filename, bytes);
            }

            var cfgSvc = services.RequireService<IConfigurationService>();
            var ldrCfg = cfgSvc.GetImageLoaders().FirstOrDefault(e => e.Label == loader);
            if (ldrCfg != null)
            {
                return CreateImageLoader<ImageLoader>(services, ldrCfg.TypeName, filename, bytes);
            }

            //$TODO: detect file extensions and load appropriate interpreter.
            var ass = Assembly.LoadFrom(loader);
            var t = ass.GetTypes().SingleOrDefault(tt => typeof(ImageLoader).IsAssignableFrom(tt));
            if (t == null)
                throw new ApplicationException(string.Format("Unable to find image loader in {0}.", loader));
            return (ImageLoader) Activator.CreateInstance(t, services, filename, bytes);
        }

        protected void CopyImportReferences(Dictionary<Address, ImportReference> importReference, Program program)
        {
            if (importReference == null)
                return;

            foreach (var item in importReference)
            {
                program.ImportReferences.Add(item.Key, item.Value);
            }
        }

        protected void CopyInterceptedCalls(Dictionary<Address, ExternalProcedure> interceptedCalls, Program program)
        {
            foreach (var item in interceptedCalls)
            {
                program.InterceptedCalls.Add(item.Key, item.Value);
            }
        }

        public SegmentMap CreatePlatformSegmentMap(IPlatform platform, Address loadAddr, byte [] rawBytes)
        {
            var segmentMap = platform.CreateAbsoluteMemoryMap();
            if (segmentMap == null)
            {
                segmentMap = new SegmentMap(loadAddr);
            }
            var mem = new MemoryArea(loadAddr, rawBytes);
            segmentMap.AddSegment(mem, "code", AccessMode.ReadWriteExecute);
            return segmentMap;
        }
    }
}
 