using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using CreateDllDemo;

namespace LoadDllDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Show loadShow=new Show();
            loadShow.show();
            Console.ReadKey();
        }
    }
}
