#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

    /// <summary>
    /// Represents members in a <see cref="ClassType"/>.
    /// </summary>
    public class ClassMember
    {
        private string? name;

        /// <summary>
        /// The protection level of this member.
        /// </summary>
        public AccessSpecifier Protection { get; set; }

        /// <summary>
        /// The attributes of this member.
        /// </summary>
        public ClassMemberAttribute Attribute { get; set; }

        /// <summary>
        /// The name of this member.
        /// </summary>
        public string Name {
            get { return name ?? DefaultFieldName(); }
            set { this.name = value; }
        }

        /// <summary>
        /// If this is a field, the offset within the class data.
        /// If this is a method, the offset within a virtual table.
        /// </summary>
        public int Offset { get; set; }  // Offset of a field, self-evident
                                         // Offset of a method, in a virtual table.

        private string DefaultFieldName()
        {
            return $"m_{Offset:X4}";
        }
    }

    /// <summary>
    /// A data field in a class.
    /// </summary>
    public class ClassField : ClassMember
    {
        /// <summary>
        /// The data type of this field.
        /// </summary>
        public DataType DataType { get; }

        /// <summary>
        /// Constructs a <see cref="ClassField" /> instance.
        /// </summary>
        /// <param name="type">Data type of the field.</param>
        public ClassField(DataType type)
        {
            this.DataType = type;
        }
    }

    /// <summary>
    /// Represents a method in a class.
    /// </summary>
    public class ClassMethod : ClassMember
    {
        /// <summary>
        /// The procedure that implements this method.
        /// </summary>
        public ProcedureBase? Procedure { get; set; }
    }

#pragma warning disable CS1591

    /// <summary>
    /// The access specifier level of a class member.
    /// </summary>
    public enum AccessSpecifier
    {
        Private,
        Protected,
        Public
    }

    /// <summary>
    /// Attributes of a class member.
    /// </summary>
    public enum ClassMemberAttribute
    {
        None,
        Virtual,
        Static,
    }

    public class ClassBase
    {
        public AccessSpecifier Protection;
        public CompositeType? BaseType;
    }
}
