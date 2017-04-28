using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Lib
{

    //        https://github.com/bjornharrtell/pggraph/blob/master/dijkstra.sql
    // calculates shortest path from startnode to endnode
    // returns destination with total cost and concatenated id's describing the path (array would be better I think)

    class LinqAlgorithms
    {
        public class MinPath
        {
            public int id;
            public int distance;
            public string path;
        }

class nodeestimate_t
    (
        id int PRIMARY KEY,      // The Node Id
        estimate int NOT NULL,   // What is the distance to this node, so far?
        predecessor int NULL,    // The node we came from to get to this node with this distance.
        done boolean NOT NULL    // Are we done with this node yet (is the estimate the final distance)?
    ) 

List<MinPath> dijkstra(startnode int, endnode int)
{ 
            /*
    rowcount int;
    currentfromnode int;
    currentestimate int;
    // Create a temporary table for storing the estimates as the algorithm runs
    var nodeestimage = new Dictionary<int, nodeestimate_t>
    CREATE TEMP TABLE xxx ON COMMIT DROP;

    // Fill the temporary table with initial data
    INSERT INTO nodeestimate(id, estimate, predecessor, done)
        SELECT node.id, 999999999, NULL, FALSE FROM node;
    
    // Set the estimate for the node we start in to be 0.
    UPDATE nodeestimate SET estimate = 0 WHERE nodeestimate.id = startnode;
        GET DIAGNOSTICS rowcount = ROW_COUNT;
    IF rowcount<> 1 THEN
        DROP TABLE nodeestimate;
        RAISE 'Could not set start node';
        RETURN;
    END IF;

    // Run the algorithm until we decide that we are finished
    LOOP
        // Reset the variable, so we can detect getting no records in the next step.
        currentfromnode := NULL;
    
        // Select the Id and current estimate for a node not done, with the lowest estimate.
        SELECT nodeestimate.id, estimate INTO currentfromnode, currentestimate
            FROM nodeestimate WHERE done = FALSE AND estimate < 999999999

            ORDER BY estimate LIMIT 1;

        // Stop if we have no more unvisited, reachable nodes.
        IF currentfromnode IS NULL OR currentfromnode = endnode THEN EXIT; END IF;

        // We are now done with this node.
        UPDATE nodeestimate SET done = TRUE WHERE nodeestimate.id = currentfromnode;

        // Update the estimates to all neighbour node of this one (all the nodes
        // there are edges to from this node). Only update the estimate if the new
        // proposal(to go via the current node) is better(lower).
        UPDATE nodeestimate n
            SET estimate = currentestimate + weight, predecessor = currentfromnode
            FROM edge AS e
            WHERE n.id = e.tonode AND n.done = FALSE AND e.fromnode = currentfromnode AND(currentestimate + e.weight) < n.estimate;

    END LOOP;

    // Select the results.We use a recursive common table expression to
    // get the full path from the start node to the current node.
    RETURN QUERY WITH RECURSIVE BacktraceCTE(id, distance, path)
    AS(
        // Anchor/base member of the recursion, this selects the start node.
        SELECT n.id, n.estimate, n.id::text
        FROM nodeestimate n JOIN node ON n.id = node.id

        WHERE n.id = startnode


        UNION ALL
		
        // Recursive member, select all the nodes which have the previous
        // one as their predecessor. Concat the paths together.
        SELECT n.id, n.estimate, cte.path || ',' || n.id::text
            FROM nodeestimate n JOIN BacktraceCTE cte ON n.predecessor = cte.id
            JOIN node ON n.id = node.id
    ) SELECT cte.id, cte.distance, cte.path FROM BacktraceCTE cte
        WHERE cte.id = endnode OR endnode IS NULL // This kind of where clause can potentially produce
        ORDER BY cte.id;                          // a bad execution plan, but I use it for simplicity here.
        */
}

        // http://www.drmaciver.com/2008/11/computing-connected-graph-components-via-sql/
        void scc()
        {
            create table items(
                    id int primary key,
                    component_id int
            );
            create table links(
                first int references items(id),
                second int references items(id)
            );

            for (;;)
            { 
            create table if not exists components_to_merge(
                        component1 int,
                        component2 int);

            insert into components_to_merge
            select distinct t1.component_id c1, t2.component_id c2 
            from links
            join items t1 on links.first = t1.id join items t2 on links.second = t2.id where t1.component_id != t2.component_id
            insert into components_to_merge
            select component2, component1 
            from components_to_merge; //ensure symmetricity
            if (components_to_merge.Count == 0)
                break;
            update items 
                join(select component1 source, min(component2) target
                from components_to_merge group by source) new_components on new_components.source = component_id 
            set
                items.component_id = least(items.component_id, target);
        }
    }
}
