#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

#nullable disable

using Reko.Core;
using Reko.Core.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using BitArray = System.Collections.BitArray;

namespace Reko.Scanning
{
	internal class RegexpBuilder
	{
		private enum Token
		{
			End,
			Byte,
			Star,
			Plus,
			LParen,
			RParen,
			LBracket,
			RBracket,
			Or,
			Dot,
			Cut
		}

		private readonly string pattern;
		private readonly StringReader rdr;
		private readonly int [] alphabet;		// maps bytes to character classes.
		private readonly List<Node> positions;
		private byte b;
		private Token tok;
		private int charClasses;
		private bool fullTable;

		private const int MaxAlphabet = 256;

		public RegexpBuilder(string s)
		{
			positions = new List<Node>();

			// Initially, all characters belong to the character class 0.

			alphabet = new int[MaxAlphabet];
			for (int i = 0; i != MaxAlphabet; ++i)
			{
				alphabet[i] = 0;
			}
			charClasses = 1;

			pattern = s;
			rdr = new StringReader(s);
			tok = Token.End;
		}

		private void AddCharacterClass(int b)
		{
			if (alphabet[b] == 0)
			{
				alphabet[b] = charClasses;
				++charClasses;
			}
		}

		/// <summary>
		/// If false, each scanned byte will first be looked up 
		/// in an equivalence class, before performing the transition lookup.
		/// If true, the scanned bytes are used directly for the transition
		/// lookup.
		/// </summary>
		public bool FullTable
		{
			get { return fullTable; }
			set { fullTable = value; }
		}

		public Regexp Build()
		{
			Node n = Parse();
			Node acc = new AcceptNode(positions.Count);
			positions.Add(acc);
			n = new SequenceNode(n, acc);

			n.ComputeNullable();
			n.ComputeFirstPos(this);
			n.ComputeLastPos(this);
			n.ComputeFollowPos(this);

#if DEBUG
			var sb = new StringBuilder();
			n.Dump(sb);
			Debug.WriteLine(sb.ToString());
#endif
			State [] states = BuildDfaTable(n);
			var tc = new TableCompacter(states, charClasses);
			tc.Compact();
			return new Regexp(alphabet, tc.ReStates, tc.Next, tc.Check);
		}

		private State [] BuildDfaTable(Node n)
		{
			var dStates = new List<State>();

			// Create the default, error state.

			var err = new State(new BitArray(n.FirstPos.Length), charClasses);
			AddState(dStates, err);

			// Create the initial state.

			var s0 = new State(n.FirstPos, charClasses);
			AddState(dStates, s0);

			// Start the worklist.

            var worklist = new WorkList<State>();
			worklist.Add(s0);
			while (worklist.TryGetWorkItem(out State t))
			{
				Debug.WriteLine(t.ToString());
				for (int a = 0; a != charClasses; ++a)
				{
					// Create U, a state consisting of the positions in 
					// FollowPos(p) where p is any position in t that has 
					// an 'a'.

					var u = new State(new BitArray(positions.Count), charClasses);
					for (int p = 0; p != t.Positions.Length; ++p)
					{
						if (!t.Positions[p])
							continue;
						ByteNode pp = (ByteNode) positions[p];
						if (pp.Any || alphabet[pp.startByte] == a)
						{
							u.Positions.Or(pp.FollowPos);
						}
						t.Accepts |= pp.Accepts;
					}

					if (IsEmptySet(u.Positions))
					{
						u = null;
					}
					else
					{
						State uu = FindState(dStates, u.Positions);
						if (uu is null)
						{
							AddState(dStates, u);
							worklist.Add(u);
						}
						else
						{
							u = uu;
						}
					}
					t.NextState[a] = u;
				}
				Debug.WriteLine("t complete: " + t);
			}
            return dStates.ToArray(); 
		}

		private static void AddState(List<State> dStates, State s)
		{
			s.Index = dStates.Count;
			dStates.Add(s);
			Debug.WriteLine("add state " + s);
		}

		private static State FindState(List<State> dStates, BitArray state)
		{
			foreach (State s in dStates)
			{
				if (IsEqualSet(s.Positions, state))
					return s;
			}
			return null;
		}

		public static string DumpSet(BitArray ba)
		{
			var sb = new StringBuilder();
			sb.Append('{');
			for (int i = 0; i < ba.Length; ++i)
			{
				if (ba.Get(i))
				{
					sb.Append(' ');
					sb.Append(i);
				}
			}
			sb.Append(" }");
			return sb.ToString();
		}

		public static bool IsEqualSet(BitArray s1, BitArray s2)
		{
			int i;
			for (i = 0; i < s1.Length && i < s2.Length; ++i)
			{
				if (s1.Get(i) != s2.Get(i))
					return false;
			}
			for (; i < s1.Length; ++i)
			{
				if (s1.Get(i))
					return false;
			}
			for (; i < s2.Length; ++i)
			{
				if (s2.Get(i))
					return false;
			}
			return true;
		}

		public static bool IsEmptySet(BitArray b)
		{
			for (int i = 0; i != b.Length; ++i)
			{
				if (b.Get(i))
					return false;
			}
			return true;
		}
			
		private void Expect(Token t)
		{
			if (GetToken() != t)
				SyntaxError();
		}
			

		private int HexDigit(int ch)
		{
			if ('0' <= ch && ch <= '9')
				return (ch - '0');
			else if ('A' <= ch && ch <= 'F')
				return (10 + ch - 'A');
			else if ('a' <= ch && ch <= 'f')
				return (10 + ch - 'a');

			SyntaxError();
			return 0;
		}


		private Node Parse()
		{
			Node n = ParseExpression();
			Token t = PeekToken();
			if (t == Token.Cut)
			{
				Expect(Token.Cut);
				Node n1 = new CutNode(positions.Count);
				positions.Add(n1);
				n = new SequenceNode(n, n1);
				n = new SequenceNode(n, ParseExpression());
			}
			return n;
		}

		private Node ParseExpression()
		{
			Node n = ParseSequence();
			Token t = PeekToken();
			for (;;)
			{
				switch (t)
				{
				case Token.End:
				case Token.RParen:
				case Token.Cut:
					return n;
				default:
					Expect(Token.Or);
					n = new OrNode(n, ParseSequence());
					break;
				}
				t = PeekToken();

			}
		}

		private Node ParseSequence()
		{
			Node n1 = ParseTerm();
			for (;;)
			{
				Token t = PeekToken(); 
				switch (t)
				{
				case Token.End:
				case Token.Or:
				case Token.RParen:
				case Token.Cut:
					return n1;
				default:
					n1 = new SequenceNode(n1, ParseTerm());
					break;
				}
			}
		}

		private Node ParseTerm()
		{
			Node n1 = ParseFactor();
			Token t = PeekToken();
			switch (t)
			{
			case Token.Star:
				Expect(Token.Star);
				return new StarNode(n1);
			case Token.Plus:
				Expect(Token.Plus);
				return new PlusNode(n1);
			default:
				return n1;
			}
		}

		private Node ParseFactor()
		{
			Node n;
			Token t = GetToken();
			switch (t)
			{
			case Token.Byte: 
				n = new ByteNode(this.b, positions.Count);
				positions.Add(n);
				AddCharacterClass(b);
				break;
			case Token.Dot:
				n = new AnyNode(positions.Count);
				positions.Add(n);
				break;
			case Token.LParen: 
				n = Parse();
				Expect(Token.RParen);
				break;
			default:
				return SyntaxError();
			}
			return n;
		}

		private Token PeekToken()
		{
			if (tok == Token.End)
			{
				tok = ReadToken();
			}
			return tok;
		}
			
		private Token GetToken()
		{
			Token t = tok;
			if (t == Token.End)
			{
				t = ReadToken();
			}
			tok = Token.End;
			return t;
		}


		// Reads hex byte sequences and regexp characters.
		// For instance, the character sequence 'B' '8' returns the 
		// byte 0xB8
		private Token ReadToken()
		{
			int ch = rdr.Read();
			if (ch == -1)
				return Token.End;
			for (;;)
			{
				switch (ch)
				{
				case '*': return Token.Star;
				case '(': return Token.LParen;
				case ')': return Token.RParen;
				case '|': return Token.Or;
				case '+': return Token.Plus;
				case '.': return Token.Dot;
				case '/': return Token.Cut;
				default:
					if (Char.IsWhiteSpace((char)ch))
						break;
					b = (byte) (HexDigit(ch) << 4);
					ch = rdr.Read();
					b |= (byte) HexDigit(ch);
					return Token.Byte;
				}
			}
		}

		private Node SyntaxError()
		{
			throw new ArgumentException("Pattern is malformed: \"" + pattern + "\".");
		}

		private abstract class Node
		{
			public abstract void ComputeNullable();
			public abstract void ComputeFirstPos(RegexpBuilder bld);
			public abstract void ComputeLastPos(RegexpBuilder bld);
			public abstract void ComputeFollowPos(RegexpBuilder bld);
			public abstract void Dump(StringBuilder sb);

			public bool Nullable;
			public BitArray FirstPos;
			public BitArray LastPos;
			public BitArray FollowPos;

			protected void DumpCore(StringBuilder sb)
			{
				sb.AppendFormat(
					"\tnull   {3}\r\n" +
					"\tfirst  {0}\r\n" +
					"\tlast   {1}\r\n" +
					"\tfollow {2}\r\n",
					RegexpBuilder.DumpSet(FirstPos),
					RegexpBuilder.DumpSet(LastPos),
					RegexpBuilder.DumpSet(FollowPos),
					Nullable);
			}
		}

		private class ByteNode : Node
		{
			public ByteNode(int b, int p)
			{
				startByte = b; 
				Position = p;
				Accepts = false;
				Cut = false;
			}

			public int startByte;
			public int Position;
			public bool Accepts;
			public bool Any;	
			public bool Cut;

			public override void ComputeNullable()
			{
				Nullable = false;
			}

			public override void ComputeFirstPos(RegexpBuilder bld)
			{
				FirstPos = new BitArray(bld.positions.Count);
				FirstPos.Set(Position, true);
			}

			public override void ComputeLastPos(RegexpBuilder bld)
			{
				LastPos = new BitArray(bld.positions.Count);
				LastPos.Set(Position, true);
			}

			public override void ComputeFollowPos(RegexpBuilder b)
			{
				FollowPos = new BitArray(b.positions.Count);
			}

			public bool Contains(int a)
			{
				return startByte == a;
			}

			public override void Dump(StringBuilder sb)
			{
				if (Any)
				{
					sb.Append("any");
				}
				else
				{
					sb.AppendFormat("{0:X2}", startByte);
				}

				sb.AppendFormat(" pos: {1} ", startByte, Position);
				sb.Append(Accepts ? " Accepts" : "");
				sb.AppendLine();
				DumpCore(sb);
			}
		}

		private class CutNode : ByteNode
		{
			public CutNode(int p) : base(-1, p)
			{
				Cut = true;
			}

			public override void ComputeNullable()
			{
				Nullable = true;
			}
		}

		private class AcceptNode : ByteNode
		{
			public AcceptNode(int p) : base(-1, p)
			{
				Accepts = true;
				Any = true;
			}
		}

		private class AnyNode : ByteNode
		{
			public AnyNode(int p) : base(-1, p)
			{
				Any = true;
			}
		}

		private class OrNode : Node
		{
			public Node Left;
			public Node Right;

			public OrNode(Node left, Node right)
			{
				Left = left; Right = right;
			}

			public override void ComputeNullable()
			{
				Left.ComputeNullable();
				Right.ComputeNullable();
				Nullable = Left.Nullable || Right.Nullable;
			}

			public override void ComputeFirstPos(RegexpBuilder bld)
			{
				Left.ComputeFirstPos(bld);
				Right.ComputeFirstPos(bld);
				FirstPos = (BitArray)Left.FirstPos.Clone();
				FirstPos.Or(Right.FirstPos);
			}

			public override void ComputeLastPos(RegexpBuilder bld)
			{
				Left.ComputeLastPos(bld);
				Right.ComputeLastPos(bld);
				LastPos = (BitArray)Left.FirstPos.Clone();
				LastPos.Or(Right.LastPos);
			}

			public override void ComputeFollowPos(RegexpBuilder b)
			{
				Left.ComputeFollowPos(b);
				Right.ComputeFollowPos(b);
				FollowPos = (BitArray)Left.FollowPos.Clone();
				FollowPos.Or(Right.FollowPos);
			}

			public override void Dump(StringBuilder sb)
			{
				Left.Dump(sb);
				Right.Dump(sb);
				sb.AppendFormat("or ");
				DumpCore(sb);
			}			
		}


		private class StarNode : Node
		{
			public Node node;

			public StarNode(Node n)
			{
				node = n;
			}

			public override void ComputeNullable()
			{
				node.ComputeNullable();
				Nullable = true;
			}

			public override void ComputeFirstPos(RegexpBuilder bld)
			{
				node.ComputeFirstPos(bld);
				FirstPos = (BitArray) node.FirstPos.Clone();
			}

			public override void ComputeLastPos(RegexpBuilder bld)
			{
				node.ComputeLastPos(bld);
				LastPos = (BitArray) node.LastPos.Clone();
			}

			public override void ComputeFollowPos(RegexpBuilder bld)
			{
				node.ComputeFollowPos(bld);
				FollowPos = node.FollowPos;
				for (int i = 0; i != LastPos.Length; ++i)
				{
					if (!LastPos.Get(i))
						continue;
					
					ByteNode ii = (ByteNode) bld.positions[i];
					ii.FollowPos.Or(FirstPos);
				}	
			}
	
			public override void Dump(StringBuilder sb)
			{
				node.Dump(sb);
				sb.Append("star ");
				DumpCore(sb);
			}			
		}


		private class PlusNode : Node
		{
			public Node node;

			public PlusNode(Node n)
			{
				node = n;
			}

			public override void ComputeNullable()
			{
				node.ComputeNullable();
				Nullable = false;
			}

			public override void ComputeFirstPos(RegexpBuilder bld)
			{
				node.ComputeFirstPos(bld);
				FirstPos = (BitArray) node.FirstPos.Clone();
			}

			public override void ComputeLastPos(RegexpBuilder bld)
			{
				node.ComputeLastPos(bld);
				LastPos = (BitArray) node.LastPos.Clone();
			}

			public override void ComputeFollowPos(RegexpBuilder bld)
			{
				node.ComputeFollowPos(bld);
				FollowPos = node.FollowPos;
				for (int i = 0; i != LastPos.Length; ++i)
				{
					if (!LastPos[i])
						continue;
					
					ByteNode ii = (ByteNode) bld.positions[i];
					ii.FollowPos.Or(FirstPos);
				}	
			}
	
			public override void Dump(StringBuilder sb)
			{
				node.Dump(sb);
				sb.Append("plus ");
				DumpCore(sb);
			}			
		}



		private class SequenceNode : Node
		{
			public Node Left;
			public Node Right;

			public SequenceNode(Node left, Node right)
			{
				Left = left; Right = right;
			}

			public override void ComputeNullable()
			{
				Left.ComputeNullable();
				Right.ComputeNullable();
				Nullable = Left.Nullable && Right.Nullable;
			}

			public override void ComputeFirstPos(RegexpBuilder bld)
			{
				Left.ComputeFirstPos(bld);
				Right.ComputeFirstPos(bld);
				FirstPos = (BitArray) Left.FirstPos.Clone();
				if (Left.Nullable)
				{
					FirstPos.Or(Right.FirstPos);
				}
			}

			public override void ComputeLastPos(RegexpBuilder bld)
			{
				Left.ComputeLastPos(bld);
				Right.ComputeLastPos(bld);
				LastPos = (BitArray) Right.LastPos.Clone();
				if (Right.Nullable)
				{
					LastPos.Or(Left.LastPos);
				}
			}

			public override void ComputeFollowPos(RegexpBuilder bld)
			{
				Left.ComputeFollowPos(bld);
				Right.ComputeFollowPos(bld);
				FollowPos = new BitArray(0);
				for (int i = 0; i != Left.LastPos.Length; ++i)
				{
					if (!Left.LastPos.Get(i))
						continue;
					
					ByteNode ii = (ByteNode) bld.positions[i];
					ii.FollowPos.Or(Right.FirstPos);
				}		
			}

			public override void Dump(StringBuilder sb)
			{
				Left.Dump(sb);
				Right.Dump(sb);
				sb.AppendFormat("cat ");
				DumpCore(sb);
			}
		}
			

		private class TableCompacter
		{
			private readonly State [] states;
			private readonly int alphabetSize;

			private int [] rowcnt;
			private int [] rowMin;
			private int [] rowMax;

			private int min;
			private int max;

			private int [] basePos;
			private int [] check;
			private int [] next;
			private Regexp.State [] restates;

			public TableCompacter(State [] states, int alphabetSize)
			{
				this.states = states;
				this.alphabetSize = alphabetSize;
#if DEBUG
				for (int i = 0; i != states.Length; ++i)
				{
					Debug.Write("s" + i + ":");
					for (int j = 0; j != alphabetSize; ++j)
					{
						State n = states[i].NextState[j];
						if (n is not null)
						{
							Debug.Write(" " +  j + ":s" + n.Index);
						}
					}
					Debug.WriteLine("");
				}
#endif
				TallyRowCounts();
				InitOutputArrays();
			}

			public int [] Check
			{
				get { return check; }
			}

			public int [] Next
			{
				get { return next; }
			}

			public int [] Base
			{
				get { return basePos; }
			}

			public Regexp.State [] ReStates
			{
				get  { return restates; } 
			}
			
			public void Compact()
			{
				min = (check.Length - alphabetSize) / 2;
				max = min;

				//$TODO: Sort the states in size order. The hope is that smaller states
				// will fit in the gaps between the big ones.

				for (int i = 0; i != states.Length; ++i)
				{
					FitState(i);
				}

				// Start with min and end with max:

				int pre = states.Length * alphabetSize;
				int post = (max - min) * 2 + states.Length;
				Debug.WriteLine("table would have been: " + pre);
				Debug.WriteLine("table is now: " + post + ", (" + post * 100.0 / (double) pre + "%)");


				restates = new Regexp.State[basePos.Length];
				for (int i = 0; i < basePos.Length; ++i)
				{
                    restates[i] = new Regexp.State
                    {
                        BasePosition = basePos[i] - min,
                        Accepts = states[i].Accepts
                    };
					Debug.WriteLine(
						string.Format(
							"s{0}{1} base:{2}", 
							i, restates[i].Accepts?" (acc)":"", restates[i].BasePosition));
				}
				int [] checkNew = new int[max-min];
				int [] nextNew = new int[max-min];
				for (int j = 0, i = min; i < max; ++i, ++j)
				{
					checkNew[j] = check[i];
					nextNew[j] = next[i];
				}

				check = checkNew;
				next = nextNew;
			}

			private void FitState(int s)
			{
				// Try fitting the state s within [min..max)
				int p;
				for (p = min; p < max - alphabetSize; ++p)
				{
					if (FitsAtPosition(s, p))
					{
						PositionState(s, p);
						return;
					}
				}
				
				for (int delta = 0; ; ++delta)
				{
					p = min - delta;
					if (FitsAtPosition(s, p))
					{
						min = p;
						max = Math.Max(max, p + alphabetSize);
						PositionState(s, p);
						return;
					}
					p = max - alphabetSize + delta;
					if (FitsAtPosition(s, p))
					{
						min = Math.Min(p, min);
						max = p + alphabetSize;
						PositionState(s, p);
						return;
					}
				}
			}

			private bool FitsAtPosition(int s, int p)
			{
				State [] n = states[s].NextState;
				for (int i = 0; i < alphabetSize; ++i)
				{
					if (n[i] is not null && check[i + p] != -1)
						return false;	// position occupied by other state.
				}
				return true;
			}


			private void PositionState(int s, int p)
			{
				basePos[s] = p;

				State [] n = states[s].NextState;
				int m = check.Length;
				for (int i = 0; i < alphabetSize; ++i)
				{
					if (n[i] is not null)
					{
						int z = (i + p) % m;
						next[z] = n[i].Index;
						check[z] = s;
					}
				}
			}

			private void InitOutputArrays()
			{
				basePos = new int[states.Length];
				next = new int[2 * states.Length * alphabetSize];
				check = new int[next.Length];
				for (int i = 0; i != next.Length; ++i)
				{
					next[i] = -1;
					check[i] = -1;
				}
			}

			private void TallyRowCounts()
			{
				rowcnt = new int[states.Length];
				rowMin = new int[states.Length];
				rowMax = new int[states.Length];
				for (int i = 0; i != states.Length; ++i)
				{
					State s = states[i];
					for (int j = 0; j != s.NextState.Length; ++j)
					{
						if (s.NextState[j] is not null)
						{
							if (rowMin[i] == 0)
								rowMin[i] = j;
							rowMax[i] = j+1;
							++rowcnt[i];
						}
					}
				}
			}
		}

		private class State
		{
			public BitArray	Positions;
			public int Index;
			public State [] NextState;
			public bool Accepts;

			public State(BitArray pos, int alphabetSize)
			{
				Positions = pos;
				NextState = new State[alphabetSize];
			}

			public override string ToString()
			{
				var sb = new StringBuilder();
				sb.AppendFormat("s{0} posn {1}", Index, RegexpBuilder.DumpSet(Positions));
				if (Accepts)
				{
					sb.Append(" (acc)");
				}
				sb.Append(" next:");
				for (int i = 0; i != NextState.Length; ++i)
				{
					if (NextState[i] is not null)
					{
						sb.AppendFormat(" {0:X2}->s{1}", i, NextState[i].Index);
					}
				}
				return sb.ToString();
			}
		}
	}
}
