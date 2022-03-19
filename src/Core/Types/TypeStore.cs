#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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

using Reko.Core.Expressions;
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Reko.Core.Types
{
    /// <summary>
    /// Stores, for a particular program, all type variables, equivalence 
    /// classes, and their mappings to each other.
    /// </summary>
    public interface ITypeStore
    {
        DataType GetDataTypeOf(Expression exp);
        void SetDataTypeOf(Expression exp, DataType dt);
        EquivalenceClass MergeClasses(TypeVariable tv1, TypeVariable tv2);
        void Write(bool showExprAddresses, TextWriter writer);
        void BuildEquivalenceClassDataTypes(TypeFactory factory);
        TypeVariable CreateTypeVariable(TypeFactory factory);
    }

    public class TypeStore : ITypeStore
    {
        private readonly SortedList<int, EquivalenceClass> usedClasses;
        private readonly Dictionary<TypeVariable, (ulong uAddr, Expression e)> tvSources;

        public TypeStore()
        {
            TypeVariables = new List<TypeVariable>();
            usedClasses = new SortedList<int, EquivalenceClass>();
            tvSources = new Dictionary<TypeVariable, (ulong,Expression)>();
            SegmentTypes = new Dictionary<ImageSegment, StructureType>();
        }

        /// <summary>
        /// All the <see cref="TypeVariable"/>s of the program.
        /// </summary>
        public List<TypeVariable> TypeVariables { get; private set; }

        public Dictionary<ImageSegment, StructureType> SegmentTypes { get; private set; }

        public TypeVariable EnsureExpressionTypeVariable(TypeFactory factory, ulong uAddr, Expression e)
        {
            return EnsureExpressionTypeVariable(factory, uAddr, e, null);
        }

        //$TODO: pass dt and dtOriginal
        public TypeVariable CreateTypeVariable(TypeFactory factory)
        {
            TypeVariable tv = factory.CreateTypeVariable();
            tv.Class = new EquivalenceClass(tv);
            this.TypeVariables.Add(tv);
            this.usedClasses.Add(tv.Class.Number, tv.Class);
            return tv;
        }

        public TypeVariable EnsureExpressionTypeVariable(TypeFactory factory, ulong uAddr, Expression e, string? name)
        {
            if (e != null && e.TypeVariable != null)
                return e.TypeVariable;

            TypeVariable tv = name != null ? factory.CreateTypeVariable(name) : factory.CreateTypeVariable();
            AddDebugSource(tv, uAddr, e!);
            tv.Class = new EquivalenceClass(tv);
            if (e != null)
                e.TypeVariable = tv;
            this.TypeVariables.Add(tv);
            this.usedClasses.Add(tv.Class.Number, tv.Class);
            return tv;
        }

        public void SetTypeVariableExpression(TypeVariable typeVariable, ulong uAddr, Expression binExp)
        {
            tvSources[typeVariable] = (uAddr, binExp);
        }

        private void AddDebugSource(TypeVariable tv, ulong uAddr, Expression e)
        {
            if (e != null)
                tvSources.Add(tv, (uAddr, e));
        }

        public void BuildEquivalenceClassDataTypes(TypeFactory factory)
        {
            Unifier u = new DataTypeBuilderUnifier(factory, this);
            foreach (TypeVariable tv in TypeVariables)
            {
                DataType dt = tv.OriginalDataType;
                EquivalenceClass c = tv.Class;
                DataType dtOld = c.DataType;
                if (dtOld != null)
                {
                    dt = u.Unify(dt, dtOld)!;
                }
                else if (dt != null)
                {
                    dt = dt.Clone();        // why clone???
                }
                c.DataType = dt!;
            }
        }


        [Conditional("DEBUG")]
        public void Dump()
        {
            var sw = new StringWriter();
            Write(false, sw);
            Debug.WriteLine(sw.ToString());
        }

        [Conditional("DEBUG")]
        public void Dump(string dir, string filename)
        {
            using var w = new StreamWriter(Path.Combine(dir, filename));
            Write(false, w);
            Debug.WriteLine(w.ToString());
        }

        public Expression? ExpressionOf(TypeVariable tv)
        {
            if (tvSources.TryGetValue(tv, out (ulong, Expression) dbg))
                return dbg.Item2;
            else
                return null;
        }

        public ulong? LinearAddressOf(TypeVariable tv)
        {
            if (tvSources.TryGetValue(tv, out (ulong, Expression) dbg))
                return dbg.Item1;
            else
                return null;
        }


        public EquivalenceClass MergeClasses(TypeVariable tv1, TypeVariable tv2)
        {
            EquivalenceClass class1 = tv1.Class;
            EquivalenceClass class2 = tv2.Class;
            usedClasses.Remove(class1.Number);
            usedClasses.Remove(class2.Number);
            EquivalenceClass merged = EquivalenceClass.Merge(class1, class2);
            usedClasses.Add(merged.Number, merged);
            tv1.Class = merged;
            tv2.Class = merged;
            return merged;
        }

        /// <summary>
        /// For each equivalence class, ensures that all of its constituent type variables
        /// have the same data type.
        /// </summary>
        public void CopyClassDataTypesToTypeVariables()
        {
            foreach (TypeVariable tv in TypeVariables)
            {
                DataType dt = tv.Class.DataType;
                tv.DataType = dt;
            }
        }

        public IList<EquivalenceClass> UsedEquivalenceClasses
        {
            get { return usedClasses.Values; }
        }

        public void Write(TextWriter w) => Write(false, w);

        public void Write(bool showExprAddresses, TextWriter w)
        {
            var writer = new TextFormatter(w);
            writer.WriteLine("// Equivalence classes ////////////");
            foreach (TypeVariable tv in TypeVariables)
            {
                if (tv.Class.Representative == tv && tv.Class.DataType != null)
                {
                    writer.WriteLine("{0}: {1}", tv.Class, tv.Class.DataType);
                    foreach (TypeVariable tvMember in tv.Class.ClassMembers)
                    {
                        writer.Write("\t{0}", tvMember);
                        WriteExpressionOf(tvMember, showExprAddresses, writer);
                        writer.WriteLine();
                    }
                }
            }

            writer.WriteLine("// Type Variables ////////////");
            foreach (TypeVariable tv in TypeVariables)
            {
                WriteEntry(tv, showExprAddresses, writer);
            }
        }

        public void WriteExpressionOf(TypeVariable tvMember, bool showExprAddresses, Formatter writer)
        {
            if (tvSources.TryGetValue(tvMember, out (ulong uAddr, Expression e) dbg) && dbg.e != null)
            {
                writer.Write(" (in {0}", dbg.e);
                if (showExprAddresses)
                {
                    writer.Write(" @ {0:X8}", dbg.uAddr);
                }
                if (dbg.e.DataType != null)
                {
                    writer.Write(" : ");
                    writer.Write(dbg.e.DataType);
                }
                writer.Write(")");
            }
        }

        public void WriteEntry(TypeVariable tv, bool showExprAddresses, Formatter writer)
        {
            writer.Write(tv.Name);
            writer.Write(":");
            WriteExpressionOf(tv, showExprAddresses, writer);
            writer.WriteLine();

            writer.Write("  Class: ");
            writer.WriteLine(tv.Class.Name);

            writer.Write("  DataType: ");
            writer.WriteLine(tv.DataType);

            writer.Write("  OrigDataType: ");
            writer.WriteLine(tv.OriginalDataType);
        }

        private void WriteEntry(TypeVariable tv, DataType dt, bool showExprAddresses, Formatter writer)
        {
            writer.Write("{0}: ", tv);
            if (dt != null)
            {
                dt.Accept(new TypeGraphWriter(writer));
                WriteExpressionOf(tv, showExprAddresses, writer);
            }
            writer.WriteLine();
        }

        public DataType GetDataTypeOf(Expression exp)
        {
            return exp.TypeVariable!.DataType;
        }

        public void SetDataTypeOf(Expression expr, DataType dt)
        {
            expr.TypeVariable!.DataType = dt;
            expr.TypeVariable.OriginalDataType = dt;
        }

        public void Clear()
        {
            foreach(var dbg in tvSources.Values)
            {
                dbg.e.TypeVariable = null;
                if (dbg.e is ProcedureConstant pc)
                    pc.Procedure.Signature.TypeVariable = null;
            }
            TypeVariables.Clear();
            usedClasses.Clear();
            tvSources.Clear();
        }
    }
}