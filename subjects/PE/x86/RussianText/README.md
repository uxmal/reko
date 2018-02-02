**RussianText.exe**

This is small x86 PE executable containing Russian text, contributed by @ptomin.

```C

//---------------------------------------------------------------------------

#pragma hdrstop

//---------------------------------------------------------------------------

#pragma argsused
int main(int argc, char* argv[])
{
    printf("Это текст на русском языке для тестирования декомпилятора Reko");
    return 0;
}
//---------------------------------------------------------------------------
```
