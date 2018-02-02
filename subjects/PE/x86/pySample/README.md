**pySample.dll**

This is a Python module sample, contributed by @ptomin.

```C

    #include <python.h>

    static PyObject* py_sum(PyObject* self, PyObject* args);
    static PyObject* py_dif(PyObject* self, PyObject* args);
    static PyObject* py_div(PyObject* self, PyObject* args);
    static PyObject* py_fdiv(PyObject* self, PyObject* args);
    static PyObject *py_unused(PyObject *self, PyObject *args);

    static PyMethodDef methods[] = {
        { "sum",     py_sum,     METH_VARARGS,   "sum(a, b) = a + b" },
        { "dif",     py_dif,     METH_VARARGS,   "dif(a, b) = a - b" },
        { "div",     py_div,     METH_VARARGS,   "div(a, b) = a / b" },
        { "fdiv",    py_fdiv,    METH_VARARGS,   "fdiv(a, b) = a / b" },
        { NULL,      NULL,       0,              NULL },
        { "unused",  py_unused,  METH_VARARGS,   "unused" },
    };


    PyObject *py_sum(PyObject *self, PyObject *args) {
        int a, b;

        if (!PyArg_ParseTuple(args, "ii:sum", &a, &b))
            return NULL;

        return Py_BuildValue("i", a + b);
    }

    PyObject *py_dif(PyObject *self, PyObject *args) {
        int a, b;

        if (!PyArg_ParseTuple(args, "ii:dif", &a, &b))
            return NULL;

        return Py_BuildValue("i", a - b);
    }

    PyObject *py_div(PyObject *self, PyObject *args) {
        int a, b;

        if (!PyArg_ParseTuple(args, "ii:div", &a, &b))
            return NULL;

        return Py_BuildValue("i", a/b);
    }

    PyObject *py_fdiv(PyObject *self, PyObject *args) {
        float a, b;

        if (!PyArg_ParseTuple(args, "ff:fdiv", &a, &b))
            return NULL;

        return Py_BuildValue("f", a/b);
    }

    PyObject *py_unused(PyObject *self, PyObject *args) {

        if (!PyArg_ParseTuple(args, ":unused"))
            return NULL;

        Py_INCREF(Py_None);
        return Py_None;
    }

    __declspec(dllexport) void initpySample() {
        Py_InitModule("pySample", methods);
    }
```
