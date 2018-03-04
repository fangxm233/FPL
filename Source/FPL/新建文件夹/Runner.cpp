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
vector<InstructionType> Runner::Instructions;
vector<int> Runner::parameters;
vector<int> Runner::methods;
vector<int> Runner::classes;
vector<int> Runner::stack;
vector<int> Runner::heap;
int* Runner::EBP;
int Runner::EAX;

void Runner::RunStart()//262144
{
	Instructions = Lexer::instructions_v;
	parameters = Lexer::parameters_v;
	methods = Lexer::methods_v;
	classes = Lexer::classes_v;
	if(Instructions.size()!=parameters.size())OutPut::RunTimeError("Ö¸ÁîÓë²ÎÊý²»Æ¥Åä");
	stack = vector<int>(262144);//1MB Õ»
	heap = vector<int>(262144 * 229);//1MB ¶Ñ....

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
	OutPut::RunTimeWarning("Õ»²»×ã£¬À©³ä...");
	stack.resize(stack.size() * 2);
}

void Runner::ExpansionHeap()
{
	OutPut::RunTimeWarning("¶Ñ²»×ã£¬À©³ä...");
	heap.resize(heap.size() * 2);
}

void Runner::RunInstructions()
{
	code_ptr = code_ptr_start + Lexer::entrance_line - 1;
	//parameter_ptr = parameter_ptr_start + Lexer::entrance_line - 1;
	//EBP = stack_ptr + 5;
	_asm
	{
		//mov ecx, [code_ptr_start]
		//mov eax, [Lexer::entrance_line]
		//lea eax, [eax*4-4]
		//add ecx, eax
		mov edx, [parameter_ptr_start]
		mov eax, [Lexer::entrance_line]
		lea eax, [eax * 4 - 4]
		add edx, eax

		mov ebp,esp
		add ebp,20
	}
	int *i = 0;
	int a = 0;
start:;
	switch (*code_ptr)
	{
	case InstructionType::pushloc:
		//*++stack_ptr = *(EBP + *parameter_ptr);
		_asm 
		{
			mov eax, [edx]
			lea eax,[eax*4]
			mov ebx, ebp
			sub ebx,eax
			push eax
		}
		break;
	case InstructionType::pusharg:
		//*++stack_ptr = *(EBP - *parameter_ptr - 4);
		_asm
		{
			mov eax, [edx]
			lea eax, [eax*4-16]
			mov ebx, ebp
			add ebx, eax
			push ebx
		}
		break;
	case InstructionType::pushfield:
		//*stack_ptr = (unsigned long int)((i + (*stack_ptr / 4)) + 1 + *parameter_ptr);
		_asm 
		{
			pop eax
			add eax, 4
			add eax,[edx]
			push eax
		}
		break;
	case InstructionType::pushadr:
		break;
	case InstructionType::pushval:
		//*++stack_ptr = *parameter_ptr;
		_asm 
		{
			push [edx]
		}
		break;
	case InstructionType::pushEAX:
		//*++stack_ptr = EAX;
		_asm
		{
			push eax
		}
		break;
	case InstructionType::popadr:
		break;
	case InstructionType::popEAX:
		//EAX = *stack_ptr--;
		_asm 
		{
			pop eax
		}
		break;
	case InstructionType::pop:
		//stack_ptr--;
		_asm 
		{
			pop ebx
		}
		break;
	case InstructionType::storeloc:
		//*(EBP + *parameter_ptr) = *stack_ptr;
		//stack_ptr -= 1;
		_asm 
		{
			mov eax, [edx]
			mov eax, [eax]
			mov ebx, ebp
			lea eax, [eax*4]
			sub ebx,eax
			pop [ebx]
		}
		break;
	case InstructionType::storearg:
		//*(EBP - *parameter_ptr - 4) = *stack_ptr;
		//stack_ptr -= 1;
		_asm 
		{
			mov eax, [edx]
			lea eax, [eax * 4 - 16]
			mov ebx, ebp
			add ebx, eax
			pop ebx
		}
		break;
	case InstructionType::storefield:
		//*((i + (*stack_ptr / 4)) + 1 + *parameter_ptr) = *(stack_ptr - 1);
		//stack_ptr -= 2;
		_asm 
		{
			pop eax
			add eax, 4
			add eax, [edx]
			pop [eax]
		}
		break;
	case InstructionType::add:
		//stack_ptr--;
		//*((i + (*stack_ptr / 4)) + 1) = *((i + (*(stack_ptr) / 4)) + 1) + *((i + (*(stack_ptr + 1) / 4)) + 1);
		//break;
		_asm 
		{
			pop ebx
			//		*((i + (*stack_ptr / 4)) + 1) = *((i + (*(stack_ptr) / 4)) + 1) + *((i + (*(stack_ptr + 1) / 4)) + 1);
			pop eax
			add eax, 4
			add ebx, 4
			mov ecx, [eax]
			add ecx, [ebx]
			mov [eax], ecx
		}
		break;
	case InstructionType::addl:
		//(*((i + (*stack_ptr-- / 4)) + 1))++;

		break;
	case InstructionType::adda:
		break;
	case InstructionType::addf:
		break;
	case InstructionType::add1:
		(*stack_ptr)++;
		break;
	case InstructionType::addl1:
		break;
	case InstructionType::adda1:
		break;
	case InstructionType::addf1:
		break;
	case InstructionType::sub:
		//stack_ptr--;
		//*((i + (*stack_ptr / 4)) + 1) = *((i + (*(stack_ptr) / 4)) + 1) - *((i + (*(stack_ptr + 1) / 4)) + 1);
		//break;
		break;
	case InstructionType::subl:
		(*((i + (*stack_ptr-- / 4)) + 1))--;
		break;
	case InstructionType::suba:
		break;
	case InstructionType::subf:
		break;
	case InstructionType::sub1:
		(*stack_ptr)--;
		break;
	case InstructionType::subl1:
		break;
	case InstructionType::suba1:
		break;
	case InstructionType::subf1:
		break;
	case InstructionType::div_:
		stack_ptr--;
		*((i + (*stack_ptr / 4)) + 1) = *((i + (*(stack_ptr) / 4)) + 1) / *((i + (*(stack_ptr + 1) / 4)) + 1);
		break;
	case InstructionType::divl:
		break;
	case InstructionType::diva:
		break;
	case InstructionType::divf:
		break;
	case InstructionType::mul:
		stack_ptr--;
		*((i + (*stack_ptr / 4)) + 1) = *((i + (*(stack_ptr) / 4)) + 1) * *((i + (*(stack_ptr + 1) / 4)) + 1);
		break;
	case InstructionType::mull:
		break;
	case InstructionType::mula:
		break;
	case InstructionType::mulf:
		break;
	case InstructionType::jmp:
		//code_ptr = code_ptr_start + *parameter_ptr - 1;
		//parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
		//goto start;
		_asm 
		{
			mov eax, [code_ptr_start]
			mov ebx, [edx]
			sub ebx, 1
			lea ebx, [ebx * 4]
			add eax, ebx
			mov ecx, [code_ptr]
			mov [ecx], eax

			mov eax, [parameter_ptr_start]
			mov ebx, [edx]
			sub ebx, 1
			lea ebx, [ebx * 4]
			add eax, ebx
			mov edx, eax
		}
		goto start;
	case InstructionType::call:
		//*++stack_ptr = (unsigned long int)EBP;
		//*++stack_ptr = (unsigned long int)code_ptr;
		//*++stack_ptr = (unsigned long int)parameter_ptr;
		//code_ptr = code_ptr_start + *(methods_ptr_start + *parameter_ptr) - 1;
		//parameter_ptr = parameter_ptr_start + *(methods_ptr_start + *parameter_ptr) - 1;
		//EBP = stack_ptr + 1;
		_asm 
		{
			push ebp
			push [code_ptr]
			push edx

			mov eax, [code_ptr_start]
			mov ebx, [edx]
			lea ebx, [ebx*4]
			add ebx, [methods_ptr_start]
			mov ebx, [ebx]
			sub ebx, 1
			lea ebx, [ebx * 4]
			add eax, ebx
			mov ecx, [code_ptr]
			mov[ecx], eax

			mov eax, [parameter_ptr_start]
			mov ebx, [edx]
			lea ebx, [ebx * 4]
			add ebx, [methods_ptr_start]
			mov ebx, [ebx]
			sub ebx, 1
			lea ebx, [ebx * 4]
			add eax, ebx
			mov edx, eax

			lea ebp, [esp - 4]
		}
		goto start;
	case InstructionType::ret:
		//parameter_ptr = i + (*stack_ptr--) / 4;
		//code_ptr = (InstructionType*) *stack_ptr--;
		//EBP = i + (*stack_ptr--) / 4;
		_asm 
		{
			pop edx
			pop [code_ptr]
			pop ebp
		}
		break;
	case InstructionType::eqt:
		if (*((i + (*(stack_ptr - 1) / 4)) + 1) == *((i + (*(stack_ptr) / 4)) + 1))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::eqf:
		if (*((i + (*(stack_ptr - 1) / 4)) + 1) != *((i + (*(stack_ptr) / 4)) + 1))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::let:
		if (*((i + (*(stack_ptr - 1) / 4)) + 1) < *((i + (*(stack_ptr) / 4)) + 1))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::lef:
		if (*((i + (*(stack_ptr - 1) / 4)) + 1) >= *((i + (*(stack_ptr) / 4)) + 1))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::mot:
		if (*((i + (*(stack_ptr - 1) / 4)) + 1) > *((i + (*(stack_ptr) / 4)) + 1))
		{
			code_ptr = code_ptr_start + *parameter_ptr - 1;
			parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
			stack_ptr -= 2;
			goto start;
		}
		stack_ptr -= 2;
		break;
	case InstructionType::mof:
		//if (*((i + (*(stack_ptr - 1) / 4)) + 1) <= *((i + (*(stack_ptr) / 4)) + 1))
		//{
		//	code_ptr = code_ptr_start + *parameter_ptr - 1;
		//	parameter_ptr = parameter_ptr_start + *parameter_ptr - 1;
		//	stack_ptr -= 2;
		//	goto start;
		//}
		//stack_ptr -= 2;
		_asm 
		{
			pop eax
			add eax, 4
			mov eax, [eax]
			pop ebx
			add ebx, 4
			mov ebx, [ebx]
			cmp ebx, eax
			pop ebx
			pop ebx
			jg end11

			mov eax, [code_ptr_start]
			mov ebx, [edx]
			sub ebx, 1
			lea ebx, [ebx * 4]
			add eax, ebx
			mov ecx, [code_ptr]
			mov[ecx], eax

			mov eax, [parameter_ptr_start]
			mov ebx, [edx]
			sub ebx, 1
			lea ebx, [ebx * 4]
			add eax, ebx
			mov edx, eax
		}
		goto start;
	case InstructionType::newobjc:
		//*heap_ptr = *parameter_ptr;
		//*++stack_ptr = (unsigned long int)heap_ptr;
		//heap_ptr += *(class_ptr_start + *parameter_ptr - 1) + 1;
		_asm 
		{
			mov eax, [heap_ptr]
			mov ebx, [edx]
			mov [eax], ebx
			push eax
			mov eax, [class_ptr_start]
			mov ebx, [edx]
			lea ebx, [ebx*4]
			sub ebx, 4
			add eax, ebx
			mov eax, [eax]
			add eax, 1
			add [heap_ptr], eax
		}
		break;
	case InstructionType::newobji:
		//*heap_ptr = 0;
		//*++stack_ptr = (unsigned long int)heap_ptr;
		//*++heap_ptr = *parameter_ptr;
		//heap_ptr++;
		_asm
		{
			mov [heap_ptr], 0
			push [heap_ptr]
			add [heap_ptr], 4
			mov ebx, [edx]
			mov [heap_ptr], ebx
			add[heap_ptr], 4
		}
		break;
	case InstructionType::newobjf:
		break;
	case InstructionType::newobjs:
		break;
	case InstructionType::newobjb:
		break;
	case InstructionType::endP:
		return;
	default:
		OutPut::RunTimeError("Î´ÖªÖ¸Áî");
		return;
	}
	//code_ptr++;
	//parameter_ptr++;
end11:;
	_asm 
	{
		add [code_ptr], 4
		add edx, 4
	}
	goto start;
}