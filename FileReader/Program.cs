using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileReader
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(@"C:\Users\Frank\Desktop\current.log", Encoding.Default);
            String[] line = sr.ReadLine().Split('\t');
            for(int i=0;i<line.Length;i++)
            {
                Console.WriteLine(line[i]);
            }
            Console.WriteLine(line.Length);
            Console.ReadKey();
        }
    }
}
