using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UiPrototype.WinForms
{
    public class StringSearcher : IEnumerable<SearchHit>
    {
        private string txt;

        public StringSearcher(string txt)
        {
            this.txt = txt;
        }

        public IEnumerator<SearchHit> GetEnumerator()
        {
            for (int i = 0x00410000; i < 0x00410200; i += 0x4C)
            {
                yield return new SearchHit
                {
                    AddressText = i.ToString("X8"),
                    Name = "",
                    Description = txt
                };
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
