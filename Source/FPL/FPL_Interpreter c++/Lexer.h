#pragma once
//#include "InstructionType.h"
//using namespace std;

class Lexer
{
public:
	static void Analysis(const char * filename);
	static vector<InstructionsType> instructions_v;
	static vector<int> parameters_v;
	static vector<int> methods_v;
	//static vector<int> methods_p_v;
	//static vector<int> methods_i_v;
private:
	static int line;
	static list<InstructionsType> instructions;
	static list<int> parameters;
	static list<int> methods;
	static int peek;
	static ifstream infile;
	static void Scan();
	static void Readch();
};

