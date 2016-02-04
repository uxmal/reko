Issue #74 of yegord/snowman 

Contains a method of a C++ class that probably looked something like this initially:

```C++
class A {
public:
    unsigned char get(int n) { return (unsigned char) this.m_dw0004 + n; }
private:
    int m_dw0004;
}
```

