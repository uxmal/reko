using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Lib
{
    public class SuffixArray2
    {

        // Structure to store information of a suffix 
        struct suffix
        {
            public int index;  // Original index 
            public short rank0;     // Rank
            public short rank1;     // Next rank
        }

        // A comparison function used by sort() to compare two suffixes 
        // Compares two pairs, returns 1 if first pair is smaller 
        static int cmp(suffix a, suffix b)
        {
            return (a.rank0 == b.rank0)
                        ? (a.rank1 - b.rank1)
                        : (a.rank0 - b.rank0);
        }

        // This is the main function that takes a string 'txt' of size n as an 
        // argument, builds and return the suffix array for the given string 
        static int[] buildSuffixArray(byte[] txt)
        {
            int n = txt.Length;
            // A structure to store suffixes and their indexes 
            suffix[] suffixes = new suffix[n];
            var sw = new Stopwatch();
            sw.Start();
            // Store suffixes and their indexes in an array of structures. 
            // The structure is needed to sort the suffixes alphabatically 
            // and maintain their old indexes while sorting 
            for (int i = 0; i < n; i++)
            {
                suffixes[i].index = i;
                suffixes[i].rank0 = (short) txt[i];
                suffixes[i].rank1 = ((i + 1) < n) ? (short) txt[i + 1] : (short) -1;
            }
            var initSuffixes = sw.ElapsedMilliseconds;

            // Sort the suffixes using the comparison function 
            // defined above. 
            Array.Sort(suffixes, cmp);

            var firstSort = sw.ElapsedMilliseconds;

            int iter = 0;
            // At this point, all suffixes are sorted according to first 
            // 2 characters.  Let us sort suffixes according to first 4 
            // characters, then first 8 and so on 
            int[] ind = new int[n];  // This array is needed to get the index in suffixes[] 
                                     // from original index.  This mapping is needed to get 
                                     // next suffix. 
            for (int k = 4; k < 2 * n; k = k * 2)
            {
                // Assigning rank and index values to first suffix 
                short rank = 0;
                int prev_rank = suffixes[0].rank0;
                suffixes[0].rank0 = rank;
                ind[suffixes[0].index] = 0;

                // Assigning rank to suffixes 
                for (int i = 1; i < n; i++)
                {
                    // If first rank and next ranks are same as that of previous 
                    // suffix in array, assign the same new rank to this suffix 
                    if (suffixes[i].rank0 == prev_rank &&
                        suffixes[i].rank1 == suffixes[i - 1].rank1)
                    {
                        prev_rank = suffixes[i].rank0;
                        suffixes[i].rank0 = rank;
                    }
                    else // Otherwise increment rank and assign 
                    {
                        prev_rank = suffixes[i].rank0;
                        suffixes[i].rank0 = ++rank;
                    }
                    ind[suffixes[i].index] = i;
                }

                // Assign next rank to every suffix 
                for (int i = 0; i < n; i++)
                {
                    int nextindex = suffixes[i].index + k / 2;
                    suffixes[i].rank1 = (nextindex < n)
                        ? suffixes[ind[nextindex]].rank0
                        : (short) -1;
                }

                // Sort the suffixes according to first k characters 
                Array.Sort(suffixes, cmp);
                ++iter;
            }

            var doneLooping = sw.ElapsedMilliseconds;

            // Store indexes of all sorted suffixes in the suffix array 
            int[] suffixArr = new int[n];
            for (int i = 0; i < n; i++)
                suffixArr[i] = suffixes[i].index;

            // Return the suffix array 
            Console.WriteLine("Iterations: {0} ({1},{2},{3}) ", iter, initSuffixes, firstSort, doneLooping);
            return suffixArr;
        }

        /* To construct and return LCP */
        static int[] kasai(byte[] txt, int[] suffixArr)
        {
            int n = suffixArr.Length;

            // To store LCP array 
            int[] lcp = new int[n];

            // An auxiliary array to store inverse of suffix array 
            // elements. For example if suffixArr[0] is 5, the 
            // invSuff[5] would store 0.  This is used to get next 
            // suffix string from suffix array. 
            int[] invSuff = new int[n];

            // Fill values in invSuff[] 
            for (int i = 0; i < n; i++)
                invSuff[suffixArr[i]] = i;

            // Initialize length of previous LCP 
            int k = 0;

            // Process all suffixes one by one starting from 
            // first suffix in txt[] 
            for (int i = 0; i < n; i++)
            {
                /* If the current suffix is at n-1, then we don’t 
                   have next substring to consider. So lcp is not 
                   defined for this substring, we put zero. */
                if (invSuff[i] == n - 1)
                {
                    k = 0;
                    continue;
                }

                /* j contains index of the next substring to 
                   be considered  to compare with the present 
                   substring, i.e., next string in suffix array */
                int j = suffixArr[invSuff[i] + 1];

                // Directly start matching from k'th index as 
                // at-least k-1 characters will match 
                while (i + k < n && j + k < n && txt[i + k] == txt[j + k])
                    k++;

                lcp[invSuff[i]] = k; // lcp for the present suffix. 

                // Deleting the starting character from the string. 
                if (k > 0)
                    k--;
            }

            // return the constructed lcp array 
            return lcp;
        }

        // Utility function to print an array 
        static void printArr(int[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
                Console.Write("{0} ", arr[i], " ");
            Console.WriteLine();
        }

        // Driver program 
        int mane()
        {
            byte[] str = Encoding.ASCII.GetBytes("banana");

            var suffixArr = buildSuffixArray(str);
            int n = suffixArr.Length;

            Console.WriteLine("Suffix Array : ");
            printArr(suffixArr);

            var lcp = kasai(str, suffixArr);

            Console.WriteLine();
            Console.WriteLine("LCP Array : ");
            printArr(lcp);
            return 0;
        }

        public static SuffixArray2 Create(byte[] a)
        {
            var sa = buildSuffixArray(a);
            var lcp = kasai(a,sa);
            return new SuffixArray2(sa, lcp);
        }

        public SuffixArray2(int[] sa, int[] lcp)
        {
            this.SuffixArray = sa;
            this.Lcp = lcp;
        }

        public int[] Lcp { get; }

        public int [] SuffixArray { get; }
    }
}
