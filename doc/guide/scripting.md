# Writing scripts for Reko

User can set names for each function, add global variables definitions, comments
and other data using Reko GUI. But what if hundreds of such operations are
required? Writing scripts can help user to customize Reko decompilation process.

## Reko scripting engine

[IronPython](https://ironpython.net) implementation of
[Python](https://python.org/) language is used.

## Reko script files

User can create new script files or add existing ones to project using
[Reko GUI](gui.md#adding-script-files). Reko scripting is event-based. There should be
attaching one or several event handers at top level of script file. Following
syntax is used:
```
reko.on.<Reko scripting event> += <User defined event handler>
```
Event handler can be Python function, lambda expression or any other callable
Python object. `Program` will be passed as single argument of these handlers.

_Warning! Script can be evaluated at undefined time and undefined number of
times. Do not place code which has side effects at top level of script. Such
code should be located at event handlers._

## Reko scripting events

Following Reko events are supported:
| Reko event             | Description                                                           |
| ---------------------- | --------------------------------------------------------------------- |
| on_program_loaded      | Fired when program was loaded to memory                               |
| on_program_decompiling | Fired before starting of program decompilation                        |
| on_program_scanned     | Fired when program was scanned                                        |
| on_program_decompiled  | Fired when program was decompiled but before output files are written |

## Examples
- [Setting names for Python extension methods](/subjects/scripting/py_func_names.py).
- [Segmented addresses](/subjects/scripting/segmented.py).
