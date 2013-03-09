using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Parsers
{
    public class LookAheadLexer
    {
        private CLexer lexer;
        private List<CToken> queue;
        private int iRead;

        public LookAheadLexer(CLexer lexer)
        {
            this.lexer = lexer;
            this.queue = new List<CToken>();
            iRead= 0;
        }

        public CToken Read()
        {
            if (iRead < queue.Count)
            {
                return queue[iRead++];
            }
            if (queue.Count > 0)
                queue.Clear();
            return lexer.Read();
        }

        public CToken Peek(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException("Can only peek ahead in the stream of tokens.");
            int i = iRead + n;
            while (IsSlotEmpty(i))
            {
                queue.Add(lexer.Read());
            }
            return queue[i];
        }

        private bool IsSlotEmpty(int n)
        {
            return queue.Count <= n;
        }
    }
}
