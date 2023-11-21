#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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

namespace Reko.Structure;

using Reko.Core;
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Path = System.Collections.Immutable.ImmutableList<Core.Absyn.AbsynStatement>;

/// <summary>
/// Inserts local variable declarations into an abstract syntax tree.
/// </summary>
/// <remarks>
/// The strategy used is based purely on the syntactic form of the abstract
/// statements. We traverse the syntax tree, maintaining a path of parent
/// compound statements; visiting a compound statement extends the path.
/// The path to each identifier reference is recorded. Once the visit is
/// completed, declaration locations are determined by finding the longest
/// common prefix of all the paths. That common prefix is the "best" location
/// for the declaration.
/// </remarks>
public class DeclarationInserter : IAbsynVisitor<int, Path>
{
    private readonly Procedure proc;
    private readonly Dictionary<AbsynStatement, List<AbsynStatement>> containingStatements;
    private readonly Dictionary<Identifier, List<Path>> paths;
    private readonly HashSet<string> parameters;
    private readonly List<AbsynStatement> statements;

    public DeclarationInserter(Procedure proc)
    {
        this.proc = proc;
        this.parameters = proc.Signature.ParametersValid
            ? proc.Signature.Parameters!.Select(p => p.Name).ToHashSet()
            : new HashSet<string>();
        this.statements = proc.Body!;
        this.containingStatements = new Dictionary<AbsynStatement, List<AbsynStatement>>();
        this.paths = new Dictionary<Identifier, List<Path>>();
    }

    public Procedure Transform()
    {
        Analyze(Path.Empty, proc.Body!);
        Generate();
        return proc;
    }

    private void Analyze(Path basePath, List<AbsynStatement> containingStatements)
    {
        foreach (var stm in containingStatements)
        {
            var path = basePath.Add(stm);
            stm.Accept(this, path!);
            this.containingStatements[stm] = containingStatements;
        }
    }

    private void AnalyzeExp(Path basePath, Expression? exp)
    {
        if (exp is not null)
        {
            var visitor = new ExpLocalVarGenerator(this, basePath);
            exp.Accept(visitor);
        }
    }

    private void Generate()
    {
        foreach (var (id, usages) in paths)
        {
            if (usages.Count == 1)
            {
                // A single use is easy.
                ReplaceWithDeclaration(id, usages[0], usages[0].Count - 1);
            }
            else
            {
                // Find the longest common prefix of all the paths.
                var first = usages[0];
                int iCommon = first.Count - 1;
                foreach (var path in usages.Skip(1))
                {
                    iCommon = FindCommon(first, path, iCommon);
                }
                ReplaceWithDeclaration(id, first, iCommon);
            }
        }
    }

    private int FindCommon(Path a, Path b, int iCommon)
    {
        iCommon = Math.Min(iCommon, a.Count - 1);
        iCommon = Math.Min(iCommon, b.Count - 1);
        int i;
        for (i = 0; i <= iCommon; ++i)
        {
            if (containingStatements[a[i]] != containingStatements[b[i]])
                return i - 1;
            if (a[i] != b[i])
                return i;
        }
        return iCommon;
    }

    private void ReplaceWithDeclaration(Identifier id, Path path, int iCommon)
    {
        if (iCommon >= 0)
        {
            var stm = path[iCommon];
            var stms = this.containingStatements[stm];
            var i = stms.IndexOf(stm);
            if (i >= 0)
            {
                if (stm is AbsynAssignment ass && 
                    ass.Dst == id)
                {
                    stms[i] = new AbsynDeclaration(id, ass.Src);
                }
                else
                {
                    stms.Insert(i, new AbsynDeclaration(id, null));
                }
                return;
            }
        }
        statements.Insert(0, new AbsynDeclaration(id, null));
    }

    private void EnsurePath(Identifier id, Path path)
    {
        if (!this.paths.TryGetValue(id, out var paths))
        {
            paths = new List<Path>();
            this.paths.Add(id, paths);
        }
        paths.Add(path);
    }

    public int VisitAssignment(AbsynAssignment ass, Path path)
    {
        AnalyzeExp(path, ass.Dst);
        AnalyzeExp(path, ass.Src);
        return 0;
    }

    private bool IsLocalIdentifier(Identifier id)
    {
        switch (id.Storage)
        {
        case GlobalStorage: return false;
        case MemoryStorage: return false;
        }
        return !parameters.Contains(id.Name);
    }

    public int VisitBreak(AbsynBreak b, Path _)
    {
        return 0;
    }

    public int VisitCase(AbsynCase c, Path _)
    {
        return 0;
    }

    public int VisitCompoundAssignment(AbsynCompoundAssignment ca, Path path)
    {
        AnalyzeExp(path, ca.Src);
        AnalyzeExp(path, ca.Dst);
        return 0;
    }

    public int VisitContinue(AbsynContinue c, Path _)
    {
        return 0;
    }

    public int VisitDeclaration(AbsynDeclaration decl, Path path)
    {
        Debug.Fail("Impossiburu: we haven't created AbsynDeclarations yet!");
        return 0;
    }

    public int VisitDefault(AbsynDefault d, Path _)
    {
        return 0;
    }

    public int VisitDoWhile(AbsynDoWhile d, Path path)
    {
        Analyze(path, d.Body);
        AnalyzeExp(path, d.Condition);
        return 0;
    }

    public int VisitFor(AbsynFor f, Path path)
    {
        var forPath = path.Add(f);
        f.Initialization.Accept(this, forPath);
        f.Iteration.Accept(this, forPath);
        AnalyzeExp(forPath, f.Condition);
        Analyze(path, f.Body);
        return 0;
    }

    public int VisitGoto(AbsynGoto g, Path path)
    {
        return 0;
    }

    public int VisitIf(AbsynIf i, Path path)
    {
        AnalyzeExp(path, i.Condition);
        Analyze(path, i.Then);
        Analyze(path, i.Else);
        return 0;
    }

    public int VisitLabel(AbsynLabel l, Path path)
    {
        return 0;
    }

    public int VisitLineComment(AbsynLineComment l, Path _)
    {
        return 0;
    }

    public int VisitReturn(AbsynReturn ret, Path path)
    {
        AnalyzeExp(path, ret.Value);
        return 0;
    }

    public int VisitSideEffect(AbsynSideEffect side, Path path)
    {
        AnalyzeExp(path, side.Expression);
        return 0;
    }

    public int VisitSwitch(AbsynSwitch sw, Path path)
    {
        this.AnalyzeExp(path, sw.Expression);
        Analyze(path, sw.Statements);
        return 0;
    }


    public int VisitWhile(AbsynWhile w, Path path)
    {
        AnalyzeExp(path, w.Condition);
        Analyze(path, w.Body);
        return 0;
    }

    private class ExpLocalVarGenerator : ExpressionVisitorBase
    {
        private readonly DeclarationInserter outer;
        private readonly Path path;

        public ExpLocalVarGenerator(DeclarationInserter outer, Path path)
        {
            this.outer = outer;
            this.path = path;
        }

        public override void VisitIdentifier(Identifier id)
        {
            if (outer.IsLocalIdentifier(id))
            {
                outer.EnsurePath(id, path);
            }
        }
    }
}