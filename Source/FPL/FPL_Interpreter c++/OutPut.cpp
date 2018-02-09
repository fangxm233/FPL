#include "stdafx.h"
#include "OutPut.h"

void OutPut::FileError(string s) 
{
	cout << "FileError" + s << endl;
	throw FileException();
}

void OutPut::RunTimeError(string s)
{
	cout << "RunTimeError" + s << endl;
	throw RunTimeException();
}

void OutPut::RunTimeWarning(string s)
{
	cout << "RunTimeWarning" + s << endl;
}