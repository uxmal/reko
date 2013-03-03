#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Types;
using Decompiler.Core.Serialization;
using Decompiler.Core.Output;
using System;
using System.ComponentModel;
using System.IO;

namespace Decompiler.Core
{
    /// <summary>
    /// Abstract base class for all applicable procedure-like entities.
    /// </summary>
	[DefaultProperty("Name")]
	public abstract class ProcedureBase
	{
		private string name;

		public ProcedureBase(string name)
		{
			this.name = name;
			this.Characteristics = DefaultProcedureCharacteristics.Instance;
		}

		public string Name 
        {
			get { return name; }
		}

		public abstract ProcedureSignature Signature { get; set; }

		public ProcedureCharacteristics Characteristics { get; set; }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            Signature.Emit(this.Name, ProcedureSignature.EmitFlags.ArgumentKind, new Formatter(sw));
            return sw.ToString();
        }

		public TypeVariable TypeVariable { get; set; }
	}
}
