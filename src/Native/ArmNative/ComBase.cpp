/*
* Copyright (C) 1999-2024 John K�ll�n.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; either version 2, or (at your option)
* any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program; see the file COPYING.  If not, write to
* the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
*/

#include "stdafx.h"

#include "ComBase.h"
#include "functions.h"

ComBase::~ComBase()
{
}

ULONG STDMETHODCALLTYPE ComBase::AddRef()
{
	//Dump("AddRef: %08x %d", this, cRef + 1);
	return ++this->cRef;
}


ULONG STDMETHODCALLTYPE ComBase::Release()
{
	//Dump("Release: %08x %d", this, cRef - 1);
	if (--this->cRef > 0)
		return this->cRef;
	Dump("Release: %08x destroyed", this);
	//--s_count;
	delete this;
	return 0;
}
