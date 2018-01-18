using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compieler
{
    public enum Command
    {
        IFETCH, ISTORE, IPUSH, IPOP, IADD, ISUB, ILT, JZ, JNZ, JMP, HALT
    }
    public class VirtualMachine
    {
        public void Run(List<object> program)
        {
            int[] vars = new int[26];
            var stack = new Stack<object>();
            var pc = 0;
            while (true)
            {
                dynamic arg = null;
                var op = program[pc];
                if (pc < program.Count - 1)
                {
                    arg = program[pc + 1];
                }
                if ((Command) op == Command.IFETCH)
                {
                    stack.Push(vars[Convert.ToInt32(arg)]);
                    pc += 2;
                }
                else if ((Command) op == Command.ISTORE)
                {
                    int value = int.Parse(stack.Pop().ToString());
                    vars[Convert.ToInt32(arg)] = value;
                    pc += 2;
                }
                else if ((Command) op == Command.IPUSH)
                {
                    stack.Push(Convert.ToInt32(((char)Convert.ToInt32(arg)).ToString()));
                    pc += 2;
                }
                else if ((Command) op == Command.IPOP)
                {
                    stack.Push(arg);
                    stack.Pop();
                    pc += 1;
                }
                else if ((Command) op == Command.IADD)
                {
                    var a = int.Parse(stack.Pop().ToString());
                    var b = int.Parse(stack.Pop().ToString());
                    stack.Push(a + b);
                    pc += 1;
                }
                else if ((Command) op == Command.ISUB)
                {
                    var a = int.Parse(stack.Pop().ToString());
                    var b = int.Parse(stack.Pop().ToString());
                    stack.Push(b - a);
                    pc += 1;
                }
                else if ((Command) op == Command.ILT)
                {
                    var a = int.Parse(stack.Pop().ToString());
                    var b = int.Parse(stack.Pop().ToString());
                    stack.Push(b < a ? 1 : 0);
                    pc += 1;
                }
                else if ((Command) op == Command.JZ)
                {
                    if (stack.Pop() == (object) 0)
                        pc = arg;
                    else pc += 2;
                }
                else if ((Command) op == Command.JNZ)
                {
                    if (stack.Pop() != (object) 0)
                        pc = arg;
                    else pc += 2;
                }
                else if ((Command) op == Command.JMP)
                {
                    pc = arg;
                }

                else if ((Command) op == Command.HALT)
                {
                    break;
                }
            }

            Console.WriteLine("Execution finished");
            for (var i = 0; i < 26; i++)
            {
                if (vars[i] != 0)
                {
                    Console.WriteLine($"{(char) (i + 'a')} = {vars[i]}");

                }

            }
            Console.ReadKey();
        }
    }
}
