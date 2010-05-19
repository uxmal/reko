using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core.Output
{
    public class RtfCodeFormatter : CodeFormatter
    {
        private const string Tab = "\tab";
        
        public RtfCodeFormatter(TextWriter writer) : base(new InnerFormatter(writer))
        {
        }

        private class InnerFormatter : Formatter
        {
            public InnerFormatter(TextWriter writer) : base(writer)
            {
            }

            public override void WriteKeyword(string keyword)
            {
                base.WriteKeyword(keyword);
            }

            public override void WriteLine()
            {
                base.WriteLine();
            }
        }
    }

#if SAMPLE
{\rtf1\ansi\ansicpg1252\deff0\deflang1053{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}{\f1\fnil Microsoft Sans Serif;}}
{\colortbl ;\red0\green0\blue255;\red0\green0\blue0;}
\viewkind4\uc1\pard\f0\fs17 word32 fn00401000(word32 dwArg00)\par
\{\par
fn00401000_entry:\par
l00401000:\par
\tab\cf1 if\cf2  (dwArg00 >= 0x00000001)\par
\cf0\tab\tab\cf1 goto\cf2  l00401011\par
\cf0 l00401011:\par
\tab\cf1 return\cf2  \cf1\ul\f1 fn00401000\cf2\ulnone\f0 (dwArg00 - 0x00000001) + \cf1\ul\f1 fn00401000\cf2\ulnone\f0 (dwArg00 - 0x00000002)\par
\cf0 l0040100A:\par
\tab\cf1 return\cf2  0x00000001\par
\cf0 fn00401000_exit:\par
\}\par
\par
}


#endif
}
