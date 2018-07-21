using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegatesOperationsExample
{
    delegate void Operation(int num);
    class Program
    {
        static void Main(string[] args)
        {
            Operation op = Double; 
            executeOperation(2, op); //Or Just op(2); 
            op = Triple; 
            executeOperation(2, op); //Or Just op(2);
            //Chaining!!! 
            Console.WriteLine("---- Chaining --- "); 
            Operation opChain; 
            opChain = Double; 
            opChain = opChain + Triple; //OR ... 
            opChain += Triple; 
            opChain += Double; 
            opChain -= Triple; 
            opChain -= Double; 
            executeOperation(2, opChain); //Or Just op(2);
            Console.ReadKey();
        }
        static void Double(int num) { 
            Console.WriteLine("{0} x 2 = {1}", num, num * 2); 
        } 
        static void Triple(int num) { 
            Console.WriteLine("{0} x 3 = {1}", num, num * 3); 
        } 
        static void executeOperation(int num, Operation op) { 
            op(num); 
        }
    }
}
