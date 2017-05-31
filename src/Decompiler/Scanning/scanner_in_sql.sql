-- change @binary_size to simulate a large executable
declare @binary_size bigint = 10
declare @offset bigint = 40000

drop table #excluded_edges
drop table #new_blocks
drop table #new_bad
drop table #blocks
drop table #links
drop table #instrs
drop table #components_to_merge

create table #instrs(
    addr bigint primary key not null,
	[size] int not null,
	type varchar(1),
	block_id bigint,
	pred int default 0,
	succ int default 0
)
	
-- links betweeen instructions
create table #links(
    first bigint,
	second bigint
)

-- basic blocks
create table #blocks(
	id bigint primary key,
	component_id bigint default 0
)	
create table #excluded_edges(
	first bigint,
	second bigint,
	PRIMARY KEY (first,second)
)

-- The following are work tables

-- newly discovered bad instructions
create table #new_bad(
	addr bigint
)

-- newly discovered instrctions that need to be added to basic blocks.
create table #new_blocks(
	addr bigint,
	addrFrom bigint,
	block_id bigint
)

-- componenets that need to be merged
create table  #components_to_merge(
	component1 bigint, 
	component2 bigint);

-- Create a simulated program ----------------------------------------------------

declare @end bigint = @offset + @binary_size
while @offset < @end
begin
	
	insert into #instrs(addr, [size], [type]) VALUES (1 + @offset, 1, 'l')
	insert into #links values (1 + @offset,  2 + @offset)

	INSERT INTo #instrs(addr, size,  type) VALUES (2 + @offset, 1, 'l')
	insert into #links values (2 + @offset,  4 + @offset)

	INSERT INTo #instrs(addr, size,  type) VALUES (3 + @offset, 1, 'C') -- Capital 'C' means this was called by someone.
	insert into #links values (3 + @offset,  4 + @offset)

	INSERT INTo #instrs(addr, size,  type) VALUES (4 + @offset, 1, 'c')
	insert into #links values (4 + @offset,  5 + @offset)
	insert into #links values (4 + @offset,  6 + @offset)

	INSERT INTo #instrs(addr, size,  type) VALUES (5 + @offset, 1, 'l')
	insert into #links values (5 + @offset,  6 + @offset)

	INSERT INTo #instrs(addr, size,  type) VALUES (6 + @offset,  1, 'x')
	insert into #links values (6 + @offset,  2 + @offset)
	insert into #links values (6 + @offset,  7 + @offset)

	INSERT INTo #instrs(addr, size,  type) VALUES (7 + @offset, 1, 'l')
	insert into #links values (7 + @offset,  8 + @offset)

	INSERT INTo #instrs(addr, size,  type) VALUES (8 + @offset, 1, 'l')
	insert into #links values (8 + @offset,  9 + @offset)
	
	INSERT INTo #instrs(addr, size,  type) VALUES (9 + @offset,  1, 'l')
	insert into #links values (9 + @offset, 10 + @offset)

	INSERT INTo #instrs(addr, size,  type) VALUES (10 + @offset,  1, 'l')

	set @offset = @offset + 20

end

-- Find transitive closure of bad instructions ------------------------

while 1=1 begin
	
	delete from #new_bad
	insert into #new_bad
	select pred.addr
	from #instrs item
	inner join #links on item.addr = #links.second
	inner join #instrs pred on #links.first = pred.addr
	where item.type = 'x' and pred.type <> 'x'
	insert into #new_bad
	select succ.addr
	from #instrs item
	inner join #links on item.addr = #links.first
	inner join #instrs succ on #links.second = succ.addr
	where item.type = 'x' and succ.type <> 'x'

if (select count(*)from #new_bad) = 0 break

update a
set type = 'x'
from #instrs as a
join #new_bad on a.addr = #new_bad.addr

end

-- Compute all basic blocks -----------------------------------------------

-- count the # of predecessors and successors for each instr
update instr
set 
    pred = coalesce(target.incoming, 0),
    succ = coalesce(target2.leaving, 0)
from #instrs instr
left join 
	(select second as addr, count(first) incoming
	from #links
	group by second) as target
	on instr.addr = target.addr
left join 
	(select first as addr, count(second) leaving
	from #links
	group by first) 
	as target2
	on instr.addr = target2.addr

update #instrs 
set  block_id = addr

while 1=1 begin
	-- find all instructions succ that have a single predecessor pred
	-- such that pred has succ as its single successor and they 
	-- are consecutive in memory.
	
	delete from #new_blocks
    insert into #new_blocks
	select succ.addr, pred.addr, pred.block_id
	from #links 
	join #instrs pred on #links.first = pred.addr
	join #instrs succ on #links.second = succ.addr
	where pred.succ = 1 and succ.pred = 1 and
		  pred.addr + pred.size = succ.addr and
		  pred.block_id <> succ.block_id
				
	if (select count (*) from #new_blocks) = 0
		break
	
	update instr
	set
		block_id = b.block_id
	from #instrs instr
	join #new_blocks b on instr.addr = b.addr 

	
	insert into #excluded_edges
	select b.addrFrom, b.addr
	from #new_blocks b
	left join #excluded_edges ex on b.addrFrom = ex.first and b.addr = ex.second
	where ex.first is null
end

-- Build global block graph
delete from #blocks
insert into #blocks
select distinct block_id, block_id from #instrs

delete link from #links link
inner join #excluded_edges ex on link.first = ex.first and link.second = ex.second

-- Compute all weakly connected components -------------------------------------------------

-- initialize components
update #blocks
set component_id = id

while 1=1 begin

-- Find all links that connect instructions that have different components
	delete from #components_to_merge
	insert into #components_to_merge
	select distinct t1.component_id c1, t2.component_id c2
	from #links 
	join #blocks t1 on #links.first = t1.id 
	join #blocks t2 on #links.second = t2.id
	where t1.component_id != t2.component_id
	-- ensure symmetricity (only for WCC, SCC should remove this)
	insert into #components_to_merge
	select component2, component1 from #components_to_merge; 

	if (select count(*) from #components_to_merge) = 0 break

	update block
	set block.component_id = case when block.component_id < target
		 then block.component_id 
		 else target
		 end 
	from #blocks block
	inner join 
		(select
			b.component1 as source, 
			min(b.component2) as [target]
		from #components_to_merge b
		group by b.component1) as new_components 
			on block.component_id = new_components.source
end

select top 300 'I' as Instrs, * from #instrs
select top 300 'L' as Links, * from #links
select top 300 'B' as Blocks, * from #blocks
