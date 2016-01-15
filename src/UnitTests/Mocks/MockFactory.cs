#region License
/* 
 * Copyright (C) 1999-2016 Pavel Tomin.
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

using Reko.Core.Serialization;
using Reko.Core.Types;
using System;
using Rhino.Mocks;

namespace Reko.UnitTests.Mocks
{
    /// <summary>
    /// Utility class to simplify common unit test setup tasks.
    /// </summary>
    public class MockFactory
    {
        private MockRepository mr;

		public MockFactory(MockRepository mr)
		{
            this.mr = mr;
		}

        /// <summary>
        /// Create a deserializer that doesn't depend on TypeLibrary.
        /// </summary>
        /// <returns></returns>
        public ISerializedTypeVisitor<DataType> CreateDeserializer()
        {
            var deserializer = mr.Stub<ISerializedTypeVisitor<DataType>>();
            deserializer.Stub(d => d.VisitPrimitive(null))
                .IgnoreArguments()
                .Do(new Func<PrimitiveType_v1, PrimitiveType>(
                    p => PrimitiveType.Create(p.Domain, p.ByteSize)));
            deserializer.Stub(d => d.VisitTypeReference(null))
                .IgnoreArguments()
                .Do(new Func<SerializedTypeReference, TypeReference>(
                    t => new TypeReference(t.TypeName, null)));

            return deserializer;
        }
    }
}
