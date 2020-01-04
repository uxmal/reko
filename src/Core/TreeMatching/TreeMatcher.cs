#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.TreeMatching
{
    //class TreeMatcher
    //{
    //    public void Build()
    //    {
    //        var T = set of tree_replacement_rule { l, tree_template };
    //        foreach (tree_replacement_rule rule in T)
    //        {
    //            foreach (path_string str in rule.generate_path_strings())
    //            {
    //                add_to_automaton(str);
    //            }
    //        }
    //    }

    //    public void visit(tree_node n)
    //    {
    //        if (is_root(n))
    //        {
    //            n.state = succ(0, n.symbol);        // 0 start stte of automaton.
    //        }
    //        else
    //            n.state = succ(succ(n.parent.State, k), n.symbol);
    //        foreach (var child in n.Children)
    //        {
    //            visit(n);
    //        }
    //        post_process(n);
    //    }
    //}
    #if noz
    let K be keywords
    make root node
    foreach k in K
        make path from root to K
    
    add transition from state s to t on c 
    failure:

#endif

    public interface TreeMatchAdapter
    {

    }
}
