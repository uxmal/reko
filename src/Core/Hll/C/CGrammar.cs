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

using System;
using System.Collections.Generic;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Factory class for creating C language constructs.
    /// </summary>
    public class CGrammar
    {
        /// <summary>
        /// Creates a constant.
        /// </summary>
        /// <param name="value">Constant value.</param>
        /// <returns>C constant.</returns>
        public CExpression Const(object value)
        {
            return new ConstExp(value);
        }

        /// <summary>
        /// Creates a binary expression.
        /// </summary>
        /// <param name="op">Operation type.</param>
        /// <param name="left">Left subexpression.</param>
        /// <param name="right">Right subexpression.</param>
        /// <returns>A C binary expression.</returns>
        public virtual CExpression Bin(CTokenType op, CExpression left, CExpression right)
        {
            return new CBinaryExpression(op, left, right);
        }

        /// <summary>
        /// Creates a unary expression.
        /// </summary>
        /// <param name="operation">Operation type.</param>
        /// <param name="expr">Operand.</param>
        /// <returns>A C unary expression.</returns>
        public CExpression Unary(CTokenType operation, CExpression expr)
        {
            return new CUnaryExpression(operation, expr);
        }

        /// <summary>
        /// Creates a C identifier.
        /// </summary>
        /// <param name="name">Identifier name.</param>
        /// <returns>A C identifier.</returns>
        public CIdentifier Id(string name)
        {
            return new CIdentifier(name);
        }

        /// <summary>
        /// Creates a C application.
        /// </summary>
        /// <param name="fn">Function.</param>
        /// <param name="arguments">Argument list.</param>
        /// <returns>A C application.</returns>
        public CExpression Application(CExpression fn, List<CExpression> arguments)
        {
            return new Application(fn, arguments);
        }

        /// <summary>
        /// Creates a C array access.
        /// </summary>
        /// <param name="e">Array expression.</param>
        /// <param name="idx">Index expression.</param>
        /// <returns>A C array access.</returns>
        public CExpression ArrayAccess(CExpression e, CExpression idx)
        {
            return new CArrayAccess(e, idx);
        }

        /// <summary>
        /// Creates a member access expression/
        /// </summary>
        /// <param name="e">Structure object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>A C member access.</returns>
        public CExpression MemberAccess(CExpression e, string fieldName)
        {
            return new MemberExpression(e, false, fieldName);
        }

        /// <summary>
        /// Creates a pointer-to-member access expression/
        /// </summary>
        /// <param name="e">Pointer to structure object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>A C member access.</returns>
        public CExpression PtrMemberAccess(CExpression e, string fieldName)
        {
            return new MemberExpression(e, true, fieldName);
        }

        /// <summary>
        /// Creates a post-increment or post-decrement expression.
        /// </summary>
        /// <param name="e">Expression to increment.</param>
        /// <param name="token">Increment or decrement token.</param>
        /// <returns>A post-inc/post-decrement expresssion.</returns>
        public CExpression PostIncrement(CExpression e, CTokenType token)
        {
            return new IncrementExpression(token, false, e);
        }

        /// <summary>
        /// Creates a <c>sizeof(type)</c> expression.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>A sizeof expression.</returns>
        public CExpression Sizeof(CType type)
        {
            return new SizeofExpression(type);
        }

        /// <summary>
        /// Creates a <c>sizeof(sexp)</c> expression.
        /// </summary>
        /// <param name="sexp"></param>
        /// <returns>A sizeof expression.</returns>
        public CExpression Sizeof(CExpression sexp)
        {
            return new SizeofExpression(sexp);
        }

        /// <summary>
        /// Creates a pre-increment or pre-decrement expression.
        /// </summary>
        /// <param name="token">Increment or decrement token.</param>
        /// <param name="uexpr">Expression to increment or decrement.</param>
        /// <returns>A pre-inc/pre-decrement expresssion.</returns>
        public CExpression PreIncrement(CTokenType token, CExpression uexpr)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a C type cast expression.
        /// </summary>
        /// <param name="type">Type to cast to.</param>
        /// <param name="exp">Operand.</param>
        /// <returns>A type cast expression.</returns>
        public CExpression Cast(CType type, CExpression exp)
        {
            return new CastExpression(type, exp);
        }

        /// <summary>
        /// Create a zero argument application.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>A new application.</returns>
        public CExpression Application(CExpression e) => Application(e, []);

        /// <summary>
        /// Creates a storage class specifier.
        /// </summary>
        /// <param name="token">Storage class.</param>
        /// <returns>A storage class specifier.</returns>
        public DeclSpec StorageClass(CTokenType token)
        {
            return new StorageClassSpec { Type = token };
        }

        /// <summary>
        /// Creates a C type quelifier.
        /// </summary>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <returns>A type qualifier.</returns>
        public TypeQualifier TypeQualifier(CTokenType qualifier)
        {
            return new TypeQualifier { Qualifier = qualifier };
        }

        /// <summary>
        /// Creates a C field declarator.
        /// </summary>
        /// <param name="decl">Base declarator.</param>
        /// <param name="bitField">Optional bitfield.</param>
        /// <returns>A field declarator.</returns>
        public FieldDeclarator FieldDeclarator(Declarator decl, CExpression? bitField)
        {
            return new FieldDeclarator(decl, bitField);
        }

        /// <summary>
        /// Creates a simple type specifier.
        /// </summary>
        /// <param name="type">Simple type.</param>
        /// <returns>A simple type specifier.</returns>
        public TypeSpec SimpleType(CTokenType type)
        {
            return new SimpleTypeSpec { Type = type };
        }

        /// <summary>
        /// Creates a type reference.
        /// </summary>
        /// <param name="name">Type name.</param>
        /// <returns>A type name reference.</returns>
        public TypeSpec TypeName(string name)
        {
            return new TypeDefName(name);
        }

        /// <summary>
        /// Creates an id declarator.
        /// </summary>
        /// <param name="name">Identifier name.</param>
        /// <returns>A declarator.</returns>
        public Declarator IdDeclarator(string name)
        {
            return new IdDeclarator(name);
        }

        /// <summary>
        /// Creates an <c>if</c> statement
        /// </summary>
        /// <param name="expr">Predicate.</param>
        /// <param name="consequence">Consequent statement.</param>
        /// <param name="alternative">Alternative statement.</param>
        /// <returns>An <c>if</c> statement.</returns>
        public Stat IfStatement(CExpression expr, Stat consequence, Stat? alternative)
        {
            return new IfStat(expr, consequence, alternative);
        }

        /// <summary>
        /// Creates a <c>switch</c> statement.
        /// </summary>
        /// <param name="expr">Switch condition.</param>
        /// <param name="switchBody">Switch body.</param>
        /// <returns>A <c>switch</c> statement.</returns>
        public Stat SwitchStatement(CExpression expr, Stat switchBody)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an <c>while</c> statement.
        /// </summary>
        /// <param name="expr">Predicate.</param>
        /// <param name="whileBody">Body of the while body.</param>
        /// <returns>A <c>while</c> statement.</returns>
        public Stat WhileStatement(CExpression expr, Stat whileBody)
        {
            return new WhileStat(expr, whileBody);
        }

        /// <summary>
        /// Creates an <c>do-while</c> statement.
        /// </summary>
        /// <param name="doBody">Body of the while body.</param>
        /// <param name="expr">Predicate.</param>
        /// <returns>A <c>do-while</c> statement.</returns>
        public Stat DoWhileStatement(Stat doBody, CExpression expr)
        {
            return new DoWhileStat(doBody, expr);
        }

        /// <summary>
        /// Creates an <c>for</c> statement.
        /// </summary>
        /// <param name="init">Initializing statement.</param>
        /// <param name="test">Predicate.</param>
        /// <param name="incr">Increment part of the for-loop.</param>
        /// <param name="forBody">Loop body.</param>
        /// <returns>A <c>for</c> statement.</returns>
        public Stat ForStatement(Stat? init, CExpression? test, CExpression? incr, Stat forBody)
        {
            return new ForStat(init, test, incr, forBody);
        }

        /// <summary>
        /// Creates a <c>goto</c> statement.
        /// </summary>
        /// <param name="gotoLabel">Target of the <c>goto</c> statement.</param>
        /// <returns>A <c>goto</c> statement.</returns>
        public Stat GotoStatement(string gotoLabel)
        {
            return new GotoStat(gotoLabel);
        }

        /// <summary>
        /// Creates a <c>continue</c> statement.
        /// </summary>
        /// <returns>A <c>continue</c> statement.
        /// </returns>
        public Stat ContinueStatement()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a <c>break</c> statement.
        /// </summary>
        /// <returns>A <c>break</c> statement.
        /// </returns>
        public Stat BreakStatement()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an empty statement.
        /// </summary>
        /// <returns>An empty statement.
        /// </returns>
        public Stat EmptyStatement()
        {
            return new ExprStat(null);
        }

        /// <summary>
        /// Creates a compound statement.
        /// </summary>
        /// <param name="statements">List of statements.</param>
        /// <returns>A compound statement.
        /// </returns>
        public Stat CompoundStatement(List<Stat> statements)
        {
            return new CompoundStatement(statements);
        }

        /// <summary>
        /// Creates a <c>return</c> statement.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns>A return statement.
        /// </returns>
        public Stat ReturnStatement(CExpression expr)
        {
            return new ReturnStat(expr);
        }

        /// <summary>
        /// Create a parameter declaration.
        /// </summary>
        /// <param name="attrs">Optional parameter attributes.</param>
        /// <param name="dsl">Parameter decl-specifications.</param>
        /// <param name="decl">Parameter declaration.</param>
        /// <returns>A parameter declaration.</returns>
        public ParamDecl ParamDecl(List<CAttribute>? attrs, List<DeclSpec> dsl, Declarator? decl)
        {
            return new ParamDecl(attrs, dsl, decl, false);
        }

        /// <summary>
        /// Create a C enum type.
        /// </summary>
        /// <param name="tag">Enum tag.</param>
        /// <param name="enums">Enumerator values.</param>
        /// <returns>A C enum type.</returns>
        public TypeSpec Enum(string? tag, List<Enumerator> enums)
        {
            return new EnumeratorTypeSpec(tag, enums);
        }

        /// <summary>
        /// Creates a complex C type.
        /// </summary>
        /// <param name="token"><see cref="CTokenType.Struct"/> or <see cref="CTokenType.Union"/>.</param>
        /// <param name="alignment">Alignment.</param>
        /// <param name="tag">Type tag.</param>
        /// <param name="decls">Field declarations.</param>
        /// <returns>Complex C type.
        /// </returns>
        public TypeSpec ComplexType(CTokenType token, int alignment, string tag, List<StructDecl> decls)
        {
            return new ComplexTypeSpec(
                token,
                tag,
                decls,
                alignment);
        }

        /// <summary>
        /// Creates a <c>default</c> case label.
        /// </summary>
        /// <returns>A default case label.</returns>
        public Label DefaultCaseLabel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a labeled statement.
        /// </summary>
        /// <param name="label">Label of the statement.</param>
        /// <param name="stat">Statement to be labeled.</param>
        /// <returns>A labeled statement.
        /// </returns>
        public Stat LabeledStatement(Label label, Stat stat)
        {
            return new LabeledStat(label, stat);
        }

        /// <summary>
        /// Creates a C case label.
        /// </summary>
        /// <param name="constExpr">Case value.</param>
        /// <returns>A case label.
        /// </returns>
        public Label CaseLabel(CExpression constExpr)
        {
            return new CaseLabel(constExpr);
        }

        /// <summary>
        /// Creates a C label.
        /// </summary>
        /// <param name="labelName">Label name.</param>
        /// <returns>A label.</returns>
        internal Label Label(string labelName)
        {
            return new LineLabel(labelName);
        }

        /// <summary>
        /// Creates an init declarator.
        /// </summary>
        /// <param name="decl">Declarator.</param>
        /// <param name="init">Initializer.</param>
        /// <returns>An init declarator.
        /// </returns>
        public InitDeclarator InitDeclarator(Declarator? decl, Initializer? init)
        {
            return new InitDeclarator(decl!, init);
        }

        /// <summary>
        /// Creates a list initializer.
        /// </summary>
        /// <param name="list">Initializers.</param>
        /// <returns>A list initializer.</returns>
        public Initializer ListInitializer(List<Initializer> list)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an expression initializer.
        /// </summary>
        /// <param name="expr">Expression to use as initializer.</param>
        /// <returns>An expression initializer.
        /// </returns>
        public Initializer ExpressionInitializer(CExpression expr)
        {
            return new ExpressionInitializer(expr);
        }

        /// <summary>
        /// Creates an expression statement.
        /// </summary>
        /// <param name="expr">Expression.</param>
        /// <returns>An expression statement.</returns>
        public Stat ExprStatement(CExpression expr)
        {
            return new ExprStat(expr);
        }

        /// <summary>
        /// Creates a conditional expression.
        /// </summary>
        /// <param name="cond">Predicate expression.</param>
        /// <param name="consequent">Consequent expression.</param>
        /// <param name="alternant">Alternate expression.</param>
        /// <returns>Conidtional expression.</returns>
        public ConditionalExpression Conditional(CExpression cond, CExpression consequent, CExpression alternant)
        {
            return new ConditionalExpression(cond, consequent, alternant);
        }

        /// <summary>
        /// Creates a <c>struct</c> declaration.
        /// </summary>
        /// <param name="sql">Specifier qualifier list.</param>
        /// <param name="decls">Field declarators.</param>
        /// <param name="attrs">Attributes.</param>
        /// <returns>A struct declaration.
        /// </returns>
        public StructDecl StructDecl(List<DeclSpec> sql, List<FieldDeclarator> decls, List<CAttribute>? attrs)
        {
            return new StructDecl(sql, decls, attrs);
        }

        /// <summary>
        /// Creates an array declarator.
        /// </summary>
        /// <param name="decl">Declarator.</param>
        /// <param name="expr">Array expression.</param>
        /// <returns>An array declarator.
        /// </returns>
        public Declarator ArrayDeclarator(Declarator decl, CExpression? expr)
        {
            return new ArrayDeclarator(decl, expr);
        }

        /// <summary>
        /// Creates a function declarator.
        /// </summary>
        /// <param name="decl">Declarator.</param>
        /// <param name="parameters">Parameter declarators.</param>
        /// <returns>A function declarator.</returns>
        public Declarator FunctionDeclarator(Declarator decl, List<ParamDecl> parameters)
        {
            return new FunctionDeclarator(decl, parameters);
        }

        /// <summary>
        /// Creates a pointer declarator.
        /// </summary>
        /// <returns>A pointer declarator.
        /// </returns>
        public PointerDeclarator PointerDeclarator()
        {
            return new PointerDeclarator { };
        }

        /// <summary>
        /// Creates a pointer declarator.
        /// </summary>
        /// <param name="decl">Base declarator.</param>
        /// <param name="tqs">Type qualifiers.</param>
        /// <returns>A pointer declarator.
        /// </returns>
        public Declarator PointerDeclarator(Declarator? decl, List<TypeQualifier>? tqs)
        {
            return new PointerDeclarator { TypeQualifierList = tqs, Pointee = decl};
        }

        /// <summary>
        /// Creates a reference declarator.
        /// </summary>
        /// <param name="decl">Base declarator.</param>
        /// <param name="tqs">Type qualifiers.</param>
        /// <returns>A reference declarator.
        /// </returns>
        public Declarator ReferenceDeclarator(Declarator? decl, List<TypeQualifier>? tqs)
        {
            return new ReferenceDeclarator { TypeQualifierList = tqs, Referent = decl };
        }

        /// <summary>
        /// Calling convention declarator.
        /// </summary>
        /// <param name="conv">Calling convention.</param>
        /// <param name="decl">Base declarator.</param>
        /// <returns></returns>
        public Declarator CallConventionDeclarator(CTokenType conv, Declarator decl)
        {
            return new CallConventionDeclarator(conv, decl);
        }

        /// <summary>
        /// Creates an enumerator in an enumerator type.
        /// </summary>
        /// <param name="id">Name of the enumerator.</param>
        /// <param name="init">Optional initializer.</param>
        /// <returns>An enumerator.</returns>
        public Enumerator Enumerator(string id, CExpression? init)
        {
            return new Enumerator(id, init);
        }

        /// <summary>
        /// Creates an ellipsis parameter.
        /// </summary>
        /// <returns>An ellipsis parameter.
        /// </returns>
        public ParamDecl Ellipsis()
        {
            return new ParamDecl(null, null, null, true);
        }

        /// <summary>
        /// Creates a declaration.
        /// </summary>
        /// <param name="attrs">Optional attributes.</param>
        /// <param name="list">Declarator specifiers.</param>
        /// <param name="listDecls">Init declarators.</param>
        /// <returns>Declaration.</returns>
        public Decl Decl(List<CAttribute>? attrs, List<DeclSpec> list, List<InitDeclarator> listDecls)
        {
            return new Decl(attrs, list, listDecls);
        }

        /// <summary>
        /// Creates a declaration.
        /// </summary>
        /// <param name="attrs">Optional attributes.</param>
        /// <param name="list">Declarator specifiers.</param>
        /// <param name="decl">Declarator.</param>
        /// <returns>Declaration.</returns>
        public Decl Decl(List<CAttribute>? attrs, List<DeclSpec> list, Declarator? decl)
        {
            return new Decl(
                attrs,
                list,
                new List<InitDeclarator> 
                {
                    InitDeclarator(decl, null)
                });
        }

        /// <summary>
        /// Creates a declaration statement.
        /// </summary>
        /// <param name="decl">Declaration.</param>
        /// <param name="initializer">Initializing expression.</param>
        /// <returns>Declaration.</returns>
        public Stat DeclStat(Decl decl, CExpression? initializer = null)
        {
            return new DeclStat(decl, initializer);
        }

        /// <summary>
        /// Creates a function definition.
        /// </summary>
        /// <param name="attrs">Optional attributes.</param>
        /// <param name="decl_spec_list">Decl spec list for return value.</param>
        /// <param name="declarator">Declarator.</param>
        /// <param name="statements">Function body.</param>
        /// <returns>A function definition.
        /// </returns>
        internal Decl FunctionDefinition(List<CAttribute>? attrs, List<DeclSpec> decl_spec_list, Declarator? declarator, List<Stat> statements)
        {
            return new FunctionDecl(
                Decl(attrs, decl_spec_list, declarator),
                statements,
                decl_spec_list        //$REVIEW: dupe?
            );
        }

        /// <summary>
        /// Creates an extended declspec.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>An extended declspec.
        /// </returns>
        public DeclSpec ExtendedDeclspec(string name)
        {
            return new ExtendedDeclspec(name);
        }

    }
}
