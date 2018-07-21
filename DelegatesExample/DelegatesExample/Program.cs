using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelegatesExample
{
    //keyword 'delegate' + method/func return type + method/func argument type 
    delegate void MySimpleDelegate(); 
    delegate void MyDelegate(string Name);
    class Program
    {
        static void SayHello() { Console.WriteLine("Hey There!"); }
        static void SayHelloWithParams(string Name) { Console.WriteLine("Hey There, {0}!", Name); }
        static void Main(string[] args) { 
            //0 - Calling a func 
            SayHello();
            
            //1- traditional way of using delegate: a class which encapsulates a function 
            MySimpleDelegate simpleDel = new MySimpleDelegate(SayHello); 
            simpleDel.Invoke();

            //2-Sugar or shorthand syntax 
            MySimpleDelegate sugarSyntaxDel = SayHello; 
            sugarSyntaxDel();
            
            //3- Passing a function as delegate             
            Test(SayHello);

            //4- Sugar or shorthand syntax 
            MyDelegate sugarSyntaxDelWithParams = SayHelloWithParams; 
            sugarSyntaxDelWithParams("Kejeiri");

            //5- Delegate with params, passing a function with params 
            TestWithParams(SayHelloWithParams);
            
            //6- Using a function to create a delegate             
            MyDelegate DelWithParams = GiveMeMyDelegate(); 
            DelWithParams("Kejeiri: GiveMeMyDelegate");
            Console.ReadKey(); 
        }
        
        static void Test(MySimpleDelegate del) { del(); }
        static void TestWithParams(MyDelegate del) { del("Adam"); }
        static MyDelegate GiveMeMyDelegate() { return new MyDelegate(SayHelloWithParams); }
    }
}
