// FPLc.cpp: 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include "Lexer.h"
#include "Token.h"
#include "Tag.h"
#include<iostream>
using namespace std;

int main()
{
	Num t = Num(2);
	int* i = (int*)t.GetValue();
	cout << &i;
	//void* a = Num(2).GetValue();
	//int* b = (int*)a;
	//cout << &b;
	int j;
	cin >> j;
}