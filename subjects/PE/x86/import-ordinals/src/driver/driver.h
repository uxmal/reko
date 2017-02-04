#pragma once

#ifndef APIENTITY
# define APIENTITY __declspec(dllimport)
#endif

class MathService
{
public:
	APIENTITY MathService();
	int APIENTITY add(int n);
	int APIENTITY sub(int n);
	int APIENTITY mul(int n);

	int total() { return m_accum; }
private:
	int m_accum;
public:
	static int APIENTITY count;
};

extern int APIENTITY globalvar;

int APIENTITY MathFunction();


