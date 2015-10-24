// BENCHFN.c
// Generated on 2015-08-20 15:08:17 by decompiling D:\dev\uxmal\reko\master\subjects\MZ-x86\BENCHFN.EXE
// using Decompiler version 0.5.1.0.

#include "BENCHFN.h"

void fn0800_0000()
{
	sp = fp;
	dx = 2523;
	Mem0[0x0800:0x01F8:word16] = 2523;
	ah = 0x30;
	al = msdos_get_dos_version(out ah);
	bp = Mem0[ds:0x02:word16];
	bx = Mem0[ds:44:word16];
	ds = 2523;
	Mem0[2523:0x92:word16] = ax;
	Mem0[2523:144:word16] = es;
	Mem0[2523:0x8C:word16] = bx;
	Mem0[2523:0xAC:word16] = bp;
	Mem0[2523:0x96:word16] = 0xFFFF;
	fn0800_0162();
	es_di = Mem0[2523:0x8A:segptr32];
	ax = di;
	bx = ax;
	cx = 0x7FFF;
	do
	{
		SCZO = cond(Mem0[es:di + 0x00:word16] - 0x3738);
		if (Test(EQ,Z))
		{
			dx = Mem0[es:di + 0x02:word16];
			SCZO = cond(dl - 0x3D);
			if (Test(EQ,Z))
			{
				dh = dh & 223;
				v20 = Mem0[2523:0x96:word16] + 0x01;
				Mem0[2523:0x96:word16] = v20;
				SCZO = cond(dh - 0x59);
				if (Test(EQ,Z))
				{
					v21 = Mem0[2523:0x96:word16] + 0x01;
					Mem0[2523:0x96:word16] = v21;
					SZO = cond(v21);
					goto l0800_0059;
				}
			}
		}
l0800_0059:
		do
		{
			if (cx == 0x00)
				break;
			SCZO = cond(al - Mem0[es:di + 0x00:byte]);
			di = di + 0x01;
			cx = cx - 0x01;
		} while (Test(NE,Z));
		if (cx == 0x00)
			goto l0800_00BE;
		bx = bx + 0x01;
		SCZO = cond(Mem0[es:di + 0x00:byte] - al);
	} while (Test(NE,Z));
	ch = ch | 0x80;
	cx = -cx;
	Mem0[2523:0x8A:word16] = cx;
	cx = 0x01;
	bx = bx << cl;
	bx = bx + 0x08;
	bx = bx & ~0x07;
	Mem0[2523:0x8E:word16] = bx;
	dx = 2523;
	bp = bp - 2523;
	di = Mem0[2523:0x023C:word16];
	SCZO = cond(di - 0x0200);
	if (Test(ULT,C))
	{
		di = 0x0200;
		Mem0[2523:0x023C:word16] = 0x0200;
	}
	di = di + 0x062E;
	SCZO = cond(di);
	if (Test(UGE,C))
	{
		di = di + Mem0[2523:0x023A:word16];
		SCZO = cond(di);
		if (Test(UGE,C))
		{
			cl = 0x04;
			di = di >>u 0x04;
			di = di + 0x01;
			SCZO = cond(bp - di);
			if (Test(UGE,C))
			{
				SCZO = cond(Mem0[2523:0x023C:word16] - 0x00);
				if (Test(NE,Z))
				{
					SCZO = cond(Mem0[2523:0x023A:word16] - 0x00);
					if (Test(EQ,Z))
					{
l0800_00B3:
						di = 0x1000;
						SCZO = cond(bp - 0x1000);
						if (Test(ULE,CZ))
							di = bp;
					}
				}
				else
					goto l0800_00B3;
				bx = di;
				bx = bx + 2523;
				Mem0[2523:0xA4:word16] = bx;
				Mem0[2523:0xA8:word16] = bx;
				ax = Mem0[2523:144:word16];
				bx = bx - ax;
				es = ax;
				ah = 0x4A;
				sp = fp - 0x02;
				wLoc02 = di;
				C = msdos_resize_memory_block(es, bx, out bx);
				di = wLoc02;
				di = di << 0x04;
				__cli();
				ss = 2523;
				sp = di;
				ax = 0x00;
				es = Mem0[0x0800:0x01F8:selector];
				di = 1512;
				cx = 0x062E;
				cx = 0x46;
				SCZO = cond(0x46);
				while (cx != 0x00)
				{
					Mem0[es:di + 0x00:byte] = al;
					di = di + 0x01;
					cx = cx - 0x01;
				}
				sp = sp - 0x02;
				SEQ(cs, Mem0[ds:0x05DA:word16])();
				fn0800_0336();
				fn0800_0421();
				ah = 0x00;
				cx = bios_get_system_time(out dx);
				Mem0[ds:0x98:word16] = dx;
				Mem0[ds:0x9A:word16] = cx;
				SEQ(cs, Mem0[ds:0x05DE:word16])();
				sp = sp - 0x02;
				Mem0[2523:sp + 0x00:word16] = Mem0[ds:0x88:word16];
				sp = sp - 0x02;
				Mem0[2523:sp + 0x00:word16] = Mem0[ds:0x86:word16];
				sp = sp - 0x02;
				Mem0[2523:sp + 0x00:word16] = Mem0[ds:0x84:word16];
				fn0800_0265();
				sp = sp - 0x02;
				Mem0[2523:sp + 0x00:word16] = ax;
				fn0800_0301();
				fn0800_0121();
				return;
			}
			else
			{
l0800_00BE:
				cx = 0x1E;
				dx = 0x56;
				ds = Mem0[0x0800:0x01F8:selector];
				fn0800_01DA();
				ax = 0x03;
				sp = fp - 0x02;
				wLoc02 = 0x03;
				fn0800_0121();
				v27 = Mem0[ds:bx + si:byte] + 0x03;
				Mem0[ds:bx + si:byte] = v27;
				SCZO = cond(v27);
				sp = fp - 0x04;
				wLoc04 = bp;
				bp = wLoc04;
				sp = fp - 0x02;
				return;
			}
		}
		else
			goto l0800_00BE;
	}
	else
		goto l0800_00BE;
}

void fn0800_0121()
{
	sp = fp;
	ds = Mem0[0x0800:0x01F8:selector];
	fn0800_01A5();
	sp = fp - 0x02;
	SEQ(cs, Mem0[ds:1500:word16])();
	ax = 0x00;
	SZO = cond(0x00);
	C = false;
	si = 0x00;
	cx = 0x2F;
	D = false;
	do
	{
		al = al + Mem0[ds:si + 0x00:byte];
		SCZO = cond(al);
		ah = ah + 0x00 + C;
		SCZO = cond(ah);
		si = si + 0x01;
		SZO = cond(si);
		cx = cx - 0x01;
	} while (cx != 0x00);
	ax = ax - 3383;
	SCZO = cond(ax);
	if (Test(NE,Z))
	{
		cx = 0x19;
		dx = 0x2F;
		fn0800_01DA();
	}
	bp = fp - 0x02;
	ah = 0x4C;
	al = bArg00;
	msdos_terminate(al);
}

void fn0800_0162()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = ds;
	es_bx = msdos_get_interrupt_vector(al);
	Mem0[ds:116:word16] = bx;
	Mem0[ds:118:word16] = es;
	es_bx = msdos_get_interrupt_vector(al);
	Mem0[ds:0x78:word16] = bx;
	Mem0[ds:122:word16] = es;
	es_bx = msdos_get_interrupt_vector(al);
	Mem0[ds:0x7C:word16] = bx;
	Mem0[ds:0x7E:word16] = es;
	es_bx = msdos_get_interrupt_vector(al);
	Mem0[ds:0x80:word16] = bx;
	Mem0[ds:0x82:word16] = es;
	ax = 0x2500;
	dx = cs;
	ds = dx;
	dx = 344;
	msdos_set_interrupt_vector(al, ds_dx);
	ds = wLoc02;
	sp = fp;
	return;
}

void fn0800_01A5()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = ds;
	ds_dx = Mem0[ds:116:segptr32];
	msdos_set_interrupt_vector(al, ds_dx);
	ds = wLoc02;
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = ds;
	ds_dx = Mem0[ds:0x78:segptr32];
	msdos_set_interrupt_vector(al, ds_dx);
	ds = wLoc02;
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = ds;
	ds_dx = Mem0[ds:0x7C:segptr32];
	msdos_set_interrupt_vector(al, ds_dx);
	ds = wLoc02;
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = ds;
	ax = 0x2506;
	ds_dx = Mem0[ds:0x80:segptr32];
	msdos_set_interrupt_vector(al, ds_dx);
	ds = wLoc02;
	sp = fp;
	return;
}

void fn0800_01DA()
{
	sp = fp;
	ah = 0x40;
	bx = 0x02;
	C = msdos_write_file(0x02, cx, ds_dx, out ax);
	return;
}

void fn0800_01FA()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_01FF()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	fn0800_01FA();
	fn0800_01FA();
	fn0800_01FA();
	fn0800_01FA();
	fn0800_01FA();
	fn0800_01FA();
	fn0800_01FA();
	fn0800_01FA();
	fn0800_01FA();
	fn0800_01FA();
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_0222()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	fn0800_01FF();
	fn0800_01FF();
	fn0800_01FF();
	fn0800_01FF();
	fn0800_01FF();
	fn0800_01FF();
	fn0800_01FF();
	fn0800_01FF();
	fn0800_01FF();
	fn0800_01FF();
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_0245()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	fn0800_0222();
	fn0800_0222();
	fn0800_0222();
	fn0800_0222();
	fn0800_0222();
	fn0800_0222();
	fn0800_0222();
	fn0800_0222();
	fn0800_0222();
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_0265()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	sp = fp - 0x0A;
	SCZO = cond(fp - 0x0A);
	ax = 404;
	sp = fp - 0x0C;
	wLoc0C = 404;
	fn0800_0E4B();
	cx = wLoc0C;
	sp = fp - 0x0A;
	ax = fp - 0x06;
	sp = fp - 0x0C;
	wLoc0C = fp - 0x06;
	ax = 0x01B0;
	sp = fp - 0x0E;
	wLoc0E = 0x01B0;
	fn0800_16D4();
	cx = wLoc0E;
	sp = fp - 0x0C;
	cx = wLoc0C;
	sp = fp - 0x0A;
	sp = fp - 0x0C;
	wLoc0C = wLoc04;
	sp = fp - 0x0E;
	wLoc0E = wLoc06;
	ax = 0x01B4;
	sp = fp - 0x10;
	wLoc10 = 0x01B4;
	fn0800_0E4B();
	sp = fp - 0x0A;
	SCZO = cond(fp - 0x0A);
	wLoc08 = 0x00;
	wLoc0A = 0x01;
	while (true)
	{
		dx = wLoc08;
		ax = wLoc0A;
		SCZO = cond(dx - wLoc04);
		if (Test(GE,SO))
		{
			if (Test(GT,SZO))
			{
				ax = 0x01CE;
				sp = fp - 0x0C;
				wLoc0C = 0x01CE;
				fn0800_0E4B();
				cx = wLoc0C;
				sp = fp - 0x02;
				bp = wLoc02;
				sp = fp;
				return;
			}
			SCZO = cond(ax - wLoc06);
			if (Test(UGT,CZ))
				;
		}
l0800_029C:
		fn0800_0245();
		v12 = wLoc0A + 0x01;
		wLoc0A = v12;
		SCZO = cond(v12);
		v14 = wLoc08 + 0x00 + C;
		wLoc08 = v14;
		SCZO = cond(v14);
	}
}

void fn0800_0301()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	ax = Mem0[ds:0x023E:word16];
	v7 = Mem0[ds:0x023E:word16] - 0x01;
	Mem0[ds:0x023E:word16] = v7;
	ax = ax | ax;
	SZO = cond(ax);
	C = false;
	while (Test(NE,Z))
	{
		bx = Mem0[ds:0x023E:word16];
		bx = bx << 0x01;
		SCZO = cond(bx);
		SEQ(cs, Mem0[ds:bx + 1512:word16])();
		ax = Mem0[ds:0x023E:word16];
		v7 = Mem0[ds:0x023E:word16] - 0x01;
		Mem0[ds:0x023E:word16] = v7;
		ax = ax | ax;
		SZO = cond(ax);
		C = false;
	}
	SEQ(cs, Mem0[ds:0x0234:word16])();
	SEQ(cs, Mem0[ds:566:word16])();
	SEQ(cs, Mem0[ds:0x0238:word16])();
	sp = fp - 0x04;
	wLoc04 = wArg02;
	fn0800_0121();
	cx = wLoc04;
	sp = fp - 0x02;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_0336()
{
	sp = fp;
	Mem0[0x0800:0x0330:word16] = wArg00;
	sp = fp + 0x02;
	Mem0[0x0800:0x0332:word16] = ds;
	D = false;
	es = Mem0[ds:144:selector];
	si = 0x80;
	ah = 0x00;
	al = Mem0[ds:0x80:byte];
	si = 0x81;
	ax = ax + 0x01;
	bp = es;
	v14 = 0x81;
	dx = 0x81;
	v16 = ax;
	ax = bx;
	bx = v16;
	si = Mem0[ds:0x8A:word16];
	si = si + 0x02;
	cx = 0x01;
	SCZO = cond(Mem0[ds:0x92:byte] - 0x03);
	if (Test(UGE,C))
	{
		es = Mem0[ds:0x8C:selector];
		di = si;
		cl = 0x7F;
		al = 0x00;
		SZO = cond(0x00);
		C = false;
		do
		{
			if (cx == 0x00)
				break;
			SCZO = cond(al - Mem0[es:di + 0x00:byte]);
			di = di + 0x01;
			cx = cx - 0x01;
		} while (Test(NE,Z));
		if (cx != 0x00)
		{
			cl = cl ^ 0x7F;
			SZO = cond(cl);
			C = false;
l0800_0374:
			sp = fp;
			ax = 0x01;
			ax = 0x01 + bx;
			ax = ax + cx;
			ax = ax & ~0x01;
			di = fp;
			di = fp - (ax & ~0x01);
			SCZO = cond(di);
			if (Test(UGE,C))
			{
				sp = di;
				ax = es;
				ds = ax;
				ax = ss;
				es = ax;
				sp = sp - 0x02;
				Mem0[ss:sp + 0x00:word16] = cx;
				cx = cx - 0x01;
				SZO = cond(cx);
				while (cx != 0x00)
				{
					v23 = Mem0[ds:si + 0x00:byte];
					Mem0[es:di + 0x00:byte] = v23;
					si = si + 0x01;
					di = di + 0x01;
					cx = cx - 0x01;
				}
				al = 0x00;
				C = false;
				Mem0[es:di + 0x00:byte] = 0x00;
				di = di + 0x01;
				ds = bp;
				v24 = 0x81;
				si = 0x81;
				v25 = cx;
				cx = bx;
				bx = v25;
				ax = bx;
				dx = ax;
				bx = bx + 0x01;
				SZO = cond(bx);
				while (true)
				{
					fn0800_03BF();
					if (Test(ULE,CZ))
						do
						{
							if (Test(ULT,C))
							{
								cx = Mem0[ss:sp + 0x00:word16];
								sp = sp + 0x02;
								cx = cx + dx;
								ds = Mem0[0x0800:0x0332:selector];
								Mem0[ds:0x84:word16] = bx;
								bx = bx + 0x01;
								bx = bx + bx;
								si = sp;
								bp = sp;
								bp = bp - bx;
								SCZO = cond(bp);
								if (Test(UGE,C))
								{
									sp = bp;
									Mem0[ds:0x86:word16] = bp;
									do
									{
										if (cx == 0x00)
											break;
										Mem0[ss:bp + 0x00:word16] = si;
										bp = bp + 0x02;
										SCZO = cond(bp);
										do
										{
											al = Mem0[ds:si + 0x00:byte];
											si = si + 0x01;
											al = al | al;
											SZO = cond(al);
											C = false;
											cx = cx - 0x01;
										} while (Test(NE,Z) && cx != 0x00);
									} while (Test(EQ,Z));
									ax = 0x00;
									SZO = cond(0x00);
									C = false;
									Mem0[ss:bp + 0x00:word16] = 0x00;
									Mem0[0x0800:0x0330:word16]();
									return;
								}
								else
									goto l0800_03E7;
							}
							fn0800_03BF();
						} while (Test(UGT,CZ));
l0800_03AF:
					SCZO = cond(al - 0x20);
					if (Test(NE,Z))
					{
						SCZO = cond(al - 0x0D);
						if (Test(NE,Z))
						{
							SCZO = cond(al - 0x09);
							if (Test(NE,Z))
								continue;
l0800_03BB:
							al = 0x00;
							SZO = cond(0x00);
							C = false;
						}
						else
							goto l0800_03BB;
					}
					else
						goto l0800_03BB;
				}
			}
			else
			{
l0800_03E7:
				cx = 0x1E;
				dx = 0x56;
				ds = Mem0[0x0800:0x01F8:selector];
				fn0800_01DA();
				ax = 0x03;
				sp = sp - 0x02;
				Mem0[ss:sp + 0x00:word16] = 0x03;
				fn0800_0121();
				v27 = Mem0[ds:bx + si:byte] + 0x03;
				Mem0[ds:bx + si:byte] = v27;
				SCZO = cond(v27);
				sp = sp - 0x02;
				Mem0[ss:sp + 0x00:word16] = bp;
				bp = Mem0[ss:sp + 0x00:word16];
				sp = sp + 0x02;
				return;
			}
		}
		else
			goto l0800_03E7;
	}
	else
		goto l0800_0374;
fn0800_0336_exit:
}

void fn0800_03BF()
{
	sp = fp;
	ax = ax | ax;
	SZO = cond(ax);
	C = false;
	if (Test(NE,Z))
	{
		dx = dx + 0x01;
		Mem0[es:di + 0x00:byte] = al;
		di = di + 0x01;
		al = al | al;
		SZO = cond(al);
		C = false;
		if (Test(EQ,Z))
		{
			bx = bx + 0x01;
			SZO = cond(bx);
		}
	}
	v12 = al;
	al = ah;
	ah = v12;
	al = 0x00;
	SZO = cond(0x00);
	C = true;
	if (cx != 0x00)
	{
		al = Mem0[ds:si + 0x00:byte];
		si = si + 0x01;
		cx = cx - 0x01;
		al = al - 0x22;
		SCZO = cond(al);
		if (Test(NE,Z))
		{
			al = al + 0x22;
			SCZO = cond(al - 0x5C);
			if (Test(EQ,Z))
			{
				SCZO = cond(Mem0[ds:si + 0x00:byte] - 0x22);
				if (Test(EQ,Z))
				{
					al = Mem0[ds:si + 0x00:byte];
					si = si + 0x01;
					cx = cx - 0x01;
					SZO = cond(cx);
				}
			}
			si = si | si;
			SZO = cond(si);
			C = false;
		}
	}
	return;
}

void fn0800_0421()
{
	sp = fp;
	cx = Mem0[ds:0x8A:word16];
	sp = fp - 0x02;
	wLoc02 = cx;
	fn0800_0570();
	cx = wLoc02;
	sp = fp;
	di = ax;
	ax = ax | ax;
	SZO = cond(ax);
	C = false;
	if (Test(NE,Z))
	{
		sp = fp - 0x02;
		wLoc02 = ds;
		sp = fp - 0x04;
		wLoc04 = ds;
		es = wLoc04;
		sp = fp - 0x02;
		ds = Mem0[ds:0x8C:selector];
		si = 0x00;
		SZO = cond(0x00);
		C = false;
		D = false;
		while (cx != 0x00)
		{
			v14 = Mem0[ds:si + 0x00:byte];
			Mem0[es:di + 0x00:byte] = v14;
			si = si + 0x01;
			di = di + 0x01;
			cx = cx - 0x01;
		}
		ds = wLoc02;
		sp = fp;
		di = ax;
		sp = fp - 0x02;
		wLoc02 = es;
		sp = fp - 0x04;
		wLoc04 = Mem0[ds:0x8E:word16];
		fn0800_0570();
		sp = fp - 0x02;
		bx = ax;
		es = wLoc02;
		sp = fp;
		Mem0[ds:0x88:word16] = ax;
		ax = ax | ax;
		SZO = cond(ax);
		C = false;
		if (Test(EQ,Z))
l0800_0454:
	}
}

void fn0800_0491()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	sp = fp - 0x04;
	wLoc04 = si;
	sp = fp - 0x06;
	wLoc06 = di;
	di = wArg02;
	ax = Mem0[ds:di + 0x06:word16];
	Mem0[ds:0x062A:word16] = ax;
	SCZO = cond(ax - di);
	if (Test(EQ,Z))
		Mem0[ds:0x062A:word16] = 0x00;
	else
	{
		si = Mem0[ds:di + 0x04:word16];
		bx = Mem0[ds:0x062A:word16];
		Mem0[ds:bx + 0x04:word16] = si;
		ax = Mem0[ds:0x062A:word16];
		Mem0[ds:si + 0x06:word16] = ax;
	}
	di = wLoc06;
	sp = fp - 0x04;
	si = wLoc04;
	sp = fp - 0x02;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_04BF()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	sp = fp - 0x04;
	wLoc04 = si;
	sp = fp - 0x06;
	wLoc06 = di;
	di = wArg02;
	ax = wArg04;
	v9 = Mem0[ds:di + 0x00:word16] - ax;
	Mem0[ds:di + 0x00:word16] = v9;
	si = Mem0[ds:di + 0x00:word16];
	si = si + di;
	ax = wArg04;
	ax = ax + 0x01;
	Mem0[ds:si + 0x00:word16] = ax;
	Mem0[ds:si + 0x02:word16] = di;
	ax = Mem0[ds:0x0628:word16];
	SCZO = cond(ax - di);
	if (Test(EQ,Z))
		Mem0[ds:0x0628:word16] = si;
	else
	{
		di = si;
		di = di + wArg04;
		SCZO = cond(di);
		Mem0[ds:di + 0x02:word16] = si;
	}
	ax = si;
	ax = ax + 0x04;
	SCZO = cond(ax);
	di = wLoc06;
	sp = fp - 0x04;
	si = wLoc04;
	sp = fp - 0x02;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_04F9()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	sp = fp - 0x04;
	wLoc04 = si;
	ax = wArg02;
	dx = 0x00;
	ax = ax & 0xFFFF;
	dx = 0x00;
	SZO = cond(0x00);
	C = false;
	sp = fp - 0x06;
	wLoc06 = 0x00;
	sp = fp - 0x08;
	wLoc08 = ax;
	fn0800_0607();
	cx = wLoc08;
	sp = fp - 0x06;
	cx = wLoc06;
	sp = fp - 0x04;
	si = ax;
	SCZO = cond(si - 0xFFFF);
	if (Test(EQ,Z))
	{
		ax = 0x00;
		SZO = cond(0x00);
		C = false;
	}
	else
	{
		ax = Mem0[ds:0x0628:word16];
		Mem0[ds:si + 0x02:word16] = ax;
		ax = wArg02;
		ax = ax + 0x01;
		Mem0[ds:si + 0x00:word16] = ax;
		Mem0[ds:0x0628:word16] = si;
		ax = Mem0[ds:0x0628:word16];
		ax = ax + 0x04;
		SCZO = cond(ax);
	}
	si = wLoc04;
	sp = fp - 0x02;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_0536()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	sp = fp - 0x04;
	wLoc04 = si;
	ax = wArg02;
	dx = 0x00;
	ax = ax & 0xFFFF;
	dx = 0x00;
	SZO = cond(0x00);
	C = false;
	sp = fp - 0x06;
	wLoc06 = 0x00;
	sp = fp - 0x08;
	wLoc08 = ax;
	fn0800_0607();
	cx = wLoc08;
	sp = fp - 0x06;
	cx = wLoc06;
	sp = fp - 0x04;
	si = ax;
	SCZO = cond(si - 0xFFFF);
	if (Test(EQ,Z))
	{
		ax = 0x00;
		SZO = cond(0x00);
		C = false;
	}
	else
	{
		Mem0[ds:0x062C:word16] = si;
		Mem0[ds:0x0628:word16] = si;
		ax = wArg02;
		ax = ax + 0x01;
		Mem0[ds:si + 0x00:word16] = ax;
		ax = si;
		ax = ax + 0x04;
		SCZO = cond(ax);
	}
	si = wLoc04;
	sp = fp - 0x02;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_0570()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	sp = fp - 0x04;
	wLoc04 = si;
	sp = fp - 0x06;
	wLoc06 = di;
	di = wArg02;
	di = di | di;
	SZO = cond(di);
	C = false;
	if (Test(NE,Z))
	{
		SCZO = cond(di - ~0x0B);
		if (Test(UGT,CZ))
		{
l0800_0581:
			ax = 0x00;
			SZO = cond(0x00);
			C = false;
		}
		else
		{
			ax = di;
			ax = ax + 0x0B;
			ax = ax & ~0x07;
			di = ax;
			SCZO = cond(Mem0[ds:0x062C:word16] - 0x00);
			if (Test(EQ,Z))
			{
				sp = fp - 0x08;
				wLoc08 = di;
				fn0800_0536();
				cx = wLoc08;
				sp = fp - 0x06;
			}
			else
			{
				si = Mem0[ds:0x062A:word16];
				ax = si;
				ax = ax | ax;
				SZO = cond(ax);
				C = false;
				if (Test(NE,Z))
				{
l0800_05A7:
					do
					{
						ax = Mem0[ds:si + 0x00:word16];
						dx = di;
						dx = dx + 0x28;
						SCZO = cond(ax - dx);
						if (Test(UGE,C))
						{
							sp = fp - 0x08;
							wLoc08 = di;
							sp = fp - 0x0A;
							wLoc0A = si;
							fn0800_04BF();
							cx = wLoc0A;
							sp = fp - 0x08;
							cx = wLoc08;
							sp = fp - 0x06;
							goto l0800_05DF;
						}
						ax = Mem0[ds:si + 0x00:word16];
						SCZO = cond(ax - di);
						if (Test(UGE,C))
						{
							sp = fp - 0x08;
							wLoc08 = si;
							fn0800_0491();
							cx = wLoc08;
							sp = fp - 0x06;
							v16 = Mem0[ds:si + 0x00:word16] + 0x01;
							Mem0[ds:si + 0x00:word16] = v16;
							ax = si;
							ax = ax + 0x04;
							SCZO = cond(ax);
							goto l0800_05DF;
						}
						si = Mem0[ds:si + 0x06:word16];
						SCZO = cond(si - Mem0[ds:0x062A:word16]);
					} while (Test(NE,Z));
l0800_05D8:
					sp = fp - 0x08;
					wLoc08 = di;
					fn0800_04F9();
					cx = wLoc08;
					sp = fp - 0x06;
					sp = fp - 0x08;
					wLoc08 = di;
					fn0800_04F9();
					cx = wLoc08;
					sp = fp - 0x06;
				}
				else
					goto l0800_05D8;
			}
		}
	}
	else
		goto l0800_0581;
l0800_05DF:
	di = wLoc06;
	sp = fp - 0x04;
	si = wLoc04;
	sp = fp - 0x02;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_0607()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	ax = wArg02;
	dx = wArg04;
	ax = ax + Mem0[ds:0x9E:word16];
	SCZO = cond(ax);
	dx = dx + 0x00 + C;
	cx = ax;
	cx = cx + 0x0100;
	SCZO = cond(cx);
	dx = dx + 0x00 + C;
	dx = dx | dx;
	SZO = cond(dx);
	C = false;
	if (Test(EQ,Z))
	{
		SCZO = cond(cx - (fp - 0x02));
		if (Test(ULT,C))
		{
			v13 = Mem0[ds:0x9E:word16];
			Mem0[ds:0x9E:word16] = ax;
			ax = v13;
		}
		else
		{
l0800_062E:
			Mem0[ds:0x94:word16] = 0x08;
			ax = 0xFFFF;
		}
	}
	else
		goto l0800_062E;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_0E4B()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	ax = 3969;
	sp = fp - 0x04;
	wLoc04 = 3969;
	ax = 0x0352;
	sp = fp - 0x06;
	wLoc06 = 0x0352;
	sp = fp - 0x08;
	wLoc08 = wArg02;
	ax = fp + 0x04;
	sp = fp - 0x0A;
	wLoc0A = fp + 0x04;
	fn0800_1073();
	sp = fp - 0x02;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_1073()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	sp = fp - 0x9A;
	SCZO = cond(fp - 0x9A);
	sp = fp - 0x9C;
	wLoc9C = si;
	sp = fp - 0x9E;
	wLoc9E = di;
	wLoc5A = 0x00;
	bLoc57 = 0x50;
	wLoc04 = 0x00;
	sp = fp - 0xA0;
	wLocA0 = es;
	D = false;
	di = fp - 0x56;
	wLoc98 = fp - 0x56;
	di = wLoc98;
	si = wArg04;
	al = Mem0[ds:si + 0x00:byte];
	si = si + 0x01;
	al = al | al;
	SZO = cond(al);
	C = false;
	while (Test(NE,Z))
	{
		SCZO = cond(al - 0x25);
		if (Test(NE,Z))
			goto l0800_10E6;
		wLoc8C = si;
		al = Mem0[ds:si + 0x00:byte];
		si = si + 0x01;
		SCZO = cond(al - 0x25);
		if (Test(NE,Z))
		{
			wLoc98 = di;
			cx = 0x00;
			SZO = cond(0x00);
			C = false;
			wLoc8E = 0x00;
			wLoc9A = 0x00;
			bLoc8F = cl;
			wLoc94 = 0xFFFF;
			wLoc92 = 0xFFFF;
			ah = 0x00;
			dx = ax;
			bx = ax;
			bl = bl - 0x20;
			SCZO = cond(bl - 0x60);
			if (Test(ULT,C))
			{
				bl = Mem0[ds:bx + 0x04F9:byte];
				ax = bx;
				SCZO = cond(ax - 0x17);
				if (Test(ULE,CZ))
				{
					bx = ax;
					bx = bx << 0x01;
					SCZO = cond(bx);
				}
			}
			si = wLoc8C;
			di = wLoc98;
			al = 0x25;
			do
			{
				fn0800_1099();
				sp = sp + ~0x01;
				al = Mem0[ds:si + 0x00:byte];
				si = si + 0x01;
				al = al | al;
				SZO = cond(al);
				C = false;
			} while (Test(NE,Z));
		}
		Mem0[ds:di + 0x00:byte] = al;
		di = di + 0x01;
		v15 = bLoc57 - 0x01;
		bLoc57 = v15;
		SZO = cond(v15);
		if (Test(GT,SZO))
			continue;
		fn0800_10A1();
		al = Mem0[ds:si + 0x00:byte];
		si = si + 0x01;
		al = al | al;
		SZO = cond(al);
		C = false;
	}
l0800_1572:
	SCZO = cond(bLoc57 - 0x50);
	if (Test(LT,SO))
	{
		fn0800_10A1();
		fn0800_10A1();
	}
	es = Mem0[ss:sp + 0x00:selector];
	sp = sp + 0x02;
	SCZO = cond(0x00);
	if (Test(NE,Z))
		ax = 0xFFFF;
	else
		ax = 0x00;
	di = Mem0[ss:sp + 0x00:word16];
	sp = sp + 0x02;
	si = Mem0[ss:sp + 0x00:word16];
	sp = fp - 0x02;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_1099()
{
	sp = fp;
	Mem0[ds:di + 0x00:byte] = al;
	di = di + 0x01;
	v9 = Mem0[ss:bp - 0x55:byte] - 0x01;
	Mem0[ss:bp - 0x55:byte] = v9;
	SZO = cond(v9);
	if (Test(GT,SZO))
		fn0800_10A1();
}

void fn0800_10A1()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bx;
	sp = fp - 0x04;
	wLoc04 = cx;
	sp = fp - 0x06;
	wLoc06 = dx;
	sp = fp - 0x08;
	wLoc08 = es;
	ax = bp - 0x54;
	di = di - ax;
	SCZO = cond(di);
	ax = bp - 0x54;
	sp = fp - 0x0A;
	wLoc0A = ax;
	sp = fp - 0x0C;
	wLoc0C = di;
	sp = fp - 0x0E;
	wLoc0E = Mem0[ss:bp + 0x08:word16];
	SEQ(cs, Mem0[ss:bp + 0x0A:word16])();
	ax = ax | ax;
	SZO = cond(ax);
	C = false;
	if (Test(EQ,Z))
		Mem0[ss:bp - 0x02:word16] = 0x01;
	Mem0[ss:bp - 0x55:byte] = 0x50;
	v16 = Mem0[ss:bp - 88:word16] + di;
	Mem0[ss:bp - 88:word16] = v16;
	SCZO = cond(v16);
	di = bp - 0x54;
	es = wLoc0E;
	sp = fp - 0x0C;
	dx = wLoc0C;
	sp = fp - 0x0A;
	cx = wLoc0A;
	sp = fp - 0x08;
	bx = wLoc08;
	sp = fp - 0x06;
	return;
}

void fn0800_16D4()
{
	sp = fp;
	sp = fp - 0x02;
	wLoc02 = bp;
	bp = fp - 0x02;
	ax = fp + 0x04;
	sp = fp - 0x04;
	wLoc04 = fp + 0x04;
	sp = fp - 0x06;
	wLoc06 = wArg02;
	ax = 0x0342;
	sp = fp - 0x08;
	wLoc08 = 0x0342;
	ax = 7525;
	sp = fp - 0x0A;
	wLoc0A = 7525;
	ax = 0x07F2;
	sp = fp - 0x0C;
	wLoc0C = 0x07F2;
	fn0800_16F3();
	sp = fp - 0x02;
	bp = wLoc02;
	sp = fp;
	return;
}

void fn0800_16F3()
{
fn0800_16F3_entry:
l0800_16F3:
	sp = fp
	sp = fp - 0x02
	wLoc02 = bp
	bp = fp - 0x02
	sp = fp - 44
	SCZO = cond(fp - 44)
	sp = fp - 0x2E
	wLoc2E = si
	sp = fp - 0x30
	wLoc30 = di
	wLoc2A = 0x00
	wLoc28 = 0x00
l0800_1721:
	sp = fp - 0x32
	wLoc32 = es
	D = false
	si = wArg08
l0800_1726:
	al = Mem0[ds:si + 0x00:byte]
	si = si + 0x01
	al = al | al
	SZO = cond(al)
	C = false
	branch Test(EQ,Z) l0800_1788
l0800_172B:
	SCZO = cond(al - 0x25)
	branch Test(EQ,Z) l0800_178B
l0800_172F:
	ax = (int16) al
	v16 = ax
	ax = di
	di = v16
	v17 = wLoc28 + 0x01
	wLoc28 = v17
	SZO = cond(v17)
	sp = fp - 0x34
	wLoc34 = wArg06
	call SEQ(cs, Mem0[ss:bp + 0x04:word16]) (retsize: 2; depth: 54)
	cx = wLoc34
	sp = fp - 0x32
	ax = ax | ax
	SZO = cond(ax)
	C = false
	branch Test(LT,SO) l0800_1764
l0800_173F:
	di = di | di
	SZO = cond(di)
	C = false
	branch Test(SG,S) l0800_1775
l0800_1743:
	SCZO = cond(Mem0[ds:di + 0x055A:byte] - 0x01)
	branch Test(NE,Z) l0800_1775
l0800_174A:
	v22 = ax
	ax = bx
	bx = v22
	bl = bl | bl
	SZO = cond(bl)
	C = false
	branch Test(SG,S) l0800_1767
l0800_174F:
	SCZO = cond(Mem0[ds:bx + 0x055A:byte] - 0x01)
	branch Test(NE,Z) l0800_1767
l0800_1756:
	v25 = wLoc28 + 0x01
	wLoc28 = v25
	SZO = cond(v25)
	sp = fp - 0x34
	wLoc34 = wArg06
	call SEQ(cs, Mem0[ss:bp + 0x04:word16]) (retsize: 2; depth: 54)
	cx = wLoc34
	sp = fp - 0x32
	ax = ax | ax
	SZO = cond(ax)
	C = false
	branch Test(GT,SZO) l0800_174A
l0800_1764:
	goto l0800_1AEB
l0800_1767:
	sp = fp - 0x34
	wLoc34 = wArg06
	sp = fp - 0x36
	wLoc36 = bx
	call SEQ(cs, Mem0[ss:bp + 0x06:word16]) (retsize: 2; depth: 56)
	cx = wLoc36
	sp = fp - 0x34
	cx = wLoc34
	sp = fp - 0x32
	v27 = wLoc28 - 0x01
	wLoc28 = v27
	SZO = cond(v27)
	goto l0800_1726
l0800_1775:
	SCZO = cond(ax - di)
	branch Test(EQ,Z) l0800_1726
l0800_1779:
	sp = fp - 0x34
	wLoc34 = wArg06
	sp = fp - 0x36
	wLoc36 = ax
	call SEQ(cs, Mem0[ss:bp + 0x06:word16]) (retsize: 2; depth: 56)
	cx = wLoc36
	sp = fp - 0x34
	cx = wLoc34
	sp = fp - 0x32
	v28 = wLoc28 - 0x01
	wLoc28 = v28
	SZO = cond(v28)
	goto l0800_1AFF
l0800_1788:
	goto l0800_1AFF
l0800_178B:
	wLoc24 = 0xFFFF
	bLoc2B = 0x00
	al = Mem0[ds:si + 0x00:byte]
	si = si + 0x01
	ax = (int16) al
	wArg08 = si
	v29 = ax
	ax = di
	di = v29
	di = di | di
	SZO = cond(di)
	C = false
	branch Test(LT,SO) l0800_17E6
l0800_179E:
	bl = Mem0[ds:di + 0x055A:byte]
	bh = 0x00
	ax = bx
	SCZO = cond(ax - 0x15)
	branch Test(ULE,CZ) l0800_17AE
l0800_17AB:
	goto l0800_1AEB
l0800_17AE:
	bx = ax
	bx = bx << 0x01
	SCZO = cond(bx)
l0800_17E6:
	goto l0800_1AFF
l0800_1AEB:
	sp = fp - 0x34
	wLoc34 = wArg06
	ax = 0xFFFF
	sp = fp - 0x36
	wLoc36 = 0xFFFF
	call SEQ(cs, Mem0[ss:bp + 0x06:word16]) (retsize: 2; depth: 56)
	cx = wLoc36
	sp = fp - 0x34
	cx = wLoc34
	sp = fp - 0x32
	SCZO = cond(0xFFFF)
	v26 = 0x00 - C
	Mem0[ss:bp - 0x28:word16] = v26
	SCZO = cond(v26)
l0800_1AFF:
	es = wLoc32
	sp = fp - 0x30
	ax = wLoc2A
l0800_1B8C:
	di = wLoc30
	sp = fp - 0x2E
	si = wLoc2E
	sp = fp - 0x02
	bp = wLoc02
	sp = fp
	return
fn0800_16F3_exit:
}

