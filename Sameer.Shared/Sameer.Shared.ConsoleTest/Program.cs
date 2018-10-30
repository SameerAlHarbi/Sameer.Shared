using System;
using Sameer.Shared.Helpers;

namespace Sameer.Shared.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt = DateTime.Today;
            Console.WriteLine(dt.ConvertToString(true,true,false));
            Console.ReadKey();
        }
    }
}
