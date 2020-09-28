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
using Reko.Core.Types;

namespace Reko.Core.Output
{
    public class TypeReferenceFormatter
    {
        private Formatter fmt;
        private bool declaration;
        private string declaredName;
        private int depth;//$BUG: used to avoid infinite recursion
        private bool wantSpace;

        public TypeReferenceFormatter(Formatter writer)
        {
            this.fmt = writer;
        }

        public Formatter Formatter { get { return fmt; } }

        public void WriteTypeReference(DataType dt)
        {
            this.declaration = false;
            this.declaredName = null;
            TypeName(dt);
        }

        /* declaration:
                declaration-specifiers init-declarator-list(opt) ;  */

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
            fmt.Write(' ');
            fmt.Write(cv);
        }

        void SpaceForPointerOperator(DataType t)
        {
            var p = t as Pointer;
            if (p != null)
            {
                if (!(t is ArrayType || t is FunctionType))
                    fmt.Write(' ');
            }
            else
                fmt.Write(' ');
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
            fmt.Write('*');
            TypeFormatter.WriteQualifier(t.Qualifier, fmt);
            TypeQualifierList(t);
        }

        void MemberPointer(MemberPointer m)
        {
            var ptPointee = m.Pointee as Pointer;
            if (ptPointee != null)
                Pointer(ptPointee);
            var mpPointee = m.Pointee as MemberPointer;
            if (mpPointee != null)
                MemberPointer(mpPointee);
            fmt.Write(' ');
            var baseType = StripPointerOperator(m.BasePointer);
            fmt.Write(baseType.Name);
            fmt.Write("::*");
            TypeQualifierList(m);
        }

        void ReferenceTo(ReferenceTo r)
        {
            WriteSpace();
            fmt.Write('&');
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
            TypeFormatter.WriteQualifier(t.Qualifier, fmt);
            if (t is UnknownType)
            {
                fmt.Write("<type-error>");
                return;
            }
            else if (t is EquivalenceClass)
            {
                fmt.Write(t.Name);
                wantSpace = true;
                return;
            }
            else if (t is PrimitiveType) {
                //case tree_code.VOID_TYPE:
                //case tree_code.BOOLEAN_TYPE:
                //case tree_code.CHAR_TYPE:
                //case tree_code.INTEGER_TYPE:
                //case tree_code.REAL_TYPE:
                //if (TYPE_NAME(t) != null)
                //    t = TYPE_NAME(t);
                //else
                //    t = c_common_type_for_mode(TYPE_MODE(t), TREE_UNSIGNED(t));
                WritePrimitiveTypeName((PrimitiveType)t);
                //if (declaration && !string.IsNullOrEmpty(declaredName))
                //    fmt.Write(' ');
                wantSpace = true;
                return;
            }
            else if (t is VoidType)
            {
                WriteVoidType((VoidType)t);
                wantSpace = true;
                return;
            }
            else if (t is UnionType)
            {
                fmt.WriteKeyword("union");
                fmt.Write(" ");
            }
            else if (t is StructureType)
            {
                fmt.WriteKeyword("struct");
                fmt.Write(" ");
            }
            else if (t is EnumType)
            {
                fmt.WriteKeyword("enum");
                fmt.Write(" ");
            }
            if (string.IsNullOrEmpty(t.Name))
                fmt.Write("<anonymous>");
            else
                fmt.Write(t.Name);
            wantSpace = true;
        }

        public virtual void WritePrimitiveTypeName(PrimitiveType t)
        {
            fmt.WriteType(t.Name, t);
            wantSpace = true;
        }

        public virtual void WriteVoidType(VoidType t)
        {
            fmt.WriteType(t.Name, t);
            wantSpace = true;
        }

        private void WriteSpace()
        {
            if (wantSpace)
            {
                fmt.Write(' ');
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
            if (pt != null || mp != null || rf != null)
            {
                // Get the types-specifier of this type.  
                DataType pointee = StripPointerOperator(
                    pt != null 
                        ? pt.Pointee
                        : mp != null 
                            ? mp.Pointee
                            : rf.Referent);
                SpecifierQualifierList(pointee);
                if (pointee is ArrayType || pointee is FunctionType)
                {
                    fmt.Write(" (");
                    wantSpace = false;
                }
                if (pt != null)
                    Pointer(pt);
                else if (mp != null)
                    MemberPointer(mp);
                else if (rf != null)
                    ReferenceTo(rf); 
                --this.depth;
                return;
            }

            var ft = t as FunctionType;
            if (ft != null && ft.ReturnValue != null)
            {
                SpecifierQualifierList(ft.ReturnValue.DataType);
                --this.depth;
                return;
            }
            var at = t as ArrayType;
            if (at != null)
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
            while (pt != null || mp != null)
            {
                if (pt != null)
                    dt = pt.Pointee;
                else
                    dt = mp.Pointee;
                pt = dt as Pointer;
                mp = dt as MemberPointer;
            }
            var eq = dt as EquivalenceClass;
            if (eq != null && eq.DataType != null)
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

        void ParameterTypeList(FunctionType ft)
        {
            var name = declaredName;
            fmt.Write('(');
            if (ft.Parameters == null || ft.Parameters.Length == 0)
            {
                // fmt.Write("void");      // In C, 0-parameter functions use 'void'
            }
            else
            {
                bool first = true;
                for (int i = 0; i < ft.Parameters.Length; ++i)
                {
                    if (!first)
                        fmt.Write(", ");
                    first = false;
                    declaredName = ft.Parameters[i].Name;
                    DeclarationSpecifiers(ft.Parameters[i].DataType);
                    if (declaration && !string.IsNullOrEmpty(declaredName))
                        Declarator(ft.Parameters[i].DataType);
                    else
                        AbstractDeclarator(ft.Parameters[i].DataType);
                }
            }
            fmt.Write(')');
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
            var pt = dt as Pointer;
            if (pt != null)
            {
                var pointee = pt.Pointee;
                var eq = pointee as EquivalenceClass;
                if (eq != null && eq.DataType != null)
                    pointee = eq.DataType;
                if (pointee is ArrayType ||
                    pointee is FunctionType)
                    fmt.Write(')');
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
            else if (t is FunctionType)
            {
                var ft = t as FunctionType;
                ParameterTypeList(ft);
                //DirectAbstractDeclarator(ft);
            }
            else if (t is ArrayType)
            {
                var at = t as ArrayType;
                fmt.Write('[');
                if (at.Length != 0)
                {
                    fmt.Write(at.Length);
                }
                fmt.Write(']');
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
                    fmt.Write(this.declaredName);
                }
                AbstractDeclarator(dt);
                return;
            }
            var at = dt as ArrayType;
            if (at != null)
            {
                AbstractDeclarator(at.ElementType);
                return;
            }
            var ft = dt as FunctionType;
            if (ft != null)
            {
                ParameterTypeList(ft);
                if (ft.ReturnValue != null)
                {
                    AbstractDeclarator(ft.ReturnValue.DataType);
                }
                return;
            }
        }

        void DirectDeclarator(FunctionType sig)
        {
            SpaceForPointerOperator(sig.ReturnValue.DataType);
            fmt.Write(declaredName);
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
