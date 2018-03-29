#include "stdafx.h"

#include "reko.h"
#include "functions.h"
#include "ComBase.h"
#include "ArmRewriter.h"
#include "ThumbRewriter.h"

void ThumbRewriter::RewriteIt()
{
	int i;
	itState = 0;
	for (i = 1; instr->mnemonic[i]; ++i)
	{
		itState = (itState << 1) | (instr->mnemonic[i] == 't');
	}
	int sh = 5 - i;
	itState = ((itState << 1) | 1) << sh;

	itStateCondition = instr->detail->arm.cc;
	m.Nop();
	m.FinishCluster(RtlClass::Linear, instr->address, instr->size);
}




