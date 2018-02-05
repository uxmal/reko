# Reko coding style document

This document formalizes the programming style conventions in use in the Reko project. When many contributors make changes to a shared 
codebase it becomes to state the coding conventions in use to avoid unneccesary merge conflicts and disputes. None of these
rules are absolute, but in general please adhere to these style rules unless you have a very good reason not to.

### Formatting
Indentation in the source code is written with spaces, not tabs. Indentation stops are 4 spaces.
Note: the Reko codebase is old and was originally written in C++ using tabs for indentation before
being translated to C#. Some older sections of the sources still have tabs. To avoid pointless whitespace
diffs in the code, those old code files have not been de-tabbified in bulk. However, if you are modifying
one of these older files, prefer using spaces in the changed code.

Modern monitors make line lengths longer than 80 feasible. Still, very long lines are harder to read than 
shorter lines. If you're working with a line that is longer than 120 characters, consider either breaking 
it up into smaller subexpressions on separate lines, or at least fold the line into multiple lines.

Formatting rules are the default settings in Visual Studio. Some of these settings are repeated here for
convenience.

Blocks starting with '{' are introduced on a separate line. Vertical space is no longer an issue now
that that we have monitors capable of displaying more than 24 lines.
```C#
if (foo)
{
    bar();
}

if (foo) {              // Please don't
    bar();
}
```

Spaces used to separate terms in expressions go 'inside' the expression, not outside:
```C#
a = b * c + (a / c);    // Preferred

a = b*c + ( a/c );      // Please don't
```

No spaces after function applications, but spaces after C# keywords that are followed by parentheses:
```C#
myclass.MyMethod(foo);  // Preferred
if (foo.bar)  // ...
while (bar.baz()) // ...
return (a + b) * c;
return x;

myclass.MyMethod (foo)    // Please don't
if(foo.bar) // ...
while( bar.baz ()) // ...
return(a+b)*c;
return (x);
```

Statements inside the substatements of `if`, `while`, `do-while`, and `for` statements are always enclosed
in blocks, even if there is only a single statement. The only exceptions are if the single statemet is a 
control transfer instruction like `return`, `break`, `continue`, `goto` or `throw` or the empty statement of 
a while loop:
```C#
if (foo == null) 
{
    bar();
}
while (bar.foo())
{
    foo.bar();
}

// But...
if (foo == null)
    throw new ArgumentException(nameof(foo));
if (bar != foo)
    return false;
while (!done())
    ;

// Please don't
if (foo == null)
    bar();
while (foo.bar()) bar.foo();
while (!done()) {}
```

### Organization
C# classes are written as follows:

```C#
public class MyClass
{
    // Event definitions
    public event EventHandler FooChanged;
    public event EventHandler BarChanged;

    // Static and constant fields
    public const int MaxValue = 0x1234;
    public static readonly Dictionary<int, string> Lookups;

    // Member fields
    private string foo;
    private string bar;

    // Constructors
    public MyClass(string value)
    {
        this.foo = value;
    }

    // Properties

    public string Foo { get{ return this.foo; } }

    // Methods:
    // Public methods first, then internal, protected, and finally private.
}
```

### Naming
We use camel-case consistently, unless using symbols from external sources that don't comply with this rule.
Private C# fields and local variables are written with an initial lower case letter:
```C#
private int xRange;
private int _xRange;        // please don't
private int m_xRange;		// please don't
```
Public members are written with initial upper case letters, as are class and namespaces:
```C#
public int XRange { get { return xRange; } }
public class Modulator // ...
```
Prefer making member variable names longer and self-documenting and avoid cryptic abbreviations.
Method parameters and local variables can be shorter, especially index or enumeration variables, but not cryptic:
```C#
public class Accumulator
{
	private int totalSum;
	private IEnvironment environment;

	public TheClass(IEnvironment env)
	{
		this.environment = env;
		this.totalSum = 0;
		foreach (var item in env.Items)
		{
			this.totalSum += item;
		}
	}
}

// Please don't
public class Acc
{
	private int n;
	private IEnvironment e;

	public Acc(IEnvironment e)
	{
		// ...
	}
}
```

Well-established acronyms are fine:
```C#
public void RenderHtml()
{
	// ...
}


// Please don't
public void RenderHyperTextMarkupLanguage()
{
	// ...
}
```

When defining a generic type with a single type argument, use the name `T`:
```C#
public class ProgramCollection<T> // ...
```
If it has more than one type argument, prefix the argument types with `T`:
```C#
public class EfficientDictionary<TKey, TValue> // ...
```

### Usage of the `var` keyword in assignments
When it is clear what the type of the expression being assigned is, prefer using `var`, 
especially when dealing with generic types:
```C#
var dict = new Dictionary<Procedure,List<Block>>();
var binExpr = expr as BinaryExpression;

// Please don't
Dictionary<Procedure,List<Block>> dict = new Dictionary<Procedure,List<Block>>();
BinaryExpression binExpr = expr as BinaryExpression;
```

### Methods
Try to keep methods small enough that they can fit in a page. It is harder to understand larger methods. In particular, 
avoid deeply nested statements; refactor them into smaller methods instead.

Resist the temptation to add comments to a large method to clarify it. The need for those comments suggests 
that you might need to break the method apart into smaller smaller methods; use the comments as the names for the 
smaller methods:

```C#
void LargeMethod()
{
	// INitialize the state
	/* Large amounts of code elided for clarity */

	// Process input
	/* Large amounts of code elided for clarity */

	// Validate state
	/* Large amounts of code elided for clarity */

	// Clean up
	/* Large amounts of code elided for clarity */
}
```
can be refactored to become:
```C#
void LargeMethod()
{
	InitializeState();
	ProcessInput();
	ValidateState();
	CleanUp();
}
```

Prefer "quick" exits from methods using `return` over nested if statements:
```C#
int Method(int arg1, float arg2)
{
	if (arg1 < 0)
		return 0;
	if (arg2 < 0.0F)
		return 1;
	//...
}

// Please don't:
int Method(int arg1, float arg2)
{
	if (arg1 < 0)
	{
		return 0;
	}
	else
	{
		if (arg2 < 0.0F)
		{
			return 1;
		}
	}
	//...
}
```

### Comments
Comments are tricky. We want them to explain the code without pointlessly duplicating the code itself.
Comments are not checked by the compiler, and they can grow stale when the code they are commenting is
changed but the comments themselves are not. Making an effort to use clear variable and function names
helps in this regard. Well chosen names alleviate the need for comments.

Prefer writing comments on "why", not "what".
```C#
int x = 1;				// Lua arrays start at offset 1.

// Please don't:
int x = 1;				// Set x to 1
```

Clarifying subtle details in algorithms with a comment is encouraged. However, if you wish to document
that the code is making assumptions about the current state of the program, consider using Debug.Assert
or throwing an `InvalidOperationException` instead:
```C#
Debug.Assert(x != null, "x should be initialized before starting the frobulation process");
if (x != null)
	throw new InvalidOperationException("x should have been initialized before starting the frobulation process.")

// Please don't

// x should be initialized here.
```

See here for more examples: [Putting comments in code: the good, the bad, and the ugly](https://medium.freecodecamp.org/code-comments-the-good-the-bad-and-the-ugly-be9cc65fbf83)
