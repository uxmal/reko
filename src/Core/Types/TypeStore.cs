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
using System.Diagnostics.CodeAnalysis;
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
        /// <summary>
        /// Get the inferred data type of <paramref name="exp"/>.
        /// </summary>
        DataType? GetDataTypeOf(Expression exp);
        void SetDataTypeOf(Expression exp, DataType dt);
        EquivalenceClass MergeClasses(TypeVariable tv1, TypeVariable tv2);
        void Write(bool showExprAddresses, TextWriter writer);
        void BuildEquivalenceClassDataTypes(TypeFactory factory);
        TypeVariable CreateTypeVariable(TypeFactory factory);

        /// <summary>
        /// Gets the <see cref="TypeVariable" /> associated with the <see cref="Expression"/>
        /// <paramref name="e""/>.
        /// </summary>
        /// <param name="e">Expression whose <see cref="TypeVariable"/> is requested.</param>
        /// <returns>A <see cref="TypeVariable"/> instance, or null if no type variable
        /// exists.
        /// </returns>
        TypeVariable GetTypeVariable(Expression e);

        /// <summary>
        /// Sets the type variable corresponding to the expression <paramref name="e"/>.
        /// </summary>
        /// <param name="e">Expression whose type variable is to be assigned.</param>
        /// <param name="tv">The expression's type varaible.</param>
        void SetTypeVariable(Expression e, TypeVariable tv);

        //$TODO: this is only used temporarily - we eventually want to get rid of this
        // but it's used in some hacky places.
        void ClearTypeVariable(Expression expr);

        bool TryGetTypeVariable(Expression expression, [MaybeNullWhen(false)] out TypeVariable tv);
    }

    public class TypeStore : ITypeStore
    {
        private readonly SortedList<int, EquivalenceClass> usedClasses;
        private readonly Dictionary<TypeVariable, (Address? uAddr, Expression e)> tvSources;
        private readonly Dictionary<Expression, TypeVariable> mapExprToTypevar;

        public TypeStore()
        {
            TypeVariables = new List<TypeVariable>();
            usedClasses = new SortedList<int, EquivalenceClass>();
            tvSources = new Dictionary<TypeVariable, (Address?,Expression)>();
            SegmentTypes = new Dictionary<ImageSegment, StructureType>();
            this.mapExprToTypevar = new Dictionary<Expression, TypeVariable>();
        }

        /// <summary>
        /// All the <see cref="TypeVariable"/>s of the program.
        /// </summary>
        public List<TypeVariable> TypeVariables { get; private set; }

        public Dictionary<ImageSegment, StructureType> SegmentTypes { get; private set; }

        public TypeVariable EnsureExpressionTypeVariable(TypeFactory factory, Address? addr, Expression e)
        {
            return EnsureExpressionTypeVariable(factory, addr, e, null);
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

        public TypeVariable EnsureExpressionTypeVariable(TypeFactory factory, Address? addr, Expression e, string? name)
        {
            if (this.TryGetTypeVariable(e, out var tv))
                return tv;

            tv = name != null ? factory.CreateTypeVariable(name) : factory.CreateTypeVariable();
            AddDebugSource(tv, addr, e!);
            tv.Class = new EquivalenceClass(tv);
            if (e != null)
                this.SetTypeVariable(e, tv);
            this.TypeVariables.Add(tv);
            this.usedClasses.Add(tv.Class.Number, tv.Class);
            return tv;
        }

        public void SetTypeVariableExpression(TypeVariable typeVariable, Address? addr, Expression binExp)
        {
            tvSources[typeVariable] = (addr, binExp);
        }

        private void AddDebugSource(TypeVariable tv, Address? uAddr, Expression e)
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
            if (tvSources.TryGetValue(tv, out (Address?, Expression) dbg))
                return dbg.Item2;
            else
                return null;
        }

        public Address? AddressOf(TypeVariable tv)
        {
            if (tvSources.TryGetValue(tv, out (Address?, Expression) dbg))
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
            if (tvSources.TryGetValue(tvMember, out (Address? addr, Expression e) dbg) && 
                dbg.e is { })
            {
                writer.Write(" (in {0}", dbg.e);
                if (showExprAddresses && dbg.addr is { })
                { 
                    writer.Write(" @ {0}", dbg.addr);
                }
                if (dbg.e.DataType is { })
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

        public DataType? GetDataTypeOf(Expression exp)
        {
            return this.mapExprToTypevar.TryGetValue(exp, out var tv)
                ? tv.DataType
                : null;
        }

        public TypeVariable GetTypeVariable(Expression expr)
        {
            return this.mapExprToTypevar[expr];
        }

        public bool TryGetTypeVariable(Expression expr, [MaybeNullWhen(false)] out TypeVariable tv)
        {
            return this.mapExprToTypevar.TryGetValue(expr, out tv);
        }

        public void SetDataTypeOf(Expression expr, DataType dt)
        {
            var tv = this.mapExprToTypevar[expr];
            tv.DataType = dt;
            tv.OriginalDataType = dt;
        }

        public void SetTypeVariable(Expression expr, TypeVariable tv)
        {
            //$TODO: ideally, the type variable of an expression is never updated. 
            // But this method does get called repeatedly on the same expression
            // Investigate why.
            this.mapExprToTypevar[expr] = tv;
        }

        public void ClearTypeVariable(Expression expr)
        {
            mapExprToTypevar.Remove(expr);
        }

        public void Clear()
        {
            foreach(var dbg in tvSources.Values)
            {
                if (dbg.e is ProcedureConstant pc)
                    pc.Signature.TypeVariable = null;
            }
            mapExprToTypevar.Clear();
            TypeVariables.Clear();
            usedClasses.Clear();
            tvSources.Clear();
        }
    }
}