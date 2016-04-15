using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;

public static class BapScanner
{
#if NYI
open Core_kernel.Std
open Bap_types.Std
open Bil.Types
open Or_error
open Image_internal_std
module Dis = Bap_disasm_basic

module Addrs = Addr.Table

type insn = Dis.full_insn with sexp_of
type lifter = mem -> insn -> bil Or_error.t

/** Interval tree spanning visited memory */
module type Span = sig
  type range = addr * addr
  type t with sexp_of

  val empty : t
  val add : t -> range -> t
  val mem : t -> addr -> bool
  val upper_bound : t -> addr -> addr option
  val min : t -> addr option
  val max : t -> addr option
  val pp : Format.formatter -> t -> unit
end

class Span {
  module Range = struct
    module T = struct
      type t = addr * addr with sexp
      let compare (a1,_) (a2,_) = Addr.compare a1 a2
    end
    type t = T.t with sexp
    include Comparable.Make(T)
  end

  type range = Range.t

  let sexp_of_range (a0,a1) =
    Sexp.List [
      Sexp.Atom (Addr.string_of_value a0);
      Sexp.Atom (Addr.string_of_value a1);
    ]

  type t =
    | Empty
    | Node of t * range * addr * t
  with sexp_of

  let rec pp fmt = function
    | Empty -> ()
    | Node (lhs,(x,y),_,rhs) ->
      Format.fprintf fmt "@[%a(%a,%a)@,%a@]"
        pp lhs Addr.pp x Addr.pp y pp rhs

  let empty = Empty

  let rng_max (_,a1) a2 = Addr.max a1 a2

  /** @pre: x <= p */
  let intersects (x,y) (p,q) = p <= y
  /** [merge x y] if [intersects x y]  */
  let merge (x,p) (_,q) : range = (x, Addr.max p q)

  let rec add t ((_,a1) as rng) = match t with
    | Empty -> Node (Empty,rng,a1,Empty)
    | Node (lhs,top,m,rhs) ->
      let m = rng_max rng m in
      if Range.(rng < top) then
        if intersects rng top
        then Node (lhs, merge rng top, m, rhs)
        else Node (add lhs rng, top, m, rhs)
      else if intersects top rng
      then Node (lhs, merge top rng, m, rhs)
      else Node (lhs, top, m, add rhs rng)

  let rec mem t addr = match t with
    | Empty -> false
    | Node (lhs,(a0,a1),mx,rhs) ->
      if Addr.(addr > mx) then false
      else if Addr.(addr < a0) then mem lhs addr
      else if Addr.(addr >= a1) then mem rhs addr
      else true

  /** [upper_bound visited addr] returns minimum address that is
      greater than [addr] and is visited. Returns [None] if no such
      address was found */
  /* returns the lowest left bound greater than [addr] */
  let rec upper_bound t a = match t with
    | Empty -> None
    | Node (_,_,mx,_) when Addr.(a > mx) -> None
    | Node (lhs,(a0,_),_,rhs) ->
      if Addr.(a > a0) then upper_bound rhs a
      else match upper_bound lhs a with
        | None -> Some a0
        | some -> some

  let rec min = function
    | Empty -> None
    | Node (Empty,(a0,_),_,_) -> Some a0
    | Node (lhs,_,_,_) -> min lhs

  let max = function
    | Empty -> None
    | Node (_,_,m,_) -> Some m
}

type dis = (Dis.empty, Dis.empty) Dis.t
#endif

enum dest_kind {
  Fall, /** a fallthrough to the next instruction  */
  Cond, /** conditional jump on a true case  */
  Jump, /** unconditional jump */
}

//type dst = [
//  | `Jump of addr option
//  | `Cond of addr option
//  | `Fall of addr
//] with sexp
public class dst {
    public Address addr;
    public object option;
    }

//type dest = addr option * dest_kind with sexp
//type dests = dest list with sexp
public class dest {
    public Address addr;
    public dest_kind dest_kind;

}

//type error = [
//  | `Failed_to_disasm of mem
//  | `Failed_to_lift of mem * insn * Error.t
//] with sexp_of

//type maybe_insn = insn option * bil option with sexp_of
//type decoded = mem * maybe_insn with sexp_of
public class maybe_insn
{
    public MachineInstruction insn;
    public RtlInstruction bil;
}

public class decoded 
{
    public mem mem;
    public maybe_insn maybe_insn;
}

//type jump = [
//  | `Jump     /** unconditional jump                  */
//  | `Cond     /** conditional jump                    */
//] with compare, sexp

//type edge = [jump | `Fall] with compare,sexp


public class block  {
  public Address addr;
  public Lazy<mem> mem;
  public List<block> blk_succs;
  public List<block> blk_preds;
  public List<blk_dest> blk_dests;
  public Lazy<List<decoded>> insns;
  public Lazy<decoded> term;
  public Lazy<decoded> lead;
}

//and blk_dest = [
//  | `Block of block * edge
//  | `Unresolved of jump
//]
public abstract class blk_dest {
    public class Block_dest : blk_dest{
        block block;
        edge edge;
    }
    public class Unresolved_dest : blk_dest
    {
        public jump jump;
    }
}

class stage1  {
  public mem @base;
  public Address addr;
  pubic Span visited;
    public List<Address> roots;
    public List<Address> inits;

  //dests : dests Addr.Table.t;
    public List<Tuple<Address, string>> errors;
  //errors : (addr * error) list;
    public lifter lifter;
  //lifter : lifter option;
}

class stage2  {
  stage1 stage1;
  public Table<Address,mem> addrs;   /* table of blocks */
  public Table<Adress,dests> succs;
    public Table<Address,List<Address> preds;
    public Func<mem, List<decoded>> disasm;
}

public class stage3 {
  public Table<block> blocks;
    public List<Tuple<Address,string>> errors;
}
//type t = stage3

public Sexp.Atom sexp_of_block ( block blk) {
    return new Sexp.Atom (blk.addr.ToString());
}

let kind_of_dests = function
  | xs when List.for_all xs ~f:(fun (_,x) -> x = `Fall) -> `Fall
  | xs -> if List.exists  xs ~f:(fun (_,x) -> x = `Jump)
    then `Jump
    else `Cond

let kind_of_branches t f =
  match kind_of_dests t, kind_of_dests f with
  | `Jump,`Jump -> `Jump
  | `Fall,`Fall -> `Fall
  | _         -> `Cond

let fold_consts = Bil.(fixpoint fold_consts)

/* [dests_of_*] return a list of jumps, without concerning about
   fallthroughs. They're added later in [update_dests] */
public List<object> dests_of_bil(object bil) {
  fold_consts(bil).SelectMany(s => dests_of_stmt(s);
}

    public static IEnumerable<dest> dests_of_stmt(MachineInstruction instr)
    {
        if (instr is Jmp)
        {
            if (instr.Target is addr)
                return Tuple.Create(new dest { addr= addr, dest_kind=dest_kind.Jump); }
            else 
                return Tuple.Create(new dest { addr = null, dest_kind=dest_kind.Jump);}
        }
        if (instr is If)
        {
            return merge_branches (instr.yes, instr.no);
        }
        if (instr is While)
        {
            return dests_of_bil(instr.ss);
        }
        else 
            return new dest[0];
    }

    public static IEnumerable<dest> merge_branches (object yes, object no {
    var  x = dests_of_bil (yes);
        var y = dests_of_bil (no);
  //let kind = kind_of_branches x y in
  //List.(rev_append x y >>| fun (a,_) -> a,kind)
    }

/* the following is a speculation...  */
    public static IEnumerable<dest> dests_of_basic(object s, object insn) {
        var @is = Dis.Insn.@is (insn );
        var kind = @is( Conditional_branch ,then) Cond ? Jump ;
        if (@is is Indirect_branch || @is is Return)
            return new [] { new dest { address = null, dest_kind= kind };
        if (!(@is is Call || @is is Condiditional_branch || Unconditional_branch)
            return new dest[0];
    var ops = Dis.Insn.ops (insn).ToArray();
    if (ops.Length == 0)
        return new dest[0];
    if (ops[0] is Dis.Op.Imm off)
    {
        /* in ARM offset is shifted by 8 bytes, in x86 jump target can
           be relative or absolute, this all shouldn't work at all. */
        int width = Addr.bitwidth (s.addr);
        //Option.value_map (Dis.Imm.to_word ~width off)
        //  ~default:[] ~f:(fun off ->
        //      [Some Addr.Int_exn.(s.addr + off), kind])
    }
    return new dest[0];
}

//let errored s ty = {s with errors = (s.addr,ty) :: s.errors}

public static update_dests next s mem insn dests =
  let is = Dis.Insn.is insn in
  let fall = Some next, `Fall in
  let dests = match kind_of_dests dests with
    | `Fall when is `Return -> [] /* if BIL doesn't get this */
    | `Jump when is `Call -> fall :: dests
    | `Cond | `Fall -> fall :: dests
    | _ -> dests in
  /* prune dests that points to unrecognized or unmapped memory */
  let dests = List.map dests ~f:(fun d -> match d with
      | Some addr,kind when Memory.contains s.base addr -> d
      | _,kind -> None, kind) in
  let key = Memory.max_addr mem in
  begin match dests with
    | [] -> Addr.Table.add_exn s.dests ~key ~data:[]
    | dests -> List.iter dests ~f:(fun data ->
        Addr.Table.add_multi s.dests ~key ~data)
  end;
  { s with
    roots = List.filter_map ~f:fst dests |> List.rev_append s.roots;
  }

let update next s mem insn : stage1 =
  match s.lifter with
  | None -> update_dests next s mem insn (dests_of_basic s insn)
  | Some lift -> match lift mem insn with
    | Ok bil -> update_dests next s mem insn (dests_of_bil bil)
    | Error err -> errored s (`Failed_to_lift (mem,insn,err))

/* switch to next root or finish if there're no roots */
let next dis s =
  let rec loop s = match s.roots with
    | [] -> Dis.stop dis s
    | r :: roots when not(Memory.contains s.base r) ->
      loop {s with roots}
    | r :: roots when Span.mem s.visited r ->
      loop {s with roots}
    | addr :: roots ->
      let mem = match Span.upper_bound s.visited addr with
        | None -> Memory.view ~from:addr s.base
        | Some r1 -> Memory.range s.base addr r1  in
      let mem =
        Result.map_error mem ~f:(fun err -> Error.tag err "next_root") in
      mem >>= fun mem -> Dis.jump dis mem {s with roots; addr} in
  let visited = Span.add s.visited (s.addr, Dis.addr dis) in
  loop {s with visited}

let stage1 ?lifter ?(roots=[]) disasm base =
  let addr,roots = match roots with
    | r :: rs -> r,rs
    | [] -> Memory.min_addr base, [] in
  let init = {base; addr; visited = Span.empty;
              roots; inits = roots;
              dests = Addr.Table.create (); errors = []; lifter} in
  Memory.view ~from:addr base >>= fun mem ->
  Dis.run disasm mem ~stop_on:[`May_affect_control_flow] ~return ~init
    ~hit:(fun d mem insn s -> next d (update (Dis.addr d) s mem insn))
    ~invalid:(fun d mem s -> next d (errored s (`Failed_to_disasm mem)))
    ~stopped:next

/* performs the initial markup.

   Returns three tables: leads, terms, and kinds. Leads is a mapping
   from leader to all predcessing terminators. Terms is a mapping from
   terminators to all successing leaders. Kinds is a mapping from a
   terminator addresses, to all outputs with each output including the
   kind annotation, e.g, Cond, Jump, etc. This also includes
   unresolved outputs.
*/

let sexp_of_addr addr =
  Sexp.Atom (Addr.string_of_value addr)

let create_indexes (dests : dests Addr.Table.t) =
  let leads = Addrs.create () in
  let terms = Addrs.create () in
  let succs = Addrs.create () in
  Addr.Table.iter dests ~f:(fun ~key:src ~data:dests ->
      List.iter dests ~f:(fun dest ->
          Addrs.add_multi succs ~key:src ~data:dest;
          match dest with
          | None,_ -> ()
          | Some dst,kind ->
            Addrs.add_multi leads ~key:dst ~data:src;
            Addrs.add_multi terms ~key:src ~data:dst));
  leads, terms, succs

let stage2 dis stage1 =
  let leads, terms, kinds = create_indexes stage1.dests in
  let addrs = Addrs.create () in
  let succs = Addrs.create () in
  let preds = Addrs.create () in
  let inits =
    List.fold ~init:Addr.Set.empty stage1.inits ~f:Set.add in
  let next = Addr.succ in
  let is_edge addr =
    Addrs.mem leads (next addr) ||
    Addrs.mem kinds addr ||
    Addrs.mem stage1.dests addr ||
    Addr.Set.mem inits (next addr) in
  let is_visited = Span.mem stage1.visited in
  let next_visited = Span.upper_bound stage1.visited in
  let create_block start finish =
    Memory.range stage1.base start finish >>= fun blk ->
    Addrs.add_exn addrs ~key:start ~data:blk;
    let () = match Addrs.find terms finish with
      | None -> ()
      | Some leaders -> List.iter leaders ~f:(fun leader ->
          Addrs.add_multi preds ~key:leader ~data:start) in
    let dests = match Addrs.find kinds finish with
      | Some dests -> dests
      | None when Addrs.mem leads (next finish) &&
                  not (Addrs.mem stage1.dests finish) ->
        Addrs.add_multi preds ~key:(next finish) ~data:start;
        [Some (next finish),`Fall]
      | None -> [] in
    Addrs.add_exn succs ~key:start ~data:dests;
    return () in

  let rec loop start curr' =
    let curr = next curr' in
    if is_visited curr then
      if is_edge curr then
        create_block start curr >>= fun () ->
        loop (next curr) curr
      else loop start curr
    else match next_visited curr with
      | Some addr -> loop addr addr
      | None when is_visited start && is_visited curr' ->
        create_block start curr'
      | None -> return () in
  match Span.min stage1.visited with
  | None -> errorf "Provided memory doesn't contain recognizable code"
  | Some addr -> loop addr addr >>= fun () ->
    let dis = Dis.store_asm dis in
    let dis = Dis.store_kinds dis in
    let disasm mem =
      Dis.run dis mem
        ~init:[] ~return:ident ~stopped:(fun s _ ->
            Dis.stop s (Dis.insns s)) |>
      List.map ~f:(function
          | mem, None -> mem,(None,None)
          | mem, (Some ins as insn) -> match stage1.lifter with
            | None -> mem,(insn,None)
            | Some lift -> match lift mem ins with
              | Ok bil -> mem,(insn,Some bil)
              | _ -> mem, (insn, None)) in
    return {stage1; addrs; succs; preds; disasm}

module Block = struct
  type t = block
  type dest = blk_dest

  let compare (x:block) (y:block) =
    Addr.compare x.addr y.addr

  open Seq.Step

  exception Empty_block

  /* for the ease of end-user we shouldn't expose totaly
     broken blocks, that have zero valid instructions.

     We can't guarantee that no such blocks exist on initial
     stages of CFG reconstruction, since wrongly decoded jump
     targets can even point us into the middle of instruction.

     For example, we sweeped through a 7 bytes of memory, that was
     perfectly decodable. But the first 3 bytes was, actually a call
     to a function that never returns, so the left over is just a
     junk, that happens to be a valid instructions. The last
     instruction is happend to be a jump at offset -1. On a first
     pass we do not follow the jump, since it is already inside the
     memory region that was marked as visited and even decoded. As
     a result, we create a BB that points to one byte of undecodable
     trash.

        +-----------------------+
        |                       |
        |      jump exit        |
        |                       |
        +-----------------------+
        |    2 bytes of trash   |
        |                       |<----+
        +-----------------------+     |   points into the middle
        |   2 bytes of trash    |     |   of trash, that is no
        | decoded as (jump -1)  +-----+   longer decodable. BOOM
        +-----------------------+

     This all this said, we have to choices:
     1. Do not preserve the invariant, that blocks contain at least
        one decoded instruction, and as a result, make `leader` and
        `terminator` functions to return an option.

     2. Enforce the variant by filtering out empty blocks from the
       CFG. This requires us to perform a yet another disassembly,
       just to look inside the block and decide, whether it has any
       instructions.
  */


  let sort_dests =
    List.sort ~cmp:(fun x y -> match x,y with
        | (_,`Fall), _ -> 1
        | _,_ -> 0)

  let rec create t addr =
    let nabes inj side =
      Seq.of_list (side addr) |> Seq.unfold_with ~init:()
        ~f:(fun () nabe -> try inj nabe with exn -> Skip ()) in
    let block_of_pred addr = Yield (create t addr, ()) in
    let block_of_succ = function
      | Some addr, _ -> block_of_pred addr
      | None,_ -> Skip () in
    let make_dest = function
      | None,`Fall -> assert false
      | Some addr,kind ->
        Yield (`Block (create t addr, kind), ())
      | None,`Jump -> Yield (`Unresolved `Jump, ())
      | None,`Cond -> Yield (`Unresolved `Cond, ()) in
    let with_nil f a = Option.value ~default:[] (f a) in
    let succs a = sort_dests (with_nil (Addrs.find t.succs) a) in
    let preds a = with_nil (Addrs.find t.preds) a in
    let get_mem () = Addrs.find_exn t.addrs addr  in
    let mem = Lazy.from_fun get_mem in
    if List.for_all (t.disasm @@ get_mem ())
        ~f:(fun (_,(insn,_)) -> insn = None)
    then raise Empty_block;
    let insns = Lazy.(mem   >>| t.disasm) in
    let lead  = Lazy.(insns >>| List.hd_exn) in
    let term  = Lazy.(insns >>| List.last_exn) in
    {
      addr; mem; insns; term; lead;
      blk_succs = nabes block_of_succ succs;
      blk_preds = nabes block_of_pred preds;
      blk_dests = nabes make_dest succs;
    }

  let addr (b : block) = b.addr

  let memory {mem = lazy x} = x
  let leader {lead = lazy (_,x)} = x
  let terminator {term = lazy (_,x)} = x
  let insns {insns = lazy x} = x
  let succs t = t.blk_succs
  let preds t = t.blk_preds
  let dests t = t.blk_dests

  let sexp_of_t blk =
    Sexp.Atom Addr.(string_of_value (addr blk))

  let sexp_of_kind = function
    | `Jump -> Sexp.Atom "jump"
    | `Cond -> Sexp.Atom "cond"
    | `Fall -> Sexp.Atom "fall"

  let sexp_of_dest = function
    | `Unresolved kind -> Sexp.List [sexp_of_kind kind]
    | `Block (blk, kind) -> Sexp.List [
        sexp_of_kind kind;
        sexp_of_t blk;
      ]

  let compare_dest d1 d2 = match d1,d2 with
    | `Block (b1,k1), `Block (b2,k2) ->
      let r = compare b1 b2 in
      if r = 0 then Polymorphic_compare.compare k1 k2 else r
    | `Block _,`Unresolved _ -> 1
    | `Unresolved _, `Block _ -> -1
    | `Unresolved k1 , `Unresolved k2 ->
      Polymorphic_compare.compare k1 k2

  include Printable(struct
      type t = block
      let module_name = Some "Bap.Std.Disasm_expert.Recursive.Block"
      let pp fmt  ({addr; mem = lazy mem} : block) =
        Format.fprintf fmt "[%s, %s]"
          Addr.(string_of_value addr)
          Addr.(string_of_value (Memory.max_addr mem))
    end)

  module T = struct
    type t = block with sexp_of
    let t_of_sexp _ = invalid_arg "table element is abstract"
    let compare = compare
    let hash b = Addr.hash (addr b)
  end

  include Hashable.Make(T)
  include Comparable.Make(T)
end

let stage3 s2 =
  let is_found addr = Addrs.mem s2.addrs addr in
  let pred_is_found = is_found in
  let succ_is_found = function
    | None,_ -> true
    | Some addr,_ -> is_found addr in
  let filter bs ~f = Addrs.filter_map bs ~f:(fun ps ->
      match List.filter ps ~f with
      | [] -> None
      | preds -> Some preds) in
  let s2 = {
    s2 with
    succs = filter s2.succs ~f:succ_is_found;
    preds = filter s2.preds ~f:pred_is_found;
  } in
  Addrs.fold ~init:(return Table.empty)
    s2.addrs ~f:(fun ~key:addr ~data:mem tab ->
        tab >>= fun tab ->
        try
          Table.add tab mem (Block.create s2 addr)
        with exn -> return tab) >>= fun blocks ->
  return {blocks; failures = s2.stage1.errors}


let run ?(backend="llvm") ?lifter ?roots arch mem =
  Dis.create ~backend (Arch.to_string arch) >>= fun dis ->
  stage1 ?lifter ?roots dis mem >>= stage2 dis >>= stage3

let blocks t = t.blocks
let compare_block = Block.compare

let errors s = List.map s.failures ~f:snd

let sexp_of_block = Block.sexp_of_t
