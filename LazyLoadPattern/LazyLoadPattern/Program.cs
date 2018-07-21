using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/*
 Definition: not getting data until you actually use it, .NET Framework has a Lazy<T> class for lazy loading any class:
 *  it comes in 4 flavors: 
 *      - lazy initialization : if SomeSingleton is not referenced anywhere in your code, then SomeSingleton will never be initialized(think about object takes a long time to load!)
 *      - virtual proxy
 *      - value holder
 *      - ghost
 */
namespace LazyLoadPattern
{
    class Program
    {
        static void Main(string[] args)
        {

            Stopwatch sw = new Stopwatch(); 
            sw.Start(); 

            Lazy<Something> lazy = new Lazy<Something>(); 
            
            sw.Stop(); 
            Console.WriteLine(String.Format("Initializing the Lazy<Something> cost: {0} ms.", sw.ElapsedMilliseconds)); 
            sw.Restart(); 
            lazy.Value.PrintUse(); 
            sw.Stop(); 
            Console.WriteLine(String.Format("Using the Lazy<Something>.Value cost: {0} ms.", sw.ElapsedMilliseconds));

            Console.WriteLine("\n\t==========Lambda===========");
            sw = new Stopwatch(); 
            Lazy<Something> lazyLambda = new Lazy<Something>(() => new Something(new object()));            
            sw.Restart();
            Console.WriteLine(String.Format("Initializing the Lazy<Something> cost: {0} ms.", sw.ElapsedMilliseconds));
            sw.Restart();
            lazyLambda.Value.PrintUse();
            sw.Stop();
            Console.WriteLine(String.Format("Using the Lazy<Something>.Value cost: {0} ms.", sw.ElapsedMilliseconds)); 

            Console.ReadKey();
        }
    }

    public class Something { 
        public Something() { 
            // Fake some heavy computation. 
            Thread.Sleep(5000);
        }

        public Something(object obj)
        {
            // Fake some heavy computation. 
            Thread.Sleep(5000);
        } 
        public void PrintUse() { 
            Console.WriteLine("Something is used."); 
        } 
    }


}
