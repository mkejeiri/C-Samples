using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowVsOverride
{
    class Program
    {

       static void Main(string[] args)

        {
            firstClass first = new firstClass();
            firstClass second = new secondClass();
            firstClass third = new thirdClass();

            //Shadowing
            first.displayShadowing();
            second.displayShadowing();
            third.displayShadowing();


            //Overriding
            first.displayOverriding();
            second.displayOverriding();
            third.displayOverriding();


            SomeClass someClass = new SomeClass();
            someClass.methodA();
            Console.ReadKey(); 
        }
    }


    public class firstClass
    {
        public void displayShadowing()
        {
            Console.WriteLine("This is firstClass : Shadowing");
        }

        public virtual void displayOverriding() {

            Console.WriteLine("This is firstClass : Overriding");        
        }
    }

    public class secondClass : firstClass
    {
        private new void displayShadowing()
        {
         
            Console.WriteLine("This is secondClass : Shadowing");
        }

        public override void displayOverriding()
        {
            Console.WriteLine("This is secondClass : Overriding");
        }
    }

    public class thirdClass : secondClass
    {
        public new void displayShadowing()
        {
            Console.WriteLine("This is thirdClass : Shadowing");
        }

        public override void displayOverriding()
        {
            Console.WriteLine("This is thirdClass : Overriding");
        }
    }

    public interface Ido {
        String myStringProp { get; set; }
        void DoSth();
        void DoSthElse();
    }

    public interface IInterface2
    {
        void DoSth2();
        void DoSthElse2();
    }

    public interface IInterface3
    {
        void DoSth3();
        void DoSthElse3();
    }


    public interface IInterface : Ido, IInterface2, IInterface3{
        void testIInterface();


    }

    public class Do : thirdClass, IInterface
    {
        public String myStringProp {get; set;}
        public void DoSth()
        {
            Console.WriteLine("DoSth");
        }

        public void DoSthElse()
        {
            Console.WriteLine("DoSthElse");
        }
        public void DoSth2()
        {
            Console.WriteLine("DoSth2");
        }

        public void DoSthElse2()
        {
            Console.WriteLine("DoSthElse2");
        }

        public void DoSth3()
        {
            Console.WriteLine("DoSth3");
        }

        public void DoSthElse3()
        {
            Console.WriteLine("DoSthElse3");
        }


        public void testIInterface(){
            Console.WriteLine("testIInterface");        
        }


        public override void displayOverriding()
        {
            Console.WriteLine("This is Do : Overriding");
        }


        public new void displayShadowing()
        {
            Console.WriteLine("This is thirdClass : Shadowing");
        }
    }


    interface ISomeInterface
    {
        void methodA();
        void methodB();
    }

    public class SomeClass: ISomeInterface
    {
        public void methodA() {
            ISomeInterface me = (ISomeInterface)this;
            Console.WriteLine("Calling methodB from inside methodA");
            me.methodB();        
        }

        void ISomeInterface.methodB(){
            Console.WriteLine("methodB");   
        }
    }

}


