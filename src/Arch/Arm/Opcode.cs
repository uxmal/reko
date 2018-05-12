namespace Reko.Arch.Arm
{
    public enum Opcode
    {
        Unknown = -1,
        Invalid = 0,
        hlt = 1,
        bkpt = 2,
        sub = 3,
        add = 4,
        blx = 5,
        bx = 6,
        mov = 7,
        str = 8,
        ldr = 9,
    }
}