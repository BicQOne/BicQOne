using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pudge_Plus.Classes
{
    public static class Print
    {
        public static void Info(string text, params object[] arguments)
        {
            Encolored(text, ConsoleColor.White, arguments);
        }

        public static void Success(string text, params object[] arguments)
        {
            Encolored(text, ConsoleColor.Green, arguments);
        }
        
        public static void Error(string text, params object[] arguments)
        {
            Encolored(text, ConsoleColor.Red, arguments);
        }

        public static void Encolored(string text, ConsoleColor color, params object[] arguments)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text, arguments);
            Console.ForegroundColor = clr;
        }
    }
}
