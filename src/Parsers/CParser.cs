using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Parsers
{
    // http://www.ssw.uni-linz.ac.at/Coco/C/C.atg

    public class CParser
    {
        /* ANSI C 89 grammar as specified in http://flash-gordon.me.uk/ansi.c.txt

	Processing the C grammar requires LL(1) conflict resolvers, some of which
	need to check whether an identifier is a type name (see IsTypeName() below).
	So the grammar assumes that there is a symbol table, from where you can
	look up an identifier and find out whether it is a type name.
	
	The language in the semantic actions here is C#, but it can easily be
	translated to any other language.
*/

        private CToken tokenCur;
        private CLexer scanner;
        private CLexer lexer;
        private CGrammar grammar;

        public CParser(CLexer lexer)
        {
            this.lexer = lexer;
            this.scanner = lexer;
            this.grammar = new CGrammar();
            this.tokenCur = new CToken(CTokenType.None);
        }

        //------------------ token sets ------------------------------------

        static BitArray startOfTypeName = NewBitArray(
            CTokenType.Const, CTokenType.Volatile, CTokenType.Void, CTokenType.Char,
            CTokenType.Short, CTokenType.Int, CTokenType.Long, CTokenType.Double,
            CTokenType.Float, CTokenType.Signed, CTokenType.Unsigned, CTokenType.Struct,
            CTokenType.Union, CTokenType.Enum);
        static BitArray startOfDecl = NewBitArray(
            CTokenType.Typedef, CTokenType.Extern, CTokenType.Static, CTokenType.Auto,
            CTokenType.Register, CTokenType.Const, CTokenType.Volatile, CTokenType.Void,
            CTokenType.Char, CTokenType.Short, CTokenType.Int, CTokenType.Long,
            CTokenType.Double, CTokenType.Float, CTokenType.Signed, CTokenType.Unsigned,
            CTokenType.Struct, CTokenType.Union, CTokenType.Enum);

        private static BitArray NewBitArray(params CTokenType[] val)
        {
            BitArray s = new BitArray(128);
            foreach (int x in val) s[x] = true;
            return s;
        }

        //---------- LL(1) conflict resolvers ------------------------------

        private System.Collections.Hashtable tab;

        private CToken ReadToken()
        {
            if (tokenCur.Type == CTokenType.None)
            {
                return lexer.Read();
            }
            var t = tokenCur;
            tokenCur = new CToken(CTokenType.None);
            return t;
        }


        private CTokenType PeekToken()
        {
            if (tokenCur.Type == CTokenType.None)
            {
                tokenCur = lexer.Read();
            }
            return tokenCur.Type;
        }

        private bool PeekThenDiscard(CTokenType type)
        {
            if (PeekToken() != type)
                return false;
            ReadToken();
            return true;
        }

        private object ExpectToken(CTokenType token)
        {
            var t = ReadToken();
            if (t.Type != token)
                throw Unexpected(token, t.Type);
            return t.Value;
        }

        private Exception Unexpected(CTokenType expected, CTokenType actual)
        {
            throw new FormatException(string.Format("Expected token '{0}' but saw '{1}'.", expected, actual));
        }

        private bool IsTypeName(CToken x)
        {
            if (x.Type != CTokenType.Id) return false;
            return tab[x.Value] is Type;
        }

        bool IsType0()
        { // return true if the next token is a type name
            PeekToken();
            return IsTypeName(tokenCur);
        }

        bool IsType1()
        { // return true if "(" TypeName
            if (PeekToken() != CTokenType.LParen) return false;
            CToken x = scanner.Peek();
            if (startOfTypeName[(int)x.Type]) return true;
            return IsTypeName(x);
        }

        bool Continued()
        { // return true if not "," "}"
            if (PeekToken() == CTokenType.Comma)
            {
                CToken x = scanner.Peek();
                if (x.Type == CTokenType.RBrace) return false;
            }
            return true;
        }

        bool Continued1()
        { // return true if ",", which is not followed by "..."
            if (tokenCur.Type == CTokenType.Comma)
            {
                CToken x = scanner.Peek();
                if (x.Type != CTokenType.Ellipsis) return true;
            }
            return false;
        }

        bool IsLabel()
        { // return true if ident ":" | "case" | "default"
            if (tokenCur.Type == CTokenType.Id)
            {
                CToken x = scanner.Peek();
                if (x.Type == CTokenType.Colon) return true;
            }
            else if (tokenCur.Type == CTokenType.Case ||
                     tokenCur.Type == CTokenType.Default)
            {
                return true;
            }
            return false;
        }

        bool IsDecl()
        { // return true if followed by Decl
            if (startOfDecl[(int)tokenCur.Type])
                return true;
            return IsTypeName(tokenCur);
        }

        bool IsAbstractDecl()
        { // return true if there is no non-type-ident after '*', '(', "const", "volatile"
            CToken x = tokenCur;
            while (x.Type == CTokenType.Star || x.Type == CTokenType.LParen || x.Type == CTokenType.Const || x.Type == CTokenType.Volatile)
                x = scanner.Peek();
            if (x.Type != CTokenType.Id)
                return true;
            return IsTypeName(x);
        }

        bool IsDeclarator()
        { // return true if '*', '(', '[', ';', noTypeIdent
            if (tokenCur.Type == CTokenType.Star || tokenCur.Type == CTokenType.LParen ||
                tokenCur.Type == CTokenType.LBracket || tokenCur.Type == CTokenType.Semicolon || tokenCur.Type == 0) return true;
            if (tokenCur.Type != CTokenType.Id) return false;
            return !IsTypeName(tokenCur);
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

        //C = 
        //    ExternalDecl {ExternalDecl}.

        public List<Decl> Parse()
        {
            var list = new List<Decl>();
            var decl = Parse_ExternalDecl();
            if (decl == null)
                return list;
            list.Add(decl);
            while (PeekToken() != CTokenType.EOF)
            {
                list.Add(Parse_ExternalDecl());
            }
            return list;
        }

        //ExternalDecl = 
        //  DeclSpecifierList 
        //  ( Declarator 
        //    ( {Decl} '{' {IF(IsDecl()) Decl | Stat} '}'   // FunctionDef
        //    | ['=' Initializer] {',' InitDeclarator}  ';' // Decl
        //    )
        //  | ';'                                           // Decl
        //  ).


        Decl Parse_ExternalDecl()
        {
            var decl_spec_list = Parse_DeclSpecifierList();
            if (decl_spec_list == null)
                return null;
            if (PeekThenDiscard(CTokenType.Semicolon))
                return grammar.Decl();
            var declarator = Parse_Declarator();
            var token = PeekToken();
            if (token == CTokenType.Assign ||
                token == CTokenType.Comma ||
                token == CTokenType.Semicolon)
            {
                Initializer init = null;

                if (PeekThenDiscard(CTokenType.Assign))
                {
                    init = Parse_Initializer();
                }
                while (PeekThenDiscard(CTokenType.Comma))
                {
                    var init_decl = Parse_InitDeclarator();
                }
                ExpectToken(CTokenType.Semicolon);
                return new ExternalDecl
                {
                };
            }
            else
            {
                while (!PeekThenDiscard(CTokenType.LBrace))
                {
                    Parse_Decl();
                }
                ExpectToken(CTokenType.LBrace);
                while (!PeekThenDiscard(CTokenType.RBrace))
                {
                    if (IsDecl())
                    {
                        Parse_Decl();
                    }
                    else
                    {
                        Parse_Stat();
                    }
                }
                return new FunctionDefinition();
            }
        }


        //---------- Declarations ----------

        //Decl = DeclSpecifierList [InitDeclarator {',' InitDeclarator}] ';'.
        public Decl Parse_Decl()
        {
            var list = Parse_DeclSpecifierList();
            if (list == null)
                return null;
            var listDecls = new List<Declarator>();
            if (!PeekThenDiscard(CTokenType.Semicolon))
            {
                listDecls.Add(Parse_InitDeclarator());
                while (PeekThenDiscard(CTokenType.Comma))
                {
                    listDecls.Add(Parse_InitDeclarator());
                }
            }
            ExpectToken(CTokenType.Semicolon);
            return grammar.Decl(list, listDecls);
        }

        //InitDeclarator = Declarator ['=' Initializer].
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

        //DeclSpecifierList = DeclSpecifier {IF(!IsDeclarator()) DeclSpecifier}.

        private List<DeclSpec> Parse_DeclSpecifierList()
        {
            var list = new List<DeclSpec>();
            var ds = Parse_DeclSpecifier();
            if (ds == null)
                return null;
            list.Add(ds);
            while (!IsDeclarator())
            {
                list.Add(Parse_DeclSpecifier());
            }
            return list;
        }

        //DeclSpecifier =
        //        "typedef" | "extern" | "static" | "auto" | "register" // storage class specifier
        //    | "const" | "volatile"                                  // TypeQualifier
        //    | TypeSpecifier.
        private DeclSpec Parse_DeclSpecifier()
        {
            var token = PeekToken();
            switch (token)
            {
            case CTokenType.Typedef:
            case CTokenType.Extern:
            case CTokenType.Static:
            case CTokenType.Auto:
            case CTokenType.Register:
                return grammar.StorageClass( ReadToken().Type);
            case CTokenType.Const:
            case CTokenType.Volatile:
                return grammar.TypeQualifier(ReadToken().Type);
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

        private TypeSpec Parse_TypeSpecifier()
        {
            var token = PeekToken();
            switch (token)
            {
            case CTokenType.Void:
            case CTokenType.Char:
            case CTokenType.Short:
            case CTokenType.Int:
            case CTokenType.Long:
            case CTokenType.Float:
            case CTokenType.Double:
            case CTokenType.Signed:
            case CTokenType.Unsigned:
                return grammar.SimpleType(ReadToken().Type);
            case CTokenType.Id: // type name
                return grammar.TypeName((string) ReadToken().Value);
            case CTokenType.Struct:
            case CTokenType.Union:
                ReadToken();
                string tag = null;
                List<StructDecl> decls = null;
                if (PeekToken() == CTokenType.Id)
                {
                    tag = (string) ReadToken().Value;
                    if (PeekThenDiscard(CTokenType.LBrace))
                    {
                        decls = new List<StructDecl>();
                        do
                        {
                            Parse_StructDecl();
                        } while (PeekToken() != CTokenType.RBrace);
                        ExpectToken(CTokenType.RBrace);
                    }
                }
                else
                {
                    ExpectToken(CTokenType.LBrace);
                    decls = new List<StructDecl>();
                    do
                    {
                        Parse_StructDecl();
                    } while (PeekToken() != CTokenType.RBrace);
                    ExpectToken(CTokenType.RBrace);
                }
                return grammar.ComplexType(token, tag, decls);
            case CTokenType.Enum:
                ReadToken();
                List<Enumerator> enums = null;
                if (PeekToken() == CTokenType.Id)
                {
                    tag = (string) ReadToken().Value;
                    if (PeekThenDiscard(CTokenType.LBrace))
                    {
                        enums = new List<Enumerator>();
                        do
                        {
                            Parse_Enumerator();
                        } while (PeekToken() != CTokenType.RBrace);
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
                        Parse_Enumerator();
                    } while (PeekToken() != CTokenType.RBrace);
                    ExpectToken(CTokenType.RBrace);
                }
                return grammar.Enum(tag, enums);
            default:
                return null;
            }
        }

        //StructDecl = SpecifierQualifierList StructDeclarator {',' StructDeclarator} ';'.

        private StructDecl Parse_StructDecl()
        {
            var sql = Parse_SpecifierQualifierList();
            List<FieldDeclarator> decls = new List<FieldDeclarator>();
            Parse_StructDeclarator();
            while (PeekThenDiscard(CTokenType.Comma))
            {
                Parse_StructDeclarator();
            }
            ExpectToken(CTokenType.Semicolon);
            return grammar.StructDecl(sql, decls);
        }

        //StructDeclarator = Declarator [':' ConstExpr] | ':'  ConstExpr.

        private FieldDeclarator Parse_StructDeclarator()
        {
            Declarator decl = null;
            CExpression bitField = null;
            if (PeekToken() != CTokenType.Colon)
            {
                decl = Parse_Declarator();
            }
            if (PeekThenDiscard(CTokenType.Colon))
            {
                bitField = Parse_ConstExpr();
            }
            return grammar.StructDeclarator(decl, bitField);
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
            do
            {
                DeclSpec t = Parse_TypeSpecifier();
                if (t == null)
                    t = Parse_TypeQualifier();
                sql.Add(t);
            } while (!IsDeclarator());
            return sql;
        }
        //TypeQualifier = "const" | "volatile".

        private TypeQualifier Parse_TypeQualifier()
        {
            var token = PeekToken();
            switch (token)
            {
            case CTokenType.Const:
            case CTokenType.Volatile:
                return grammar.TypeQualifier(token);
            }
            return null;
        }

        //Declarator =
        //    [Pointer]
        //    ( ident
        //    | '(' Declarator ')'
        //    )
        //    { '[' [ConstExpr] ']' 
        //    | '(' [IF(!IsType0()) IdentList | ParamTypeList] ')' 
        //    }.
        public Declarator Parse_Declarator()
        {
            PointerDeclarator ptr = null;
            if (PeekToken() == CTokenType.Star)
            {
                ptr = Parse_Pointer();
            }
            Declarator decl;
            var token = PeekToken();
            switch (token)
            {
            case CTokenType.Id:
                decl = grammar.IdDeclarator((string) ReadToken().Value);
                break;
            case CTokenType.LParen:
                ExpectToken(CTokenType.LParen);
                decl = Parse_Declarator();
                ExpectToken(CTokenType.RParen);
                break;
            default:
                return null;
            }
            for (; ; )
            {
                switch (PeekToken())
                {
                case CTokenType.LBracket:
                    ReadToken();
                    CExpression expr = null;
                    if (PeekToken() != CTokenType.RBracket)
                    {
                        expr = Parse_ConstExpr();
                    }
                    ExpectToken(CTokenType.RBracket);
                    decl = grammar.ArrayDeclarator(decl, expr);
                    break;
                case CTokenType.LParen:
                    if (!IsType0())
                    {
                        Parse_IdentList();
                    }
                    else
                    {
                        Parse_ParamTypeList();
                    }
                    break;
                default: 
                    return decl;
                }
            }
        }

        // Pointer = '*'  {TypeQualifier} {'*'  {TypeQualifier}}.

        PointerDeclarator Parse_Pointer()
        {
            ExpectToken(CTokenType.Star);
            var tq = Parse_TypeQualifier();
            while (tq != null)
            {
                tq = Parse_TypeQualifier();
            }
            while (PeekThenDiscard(CTokenType.Star))
            {
                tq = Parse_TypeQualifier();
                while (tq != null)
                {
                    tq = Parse_TypeQualifier();
                }
            }
            return grammar.PointerDeclarator();
        }

        //ParamTypeList = ParamDecl {IF(Continued1()) ',' ParamDecl} [',' "..."].

        List<ParamDecl> Parse_ParamTypeList()
        {
            var pds = new List<ParamDecl>();
            pds.Add(Parse_ParamDecl());
            while (Continued1())
            {
                ExpectToken(CTokenType.Comma);
                pds.Add(Parse_ParamDecl());
            }
            if (PeekThenDiscard(CTokenType.Comma))
            {
                ExpectToken(CTokenType.Ellipsis);
                pds.Add(grammar.Ellipsis());
            }
            return pds;
        }

        // ParamDecl = DeclSpecifierList [IF(IsAbstractDecl()) AbstractDeclarator | Declarator].

        ParamDecl Parse_ParamDecl()
        {
            Declarator decl = null;
            var dsl = Parse_DeclSpecifierList();
            if (dsl == null)
                return null;
            if (IsAbstractDecl())
            {
                decl = Parse_AbstractDeclarator();
            }
            else
            {
                decl = Parse_Declarator();
            }
            return grammar.ParamDecl(dsl, decl);
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
            switch (PeekToken())
            {
            case CTokenType.Star:
            case CTokenType.LParen:
            case CTokenType.LBracket:
                Parse_AbstractDeclarator();
                break;
            }
            return new CType
            {
            };
        }


        //AbstractDeclarator =
        //    Pointer [DirectAbstractDeclarator]
        //| DirectAbstractDeclarator.

        Declarator Parse_AbstractDeclarator()
        {
            Declarator decl;
            if (PeekToken() == CTokenType.Star)
            {
                var ptr = Parse_Pointer();
                decl = Parse_DirectAbstractDeclarator();
            }
            else
            {
                decl = Parse_DirectAbstractDeclarator();
            }
            return decl;
        }

        //DirectAbstractDeclarator =
        //    ( '(' [AbstractDeclarator | ParamTypeList] ')'
        //    | '[' [ConstExpr] ']'
        //    )
        //    { '[' [ConstExpr] ']' 
        //    | '(' [ParamTypeList] ')'
        //    }
        Declarator Parse_DirectAbstractDeclarator()
        {
            CExpression expr;
            Declarator decl = null;
            switch (PeekToken())
            {
            case CTokenType.LParen:
                ReadToken();
                break;
            case CTokenType.LBracket:
                ReadToken();
                expr = null;
                if (PeekToken() != CTokenType.RBracket)
                {
                    expr = Parse_ConstExpr();
                }
                ExpectToken(CTokenType.RBracket);
                decl = grammar.ArrayDeclarator(decl, expr);
                break;
            default:
                return null;
            }

            for (; ; )
            {
                switch (PeekToken())
                {
                case CTokenType.LParen:
                    ReadToken();
                    break;
                case CTokenType.LBracket:
                    ReadToken();
                    expr = null;
                    if (PeekToken() != CTokenType.RBracket)
                    {
                        expr = Parse_ConstExpr();
                    }
                    ExpectToken(CTokenType.RBracket);
                    decl = grammar.ArrayDeclarator(decl, expr);
                    break;
                default:
                    return null;
                }
            }
        }

        //Initializer = 
        //    AssignExpr 
        //  | '{'  Initializer {IF(Continued()) ',' Initializer} [','] '}'.

        Initializer Parse_Initializer()
        {
            if (PeekThenDiscard(CTokenType.LBrace))
            {
                var list = new List<Initializer>();
                list.Add(Parse_Initializer());
                while (Continued())
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

        //---------- Expressions ----------

        //Expr       = AssignExpr {','  AssignExpr}.
        public CExpression Parse_Expr()
        {
            var left = Parse_AssignExpr();
            while (PeekThenDiscard(CTokenType.Comma))
            {
                var right = Parse_AssignExpr();
                left = grammar.Bin(CTokenType.Comma, left, right);
            }
            return left;
        }

        //AssignExpr = CondExpr [AssignOp AssignExpr]. // relaxed
        public CExpression Parse_AssignExpr()
        {
            var left = Parse_CondExpr();
            var token = PeekToken();
            switch (token)
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
                ReadToken();
                var right = Parse_AssignExpr();
                return grammar.Bin(token, left, right);
            default:
                return left;
            }
        }

        //CondExpr   = LogOrExpr ['?' Expr ':' CondExpr].
        public CExpression Parse_CondExpr()
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
        public CExpression Parse_LogOrExpr()
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
        public CExpression Parse_LogAndExpr()
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
        public CExpression Parse_OrExpr()
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
        public CExpression Parse_XorExpr()
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
        public CExpression Parse_AndExpr()
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
        public CExpression Parse_EqlExpr()
        {
            var left = Parse_RelExpr();
            var token = PeekToken();
            while (token == CTokenType.Eq || token == CTokenType.Ne)
            {
                ReadToken();
                var right = Parse_RelExpr();
                left = grammar.Bin(token, left, right);
                token = PeekToken();
            }
            return left;
        }
        //RelExpr    = ShiftExpr {('<' | '>' | "<=" | ">=") ShiftExpr}.
        public CExpression Parse_RelExpr()
        {
            var left = Parse_ShiftExpr();
            var token = PeekToken();
            while (token == CTokenType.Le || token == CTokenType.Lt ||
                   token == CTokenType.Ge || token == CTokenType.Gt)
            {
                ReadToken();
                var right = Parse_ShiftExpr();
                left = grammar.Bin(token, left, right);
                token = PeekToken();
            }
            return left;
        }
        //ShiftExpr  = AddExpr {("<<" | ">>") AddExpr}.
        public CExpression Parse_ShiftExpr()
        {
            var left = Parse_AddExpr();
            var token = PeekToken();
            while (token == CTokenType.Shl || token == CTokenType.Shr)
            {
                ReadToken();
                var right = Parse_AddExpr();
                left = grammar.Bin(token, left, right);
                token = PeekToken();
            }
            return left;
        }
        //AddExpr    = MultExpr {('+' | '-') MultExpr}.
        public CExpression Parse_AddExpr()
        {
            var left = Parse_MultExpr();
            var token = PeekToken();
            while (token == CTokenType.Plus || token == CTokenType.Minus)
            {
                ReadToken();
                var right = Parse_MultExpr();
                left = grammar.Bin(token, left, right);
                token = PeekToken();
            }
            return left;
        }
        //MultExpr   = CastExpr {('*' | '/' | '%') CastExpr}.
        public CExpression Parse_MultExpr()
        {
            var left = Parse_CastExpr();
            var token = PeekToken();
            while (token == CTokenType.Star || token == CTokenType.Slash ||
                   token == CTokenType.Percent)
            {
                ReadToken();
                var right = Parse_CastExpr();
                left = grammar.Bin(token, left, right);
                token = PeekToken();
            }
            return left;
        }
        //CastExpr   = IF(IsType1()) '(' TypeName ')' CastExpr
        //           | UnaryExpr.
        public CExpression Parse_CastExpr()
        {
            while (IsType1())
            {
                ExpectToken(CTokenType.LParen);
                var type = Parse_TypeName();
                ExpectToken(CTokenType.RParen);
            }
            return Parse_UnaryExpr();
        }
        //UnaryExpr =
        //  {"++" | "--"}
        //  ( PostfixExpr
        //  | UnaryOp CastExpr
        //  | "sizeof"  (IF(IsType1()) '(' TypeName ')' | UnaryExpr)
        //  ).
        public CExpression Parse_UnaryExpr()
        {
            CExpression expr;
            var token = PeekToken();
            switch (token)
            {
            case CTokenType.Increment:
            case CTokenType.Decrement:
                ReadToken();
                expr = Parse_UnaryExpr();
                return grammar.PreIncrement(token, expr);
            case CTokenType.Ampersand:
            case CTokenType.Star:
            case CTokenType.Plus:
            case CTokenType.Minus:
            case CTokenType.Tilde:
            case CTokenType.Bang:
                ReadToken();
                return grammar.Unary(token, Parse_CastExpr());
            case CTokenType.Sizeof:
                ReadToken();
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
        public CExpression Parse_PostfixExpr()
        {
            var left = Parse_Primary();
            string id = null;
            var token = PeekToken();
            switch (token)
            {
            case CTokenType.LBracket:
                ReadToken();
                var expr = Parse_Expr();
                ExpectToken(CTokenType.RBracket);
                return grammar.ArrayAccess(left, expr);
            case CTokenType.Dot:
                ReadToken();
                id = (string) ExpectToken(CTokenType.Id);
                return grammar.MemberAccess(left, id);
            case CTokenType.Arrow:
                ReadToken();
                id = (string) ExpectToken(CTokenType.Id);
                return grammar.PtrMemberAccess(left, id);
            case CTokenType.LParen:
                ReadToken();
                List<CExpression> args = null;
                if (PeekThenDiscard(CTokenType.LParen))
                {
                    args = Parse_ArgExprList();
                }
                else
                {
                    args = new List<CExpression>();
                }
                ExpectToken(CTokenType.RParen);
                return grammar.Application(left, args);
            case CTokenType.Increment:
            case CTokenType.Decrement:
                ReadToken();
                return grammar.PostIncrement(left, token);
            default:
                return left;
            }
        }

        //Primary = ident | intcon | floatcon | charcon | string | '(' Expr ')'.

        public CExpression Parse_Primary()
        {
            var token = PeekToken();
            switch (token)
            {
            case CTokenType.Id:
                return grammar.Id((string) ReadToken().Value);
            case CTokenType.NumericLiteral:
                return grammar.Const(ReadToken().Value);
            case CTokenType.StringLiteral:
                return grammar.Const(ReadToken().Value);
            case CTokenType.RealLiteral:
                return grammar.Const(ReadToken().Value);
            case CTokenType.CharLiteral:
                return grammar.Const(ReadToken().Value);
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
            Debug.Print("Parse_ConstExpr: peek {0}: {1}", tokenCur.Type, tokenCur.Value); 
            var e = Parse_CondExpr();
            Debug.Print("Parse_ConstExpr: e: {0}", e);
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
        Stat Parse_Stat()
        {
            if (IsLabel())
            {
                Label label = null;
                CExpression constExpr = null;
                switch (PeekToken())
                {
                case CTokenType.Id:
                    label = grammar.Label((string) ReadToken().Value);
                    break;
                case CTokenType.Case:
                    ExpectToken(CTokenType.Case);
                    constExpr = Parse_ConstExpr();
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
            switch (PeekToken())
            {
            case CTokenType.LBrace:
                ExpectToken(CTokenType.LBrace);
                while (!PeekThenDiscard(CTokenType.RBrace))
                {
                    if (IsDecl())
                        Parse_Decl();
                    else
                        Parse_Stat();
                }
                return null;
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
                Stat initStat;
                if (IsDecl())
                {
                    initStat = grammar.DeclStat(Parse_Decl());
                }
                else if (PeekToken() != CTokenType.Semicolon)
                {
                    initStat = grammar.ExprStatement(Parse_Expr());
                }
                ExpectToken(CTokenType.Semicolon);
                var test = Parse_Expr();
                ExpectToken(CTokenType.Semicolon);
                var incr = Parse_Expr();
                ExpectToken(CTokenType.RParen);
                var forBody = Parse_Stat();
                return grammar.ForStatement(null, test, incr, forBody);
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
                if (PeekToken() != CTokenType.Semicolon)
                    expr = Parse_Expr();
                ExpectToken(CTokenType.Semicolon);
                return grammar.ReturnStatement(expr);
            case CTokenType.Semicolon:
                ExpectToken(CTokenType.Semicolon);
                return grammar.EmptyStatement();
            default:
                expr = Parse_Expr();
                ExpectToken(CTokenType.Semicolon);
                return grammar.ExprStatement(expr);
            }

        }
    }
}
