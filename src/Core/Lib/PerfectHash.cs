using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#nullable disable
#pragma warning disable CS1591

namespace Reko.Core.Lib
{
    /** Perfect hashing function library. Contains functions to generate perfect
        hashing functions */
    public class PerfectHash
    {
        ushort[] T1base;
        ushort[] T2base;   /* Pointers to start of T1, T2 */
        short[] g;         /* g[] */

        int NumEntry;   /* Number of entries in the hash table (# keys) */
        int EntryLen;   /* Size (bytes) of each entry (size of keys) */
        int SetSize;    /* Size of the char set */
        char SetMin;     /* First char in the set */
        int NumVert;    /* c times NumEntry */
                        /** Set the parameters for the hash table */

        //public void assign(); /* Part 2 of creating the tables */
        //public int hash(string @string); /* Hash the string to an int 0 .. NUMENTRY-1 */
        public ushort[] readT1() { return T1base; }
        public ushort[] readT2() { return T2base; }
        public short[] readG() { return g; }

        PatternCollector m_collector; /* used to retrieve the keys */
/*
 *$Log: perfhlib.c,v $
 * Revision 1.5  93/09/29  14:45:02  emmerik
 * Oops, didn't do the casts last check in
 *
 * Revision 1.4  93/09/29  14:41:45  emmerik
 * Added casts to mod instructions to keep the SVR4 compiler happy
 *
 *
 * Perfect hashing function library. Contains functions to generate perfect
 * hashing functions
 */
        /* Private data structures */

        //static  int     NumEntry;   /* Number of entries in the hash table (# keys) */
        //static  int     EntryLen;   /* Size (bytes) of each entry (size of keys) */
        //static  int     SetSize;    /* Size of the char set */
        //static  char    SetMin;     /* First char in the set */
        //static  int     NumVert;    /* c times NumEntry */

        //static  uint16_t    *T1base, *T2base;   /* Pointers to start of T1, T2 */
        //static ushort[] T1, T2;   /* Pointers to T1[i], T2[i] */

        static int[] graphNode; /* The array of edges */
        static int[] graphNext; /* Linked list of edges */
        static int[] graphFirst;/* First edge at a vertex */


        static int numEdges;   /* An edge counter */
        static bool[] visited;   /* Array of bools: whether visited */
        static bool[] deleted;   /* Array of bools: whether deleted */

        /* Private prototypes */

        public void setHashParams(
            int _NumEntry, int _EntryLen, int _SetSize, char _SetMin,
            int _NumVert)
        {
            /* These parameters are stored in statics so as to obviate the need for
                passing all these (or defererencing pointers) for every call to hash()
            */

            NumEntry = _NumEntry;
            EntryLen = _EntryLen;
            SetSize = _SetSize;
            SetMin = _SetMin;
            NumVert = _NumVert;

            /* Allocate the variable sized tables etc */
            T1base = new ushort[EntryLen * SetSize];
            T2base = new ushort[EntryLen * SetSize];
            graphNode = new int[NumEntry * 2 + 1];
            graphNext = new int[NumEntry * 2 + 1];
            graphFirst = new int[NumVert + 1];
            g = new short[NumVert + 1];
            visited = new bool[NumVert + 1];
            deleted = new bool[NumEntry + 1];
        }

        /* Part 1 of creating the tables */
        public void map(PatternCollector collector)
        {
            m_collector = collector;
            Debug.Assert(null != collector);
            int i, j, c;
            ushort f1, f2;
            bool cycle;
            byte[] keys;

            c = 0;
            var rnd = new Random();
            do
            {
                initGraph();
                cycle = false;

                /* Randomly generate T1 and T2 */
                for (i = 0; i < SetSize * EntryLen; i++)
                {
                    T1base[i] = (ushort)rnd.Next(NumVert);
                    T2base[i] = (ushort)rnd.Next(NumVert);
                }

                for (i = 0; i < NumEntry; i++)
                {
                    f1 = 0; f2 = 0;
                    keys = m_collector.getKey(i);
                    for (j = 0; j < EntryLen; j++)
                    {
                        f1 += T1base[ j * SetSize +(keys[j] - SetMin)];
                        f2 += T2base[ j * SetSize + (keys[j] - SetMin)];
                    }
                    f1 %= (ushort)NumVert;
                    f2 %= (ushort)NumVert;
                    if (f1 == f2)
                    {
                        /* A self loop. Reject! */
                        Debug.Print("Self loop on vertex %d!\n", f1);
                        cycle = true;
                        break;
                    }
                    addToGraph(numEdges++, f1, f2);
                }
                if (cycle || (cycle = isCycle()))   /* OK - is there a cycle? */
                {
                    Debug.Print("Iteration {0}", ++c);
                }
                else
                {
                    break;
                }
            }
            while (/* there is a cycle */ true);

        }

        /* Initialise the graph */
        void initGraph()
        {
            int i;

            for (i = 1; i <= NumVert; i++)
            {
                graphFirst[i] = 0;
            }

            for (i = -NumEntry; i <= NumEntry; i++)
            {
                /* No need to init graphNode[] as they will all be filled by successive
                    calls to addToGraph() */
                graphNext[NumEntry + i] = 0;
            }

            numEdges = 0;
        }

        /* Add an edge e between vertices v1 and v2 */
        /* e, v1, v2 are 0 based */
        void addToGraph(int e, int v1, int v2)
        {
            e++; v1++; v2++;                         /* So much more convenient */

            graphNode[NumEntry + e] = v2;             /* Insert the edge information */
            graphNode[NumEntry - e] = v1;

            graphNext[NumEntry + e] = graphFirst[v1]; /* Insert v1 to list of alphas */
            graphFirst[v1] = e;
            graphNext[NumEntry - e] = graphFirst[v2]; /* Insert v2 to list of omegas */
            graphFirst[v2] = -e;
        }

        bool DFS(int parentE, int v)
        {
            int e, w;

            // Depth first search of the graph, starting at vertex v, looking for
            // cycles. parent and v are origin 1. Note parent is an EDGE,
            // not a vertex

            visited[v] = true;

            // For each e incident with v ..
            for (e = graphFirst[v]; e!=0; e = graphNext[NumEntry + e])
            {
                byte[] key1;

                if (deleted[Math.Abs(e)])
                {
                    /* A deleted key. Just ignore it */
                    continue;
                }
                key1 = m_collector.getKey(Math.Abs(e) - 1);
                w = graphNode[NumEntry + e];
                if (visited[w])
                {
                    /* Did we just come through this edge? If so, ignore it. */
                    if (Math.Abs(e) != Math.Abs(parentE))
                    {
                        /* There is a cycle in the graph. There is some subtle code here
                            to work around the distinct possibility that there may be
                            duplicate keys. Duplicate keys will always cause unit
                            cycles, since f1 and f2 (used to select v and w) will be the
                            same for both. The edges (representing an index into the
                            array of keys) are distinct, but the key values are not.
                            The logic is as follows: for the candidate edge e, check to
                            see if it terminates in the parent vertex. If so, we test
                            the keys associated with e and the parent, and if they are
                            the same, we can safely ignore e for the purposes of cycle
                            detection, since edge e adds nothing to the cycle. Cycles
                            involving v, w, and e0 will still be found. The parent
                            edge was not similarly eliminated because at the time when
                            it was a candidate, v was not yet visited.
                            We still have to remove the key from further consideration,
                            since each edge is visited twice, but with a different
                            parent edge each time.
                        */
                        /* We save some stack space by calculating the parent vertex
                            for these relatively few cases where it is needed */
                        int parentV = graphNode[NumEntry - parentE];

                        if (w == parentV)
                        {
                            byte[] key2;

                            key2 = m_collector.getKey(Math.Abs(parentE) - 1);
                            if (ByteMemoryArea.CompareArrays(key1, 0, key2, EntryLen))
                            {
                                Debug.Print("Duplicate keys with edges {0} and {1} (",
                                       e, parentE);
                                m_collector.dispKey(Math.Abs(e) - 1);
                                Debug.Print(" & ");
                                m_collector.dispKey(Math.Abs(parentE) - 1);
                                Debug.Print(")\n");
                                deleted[Math.Abs(e)] = true;      /* Wipe the key */
                            }
                            else
                            {
                                /* A genuine (unit) cycle. */
                                Debug.Print("There is a unit cycle involving vertex %d and edge %d\n", v, e);
                                return true;
                            }
                        }
                        else
                        {
                            // We have reached a previously visited vertex not the
                            // parent. Therefore, we have uncovered a genuine cycle
                            Debug.Print("There is a cycle involving vertex {0} and edge {1}", v, e);
                            return true;
                        }
                    }
                }
                else                                // Not yet seen. Traverse it 
                {
                    if (DFS(e, w))
                    {
                        // Cycle found deeper down. Exit
                        return true;
                    }
                }
            }
            return false;
        }

        bool isCycle()
        {
            int v, e;

            for (v = 1; v <= NumVert; v++)
            {
                visited[v] = false;
            }
            for (e = 1; e <= NumEntry; e++)
            {
                deleted[e] = false;
            }
            for (v = 1; v <= NumVert; v++)
            {
                if (!visited[v])
                {
                    if (DFS(-32767, v))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void traverse(int u)
        {
            int w, e;

            visited[u] = true;
            // Find w, the neighbours of u, by searching the edges e associated with u
            e = graphFirst[1 + u];
            while (e != 0)
            {
                w = graphNode[NumEntry + e] - 1;
                if (! visited[w])
                {
                    g[w] = (short)((Math.Abs(e) - 1 - g[u]) % NumEntry);
                    if (g[w] < 0)
                        g[w] += (short) NumEntry;     // Keep these positive
                    traverse(w);
                }
                e = graphNext[NumEntry + e];
            }
        }

        void assign()
        {
            int v;
            for (v = 0; v < NumVert; v++)
            {
                g[v] = 0;                           // g is sparse; leave the gaps 0
                visited[v] = false;
            }

            for (v = 0; v < NumVert; v++)
            {
                if (!visited[v])
                {
                    g[v] = 0;
                    traverse(v);
                }
            }
        }

        public int hash(byte[] @string)
        {
            ushort u, v;
            int j;

            u = 0;
            for (j = 0; j < EntryLen; j++)
            {
                //T1 = T1base + j * SetSize;
                //u += T1[@string[j] - SetMin];
                u += T1base[j * SetSize + (@string[j] - SetMin)];
            }
            u = (ushort)(u % NumVert);

            v = 0;
            for (j = 0; j < EntryLen; j++)
            {
                //T2 = T2base + j * SetSize;
                //v += T2[@string[j] - SetMin];
                v += T2base[j * SetSize + (@string[j] - SetMin)];
            }
            v = (ushort)(v % NumVert);

            return (g[u] + g[v]) % NumEntry;
        }
    }
}
