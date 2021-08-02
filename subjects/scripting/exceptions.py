class ExceptionPrinter:
    def __init__(self, errors):
        self._errors = errors

    def __enter__(self):
        pass

    def __exit__(self, exc_type, exc_value, traceback):
        if exc_type is None:
            return False
        self._errors.append("{}: {}".format(exc_type.__name__, exc_value))
        return True

def define_entry_point(program):
    program.procedures[(0x1000, 0x0020)] = "entry"

def set_comments(program, comments):
    with ExceptionPrinter(comments):
        procedure = program.procedures[(0x1000, 0x0010)]
    program.comments[(0x1000, 0x0020)] = '\n'.join(comments)

def infinite_recursion():
    infinite_recursion()

errors = []
with ExceptionPrinter(errors):
    reko.on.undefined_event = None
with ExceptionPrinter(errors):
    reko.on.undefined_event += define_entry_point
with ExceptionPrinter(errors):
    infinite_recursion()
reko.on.program_decompiling += define_entry_point
reko.on.program_decompiling += lambda program: set_comments(program, errors)
