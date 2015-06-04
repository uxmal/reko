#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Configuration;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Rtl;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core
{
	/// <summary>
	/// A Platform is an abstraction of the operating environment,
    /// say MS-DOS, Win32, or Posix.
	/// </summary>
	public abstract class Platform
	{
        /// <summary>
        /// Initializes a Platform instance
        /// </summary>
        /// <param name="arch"></param>
        protected Platform(IServiceProvider services, IProcessorArchitecture arch) 
        {
            this.Services = services;
            this.Architecture = arch;
        }

        public IProcessorArchitecture Architecture { get; private set; }
        public IServiceProvider Services { get; private set; }
        public TypeLibrary[] TypeLibs { get; private set; }
        public CharacteristicsLibrary[] CharacteristicsLibs { get; private set; }

        public virtual PrimitiveType FramePointerType { get { return Architecture.FramePointerType; } }
        public virtual PrimitiveType PointerType { get { return Architecture.PointerType; } }

        /// <summary>
        /// The default encoding for byte-encoded text.
        /// </summary>
        /// <remarks>
        /// We use ASCII as the lowest common denominator here, but some arcane platforms (e.g.
        /// ZX-81) don't use ASCII.
        /// </remarks>
        public virtual Encoding DefaultTextEncoding { get { return Encoding.ASCII; } }

        public abstract string DefaultCallingConvention { get; }

        /// <summary>
        /// Creates a bitset that represents those registers that are never used as arguments to a 
        /// procedure. 
        /// </summary>
        /// <remarks>
        /// Typically, the stack pointer register is one of these registers. Some architectures define
        /// global registers that are preserved across calls; these should also be present in this set.
        /// </remarks>
        public abstract BitSet CreateImplicitArgumentRegisters();

        public IEnumerable<Address> CreatePointerScanner(ImageMap imageMap, ImageReader rdr, Address[] address, PointerScannerFlags pointerScannerFlags)
        {
            return Architecture.CreatePointerScanner(imageMap, rdr, address, pointerScannerFlags);
        }

        /// <summary>
        /// Utility function for subclasses that loads all type libraries and characteristics libraries 
        /// </summary>
        /// <param name="envName"></param>
        protected void EnsureTypeLibraries(string envName)
        {
            if (TypeLibs == null)
            {
                var cfgSvc = Services.RequireService<IConfigurationService>();
                var envCfg = cfgSvc.GetEnvironment(envName);
                if (envCfg == null)
                    throw new ApplicationException(string.Format(
                        "Environment '{0}' doesn't appear in the configuration file. Your installation may be out-of-date.",
                        envName));
                var tlSvc = Services.RequireService<ITypeLibraryLoaderService>();
                this.TypeLibs = ((System.Collections.IEnumerable)envCfg.TypeLibraries)
                    .OfType<ITypeLibraryElement>()
                    .Select(tl => tlSvc.LoadLibrary(Architecture, cfgSvc.GetPath(tl.Name)))
                    .Where(tl => tl != null).ToArray();
                this.CharacteristicsLibs = ((System.Collections.IEnumerable)envCfg.CharacteristicsLibraries)
                    .OfType<ITypeLibraryElement>()
                    .Select(cl => tlSvc.LoadCharacteristics(cl.Name))
                    .Where(cl => cl != null).ToArray();
            }
        }

		public abstract SystemService FindService(int vector, ProcessorState state);

        public virtual SystemService FindService(RtlInstruction rtl, ProcessorState state)
        {
            return null;
        }

        public virtual SystemService FindService(string name)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// If the instructions located at the address the image reader is reading are a 
        /// trampoline, returns the procedure where the destination is located, otherwise
        /// returns null.
        /// </summary>
        /// <param name="imageReader"></param>
        /// <returns></returns>
        public abstract ProcedureBase GetTrampolineDestination(ImageReader imageReader, IRewriterHost host);

        public virtual Address MakeAddressFromConstant(Constant c)
        {
            return Architecture.MakeAddressFromConstant(c);
        }

        /// <summary>
        /// Given a linear address, converts it to an Address instance. By default,
        /// use the architecture pointer size for the address.
        /// </summary>
        /// <remarks>
        /// The method is virtual to allow a platform to override the pointer size. For instance
        /// although the PowerPC 64 has 64-bit addresses, the Playstation3 implementation 
        /// uses 32-bit addresses.
        /// </remarks>
        /// <param name="uAddr"></param>
        /// <returns></returns>
        public virtual Address MakeAddressFromLinear(ulong uAddr)
        {
            return Address.Create(Architecture.PointerType, uAddr);
        }

        public abstract ExternalProcedure LookupProcedureByName(string moduleName, string procName);

        /// <summary>
        /// Guess signature from the name of the procedure.
        /// </summary>
        /// <param name="fnName"></param>
        /// <returns>null if there is no way to guess a ProcedureSignature from the name.</returns>
        public virtual ProcedureSignature SignatureFromName(string fnName)
        {
            return null;
        }

        public virtual ExternalProcedure LookupProcedureByOrdinal(string moduleName, int ordinal)
        {
            return null;
        }
   }

    /// <summary>
    /// The default platform is used when a specific platform cannot be determind.
    /// </summary>
    public class DefaultPlatform : Platform
    {
        public DefaultPlatform(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch)
        {
            this.TypeLibraries = new List<TypeLibrary>();
        }

        public List<TypeLibrary> TypeLibraries { get; private set; }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override BitSet CreateImplicitArgumentRegisters()
        {
            return Architecture.CreateRegisterBitset();
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotSupportedException();
        }

        public override ProcedureBase GetTrampolineDestination(ImageReader imageReader, IRewriterHost host)
        {
            // No trampolines are supported.
            return null;
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            //$IDentical to Win32, move into base class?
            return TypeLibraries
                .Select(t => t.Lookup(procName))
                .Where(sig => sig != null)
                .Select(sig => new ExternalProcedure(procName, sig))
                .FirstOrDefault();
        }
    }
}
