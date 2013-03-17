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

namespace Decompiler.Tools.C2Xml
{
    public class CGrammar
    {
        public CExpression Const(object value)
        {
            return new ConstExp { Const = value };
        }

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

        public CExpression Application(CExpression e, List<CExpression> list)
        {
            throw new NotImplementedException();
        }

        public CExpression ArrayAccess(CExpression e, CExpression idx)
        {
            throw new NotImplementedException();
        }

        public CExpression MemberAccess(CExpression e, string id)
        {
            throw new NotImplementedException();
        }

        public CExpression PtrMemberAccess(CExpression e, string id)
        {
            throw new NotImplementedException();
        }

        public CExpression PostIncrement(CExpression e, CTokenType token)
        {
            throw new NotImplementedException();
        }

        public CExpression Sizeof(CType type)
        {
            throw new NotImplementedException();
        }

        public CExpression Sizeof(CExpression sexp)
        {
            throw new NotImplementedException();
        }

        public CExpression PreIncrement(CTokenType token, CExpression uexpr)
        {
            throw new NotImplementedException();
        }

        public CExpression Cast(CType type, CExpression exp)
        {
            return new CastExpression { Type = type, Expression = exp };
        }

        public CExpression Application(CExpression e)
        {
            throw new NotImplementedException();
        }

        public DeclSpec StorageClass(CTokenType token)
        {
            return new StorageClassSpec {  Type = token };
        }

        public TypeQualifier TypeQualifier(CTokenType qualifier)
        {
            return new TypeQualifier { Qualifier = qualifier };
        }

        public FieldDeclarator FieldDeclarator(Declarator decl, CExpression bitField)
        {
            return new FieldDeclarator { Declarator = decl, FieldSize = bitField };
        }

        public TypeSpec SimpleType(CTokenType type)
        {
            return new SimpleTypeSpec { Type = type };
        }

        public TypeSpec TypeName(string name)
        {
            return new TypeDefName { Name = name };
        }

        public Declarator IdDeclarator(string name)
        {
            return new IdDeclarator { Name = name };
        }

        public PointerDeclarator PointerDeclarator()
        {
            return new PointerDeclarator { };
        }

        public Stat IfStatement(CExpression expr, Stat consequence, Stat alternative)
        {
            throw new NotImplementedException();
        }

        public Stat SwitchStatement(CExpression expr, Stat switchBody)
        {
            throw new NotImplementedException();
        }

        public Stat WhileStatement(CExpression expr, Stat switchBody)
        {
            throw new NotImplementedException();
        }

        public Stat DoWhileStatement(Stat doBody, CExpression expr)
        {
            throw new NotImplementedException();
        }

        public Stat ForStatement(Stat init, CExpression test, CExpression  incr, Stat forBody)
        {
            return new ForStat
            {
                Initializer = init,
                Test = test,
                Update = incr,
                Body = forBody
            };
        }

        public Stat GotoStatement(string gotoLabel)
        {
            throw new NotImplementedException();
        }

        public Stat ContinueStatement()
        {
            throw new NotImplementedException();
        }

        public Stat BreakStatement()
        {
            throw new NotImplementedException();
        }

        public Stat EmptyStatement()
        {
            return new ExprStat();
        }

        public Stat ReturnStatement(CExpression expr)
        {
            return new ReturnStat { Expression = expr };
        }

        public ParamDecl ParamDecl(List<DeclSpec> dsl, Declarator decl)
        {
            return new ParamDecl
            {
                DeclSpecs = dsl,
                Declarator = decl,
            };
        }

        public TypeSpec Enum(string tag, List<Enumerator> enums)
        {
            return new EnumeratorTypeSpec
            {
                Tag = tag,
                Enums = enums,
            };
        }

        public TypeSpec ComplexType(CTokenType token, string tag, List<StructDecl> decls)
        {
            return new ComplexTypeSpec
            {
                Type = token,
                Name = tag,
                DeclList = decls,
            };
        }

        public Label DefaultCaseLabel()
        {
            throw new NotImplementedException();
        }

        public Stat LabeledStatement(Label label, Stat stat)
        {
            return new LabeledStat {
                Label = label,
                Stat = stat,
            };
        }

        public Label CaseLabel(CExpression constExpr)
        {
            return new CaseLabel { Value = constExpr };
        }

        internal Label Label(string p)
        {
            throw new NotImplementedException();
        }

        public InitDeclarator InitDeclarator(Declarator decl, Initializer init)
        {
            return new InitDeclarator { Declarator = decl, Init = init };
        }

        internal Initializer ListInitializer(List<Initializer> list)
        {
            throw new NotImplementedException();
        }

        internal Initializer ExpressionInitializer(CExpression expr)
        {
            return new ExpressionInitializer { Expression = expr };
        }

        public Stat ExprStatement(CExpression expr)
        {
            return new ExprStat { Expression = expr };
        }

        internal CExpression Conditional(CExpression cond, CExpression consequent, CExpression alternant)
        {
            throw new NotImplementedException();
        }

        public StructDecl StructDecl(List<DeclSpec> sql, List<FieldDeclarator> decls)
        {
            return new StructDecl { SpecQualifierList = sql, FieldDeclarators = decls };
        }

        public Declarator ArrayDeclarator(Declarator decl, CExpression expr)
        {
            return new ArrayDeclarator { Declarator = decl, Size = expr };
        }

        public Declarator FunctionDeclarator(Declarator decl, List<ParamDecl> parameters)
        {
            return new FunctionDeclarator { Declarator = decl, Parameters = parameters };
        }

        public Declarator PointerDeclarator(Declarator decl, List<TypeQualifier> tqs)
        {
            return new PointerDeclarator { TypeQualifierList = tqs, Pointee = decl};
        }

        public Enumerator Enumerator(string id, CExpression init)
        {
            return new Enumerator { Name = id, Value = init };
        }

        public ParamDecl Ellipsis()
        {
            throw new NotImplementedException();
        }

        internal Decl Decl(List<DeclSpec> list, List<InitDeclarator> listDecls)
        {
            return new Decl
            {
                decl_specs = list,
                init_declarator_list = listDecls
            };
        }

        internal Decl Decl(List<DeclSpec> list, Declarator decl)
        {
            return new Decl
            {
                decl_specs = list,
                init_declarator_list = new List<InitDeclarator> 
                {
                    new InitDeclarator {
                        Declarator = decl,
                        Init= null
                    }
                }
            };
        }

        public Stat DeclStat(Decl decl)
        {
            return new DeclStat { Declaration = decl };
        }

        internal Decl FunctionDefinition(List<DeclSpec> decl_spec_list, Declarator declarator, List<Stat> statements)
        {
            return new FunctionDecl
            {
                Signature = Decl(decl_spec_list, declarator),
                Body = statements
            };
                
        }
    }

}
