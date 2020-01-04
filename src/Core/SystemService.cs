#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
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
        public string ModuleName;
		public string Name;
		public SyscallInfo SyscallInfo;
		public FunctionType Signature;
		public ProcedureCharacteristics Characteristics;

		public ExternalProcedure CreateExternalProcedure(IProcessorArchitecture arch)
		{
            return new ExternalProcedure(Name, Signature, Characteristics);
		}
	}

	public class RegValue
	{
		public RegisterStorage Register;
		public int Value;
	}

    public class StackValue
    {
        public int Offset;
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
		public RegValue [] RegisterValues;

        /// <summary>
        /// Stack values that select which subservice of the system call to invoke.
        /// </summary>
        public StackValue[] StackValues;

		public bool Matches(int vector, ProcessorState state)
		{
			if (Vector != vector)
				return false;
            return Matches(state);
		}

        public bool Matches(ProcessorState state)
        {
            if (state == null &&
                ((RegisterValues != null && RegisterValues.Length > 0) ||
                 (StackValues != null && StackValues.Length > 0)))
            {
                return false;
            }
            for (int i = 0; i < RegisterValues.Length; ++i)
            {
                Constant v = state.GetRegister(RegisterValues[i].Register);
                if (v == null || v == Constant.Invalid)
                    return false;
                if (v.ToUInt32() != RegisterValues[i].Value)
                    return false;
            }
            if (StackValues != null && StackValues.Length > 0)
            {
                for (int i = 0; i < StackValues.Length; ++i)
                {
                    var c = state.GetStackValue(StackValues[i].Offset) as Constant;
                    if (c == null || c == Constant.Invalid)
                        return false;
                    if (c.ToUInt32() != StackValues[i].Value)
                        return false;
                }
            }
            return true;
        }
	}	
}
