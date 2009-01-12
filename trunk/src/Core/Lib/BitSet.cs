/* 
 * Copyright (C) 1999-2009 John Källén.
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

using System;
using IEnumerable = System.Collections.IEnumerable;
using IEnumerator = System.Collections.IEnumerator;

namespace Decompiler.Core.Lib
{
	/// <summary>
	/// Represents a set of bits. The System.Collection.BitArray class has bugs, so we circumvent them here.
	/// </summary>
	public class BitSet : ICloneable, IEnumerable
	{
		private int [] bits;
		private int bitMax;
		private const int BitsPerInt = 32;

		public BitSet(int length)
		{
			bits = new int[(length + BitsPerInt - 1) / BitsPerInt];
			bitMax = length;
		}

		public BitSet(BitSet b)
		{
			bits = (int []) b.bits.Clone();
			bitMax = b.bitMax;
		}

		public bool this[int i]
		{
			get { return (bits[i / BitsPerInt] & (1 << (i % BitsPerInt))) != 0; }
			set 
			{
				if (value)
				{
					bits[i / BitsPerInt] |= (1 << (i % BitsPerInt));
				}
				else
				{
					bits[i / BitsPerInt] &= ~(1 << (i % BitsPerInt));
				}
			}
		}

		public static BitSet operator & (BitSet a, BitSet b)
		{
			if (a.Count != b.Count)
				throw new ArgumentException("Bitsets must have equal lengths");
			BitSet result = new BitSet(a.Count);
			for (int i = 0; i < a.bits.Length; ++i)
			{
				result.bits[i] = a.bits[i] & b.bits[i];				
			}
			return result;
		}

		public static BitSet operator | (BitSet a, BitSet b)
		{
			if (a.Count != b.Count)
				throw new ArgumentException("Bitsets must have equal lengths");
			BitSet result = new BitSet(a.Count);
			for (int i = 0; i < a.bits.Length; ++i)
			{
				result.bits[i] = a.bits[i] | b.bits[i];				
			}
			return result;
		}

		/// <summary>
		/// Computes the set difference a \ b.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static BitSet operator - (BitSet a, BitSet b)
		{
			if (a.Count != b.Count)
				throw new ArgumentException("Bitsets must have equal lengths");
			BitSet result = new BitSet(a.Count);
			for (int i = 0; i < a.bits.Length; ++i)
			{
				result.bits[i] = a.bits[i] & ~b.bits[i];				
			}
			return result;
		}

		public static BitSet operator ~ (BitSet a)
		{
			BitSet result = new BitSet(a.Count);
			for (int i = 0; i < a.bits.Length; ++i)
			{
				result.bits[i] = ~a.bits[i];
			}
			return result;
		}

		public int Count
		{
			get { return this.bitMax; }
		}

		public bool IsEmpty
		{
			get 
			{
				int tot = 0;
				foreach (int grb in bits)
				{
					tot |= grb;
				}
				return tot == 0;
			}
		}

		public IEnumerable Reverse
		{
			get { return new ReverseEnumerator(this); }
		}

		public void SetAll(bool v)
		{
			int val = v ? -1 : 0;
			for (int i = 0; i < bits.Length; ++i)
			{
				bits[i] = val;
			}
		}

		public override string ToString()
		{
			char [] rep = new char[bitMax];
			int i = 0;
			int c = bitMax;
			foreach (int grb in bits)
			{
				for (int m = 1; m != 0; m <<= 1)			// $REVIEW; perhaps too clever? Depends on int == int32.
				{
					rep[i++] = ((grb & m) != 0) ? '1' : '0';
					if (--c == 0)
						return new string(rep);
				}
			}
			return new string(rep);
		}

		public BitSet Clone()
		{
			return new BitSet(this);
		}

		#region ICloneable Members

		object ICloneable.Clone()
		{
			return new BitSet(this);
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return new BitSetEnumerator(this);
		}

		#endregion

		private class BitSetEnumerator : IEnumerator
		{
			private BitSet bitset;
			private int bit;

			public BitSetEnumerator(BitSet bitset)
			{
				this.bitset = bitset;
				Reset();
			}
			#region IEnumerator Members

			public void Reset()
			{
				bit = -1;
			}

			public object Current
			{
				get
				{
					if (bit < 0)
						throw new InvalidOperationException("Must call MoveNext first.");
					if (bit >= bitset.bitMax)
						throw new InvalidOperationException("Enumerator at end of bitset");
					return bit;
				}
			}

			public bool MoveNext()
			{
				for (;;)
				{
					++bit;
					if (bit >= bitset.bitMax)
						return false;
					if (bitset[bit])
						return true;
				}			
			}

			#endregion
		}

		private class ReverseEnumerator : IEnumerator, IEnumerable
		{
			private BitSet bitset;
			private int bit;
			private int bitMax;

			public ReverseEnumerator(BitSet bitset)
			{
				this.bitset = bitset;
				this.bitMax = bitset.bitMax;
				this.bit = bitMax;
			}

			#region IEnumerator Members

			public void Reset()
			{
				bit = bitset.bits.Length * BitSet.BitsPerInt;
			}

			public object Current
			{
				get
				{
					if (bit >= bitMax)
						throw new InvalidOperationException("Must call MoveNext first.");
					if (bit < 0)
						throw new InvalidOperationException("Enumerator at end of bitset");
					return bit;
				}
			}

			public bool MoveNext()
			{
				for (;;)
				{
					--bit;
					if (bit < 0)
						return false;
					if (bitset[bit])
						return true;
				}
			}

			#endregion

			#region IEnumerable Members

			public IEnumerator GetEnumerator()
			{
				return this;
			}

			#endregion
		}	
	}
}
