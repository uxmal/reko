using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Scanning
{
    public class CommonPatternFinder
    {

        /*
         * 

This is as task similar to Nussinov algorithm and actually even simpler as we do not allow any gaps, insertions or mismatches in the alignment.

For the string A having the length N, define a F[-1 .. N, -1 .. N] table and fill in using the following rules:

  for i = 0 to N
    for j = 0 to N
      if i != j
        {
          if A[i] == A[j]
            F[i,j] = F [i-1,j-1] + 1;
          else
            F[i,j] = 0;
        }

For instance, for B A O B A B:

AlgChart

This runs in O(n^2) time. The largest values in the table now point to the end positions of the longest self-matching subquences (i - the end of one occurence, j - another). In the beginning, the array is assumed to be zero-initialized. I have added condition to exclude the diagonal that is the longest but probably not interesting self-match.

Thinking more, this table is symmetric over diagonal so it is enough to compute only half of it. Also, the array is zero initialized so assigning zero is redundant. That remains

  for i = 0 to N
    for j = i + 1 to N
      if A[i] == A[j]
         F[i,j] = F [i-1,j-1] + 1;

Shorter but potentially more difficult to understand. The computed table contains all matches, short and long. You can add further filtering as you need.

On the next step, you need to recover strings, following from the non zero cells up and left by diagonal. During this step is also trivial to use some hashmap to count the number of self-similarity matches for the same string. With normal string and normal minimal length only small number of table cells will be processed through this map.

I think that using hashmap directly actually requires O(n^3) as the key strings at the end of access must be compared somehow for equality. This comparison is probably O(n).
*/
    }
}
