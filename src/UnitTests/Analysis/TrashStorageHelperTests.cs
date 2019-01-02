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

using Reko.Analysis;
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class TrashStorageHelperTests
    {
        private IStorageBinder binder;
        private TrashStorageHelper tsh;
        private TemporaryStorage trash;

        [SetUp]
        public void Setup()
        {
            binder = new Frame(PrimitiveType.Word32);
            trash = new TemporaryStorage("v0", 1, PrimitiveType.Word32);
            tsh = new TrashStorageHelper(trash);
        }

        [Test]
        public void TrashIdentifier()
        {
            var eax = binder.EnsureRegister(Registers.eax);
            tsh.Trash(eax, trash);
            Assert.AreEqual(1, tsh.TrashedRegisters.Count);
            Assert.AreSame(trash, tsh.TrashedRegisters[eax.Storage]);
        }

        [Test]
        public void CopyIdentifier()
        {
            var eax = binder.EnsureRegister(Registers.eax);
            var ebx = binder.EnsureRegister(Registers.ebx);
            tsh.Copy(eax, ebx);
            Assert.AreEqual(1, tsh.TrashedRegisters.Count);
            Assert.AreEqual("ebx", ((RegisterStorage) tsh.TrashedRegisters[eax.Storage]).Name);
        }

        [Test]
        public void TrashSequence()
        {
            Identifier es = binder.EnsureRegister(Registers.es);
            Identifier bx = binder.EnsureRegister(Registers.bx);
            Identifier es_bx = binder.EnsureSequence(PrimitiveType.Ptr32, es.Storage, bx.Storage);
            tsh.Trash(es_bx, trash);
            Assert.AreEqual("(bx:TRASH) (es:TRASH) (Sequence es:bx:TRASH) ", Dump(tsh.TrashedRegisters));
        }

        private string Dump(Dictionary<Storage, Storage> trashedRegs)
        {
            StringBuilder sb = new StringBuilder();
            SortedList<Storage, string> sl = new SortedList<Storage, string>(new StorageComparer());
            foreach (KeyValuePair<Storage, Storage> de in trashedRegs)
            {
                sl.Add(de.Key, de.Value != trash ? de.Value.ToString() : "TRASH");
            }
            foreach (KeyValuePair<Storage,string> de in sl)
            {
                sb.AppendFormat("({0}:{1}) ", de.Key, de.Value);
            }
            return sb.ToString();
        }
    }

    public class StorageComparer : IComparer<Storage>
    {
        #region IComparer Members

        public int Compare(Storage x, Storage y)
        {
            return String.Compare(x.ToString(), y.ToString());
        }

        #endregion
    }
}