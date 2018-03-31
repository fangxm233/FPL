#include "stdafx.h"
#include "Runner.h"
#include "OutPut.h"
#include "Lexer.h"

int* Runner::code_ptr_;
int* Runner::code_ptr_start_;
int* Runner::stack_ptr_;
//int* Runner::parameter_ptr_;
//int* Runner::parameter_ptr_start_;
int* Runner::methods_ptr_start_;
int* Runner::heap_ptr_;
int* Runner::heap_ptr_start_;
int* Runner::class_ptr_start_;
int* Runner::static_ptr_start_;
vector<int> Runner::instructions_;
//vector<int> Runner::parameters_;
vector<int> Runner::methods_;
vector<int> Runner::classes_;
vector<int> Runner::stack_;
vector<int> Runner::heap_;
vector<int> static_variable;
int* Runner::EBP;
int EBX;
int Runner::EAX;
int Runner::static_count_;

void Runner::RunStart()
{
	instructions_ = Lexer::instructions_v;
	//parameters_ = Lexer::parameters_v;
	methods_ = Lexer::methods_v;
	classes_ = Lexer::classes_v;
	static_count_ = Lexer::static_count;
	static_variable = vector<int>(static_count_);
	//if(instructions_.size()!=parameters_.size())OutPut::RunTimeError("Ö¸ÁîÓë²ÎÊý²»Æ¥Åä");
	stack_ = vector<int>(262144); //1MB Õ»
	heap_ = vector<int>(262144); //1MB ¶Ñ

	static_ptr_start_ = static_variable.begin()._Ptr;
	code_ptr_ = instructions_.begin()._Ptr;
	code_ptr_start_ = instructions_.begin()._Ptr;
	//parameter_ptr_ = parameters_.begin()._Ptr;
	//parameter_ptr_start_ = parameters_.begin()._Ptr;
	methods_ptr_start_ = methods_.begin()._Ptr;
	stack_ptr_ = stack_.begin()._Ptr;
	stack_ptr_--;
	heap_ptr_start_ = heap_.begin()._Ptr;
	heap_ptr_ = heap_.begin()._Ptr;
	class_ptr_start_ = classes_.begin()._Ptr;

	RunInstructions();
}

void Runner::ExpandStack() 
{
	OutPut::RunTimeWarning("Õ»²»×ã£¬À©³ä...");
	stack_.resize(stack_.size() * 2);
}
void Runner::ExpandHeap()
{
	OutPut::RunTimeWarning("¶Ñ²»×ã£¬À©³ä...");
	heap_.resize(heap_.size() * 2);
}

void Runner::RunInstructions()
{
	code_ptr_ = code_ptr_start_ + Lexer::entrance_line * 2 - 2;
	//parameter_ptr_ = parameter_ptr_start_ + Lexer::entrance_line - 1;
	EBP = stack_ptr_ + 5;
	int* i = nullptr;
	//int a = 0;
start:;
	//a++;
	switch (*code_ptr_)
	{
	case pushloc: //(unsigned long int)
		*++stack_ptr_ = *(EBP + *++code_ptr_);
		break;
	case pusharg:
		*++stack_ptr_ = *(EBP - *++code_ptr_ - 4);
		break;
	case pushfield:
		*stack_ptr_ = reinterpret_cast<unsigned long int>((i + (*stack_ptr_ / 4)) + 1 + *++code_ptr_);
		break;
	case pushval:
		*++stack_ptr_ = *++code_ptr_;
		break;
	case pushsta:
		*++stack_ptr_ = *(static_ptr_start_ + *++code_ptr_);
		break;
	//case pushEAX:
	//	*++stack_ptr_ = EAX;
	//	break;
	//case popEAX:
	//	EAX = *stack_ptr_--;
	//	break;
	case pop:
		stack_ptr_--;
		code_ptr_++;
		break;
	case storeloc:
		*(EBP + *++code_ptr_) = *stack_ptr_;
		stack_ptr_ -= 1;
		break;
	case storearg:
		*(EBP - *++code_ptr_ - 4) = *stack_ptr_;
		stack_ptr_ -= 1;
		break;
	case storefield:
		*(i + (*stack_ptr_ / 4) + 1 + *++code_ptr_) = *(stack_ptr_ - 1);
		stack_ptr_ -= 2;
		break;
	case storesta:
		*(static_ptr_start_ + *++code_ptr_) = *stack_ptr_--;
		break;
	case add_i:
		stack_ptr_--;
		*stack_ptr_ = *stack_ptr_ + *(stack_ptr_ + 1);
		code_ptr_++;
		break;
	case addl:
		*(EBP + *++code_ptr_) += *stack_ptr_--;
		break;
	case adda:
		*(EBP - *++code_ptr_ - 4) += *stack_ptr_--;
		break;
	case addf:
		*(i + (*stack_ptr_ / 4) + 1 + *++code_ptr_) += *stack_ptr_--;
		break;
	case add1:
		(*stack_ptr_)++;
		code_ptr_++;
		break;
	case addl1:
		*(EBP + *++code_ptr_) += 1;
		break;
	case adda1:
		*(EBP - *++code_ptr_ - 4) += 1;
		break;
	case addf1:
		*((i + (*stack_ptr_ / 4)) + 1 + *++code_ptr_) += 1;
		break;
	case sub_i:
		stack_ptr_--;
		*stack_ptr_ = *stack_ptr_ - *(stack_ptr_ + 1);
		code_ptr_++;
		break;
	case subl:
		*(EBP + *++code_ptr_) -= *stack_ptr_--;
		break;
	case suba:
		*(EBP - *++code_ptr_ - 4) -= *stack_ptr_--;
		break;
	case subf:
		*((i + (*stack_ptr_ / 4)) + 1 + *++code_ptr_) -= *stack_ptr_--;
		break;
	case sub1:
		(*stack_ptr_)--;
		code_ptr_++;
		break;
	case subl1:
		*(EBP + *++code_ptr_) -= 1;
		break;
	case suba1:
		*(EBP - *++code_ptr_ - 4) -= 1;
		break;
	case subf1:
		*((i + (*stack_ptr_ / 4)) + 1 + *++code_ptr_) -= 1;
		break;
	case div_i:
		stack_ptr_--;
		*stack_ptr_ = *stack_ptr_ / *(stack_ptr_ + 1);
		code_ptr_++;
		break;
	case divl:
		*(EBP + *++code_ptr_) /= *stack_ptr_--;
		break;
	case diva:
		*(EBP - *++code_ptr_ - 4) /= *stack_ptr_--;
		break;
	case divf:
		*((i + (*stack_ptr_ / 4)) + 1 + *++code_ptr_) /= *stack_ptr_--;
		break;
	case mul_i:
		stack_ptr_--;
		*stack_ptr_ = *stack_ptr_ * *(stack_ptr_ + 1);
		code_ptr_++;
		break;
	case mull:
		*(EBP + *++code_ptr_) *= *stack_ptr_--;
		break;
	case mula:
		*(EBP - *++code_ptr_ - 4) *= *stack_ptr_--;
		break;
	case mulf:
		*((i + (*stack_ptr_ / 4)) + 1 + *++code_ptr_) *= *stack_ptr_--;
		break;
	case jmp:
		code_ptr_ = code_ptr_start_ + *++code_ptr_ * 2 - 2;
		//parameter_ptr_ = parameter_ptr_start_ + *++code_ptr_ - 1;
		goto start;
	case call:
		*++stack_ptr_ = reinterpret_cast<unsigned long int>(EBP);
		*++stack_ptr_ = reinterpret_cast<unsigned long int>(code_ptr_);
		//*++stack_ptr_ = reinterpret_cast<unsigned long int>(parameter_ptr_);
		code_ptr_ = code_ptr_start_ + *(methods_ptr_start_ + *++code_ptr_) * 2 - 2;
		//parameter_ptr_ = parameter_ptr_start_ + *(methods_ptr_start_ + *++code_ptr_) - 1;
		EBP = stack_ptr_ + 1;
		goto start;
	case ret:
		//parameter_ptr_ = i + (*stack_ptr_--) / 4;
		//parameter_ptr_++;
		EBX = *stack_ptr_--;
		stack_ptr_ = EBP--;
		EBP = i + (*stack_ptr_--) / 4;
		code_ptr_ = reinterpret_cast<int*>(*stack_ptr_--);
		code_ptr_ += 2;
		*++stack_ptr_ = EBX;
		goto start;
	case eq:
		stack_ptr_--;
		*stack_ptr_ = *stack_ptr_ == *(stack_ptr_ + 1) ? 1 : 0;
		code_ptr_++;
		break;
	case le:
		stack_ptr_--;
		*stack_ptr_ = *stack_ptr_ < *(stack_ptr_ + 1) ? 1 : 0;
		code_ptr_++;
		break;
	case mo:
		stack_ptr_--;
		*stack_ptr_ = *stack_ptr_ > *(stack_ptr_ + 1) ? 1 : 0;
		code_ptr_++;
		break;
	case jt:
		if (*stack_ptr_ == 1)
		{
			code_ptr_ = code_ptr_start_ + *++code_ptr_ * 2 - 2;
			//parameter_ptr_ = parameter_ptr_start_ + *parameter_ptr_ - 1;
			stack_ptr_--;
			goto start;
		}
		stack_ptr_--;
		code_ptr_++;
		break;
	case jf:
		if (*stack_ptr_ == 0)
		{
			code_ptr_ = code_ptr_start_ + *++code_ptr_ * 2 - 2;
			//parameter_ptr_ = parameter_ptr_start_ + *parameter_ptr_ - 1;
			stack_ptr_--;
			goto start;
		}
		stack_ptr_--;
		code_ptr_++;
		break;
	case newobjc:
		*heap_ptr_ = *++code_ptr_;
		*++stack_ptr_ = reinterpret_cast<unsigned long int>(heap_ptr_);
		heap_ptr_ += *(class_ptr_start_ + *code_ptr_ - 1) + 1;
		break;
	case endP:
		return;
	default:
		OutPut::RunTimeError("Î´ÖªÖ¸Áî");
		return;
	}
	code_ptr_++;
	//parameter_ptr_++;
	goto start;
}
