using Reko.Core;
using Reko.Core.Lib;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//type insn = Bap_disasm_insn.t
using insn = Reko.Core.Machine.MachineInstruction;
//open Core_kernel.Std
//open Bap_types.Std
//open Bap_insn_aliasing

//module Dis = Bap_disasm_basic
//module Memory = Bap_memory
//module Memmap = Bap_memmap
//module Insn = Bap_disasm_insn



namespace Reko.Scanning
{
    class Sheerable { }

    //type sheerable = Maybe_code of insn
    //               | Data
    class MaybeCode : Sheerable { public insn insn; }
    class Data : Sheerable { }

    class ShingledScanner
    {


        //let prev_chunk mem ~addr =
        //  let prev_addr = Addr.pred addr in
        //  Memory.view ~from:prev_addr mem

        //let disasm dis mem =
        //  let open Seq.Generator in
        //  let prev = prev_chunk mem in
        //  let rec back cur_mem =
        //    let cur_point = Memory.min_addr cur_mem in
        //    let elem = match Dis.insn_of_mem dis cur_mem with
        //      | Ok (mem, insn, _) ->  (mem, insn)
        //      | Error _ -> (cur_mem, None) in
        //    match prev ~addr:cur_point with
        //    | Ok next_mem -> yield elem >>= fun () -> back next_mem
        //    | Error _ -> yield elem >>= fun () -> return () in
        //  match (Memory.view mem ~from:(Memory.max_addr mem)) with
        //  | Ok mem -> back mem
        //  | Error err -> raise (Error.to_exn err)

        //let all_shingles dis mem ~init ~at =
        //  Sequence.fold (Sequence.Generator.run (disasm dis mem)) ~init ~f:at

        //let markup dis gmem =
        //  let open Seq.Generator in
        //  let shingles = Memmap.empty in
        //  let at shingles (mem, insn) = (
        //    match insn with
        //    | None -> (Memmap.add shingles mem Data)
        //    | Some insn -> 
        //      (Memmap.add shingles mem (Maybe_code (Insn.of_basic insn))) 
        //  ) in (all_shingles dis gmem ~init:shingles ~at)

        //let static_successors gmin gmax lmem insn =
        //  let fall_through =
        //    if not (is_exec_ok gmin gmax lmem) then [None, `Fall]
        //    else
        //      let i = Word.((Memory.min_addr lmem) ++ (Memory.length lmem)) in      
        //      [Some i, `Fall] in
        //  if (Insn.may_affect_control_flow insn) then (
        //    let targets = (Insn.bil insn |> dests_of_bil) in
        //    if (not @@ Insn.is_unconditional_jump insn) then
        //      List.append fall_through targets
        //    else
        //      targets
        //  ) else fall_through


        //(* Sheer simply prunes known unacceptable receiving state points by
        //   calculating the transitive closure of a node set of x, if it
        //   contains an invalid instruction, this x is invalid too  *)
        //let sheer shingles gmem =
        //  (* depth first search for all that bad points to *)
        //  let mark_data shingles mem =
        //    Memmap.add (Memmap.remove shingles mem) mem Data in
        //  let gmin = Memory.min_addr gmem in
        //  let gmax = Memory.max_addr gmem in
        //  let target_in_mem =
        //    targ_in_mem gmin gmax in
        //  let get_targets = static_successors gmin gmax in
        //  let lookup addr = Memmap.lookup shingles addr |>
        //          Seq.find ~f:(fun (mem,_) ->
        //                        Addr.equal addr (Memory.min_addr mem)) in
        //  let find_mem addr f default= match lookup addr with
        //    | Some (mem, _) -> f default mem
        //    | _ -> default in
        //  let is_data addr = not (target_in_mem addr) ||
        //                     match lookup addr with
        //                     | Some (mem, (Maybe_code _)) -> false
        //                     | _ -> true in
        //  let rec sheer_data shingles addr = 
        //    if not (target_in_mem addr) then
        //      (find_mem addr mark_data shingles)
        //    else
        //      match lookup addr with
        //      | Some (mem, (Maybe_code insn)) ->
        //        let targets = (get_targets mem insn) in
        //        let shingles, result = List.fold ~init:(shingles,false) targets
        //            ~f:(fun (shingles, so_far) (target,_) ->
        //                match target with
        //                | Some target ->
        //                  if (so_far || (is_data target)) then
        //                    mark_data shingles mem, true
        //                  else
        //                    sheer_data shingles target, false
        //                | None -> mark_data shingles mem, true) in
        //        let shingles, final_result =
        //          List.fold ~init:(shingles, false || result) targets
        //            ~f:(fun (shingles, so_far) (target,_) ->
        //                match target with
        //                | Some target ->
        //                  if (so_far || is_data target) then
        //                    mark_data shingles mem, true
        //                  else shingles, so_far
        //                | None -> shingles, true
        //              ) in
        //        if result then
        //          sheer_data (mark_data shingles mem) (Addr.succ addr)
        //        else sheer_data shingles (Addr.succ addr)
        //      | Some (_, Data) | None -> sheer_data shingles (Addr.succ addr)
        //  in sheer_data shingles gmin

        //(* TODO: this should apply the PFSM to the memmap to make selections *)
        //let maximum_probable_paths shingles gmem =
        //  let shingles = sheer shingles gmem in
        //  Memmap.filter_map shingles ~f:(function
        //      | Maybe_code insn -> Some insn
        //      | _ -> None)

        //let sheered_shingles ?backend arch ?dis mem =
        //  let backend = Option.value backend ~default:"llvm" in
        //  let dis = Option.value dis ~default:
        //      (Dis.create ~backend (Arch.to_string arch) |> ok_exn)  in
        //  let shingles = markup dis mem in
        //  maximum_probable_paths shingles mem

        public void Dasm(byte[] data, IProcessorArchitecture arch)
        {
            /*
            const int bad = -1;
            const byte MaybeCode = 1;
            const byte Data = 2;
            var G = new DiGraph<int>();
            var y = new byte[data.Length];
            var step = arch.InstructionBitSize / 8;
            for (var a = 0; a < data.Length; a += step)
            {
                y[a] = MaybeCode;
                MachineInstruction i = Dasm(data, a);
                if (!i.IsValid)
                    G.AddEdge(bad, a);
                else if ((i.InstructionClass & InstructionClass.Linear | InstructionClass.Conditional) != 0)
                {
                    if (a + i.Length < data.Length)
                        G.AddEdge(a + i.Length, a);
                    else
                        G.AddEdge(bad, a);
                }
                else if (i.InstructionClass == InstructionClass.Transfer || i.InstructionClass == InstructionClass.Call ||
                        i.InstructionClass == InstructionClass.Conditional | InstructionClass.Transfer)
                {
                    if (is_executable(dest(i))
                        G.AddEdge(dest(i), a);
                    else
                        G.AddEdge(bad, a);
                }
            }
            foreach (var a in new DfsIterator<int>(G).PreOrder(bad))
            {
                y[a] = Data;
            }
            */
        }
    }
}
