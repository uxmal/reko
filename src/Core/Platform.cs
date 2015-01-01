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

using Decompiler.Core.Rtl;
using System;
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
        /// <remarks>We don't actually need the architecture in the base class, but we have to force a 
        /// uniform constructor signature across all derived classes. All subclasses must implement this 
        /// constructor.
        /// </remarks>
        /// <param name="arch"></param>
        protected Platform(IServiceProvider services, IProcessorArchitecture arch) 
        {
            this.Services = services;
            this.Architecture = arch;
        }

        public IServiceProvider Services { get; private set; }
        public IProcessorArchitecture Architecture { get; private set; }

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
        /// The default encoding for byte-encoded text.
        /// </summary>
        /// <remarks>
        /// We use ASCII as the lowest common denominator here, but some arcane platforms (e.g.
        /// ZX-81) don't use ASCII.
        /// </remarks>
        public virtual Encoding DefaultTextEncoding { get { return Encoding.ASCII; } }

        public abstract string DefaultCallingConvention { get; }

        public abstract ProcedureSignature LookupProcedureByName(string moduleName, string procName);

        public virtual ProcedureSignature SignatureFromName(string fnName)
        {
            throw new NotImplementedException();
        }

        public virtual ProcedureSignature LookupProcedureByOrdinal(string moduleName, int ordinal)
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
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotSupportedException();
        }

        public override string DefaultCallingConvention
        {
            get { return ""; }
        }

        public override ProcedureSignature LookupProcedureByName(string moduleName, string procName)
        {
            return null;
        }
    }
}
