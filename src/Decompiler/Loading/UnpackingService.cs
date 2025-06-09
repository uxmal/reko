#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Configuration;
using Reko.Core.IO;
using Reko.Core.Lib;
using Reko.Core.Loading;
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

/// https://sourceforge.net/p/odbgscript/code/HEAD/tree/OllyLangCommands.cpp Olly
/// https://plusvic.github.io/yara/
/// 
namespace Reko.Loading
{
    /// <summary>
    /// UnpackingService is used by loaders that want to look up a particular
    /// packer based on its signature.
    /// </summary>
    public class UnpackingService : IUnpackerService
    {
        public UnpackingService(IServiceProvider services)
        {
            this.Signatures = new List<ImageSignature>();
            this.Services = services;
        }

        public IServiceProvider Services { get; }
        public List<ImageSignature> Signatures { get; }

        /// <summary>
        /// Attempt to load as many signature files as possible.
        /// </summary>
        public void LoadSignatureFiles()
        {
            var cfgSvc = Services.GetService<IConfigurationService>();
            if (cfgSvc is null)
                return;
            foreach (SignatureFileDefinition sfe in cfgSvc.GetSignatureFiles())
            {
                if (sfe.Filename is null || sfe.TypeName is null)
                    continue;
                try
                {
                    var ldr = CreateSignatureLoader(sfe.TypeName);
                    if (ldr is null)
                        continue;
                    Signatures.AddRange(ldr.Load(cfgSvc.GetInstallationRelativePath(sfe.Filename)));
                }
                catch (Exception ex)
                {
                    Services.RequireService<IDecompilerEventListener>().Error(
                        new NullCodeLocation(sfe.Filename),
                        ex,
                        "Unable to load signatures from {0} with loader {1}.", sfe.Filename, sfe.TypeName);
                }
            }
        }

        // This method is virtual so that it can be overload in unit tests.
        public virtual SignatureLoader CreateSignatureLoader(string typeName)
        {
            var svc = Services.RequireService<IPluginLoaderService>();
            Type t = svc.GetType(typeName);

            var ldr = (SignatureLoader)Activator.CreateInstance(t)!;
            return ldr;
        }

        /// <summary>
        /// Using the image of the program under investigation, find an 
        /// unpacker capable of unpacking it, and returns a new <see cref="ImageLoader"/>
        /// instance.
        /// </summary>
        /// <param name="loader">Raw image loader.</param>
        /// <param name="entryPointOffset">Offset of the program entry point.</param>
        /// <returns>If an unpacker was found, returns a new wrapping ImageLoader. Otherwise 
        /// the original loader is returned.</returns>
        public ProgramImageLoader FindUnpackerBySignature(ProgramImageLoader loader, uint entryPointOffset)
        {
            var listener = Services.RequireService<IDecompilerEventListener>();

            // $TODO: the code below triggers the creation of the suffix array
            // The suffix array is currently unused but the algorithm that generates it scales poorly
            // making Reko unable to load certain EXE files (due to the endless wait times)
            // EnsureSuffixArray(filename + ".sufa-raw.ubj", image);
            var signature = Signatures.Where(s => Matches(s, loader.RawImage, entryPointOffset)).FirstOrDefault();
            if (signature is null || signature.Name is null)
                return loader;
            listener.Info("Signature of '{0}' detected.", signature.Name);
            var le = Services.RequireService<IConfigurationService>().GetImageLoader(signature.Name);  //$REVIEW: all of themn?
            if (le is null || (le.Type is null && le.TypeName is null))
            {
                Debug.Print("Signature of '{0}' recognized, but no loader is available for it.", signature.Name);
                return loader;
            }
            var unpacker = Loader.CreateOuterImageLoader<ProgramImageLoader>(Services, le.Type, le.TypeName, loader);
            if (unpacker is null)
            {
                listener.Warn("Unable to create loader for '{0}'.", signature.Name);
                return loader;
            }
            listener.Info("Using loader for '{0}'.", signature.Name);
            unpacker.Argument = le.Argument;
            return unpacker;
        }

        //$PERF: of course we should compile pattern files into a trie for super performance.
        //$REVIEW: move to ImageSignature class?
        // See https://www.hex-rays.com/products/ida/tech/flirt/in_depth.shtml for implementation
        // ideas.
        public static bool Matches(ImageSignature sig, byte[] image, uint entryPointOffset)
        {
            try
            {
                if (entryPointOffset >= image.Length || string.IsNullOrEmpty(sig.EntryPointPattern))
                    return false;
                int iImage =  (int)entryPointOffset;
                int iPattern = 0;
                while (iPattern < sig.EntryPointPattern!.Length - 1 && iImage < image.Length)
                {
                    var msn = sig.EntryPointPattern[iPattern];
                    var lsn = sig.EntryPointPattern[iPattern + 1];
                    if (msn != '?' && lsn != '?')
                    {
                        if (!BytePattern.TryParseHexDigit(msn, out var ms) ||
                            !BytePattern.TryParseHexDigit(lsn, out var ls))
                            return false;
                        var pat = ms << 4 | ls;
                        var img = image[iImage];
                        if (pat != img)
                            return false;
                    }
                    iImage += 1;
                    iPattern += 2;
                }
                return iPattern == sig.EntryPointPattern.Length;
            } catch
            {
                Debug.Print("Pattern for '{0}' is unhandled: {1}", sig.Name, sig.EntryPointPattern);
                return false;
            }
        }

        private object EnsureSuffixArray(string filename, byte[] image)
        {
            var fsSvc = Services.RequireService<IFileSystemService>();
            var listener = Services.RequireService<IDecompilerEventListener>();
            Stream? stm = null;
            try
            {
                if (fsSvc.FileExists(filename))
                {
                    stm = fsSvc.CreateFileStream(filename, FileMode.Open, FileAccess.Read);
                    try
                    {
                        var sSuffix = (int[])new UbjsonReader(stm).Read()!;
                        return SuffixArray.Load(image, sSuffix);
                    }
                    catch (Exception ex)
                    {
                        listener.Warn("Unable to load suffix array {0}. {1}", filename, ex.Message);
                    } finally
                    {
                        stm.Close();
                    }
                }
                var sa = SuffixArray.Create(image);
                stm = fsSvc.CreateFileStream(filename, FileMode.Create, FileAccess.Write);
                new UbjsonWriter(stm).Write(sa.Save());
                return sa;
            }
            finally
            {
                stm?.Dispose();
            }
        }
    }
}
