using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationTokenTaskFactoryParallelLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            //most common way to create a cancellation token 
            //create a source which will be a reference throughout the code 
            //we have to do manually the checking of cancellation signal, 
            //there isn't an auto way. 
            var source = new CancellationTokenSource(); 
            try {
                var t1 = Task.Factory.StartNew(() => DoSomeImportantWork(1, 1500, source.Token)).ContinueWith((prevTask) => DoSomeMoreImportantWork(1, 500, source.Token)); 
                source.Cancel(); 
            } catch (Exception ex) { 
                Console.WriteLine(ex.GetType()); 
            } 
            Console.WriteLine("Press any key to quit");
            Console.ReadKey();
        }

        private static void DoSomeImportantWork(int id, int sleepTime, CancellationToken token) { 
            if (token.IsCancellationRequested) { 
                Console.WriteLine("DoSomeImportantWork: Cancelation requested"); 
                //we have to handle this exception in our code 
                token.ThrowIfCancellationRequested();
            } 
            Console.WriteLine("{0} is begining", id); 
            Thread.Sleep(sleepTime); 
            Console.WriteLine("{0} is Completed", id); 
        } 
        private static void DoSomeMoreImportantWork(int id, int sleepTime, CancellationToken token) { 
            if (token.IsCancellationRequested) { 
                Console.WriteLine("DoSomeMoreImportantWork: Cancelation requested"); 
                //we have to handle this exception in our code 
                token.ThrowIfCancellationRequested(); 
            } 
            Console.WriteLine("{0} is begining more work", id); 
            Thread.Sleep(sleepTime); 
            Console.WriteLine("{0} is Completed more work", id); 
        }
    }
}
