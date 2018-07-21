using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LambdaExpressionsExample
{
    delegate void OperationVoid(int num); 
    delegate int OperationFunc(int num); 
    class Program { 
        static void Main(string[] args) { 
            //0 - Delegate 
            OperationVoid op = Double; 
            op(2);

            //1 - Anonymous Method :allow us to skip to delcare "Double" Method and write the functionality inline... 
            //there is no limitation on how many line of code inside anonymous method/func, however if it grows the code will become unreadable 
            //keep 3 lines max!
            OperationVoid AnonymousMethod = delegate(int num){ 
                Console.WriteLine("{0} x 2 = {1}", num, num * 2); 
                Console.WriteLine("{0} x 3 = {1}", num, num * 3); 
            }; 
            AnonymousMethod(2);

            //2 - Anonymous function 
            OperationFunc opFunc = delegate(int num) { 
                return num * 2; 
            }; 
            int IntValue = opFunc(2); 
            Console.WriteLine("Anonymous function Result: {0}",IntValue);

            /* * Parenthesis are required in case of multiple params and the type could also be infered */ 
            //OperationVoid LAMDA = (int num) => { Console.WriteLine("LAMDA: {0} x 2 = {1}", num, num * 2); }; 
            OperationVoid LAMDA = num => { 
                Console.WriteLine("LAMDA: {0} x 2 = {1}", num, num * 2); 
            }; 
            LAMDA(2);

            /* * to skip the declaration of delegate we could also use generic delegates such as : * Action<> (doesn't return value) OR Func<> (retrun value) both are built-in delegate */
            Action<int> opAction = num => { 
                Console.WriteLine("Action: {0} x 2 = {1}", num, num * 2); 
            }; 
            opAction(2);
        
            Func<int, int> opFunction = num => { 
                return num * 2; 
            }; 
            Console.WriteLine("func: {0} ", opFunction(2)); 
            Console.ReadKey(); 
        }
        static void Double(int num) { 
            Console.WriteLine("{0} x 2 = {1}", num, num * 2); 
        } 
    }
}
