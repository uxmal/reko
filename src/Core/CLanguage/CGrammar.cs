#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

namespace Reko.Core.CLanguage
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

        public CExpression Application(CExpression fn, List<CExpression> list)
        {
            return new Application { 
                Function = fn, 
                Arguments = list
            };
        }

        public CExpression ArrayAccess(CExpression e, CExpression idx)
        {
            throw new NotImplementedException();
        }

        public CExpression MemberAccess(CExpression e, string fieldName)
        {
            return new MemberExpression
            {
                Expression = e,
                Dereference = false,
                FieldName = fieldName,
            };
        }

        public CExpression PtrMemberAccess(CExpression e, string fieldName)
        {
            return new MemberExpression
            {
                Expression = e,
                Dereference = true,
                FieldName = fieldName,
            };
        }

        public CExpression PostIncrement(CExpression e, CTokenType token)
        {
            return new IncrementExpression
            {
                Expression = e,
                Incrementor = token,
                Prefix = false
            };
        }

        public CExpression Sizeof(CType type)
        {
            return new SizeofExpression { Type = type };
        }

        public CExpression Sizeof(CExpression sexp)
        {
            return new SizeofExpression { Expression = sexp };
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

        public Stat IfStatement(CExpression expr, Stat consequence, Stat alternative)
        {
            throw new NotImplementedException();
        }

        public Stat SwitchStatement(CExpression expr, Stat switchBody)
        {
            throw new NotImplementedException();
        }

        public Stat WhileStatement(CExpression expr, Stat whileBody)
        {
            return new WhileStat
            {
                Expression = expr,
                Body = whileBody
            };
        }

        public Stat DoWhileStatement(Stat doBody, CExpression expr)
        {
            return new DoWhileStat
            {
                Body = doBody,
                Expression = expr,
            };
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

        public Stat CompoundStatement(List<Stat> statements)
        {
            return new CompoundStatement
            {
                Statements = statements,
            };
        }


        public Stat ReturnStatement(CExpression expr)
        {
            return new ReturnStat { Expression = expr };
        }

        public ParamDecl ParamDecl(List<CAttribute> attrs, List<DeclSpec> dsl, Declarator decl)
        {
            return new ParamDecl
            {
                Attributes = attrs,
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

        public TypeSpec ComplexType(CTokenType token, int alignment, string tag, List<StructDecl> decls)
        {
            return new ComplexTypeSpec
            {
                Type = token,
                Name = tag,
                Alignment = alignment,
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

        public CExpression Conditional(CExpression cond, CExpression consequent, CExpression alternant)
        {
            return new ConditionalExpression
            {
                Condition = cond,
                Consequent = consequent,
                Alternative = alternant,
            };
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

        public PointerDeclarator PointerDeclarator()
        {
            return new PointerDeclarator { };
        }

        public Declarator PointerDeclarator(Declarator decl, List<TypeQualifier> tqs)
        {
            return new PointerDeclarator { TypeQualifierList = tqs, Pointee = decl};
        }

        public Declarator ReferenceDeclarator(Declarator decl, List<TypeQualifier> tqs)
        {
            return new ReferenceDeclarator { TypeQualifierList = tqs, Referent = decl };
        }


        public Declarator CallConventionDeclarator(CTokenType conv, Declarator decl)
        {
            return new CallConventionDeclarator
            {
                Convention = conv,
                Declarator = decl,
            };
        }

        public Enumerator Enumerator(string id, CExpression init)
        {
            return new Enumerator { Name = id, Value = init };
        }

        public ParamDecl Ellipsis()
        {
            return new ParamDecl
            {
                IsEllipsis = true,
            };
        }

        internal Decl Decl(List<CAttribute> attrs, List<DeclSpec> list, List<InitDeclarator> listDecls)
        {
            return new Decl
            {
                attribute_list = attrs,
                decl_specs = list,
                init_declarator_list = listDecls
            };
        }

        internal Decl Decl(List<CAttribute> attrs, List<DeclSpec> list, Declarator decl)
        {
            return new Decl
            {
                attribute_list = attrs,
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

        internal Decl FunctionDefinition(List<CAttribute> attrs, List<DeclSpec> decl_spec_list, Declarator declarator, List<Stat> statements)
        {
            return new FunctionDecl
            {
                decl_specs = decl_spec_list,        //$REVIEW: dupe?
                Signature = Decl(attrs, decl_spec_list, declarator),
                Body = statements
            };
                
        }


        public DeclSpec ExtendedDeclspec(string s)
        {
            return new ExtendedDeclspec
            {
                Name = s,
            };
        }

    }
}
