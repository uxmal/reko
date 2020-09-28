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
using Reko.Core;
using Reko.Typing;
using NUnit.Framework;
using System;
using Reko.Core.Expressions;

namespace Reko.UnitTests.Typing
{
	[TestFixture]
	public class TypeStoreTests
	{
        private TypeFactory factory;
        private TypeStore store;

        [SetUp]
        public void Setup()
        {
            this.store = new TypeStore();
            this.factory = new TypeFactory();
        }

        [Test]
		public void TystCopyClassToTypes()
		{
			TypeVariable tv1 = store.CreateTypeVariable(factory);
            TypeVariable tv2 = store.CreateTypeVariable(factory);
			Assert.IsNotNull(tv1.Class, "Expected store.EnsureTypeVariable to create an equivalence class");
			Assert.IsNotNull(tv2.Class, "Expected store.EnsureTypeVariable to create an equivalence class");
			EquivalenceClass e = store.MergeClasses(tv1, tv2);
			
			e.DataType = PrimitiveType.Word16;

			store.CopyClassDataTypesToTypeVariables();
			foreach (TypeVariable tv in store.TypeVariables)
			{
				Assert.IsNotNull(tv.DataType);
			}
		}
    }
}
