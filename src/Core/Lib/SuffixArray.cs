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
using System;
using System.Linq;
using System.Collections.Generic;

namespace Reko.Core.Lib
{
    public class SuffixArray
    {
        private const int EOC = int.MaxValue;
        private int[] m_sa;
        private int[] m_isa;
        private int[] m_lcp;
        private Dictionary<char, int> m_chainHeadsDict = new Dictionary<char, int>();
        private List<Chain> m_chainStack = new List<Chain>();
        private List<Chain> m_subChains = new List<Chain>();
        private int m_nextRank = 1;
        private string m_str;

        /// <summary>
        /// Build a suffix array from string str
        /// </summary>
        /// <param name="str">A string for which to build a suffix array with LCP information</param>
        /// <param name="buildLcps">Also build LCP array</param>
        public SuffixArray(string str) : this(str, true) { }

        /// 
        /// <summary>
        /// Build a suffix array from string str
        /// </summary>
        /// <param name="str">A string for which to build a suffix array</param>
        /// <param name="buildLcps">Also calculate LCP information</param>
        public SuffixArray(string str, bool buildLcps)
        {
            m_str = str;
            if (m_str == null)
            {
                m_str = "";
            }
            m_sa = new int[m_str.Length];
            m_isa = new int[m_str.Length];

            FormInitialChains();
            BuildSuffixArray();
            m_chainHeadsDict = null;  // free the mem.
            if (buildLcps)
                BuildLcpArray();
        }

        public int Length
        {
            get { return m_sa.Length; }
        }

        public int this[int index]
        {
            get { return m_sa[index]; }
        }

        /// <summary>
        /// Longest common prefix array
        /// </summary>
        public int[] Lcp
        {
            get { return m_lcp; }
        }

        public string Str
        {
            get { return m_str; }
        }

        /// 
        /// <summary>Find the index of a substring </summary>
        /// <param name="substr">Substring to look for</param>
        /// <returns>First index in the original string. -1 if not found</returns>
        public int IndexOf(string substr)
        {
            int l = 0;
            int r = m_sa.Length;
            int m = -1;

            if ((substr == null) || (substr.Length == 0))
            {
                return -1;
            }

            // Binary search for substring
            while (r > l)
            {
                m = (l + r) / 2;
                if (m_str.Substring(m_sa[m]).CompareTo(substr) < 0)
                {
                    l = m + 1;
                }
                else
                {
                    r = m;
                }
            }
            if ((l == r) && (l < m_str.Length) && (m_str.Substring(m_sa[l]).StartsWith(substr)))
            {
                return m_sa[l];
            }
            else
            {
                return -1;
            }
        }

        public IEnumerable<int> FindOccurences(string substr)
        {
            int lo = 0;
            int hi = m_sa.Length - 1;
            int m = -1;

            if (string.IsNullOrEmpty(substr))
            {
                yield break;
            }


            int iEnd = -1;
            // Binary search for substring, find the last one.
            while (lo <= hi)
            {
                m = lo + (hi - lo) / 2;
                int cmp = Match(m_sa[m], substr);
                if (cmp > 0)
                {
                    hi = m - 1;
                }
                else if (cmp < 0)
                {
                    lo = m + 1;
                }
                else
                {
                    iEnd = m;
                    lo = m + 1;
                }
            }
            if (lo > hi && 0 <= iEnd && iEnd < m_str.Length)
            {
                do
                {
                    yield return m_sa[iEnd];
                    --iEnd;
                } while (iEnd >= 0 && Match(m_sa[iEnd], substr) == 0);
            }
            else
            {
                yield break;
            }
        }

        int Match(int iText, string substr)
        {
            for (int i = 0; i < substr.Length; ++i)
            {
                if (iText >= m_str.Length)
                    return -1;
                int cmp = m_str[iText].CompareTo(substr[i]);
                if (cmp != 0)
                    return cmp;
                ++iText;
            }
            return 0;
        }

        private void FormInitialChains()
        {
            // Link all suffixes that have the same first character
            FindInitialChains();
            SortAndPushSubchains();
        }

        private void FindInitialChains()
        {
            // Scan the string left to right, keeping rightmost occurences of characters as the chain heads
            for (int i = 0; i < m_str.Length; i++)
            {
                if (m_chainHeadsDict.ContainsKey(m_str[i]))
                {
                    m_isa[i] = m_chainHeadsDict[m_str[i]];
                }
                else
                {
                    m_isa[i] = EOC;
                }
                m_chainHeadsDict[m_str[i]] = i;
            }

            // Prepare chains to be pushed to stack
            foreach (int headIndex in m_chainHeadsDict.Values)
            {
                Chain newChain = new Chain(m_str);
                newChain.head = headIndex;
                newChain.length = 1;
                m_subChains.Add(newChain);
            }
        }

        private void SortAndPushSubchains()
        {
            m_subChains.Sort();
            for (int i = m_subChains.Count - 1; i >= 0; i--)
            {
                m_chainStack.Add(m_subChains[i]);
            }
        }

        private void BuildSuffixArray()
        {
            while (m_chainStack.Count > 0)
            {
                // Pop chain
                Chain chain = m_chainStack[m_chainStack.Count - 1];
                m_chainStack.RemoveAt(m_chainStack.Count - 1);

                if (m_isa[chain.head] == EOC)
                {
                    // Singleton (A chain that contain only 1 suffix)
                    RankSuffix(chain.head);
                }
                else
                {
                    //RefineChains(chain);
                    RefineChainWithInductionSorting(chain);
                }
            }
        }

        private void RefineChains(Chain chain)
        {
            m_chainHeadsDict.Clear();
            m_subChains.Clear();
            while (chain.head != EOC)
            {
                int nextIndex = m_isa[chain.head];
                if (chain.head + chain.length > m_str.Length - 1)
                {
                    RankSuffix(chain.head);
                }
                else
                {
                    ExtendChain(chain);
                }
                chain.head = nextIndex;
            }
            // Keep stack sorted
            SortAndPushSubchains();
        }

        private void ExtendChain(Chain chain)
        {
            char sym = m_str[chain.head + chain.length];
            if (m_chainHeadsDict.ContainsKey(sym))
            {
                // Continuation of an existing chain, this is the leftmost
                // occurence currently known (others may come up later)
                m_isa[m_chainHeadsDict[sym]] = chain.head;
                m_isa[chain.head] = EOC;
            }
            else
            {
                // This is the beginning of a new subchain
                m_isa[chain.head] = EOC;
                Chain newChain = new Chain(m_str);
                newChain.head = chain.head;
                newChain.length = chain.length + 1;
                m_subChains.Add(newChain);
            }
            // Save index in case we find a continuation of this chain
            m_chainHeadsDict[sym] = chain.head;
        }

        private void RefineChainWithInductionSorting(Chain chain)
        {
            // TODO - refactor/beautify some
            List<SuffixRank> notedSuffixes = new List<SuffixRank>();
            m_chainHeadsDict.Clear();
            m_subChains.Clear();

            while (chain.head != EOC)
            {
                int nextIndex = m_isa[chain.head];
                if (chain.head + chain.length > m_str.Length - 1)
                {
                    // If this substring reaches end of string it cannot be extended.
                    // At this point it's the first in lexicographic order so it's safe
                    // to just go ahead and rank it.
                    RankSuffix(chain.head);
                }
                else if (m_isa[chain.head + chain.length] < 0)
                {
                    SuffixRank sr = new SuffixRank();
                    sr.head = chain.head;
                    sr.rank = -m_isa[chain.head + chain.length];
                    notedSuffixes.Add(sr);
                }
                else
                {
                    ExtendChain(chain);
                }
                chain.head = nextIndex;
            }
            // Keep stack sorted
            SortAndPushSubchains();
            SortAndRankNotedSuffixes(notedSuffixes);
        }

        private void SortAndRankNotedSuffixes(List<SuffixRank> notedSuffixes)
        {
            notedSuffixes.Sort(new SuffixRankComparer());
            // Rank sorted noted suffixes 
            for (int i = 0; i < notedSuffixes.Count; ++i)
            {
                RankSuffix(notedSuffixes[i].head);
            }
        }

        private void RankSuffix(int index)
        {
            // We use the ISA to hold both ranks and chain links, so we differentiate by setting
            // the sign.
            m_isa[index] = -m_nextRank;
            m_sa[m_nextRank - 1] = index;
            m_nextRank++;
        }

        private void BuildLcpArray()
        {
            m_lcp = new int[m_sa.Length + 1];
            m_lcp[0] = m_lcp[m_sa.Length] = 0;

            for (int i = 1; i < m_sa.Length; i++)
            {
                m_lcp[i] = CalcLcp(m_sa[i - 1], m_sa[i]);
            }
        }

        private int CalcLcp(int i, int j)
        {
            int lcp;
            int maxIndex = m_str.Length - Math.Max(i, j);       // Out of bounds prevention
            for (lcp = 0; (lcp < maxIndex) && (m_str[i + lcp] == m_str[j + lcp]); lcp++) ;
            return lcp;
        }

        public override string ToString()
        {
            return string.Format("[{0}]", string.Join(", ", m_sa.Take(80)));
        }

    }

    #region HelperClasses
    [Serializable]
    internal class Chain : IComparable<Chain>
    {
        public int head;
        public int length;
        private string m_str;

        public Chain(string str)
        {
            m_str = str;
        }

        public int CompareTo(Chain other)
        {
            return m_str.Substring(head, length).CompareTo(m_str.Substring(other.head, other.length));
        }

        public override string ToString()
        {
            return m_str.Substring(head, length);
        }
    }

    [Serializable]
    internal class CharComparer : System.Collections.Generic.EqualityComparer<char>
    {
        public override bool Equals(char x, char y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(char obj)
        {
            return obj.GetHashCode();
        }
    }

    [Serializable]
    internal struct SuffixRank
    {
        public int head;
        public int rank;
    }

    [Serializable]
    internal class SuffixRankComparer : IComparer<SuffixRank>
    {
        public bool Equals(SuffixRank x, SuffixRank y)
        {
            return x.rank.Equals(y.rank);
        }

        public int Compare(SuffixRank x, SuffixRank y)
        {
            return x.rank.CompareTo(y.rank);
        }
    }
    #endregion
}

