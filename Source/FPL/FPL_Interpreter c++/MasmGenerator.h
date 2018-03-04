#pragma once
class MasmGenerator
{
public:
	MasmGenerator();
	~MasmGenerator();

private:
	static vector<char> Gen_pushloc();
	static vector<char> Gen_pusharg();
	static vector<char> Gen_pushfield();
	static vector<char> Gen_pushadr();
	static vector<char> Gen_pushval();
	static vector<char> Gen_pushsta();
	static vector<char> Gen_pushEAX();
	static vector<char> Gen_popadr();
	static vector<char> Gen_popEAX();
	static vector<char> Gen_pop();
	static vector<char> Gen_storeloc();
	static vector<char> Gen_storearg();
	static vector<char> Gen_storefield();
	static vector<char> Gen_add_i();
	static vector<char> Gen_add_f();
	static vector<char> Gen_add_c();
	static vector<char> Gen_addl();
	static vector<char> Gen_adda();
	static vector<char> Gen_addf();
	static vector<char> Gen_add1();
	static vector<char> Gen_addl1();
	static vector<char> Gen_adda1();
	static vector<char> Gen_addf1();
	static vector<char> Gen_sub_i();
	static vector<char> Gen_sub_f();
	static vector<char> Gen_sub_c();
	static vector<char> Gen_subl();
	static vector<char> Gen_suba();
	static vector<char> Gen_subf();
	static vector<char> Gen_sub1();
	static vector<char> Gen_subl1();
	static vector<char> Gen_suba1();
	static vector<char> Gen_subf1();
	static vector<char> Gen_div_i();
	static vector<char> Gen_div_f();
	static vector<char> Gen_div_c();
	static vector<char> Gen_divl();
	static vector<char> Gen_diva();
	static vector<char> Gen_divf();
	static vector<char> Gen_mul_i();
	static vector<char> Gen_mul_f();
	static vector<char> Gen_mul_c();
	static vector<char> Gen_mull();
	static vector<char> Gen_mula();
	static vector<char> Gen_mulf();
	static vector<char> Gen_jmp();
	static vector<char> Gen_call();
	static vector<char> Gen_ret();
	static vector<char> Gen_eqt();
	static vector<char> Gen_eqf();
	static vector<char> Gen_let();
	static vector<char> Gen_lef();
	static vector<char> Gen_mot();
	static vector<char> Gen_mof();
	static vector<char> Gen_newobjc();
	static vector<char> Gen_endP();
};