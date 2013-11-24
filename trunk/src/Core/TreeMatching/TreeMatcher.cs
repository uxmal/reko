using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Core.TreeMatching
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
