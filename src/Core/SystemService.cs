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

using Reko.Core.Expressions;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System.ComponentModel;

namespace Reko.Core
{
    /// <summary>
    /// Models a system service. The syscallinfo member indicates how the
    /// system service is selected (typically by loading specific values in
    /// processor registers).
    /// </summary>
    [Designer("Reko.Gui.Design.SystemServiceDesigner,Reko.Gui")]
	public class SystemService
	{
        /// <summary>
        /// Optional name of the module that exports this system service.
        /// </summary>
        public string? ModuleName { get; set; }

        /// <summary>
        /// Optional name of the system service. At times, names are not
        /// available.
        /// </summary>

        /// <summary>
        /// Name of the system service.
        /// </summary>
		public string? Name;

        /// <summary>
        /// Register state that selects the system service.
        /// </summary>
		public SyscallInfo? SyscallInfo;

        /// <summary>
        /// Function signature of the service.
        /// </summary>
		public FunctionType? Signature;

        /// <summary>
        /// Optional characteristics of the system service.
        /// </summary>
		public ProcedureCharacteristics? Characteristics;

        /// <summary>
        /// Creates an external procedure from this system service.
        /// </summary>
        /// <param name="arch"><see cref="IProcessorArchitecture"/> to use.</param>
        /// <returns>An <see cref="ExternalProcedure"/>.</returns>
		public ExternalProcedure CreateExternalProcedure(IProcessorArchitecture arch)
		{
            return new ExternalProcedure(Name!, Signature!, Characteristics);
		}
	}

    /// <summary>
    /// A register value.
    /// </summary>
	public class RegValue
	{
        /// <summary>
        /// Register.
        /// </summary>
		public RegisterStorage? Register;

        /// <summary>
        /// Value in register.
        /// </summary>
		public int Value;
	}

    /// <summary>
    /// A stack value.
    /// </summary>
    public class StackValue
    {
        /// <summary>
        /// Stack offset.
        /// </summary>
        public int Offset;

        /// <summary>
        /// Value in stack.
        /// </summary>
        public int Value;
    }

    /// <summary>
    /// Describes an exported entry point to an operating system service, or an
    /// ordinal entry point to a dynamic library.
    /// </summary>
	public class SyscallInfo
	{
        /// <summary>
        /// Either an interrupt vector number or an ordinal entry point to a dynamic library.
        /// </summary>
		public int Vector;

        /// <summary>
        /// Register values that select which subservice of the system call to invoke.
        /// </summary>
		public RegValue[]? RegisterValues;

        /// <summary>
        /// Stack values that select which subservice of the system call to invoke.
        /// </summary>
        public StackValue[]? StackValues;

        /// <summary>
        /// Returns true if the given vector and processor state match this system call.
        /// </summary>
        /// <param name="vector">Interrupt vector invoked.</param>
        /// <param name="state">Current processor state.</param>
        /// <returns>True if this syscallinfo matches; otherwise false.
        /// </returns>
		public bool Matches(int vector, ProcessorState? state)
		{
			if (Vector != vector)
				return false;
            return Matches(state);
		}

        /// <summary>
        /// Returns true if the given processor state matches this system call.
        /// </summary>
        /// <param name="state">Current processor state.</param>
        /// <returns>True if this syscallinfo matches; otherwise false.
        /// </returns>
        public bool Matches(ProcessorState? state)
        {
            if (state is null &&
                ((RegisterValues is not null && RegisterValues.Length > 0) ||
                 (StackValues is not null && StackValues.Length > 0)))
            {
                return false;
            }
            if (RegisterValues is not null)
            {
                for (int i = 0; i < RegisterValues.Length; ++i)
                {
                    Constant v = state!.GetRegister(RegisterValues[i].Register!);
                    if (v is null || !v.IsValid)
                        return false;
                    if (v.ToUInt32() != RegisterValues[i].Value)
                        return false;
                }
            }
            if (StackValues is not null && StackValues.Length > 0)
            {
                for (int i = 0; i < StackValues.Length; ++i)
                {
                    if (state!.GetStackValue(StackValues[i].Offset) is not Constant c || !c.IsValid)
                        return false;
                    if (c.ToUInt32() != StackValues[i].Value)
                        return false;
                }
            }
            return true;
        }
	}	
}
