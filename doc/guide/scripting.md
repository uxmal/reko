# Writing scripts for Reko

Users can give arbitrary names to each decompiled procedure, add global variable definitions, comments, 
and otherwise customize the decompiler output using the Reko GUI. But what if hundreds of such operations are
required? Writing scripts can help users automate customizing the Reko decompilation process.

## Reko scripting engine

Reko uses the [IronPython](https://ironpython.net) implementation of the [Python](https://python.org/) language.

## Reko script files

Users can create new script files or add existing ones to a Reko project using the
[Reko GUI](gui.md#adding-script-files). Reko scripting is event-based. Scripts 
attach one or more event handers at the top level of script file. The following
syntax is used:
```Python
reko.on.<Reko scripting event> += <User defined event handler>
```
An event handler can be a Python function, lambda expression or any other callable
Python object. A reference to a `Program` instance will be passed as the single argument
 of these handlers.

_**Note:** Scripts can be evaluated at undefined times and more than one time. Do not place 
code which has side effects at top level of scripts. Such code should be located inside the event handlers._

## Reko scripting events

The following Reko events are supported:
| Reko event               | Description                                                           |
|--------------------------|-----------------------------------------------------------------------|
| `on_program_loaded`      | Fired when program was loaded into memory                             |
| `on_program_decompiling` | Fired before program decompilation starts                             |
| `on_program_scanned`     | Fired after program has been scanned                                  |
| `on_program_decompiled`  | Fired after program has been decompiled but before output files are written |

## Examples
- [Setting names for Python extension methods](/subjects/scripting/py_func_names.py).
- [Segmented addresses](/subjects/scripting/segmented.py).
