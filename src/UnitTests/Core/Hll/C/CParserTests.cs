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

using Reko.Core.Serialization;
using Reko.Core.Hll.C;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Reko.Libraries.Microchip.V1;

namespace Reko.UnitTests.Core.Hll.C
{
    [TestFixture]
    public class CParserTests
    {
        private CLexer lexer;
        private CParser parser;
        private ParserState parserState;

        [SetUp]
        public void Setup()
        {
            parserState = new ParserState();
        }

        private void GccLex(string str)
        {
            lexer = new CLexer(new StringReader(str), CLexer.GccKeywords);
            parser = new CParser(parserState, lexer);
        }

        private void MsvcLex(string str)
        {
            lexer = new CLexer(new StringReader(str), CLexer.MsvcKeywords);
            parser = new CParser(parserState, lexer);
        }

        [Test]
        public void CParser_decl()
        {
            GccLex("  int x;");
            parser.Parse_Decl();
        }

        [Test]
        public void CParser_empty_decl()
        {
            GccLex("");
            var decl = parser.Parse_Decl();
            Assert.IsNull(decl);
        }

        [Test]
        public void CParser_Nested_Array()
        {
            GccLex("a[3][4]");
            var decl = parser.Parse_Declarator();
            Debug.Print("{0}: ", decl);
            //var arr = (ArrayDeclarator) decl.DirectDeclarator;
            //Assert.AreEqual("3", arr.Size.ToString());
            //arr = (ArrayDeclarator) arr.DirectDeclarator;
            //Assert.AreEqual("4", arr.Size.ToString());
        }

        [Test]
        public void CParser_typedef_struct()
        {
            GccLex("typedef struct tagFoo { int x; int y; } FOO, *PFOO;");
            var decl = parser.Parse_Decl();
            var sExp =
                "(decl Typedef (Struct tagFoo " +
                       "((Int) ((x))" +
                       " (Int) ((y))) " +
                       "((init-decl FOO)" +
                       " (init-decl (ptr PFOO))))";
            Assert.AreEqual(sExp, decl.ToString());
            Debug.Write(decl.ToString());
        }

        [Test]
        public void CParser_Initialized()
        {
            GccLex("int * p = &x;");
            var decl = parser.Parse_Decl();
            Debug.Write(decl.ToString());
        }

        [Test]
        public void CParser_Typedef_Ulong()
        {
            GccLex("typedef unsigned long ULONG;");
            var decl = parser.Parse_ExternalDecl();
            Debug.WriteLine(decl.ToString());
            Assert.IsTrue(parser.IsTypeName("ULONG"));
        }

        [Test]
        public void CParser_Typef_Pulong()
        {
            GccLex("typedef unsigned long ULONG; typedef ULONG *PULONG;");
            var decl = parser.Parse_ExternalDecl();
            decl = parser.Parse_ExternalDecl();
            Debug.WriteLine(decl.ToString());
            Assert.AreEqual("(decl Typedef ULONG ((init-decl (ptr PULONG))))", decl.ToString());
        }

        [Test]
        public void CParser_Function()
        {
            GccLex("int atoi(char * number);");
            var decl = parser.Parse_Decl();
            Debug.WriteLine(decl.ToString());
        }

        [Test]
        public void CParser_ZeroArityFn()
        {
            GccLex("void _mm_pause (void);");
            var decl = parser.Parse_ExternalDecl();
            Assert.AreEqual("(decl Void ((init-decl (func _mm_pause ((Void ))))))", decl.ToString());
        }

        [Test]
        public void CParser_SelfReferencingStructure()
        {
            GccLex("typedef struct _LIST_ENTRY { " +
                "    struct _LIST_ENTRY *Flink;" +
                "    struct _LIST_ENTRY *Blink;" +
                "} LIST_ENTRY, *PLIST_ENTRY, * PRLIST_ENTRY;");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(decl Typedef (Struct _LIST_ENTRY " +
                "(((Struct _LIST_ENTRY)) (((ptr Flink)))" +
                " ((Struct _LIST_ENTRY)) (((ptr Blink)))) " +
                "((init-decl LIST_ENTRY)" +
                " (init-decl (ptr PLIST_ENTRY))" +
                " (init-decl (ptr PRLIST_ENTRY))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_Regression1()
        {
            GccLex("__inline PVOID GetFiberData( void )    { return *(PVOID *) (ULONG_PTR) __readfsdword (0x10);}\r\n");
            parser.ParserState.Typedefs.Add("PVOID");
            parser.ParserState.Typedefs.Add("ULONG_PTR");

            var decl = parser.Parse_ExternalDecl();
            var sExp =
                "(fndecl (decl __Inline PVOID " +
                    "((init-decl (func GetFiberData ((Void )))))) " +
                    "(Reko.Core.Hll.C.ReturnStat))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_Enum()
        {
            GccLex("typedef enum _S { Item = 1, Folder } S;");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(decl Typedef Reko.Core.Hll.C.EnumeratorTypeSpec " +
                "((init-decl S)))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_declspec_align()
        {
            GccLex("struct __declspec(align(16)) _S { int x; };");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(decl (Struct 16 _S ((Int) ((x))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_stdcall_in_typedef()
        {
            GccLex("typedef void (__stdcall *PFN) ();");
            var decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl Typedef Void ((init-decl " +
                "(func (__Stdcall (ptr PFN)))))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_Bitfields()
        {
            GccLex("typedef struct _BitField { unsigned int : 1; } foo;");
            var decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl Typedef (Struct _BitField ((Unsigned Int) (( 1))) " +
                "((init-decl foo)))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_DoWhile()
        {
            GccLex("do {__noop(TagBase);} while((0,0));");
            var stat = parser.Parse_Stat();
            var sExp =
                "(do " + Environment.NewLine +
                "(((__noop TagBase)))) " +
                "(Comma 0 0))";
            Assert.AreEqual(sExp, stat.ToString());
        }

        [Test]
        public void CParser_IdList()
        {
            parserState.Typedefs.Add("PVOID");
            parserState.Typedefs.Add("BOOLEAN");
            GccLex("typedef void (__stdcall * FOO) (PVOID, BOOLEAN );   ");
            var decl = parser.Parse_Decl();
            var sExp =
                "(decl Typedef Void " +
                "((init-decl (func (__Stdcall (ptr FOO)) ((PVOID ) (BOOLEAN ))))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_Derefs()
        {
            GccLex("CallbackEnviron->u.Flags = 0;");
            var stat = parser.Parse_Stat();
        }

        [Test]
        public void CParser_Ptr_to_Const_Int()
        {
            GccLex(" int const * pint;");
            var decl = parser.Parse_Decl();
            var sExp =
                "(decl Int Const ((init-decl (ptr pint))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_StructField_Ptr_to_Const_Int()
        {
            GccLex("struct { int const * pint; };");
            var decl = parser.Parse_Decl();
            var sExp =
                "(decl (Struct  ((Int Const) (((ptr pint)))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_ForwardTypedef()
        {
            GccLex("typedef struct _tagFoo Foo; ");
            var decl = parser.Parse_Decl();
            Assert.AreEqual("(decl Typedef (Struct _tagFoo) ((init-decl Foo)))", decl.ToString());
        }

        [Test]
        public void CParser_typedef_struct_redef()
        {
            GccLex(
                "typedef struct _M { int x; } M, *PM;" +
                "typedef struct _M M, *PM;");
            parser.Parse_ExternalDecl();
            Assert.IsTrue(parserState.Typedefs.Contains("M"));
            parser.Parse_ExternalDecl();
        }

        [Test]
        public void CParser_typedef_union_redef()
        {
            GccLex(
                "typedef union _M { int x; } M, *PM;" +
                "typedef union _M M, *PM;");
            parser.Parse_ExternalDecl();
            Assert.IsTrue(parserState.Typedefs.Contains("M"));
            parser.Parse_ExternalDecl();
        }

        [Test]
        public void CParser_Use_typedefname_As_variable()
        {
            parserState.Typedefs.Add("Doc");
            GccLex("typeof struct vtbl {\r\n" +
                "int (__stdcall * method)(\r\n" +
                    "int ** Doc);\r\n" +
                "} vtbl;");
            var decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl typeof (Struct vtbl " +
                    "((Int)" +
                    " (((func (__Stdcall (ptr method)) " +
                        "((Int (ptr (ptr Doc)))))))) " +
                    "((init-decl vtbl)))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_vtable_method_With_pfn()
        {
            GccLex(
                "typedef struct myVtbl { \n" +
                "int (__stdcall  * Do) (\n" +
                "    int x,\n" +
                "    bool (__stdcall * cont) ());\n" +
                "} myVtbl;");
            var decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl Typedef (Struct myVtbl " +
                    "((Int)" +
                    " (((func (__Stdcall (ptr Do)) " +
                        "((Int x)" +
                        " (Bool (func (__Stdcall (ptr cont)))))))))) " +
                    "((init-decl myVtbl)))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_typedef_array()
        {
            GccLex("typedef char (*array)[10];");
            var decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl Typedef Char ((init-decl (arr (ptr array) 10))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_typedef_2darray()
        {
            GccLex("typedef char matrix[3][4];");
            var decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl Typedef Char ((init-decl (arr (arr matrix 3) 4))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_declspec_deprecated_with_text()
        {
            GccLex("__declspec(deprecated(\"foo\")) int bar(); ");
            var decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl (__declspec deprecated) Int ((init-decl (func bar)))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_array_with_complex_size_expression()
        {
            GccLex(
                "typedef long LONG;\n" + 
                "typedef long LONG_PTR;\n" +
                "typedef struct { int x; } XSAVE_AREA;\n"+
                "typedef char __C_ASSERT__[((((LONG)(LONG_PTR)&(((XSAVE_AREA *)0)->Header)) & (64 - 1)) == 0)?1:-1];");
            var decl = parser.Parse_ExternalDecl();
            decl = parser.Parse_ExternalDecl();
            decl = parser.Parse_ExternalDecl();
            decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl Typedef Char ((init-decl " +
                    "(arr __C_ASSERT__ " +
                        "(cond (Eq (Ampersand (cast (LONG ) (cast (LONG_PTR ) " +
                        "(Ampersand ((cast (XSAVE_AREA (ptr )) 0) -> Header))) " +
                        "(Minus 64 1)) 0) 1 -1)))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_legal_duplicate_typedef()
        {
            GccLex(
                "typedef int FOO[1];\n" +
                "typedef int FOO[1];\n");
            var decl = parser.Parse_ExternalDecl();
            decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl Typedef Int ((init-decl " +
                    "(arr FOO 1))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_legal_duplicate_typedef2()
        {
            GccLex(
                "typedef __int64 __time64_t;\n" +
                "typedef __time64_t time_t;\n");
            var decl = parser.Parse_ExternalDecl();
            decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl Typedef __time64_t ((init-decl time_t)))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_legal_duplicate_typedef3()
        {
            GccLex(
                "typedef wchar_t WCHAR;\n" +
                "typedef const WCHAR *LPCWCH, *PCWCH;\n");
            var decl = parser.Parse_ExternalDecl();
            decl = parser.Parse_ExternalDecl();
            var sExp =
                "(decl Typedef Const WCHAR ((init-decl (ptr LPCWCH)) (init-decl (ptr PCWCH))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_application_empty_arglist()
        {
            GccLex("void FOO(void) {\r\n" +
                    "bar();\r\n" + 
                "}");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(fndecl (decl Void ((init-decl (func FOO ((Void )))))) (((bar))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_abstract_fn_parameter()
        {
            GccLex("int    __cdecl atexit(void (__cdecl *)(void));");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(decl Int __Cdecl ((init-decl (func atexit ((Void (func (__Cdecl (ptr )) ((Void )))))))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_typedef__success()
        {
            GccLex("typedef __success(return >= 0) long HRESULT;");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(decl Typedef Long ((init-decl HRESULT)))"; 
            Assert.AreEqual(sExp, decl.ToString());
        }

        private string windows_h =
        #region Windows.h
@"#line 1 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\windows.h""

#line 1 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\sdkddkver.h""
#pragma once

#pragma once

#pragma warning(disable:4116)       
                                    
#pragma warning(disable:4514)

#pragma warning(disable:4103)

#pragma warning(push)
#pragma warning(disable:4001)
#pragma warning(disable:4201)
#pragma warning(disable:4214)

#line 1 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\windef.h""

#pragma once

typedef unsigned long ULONG;
typedef ULONG *PULONG;
typedef unsigned short USHORT;
typedef USHORT *PUSHORT;
typedef unsigned char UCHAR;
typedef UCHAR *PUCHAR;
typedef char *PSZ;

typedef unsigned long       DWORD;
typedef int                 BOOL;
typedef unsigned char       BYTE;
typedef unsigned short      WORD;
typedef float               FLOAT;
typedef FLOAT               *PFLOAT;
typedef BOOL            *PBOOL;
typedef BOOL             *LPBOOL;
typedef BYTE            *PBYTE;
typedef BYTE             *LPBYTE;
typedef int             *PINT;
typedef int              *LPINT;
typedef WORD            *PWORD;
typedef WORD             *LPWORD;
typedef long             *LPLONG;
typedef DWORD           *PDWORD;
typedef DWORD            *LPDWORD;
typedef void             *LPVOID;
typedef const void       *LPCVOID;

typedef int                 INT;
typedef unsigned int        UINT;
typedef unsigned int        *PUINT;
#line 1 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\winnt.h""

#line 1 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\specstrings.h""

#pragma once
#line 9 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\specstrings.h""
 
 

#line 1 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\specstrings_adt.h""
#pragma once
typedef     char* ValidCompNameA;
typedef     unsigned short* ValidCompNameW;
typedef     const unsigned short* ConstValidCompNameW;
typedef      char* SAL_ValidCompNameT;
typedef     const  char* SAL_ConstValidCompNameT;
#line 1 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\specstrings_strict.h""
#pragma once
#line 1 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\specstrings_undef.h""

#line 1 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\basetsd.h""

 

  
   
  
  typedef unsigned long POINTER_64_INT;
 #line 42 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\basetsd.h""
 

#pragma once

typedef signed char         INT8, *PINT8;
typedef signed short        INT16, *PINT16;
typedef signed int          INT32, *PINT32;
typedef signed __int64      INT64, *PINT64;
typedef unsigned char       UINT8, *PUINT8;
typedef unsigned short      UINT16, *PUINT16;
typedef unsigned int        UINT32, *PUINT32;
typedef unsigned __int64    UINT64, *PUINT64;
typedef signed int LONG32, *PLONG32;
typedef unsigned int ULONG32, *PULONG32;
typedef unsigned int DWORD32, *PDWORD32;

    typedef __w64 int INT_PTR, *PINT_PTR;
    typedef __w64 unsigned int UINT_PTR, *PUINT_PTR;

    typedef __w64 long LONG_PTR, *PLONG_PTR;
    typedef __w64 unsigned long ULONG_PTR, *PULONG_PTR;

    

typedef unsigned short UHALF_PTR, *PUHALF_PTR;
typedef short HALF_PTR, *PHALF_PTR;
typedef __w64 long SHANDLE_PTR;
typedef __w64 unsigned long HANDLE_PTR;
__inline
void * __ptr64
PtrToPtr64(
    const void *p
    )
{
    return((void * __ptr64) (unsigned __int64) (ULONG_PTR)p );
}

__inline
void *
Ptr64ToPtr(
    const void * __ptr64 p
    )
{
    return((void *) (ULONG_PTR) (unsigned __int64) p);
}

__inline
void * __ptr64
HandleToHandle64(
    const void *h
    )
{
    return((void * __ptr64)(__int64)(LONG_PTR)h );
}

__inline
void *
Handle64ToHandle(
    const void * __ptr64 h
    )
{
    return((void *) (ULONG_PTR) (unsigned __int64) h );
}
typedef ULONG_PTR SIZE_T, *PSIZE_T;
typedef LONG_PTR SSIZE_T, *PSSIZE_T;
typedef ULONG_PTR DWORD_PTR, *PDWORD_PTR;
typedef __int64 LONG64, *PLONG64;

typedef unsigned __int64 ULONG64, *PULONG64;
typedef unsigned __int64 DWORD64, *PDWORD64;
typedef ULONG_PTR KAFFINITY;
typedef KAFFINITY *PKAFFINITY;
typedef void *PVOID;
typedef void * __ptr64 PVOID64;

typedef char CHAR;
typedef short SHORT;
typedef long LONG;

typedef int INT;
typedef wchar_t WCHAR;    
typedef WCHAR *PWCHAR, *LPWCH, *PWCH;
typedef const WCHAR *LPCWCH, *PCWCH;
typedef  WCHAR *NWPSTR, *LPWSTR, *PWSTR;
typedef  PWSTR *PZPWSTR;
typedef  const PWSTR *PCZPWSTR;
typedef  WCHAR  *LPUWSTR, *PUWSTR;
typedef  const WCHAR *LPCWSTR, *PCWSTR;
typedef  PCWSTR *PZPCWSTR;
typedef  const WCHAR  *LPCUWSTR, *PCUWSTR;

typedef const WCHAR *LPCWCHAR, *PCWCHAR;
typedef const WCHAR  *LPCUWCHAR, *PCUWCHAR;
typedef unsigned long UCSCHAR;
typedef UCSCHAR *PUCSCHAR;
typedef const UCSCHAR *PCUCSCHAR;

typedef UCSCHAR *PUCSSTR;
typedef UCSCHAR  *PUUCSSTR;

typedef const UCSCHAR *PCUCSSTR;
typedef const UCSCHAR  *PCUUCSSTR;

typedef UCSCHAR  *PUUCSCHAR;
typedef const UCSCHAR  *PCUUCSCHAR;
typedef CHAR *PCHAR, *LPCH, *PCH;
typedef const CHAR *LPCCH, *PCCH;

typedef  CHAR *NPSTR, *LPSTR, *PSTR;
typedef  PSTR *PZPSTR;
typedef  const PSTR *PCZPSTR;
typedef  const CHAR *LPCSTR, *PCSTR;
typedef  PCSTR *PZPCSTR;
typedef char TCHAR, *PTCHAR;
typedef unsigned char TBYTE , *PTBYTE ;
typedef LPCH LPTCH, PTCH;
typedef LPSTR PTSTR, LPTSTR, PUTSTR, LPUTSTR;
typedef LPCSTR PCTSTR, LPCTSTR, PCUTSTR, LPCUTSTR;

typedef SHORT *PSHORT;  
typedef LONG *PLONG;    
typedef void *HANDLE;
typedef HANDLE *PHANDLE;
typedef BYTE   FCHAR;
typedef WORD   FSHORT;
typedef DWORD  FLONG;

typedef  long HRESULT;
    
typedef char CCHAR;          
typedef DWORD LCID;         
typedef PDWORD PLCID;       
typedef WORD   LANGID;      

typedef struct _FLOAT128 {
    __int64 LowPart;
    __int64 HighPart;
} FLOAT128;

typedef FLOAT128 *PFLOAT128;
typedef __int64 LONGLONG;
typedef unsigned __int64 ULONGLONG;
typedef LONGLONG *PLONGLONG;
typedef ULONGLONG *PULONGLONG;
typedef LONGLONG USN;

typedef union _LARGE_INTEGER {
    struct {
        DWORD LowPart;
        LONG HighPart;
    };
    struct {
        DWORD LowPart;
        LONG HighPart;
    } u;

    LONGLONG QuadPart;
} LARGE_INTEGER;

typedef LARGE_INTEGER *PLARGE_INTEGER;

typedef union _ULARGE_INTEGER {
    struct {
        DWORD LowPart;
        DWORD HighPart;
    };
    struct {
        DWORD LowPart;
        DWORD HighPart;
    } u;

    ULONGLONG QuadPart;
} ULARGE_INTEGER;

typedef ULARGE_INTEGER *PULARGE_INTEGER;

typedef struct _LUID {
    DWORD LowPart;
    LONG HighPart;
} LUID, *PLUID;
typedef ULONGLONG  DWORDLONG;
typedef DWORDLONG *PDWORDLONG;

ULONGLONG
__stdcall
Int64ShllMod32 (
    ULONGLONG Value,
    DWORD ShiftCount
    );

LONGLONG
__stdcall
Int64ShraMod32 (
    LONGLONG Value,
    DWORD ShiftCount
    );

ULONGLONG
__stdcall
Int64ShrlMod32 (
    ULONGLONG Value,
    DWORD ShiftCount
    );
#pragma warning(push)

#pragma warning(disable:4035 4793)               

__inline ULONGLONG
__stdcall
Int64ShllMod32 (
    ULONGLONG Value,
    DWORD ShiftCount
    )
{
    __asm    {
        mov     ecx, ShiftCount
        mov     eax, dword ptr [Value]
        mov     edx, dword ptr [Value+4]
        shld    edx, eax, cl
        shl     eax, cl
    }
}

__inline LONGLONG
__stdcall
Int64ShraMod32 (
    LONGLONG Value,
    DWORD ShiftCount
    )
{
    __asm {
        mov     ecx, ShiftCount
        mov     eax, dword ptr [Value]
        mov     edx, dword ptr [Value+4]
        shrd    eax, edx, cl
        sar     edx, cl
    }
}

__inline ULONGLONG
__stdcall
Int64ShrlMod32 (
    ULONGLONG Value,
    DWORD ShiftCount
    )
{
    __asm    {
        mov     ecx, ShiftCount
        mov     eax, dword ptr [Value]
        mov     edx, dword ptr [Value+4]
        shrd    eax, edx, cl
        shr     edx, cl
    }
}
#pragma warning(pop)

unsigned int
__cdecl
_rotl (
     unsigned int Value,
     int Shift
    );

unsigned __int64
__cdecl
_rotl64 (
     unsigned __int64 Value,
     int Shift
    );
unsigned int
__cdecl
_rotr (
     unsigned int Value,
     int Shift
    );

unsigned __int64
__cdecl
_rotr64 (
     unsigned __int64 Value,
     int Shift
    );

#pragma intrinsic(_rotl)
#pragma intrinsic(_rotl64)
#pragma intrinsic(_rotr)
#pragma intrinsic(_rotr64)
typedef BYTE  BOOLEAN;           
typedef BOOLEAN *PBOOLEAN;       
typedef struct _LIST_ENTRY {
   struct _LIST_ENTRY *Flink;
   struct _LIST_ENTRY *Blink;
} LIST_ENTRY, *PLIST_ENTRY, * PRLIST_ENTRY;

typedef struct _SINGLE_LIST_ENTRY {
    struct _SINGLE_LIST_ENTRY *Next;
} SINGLE_LIST_ENTRY, *PSINGLE_LIST_ENTRY;
typedef struct LIST_ENTRY32 {
    DWORD Flink;
    DWORD Blink;
} LIST_ENTRY32;
typedef LIST_ENTRY32 *PLIST_ENTRY32;

typedef struct LIST_ENTRY64 {
    ULONGLONG Flink;
    ULONGLONG Blink;
} LIST_ENTRY64;
typedef LIST_ENTRY64 *PLIST_ENTRY64;
#line 1 ""\\program files\\Microsoft SDKs\\Windows\\v6.0A\\Include\\guiddef.h""
typedef struct _GUID {
    unsigned long  Data1;
    unsigned short Data2;
    unsigned short Data3;
    unsigned char  Data4[ 8 ];
} GUID;

typedef GUID *LPGUID;

typedef const GUID *LPCGUID;
typedef GUID IID;
typedef IID *LPIID;
typedef GUID CLSID;
typedef CLSID *LPCLSID;
typedef GUID FMTID;
typedef FMTID *LPFMTID;

typedef struct  _OBJECTID {     
    GUID Lineage;
    DWORD Uniquifier;
} OBJECTID;

  

  

typedef ULONG_PTR KSPIN_LOCK;
typedef KSPIN_LOCK *PKSPIN_LOCK;

#pragma warning(push)

#pragma warning(disable:4164)   
                                

#pragma function(_enable)
#pragma function(_disable)

#pragma warning(pop)

BOOLEAN
_bittest (
     LONG const *Base,
     LONG Offset
    );

BOOLEAN
_bittestandcomplement (
     LONG *Base,
     LONG Offset
    );

BOOLEAN
_bittestandset (
     LONG *Base,
     LONG Offset
    );

BOOLEAN
_bittestandreset (
     LONG *Base,
     LONG Offset
    );

BOOLEAN
_interlockedbittestandset (
     LONG volatile *Base,
     LONG Offset
    );

BOOLEAN
_interlockedbittestandreset (
     LONG volatile *Base,
     LONG Offset
    );

#pragma intrinsic(_bittest)
#pragma intrinsic(_bittestandcomplement)
#pragma intrinsic(_bittestandset)
#pragma intrinsic(_bittestandreset)
#pragma intrinsic(_interlockedbittestandset)
#pragma intrinsic(_interlockedbittestandreset)

BOOLEAN
_BitScanForward (
     DWORD *Index,
     DWORD Mask
    );

BOOLEAN
_BitScanReverse (
     DWORD *Index,
     DWORD Mask
    );

#pragma intrinsic(_BitScanForward)
#pragma intrinsic(_BitScanReverse)
SHORT
_InterlockedCompareExchange16 (
     SHORT volatile *Destination,
     SHORT ExChange,
     SHORT Comperand
    );

#pragma intrinsic(_InterlockedCompareExchange16)
#pragma warning(push)
#pragma warning(disable:4035 4793)

__forceinline
BOOLEAN
InterlockedBitTestAndComplement (
     LONG volatile *Base,
     LONG Bit
    )
{
    __asm {
           mov eax, Bit
           mov ecx, Base
           lock btc [ecx], eax
           setc al
    };
}
#pragma warning(pop)
BYTE 
__readfsbyte (
     DWORD Offset
    );
 
WORD  
__readfsword (
     DWORD Offset
    );
 
DWORD
__readfsdword (
     DWORD Offset
    );
 
void
__writefsbyte (
     DWORD Offset,
     BYTE  Data
    );
 
void
__writefsword (
     DWORD Offset,
     WORD   Data
    );
 
void
__writefsdword (
     DWORD Offset,
     DWORD Data
    );

#pragma intrinsic(__readfsbyte)
#pragma intrinsic(__readfsword)
#pragma intrinsic(__readfsdword)
#pragma intrinsic(__writefsbyte)
#pragma intrinsic(__writefsword)
#pragma intrinsic(__writefsdword)
void
__incfsbyte (
    DWORD Offset
    );
 
void
__addfsbyte (
    DWORD Offset,
    BYTE  Value
    );
 
void
__incfsword (
    DWORD Offset
    );
 
void
__addfsword (
    DWORD Offset,
    WORD   Value
    );
 
void
__incfsdword (
    DWORD Offset
    );
 
void
__addfsdword (
    DWORD Offset,
    DWORD Value
    );
 
void
_mm_pause (
    void
    );

LONGLONG
__forceinline
_InterlockedOr64 (
      LONGLONG volatile *Destination,
      LONGLONG Value
    )
{
    LONGLONG Old;

    do {
        Old = *Destination;
    } while (_InterlockedCompareExchange64(Destination,
                                          Old | Value,
                                          Old) != Old);

    return Old;
}

#pragma intrinsic(_mm_pause)
#pragma warning( push )
#pragma warning( disable : 4793 )
__forceinline
void
MemoryBarrier (
    void
    )
{
    LONG Barrier;
    __asm {
        xchg Barrier, eax
    }
}


";
#endregion

        [Test]
        public void CParser_Windows_h()
        {
            GccLex(windows_h);
            var decls = parser.Parse();
            for (int i = 0; i < decls.Count; ++i)
            {
                Debug.Print("{0}: {1}", i, decls[i].ToString());
                Debug.WriteLine("");
            }
            Assert.AreEqual(186, decls.Count);
        }

        [Test]
        public void CParser_FunctionPtr_Parameters()
        {
            GccLex("int __libc_start_main(int (*main) (int, char **, char **), int argc, char ** ubp_av, void (*init) (void), void (*fini) (void), void (*rtld_fini) (void), void (* stack_end));");
            var decl = parser.Parse_ExternalDecl();
            var sExp = 
                "(decl Int ((init-decl (func __libc_start_main (" +
                    "(Int (func (ptr main) ((Int ) (Char (ptr (ptr ))) (Char (ptr (ptr )))))) " +
                    "(Int argc) " +
                    "(Char (ptr (ptr ubp_av))) " +
                    "(Void (func (ptr init) ((Void )))) " +
                    "(Void (func (ptr fini) ((Void )))) " +
                    "(Void (func (ptr rtld_fini) ((Void )))) " +
                    "(Void (ptr stack_end))" +
                    ")))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_FunctionPtr_AbstractParameters()
        {
            GccLex("int main(int, char **, char **);");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(decl Int ((init-decl (func main ((Int ) (Char (ptr (ptr ))) (Char (ptr (ptr ))))))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_Attribute()
        {
            GccLex("[[reko::reg(\"D0\")]]");
            var attr = parser.Parse_AttributeSpecifier();
            var sExp = "(attr reko::reg (StringLiteral D0))";
            Assert.AreEqual(sExp, attr.ToString());
        }

        [Test]
        public void CParser_AttributedDeclaration()
        {
            GccLex("[[reko::reg(\"D0\")]] BYTE foo([[reko::reg(\"A1\")]] void * arg);");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(decl " +
                            "(attr reko::reg (StringLiteral D0)) " +
                            "BYTE ((init-decl (func foo ((" +
                                "(attr reko::reg (StringLiteral A1)) " +
                                "Void (ptr arg)))))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_Far_Pointer()
        {
            GccLex("typedef void _far*LPVOID;");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(decl Typedef Void _Far ((init-decl (ptr LPVOID))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_Regression()
        {
            GccLex("char * get(int n);");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(decl Char ((init-decl (ptr (func get ((Int n)))))))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_Typedef_using_undefinedType()
        {
            // Even though 'a' is not defined, it should still be parseable. 
            // We make the assumption in a typedef that if it is followed by 
            // an id, the id is a reference to a type.
            GccLex("typedef a b;");
            var decl = parser.Parse_ExternalDecl();
            var sExp = "(decl Typedef a ((init-decl b)))";
            Assert.AreEqual(sExp, decl.ToString());
        }

        [Test]
        public void CParser_IncorrectStatement1()
        {
            GccLex("int a()b;");
            try
            {
                parser.Parse_ExternalDecl();
            }
            catch (CParserException)
            {
                return;
            }
            Assert.Fail("Should have failed to parse");
        }

        [Test]
        public void CParser_IncorrectStatement2()
        {
            GccLex("int a);");
            try
            {
                parser.Parse_ExternalDecl();
            }
            catch (CParserException)
            {
                return;
            }
            Assert.Fail("Should have failed to parse");
        }

        [Test]
        public void CParser_Argument_Attributes()
        {
            GccLex("int _ftol([[reko::x87_fpu_arg]]double);");
            var decl = parser.Parse_ExternalDecl();
        }

        [Test]
        public void CParser_Pragma_Prefast()
        {
            GccLex(
@"
#pragma prefast(push)
#pragma prefast(disable: 6001 28113, ""The barrier variable is accessed only to create a side effect."")
#pragma prefast(pop)
int x;
 ");
            var decl = parser.Parse_Decl();
        }

        [Test]
        public void CParser_Semicolon_after_pragma()
        {
            GccLex(
@"
#pragma region

;
#pragma endregion
int x = 3;
");
            var decls = parser.Parse();
            Assert.AreEqual(1, decls.Count);
        }

        [Test(Description = "Test non-standard use of __thiscall keyword in a non-member function declaration.")]
        public void CParser_thiscall()
        {
            GccLex("int __thiscall foo(char * bar, const float * baz);");
            var decl = parser.Parse_Decl();
            Assert.AreEqual(
                "(decl Int __Thiscall ((init-decl (func foo ((Char (ptr bar)) (Const Float (ptr baz)))))))",
                decl.ToString());
        }

        [Test]
        public void CParser_thiscall_return_pointer_to_int()
        {
            GccLex("int * __thiscall foo();");

            var decl = parser.Parse_Decl();

            Assert.AreEqual(
                "(decl Int ((init-decl (ptr (__Thiscall (func foo)))))))",
                decl.ToString());
        }

        [Test]
        public void CParser_thiscall_abstract_parameter()
        {
            GccLex("float func(int x, bool (__thiscall * fn)());");
            var decl = parser.Parse_Decl();
            Assert.AreEqual(
                "(decl Float ((init-decl (func func ((Int x) (Bool (func (__Thiscall (ptr fn))))))))))",
                decl.ToString());
        }

        [Test(Description = "#506 on Github")]
        public void CParser_Issue_506()
        {
            GccLex("[[reko::returns(register, \"d0\")]] bool _DATAINIT([[reko::arg(register, \"a5\")]] long a5);");
            var decl = parser.Parse_Decl();
            Assert.AreEqual(
                "(decl (attr reko::returns (Register Comma StringLiteral d0)) " +
                  "Bool ((init-decl (func _DATAINIT " +
                  "(((attr reko::arg (Register Comma StringLiteral a5)) Long a5))))))",
                decl.ToString());
        }

        [Test]
        public void CParser_ExternalTypes()
        {
            GccLex("word32 fn00401410(Eq_25 ebp, Eq_26 dwArg04, word32 dwArg08, " +
                "Eq_25 dwArg0C, ptr32 & ebxOut, ptr32 & esiOut);");
            parserState.Typedefs.UnionWith(new[]{
                "Eq_25", "Eq_26", "ptr32", "word32" });
            var decl = parser.Parse_ExternalDecl();
            Assert.AreEqual(
                "(decl word32 ((init-decl (func fn00401410 (" +
                    "(Eq_25 ebp) " +
                    "(Eq_26 dwArg04) " +
                    "(word32 dwArg08) " +
                    "(Eq_25 dwArg0C) " +
                    "(ptr32 (ref ebxOut)) " +
                    "(ptr32 (ref esiOut)))))))",
                decl.ToString());
        }

        [Test]
        public void CParser_Far_Pascal_Pointer_to_function()
        {
            GccLex("typedef int (__pascal __far  *PFN)();");
            var decl = parser.Parse_Decl();
            Assert.AreEqual(
                "(decl Typedef Int ((init-decl (func (__Pascal (ptr _Far PFN)))))))",
                decl.ToString());
        }

        [Test]
        public void CParser_loadds()
        {
            GccLex("int __pascal __loadds fn();");
            var decl = parser.Parse_Decl();
            Assert.AreEqual(
                "(decl Int __Pascal __LoadDs ((init-decl (func fn)))))",
                decl.ToString());
        }

        [Test]
        public void CParser_Far_Ptr()
        {
            GccLex("int __pascal CHKDSK(int argc, char __far** argv, char __far** envp);");
            var decl = parser.Parse_ExternalDecl();
            Assert.AreEqual("(decl Int __Pascal ((init-decl (func CHKDSK ((Int argc) (Char _Far (ptr (ptr argv))) (Char _Far (ptr (ptr envp))))))))", decl.ToString());
        }

        [Test]
        public void CParser_MultilineComent()
        {
            GccLex(@"int /* This is a multi line
comment*/ x;");
            var decl = parser.Parse_Decl();
            Assert.AreEqual("(decl Int ((init-decl x)))", decl.ToString());
        }

        [Test]
        public void CParser_Far_ptr_struct_field()
        {
            GccLex(@"
struct TheStruct {
    void _far * field;
};");
            var decl = parser.Parse_Decl();
            Assert.AreEqual("(decl (Struct TheStruct ((Void) (((ptr _Far field)))))", decl.ToString());
        }

        [Test]
        public void CParser_Regression2()
        {
            GccLex(@"
int __far __pascal FS_ALLOCATEPAGESPACE(struct sffsi __far * psffsi, struct sffsd __far * psffsd, unsigned long ulsize, unsigned short ulWantContig);
");
            var decl = parser.Parse_Decl();
            Assert.AreEqual(
                "(decl Int _Far __Pascal ((init-decl (func FS_ALLOCATEPAGESPACE (((Struct sffsi) (ptr _Far psffsi)) ((Struct sffsd) (ptr _Far psffsd)) (Unsigned Long ulsize) (Unsigned Short ulWantContig))))))", decl.ToString());
        }

        [Test]
        public void CParser_Regression3()
        {
            GccLex(@"
int __far __pascal FS_FILELOCKS(struct sffsi __far * psffsi, struct sffsd __far * psffsd, struct filelock __far * pUnLockRange, struct filelock __far * pLockRange, unsigned long timeout, unsigned long flags);
");
            var decl = parser.Parse();
        }

        [Test]
        public void CParser_GccStyleAttributes()
        {
            GccLex(@"
extern char *tempnam (const char *__dir, const char *__pfx)
     __attribute__ ((__nothrow__ , __leaf__)) __attribute__ ((__malloc__));");
            var decls = parser.Parse();
            Assert.AreEqual(1, decls.Count);
            var attrs = decls[0].attribute_list;
            Assert.AreEqual(3, attrs.Count);
            Assert.AreEqual("__nothrow__", attrs[0].Name.Components[0]);
            Assert.AreEqual("__leaf__", attrs[1].Name.Components[0]);
            Assert.AreEqual("__malloc__", attrs[2].Name.Components[0]);
        }

        [Test]
        public void CParser_restrict()
        {
            GccLex(@"
extern struct __iob *fopen (const char *__restrict __filename,
      const char *__restrict __modes) ;");
            var decl = parser.Parse()[0];
            Assert.AreEqual("(decl Extern (Struct __iob) ((init-decl (ptr (func fopen ((Const Char (ptr Restrict __filename)) (Const Char (ptr Restrict __modes))))))))", decl.ToString());
        }

        [Test]
        public void CParser_GccAsm()
        {
            GccLex(@"
extern int myfunc() __asm__ ("""" ""__flub"");");
            var attrs = parser.Parse()[0].attribute_list;
            Assert.AreEqual(1, attrs.Count);
            Assert.AreEqual("__asm__", attrs[0].Name.Components[0]);
            Assert.AreEqual("__flub", attrs[0].Tokens[0].Value);
        }

        [Test]
        public void CParser_GccParserState_Float32()
        {
            parserState.Typedefs.Add("_Float32");
            GccLex(@"_Float32 x = 42;");
            var decl = parser.Parse()[0];
            Assert.AreEqual("(decl _Float32 ((init-decl x 42)))", decl.ToString());
        }

        [Test]
        public void CParser_GccRestrict()
        {
            parserState.Typedefs.Add("_Float32");
            GccLex(@"extern _Float32 strtof32 (const char *__restrict __nptr,char **__restrict __endptr)__attribute__ ((__nothrow__ , __leaf__)) __attribute__ ((__nonnull__ (1)));");
            var decl = parser.Parse()[0];
            Assert.AreEqual("(decl (attr __nothrow__) (attr __leaf__) (attr __nonnull__ (NumericLiteral 1)) Extern _Float32 ((init-decl (func strtof32 ((Const Char (ptr Restrict __nptr)) (Char (ptr (ptr Restrict __endptr))))))))", decl.ToString());
        }

        [Test]
        public void CParser_GccStructAttributes()
        {
            GccLex(
@"typedef struct {
    long long __max_align_ll __attribute__((__aligned__(__alignof__(long long))));
} max_align_t;");
            var decl = parser.Parse()[0];
            Assert.AreEqual("(decl Typedef (Struct  ((Long Long) ((__max_align_ll)) ((attr __aligned__ (Id __alignof__LParen Long Long RParen )))) ((init-decl max_align_t)))", decl.ToString());
        }

        [Test]
        public void CParser_Atomic()
        {
            GccLex("typedef _Atomic char atomic_char;");
            var decl = parser.Parse()[0];
            Assert.AreEqual("(decl Typedef _Atomic Char ((init-decl atomic_char)))", decl.ToString());
        }

        [Test]
        public void CParser_declspec_restrict()
        {
            GccLex("  __declspec(noalias) __declspec(restrict)      void * __cdecl test();");
            var decl = parser.Parse()[0];
            
            Assert.AreEqual("(decl (__declspec noalias) (__declspec restrict) Void ((init-decl (ptr (__Cdecl (func test)))))))", decl.ToString());
        }

        [Test]
        public void CParser_pragma_function_many_args()
        {
            GccLex("  #pragma function(floor, floorf, ceil, ceilf)");
            parser.Parse();
        }

        [Test]
        public void CParser_pragma_pack2()
        {
            GccLex(
@"#pragma pack(push)
#pragma pack(push,4)
");
            parser.Parse();
        }

        [Test]
        public void CParser_pragma_warning_disable()
        {
            GccLex("#pragma warning(default : 4200)\r\n");
            parser.Parse();
        }

        [Test]
        public void CParser_inline_function()
        {
            MsvcLex(
@"_inline bool IsDebuggerPresent(void) {
    bool bIsDebuggerPresent;
    if (CheckRemoteDebuggerPresent(GetCurrentProcess(), &bIsDebuggerPresent)) {
        return bIsDebuggerPresent;
    }
    return 0;
}");
            var decl = parser.Parse()[0];
        }

        [Test]
        public void CParser_If()
        {
            GccLex(
@"{
    HRESULT hr;
    if (cchDest > 2147483647)
    {
        hr = ((HRESULT)0x80070057L);
    }
    else
    {
        hr = StringCopyWorkerA(pszDest, cchDest, pszSrc);
    }
    return hr;
}");
            parser.ParserState.Typedefs.Add("HRESULT");
            var stm = parser.Parse_Stat();
        }

        [Test]
        public void CParser_Parenthesized_Statements()
        {
            GccLex(@" {
  	(hKey);    
	(SecurityInformation);    
	(pSecurityDescriptor);    
	return 0L;
}");
            var stm = parser.Parse_Stat();
        }

        [Test]
        public void CParser_cdecl_declaration()
        {
            GccLex(@"size_t  __cdecl _msize(     void * _Memory);");
            parser.Parse();
        }

        [Test]
        public void CParser_const_before_ptr()
        {
            GccLex("typedef struct _REGINI const *LPCREGINI;");
            var decl = parser.Parse()[0];
            Assert.AreEqual("(decl Typedef (Struct _REGINI) ((init-decl (ptr Const LPCREGINI))))", decl.ToString());
        }

        [Test]
        public void CParser_forward_decl()
        {
            GccLex(@"
            typedef unsigned int Request;
            struct test {
                int Request[42];
            };
            ");
            var decl = parser.Parse()[1];

        }

        [Test]
        public void CParser_huge_ptr()
        {
            MsvcLex(@"
typedef unsigned int UINT;
typedef int         HFILE;
UINT __far __pascal _lread(HFILE, void __huge *, UINT );");
            var decl = parser.Parse()[2];
            Assert.AreEqual("(decl UINT _Far __Pascal ((init-decl (func _lread ((HFILE ) (Void _Huge (ptr )) (UINT ))))))", decl.ToString());
        }

        [Test]
        public void CParser_gcc_attribute_struct()
        {
            GccLex(@"
struct __attribute__((aligned(4))) yapl_vm
{
};");
            var decl = parser.Parse()[0];
            Assert.AreEqual("(decl (Struct 4 yapl_vm))", decl.ToString());
        }

        [Test]
        public void CParser_gcc_attribute_field()
        {
            GccLex(@"
struct __attribute__((aligned(4))) yapl_vm
{
    int field0;
    __attribute__((aligned(8))) double field8;
};");
            var decl = parser.Parse()[0];
            Assert.AreEqual("(decl (Struct 4 yapl_vm " +
                "((Int) ((field0)) " +
                "(Double) ((field8)) ((attr aligned (NumericLiteral 8)))))",
                decl.ToString());
        }

        [Test]
        public void CParser_msvc_near_fn()
        {
            MsvcLex(@"
void __near exit(int status);
");
            var decl = parser.Parse()[0];
            Assert.AreEqual("(decl Void _Near ((init-decl (func exit ((Int status))))))",
                decl.ToString());
        }
    }
}
