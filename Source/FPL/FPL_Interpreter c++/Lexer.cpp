#include "stdafx.h"
#include "Lexer.h"
#include <sstream>

vector<InstructionType> Lexer::instructions_v;
vector<int> Lexer::parameters_v;
vector<int> Lexer::methods_v;
vector<int> Lexer::classes_v;
list<InstructionType> Lexer::instructions;
list<int> Lexer::parameters;
list<int> Lexer::methods;
list<int> Lexer::classes;
ifstream Lexer::infile;
int Lexer::line;
int Lexer::entrance_line;
int peek_int;
char peek_char;
string peek_string;

void Lexer::Analysis(string filename)
{
	instructions = list<InstructionType>();
	parameters = list<int>();
	methods = list<int>();
	infile = ifstream(filename);
	if (!infile.good())
	{
		cout << "文件打开失败" << endl;
		throw;
	}
	AnalysisFilehead();
	while (!infile.eof())
	{
		Scan();
	}
	TurnToVec();
}

void Lexer::AnalysisFilehead() 
{
	ReadchInt();//跳掉两个版本号
	ReadchInt();
	ReadchInt();
	entrance_line = peek_int;
	infile >> peek_char;
	for (; !isdigit(peek_char) || peek_char == 0; infile >> peek_char)
	{
		infile.seekg(-1, ios::cur);
		ReadchString();
		ReadchInt();
		ReadchInt();
		classes.push_back(peek_int);
		ReadchString();
		for (; ;ReadchString())
		{
			if (StringIsdigit(peek_string)) 
			{
				if (peek_string == "0")break;
				int temp = atoi(peek_string.c_str());
				methods.push_back(temp);
			}
		}
	}
	infile.seekg(-1, ios::cur);
	ReadchInt();
	ReadchInt();
}

void Lexer::Scan()
{
	ReadchInt();
	instructions.push_back((InstructionType)peek_int);
	//if (instructions.back() == InstructionType::func) 
	//{
	//	instructions.pop_back();
	//	ReadchInt();
	//	methods.push_back(peek_int);
	//	return;
	//}
	ReadchInt();
	parameters.push_back(peek_int);
	line++;
}

void  Lexer::TurnToVec()
{
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

	itor_i = classes.begin();
	classes_v = vector<int>(classes.size());
	for (int i = 0; itor_i != classes.end(); i++)
	{
		classes_v[i] = *itor_i++;
	}
}
void Lexer::ReadchInt() 
{
	infile >> peek_int;
	//infile.
}
void Lexer::ReadchString()
{
	infile >> peek_string;
}

bool Lexer::StringIsdigit(string s) 
{
	for (unsigned int i = 0; i < s.size(); i++)
	{
		if (!isdigit(s.at(i)))
		{
			return false;
		}
	}
	return true;
}