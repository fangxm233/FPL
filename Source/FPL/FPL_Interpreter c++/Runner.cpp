#include "stdafx.h"
#include "Runner.h"
#include "OutPut.h"
#include "Lexer.h"

InstructionType* Runner::code_ptr;
InstructionType* Runner::code_ptr_start;
int* Runner::stack_ptr;
int* Runner::parameter_ptr;
int* Runner::parameter_ptr_start;
int* Runner::methods_ptr_start;
int* Runner::heap_ptr;
int* Runner::heap_ptr_start;
int* Runner::class_ptr_start;
int* Runner::static_ptr_start;
vector<InstructionType> Runner::Instructions;
vector<int> Runner::parameters;
vector<int> Runner::methods;
vector<int> Runner::classes;
vector<int> Runner::stack;
vector<int> Runner::heap;
vector<int> static_variable;
int* Runner::EBP;
int Runner::EAX;
int Runner::static_count;

void Runner::RunStart()
{
	Instructions = Lexer::instructions_v;
	parameters = Lexer::parameters_v;
	methods = Lexer::methods_v;
	classes = Lexer::classes_v;
	static_count = Lexer::static_count;
	static_variable = vector<int>(static_count);
	if(Instructions.size()!=parameters.size())OutPut::RunTimeError("指令与参数不匹配");
	stack = vector<int>(262144);//1MB 栈
	heap = vector<int>(262144);//1MB 堆

	static_ptr_start = static_variable.begin()._Ptr;
	code_ptr = Instructions.begin()._Ptr;
	code_ptr_start = Instructions.begin()._Ptr;
	parameter_ptr = parameters.begin()._Ptr;
	parameter_ptr_start = parameters.begin()._Ptr;
	methods_ptr_start = methods.begin()._Ptr;
	stack_ptr = stack.begin()._Ptr;
	stack_ptr--;
	heap_ptr_start = heap.begin()._Ptr;
	heap_ptr = heap.begin()._Ptr;
	class_ptr_start = classes.begin()._Ptr;

	RunInstructions();
}

void Runner::ExpansionStack() 
{
	OutPut::RunTimeWarning("栈不足，扩充...");
	stack.resize(stack.size() * 2);
}
void Runner::ExpansionHeap()
{
	OutPut::RunTimeWarning("堆不足，扩充...");
	heap.resize(heap.size() * 2);
}

void Runner::RunInstructions()
{
	code_ptr = code_ptr_start + Lexer::entrance_line - 1;
	parameter_ptr = parameter_ptr_start + Lexer::entrance_line - 1;
	EBP = stack_ptr + 5;
	int *i = 0;
	int a = 0;
	int stack_end = (unsigned long int)stack.end()._Ptr;
	int heap_end = (unsigned long int)heap.end()._Ptr;
start:;
	//if (stack_end - (unsigned long int)stack_ptr <= 10)
	//{
	//	OutPut::RunTimeError("栈溢出,暂时无法扩充");
	//}
	//if (heap_end - (unsigned long int)heap_ptr <= 10)
	//{
	//	OutPut::RunTimeError("堆溢出,暂无GC");
	//}
	a++;
	switch (*code_ptr)
	{
	case InstructionType::pushloc://(unsigned long int)
		*++stack_ptr = *(EBP + *parameter_ptr);
		break;
	case InstructionType::pusharg:
		*++stack_ptr = *(EBP - *parameter_ptr - 4);
		break;
	case InstructionType::pushfield:
		*stack_ptr = (unsigned long int)((i + (*stack_ptr / 4)) + 1 + *parameter_ptr);
		break;
	case InstructionType::pushval:
		*++stack_ptr = *parameter_ptr;
		break;
	case InstructionType::pushsta:
		*++stack_ptr = *(static_ptr_start + *parameter_ptr);
		break;
	case InstructionType::pushEAX:
		*++stack_ptr = EAX;
		break;
	case InstructionType::popEAX:
		EAX = *stack_ptr--;
		break;
	case InstructionType::pop:
		stack_ptr--;
		break;
	case InstructionType::storeloc:
		*(EBP + *parameter_ptr) = *stack_ptr;
		stack_ptr -= 1;
		break;
	case InstructionType::storearg:
		*(EBP - *parameter_ptr - 4) = *stack_ptr;
		stack_ptr -= 1;
		break;
	case InstructionType::storefield:
		*((i + (*stack_ptr / 4)) + 1 + *parameter_ptr) = *(stack_ptr - 1);
		stack_ptr -= 2;
		break;
	case InstructionType::storesta:
		*(static_ptr_start + *parameter_ptr) = *stack_ptr--;
		break;
	case InstructionType::add_i:
		stack_ptr--;
		*stack_ptr = *stack_ptr + *(stack_ptr + 1);
		break;
	case InstructionType::addl:
		*(EBP + *parameter_ptr) += *stack_ptr--;
		break;
	case InstructionType::adda:
		*(EBP - *parameter_ptr - 4) += *stack_ptr--;
		break;
	case InstructionType::addf:
		*((i + (*stack_ptr / 4)) + 1 + *parameter_ptr) += *stack_ptr--;
		break;
	case InstructionType::add1:
		(*stack_ptr)++;
		break;
	case InstructionType::addl1:
		*(EBP + *parameter_ptr) += 1;
		break;
	case InstructionType::adda1:
		*(EBP - *parameter_ptr - 4) += 1;
		break;
	case InstructionType::addf1:
		*((i + (*stack_ptr / 4)) + 1 + *parameter_ptr) += 1;
		break;
	case InstructionType::sub_i:
		stack_ptr--;
		*stack_ptr = *stack_ptr - *(stack_ptr + 1);
		break;
	case InstructionType::subl:
		*(EBP + *parameter_ptr) -= *stack_ptr--;
		break;
	case InstructionType::suba:
		*(EBP - *parameter_ptr - 4) -= *stack_ptr--;
		break;
	case InstructionType::subf:
		*((i + (*stack_ptr / 4)) + 1 + *parameter_ptr) -= *stack_ptr--;
		break;
	case InstructionType::sub1:
		(*stack_ptr)--;
		break;
	case InstructionType::subl1:
		*(EBP + *parameter_ptr) -= 1;
		break;
	case InstructionType::suba1:
		*(EBP - *parameter_ptr - 4) -= 1;
		break;
	case InstructionType::subf1:
		*((i + (*stack_ptr / 4)) + 1 + *parameter_ptr) -= 1;
		break;
	case InstructionType::div_i:
		stack_ptr--;
		*stack_ptr = *stack_ptr / *(stack_ptr + 1);
		break;
	case InstructionType::divl:
		*(EBP + *parameter_ptr) /= *stack_ptr--;
		break;
	case InstructionType::diva:
		*(EBP - *parameter_ptr - 4) /= *stack_ptr--;
		break;
	case InstructionType::divf:
		*((i + (*stack_ptr / 4)) + 1 + *parameter_ptr) /= *stack_ptr--;
		break;
	case InstructionType::mul_i:
		stack_ptr--;
		*stack_ptr = *stack_ptr * *(stack_ptr + 1);
		break;
	case InstructionType::mull:
		*(EBP + *parameter_ptr) *= *stack_ptr--;
		break;
	case InstructionType::mula:
		*(EBP - *parameter_ptr - 4) *= *stack_ptr--;
		break;
	case InstructionType::mulf:
		*((i + (*stack_ptr / 4)) + 1 + *parameter_ptr) *= *stack_ptr--;
		break;
	case InstructionType::jmp:
		code_ptr = code_ptr_start + *parameter_ptr - 1;
		parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
		goto start;
	case InstructionType::call:
		*++stack_ptr = (unsigned long int)EBP;
		*++stack_ptr = (unsigned long int)code_ptr;
		*++stack_ptr = (unsigned long int)parameter_ptr;
		code_ptr = code_ptr_start + *(methods_ptr_start + *parameter_ptr) - 1;
		parameter_ptr = parameter_ptr_start + *(methods_ptr_start + *parameter_ptr) - 1;
		EBP = stack_ptr + 1;
		goto start;
	case InstructionType::ret:
		parameter_ptr = i + (*stack_ptr--) / 4;
		parameter_ptr++;
		code_ptr = (InstructionType*) *stack_ptr--;
		code_ptr++;
		EBP = i + (*stack_ptr--) / 4;
		goto start;
	case InstructionType::eqt:
		if (*(stack_ptr - 1) == *stack_ptr)
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::eqf:
		if (*(stack_ptr - 1) != *stack_ptr)
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::let:
		if (*(stack_ptr - 1) < *stack_ptr)
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::lef:
		if (*(stack_ptr - 1) >= *stack_ptr)
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::mot:
		if (*(stack_ptr - 1) > *stack_ptr)
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::mof:
		if (*(stack_ptr - 1) <= *stack_ptr)
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::newobjc:
		*heap_ptr = *parameter_ptr;
		*++stack_ptr = (unsigned long int)heap_ptr;
		heap_ptr += *(class_ptr_start + *parameter_ptr - 1) + 1;
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