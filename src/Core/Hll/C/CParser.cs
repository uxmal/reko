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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Hll.C
{
    // https://web.archive.org/web/20200223191833/http://www.ssw.uni-linz.ac.at/Coco/C/C.atg

    /// <summary>
    /// A parser capable of parsing various dialects of the C language.
    /// </summary>
    public class CParser
    {
        /* ANSI C 89 grammar as specified in
        https://web.archive.org/web/20170605121212/http://flash-gordon.me.uk/ansi.c.txt

	    Processing the C grammar requires LL(1) conflict resolvers, some of which
	    need to check whether an identifier is a type name (see IsTypeName() below).
	    So the grammar assumes that there is a symbol table, from where you can
	    look up an identifier and find out whether it is a type name.
	
	    The language in the semantic actions here is C#, but it can easily be
	    translated to any other language.
        */

        private readonly LookAheadLexer lexer;
        private readonly CGrammar grammar;

        /// <summary>
        /// Constructs a C parser.
        /// </summary>
        /// <param name="parserState">Initial parser state.</param>
        /// <param name="lexer">Lexer to use.</param>
        public CParser(ParserState parserState, CLexer lexer)
        {
            this.ParserState = parserState;
            this.lexer = new LookAheadLexer(
                new StringConcatenator(
                    new CDirectiveLexer(parserState, lexer)));
            this.grammar = new CGrammar();
        }

        /// <summary>
        /// Parser state, which contains the symbol table and other information.
        /// </summary>
        public ParserState ParserState { get; }

#nullable disable

        //------------------ token sets ------------------------------------

        static readonly BitArray startOfTypeName = NewBitArray(
            CTokenType.Const, CTokenType.Volatile, CTokenType.Restrict,
            CTokenType._Atomic,
            CTokenType.Void, CTokenType.Wchar_t,
            CTokenType.Char, CTokenType.Short, CTokenType.Int, CTokenType.__Int64, 
            CTokenType.Long, CTokenType.Double,CTokenType.Float, CTokenType.Signed, 
            CTokenType.Unsigned, CTokenType.Struct,
            CTokenType.Union, CTokenType.Enum,
            CTokenType.__Stdcall);
        static readonly BitArray startOfDecl = NewBitArray(
            CTokenType.Typedef, CTokenType.Extern, CTokenType.Static, CTokenType.Auto,
            CTokenType.Register,
            CTokenType.Const, CTokenType.Volatile, CTokenType.Restrict,
            CTokenType._Atomic,
            CTokenType.Void,
            CTokenType.Bool, CTokenType._Bool,
            CTokenType.Char, CTokenType.Wchar_t, CTokenType.Short, CTokenType.Int, 
            CTokenType.__Int64, CTokenType.Long,CTokenType.Double, CTokenType.Float,
            CTokenType.Signed, CTokenType.Unsigned,CTokenType.Struct, CTokenType.Union,
            CTokenType.Enum, CTokenType._Huge, CTokenType._Far, CTokenType._Huge, CTokenType._Near,
            CTokenType.__Unaligned, CTokenType.__Inline);
        static readonly BitArray startOfDeclarator = NewBitArray(
            CTokenType.Star, CTokenType.Ampersand, CTokenType.LParen, CTokenType.LBracket,
            CTokenType.Semicolon);


        private static BitArray NewBitArray(params CTokenType[] val)
        {
            BitArray s = new BitArray(128);
            foreach (int x in val) s[x] = true;
            return s;
        }

        //---------- LL(1) conflict resolvers ------------------------------

        private CToken PeekToken()
        {
            return lexer.Peek(0);
        }

        private bool PeekThenDiscard(CTokenType type)
        {
            if (lexer.Peek(0).Type != type)
                return false;
            lexer.Read();
            return true;
        }

        private object ExpectToken(CTokenType token)
        {
            var t = lexer.Read();
            if (t.Type != token)
                throw Unexpected(token, t.Type);
            return t.Value;
        }

        private Exception Unexpected(CToken unexpected)
        {
            return new CParserException(
                string.Format("Unexpected token '{0}' ({1}) on line {2}.",
                unexpected.Type,
                unexpected.Value ?? unexpected.Type, lexer.LineNumber));
        }

        private Exception Unexpected(CTokenType expected, CTokenType actual)
        {
            throw new CParserException(string.Format("Expected token '{0}' but saw '{1}' on line {2}.", expected, actual, lexer.LineNumber));
        }

        private bool IsTypeName(CToken x)
        {
            if (x.Type != CTokenType.Id)
                return false;
            return ParserState.Typedefs.Contains((string)x.Value);
        }

        /// <summary>
        /// Determines whether the given string is a type name.
        /// </summary>
        /// <param name="id">String to test.</param>
        /// <returns>True if the string is a known typedef.</returns>
        public bool IsTypeName(string id)
        {
            return ParserState.Typedefs.Contains(id);
        }

        /// <summary>
        /// Return true if the next token is a type name
        /// </summary>
        bool IsType0()
        {
            var token = lexer.Peek(0);
            if (startOfTypeName[(int) token.Type])
                return true;
            return IsTypeName(lexer.Peek(0));
        }

        
        /// <summary>
        /// return true if "(" TypeName
        /// </summary>
        bool IsType1()
        {
            if (lexer.Peek(0).Type != CTokenType.LParen) return false;
            CToken x = lexer.Peek(1);
            if (startOfTypeName[(int)x.Type]) return true;
            return IsTypeName(x);
        }

        /// <summary>
        /// return true if not "," "}"
        /// </summary>
        bool IsContinued()
        {
            if (lexer.Peek(0).Type == CTokenType.Comma)
            {
                CToken x = lexer.Peek(1);
                if (x.Type == CTokenType.RBrace) return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if ",", which is not followed by "..."
        /// </summary>
        bool IsContinued1()
        { 
            if (lexer.Peek(0).Type == CTokenType.Comma)
            {
                CToken x = lexer.Peek(1);
                if (x.Type != CTokenType.Ellipsis)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if ident ":" | "case" | "default"
        /// </summary>
        bool IsLabel()
        {
            var type = lexer.Peek(0).Type;
            if (type == CTokenType.Id)
            {
                CToken x = lexer.Peek(1);
                if (x.Type == CTokenType.Colon)
                    return true;
            }
            else if (type == CTokenType.Case ||
                     type == CTokenType.Default)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if followed by Decl
        /// </summary>
        bool IsDecl()
        {
            if (startOfDecl[(int)lexer.Peek(0).Type])
                return true;
            return IsTypeName(lexer.Peek(0));
        }

        /// <summary>
        /// Return true if there is no non-type-ident after '*', '(', "const", "volatile"
        /// </summary>
        bool IsAbstractDecl()
        {
            int i = 0;
            CToken x = lexer.Peek(i);
            while (x.Type == CTokenType.Star || x.Type == CTokenType.Ampersand || x.Type == CTokenType.LParen || 
                   x.Type == CTokenType.Const || x.Type == CTokenType.Volatile || x.Type == CTokenType.Restrict || 
                   x.Type == CTokenType._Atomic ||
                   x.Type == CTokenType.__Ptr64 ||
                   x.Type == CTokenType.__Fastcall || x.Type == CTokenType.__Stdcall ||
                   x.Type == CTokenType.__Thiscall || x.Type == CTokenType.__Cdecl ||
                   x.Type == CTokenType.__Pascal ||
                   x.Type == CTokenType._Far || x.Type == CTokenType._Huge ||
                   x.Type == CTokenType._Near || x.Type == CTokenType.__Unaligned)
            {
                x = lexer.Peek(++i);
            }
            if (x.Type != CTokenType.Id)
                return true;
            if (!IsTypeName(x))
                return false;
            x = lexer.Peek(++i);
            return x.Type != CTokenType.RParen &&
                x.Type != CTokenType.Comma;
        }

        // return true if '*', '&' '(', '[', ';', noTypeIdent
        bool IsDeclarator(bool checkForTypes)
        {
            var token = lexer.Peek(0);
            if (startOfDeclarator[(int)token.Type])
                return true;
            if (token.Type != CTokenType.Id) 
                return false;
            return !checkForTypes || !IsTypeName(token);
        }

#if not
CHARACTERS
	letter     = 'A'..'Z' + 'a'..'z' + '_'.
	oct        = '0'..'7'.
	digit      = '0'..'9'.
	nzdigit    = '1'..'9'.
	hex        = digit + 'a'..'f' + 'A'..'F'.
	notQuote   = ANY - '"' - "\r\n".
	notApo     = ANY - '\'' - "\r\n".
	
	tab        = '\t'.
	cr         = '\r'.
	lf         = '\n'.
	newLine    = cr + lf.
	notNewLine = ANY - newLine .
	ws         = " " + tab + '\u000b' + '\u000c'.
	
	
TOKENS
	ident    = letter {letter | digit}.
	
	floatcon = ( '.' digit {digit} [('e'|'E')  ['+'|'-'] digit {digit}]
						 | digit {digit} '.' {digit} [('e'|'E')  ['+'|'-'] digit {digit}]
						 | digit {digit} ('e'|'E')  ['+'|'-'] digit {digit}
						 )
						 ['f'|'l'|'F'|'L'].

	intcon   = ( nzdigit {digit}
						 | '0' {oct}
						 | ("0x"|"0X") hex {hex}
						 )
						 {'u'|'U'|'l'|'L'}.
	
	string   = '"' {notQuote} '"'.        // no check for valid escape sequences
	
	charcon  = '\'' notApo {notApo} '\''. // no check for valid escape sequences

	// tokens defined in order to get their names for LL(1) conflict resolvers
	auto      = "auto".
	case      = "case".
	char      = "char".
	const     = "const".
	default   = "default".
	double    = "double".
	enum      = "enum".
	extern    = "extern".
	float     = "float".
	int       = "int".
	long      = "long".
	register  = "register".
	short     = "short".
	signed    = "signed".
	static    = "static".
	struct    = "struct".
	typedef   = "typedef".
	union     = "union".
	unsigned  = "unsigned".
	void      = "void".
	volatile  = "volatile".
	comma     = ','.
	semicolon = ';'.
	colon     = ':'.
	star      = '*'.
	lpar      = '('.
	rpar      = ')'.
	lbrack    = '['.
	rbrace    = '}'.
	ellipsis  = "...".


PRAGMAS
	//---- preprocessor commands (not handled here)
	ppDefine  = '#' {ws} "define" {notNewLine} newLine.
	ppUndef   = '#' {ws} "undef" {notNewLine} newLine.
	ppIf      = '#' {ws} "if" {notNewLine} newLine.
	ppElif    = '#' {ws} "elif" {notNewLine} newLine.
	ppElse    = '#' {ws} "else" {notNewLine} newLine.
	ppEndif   = '#' {ws} "endif" {notNewLine} newLine.
	ppInclude = '#' {ws} "include" {notNewLine} newLine.

COMMENTS FROM "/*" TO "*/"
COMMENTS FROM "//" TO lf

IGNORE tab + cr + lf
#endif

        //---------- Compilation Unit ----------

        /// <summary>
        /// Parses a compilation unit, which consists of a list of external declarations.
        /// </summary>
        /// <returns>A list of external declarations./</returns>
        //CompilationUnit = 
        //    ExternalDecl {ExternalDecl}.
        public List<Decl> Parse()
        {
            var list = new List<Decl>();
            var decl = Parse_ExternalDecl();
            if (decl is null)
                return list;
            list.Add(decl);
            while (PeekToken().Type != CTokenType.EOF)
            {
                list.Add(Parse_ExternalDecl());
            }
            return list;
        }

        /// <summary>
        /// Parses an external declaration, which can be a function definition or a declaration.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CParserException"></exception>
        //ExternalDecl = 
        //  [ AttributeSequence ]
        //  DeclSpecifierList 
        //  ( Declarator 
        //    ( {Decl} '{' {IF(IsDecl()) Decl | Stat} '}'   // FunctionDef
        //    | ['=' Initializer] {',' InitDeclarator}  ';' // Decl
        //    )
        //  | ';'                                           // Decl
        //  ).
#nullable enable
        public Decl? Parse_ExternalDecl()
        {
            // Eat leading semicolons.
            while (PeekThenDiscard(CTokenType.Semicolon))
                ;
            var attrs = ParseAttributeSpecifierSeq();
            var decl_spec_list = Parse_DeclSpecifierList();
            if (decl_spec_list is null)
            {
                if (attrs is null || attrs.Count == 0)
                    return null;
                return new Decl(attrs, new(), new());
            }
            var inits = new List<InitDeclarator>();
            if (PeekThenDiscard(CTokenType.Semicolon))
                return grammar.Decl(attrs, decl_spec_list, inits);
            var declarator = Parse_Declarator();
            var attrsGcc = Parse_GccExtensions();
            var token = PeekToken().Type;
            if (token == CTokenType.Assign ||
                token == CTokenType.Comma ||
                token == CTokenType.Semicolon)
            {
                // Declaration.
                Initializer? init = null;
                if (PeekThenDiscard(CTokenType.Assign))
                {
                    init = Parse_Initializer();
                }
                inits.Add(grammar.InitDeclarator(declarator, init));
                while (PeekThenDiscard(CTokenType.Comma))
                {
                    inits.Add(Parse_InitDeclarator());
                }
                ExpectToken(CTokenType.Semicolon);
                var decl = grammar.Decl(MergeAttributeLists(attrs, attrsGcc), decl_spec_list, inits);
                UpdateNamespaceWithTypedefs(decl_spec_list, inits);
                return decl;
            }
            else if (token != CTokenType.EOF)
            {
                // Function definition
                CToken tok;
                for (;;)
                {
                    tok = lexer.Peek(0);
                    if (tok.Type == CTokenType.EOF || tok.Type == CTokenType.LBrace)
                        break;
                    // Old-style C definition.
                    var decl = Parse_Decl();
                    if (decl is null)
                        break;
                }
                ExpectToken(CTokenType.LBrace);
                var statements = new List<Stat>();
                while (!PeekThenDiscard(CTokenType.RBrace))
                {
                    if (IsDecl())
                    {
                        statements.Add(grammar.DeclStat(Parse_Decl()!));
                    }
                    else
                    {
                        statements.Add(Parse_Stat());
                    }
                }
                return grammar.FunctionDefinition(attrs, decl_spec_list, declarator, statements);
            }
            else
                throw new CParserException($"Expected ';' on line {lexer.LineNumber}.");
        }

#nullable disable

        private List<CAttribute> MergeAttributeLists(params List<CAttribute> [] lists)
        {
            List<CAttribute> totalList = null;
            foreach (var list in lists)
            {
                if (list is not null)
                {
                    if (totalList is null)
                    {
                        totalList = new List<CAttribute>();
                    }
                    totalList.AddRange(list);
                }
            }
            return totalList;
        }

        private void UpdateNamespaceWithTypedefs(List<DeclSpec> declspecs, List<InitDeclarator> declarators)
        {
            if (declspecs.Count == 0)
               return;
            if (declspecs[0] is StorageClassSpec typedefSpec && typedefSpec.Type == CTokenType.Typedef)
            {
                foreach (var declarator in declarators)
                {
                    if (declarator.Init is not null)
                        throw new CParserException("typedefs can't be initialized.");
                    var name = NameExtractor.GetName(declspecs.Skip(1), declarator.Declarator, ParserState);
                    ParserState.Typedefs.Add(name);
                }
            }
        }

        //---------- Declarations ----------

        /// <summary>
        /// Parses a declaration, which can be a typedef, variable, or function declaration.
        /// </summary>
        /// <returns></returns>
        //Decl ::= [attributes-seq] DeclSpecifierList [InitDeclarator {',' InitDeclarator}] ';'.
        public Decl Parse_Decl()
        {
            var attrs = ParseAttributeSpecifierSeq();
            var declSpecifiers = Parse_DeclSpecifierList();
            if (declSpecifiers is null)
                return null;
            var listDecls = new List<InitDeclarator>();
            if (!PeekThenDiscard(CTokenType.Semicolon))
            {
                listDecls.Add(Parse_InitDeclarator());
                while (PeekThenDiscard(CTokenType.Comma))
                {
                    listDecls.Add(Parse_InitDeclarator());
                }
                ExpectToken(CTokenType.Semicolon);
            }
            //UpdateNamespaceWithTypedefs(decl_spec_list, inits);
            return grammar.Decl(attrs, declSpecifiers, listDecls);
        }

        /// <summary>
        /// Parses a (possibly initialized) declarator.
        /// </summary>
        /// <remarks>
        /// InitDeclarator ::= Declarator ['=' Initializer].
        /// </remarks>
        private InitDeclarator Parse_InitDeclarator()
        {
            var decl = Parse_Declarator();
            Initializer init = null;
            if (PeekThenDiscard(CTokenType.Assign))
            {
                init = Parse_Initializer();
            }
            return grammar.InitDeclarator(decl, init);
        }

        // DeclSpecifierList ::= DeclSpecifier {IF(!IsDeclarator()) DeclSpecifier}.
        private List<DeclSpec> Parse_DeclSpecifierList()
        {
            int idsSeen = 0;
            int typeDeclsSeen = 0;
            var list = new List<DeclSpec>();
            var ds = Parse_DeclSpecifier();
            if (ds is null)
                return null;
            list.Add(ds);
            bool inTypeDef = (ds is StorageClassSpec scspec && scspec.Type == CTokenType.Typedef);
                
            while (!IsComplexType(ds))
            {
                var token = lexer.Peek(0);
                if (inTypeDef)
                {
                    if (startOfDeclarator[(int) token.Type])
                        break;
                    if (token.Type == CTokenType.Id && typeDeclsSeen > 0)
                        break;
                }
                else
                {
                    if (IsDeclarator(true))
                        break;
                }
                if (token.Type == CTokenType.Id)
                    ++idsSeen;
                ds = Parse_DeclSpecifier();
                if (ds is null)
                    break;
                if (ds is TypeSpec)
                    ++typeDeclsSeen;
                list.Add(ds);
            }
            return list;
        }

        private bool IsComplexType(DeclSpec ds)
        {
            return ds is ComplexTypeSpec;
        }

        //DeclSpecifier ::=
        //        "typedef" | "extern" | "static" | "auto" | "register" // storage class specifier
        //    | "const" | "volatile"                                  // TypeQualifier
        //    | TypeSpecifier.
        private DeclSpec Parse_DeclSpecifier()
        {
            var token = PeekToken();
            switch (token.Type)
            {
            case CTokenType.Typedef:
            case CTokenType.Extern:
            case CTokenType.Static:
            case CTokenType.Auto:
            case CTokenType.Register:
            case CTokenType.__Cdecl:
            case CTokenType.__Fastcall:
            case CTokenType.__ForceInline:
            case CTokenType.__Inline:
            case CTokenType.__LoadDs:
            case CTokenType.__Pascal:
            case CTokenType.__Stdcall:
            case CTokenType.__Thiscall:
                return grammar.StorageClass(lexer.Read().Type);
            case CTokenType.Const:
            case CTokenType.Volatile:
            case CTokenType.Restrict:
            case CTokenType.__Ptr64:
            case CTokenType._Atomic:
            case CTokenType._Far:
            case CTokenType._Huge:
            case CTokenType._Near:
            case CTokenType.__Unaligned:
                return grammar.TypeQualifier(lexer.Read().Type);
            case CTokenType.__Declspec:
                lexer.Read();
                ExpectToken(CTokenType.LParen);
                var sToken = lexer.Read();
                if (sToken.Type == CTokenType.Restrict)
                {
                    ExpectToken(CTokenType.RParen);
                    return grammar.ExtendedDeclspec("restrict");
                }
                else if (sToken.Type == CTokenType.Id)
                {
                    var s = (string) sToken.Value;
                    if (s == "align")
                    {
                        ExpectToken(CTokenType.LParen);
                        ExpectToken(CTokenType.NumericLiteral);
                        ExpectToken(CTokenType.RParen);
                    }
                    else if (s == "deprecated")
                    {
                        if (PeekThenDiscard(CTokenType.LParen))
                        {
                            ExpectToken(CTokenType.StringLiteral);
                            ExpectToken(CTokenType.RParen);
                        }
                    }
                    else if (s == "dllimport")
                    {
                    }
                    else if (s == "noreturn")            //$BUG: use for termination analysis
                    {
                    }
                    else if (s == "noalias")
                    {
                    }
                    else if (s == "allocator")
                    {
                    }
                    else
                        throw new CParserException($"Unknown __declspec '{s}'.");
                    ExpectToken(CTokenType.RParen);
                    return grammar.ExtendedDeclspec(s);
                }
                else
                {
                    throw new CParserException($"Unknown __declspec '{sToken}'.");
                }
            case CTokenType.__Success:
                lexer.Read();
                ExpectToken(CTokenType.LParen);
                ExpectToken(CTokenType.Return);
                lexer.Read();   //   >= 
                lexer.Read();   // 0
                ExpectToken(CTokenType.RParen);
                return Parse_DeclSpecifier();
            default:
                return Parse_TypeSpecifier();
            }
        }

        // TypeSpecifier =
        //      "void" | "char" | "short" | "int" | "long" | "float" | "double" | "signed" | "unsigned"
        //  | ident // type name
        //| ("struct" | "union")
        //  ( ident ['{' StructDecl {StructDecl} '}']
        //  | '{' StructDecl {StructDecl} '}'
        //  )
        //| "enum"
        //  ( ident ['{' Enumerator {',' Enumerator} '}']
        //  | '{' Enumerator {',' Enumerator} '}'
        //  ).

        // struct __declspec(align(16)) X 
        // 
        private TypeSpec Parse_TypeSpecifier()
        {
            var token = PeekToken();
            switch (token.Type)
            {
            case CTokenType.Void:
            case CTokenType.Bool:
            case CTokenType.Char:
            case CTokenType.Short:
            case CTokenType.Int:
            case CTokenType.__Int64:
            case CTokenType.Long:
            case CTokenType.Float:
            case CTokenType.Double:
            case CTokenType.Signed:
            case CTokenType.Unsigned:
            case CTokenType.Wchar_t:
            case CTokenType._Bool:
            case CTokenType.__W64:
                return grammar.SimpleType(lexer.Read().Type);
            case CTokenType.Id: // type name
                return grammar.TypeName((string) lexer.Read().Value);
            case CTokenType.Struct:
            case CTokenType.Union:
                lexer.Read();
                int alignment = ParserState.Alignment;
                string tag = null;
                List<StructDecl> decls = null;
                if (PeekThenDiscard(CTokenType.__Declspec))
                {
                    ExpectToken(CTokenType.LParen);
                    var s= (string)ExpectToken(CTokenType.Id);
                    if (s == "align")
                    {
                        ExpectToken(CTokenType.LParen);
                        alignment = (int) ExpectToken(CTokenType.NumericLiteral);
                        ExpectToken(CTokenType.RParen);
                    }
                    else if (s == "no_init_all")
                    {
                    }
                    else
                        throw new CParserException($"Unexpected __declspec({s}).");
                    ExpectToken(CTokenType.RParen);
                }
                var attrs = Parse_GccExtensions();
                if (attrs is not null)
                {
                    var alignAttr = attrs.FirstOrDefault(a => a.Name.ToString() == "aligned");
                    if (alignAttr is not null && alignAttr.Tokens.Count == 1 && alignAttr.Tokens[0].Type == CTokenType.NumericLiteral)
                    {
                        alignment = (int) alignAttr.Tokens[0].Value;
                    }
                }
                if (PeekToken().Type == CTokenType.Id)
                {
                    tag = (string) lexer.Read().Value;
                    if (PeekThenDiscard(CTokenType.LBrace))
                    {
                        decls = new List<StructDecl>();
                        while (PeekToken().Type != CTokenType.RBrace) 
                        {
                            decls.Add(Parse_StructDecl());
                        }
                        ExpectToken(CTokenType.RBrace);
                    }
                }
                else
                {
                    ExpectToken(CTokenType.LBrace);
                    decls = new List<StructDecl>();
                    do
                    {
                        decls.Add(Parse_StructDecl());
                    } while (PeekToken().Type != CTokenType.RBrace);
                    ExpectToken(CTokenType.RBrace);
                }
                return grammar.ComplexType(token.Type, alignment, tag, decls);
            case CTokenType.Enum:
                lexer.Read();
                List<Enumerator> enums = null;
                if (PeekToken().Type == CTokenType.Id)
                {
                    tag = (string) lexer.Read().Value;
                    if (PeekThenDiscard(CTokenType.LBrace))
                    {
                        enums = new List<Enumerator>();
                        do
                        {
                            enums.Add(Parse_Enumerator());
                            PeekThenDiscard(CTokenType.Comma);
                        } while (PeekToken().Type != CTokenType.RBrace);
                        ExpectToken(CTokenType.RBrace);
                    }
                }
                else
                {
                    tag = null;
                    ExpectToken(CTokenType.LBrace);
                    enums = new List<Enumerator>();
                    do
                    {
                        enums.Add(Parse_Enumerator());
                        PeekThenDiscard(CTokenType.Comma);
                    } while (lexer.Peek(0).Type != CTokenType.RBrace);
                    ExpectToken(CTokenType.RBrace);
                }
                return grammar.Enum(tag, enums);
            case CTokenType.EOF:
            case CTokenType.LParen:
            case CTokenType.RParen:
            case CTokenType.Colon:
            case CTokenType.Const:
            case CTokenType.Volatile:
            case CTokenType.Restrict:
            case CTokenType.Comma:
            case CTokenType.RBrace:
            case CTokenType._Atomic:
            case CTokenType._Far:
            case CTokenType._Huge:
            case CTokenType._Near:
            case CTokenType.__Unaligned:
            case CTokenType.Star:
            case CTokenType.Semicolon:
                return null;
            default:
                throw Unexpected(token);
            }
        }

        //StructDecl = SpecifierQualifierList StructDeclarator {',' StructDeclarator} ';'.

        private StructDecl Parse_StructDecl()
        {
            var gccAttr = Parse_GccExtensions();
            var sql = Parse_SpecifierQualifierList();
            List<FieldDeclarator> decls = new List<FieldDeclarator>();
            decls.Add(Parse_StructDeclarator());
            while (PeekThenDiscard(CTokenType.Comma))
            {
                decls.Add(Parse_StructDeclarator());
            }
            var gccAttr2 = Parse_GccExtensions();
            ExpectToken(CTokenType.Semicolon);
            return grammar.StructDecl(sql, decls, CollectAttributes(gccAttr, gccAttr2));
        }

        private List<CAttribute> CollectAttributes(List<CAttribute> list1, List<CAttribute> list2)
        {
            if (list1 is null)
                return list2;
            if (list2 is null)
                return list1;
            list1.AddRange(list2);
            return list1;
        }

        //StructDeclarator = Declarator [':' ConstExpr] | ':'  ConstExpr.

        private FieldDeclarator Parse_StructDeclarator()
        {
            Declarator decl = null;
            CExpression bitField = null;
            if (PeekToken().Type != CTokenType.Colon)
            {
                decl = Parse_Declarator();
            }
            if (PeekThenDiscard(CTokenType.Colon))
            {
                bitField = Parse_ConstExpr();
            }
            return grammar.FieldDeclarator(decl, bitField);
        }

        //Enumerator = ident ['=' ConstExpr].

        private Enumerator Parse_Enumerator()
        {
            var id = (string) ExpectToken(CTokenType.Id);
            CExpression init = null;
            if (PeekThenDiscard(CTokenType.Assign))
            {
                init = Parse_ConstExpr();
            }
            return grammar.Enumerator(id, init);
        }

        //SpecifierQualifierList =
        //  (TypeSpecifier | TypeQualifier)
        //  { IF(!IsDeclarator())
        //    (TypeSpecifier | TypeQualifier)
        //  }.
        private List<DeclSpec> Parse_SpecifierQualifierList()
        {
            var sql = new List<DeclSpec>();
            while (true)
            {
                DeclSpec t = Parse_TypeSpecifier();
                if (t is null)
                    t = Parse_TypeQualifier();
                if (t is null)
                    break;
                sql.Add(t);
                var nextToken = PeekToken();
                if (nextToken.Type == CTokenType.Id &&
                    (t is TypeDefName || t is TypeSpec))
                    break;
            }
            return sql;
        }

        // TypeQualifier ::= "const" | "volatile".
        private TypeQualifier Parse_TypeQualifier()
        {
            var token = PeekToken();
            switch (token.Type)
            {
            case CTokenType.Const:
            case CTokenType.Volatile:
            case CTokenType.Restrict:
            case CTokenType.__Ptr64:
            case CTokenType._Atomic:
                lexer.Read();
                return grammar.TypeQualifier(token.Type);
            }
            return null;
        }

        /// <summary>
        /// Parses a declarator, which can be a pointer, reference, or function pointer.
        /// </summary>
        /// <returns></returns>
        //Declarator ::=
        //    [Pointer]
        //    ( ident
        //    | '(' Declarator ')'
        //    )
        //    { '[' [ConstExpr] ']' 
        //    | '(' [IF(!IsType0()) IdentList | ParamTypeList] ')' 
        //    }.
#nullable enable
        public Declarator? Parse_Declarator()
        {
            Declarator? decl;
            var token = lexer.Peek(0).Type;
            switch (token)
            {
            case CTokenType.Id:
                decl = grammar.IdDeclarator((string) lexer.Read().Value!);
                break;
            case CTokenType.LParen:
                ExpectToken(CTokenType.LParen);
                decl = Parse_Declarator();
                ExpectToken(CTokenType.RParen);
                break;
            case CTokenType.Star:
                return Parse_Pointer();
            case CTokenType.Ampersand:
                return Parse_Reference();
            case CTokenType._Far:
            case CTokenType._Huge:
            case CTokenType._Near:
            case CTokenType.__Unaligned:
            case CTokenType.Const:
            case CTokenType.Volatile:
            case CTokenType.Restrict:
                var tq = grammar.TypeQualifier(lexer.Read().Type);
                decl = Parse_Declarator();
                if (decl is PointerDeclarator ptr)
                {
                    if (ptr.TypeQualifierList is null)
                        ptr.TypeQualifierList = new List<TypeQualifier>();
                    ptr.TypeQualifierList.Add(tq);
                }
                break;
            case CTokenType.__Cdecl:
            case CTokenType.__Fastcall:
            case CTokenType.__Pascal:
            case CTokenType.__Stdcall:
            case CTokenType.__Thiscall:
                lexer.Read();
                decl = Parse_Declarator();
                if (decl is null)
                    return null;
                return grammar.CallConventionDeclarator(token, decl);
            default:
                return null;
            }
            for (; ; )
            {
                switch (PeekToken().Type)
                {
                case CTokenType.LBracket:
                    lexer.Read();
                    CExpression? expr = null;
                    if (PeekToken().Type != CTokenType.RBracket)
                    {
                        expr = Parse_ConstExpr();
                    }
                    ExpectToken(CTokenType.RBracket);
                    decl = grammar.ArrayDeclarator(decl!, expr);
                    break;
                case CTokenType.LParen:
                    lexer.Read();
                    if (lexer.Peek(0).Type == CTokenType.RParen)
                    {
                        var parameters = new List<ParamDecl>();
                        decl = grammar.FunctionDeclarator(decl!, parameters);
                    } 
                    else if (!IsType0() && lexer.Peek(0).Type != CTokenType.LBracket)
                    {
                        Parse_IdentList();
                    }
                    else
                    {
                        var parameters = Parse_ParamTypeList();
                        decl = grammar.FunctionDeclarator(decl!, parameters!); 
                    }
                    ExpectToken(CTokenType.RParen);
                    break;
                default: 
                    return decl;
                }
            }
        }

        // Pointer = '*'  {TypeQualifier} {'*'  {TypeQualifier}}.

        Declarator Parse_Pointer()
        {
            ExpectToken(CTokenType.Star);
            List<TypeQualifier>? tqs = null;
            var tq = Parse_TypeQualifier();
            while (tq is not null)
            {
                if (tqs is null)
                    tqs = new List<TypeQualifier>();
                tqs.Add(tq);
                tq = Parse_TypeQualifier();
            }
            var declarator = Parse_Declarator();
            return grammar.PointerDeclarator(declarator, tqs);
        }

        Declarator Parse_Reference()
        {
            ExpectToken(CTokenType.Ampersand);
            List<TypeQualifier>? tqs = null;
            var tq = Parse_TypeQualifier();
            while (tq is not null)
            {
                if (tqs is null)
                    tqs = new List<TypeQualifier>();
                tqs.Add(tq);
                tq = Parse_TypeQualifier();
            }
            var declarator = Parse_Declarator();
            return grammar.ReferenceDeclarator(declarator, tqs);
        }

        Declarator Parse_AbstractPointer()
        {
            ExpectToken(CTokenType.Star);
            List<TypeQualifier>? tqs = null;
            var tq = Parse_TypeQualifier();
            while (tq is not null)
            {
                if (tqs is null)
                    tqs = new List<TypeQualifier>();
                tqs.Add(tq);
                tq = Parse_TypeQualifier();
            }
            var declarator = Parse_DirectAbstractDeclarator();
            return grammar.PointerDeclarator(declarator, tqs);
        }

        // ParamTypeList ::= ParamDecl {IF(Continued1()) ',' ParamDecl} [',' "..."].

        List<ParamDecl>? Parse_ParamTypeList()
        {
            var pds = new List<ParamDecl>();
            var pd = Parse_ParamDecl();
            if (pd is null)
                return null;
            pds.Add(pd);
            while (IsContinued1())
            {
                ExpectToken(CTokenType.Comma);
                var paramDecl = Parse_ParamDecl();
                if (paramDecl is null)
                    return pds;
                pds.Add(paramDecl);
            }
            if (PeekThenDiscard(CTokenType.Comma))
            {
                ExpectToken(CTokenType.Ellipsis);
                pds.Add(grammar.Ellipsis());
            }
            return pds;
        }

        // ParamDecl ::= DeclSpecifierList [IF(IsAbstractDecl()) AbstractDeclarator | Declarator].

        ParamDecl? Parse_ParamDecl()
        {
            List<CAttribute>? attrs = ParseAttributeSpecifierSeq();
            var dsl = Parse_DeclSpecifierList();
            if (dsl is null)
                return null;
            Declarator? decl;
            if (IsAbstractDecl())
            {
                decl = Parse_AbstractDeclarator();
            }
            else
            {
                decl = Parse_Declarator();
            }
            return grammar.ParamDecl(attrs, dsl, decl);
        }

        //IdentList = ident {',' ident}.

        List<CIdentifier> Parse_IdentList()
        {
            var list = new List<CIdentifier>();
            list.Add(grammar.Id((string) ExpectToken(CTokenType.Id)));
            while (PeekThenDiscard(CTokenType.Comma))
            {
                list.Add(grammar.Id((string) ExpectToken(CTokenType.Id)));
            }
            return list;
        }

        //    TypeName = // a better name would be Type
        //          SpecifierQualifierList [AbstractDeclarator].

        CType Parse_TypeName()
        {
            var sql = Parse_SpecifierQualifierList();
            Declarator? decl = null;
            switch (PeekToken().Type)
            {
            case CTokenType.Star:
            case CTokenType.LParen:
            case CTokenType.LBracket:
                decl = Parse_AbstractDeclarator();
                break;
            }
            return new CType(sql, decl);
        }

        //AbstractDeclarator =
        //    Pointer [DirectAbstractDeclarator]
        // | DirectAbstractDeclarator.
        // | Pointer DirectAbstractDeclarator.

        Declarator? Parse_AbstractDeclarator()
        {
            Declarator? decl;
            var token = lexer.Peek(0);
            switch (token.Type)
            {
            case CTokenType.__Cdecl:
            case CTokenType.__Pascal:
                lexer.Read();
                decl = Parse_AbstractPointer();
                return grammar.CallConventionDeclarator(token.Type, decl);
            case CTokenType.Star:
                var ptr = Parse_Pointer();
                decl = Parse_DirectAbstractDeclarator();
                if (decl is null)
                    return ptr;
                return ptr;
            default:
                return Parse_DirectAbstractDeclarator();
            }
        }

        //DirectAbstractDeclarator =
        //    ( '(' [AbstractDeclarator | ParamTypeList] ')'
        //    | '[' [ConstExpr] ']'
        //    )
        //    { '[' [ConstExpr] ']' 
        //    | '(' [ParamTypeList] ')'
        //    }
        Declarator? Parse_DirectAbstractDeclarator()
        {
            CExpression? expr;
            Declarator? decl = null;
            var token = lexer.Peek(0);
            switch (token.Type)
            {
            case CTokenType.LParen:
                lexer.Read();
                decl = Parse_AbstractDeclarator();
                ExpectToken(CTokenType.RParen);
                break;
            case CTokenType.LBracket:
                lexer.Read();
                expr = null;
                if (PeekToken().Type != CTokenType.RBracket)
                {
                    expr = Parse_ConstExpr();
                }
                ExpectToken(CTokenType.RBracket);
                decl = grammar.ArrayDeclarator(decl!, expr);
                break;
            default:
                return null;
            }

            for (; ; )
            {
                token = lexer.Peek(0);
                switch (token.Type)
                {
                case CTokenType.LParen:
                    lexer.Read();
                    var ptl2 = Parse_ParamTypeList();
                    ExpectToken(CTokenType.RParen);
                    decl = grammar.FunctionDeclarator(decl!, ptl2!);
                    break;
                case CTokenType.LBracket:
                    lexer.Read();
                    expr = null;
                    if (PeekToken().Type != CTokenType.RBracket)
                    {
                        expr = Parse_ConstExpr();
                    }
                    ExpectToken(CTokenType.RBracket);
                    decl = grammar.ArrayDeclarator(decl!, expr);
                    break;
                default:
                    return decl;
                }
            }
        }

#nullable disable

        //Initializer = 
        //    AssignExpr 
        //  | '{'  Initializer {IF(Continued()) ',' Initializer} [','] '}'.

        Initializer Parse_Initializer()
        {
            if (PeekThenDiscard(CTokenType.LBrace))
            {
                var list = new List<Initializer>();
                list.Add(Parse_Initializer());
                while (IsContinued())
                {
                    ExpectToken(CTokenType.Comma);
                    list.Add(Parse_Initializer());
                }
                PeekThenDiscard(CTokenType.Comma);  // trailing comma
                ExpectToken(CTokenType.RBrace);
                return grammar.ListInitializer(list);
            }
            else
            {
                var expr = Parse_AssignExpr();
                return grammar.ExpressionInitializer(expr);
            }
        }

#nullable enable
        // May be null.
        // AttributeSpecifier ::= [ [ <attrs> ] ] 
        private List<CAttribute>? ParseAttributeSpecifierSeq()
        {
            if (lexer.Peek(0).Type == CTokenType.LBracket &&
                lexer.Peek(1).Type == CTokenType.LBracket)
            {
                var attrs = new List<CAttribute>();
                do
                {
                    attrs.Add(Parse_AttributeSpecifier());
                } while (lexer.Peek(0).Type == CTokenType.LBracket &&
                         lexer.Peek(1).Type == CTokenType.LBracket);
                return attrs;
            }
            return null;
        }

        // May be null:
        private List<CAttribute>? Parse_GccExtensions()
        {
            List<CAttribute>? attrs = null;
            for (; ;)
            {
                if (PeekThenDiscard(CTokenType.__Attribute))
                {
                    attrs = attrs ?? new List<CAttribute>();
                    attrs.AddRange(Parse_GccAttributeSpecifier());
                }
                else if (PeekThenDiscard(CTokenType.__Asm))
                {
                    attrs = attrs ?? new List<CAttribute>();
                    attrs.Add(Parse_GccAsm());
                }
                else
                    return attrs;
            }
        }


        /// <summary>
        /// Parse an attribute specifier.
        /// </summary>
        /// <returns></returns>
        public CAttribute Parse_AttributeSpecifier()
        {
            ExpectToken(CTokenType.LBracket);
            ExpectToken(CTokenType.LBracket);
            QualifiedName qname = Parse_AttributeToken();
            List<CToken>? tokens = null;
            if (PeekThenDiscard(CTokenType.LParen))
            {
                tokens = Parse_BalancedTokenSeq();
                ExpectToken(CTokenType.RParen);
            }
            ExpectToken(CTokenType.RBracket);
            ExpectToken(CTokenType.RBracket);
            return new CAttribute
            {
                Name = qname,
                Tokens = tokens
            };
        }

#nullable disable


        private List<CAttribute> Parse_GccAttributeSpecifier()
        {
            var attrs = new List<CAttribute>();
            ExpectToken(CTokenType.LParen);
            ExpectToken(CTokenType.LParen);
            while (PeekToken().Type != CTokenType.RParen)
            {
                string name = (string) ExpectToken(CTokenType.Id);
                List<CToken> tokens = null;
                if (PeekThenDiscard(CTokenType.LParen))
                {
                    tokens = Parse_BalancedTokenSeq();
                    ExpectToken(CTokenType.RParen);
                }
                attrs.Add(new CAttribute
                {
                    Name = new QualifiedName(name),
                    Tokens = tokens
                });
                PeekThenDiscard(CTokenType.Comma);
            }
            ExpectToken(CTokenType.RParen);
            ExpectToken(CTokenType.RParen);
            return attrs;
        }

        private CAttribute Parse_GccAsm()
        {
            var names = new List<CToken>();
            ExpectToken(CTokenType.LParen);
            while (!PeekThenDiscard(CTokenType.RParen))
            {
                var name = lexer.Read();
                if (name.Type != CTokenType.StringLiteral)
                    Unexpected(CTokenType.StringLiteral, name.Type);
                names.Add(name);
            }
            return new CAttribute
            {
                Name = new QualifiedName("__asm__"),
                Tokens = names
            };
        }

        private QualifiedName Parse_AttributeToken()
        {
            var comps = new List<string>();
            comps.Add((string)ExpectToken(CTokenType.Id));
            while (PeekThenDiscard(CTokenType.ColonColon))
            {
                comps.Add((string)ExpectToken(CTokenType.Id));
            }
            return new QualifiedName(comps.ToArray());
        }

        private List<CToken> Parse_BalancedTokenSeq()
        {
            var tokens = new List<CToken>();
            for (;;)
            {
                var tok = lexer.Peek(0);
                switch (tok.Type)
                {
                case CTokenType.RParen:
                case CTokenType.RBrace:
                case CTokenType.RBracket:
                case CTokenType.EOF:
                    return tokens;
                default:
                    var sublist = Parse_BalancedToken();
                    tokens.AddRange(sublist);
                    break;
                }
            }
        }

        private List<CToken> Parse_BalancedToken()
        {
            var tokens = new List<CToken>();
            var token = lexer.Read();
            switch (token.Type)
            {
            case CTokenType.EOF:
                Unexpected(CTokenType.LParen, token.Type);
                break;
            case CTokenType.LParen:
                tokens.Add(token);
                tokens.AddRange(Parse_BalancedTokenSeq());
                token = lexer.Read();
                if (token.Type != CTokenType.RParen)
                    throw Unexpected(CTokenType.RParen, token.Type);
                tokens.Add(token);
                break;
            case CTokenType.LBrace:
                tokens.Add(token);
                tokens.AddRange(Parse_BalancedTokenSeq());
                token = lexer.Read();
                if (token.Type != CTokenType.RBrace)
                    throw Unexpected(CTokenType.RBrace, token.Type);
                tokens.Add(token);
                break;
            case CTokenType.LBracket:
                tokens.Add(token);
                tokens.AddRange(Parse_BalancedTokenSeq());
                token = lexer.Read();
                if (token.Type != CTokenType.RBracket)
                    throw Unexpected(CTokenType.RBracket, token.Type);
                tokens.Add(token);
                break;
            default:
                tokens.Add(token);
                break;
            }
            return tokens;
        }

        //---------- Expressions ----------

        /// <summary>
        /// Parse C expression.
        /// </summary>
        /// <param name="min_bp"></param>
        /// <returns></returns>
        /// <remarks>
        /// This implementation uses the Pratt precedence-based parser algorithm.
        /// It is friendlier on the stack space.</remarks>
        public CExpression expr_bp(int min_bp)
        {
            var token = lexer.Read();
            CExpression lhs;
            switch (token.Type)
            {
            case CTokenType.Id:
                lhs = new CIdentifier((string)token.Value);
                break;
            case CTokenType.NumericLiteral:
            case CTokenType.StringLiteral:
            case CTokenType.WideStringLiteral:
            case CTokenType.RealLiteral:
            case CTokenType.CharLiteral:
            case CTokenType.WideCharLiteral:
                lhs = grammar.Const(lexer.Read().Value);
                break;
            case CTokenType.LParen:
                if (IsType0())
                {
                    //$TODO: cast
                }
                lhs = expr_bp(0);
                ExpectToken(CTokenType.RParen);
                break;
            default:
                var r_bp = prefixBindingPower(token.Type);
                if (r_bp is null)
                    throw Unexpected(token);
                var rhs = expr_bp(r_bp.Value);
                lhs = new CUnaryExpression(token.Type, rhs);
                break;
            }
            for (; ; )
            {
                token = lexer.Peek(0);
                if (token.Type == CTokenType.EOF)
                    return lhs;
                var l_bp = postfixBindingPower(token.Type);
                if (l_bp is not null)
                {
                    if (l_bp.Value < min_bp)
                        return lhs;
                    lexer.Read();
                    if (token.Type == CTokenType.LBracket)
                    {
                        var rhs = expr_bp(0);
                        ExpectToken(CTokenType.RBracket);
                        lhs = new CArrayAccess(lhs, rhs);
                    }
                    else
                    {
                        lhs = new CUnaryExpression(token.Type, lhs);
                    }
                    continue;
                }
                (l_bp, int? r_bp) = infixBindingPower(token.Type);
                if (l_bp is not null)
                {
                    if (l_bp < min_bp)
                    {
                        return lhs;
                    }
                    lexer.Read();
                    if (token.Type == CTokenType.Question)
                    {
                        var mhs = expr_bp(0);
                        ExpectToken(CTokenType.Colon);
                        var rhs = expr_bp(r_bp.Value);
                        lhs = new ConditionalExpression(lhs, mhs, rhs);
                    }
                    else
                    {
                        var rhs = expr_bp(r_bp.Value);
                        lhs = new CBinaryExpression(token.Type, lhs, rhs);
                        continue;
                    }
                    return lhs;
                }
            }
        }

        private static int? prefixBindingPower(CTokenType tokenType)
        {
            return bindingPowers.TryGetValue(tokenType, out var bp)
                ? bp.Item1
                : null;
        }
        private static (int?, int?) infixBindingPower(CTokenType tokenType)
        {
            return bindingPowers.TryGetValue(tokenType, out var bp)
                ? (bp.Item2, bp.Item3)
                : (null, null);
        }
        private static int? postfixBindingPower(CTokenType tokenType)
        {
            return bindingPowers.TryGetValue(tokenType, out var bp)
                ? bp.Item3
                : null;
        }



        private static readonly Dictionary<CTokenType, (int?, int?, int?)> bindingPowers = new()
        {
            {  CTokenType.Comma, (null, 1, 2)},

            { CTokenType.Assign, (null, 3, 4) },
            { CTokenType.MulAssign, (null, 3, 4) },
            { CTokenType.DivAssign, (null, 3, 4) },
            { CTokenType.ModAssign, (null, 3, 4) },
            { CTokenType.PlusAssign, (null, 3, 4) },
            { CTokenType.MinusAssign, (null, 3, 4) },
            { CTokenType.ShlAssign, (null, 3, 4) },
            { CTokenType.ShrAssign, (null, 3, 4) },
            { CTokenType.AndAssign, (null, 3, 4) },
            { CTokenType.OrAssign, (null, 3, 4) },
            { CTokenType.XorAssign, (null, 3, 4) },

            { CTokenType.Question, (null, 6, 5) },

            { CTokenType.LogicalOr, (null, 7, 8) },

            { CTokenType.LogicalAnd, (null, 9, 10) },

            { CTokenType.Pipe, (null, 11, 12) },

            { CTokenType.Xor, (null, 13, 14) },

            { CTokenType.Ampersand, (null, 15, 16) },

            { CTokenType.Eq, (null, 17, 18) },
            { CTokenType.Ne, (null, 17, 18) },

            { CTokenType.Ge, (null, 19, 20) },
            { CTokenType.Gt, (null, 19, 20) },
            { CTokenType.Le, (null, 19, 20) },
            { CTokenType.Lt, (null, 19, 20) },

            { CTokenType.Shl, (null, 21, 22) },
            { CTokenType.Shr, (null, 21, 22) },

            { CTokenType.Plus, (null, 23, 24) },
            { CTokenType.Minus, (null, 23, 24) },

            { CTokenType.Star, (null, 25, 26) },
            { CTokenType.Slash, (null, 25, 26) },
            { CTokenType.Percent, (null, 25, 26) },


        };


        /// <summary>
        /// Parse a C expression.
        /// </summary>
        /// <returns></returns>
        //Expr       = AssignExpr {','  AssignExpr}.
        public CExpression Parse_Expr()
        {
            //return this.expr_bp(0);

            var left = Parse_AssignExpr();
            while (PeekThenDiscard(CTokenType.Comma))
            {
                var right = Parse_AssignExpr();
                left = grammar.Bin(CTokenType.Comma, left, right);
            }
            return left;
        }

        //AssignExpr = CondExpr [AssignOp AssignExpr]. // relaxed
        private CExpression Parse_AssignExpr()
        {
            var left = Parse_CondExpr();
            var token = PeekToken();
            switch (token.Type)
            {
            case CTokenType.Assign:
            case CTokenType.MulAssign:
            case CTokenType.DivAssign:
            case CTokenType.ModAssign:
            case CTokenType.PlusAssign:
            case CTokenType.MinusAssign:
            case CTokenType.ShlAssign:
            case CTokenType.ShrAssign:
            case CTokenType.AndAssign:
            case CTokenType.OrAssign:
            case CTokenType.XorAssign:
                lexer.Read();
                var right = Parse_AssignExpr();
                return grammar.Bin(token.Type, left, right);
            default:
                return left;
            }
        }

        //CondExpr   = LogOrExpr ['?' Expr ':' CondExpr].
        private CExpression Parse_CondExpr()
        {
            var cond = Parse_LogOrExpr();
            if (PeekThenDiscard(CTokenType.Question))
            {
                var consequent = Parse_Expr();
                ExpectToken(CTokenType.Colon);
                var alternant = Parse_CondExpr();
                return grammar.Conditional(cond, consequent, alternant);
            }
            return cond;
        }

        //LogOrExpr  = LogAndExpr {"||" LogAndExpr}.
        private CExpression Parse_LogOrExpr()
        {
            var left = Parse_LogAndExpr();
            while (PeekThenDiscard(CTokenType.LogicalOr))
            {
                var right = Parse_LogAndExpr();
                left = grammar.Bin(CTokenType.LogicalOr, left, right);
            }
            return left;
        }

        //LogAndExpr = OrExpr {"&&" OrExpr}.
        private CExpression Parse_LogAndExpr()
        {
            var left = Parse_OrExpr();
            while (PeekThenDiscard(CTokenType.LogicalAnd))
            {
                var right = Parse_OrExpr();
                left = grammar.Bin(CTokenType.LogicalAnd, left, right);
            }
            return left;
        }
        //OrExpr     = XorExpr {'|' XorExpr}.
        private CExpression Parse_OrExpr()
        {
            var left = Parse_XorExpr();
            while (PeekThenDiscard(CTokenType.Pipe))
            {
                var right = Parse_XorExpr();
                left = grammar.Bin(CTokenType.Pipe, left, right);
            }
            return left;
        }

        //XorExpr    = AndExpr {'^' AndExpr}.
        private CExpression Parse_XorExpr()
        {
            var left = Parse_AndExpr();
            while (PeekThenDiscard(CTokenType.Xor))
            {
                var right = Parse_AndExpr();
                left = grammar.Bin(CTokenType.Xor, left, right);
            }
            return left;
        }

        //AndExpr    = EqlExpr {'&' EqlExpr}.
        private CExpression Parse_AndExpr()
        {
            var left = Parse_EqlExpr();
            while (PeekThenDiscard(CTokenType.Ampersand))
            {
                var right = Parse_EqlExpr();
                left = grammar.Bin(CTokenType.Ampersand, left, right);
            }
            return left;
        }
        //EqlExpr    = RelExpr {("==" | "!=") RelExpr}.
        private CExpression Parse_EqlExpr()
        {
            var left = Parse_RelExpr();
            var token = PeekToken().Type;
            while (token == CTokenType.Eq || token == CTokenType.Ne)
            {
                lexer.Read();
                var right = Parse_RelExpr();
                left = grammar.Bin(token, left, right);
                token = PeekToken().Type;
            }
            return left;
        }
        //RelExpr    = ShiftExpr {('<' | '>' | "<=" | ">=") ShiftExpr}.
        private CExpression Parse_RelExpr()
        {
            var left = Parse_ShiftExpr();
            var token = PeekToken().Type;
            while (token == CTokenType.Le || token == CTokenType.Lt ||
                   token == CTokenType.Ge || token == CTokenType.Gt)
            {
                lexer.Read();
                var right = Parse_ShiftExpr();
                left = grammar.Bin(token, left, right);
                token = PeekToken().Type;
            }
            return left;
        }
        //ShiftExpr  = AddExpr {("<<" | ">>") AddExpr}.
        private CExpression Parse_ShiftExpr()
        {
            var left = Parse_AddExpr();
            var token = PeekToken().Type;
            while (token == CTokenType.Shl || token == CTokenType.Shr)
            {
                lexer.Read();
                var right = Parse_AddExpr();
                left = grammar.Bin(token, left, right);
                token = PeekToken().Type;
            }
            return left;
        }
        //AddExpr    = MultExpr {('+' | '-') MultExpr}.
        private CExpression Parse_AddExpr()
        {
            var left = Parse_MultExpr();
            var token = PeekToken().Type;
            while (token == CTokenType.Plus || token == CTokenType.Minus)
            {
                lexer.Read();
                var right = Parse_MultExpr();
                left = grammar.Bin(token, left, right);
                token = PeekToken().Type;
            }
            return left;
        }
        //MultExpr   = CastExpr {('*' | '/' | '%') CastExpr}.
        private CExpression Parse_MultExpr()
        {
            var left = Parse_CastExpr();
            var token = PeekToken().Type;
            while (token == CTokenType.Star || token == CTokenType.Slash ||
                   token == CTokenType.Percent)
            {
                lexer.Read();
                var right = Parse_CastExpr();
                left = grammar.Bin(token, left, right);
                token = PeekToken().Type;
            }
            return left;
        }
        //CastExpr   = IF(IsType1()) '(' TypeName ')' CastExpr
        //           | UnaryExpr.
        private CExpression Parse_CastExpr()
        {
            if (IsType1())
            {
                ExpectToken(CTokenType.LParen);
                var type = Parse_TypeName();
                ExpectToken(CTokenType.RParen);
                var expr = Parse_CastExpr();
                return grammar.Cast(type, expr);
            }
            else
            {
                return Parse_UnaryExpr();
            }
        }

        //UnaryExpr =
        //  {"++" | "--"}
        //  ( PostfixExpr
        //  | UnaryOp CastExpr
        //  | "sizeof"  (IF(IsType1()) '(' TypeName ')' | UnaryExpr)
        //  ).
        private CExpression Parse_UnaryExpr()
        {
            CExpression expr;
            var token = PeekToken().Type;
            switch (token)
            {
            case CTokenType.Increment:
            case CTokenType.Decrement:
                lexer.Read();
                expr = Parse_UnaryExpr();
                return grammar.PreIncrement(token, expr);
            case CTokenType.Ampersand:
            case CTokenType.Star:
            case CTokenType.Plus:
            case CTokenType.Minus:
            case CTokenType.Tilde:
            case CTokenType.Bang:
                lexer.Read();
                return grammar.Unary(token, Parse_CastExpr());
            case CTokenType.Sizeof:
                lexer.Read();
                if (IsType1())
                {
                    ExpectToken(CTokenType.LParen);
                    CType type = Parse_TypeName();
                    ExpectToken(CTokenType.RParen);
                    return grammar.Sizeof(type);
                }
                else
                {
                    expr = Parse_UnaryExpr();
                    return grammar.Sizeof(expr);
                }
            default:
                return Parse_PostfixExpr();
            }
        }

        //PostfixExpr =
        //  Primary
        //  { '[' Expr ']' 
        //  | '.'  ident
        //  | "->" ident
        //  | '(' [ArgExprList] ')' 
        //  | "++" 
        //  | "--"
        //  }.
        private CExpression Parse_PostfixExpr()
        {
            var left = Parse_Primary();
            string id;
            for (;;)
            {
                var token = PeekToken();
                switch (token.Type)
                {
                case CTokenType.LBracket:
                    lexer.Read();
                    var expr = Parse_Expr();
                    ExpectToken(CTokenType.RBracket);
                    left = grammar.ArrayAccess(left, expr);
                    break;
                case CTokenType.Dot:
                    lexer.Read();
                    id = (string) ExpectToken(CTokenType.Id);
                    left =  grammar.MemberAccess(left, id);
                    break;
                case CTokenType.Arrow:
                    lexer.Read();
                    id = (string) ExpectToken(CTokenType.Id);
                    left = grammar.PtrMemberAccess(left, id);
                    break;
                case CTokenType.LParen:
                    lexer.Read();
                    List<CExpression> args;
                    if (PeekThenDiscard(CTokenType.RParen))
                    {
                        args = new List<CExpression>();
                    }
                    else
                    {
                        args = Parse_ArgExprList();
                        ExpectToken(CTokenType.RParen);
                    }
                    left = grammar.Application(left, args);
                    break;
                case CTokenType.Increment:
                case CTokenType.Decrement:
                    lexer.Read();
                    left = grammar.PostIncrement(left, token.Type);
                    break;
                default:
                    return left;
                }
            }
        }

        //Primary = ident | intcon | floatcon | charcon | string | '(' Expr ')'.

        private CExpression Parse_Primary()
        {
            var token = PeekToken();
            switch (token.Type)
            {
            case CTokenType.Id:
                return grammar.Id((string) lexer.Read().Value);
            case CTokenType.NumericLiteral:
                return grammar.Const(lexer.Read().Value);
            case CTokenType.StringLiteral:
                return grammar.Const(lexer.Read().Value);
            case CTokenType.WideStringLiteral:
                return grammar.Const(lexer.Read().Value);
            case CTokenType.RealLiteral:
                return grammar.Const(lexer.Read().Value);
            case CTokenType.CharLiteral:
                return grammar.Const(lexer.Read().Value);
            case CTokenType.WideCharLiteral:
                return grammar.Const(lexer.Read().Value);
            default:
                ExpectToken(CTokenType.LParen);
                var expr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                return expr;
            }
        }

        //ConstExpr = CondExpr.
        private CExpression Parse_ConstExpr()
        {
            PeekToken();
            var e = Parse_CondExpr();
            return e;
        }

        //ArgExprList = AssignExpr {','  AssignExpr}.
        private List<CExpression> Parse_ArgExprList()
        {
            var list = new List<CExpression>();
            list.Add(Parse_AssignExpr());
            while (PeekThenDiscard(CTokenType.Comma))
            {
                list.Add(Parse_AssignExpr());
            }
            return list;
        }
        //UnaryOp = '&' | '*' | '+' | '-' | '~' | '!'.

        //AssignOp = '=' | "*=" | "/=" | "%=" | "+=" | "-=" | "<<=" | ">>=" | "&=" | "^=" | "|=".


        //---------- Statements ----------

        /// <summary>
        /// Parse a C statement.
        /// </summary>
        /// <returns>The parsed statement.</returns>
        //Stat =
        //      IF(IsLabel()) (ident | "case" ConstExpr | "default") ':' Stat
        //    | Expr ';'
        //    | '{' {IF(IsDecl()) Decl | Stat} '}'
        //    | "if" '(' Expr ')' Stat ["else" Stat]
        //    | "switch" '(' Expr ')' Stat
        //    | "while" '(' Expr ')' Stat
        //    | "do" Stat "while" '(' Expr ')' ';'
        //    | "for" '(' (IF(IsDecl()) Decl | [Expr] ';') [Expr] ';' [Expr] ')' Stat
        //    | "goto" ident ';'
        //    | "continue" ';'
        //    | "break" ';'
        //    | "return" [Expr] ';'
        //    | ';'
        public Stat Parse_Stat()
        {
            if (IsLabel())
            {
                Label label = null;
                switch (PeekToken().Type)
                {
                case CTokenType.Id:
                    label = grammar.Label((string) lexer.Read().Value);
                    break;
                case CTokenType.Case:
                    ExpectToken(CTokenType.Case);
                    CExpression constExpr = Parse_ConstExpr();
                    label = grammar.CaseLabel(constExpr);
                    break;
                case CTokenType.Default:
                    label = grammar.DefaultCaseLabel();
                    break;
                }
                ExpectToken(CTokenType.Colon);
                var stat = Parse_Stat();
                return grammar.LabeledStatement(label, stat);
            }

            CExpression expr;
            var q = PeekToken();
            switch (PeekToken().Type)
            {
            case CTokenType.LBrace:
                ExpectToken(CTokenType.LBrace);
                var stms = new List<Stat>();
                while (!PeekThenDiscard(CTokenType.RBrace))
                {
                    if (IsDecl())
                        stms.Add(grammar.DeclStat(Parse_Decl()));
                    else
                        stms.Add(Parse_Stat());
                }
                return grammar.CompoundStatement(stms);
            case CTokenType.If:
                ExpectToken(CTokenType.If);
                ExpectToken(CTokenType.LParen);
                expr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                Stat consequence = Parse_Stat();
                Stat alternative = null;
                if (PeekThenDiscard(CTokenType.Else))
                {
                    alternative = Parse_Stat();
                }
                return grammar.IfStatement(expr, consequence, alternative);
            case CTokenType.Switch:
                ExpectToken(CTokenType.Switch);
                ExpectToken(CTokenType.LParen);
                expr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                var switchBody = Parse_Stat();
                return grammar.SwitchStatement(expr, switchBody);
            case CTokenType.While:
                ExpectToken(CTokenType.While);
                ExpectToken(CTokenType.LParen);
                expr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                var whileBody = Parse_Stat();
                return grammar.WhileStatement(expr, whileBody);
            case CTokenType.Do:
                ExpectToken(CTokenType.Do);
                var doBody = Parse_Stat();
                ExpectToken(CTokenType.While);
                ExpectToken(CTokenType.LParen);
                expr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                ExpectToken(CTokenType.Semicolon);
                return grammar.DoWhileStatement(doBody, expr);
            case CTokenType.For:
                ExpectToken(CTokenType.For);
                ExpectToken(CTokenType.LParen);
                Stat initStat = null;
                if (IsDecl())
                {
                    initStat = grammar.DeclStat(Parse_Decl());
                }
                else if (PeekToken().Type != CTokenType.Semicolon)
                {
                    initStat = grammar.ExprStatement(Parse_Expr());
                }
                ExpectToken(CTokenType.Semicolon);
                var test = Parse_Expr();
                ExpectToken(CTokenType.Semicolon);
                var incr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                var forBody = Parse_Stat();
                return grammar.ForStatement(initStat, test, incr, forBody);
            case CTokenType.Goto:
                ExpectToken(CTokenType.Goto);
                var gotoLabel = (string) ExpectToken(CTokenType.Id);
                ExpectToken(CTokenType.Semicolon);
                return grammar.GotoStatement(gotoLabel);
            case CTokenType.Continue:
                ExpectToken(CTokenType.Continue);
                ExpectToken(CTokenType.Semicolon);
                return grammar.ContinueStatement();
            case CTokenType.Break:
                ExpectToken(CTokenType.Break);
                ExpectToken(CTokenType.Semicolon);
                return grammar.BreakStatement();
            case CTokenType.Return:
                ExpectToken(CTokenType.Return);
                expr = null;
                if (PeekToken().Type != CTokenType.Semicolon)
                    expr = Parse_Expr();
                ExpectToken(CTokenType.Semicolon);
                return grammar.ReturnStatement(expr);
            case CTokenType.Semicolon:
                ExpectToken(CTokenType.Semicolon);
                return grammar.EmptyStatement();
            case CTokenType.__Asm:
                ExpectToken(CTokenType.__Asm);
                ExpectToken(CTokenType.LBrace);
                while (lexer.Peek(0).Type != CTokenType.RBrace)
                    lexer.Read();
                ExpectToken(CTokenType.RBrace);
                return grammar.EmptyStatement();        // don't care?
            default:
                expr = Parse_Expr();
                ExpectToken(CTokenType.Semicolon);
                return grammar.ExprStatement(expr);
            }
        }
    }
}
