using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
/* 
 Run process on multicore. Running parallel task on a single core will have an extra cost, 
 it cause a lot of context switching trying to execute the processes as parallel as possible 
 slowing down of performance. Avoid using the old Threading library (a lot of overhead!) and use this new MS approach.
 */
namespace TaskParallelLibraryOldWay
{
    class Program
    {
        static void Main(string[] args)
        {
            var t1 = new Task(() => DoSomeImportantWork(1, 1500)); 
            t1.Start(); 
            var t2 = new Task(() => DoSomeImportantWork(2, 3000)); 
            t2.Start(); 
            var t3 = new Task(() => DoSomeImportantWork(3, 1000)); 
            //1 second 
            t3.Start(); 
            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }
        private static void DoSomeImportantWork(int id, int sleepTime) { 
            Console.WriteLine("{0} is begining", id); 
            Thread.Sleep(sleepTime); 
            Console.WriteLine("{0} is Completed", id); 
        }
    }
}
