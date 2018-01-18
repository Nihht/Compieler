using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compieler
{
    public class Compiler
    {
        public List<object> ProgramStack = new List<object>();
        public int PC = 0;

        public void Generate(object command)
        {
            ProgramStack.Add(command);
            PC = PC + 1;
        }

        public List<object> Compile(Node node)
        {
            switch (node.Kind)
            {
                case Parser.Kind.Var:
                    Generate(Command.IFETCH);
                    Generate(node.Value);
                    break;
                case Parser.Kind.Const:
                    Generate(Command.IPUSH);
                    Generate(node.Value);
                    break;
                case Parser.Kind.Add:
                    Compile(node.Op1);
                    Compile(node.Op2);
                    Generate(Command.IADD);
                    break;
                case Parser.Kind.Sub:
                    Compile(node.Op1);
                    Compile(node.Op2);
                    Generate(Command.ISUB);
                    break;
                case Parser.Kind.Lt:
                    Compile(node.Op1);
                    Compile(node.Op2);
                    Generate(Command.ILT);
                    break;
                case Parser.Kind.Set:
                    Compile(node.Op2);
                    Generate(Command.ISTORE);
                    Generate(node.Op1.Value);
                    break;
                case Parser.Kind.If1:
                {
                    Compile(node.Op1);
                    Generate(Command.JZ);
                    var addr = PC;
                    Generate(0);
                    Compile(node.Op2);
                    ProgramStack[addr] = PC;
                    break;
                }
                case Parser.Kind.If2:
                {
                    Compile(node.Op1);
                    Generate(Command.JZ);
                    var addr1 = PC;
                    Generate(0);
                    Compile(node.Op2);
                    Generate(Command.JMP);
                    var addr2 = PC;
                    Generate(0);
                    ProgramStack[addr1] = PC;
                    Compile(node.Op3);
                    ProgramStack[addr2] = PC;
                    break;
                }
                case Parser.Kind.While:
                {
                    var addr1 = PC;
                    Compile(node.Op1);
                    Generate(Command.JZ);
                    var addr2 = PC;
                    Generate(0);
                    Compile(node.Op2);
                    Generate(Command.JMP);
                    Generate(addr1);
                    ProgramStack[addr2] = PC;
                    break;
                }
                case Parser.Kind.Do:
                {
                    var addr = PC;
                    Compile(node.Op1);
                    Compile(node.Op2);
                    Generate(Command.JNZ);
                    Generate(addr);
                    break;
                }
                case Parser.Kind.Seq:
                    Compile(node.Op1);
                    Compile(node.Op2);
                    break;
                case Parser.Kind.Expr:
                    Compile(node.Op1);
                    Generate(Command.IPOP);
                    break;
                case Parser.Kind.Prog:
                    Compile(node.Op1);
                    Generate(Command.HALT);
                    break;
            }

            return ProgramStack;
        }
    }
}
