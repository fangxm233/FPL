#include "stdafx.h"
#include "Lexer.h"

vector<InstructionType> Lexer::instructions_v;
vector<int> Lexer::parameters_v;
//vector<int> Lexer::methods_p_v;
//vector<int> Lexer::methods_i_v;
vector<int> Lexer::methods_v;
list<InstructionType> Lexer::instructions;
list<int> Lexer::parameters;
list<int> Lexer::methods;
int Lexer::peek;
ifstream Lexer::infile;
int Lexer::line;

void Lexer::Analysis(const char * filename)
{
	instructions = list<InstructionType>();
	parameters = list<int>();
	methods = list<int>();
	infile = ifstream("G://GIT/FPL/Source/FPL/Debug/Program.fplc");
	//infile.open("Program.fplc");
	if (!infile.good()) 
	{
		cout << "�ļ���ʧ��" << endl;
		throw;
	}
	while (!infile.eof())
	{
		Scan();
	}
	list<InstructionType>::iterator itor_ins = instructions.begin();
	instructions_v = vector<InstructionType>(instructions.size());
	for (int i = 0; itor_ins != instructions.end(); i++)
	{
		instructions_v[i] = *itor_ins++;
	}
	list<int>::iterator itor_i = parameters.begin();
	parameters_v = vector<int>(parameters.size());
	for (int i = 0; itor_i != parameters.end(); i++)
	{
		parameters_v[i] = *itor_i++;
	}
	itor_i = methods.begin();
	methods_v = vector<int>(methods.size());
	for (int i = 0; itor_i != methods.end(); i++)
	{
		methods_v[i] = *itor_i++;
	}
	//itor_i = methods.begin();
	//methods_i_v = vector<int>(methods.size());
	//for (int i = 0; itor_i != methods.end(); i++)
	//{
	//	methods_i_v[i] = (int)(instructions.begin()._Ptr + *itor_i++);
	//}
}

void Lexer::Scan()
{
	Readch();
	instructions.push_back((InstructionType)peek);
	if (instructions.back() == InstructionType::func) 
	{
		instructions.pop_back();
		Readch();
		methods.push_back(peek);
		return;
	}
	Readch();
	parameters.push_back((InstructionType)peek);
	//switch (instructions.back())
	//{
	//case InstructionType::jmp:
	//	parameters.back() = parameters.back() - line - 1;
	//	break;
	//case InstructionType::eqt:
	//	parameters.back() = parameters.back() - line - 1;
	//	break;
	//case InstructionType::eqf:
	//	parameters.back() = parameters.back() - line - 1;
	//	break;
	//case InstructionType::let:
	//	parameters.back() = parameters.back() - line - 1;
	//	break;
	//case InstructionType::lef:
	//	parameters.back() = parameters.back() - line - 1;
	//	break;
	//case InstructionType::mot:
	//	parameters.back() = parameters.back() - line - 1;
	//	break;
	//case InstructionType::mof:
	//	parameters.back() = parameters.back() - line - 1;
	//	break;
	//default:
	//	break;
	//}
	line++;
}

void Lexer::Readch() 
{
	infile >> peek;
	//infile.
}