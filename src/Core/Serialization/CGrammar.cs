#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core.Serialization
{
    public class CGrammar
    {
        public virtual CExpression Bin(CTokenType op, CExpression left, CExpression right)
        {
            return new CBinaryExpression
            {
                Operation = op,
                Left = left,
                Right = right
            };
        }

        public CExpression Unary(CTokenType operation, CExpression expr)
        {
            return new CUnaryExpression { Operation = operation, Expression = expr };
        }

        public CIdentifier Id(string name)
        {
            return new CIdentifier { Name = name };
        }

        internal CExpression Application(CExpression e, List<CExpression> list)
        {
            throw new NotImplementedException();
        }

        internal CExpression ArrayAccess(CExpression e, CExpression idx)
        {
            throw new NotImplementedException();
        }

        internal CExpression MemberAccess(CExpression e, string id)
        {
            throw new NotImplementedException();
        }

        internal CExpression PtrMemberAccess(CExpression e, string id)
        {
            throw new NotImplementedException();
        }

        internal CExpression PostIncrement(CExpression e, CTokenType token)
        {
            throw new NotImplementedException();
        }

        internal CExpression Const(object value)
        {
            throw new NotImplementedException();
        }

        internal CExpression Sizeof(CType type)
        {
            throw new NotImplementedException();
        }

        internal CExpression Sizeof(CExpression sexp)
        {
            throw new NotImplementedException();
        }

        internal CExpression PreIncrement(CTokenType token, CExpression uexpr)
        {
            throw new NotImplementedException();
        }

        internal CExpression Cast(CType type, CExpression exp)
        {
            throw new NotImplementedException();
        }

        internal CExpression Application(CExpression e)
        {
            throw new NotImplementedException();
        }

        internal DeclSpec StorageClass(CTokenType token)
        {
            throw new NotImplementedException();
        }

        internal TypeQualifier TypeQualifier(CTokenType token)
        {
            throw new NotImplementedException();
        }

        internal StructDeclarator StructDeclarator(Declarator decl, CExpression bitField)
        {
            throw new NotImplementedException();
        }

        internal TypeSpec SimpleType(CTokenType cTokenType)
        {
            throw new NotImplementedException();
        }

        internal TypeSpec TypeName(string name)
        {
            throw new NotImplementedException(name);
        }

        internal Declarator IdDeclarator(string name)
        {
            return new IdDeclarator { Id = name };
        }

        internal PointerDeclarator PointerDeclarator()
        {
            throw new NotImplementedException();
        }

        internal Stat IfStatement(CExpression expr, Stat consequence, Stat alternative)
        {
            throw new NotImplementedException();
        }

        internal Stat SwitchStatement(CExpression expr, Stat switchBody)
        {
            throw new NotImplementedException();
        }

        internal Stat WhileStatement(CExpression expr, Stat switchBody)
        {
            throw new NotImplementedException();
        }

        internal Stat DoWhileStatement(Stat doBody, CExpression expr)
        {
            throw new NotImplementedException();
        }

        internal Stat ForStatement(Statement test1, CExpression test2, CExpression  incr, Stat forBody)
        {
            throw new NotImplementedException();
        }

        internal Stat GotoStatement(string gotoLabel)
        {
            throw new NotImplementedException();
        }

        internal Stat ContinueStatement()
        {
            throw new NotImplementedException();
        }

        internal Stat BreakStatement()
        {
            throw new NotImplementedException();
        }

        internal Stat EmptyStatement()
        {
            throw new NotImplementedException();
        }

        internal Stat ReturnStatement(CExpression expr)
        {
            throw new NotImplementedException();
        }

        internal ParamDecl ParamDecl(List<DeclSpec> dsl, Declarator decl)
        {
            throw new NotImplementedException();
        }

        internal TypeSpec Enum(string tag, List<Enumerator> enums)
        {
            throw new NotImplementedException();
        }

        internal TypeSpec ComplexType(CTokenType token, string tag, List<StructDecl> decls)
        {
            throw new NotImplementedException();
        }

        internal Label DefaultCaseLabel()
        {
            throw new NotImplementedException();
        }

        internal Stat LabeledStatement(Label label, Stat stat)
        {
            throw new NotImplementedException();
        }

        internal Label CaseLabel(CExpression constExpr)
        {
            throw new NotImplementedException();
        }

        internal Label Label(string p)
        {
            throw new NotImplementedException();
        }

        internal Decl Decl()
        {
            throw new NotImplementedException();
        }

        internal InitDeclarator InitDeclarator(Declarator decl, Initializer init)
        {
            throw new NotImplementedException();
        }

        internal Initializer ListInitializer(List<Initializer> list)
        {
            throw new NotImplementedException();
        }

        internal Initializer ExpressionInitializer(CExpression expr)
        {
            throw new NotImplementedException();
        }

        internal Stat ExprStatement(CExpression expr)
        {
            throw new NotImplementedException();
        }

        internal CExpression Conditional(CExpression cond, CExpression consequent, CExpression alternant)
        {
            throw new NotImplementedException();
        }

        internal Enumerator Enumerator(CIdentifier id, ConstExp init)
        {
            throw new NotImplementedException();
        }

        internal StructDecl StructDecl(List<DeclSpec> sql, List<StructDeclarator> decls)
        {
            throw new NotImplementedException();
        }

        internal Declarator ArrayDeclarator(CExpression expr)
        {
            throw new NotImplementedException();
        }

        internal Enumerator Enumerator(CIdentifier id, CExpression init)
        {
            throw new NotImplementedException();
        }

        internal ParamDecl Ellipsis()
        {
            throw new NotImplementedException();
        }
    }

    public class CType
    {
    }

    public class ExternalDecl: Decl
    {
    }

    public class Decl
    {
        public List<DeclSpec> decl_specs;
        public List<InitDeclarator> init_declarator_list;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(decl");
            foreach (var ds in decl_specs)
            {
                sb.Append(" ");
                sb.Append(ds);
            }
            if (init_declarator_list != null && init_declarator_list.Count > 0)
            {
                var sep = " (";
                foreach (var init in init_declarator_list)
                {
                    sb.AppendFormat("{0}{1}", sep, init);
                    sep = " ";
                }
                sb.Append(")");
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

    public abstract class DeclSpec
    {
    }

    public class StorageClassSpec : DeclSpec
    {
        public CTokenType Type;
        public override string ToString() { return Type.ToString(); }
    }

    public abstract class TypeSpec : DeclSpec
    {
    }

    public class SimpleTypeSpec : TypeSpec
    {
        public CTokenType Type;
        public override string ToString() { return Type.ToString(); }
    }

    public class TypeDefName : TypeSpec
    {
        public string Id;
    }

    public class TypeQualifier : DeclSpec
    {
        public CTokenType Type;
    }


    public class ComplexTypeSpec : TypeSpec
    {
        public CTokenType Type;
        public string Name;
        public List<StructDecl> DeclList;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("({0} ", Type);
            if (!string.IsNullOrEmpty(Name))
                sb.AppendFormat("{0} ", Name);
            var sep = "(";
            foreach (var sd in DeclList)
            {
                sb.AppendFormat("{0}{1}", sep, sd);
                sep = " ";
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

    public class Enumerator
    {
    }

    public class InitDeclarator
    {
        public Declarator Declarator;
        public Initializer Init;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(init-decl ");
            sb.Append(Declarator);
            if (this.Init != null)
            {
                sb.AppendFormat(" {0}", Init);
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

    public abstract class Declarator
    {
    }

    public class DirectDeclarator : Declarator
    {
        public PointerDeclarator Pointer;
        public Declarator Declarator;

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Pointer != null)
            {
                sb.Append("(");
                sb.Append(Pointer);
                sb.Append(" ");
                sb.Append(Declarator);
                sb.Append(")");
            }
            else
            {
                sb.Append(Declarator);
            }
            return sb.ToString();
        }
    }

    public class ParamDecl
    {
    }

    public class PointerDeclarator : Declarator
    {
        public PointerDeclarator Pointer;
        public List<TypeQualifier> TypeQualifierList;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(ptr");
            if (TypeQualifierList != null && TypeQualifierList.Count > 0)
            {
                foreach (var tq in TypeQualifierList)
                {
                    sb.AppendFormat(" {0}", tq);
                }
            }
            sb.AppendFormat(" {0})", Pointer);
            return sb.ToString();
        }
    }

    public class IdDeclarator : DirectDeclarator
    {
        public string Id;
        public override string ToString() { return Id; }
    }

    public class ArrayDeclarator : DirectDeclarator
    {
        public DirectDeclarator DirectDeclarator;
        public ConstExp Size;
    }

    public class FuncDeclarator : DirectDeclarator
    {
        public DirectDeclarator DirectDeclarator;
        public object args;
    }

    public class ConstExp
    {
        public object Const;
        public override string ToString() { return Const != null ? Const.ToString() : "";}
    }

    public class StructDecl
    {
        public List<DeclSpec> SpecQualifierList;
        public List<StructDeclarator> Declarators;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("(");
            foreach (var sq in SpecQualifierList)
            {
                sb.AppendFormat(" {0}", sq);
            }
            sb.Append(") (");
            foreach (var d in Declarators)
            {
                sb.AppendFormat(" {0}", d);
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

    public class StructDeclarator
    {
        public Declarator Declarator;
        public ConstExp FieldSize;
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("({0}", Declarator);
            if (FieldSize != null)
                sb.AppendFormat(" {0}", FieldSize);
            sb.Append(")");
            return sb.ToString();
        }
    }
    public abstract class Initializer 
    {
    }

    public class ListInitializer : Initializer
    {
        public List<Initializer> List;
    }

    public class ExpressionInitializer : Initializer
    {
        public CExpression Expression;
        public override string ToString() { return Expression.ToString(); }
    }

    public abstract class CExpression
    {
    }

    public class CIdentifier : CExpression
    {
        public string Name;
        public override string ToString() { return Name; }
    }
    
    public class CUnaryExpression : CExpression
    {
        public CTokenType Operation;
        public CExpression Expression;
        public override string ToString() { return string.Format("({0} {1})", Operation, Expression); }
    }

    public class CBinaryExpression : CExpression
    {
        public CTokenType Operation;
        public CExpression Left;
        public CExpression Right;
        public override string ToString() { return string.Format("({0} {1} {2})", Operation, Left, Right); }
    }

    public class AssignExpression : CExpression
    {
        public CTokenType AssingmentOp;
        public CExpression LValue;
        public CExpression RValue;
    }

    public class ConditionalExpression : CExpression
    {
        public CExpression Condition;
        public CExpression Consequent;
        public CExpression Alternative;
    }

    public class FunctionDefinition : Decl
    {
        public Declarator declarator;
        public List<Stat> compound_stat;
    }

    public abstract class Stat
    {
    }

    public class LabeledStat : Stat
    {
        public Label Label;
    }

    public class Label
    {
    }

    public class LineLabel : Label
    {
        public string Name;
    }
    public class CaseLabel : Label
    {
        public ConstExp Value;
    }
}
