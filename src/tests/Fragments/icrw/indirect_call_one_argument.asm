;;; indirect_call_one_argument.asm
;;; Test for IndirectCallRewriter.

.i386

main proc
        mov eax, [esp+4]
        mov ecx, [eax]
        push 0x0000000A
        call dword ptr [ecx+4]
        ret
main endp
