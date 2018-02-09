#include "stdafx.h"
#include "Runner.h"
#include "OutPut.h"
#include "Lexer.h"
//#include "InstructionType.h"
InstructionType* Runner::code_ptr;
InstructionType* Runner::code_ptr_start;
int* Runner::varStack_ptr;
int* Runner::tmpStack_ptr;
int* Runner::parameter_ptr;
int* Runner::parameter_ptr_start;
int* Runner::methods_ptr_start;
//int Runner::backup_code_ptr = 0;
//int Runner::backup_varStack_ptr = -1;
//int Runner::backup_tmpStack_ptr = -1;
vector<InstructionType> Runner::Instructions = Lexer::instructions_v;
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
		//vector<InstructionType>::iterator i = Instructions.begin();
		//InstructionType * aaa = i._Ptr;
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
	case InstructionType::loadi:
		*++varStack_ptr = 0;
		break;
	case InstructionType::unloadi:
		varStack_ptr--;
		break;
	case InstructionType::pushvar:
		*++tmpStack_ptr = *(varStack_ptr - *parameter_ptr);
		break;
	case InstructionType::pushadr:
		break;
	case InstructionType::pushval:
		*++tmpStack_ptr = *parameter_ptr;
		break;
	case InstructionType::poparg:
		*++varStack_ptr = *tmpStack_ptr--;
		break;
	case InstructionType::popvar:
		*(varStack_ptr - *parameter_ptr) = *tmpStack_ptr--;
		break;
	case InstructionType::popadr:
		break;
	case InstructionType::pop:
		tmpStack_ptr--;
		break;
	case InstructionType::add:
		tmpStack_ptr--;
		*tmpStack_ptr = *tmpStack_ptr + *(tmpStack_ptr + 1);
		break;
	case InstructionType::addv:
		*(varStack_ptr - *parameter_ptr) += *tmpStack_ptr--;
		break;
	case InstructionType::add1:
		(*tmpStack_ptr)++;
		break;
	case InstructionType::addv1:
		(*(varStack_ptr - *parameter_ptr))++;
		break;
	case InstructionType::sub:
		tmpStack_ptr--;
		*tmpStack_ptr = *tmpStack_ptr - *(tmpStack_ptr + 1);
		break;
	case InstructionType::subv:
		*(varStack_ptr - *parameter_ptr) -= *tmpStack_ptr--;
		break;
	case InstructionType::sub1:
		(*tmpStack_ptr)--;
		break;
	case InstructionType::subv1:
		(*(varStack_ptr - *parameter_ptr))--;
		break;
	case InstructionType::div_:
		tmpStack_ptr--;
		*tmpStack_ptr = *tmpStack_ptr / *(tmpStack_ptr + 1);
		break;
	case InstructionType::divv:
		*(varStack_ptr - *parameter_ptr) /= *tmpStack_ptr--;
		break;
	case InstructionType::mul:
		tmpStack_ptr--;
		*tmpStack_ptr = *tmpStack_ptr * *(tmpStack_ptr + 1);
		break;
	case InstructionType::mulv:
		*(varStack_ptr - *parameter_ptr) *= *tmpStack_ptr--;
		break;
	case InstructionType::jmp:
		code_ptr = code_ptr_start + *parameter_ptr - 1;
		parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
		goto start;
	case InstructionType::call:
		//cout << "call" << endl;
		*++tmpStack_ptr = (int)(code_ptr + 1);
		*++tmpStack_ptr = (int)(parameter_ptr + 1);
		code_ptr = code_ptr_start + *(methods_ptr_start + *parameter_ptr) - 1;
		parameter_ptr = parameter_ptr_start + *(methods_ptr_start + *parameter_ptr) - 1;
		goto start;
	case InstructionType::ret:
		parameter_ptr = (int*) *--tmpStack_ptr;
		code_ptr = (InstructionType*) *--tmpStack_ptr;
		*tmpStack_ptr = *(tmpStack_ptr + 2);
		goto start;
	case InstructionType::eqt:
		if (*(tmpStack_ptr - 1) == *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionType::eqf:
		if (*(tmpStack_ptr - 1) != *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionType::let:
		if (*(tmpStack_ptr - 1) < *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionType::lef:
		if (*(tmpStack_ptr - 1) >= *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionType::mot:
		if (*(tmpStack_ptr - 1) > *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionType::mof:
		if (*(tmpStack_ptr - 1) <= *(tmpStack_ptr))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			tmpStack_ptr -= 2;
			goto start;
		}
		tmpStack_ptr -= 2;
		break;
	case InstructionType::endP:
		return;
	default:
		OutPut::RunTimeError("未知指令");
		return;
	}
	code_ptr++;
	parameter_ptr++;
	goto start;
}