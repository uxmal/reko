using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Scanning
{
    class ScannerInLinq
    {
        // change @binary_size to simulate a large executable
        private const int binary_size = 10;
        private Dictionary<long, instr> the_instrs;
        private List<link> the_links;

        private class instr
        {
            public long addr; // primary key not null,
            public int size;
            public char type;
            public long block_id;
            public int pred;
            public int succ;
        }

        private class link
        {
            public long first;
            public long second;

            public override bool Equals(object obj)
            {
                var that = obj as link;
                if (that == null)
                    return false;
                return that.first == this.first && that.second == this.second;
            }

            public override int GetHashCode()
            {
                return first.GetHashCode() ^ 13 * second.GetHashCode();
            }
        }

        private class block
        {
            public long id;
            public long component_id;
        }

        private class new_block
        {
            public long addr;
            public long addrFrom;
            public long block_id;
        }

        public void Doit()
        {
            the_instrs = new Dictionary<long, instr>();
            // links betweeen instructions
            the_links = new List<link>();
            // basic blocks
            var the_blocks = new Dictionary<long, block>();

            var the_excluded_edges = new List<link>();

            // The following are work tables

            // newly discovered bad instructions
            var new_bad = new HashSet<long>();
            // newly discovered instrctions that need to be added to basic blocks.
            var new_blocks = new List<new_block>();
            // componenets that need to be merged
            var components_to_merge = new List<link>();

        // Create a simulated program ----------------------------------------------------

        long @offset = 40000;
        long @end = @offset + @binary_size;
            while (@offset < @end)
            {
                AddInstr(1 + @offset, 1, 'l'); ;
                AddLink(1 + @offset, 2 + @offset);

                AddInstr(2 + @offset, 1, 'l'); ;
                AddLink(2 + @offset, 4 + @offset);

                AddInstr(3 + @offset, 1, 'C'); ; // Capital 'C' means this was called by someone.
                AddLink(3 + @offset, 4 + @offset);

                AddInstr(4 + @offset, 1, 'c');
                AddLink(4 + @offset, 5 + @offset);
                AddLink(4 + @offset, 6 + @offset);

                AddInstr(5 + @offset, 1, 'l');
                AddLink(5 + @offset, 6 + @offset);

                AddInstr(6 + @offset, 1, 'x');
                AddLink(6 + @offset, 2 + @offset);
                AddLink(6 + @offset, 7 + @offset);

                AddInstr(7 + @offset, 1, 'l');
                AddLink(7 + @offset, 8 + @offset);

                AddInstr(8 + @offset, 1, 'l');
                AddLink(8 + @offset, 9 + @offset);

                AddInstr(9 + @offset, 1, 'l');
                AddLink(9 + @offset, 10 + @offset);

                AddInstr(10 + @offset, 1, 'l');

                @offset = @offset + 20;
    
        }

        // Find transitive closure of bad instructions ------------------------

        for (; ;)
            {

                new_bad = 
            insert into new_bad
            select pred.addr
            from the_instrs item
            inner join the_links on item.addr = the_links.second
            inner join the_instrs pred on the_links.first = pred.addr
            where item.type = 'x' and pred.type <> 'x'
            insert into new_bad
            select succ.addr
            from the_instrs item
            inner join the_links on item.addr = the_links.first
            inner join the_instrs succ on the_links.second = succ.addr
            where item.type = 'x' and succ.type <> 'x'

        if (select count(*)from new_bad) = 0 break

        update a
        set type = 'x'
        from the_instrs as a
        join new_bad on a.addr = new_bad.addr

        end

        // Compute all basic blocks -----------------------------------------------

        // count the # of predecessors and successors for each instr
        update instr
        set 
            pred = coalesce(target.incoming, 0),
            succ = coalesce(target2.leaving, 0)
        from the_instrs instr
        left join 
            (select second as addr, count(first) incoming
            from the_links
            group by second) as target
            on instr.addr = target.addr
        left join 
            (select first as addr, count(second) leaving
            from the_links
            group by first) 
            as target2
            on instr.addr = target2.addr

        update the_instrs 
        set  block_id = addr

        while 1=1 begin
            // find all instructions succ that have a single predecessor pred
            // such that pred has succ as its single successor and they 
            // are consecutive in memory.

            delete from new_blocks
            insert into new_blocks
            select succ.addr, pred.addr, pred.block_id
            from the_links 
            join the_instrs pred on the_links.first = pred.addr
            join the_instrs succ on the_links.second = succ.addr
            where pred.succ = 1 and succ.pred = 1 and
                  pred.addr + pred.size = succ.addr and
                  pred.block_id <> succ.block_id

            if (select count (*) from new_blocks) = 0
                break

            update instr
            set
                block_id = b.block_id
            from the_instrs instr
            join new_blocks b on instr.addr = b.addr 


            insert into #excluded_edges
            select b.addrFrom, b.addr
            from new_blocks b
            left join #excluded_edges ex on b.addrFrom = ex.first and b.addr = ex.second
            where ex.first is null
        end

        // Build global block graph
        delete from the_blocks
        insert into the_blocks
        select distinct block_id, block_id from the_instrs

        delete link from the_links link
        inner join #excluded_edges ex on link.first = ex.first and link.second = ex.second

        // Compute all weakly connected components -------------------------------------------------

        // initialize components
        update the_blocks
        set component_id = id

        while 1=1 begin

        // Find all links that connect instructions that have different components
            delete from #components_to_merge
            insert into #components_to_merge
            select distinct t1.component_id c1, t2.component_id c2
            from the_links 
            join the_blocks t1 on the_links.first = t1.id 
            join the_blocks t2 on the_links.second = t2.id
            where t1.component_id != t2.component_id
            // ensure symmetricity (only for WCC, SCC should remove this)
            insert into #components_to_merge
            select component2, component1 from #components_to_merge; 

            if (select count(*) from #components_to_merge) = 0 break

        update block
        set block.component_id = case when block.component_id < target then block.component_id else target end 
        from the_blocks block
        inner join 
            (select
                b.component1 as source, 
                min(b.component2) as [target]
            from #components_to_merge b
            group by b.component1) as new_components 
                on block.component_id = new_components.source
        end

        select top 300 'I' as Instrs, * from the_instrs
        select top 300 'L' as Links, * from the_links
        select top 300 'B' as Blocks, * from the_blocks
         */
    }

        private void AddInstr(long addr, int size, char type)
        {
            the_instrs.Add(addr, new instr
            {
                addr = addr,
                size = size,
                type = type
            });
        }

        private void AddLink(long from, long to)
        {
            the_links.Add(new link { first = from, second = to });
        }
    }
