using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//dynamic allow us to bypass the compiler and everything is checked at runtime.
namespace DynamicAndLateBindingExample
{
    class Program
    {
        static void Main(string[] args)
        {
            runPythonFunction();
            runExpandoObject();
            OptionalParamsRun();
            Console.ReadKey();

        }

        private static void OptionalParamsRun()
        {
            //Optional parameters must appear after all required parameters 
            //Optional parameters need to be specified in order of the method signature : 
            //Argument 2: cannot convert from 'int' to 'string' 
            PrintData("John"); 
            PrintData("Sally", "Williams",35);
            //use this if you don't mind the order 
            PrintData(age:35, lastName:"Doe", firstName:"John"); 
            PrintData("John", age:35);
        }
        private static void PrintData(string firstName, string lastName = null, int age = 0) { 
            Console.WriteLine("{0} {1} is {2} year old.", firstName, lastName, age); 
        }

        private static void runExpandoObject()
        {
            dynamic test = new ExpandoObject(); 
            test.Name = "John"; 
            test.Age = 25; 
            Console.WriteLine("Name {0} and Age {1}", test.Name, test.Age);    
        }

        private static void runPythonFunction()
        {
            //var pythonRuntime = PythonRuntime.CreateRuntime(); 
            //dynamic pythonFile = pythonRuntime.UseFile("file.py"); 
            //pythonFile.SayHelloToPython();

            /*
             //file.py 
             import sys; 
                def SayHelloToPython(): 
                print "Hello there C#" print "Nice to finally chat"
             */
        }
    }
}
