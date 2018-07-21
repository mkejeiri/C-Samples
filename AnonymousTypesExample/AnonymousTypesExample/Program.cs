using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymousTypesExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var Peoples = new List<Person> { 
                new Person(){FirstName="John", LastName="Doe", Age=25}, 
                new Person(){FirstName="Jane", LastName="Doe", Age=29}, 
                new Person(){FirstName="Bob", LastName="Williams", Age=19}, 
                new Person(){FirstName="Walker", LastName="Williams", Age=35}, 
                new Person(){FirstName="Jenny", LastName="Doe", Age=40}, 
                new Person(){FirstName="Harry", LastName="Harper", Age=38}, 
            }; 
            var result = from p in Peoples where p.LastName == "Doe" select p; 
            foreach (var res in result) { 
                Console.WriteLine("{0} - {1}", res.LastName, res.FirstName); 
            }
            
            Console.WriteLine("\n--------- Anonymous ---------\n"); 
            var resultAnonymous = from p in Peoples where p.LastName == "Doe" select new {LName= p.LastName, FName= p.FirstName};
            foreach (var res in resultAnonymous) { 
                Console.WriteLine("{0} - {1}", res.LName, res.FName); 
                //Error: Property or indexer 'AnonymousType#1.LName' 
                //cannot be assigned to -- it is read only 
                //res.LName = "NotPossible"; //Immutable
            }
            
            Console.ReadKey();
        }
    }
    public class Person
    {
        public string FirstName { get; set; } public string LastName { get; set; } public int Age { get; set; }
        public string MyProperty1 { get; set; } public int MyProperty2 { get; set; } public int MyProperty3 { get; set; }
    }
}