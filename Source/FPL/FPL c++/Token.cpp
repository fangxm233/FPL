#include "stdafx.h"
#include "Token.h"
#include "Lexer.h"
#include "Tag.h"
#include<iostream>

using namespace std;

Token::Token(int const i)
{
	tag = i;
	//line = Lexer::line;
}

Token::~Token()
{
}

string Token::ToString()
{
	return "" + tag;
}

void * Token::GetValue() 
{
	return nullptr;
}

#pragma region Num

Num::Num(int const i) : Token::Token(Tag::NUM)
{
	value = i;
}
string Num::ToString()
{
	return "" + value;
}

void * Num::GetValue()
{
	cout << 666;
	return &value;
}

#pragma endregion

