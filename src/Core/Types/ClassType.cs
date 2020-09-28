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
        public ClassType() : this(null)
        {
        }

        public ClassType(string name) : base(name)
        {
            Fields = new List<ClassField>();
            Methods = new List<ClassMethod>();
            Bases = new List<ClassBase>();
        }

        public bool UserDefined { get; set; }
        public List<ClassField> Fields { get; private set; }
        public List<ClassMethod> Methods { get; private set; }
        public List<ClassBase> Bases { get; private set; }
        public override int Size { get; set; }

        public override void Accept(IDataTypeVisitor v)
        {
            v.VisitClass(this);
        }

        public override T Accept<T>(IDataTypeVisitor<T> v)
        {
            return v.VisitClass(this);
        }

        public override DataType Clone(IDictionary<DataType, DataType> clonedTypes)
        {
            throw new NotImplementedException();
        }

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
        public ClassProtection Protection;
        public ClassMemberAttribute Attribute;
        public string Name;
        public int Offset;  // Offset of a field, self-evident
                            // Offset of a method, in a virtual table.
    }

    public class ClassField : ClassMember
    {
        public DataType DataType;
    }

    public class ClassMethod : ClassMember
    {
       public ProcedureBase Procedure;
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
        public CompositeType BaseType;
    }
}
