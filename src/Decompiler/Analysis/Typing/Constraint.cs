/* 
 * Copyright (C) 1999-2007 John Källén.
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
using System.Text;

/// Types
/// 0: NONE
/// 1: bool
/// 2: char
/// 3: byte
/// 4: short
/// 5: unsigned short
/// 6: int
/// 7: unsigned int
/// 8: long
/// 9: unsigned long
/// 10: float
/// 11: double
/// 12: segment
/// 13: offset
/// 

namespace Decompiler.Analysis.Typing
{
	/// <summary>
	/// There are different kinds of constraint;
	///  [t1] == [t2]: equivalence constraint; t1 is identical to t2.
	///  [t1] isa Tycon<a, b>; t1 is a 
	/// </summary>
	public class Constraint
	{
		public Constraint()
		{
		}
	}

	public class EquivalenceConstraint : Constraint
	{
		public int t1;
		public int t2;

		public EquivalenceConstraint(int t1, int t2)
		{
		}
	}

	public class TypeConstructorConstraint : Constraint
	{
		public int t1;
		public TypeConstructor tycon;

		public TypeConstructorConstraint(int t1, TypeConstructor tycon)
		{
		}
	}

	public class RecordFieldConstraint : Constraint
	{
		public int t1;
		public int t2;
		public int fieldOffset;

		public RecordFieldConstraint(int t1, int t2, int fieldOffset)
		{
		}
	}

	public class AssignmentCompatibleConstraint : Constraint
	{
		public int t1;
		public int t2;

		public AssignmentCompatibleConstraint(int t1, int t2)
		{
			this.t1 = t1; 
			this.t2 = t2;
		}
	}

	// Used for basic types, like int, char, byte....
	// as well type constructors, like Ptr<int>, Ptr<*>
	// where '*' means any.

	public class Label
	{
	}

	public class TypeConstructor : Label
	{
		private string name;

		public TypeConstructor(string name)
		{
			this.name = name;
		}

		public TypeNode [] Components
		{
			get { return null; }
		}

		public string Name
		{
			get { return name; }
		}
	}

	public class PointerConstructor : TypeConstructor
	{
		public TypeNode node;

		public PointerConstructor(TypeNode node) :
			base("ptr")
		{
		}
	}

	public class RecordConstructor : TypeConstructor
	{
		public RecordConstructor() :
			base("record")
		{

		}

		public struct Field
		{
			public int offset;
			public TypeNode type;
		}

		public Field [] Fields;
	}

	public class TypeVariable
	{
		public int Number;
		public TypeNode node;
	}

	// Nodes contain labels.

	public class TypeNode
	{
		private int n;
		public Label [] labels;
		public Relation [] relations;

		public TypeNode()
		{
			n = ++count;
		}

		private static int count;
	}

	// Each constraint in a relation specifies a possible solution to the relation.

	public class Relation
	{
		public Constraint [] constraints;
		public TypeNode [] nodes;
	}

	// We are interested in the set of solutions to a large number of constraints.
	// The set of solutions is represented by a graph where nodes contain
	// _labels_

	public class TypeGraph
	{
		// The labels of the resulting collapsed node are the
		// intersection of the two labels.

		public TypeNode CollapseNodes(TypeNode n1, TypeNode n2)
		{
			return null;
		}

		// Remove constraint items that are no longer possible.
		// May result in collapsing and reduction of nodes.

		public void ReduceRelation(Relation r)
		{
		}

		// Remove one or more labels from the node. May
		// result in reduction of relations.

		public void ReduceNode(TypeNode n)
		{
		}
	}
}
