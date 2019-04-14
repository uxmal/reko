#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Core.Types;
using Reko.Core.Serialization;
using Reko.Core.Output;
using System;
using System.ComponentModel;
using System.IO;

namespace Reko.Core
{
    
    /// <summary>
    /// Abstract base class for all things callable.
    /// </summary>
	[DefaultProperty("Name")]
	public abstract class ProcedureBase
	{ 
		public ProcedureBase(string name)
		{
            NameProvenance = ProvenanceType.Scanning;
            this.name = name;
			this.Characteristics = DefaultProcedureCharacteristics.Instance;
		}

        /// <summary>
        /// The name of the procedure.
        /// </summary>
        public string Name { get { return name; }
        }

        public event EventHandler NameChanged;
        private string name;
        private ProvenanceType NameProvenance;

        public void ChangeName(string sigName, ProvenanceType provenance)
        {
            if((provenance == ProvenanceType.ByteSignature))
            {
                // We want to prevent a signature over writing a user name
                if (NameProvenance == ProvenanceType.Scanning)
                {
                    name = sigName;
                    NameProvenance = ProvenanceType.ByteSignature;
                    NameChanged.Fire(this);
                }
            }
            else
            {
                name = sigName;
                NameProvenance = ProvenanceType.UserInput;
                NameChanged.Fire(this);
            }
        }

        public void UpdateNameByUser(string sigName)
        {
            name = sigName;
            NameProvenance = ProvenanceType.UserInput;
            NameChanged.Fire(this);
        }

        public void UpdateNameBySystem(string sigName)
        {
            name = sigName;
            NameProvenance = ProvenanceType.Scanning;
            NameChanged.Fire(this);
        }

        public ProvenanceType NameAssignedBy
        {
            get
            {
                return NameProvenance;
            }
        }

		public abstract FunctionType Signature { get; set; }

		public ProcedureCharacteristics Characteristics { get; set; }

        /// <summary>
        /// If this is a member function of a class or struct, this property
        /// will be a reference to the enclosing type.
        /// </summary>
        //$TODO: all the infrastructure for class loading remains to be
        // implemented; for now we just store the class name.
        public SerializedType EnclosingType { get; set; }

        public override string ToString()
        {
            StringWriter sw = new StringWriter();
            Signature.Emit(this.Name, FunctionType.EmitFlags.ArgumentKind, new TextFormatter(sw));
            return sw.ToString();
        }
	}
}
