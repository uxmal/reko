#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using NUnit.Framework;
using Reko.Core;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Lib
{
    [TestFixture]
    public class ByteTrieTests
    {
        private static byte[] B(string hexString) => BytePattern.FromHexBytes(hexString);

        [Test]
        public void ByteTrie_GetEmpty()
        {
            var trie = new ByteTrie<string>();

            var m = trie.Match(B("00"));

            Assert.IsFalse(m.Success);
        }

        [Test]
        public void ByteTrie_Match()
        {
            var trie = new ByteTrie<string>();
            trie.Add(B("00"), "nop");
            trie.Add(B("01"), "add");

            var m = trie.Match(B("030401"), 1);

            Assert.IsTrue(m.Success);
            Assert.AreEqual(2, m.Index);
            Assert.AreEqual(1, m.Length);
            Assert.AreEqual("add", m.Value);
        }

        [Test]
        public void ByteTrie_Match_Longest()
        {
            var trie = new ByteTrie<string>();
            trie.Add(B("04"), "short");
            trie.Add(B("0401"), "long");

            var m = trie.Match(B("030401"));

            Assert.IsTrue(m.Success);
            Assert.AreEqual(1, m.Index);
            Assert.AreEqual(2, m.Length);
            Assert.AreEqual("long", m.Value);
        }

        [Test]
        public void ByteTrie_Match_Best()
        {
            var trie = new ByteTrie<string>();
            trie.Add(B("012230"), "long");
            trie.Add(B("0122"), "short");

            var m = trie.Match(B("03012245"));

            Assert.IsTrue(m.Success);
            Assert.AreEqual(1, m.Index);
            Assert.AreEqual(2, m.Length);
            Assert.AreEqual("short", m.Value);
        }

        [Test]
        public void ByteTrie_Match_NextMatch()
        {
            var trie = new ByteTrie<string>();
            trie.Add(B("4711"), "magic");

            var bytes = B("000047114711FF471147");
            var m = trie.Match(bytes);

            Assert.IsTrue(m.Success);
            Assert.AreEqual(2, m.Index);

            m = m.NextMatch(bytes);
            Assert.IsTrue(m.Success);
            Assert.AreEqual(4, m.Index);

            m = m.NextMatch(bytes);
            Assert.IsTrue(m.Success);
            Assert.AreEqual(7, m.Index);

            m = m.NextMatch(bytes);
            Assert.IsFalse(m.Success);
        }

        [Test]
        public void ByteTrie_Match_Mask()
        {
            var trie = new ByteTrie<string>();
            trie.Add(B("0404"), B("C4F4"), "masked");
            trie.Dump();

            var m = trie.Match(B("0404"));
            Assert.IsTrue(m.Success);
            Assert.AreEqual(0, m.Index);

            m = trie.Match(B("B33D05"));
            Assert.IsTrue(m.Success);
            Assert.AreEqual(1, m.Index);
        }
    }
}
