using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFormatBumber
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = 23456789.409233;
            var y = 23456789;
            var z = 0.409233;
            Console.WriteLine("{0:N2} {1:N2} {2:N2}", x, y, z);
            Console.WriteLine("{0:0.00} {1:0.00} {2:0.00}", x, y, z);
            Console.WriteLine("{0:#0.00} {1:#0.00} {2:#0.00}", x, y, z);
            Console.ReadKey(false);
        }
    }
}
