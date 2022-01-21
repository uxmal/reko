using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp10.Rewriter
{
    public partial class Pdp10Rewriter
    {
        private void RewriteMove()
        {
            var src = AccessEa();
            var dst = Ac();
            m.Assign(dst, src);
        }

        private void RewriteMovei()
        {
            var src = RewriteEa();
            var dst = Ac();
            m.Assign(dst, src);
        }
    }
}
