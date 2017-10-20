#pragma once
class Tag
{
public:
	 const static int
		ASSIGN = 280, STATEMENT = 287, FUNCTION = 291, FUNCTIONCALL = 293, QUOTE = 295;

	//标识符的tag
	 const static int
		 DO = 259, ELSE = 260, BREAK = 258, FALSE = 262, TRUE = 274,
		WHILE = 275, CONTINUE = 290, IF = 265, FOR = 284, RETURN = 292,
		USING = 294;

	//表达式的tag
	 const static int
		 BASIC = 257, ID = 264, MINUS = 268, NUM = 270, REAL = 272,
		STR = 276, PLUS = 277, MULTIPLY = 278, DIVIDE = 279;

	//布尔型表达式的tag
	 const static int
		 AND = 256, EQ = 261, GE = 263, LE = 267, NE = 269,
		OR = 271, MORE = 288, LESS = 289;

	//标点符号的tag
	 const static int
		 SEMICOLON = 281, LPARENTHESIS = 282, RPARENTHESIS = 283, LBRACE = 285, RBRACE = 286;

	//其他
	 const static int
		 INDEX = 266, TEMP = 273, Eof = 65535, EOL = 65534;
	Tag();
	~Tag();
};

