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

using System;
using System.ComponentModel;
using System.Reflection;

using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
	/// <summary>
	/// Extra characteristics that describe the semantics of a procedure.
	/// </summary>
	public class ProcedureCharacteristics
	{
        private bool allocator;
		private bool isAlloca;
		private bool terminates;
        private ArraySizeCharacteristic arraySize;

		public ProcedureCharacteristics()
		{
		}

		[XmlElement("is-alloca")]
		[DefaultValue(false)]
		public virtual bool IsAlloca
		{ 
			get { return isAlloca; }
			set { isAlloca = value; }
		}

		[XmlElement("terminates")]
		[DefaultValue(false)]
		public virtual bool Terminates
		{
			get { return terminates; }
			set { terminates = value; }
		}

		public void Write()
		{
			foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this))
			{
				DefaultValueAttribute dv = (DefaultValueAttribute)
					prop.Attributes[typeof(DefaultValueAttribute)];
			}
		}

        [XmlElement("allocator")]
        [DefaultValue(false)]
        public bool Allocator
        {
            get { return allocator; }
            set { allocator = value; }
        }

        [XmlElement("array-size")]
        public ArraySizeCharacteristic ArraySize
        {
            get { return arraySize; }
            set { arraySize = value; }
        }
    }

	public class DefaultProcedureCharacteristics : ProcedureCharacteristics
	{
		private static DefaultProcedureCharacteristics instance;

		private DefaultProcedureCharacteristics()
		{
		}

		static DefaultProcedureCharacteristics()
		{
			instance = new DefaultProcedureCharacteristics();
		}

		public static DefaultProcedureCharacteristics Instance
		{
			get { return instance; }
		}

		public override bool IsAlloca
		{
			get { return base.IsAlloca; }
			set { throw Invalid(); }
		}

		public override bool Terminates
		{
			get { return base.Terminates; }
			set { throw Invalid(); }
		}

		private Exception Invalid()
		{
			return new InvalidOperationException("Default characteristics may not be assigned");
		}
	}
}
