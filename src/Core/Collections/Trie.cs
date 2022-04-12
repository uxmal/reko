using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.Collections
{
    public class qTrie<Key>
    {
        //open Core_kernel.Std
        //open Format
        //open Option.Monad_infix
        //open Bap_trie_intf

        //module Make(Key : Key) = struct
        //  module Tokens = Hashtbl.Make_binable(struct
        //      type t = Key.token with bin_io, compare, sexp
        //      let hash = Key.token_hash
        //    end)

        //type key = Key.t
        //type 'a t = {
        //  mutable data : 'a option;
        //  subs : 'a t Tokens.t;
        //} with bin_io, fields, sexp

        //let init v = {data = Some v; subs = Tokens.create ()}
        //let create () = {data = None; subs = Tokens.create ()}

        //let change trie k f {return} =
        public qTrie<Key> change(Key k, Func<bool> f)
        {
            throw new NotImplementedException();
#if NYI
    //let len = Key.length k in
            var len = k.Length;
    //let rec loop n {subs} =
      //let c = Key.nth_token k n in
      //if n + 1 = len then found subs c else find subs n c
            Func<int,
    and found parent c = Tokens.change parent c (function
        | None -> f None >>| init
        | Some t -> Some {t with data = f t.data})
    and find parent n c = match Tokens.find parent c with
      | Some s -> loop (n+1) s
      | None -> match f None with
        | None -> return ()
        | Some _ ->
          let sub = create () in
          Tokens.add_exn parent c sub;
          loop (n+1) sub in
    if len > 0 then loop 0 trie
    else trie.data <- f trie.data

  let walk root k ~init ~f  =
    let len = Key.length k in
    let rec lookup best n trie =
      match Tokens.find trie.subs (Key.nth_token k n) with
      | None -> best
      | Some sub ->
        let best = f best sub.data in
        if n + 1 = len then best else lookup best (n+1) sub in
    let best = f init root.data in
    if len > 0 then lookup best 0 root else best

  let length trie =
    let rec count trie =
      let n = if trie.data = None then 0 else 1 in
      Tokens.fold ~init:n trie.subs
        ~f:(fun ~key:_ ~data:trie n -> n + count trie) in
    count trie

  let longest_match root k =
    walk root k ~init:(0,0,None) ~f:(fun (n,i,best) -> function
        | None -> (n+1,i,best)
        | better -> (n+1,n,better)) |> function
    | (_,i,Some thing) -> Some (i,thing)
    | _ -> None

  let change trie k f = with_return (change trie k f)
  let add trie ~key ~data:v = change trie key (fun _ -> Some v)
  let remove trie k = change trie k (fun _ -> None)

  let find trie k = match longest_match trie k with
    | Some (s,v) when s = Key.length k -> Some v
    | _ -> None

  let rec pp pp_val fmt t =
    let pp_some_data fmt = function
      | None -> ()
      | Some v -> fprintf fmt "data =@ %a@," pp_val v in
    let pp_table fmt cs =
      Tokens.iter cs (fun ~key ~data ->
          let toks = Key.sexp_of_token key in
          fprintf fmt "@[%a ->@ %a@]"
            Sexp.pp toks (pp pp_val) data) in
    fprintf fmt "{@;@[%a@ %a@]}@;"
      pp_some_data t.data pp_table t.subs
end

module String = struct
  module Common = struct
    type t = string
    type token = char with bin_io, compare, sexp
    let length = String.length
    let token_hash = Char.to_int
  end
  module Prefix = Make(struct
      include Common
      let nth_token = String.get
    end)

  module Suffix = Make(struct
      include Common
      let nth_token s n =
        s.[String.length s - n - 1]
    end)
end

module Array = struct
  module Common(T : Token) = struct
    type t = T.t array
    type token = T.t with bin_io, compare, sexp
    let length = Array.length
    let token_hash = T.hash
  end
  module Prefix(T : Token) = Make(struct
      include Common(T)
      let nth_token = Array.get
    end)

  module Suffix(T : Token) = Make(struct
      include Common(T)
      let nth_token xs n = xs.(Array.length xs - n - 1)
    end)
end
#endif
        }
    }
}
