;;; indirect_call_two_arguments.asm
;;; Test for IndirectCallRewriter.

.i386

main proc
        mov eax, [esp+4]
        mov ecx, [eax]
        push 0x0000000B
        push 0x0000000A
        call dword ptr [ecx+8]
        ret
main endp
