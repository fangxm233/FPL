#include "stdafx.h"
#include "Runner.h"
#include "OutPut.h"
#include "Lexer.h"
//#include "InstructionType.h"
int Runner::code_ptr = 0;
int Runner::varStack_ptr = -1;
int Runner::tmpStack_ptr = -1;
//int Runner::backup_code_ptr = 0;
//int Runner::backup_varStack_ptr = -1;
//int Runner::backup_tmpStack_ptr = -1;
vector<InstructionsType> Runner::Instructions = Lexer::instructions_v;
vector<int> Runner::parameters = Lexer::parameters_v;
vector<int> Runner::methods = Lexer::methods_v;
vector<int> Runner::tmp_stack;
vector<int> Runner::call_stack;

void Runner::RunStart() 
{
	Instructions = Lexer::instructions_v;
	parameters = Lexer::parameters_v;
	methods = Lexer::methods_v;
	if(Instructions.size()!=parameters.size())OutPut::RunTimeError("指令与参数不匹配");
	call_stack = vector<int>(262144);//1MB 调用栈
	tmp_stack = vector<int>(131072);//0.5MB 栈堆
//back:
	try
	{
		RunInstructions();
		return;
		vector<InstructionsType>::iterator i = Instructions.begin();
		InstructionsType * aaa = i._Ptr;
		cout << *aaa << endl;
		aaa++;
		cout << *aaa << endl;
		vector<int>::iterator ii = tmp_stack.begin();
		int * aaaa = ii._Ptr;
		cout << *aaaa << endl;
		aaaa++;
		cout << *aaaa << endl;
	}
	catch (const std::exception&)
	{
		//if (varStack_ptr == call_stack.size()) 
		//{
		//	call_stack.resize(call_stack.size() * 2);
		//	OutPut::RunTimeWarning("调用栈不足，扩充...");
		//	code_ptr = backup_code_ptr;
		//	tmpStack_ptr = backup_tmpStack_ptr;
		//	varStack_ptr = backup_varStack_ptr;
		//	goto back;
		//}
		//if (tmpStack_ptr == tmp_stack.size())
		//{
		//	tmp_stack.resize(tmp_stack.size() * 2);
		//	OutPut::RunTimeWarning("临时栈不足，扩充...");
		//	code_ptr = backup_code_ptr;
		//	tmpStack_ptr = backup_tmpStack_ptr;
		//	varStack_ptr = backup_varStack_ptr;
		//	goto back;
		//}
	}
}

void Runner::Expansion_tmp() 
{
	//tmp_stack.resize(tmp_stack.size() * 2);
	OutPut::RunTimeWarning("临时栈不足，扩充...");
	//code_ptr = backup_code_ptr;
	//tmpStack_ptr = backup_tmpStack_ptr;
	//varStack_ptr = backup_varStack_ptr;
}

void Runner::Expansion_var()
{
	//call_stack.resize(call_stack.size() * 2);
	OutPut::RunTimeWarning("调用栈不足，扩充...");
	//code_ptr = backup_code_ptr;
	//tmpStack_ptr = backup_tmpStack_ptr;
	//varStack_ptr = backup_varStack_ptr;
}

void Runner::RunInstructions() 
{
start:;
	//backup_code_ptr = code_ptr;
	//backup_tmpStack_ptr = tmpStack_ptr;
	//backup_varStack_ptr = varStack_ptr;
	//if ((tmp_stack.size() - tmpStack_ptr) <= 10)Expansion_tmp();
	//if ((call_stack.size() - varStack_ptr) <= 10)Expansion_var();
	switch (Instructions[code_ptr])
	{
	case InstructionsType::loadi:
		call_stack[++varStack_ptr] = 0;
		break;
	case InstructionsType::unloadi:
		varStack_ptr--;
		break;
	case InstructionsType::pushvar:
		tmp_stack[++tmpStack_ptr] = call_stack[varStack_ptr - parameters[code_ptr]];
		break;
	case InstructionsType::pushadr:
		break;
	case InstructionsType::pushval:
		tmp_stack[++tmpStack_ptr] = parameters[code_ptr];
		break;
	case InstructionsType::poparg:
		call_stack[++varStack_ptr] = tmp_stack[tmpStack_ptr--];
		break;
	case InstructionsType::popvar:
		call_stack[varStack_ptr - parameters[code_ptr]] = tmp_stack[tmpStack_ptr--];
		break;
	case InstructionsType::popadr:
		break;
	case InstructionsType::pop:
		tmpStack_ptr--;
		break;
	case InstructionsType::add:
		tmpStack_ptr -= 1;
		tmp_stack[tmpStack_ptr] = tmp_stack[tmpStack_ptr] + tmp_stack[tmpStack_ptr + 1];
		break;
	case InstructionsType::addv:
		call_stack[varStack_ptr - parameters[code_ptr]] += tmp_stack[tmpStack_ptr--];
		break;
	case InstructionsType::add1:
		tmp_stack[tmpStack_ptr]++;
		break;
	case InstructionsType::addv1:
		call_stack[varStack_ptr - parameters[code_ptr]]++;
		break;
	case InstructionsType::sub:
		tmpStack_ptr -= 1;
		tmp_stack[tmpStack_ptr] = tmp_stack[tmpStack_ptr] - tmp_stack[tmpStack_ptr + 1];
		break;
	case InstructionsType::subv:
		call_stack[varStack_ptr - parameters[code_ptr]] -= tmp_stack[tmpStack_ptr--];
		break;
	case InstructionsType::sub1:
		tmp_stack[tmpStack_ptr]--;
		break;
	case InstructionsType::subv1:
		call_stack[varStack_ptr - parameters[code_ptr]]--;
		break;
	case InstructionsType::div_:
		tmpStack_ptr -= 1;
		tmp_stack[tmpStack_ptr] = tmp_stack[tmpStack_ptr] / tmp_stack[tmpStack_ptr + 1];
		break;
	case InstructionsType::divv:
		call_stack[varStack_ptr - parameters[code_ptr]] /= tmp_stack[tmpStack_ptr--];
		break;
	case InstructionsType::mul:
		tmpStack_ptr -= 1;
		tmp_stack[tmpStack_ptr] = tmp_stack[tmpStack_ptr] * tmp_stack[tmpStack_ptr + 1];
		break;
	case InstructionsType::mulv:
		call_stack[varStack_ptr - parameters[code_ptr]] *= tmp_stack[tmpStack_ptr--];
		break;
	case InstructionsType::jmp:
		code_ptr = parameters[code_ptr] - 1;
		goto start;
	case InstructionsType::call:
		tmp_stack[++tmpStack_ptr] = code_ptr + 1;
		code_ptr = methods[parameters[code_ptr]] - 1;
		goto start;
	case InstructionsType::ret:
		code_ptr = tmp_stack[--tmpStack_ptr];
		tmp_stack[tmpStack_ptr] = tmp_stack[tmpStack_ptr + 1];
		goto start;
	case InstructionsType::eqt:
		if (tmp_stack[tmpStack_ptr - 1] == tmp_stack[tmpStack_ptr])
		{
			code_ptr = parameters[code_ptr] - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::eqf:
		if (tmp_stack[tmpStack_ptr - 1] != tmp_stack[tmpStack_ptr])
		{
			code_ptr = parameters[code_ptr] - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::let:
		if (tmp_stack[tmpStack_ptr - 1] < tmp_stack[tmpStack_ptr])
		{
			code_ptr = parameters[code_ptr] - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::lef:
		if (tmp_stack[tmpStack_ptr - 1] >= tmp_stack[tmpStack_ptr])
		{
			code_ptr = parameters[code_ptr] - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::mot:
		if (tmp_stack[tmpStack_ptr - 1] > tmp_stack[tmpStack_ptr])
		{
			code_ptr = parameters[code_ptr] - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::mof:
		if (tmp_stack[tmpStack_ptr - 1] <= tmp_stack[tmpStack_ptr])
		{
			code_ptr = parameters[code_ptr] - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::endP:
		return;
	default:
		OutPut::RunTimeError("未知指令");
		return;
	}
	code_ptr++;
	goto start;
}