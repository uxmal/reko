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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class NativeTypeFactory : MarshalByRefObject, INativeTypeFactory
    {
        private SortedList<DataType,HExpr> handles;
        private SortedList<HExpr, DataType> types;
        private int counter = 1000;

        public NativeTypeFactory()
        {
            this.handles = new SortedList<DataType, HExpr>(new DataTypeComparer());
            this.types = new SortedList<HExpr, DataType>();
            foreach (var de in Interop.DataTypes)
            {
                handles.Add(de.Value, (HExpr)de.Key);
                types.Add((HExpr)de.Key, de.Value);
            }
        }

        public DataType GetRekoType(HExpr a)
        {
            return types[a];
        }

        private HExpr MapToHandle(DataType dt)
        {
            if (handles.TryGetValue(dt, out HExpr h))
                return h;
            ++counter;
            h = (HExpr)(handles.Count + counter);
            handles.Add(dt, h);
            types.Add(h, dt);
            return h;
        }

        private DataType GetDataType(HExpr dt)
        {
            return types[dt];
        }

        public HExpr ArrayOf(HExpr dt, int length)
        {
            var dataType = GetDataType(dt);
            var a = new ArrayType(dataType, length);
            return MapToHandle(a);
        }

        public void BeginFunc(HExpr dt, int byte_size)
        {
            throw new NotImplementedException();
        }

        public void BeginStruct(HExpr dt, int byte_size)
        {
            throw new NotImplementedException();
        }

        public HExpr EndFunc()
        {
            throw new NotImplementedException();
        }

        public HExpr EndStruct()
        {
            throw new NotImplementedException();
        }

        public void Field(HExpr dt, int offset, string name)
        {
            throw new NotImplementedException();
        }

        public void Parameter(HExpr dt, string name)
        {
            throw new NotImplementedException();
        }

        public HExpr PtrTo(HExpr dt, int byte_size)
        {
            throw new NotImplementedException();
        }
    }
}
