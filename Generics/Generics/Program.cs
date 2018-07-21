using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generics
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = new Result<String, int> { success = true, data = "String Data 1", extraData = 10 }; 
            //Console.WriteLine(result.success); 
            //Console.WriteLine(result.data); 
            //Console.ReadKey(); Helper.print(result); 
            var result2 = new Result<int,string> { success = true, data = 10, extraData="String Data 2" }; 
            //Console.WriteLine(result2.success); 
            //Console.WriteLine(result2.data); 
            Helper.print(result2);
            Console.ReadKey();
        }
    }
    public class Helper
    {
        public static void print<T, U>(Result<T, U> result){ 
            Console.WriteLine(result.success); 
            Console.WriteLine(result.data); 
            Console.WriteLine(result.extraData); 
        }
    }
    public class Result<T, U> { 
        public bool success { get; set; } 
        public T data { get; set; } 
        public U extraData { get; set; } }
}
