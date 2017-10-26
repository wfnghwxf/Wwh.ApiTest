using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wwh.ConsoleApp
{
    class A
    {
        public string Str;
        public A()
        {
            Console.WriteLine("A");
        }
    }

    class B : A
    {
        public B()
        {
            Console.WriteLine("B");
        }
    }

}
