'''
We have to decompile python extension dll. There was method table defined at
0x10003010 address. The definition looks like

PyMethodDef methods[] = {
    { "name", function_ptr, METH_VARARGS, "Documentation" },
    <...>
    { NULL, NULL, 0, NULL },
};

The definition of PyMethodDef is

struct PyMethodDef {
    char *ml_name;
    PyCFunction ml_meth;
    int ml_flags;
    char *ml_doc;
};

We can set user defined names for each function from the method table using
Reko GUI. But what if we have hundreds of such functions? This script gives
possibility to use python extension method name (ml_name field of PyMethodDef
structure) as pattern for name of C wrapper function (ml_meth field of
PyMethodDef structure). All function names from the table should be defined
after running of event handler defined at this script.
'''

# OnProgramLoaded event handler. Called when program was loaded to memory
def on_program_loaded(program):
    mem = program.memory
    num_methods = 0
    # Python extension module method table start address
    addr = 0x10003010
    while True:
        # read pointer to method name
        char_ptr_addr = mem[addr].int32
        addr += 0x04
        # if null then it's the end of table
        if char_ptr_addr == 0:
            break
        # read method name
        method_name = mem[char_ptr_addr].c_str
        # read method address
        method_addr = mem[addr].int32
        # go to next method entry
        addr += 0x0C
        # set procedure name for method based on name defined at table entry
        program.procedures[method_addr] = '{}_wrapper'.format(method_name)
        # move decompiled code of procedure to separate file
        program.procedures[method_addr].file = '{}.c'.format(method_name)
        num_methods += 1
    program.procedures[0x10001140] = \
        'PyObject *unused_wrapper(PyObject *self, PyObject *args)'
    program.globals[0x10003010] = 'PyMethodDef methods[{}]'.format(num_methods)
    exclude_dll_main(program)
    comments = []
    dump_procedures(program, comments)
    get_procedure(program, 0x00000001, comments)
    get_procedure(program, 0x1000149E, comments)
    get_procedure(program, 0x1000149D, comments)
    check_procedure(program, 0x00000002, comments)
    check_procedure(program, 0x100010F0, comments)
    check_procedure(program, 0x100010F1, comments)
    test_memory_reading(program, comments)
    program.comments[0x10001170] = "\n".join(comments)
    program.comments[0x10001183] = \
        'This is initialization of Python extension module'

def exclude_dll_main(program):
    program.procedures[0x1000149E] = 'DllMain'
    program.procedures[0x1000149E].decompile = False

def dump_procedures(program, lines):
    lines.append('Procedures:')
    for addr, proc in program.procedures.items():
        lines.append('    {}:'.format(proc.name))
        lines.append('        Address: 0x{:08X}'.format(addr))
        lines.append('        File: {}'.format(proc.file))
        lines.append('        Decompile: {}'.format(proc.decompile))

def get_procedure(program, addr, lines):
    proc = program.procedures.get(addr)
    name = proc.name if proc is not None else None
    lines.append('    Get procedure at 0x{:08X}: {}'.format(addr, name))

def check_procedure(program, addr, lines):
    if addr in program.procedures:
        lines.append('    There is procedure entry at 0x{:08X}'.format(addr))
    else:
        lines.append('    There is no procedure entry at 0x{:08X}'.format(addr))

def test_memory_reading(program, lines):
    mem = program.memory
    lines.append('Memory at 0x10003020:')
    lines.append('    Byte: 0x{:X}'.format(mem[0x10003020].byte))
    lines.append('    16-bit integer: 0x{:X}'.format(mem[0x10003020].int16))
    lines.append('    32-bit integer: 0x{:X}'.format(mem[0x10003020].int32))
    lines.append('    64-bit integer: 0x{:X}'.format(mem[0x10003020].int64))
    lines.append('Memory at [0x10003020:0x10003024]:')
    lines.append('    Bytes: {}'.format(
        bytes_to_list(mem[0x10003020:0x10003024].byte)))
    lines.append('    16-bit integers: {}'.format(
        bytes_to_list(mem[0x10003020:0x10003024].int16)))
    lines.append('Memory at [0x10003020:0x10003030]:')
    lines.append('    32-bit integers: {}'.format(
        bytes_to_list(mem[0x10003020:0x10003030].int32)))
    lines.append('    64-bit integers: {}'.format(
        bytes_to_list(mem[0x10003020:0x10003030].int64)))

def bytes_to_list(numbers):
    return ['0x{:X}'.format(num) for num in numbers]
