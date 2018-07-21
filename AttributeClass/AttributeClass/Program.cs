using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AttributeClass
{
  class Program { 
      static void Main(string[] args) { 
          var types = from t in Assembly.GetExecutingAssembly().GetTypes() 
                      where t.GetCustomAttributes<SampleAttribute>().Count() > 0 //get the class where we applies the SampleAttribute decorators 
                      select t;
        foreach (var t in types){ 
            Console.WriteLine(t); 
            foreach (var p in t.GetProperties()) { Console.WriteLine(p); }
            foreach (var m in t.GetMethods()) { Console.WriteLine(m); } 
        }
       

        Console.ReadKey(); 
      }
  }
    //[AttributeUsage(AttributeTargets.Class|AttributeTargets.Property|AttributeTargets.Method)] [AttributeUsage(AttributeTargets.Class)] 
    public class SampleAttribute : Attribute { 
        public string Name { get; set; }
        public int Version { get; set; }
    }

    [Sample(Name = "John", Version = 1)] //<- sugar syntax! or [SampleAttribute(Name ="John", Version=1)] 
    public class Test { public int IntValue { get; set; } public void Method() {} 
    }
    public class NoAttribute { }
}
