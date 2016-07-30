;;; indirect_call_no_arguments.asm
;;; Test for IndirectCallRewriter.

.i386

main proc
        mov eax, [esp+4]
        mov ecx, [eax]
        call dword ptr [ecx]
        ret
main endp
