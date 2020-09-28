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

using NUnit.Framework;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Lib
{
    public class BTreeDictionaryTests
    {
        private BTreeDictionary<string, int> Given_Dictionary(IEnumerable<int> items)
        {
            var btree = new BTreeDictionary<string, int>();
            foreach (var item in items)
            {
                btree.Add(item.ToString(), item);
            }
            return btree;
        }

        [Test]
        public void BTree_Create()
        {
            var btree = new BTreeDictionary<string, int>();
        }

        [Test]
        public void BTree_AddItem()
        {
            var btree = new BTreeDictionary<string, int>();
            btree.Add("3", 3);
            Assert.AreEqual(1, btree.Count);
        }

        [Test]
        public void BTree_AddTwoItems()
        {
            var btree = new BTreeDictionary<string, int>();
            btree.Add("3", 3);
            btree.Add("2", 2);
            Assert.AreEqual(2, btree.Count);
        }

        [Test]
        public void BTree_Enumerate()
        {
            var btree = new BTreeDictionary<string, int>();
            btree.Add("3", 3);
            btree.Add("2", 2);
            var e = btree.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("2", e.Current.Key);
            Assert.AreEqual(2, e.Current.Value);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("3", e.Current.Key);
            Assert.AreEqual(3, e.Current.Value);
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void BTree_Get()
        {
            var btree = new BTreeDictionary<string, int>();
            btree.Add("3", 3);
            Assert.AreEqual(3, btree["3"]);
        }

        [Test]
        public void BTree_EnumeratorThrowIfMutated()
        {
            var btree = new BTreeDictionary<string, int>();
            btree.Add("3", 3);
            var e = btree.GetEnumerator();
            Assert.True(e.MoveNext());
            btree.Add("2", 2);
            try
            {
                e.MoveNext();
                Assert.Fail("Should have thrown exception");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Test]
        public void BTree_SetNonExisting()
        {
            var btree = new BTreeDictionary<string, int>();
            btree["3"] = 3;
            Assert.AreEqual(3, btree["3"]);
        }

        [Test]
        public void BTree_SetExisting()
        {
            var btree = new BTreeDictionary<string, int>();
            btree["3"] = 3;
            btree["3"] = 2;
            Assert.AreEqual(2, btree["3"]);
        }

        [Test]
        public void BTree_ForceInternalNode()
        {
            var btree = new BTreeDictionary<string, int>();
            foreach (var i in Enumerable.Range(0, 256))
            {
                btree.Add(i.ToString(), i);
            }
            btree.Add("256", 256);
        }

        [Test]
        public void BTree_GetFromDeepTree()
        {
            var btree = new BTreeDictionary<string, int>();
            foreach (var i in Enumerable.Range(0, 1000))
            {
                btree.Add(i.ToString(), i);
            }
            btree.Dump();
            Assert.AreEqual(0, btree["0"]);
            Assert.AreEqual(500, btree["500"]);
        }

        [Test]
        public void BTree_ItemsSorted()
        {
            var rnd = new Random(42);
            var btree = new BTreeDictionary<string, int>();
            while (btree.Count < 500)
            {
                var n = rnd.Next(3000);
                var s = n.ToString();
                btree[s] = n;
            }
            string prev = "";
            foreach (var item in btree)
            {
                var cur = item.Key;
                Debug.Print("item.Key: {0}", item.Key);
                Assert.Less(prev, cur);
                prev = cur;
            }
        }

        [Test]
        public void BTree_IndexOf_Empty()
        {
            var btree = new BTreeDictionary<string, int>();
            int i = btree.Keys.IndexOf("3");
            Assert.AreEqual(-1, i);
        }

        [Test]
        public void BTree_IndexOf_existing_leaf_item()
        {
            var btree = new BTreeDictionary<string, int> { { "3", 3 } };
            int i = btree.Keys.IndexOf("3");
            Assert.AreEqual(0, i);
        }


        [Test]
        public void BTree_IndexOf_existing_leaf_item_2()
        {
            var btree = new BTreeDictionary<string, int> {
                { "3", 3 },
                { "2", 2 }
            };
            int i = btree.Keys.IndexOf("3");
            Assert.AreEqual(1, i);
        }

        [Test]
        public void BTree_IndexOf_nonexisting_small_leafitem()
        {
            var btree = new BTreeDictionary<string, int> {
                { "3", 3 },
                { "2", 2 }
            };
            int i = btree.Keys.IndexOf("1");
            Assert.AreEqual(~0, i);
        }

        [Test]
        public void BTree_IndexOf_nonexisting_middle_leafitem()
        {
            var btree = new BTreeDictionary<string, int> {
                { "4", 4 },
                { "2", 2 }
            };
            int i = btree.Keys.IndexOf("3");
            Assert.AreEqual(~1, i);
        }

        [Test]
        public void BTree_IndexOf_nonexisting_large_leafitem()
        {
            var btree = new BTreeDictionary<string, int> {
                { "4", 4 },
                { "2", 2 }
            };
            int i = btree.Keys.IndexOf("5");
            Assert.AreEqual(~2, i);
        }

        [Test]
        public void BTree_IndexOf_existing_item_1_ply_tree()
        {
            var btree = Given_Dictionary(Enumerable.Range(0, 20).Select(n => 1 + n * 2));
            int i = btree.Keys.IndexOf("1");
            Assert.AreEqual(0, i);
        }

        [Test]
        public void BTree_IndexOf_nonexisting_small_item_1_ply_tree()
        {
            var btree = Given_Dictionary(Enumerable.Range(0, 20).Select(n => 1 + n * 2));
            int i = btree.Keys.IndexOf("0");
            Assert.AreEqual(~0, i);
        }

        [Test]
        public void BTree_IndexOf_nonexisting_middle_item_1_ply_tree()
        {
            var btree = Given_Dictionary(Enumerable.Range(0, 20).Select(n => 1 + n * 2));
            int i = btree.Keys.IndexOf("14");
            Assert.AreEqual(~3, i);
        }

        [Test]
        public void BTree_IndexOf_nonexisting_middle_item_1_ply_tree_2()
        {
            var btree = Given_Dictionary(Enumerable.Range(0, 20).Select(n => 1 + n * 2));
            int i = btree.Keys.IndexOf("30");
            Assert.AreEqual(~12, i);
        }

        [Test]
        public void BTree_IndexOf_nonexisting_last_item_1_ply_tree_2()
        {
            var btree = Given_Dictionary(Enumerable.Range(0, 20).Select(n => 1 + n * 2));
            int i = btree.Keys.IndexOf("9999");
            Assert.AreEqual(~20, i);
        }

        [Test]
        public void BTree_IndexOf()
        {
            var rnd = new Random(42);
            var btree = new BTreeDictionary<string, int>();
            while (btree.Count < 100)
            {
                var n = rnd.Next(200);
                var s = n.ToString();
                btree[s] = n;
            }
            var items = btree.Keys.ToArray();
            for (int i = 0; i < items.Length; ++i)
            {
                Assert.AreEqual(i, btree.Keys.IndexOf(items[i]));
            }
        }

        [Test]
        public void BTree_GetItemByIndex()
        {
            var btree = Given_Dictionary(new[] {
                5,6,9,1,3, 4,2,7,8,0,
                10,18,17,12,14, 13,11,19,16,15});
            var items = btree.Keys.ToArray();
            for (int i = 0; i < btree.Count; ++i)
            {
                Assert.AreEqual(items[i], btree.Keys[i]);
            }
        }

        [Test]
        public void BTree_Remove_existing_leaf()
        {
            var btree = Given_Dictionary(new[] { 3 });
            bool removed = btree.Remove("3");
            Assert.IsTrue(removed);
            Assert.AreEqual(0, btree.Count);
            int items = 0;
            foreach (var entry in btree)
            {
                ++items;
            }
            Assert.AreEqual(0, items);
        }

        [Test]
        public void BTree_Remove_existing_leaf2()
        {
            var btree = Given_Dictionary(new[] { 3, 4 });
            bool removed = btree.Remove("3");
            Assert.IsTrue(removed);
            Assert.AreEqual(1, btree.Count);
            int items = 0;
            foreach (var entry in btree)
            {
                ++items;
            }
            Assert.AreEqual(1, items);
        }

        [Test]
        public void BTree_Remove_existing_deep_leaf()
        {
            var btree = Given_Dictionary(Enumerable.Range(0, 40));
            bool removed = btree.Remove("3");
            Assert.IsTrue(removed);
            Assert.AreEqual(39, btree.Count);
            int items = 0;
            foreach (var entry in btree)
            {
                ++items;
            }
            Assert.AreEqual(39, items);
        }

        [Test]
        public void BTree_Remove_Exercise()
        {
            var btree = Given_Dictionary(Enumerable.Range(0, 40));
            foreach (var n in Enumerable.Range(0, 40))
            {
                Assert.IsTrue(btree.Remove(n.ToString()));
            }
            foreach (var n in Enumerable.Range(0, 40))
            {
                btree.Add(n.ToString(), n);
            }
            Assert.AreEqual(40, btree.Count);
        }
    }
}
