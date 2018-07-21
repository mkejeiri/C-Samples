using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var Peoples = new List<Person>() { 
                new Person() {FirstName="John", LastName="Doe", Age = 25}, 
                new Person() {FirstName="Jane", LastName="Doe", Age = 27}, 
                new Person() {FirstName="John", LastName="Williams", Age = 30}, 
                new Person() {FirstName="Samantha", LastName="Williams", Age = 35}, 
                new Person() {FirstName="Bob", LastName="Walters", Age = 36} 
            };

            var result = from p in Peoples orderby p.LastName descending group p by p.LastName; 
            foreach (var res in result) { 
                Console.WriteLine("{0} - {1}", res.Key, res.Count()); 
                foreach (var p in res) { 
                    Console.WriteLine("\t\t {0} , {1}", p.FirstName, p.LastName); 
                } 
            } 

            Console.ReadKey();
        }
    }
    public class Person { 
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public int Age { get; set; } 
    }
}
