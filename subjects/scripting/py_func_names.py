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
        num_methods += 1
    program.globals[0x10003010] = 'PyMethodDef methods[{}]'.format(num_methods)
    exclude_dll_main(program)
    comments = []
    dump_procedures(program, comments)
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
        lines.append('        Decompile: {}'.format(proc.decompile))
