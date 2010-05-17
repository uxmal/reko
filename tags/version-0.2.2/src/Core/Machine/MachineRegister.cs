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

using Decompiler.Core.Lib;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Machine
{
	/// <summary>
	/// Flyweight representation of a machine register.
	/// </summary>
	public class MachineRegister : MachineRegisterBase
	{
		private int number;

		public MachineRegister(string name, int number, PrimitiveType dt) : base(name, dt)
		{
			this.number = number;
		}

        /// <summary>
        /// If this register is a subregister of a wider register, this property the bit offset within that wider register.
        /// </summary>
        /// <remarks>For instance, on i386 systems, AH would return 8 here, since it is located at that bit offset of EAX.</remarks>
		public virtual int AliasOffset
		{
			get { return 0; }
		}

        /// <summary>
        /// Returns a number that uniquely identifies this identifier.
        /// </summary>
		public int Number
		{
			get { return number; }
		}

		public virtual MachineRegister GetSubregister(int offset, int size)
		{
			throw new NotSupportedException(string.Format("Invalid offset {0} or size {1} for register {2}.", offset, size, Name));
		}

		public override int SubregisterOffset(MachineRegisterBase subReg)
		{
			MachineRegister sub = subReg as MachineRegister;
			if (sub != null)
			{
				if (Number == sub.Number)
					return 0;
			}
			return -1;
		}

        /// <summary>
        /// Returns true if this is an ALU register that supports operations like addition, address dereference and the like.
        /// </summary>
		public virtual bool IsAluRegister
		{
			get { return true; }
		}

		/// <summary>
		/// Returns true if this register is strictly contained by reg2.
		/// </summary>
		/// <param name="reg1"></param>
		/// <param name="reg2"></param>
		/// <returns></returns>
		public virtual bool IsSubRegisterOf(MachineRegister reg2)
		{
			return false;
		}
		
		/// <summary>
		/// Given a register, sets/resets the bits corresponding to the register
		/// and any other registers it aliases.
		/// </summary>
		/// <param name="iReg">Register to set</param>
		/// <param name="bits">BitSet to modify</param>
		/// <param name="f">truth value to set</param>
		public virtual void SetAliases(BitSet bitset, bool f)
		{
			bitset[Number] = f;
		}

		public virtual void SetRegisterFileValues(ulong [] registerFile, ulong value, bool [] valid)
		{
			registerFile[Number] = value;
			valid[Number] = true;
		}

		public MachineRegister GetPart(DataType width)
		{
			return GetSubregister(0, width.BitSize);
		}

		public virtual MachineRegister GetWidestSubregister(BitSet bits)
		{
			return null;
		}

        public static MachineRegister None { get { return none; } }

        private static MachineRegister none = new MachineRegister("None", -1, PrimitiveType.Void);
    }

	public abstract class MachineRegisterBase
	{
		private string name;
		private PrimitiveType dt;

		public MachineRegisterBase(string name, PrimitiveType dt)
		{
			this.name = name;
			this.dt = dt;
		}

		public string Name
		{
			get { return name; }
		}

		public PrimitiveType DataType 
		{
			get { return dt; }
		}

		public abstract int SubregisterOffset(MachineRegisterBase subReg);

		public override string ToString()
		{
			return Name;
		}
	}
}
