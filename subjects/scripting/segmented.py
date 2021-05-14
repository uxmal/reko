def define_entry_point(program):
    program.procedures[(0x1234, 0x0000)] = "entry"
    comments = []
    dump_procedures(program, comments)
    program.comments[(0x1234, 0x0000)] = '\n'.join(comments)

def define_procedures(program):
    mem = program.memory
    for addr, proc in program.procedures.items():
        if proc.decompile:
            proc.decompile = False
            if proc.decompile:
                raise Exception("'decompile' flag should be set to 'False'")
            decl_addr = find_procedure_end(program, addr)
            decl = mem[decl_addr].c_str
            program.procedures[addr] = decl
            program.globals[decl_addr] = "char {}_decl[{}]".format(
                proc.name, len(decl))
            if not proc.decompile:
                raise Exception(
                    "'decompile' flag should be reset to 'True' after declaration setting"
                )

def find_procedure_end(program, proc_addr):
    (seg, offset) = proc_addr
    mem = program.memory
    # This search don't work correctly in commom cases ('0xC3' byte can be
    # inside of other instructions). But it's enough for this little sample
    ret = 0xC3
    while mem[(seg, offset)].byte != ret:
        offset += 1
    return (seg, offset + 1)

def dump_procedures(program, lines):
    lines.append('Procedures:')
    for addr, proc in program.procedures.items():
        (seg, offset) = addr
        lines.append('    {}:'.format(proc.name))
        lines.append('        Address: 0x{:04X}:0x{:04X}'.format(seg, offset))
        lines.append('        File: {}'.format(proc.file))
        lines.append('        Decompile: {}'.format(proc.decompile))

reko.on.program_decompiling += define_entry_point
reko.on.program_scanned += define_procedures
