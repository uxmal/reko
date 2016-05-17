/*
 Copyright (c) 2012 Eran Meir

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
*/

using Reko.Core.Lib;
using System;
using NUnit.Framework;
using System.Linq;
using System.Diagnostics;

namespace Reko.UnitTests.Core.Lib
{
    public class SuffixArrayTests
    {

        [SetUp]
        public void Init()
        {
        }

        [Test]
        public void Test1()
        {
            SuffixArray sa = new SuffixArray("");
            Assert.AreEqual(sa.IndexOf("a"), -1, "Found wrong substring");
        }

        [Test]
        public void Test2()
        {
            SuffixArray sa = new SuffixArray(null);
            Assert.AreEqual(sa.IndexOf("a"), -1, "Found wrong substring");
        }

        /// <summary>
        /// Testing sample from wikipedia's entry for Suffix Array 
        /// http://en.wikipedia.org/wiki/Suffix_array
        /// </summary>
        [Test]
        public void Test3()
        {
            string str = "abracadabra";
            string[] expectedSubstrs = { "a", "abra", "abracadabra", "acadabra", "adabra", "bra", "bracadabra", "cadabra", "dabra", "ra", "racadabra" };
            int[] expectedLcps = { 0, 1, 4, 1, 1, 0, 3, 0, 0, 0, 2 };
            SuffixArray sa = new SuffixArray(str);

            PrintSortedArray(sa);

            Assert.AreEqual(sa.Length, str.Length, "Wrong SA length");
            Assert.AreEqual(sa.Lcp.Length, str.Length + 1, "Wrong LCP length");

            for (int i = 0; i < str.Length; ++i)
            {
                Assert.AreEqual(str.Substring(sa[i]), expectedSubstrs[i], String.Format("Wrong entry {0}", i));
                Assert.AreEqual(sa.Lcp[i], expectedLcps[i], String.Format("Wrong LCP {0}", i));
            }
        }
        /// <summary>
        /// Simple functional tests
        /// </summary>
        [Test]
        public void TestSearch1()
        {
            SuffixArray sa = new SuffixArray("yakawow");
            Assert.AreEqual(sa.IndexOf("a"), 1, "Substring not found/Wrong index");
        }
        [Test]
        public void TestSearch2()
        {
            SuffixArray sa = new SuffixArray("yakawow");
            Assert.AreEqual(sa.IndexOf("yakawow"), 0, "Wrong index");
        }
        [Test]
        public void TestSearch3()
        {
            SuffixArray sa = new SuffixArray("yakawow");
            Assert.AreEqual(sa.IndexOf("z"), -1, "Found wrong substring.");
        }
        [Test]
        public void TestSearch4()
        {
            SuffixArray sa = new SuffixArray("yakawow");
            Assert.AreEqual(sa.IndexOf(null), -1, "Found wrong substring.");
        }
        [Test]
        public void TestSearch5()
        {
            SuffixArray sa = new SuffixArray("yakawow");
            Assert.AreEqual(sa.IndexOf(""), -1, "Wrong index");
        }

        public void PrintSortedArray(SuffixArray sa)
        {
            for (int i = 0; i < sa.Length; i++)
            {
                Console.Write("{0} {1} {2} lcp = {3}",
                    i, 
                    sa[i],
                    sa.Str.Substring(sa[i]),
                    sa.Lcp[i].ToString());
            }
        }

        [Test]
        public void Occurences()
        {
            var sa = new SuffixArray("zappa");
            Assert.AreEqual(new[] { 1, 4 }, sa.FindOccurences("a").ToArray());
        }

        [Test]
        public void Occurences2()
        {
            var sa = new SuffixArray("papapa");
            PrintSortedArray(sa);
            Assert.AreEqual(new[] { 0, 2 }, sa.FindOccurences("papa").ToArray());
        }

        [Test]
        public void Timing()
        {
            for (int N = 2; N < 100000000; N = N * 3 / 2)
            {
                var rnd = new Random(0x4711);
                var bytes = new byte[N];
                rnd.NextBytes(bytes);
                var str = new string(bytes.Select(b => (char)b).ToArray());
                var sw1 = Time(str, s => new SuffixArray(s));
                var sw2 = Time(str, s => { });
                var beforeGc = GC.GetTotalMemory(false) / 1024.0 / 1024.0;
                GC.Collect();
                var afterGc = GC.GetTotalMemory(false) / 1024.0 / 1024.0;
                Debug.Print("{0,15} {1,15} msec {2,15} msec {3} / {4} MiB", N, sw1.ElapsedMilliseconds, sw2.ElapsedMilliseconds,
                    beforeGc,
                    afterGc);
            }
        }

        private Stopwatch Time(string str, Action<string> fn)
        {
            var sw = new Stopwatch();
            sw.Start();
            fn(str);
            sw.Stop();
            return sw;
        }

        [Test]
        public void Sufa_Abracadabra()
        {
            var sa = new SuffixArray("abracadabra");
            PrintSortedArray(sa);
            Debug.Print(sa.ToString());
            Assert.AreEqual(new[] { 0, 3, 5, 7, 10 }, sa.FindOccurences("a").OrderBy(i=>i).ToArray());
        }
    }
}

