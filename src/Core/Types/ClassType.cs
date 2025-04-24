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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Types
{
    /// <summary>
    /// Models a C++ class.
    /// </summary>
    public class ClassType : CompositeType
    {
        /// <summary>
        /// Constructs a ClassType instance.
        /// </summary>
        /// <param name="name">Optional name for the class.</param>
        public ClassType(string? name = null) :
            base(Domain.Class, name)
        {
            Fields = new List<ClassField>();
            Methods = new List<ClassMethod>();
            Bases = new List<ClassBase>();
        }

        /// <summary>
        /// The fields of this class, ordered by offset.
        /// </summary>
        public List<ClassField> Fields { get; private set; }

        /// <summary>
        /// The methods of this class.
        /// </summary>
        public List<ClassMethod> Methods { get; private set; }

        /// <summary>
        /// Zero or more base classes of this class.
        /// </summary>
        public List<ClassBase> Bases { get; private set; }

        /// <inheritdoc/>
        public override int Size { get; set; }

        /// <inheritdoc/>
        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitClass(this);
        }

        /// <inheritdoc/>
        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitClass(this);
        }

        /// <inheritdoc/>
        public override DataType Clone(IDictionary<DataType, DataType>? clonedTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fetches all the virtual methods of this class, ordered by offset.
        /// </summary>
        public IEnumerable<ClassMethod> VirtualMethods
        {
            get
            {
                return Methods
                    .Where(m => m.Attribute == ClassMemberAttribute.Virtual)
                    .OrderBy(m => m.Offset);
            }
        }
    }

    public class ClassMember
    {
        private string? name;

        public ClassProtection Protection { get; set; }
        public ClassMemberAttribute Attribute { get; set; }
        public string Name {
            get { return name ?? DefaultFieldName(); }
            set { this.name = value; }
        }

        public int Offset { get; set; }  // Offset of a field, self-evident
                                         // Offset of a method, in a virtual table.

        private string DefaultFieldName()
        {
            return $"m_{Offset:X4}";
        }
    }

    public class ClassField : ClassMember
    {
        public DataType DataType;

        public ClassField(DataType type)
        {
            this.DataType = type;
        }
    }

    public class ClassMethod : ClassMember
    {
       public ProcedureBase? Procedure;
    }

    public enum ClassProtection
    {
        Private,
        Protected,
        Public
    }

    public enum ClassMemberAttribute
    {
        None,
        Virtual,
        Static,
    }

    public class ClassBase
    {
        public ClassProtection Protection;
        public CompositeType? BaseType;
    }
}
