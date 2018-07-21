using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
    .NET Framework makes use of the Strategy Pattern as well, such as when calling List<T>.Sort()
 */
namespace StrategyPatternSorting
{
    class Program
    {
        static void Main(string[] args)
        {
            Person bill = new Person { FirstName = "Bill", LastName = "Gates" }; 
            Person steve = new Person { FirstName = "Steve", LastName = "Ballmer" }; 
            Person satya = new Person { FirstName = "Satya", LastName = "Nadella" }; 
            List<Person> ceos = new List<Person>(); 
            ceos.Add(bill); ceos.Add(steve); 
            ceos.Add(satya); 
            ceos.Sort(new FirstNameSorter()); 
            foreach (Person p in ceos) { 
                Console.WriteLine(String.Format("{0} {1}", p.FirstName, p.LastName)); 
            } 

            Console.WriteLine(); 
            ceos.Sort(new LastNameSorter()); 
            foreach (Person p in ceos) { 
                Console.WriteLine(String.Format("{0}, {1}", p.LastName, p.FirstName)); 
            } 
            Console.ReadKey();
        }
    }

    public class Person { public string FirstName { get; set; } public string LastName { get; set; } }
    public class FirstNameSorter : IComparer<Person> { public int Compare(Person x, Person y) { return x.FirstName.CompareTo(y.FirstName); } }
    public class LastNameSorter : IComparer<Person> { public int Compare(Person x, Person y) { return x.LastName.CompareTo(y.LastName); } }
}
