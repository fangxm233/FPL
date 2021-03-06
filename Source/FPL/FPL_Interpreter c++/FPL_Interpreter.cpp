// FPL_Interpreter.cpp: 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include "Lexer.h"
#include "Runner.h"
#include "OutPut.h"
#include <time.h>

int main()
{
	clock_t start, finish;
	try
	{
		start = clock();
		Lexer::Analysis("./Program.fplc");
		finish = clock();
		cout << "文件解析完成，耗时";
		cout << (finish - start);
		cout << "毫秒" << endl;
	}
	catch (const FileException&)
	{
	}
	try
	{
		start = clock();
		Runner::RunStart();
		finish = clock();
		cout << "程序运行完成，耗时";
		cout << (finish - start);
		cout << "毫秒" << endl;
	}
	catch (const RunTimeException&)
	{
	}
	int i;
	cin >> i;
    return 0;
}

