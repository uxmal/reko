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

using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	/// <summary>
	/// This class describes extra-lingustic semantics of a procedure.
    /// These are often helpful when decompiling.
	/// </summary>
	public class ProcedureCharacteristics
	{
		public ProcedureCharacteristics()
		{
		}

        public ProcedureCharacteristics(ProcedureCharacteristics old)
        {
            IsAlloca = old.IsAlloca;
            Terminates = old.Terminates;
            Allocator = old.Allocator;
            ArraySize = old.ArraySize;
            VarargsParserClass = old.VarargsParserClass;
        }

		[XmlElement("is-alloca")]
		[DefaultValue(false)]
		public virtual bool IsAlloca { get; set; }

        /// <summary>
        /// This property is set to true if calling it terminates the thread of execution, i.e. control
        /// never returns.
        /// </summary>
		[XmlElement("terminates")]
        [DefaultValue(false)]
        public virtual bool Terminates { get; set; }

        /// <summary>
        /// This property is set to true if this procedure returns an allocated chunk of memory.
        /// </summary>
        [XmlElement("allocator")]
        [DefaultValue(false)]
        public bool Allocator { get; set; }

        [XmlElement("array-size")]
        public ArraySizeCharacteristic ArraySize { get; set; }

        [XmlElement("varargs")]
        public virtual string VarargsParserClass { get; set; }

        public void Write()
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this))
            {
                DefaultValueAttribute dv = (DefaultValueAttribute)
                    prop.Attributes[typeof(DefaultValueAttribute)];
            }
        }
    }

	public class DefaultProcedureCharacteristics : ProcedureCharacteristics
	{
		private DefaultProcedureCharacteristics()
		{
		}

		static DefaultProcedureCharacteristics()
		{
			Instance = new DefaultProcedureCharacteristics();
		}

		public static DefaultProcedureCharacteristics Instance { get; }

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

        [XmlElement("varargs")]
        public override string VarargsParserClass { get { return ""; } set { throw Invalid(); } }
		
        private Exception Invalid()
		{
			return new InvalidOperationException("Default characteristics may not be assigned.");
		}
	}
}
