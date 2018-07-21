using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReflexionExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //FirstRun();
            SecondRun();
            Console.ReadKey();
        }

      

        private static void FirstRun()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine(assembly.FullName);
            var types = assembly.GetTypes();
            foreach (var t in types)
            {
                Console.WriteLine("Type: " + t.Name + " - BaseType: " + t.BaseType);
                var props = t.GetProperties();
                foreach (var p in props)
                {
                    Console.WriteLine("\tProperty: " + p.Name + " - PropertyType: " + p.PropertyType);
                }
                var methods = t.GetMethods();
                foreach (var m in methods)
                {
                    Console.WriteLine("\tMethod: " + m.Name + " - ReturnType: " + m.ReturnType);
                }
                var fields = t.GetFields();
                foreach (var f in fields)
                {
                    Console.WriteLine("\tField: " + f.Name + " - FieldType: " + f.FieldType);
                }
            }
            /* * Manipulation of Reflexion * OUTPUT-> Property: John */
            var sample = new Sample { Name = "John", Age = 25 }; //'typeof(Sample)' is a compile time operation whereas 'sample.GetType()' 
            //a runtime operation we don't know for sure which //type we will get!!! 
            var sampleType = typeof(Sample);
            // sample.GetType(); 
            var nameProperty = sampleType.GetProperty("Name");
            Console.WriteLine("\n\t===========Properties value================");
            foreach (var p in sampleType.GetProperties())
            {
                Console.WriteLine("Property: " + p.Name + " - Value: " + p.GetValue(sample));
            }

            Console.WriteLine("Property: " + nameProperty.GetValue(sample));
            var myMethod = sampleType.GetMethod("MyMethod");
            //we need to specify the object which we run the method on ('sample') and //the params if any myMethod.Invoke(sample, null);
        }
        private static void SecondRun()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => t.GetCustomAttributes<MyClassAttribute>().Count() > 0);
            foreach (var type in types)
            {
                Console.WriteLine(type.Name);
                var methodTypes = type.GetMethods().Where(t => t.GetCustomAttributes<MyMethodAttribute>().Count() > 0);
                foreach (var m in methodTypes)
                {
                    Console.WriteLine(m.Name);
                }
            }
        }
    }
    //public class Sample
    //{ /* Sugar Syntax : C# creates behind the scene!!! Method: get_Name - System.String Method: set_Name - System.Void */
    //    public string Name { get; set; } public int Age;
    //    public void MyMethod() { Console.WriteLine("Hello From MyMethod!"); }
    //}

    [MyClass] 
    public class Sample { /* Sugar Syntax : C# creates behind the scene!!! Method: get_Name - System.String Method: set_Name - System.Void */
        public string Name { get; set; } 
        public int Age; 
        [MyMethod] 
        public void MyMethod(){ 
            Console.WriteLine("Hello From MyMethod!"); 
        } 
        public void NoAttributeMethod() { } 
    } 
    [AttributeUsage(AttributeTargets.Class)] 
    public class MyClassAttribute : Attribute { } 
    [AttributeUsage(AttributeTargets.Method)] 
    public class MyMethodAttribute : Attribute { }

}
