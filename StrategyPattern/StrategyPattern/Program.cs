using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
  Behavioral Patterns are about changing runtime behaviors of algorithms and classes.
  Strategy Pattern allows you to encapsulate a family of algorithms and use them interchangeably. 
 */
namespace StrategyPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Unit warrior = new Unit("Warrior", new WarriorBehavior()); 
            Unit defender = new Unit("Defender", new DefenderBehavior());
            Unit priest = new Unit("Priest", new PriestBehavior());
            List<Unit> units = new List<Unit>(); 
            units.Add(warrior); 
            units.Add(defender);
            units.Add(priest);
            foreach (Unit unit in units) { 
                unit.Render(); 
                Console.WriteLine(); 
            } 
            Console.WriteLine("Your troops are on an important mission!"); 
            Console.WriteLine("Press Escape to quit or any other key to continue."); 
            Console.WriteLine(); 
            Random rnd = new Random(); 
            while (Console.ReadKey().Key != ConsoleKey.Escape) { 
                foreach (Unit unit in units) { 
                    unit.Move(); 
                } 
                Console.WriteLine(); 
                // Returns 0, 1, or 2. 
                if (rnd.Next(3) == 0) { 
                    Console.WriteLine("The enemy attacks!"); 
                    foreach (Unit unit in units) { 
                        unit.ReactToOpponent(); 
                    } 
                } else { 
                    Console.WriteLine("Nothing happened..."); 
                } 
                Console.WriteLine(); 
            }

            
          

        }
    }
   

    public interface IUnitBehavior { 
        void ReactToOpponent(); 
    } 
    public class Unit { 
        private string name; 
        private IUnitBehavior behavior; 
        public Unit(string name, IUnitBehavior behavior) { 
            this.behavior = behavior;
            this.name = name; 
        } 
        public void Render(){ 
            Console.WriteLine(@"\o/ "); 
            Console.WriteLine(@" O "); 
            Console.WriteLine(@"/ \"); } 
        public void Move() { 
            Console.WriteLine(String.Format("{0} moves...", name)); 
        } 
        public void ReactToOpponent() { 
            behavior.ReactToOpponent(); 
        } 
    }
 

    public class WarriorBehavior : IUnitBehavior { 
        public void ReactToOpponent() { 
            Console.WriteLine("\"ATTACK!!!\""); 
        } 
    }
 

    public class DefenderBehavior : IUnitBehavior { 
        public void ReactToOpponent() { 
            Console.WriteLine("\"Hold the line!\""); 
        } 
    }

    public class PriestBehavior : IUnitBehavior
    {
        public void ReactToOpponent()
        {
            Console.WriteLine("\"Heal our troops!\"");
        }
    }

}
