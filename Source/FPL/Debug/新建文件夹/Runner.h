#pragma once
//#include "InstructionType.h"
#include "Lexer.h"

class Runner
{
public:
	static void RunStart();
private:
	static vector<InstructionsType> Instructions;
	static vector<int> parameters;
	static vector<int> tmp_stack;
	static vector<int> call_stack;
	static vector<int> methods;

	static int code_ptr;
	static int varStack_ptr;
	static int tmpStack_ptr;

	//static int backup_code_ptr;
	//static int backup_varStack_ptr;
	//static int backup_tmpStack_ptr;
	static void RunInstructions();
	static void Expansion_tmp();
	static void Expansion_var();
};

