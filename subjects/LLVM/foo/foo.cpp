#include <stdio.h>

class number
{
public:
	number();

	number negate();
	number complement();
public:
	int field1;
	int field2;
};

struct node
{
	node * next;
	int value; 
};

enum VARIANT_T
{
	VT_UNKNOWN,
	VT_INT,
	VT_STR,
	VT_PFN
};

struct variant
{
	int type;
	union {
		int i;
		char * s;
		void (*pfn)();
	};
};
const char * switch_compact(int n)
{
	switch (n)
	{
	case 0: return "zero";
	case 1: return "one";
	case 2: return "two";
	case 3: return "three";
	case 4: return "four";
	case 5: return "five";
	case 6: return "six";
	default: return "many";
	}
}

const char * switch_sparse(int n)
{
	switch (n)
	{
	case 1: return "ett";
	case 10: return "tio";
	case 100: return "hundra";
	case 1000: return "tusen";
	case 1000000: return "miljoner";
	case 1000000000: return "miljarder";
	default: return "otal";
	}
}

node * find_node(node * n, int v)
{
	while (n != nullptr && n->value != v)
	{
		n = n->next;
	}
	return n;
}

number apply_member_pointer(number(number::*pmfn)(), number n)
{
	return (n.*pmfn)();
}

void update_member_field(int number::* field, number n)
{
	(n.*field)++;
}

// Stack overruns, memory leaks? lol.
const variant & print(const variant & a)
{
	switch (a.type)
	{
		case VT_INT:
			printf("%d", a.i);
			break;
		case VT_STR:
			printf("%s", a.s);
			break;
		case VT_PFN:
			printf("fn %p", a.pfn);
			break;
	}
	fprintf(stderr, "Not supported yet.\n");
	return a;
}