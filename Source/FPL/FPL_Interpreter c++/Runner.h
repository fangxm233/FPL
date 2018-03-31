#pragma once

class Runner
{
public:
	static void RunStart();
private:
	static vector<int> instructions_;
	//static vector<int> parameters_;
	static vector<int> stack_;
	static vector<int> methods_;
	static vector<int> classes_;
	static vector<int> heap_;

	static int* code_ptr_;
	static int* code_ptr_start_;
	//static int* parameter_ptr_;
	//static int* parameter_ptr_start_;
	static int* stack_ptr_;
	static int* methods_ptr_start_;
	static int* heap_ptr_;
	static int* heap_ptr_start_;
	static int* class_ptr_start_;
	static int* static_ptr_start_;

	static int static_count_;

	static int* EBP;
	static int EAX;

	static void RunInstructions();
	static void ExpandStack();
	static void ExpandHeap();
};

