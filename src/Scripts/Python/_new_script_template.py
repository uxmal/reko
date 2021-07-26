def on_program_loaded(program):
    """Implement this function to process event fired when program was loaded
    to memory.
    """

def on_program_decompiling(program):
    """Implement this function to process event fired before starting of
    program decompilation.
    """

def on_program_scanned(program):
    """Implement this function to process event fired when program was scanned.
    """

def on_program_decompiled(program):
    """Implement this function to process event fired when program was
    decompiled but before output files are written.
    """

# Subscribe to Reko events
reko.on.program_loaded += on_program_loaded
reko.on.program_decompiling += on_program_decompiling
reko.on.program_scanned += on_program_scanned
reko.on.program_decompiled += on_program_decompiled
