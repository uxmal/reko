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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Hll.C
{
    public class CGrammar
    {
        public CExpression Const(object value)
        {
            return new ConstExp(value);
        }

        public virtual CExpression Bin(CTokenType op, CExpression left, CExpression right)
        {
            return new CBinaryExpression(op, left, right);
        }

        public CExpression Unary(CTokenType operation, CExpression expr)
        {
            return new CUnaryExpression(operation, expr);
        }

        public CIdentifier Id(string name)
        {
            return new CIdentifier(name);
        }

        public CExpression Application(CExpression fn, List<CExpression> arguments)
        {
            return new Application(fn, arguments);
        }

        public CExpression ArrayAccess(CExpression e, CExpression idx)
        {
            return new CArrayAccess(e, idx);
        }

        public CExpression MemberAccess(CExpression e, string fieldName)
        {
            return new MemberExpression(e, false, fieldName);
        }

        public CExpression PtrMemberAccess(CExpression e, string fieldName)
        {
            return new MemberExpression(e, true, fieldName);
        }

        public CExpression PostIncrement(CExpression e, CTokenType token)
        {
            return new IncrementExpression(token, false, e);
        }

        public CExpression Sizeof(CType type)
        {
            return new SizeofExpression(type);
        }

        public CExpression Sizeof(CExpression sexp)
        {
            return new SizeofExpression(sexp);
        }

        public CExpression PreIncrement(CTokenType token, CExpression uexpr)
        {
            throw new NotImplementedException();
        }

        public CExpression Cast(CType type, CExpression exp)
        {
            return new CastExpression(type, exp);
        }

        public CExpression Application(CExpression e) => throw new NotImplementedException();

        public DeclSpec StorageClass(CTokenType token)
        {
            return new StorageClassSpec {  Type = token };
        }

        public TypeQualifier TypeQualifier(CTokenType qualifier)
        {
            return new TypeQualifier { Qualifier = qualifier };
        }

        public FieldDeclarator FieldDeclarator(Declarator decl, CExpression? bitField)
        {
            return new FieldDeclarator(decl, bitField);
        }

        public TypeSpec SimpleType(CTokenType type)
        {
            return new SimpleTypeSpec { Type = type };
        }

        public TypeSpec TypeName(string name)
        {
            return new TypeDefName(name);
        }

        public Declarator IdDeclarator(string name)
        {
            return new IdDeclarator(name);
        }

        public Stat IfStatement(CExpression expr, Stat consequence, Stat? alternative)
        {
            return new IfStat(expr, consequence, alternative);
        }

        public Stat SwitchStatement(CExpression expr, Stat switchBody)
        {
            throw new NotImplementedException();
        }

        public Stat WhileStatement(CExpression expr, Stat whileBody)
        {
            return new WhileStat(expr, whileBody);
        }

        public Stat DoWhileStatement(Stat doBody, CExpression expr)
        {
            return new DoWhileStat(doBody, expr);
        }

        public Stat ForStatement(Stat? init, CExpression? test, CExpression? incr, Stat forBody)
        {
            return new ForStat(init, test, incr, forBody);
        }

        public Stat GotoStatement(string gotoLabel)
        {
            return new GotoStat(gotoLabel);
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
            return new ExprStat(null);
        }

        public Stat CompoundStatement(List<Stat> statements)
        {
            return new CompoundStatement(statements);
        }


        public Stat ReturnStatement(CExpression expr)
        {
            return new ReturnStat(expr);
        }

        public ParamDecl ParamDecl(List<CAttribute>? attrs, List<DeclSpec> dsl, Declarator? decl)
        {
            return new ParamDecl(attrs, dsl, decl, false);
        }

        public TypeSpec Enum(string? tag, List<Enumerator> enums)
        {
            return new EnumeratorTypeSpec(tag, enums);
        }

        public TypeSpec ComplexType(CTokenType token, int alignment, string tag, List<StructDecl> decls)
        {
            return new ComplexTypeSpec(
                token,
                tag,
                decls,
                alignment);
        }

        public Label DefaultCaseLabel()
        {
            throw new NotImplementedException();
        }

        public Stat LabeledStatement(Label label, Stat stat)
        {
            return new LabeledStat(label, stat);
        }

        public Label CaseLabel(CExpression constExpr)
        {
            return new CaseLabel(constExpr);
        }

        internal Label Label(string labelName)
        {
            return new LineLabel(labelName);
        }

        public InitDeclarator InitDeclarator(Declarator? decl, Initializer? init)
        {
            return new InitDeclarator(decl!, init);
        }

        public Initializer ListInitializer(List<Initializer> list)
        {
            throw new NotImplementedException();
        }

        public Initializer ExpressionInitializer(CExpression expr)
        {
            return new ExpressionInitializer(expr);
        }

        public Stat ExprStatement(CExpression expr)
        {
            return new ExprStat(expr);
        }

        public CExpression Conditional(CExpression cond, CExpression consequent, CExpression alternant)
        {
            return new ConditionalExpression(cond, consequent, alternant);
        }

        public StructDecl StructDecl(List<DeclSpec> sql, List<FieldDeclarator> decls, List<CAttribute>? attrs)
        {
            return new StructDecl(sql, decls, attrs);
        }

        public Declarator ArrayDeclarator(Declarator decl, CExpression? expr)
        {
            return new ArrayDeclarator(decl, expr);
        }

        public Declarator FunctionDeclarator(Declarator decl, List<ParamDecl> parameters)
        {
            return new FunctionDeclarator(decl, parameters);
        }

        public PointerDeclarator PointerDeclarator()
        {
            return new PointerDeclarator { };
        }

        public Declarator PointerDeclarator(Declarator? decl, List<TypeQualifier>? tqs)
        {
            return new PointerDeclarator { TypeQualifierList = tqs, Pointee = decl};
        }

        public Declarator ReferenceDeclarator(Declarator? decl, List<TypeQualifier>? tqs)
        {
            return new ReferenceDeclarator { TypeQualifierList = tqs, Referent = decl };
        }


        public Declarator CallConventionDeclarator(CTokenType conv, Declarator decl)
        {
            return new CallConventionDeclarator(conv, decl);
        }

        public Enumerator Enumerator(string id, CExpression? init)
        {
            return new Enumerator(id, init);
        }

        public ParamDecl Ellipsis()
        {
            return new ParamDecl(null, null, null, true);
        }

        public Decl Decl(List<CAttribute>? attrs, List<DeclSpec> list, List<InitDeclarator> listDecls)
        {
            return new Decl(attrs, list, listDecls);
        }

        internal Decl Decl(List<CAttribute>? attrs, List<DeclSpec> list, Declarator? decl)
        {
            return new Decl(
                attrs,
                list,
                new List<InitDeclarator> 
                {
                    InitDeclarator(decl, null)
                });
        }

        public Stat DeclStat(Decl decl, CExpression? initializer = null)
        {
            return new DeclStat(decl, initializer);
        }

        internal Decl FunctionDefinition(List<CAttribute>? attrs, List<DeclSpec> decl_spec_list, Declarator? declarator, List<Stat> statements)
        {
            return new FunctionDecl(
                Decl(attrs, decl_spec_list, declarator),
                statements,
                decl_spec_list        //$REVIEW: dupe?
            );
        }


        public DeclSpec ExtendedDeclspec(string name)
        {
            return new ExtendedDeclspec(name);
        }

    }
}
