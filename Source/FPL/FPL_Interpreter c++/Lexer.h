#pragma once
//#include "InstructionType.h"
//using namespace std;

class Lexer
{
public:
	static void Analysis(string filename);
	static vector<InstructionType> instructions_v;
	static vector<int> parameters_v;
	static vector<int> methods_v;
	static vector<int> classes_v;
	static int entrance_line;
	static int static_count;
private:
	static int line;
	static list<InstructionType> instructions;
	static list<int> parameters;
	static list<int> methods;
	static list<int> classes;
	//static int peek_int;
	static ifstream infile;
	static void Scan();
	static void ReadchInt();
	static void ReadchString();
	static void TurnToVec();
	static void AnalysisFilehead();
	static bool StringIsdigit(string s);
};

