using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    An event is a way for an object to subscribe to an event happening in other object and do some logic around that.
 */
namespace EventsExample
{
    /*public*/internal delegate void ChimeEventHandler(object sender, ClockTowerEventArgs e); 
    class Program { 
        static void Main(string[] args) { 
            var tower = new ClockTower(); 
            var john = new Person("John", tower); 

            tower.Chime +=(s, e)=> { 
                Console.WriteLine("{0} : heard the clock chime.", john.GetName()); 
                switch (e.message) { 
                    case 6: 
                        Console.WriteLine("{0} is wakking up!", john.GetName()); 
                        break; 

                    case 17: 
                        Console.WriteLine("{0} is going home!", john.GetName()); 
                        break; 
                } 
            }; 
            tower.ChimeAtSixAm(); 
            tower.ChimeAtFivePm(); 
            Console.ReadKey(); 
        }
    }

    public class ClockTower {
        /*public*/internal event ChimeEventHandler Chime;
        public void ChimeAtSixAm() { 
            var chimeMessage = new ClockTowerEventArgs(6); 
            Chime(this, chimeMessage); 
        }
        public void ChimeAtFivePm() { 
            var chimeMessage = new ClockTowerEventArgs(17); 
            Chime(this, chimeMessage); 
        } 
    } 

    public class Person { 
        string _name; 
        ClockTower _tower;

        public Person(string Name, ClockTower Tower) { 
            this._name = Name; 
            this._tower = Tower; 
        }

        public string GetName() { 
            return this._name; 
        }
    }

    public class ClockTowerEventArgs : EventArgs {
        public int message { get; set; } 
        public ClockTowerEventArgs(int message){ 
            this.message = message; 
        } 
    }
}