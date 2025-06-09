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
using System.Linq;
using System.Text;
using Reko.Core.Types;

namespace Reko.Core.Output
{
    /// <summary>
    /// Formats a type reference in C/C++ style.
    /// </summary>
    public class TypeReferenceFormatter
    {
        private bool declaration;
        private string? declaredName;
        private int depth;//$BUG: used to avoid infinite recursion
        private bool wantSpace;

        /// <summary>
        /// Constructs a <see cref="TypeReferenceFormatter"/> instance.
        /// </summary>
        /// <param name="writer">Output sink.</param>
        public TypeReferenceFormatter(Formatter writer)
        {
            this.Formatter = writer;
        }

        /// <summary>
        /// Output sink being used.
        /// </summary>
        public Formatter Formatter { get; }

        /// <summary>
        /// Writes a type reference to the output sink.
        /// </summary>
        /// <param name="dt">Data type reference.</param>
        public void WriteTypeReference(DataType dt)
        {
            this.declaration = false;
            this.declaredName = null;
            TypeName(dt);
        }

        /* declaration:
                declaration-specifiers init-declarator-list(opt) ;  */

        /// <summary>
        /// Writes a type declaration to the output sink.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="name"></param>
        public void WriteDeclaration(DataType dt, string name)
        {
            this.declaration = true;
            this.declaredName = name;
            DeclarationSpecifiers(dt);
            InitDeclarator(dt);
        }

        /* init-declarator:
      declarator:
      declarator = initializer   */

        void InitDeclarator(DataType t)
        {
            Declarator(t);
            //if (DECL_INITIAL(t))
            //{
            //    tree init = DECL_INITIAL(t);
            //    /* This C++ bit is handled here because it is easier to do so.
            //       In templates, the C++ parser builds a TREE_LIST for a
            //       direct-initialization; the TREE_PURPOSE is the variable to
            //       initialize and the TREE_VALUE is the initializer.  */
            //    if (TREE_CODE(init) == TREE_LIST)
            //    {
            //        pp_c_left_paren(pp);
            //        pp_expression(TREE_VALUE(init));
            //        pp_right_paren(pp);
            //    }
            //    else
            //    {
            //        pp_space(pp);
            //        pp_equal(pp);
            //        pp_space(pp);
            //        pp_c_initializer(init);
            //    }
            //}
        }

        void CvQualifier(string cv)
        {
            Formatter.Write(' ');
            Formatter.Write(cv);
        }

        void SpaceForPointerOperator(DataType t)
        {
            if (t is Pointer p)
            {
                if (t is not ArrayType && t is not FunctionType)
                    Formatter.Write(' ');
            }
            else
                Formatter.Write(' ');
        }


        /* Declarations.  */

        /* C++ cv-qualifiers are called type-qualifiers in C.  Print out the
           cv-qualifiers of T.  If T is a declaration then it is the cv-qualifier
           of its type.  Take care of possible extensions.

           type-qualifier-list:
               type-qualifier
               type-qualifier-list type-qualifier

           type-qualifier:
               const
               restrict                              -- C99
               __restrict__                          -- GNU C
               volatile    */

        //$REVIEW: we don't do cv-qualifiers... yet?
        void TypeQualifierList(DataType t)
        {
            //type_qual qualifiers;

            //if (!TYPE_P(t))
            //    t = TREE_TYPE(t);

            //qualifiers = TYPE_QUALS(t);
            //if ((qualifiers & type_qual.CONST) != 0)
            //    pp_c_cv_qualifier("const");
            //if ((qualifiers & type_qual.VOLATILE) != 0)
            //    pp_c_cv_qualifier("volatile");
            //if ((qualifiers & type_qual.RESTRICT) != 0)
            //    pp_c_cv_qualifier(flag_isoc99 ? "restrict" : "__restrict__");
        }

        /* pointer:
              * type-qualifier-list(opt)
              * type-qualifier-list(opt) pointer  */

        void Pointer(Pointer t)
        {
            if (t.Pointee is Pointer ptPointee)
                Pointer(ptPointee);
            WriteSpace();
            Formatter.Write('*');
            TypeFormatter.WriteQualifier(t.Qualifier, Formatter);
            TypeQualifierList(t);
        }

        void MemberPointer(MemberPointer m)
        {
            if (m.Pointee is Pointer ptPointee)
                Pointer(ptPointee);
            if (m.Pointee is MemberPointer mpPointee)
                MemberPointer(mpPointee);
            Formatter.Write(' ');
            var baseType = StripPointerOperator(m.BasePointer);
            Formatter.Write(baseType.Name);
            Formatter.Write("::*");
            TypeQualifierList(m);
        }

        void ReferenceTo(ReferenceTo r)
        {
            WriteSpace();
            Formatter.Write('&');
            TypeQualifierList(r);
        }

        /* type-specifier:
              void
              char
              short
              int
              long
              float
              double
              signed
              unsigned
              _Bool                          -- C99
              _Complex                       -- C99
              _Imaginary                     -- C99
              struct-or-union-specifier
              enum-specifier
              typedef-name.

          GNU extensions.
          simple-type-specifier:
              __complex__
              __vector__   */

        void TypeSpecifier(DataType t)
        {
            TypeFormatter.WriteQualifier(t.Qualifier, Formatter);
            switch (t)
            {
            case UnknownType:
                Formatter.Write("<unknown>");
                return;
            case EquivalenceClass:
                Formatter.Write(t.Name);
                wantSpace = true;
                return;
            case PrimitiveType pt:
                //case tree_code.VOID_TYPE:
                //case tree_code.BOOLEAN_TYPE:
                //case tree_code.CHAR_TYPE:
                //case tree_code.INTEGER_TYPE:
                //case tree_code.REAL_TYPE:
                //if (TYPE_NAME(t) is not null)
                //    t = TYPE_NAME(t);
                //else
                //    t = c_common_type_for_mode(TYPE_MODE(t), TREE_UNSIGNED(t));
                WritePrimitiveTypeName(pt);
                //if (declaration && !string.IsNullOrEmpty(declaredName))
                //    fmt.Write(' ');
                wantSpace = true;
                return;
            case VoidType vt:
                WriteVoidType(vt);
                wantSpace = true;
                return;
            case UnionType _:
                Formatter.WriteKeyword("union");
                Formatter.Write(" ");
                break;
            case StructureType _:
                Formatter.WriteKeyword("struct");
                Formatter.Write(" ");
                break;
            case EnumType _:
                Formatter.WriteKeyword("enum");
                Formatter.Write(" ");
                break;
            }
            if (string.IsNullOrEmpty(t.Name))
                Formatter.Write("<anonymous>");
            else
                Formatter.Write(t.Name);
            wantSpace = true;
        }

        /// <summary>
        /// Writes the name of a primitive type to the output sink.
        /// </summary>
        /// <param name="t">Primitive type.</param>
        public virtual void WritePrimitiveTypeName(PrimitiveType t)
        {
            Formatter.WriteType(t.Name, t);
            wantSpace = true;
        }

        /// <summary>
        /// Writes the name of a void type to the output sink.
        /// </summary>
        /// <param name="t">Void type.</param>
        public virtual void WriteVoidType(VoidType t)
        {
            Formatter.WriteType(t.Name, t);
            wantSpace = true;
        }

        private void WriteSpace()
        {
            if (wantSpace)
            {
                Formatter.Write(' ');
                wantSpace = false;
            }
        }

        /* specifier-qualifier-list:
              type-specifier specifier-qualifier-list-opt
              type-qualifier specifier-qualifier-list-opt


          Implementation note:  Because of the non-linearities in array or
          function declarations, this routine prints not just the
          specifier-qualifier-list of such entities or types of such entities,
          but also the 'pointer' production part of their declarators.  The
          remaining part is done by pp_declarator or pp_c_abstract_declarator.  */

        void SpecifierQualifierList(DataType t)
        {
            if (this.depth > 50) //$BUG: used to avoid infinite recursion
                return;
            ++this.depth;
            if (!(t is Pointer))
                TypeQualifierList(t);
            var pt = t as Pointer;
            var mp = t as MemberPointer;
            var rf = t as ReferenceTo;
            if (pt is not null || mp is not null || rf is not null)
            {
                // Get the types-specifier of this type.  
                DataType pointee = StripPointerOperator(
                    pt is not null 
                        ? pt.Pointee
                        : mp is not null 
                            ? mp.Pointee
                            : rf!.Referent);
                SpecifierQualifierList(pointee);
                if (pointee is ArrayType || pointee is FunctionType)
                {
                    Formatter.Write(" (");
                    wantSpace = false;
                }
                if (pt is not null)
                    Pointer(pt);
                else if (mp is not null)
                    MemberPointer(mp);
                else if (rf is not null)
                    ReferenceTo(rf); 
                --this.depth;
                return;
            }

            if (t is FunctionType ft && ft.ReturnValue is not null)
            {
                SpecifierQualifierList(ft.ReturnValue.DataType);
                --this.depth;
                return;
            }
            if (t is ArrayType at)
            {
                SpecifierQualifierList(at.ElementType);
            }
            else
            {
                TypeSpecifier(t);
            }
            --this.depth;
        }

        private DataType StripPointerOperator(DataType dt)
        {
            var pt = dt as Pointer;
            var mp = dt as MemberPointer;
            while (pt is not null || mp is not null)
            {
                if (pt is not null)
                    dt = pt.Pointee;
                else
                    dt = mp!.Pointee;
                pt = dt as Pointer;
                mp = dt as MemberPointer;
            }
            if (dt is EquivalenceClass eq && eq.DataType is not null)
                dt = eq.DataType;
            return dt;
        }

        /* parameter-type-list:
              parameter-list
              parameter-list , ...

           parameter-list:
              parameter-declaration
              parameter-list , parameter-declaration

           parameter-declaration:
              declaration-specifiers declarator
              declaration-specifiers abstract-declarator(opt)   */

        void ParameterTypeList(FunctionType? ft)
        {
            var name = declaredName;
            Formatter.Write('(');
            if (ft is null || ft.Parameters is null || ft.Parameters.Length == 0)
            {
                // fmt.Write("void");      // In C, 0-parameter functions use 'void'
            }
            else
            {
                bool first = true;
                for (int i = 0; i < ft.Parameters.Length; ++i)
                {
                    if (!first)
                        Formatter.Write(", ");
                    first = false;
                    declaredName = ft.Parameters[i].Name;
                    DeclarationSpecifiers(ft.Parameters[i].DataType);
                    if (declaration && !string.IsNullOrEmpty(declaredName))
                        Declarator(ft.Parameters[i].DataType);
                    else
                        AbstractDeclarator(ft.Parameters[i].DataType);
                }
            }
            Formatter.Write(')');
            declaredName = name;
        }

        /* abstract-declarator:
              pointer
              pointer(opt) direct-abstract-declarator  */

        void AbstractDeclarator(DataType dt)
        {
            if (this.depth > 50)
                return;         //$BUG: discover cause of the deep recursion?
            ++this.depth;
            if (dt is Pointer pt)
            {
                var pointee = pt.Pointee;
                if (pointee is EquivalenceClass eq && eq.DataType is not null)
                    pointee = eq.DataType;
                if (pointee is ArrayType ||
                    pointee is FunctionType)
                    Formatter.Write(')');
                dt = pointee;
            }
            DirectAbstractDeclarator(dt);
            --this.depth;
        }

        /* direct-abstract-declarator:
              ( abstract-declarator )
              direct-abstract-declarator(opt) [ assignment-expression(opt) ]
              direct-abstract-declarator(opt) [ * ]
              direct-abstract-declarator(opt) ( parameter-type-list(opt) )  */

        void DirectAbstractDeclarator(DataType t)
        {
            if (t is Pointer)
            {
                AbstractDeclarator(t);
            }
            else if (t is FunctionType ft)
            {
                ParameterTypeList(ft);
                //DirectAbstractDeclarator(ft);
            }
            else if (t is ArrayType at)
            {
                Formatter.Write('[');
                if (at.Length != 0)
                {
                    Formatter.Write(at.Length);
                }
                Formatter.Write(']');
                DirectAbstractDeclarator(at.ElementType);
            }
            else
            {
                return;
            }
        }

        /* type-name:
              specifier-qualifier-list  abstract-declarator(opt)  */

        void TypeName(DataType t)
        {
            SpecifierQualifierList(t);
            AbstractDeclarator(t);
        }

        /* storage-class-specifier:
              typedef
              extern
              static
              auto
              register  */

        //void StorageClassSpecifier(tree t)
        //{
        //    //if (TREE_CODE(t) == TYPE_DECL)
        //    //    fmt.Write("typedef");
        //    //else if (DECL_P(t))
        //    //{
        //    //    if (DECL_REGISTER(t))
        //    //        fmt.Write("register");
        //    //    else if (TREE_STATIC(t) && TREE_CODE(t) == VAR_DECL)
        //    //        fmt.Write("static");
        //    //}
        //}

        /* function-specifier:
              inline   */

        void FunctionSpecifier(DataType dt)
        {
        //    if (TREE_CODE(t) == tree_code.FUNCTION_DECL && DECL_DECLARED_INLINE_P(t))
        //        fmt.Write("inline");
        }

        /* declaration-specifiers:
              storage-class-specifier declaration-specifiers(opt)
              type-specifier declaration-specifiers(opt)
              type-qualifier declaration-specifiers(opt)
              function-specifier declaration-specifiers(opt)  */

        void DeclarationSpecifiers(DataType dt)
        {
            // StorageClassSpecifier(t);
            FunctionSpecifier(dt);
            SpecifierQualifierList(dt);
        }

        /* direct-declarator
              identifier
              ( declarator )
              direct-declarator [ type-qualifier-list(opt) assignment-expression(opt) ]
              direct-declarator [ static type-qualifier-list(opt) assignment-expression(opt)]
              direct-declarator [ type-qualifier-list static assignment-expression ]
              direct-declarator [ type-qualifier-list * ]
              direct-declaratpr ( parameter-type-list )
              direct-declarator ( identifier-list(opt) )  */

        void DirectDeclarator(DataType dt)
        {
            if (declaration)
            {
                if (!string.IsNullOrEmpty(this.declaredName))
                {
                    SpaceForPointerOperator(dt);
                    Formatter.Write(this.declaredName ?? "");
                }
                AbstractDeclarator(dt);
                return;
            }
            if (dt is ArrayType at)
            {
                AbstractDeclarator(at.ElementType);
                return;
            }
            if (dt is FunctionType ft)
            {
                ParameterTypeList(ft);
                if (ft.ReturnValue is not null)
                {
                    AbstractDeclarator(ft.ReturnValue.DataType);
                }
                return;
            }
        }

        void DirectDeclarator(FunctionType sig)
        {
            SpaceForPointerOperator(sig.ReturnValue!.DataType);
            Formatter.Write(declaredName ?? "");
            ParameterTypeList(null);
            AbstractDeclarator(sig.ReturnValue.DataType);
        }

        /* declarator:
              pointer(opt)  direct-declarator   */

        void Declarator(DataType dt)
        {
            //if (dt is PrimitiveType || dt is EnumType ||
            //    dt is StructureType || dt is UnionType)
            //{
            //    return;
            //}
            DirectDeclarator(dt);
        }
    }
}
