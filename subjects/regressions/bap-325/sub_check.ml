open Core_kernel.Std
open Bap.Std
open Option.Monad_infix
open Sys
open Format
open Graph
    
let get_sub_root program_name sub = Term.first blk_t sub
let get_sub_root_exn program_name sub = Option.value (Term.first blk_t sub)
let sub_from_prg prg func_name =
  (Term.to_sequence sub_t prg
   |> Seq.find ~f:(fun s -> (Sub.name s) = func_name))

let sub_from_prg_exn prg func_name =
  Option.value_exn (sub_from_prg prg func_name)

let check_sub_roots ?(verbose=false) program_name sub = 
  let g = Sub.to_graph sub in
  let has_no_preds g tid = Graphlib.Tid.Tid.Node.degree ~dir:`In tid g = 0 in
  let roots =  Graphlib.Tid.Tid.nodes g |> Seq.filter ~f:(has_no_preds g) in
  let ok_num_roots = 
    if (Seq.length roots) <> 1 then begin
      if verbose then
        sprintf "program:%s sub:%s #roots=%d"
          program_name (Sub.name sub) (Seq.length roots) |> print_endline;
      false
    end
    else true
  and ok_root = match (get_sub_root program_name sub) with
      None -> false
    | Some root ->
      match Graphlib.Tid.Tid.Node.degree ~dir:`In (Term.tid root) g with
        0 -> true
      | p -> begin
          if verbose then sprintf "program:%s sub:%s root's #preds=%d"
              program_name (Sub.name sub) p |> print_endline;
          false
        end
  in
  ok_num_roots && ok_root
  
let check_sym proj sym =
  let program = Project.program proj in
  let program_name = Option.value (Project.get proj filename) ~default:"?" in
  let sub = Symtab.name_of_fn sym |> sub_from_prg_exn program in
  let _ok_roots = check_sub_roots ~verbose:true program_name sub in
  ()
  
let main argv proj =
  let symbols = Project.symbols proj in
  let seq_syms = Symtab.to_sequence symbols in
  Seq.iter seq_syms ~f:(fun sym -> check_sym proj sym);
  proj
  
let () = Project.register_pass_with_args "sub_check" main
