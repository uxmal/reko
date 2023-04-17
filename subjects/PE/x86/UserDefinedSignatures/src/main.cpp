
#include <stdio.h>
#include <stdlib.h>
#include <string.h>


typedef struct data_struct data_struct;

struct data_struct
{
    int i32;
    float r32;
    double r64;
};

static int gbl_i32;
static float gbl_r32;
static double gbl_r64;

#define INTEGER 0
#define FLOAT   1
#define DOUBLE  2


void setInteger(data_struct *d, int value)
{
	d->i32 = value;
}

void setFloat(data_struct *d, float value)
{
	d->r32 = value;
}

void setDouble(data_struct *d, double value)
{
	d->r64 = value;
}

void setParameter(data_struct *d, int type, void *value)
{
	switch (type)
	{
	case INTEGER:
		{
			int *i32 = (int *)value;
			setInteger(d, *i32);
			break;
		}
	case FLOAT:
		{
			float *r32 = (float *)value;
			setFloat(d, *r32);
			break;
		}
	case DOUBLE:
		{
			double *r64 = (double *)value;
			setDouble(d, *r64);
			break;
		}
	}
}

data_struct *new_data_struct()
{
	return new data_struct;
}


int main(int argc, char *argv[])
{
    data_struct *d = new_data_struct();
	for (int i = 1; i < (argc - 1); i++)
	{
		if (!strcmp(argv[i], "i32"))
		{
			int i32 = atoi(argv[++i]);
			setParameter(d, INTEGER, &i32);
		}
		else if (!strcmp(argv[i], "r32"))
		{
			float r32 = atof(argv[++i]);
			setParameter(d, FLOAT, &r32);
		}
		else if (!strcmp(argv[i], "r64"))
		{
			double r64 = atof(argv[++i]);
			setParameter(d, DOUBLE, &r64);
		}
	}
	*((int *)&gbl_i32) = *((int *)&d->i32);
	*((int *)&gbl_r32) = *((int *)&d->r32);
	*((long long *)&gbl_r64) = *((long long *)&d->r64);
	free(d);
	//printf("%d %f %f\n", gbl_i32, gbl_r32, gbl_r64);
}

