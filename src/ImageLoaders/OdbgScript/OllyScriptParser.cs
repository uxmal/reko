using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Reko.ImageLoaders.OdbgScript
{
    class OllyScriptParser
    {
        /*
         script ::= Ø 
                |   line script
                ;
            
        line ::=    label? command? end-comment? (EOL|EOF)

        label ::=   id ':'

        command ::= verb arglist

        verb    ::= id

        arglist ::= Ø
                |  exp
                |  exp ',' arglist

        exp ::=     number
              |      string
              |     interpolated_string
              |     id
              |     exp : exp
              |     [ exp ]
    */
        public class Lexer
        {
            private StreamReader rdr;
            private int linenumber;

            public Lexer(StreamReader rdr)
            {
                this.rdr = rdr;
                this.linenumber = 1;
            }

            public Token Get()
            {
                return default;
            }
        }

        public struct Token
        {
            public TokenType Type;
            public object Value;
        }

        public enum TokenType
        {
            EOF,
            Id,
            String,
            InterpolatedString,
            HexNumber,
            DecimalNumber,
            LBracket,
            RBracket,
            Comma,
            Colon,
            Newline,
        }
    }

}
