/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Lib;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Decompiler.Structure
{
	/// <summary>
	/// Builder class that finds all the intervals of a procedure, resulting in an
	/// IntervalCollection.
	/// </summary>
	public class IntervalFinder
	{
		private IntervalCollection intervals;
		private Interval [] intervalOf;

		public IntervalFinder(Procedure proc)
		{
			ComputeIntervals(proc.EntryBlock, proc.RpoBlocks);
		}

		private void ComputeIntervals(Block entry, BlockList blocks)
		{
			intervals = new IntervalCollection(blocks.Count);
			intervalOf = new Interval[blocks.Count];

			Hashtable processed = new Hashtable();
			WorkList wlIntHeaders = new WorkList();		// Possible interval headers
			WorkList wlIntMembers = new WorkList();		// Known interval members.
			int [] linksFollowed = new int[blocks.Count];

			wlIntHeaders.Add(entry);
			while (!wlIntHeaders.IsEmpty)
			{
				Block h = (Block) wlIntHeaders.GetWorkItem();
				Interval interval = new Interval(h, blocks.Count);
				interval.AddBlock(h);
				intervalOf[h.RpoNumber] = interval;

				wlIntMembers.Add(h);
				intervals.Add(interval);

				while (!wlIntMembers.IsEmpty)
				{
					Block n = (Block) wlIntMembers.GetWorkItem();

					// n is known to be in the interval; see if the successors also are in the interval.

					foreach (Block s in n.Succ)
					{
						++linksFollowed[s.RpoNumber];		// followed another predecessor link.
						if (intervalOf[s.RpoNumber] == null)
						{
							if (linksFollowed[s.RpoNumber] == s.Pred.Count)
							{
								// This is the only node that reaches this interval, so
								// it has to be a member of the interval.
								// Therefore, we remove it from the list of potential headers
								wlIntHeaders.Remove(s);		

								// And add it to this interval.
								interval.AddBlock(s);
								wlIntMembers.Add(s);
								intervalOf[s.RpoNumber] = interval;
							}
							else if (!processed.ContainsKey(s))
							{
								processed[s] = s;

								// s may be reached by another node. Therefore, s is a possible new header.
								// Add it to the interval work list.

								wlIntHeaders.Add(s);
							}
						}
						else
						{
							if (linksFollowed[s.RpoNumber] == s.Pred.Count &&
								intervalOf[s.RpoNumber] == interval &&
								intervalOf[s.RpoNumber].Header == h)
							{
								if (s != h)  // Don't add the back edge to header again, since it's already in the interval.
								{
									interval.AddBlock(s);
									intervalOf[s.RpoNumber] = interval;
									wlIntHeaders.Remove(s);
									wlIntMembers.Add(s);
								}
							}
						}
					}
				}
			}
		}


		public Interval IntervalOf(Block b)
		{
			return intervalOf[b.RpoNumber];
		}

		public IntervalCollection Intervals
		{
			get { return intervals; }
		}

		public bool IsIntervalHeader(Block b)
		{
			return intervalOf[b.RpoNumber].Header == b;
		}
	}
}
