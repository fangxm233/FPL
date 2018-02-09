#include "stdafx.h"
#include "Runner.h"
#include "OutPut.h"
#include "Lexer.h"
//#include "InstructionType.h"
InstructionsType* Runner::code_ptr;
InstructionsType* Runner::code_ptr_start;
int* Runner::varStack_ptr;
int* Runner::tmpStack_ptr;
int* Runner::parameter_ptr;
int* Runner::parameter_ptr_start;
int* Runner::methods_ptr_start;
//int Runner::backup_code_ptr = 0;
//int Runner::backup_varStack_ptr = -1;
//int Runner::backup_tmpStack_ptr = -1;
vector<InstructionsType> Runner::Instructions = Lexer::instructions_v;
vector<int> Runner::parameters = Lexer::parameters_v;
//vector<int> Runner::methods_p = Lexer::methods_p_v;
vector<int> Runner::methods = Lexer::methods_v;
vector<int> Runner::tmp_stack;
vector<int> Runner::call_stack;

void Runner::RunStart() 
{
	Instructions = Lexer::instructions_v;
	parameters = Lexer::parameters_v;
	//methods_p = Lexer::methods_p_v;
	methods = Lexer::methods_v;
	if(Instructions.size()!=parameters.size())OutPut::RunTimeError("指令与参数不匹配");
	call_stack = vector<int>(262144);//1MB 调用栈
	tmp_stack = vector<int>(131072);//0.5MB 栈堆

	code_ptr = Instructions.begin()._Ptr;
	parameter_ptr = parameters.begin()._Ptr;
	code_ptr_start = Instructions.begin()._Ptr;
	parameter_ptr_start = parameters.begin()._Ptr;
	methods_ptr_start = methods.begin()._Ptr;
	varStack_ptr = call_stack.begin()._Ptr;
	varStack_ptr--;
	tmpStack_ptr = tmp_stack.begin()._Ptr;
	tmpStack_ptr--;
	try
	{
		RunInstructions();
		return;
		//vector<InstructionsType>::iterator i = Instructions.begin();
		//InstructionsType * aaa = i._Ptr;
		//cout << *aaa << endl;
		//aaa++;
		//cout << *aaa << endl;
		//vector<int>::iterator ii = tmp_stack.begin();
		//int * aaaa = ii._Ptr;
		//cout << *aaaa << endl;
		//aaaa++;
		//cout << *aaaa << endl;
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
	switch (*code_ptr)
	{
	case InstructionsType::loadi:
		*++varStack_ptr = 0;
		break;
	case InstructionsType::unloadi:
		varStack_ptr--;
		break;
	case InstructionsType::pushvar:
		*++tmpStack_ptr = *(varStack_ptr - *parameter_ptr);
		break;
	case InstructionsType::pushadr:
		break;
	case InstructionsType::pushval:
		*++tmpStack_ptr = *parameter_ptr;
		break;
	case InstructionsType::poparg:
		*++varStack_ptr = *tmpStack_ptr--;
		break;
	case InstructionsType::popvar:
		*(varStack_ptr - *parameter_ptr) = *tmpStack_ptr--;
		break;
	case InstructionsType::popadr:
		break;
	case InstructionsType::pop:
		tmpStack_ptr--;
		break;
	case InstructionsType::add:
		tmpStack_ptr--;
		*tmpStack_ptr = *tmpStack_ptr + *(tmpStack_ptr + 1);
		break;
	case InstructionsType::addv:
		*(varStack_ptr - *parameter_ptr) += *tmpStack_ptr--;
		break;
	case InstructionsType::add1:
		(*tmpStack_ptr)++;
		break;
	case InstructionsType::addv1:
		(*(varStack_ptr - *parameter_ptr))++;
		break;
	case InstructionsType::sub:
		tmpStack_ptr--;
		*tmpStack_ptr = *tmpStack_ptr - *(tmpStack_ptr + 1);
		break;
	case InstructionsType::subv:
		*(varStack_ptr - *parameter_ptr) -= *tmpStack_ptr--;
		break;
	case InstructionsType::sub1:
		(*tmpStack_ptr)--;
		break;
	case InstructionsType::subv1:
		(*(varStack_ptr - *parameter_ptr))--;
		break;
	case InstructionsType::div_:
		tmpStack_ptr--;
		*tmpStack_ptr = *tmpStack_ptr / *(tmpStack_ptr + 1);
		break;
	case InstructionsType::divv:
		*(varStack_ptr - *parameter_ptr) /= *tmpStack_ptr--;
		break;
	case InstructionsType::mul:
		tmpStack_ptr--;
		*tmpStack_ptr = *tmpStack_ptr * *(tmpStack_ptr + 1);
		break;
	case InstructionsType::mulv:
		*(varStack_ptr - *parameter_ptr) *= *tmpStack_ptr--;
		break;
	case InstructionsType::jmp:
		code_ptr = code_ptr_start + *parameter_ptr - 1;
		parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
		goto start;
	case InstructionsType::call:
		//cout << "call" << endl;
		*++tmpStack_ptr = (int)(code_ptr + 1);
		*++tmpStack_ptr = (int)(parameter_ptr + 1);
		code_ptr = code_ptr_start + *(methods_ptr_start + *parameter_ptr) - 1;
		parameter_ptr = parameter_ptr_start + *(methods_ptr_start + *parameter_ptr) - 1;
		goto start;
	case InstructionsType::ret:
		parameter_ptr = (int*) *--tmpStack_ptr;
		code_ptr = (InstructionsType*) *--tmpStack_ptr;
		*tmpStack_ptr = *(tmpStack_ptr + 2);
		goto start;
	case InstructionsType::eqt:
		if (*(tmpStack_ptr - 1) == *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::eqf:
		if (*(tmpStack_ptr - 1) != *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::let:
		if (*(tmpStack_ptr - 1) < *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::lef:
		if (*(tmpStack_ptr - 1) >= *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::mot:
		if (*(tmpStack_ptr - 1) > *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionsType::mof:
		if (*(tmpStack_ptr - 1) <= *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
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
	parameter_ptr++;
	goto start;
}