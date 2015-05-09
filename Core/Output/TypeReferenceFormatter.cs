using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decompiler.Core.Types;

namespace Decompiler.Core.Output
{
    public class TypeReferenceFormatter
    {
        private Formatter fmt;
        private bool declaration;
        private string declaredName;
        private bool typeReference;

        public TypeReferenceFormatter(Formatter writer, bool typeReference)
        {
            this.fmt = writer;
            this.typeReference = typeReference;
        }

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
                DataType pointee = StripPointerOperator(p.Pointee);
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
        [Flags]
        enum type_qual
        {
            CONST = 1,
            VOLATILE = 2,
            RESTRICT = 4
        }

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
            var ptPointee = t.Pointee as Pointer;
            if (ptPointee != null)
                Pointer(ptPointee);
            fmt.Write('*');
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
            if (t is UnknownType)
            {
                fmt.Write("<type-error>");
                return;
            }
            else if (t is EquivalenceClass)
            {
                fmt.Write(t.Name);
                return;
            }
            else if (t is PrimitiveType || t is VoidType)
            {
                //case tree_code.VOID_TYPE:
                //case tree_code.BOOLEAN_TYPE:
                //case tree_code.CHAR_TYPE:
                //case tree_code.INTEGER_TYPE:
                //case tree_code.REAL_TYPE:
                //if (TYPE_NAME(t) != null)
                //    t = TYPE_NAME(t);
                //else
                //    t = c_common_type_for_mode(TYPE_MODE(t), TREE_UNSIGNED(t));
                fmt.Write(t.Name);
                //if (declaration && !string.IsNullOrEmpty(declaredName))
                //    fmt.Write(' ');
                return;
            }
            else if (t is UnionType)
            {
                fmt.WriteKeyword("union");
            }
            else if (t is StructureType)
            {
                fmt.WriteKeyword("struct");
            }
            else if (t is EnumType)
            {
                fmt.WriteKeyword("enum");
            }
            fmt.Write(" ");
            if (string.IsNullOrEmpty(t.Name))
                fmt.Write("<anonymous>");
            else
                fmt.Write(t.Name);
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
            if (!(t is Pointer))
                TypeQualifierList(t);
            var pt = t as Pointer;
            var mp = t as MemberPointer;
            if (pt != null || mp != null)
            {
                // Get the types-specifier of this type.  
                DataType pointee = StripPointerOperator(
                    pt != null 
                        ? pt.Pointee
                        : mp.Pointee);
                SpecifierQualifierList(pointee);
                if (pointee is ArrayType || pointee is FunctionType)
                {
                    fmt.Write(" (");
                }
                if (pt != null)
                    Pointer(pt);
                else
                    MemberPointer(mp);
                return;
            }
            var ft = t as FunctionType;
            if (ft != null)
            {
                SpecifierQualifierList(ft.ReturnType);
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
            if (ft.ArgumentTypes.Length == 0)
            {
                // fmt.Write("void");      // In C, 0-parameter functions use 'void'
            }
            else
            {
                bool first = true;
                for (int i = 0; i < ft.ArgumentTypes.Length; ++i)
                {
                    if (!first)
                        fmt.Write(", ");
                    first = false;
                    declaredName = ft.ArgumentNames != null ? ft.ArgumentNames[i] : null;
                    DeclarationSpecifiers(ft.ArgumentTypes[i]);
                    if (declaration && declaredName != null)
                        Declarator(ft.ArgumentTypes[i]);
                    else
                        AbstractDeclarator(ft.ArgumentTypes[i]);
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
            var pt = dt as Pointer;
            if (pt != null)
            {
                if (pt.Pointee is ArrayType ||
                    pt.Pointee is FunctionType)
                    fmt.Write(')');
                dt = pt.Pointee;
            }
            DirectAbstractDeclarator(dt);
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
                AbstractDeclarator(ft.ReturnType);
                return;
            }
        }

        void DirectDeclarator(ProcedureSignature sig)
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
