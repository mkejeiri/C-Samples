//https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/
//Anders Hejlsberg: Coavariance allowing you to do things in your code that previously 
//you where suprised you could not do 
using System;
using System.Collections.Generic;
using System.IO;
namespace CovarianceContravariance {
    public class Animal {
        public void Eat () => System.Console.WriteLine ("Eat");
    }

    public class Bird : Animal {
        public void Fly () => System.Console.WriteLine ("Fly");
    }

    public class Humain : Animal { }

    interface IProcess<out T> { //out for covariance
        T Process ();
    }

    public class AnimalProcessor<T> : IProcess<T> {
        public T Process () {
            throw new NotImplementedException ();
        }
    }

    interface IZoo<in T> //in for contravariance
    {
        void add(T t);
    }

    public class Zoo<T> : IZoo<T>
    {
        public void add(T t) => throw new NotImplementedException();
    }

    delegate Animal ReturnAnimalDelegate ();
    delegate Bird ReturnBirdDelegate ();

    delegate void TakeAnimalDelegate (Animal a);
    delegate void TakeBirdDelegate (Bird a);

    public class Program {
        public static Animal GetAnimal () => new Animal ();
        public static Bird GetBird () => new Bird ();

        public static void Eat (Animal a) => a.Eat ();
        public static void Fly (Bird b) => b.Fly ();

        public static void Main (string[] args) {
            /******************************************************************
            - Covariance :preserve assignement compatibility between parent and child (IS relation) 
                          relationship during dynamic polymorphism
                          example : Animal a = new Dog(); 
                          and also group of animals -> IEnumerable<Animal> an = new List<Dog>(); //wasn't possible before 4.0
            - Contravariance: reverses assignement compatibility
            - it deals with 3 types : delegate, array and generics type arguments
            *******************************************************************/

            /* 
                1- D E L E G A T E S
                return type is covariant and the parameters are contravariant
            */

            Animal a = new Bird ();

            //a- return type example
            ReturnAnimalDelegate da= GetAnimal;
            ReturnAnimalDelegate db = GetBird;
            a = db ();

            //The other way (reverse asignement) around doesn't work
            ReturnBirdDelegate dbb = GetAnimal;  //Failed otherwise  dbb().Fly() should work, 
            
            //failed the same way as 
            Bird b = new Animal();

            //b- the parameters example
            TakeBirdDelegate tb = Fly;
            TakeAnimalDelegate ta = Eat;
            tb = Eat;
            
            //In delegate reverse asignement is contravariant in argument type (success in passing the Bird instead of animal)
            ta (new Bird ());
            
            //(reverse asignement) failed
            tb(new Animal()); ta = Fly;

            /**************************************S U M M A R Y *********************************************
             for delegates the return type is covariant and the parameters are contravariant, 
             the delegates are the only construct in C# that supports Covariance & Contravariance  
             *********************************************************************************************/

            /* 
                2- A R R A Y S
                Array with refrence type are also covariants
            */
            //assignment is preserved
            Animal[] animals = new Bird[10];
            animals[0] = new Humain (); //compilers doesn't complain but runtime Exception (ArrayTypeMismatchException)


            /* 
                3- G E N E R I C S
                 
            */
            IProcess<Animal> animalProcessor = new AnimalProcessor<Animal> ();
            IProcess<Bird> birdProcessor = new AnimalProcessor<Bird> ();

            //covariance makes sense, since birdProcessor.Process() return bird which is an animal
            Animal animal = birdProcessor.Process ();

            //Cannot implicitly convert type 'IProcess<Bird>' to 'IProcess<Animal>'. An explicit conversion exists (are you missing a cast?
            //even the compiler gives an error, since .net 4.0 we could make it covariant by adding an out keyword to the interface           
            animalProcessor = birdProcessor;

            //public interface IEnumerable<out T> : IEnumerable
            IEnumerable<Animal> animalList = new List<Bird> ();

            //generic contravariance
            IZoo<Animal> animalZoo = new Zoo<Animal>();
            animalZoo.add(new Animal()); 
            
            //we could also a
            nimalZoo.add(new Bird());
            
            IZoo<Bird> birdZoo = new Zoo<Bird>();
            birdZoo.add(new Bird());

            //generic contravariance
            birdZoo = animalZoo;
            birdZoo = new Zoo<Animal>();
            
            //doesn't works 
            birdZoo = new Zoo<Humain>();
            /*****************************************************************************
            public interface IComparer<in T> : 
            makes senses if we want to compare not only birds by any abstract 
            animal with birds            
            ******************************************************************************/
            //Also the same goes for generics delegates func and action

        }
    }
}
