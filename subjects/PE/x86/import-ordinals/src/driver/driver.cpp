#define APIEXPORT

#include "driver.h"

APIENTITY MathService::MathService()
{
	m_accum = 0;
	++count;
	++globalvar;
}

int APIENTITY MathService::add(int n) {
	m_accum += n;
	return m_accum;
}

int APIENTITY MathService::sub(int n) {
	m_accum -= n;
	return m_accum;
}

int APIENTITY MathService::mul(int n) {
	m_accum *= n;
	return m_accum;
}

int APIENTITY MathFunction()
{
	return globalvar;
}

int APIENTITY MathService::count;

int APIENTITY globalvar;
