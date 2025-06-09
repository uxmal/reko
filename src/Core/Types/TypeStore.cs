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

using Reko.Core.Expressions;
using Reko.Core.Loading;
using Reko.Core.Output;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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

        /// <summary>
        /// Sets the data type of <paramref name="exp"/> to <paramref name="dt"/>.
        /// </summary>
        /// <param name="exp"><see cref="Expression"/> whose data type is to be set.</param>
        /// <param name="dt">Datatype to set.</param>
        void SetDataTypeOf(Expression exp, DataType dt);

        /// <summary>
        /// Merge the equivalence classes associated with <paramref name="tv1"/> and
        /// <paramref name="tv2"/>.
        /// </summary>
        /// <param name="tv1">The first type variable.</param>
        /// <param name="tv2">The other type varaible.</param>
        /// <returns>The resulting equivalence class.</returns>
        EquivalenceClass MergeClasses(TypeVariable tv1, TypeVariable tv2);

        /// <summary>
        /// Writes the contents of the type store to the given <paramref name="writer"/>.
        /// </summary>
        /// <param name="showExprAddresses">If true, write the address associated with the
        /// expressions in the type store.</param>
        /// <param name="writer"><see cref="TextWriter"/> to which the output is written.
        /// </param>
        void Write(bool showExprAddresses, TextWriter writer);

        /// <summary>
        /// Once the type store has been populated with type variables and their equiva
        /// , this method
        /// </summary>
        /// <param name="factory"></param>
        void BuildEquivalenceClassDataTypes(TypeFactory factory);

        /// <summary>
        /// Creates a type variable.
        /// </summary>
        /// <param name="factory">Factory to create the type variable with.</param>
        TypeVariable CreateTypeVariable(TypeFactory factory);

        /// <summary>
        /// Gets the <see cref="TypeVariable" /> associated with the <see cref="Expression"/>
        /// <paramref name="e"/>.
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
        /// <summary>
        /// Clears the type variable associated with the expression <paramref name="expr"/>.
        /// </summary>
        /// <param name="expr">Expression whose type variable we wish to clear.</param>
        void ClearTypeVariable(Expression expr);

        /// <summary>
        /// Get the type variable for the <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">Expression whose type variable is to be found.</param>
        /// <param name="tv">The <see cref="TypeVariable"/> of <paramref name="expression"/>,
        /// if present in the store.</param>
        /// <returns>True if there is a type variable for the expression; otherwise false.
        /// </returns>
        bool TryGetTypeVariable(Expression expression, [MaybeNullWhen(false)] out TypeVariable tv);
    }

    /// <summary>
    /// Implementation of <see cref="ITypeStore"/>.
    /// </summary>
    public class TypeStore : ITypeStore
    {
        private readonly SortedList<int, EquivalenceClass> usedClasses;
        private readonly Dictionary<TypeVariable, (Address? uAddr, Expression e)> tvSources;
        private readonly Dictionary<Expression, TypeVariable> mapExprToTypevar;

        /// <summary>
        /// Constructs an empty type store.
        /// </summary>
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

        /// <summary>
        /// All the <see cref="ImageSegment"/>s and their corresponding <see cref="StructureType"/>s."/>
        /// </summary>
        public Dictionary<ImageSegment, StructureType> SegmentTypes { get; private set; }

        //$TODO: pass dt and dtOriginal
        /// <summary>
        /// Creates a type variable.
        /// </summary>
        /// <param name="factory">Type factory to use.</param>
        /// <returns>A new type variable.</returns>
        public TypeVariable CreateTypeVariable(TypeFactory factory)
        {
            TypeVariable tv = factory.CreateTypeVariable();
            tv.Class = new EquivalenceClass(tv);
            this.TypeVariables.Add(tv);
            this.usedClasses.Add(tv.Class.Number, tv.Class);
            return tv;
        }

        /// <summary>
        /// Ensures that the expression <paramref name="e"/> has a type variable associated with it.
        /// </summary>
        /// <param name="factory">Type factory to use.</param>
        /// <param name="addr">Optional address of the expression.</param>
        /// <param name="e">Expression.</param>
        /// <returns>A new or existing <see cref="TypeVariable"/>.</returns>
        public TypeVariable EnsureExpressionTypeVariable(TypeFactory factory, Address? addr, Expression e)
        {
            return EnsureExpressionTypeVariable(factory, addr, e, null);
        }

        /// <summary>
        /// Ensures that the expression <paramref name="e"/> has a type variable associated with it.
        /// </summary>
        /// <param name="factory">Type factory to use.</param>
        /// <param name="addr">Optional address of the expression.</param>
        /// <param name="e">Expression.</param>
        /// <param name="name">Optional name of the type variable.</param>
        /// <returns>A new or existing <see cref="TypeVariable"/>.</returns>
        public TypeVariable EnsureExpressionTypeVariable(TypeFactory factory, Address? addr, Expression e, string? name)
        {
            if (this.TryGetTypeVariable(e, out var tv))
                return tv;

            tv = name is not null ? factory.CreateTypeVariable(name) : factory.CreateTypeVariable();
            AddDebugSource(tv, addr, e!);
            tv.Class = new EquivalenceClass(tv);
            if (e is not null)
                this.SetTypeVariable(e, tv);
            this.TypeVariables.Add(tv);
            this.usedClasses.Add(tv.Class.Number, tv.Class);
            return tv;
        }

        /// <summary>
        /// Sets the expression associated with a type variable.
        /// </summary>
        /// <param name="typeVariable">Type variable.</param>
        /// <param name="addr">Optional address of the expression.</param>
        /// <param name="binExp">Expression.</param>
        public void SetTypeVariableExpression(TypeVariable typeVariable, Address? addr, Expression binExp)
        {
            tvSources[typeVariable] = (addr, binExp);
        }

        private void AddDebugSource(TypeVariable tv, Address? uAddr, Expression e)
        {
            if (e is not null)
                tvSources.Add(tv, (uAddr, e));
        }

        /// <inheritdoc/>
        public void BuildEquivalenceClassDataTypes(TypeFactory factory)
        {
            Unifier u = new DataTypeBuilderUnifier(factory, this);
            foreach (TypeVariable tv in TypeVariables)
            {
                DataType dt = tv.OriginalDataType;
                EquivalenceClass c = tv.Class;
                DataType dtOld = c.DataType;
                var dtNew = dt;
                if (dtOld is not null)
                {
                    dtNew = u.Unify(dt, dtOld)!;
                }
                else if (dt is not null)
                {
                    dtNew = dt.Clone();        // why clone???
                }
                c.DataType = dtNew;
            }
        }

        /// <summary>
        /// Dumps the contents of the type store to the debugger output window.
        /// </summary>

        [Conditional("DEBUG")]
        public void Dump()
        {
            var sw = new StringWriter();
            Write(false, sw);
            Debug.WriteLine(sw.ToString());
        }

        /// <summary>
        /// Dumps the contents of the type store to the given file.
        /// </summary>
        [Conditional("DEBUG")]
        public void Dump(string dir, string filename)
        {
            using var w = new StreamWriter(Path.Combine(dir, filename));
            Write(false, w);
            Debug.WriteLine(w.ToString());
        }

        /// <summary>
        /// Finds the expression associated with a type variable.
        /// </summary>
        /// <param name="tv">Type variable whose association is 
        /// requested.</param>
        /// <returns>The corresponding <see cref="Expression"/>, or
        /// null if none is found.
        /// </returns>
        public Expression? ExpressionOf(TypeVariable tv)
        {
            if (tvSources.TryGetValue(tv, out (Address?, Expression) dbg))
                return dbg.Item2;
            else
                return null;
        }

        /// <summary>
        /// Retrieves the address of the expression associated with a type variable, if any.
        /// </summary>
        /// <param name="tv">Type variable whose association is requested.</param>
        /// <returns>The corresponding <see cref="Address"/>, or 
        /// null if none is found.
        /// </returns>
        public Address? AddressOf(TypeVariable tv)
        {
            if (tvSources.TryGetValue(tv, out (Address?, Expression) dbg))
                return dbg.Item1;
            else
                return null;
        }

        /// <inheritdoc/>
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

        /// <summary>
        /// Returns the equivalence classes that are in use in the program.
        /// </summary>
        public IList<EquivalenceClass> UsedEquivalenceClasses
        {
            get { return usedClasses.Values; }
        }

        /// <inheritdoc/>
        public void Write(bool showExprAddresses, TextWriter w)
        {
            var writer = new TextFormatter(w);
            writer.WriteLine("// Equivalence classes ////////////");
            foreach (TypeVariable tv in TypeVariables)
            {
                if (tv.Class.Representative == tv && tv.Class.DataType is not null)
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

        /// <summary>
        /// Given a type variable, writes the corresponding expression if one is available.
        /// </summary>
        /// <param name="tv">Type varaible</param>
        /// <param name="showExprAddresses">If trie show the address of the expression.</param>
        /// <param name="writer">Output sink.</param>
        public void WriteExpressionOf(TypeVariable tv, bool showExprAddresses, Formatter writer)
        {
            if (tvSources.TryGetValue(tv, out (Address? addr, Expression e) dbg) && 
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

        private void WriteEntry(TypeVariable tv, bool showExprAddresses, Formatter writer)
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

        /// <inheritdoc/>
        public DataType? GetDataTypeOf(Expression exp)
        {
            return this.mapExprToTypevar.TryGetValue(exp, out var tv)
                ? tv.DataType
                : null;
        }

        /// <inheritdoc/>
        public TypeVariable GetTypeVariable(Expression expr)
        {
            return this.mapExprToTypevar[expr];
        }

        /// <inheritdoc/>
        public bool TryGetTypeVariable(Expression expr, [MaybeNullWhen(false)] out TypeVariable tv)
        {
            return this.mapExprToTypevar.TryGetValue(expr, out tv);
        }

        /// <inheritdoc/>
        public void SetDataTypeOf(Expression expr, DataType dt)
        {
            var tv = this.mapExprToTypevar[expr];
            tv.DataType = dt;
            tv.OriginalDataType = dt;
        }

        /// <inheritdoc/>
        public void SetTypeVariable(Expression expr, TypeVariable tv)
        {
            //$TODO: ideally, the type variable of an expression is never updated. 
            // But this method does get called repeatedly on the same expression
            // Investigate why.
            this.mapExprToTypevar[expr] = tv;
        }

        /// <inheritdoc/>
        public void ClearTypeVariable(Expression expr)
        {
            mapExprToTypevar.Remove(expr);
        }

        /// <summary>
        /// Clears the type store.
        /// </summary>
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