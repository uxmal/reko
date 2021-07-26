def define_entry_point(program):
    program.procedures[(0x1234, 0x0000)] = "entry"
    comments = []
    dump_procedures(program, comments)
    test_memory_reading(program, comments)
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
    # This search don't work correctly in common cases ('0xC3' byte can be
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

def test_memory_reading(program, lines):
    mem = program.memory
    lines.append('Memory at 0x1234:0x0000:')
    lines.append('    Byte: 0x{:X}'.format(mem[(0x1234, 0x0000)].byte))
    lines.append('    16-bit integer: 0x{:X}'.format(mem[(0x1234, 0x0000)].int16))
    lines.append('    32-bit integer: 0x{:X}'.format(mem[(0x1234, 0x0000)].int32))
    lines.append('    64-bit integer: 0x{:X}'.format(mem[(0x1234, 0x0000)].int64))
    lines.append('Memory at ["0x1234:0x0000":"0x1234:0x0004"]:')
    lines.append('    Bytes: {}'.format(
        bytes_to_list(mem[(0x1234, 0x0000):(0x1234, 0x0004)].byte)))
    lines.append('    16-bit integers: {}'.format(
        bytes_to_list(mem[(0x1234, 0x0000):(0x1234, 0x0004)].int16)))
    lines.append('Memory at ["0x1234:0x0000":"0x1234:0x0010"]:')
    lines.append('    32-bit integers: {}'.format(
        bytes_to_list(mem[(0x1234, 0x0000):(0x1234, 0x0010)].int32)))
    lines.append('    64-bit integers: {}'.format(
        bytes_to_list(mem[(0x1234, 0x0000):(0x1234, 0x0010)].int64)))

def bytes_to_list(numbers):
    return ['0x{:X}'.format(num) for num in numbers]

reko.on.program_decompiling += define_entry_point
reko.on.program_scanned += define_procedures
