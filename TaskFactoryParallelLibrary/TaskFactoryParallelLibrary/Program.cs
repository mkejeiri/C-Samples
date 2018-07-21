using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


/*
* 00- Simple example using Task.Factory : see SimpleTaskFactoryRun()
* 01- Advanced example: see TaskFactoryWaitTaskToCompleteRun()
* 02- More Advanced example 'continueWith' : see TaskFactoryContinueWithRun()
* 03- Parallel.ForEach and Parallel.For : see ParallelForEachTaskFactoryRun()
*/
namespace TaskFactoryParallelLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            //0- simple example
            SimpleTaskFactoryRun();
            
            /*************************************************************************************************************************************************
             * 01- Advanced example
                Instead of waiting when a task is completed and start a new one MS has introduced continueWith method. 
                We could chain as much as required on single task/thread using this method.
            *************************************************************************************************************************************************/
            TaskFactoryContinueWithRun();

            /*************************************************************************************************************************************************
             * 02- More Advanced example
                 We could press any key and interrupt the program at any time, but if we want to wait of any specific or 
                 all tasks to be completed we need to do this. WaitAll will block the execution until the specified lists of tasks are completed.             
             *************************************************************************************************************************************************/
            TaskFactoryWaitTaskToCompleteRun();

            /*************************************************************************************************************************************************
             * 03-Parallel.ForEach and Parallel.For
             Parallel.ForEach and Parallel.For are blocking operations, inside them everything happens in parallel and without sequential order.
             *************************************************************************************************************************************************/
            ParallelForEachTaskFactoryRun();

            Console.WriteLine("Press any key to quit"); 
            Console.ReadKey();
        }

        private static void ParallelForEachTaskFactoryRun()
        {
            var intList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 25, 26, 27, 28, 29, 30 }; 
            Parallel.ForEach(intList, (i) => Console.WriteLine("{0}", i)); 
            Parallel.For(1, 100, (i) => Console.WriteLine("iteration : {0}", i)); 
            //execution will reach this point when the two parallel loops are completed
        }

        private static void TaskFactoryWaitTaskToCompleteRun()
        {
            var t1 = Task.Factory.StartNew(() => DoSomeImportantWork(1, 1500)).ContinueWith((prevTask) => DoSomeMoreImportantWork(1, 3000)); 
            var t2 = Task.Factory.StartNew(() => DoSomeImportantWork(2, 3000)).ContinueWith((prevTask) => DoSomeMoreImportantWork(2, 2000)); 
            var t3 = Task.Factory.StartNew(() => DoSomeImportantWork(3, 1000)).ContinueWith((prevTask) => DoSomeMoreImportantWork(3, 500));
            //meanwhile doing some other logic while waiting for all task to finish 
            for (int i = 0; i < 10; i++) { 
                Console.WriteLine("Doing some other work"); 
                Thread.Sleep(250); 
                Console.WriteLine("iteration : {0}", i); 
            } 
            var taskList = new List<Task> { t1, t2, t3 }; 
            Task.WaitAll(taskList.ToArray());
        }

        
        private static void TaskFactoryContinueWithRun()
        {
            var t1 = Task.Factory.StartNew(() => DoSomeImportantWork(1, 1500)).ContinueWith((prevTask) => DoSomeMoreImportantWork(1, 3000));
            var t2 = Task.Factory.StartNew(() => DoSomeImportantWork(2, 3000)).ContinueWith((prevTask) => DoSomeMoreImportantWork(2, 2000)); 
            var t3 = Task.Factory.StartNew(() => DoSomeImportantWork(3, 1000)).ContinueWith((prevTask) => DoSomeMoreImportantWork(3, 500));
        }

        private static void SimpleTaskFactoryRun()
        {
            var t1 = Task.Factory.StartNew(() => DoSomeImportantWork(1, 1500));
            var t2 = Task.Factory.StartNew(() => DoSomeImportantWork(2, 3000));
            var t3 = Task.Factory.StartNew(() => DoSomeImportantWork(3, 1000));
        }
        private static void DoSomeImportantWork(int id, int sleepTime) { 
            Console.WriteLine("{0} is begining", id); 
            Thread.Sleep(sleepTime); 
            Console.WriteLine("{0} is Completed", id); 
        }
        private static void DoSomeMoreImportantWork(int id, int sleepTime) { 
            Console.WriteLine("{0} is begining more work", id); 
            Thread.Sleep(sleepTime); 
            Console.WriteLine("{0} is Completed more work", id); 
        }
    }
}
