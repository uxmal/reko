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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Arch.Pdp10.Rewriter
{
    public partial class Pdp10Rewriter
    {
        private void RewriteBlki()
        {
            m.SideEffect(m.Fn(blkiIntrinsic, Imm(0), AccessEa(1)));
        }

        private void RewriteBlko()
        {
            m.SideEffect(m.Fn(blkoIntrinsic, Imm(0), AccessEa(1)));
        }

        private void RewriteBlt()
        {
            m.SideEffect(m.Fn(bltIntrinsic, Ac(), AccessEa(1)));
        }

        private void RewriteCalli()
        {
            m.SideEffect(m.Fn(calliIntrinsic, Ac(), AccessEa(1)));
        }

        private void RewriteConi()
        {
            m.SideEffect(m.Fn(coniIntrinsic, Imm(0), AccessEa(1)));
        }

        private void RewriteDatai()
        {
            m.Assign(AccessEa(1), m.Fn(dataiIntrinsic, Imm(0)));
        }

        private void RewriteDatao()
        {
            m.SideEffect(m.Fn(dataoIntrinsic, Imm(0), AccessEa(1)));
        }

        private void RewriteDpb()
        {
            m.SideEffect(m.Fn(dpbIntrinsic, Ac(), RewriteEa(1)));
        }

        private void RewriteHalt()
        {
            m.SideEffect(m.Fn(haltIntrinsic, AccessEa(0)));
        }

        private void RewriteIbp()
        {
            var tmp = binder.CreateTemporary(ibpIntrinsic.ReturnType);
            m.Assign(tmp, m.Fn(ibpIntrinsic, AccessEa(1)));
            m.Assign(AccessEa(1), tmp);
        }

        private void RewriteIdpb()
        {
            m.Assign(AccessEa(1), m.Fn(idpbIntrinsic, AccessEa(1), Ac()));
        }


        private void RewriteIldb()
        {
            m.Assign(Ac(), m.Fn(ildbIntrinsic, RewriteEa(1)));
        }

        private void RewriteIniti()
        {
            m.SideEffect(m.Fn(initiIntrinsic, Ac(), AccessEa(1)));
        }

        private void RewriteLdb()
        {
            m.Assign(Ac(), m.Fn(ldbIntrinsic, RewriteEa(1)));
        }

        private void RewriteLookup()
        {
            m.SideEffect(m.Fn(lookupIntrinsic, Ac(), AccessEa(1)));
        }

        private void RewriteLuuo(int n)
        {
            m.SideEffect(m.Fn(luuoIntrinsic, Constant.Create(word36, n), Ac(), AccessEa(1)));
        }

        private void RewriteRename()
        {
            m.SideEffect(m.Fn(renameIntrinsic, Ac(), AccessEa(1)));
        }

        private void RewriteTtcall()
        {
            m.SideEffect(m.Fn(ttcallIntrinsic, Ac(), AccessEa(1)));
        }

        private void RewriteXct()
        {
            m.SideEffect(m.Fn(xctIntrinsic, Ac(), RewriteEa(1)));
        }
    }
}
