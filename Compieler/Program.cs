using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Compieler
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length == 0)
            //{
            //    Console.WriteLine("No Code");
            //    return;
            //}
            //string path = args[0];

            //// This text is always added, making the file longer over time
            //// if it is not deleted.
            //string code = File.ReadAllText(path);
            //code = Regex.Replace(code, @"\t|\n|\r", "");
            //Console.WriteLine(code);
            //// Open the file to read from.
            //string readText = File.ReadAllText(path);
            ////Console.ReadLine();
            var code = Console.ReadLine();
            var example = code;
            var lex = new Lexer(new StringReader(example));
            var par = new Parser(lex);
            var ast = par.Parse();
            var c = new Compiler();
            var program = c.Compile(ast);
            var vm = new VirtualMachine();
            vm.Run(program);
        }
    }
}
