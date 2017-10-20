#pragma once
#include <string>
using namespace std;
class Token
{
public:
	int tag;
	//int line;
	Token(int const i);
	~Token();
	virtual string ToString();
	virtual void* GetValue();
};

class Num : public Token 
{
public:
	int value;
	Num(int const i);
	string ToString() override;
	void* GetValue() override;
};
