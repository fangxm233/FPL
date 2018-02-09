using FPL.lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPL_Interpreter
{
    public class Runner
    {
        static InstructionsType[] Instructions;
        static int[] parameters;
        static int[] tmp_stack;
        static int[] call_stack;
        static int[] methods;

        static int code_ptr;
        static int varStack_ptr = -1;
        static int tmpStack_ptr = -1;

        static int backup_code_ptr;
        static int backup_varStack_ptr = -1;
        static int backup_tmpStack_ptr = -1;

        public static void RunStart()
        {
            Instructions = Lexer.instructions.ToArray();
            parameters = Lexer.parameters.ToArray();
            methods = Lexer.methods.ToArray();
            if (Instructions.Length != parameters.Length) OutPut.RunTimeError("指令与参数不匹配");
            call_stack = new int[262144];//1MB 调用栈
            tmp_stack = new int[131072];//0.5MB 栈堆
            back:
            try
            {
                RunInstructions();
            }
            catch (Exception)
            {
                if (varStack_ptr == call_stack.Length)
                {
                    int[] call_stack_ = new int[call_stack.Length * 2];
                    call_stack.CopyTo(call_stack_, 0);
                    call_stack = call_stack_;
                    OutPut.RunTimeWarning("调用栈不足，扩充...");
                    code_ptr = backup_code_ptr;
                    tmpStack_ptr = backup_tmpStack_ptr;
                    varStack_ptr = backup_varStack_ptr;
                    goto back;
                }
                if (tmpStack_ptr == tmp_stack.Length)
                {
                    int[] tmp_stack_ = new int[tmp_stack.Length * 2];
                    tmp_stack.CopyTo(tmp_stack_, 0);
                    tmp_stack = tmp_stack_;
                    OutPut.RunTimeWarning("临时栈不足，扩充...");
                    code_ptr = backup_code_ptr;
                    tmpStack_ptr = backup_tmpStack_ptr;
                    varStack_ptr = backup_varStack_ptr;
                    goto back;
                }
                throw;
            }
            GC.Collect();
            //Console.WriteLine(stack[0]);
        }

        static void RunInstructions()
        {
            start:;
            backup_code_ptr = code_ptr;
            backup_tmpStack_ptr = tmpStack_ptr;
            backup_varStack_ptr = varStack_ptr;
            switch (Instructions[code_ptr])
            {
                case InstructionsType.loadi:
                    call_stack[++varStack_ptr] = 0;
                    break;
                case InstructionsType.unloadi:
                    varStack_ptr--;
                    break;
                case InstructionsType.pushvar:
                    tmp_stack[++tmpStack_ptr] = call_stack[varStack_ptr - parameters[code_ptr]];
                    break;
                case InstructionsType.pushadr:
                    break;
                case InstructionsType.pushval:
                    tmp_stack[++tmpStack_ptr] = parameters[code_ptr];
                    break;
                case InstructionsType.poparg:
                    call_stack[++varStack_ptr] = tmp_stack[tmpStack_ptr--];
                    break;
                case InstructionsType.popvar:
                    call_stack[varStack_ptr - parameters[code_ptr]] = tmp_stack[tmpStack_ptr--];
                    break;
                case InstructionsType.popadr:
                    break;
                case InstructionsType.pop:
                    tmpStack_ptr--;
                    break;
                case InstructionsType.add:
                    tmpStack_ptr -= 1;
                    tmp_stack[tmpStack_ptr] = tmp_stack[tmpStack_ptr] + tmp_stack[tmpStack_ptr + 1];
                    break;
                case InstructionsType.addv:
                    call_stack[varStack_ptr - parameters[code_ptr]] += tmp_stack[tmpStack_ptr--];
                    break;
                case InstructionsType.add1:
                    tmp_stack[tmpStack_ptr]++;
                    break;
                case InstructionsType.addv1:
                    call_stack[varStack_ptr - parameters[code_ptr]]++;
                    break;
                case InstructionsType.sub:
                    tmpStack_ptr -= 1;
                    tmp_stack[tmpStack_ptr] = tmp_stack[tmpStack_ptr] - tmp_stack[tmpStack_ptr + 1];
                    break;
                case InstructionsType.subv:
                    call_stack[varStack_ptr - parameters[code_ptr]] -= tmp_stack[tmpStack_ptr--];
                    break;
                case InstructionsType.sub1:
                    tmp_stack[tmpStack_ptr]--;
                    break;
                case InstructionsType.subv1:
                    call_stack[varStack_ptr - parameters[code_ptr]]--;
                    break;
                case InstructionsType.div:
                    tmpStack_ptr -= 1;
                    tmp_stack[tmpStack_ptr] = tmp_stack[tmpStack_ptr] / tmp_stack[tmpStack_ptr + 1];
                    break;
                case InstructionsType.divv:
                    call_stack[varStack_ptr - parameters[code_ptr]] /= tmp_stack[tmpStack_ptr--];
                    break;
                case InstructionsType.mul:
                    tmpStack_ptr -= 1;
                    tmp_stack[tmpStack_ptr] = tmp_stack[tmpStack_ptr] * tmp_stack[tmpStack_ptr + 1];
                    break;
                case InstructionsType.mulv:
                    call_stack[varStack_ptr - parameters[code_ptr]] *= tmp_stack[tmpStack_ptr--];
                    break;
                case InstructionsType.jmp:
                    code_ptr = parameters[code_ptr] - 1;
                    goto start;
                case InstructionsType.call:
                    tmp_stack[++tmpStack_ptr] = code_ptr + 1;
                    code_ptr = methods[parameters[code_ptr]] - 1;
                    goto start;
                case InstructionsType.ret:
                    code_ptr = tmp_stack[--tmpStack_ptr];
                    tmp_stack[tmpStack_ptr] = tmp_stack[tmpStack_ptr + 1];
                    goto start;
                case InstructionsType.eqt:
                    if (tmp_stack[tmpStack_ptr - 1] == tmp_stack[tmpStack_ptr])
                    {
                        code_ptr = parameters[code_ptr] - 1;
                        tmpStack_ptr -= 2;
                        goto start;
                    }
                    tmpStack_ptr -= 2;
                    break;
                case InstructionsType.eqf:
                    if (tmp_stack[tmpStack_ptr - 1] != tmp_stack[tmpStack_ptr])
                    {
                        code_ptr = parameters[code_ptr] - 1;
                        tmpStack_ptr -= 2;
                        goto start;
                    }
                    tmpStack_ptr -= 2;
                    break;
                case InstructionsType.let:
                    if (tmp_stack[tmpStack_ptr - 1] < tmp_stack[tmpStack_ptr])
                    {
                        code_ptr = parameters[code_ptr] - 1;
                        tmpStack_ptr -= 2;
                        goto start;
                    }
                    tmpStack_ptr -= 2;
                    break;
                case InstructionsType.lef:
                    if (tmp_stack[tmpStack_ptr - 1] >= tmp_stack[tmpStack_ptr])
                    {
                        code_ptr = parameters[code_ptr] - 1;
                        tmpStack_ptr -= 2;
                        goto start;
                    }
                    tmpStack_ptr -= 2;
                    break;
                case InstructionsType.mot:
                    if (tmp_stack[tmpStack_ptr - 1] > tmp_stack[tmpStack_ptr])
                    {
                        code_ptr = parameters[code_ptr] - 1;
                        tmpStack_ptr -= 2;
                        goto start;
                    }
                    tmpStack_ptr -= 2;
                    break;
                case InstructionsType.mof:
                    if (tmp_stack[tmpStack_ptr - 1] <= tmp_stack[tmpStack_ptr])
                    {
                        code_ptr = parameters[code_ptr] - 1;
                        tmpStack_ptr -= 2;
                        goto start;
                    }
                    tmpStack_ptr -= 2;
                    break;
                case InstructionsType.endP:
                    return;
                default:
                    OutPut.RunTimeError("未知指令");
                    return;
            }
            code_ptr++;
            goto start;
        }
    }
}
