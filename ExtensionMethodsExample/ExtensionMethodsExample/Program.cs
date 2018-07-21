using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* 
     Use extension if you cannot add methods/function to the class in the usual way, more often the native classes... 
     Step to follow to add extension: 
        1- Create a public static class such as Extensions  
        2- Create a public static Method or function, first param should be preceded by this keyword and type of class 
           you should operates on (e.g. : public static void SayHello(this Person person)) 
 */
namespace ExtensionMethodsExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var john = new Person() { Name = "John", Age = 32 }; 
            var Sally = new Person() { Name = "Sally", Age = 33 }; 
            john.SayHello(); 
            Sally.SayHello(); 
            john.SayHelloTo(Sally); 
            Sally.SayHelloTo(john); 
            Console.ReadKey();
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    public static class Extensions { 
        public static void SayHello(this Person person) { 
            Console.WriteLine("{0} says hello", person.Name); 
        } 
        public static void SayHelloTo(this Person person, Person personTo) 
        { 
            Console.WriteLine("{0} says hello to {1}", person.Name, personTo.Name); 
        } 
    }   
}