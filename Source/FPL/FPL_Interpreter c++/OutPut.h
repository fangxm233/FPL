#pragma once
//#include <string>
//#include <iostream>
//using namespace std;

class OutPut
{
public:
	static void FileError(string s);
	static void RunTimeError(string s);
	static void RunTimeWarning(string s);
};

class FileException : exception { };
class RunTimeException : exception { };