;;; This fragment is for testing type inference.
;;; It has so little information about the types in question that the 
;;; type inference has to stop with: u32

.i86

mov eax,[bx+04]
mov [bx],eax
ret
