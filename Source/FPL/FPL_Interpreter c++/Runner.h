#pragma once
//#include "InstructionType.h"
#include "Lexer.h"

class Runner
{
public:
	static void RunStart();
private:
	static vector<InstructionType> Instructions;
	static vector<int> parameters;
	static vector<int> stack;
	static vector<int> methods;
	static vector<int> classes;
	static vector<int> heap;

	static InstructionType* code_ptr;
	static InstructionType* code_ptr_start;
	static int* parameter_ptr;
	static int* parameter_ptr_start;
	static int* stack_ptr;
	static int* methods_ptr_start;
	static int* heap_ptr;
	static int* heap_ptr_start;
	static int* class_ptr_start;

	static int* EBP;
	static int EAX;

	static void RunInstructions();
	static void ExpansionStack();
	static void ExpansionHeap();
};

